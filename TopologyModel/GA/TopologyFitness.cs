using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System;

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