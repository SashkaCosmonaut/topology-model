using System;
using TopologyModel.Enumerations;
using TopologyModel.TopologyGraphs;

namespace TopologyModel.Tools
{
	/// <summary>
	/// Класс абстрактного устройства.
	/// </summary>
	public abstract class AbstractDevice : AbstractTool
	{
		/// <summary>
		/// множество стандартов отправки данных, по которым данное устройство отправляет данные
		/// </summary>
		public Protocol[] SendingProtocols { get; set; }

		/// <summary>
		/// требуется ли питание от электрической сети 220В для работы устройства
		/// </summary>
		public bool IsPowerRequired { get; set; }

		/// <summary>
		/// время в часах автономной работы от аккумуляторных батарей, если имеются
		/// </summary>
		public TimeSpan BatteryTime { get; set; }

		/// <summary>
		/// стоимость сервисного обслуживания для замены аккумуляторной батареи 
		/// </summary>
		public double BatteryServicePrice { get; set; }

        /// <summary>
        /// Рассчитать затраты на использование данного инструмента для формирования сети.
        /// </summary>
        /// <param name="project">Свойства проекта.</param>
        /// <param name="vertex">Вершина графа, в которой установлен инструмент.</param>
        /// <returns>Значение выбранных затрат на данный инструмент.</returns>
        public override double GetCost(Project project, TopologyVertex vertex)
        {
            try
            {
                var cost = 0.0;

                if (project.MinimizationGoal == CostType.Time)
                {
                    cost = InstallationTime.TotalHours;     // Для времени важно только время а установку оборудования
                }
                else
                {
                    // Умножаем стоимость установки на трудоемкость проведения работ (которая максимум может увеличить стоимость втрое)
                    cost += PurchasePrice + InstallationPrice * vertex.LaboriousnessWeight / 10;

                    // Если учитываем стоимость обслуживания, то добавляем стоимость замены батареек, 
                    // если на участке нет питания или оно вообще не требуется и задано время работы от баратеек
                    if (project.MinimizationGoal == CostType.InstantAndMaintenanceMoney &&
                        !(IsPowerRequired && vertex.Region.HasPower) && BatteryTime.TotalMilliseconds > 0)
                        cost += (new TimeSpan((int)project.UsageMonths * 30, 0, 0, 0).TotalHours / BatteryTime.TotalHours) * BatteryServicePrice;
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
