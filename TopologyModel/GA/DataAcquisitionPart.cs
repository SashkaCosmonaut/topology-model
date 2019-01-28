using TopologyModel.Tools;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс той части секции топологии, в которой находится УСПД.
    /// </summary>
    public class DataAcquisitionPart : AbstractTopologySectionPart
    {
        /// <summary>
        /// УСПД, которое используется в данной секции топологии.
        /// </summary>
        public DataAcquisitionDevice DAD;
    }
}
