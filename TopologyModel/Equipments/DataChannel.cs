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
        /// Является ли канал передачи данных беспроводным 
        /// </summary>
        public bool IsWireless { get; set; }

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
        /// Максимально допустимое количество устройств, которые могут передавать данные через данный КПД
        /// </summary>
        public uint MaxDevicesConnected { get; set; }

        /// <summary>
        /// Рассчитать базовые затраты на использование данного КПД для формирования сети.
        /// </summary>
        /// <param name="project">Свойства проекта.</param>
        /// <param name="vertex">Вершина графа, в которой установлен инструмент.</param>
        /// <returns>Значение выбранных затрат на данный инструмент.</returns>
        public override double GetCost(Project project, TopologyVertex vertex)
        {
            try
            {
                // Для беспроводной связи метр проведения ничего не стоит
                return IsWireless ? 0 : base.GetCost(project, vertex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DataChannel GetCost failed! {0}", ex.Message);
                return TopologyFitness.UNACCEPTABLE;
            }
        }
    }
}
