using TopologyModel.TopologyGraphs;
using System;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс абстрактной части секции топологии, содержащий вершину расположения части секции.
    /// </summary>
    public abstract class AbstractTopologySectionPart : IComparable
    {
        /// <summary>
        /// Узел графа, в котором находится данная часть секции топологии.
        /// </summary>
        public TopologyVertex Vertex { get; protected set; }

        /// <summary>
        /// Сравнить части секции топологии.
        /// </summary>
        /// <param name="obj">Другая часть секции топологии.</param>
        /// <returns>0, если части секции одинаковые, иное значение, если нет.</returns>
        public abstract int CompareTo(object obj);

        /// <summary>
        /// Декодировать вершину графа части секции.
        /// </summary>
        /// <param name="project">Текущий используемый проект.</param>
        /// <param name="vertexGene">Ген, характеризующий вершину графа части секции.</param>
        protected void DecodeVertex(Project project, int vertexGene)
        {
            try
            {
                Vertex = project.Graph.VerticesArray[vertexGene];
            }
            catch (Exception ex)
            {
                Console.WriteLine("AbstractTopologySectionPart Decode failed! {0}", ex.Message);
            }
        }
    }
}
