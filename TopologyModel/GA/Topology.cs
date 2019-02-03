﻿using System;
using System.Collections.Generic;
using System.Linq;
using TopologyModel.Tools;
using TopologyModel.TopologyGraphs;

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
        public Dictionary<DataAcquisitionDevice, Dictionary<DataChannel, IEnumerable<TopologyEdge>>> Pathes { get; }

        /// <summary>
        /// Создать новую топологию - декодировать фенотип хромосомы на базе её текущего генотипа.
        /// </summary>
        /// <param name="chromosome">Декодируемая хромосома.</param>
        public Topology(TopologyChromosome chromosome)
        {
            try
            {
                Sections = new TopologySection[chromosome.CurrentProject.MCZs.Length];
                Pathes = new Dictionary<DataAcquisitionDevice, Dictionary<DataChannel, IEnumerable<TopologyEdge>>>();

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

        /// <summary>
        /// Декодировать пути между элементами секций хромосомы.
        /// </summary>
        /// <param name="chromosome">Декодируемая хромосома.</param>
        protected void DecodePathes(TopologyChromosome chromosome)
        {
            try
            {
                foreach (var dadGroup in Sections.GroupBy(q => q.DADPart))                         // Группируем секции по частям с УСПД и перебираем группы
                {
                    foreach (var channelGroup in dadGroup.GroupBy(q => q.Channel))                          // Группы секций группируем по каналам передачи данных, которые связывают КУ и УСПД 
                    {
                        if (channelGroup.Count() > channelGroup.Key.MaxDevicesConnected)                    // Если в группе больше устройств, чам можно подключить по КПД
                        {
                         //   return Double.MaxValue;                                                         // С помощью данной хромосомы нельзя соединить устройства, прерываем поиск
                        }
                        
                        // Находим путь между УСПД и всеми счётчиками по каждому каналу передачи в группе секций
                        var path = TopologyPathfinder.SectionShortestPath(chromosome.CurrentProject.Graph, dadGroup.Key.Vertex, channelGroup.Select(q => q.MACPart.Vertex), channelGroup.Key);

                        // Рассчитываем стоимость денег или времени пути
                        // TODO: Надо учитывать режимы, по какому критерию считаются стоимости - по деньгам и по времени
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DecodePathes failed! {0}", ex.Message);
            }
        }
    }
}
