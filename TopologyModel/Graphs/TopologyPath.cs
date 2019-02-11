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
        /// Получить расстояние данного пути с учётом окружающей среды.
        /// </summary>
        /// <returns>Длина пути с учётом всех трудностей, которые накладывает окружающая среда.</returns>
        public double GetDistance()
        {
            try
            {
                // Если ограничение на беспроводную связь больше 1, то каждое значение ограничения снижает дальность на 5 м.
                if (DataChannel.IsWireless)
                    return Path?.Sum(w => w.WirelessWeight * (w.WirelessWeight == 1 ? 1 : 5)) ?? 0;    // Если путь в одной вершине - расстояние нулевое

                // Ограничение на беспроводную свзязь в худшем случае увличит расход кабелей до 4 метров на 1 метр расстояния
                return Path?.Sum(q => 1 + q.WiredWeight / 10) ?? 0;     // Если путь в одной вершине - расстояние нулевое
            }
            catch (Exception ex)
            {
                Console.WriteLine("TopologyPath GetCost failed! {0}", ex.Message);
                return TopologyFitness.UNACCEPTABLE;
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
                // Если путь на одной вершине, то считаем стоимость только её
                return Path?.Sum(edge => GetDistance() * DataChannel.GetCost(project, edge)) ?? DataChannel.GetCost(project, Source);
            }
            catch (Exception ex)
            {
                Console.WriteLine("TopologyPath GetCost failed! {0}", ex.Message);
                return TopologyFitness.UNACCEPTABLE;
            }
        }
    }
}
