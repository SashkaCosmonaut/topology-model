using TopologyModel.Enumerations;

namespace TopologyModel.Tools
{
	/// <summary>
	/// Класс конечного устройства.
	/// </summary>
	public class MeasurementAndControlDevice : Device
	{
		/// <summary>
		/// множество всех измерений потребления энергоресурсов, выдаваемых данным КУ
		/// </summary>
		public Measurement[] Measurements { get; set; } = new Measurement[] { };

		/// <summary>
		/// множество всех управляющих воздействий, позволяющих осуществлять данным КУ
		/// </summary>
		public Control[] Сontrols { get; set; } = new Control[] { };
	}
}
