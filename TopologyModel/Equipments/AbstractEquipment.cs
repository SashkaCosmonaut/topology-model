using System;
using TopologyModel.Enumerations;
using TopologyModel.GA;
using TopologyModel.Graphs;

namespace TopologyModel.Equipments
{
    /// <summary>
    /// Абстрактный класс некоторого инструмента для реализации сети, устройства.
    /// </summary>
    public abstract class AbstractEquipment
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

        /// <summary>
        /// Рассчитать базовые затраты на использование данного инструмента для формирования сети.
        /// </summary>
        /// <param name="project">Свойства проекта.</param>
        /// <param name="vertex">Вершина графа, в которой установлен инструмент.</param>
        /// <returns>Значение выбранных затрат на данный инструмент.</returns>
        public virtual double GetCost(Project project, TopologyVertex vertex)
        {
            try
            {
                var cost = 0.0;

                var laboriousnessFactor = 1 + vertex.LaboriousnessWeight / 10;  // В худшем случае затраты могут возрасти в 4 раза

                // TODO: добавить в свойства проекта час работы, чтобы можно было точнее считать затраты на всё вместе
                if (project.MinimizationGoal == CostType.Time || project.MinimizationGoal == CostType.All)
                {
                    // Для расчета временных и всех затрат важно время на установку оборудования
                    cost += InstallationTime.TotalHours * laboriousnessFactor;
                }
                else if (project.MinimizationGoal != CostType.Time)
                {
                    // Если не используем мастную рабочую силу, то умножаем стоимость установки на трудоемкость проведения работ
                    cost += PurchasePrice + (project.UseLocalEmployee ? 0 : InstallationPrice * laboriousnessFactor);
                }

                return cost;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AbstractEquipment GetCost failed! {0}", ex.Message);
                return TopologyFitness.UNACCEPTABLE;
            }
        }
    }
}
