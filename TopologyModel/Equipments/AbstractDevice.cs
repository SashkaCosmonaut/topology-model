using System;
using System.Collections.Generic;
using TopologyModel.Enumerations;
using TopologyModel.GA;
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
        /// Рассчитать базовые затраты на использование данного оборудования для формирования сети.
        /// </summary>
        /// <param name="project">Свойства проекта.</param>
        /// <param name="vertex">Вершина графа, в которой установлен инструмент.</param>
        /// <returns>Значение выбранных затрат на данный инструмент.</returns>
        public override double GetCost(Project project, TopologyVertex vertex)
        {
            try
            {
                var cost = base.GetCost(project, vertex);

                var laboriousnessFactor = 1 + vertex.LaboriousnessWeight / 10;

                // Если учитываем стоимость обслуживания или всё подряд
                if (project.MinimizationGoal == CostType.InstantAndMaintenanceMoney || project.MinimizationGoal == CostType.All)
                {
                    // Если не используем местную рабочую силу и на участке нет питания или оно вообще не требуется, 
                    // то добавляем стоимость замены батареек за весь период эксплуатации
                    if (!project.UseLocalEmployee && !(IsPowerRequired && vertex.Region.HasPower) && BatteryTime.TotalHours > 0)
                        cost += (new TimeSpan((int)project.UsageMonths * 30, 0, 0, 0).TotalHours / BatteryTime.TotalHours) * BatteryServicePrice * laboriousnessFactor;
                }

                return cost;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AbstractDevice GetCost failed! {0}", ex.Message);
                return TopologyFitness.UNACCEPTABLE;
            }
        }
    }
}
