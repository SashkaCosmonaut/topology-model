using System;

namespace TopologyModel
{
    /// <summary>
    /// Класс методов расширения.
    /// </summary>
    public static class TopologyExtensionMethods
    {
        /// <summary>
        /// Получить экспертную оценку из массива оценок в зависимости от индекса границы и количества оценок в массиве.
        /// </summary>
        /// <param name="estimates">Массив экспертных оценок для участка.</param>
        /// <param name="borderIndex">Индекс границы участка.</param>
        /// <returns>Значение оценки из массива.</returns>
        public static ushort GetEstimate(this ushort[] estimates, uint borderIndex)
        {
            if (estimates == null || estimates.Length == 0 || borderIndex >= estimates.Length)
                throw new ArgumentNullException(nameof(estimates), "Incorrect size of estimates array!");

            return estimates.Length == 4        // Если в массиве все четыре значения
                ? estimates[borderIndex]        // Берём в зависимости от узла-источника и узла-цели
                : estimates[0];                 // Иначе берём первое значение
        }
    }
}
