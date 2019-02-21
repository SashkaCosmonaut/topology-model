using System;
using TopologyModel.Enumerations;
using TopologyModel.GA;
using TopologyModel.Graphs;

namespace TopologyModel.Equipments
{
    /// <summary>
    /// Класс канала передачи данных.
    /// </summary>
    public class DataChannel : AbstractEquipment
    {
        /// <summary>
        /// Тип канала передачи данных
        /// </summary>
        public ConnectionType ConnectionType { get; set; }

        /// <summary>
        /// Стандарт передачи данных, используемый в данном КПД
        /// </summary>
        public DataChannelCommunication Communication { get; set; }

        /// <summary>
        /// Топология, допустимая для использования в данном КПД
        /// </summary>
        public DataChannelTopology Topology { get; set; }

        /// <summary>
        /// Максимальная дальность передачи данных по данному КПД
        /// </summary>
        public double MaxRange { get; set; }

        /// <summary>
        /// Рассчитать базовые затраты на использование данного КПД на одном узле для формирования сети.
        /// </summary>
        /// <param name="project">Свойства проекта.</param>
        /// <param name="vertex">Вершина графа, в которой установлен инструмент.</param>
        /// <returns>Значение выбранных затрат на данный инструмент.</returns>
        public override double GetCost(Project project, TopologyVertex vertex)
        {
            try
            {
                switch (ConnectionType)
                {
                    case ConnectionType.Wired:
                        return base.GetCost(project, vertex);

                    case ConnectionType.Wireless:
                        return 0;        // Для беспроводной связи проведение в вершине ничего не стоит

                    case ConnectionType.None:
                        return 1;

                    default:
                        return TopologyFitness.UNACCEPTABLE;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DataChannel GetCost failed! {0}", ex.Message);
                return TopologyFitness.UNACCEPTABLE;
            }
        }

        /// <summary>
        /// Рассчитать затраты на проведение данного КПД по одной грани графа.
        /// </summary>
        /// <param name="project">Свойства проекта.</param>
        /// <param name="edge">Грань, вдоль которой проведён данный КПД.</param>
        /// <returns>Значение выбранных затрат на проведение КПД.</returns>
        public double GetCost(Project project, TopologyEdge edge)
        {
            try
            {
                switch (ConnectionType)
                {
                    case ConnectionType.Wired:
                        return GetCost(project, 1 + edge.Weights[ConnectionType] / 10);

                    case ConnectionType.Wireless:
                        return 0;

                    case ConnectionType.None:
                        return edge.Weights[ConnectionType];

                    default:
                        return TopologyFitness.UNACCEPTABLE;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DataChannel GetCost failed! {0}", ex.Message);
                return TopologyFitness.UNACCEPTABLE;
            }
        }
    }
}
