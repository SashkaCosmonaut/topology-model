using System;
using System.Collections.Generic;
using System.Linq;
using TopologyModel.Enumerations;
using TopologyModel.GA;
using TopologyModel.Graphs;
using TopologyModel.Regions;

namespace TopologyModel.Equipments
{
    /// <summary>
    /// Класс УСПД.
    /// </summary>
    public class DataAcquisitionDevice : AbstractDevice
    {
        /// <summary>
        /// Множество доступных стандартов приёма данных и максимальное количество подключаемых устройств 
        /// </summary>
        public Dictionary<DataChannelCommunication, int> ReceivingCommunications { get; set; }

        /// <summary>
        /// Множество доступных способов передачи данных на сервер
        /// </summary>
        public InternetConnection[] ServerConnections { get; set; }

        /// <summary>
        /// Рассчитать затраты на использование данного УСПД для формирования сети.
        /// </summary>
        /// <param name="project">Свойства проекта.</param>
        /// <param name="vertex">Вершина графа, в которой установлен инструмент.</param>
        /// <returns>Значение выбранных затрат на данный инструмент.</returns>
        public override double GetCost(Project project, TopologyVertex vertex)
        {
            var cost = base.GetCost(project, vertex);

            try
            {
                if (!CanSendViaLocalNetwork(project, vertex.Region))
                {
                    var availableConnections = ServerConnections.Where(q => project.MobileInternetMonthlyPayment.ContainsKey(q));

                    if (!availableConnections.Any())    // Если в проекте не задано не одного тарифа мобильного Интернета, который поддерживается УСПД
                        return TopologyFitness.UNACCEPTABLE;

                    // Берём самый дешёвый способ передачи данных
                    cost += availableConnections.Min(q => project.MobileInternetMonthlyPayment[q]) * project.UsageMonths;

                    cost += PurchasePrice * (1 + vertex.Region.BadMobileInternetSignal / 10); // Учитываем затраты на плохую связь
                }   
            }
            catch (Exception ex)
            {
                Console.WriteLine("DataAcquisitionDevice GetCost failed! {0}", ex.Message);
            }

            return cost;
        }

        /// <summary>
        /// Можно ли передавать данные на сервер через локальную сеть или Интернет.
        /// </summary>
        /// <param name="project">Свойства проекта.</param>
        /// <param name="vertex">Вершина графа, в которой установлен инструмент.</param>
        /// <returns>Результат проверки.</returns>
        protected bool CanSendViaLocalNetwork(Project project, FacilityRegion region)
        {
            try
            {
                return (project.UseLocalServer || project.IsInternetAvailable) &&
                    (region.HasLan && ServerConnections.Contains(InternetConnection.Ethernet) ||
                     region.HasWiFi && ServerConnections.Contains(InternetConnection.WiFi));
            }
            catch (Exception ex)
            {
                Console.WriteLine("CanSendViaLocalNetworks failed! {0}", ex.Message);
                return false;
            }
        }
    }
}
