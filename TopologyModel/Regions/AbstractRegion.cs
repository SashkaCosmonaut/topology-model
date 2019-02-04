namespace TopologyModel.Regions
{
    /// <summary>
    /// Класс некоторого абстрактного участка предприятия.
    /// </summary>
    public abstract class AbstractRegion
    {
        /// <summary>
        /// Наименование участка.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Координата Х левого верхнего угла участка на территории предприятия.
        /// </summary>
        public uint X { get; set; }

        /// <summary>
        /// Координата Y левого верхнего угла участка на территории предприятия.
        /// </summary>
        public uint Y { get; set; }

        /// <summary>
        /// Ширина участка по схеме в метрах.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// Высота участка по схеме в метрах.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// Получить строковую интерпретацию участка.
        /// </summary>
        /// <returns>Строка с описанием свойств объекта участка.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Получить подробную информацию в текстовом виде о свойствах участка.
        /// </summary>
        /// <returns>Строка с информацией об участке.</returns>
        public abstract string GetInfo();
    }
}
