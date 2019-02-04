using System;
using System.Collections.Generic;
using TopologyModel.Enumerations;
using TopologyModel.Tools;

namespace TopologyModel.Graphs
{
    /// <summary>
    /// Класс для поиска путей для секций между вершинами графа.
    /// </summary>
    public static class TopologyPathfinder
    {
        /// <summary>
        /// Найти кратчайший путь в секции от вершины источника - УСПД, через все целевые вершины КУ.
        /// </summary>
        /// <param name="graph">Граф, в котором ищем путь.</param>
        /// <param name="source">Вершина графа с УСПД.</param>
        /// <param name="targets">Вершины КУ, которые нужно соединить кратчайшим путём.</param>
        /// <param name="dataChannel">Канал передачи данных, по которому соеденяются КУ и УСПД.</param>
        /// <returns>Массив вершин пути в графе.</returns>
        public static IEnumerable<TopologyEdge> SectionShortestPath(TopologyGraph graph, TopologyVertex source, IEnumerable<TopologyVertex> targets, DataChannel dataChannel)
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
        /// <returns>Массив вершин пути в графе.</returns>
        private static IEnumerable<TopologyEdge> MeshShortestPath(TopologyGraph graph, TopologyVertex source, IEnumerable<TopologyVertex> targets, DataChannel dataChannel)
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
        /// <returns>Массив вершин пути в графе.</returns>
        private static IEnumerable<TopologyEdge> StarShortestPath(TopologyGraph graph, TopologyVertex source, IEnumerable<TopologyVertex> targets, DataChannel dataChannel)
        {
            return null;
        }

        /// <summary>
        /// Найти кратчайший путь по топологии "Шина".
        /// </summary>
        /// <param name="graph">Граф, в котором ищем путь.</param>
        /// <param name="source">Вершина графа с УСПД.</param>
        /// <param name="targets">Вершины КУ, которые нужно соединить кратчайшим путём.</param>
        /// <param name="dataChannel">Канал передачи данных, по которому соеденяются КУ и УСПД.</param>
        /// <returns>Массив вершин пути в графе.</returns>
        private static IEnumerable<TopologyEdge> BusShortestPath(TopologyGraph graph, TopologyVertex source, IEnumerable<TopologyVertex> targets, DataChannel dataChannel)
        {
            return null;
        }
    }
}
