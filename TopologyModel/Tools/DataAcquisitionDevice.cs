using System;
using System.Collections.Generic;
using TopologyModel.Enumerations;
using TopologyModel.TopologyGraphs;

namespace TopologyModel.Tools
{
	/// <summary>
	/// Класс УСПД.
	/// </summary>
	public class DataAcquisitionDevice : AbstractDevice	
	{
		/// <summary>
		/// множество доступных стандартов приёма данных и максимальное количество подключаемых устройств 
		/// </summary>
		public Dictionary<Protocol, int> ReceivingProtocols { get; set; }

		/// <summary>
		/// множество доступных способов передачи данных на сервер
		/// </summary>
		public InternetConnection[] ServerSendingProtocols { get; set; }

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
                Console.WriteLine("DataAcquisitionDevice GetCost failed! {0}", ex.Message);
                return 0;
            }
        }
    }
}
