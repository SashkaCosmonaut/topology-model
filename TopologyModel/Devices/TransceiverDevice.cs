using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopologyModel.Enumerations;

namespace TopologyModel.Devices
{
	public class TransceiverDevice : Device
	{
		/// <summary>
		/// множество стандартов приёма данных от КУ или КПД и количество подключаемых устройств
		/// </summary>
		public Dictionary<Protocol, int> ReceivingProtocols { get; set; } = new Dictionary<Protocol, int> { };
	}
}
