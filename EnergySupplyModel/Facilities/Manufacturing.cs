namespace EnergySupplyModel.Facilities
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
            Name = "Factory";

            Subfacilities = new []
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
            };
        }
    }
}
