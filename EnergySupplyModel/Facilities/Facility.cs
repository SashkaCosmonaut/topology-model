using EnergySupplyModel.Enumerations;
using EnergySupplyModel.Input;
using EnergySupplyModel.Materials;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnergySupplyModel.Facilities
{
    /// <summary>
    /// Объектом предприятия является потребитель каких-либо энергоресурсов предприятия,
    /// который может производить один и более типов продукции.
    /// </summary>
    public class Facility
    {
        /// <summary>
        /// Наименование объекта.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Параметры данного объекта предприятия.
        /// </summary>
        public FacilityParameters Parameters { get; set; }

        /// <summary>
        /// Функция рассчета ожидаемоего значения потребления энергоресурса с текущими характеристиками данного объекта.
        /// </summary>
        /// <param name="dateTimeParameters">Параметры времени и даты для запроса данных.</param>
        /// <returns>Множество данных различного потребления.</returns>
        public virtual IEnumerable<DataSet> GetExpectedConsumption(InputDateTimeParameters dateTimeParameters)
        {
            if (Parameters == null)
                return new DataSet[] { };

            var energyResourceTypes = GetAllEnergyResourceTypes();

            // Для каждого типа потребляемых энергоресрсов создаём датасет
            var dataSets = energyResourceTypes.Select(energyResourceType => new DataSet
            {
                DataSource = new DataSource
                {
                    EnergyResourceType = energyResourceType,
                    FacilityName = Name,
                    TimeInterval = dateTimeParameters.Interval
                }
            }).ToArray();

            // Считаем количество элементов в датасете. TODO: Здесь должен быть switch по интервалу данных
            var numberOfDataItems = (int)dateTimeParameters.End.Subtract(dateTimeParameters.Start).TotalHours;

            // Формируем множество объектов дат за указанный период времени от start до end и перебираем его
            foreach (var dateTime in Enumerable.Range(0, numberOfDataItems).Select(hour => dateTimeParameters.Start.AddHours(hour)))
            {
                var productionConsumption = GetProductionConsumption(dateTime); // Получаем значение потребления в данный момент

                foreach (var dataSet in dataSets)   // Наполняем датасеты значениями соответствующих типов
                {
                    var energyResourceType = dataSet.DataSource.EnergyResourceType;

                    // Потребление состоит из постоянного и зависящего от производительности
                    var currentConstantConsumption = Parameters.ConstantConsumption.ContainsKey(energyResourceType)
                        ? Parameters.ConstantConsumption[energyResourceType]
                        : 0;

                    var currentProductionConsumption = productionConsumption.ContainsKey(energyResourceType)
                        ? productionConsumption[energyResourceType]
                        : 0;

                    dataSet.Add(dateTime, new DataItem
                    {
                        ItemValue = currentConstantConsumption + currentProductionConsumption,
                        TimeStamp = dateTime     // TODO: Тут ещё можно задавать какие-нибудь методанные или оценки качества данных
                    });
                }
            }

            return dataSets;
        }

        /// <summary>
        /// Получить все типы потребляемых энергоресурсов.
        /// </summary>
        /// <returns>Множество типов энерогоресурсов.</returns>
        protected IEnumerable<EnergyResourceType> GetAllEnergyResourceTypes()
        {
            if (Parameters == null)
                return new EnergyResourceType[] { };

            // Узнаем, какие энергоресурсы потребляются постоянно и перемено и суммируем
            var constantConsumptionTypes = Parameters.ConstantConsumption != null
                ? Parameters.ConstantConsumption.Keys.ToArray() 
                : new EnergyResourceType[] { };

            var productionConsumptionTypes = Parameters.Productivity != null 
                ? Parameters.Productivity.SelectMany(q => q.Value.Keys)
                : new EnergyResourceType[] { };

            return Enumerable.Concat(constantConsumptionTypes, productionConsumptionTypes).Distinct();
        }
    
        /// <summary>
        /// Получить значене потребляемых энергоресурсов для определенного момента времени.
        /// </summary>
        /// <param name="timeStamp">Момент времени.</param>
        /// <returns>Значение потребленного энергоресурса в момент времени.</returns>
        protected Dictionary<EnergyResourceType, double> GetProductionConsumption(DateTime timeStamp)
        {
            if (Parameters == null || Parameters.Productivity == null || 
                Parameters.ProductionPlan == null || !Parameters.Productivity.Any())
                    return new Dictionary<EnergyResourceType, double>();

            var productionVolumes = Parameters.ProductionPlan.Invoke(timeStamp);

            // Получаем все объемы потребленных энергоресурсов на производство изделий определённого количества
            var productionConsumption = productionVolumes.Select(production =>
                Parameters. Productivity[production.Key].ToDictionary(
                        consumption => consumption.Key, 
                        consumption => consumption.Value * production.Value));

            // Собираем все потребленные энергоресурсы в кучу (ключи могут повторяться), группируя по их типам и суммируем
            return productionConsumption
                .SelectMany(consumption => consumption)
                .ToLookup(consumption => consumption.Key, consumption => consumption.Value)
                .ToDictionary(consumption => consumption.Key, consumption => consumption.Sum());
        }

        /// <summary>
        /// Функция рассчета потенциального значения потребления энергоресурса с применением мероприятий по оптимизации.
        /// </summary>
        /// <param name="dateTimeParameters">Параметры времени и даты для запроса данных.</param>
        /// <returns>Данные потребления.</returns>
        public IEnumerable<DataSet> GetPotentialConsumption(InputDateTimeParameters dateTimeParameters)
        {
            return new DataSet[] { };
        }

        /// <summary>
        /// Получить измеренное значение потребления энергоресурса со счётчика объекта.
        /// </summary>
        /// <param name="dateTimeParameters">Параметры времени и даты для запроса данных.</param>
        /// <returns>Данные потребления.</returns>
        public IEnumerable<DataSet> GetMeasuredConsumption(InputDateTimeParameters dateTimeParameters)
        {
            var dataSources = new DataSource[]
            {
                new DataSource
                {
                    EnergyResourceType = EnergyResourceType.ColdWater,
                    FacilityName = Name,
                    TimeInterval = dateTimeParameters.Interval
                },
                new DataSource
                {
                    EnergyResourceType = EnergyResourceType.Electricity,
                    FacilityName = Name,
                    TimeInterval = dateTimeParameters.Interval
                }
            };

            return dataSources.Select(dataSource => DatabaseModel.GetMeasuredData(dataSource, dateTimeParameters));
        }
    }
}
