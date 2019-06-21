using System.Collections.Generic;
using EnergySupplyModel.Materials;

namespace EnergySupplyModel
{
    /// <summary>
    /// Производство продукции осуществляется целевой системой - системой производства. В систему производства входят объекты предприятия.
    /// На самом верхнем уровне в качестве объекта рассматривается всё предприятие в целом, которое разбивается на совокупность подобъектов: 
    /// территории, здания, цеха, участки, вплоть до отдельного станка или единицы оборудования.
    /// </summary>
    public class Manufacturing : Facility
    {
        /// <summary>
        /// Произвести продукцию только на этом объекте по требованию.
        /// </summary>
        /// <param name="requiredProducts">Планируемая для производства продукция.</param>
        /// <returns>Затраченные энергоресурсы.</returns>
        protected override IEnumerable<EnergyResourceConsumption> ManufactureLocal(IEnumerable<ManufacturedProduction> requiredProducts)
        {
            // Само по себе предприятие ничего не потребляет, только вложенные в него объекты.
            return new EnergyResourceConsumption[] { };
        }
    }
}
