using GeneticSharp.Domain.Chromosomes;

namespace TopologyModel.GA
{
	/// <summary>
	/// Класс хромосомы топологии.
	/// </summary>
	public class TopologyChromosome : ChromosomeBase
	{
		/// <summary>
		/// Change the argument value passed to base construtor to change the length of your chromosome.
		/// </summary>
		public TopologyChromosome() : base(10)
		{
			CreateGenes();
		}

		/// <summary>
		/// Generate a gene base on my problem chromosome representation.
		/// </summary>
		/// <param name="geneIndex"></param>
		/// <returns></returns>
		public override Gene GenerateGene(int geneIndex) => throw new System.NotImplementedException();

		/// <summary>
		/// Создать новую хромосому текущего класса.
		/// </summary>
		/// <returns>Новый объект текущего класса (хромосомы).</returns>
		public override IChromosome CreateNew()
		{
			return new TopologyChromosome();
		}
	}
}
