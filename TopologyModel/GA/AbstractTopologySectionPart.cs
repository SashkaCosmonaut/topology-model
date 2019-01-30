using TopologyModel.TopologyGraphs;
using TopologyModel.Tools;
using System;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс абстрактной части секции топологии, содержащий параметры выбора КПД
    /// и месторасположения данной части секции.
    /// </summary>
    public abstract class AbstractTopologySectionPart
    {
        /// <summary>
        /// Узел графа, в котором находится данная часть секции топологии.
        /// </summary>
        public TopologyVertex Vertex { get; protected set; }

        /// <summary>
        /// Канал передачи данных, который используется для отправки или приема данных.
        /// </summary>
        public DataChannel Channel { get; protected set; }

        /// <summary>
        /// Декодировать часть секции топологии, раскодировав вершину графа части секции и используемый канал передачи данных.
        /// </summary>
        /// <param name="project">Текущий используемый проект.</param>
        /// <param name="vertexGene">Ген, характеризующий вершину графа части секции.</param>
        /// <param name="channelGene">Ген, характеризующий используемый канал передачи данных части секции.</param>
        protected void Decode(Project project, int vertexGene, int channelGene)
        {
            try
            {
                Vertex = project.Graph.VerticesArray[vertexGene];
                Channel = project.AvailableTools.DCs[channelGene];
            }
            catch (Exception ex)
            {
                Console.WriteLine("AbstractTopologySectionPart Decode failed! {0}", ex.Message);
            }
        }
    }
}
