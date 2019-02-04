namespace TopologyModel.Enumerations
{
    /// <summary>
    /// Перечисление типов расходов, которые минимизируются в алгоритме поиска топологии.
    /// </summary>
    public enum CostType
    {
        /// <summary>
        /// Минимизировать финансовые расходы на установку при генерации топологии.
        /// </summary>
        InstantMoney,

        /// <summary>
        /// Минимизировать финансовые расходы на установку и дальнейшее обслуживание при генерации топологии.
        /// </summary>
        InstantAndMaintenanceMoney,

        /// <summary>
        /// Минимизировать временные расходы при генерации топологии.
        /// </summary>
        Time,

        /// <summary>
        /// Минимизировать нужно всё суммарно.
        /// </summary>
        All
    }
}
