using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using System;
using TopologyModel.GA;

namespace TopologyModel
{
	public class Program
	{
		/// <summary>
		/// Объект проекта, пока один.
		/// </summary>
		public Project CurrentProject { get; set; } = new Project();

		public static void Main(string[] args)
		{
			var selection = new EliteSelection();
			var crossover = new OrderedCrossover();
			var mutation = new ReverseSequenceMutation();
			var fitness = new TopologyFitness();
			var chromosome = new TopologyChromosome();
			var population = new Population(50, 70, chromosome);

			var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
			{
				Termination = new GenerationNumberTermination(100)
			};

			Console.WriteLine("GA running...");

			ga.Start();

			Console.WriteLine("Best solution found has {0} fitness", ga.BestChromosome.Fitness);
		}
	}
}
