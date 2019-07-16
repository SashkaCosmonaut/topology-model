using EnergySupplyModel.Enumerations;
using EnergySupplyModel.Facilities;
using EnergySupplyModel.Materials;
using System;
using System.Collections.Generic;

namespace EnergySupplyModel.Input
{
    /// <summary>
    /// Класс входных параметров для модели.
    /// </summary>
    public class InputParameters
    {
        /// <summary>
        /// Порог выявления проблемы.
        /// </summary>
        public double EpsilonP = 100;

        /// <summary>
        /// Порог выявления утечки.
        /// </summary>
        public double EpsilonS = 100;

        /// <summary>
        /// Порог выявления рентабельности меропритий.
        /// </summary>
        public double EpsilonC = 800;

        /// <summary>
        /// Параметры времени и даты для модели. 
        /// </summary>
        public InputDateTimeParameters DateTimeParams { get; set; } = new InputDateTimeParameters();

        /// <summary>
        /// Множество всех мероприятий, которые применяются на предприятии.
        /// </summary>
        public Measure[] Measures { get; set; }

        /// <summary>
        /// Функция расчета стоимости энергоресурса в укзанный момент времени и для указанного энергоресурса.
        /// </summary>
        public Func<DataItem, DataSource, double> EnergyResourceCost { get; set; } = (dataItem, dataSource) =>
        {
            var result = dataItem.ItemValue;

            switch (dataSource.EnergyResourceType)
            {
                case EnergyResourceType.Electricity:
                    if (dataItem.TimeStamp.DayOfWeek == DayOfWeek.Saturday || dataItem.TimeStamp.DayOfWeek == DayOfWeek.Sunday)
                        return result * 5;

                    if (dataItem.TimeStamp.TimeOfDay.Hours < 6 || dataItem.TimeStamp.TimeOfDay.Hours > 20)
                        return result * 5;

                    return result * 10;

                case EnergyResourceType.ColdWater:
                    return result * 5;

                default:
                    return result;
            }
        };

        /// <summary>
        /// Функция расчета штрафа за превышение объема потребления в указанный момент времени и для указанного энергоресурса.
        /// </summary>
        public Func<DataItem, DataSource, double> Penalty { get; set; } = (dataItem, dataSource) =>
        {
            switch (dataSource.EnergyResourceType)
            {
                case EnergyResourceType.Electricity:
                    return dataItem.ItemValue > 100 ? 100 : 0;

                case EnergyResourceType.ColdWater:
                    return dataItem.ItemValue > 1000 ? 100 : 0;

                default:
                    return 0;
            }
        };

        /// <summary>
        /// Производство продукции осуществляется целевой системой - системой производства. В систему производства входят объекты предприятия.
        /// На самом верхнем уровне в качестве объекта рассматривается всё предприятие в целом, которое разбивается на совокупность подобъектов: 
        /// территории, здания, цеха, участки, вплоть до отдельного станка или единицы оборудования.
        /// </summary>
        public ComplexFacility Factory { get; set; }

        /// <summary>
        /// Инициализация входных параметров.
        /// </summary>
        public InputParameters()
        {
            // Постоянное потребление объектов
            var constantСonsumption = new Dictionary<EnergyResourceType, double>
            {
                { EnergyResourceType.ColdWater, 1 },
                { EnergyResourceType.Electricity, 1 }
            };

            // Производительность объектов - кол-во затрачиваемых ресурсов для изготовления единиц продукции
            var productivity = new Dictionary<ProductType, Dictionary<EnergyResourceType, double>>
            {
                { ProductType.Bolt, new Dictionary<EnergyResourceType, double> { { EnergyResourceType.ColdWater, 11 }, { EnergyResourceType.Electricity, 1 }  } },
                { ProductType.Nut, new Dictionary<EnergyResourceType, double> { { EnergyResourceType.ColdWater, 9.5 }, { EnergyResourceType.Electricity, 1 }  } }
            };

            // План производства болтов для участков цеха 1
            Dictionary<ProductType, int> boltProductionPlan(DateTime dateTime)
            {
                if (dateTime.Hour > 7)                      // Только в рабочее время, с 8 до 23 включительно, производим
                    return new Dictionary<ProductType, int>
                    {
                        { ProductType.Bolt, 50 }
                    };

                return new Dictionary<ProductType, int>();
            }

            // План производства гаек для участков цеха 2
            Dictionary<ProductType, int> nutProductionPlan(DateTime dateTime)
            {
                if (dateTime.Hour > 7)                      // Только в рабочее время, с 8 до 23 включительно, производим
                    return new Dictionary<ProductType, int>
                    {
                        { ProductType.Nut, 50 }
                    };

                return new Dictionary<ProductType, int>();
            }

            // Параметры участков первого и второго цехов
            var area1Parameters = new FacilityParameters
            {
                ConstantConsumption = constantСonsumption,
                Productivity = productivity,
                ProductionPlan = boltProductionPlan
            };

            var area2Parameters = new FacilityParameters
            {
                ConstantConsumption = constantСonsumption,
                Productivity = productivity,
                ProductionPlan = nutProductionPlan
            };

            // Иерархие объектов предприятия и их параметры
            Factory = new ComplexFacility
            {
                Name = "Factory",
                Parameters = new FacilityParameters { ConstantConsumption = constantСonsumption },
                Subfacilities = new[]
                {
                    new ComplexFacility
                    {
                        Name = "Workshop1",
                        Parameters = new FacilityParameters { ConstantConsumption = constantСonsumption },
                        Subfacilities = new []
                        {
                            new Facility { Name = "Area1.1", Parameters = area1Parameters },
                            new Facility { Name = "Area1.2", Parameters = area1Parameters  },
                            new Facility { Name = "Area1.3", Parameters = area1Parameters  },
                        }
                    },
                    new ComplexFacility
                    {
                        Name = "Workshop2",
                        Parameters = new FacilityParameters { ConstantConsumption = constantСonsumption },
                        Subfacilities = new []
                        {
                            new Facility { Name = "Area2.1", Parameters = area2Parameters },
                            new Facility { Name = "Area2.2", Parameters = area2Parameters },
                            new Facility { Name = "Area2.3", Parameters = area2Parameters },
                        }
                    }
                }
            };
        }
    }
}
