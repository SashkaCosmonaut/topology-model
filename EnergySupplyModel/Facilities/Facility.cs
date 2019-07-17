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
        /// Текущие параметры данного объекта предприятия.
        /// </summary>
        public FacilityParameters CurrentParameters { get; set; }

        /// <summary>
        /// Примененное к данному объекту мероприятие с обновленными параметрами
        /// данного объекта предприятия после выполнения мероприятий.
        /// </summary>
        public Measure AppliedMeasure { get; set; }

        /// <summary>
        /// Функция расчета ожидаемого значения потребления энергоресурсов с текущими характеристиками данного объекта.
        /// </summary>
        /// <param name="dateTimeParameters">Параметры времени и даты для запроса данных.</param>
        /// <returns>Множество данных различного потребления.</returns>
        public virtual IEnumerable<DataSet> GetExpectedConsumption(InputDateTimeParameters dateTimeParameters)
        {
            return GetConsumptionByParameters(dateTimeParameters, CurrentParameters, Name);
        }

        /// <summary>
        /// Функция расчета потенциального значения потребления энергоресурсов с применением мероприятий по оптимизации.
        /// </summary>
        /// <param name="dateTimeParameters">Параметры времени и даты для запроса данных.</param>
        /// <returns>Данные потребления.</returns>
        public IEnumerable<DataSet> GetPotentialConsumption(InputDateTimeParameters dateTimeParameters)
        {
            if (AppliedMeasure == null || AppliedMeasure.NewFacilityParameters == null)
                return new DataSet[] { };

            // Если в новых параметрах какие-то параметры не заданы, используем параметры по умолчанию
            AppliedMeasure.NewFacilityParameters.FillMissingData(CurrentParameters);

            return GetConsumptionByParameters(dateTimeParameters, AppliedMeasure.NewFacilityParameters, Name);
        }

        /// <summary>
        /// Получить потребление некоторого объекта в соответствии с параметрами.
        /// </summary>
        /// <param name="dateTimeParameters">Временные параметры запроса получения данных.</param>
        /// <param name="facilityParameters">Параметры объекта предприятия.</param>
        /// <param name="facilityName">Наименование объекта предприятия.</param>
        /// <returns>Множество данных различного типа потребления.</returns>
        protected static IEnumerable<DataSet> GetConsumptionByParameters(InputDateTimeParameters dateTimeParameters, FacilityParameters facilityParameters, string facilityName)
        {
            if (facilityParameters == null)     // Без параметров предприятия делать нечего
                return new DataSet[] { };

            var energyResourceTypes = GetAllEnergyResourceTypes(dateTimeParameters, facilityParameters);

            // Для каждого типа потребляемых энергоресрсов создаём датасет
            var dataSets = energyResourceTypes.Select(energyResourceType => new DataSet
            {
                DataSource = new DataSource
                {
                    EnergyResourceType = energyResourceType,
                    FacilityName = facilityName,
                    TimeInterval = dateTimeParameters.Interval
                }
            }).ToArray();

            // Считаем количество элементов в датасете. TODO: Здесь должен быть switch по интервалу данных
            var numberOfDataItems = (int)dateTimeParameters.End.Subtract(dateTimeParameters.Start).TotalHours;

            // Формируем множество объектов дат за указанный период времени от start до end и перебираем его
            foreach (var dateTime in Enumerable.Range(0, numberOfDataItems).Select(hour => dateTimeParameters.Start.AddHours(hour)))
            {
                var productionConsumption = GetProductionConsumption(dateTime, facilityParameters); // Получаем значение потребления в данный момент

                foreach (var dataSet in dataSets)   // Наполняем датасеты значениями соответствующих типов
                {
                    var energyResourceType = dataSet.DataSource.EnergyResourceType;

                    var constantConsumption = facilityParameters.ConstantConsumption.Invoke(dateTime);

                    // Потребление состоит из постоянного и зависящего от производительности
                    var currentConstantConsumption = constantConsumption.ContainsKey(energyResourceType)
                        ? constantConsumption[energyResourceType]
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
        /// <param name="dateTimeParameters">Временные параметры запроса получения данных.</param>
        /// <param name="facilityParameters">Параметры объекта предприятия.</param>
        /// <returns>Множество типов энерогоресурсов.</returns>
        protected static IEnumerable<EnergyResourceType> GetAllEnergyResourceTypes(InputDateTimeParameters dateTimeParameters, FacilityParameters facilityParameters)
        {
            if (facilityParameters == null)
                return new EnergyResourceType[] { };

            // Узнаем, какие энергоресурсы потребляются постоянно и перемено
            var constantConsumptionTypes = facilityParameters.ConstantConsumption != null
                ? facilityParameters.ConstantConsumption.Invoke(dateTimeParameters.Start).Keys.ToArray() 
                : new EnergyResourceType[] { };

            var productionConsumptionTypes = facilityParameters.Productivity != null 
                ? facilityParameters.Productivity.SelectMany(q => q.Value.Keys)
                : new EnergyResourceType[] { };

            // Объединяем множества и удаляем повторения
            return Enumerable.Concat(constantConsumptionTypes, productionConsumptionTypes).Distinct();
        }
    
        /// <summary>
        /// Получить значене потребляемых энергоресурсов для определенного момента времени.
        /// </summary>
        /// <param name="timeStamp">Момент времени.</param>
        /// <returns>Значение потребленного энергоресурса в момент времени.</returns>
        protected static Dictionary<EnergyResourceType, double> GetProductionConsumption(DateTime timeStamp, FacilityParameters facilityParameters)
        {
            if (facilityParameters == null || facilityParameters.Productivity == null ||
                facilityParameters.ProductionPlan == null || !facilityParameters.Productivity.Any())
                    return new Dictionary<EnergyResourceType, double>();

            var productionVolumes = facilityParameters.ProductionPlan.Invoke(timeStamp);

            // Получаем все объемы потребленных энергоресурсов на производство изделий определённого количества
            var productionConsumption = productionVolumes.Select(production =>
                facilityParameters. Productivity[production.Key].ToDictionary(
                        consumption => consumption.Key, 
                        consumption => consumption.Value * production.Value));

            // Собираем все потребленные энергоресурсы в кучу (ключи могут повторяться), группируя по их типам и суммируем
            return productionConsumption
                .SelectMany(consumption => consumption)
                .ToLookup(consumption => consumption.Key, consumption => consumption.Value)
                .ToDictionary(consumption => consumption.Key, consumption => consumption.Sum());
        }

        /// <summary>
        /// Получить измеренное значение потребления энергоресурсов со счётчика объекта.
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
