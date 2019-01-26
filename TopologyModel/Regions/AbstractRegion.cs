namespace TopologyModel.Regions
{
    /// <summary>
    /// Класс некоторого абстрактного участка предприятия.
    /// </summary>
    public class AbstractRegion
    {
        /// <summary>
        /// Уникальный идентификатор участка.
        /// </summary>
        public uint Id { get; set; }

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
    }
}
