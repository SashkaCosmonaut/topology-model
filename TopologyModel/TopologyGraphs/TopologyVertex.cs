namespace TopologyModel.TopologyGraphs
{
	/// <summary>
	/// Класс вершины графа топологии.
	/// </summary>
	public class TopologyVertex
	{
		/// <summary>
		/// Ссылка на участок, на котором располагается данная вершина.
		/// </summary>
		public Region Region { get; set; }

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
		public TopologyVertex(Region region, uint regionX, uint regionY)
		{
			Region = region;
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
	}
}
