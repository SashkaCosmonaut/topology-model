using System;
using System.Collections.Generic;
using System.Linq;
using TopologyModel.Graphs;
using TopologyModel.Equipments;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс предлагаемой топологии - результат работы метода, фенотип хромосомы.
    /// </summary>
    public class Topology
    {
        /// <summary>
        /// Секции топологии.
        /// </summary>
        public TopologySection[] Sections { get; }

        /// <summary>
        /// Словарь связей между элементами одной и более секций, где ключ - УСПД, из которого исходит связь, а значение - также словарь, 
        /// в котором ключ - канал передачи, соединяющий УСПД и КУ, а значение - перечисление граней графа связи, в которых находятся КУ.
        /// </summary>
        public Dictionary<DataAcquisitionSectionPart, Dictionary<DataChannel, IEnumerable<TopologyEdge>>> Pathes { get; }

        /// <summary>
        /// Создать новую топологию - декодировать фенотип хромосомы на базе её текущего генотипа.
        /// </summary>
        /// <param name="chromosome">Декодируемая хромосома.</param>
        public Topology(TopologyChromosome chromosome)
        {
            try
            {
                Sections = new TopologySection[chromosome.CurrentProject.MCZs.Length];
                Pathes = new Dictionary<DataAcquisitionSectionPart, Dictionary<DataChannel, IEnumerable<TopologyEdge>>>();

                DecodeSections(chromosome);
                DecodePathes(chromosome);
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
                for (var sectionIndex = 0; sectionIndex < chromosome.Length / TopologyChromosome.GENES_IN_SECTION; sectionIndex++)
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

        /// <summary>
        /// Декодировать пути между элементами секций хромосомы.
        /// </summary>
        /// <param name="chromosome">Декодируемая хромосома.</param>
        protected void DecodePathes(TopologyChromosome chromosome)
        {
            try
            {
                foreach (var dadGroup in Sections.GroupBy(q => q.DADPart))          // Группируем секции по частям с УСПД и перебираем группы
                {
                    var channelPathes = new Dictionary<DataChannel, IEnumerable<TopologyEdge>>();

                    foreach (var channelGroup in dadGroup.GroupBy(w => w.Channel))  // Группы секций группируем по каналам передачи данных, которые связывают КУ и УСПД 
                    {
                        // Находим путь между УСПД и всеми КУ по каждому каналу передачи в группе секций
                        var path = TopologyPathfinder.SectionShortestPath(chromosome.CurrentProject.Graph, dadGroup.Key.Vertex, channelGroup.Select(q => q.MACPart.Vertex), channelGroup.Key);

                        channelPathes.Add(channelGroup.Key, path);
                    }

                    Pathes.Add(dadGroup.Key, channelPathes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DecodePathes failed! {0}", ex.Message);
            }
        }
    }
}
