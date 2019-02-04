using System.Collections.Generic;

namespace TopologyModel.Equipments
{
    /// <summary>
    /// БД инструментов для использования в проекте.
    /// </summary>
    public class EquipmentDB
    {
        /// <summary>
        /// Массив доступных конечных устройств.
        /// </summary>
		public MeasurementAndControlDevice[] MCDs { get; set; }

        /// <summary>
        /// Массив доступных каналов передачи данных.
        /// </summary>
		public List<DataChannel> DCs { get; set; }

        /// <summary>
        /// Массив доступных УСПД.
        /// </summary>
		public DataAcquisitionDevice[] DADs { get; set; }
    }
}
