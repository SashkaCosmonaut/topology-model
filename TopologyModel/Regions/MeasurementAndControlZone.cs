using TopologyModel.Enumerations;

namespace TopologyModel.Regions
{
    /// <summary>
    /// Класс места учёта и управления (или только точки учёта и управления).
    /// </summary>
    public class MeasurementAndControlZone : AbstractRegion
	{
        /// <summary>
        /// экспертная оценка приоритета покрытия ТУУ сетью
        /// </summary>
        public uint Priority { get; set; }

		/// <summary>
		/// множество всех измерений потребления энергоресурсов, доступных на данной ТУУ
		/// </summary>
		public Measurement[] AllMeasurements { get; set; } = new Measurement[] { };

		/// <summary>
		/// множество всех управляющих воздействий, доступных на данной ТУУ
		/// </summary>
		public Control[] AllСontrols { get; set; } = new Control[] { };

		/// <summary>
		/// допустима ли замена уже имеющихся КУ на ТУУ на более новые
		/// </summary>
		public bool IsDeviceReplacementAvailable { get; set; }
	}
}
