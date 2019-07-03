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
    }
}
