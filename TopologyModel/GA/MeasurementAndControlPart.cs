using TopologyModel.Tools;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс той части секции топологии, в которой находится устройство учёта или управления.
    /// </summary>
    public class MeasurementAndControlPart : AbstractTopologySectionPart
    {
        /// <summary>
        /// Устройство учёта или управления, которое используется в данной секции.
        /// </summary>
        public MeasurementAndControlDevice MCD { get; set; }
    }
}
