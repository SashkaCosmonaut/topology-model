﻿using EnergySupplyModel.Enumerations;
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
        /// Затраты на мероприятия по оптимизации данного объекта.
        /// </summary>
        public double ActivityCost { get; set; } = 1000;

        /// <summary>
        /// Параметры времени и даты для модели. 
        /// </summary>
        public InputDateTimeParameters DateTimeParams { get; set; } = new InputDateTimeParameters();

        /// <summary>
        /// Функция расчета стоимости энергоресурса в текущий момент времени.
        /// </summary>
        public Func<DataItem, double> EnergyResourceCost { get; set; } = (dataItem) =>
        {
            var result = dataItem.ItemValue;

            if (dataItem.TimeStamp.DayOfWeek == DayOfWeek.Saturday || dataItem.TimeStamp.DayOfWeek == DayOfWeek.Sunday)
                return result * 5;

            if (dataItem.TimeStamp.TimeOfDay.Hours < 6 || dataItem.TimeStamp.TimeOfDay.Hours > 20)
                return result * 5;

            return result * 10;
        };

        /// <summary>
        /// Функция расчета штрафа за превышение объема потребления ресурса.
        /// </summary>
        public Func<DataItem, double> Penalty { get; set; } = (dataItem) =>
        {
            return dataItem.ItemValue > 100 ? 100 : 0;
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
            var constantСonsumption = new Dictionary<EnergyResourceType, double>
            {
                { EnergyResourceType.ColdWater, 50 },
                { EnergyResourceType.Electricity, 10 }
            };

            Factory = new ComplexFacility
            {
                Name = "Factory",
                ConstantConsumption = constantСonsumption,
                Subfacilities = new[]
                {
                    new ComplexFacility
                    {
                        Name = "Workshop1",
                        ConstantConsumption = constantСonsumption,
                        Subfacilities = new []
                        {
                            new Facility { Name = "Area1.1", ConstantConsumption = constantСonsumption },
                            new Facility { Name = "Area1.2", ConstantConsumption = constantСonsumption },
                            new Facility { Name = "Area1.3", ConstantConsumption = constantСonsumption },
                        }
                    },
                    new ComplexFacility
                    {
                        Name = "Workshop2",
                        ConstantConsumption = constantСonsumption,
                        Subfacilities = new []
                        {
                            new Facility { Name = "Area2.1", ConstantConsumption = constantСonsumption },
                            new Facility { Name = "Area2.2", ConstantConsumption = constantСonsumption },
                            new Facility { Name = "Area2.3", ConstantConsumption = constantСonsumption },
                        }
                    },
                }
            };
        }
    }
}
