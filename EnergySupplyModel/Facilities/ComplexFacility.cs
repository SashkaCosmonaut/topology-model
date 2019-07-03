using System;
using System.Collections.Generic;
using System.Linq;

namespace EnergySupplyModel.Facilities
{
    public class ComplexFacility : Facility
    {
        /// <summary>
        /// Совокупность вложенных в него таких же подобъектов-потребителей.
        /// </summary>
        public IEnumerable<Facility> Subfacilities { get; set; }

        /// <summary>
        /// Суммарное значение потребления энергоресурса вложенными подобъектами данного объекта.
        /// </summary>
        /// <param name="start">Начало периода измерения.</param>
        /// <param name="end">Конец периода измерения.</param>
        /// <returns>Словарь данных потребления.</returns>
        public Dictionary<DateTime, double> GetSummaryConsumption(DateTime start, DateTime end)
        {
            var result = new Dictionary<DateTime, double>();

            if (Subfacilities == null)
                return result;

            var measuredConsumptions = Subfacilities.Select(q => q.GetMeasuredConsumption(start, end));

            if (!measuredConsumptions.Any())
                return result;

            foreach (var dataItem in measuredConsumptions.First())
                result.Add(dataItem.Key, measuredConsumptions.Sum(q => q[dataItem.Key]));

            return result;
        }
    }
}
