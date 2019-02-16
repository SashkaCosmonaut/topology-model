using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System;
using System.Collections.Generic;
using System.Linq;
using TopologyModel.Enumerations;
using TopologyModel.Equipments;
using TopologyModel.Graphs;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс фитнес-функции для топологии.
    /// </summary>
    public class TopologyFitness : IFitness
    {
        /// <summary>
        /// Значение фитнес-функции для недопустимого случая.
        /// </summary>
        public const double UNACCEPTABLE = 999_999_999_999;

        /// <summary>
        /// Значение фитнес-функции для очень плохого случая.
        /// </summary>
        public const double VERY_BAD = 999_999_999;

        /// <summary>
        /// Значение фитнес-функции для плохого случая.
        /// </summary>
        public const double BAD = 999_999;

        /// <summary>
        /// Оценить приспособленность хромосомы топологии.
        /// </summary>
        /// <param name="chromosome">Оцениваемая хромосома с топологией.</param>
        /// <returns>Результат оценки.</returns>
        public double Evaluate(IChromosome chromosome)
        {
            try
            {
                var topologyChromosome = chromosome as TopologyChromosome;

                if (topologyChromosome == null) return 0;

                var topology = topologyChromosome.Decode();

                var fitness = 0.0;  // Результирующее значение функции

                var project = topologyChromosome.CurrentProject;

                // 0. Считаем стоимость использования сервера
                if (project.UseLocalServer)
                    fitness += project.LocalServerMonthlyPayment * project.UsageMonths;
                else
                    fitness += project.RemoteServerMonthlyPayment * project.UsageMonths;

                // 1. Группируем секции по УСПД. 2. Перебираем группы
                foreach (var dadGroup in topology.Sections.GroupBy(q => q.DADPart, new DataAcquisitionSectionPartComparer()))
                {
                    // 2.1. Считаем стоимость УСПД каждой группы, чем больше УСПД в общих группах, тем ниже стоимость
                    fitness += dadGroup.Key.GetCost(project);

                    // 2.2. Считаем суммарную стоимость счётчиков в группе
                    fitness += dadGroup.Sum(q => q.MCDPart.GetCost(project));

                    fitness += dadGroup
                        .GroupBy(q => q.Channel)        // 2.3. Группируем группы секций по используемому КПД
                        .Sum(channelGroup =>            // 2.4. Просуммировать стоимости связий УСПД и КУ по каждому КПД, если они есть
                            GetConnectionCost(topologyChromosome.CurrentProject, dadGroup.Key, channelGroup.Select(q => q.MCDPart).ToArray(), channelGroup.Key));  
                }

                return -fitness;     // Значение общей стоимости и будет результатом фитнес функции
            }
            catch (Exception ex)
            {
                Console.WriteLine("Evaluate failed! {0}", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Проверить, что КУ и УСПД могут связываться по данному КПД, найти путь от КПД до всех КУ и рассчитать его стоимость.
        /// </summary>
        /// <param name="project">Свойства текущего проекта.</param>
        /// <param name="dadPart">Часть секции с УСПД.</param>
        /// <param name="dataChannel">Канал, соединяющий УСПД и КУ.</param>
        /// <param name="connectedMCDs">Соединяемые с УСПД КУ по КПД.</param>
        /// <returns>Стоимость путей от УСПД до КУ по КПД.</returns>
        protected double GetConnectionCost(Project project, DataAcquisitionSectionPart dadPart, MeasurementAndControlSectionPart[] connectedMCDs, DataChannel dataChannel)
        {
            try
            {
                if (connectedMCDs.Any(q => !q.MCD.SendingCommunications.Contains(dataChannel.Communication)))   // Если есть хоть одно КУ, которое не поддерживает канал, то дальше можно не смотреть
                    return 2 * UNACCEPTABLE;

                if (!dadPart.DAD.ReceivingCommunications.Keys.Contains(dataChannel.Communication))              // Если УСПД не поддерживает данный канал, дальше можно не смотреть
                    return UNACCEPTABLE;

                var dadUsedCommunication = dadPart.DAD.ReceivingCommunications.Single(q => q.Key == dataChannel.Communication);

                // Проверить, что УСПД поддерживает количество подключенных устройств по КПД
                if (connectedMCDs.Length > dadUsedCommunication.Value)
                    return VERY_BAD * (connectedMCDs.Length - dadUsedCommunication.Value);
                
                // Найти все пути, соединяющие УСПД и все КУ, присоединённые по данному КПД
                var pathes = TopologyPathfinder.GetShortestPath(project.Graph, dadPart.Vertex, connectedMCDs.Select(q => q.Vertex), dataChannel);

                // Проверить длину пути и проходимость сквозь участки беспроводной связи 
                var distance = GetMinDistance(pathes, dataChannel);

                if (distance > dataChannel.MaxRange)
                    return distance * BAD;        // Чем дальше, тем хуже значение фитнес-функции  

                return pathes?.Sum(q => q.GetCost(project)) ?? UNACCEPTABLE;  // Вернуть сумму стоимостей всех составных частей пути, если путь не найден, то плохо - высокая стоимость
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetConnectionCost failed! {0}", ex.Message);
                return 10 * UNACCEPTABLE;
            }
        }

        /// <summary>
        /// Получить минимальную длину пути (для звезды и mesh) или всего пути для шины.
        /// </summary>
        /// <param name="pathes">Пути, по которым КУ соединены с УСПД.</param>
        /// <param name="dataChannel">КПД, которым соединены устройства.</param>
        /// <returns>Значение расстояния.</returns>
        protected double GetMinDistance(IEnumerable<TopologyPath> pathes, DataChannel dataChannel)
        {
            try
            {
                if (pathes == null) return UNACCEPTABLE;

                switch (dataChannel.Topology)
                {
                    // Для звезды и меша проверяем, что длина максимального пути не больше требуемого
                    case DataChannelTopology.Mesh:
                    case DataChannelTopology.Star:
                        return pathes.Max(q => q.GetDistance());

                    // Для шины проверяем, что суммарная длина не больше требуемой
                    case DataChannelTopology.Bus:
                        return pathes.Sum(q => q.GetDistance());

                    default:
                        return UNACCEPTABLE;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetDistance failed! {0}", ex.Message);
                return UNACCEPTABLE;
            }
        }
    }
}
