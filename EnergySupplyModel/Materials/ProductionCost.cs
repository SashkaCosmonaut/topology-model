using EnergySupplyModel.Enumerations;
using System.Collections.Generic;

namespace EnergySupplyModel.Materials
{
    /// <summary>
    /// Затраты энергоресурсов различных типов для производства единицы продукции определённого типа.
    /// </summary>
    public class ProductionCost
    {
        /// <summary>
        /// Тип (наименование) производимой продкции.
        /// </summary>
        public ProductType Type { get; set; }

        /// <summary>
        /// Требуемые затраты производимой продукции в виде словаря, в котором ключ - это тип
        /// энергоресурса, а значение - объем его потребления для изготовления единицы продукции.
        /// </summary>
        public Dictionary<EnergyResourceType, double> EnergyResources { get; set; }
    }
}
