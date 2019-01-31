using System.Collections.Generic;
using TopologyModel.Tools;
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
        /// Словарь связей между элементами одной и более секций, где ключ - УСПД, 
        /// из которого исходит связь, а значение - перечисление граней графа связи.
        /// </summary>
        public Dictionary<DataAcquisitionDevice, IEnumerable<TopologyEdge>> Pathes { get; set; }
    }
}
