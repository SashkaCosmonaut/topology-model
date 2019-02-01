namespace TopologyModel.Enumerations
{
    /// <summary>
    /// Перечисление целей минимизации в алгоритме поиска топологии.
    /// </summary>
    public enum MinimizationGoal
    {
        /// <summary>
        /// Минимизировать финансовые затраты при генерации топологии.
        /// </summary>
        Money,

        /// <summary>
        /// Минимизировать временные затраты при генерации топологии.
        /// </summary>
        Time
    }
}
