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
        public EnergyResourceType Type { get; set; }

        /// <summary>
        /// Словарь, где ключ - время дня, а значение - стоимость энергоресурса (тариф на указанный час).
        /// </summary>
        public Dictionary<TimeSpan, int> Costs { get; set; }
    }
}
