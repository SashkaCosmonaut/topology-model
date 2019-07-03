using EnergySupplyModel.Materials;
using System;
using System.Collections.Generic;

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
        /// Функция рассчета ожидаемоего значения потребления энергоресурса с текущими характеристиками данного объекта.
        /// </summary>
        /// <param name="start">Начало периода измерения.</param>
        /// <param name="end">Конец периода измерения.</param>
        /// <returns>Данные потребления.</returns>
        public Data GetExpectedConsumption(DateTime start, DateTime end)
        {
            return null;
        }

        /// <summary>
        /// Функция рассчета потенциального значения потребления энергоресурса с применением мероприятий по оптимизации.
        /// </summary>
        /// <param name="start">Начало периода измерения.</param>
        /// <param name="end">Конец периода измерения.</param>
        /// <returns>Данные потребления.</returns>
        public Data GetPotentialConsumption(DateTime start, DateTime end)
        {
            return null;
        }

        /// <summary>
        /// Получить измеренное значение потребления энергоресурса со счётчика объекта.
        /// </summary>
        /// <param name="start">Начало периода измерения.</param>
        /// <param name="end">Конец периода измерения.</param>
        /// <returns>Данные потребления.</returns>
        public Data GetMeasuredConsumption(DateTime start, DateTime end)
        {
            return null;
        }
    }
}
