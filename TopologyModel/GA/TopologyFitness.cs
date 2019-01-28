using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

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
            return 0;
		}
	}
}