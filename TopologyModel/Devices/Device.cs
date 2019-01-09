using System;
using TopologyModel.Enumerations;

namespace TopologyModel.Devices
{
	/// <summary>
	/// Класс абстрактного устройства.
	/// </summary>
	public abstract class Device
	{
		/// <summary>
		/// множество стандартов передачи данных, по которым данное устройство отправляет данные
		/// </summary>
		public Protocol[] SendingProtocols { get; set; } = new Protocol[] { };


		/// <summary>
		/// стоимость КУ
		/// </summary>
		public double PurchasePrice { get; set; }

		/// <summary>
		/// стоимость монтажа КУ, в которую входят финансовые затраты на монтажные работы
		/// </summary>
		public double InstallationPrice { get; set; }

		/// <summary>
		/// время на установку КУ, требуемое для монтажных работы по установке данного устройства и дополнительных материалов
		/// </summary>
		public TimeSpan InstallationTime { get; set; }


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
