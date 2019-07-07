using EnergySupplyModel.Input;
using EnergySupplyModel.Materials;
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
        /// <param name="parameters">Параметры времени и даты для запроса данных.</param>
        /// <returns>Словарь данных потребления.</returns>
        public IEnumerable<Data> GetSummaryConsumption(InputDateTimeParameters parameters)
        {
            var result = new List<Data>();

            if (Subfacilities == null)
                return result;

            var measuredConsumptions = Subfacilities.SelectMany(q => q.GetMeasuredConsumption(parameters));

            if (!measuredConsumptions.Any())
                return result;

            var energyResourceGroups = measuredConsumptions.GroupBy(q => q.DataSource.EnergyResourceType);

            foreach (var energyResourceGroup in energyResourceGroups)
            {
                var newData = new Data
                {
                    DataSource = new DataSource
                    {
                        EnergyResourceType = energyResourceGroup.Key,
                        FacilityName = Name,
                        TimeInterval = parameters.Interval  // Здесь должно быть приведение данных к одному интервалу
                    }
                };

                foreach (var dataItem in energyResourceGroup.First())
                    newData.Add(dataItem.Key, new DataItem
                    {
                        // Здесь также должно быть объединение метаданных и расчёт общих оценок качества данных
                        ItemValue = energyResourceGroup.Sum(q => q[dataItem.Key].ItemValue),
                        TimeStamp = dataItem.Key
                    });

                result.Add(newData);
            }

            return result;
        }
    }
}
