using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System;
using System.Linq;

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
                    fitness += dadGroup.Key.GetCost(topologyChromosome.CurrentProject);                     // Рассчитываем целевую стоимость УСПД в группе
                    fitness += dadGroup.Sum(q => q.MACPart.GetCost(topologyChromosome.CurrentProject));     // Рассчитываем целевую стоимость всех КУ в группе
                }

                // Рассчитываем общую сумму всех полученных выше значений плюс затраты на эксплуатацию во времени 
                // topology.Pathes; 
                // TODO: рассчитать стоимость КПД

                return -fitness;     // Значение общей стоимости и будет результатом фитнес функции
            }
            catch (Exception ex)
            {
                Console.WriteLine("Evaluate failed! {0}", ex.Message);
                return 0;
            }
        }
    }
}
