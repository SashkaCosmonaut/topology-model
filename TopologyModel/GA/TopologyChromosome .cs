using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace TopologyModel.GA
{
	/// <summary>
	/// Класс хромосомы топологии.
	/// </summary>
	public class TopologyChromosome : ChromosomeBase
	{
		public TopologyChromosome() : base (3)
		{
			ReplaceGene(0, GenerateGene(0));
			ReplaceGene(1, GenerateGene(1));
			ReplaceGene(2, GenerateGene(2));
		}

		// These properties represents your phenotype.
		public int X
		{
			get {
				return (int)GetGene(0).Value;
			}
		}

		public int Y
		{
			get {
				return (int)GetGene(1).Value;
			}
		}

		public int Z
		{
			get {
				return (int)GetGene(2).Value;
			}
		}

		public override Gene GenerateGene(int geneIndex)
		{
			int value;

			if (geneIndex == 0)
			{
				value = RandomizationProvider.Current.GetInt(0, 101);
			}
			else if (geneIndex == 1)
			{
				value = RandomizationProvider.Current.GetInt(0, 101);
			}
			else
			{
				value = RandomizationProvider.Current.GetInt(1, 101);
			}

			return new Gene(value);
		}

		public override IChromosome CreateNew()
		{
			return new TopologyChromosome();
		}

		public override IChromosome Clone()
		{
			var clone = base.Clone() as TopologyChromosome;

			return clone;
		}
	}
}
