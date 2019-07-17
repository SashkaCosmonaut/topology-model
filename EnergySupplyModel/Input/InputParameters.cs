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
        public Measure[] Measures { get; set; } = new Measure[]
        {
            new Measure
            {
                Name = "Мероприятие на Area1.3",
                Cost = 100,
                NewFacilityParameters = new FacilityParameters
                {
                    ConstantConsumption = (dateTime) =>
                    {
                        return new Dictionary<EnergyResourceType, double>
                        {
                            { EnergyResourceType.ColdWater, 0 },
                            { EnergyResourceType.Electricity, 0 }
                        };
                    }
                }
            },
            new Measure
            {
                Name = "Мероприятие на Area2.3",
                Cost = 10000,
                NewFacilityParameters = new FacilityParameters
                {
                    ProductionPlan = (dateTime) =>
                    {
                        if (dateTime.Hour < 8 || dateTime.Hour > 19)
                            return new Dictionary<ProductType, int>
                            {
                                { ProductType.Nut, 50 }
                            };

                        return new Dictionary<ProductType, int>();
                    }
                }
            }
        };

        /// <summary>
        /// Функция расчета стоимости энергоресурса в укзанный момент времени и для указанного энергоресурса.
        /// </summary>
        public Func<DataItem, EnergyResourceType, double> EnergyResourceCost { get; set; } = (dataItem, energyResourceType) =>
        {
            var result = dataItem.ItemValue;

            switch (energyResourceType)
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
        public Func<DataItem, EnergyResourceType, double> Penalty { get; set; } = (dataItem, energyResourceType) =>
        {
            switch (energyResourceType)
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
            Dictionary<EnergyResourceType, double> constantСonsumption(DateTime dateTime)
            {
                return new Dictionary<EnergyResourceType, double>
                {
                    { EnergyResourceType.ColdWater, 1 },
                    { EnergyResourceType.Electricity, 1 }
                };
            }

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
                CurrentParameters = new FacilityParameters { ConstantConsumption = constantСonsumption },
                Subfacilities = new[]
                {
                    new ComplexFacility
                    {
                        Name = "Workshop1",
                        CurrentParameters = new FacilityParameters { ConstantConsumption = constantСonsumption },
                        Subfacilities = new []
                        {
                            new Facility { Name = "Area1.1", CurrentParameters = area1Parameters },
                            new Facility { Name = "Area1.2", CurrentParameters = area1Parameters },
                            new Facility { Name = "Area1.3", CurrentParameters = area1Parameters, AppliedMeasure = Measures[0] },
                        }
                    },
                    new ComplexFacility
                    {
                        Name = "Workshop2",
                        CurrentParameters = new FacilityParameters { ConstantConsumption = constantСonsumption },
                        Subfacilities = new []
                        {
                            new Facility { Name = "Area2.1", CurrentParameters = area2Parameters },
                            new Facility { Name = "Area2.2", CurrentParameters = area2Parameters },
                            new Facility { Name = "Area2.3", CurrentParameters = area2Parameters, AppliedMeasure = Measures[1] },
                        }
                    }
                }
            };
        }
    }
}
