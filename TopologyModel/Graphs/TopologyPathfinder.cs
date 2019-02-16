using QuickGraph.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using TopologyModel.Enumerations;
using TopologyModel.Equipments;

namespace TopologyModel.Graphs
{
    /// <summary>
    /// Класс для поиска путей для секций между вершинами графа.
    /// </summary>
    public static class TopologyPathfinder
    {
        /// <summary>
        /// Кэш уже найденных путей в графе.
        /// </summary>
        private static List<TopologyPathesCacheItem> PathesCache = new List<TopologyPathesCacheItem>();

        /// <summary>
        /// Найти кратчайший путь из источника в цель в кеше или на графе.
        /// </summary>
        /// <param name="source">Вершина графа - источник пути.</param>
        /// <param name="target">Вершина пути - цель пути.</param>
        /// <param name="dataChannel">КПД, по которому строится путь.</param>
        /// <returns>Найденный заново или взятый из кэша путь.</returns>
        public static IEnumerable<TopologyEdge> GetPath(TopologyGraph graph, TopologyVertex source, TopologyVertex target, DataChannel dataChannel)
        {
            try
            {
                var cachedPath = PathesCache.SingleOrDefault(q =>       // Пытаемся найти такой прямой или обратный путь в кэше
                    q.ConnectionType == dataChannel.ConnectionType &&
                    ((q.Source == source && q.Target == target) || (q.Target == source && q.Source == target)));

                if (cachedPath != null)
                    return cachedPath.Path;

                // Если не нашли, ищем путь в графе
                var tryGetPath = graph.ShortestPathsDijkstra((edge) => { return edge.Weights[dataChannel.ConnectionType]; }, source);

                tryGetPath(target, out var path);

                // Сохраняем найденный путь в кэше и возвращаем
                PathesCache.Add(new TopologyPathesCacheItem(source, target, dataChannel.ConnectionType, path));

                return path;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetPath failed! {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Найти кратчайший путь в секции от вершины источника - УСПД, через все целевые вершины КУ.
        /// </summary>
        /// <param name="graph">Граф, в котором ищем путь.</param>
        /// <param name="source">Вершина графа с УСПД.</param>
        /// <param name="targets">Вершины КУ, которые нужно соединить кратчайшим путём.</param>
        /// <param name="dataChannel">Канал передачи данных, по которому соеденяются КУ и УСПД.</param>
        /// <returns>Перечисление путей в графе.</returns>
        public static IEnumerable<TopologyPath> SectionShortestPath(TopologyGraph graph, TopologyVertex source, IEnumerable<TopologyVertex> targets, DataChannel dataChannel)
        {
            try
            {
                switch (dataChannel.Topology)
                {
                    case DataChannelTopology.Mesh:
                        return MeshShortestPath(graph, source, targets, dataChannel);

                    case DataChannelTopology.Star:
                        return StarShortestPath(graph, source, targets, dataChannel);

                    case DataChannelTopology.Bus:
                        return BusShortestPath(graph, source, targets, dataChannel);

                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SectionShortestPath failed! {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Найти кратчайший путь по смешанной топологии.
        /// </summary>
        /// <param name="graph">Граф, в котором ищем путь.</param>
        /// <param name="source">Вершина графа с УСПД.</param>
        /// <param name="targets">Вершины КУ, которые нужно соединить кратчайшим путём.</param>
        /// <param name="dataChannel">Канал передачи данных, по которому соеденяются КУ и УСПД.</param>
        /// <returns>Перечисление путей в графе.</returns>
        private static IEnumerable<TopologyPath> MeshShortestPath(TopologyGraph graph, TopologyVertex source, IEnumerable<TopologyVertex> targets, DataChannel dataChannel)
        {
            try
            {
                // Кандидаты составных частей сети, где ключ - пара источника и целевой вершины, а значение - путь между ними
                var meshPartCandidates = new Dictionary<KeyValuePair<TopologyVertex, TopologyVertex>, IEnumerable<TopologyEdge>>();
                var mesh = new List<TopologyPath>();                            // Итоговая сеть смешанной топологии
                var meshPartSources = new List<TopologyVertex> { source };      // Все возможные источники сети
                var remainedTargets = new List<TopologyVertex>(targets);        // Оставшиеся целевые вершины

                for (var i = 0; i < targets.Count(); i++)           // Пока не переберём все целевые вершины
                {
                    foreach (var partSource in meshPartSources)     // Перебираем все имеющиеся источники сети
                    {
                        foreach (var target in remainedTargets)     // Перебираем все оставшиеся целевые вершины
                        {
                            var path = GetPath(graph, partSource, target, dataChannel);    // Ищем кратчайший путь из источника в цель и сохраняем его как кандидат на кратчайший 

                            meshPartCandidates.Add(new KeyValuePair<TopologyVertex, TopologyVertex>(partSource, target), path);
                        }
                    }

                    // Находим минимальный вес среди кандидатов, причём, если источник и цель в одной вершине - вес нулевой
                    var minWeight = meshPartCandidates.Min(candidate => candidate.Value?.Sum(edge => edge.Weights[dataChannel.ConnectionType]) ?? 0);

                    // Определяем путь с минимальным весом и добавляем в шину
                    var shortestPath = meshPartCandidates.FirstOrDefault(candidate => (candidate.Value?.Sum(edge => edge.Weights[dataChannel.ConnectionType]) ?? 0) == minWeight);

                    var shortestPathSource = shortestPath.Key.Key;      // Источник находится в ключе пары найденного пути, где сам является ключем
                    var shortestPathTarget = shortestPath.Key.Value;    // Целевая вершина находится в ключе пары найденного пути, где является значением

                    mesh.Add(new TopologyPath
                    {
                        DataChannel = dataChannel,
                        Path = shortestPath.Value,
                        Source = shortestPathSource,
                        Target = shortestPathTarget
                    });

                    // Вершина-источник найденной части сети тоже теперь может являться источником, если ещё не является
                    if (!meshPartSources.Contains(shortestPathTarget))
                        meshPartSources.Add(shortestPathTarget);

                    remainedTargets.Remove(shortestPathTarget);     // Больше не рассматриваем целевую вершину из найденной части сети
                    meshPartCandidates.Clear();                     // Ищем кандидатов заново
                }

                return mesh;
            }
            catch (Exception ex)
            {
                Console.WriteLine("BusShortestPath failed! {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Найти кратчайший путь по топологии "Звезда".
        /// </summary>
        /// <param name="graph">Граф, в котором ищем путь.</param>
        /// <param name="source">Вершина графа с УСПД.</param>
        /// <param name="targets">Вершины КУ, которые нужно соединить кратчайшим путём.</param>
        /// <param name="dataChannel">Канал передачи данных, по которому соеденяются КУ и УСПД.</param>
        /// <returns>Перечисление путей в графе.</returns>
        private static IEnumerable<TopologyPath> StarShortestPath(TopologyGraph graph, TopologyVertex source, IEnumerable<TopologyVertex> targets, DataChannel dataChannel)
        {
            try
            {
                // Задаём результирующий контейнер путей, источник и алгоритм расчёта весов   
                var resultPath = new List<TopologyPath>();

                foreach (var target in targets)          // Для звезды находим пути из источника ко всем целям
                {
                    var path = GetPath(graph, source, target, dataChannel);

                    resultPath.Add(new TopologyPath
                    {
                        DataChannel = dataChannel,
                        Path = path,
                        Source = source,
                        Target = target
                    });
                }

                return resultPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine("StarShortestPath failed! {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Найти кратчайший путь по топологии "Шина".
        /// </summary>
        /// <param name="graph">Граф, в котором ищем путь.</param>
        /// <param name="source">Вершина графа с УСПД.</param>
        /// <param name="targets">Вершины КУ, которые нужно соединить кратчайшим путём.</param>
        /// <param name="dataChannel">Канал передачи данных, по которому соеденяются КУ и УСПД.</param>
        /// <returns>Перечисление путей в графе.</returns>
        private static IEnumerable<TopologyPath> BusShortestPath(TopologyGraph graph, TopologyVertex source, IEnumerable<TopologyVertex> targets, DataChannel dataChannel)
        {
            try
            {
                var bus = new List<TopologyPath>();                                                     // Итоговая шина, состоящая из составных частей - путей между вершинами шины
                var busPartCandidates = new Dictionary<TopologyVertex, IEnumerable<TopologyEdge>>();    // Кандидаты на составные части шины, где ключ - целевая вершина, значение - путь до неё
                var busPartSource = source;                                                             // Текущая вершина-источник части шины
                var remainedTargets = new List<TopologyVertex>(targets);                                // Оставшиеся целевые вершины

                for (var i = 0; i < targets.Count(); i++)       // Пока не переберём все целевые вершины
                {
                    foreach (var target in remainedTargets)     // Перебираем все оставшиеся целевые вершины
                    {
                        var path = GetPath(graph, busPartSource, target, dataChannel);       // Ищем кратчайший путь из источника в цель и сохраняем его как кандидат на кратчайший

                        busPartCandidates.Add(target, path);
                    }

                    // Находим минимальный вес среди кандидатов, причём, если источник и цель в одной вершине - вес нулевой
                    var minWeight = busPartCandidates.Min(candidate => candidate.Value?.Sum(edge => edge.Weights[dataChannel.ConnectionType]) ?? 0);

                    // Определяем путь с минимальным весом и добавляем в шину
                    var shortestPath = busPartCandidates.FirstOrDefault(candidate => (candidate.Value?.Sum(edge => edge.Weights[dataChannel.ConnectionType]) ?? 0) == minWeight);
                    var shortestPathTarget = shortestPath.Key;    // Целевая вершина находится в ключе найденной пары пути

                    bus.Add(new TopologyPath
                    {
                        DataChannel = dataChannel,
                        Path = shortestPath.Value,
                        Source = busPartSource,
                        Target = shortestPathTarget
                    });

                    busPartSource = shortestPathTarget;             // Целевая вершина найденного пути теперь источник    
                    remainedTargets.Remove(shortestPathTarget);     // Убираем эту вершину из оставшихся целевых вершин
                    busPartCandidates.Clear();                      // Ищем кандидатов заново
                }

                return bus;
            }
            catch (Exception ex)
            {
                Console.WriteLine("BusShortestPath failed! {0}", ex.Message);
                return null;
            }
        }
    }
}
