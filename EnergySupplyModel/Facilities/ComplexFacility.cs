using EnergySupplyModel.Input;
using EnergySupplyModel.Materials;
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
        /// <param name="parameters">Параметры времени и даты для запроса данных.</param>
        /// <returns>Данные потребления.</returns>
        public IEnumerable<DataSet> GetSummaryConsumption(InputDateTimeParameters parameters)
        {
            var result = new List<DataSet>();

            if (Subfacilities == null)
                return result;

            var measuredConsumptions = Subfacilities.SelectMany(subFacility => GetSubfacilityData(subFacility, parameters));

            if (!measuredConsumptions.Any())
                return result;

            var energyResourceGroups = measuredConsumptions.GroupBy(q => q.DataSource.EnergyResourceType);

            foreach (var energyResourceGroup in energyResourceGroups)
            {
                var newData = new DataSet
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

        /// <summary>
        /// Получить данные от подобъекта в зависимости от того, простой он или составной.
        /// </summary>
        /// <param name="facility">Объект, данные которого получаем.</param>
        /// <param name="parameters">Параметры времени и даты для запроса данных.</param>
        /// <returns>Данные потребления.</returns>
        protected IEnumerable<DataSet> GetSubfacilityData(Facility facility, InputDateTimeParameters parameters)
        {
            if (facility is ComplexFacility compexFacility)
                return compexFacility.GetSummaryConsumption(parameters);

            return facility.GetMeasuredConsumption(parameters);
        }

        /// <summary>
        /// Функция рассчета ожидаемоего значения потребления энергоресурса составного объекта.
        /// </summary>
        /// <param name="parameters">Параметры времени и даты для запроса данных.</param>
        /// <returns>Множество данных различного потребления.</returns>
        public override IEnumerable<DataSet> GetExpectedConsumption(InputDateTimeParameters parameters)
        {
            var currentExpected = base.GetExpectedConsumption(parameters);  // Получаем ожидаемое потребление текущего объекта

            // Собираем все типы нергоресурсов, которые есть у этого объекта
            var energyResourceTypes = currentExpected.Select(q => q.DataSource.EnergyResourceType);

            // Собираем в общую кучу данные со всех подобъектов
            var subfacilitiesData = Subfacilities.SelectMany(q => q.GetExpectedConsumption(parameters));

            foreach (var energyResourceType in energyResourceTypes) // Перебирем все типы энергоресурсов
            {
                // Выбираем из данных подобектов данные только текущего типа энергоресурсов
                var subfacilitiesEnergyResourceData = subfacilitiesData.Where(q => q.DataSource.EnergyResourceType == energyResourceType);

                // Берём ожидаемое потребление текущего объекта текущего типа
                var currentExpectedEnergyResourceData = currentExpected.SingleOrDefault(q => q.DataSource.EnergyResourceType == energyResourceType);

                foreach (var item in currentExpectedEnergyResourceData) // Добавляем значения вложенных объектов
                {
                    item.Value.ItemValue += subfacilitiesEnergyResourceData.Sum(q => q[item.Key].ItemValue);
                }
            }

            return currentExpected;
        }
    }
}
