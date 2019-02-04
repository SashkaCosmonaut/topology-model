namespace TopologyModel.Enumerations
{
    /// <summary>
    /// Перечисление существующих на предприятии управляющих воздействий.
    /// </summary>
    public enum Control
    {
        /// <summary>
        /// Включение/отключение какого-либо оборудования.
        /// </summary>
        EquipmentOnOff,

        /// <summary>
        /// Изменение уровня какого-либо воздействия в оборудовании.
        /// </summary>
        LevelChanging,

        /// <summary>
        /// Переключение какого-либо реле.
        /// </summary>
        RelaySwitching
    }
}
