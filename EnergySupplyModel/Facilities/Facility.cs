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
        /// Функция задания производственного плана для текущего объекта предприятия, которая возвращает количество
        /// произведённых единиц продукции различных типов в определённый момент времени. Для составного объекта может не задаваться.
        /// Может браться из файла или БД, аналогично тому, как берутся данные потребления со средств измерения.
        /// </summary>
        public Func<DateTime, Dictionary<ProductType, int>> ProductionPlan { get; set; }

        /// <summary>
        /// Функция рассчета ожидаемоего значения потребления энергоресурса с текущими характеристиками данного объекта.
        /// </summary>
        /// <param name="parameters">Параметры времени и даты для запроса данных.</param>
        /// <returns>Множество данных различного потребления.</returns>
        public virtual IEnumerable<DataSet> GetExpectedConsumption(InputDateTimeParameters parameters)
        {
            var energyResourceTypes = GetAllEnergyResourceTypes();

            // Для каждого типа потребляемых энергоресрсов создаём датасет
            var dataSets = energyResourceTypes.Select(energyResourceType => new DataSet
            {
                DataSource = new DataSource
                {
                    EnergyResourceType = energyResourceType,
                    FacilityName = Name,
                    TimeInterval = parameters.Interval
                }
            }).ToArray();

            // Считаем количество элементов в датасете
            var numberOfDataItems = (int)parameters.End.Subtract(parameters.Start).TotalHours;   // TODO: Здесь должен быть switch по интервалу данных

            // Формируем множество объектов дат за указанный период времени от start до end и перебираем его
            foreach (var dateTime in Enumerable.Range(0, numberOfDataItems).Select(hour => parameters.Start.AddHours(hour)))
            {
                var productionConsumption = GetProductionConsumption(dateTime); // Получаем значение потребления в данный момент

                foreach (var dataSet in dataSets)   // Наполняем датасеты значениями соответствующих типов
                {
                    var energyResourceType = dataSet.DataSource.EnergyResourceType;

                    // Потребление состоит из постоянного и зависящего от производительности
                    var currentConstantConsumption = ConstantConsumption.ContainsKey(energyResourceType) ? ConstantConsumption[energyResourceType] : 0;

                    var currentProductionConsumption = productionConsumption.ContainsKey(energyResourceType) ? productionConsumption[energyResourceType] : 0;

                    dataSet.Add(dateTime, new DataItem
                    {
                        ItemValue = currentConstantConsumption + currentProductionConsumption,
                        TimeStamp = dateTime        // TODO: Тут ещё можно задавать какие-нибудь методанные или оценки качества данных
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
            // Узнаем, какие энергоресурсы потребляются постоянно и перемено и суммируем
            var constantConsumptionTypes = ConstantConsumption != null ? ConstantConsumption.Keys.ToArray() : new EnergyResourceType[] { };
            var productionConsumptionTypes = Productivity != null ? Productivity.SelectMany(q => q.Value.Keys) : new EnergyResourceType[] { };

            return Enumerable.Concat(constantConsumptionTypes, productionConsumptionTypes).Distinct();
        }
    
        /// <summary>
        /// Получить значене потребляемых энергоресурсов для определенного момента времени.
        /// </summary>
        /// <param name="timeStamp">Момент времени.</param>
        /// <returns>Значение потребленного энергоресурса в момент времени.</returns>
        protected Dictionary<EnergyResourceType, double> GetProductionConsumption(DateTime timeStamp)
        {
            if (Productivity == null || ProductionPlan == null || !Productivity.Any())
                return new Dictionary<EnergyResourceType, double>();

            var productionVolumes = ProductionPlan.Invoke(timeStamp);

            // Получаем все объемы потребленных энергоресурсов на производство изделий определённого количества
            var productionConsumption = productionVolumes.Select(production =>
                    Productivity[production.Key].ToDictionary(
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
