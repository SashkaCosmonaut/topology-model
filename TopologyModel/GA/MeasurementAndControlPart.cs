using System;
using TopologyModel.Tools;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс той части секции топологии, в которой находится устройство учёта или управления.
    /// </summary>
    public class MeasurementAndControlPart : AbstractTopologySectionPart
    {
        /// <summary>
        /// Устройство учёта или управления, которое используется в данной секции.
        /// </summary>
        public MeasurementAndControlDevice MCD { get; protected set; }

        /// <summary>
        /// Декодировать содержимое данной части секции.
        /// </summary>
        /// <param name="project">Текщуий используемый проект.</param>
        /// <param name="mcdGene">Ген, характеризующий устройство учёта и управления секции.</param>
        /// <param name="vertexGene">Ген, характеризующий вершину графа части секции.</param>
        /// <param name="channelGene">Ген, характеризующий используемый канал передачи данных части секции.</param>
        public void Decode(Project project, int mcdGene, int vertexGene, int channelGene)
        {
            Decode(project, vertexGene, channelGene);

            try
            {
                MCD = project.AvailableTools.MCDs[mcdGene];
            }
            catch (Exception ex)
            {
                Console.WriteLine("MeasurementAndControlPart Decode failed! {0}", ex.Message);
            }
        }
    }
}
