using System;
using TopologyModel.Graphs;

namespace TopologyModel.Tools
{
    /// <summary>
    /// Абстрактный класс некоторого инструмента для реализации сети, устройства.
    /// </summary>
    public abstract class AbstractTool
    {
        /// <summary>
        /// Наименование устройства.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Стоимость устройства
        /// </summary>
        public double PurchasePrice { get; set; }

        /// <summary>
        /// Стоимость монтажа, в которую входят финансовые затраты на монтажные работы
        /// </summary>
        public double InstallationPrice { get; set; }

        /// <summary>
        /// Время на установку, требуемое для монтажных работы по установке данного устройства и дополнительных материалов
        /// </summary>
        public TimeSpan InstallationTime { get; set; }

        /// <summary>
        /// Получить строковую интерпретацию инструмента.
        /// </summary>
        /// <returns>Строка с описанием свойств объекта инструмента.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
