using System;
using TopologyModel.Tools;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс той части секции топологии, в которой находится УСПД.
    /// </summary>
    public class DataAcquisitionPart : AbstractTopologySectionPart
    {
        /// <summary>
        /// УСПД, которое используется в данной секции топологии.
        /// </summary>
        public DataAcquisitionDevice DAD { get; protected set; }

        /// <summary>
        /// Декодировать содержимое данной части секции.
        /// </summary>
        /// <param name="project">Текщуий используемый проект.</param>
        /// <param name="dadGene">Ген, характеризующий УСПД секции.</param>
        /// <param name="vertexGene">Ген, характеризующий вершину графа части секции.</param>
        /// <param name="channelGene">Ген, характеризующий используемый канал передачи данных части секции.</param>
        public void Decode(Project project, int dadGene, int vertexGene, int channelGene)
        {
            Decode(project, vertexGene, channelGene);

            try
            {
                DAD = project.AvailableTools.DADs[dadGene];
            }
            catch (Exception ex)
            {
                Console.WriteLine("DataAcquisitionPart Decode failed! {0}", ex.Message);
            }
        }
    }
}
