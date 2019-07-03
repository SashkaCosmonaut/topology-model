using EnergySupplyModel.Enumerations;
using EnergySupplyModel.Facilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnergySupplyModel
{
    /// <summary>
    /// Класс входных параметров для модели.
    /// </summary>
    public class InputParameters
    {
        /// <summary>
        /// Порог выявления проблемы.
        /// </summary>
        public double EpsilonP = 10;

        /// <summary>
        /// Порог выявления утечки.
        /// </summary>
        public double EpsilonS = 10;

        /// <summary>
        /// Порог выявления рентабельности меропритий.
        /// </summary>
        public double EpsilonC = 80;

        /// <summary>
        /// Стоимость энергоресурса в текущий момент времени.
        /// </summary>
        public double EnergyResourceCost { get; set; } = 8;

        /// <summary>
        /// Затраты на мероприятия по оптимизации данного объекта.
        /// </summary>
        public double ActivityCost { get; set; } = 1000;

        /// <summary>
        /// Начало периода анализа.
        /// </summary>
        public DateTime Start { get; set; } = new DateTime(2010, 01, 10);

        /// <summary>
        /// Конец периода анализа.
        /// </summary>
        public DateTime End { get; set; } = new DateTime(2010, 01, 11);

        /// <summary>
        /// Шаг разбиения периода времени.
        /// </summary>
        public TimeInterval Step { get; set; } = TimeInterval.Hour1;

        /// <summary>
        /// Функция расчета штрафа за превышение объема потребления ресурса.
        /// </summary>
        public Func<Dictionary<DateTime, double>, double> Penalty { get; set; } = (consumption) =>
        {
            return consumption.Values.Select(q => q > 100 ? 100 : 0).Sum();
        };

        /// <summary>
        /// Производство продукции осуществляется целевой системой - системой производства. В систему производства входят объекты предприятия.
        /// На самом верхнем уровне в качестве объекта рассматривается всё предприятие в целом, которое разбивается на совокупность подобъектов: 
        /// территории, здания, цеха, участки, вплоть до отдельного станка или единицы оборудования.
        /// </summary>
        public ComplexFacility Fctory { get; set; } = new ComplexFacility
        {
            Name = "Factory",
            Subfacilities = new[]
            {
                new ComplexFacility
                {
                    Name = "Workshop1",
                    Subfacilities = new []
                    {
                        new Facility { Name = "Area1.1" },
                        new Facility { Name = "Area1.2" },
                        new Facility { Name = "Area1.3" },
                    }
                },
                new ComplexFacility
                {
                    Name = "Workshop2",
                    Subfacilities = new []
                    {
                        new Facility { Name = "Area2.1" },
                        new Facility { Name = "Area2.2" },
                        new Facility { Name = "Area2.3" },
                    }
                },
            }
        };
    }
}
