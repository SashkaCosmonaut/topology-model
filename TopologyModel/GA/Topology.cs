using System.Collections.Generic;
using TopologyModel.TopologyGraphs;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс предлагаемой топологии - результат работы метода.
    /// </summary>
    public class Topology
    {
        /// <summary>
        /// Секции топологии.
        /// </summary>
        public TopologySection[] Sections { get; set; }

        /// <summary>
        /// Связи между элементами секций.
        /// </summary>
        public IEnumerable<TopologyEdge> Pathes { get; set; }
    }
}
