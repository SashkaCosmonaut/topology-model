using EnergySupplyModel.Facilities;
using System;
using System.Linq;

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
            Check(Params.Factory);
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
            var measuredConsumption = facility.GetMeasuredConsumption(Params.Start, Params.End).Sum(q => q.Value.ItemValue);

            var expectedConsumption = facility.GetExpectedConsumption(Params.Start, Params.End).Sum(q => q.Value.ItemValue);

            var exprectedDiff = measuredConsumption - expectedConsumption;

            Console.WriteLine($"Expected: {expectedConsumption}, Measured: {measuredConsumption}, ExpectedDiff: {exprectedDiff}, Result: {exprectedDiff <= Params.EpsilonP}");
        }

        /// <summary>
        /// Проверить, что суммарные значения подобъектов соответствуют действительному значению объекта.
        /// </summary>
        /// <param name="facility">Проверяемый объект предприятия.</param>
        protected static void CheckLeakDiff(Facility facility)
        {
            var measuredConsumption = facility.GetMeasuredConsumption(Params.Start, Params.End).Sum(q => q.Value.ItemValue);

            if (facility is ComplexFacility complexFacility)
            {
                var summaryConsumption = complexFacility.GetSummaryConsumption(Params.Start, Params.End).Sum(q => q.Value);

                var leakDiff = measuredConsumption - summaryConsumption;

                Console.WriteLine($"Summary: {summaryConsumption}, Measured: {measuredConsumption}, LeakDiff: {leakDiff}, Result: {leakDiff <= Params.EpsilonS}");
            }
        }

        /// <summary>
        /// Проверить, что применяемые меры являются рентабельными.
        /// </summary>
        /// <param name="facility">Проверяемый объект предприятия.</param>
        protected static void CheckEffectDiff(Facility facility)
        {
            var expectedConsumption = facility.GetExpectedConsumption(Params.Start, Params.End);

            var potentialConsumption = facility.GetPotentialConsumption(Params.Start, Params.End);

            var expectedCost = expectedConsumption.Select(q => Params.EnergyResourceCost.Invoke(q.Value)).Sum() +
                               expectedConsumption.Select(q => Params.Penalty.Invoke(q.Value)).Sum();

            var potentialCost = potentialConsumption.Select(q => Params.EnergyResourceCost.Invoke(q.Value)).Sum() +
                                potentialConsumption.Select(q => Params.Penalty.Invoke(q.Value)).Sum() +
                                Params.ActivityCost;

            var effectDiff = expectedCost - potentialCost;

            Console.WriteLine($"Expected cost: {expectedCost}, Potential cost: {potentialCost}, EffectDiff: {effectDiff}, Result: {effectDiff > Params.EpsilonC}");
        }
    }
}
