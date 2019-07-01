using EnergySupplyModel.Enumerations;
using System;
using System.Collections.Generic;

namespace EnergySupplyModel.Materials
{
    /// <summary>
    /// Производимая продукция.
    /// </summary>
    public class ManufacturedProduction
    {
        /// <summary>
        /// Тип (наименование) производимой продкции.
        /// </summary>
        public ProductType Type { get; set; }

        /// <summary>
        /// Требуемые объемы производимой продукции в соответствии с планом, штук в час.
        /// </summary>
        public Dictionary<TimeSpan, int> PlannedVolumes { get; set; }
    }
}
