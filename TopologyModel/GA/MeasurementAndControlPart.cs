﻿using GeneticSharp.Domain.Randomizations;
using System;
using System.Linq;
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

        /// <summary>
        /// Генерировать новое случайное значение гена, представляющего устройство учёта и управления.
        /// Из всех доступных устройств, выбираются только те, которые подходят для данного места учёта и управления.
        /// Ген является индексом в массиве всех доступных устройств.
        /// </summary>
        /// <param name="chromosome">Текущая хромосома.</param>
        /// <param name="sectionIndex">Индекс секции, для которой генерируется ген.</param>
        /// <returns>Целочисленное значение случайного гена, соответствующее индексу в массиве доступных устройств.</returns>
        public static int GenerateDeviceGene(TopologyChromosome chromosome, int sectionIndex)
        {
            try
            {
                var suitableMCDs = chromosome.CurrentProject.AvailableTools.MCDs
                    .Select((mcd, index) => new { MCD = mcd, Index = index })                           // Запоминаем индекс устройства в массиве всех доступных устройств
                    .Where(q => q.MCD.IsSuitableForMCZ(chromosome.CurrentProject.MCZs[sectionIndex]))   // Выбираем только те устройства, которые подходят для места
                    .ToArray();

                var randomIndex = RandomizationProvider.Current.GetInt(0, suitableMCDs.Count());        // Выбираем из массива выбранных устройств случайное

                return suitableMCDs[randomIndex].Index;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenerateMCDevice failed! {0}", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Сгенерировать новый ген, представляющий случайную вершину графа, который находится на выбранном 
        /// месте учёта и управления. Ген является индексом в массиве вершин графа, на которых
        /// находится данное место учёта и управления.
        /// </summary>
        /// <param name="chromosome">Текущая хромосома.</param>
        /// <param name="sectionIndex">Индекс секции, для которой генерируется ген.</param>
        /// <returns>Целочисленное значение случайного гена, соответствующее индексу в массиве вершин графа.</returns>
        public static int GenerateVertexGene(TopologyChromosome chromosome, int sectionIndex)
        {
            try
            {
                var currentMCZ = chromosome.CurrentProject.MCZs[sectionIndex];    // Текущее место учёта и управления

                var mczVertices = chromosome.CurrentProject.Graph.VerticesArray
                    .Select((vertex, index) => new { Vertex = vertex, Index = index })  // Запоминаем индекс в массиве каждого места
                    .Where(q => q.Vertex.MCZs.Contains(currentMCZ)).ToArray();          // Берём те вершины графа, где располагается данное место

                var randomIndex = RandomizationProvider.Current.GetInt(0, mczVertices.Count());

                return mczVertices[randomIndex].Index;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenerateMCDVertexGene failed! {0}", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Сгенерировать новый ген, представляющий случайный КПД для устройства учёта и управления, выбирается 
        /// такой, чтобы подходил к устройству.
        /// </summary>
        /// <param name="chromosome">Текущая хромосома.</param>
        /// <param name="sectionIndex">Индекс секции, для которой генерируется ген.</param>
        /// <returns>Целочисленное значение случайного гена, соответствующее индексу в массиве доступных каналов передачи данных.</returns>
        public static int GenerateChannelGene(TopologyChromosome chromosome, int sectionIndex)
        {
            try
            {
                // Декодируем устройство из гена, которое выбрано в данной секции (оно идёт первым хромосоме)
                var device = chromosome.CurrentProject.AvailableTools.MCDs[(int)chromosome.GetGene(sectionIndex * TopologyChromosome.GENES_FOR_SECTION).Value];

                var availableChannels = chromosome.CurrentProject.AvailableTools.DCs
                    .Select((channel, index) => new { Channel = channel, Index = index })
                    .Where(q => device.SendingProtocols.Contains(q.Channel.Protocol))   // Выбираем те КПД, которые совместимы с данным устройством
                    .ToArray();

                var randomIndex = RandomizationProvider.Current.GetInt(0, availableChannels.Count());

                return availableChannels[randomIndex].Index;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenerateMCTransmission failed! {0}", ex.Message);
                return 0;
            }
        }
    }
}
