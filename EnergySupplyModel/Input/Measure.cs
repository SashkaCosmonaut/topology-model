using EnergySupplyModel.Facilities;

namespace EnergySupplyModel.Input
{
    /// <summary>
    /// Класс, описывающий мероприятие, миимизирующее затраты потребления энергоресурсов.
    /// </summary>
    public class Measure
    {
        /// <summary>
        /// Наименование мероприятия.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Новые параметры некоторого объекта после принятия данного мероприятия.
        /// </summary>
        public FacilityParameters NewFacilityParameters { get; set; }

        /// <summary>
        /// Финансовые затраты на проведение данного мероприятия.
        /// </summary>
        public double Cost { get; set; }
    }
}
