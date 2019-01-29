﻿using System;

namespace TopologyModel.Tools
{
    /// <summary>
    /// Абстрактный класс некоторого инструмента сети, устройства.
    /// </summary>
    public abstract class AbstractTool
    {
        /// <summary>
        /// Глобальный автоприращиваемый идентификатор.
        /// </summary>
        private static uint GlobalId = 0;

        /// <summary>
        /// уникальный идентификатор устройства
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// Наименование устройства.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Стоимость устройства
        /// </summary>
        public double PurchasePrice { get; set; }

        /// <summary>
        /// стоимость монтажа, в которую входят финансовые затраты на монтажные работы
        /// </summary>
        public double InstallationPrice { get; set; }

        /// <summary>
        /// время на установку, требуемое для монтажных работы по установке данного устройства и дополнительных материалов
        /// </summary>
        public TimeSpan InstallationTime { get; set; }

        /// <summary>
        /// Создать некоторое инструмент сети.
        /// </summary>
        public AbstractTool()
        {
            Id = ++GlobalId;
        }

        /// <summary>
        /// Получить строковую интерпретацию инструмента.
        /// </summary>
        /// <returns>Строка с описанием свойств объекта инструмента.</returns>
        public override string ToString()
        {
            return Id + ". " + Name;
        }
    }
}