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
        public const ushort MAX_NUMBER_OF_ESTIMATES = 5;

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

        /// <summary>
        /// Получить оценку агрессивности.
        /// </summary>
        /// <param name="location">Расположение внутри участка.</param>
        /// <returns>Значение оценки.</returns>
        public float GetAggressivenessEstimate(LocationInRegion location)
        {
            return GetEstimate(Aggressiveness, location);
        }

        /// <summary>
        /// Получить оценку непригодности.
        /// </summary>
        /// <param name="location">Расположение внутри участка.</param>
        /// <returns>Значение оценки.</returns>
        public float GetUnavailabilityEstimate(LocationInRegion location)
        {
            return GetEstimate(Unavailability, location);
        }

        /// <summary>
        /// Получить оценку трудоемкости.
        /// </summary>
        /// <param name="location">Расположение внутри участка.</param>
        /// <returns>Значение оценки.</returns>
        public float GetLaboriousnessEstimate(LocationInRegion location)
        {
            return GetEstimate(Laboriousness, location);
        }

        /// <summary>
        /// Получить оценку радио непроходимости.
        /// </summary>
        /// <param name="location">Расположение внутри участка.</param>
        /// <returns>Значение оценки.</returns>
        public float GetBadRadioTransmittanceEstimate(LocationInRegion location)
        {
            return GetEstimate(BadRadioTransmittance, location);
        }

        /// <summary>
        /// Получить оценку трудности прокладывания кабелей.
        /// </summary>
        /// <param name="location">Расположение внутри участка.</param>
        /// <returns>Значение оценки.</returns>
        public float GetBadWiredTransmittanceEstimate(LocationInRegion location)
        {
            return GetEstimate(BadWiredTransmittance, location);
        }

        /// <summary>
        /// Получить экспертную оценку в зависимости от месторасположения и количества оценок в массиве.
        /// </summary>
        /// <param name="estimates">Массив экспертных оценок для участка.</param>
        /// <param name="location">Расположение внутри участка.</param>
        /// <returns>Значение оценки.</returns>
        protected static float GetEstimate(ushort[] estimates, LocationInRegion location)
        {
            try
            {
                if (estimates == null || estimates.Length == 0)
                    throw new ArgumentNullException(nameof(estimates), "Incorrect size of estimates array!");

                if (location == LocationInRegion.Inside)                // Для внутренней части участка - последняя оценка 
                    return GetEstimateFromArray(estimates, MAX_NUMBER_OF_ESTIMATES - 1);

                if ((int)location < MAX_NUMBER_OF_ESTIMATES - 1)        // Если требуется не угловая оценка, берём просто по индексу в массиве оценок 
                    return GetEstimateFromArray(estimates, (int)location);

                switch (location)       // Для угловых месторасположений считаем среднее по двум границам
                {
                    case LocationInRegion.LeftTopCorder:
                        return (float)(GetEstimateFromArray(estimates, (int)LocationInRegion.LeftBorder) +
                                       GetEstimateFromArray(estimates, (int)LocationInRegion.TopBorder)) / 2;

                    case LocationInRegion.RightTopCorner:
                        return (float)(GetEstimateFromArray(estimates, (int)LocationInRegion.RightBorder) +
                                       GetEstimateFromArray(estimates, (int)LocationInRegion.TopBorder)) / 2;

                    case LocationInRegion.RightBottomCorner:
                        return (float)(GetEstimateFromArray(estimates, (int)LocationInRegion.RightBorder) +
                                       GetEstimateFromArray(estimates, (int)LocationInRegion.BottomBorder)) / 2;

                    case LocationInRegion.LeftBottomCorner:
                        return (float)(GetEstimateFromArray(estimates, (int)LocationInRegion.LeftBorder) +
                                       GetEstimateFromArray(estimates, (int)LocationInRegion.BottomBorder)) / 2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetEstimate failed! {0}", ex.Message);
            }

            return 0;
        }

        /// <summary>
        /// Получить экспертную оценку из массива оценок в зависимости от количества оценок в массиве.
        /// </summary>
        /// <param name="estimates">Массив экспертных оценок для участка.</param>
        /// <param name="location">Расположение внутри участка.</param>
        /// <returns>Значение оценки из массива.</returns>
        protected static ushort GetEstimateFromArray(ushort[] estimates, int intLocation)
        {
            if (intLocation >= MAX_NUMBER_OF_ESTIMATES) return 0;   // Проверяем, что локация адекватна

            return estimates.Length == MAX_NUMBER_OF_ESTIMATES  // Если в массиве все значения
                ? estimates[intLocation]                        // Берём в зависимости от местарасположения
                : estimates[0];                                 // Иначе берём первое значение
        }
    }
}
