using System.Collections.Generic;
using TopologyModel.Enumerations;

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
    }
}
