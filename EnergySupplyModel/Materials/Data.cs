using System;
using System.Collections.Generic;

namespace EnergySupplyModel.Materials
{
    /// <summary>
    /// Класс данных о потреблении энергоресурсов.
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Элементы данных этого блока данных.
        /// </summary>
        public Dictionary<DateTime, DataItem> DataItems { get; set; }
    }
}
