using TopologyModel.Enumerations;

namespace TopologyModel
{
	/// <summary>
	/// Класс точки учёта и управления.
	/// </summary>
	public class MeasurementAndControlPoint
	{
		/// <summary>
		/// уникальный идентификатор ТУУ
		/// </summary>
		public uint Id { get; set; }

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
