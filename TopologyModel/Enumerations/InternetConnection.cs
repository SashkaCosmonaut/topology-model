namespace TopologyModel.Enumerations
{
    /// <summary>
    /// Варианты подключения УСПД к сети Интернет.
    /// </summary>
    public enum InternetConnection
    {
        GPRS,

        GSM,

        _3G,

        LTE,

        LPWAN,

        WiFi,

        Ethernet,

        /// <summary>
        /// Ручной сбор данных.
        /// </summary>
        Manual
    }
}
