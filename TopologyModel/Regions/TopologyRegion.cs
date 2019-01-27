using Newtonsoft.Json;

namespace TopologyModel.Regions
{
	/// <summary>
	/// Класс участка объекта предприятия.
	/// </summary>
	public class TopologyRegion : AbstractRegion
	{
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
		/// Экспертная оценка агрессивности окружающей среды внутри самого участка
		/// </summary>
		public ushort InsideAggressivenessEstimate { get; set; }

		/// <summary>
		/// Экспертная оценка непригодности пространства внутри участка для монтажа оборудования
		/// </summary>
		public ushort InsideUnavailabilityEstimate { get; set; }

		/// <summary>
		/// Экспертная оценка от 1 до 10 трудоемкости проведения монтажных работ внутри участка
		/// </summary>
		public ushort InsideLaboriousnessEstimate { get; set; }

		/// <summary>
		/// непроходимость радиоволн на самом участке (наличие металлоконструкций, электромагнитных помех, экранов)
		/// </summary>
		public ushort InsideBadRadioTransmittanceEstimate { get; set; }

		/// <summary>
		/// Экспертная оценка агрессивности окружающей среды на стенах участка: сверху, справа, снизу, слева
		/// </summary>
		public ushort[] WallsAggressivenessEstimate { get; set; }

		/// <summary>
		/// Экспертная оценка непригодности стен участка для монтажа оборудования: сверху, справа, снизу, слева
		/// </summary>
		public ushort[] WallsUnavailabilityEstimate { get; set; }

		/// <summary>
		/// экспертная оценка трудоемкости проведения монтажных работ на стенах участка: сверху, справа, снизу, слева
		/// </summary>
		public ushort[] WallsLaboriousnessEstimate { get; set; }

		/// <summary>
		/// непроходимость радиоволн через стены участка: сверху, справа, снизу, слева
		/// </summary>
		public ushort[] WallsBadRadioTransmittanceEstimate { get; set; }

		/// <summary>
		/// Трудоемкость проведения связи (кабелей) на соседние участки через стены: сверху, справа, снизу, слева
		/// </summary>
		public ushort[] WallsBadWiredTransmittanceEstimate { get; set; }

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
                Inside = new {
                    Aggressiveness  = InsideAggressivenessEstimate,       
                    Unavailability  = InsideUnavailabilityEstimate,
                    Laboriousness   = InsideLaboriousnessEstimate,
                    Radio           = InsideBadRadioTransmittanceEstimate,
                },
                AlongTheWalls = new {
                    Aggressiveness  = WallsAggressivenessEstimate,
                    Unavailability  = WallsUnavailabilityEstimate,
                    Laboriousness   = WallsLaboriousnessEstimate,
                    Radio           = InsideBadRadioTransmittanceEstimate,
                },
                AcrossTheWalls = new {
                    Radio = WallsBadRadioTransmittanceEstimate,
                    Wired = WallsBadWiredTransmittanceEstimate
                }
            });
        }
	}
}
