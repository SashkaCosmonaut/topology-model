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
        public const double UNACCEPTABLE = 999999;

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
            var cost = 0.0;

            try
            {
                // Условие разбито на несколько для повышения производительности
                if (!dadPart.DAD.ReceivingCommunications.Keys.Contains(dataChannel.Communication))              // Если УСПД не поддерживает данный канал, дальше можно не смотреть
                    cost += UNACCEPTABLE;
    
                if (connectedMCDs.Any(q => !q.MCD.SendingCommunications.Contains(dataChannel.Communication)))   // Если есть хоть одно КУ, которое не поддерживает канал, то дальше можно не смотреть
                    cost += UNACCEPTABLE;

                if (dadPart.DAD.ReceivingCommunications.Any(q => q.Key == dataChannel.Communication))
                {
                    var dadUsedCommunication = dadPart.DAD.ReceivingCommunications.Single(q => q.Key == dataChannel.Communication);

                    // Проверить, что УСПД поддерживает количество подключенных устройств по КПД
                    if (connectedMCDs.Length > dadUsedCommunication.Value)
                        cost += UNACCEPTABLE;
                }
                else
                    cost += UNACCEPTABLE;

                // Найти все пути, соединяющие УСПД и все КУ, присоединённые по данному КПД
                var pathes = TopologyPathfinder.SectionShortestPath(project.Graph, dadPart.Vertex, connectedMCDs.Select(q => q.Vertex), dataChannel);

                if (!IsInRange(pathes, dataChannel))    // Проверить дальность пути и проходимость сквозь участки беспроводной связи 
                    cost += UNACCEPTABLE;

                cost += pathes?.Sum(q => q.GetCost(project)) ?? UNACCEPTABLE;  // Вернуть сумму стоимостей всех составных частей пути, если путь не найден, то плохо - высокая стоимость
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetConnectionCost failed! {0}", ex.Message);
                cost += UNACCEPTABLE;
            }

            return cost;
        }

        /// <summary>
        /// Проверить, что все пути, по которым соеденены устройства удовлетворяют требованию длины.
        /// </summary>
        /// <param name="pathes">Пути, по которым КУ соединены с УСПД.</param>
        /// <param name="dataChannel">КПД, которым соединены устройства.</param>
        /// <returns>True, если удовлентворяют.</returns>
        protected bool IsInRange(IEnumerable<TopologyPath> pathes, DataChannel dataChannel)
        {
            try
            {
                if (pathes == null) return false;

                switch (dataChannel.Topology)
                {
                    // Для звезды и меша проверяем, что длина максимального пути не больше требуемого
                    case DataChannelTopology.Mesh:
                    case DataChannelTopology.Star:
                        return pathes.Max(q => q.GetDistance()) < dataChannel.MaxRange;

                    // Для шины проверяем, что суммарная длина не больше требуемой
                    case DataChannelTopology.Bus:
                        return pathes.Sum(q => q.GetDistance()) < dataChannel.MaxRange;

                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("IsInRange failed! {0}", ex.Message);
                return false;
            }
        }
    }
}
