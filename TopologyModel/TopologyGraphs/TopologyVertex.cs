using System;
using TopologyModel.Enumerations;
using TopologyModel.Regions;

namespace TopologyModel.TopologyGraphs
{
	/// <summary>
	/// Класс вершины графа топологии.
	/// </summary>
	public class TopologyVertex : IComparable
	{
		/// <summary>
		/// Ссылка на участок, на котором располагается данная вершина.
		/// </summary>
		public TopologyRegion Region { get; set; }

        /// <summary>
        /// Ссылки на места учёта и управления, которые находятся на данной вершине.
        /// </summary>
        public MeasurementAndControlZone[] MCZs { get; set; }

		/// <summary>
		/// Координата вершины внутри участка по оси Х в матрице и в графе.
		/// </summary>
		public uint RegionX { get; set; }

		/// <summary>
		/// Координата вершины внутри участка по оси Y в матрице и в графе.
		/// </summary>
		public uint RegionY { get; set; }

        /// <summary>
        /// Вес вершины для расчёта трудоемкости проведения на ней работ.
        /// </summary>
        public float LaboriousnessWeight { get; }

        /// <summary>
        /// Создать вершину графа, которая находится на определённом участке предприятия.
        /// </summary>
        /// <param name="region">Ссылка на участкок предприятия, на котором находтся данная вершина.</param>
        /// <param name="regionX">Координата вершины внутри участка по оси Х в матрице и в графе.</param>
        /// <param name="regionY">Координата вершины внутри участка по оси Y в матрице и в графе.</param>
        /// <param name="mczs">Ссылки на места учёта и управления, которые находятся на данной вершине.</param>
        public TopologyVertex(TopologyRegion region, uint regionX, uint regionY, MeasurementAndControlZone[] mczs = null)
		{
            Region = region ?? throw new ArgumentNullException(nameof(region));

			RegionX = regionX;
			RegionY = regionY;

            MCZs = mczs;

            LaboriousnessWeight = GetLaboriousnessWeight();
        }

        /// <summary>
        /// Рассчитать вес трудоемкости проведения работ в вершине.
        /// </summary>
        /// <returns>Значение веса вершины.</returns>
        protected float GetLaboriousnessWeight()
        {
            try
            {
                var location = GetLocation();

                return Region.GetLaboriousnessEstimate(location) +
                       Region.GetAggressivenessEstimate(location) +
                       Region.GetUnavailabilityEstimate(location);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetLaboriousnessWeight failed! {0}", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Полчить строковое представление данной вершины.
        /// </summary>
        /// <returns>Строка, содержащая идентификатор участка данной вершины в графе или матрице,
        /// а также её координаты по оси У и Х внутри участка, начинающиеся с 1, а не с 0.</returns>
        public override string ToString()
		{
			return $"{Region.Id}.{RegionY + 1}.{RegionX + 1}";
		}

        /// <summary>
        /// Сравнить две вершины графа.
        /// </summary>
        /// <param name="other">Другая вершина графа или иной объект.</param>
        /// <returns>0 - данный экземпляр занимает ту же позицию в порядке сортировки, что и параметр other.
        /// -1 - данный экземпляр предшествует параметру other в порядке сортировки.
        /// 1- данный экземпляр следует за параметром obj в порядке сортировки.</returns>
        public int CompareTo(object other)
        {
            var otherVertex = other as TopologyVertex;

            if (otherVertex == null) throw new ArgumentException("Incorrect type or null of: ", nameof(other));

            // Для сравнения используются идентификаторы участков
            return (int)Region.Id - (int)otherVertex.Region.Id;
        }

        /// <summary>
        /// Получить месторасположение вершины внутри участка.
        /// </summary>
        /// <returns>Значение из перечисления месторасположений.</returns>
        public LocationInRegion GetLocation()
        {
            if (RegionY == 0)
            {
                if (RegionX == 0)
                    return LocationInRegion.LeftTopCorder;

                if (RegionX == Region.Width - 1)
                    return LocationInRegion.RightTopCorner;

                return LocationInRegion.TopBorder;
            }

            if (RegionY == Region.Height - 1)
            {
                if (RegionX == 0)
                    return LocationInRegion.LeftBottomCorner;

                if (RegionX == Region.Width - 1)
                    return LocationInRegion.RightBottomCorner;

                return LocationInRegion.BottomBorder;
            }

            if (RegionX == 0)
                return LocationInRegion.LeftBorder;

            if (RegionX == Region.Width - 1)
                return LocationInRegion.RightBorder;

            return LocationInRegion.Inside;
        }

        /// <summary>
        /// Проверить, находится ли данная вершина внутри участка.
        /// </summary>
        /// <returns>True, если она внутри участка.</returns>
        public bool IsInside()
        {
            return GetLocation() == LocationInRegion.Inside;
        }
    }
}
