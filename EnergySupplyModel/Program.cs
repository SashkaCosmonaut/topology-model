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
            // Получаем измеренное и ожидаемое значения потребления и группируем их по типам энергоресурсов
            var measuredConsumption = facility.GetMeasuredConsumption(Params.Start, Params.End).GroupBy(q => q.DataSource.EnergyResourceType);
            var expectedConsumption = facility.GetExpectedConsumption(Params.Start, Params.End).GroupBy(q => q.DataSource.EnergyResourceType);

            foreach (var measuredEnergyResourceData in measuredConsumption)     // Перебираем данные по типам энергоресурсов
            {
                Console.WriteLine(measuredEnergyResourceData.Key.ToString() + ":");

                // Берём ожидаемые данные того же типа
                var expectedEnergyResourceData = expectedConsumption.SingleOrDefault(q => q.Key == measuredEnergyResourceData.Key);

                if (expectedEnergyResourceData == null)
                    continue;

                // Поскольку данные по каждому типу данных в одном экземпляре, берём их все вместе и суммируем значения
                var mesuredSum = measuredEnergyResourceData.SelectMany(q => q).Sum(q => q.Value.ItemValue);
                var expectedSum = expectedEnergyResourceData.SelectMany(q => q).Sum(q => q.Value.ItemValue);

                var exprectedDiff = mesuredSum - expectedSum;

                Console.WriteLine($"Expected: {expectedConsumption}, Measured: {measuredConsumption}, ExpectedDiff: {exprectedDiff}, Result: {exprectedDiff <= Params.EpsilonP}");
            }
        }

        /// <summary>
        /// Проверить, что суммарные значения подобъектов соответствуют действительному значению объекта.
        /// </summary>
        /// <param name="facility">Проверяемый объект предприятия.</param>
        protected static void CheckLeakDiff(Facility facility)
        {
            if (facility is ComplexFacility complexFacility)    // Проверяем только если объект является комплексным 
            {
                // Получаем измеренное и суммарное значения потребления и группируем их по типам энергоресурсов
                var measuredConsumption = facility.GetMeasuredConsumption(Params.Start, Params.End).GroupBy(q => q.DataSource.EnergyResourceType);
                var summaryConsumption = complexFacility.GetSummaryConsumption(Params.Start, Params.End).GroupBy(q => q.DataSource.EnergyResourceType);

                foreach (var measuredEnergyResourceData in measuredConsumption)     // Перебираем данные по типам энергоресурсов
                {
                    Console.WriteLine(measuredEnergyResourceData.Key.ToString() + ":");

                    // Берём суммируемые данные того же типа
                    var summaryEnergyResourceData = summaryConsumption.SingleOrDefault(q => q.Key == measuredEnergyResourceData.Key);

                    if (summaryEnergyResourceData == null)
                        continue;

                    // Поскольку данные по каждому типу данных в одном экземпляре, берём их все вместе и суммируем значения
                    var measuredSum = measuredEnergyResourceData.SelectMany(q => q).Sum(q => q.Value.ItemValue);
                    var summarySum = summaryEnergyResourceData.SelectMany(q => q).Sum(q => q.Value.ItemValue);

                    var leakDiff = measuredSum - summarySum;

                    Console.WriteLine($"Summary: {summarySum}, Measured: {measuredSum}, LeakDiff: {leakDiff}, Result: {leakDiff <= Params.EpsilonS}");
                }
            }
        }

        /// <summary>
        /// Проверить, что применяемые меры являются рентабельными.
        /// </summary>
        /// <param name="facility">Проверяемый объект предприятия.</param>
        protected static void CheckEffectDiff(Facility facility)
        {
            // Получаем ожидаемое и потенциальное значения потребления и группируем их по типам энергоресурсов
            var expectedConsumption = facility.GetExpectedConsumption(Params.Start, Params.End).GroupBy(q => q.DataSource.EnergyResourceType);
            var potentialConsumption = facility.GetPotentialConsumption(Params.Start, Params.End).GroupBy(q => q.DataSource.EnergyResourceType);

            foreach (var expectedEnergyResourceData in expectedConsumption)         // Перебираем данные по типам энергоресурсов
            {
                Console.WriteLine(expectedEnergyResourceData.Key.ToString() + ":");

                // Берём потенциальные данные того же типа
                var potentialEnergyResourceData = potentialConsumption.SingleOrDefault(q => q.Key == expectedEnergyResourceData.Key);

                if (potentialEnergyResourceData == null)
                    continue;

                // Рассчитываем ожидаемую и потенциальную стоимости
                var expectedCost = expectedEnergyResourceData.SelectMany(q => q).Sum(q => Params.EnergyResourceCost.Invoke(q.Value)) +
                                   expectedEnergyResourceData.SelectMany(q => q).Sum(q => Params.Penalty.Invoke(q.Value));

                var potentialCost = potentialEnergyResourceData.SelectMany(q => q).Sum(q => Params.EnergyResourceCost.Invoke(q.Value)) +
                                    potentialEnergyResourceData.SelectMany(q => q).Sum(q => Params.Penalty.Invoke(q.Value)) +
                                    Params.ActivityCost;

                var effectDiff = expectedCost - potentialCost;

                Console.WriteLine($"Expected cost: {expectedCost}, Potential cost: {potentialCost}, EffectDiff: {effectDiff}, Result: {effectDiff > Params.EpsilonC}");
            }
        }
    }
}
