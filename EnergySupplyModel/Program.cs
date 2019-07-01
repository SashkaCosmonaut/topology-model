using EnergySupplyModel.Facilities;
using System;

namespace EnergySupplyModel
{
    public class Program
    {
        /// <summary>
        /// Порог выявления проблемы.
        /// </summary>
        public const double EPSILON_P = 10;

        /// <summary>
        /// Порог выявления утечки.
        /// </summary>
        public const double EPSILON_S = 10;

        /// <summary>
        /// Порог выявления рентабельности меропритий.
        /// </summary>
        public const double EPSILON_C = 80;

        /// <summary>
        /// Стоимость энергоресурса в текущий момент времени.
        /// </summary>
        public static double Cost { get; set; } = 8;

        /// <summary>
        /// Затраты на мероприятия по оптимизации данного объекта.
        /// </summary>
        public static double ActivityCost { get; set; } = 10;

        /// <summary>
        /// Главная функция программы.
        /// </summary>
        /// <param name="args">Аргументы командной строки.</param>
        public static void Main(string[] args)
        {
            var factory = new Manufacturing();

            Check(factory);
        }

        /// <summary>
        /// Выполнить функцию проверки объектов.
        /// </summary>
        protected static void Check(Facility facility)
        {
            Console.WriteLine(facility.Name);

            CheckExprectedDiff(facility);

            CheckLeakDiff(facility);

            CheckEffectDiff(facility);

            Console.WriteLine("--------------------");

            if (facility is ComplexFacility complexFacility && complexFacility.Subfacilities != null)
            {
                foreach (var subFacility in complexFacility.Subfacilities)
                {
                    Check(subFacility);
                }
            }
        }

        /// <summary>
        /// Проверить, что ожидаемые значения сответствуют действительным.
        /// </summary>
        /// <param name="facility">Проверяемый объект предприятия.</param>
        protected static void CheckExprectedDiff(Facility facility)
        {
            var measuredConsumption = facility.GetMeasuredConsumption();

            if (!measuredConsumption.HasValue || facility.GetExpectedConsumption == null)
                return;

            var expectedConsumption = facility.GetExpectedConsumption.Invoke();

            var exprectedDiff = Math.Abs(measuredConsumption.Value - expectedConsumption);

            Console.WriteLine($"Expected: {expectedConsumption}, Measured: {measuredConsumption.Value}, ExpectedDiff: {exprectedDiff}, Result: {exprectedDiff <= EPSILON_P}");
        }

        /// <summary>
        /// Проверить, что суммарные значения подобъектов соответствуют действительному значению объекта.
        /// </summary>
        /// <param name="facility">Проверяемый объект предприятия.</param>
        protected static void CheckLeakDiff(Facility facility)
        {
            var measuredConsumption = facility.GetMeasuredConsumption();

            if (!measuredConsumption.HasValue)
                return;

            if (facility is ComplexFacility complexFacility)
            {
                var summaryConsumption = complexFacility.GetSummaryConsumption();

                if (!summaryConsumption.HasValue)
                    return;

                var leakDiff = Math.Abs(measuredConsumption.Value - summaryConsumption.Value);

                Console.WriteLine($"Summary: {summaryConsumption.Value}, Measured: {measuredConsumption.Value}, LeakDiff: {leakDiff}, Result: {leakDiff <= EPSILON_S}");
            }
        }

        /// <summary>
        /// Проверить, что применяемые меры являются рентабельными.
        /// </summary>
        /// <param name="facility">Проверяемый объект предприятия.</param>
        protected static void CheckEffectDiff(Facility facility)
        {
            if (facility.GetExpectedConsumption == null || facility.GetPotentialConsumption == null)
                return;

            var expectedConsumption = facility.GetExpectedConsumption.Invoke();

            var potentialConsumption = facility.GetPotentialConsumption.Invoke();

            var effectDiff = Cost * (expectedConsumption - potentialConsumption) - ActivityCost;

            Console.WriteLine($"Expected: {expectedConsumption}, Potential: {potentialConsumption}, EffectDiff: {effectDiff}, Result: {effectDiff <= EPSILON_C}");
        }
    }
}
