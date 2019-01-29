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
        public MeasurementAndControlPart MACPart { get; set; } = new MeasurementAndControlPart();

        /// <summary>
        /// Параметры выбора и расположения первого приемопередатчика, а так же исходящего КПД.
        /// </summary>
        public TransmissionPart FirstTransmissionPart { get; set; } = new TransmissionPart();

        /// <summary>
        /// Параметры выбора и расположения второго приемопередатчика, а так же исходящего КПД.
        /// </summary>
        public TransmissionPart SecondTransmissionPart { get; set; } = new TransmissionPart();

        /// <summary>
        /// Параметры выбора и расположения УСПД, а так же входящего КПД.
        /// </summary>
        public DataAcquisitionPart DADPart { get; set; } = new DataAcquisitionPart();
    }
}
