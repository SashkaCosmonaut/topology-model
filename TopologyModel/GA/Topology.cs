using System;
using System.Collections.Generic;
using System.Linq;
using TopologyModel.Graphs;
using TopologyModel.Equipments;
using Newtonsoft.Json;
using GeneticSharp.Domain.Randomizations;
using TopologyModel.Enumerations;

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
                GroupDADs(chromosome);
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
        /// Сгруппировать УСПД одной модели, расположенные на одном участке.
        /// </summary>
        /// <param name="chromosome">Декодириуемая хромосома.</param>
        protected void GroupDADs(TopologyChromosome chromosome)
        {
            try
            {
                var dadSectionPartGroups = Sections
                    .Select((section, Index) => new { Part = section.DADPart, Index })  // Запоминаем и индекс секции
                    .GroupBy(q => q.Part.DAD)                                           // Группируем секции с одинаковым УСПД
                    .Where(q => q.Count() > 1);                                         // Берём те группы, если хоть какие-то УСПД сгруппировали

                foreach (var dadSectionPartGroup in dadSectionPartGroups)                            
                {
                    // Далее группируем по участкам и берём те группы, где их больше 1 и где они ещё не на одной вершине
                    var regionSectionPartGroups = dadSectionPartGroup
                        .GroupBy(q => q.Part.Vertex.Region)
                        .Where(q => q.Count() > 1);

                    foreach (var regionSectionPartGroup in regionSectionPartGroups)
                    {
                        // Если все устройства и так уже в одной вершине, пропускаем эту группу
                        var firstVertex = dadSectionPartGroup.FirstOrDefault()?.Part.Vertex;

                        if (firstVertex == null || regionSectionPartGroup.All(q => q.Part.Vertex == firstVertex))
                            continue;

                        // Берём все вершины текущего региона
                        var regionVertices = chromosome.CurrentProject.Graph.VerticesArray
                            .Select((Vertex, Index) => new { Vertex, Index })
                            .Where(q => q.Vertex.Region == regionSectionPartGroup.Key);

                        var minVertexWeight = regionVertices.Min(q => q.Vertex.LaboriousnessWeight);    // Определяем самый малый вес вершины

                        // Находим все вершины с наименьшим весом
                        var minWeightVertices = regionVertices.Where(q => q.Vertex.LaboriousnessWeight == minVertexWeight);

                        // Можно выбирать из вершин с наименьшим весом те, которые ближе всего ко всем текущим вершинам УСПД, но это долго
                        //var minWeightVertexSumOfDistances = minWeightVertices
                        //    .Select(indexedVertex =>
                        //    new
                        //    {
                        //        IndexedVertex = indexedVertex,
                        //        SumOfDistances = dadSectionPartGroup.Sum(section =>
                        //            TopologyPathfinder.GetPath(chromosome.CurrentProject.Graph, indexedVertex.Vertex, section.Part.Vertex, ConnectionType.None)?.Count() ?? 0)
                        //    });

                        //var minSumOfDistance = minWeightVertexSumOfDistances.Min(q => q.SumOfDistances);

                        //var optimalVertices = minWeightVertexSumOfDistances.Where(q => q.SumOfDistances == minSumOfDistance);

                        //if (!optimalVertices.Any()) continue;

                        // Выбираем случайно одну вершину из тех, что с наименьшим весом 
                        var randomOptimalVertex = minWeightVertices.ElementAt(RandomizationProvider.Current.GetInt(0, minWeightVertices.Count()));

                        foreach (var sectionPart in regionSectionPartGroup)     // Перемещаем все УСПД в эту вершину, чтобы они стали одним УСПД
                        {
                            sectionPart.Part.Move(randomOptimalVertex.Vertex);

                            // Модифицируем хромосому тоже, находим ген позиции УСПД и обновляем индекс новой вершины
                            chromosome.ReplaceGene(sectionPart.Index * TopologyChromosome.GENES_IN_SECTION + 3, new GeneticSharp.Domain.Chromosomes.Gene(randomOptimalVertex.Index));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GroupDADs failed! {0}", ex.Message);
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
