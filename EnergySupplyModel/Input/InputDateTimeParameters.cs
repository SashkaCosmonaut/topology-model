using EnergySupplyModel.Enumerations;
using System;

namespace EnergySupplyModel.Input
{
    /// <summary>
    /// Класс временных параметров запроса данных.
    /// </summary>
    public class InputDateTimeParameters
    {
        /// <summary>
        /// Начало периода анализа.
        /// </summary>
        public DateTime Start { get; set; } = new DateTime(2010, 01, 10);

        /// <summary>
        /// Конец периода анализа.
        /// </summary>
        public DateTime End { get; set; } = new DateTime(2010, 01, 11);

        /// <summary>
        /// Шаг разбиения периода времени.
        /// </summary>
        public TimeInterval Interval { get; set; } = TimeInterval.Hour1;
    }
}
