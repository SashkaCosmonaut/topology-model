using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System;
using System.Collections.Generic;
using System.Linq;
using TopologyModel.Equipments;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс фитнес-функции для топологии.
    /// </summary>
    public class TopologyFitness : IFitness
    {
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

                // 1. Группируем секции по УСПД
                foreach (var dadGroup in topology.Sections.GroupBy(q => q.DADPart))     // 2. Перебираем группы
                {
                    // 2.1. Считаем стоимость УСПД каждой группы, чем больше УСПД в общих группах, тем ниже стоимость
                    fitness += dadGroup.Key.GetCost(project);

                    // 2.2. Считаем суммарную стоимость счётчиков в группе
                    fitness += dadGroup.Sum(q => q.MCDPart.GetCost(project));

                    fitness += dadGroup
                        .GroupBy(q => q.Channel)        // 2.3. Группируем группы секций по используемому КПД
                        .Sum(channelGroup =>            // 2.4. Просуммировать стоимости связий УСПД и КУ по каждому КПД, если они есть
                            GetConnectionCost(dadGroup.Key, channelGroup.Key, channelGroup.Select(q => q.MCDPart).ToArray()));  
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
        /// <param name="dadPart">Часть секции с УСПД.</param>
        /// <param name="dataChannel">Канал, соединяющий УСПД и КУ.</param>
        /// <param name="connectedMCDs">Соединяемые с УСПД КУ по КПД.</param>
        /// <returns>Стоимость путей от УСПД до КУ по КПД.</returns>
        protected double GetConnectionCost(DataAcquisitionSectionPart dadPart, DataChannel dataChannel, MeasurementAndControlSectionPart[] connectedMCDs)
        {
            try
            {
                // Условие разбито на несколько для повышения производительности
                if (!dadPart.DAD.ReceivingCommunications.Keys.Contains(dataChannel.Communication))              // Если УСПД не поддерживает данный канал, дальше можно не смотреть
                    return 999999;
    
                if (connectedMCDs.Any(q => !q.MCD.SendingCommunications.Contains(dataChannel.Communication)))   // Если есть хоть одно КУ, которое не поддерживает канал, то дальше можно не смотреть
                    return 999999;

                // TODO: Проверить, что УСПД поддерживает количество подключенных устройств по КПД и КПД поддерживает количество передаваемых устройств
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetConnectionCost failed! {0}", ex.Message);
                return 999999;
            }
        }
    }
}
