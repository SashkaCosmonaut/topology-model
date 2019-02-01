using System;
using System.Collections.Generic;
using TopologyModel.Tools;
using TopologyModel.TopologyGraphs;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс предлагаемой топологии - результат работы метода.
    /// </summary>
    public class Topology
    {
        /// <summary>
        /// Секции топологии.
        /// </summary>
        public TopologySection[] Sections { get; }

        /// <summary>
        /// Словарь связей между элементами одной и более секций, где ключ - УСПД, 
        /// из которого исходит связь, а значение - перечисление граней графа связи.
        /// </summary>
        public Dictionary<DataAcquisitionDevice, IEnumerable<TopologyEdge>> Pathes { get; }

        /// <summary>
        /// Создать новую топологию - декодировать фенотип хромосомы на базе её текущего генотипа.
        /// </summary>
        /// <param name="chromosome">Декодируемая хромосома.</param>
        public Topology(TopologyChromosome chromosome)
        {
            try
            {
                Sections = new TopologySection[chromosome.CurrentProject.MCZs.Length];

                DecodeSections(chromosome);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Topology failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Поочерёдно декодировать каждую секцию хромосомы.
        /// </summary>
        /// <param name="chromosome">Декодируемая хромосома.</param>
        protected void DecodeSections(TopologyChromosome chromosome)
        {
            try
            {
                for (var sectionIndex = 0; sectionIndex < chromosome.Length / TopologyChromosome.GENES_FOR_SECTION; sectionIndex++)
                {
                    if (Sections[sectionIndex] == null)
                        Sections[sectionIndex] = new TopologySection();

                    Sections[sectionIndex].Decode(chromosome, sectionIndex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DecodeSections failed! {0}", ex.Message);
            }
        }
    }
}
