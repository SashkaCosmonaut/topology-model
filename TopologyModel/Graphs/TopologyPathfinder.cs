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
                var resultPath = new List<TopologyPath>();

                // Задаём источник и способ расчёта весов   
                var tryGetPath = graph.ShortestPathsDijkstra((edge) => { return dataChannel.IsWireless ? edge.WirelessWeight : edge.WiredWeight; }, source);

                foreach (var target in targets.Where(t => t != source)) // Пропускаем случаи, когда УСПД и КУ находятся в одной вершине
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
            return null;
        }
    }
}
