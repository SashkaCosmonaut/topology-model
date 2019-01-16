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
		/// Рассчитать приспособленность хромосомы.
		/// </summary>
		/// <param name="chromosome"></param>
		/// <returns></returns>
		public double Evaluate(IChromosome chromosome) => throw new System.NotImplementedException();
	}
}