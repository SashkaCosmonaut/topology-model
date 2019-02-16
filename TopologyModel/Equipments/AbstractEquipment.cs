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
        /// <returns>Значение затрат на внедрение данного инструмента.</returns>
        public virtual double GetCost(Project project, TopologyVertex vertex)
        {
            try
            {
                return GetCost(project, 1 + vertex.LaboriousnessWeight / 10);   // В худшем случае затраты могут возрасти в 4 раза
            }
            catch (Exception ex)
            {
                Console.WriteLine("AbstractEquipment GetCost failed! {0}", ex.Message);
                return TopologyFitness.UNACCEPTABLE;
            }
        }

        /// <summary>
        /// Рассчитать стоимость в зависимости от коэффициента трудоемкости реализации.
        /// </summary>
        /// <param name="project">Свойства проекта.</param>
        /// <param name="laboriousnessFactor">Коэффициент трудоемкости.</param>
        /// <returns>Значение затрат на внедрение данного инструмента.</returns>
        protected double GetCost(Project project, double laboriousnessFactor)
        {
            try
            {
                var cost = 0.0;

                if (project.MinimizationGoal == CostType.Time || project.MinimizationGoal == CostType.All)
                {
                    // Для расчета временных и всех затрат важно время на установку оборудования
                    cost += InstallationTime.TotalHours * laboriousnessFactor;

                    // Если считаем всё, то учитываем время умножаем на час работ, чтобы стоимость привести к деньгам
                    if (project.MinimizationGoal == CostType.All)
                        cost *= project.CostPerHour;
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
