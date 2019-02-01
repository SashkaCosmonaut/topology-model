using System.Collections.Generic;
using TopologyModel.Enumerations;
using TopologyModel.TopologyGraphs;

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
        /// <param name="project">Свойства проекта.</param>
        /// <param name="vertex">Вершина графа, в которой установлен инструмент.</param>
        /// <returns>Значение выбранных затрат на данный инструмент.</returns>
        public override double GetCost(Project project, TopologyVertex vertex) => throw new System.NotImplementedException();
    }
}
