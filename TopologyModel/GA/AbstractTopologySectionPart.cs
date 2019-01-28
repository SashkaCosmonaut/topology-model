using TopologyModel.TopologyGraphs;
using TopologyModel.Tools;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс абстрактной части секции топологии, содержащий параметры выбора КПД
    /// и месторасположения данной части секции.
    /// </summary>
    public abstract class AbstractTopologySectionPart
    {
        /// <summary>
        /// Узел графа, в котором находится данная часть секции топологии.
        /// </summary>
        public TopologyEdge Edge { get; set; }

        /// <summary>
        /// Канал передачи данных, который используется для отправки или приема данных.
        /// </summary>
        public DataChannel Channel { get; set; }
    }
}
