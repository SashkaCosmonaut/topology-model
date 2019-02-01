using System.Collections.Generic;
using TopologyModel.Enumerations;

namespace TopologyModel.Tools
{
	/// <summary>
	/// Класс передатчика для КПД.
	/// </summary>
	public class TransceiverDevice : AbstractDevice
	{
		/// <summary>
		/// множество стандартов приёма данных от КУ или КПД и количество подключаемых устройств
		/// </summary>
		public Dictionary<Protocol, int> ReceivingProtocols { get; set; }

        /// <summary>
        /// Рассчитать затраты на использование данного инструмента для формирования сети.
        /// </summary>
        /// <param name="costType">Тип затрат, которые рассчитываются.</param>
        /// <returns>Значение выбранных затрат на данный инструмент.</returns>
        public override double GetCost(CostType costType) => throw new System.NotImplementedException();
    }
}
