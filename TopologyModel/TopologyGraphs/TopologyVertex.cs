using System;
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
		/// Координата вершины внутри участка по оси Х в матрице и в графе.
		/// </summary>
		public uint RegionX { get; set; }

		/// <summary>
		/// Координата вершины внутри участка по оси Y в матрице и в графе.
		/// </summary>
		public uint RegionY { get; set; }

		/// <summary>
		/// Создать вершину графа, которая находится на определённом участке предприятия.
		/// </summary>
		/// <param name="region">Ссылка на участкок предприятия, на котором находтся данная вершина.</param>
		/// <param name="regionX">Координата вершины внутри участка по оси Х в матрице и в графе.</param>
		/// <param name="regionY">Координата вершины внутри участка по оси Y в матрице и в графе.</param>
		public TopologyVertex(TopologyRegion region, uint regionX, uint regionY)
		{
            Region = region ?? throw new ArgumentNullException(nameof(region));

			RegionX = regionX;
			RegionY = regionY;
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
        /// Проверить, находится ли данная вершина внутри участка.
        /// </summary>
        /// <returns>True, если она внутри участка.</returns>
        public bool IsInside()
        {
            // Вершина находится внутри участка, сли у неё нет ни одной координаты на границе участка
            return RegionX > 0 && RegionY > 0 && RegionX < Region.Width - 1 && RegionY < Region.Height - 1;
        }
    }
}
