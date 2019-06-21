using EnergySupplyModel.Enumerations;
using EnergySupplyModel.Materials;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnergySupplyModel
{
    public class Program
    {
        /// <summary>
        /// Производство продукции осуществляется предприятием, в которое входят объекты.
        /// </summary>
        public static Manufacturing Factory { get; set; }

        /// <summary>
        /// Для каждого энергоресурса задан тариф - стоимость его потребления в определенные моменты времени
        /// </summary>
        public static EnergyResourceCost[] EnergyResourceCosts { get; set; }

        /// <summary>
        /// Для предприятия задаётся план производства - в какое время в каких объемах нужно производить какую продукцию.
        /// TODO: добавить, каким объектом
        /// </summary>
        public static ManufacturedProduction[] ProductionPlan { get; set; }

        /// <summary>
        /// Главная функция программы.
        /// </summary>
        /// <param name="args">Аргументы командной строки.</param>
        public static void Main(string[] args)
        {
            Init();

            var totalConsumption = Factory.Manufacture(ProductionPlan);

            var totalCost = 0.0;

            foreach (var totalConsumptionGroup in totalConsumption.GroupBy(q => q.Type))
            {
                var typeCosts = EnergyResourceCosts.SingleOrDefault(q => q.EnergyResourceType == totalConsumptionGroup.Key).Costs;

                var consumptions = totalConsumptionGroup.Select(q => q.ConsumedVolumes);

                foreach (var consumption in consumptions)
                {

                }
            }

        }

        /// <summary>
        /// Инициализация параметров программы.
        /// </summary>
        public static void Init()
        {
            Factory = new Manufacturing
            {
                Name = "Тестовый завод",
                Subfacilities = new Facility[]
                {

                }
            };

            EnergyResourceCosts = new EnergyResourceCost[]
            {
                new EnergyResourceCost
                {
                    EnergyResourceType = EnergyResourceType.Test,
                    Costs = new Dictionary<TimeSpan, int>
                    {
                        { new TimeSpan(0, 0, 0), 0 }
                    }
                }
            };

            ProductionPlan = new ManufacturedProduction[]
            {
                new ManufacturedProduction
                {
                    Type = ProductType.Test,
                    PlannedVolumes = new Dictionary<TimeSpan, int>
                    {
                        { new TimeSpan(0, 0, 0), 0 }
                    }
                }
            };
        }
    }
}
