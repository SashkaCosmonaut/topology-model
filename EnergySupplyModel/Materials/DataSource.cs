using EnergySupplyModel.Enumerations;

namespace EnergySupplyModel.Materials
{
    /// <summary>
    /// Класс с настройками источника данных.
    /// </summary>
    public class DataSource
    {
        /// <summary>
        /// Тип энергоресурса.
        /// </summary>
        public EnergyResourceType EnergyResourceType { get; set; }

        /// <summary>
        /// Интервал сбора данных из данного источника.
        /// </summary>
        public TimeInterval TimeInterval { get; set; }

        /// <summary>
        /// Наименование объекта.
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// Сравнить два источника данных.
        /// </summary>
        /// <param name="obj">Другой объект источника данных.</param>
        /// <returns>Одинаковые ли источники данных.</returns>
        public override bool Equals(object obj)
        {
            return obj is DataSource source &&
                   EnergyResourceType == source.EnergyResourceType &&
                   FacilityName == source.FacilityName &&
                   TimeInterval == source.TimeInterval;
        }
    }
}
