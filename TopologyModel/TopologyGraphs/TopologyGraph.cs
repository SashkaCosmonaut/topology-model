using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopologyModel.TopologyGraphs
{
	/// <summary>
	/// Класс ненаправленного графа топологии сети, который содержит вершины, привязанные 
	/// к участкам предприятия, и ненаправленные взвешенные (дробным числом) ребра.
	/// </summary>
	public class TopologyGraph : UndirectedGraph<TopologyVertex, SUndirectedTaggedEdge<TopologyVertex, float>>
	{

	}
}
