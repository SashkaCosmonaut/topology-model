using System;
using TopologyModel.Enumerations;
using TopologyModel.TopologyGraphs;

namespace TopologyModel.Tools
{
	/// <summary>
	/// Класс канала передачи данных.
	/// </summary>
	public class DataChannel : AbstractTool
	{
		/// <summary>
		/// является ли канал передачи данных беспроводным 
		/// </summary>
		public bool IsWireless { get; set; }

		/// <summary>
		/// стандарт передачи данных, используемый в данном КПД
		/// </summary>
		public Protocol Protocol { get; set; }

		/// <summary>
		/// топология, допустимая для использования в данном КПД
		/// </summary>
		public Topology Topology { get; set; }

		/// <summary>
		/// максимальная дальность передачи данных по данному КПД
		/// </summary>
		public double MaxRange { get; set; }

		/// <summary>
		/// максимально допустимое количество устройств, которые могут передавать данные через данный КПД
		/// </summary>
		public uint MaxDevicesConnected { get; set; }

        /// <summary>
        /// Рассчитать затраты на использование данного инструмента для формирования сети.
        /// </summary>
        /// <param name="costType">Тип затрат, которые рассчитываются.</param>
        /// <param name="vertex">Вершина графа, в которой установлен инструмент.</param>
        /// <returns>Значение выбранных затрат на данный инструмент.</returns>
        public override double GetCost(CostType costType, TopologyVertex vertex)
        {
            try
            {
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("DataChannel GetCost failed! {0}", ex.Message);
                return 0;
            }
        }
    }
}
