﻿using EnergySupplyModel.Enumerations;
using System;
using System.Collections.Generic;

namespace EnergySupplyModel.Materials
{
    /// <summary>
    /// Потребляемый энергоресурс.
    /// </summary>
    public class EnergyResourceConsumption
    {
        /// <summary>
        /// Тип (наименование) потребляемого энергоресурса.
        /// </summary>
        public EnergyResourceType Type { get; set; }

        /// <summary>
        /// Потреблённые объемы энергоресурса в моменты времени (по 1 часу).
        /// </summary>
        public Dictionary<DateTime, double> ConsumedVolumes { get; set; }
    }
}
