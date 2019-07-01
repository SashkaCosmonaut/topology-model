using System.Collections.Generic;
using System.Linq;

namespace EnergySupplyModel.Facilities
{
    public abstract class ComplexFacility : Facility
    {
        /// <summary>
        /// Совокупность вложенных в него таких же подобъектов-потребителей.
        /// </summary>
        public IEnumerable<Facility> Subfacilities { get; set; }

        /// <summary>
        /// Суммарное значение потребления энергоресурса вложенными подобъектами данного объекта.
        /// </summary>
        public double? GetSummaryConsumption()
        {
            if (Subfacilities == null)
                return null;

            var measuredConsumptions = Subfacilities.Select(q => q.GetMeasuredConsumption());

            if (measuredConsumptions.Count() == 0 || measuredConsumptions.Any(q => !q.HasValue))
                return null;

            return measuredConsumptions.Sum(q => q.Value);
        }
    }
}
