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
        // TODO: Сделать кэш уже найденных путей и брать из него, если можно не искать

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
            return null;
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
                var tryGetPath = graph.ShortestPathsDijkstra((edge) => { return edge.ChooseWeight(dataChannel); }, source);

                // TODO: Пропускаем случаи, когда УСПД и КУ находятся в одной вершине
                foreach (var target in targets)          // Для звезды находим пути из источника ко всем целям
                {
                    tryGetPath(target, out var path);

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
                var bus = new List<TopologyPath>();
                var busPartCandidates = new Dictionary<TopologyVertex, IEnumerable<TopologyEdge>>();
                var busPartSource = source;

                for (var i = 0; i < targets.Count(); i++)   // Пока не переберём все вершины целей
                {
                    // Перебираем те цели, которые ещё не уложили в шину
                    foreach (var target in targets.Where(q => !bus.Select(w => w.Target).Contains(q)))
                    {
                        var tryGetPath = graph.ShortestPathsDijkstra((edge) => { return edge.ChooseWeight(dataChannel); }, busPartSource);

                        tryGetPath(target, out var path);   // Ищем кратчайший путь из источника в цель и сохраняем его как кандидат на кратчайший

                        busPartCandidates.Add(target, path);
                    }

                    // Находим минимальный вес среди кандидатов, причём, если источник и цель в одной вершине - вес нулевой
                    var minWeight = busPartCandidates.Min(candidate => candidate.Value?.Sum(edge => edge.ChooseWeight(dataChannel)) ?? 0);

                    // Определяем путь с минимальным весом и добавляем в шину
                    var shortestPath = busPartCandidates.FirstOrDefault(candidate => (candidate.Value?.Sum(edge => edge.ChooseWeight(dataChannel)) ?? 0) == minWeight);

                    bus.Add(new TopologyPath
                    {
                        DataChannel = dataChannel,
                        Path = shortestPath.Value,
                        Source = busPartSource,
                        Target = shortestPath.Key
                    });

                    // Ищем дальше путь из цели лучшего кандидата и ищем кандидатов заново
                    busPartSource = shortestPath.Key;
                    busPartCandidates.Clear();
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
