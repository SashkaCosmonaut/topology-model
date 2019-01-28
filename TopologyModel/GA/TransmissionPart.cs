using TopologyModel.Tools;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс той части секции топологии, в которой находится приемопередатчик.
    /// </summary>
    public class TransmissionPart : AbstractTopologySectionPart
    {
        /// <summary>
        /// Приемопередатчик, который используется в данной секции.
        /// </summary>
        public TransceiverDevice Transceiver { get; set; }
    }
}
