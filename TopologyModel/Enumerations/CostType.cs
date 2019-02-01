namespace TopologyModel.Enumerations
{
    /// <summary>
    /// Перечисление типов расходов, которые минимизируются в алгоритме поиска топологии.
    /// </summary>
    public enum CostType
    {
        /// <summary>
        /// Минимизировать финансовые расходы при генерации топологии.
        /// </summary>
        Money,

        /// <summary>
        /// Минимизировать временные расходы при генерации топологии.
        /// </summary>
        Time
    }
}
