using System;
using System.Collections.Generic;
using TopologyModel.Enumerations;
using TopologyModel.Graphs;

namespace TopologyModel.Equipments
{
    /// <summary>
    /// Класс абстрактного устройства.
    /// </summary>
    public abstract class AbstractDevice : AbstractEquipment
    {
        /// <summary>
        /// Множество способов отправки данных, по которым данное устройство отправляет данные
        /// </summary>
        public DataChannelCommunication[] SendingCommunications { get; set; }

        /// <summary>
        /// Требуется ли питание от электрической сети 220В для работы устройства
        /// </summary>
        public bool IsPowerRequired { get; set; }

        /// <summary>
        /// Время в часах автономной работы от аккумуляторных батарей, если имеются
        /// </summary>
        public TimeSpan BatteryTime { get; set; }

        /// <summary>
        /// Стоимость сервисного обслуживания для замены аккумуляторной батареи 
        /// </summary>
        public double BatteryServicePrice { get; set; }

        /// <summary>
        /// Рассчитать затраты на использование данного инструмента для формирования сети.
        /// </summary>
        /// <param name="project">Свойства проекта.</param>
        /// <param name="vertex">Вершина графа, в которой установлен инструмент.</param>
        /// <returns>Значение выбранных затрат на данный инструмент.</returns>
        public double GetCost(Project project, TopologyVertex vertex)
        {
            try
            {
                var cost = 0.0;

                var vertexLaboriousness = 1 + vertex.LaboriousnessWeight / 10;

                // TODO: добавить в свойства проекта час работы, чтобы можно было точнее считать затраты на всё вместе
                if (project.MinimizationGoal == CostType.Time || project.MinimizationGoal == CostType.All)
                {
                    // Для времени важно только время на установку оборудования, которое может возрасти втрое из-за трудоемкости
                    cost += InstallationTime.TotalHours * vertexLaboriousness;
                }
                else if (project.MinimizationGoal != CostType.Time)
                {
                    // Если не используем мастную рабочую силу, то умножаем стоимость установки 
                    // на трудоемкость проведения работ, которая может увеличить стоимость втрое
                    cost += PurchasePrice + (project.UseLocalEmployee ? 0 : InstallationPrice * vertexLaboriousness);

                    // Если учитываем стоимость обслуживания или всё подряд
                    if (project.MinimizationGoal == CostType.InstantAndMaintenanceMoney || project.MinimizationGoal == CostType.All)
                    {
                        // Если не используем местную рабочую силу и на участке нет питания или оно вообще не требуется, 
                        // то добавляем стоимость замены батареек за весь период эксплуатации
                        if (!project.UseLocalEmployee && !(IsPowerRequired && vertex.Region.HasPower) && BatteryTime.TotalHours > 0)
                            cost += (new TimeSpan((int)project.UsageMonths * 30, 0, 0, 0).TotalHours / BatteryTime.TotalHours) * BatteryServicePrice * vertexLaboriousness;
                    }
                }

                return cost;
            }
            catch (Exception ex)
            {
                Console.WriteLine("MeasurementAndControlDevice GetCost failed! {0}", ex.Message);
                return 0;
            }
        }
    }
}
