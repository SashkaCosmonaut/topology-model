using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System;
using System.Linq;
using TopologyModel.TopologyGraphs;

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

                foreach (var dadGroup in topology.Sections.GroupBy(q => q.DADPart))                         // Группируем секции по частям с УСПД и перебираем группы
                {
                    fitness += dadGroup.Sum(q => q.MACPart.GetCost(topologyChromosome.CurrentProject));     // Рассчитываем целевую стоимость всех КУ в группе
                    fitness += dadGroup.Sum(q => q.DADPart.GetCost(topologyChromosome.CurrentProject));     // Рассчитываем целевую стоимость всех УСПД в группе

                    foreach (var channelGroup in dadGroup.GroupBy(q => q.Channel))                          // Группы секций группируем по каналам передачи данных, которые связывают КУ и УСПД 
                    {
                        if (channelGroup.Count() > channelGroup.Key.MaxDevicesConnected)                    // Если в группе больше устройств, чам можно подключить по КПД
                            return Double.MaxValue;                                                         // С помощью данной хромосомы нельзя соединить устройства, прерываем поиск

                        // Находим путь между УСПД и всеми счётчиками по каждому каналу передачи в группе секций
                        var path = TopologyPathfinder.SectionShortestPath(topologyChromosome.CurrentProject.Graph, dadGroup.Key.Vertex, channelGroup.Select(q => q.MACPart.Vertex), channelGroup.Key);

                        // Рассчитываем стоимость денег или времени пути
                        // TODO: Надо учитывать режимы, по какому критерию считаются стоимости - по деньгам и по времени
                    }
                }

                // Рассчитываем общую сумму всех полученных выше значений плюс затраты на эксплуатацию во времени 

                return fitness;     // Значение общей стоимости и будет результатом фитнес функции
            }
            catch (Exception ex)
            {
                Console.WriteLine("Evaluate failed! {0}", ex.Message);
                return 0;
            }
		}
	}
}