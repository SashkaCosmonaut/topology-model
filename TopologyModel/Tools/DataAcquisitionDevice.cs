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
        /// Множество доступных стандартов приёма данных и максимальное количество подключаемых устройств 
        /// TODO: в фитнес-функции нужно учитывать количество подключаемых устройств
        /// </summary>
        public Dictionary<DataChannelCommunication, int> ReceivingCommunications { get; set; }

        /// <summary>
        /// Множество доступных способов передачи данных на сервер
        /// TODO: в фитнес функции надо учитывать, подходит ли устройство участку
        /// </summary>
        public InternetConnection[] ServerConnections { get; set; }
    }
}
