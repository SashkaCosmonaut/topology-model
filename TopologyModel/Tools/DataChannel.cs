using System;
using TopologyModel.Enumerations;
using TopologyModel.Graphs;

namespace TopologyModel.Tools
{
    /// <summary>
    /// Класс канала передачи данных.
    /// </summary>
    public class DataChannel : AbstractTool
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
    }
}
