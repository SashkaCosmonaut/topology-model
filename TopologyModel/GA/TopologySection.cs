namespace TopologyModel.GA
{
    /// <summary>
    /// Класс части (секции), на которые разбита топология, 
    /// для связи одного места учёта и управления с сервером.
    /// </summary>
    public class TopologySection
    {
        /// <summary>
        /// Параметры выбора и расположения устройства учёта и управления, а так же исходящего КПД.
        /// </summary>
        public MeasurementAndControlPart MACPart { get; set; }

        /// <summary>
        /// Параметры выбора и расположения первого приемопередатчика, а так же исходящего КПД.
        /// </summary>
        public TransmissionPart FirstTransmissionPart { get; set; }

        /// <summary>
        /// Параметры выбора и расположения второго приемопередатчика, а так же исходящего КПД.
        /// </summary>
        public TransmissionPart SecondTransmissionPart { get; set; }

        /// <summary>
        /// Параметры выбора и расположения УСПД, а так же входящего КПД.
        /// </summary>
        public DataAcquisitionPart DADPart { get; set; }
    }
}
