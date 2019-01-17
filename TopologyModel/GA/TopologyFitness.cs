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
		public double Evaluate(IChromosome chromosome)
		{
			double n = 9;

			var ch = chromosome as TopologyChromosome;

			if (ch == null) return 0;

			return (ch.X + ch.Y) / ch.Z;
		}
	}
}