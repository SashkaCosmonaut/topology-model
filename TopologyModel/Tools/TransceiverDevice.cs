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
	}
}
