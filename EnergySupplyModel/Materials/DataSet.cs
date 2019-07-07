using System;
using System.Collections.Generic;

namespace EnergySupplyModel.Materials
{
    /// <summary>
    /// Класс данных о потреблении энергоресурсов, который содержит в себе элементы данных этого блока данных.
    /// </summary>
    public class DataSet : Dictionary<DateTime, DataItem>
    {
        /// <summary>
        /// Характеристики источника данных.
        /// </summary>
        public DataSource DataSource { get; set; }
    }
}
