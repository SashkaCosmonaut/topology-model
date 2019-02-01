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

                // Надо сделать режимы, по какому критерию считаются стоимости - по деньгам и по времени, надо добавить в проект поле 
                        
                // Группируем секции по частям с УСПД и перебираем группы
                foreach (var dadGroup in topology.Sections.GroupBy(q => q.DADPart))
                {
                    // Рассчитываем стоимость денег или времени установки и обслуживания всех счётчиков в группе
                    // Рассчитываем стоимость денег или времени  установки и обслуживания УСПД
                    // Находим путь между УСПД и всеми счётчиками по указанной в канале топологии
                    // Рассчитываем стоимость денег или времени пути
                }

                // Рассчитываем общую сумму всех полученных выше значений плюс затраты на эксплуатацию во времени 
                // Значение общей стоимости и будет результатом фитнес функции

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Evaluate failed! {0}", ex.Message);
                return 0;
            }
		}
	}
}