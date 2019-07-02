﻿namespace EnergySupplyModel.Facilities
{
    /// <summary>
    /// Производство продукции осуществляется целевой системой - системой производства. В систему производства входят объекты предприятия.
    /// На самом верхнем уровне в качестве объекта рассматривается всё предприятие в целом, которое разбивается на совокупность подобъектов: 
    /// территории, здания, цеха, участки, вплоть до отдельного станка или единицы оборудования.
    /// </summary>
    public class Manufacturing : ComplexFacility
    {
        public Manufacturing()
        {
            Name = "Всё предприятие";

            Subfacilities = new Facility[]
            {
                new Facility
                {
                    Name = "Тестовое здание",
                    GetExpectedConsumption = () =>
                    {
                        return 100;
                    },
                    GetPotentialConsumption = () => 
                    {
                        return 90;    
                    }
                }
            };
            GetExpectedConsumption = () =>
            {
                return 100;
            };

            GetPotentialConsumption = () =>
            {
                return 90;
            };
        }
    }
}