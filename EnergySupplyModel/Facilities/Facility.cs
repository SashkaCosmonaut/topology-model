using EnergySupplyModel.Enumerations;
using EnergySupplyModel.Input;
using EnergySupplyModel.Materials;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnergySupplyModel.Facilities
{
    /// <summary>
    /// Объектом предприятия является потребитель каких-либо энергоресурсов предприятия, который может производить один и более типов продукции.
    /// </summary>
    public class Facility
    {
        /// <summary>
        /// Наименование объекта.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Постоянное потребление данного объекте в виде словаря, где ключ - тип энергоресурса, а значение - объем потребления.
        /// </summary>
        public Dictionary<EnergyResourceType, double> ConstantConsumption { get; set; }

        /// <summary>
        /// Производительность объекта, которая задаётся как словарь, в котором ключ - это тип продукции, а значение - 
        /// это словарь, в котором ключ - это тип энергоресурса, а значение - объем его потребления для изготовления единицы продукции.
        /// </summary>
        public Dictionary<ProductType, Dictionary<EnergyResourceType, double>> Productivity { get; set; }

        /// <summary>
        /// Функция рассчета ожидаемоего значения потребления энергоресурса с текущими характеристиками данного объекта.
        /// </summary>
        /// <param name="parameters">Параметры времени и даты для запроса данных.</param>
        /// <returns>Множество данных различного потребления.</returns>
        public virtual IEnumerable<DataSet> GetExpectedConsumption(InputDateTimeParameters parameters)
        {
            // Для каждого типа постоянного потребления энергоресрсов создаём датасет
            var dataSets = ConstantConsumption.Keys.Select(energyResourceType => new DataSet
            {
                DataSource = new DataSource
                {
                    EnergyResourceType = energyResourceType,
                    FacilityName = Name,
                    TimeInterval = parameters.Interval
                }
            }).ToArray();

            // Считаем количество элементов в датасете
            var numberOfDataItems = (int)parameters.End.Subtract(parameters.Start).TotalHours;   // Здесь должен быть switch по интервалу данных

            foreach (var dataSet in dataSets)   // Наполняем датасет заданными значениями
            {
                // Формируем множество объектов дат за указанный период времени от start до end и перебираем его
                foreach (var dateTime in Enumerable.Range(0, numberOfDataItems).Select(hour => parameters.Start.AddHours(hour)))
                {
                    dataSet.Add(dateTime, new DataItem
                    {
                        // Потребление состоит из постоянного и зависящего от производительности 
                        ItemValue = ConstantConsumption[dataSet.DataSource.EnergyResourceType] + GetProductionConsumption(dataSet.DataSource.EnergyResourceType, dateTime),
                        TimeStamp = dateTime
                    });
                }
            }

            return dataSets;
        }
    
        /// <summary>
        /// Получить значене потребленного энергоресурса для определенного момента времени
        /// </summary>
        /// <param name="energyResourceType">Тип энергоресурса.</param>
        /// <param name="timeStamp">Момент времени.</param>
        /// <returns>Значение потребленного энергоресурса в момент времени.</returns>
        protected double GetProductionConsumption(EnergyResourceType energyResourceType, DateTime timeStamp)
        {
            if (Productivity == null || !Productivity.Any())
                return 0;

            // В плане надо задавать в какой момент времени какие изделия производим и в каких количествах (надо домножить)
            return Productivity.Values.SelectMany(q => q).Where(q => q.Key == energyResourceType).Sum(q => q.Value);
        }

        /// <summary>
        /// Функция рассчета потенциального значения потребления энергоресурса с применением мероприятий по оптимизации.
        /// </summary>
        /// <param name="parameters">Параметры времени и даты для запроса данных.</param>
        /// <returns>Данные потребления.</returns>
        public IEnumerable<DataSet> GetPotentialConsumption(InputDateTimeParameters parameters)
        {
            return new DataSet[] { };
        }

        /// <summary>
        /// Получить измеренное значение потребления энергоресурса со счётчика объекта.
        /// </summary>
        /// <param name="parameters">Параметры времени и даты для запроса данных.</param>
        /// <returns>Данные потребления.</returns>
        public IEnumerable<DataSet> GetMeasuredConsumption(InputDateTimeParameters parameters)
        {
            var dataSources = new DataSource[]
            {
                new DataSource
                {
                    EnergyResourceType = EnergyResourceType.ColdWater,
                    FacilityName = Name,
                    TimeInterval = parameters.Interval
                },
                new DataSource
                {
                    EnergyResourceType = EnergyResourceType.Electricity,
                    FacilityName = Name,
                    TimeInterval = parameters.Interval
                }
            };

            return dataSources.Select(dataSource => DatabaseModel.GetMeasuredData(dataSource, parameters));
        }
    }
}
