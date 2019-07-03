using System;
using System.Collections.Generic;

namespace EnergySupplyModel.Materials
{
    /// <summary>
    /// Элемент данных.
    /// </summary>
    public class DataItem
    {
        /// <summary>
        /// Отметка даты времени данного элемента данных.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Значение элемента данных.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Метаданные данного элемента данных.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Частные оценки качества данных.
        /// </summary>
        public Dictionary<string, string> DQAssessments { get; set; }

        /// <summary>
        /// Агрегированная оценка качества данных в диапазоне [0..1].
        /// </summary>
        public double DQTotalAssessment { get; set; }
    }
}
