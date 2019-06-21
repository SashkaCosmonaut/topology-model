using EnergySupplyModel.Materials;
using System.Collections.Generic;
using System.Linq;

namespace EnergySupplyModel
{
    /// <summary>
    /// Объектом предприятия является потребитель каких-либо энергоресурсов предприятия, который может производить один и более типов продукции.
    /// </summary>
    public abstract class Facility
    {
        /// <summary>
        /// Наименование объекта.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Совокупность вложенных в него таких же подобъектов-потребителей.
        /// </summary>
        public IEnumerable<Facility> Subfacilities { get; set; }

        /// <summary>
        /// Произвести продукцию на этом и вложенном подобъектах по требованию.
        /// </summary>
        /// <param name="requredProducts">Планируемая для производства продукция.</param>
        /// <returns>Затраченные энергоресурсы.</returns>
        public IEnumerable<EnergyResourceConsumption> Manufacture(IEnumerable<ManufacturedProduction> requredProducts)
        {
            // Результат производства - сколько энергоресурсов потреблено на этом объекте + сумма того, что произведено на подобъектах
            return Enumerable.Concat(
                ManufactureLocal(requredProducts),
                Subfacilities == null                   // Если подобъектов нет, то от них ничего не берём
                    ? new EnergyResourceConsumption[] { }
                    : Subfacilities.SelectMany(q => q.Manufacture(requredProducts)));
        }

        /// <summary>
        /// Произвести продукцию только на этом объекте по требованию.
        /// </summary>
        /// <param name="requiredProducts">Планируемая для производства продукция.</param>
        /// <returns>Затраченные энергоресурсы.</returns>
        protected abstract IEnumerable<EnergyResourceConsumption> ManufactureLocal(IEnumerable<ManufacturedProduction> requiredProducts);
    }
}
