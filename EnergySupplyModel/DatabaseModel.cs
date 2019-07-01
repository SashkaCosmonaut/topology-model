namespace EnergySupplyModel
{
    /// <summary>
    /// Класс, представляющий собой источник данных, предоставляющий данные из БД.
    /// </summary>
    public static class DatabaseModel
    {
        /// <summary>
        /// Получить данные о потреблении энергоресурсов объектом.
        /// </summary>
        /// <param name="name">Наименование объекта.</param>
        /// <returns>Массив данных потребления.</returns>
        public static double? GetData(string name)
        {
            return 105;
        }
    }
}
