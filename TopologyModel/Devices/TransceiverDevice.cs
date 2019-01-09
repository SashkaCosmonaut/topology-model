using System.Collections.Generic;
using TopologyModel.Enumerations;

namespace TopologyModel.Devices
{
	/// <summary>
	/// Класс передатчика для КПД.
	/// </summary>
	public class TransceiverDevice : Device
	{
		/// <summary>
		/// множество стандартов приёма данных от КУ или КПД и количество подключаемых устройств
		/// </summary>
		public Dictionary<Protocol, int> ReceivingProtocols { get; set; } = new Dictionary<Protocol, int> { };
	}
}
