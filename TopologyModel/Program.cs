using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Collections;
using QuickGraph.Serialization;
using System;
using TopologyModel.GA;
using System.IO;
using System.Xml;
using QuickGraph.Algorithms;
using System.Collections.Generic;

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
			GraphTest();
			//GATest();
		}

		public static void GATest()
		{
			var selection = new EliteSelection();
			var crossover = new OnePointCrossover(0);
			var mutation = new UniformMutation(true);
			var fitness = new TopologyFitness();
			var chromosome = new TopologyChromosome();
			var population = new Population(50, 50, chromosome);

			var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
			ga.Termination = new GenerationNumberTermination(100);
			ga.GenerationRan += (c, e) =>
			{
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

		public static void GraphTest()
		{
			//var g = new <string, Edge<string>>();
			//using (var reader = new StreamReader(""))
			//{
			//	g.DeserializeFromGraphML(
			//		reader,
			//		id => id,
			//		(source, target, id) => new Edge<string>(source, target)
			//		);
			//}

			var edges = new UndirectedEdge<int>[] {
				new UndirectedEdge<int>(1, 2),
				new UndirectedEdge<int>(0, 1),
				new UndirectedEdge<int>(0, 3),
				new UndirectedEdge<int>(2, 3)
			};

			//var graph = edges.ToAdjacencyGraph<int, UndirectedEdge<int>>();





			//var cities = edges.ToBidirectionalGraph<int, UndirectedEdge<int>>();
			var cities = new UndirectedGraph<int, UndirectedEdge<int>>();
			cities.AddVerticesAndEdgeRange(edges);

			var weights = new int[,]
			{
				{ 0, 4, 100, 1 },
				{ 4, 0, 2, 100 },
				{ 100, 2, 0, 3 },
				{ 1, 100, 3, 0 }
			};


			using (StreamWriter sw = File.CreateText("test.svg"))
			{
				sw.Write(cities.ToSvg());
			}

			Func<UndirectedEdge<int>, double> cityDistances = (e) =>
			{
				return weights[e.Source, e.Target];
			};

			int sourceCity = 1; // starting city
			int targetCity = 0; // ending city

			// vis can create all the shortest path in the graph
			// and returns a delegate that can be used to retreive the graphs
			var tryGetPath = cities.ShortestPathsDijkstra(cityDistances, sourceCity);
			
			// enumerating path to targetCity, if any
			IEnumerable<UndirectedEdge<int>> path;

			if (tryGetPath(targetCity, out path))
				foreach (var e in path)
					Console.WriteLine(e);


			GraphvizAlgorithm<int, UndirectedEdge<int>> graphviz = new GraphvizAlgorithm<int, UndirectedEdge<int>>(cities);
			graphviz.FormatVertex += (sender, args) => args.VertexFormatter.Comment = args.Vertex.ToString();
			graphviz.FormatEdge += (sender, args) => { args.EdgeFormatter.Label.Value = weights[args.Edge.Source, args.Edge.Target].ToString(); };

			graphviz.Generate(new FileDotEngine(), "test.dot");


			//var graph = new UndirectedGraph<int, TaggedUndirectedEdge<int, int>>();

			//var e1 = new TaggedUndirectedEdge<int, int>(1, 2, 57);//dem(1, 2, 57).
			//var e2 = new TaggedUndirectedEdge<int, int>(1, 4, 65);//dem(1, 4, 65).

			//graph.AddVerticesAndEdge(e1);
			//graph.AddVerticesAndEdge(e2);


			//using (var writer = new StringWriter())
			//{
			//	var settins = new XmlWriterSettings();
			//	settins.Indent = true;
			//	using (var xwriter = XmlWriter.Create(writer, settins))
			//		graph.SerializeToGraphML<int, TaggedUndirectedEdge<int, int>, UndirectedGraph<int, TaggedUndirectedEdge<int, int>>>(
			//			xwriter);

			//	using (StreamWriter sw = File.CreateText("test.graphml"))
			//	{
			//		sw.Write(writer.ToString());
			//	}
			//}

		}
	}
}
