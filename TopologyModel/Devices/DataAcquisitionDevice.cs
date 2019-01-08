using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopologyModel.Enumerations;

namespace TopologyModel.Devices
{
	public class DataAcquisitionDevice : Device	
	{
		/// <summary>
		/// множество доступных стандартов приёма данных и максимальное количество подключаемых устройств 
		/// </summary>
		public Dictionary<Protocol, int> ReceivingProtocols { get; set; } = new Dictionary<Protocol, int> { };

		/// <summary>
		/// множество доступных способов передачи данных на сервер
		/// </summary>
		public InternetConnection[] ServerSendingProtocols { get; set; } = new InternetConnection[] { };
	}
}
