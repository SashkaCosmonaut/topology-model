using EnergySupplyModel.Enumerations;
using EnergySupplyModel.Facilities;
using System;

namespace EnergySupplyModel
{
    public class Program
    {
        /// <summary>
        /// Входные параметры для модели.
        /// </summary>
        public static InputParameters Params { get; set; } = new InputParameters();

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

            Console.WriteLine($"Expected: {expectedConsumption}, Measured: {measuredConsumption.Value}, ExpectedDiff: {exprectedDiff}, Result: {exprectedDiff <= Params.EpsilonP}");
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

                Console.WriteLine($"Summary: {summaryConsumption.Value}, Measured: {measuredConsumption.Value}, LeakDiff: {leakDiff}, Result: {leakDiff <= Params.EpsilonS}");
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

            var effectDiff = Params.EnergyResourceCost * (expectedConsumption - potentialConsumption) - Params.ActivityCost;

            Console.WriteLine($"Expected: {expectedConsumption}, Potential: {potentialConsumption}, EffectDiff: {effectDiff}, Result: {effectDiff <= Params.EpsilonC}");
        }
    }
}
