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
			var crossover = new OnePointCrossover(0);
			var mutation = new UniformMutation(true);
			var fitness = new TopologyFitness();
			var chromosome = new TopologyChromosome();
			var population = new Population(50, 50, chromosome);

			var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
			ga.Termination = new GenerationNumberTermination(100);
			ga.GenerationRan += (c, e) => {
				var bc = ga.Population.BestChromosome as TopologyChromosome;

				Console.WriteLine("Generations: {0}", ga.Population.GenerationsNumber);
				Console.WriteLine("Time: {0}", ga.TimeEvolving);
				Console.WriteLine("Best solution found is X:{0}, Y:{1}, Z:{2} with {3} fitness.",
				bc.X, bc.Y, bc.Z, bc.Fitness);
			};

			Console.WriteLine("GA running...");
			ga.Start();
			Console.WriteLine("GA done in {0} generations.", ga.GenerationsNumber);

			var bestChromosome = ga.BestChromosome as TopologyChromosome;
			Console.WriteLine("Best solution found is X:{0}, Y:{1}, Z:{2} with {3} fitness.", 
				bestChromosome.X, bestChromosome.Y, bestChromosome.Z, bestChromosome.Fitness);
			Console.ReadKey();
		}
	}
}
