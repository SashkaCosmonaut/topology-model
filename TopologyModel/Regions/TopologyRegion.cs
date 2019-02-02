using Newtonsoft.Json;
using System;
using TopologyModel.Enumerations;

namespace TopologyModel.Regions
{
    /// <summary>
    /// Класс участка объекта предприятия.
    /// </summary>
    public class TopologyRegion : AbstractRegion
    {
        /// <summary>
        /// Максимальное количество оценок в массивах оценок.
        /// </summary>
        public const ushort EstimatesArraysLength = 5;

        /// <summary>
        /// наличие на данном участке проводного подключения к локальной сети предприятия 
        /// </summary>
        public bool HasLan { get; set; }

		/// <summary>
		/// экспертная оценка недоступности (слабости сигнала) на данном участке беспроводного подключения к локальной сети объекта предприятия
		/// </summary>
		public bool HasWiFi { get; set; }

		/// <summary>
		/// наличие на данном участке питающей электрической сети 220 В
		/// </summary>
		public bool HasPower { get; set; }

		/// <summary>
		/// экспертная оценка доступности на данном участке различных радиоволн извне участка
		/// </summary>
		public ushort MobileInternetSignalEstimate { get; set; }    // Вес проводной связи сквозь стену


		/// <summary>
		/// Экспертная оценка агрессивности окружающей среды на стенах участка и внутри участка: сверху, справа, снизу, слева, внутри
		/// </summary>
		public ushort[] Aggressiveness { get; set; }

		/// <summary>
		/// Экспертная оценка непригодности стен участка для монтажа оборудования и внутри: сверху, справа, снизу, слева, внутри
		/// </summary>
		public ushort[] Unavailability { get; set; }

		/// <summary>
		/// Экспертная оценка трудоемкости проведения монтажных работ на стенах участка и внутри участка: сверху, справа, снизу, слева, внутри
		/// </summary>
		public ushort[] Laboriousness { get; set; }

        /// <summary>
        /// Экспертная оценка непроходимости радиоволн через стены участка и внутри участка: сверху, справа, снизу, слева, внутри
        /// </summary>
        public ushort[] BadRadioTransmittance { get; set; }

        /// <summary>
        /// Экспертная оценка трудоемкости проведения связи (кабелей) внутри и на соседние участки через стены: сверху, справа, снизу, слева, внутри
        /// </summary>
        public ushort[] BadWiredTransmittance { get; set; }

        /// <summary>
        /// Получить информацию об основных свойствах участка.
        /// </summary>
        /// <returns>Строка с JSON-объектом свойств участка.</returns>
        public override string GetInfo()
        {
            return JsonConvert.SerializeObject(new
            {
                Name,
                Equipment = new {
                    HasLan,
                    HasWiFi,
                    HasPower,
                    Mobile = MobileInternetSignalEstimate,
                },
                Estimates = new {
                    Aggressiveness,
                    Unavailability,
                    Laboriousness,
                    BadRadioTransmittance,
                    BadWiredTransmittance
                }
            });
        }
    }
}
