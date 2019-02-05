﻿using GeneticSharp.Domain.Randomizations;
using System;
using System.Drawing;
using System.Linq;
using TopologyModel.Equipments;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс той части секции топологии, в которой находится УСПД.
    /// </summary>
    public class DataAcquisitionSectionPart : AbstractTopologySectionPart
    {
        /// <summary>
        /// УСПД, которое используется в данной секции топологии.
        /// </summary>
        public DataAcquisitionDevice DAD { get; protected set; }

        /// <summary>
        /// Случайный цвет для закраски КПД, исходящих из УСПД данной части.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Декодировать содержимое данной части секции.
        /// </summary>
        /// <param name="project">Текщуий используемый проект.</param>
        /// <param name="dadGene">Ген, характеризующий УСПД секции.</param>
        /// <param name="vertexGene">Ген, характеризующий вершину графа части секции.</param>
        public void Decode(Project project, int dadGene, int vertexGene)
        {
            DecodeVertex(project, vertexGene);

            try
            {
                DAD = project.Equipments.DADs[dadGene];
                Color = Color.FromArgb(RandomizationProvider.Current.GetInt(0, 255),
                                       RandomizationProvider.Current.GetInt(0, 255),
                                       RandomizationProvider.Current.GetInt(0, 255));
            }
            catch (Exception ex)
            {
                Console.WriteLine("DataAcquisitionPart Decode failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Сгенерировать новый ген, представляющий случайное устройство сбора и передачи данных.
        /// </summary>
        /// <param name="chromosome">Текущая хромосома.</param>
        /// <param name="sectionIndex">Индекс секции, для которой генерируется ген.</param>
        /// <returns>Целочисленное значение случайного гена, соответствующее индексу в массиве УСПД.</returns>
        public static int GenerateDeviceGene(TopologyChromosome chromosome, int sectionIndex)
        {
            try
            {
                // Декодируем КУ из гена, которое выбрано в данной секции (оно идёт первым в хромосоме)
                var mcd = chromosome.CurrentProject.Equipments.MCDs[(int)chromosome.GetGene(sectionIndex * TopologyChromosome.GENES_IN_SECTION).Value];

                var availableDADs = chromosome.CurrentProject.Equipments.DADs
                    .Select((dad, index) => new { DAD = dad, Index = index })
                    .Where(q => q.DAD.ReceivingCommunications.Keys.Any(w => mcd.SendingCommunications.Contains(w)))
                    .ToArray();

                var randomIndex = RandomizationProvider.Current.GetInt(0, availableDADs.Count());

                return availableDADs[randomIndex].Index;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenerateDADeviceGene failed! {0}", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Сгенерировать новый ген, представляющий случайную вершину графа, для расположения в ней УСПД.
        /// </summary>
        /// <param name="chromosome">Текущая хромосома.</param>
        /// <param name="sectionIndex">Индекс секции, для которой генерируется ген.</param>
        /// <returns>Целочисленное значение случайного гена, соответствующее индексу в массиве вершин графа.</returns>
        public static int GenerateVertexGene(TopologyChromosome chromosome, int sectionIndex)
        {
            try
            {
                // TODO: Можно сортировать вершины по качеству и выбирать лучшую из случайных
                return RandomizationProvider.Current.GetInt(0, chromosome.CurrentProject.Graph.VerticesArray.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenerateDAVertexGene failed! {0}", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Сравнить части с УСПД секции топологии. Они равны, если расположены в одной вершине и 
        /// используется одинаковое УСПД.
        /// </summary>
        /// <param name="obj">Другая часть секции топологии.</param>
        /// <returns>0, если части секции одинаковые, иное значение, если нет.</returns>
        public override int CompareTo(object obj)
        {
            try
            {
                if (obj is DataAcquisitionSectionPart other)
                    return (Vertex == other.Vertex && DAD == other.DAD) ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("DataAcquisitionPart CompareTo failed! {0}", ex.Message);
            }

            return -1;
        }

        /// <summary>
        /// Рассчитать затраты на использование инструмента в данной части секции для формирования сети.
        /// </summary>
        /// <param name="project">Свойства проекта.</param>
        /// <param name="vertex">Вершина графа, в которой установлен инструмент.</param>
        /// <returns>Значение выбранных затрат на данный инструмент.</returns>
        public override double GetCost(Project project)
        {
            return DAD.GetCost(project, Vertex);
        }
    }
}