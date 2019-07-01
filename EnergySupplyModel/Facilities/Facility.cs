using System;

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
        public Func<double> GetExpectedConsumption { get; set; }

        /// <summary>
        /// Функция рассчета потенциального значения потребления энергоресурса с применением мероприятий по оптимизации.
        /// </summary>
        public Func<double> GetPotentialConsumption { get; set; }

        /// <summary>
        /// Получить измеренное значение потребления энергоресурса со счётчика объекта.
        /// </summary>
        /// <returns>Значение объема потребления.</returns>
        public double? GetMeasuredConsumption()
        {
            return DatabaseModel.GetData(Name);
        }
    }
}
