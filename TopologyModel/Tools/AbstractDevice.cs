using System;
using TopologyModel.Enumerations;

namespace TopologyModel.Tools
{
	/// <summary>
	/// Класс абстрактного устройства.
	/// </summary>
	public abstract class AbstractDevice : AbstractTool
	{
		/// <summary>
		/// множество стандартов отправки данных, по которым данное устройство отправляет данные
		/// </summary>
		public Protocol[] SendingProtocols { get; set; }

		/// <summary>
		/// требуется ли питание от электрической сети 220В для работы устройства
		/// </summary>
		public bool IsPowerRequired { get; set; }

		/// <summary>
		/// время в часах автономной работы от аккумуляторных батарей, если имеются
		/// </summary>
		public TimeSpan BatteryTime { get; set; }

		/// <summary>
		/// стоимость сервисного обслуживания для замены аккумуляторной батареи 
		/// </summary>
		public double BatteryServicePrice { get; set; }
	}
}
