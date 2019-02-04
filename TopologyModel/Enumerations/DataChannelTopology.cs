﻿namespace TopologyModel.Enumerations
{
    /// <summary>
    /// Топологии каналов передачи данных.
    /// </summary>
    public enum DataChannelTopology
    {
        /// <summary>
        /// Смешанная топология - сами источники данных могут передавать через себя данные от других источников к приемнику.
        /// </summary>
        Mesh,

        /// <summary>
        /// Звезда - данные переллельно передаются от множества источников к одному приемнику.
        /// </summary>
        Star,

        /// <summary>
        /// Шина - данные передаются от множества источников, соединённых последовательно, к одному приемнику.
        /// </summary>
        Bus
    }
}
