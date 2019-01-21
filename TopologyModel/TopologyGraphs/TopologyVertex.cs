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
		/// Создать вершину графа, которая находится на определённом участке предприятия.
		/// </summary>
		/// <param name="region">Ссылка на участкок предприятия, на котором находтся данная вершина.</param>
		public TopologyVertex(Region region)
		{
			Region = region;
		}
	}
}
