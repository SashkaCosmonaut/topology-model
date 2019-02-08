using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TopologyModel.Equipments;
using TopologyModel.GA;

namespace TopologyModel.Graphs
{
    /// <summary>
    /// Класс пути в топологии от одной вершины до другой по определённому КПД.
    /// </summary>
    public class TopologyPath
    {
        /// <summary>
        /// Вершина графа - источник пути.
        /// </summary>
        public TopologyVertex Source { get; set; }

        /// <summary>
        /// Вершина пути - цель пути.
        /// </summary>
        public TopologyVertex Target { get; set; }

        /// <summary>
        /// КПД, по которому связаны источник и цель.
        /// </summary>
        public DataChannel DataChannel { get; set; }

        /// <summary>
        /// Перечисление граней графа, составляющих путь.
        /// </summary>
        public IEnumerable<TopologyEdge> Path { get; set; }

        /// <summary>
        /// Случайный цвет для закраски данного пути.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Создать и проинициализировать путь по умолчанию.
        /// </summary>
        public TopologyPath()
        {
            try
            {
                Color = Color.FromArgb(RandomizationProvider.Current.GetInt(0, 255),
                                       RandomizationProvider.Current.GetInt(0, 255),
                                       RandomizationProvider.Current.GetInt(0, 255));
            }
            catch (Exception ex)
            {
                Console.WriteLine("TopologyPath failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Получить стоимость реализации данного пути.
        /// </summary>
        /// <param name="project">Свойства проекта.</param>
        /// <returns>Стоимость реализации проведения данного пути.</returns>
        public double GetCost(Project project)
        {
            try
            {
                if (Path == null)           // Пути может не быть если источник и цель совпадают или путь не нашли
                {
                    if (Source == Target)   // Для случая, когда УСПД и КУ находятся в одной вершине, считаем путь только в ней
                        return DataChannel.GetCost(project, Source);

                    return TopologyFitness.UNACCEPTABLE;
                }

                return Path.Sum(edge => DataChannel.GetCost(project, edge));
            }
            catch (Exception ex)
            {
                Console.WriteLine("TopologyPath GetCost failed! {0}", ex.Message);
                return TopologyFitness.UNACCEPTABLE;
            }
        }
    }
}
