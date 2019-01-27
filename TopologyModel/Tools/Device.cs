using System;
using TopologyModel.Enumerations;

namespace TopologyModel.Tools
{
	/// <summary>
	/// Класс абстрактного устройства.
	/// </summary>
	public abstract class Device
	{
        /// <summary>
        /// Глобальный автоприращиваемый идентификатор.
        /// </summary>
        private static uint GlobalId = 0;

        /// <summary>
        /// уникальный идентификатор устройства
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// Наименование устройства.
        /// </summary>
        public string Name { get; set; }

		/// <summary>
		/// множество стандартов отправки данных, по которым данное устройство отправляет данные
		/// </summary>
		public Protocol[] SendingProtocols { get; set; }


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

        /// <summary>
        /// Создать некоторое устройство.
        /// </summary>
        public Device()
        {
            Id = ++GlobalId;
        }
	}
}
