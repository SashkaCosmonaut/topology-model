using System;
using System.Collections.Generic;
using System.Linq;
using TopologyModel.Graphs;
using TopologyModel.Equipments;
using Newtonsoft.Json;

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
        /// Словарь связей между элементами одной и более секций, где ключ - УСПД, значение - множество путей, исходящих из него по разным КПД
        /// </summary>
        public Dictionary<DataAcquisitionSectionPart, IEnumerable<TopologyPath>> Pathes { get; }

        /// <summary>
        /// Создать новую топологию - декодировать фенотип хромосомы на базе её текущего генотипа.
        /// </summary>
        /// <param name="chromosome">Декодируемая хромосома.</param>
        public Topology(TopologyChromosome chromosome)
        {
            try
            {
                Sections = new TopologySection[chromosome.CurrentProject.MCZs.Length];
                Pathes = new Dictionary<DataAcquisitionSectionPart, IEnumerable<TopologyPath>>();

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
                // Группируем секции по частям с УСПД и перебираем группы
                foreach (var dadGroup in Sections.GroupBy(q => q.DADPart, new DataAcquisitionSectionPartComparer()))
                {
                    var pathes = new List<TopologyPath>();

                    foreach (var channelGroup in dadGroup.GroupBy(w => w.Channel))  // Группы секций группируем по каналам передачи данных, которые связывают КУ и УСПД 
                    {
                        // Находим путь между УСПД и всеми КУ по каждому каналу передачи в группе секций
                        var path = TopologyPathfinder.SectionShortestPath(chromosome.CurrentProject.Graph, dadGroup.Key.Vertex, channelGroup.Select(q => q.MCDPart.Vertex), channelGroup.Key);

                        if (path != null)
                            pathes.AddRange(path);
                    }

                    Pathes.Add(dadGroup.Key, pathes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DecodePathes failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Получить информацию о топологии.
        /// </summary>
        /// <returns>Строковое представление топологии.</returns>
        public string GetInfo()
        {
            try
            {
                var groups = Sections
                    .GroupBy(section => section.DADPart, new DataAcquisitionSectionPartComparer())
                    .Select(dadGroup =>
                        new
                        {
                            DAD = new { dadGroup.Key.DAD.Name, Vertex = dadGroup.Key.Vertex.ToString() },
                            Connected = dadGroup
                                .GroupBy(group => group.Channel)
                                .Select(channelGroup =>
                                    new
                                    {
                                        Channel = channelGroup.Key.Name,
                                        Devices = channelGroup.Select(q =>
                                            new
                                            {
                                                q.MCDPart.MCD.Name,
                                                Vertex = q.MCDPart.Vertex.ToString()
                                            }).ToArray()
                                    }).ToArray()
                        }).ToArray();

                return JsonConvert.SerializeObject(groups);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Topology GetInfo failed! {0}", ex.Message);
                return "";
            }
        }
    }
}
