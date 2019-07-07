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
        public Dictionary<EnergyResourceType, double> СonstantСonsumption { get; set; }

        /// <summary>
        /// Функция рассчета ожидаемоего значения потребления энергоресурса с текущими характеристиками данного объекта.
        /// </summary>
        /// <param name="parameters">Параметры времени и даты для запроса данных.</param>
        /// <returns>Множество данных различного потребления.</returns>
        public IEnumerable<DataSet> GetExpectedConsumption(InputDateTimeParameters parameters)
        {
            return new DataSet[] { };
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
