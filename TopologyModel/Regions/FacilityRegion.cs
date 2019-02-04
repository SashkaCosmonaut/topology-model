using Newtonsoft.Json;
using System;
using TopologyModel.Enumerations;

namespace TopologyModel.Regions
{
    /// <summary>
    /// Класс участка объекта предприятия.
    /// </summary>
    public class FacilityRegion : AbstractRegion
    {
        /// <summary>
        /// Длина массивов оценок свойств участка.
        /// </summary>
        public const ushort ESTIMATES_ARRAY_LENGTH = 5;

        /// <summary>
        /// Глобальный автоприращиваемый идентификатор участка.
        /// </summary>
        private static uint GlobalId = 0;

        /// <summary>
        /// Уникальный идентификатор участка.
        /// </summary>
        public uint Id { get; protected set; }

        #region EquipmentProperties

        /// <summary>
        /// Наличие на данном участке проводного подключения к локальной сети предприятия 
        /// </summary>
        public bool HasLan { get; set; }

        /// <summary>
        /// Наличие на данном участке беспроводного подключения к локальной сети объекта предприятия
        /// </summary>
        public bool HasWiFi { get; set; }

        /// <summary>
        /// Наличие на данном участке питающей электрической сети 220 В
        /// </summary>
        public bool HasPower { get; set; }

        /// <summary>
        /// Экспертная оценка доступности на данном участке различных радиоволн извне участка
        /// </summary>
        public ushort BadMobileInternetSignal { get; set; }    // Вес проводной связи сквозь стену

        #endregion

        #region EstimatesArrays

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

        #endregion

        /// <summary>
        /// Создать и проинициализировать участок по умолчанию.
        /// </summary>
        public FacilityRegion()
        {
            Id = ++GlobalId;
        }

        /// <summary>
        /// Получить строковую интерпретацию участка.
        /// </summary>
        /// <returns>Строка с описанием свойств объекта участка.</returns>
        public override string ToString()
        {
            return Id + ". " + Name;
        }

        /// <summary>
        /// Получить информацию об основных свойствах участка.
        /// </summary>
        /// <returns>Строка с JSON-объектом свойств участка.</returns>
        public override string GetInfo()
        {
            return JsonConvert.SerializeObject(new
            {
                Name,
                Equipment = new
                {
                    HasLan,
                    HasWiFi,
                    HasPower,
                    MobileSignal = BadMobileInternetSignal,
                },
                Estimates = new
                {
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
                    throw new ArgumentNullException(nameof(estimates), "Estimates array is null or empty!");

                if (location == LocationInRegion.Inside)                // Для внутренней части участка - последняя оценка 
                    return GetEstimateFromArray(estimates, ESTIMATES_ARRAY_LENGTH - 1);

                if ((int)location < ESTIMATES_ARRAY_LENGTH - 1)         // Если требуется не угловая оценка, берём просто по индексу в массиве оценок 
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
        /// <param name="intLocation">Расположение внутри участка в виде целого числа.</param>
        /// <returns>Значение оценки из массива.</returns>
        protected static ushort GetEstimateFromArray(ushort[] estimates, int intLocation)
        {
            if (intLocation >= ESTIMATES_ARRAY_LENGTH) return 0;    // Проверяем, что локация адекватна

            return estimates.Length == ESTIMATES_ARRAY_LENGTH       // Если в массиве все значения
                ? estimates[intLocation]                            // Берём в зависимости от местарасположения
                : estimates[0];                                     // Иначе берём первое значение
        }
    }
}
