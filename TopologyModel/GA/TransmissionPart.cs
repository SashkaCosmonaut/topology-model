﻿using TopologyModel.Tools;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс той части секции топологии, в которой находится приемопередатчик.
    /// </summary>
    public class TransmissionPart : AbstractTopologySectionPart
    {
        /// <summary>
        /// Приемопередатчик, который используется в данной секции.
        /// </summary>
        public TransceiverDevice Transceiver { get; set; }

        /// <summary>
        /// Сравнить части секции топологии.
        /// </summary>
        /// <param name="obj">Другая часть секции топологии.</param>
        /// <returns>0, если части секции одинаковые, иное значение, если нет.</returns>
        public override int CompareTo(object obj) => throw new System.NotImplementedException();
    }
}
