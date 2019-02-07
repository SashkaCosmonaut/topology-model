using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        /// TODO: цвет перенести в DataChannel
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
        /// <returns>Стоимость реализации проведения данного пути.</returns>
        public double GetCost()
        {
            try
            {
                if (Path == null)           // Пути может не быть если источник и цель совпадают или путь не нашли
                {
                    if (Source == Target)
                        return 0;

                    return TopologyFitness.UNACCEPTABLE;
                }
                // TODO: нужно учитывать количество подключаемых устройств

                // TODO: проверить дальность пути и проходимость сквозь участки беспроводной связи 

                // TODO: проверить ещё те, у которых источник и приёмник находятся в одной вершине, для них пути не будет

                // TODO: Проверить, что УСПД поддерживает количество подключенных устройств по КПД и КПД поддерживает количество передаваемых устройств

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("TopologyPath GetCost failed! {0}", ex.Message);
                return TopologyFitness.UNACCEPTABLE;
            }
        }
    }
}
