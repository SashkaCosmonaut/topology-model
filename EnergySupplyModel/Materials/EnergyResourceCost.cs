using EnergySupplyModel.Enumerations;
using System;
using System.Collections.Generic;

namespace EnergySupplyModel.Materials
{
    /// <summary>
    /// Для каждого энергоресурса задан тариф - стоимость его потребления в определенные моменты времени.
    /// </summary>
    public class EnergyResourceCost
    {
        /// <summary>
        /// Тип потребляемого энергоресурса.
        /// </summary>
        public EnergyResourceType EnergyResourceType { get; set; }

        /// <summary>
        /// Словарь, где ключ - время дня, а значение - стоимость энергоресурса (тариф).
        /// </summary>
        public Dictionary<TimeSpan, int> Costs { get; set; }
    }
}
