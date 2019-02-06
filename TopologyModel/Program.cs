using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using TopologyModel.GA;

namespace TopologyModel
{
    /// <summary>
    /// Главный класс всей программы.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Главная функция программы.
        /// </summary>
        /// <param name="args">Параметры командной строки.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Main starting...");

            try
            {
                var project = ReadProject();

                if (project == null) return;

                if (!project.InitializeGraph()) return;

                GenerateGraphFile(project);             // Сгенерировать исходный граф без отрисовки топологии

                var topology = CalculateTopologyWithGA(project);

                GenerateGraphFile(project, topology);   // Сгенерировать результирующий граф с топологией

                // если не достигли желаемых значений фитнес функции по деньгам или времени, то в зависимости от приоритета (с наименьшим, т.е. большим значением)
                // удаляем одно из мест учёта из проекта (просто обнуляем), и говорим какое, и запускаем повторно алгоритм 
                // Но граф всё равно надо сгенерировать и показать, что получилось, а с новым рассчётом просто меняем имя на 1, 2 и т.д. когда удаляем места учета
            }
            catch (Exception ex)
            {
                Console.WriteLine("Main failed! {0}", ex.Message);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Считать параметры всего проекта из JSON-файла.
        /// </summary>
        /// <returns>Считанный новый объект проекта.</returns>
        public static Project ReadProject()
        {
            Console.Write("Reading the project config... ");

            try
            {
                using (var sr = File.OpenText("./Configs/Config.json"))
                {
                    var result = JsonConvert.DeserializeObject<Project>(sr.ReadToEnd());

                    Console.WriteLine(result == null ? "Failed!" : "Done!");

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ReadProject failed! {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Создать dot-файл наосновании проекта и других параметров.
        /// </summary>
        /// <param name="project">Объект проекта.</param>
        /// <param name="topology">Предлагаемая методом топология.</param>
        public static void GenerateGraphFile(Project project, Topology topology = null)
        {
            try
            {
                // Везде добавляем переносы строк для красоты
                var graphLabel = topology == null 
                    ? ""
                    : PrepareJSONForGraphviz(topology.GetInfo() ?? "").Replace("{'DAD", "\r\n\r\n{'DAD");

                graphLabel += project.Regions
                    .Select(q => PrepareJSONForGraphviz(q.GetInfo()))
                    .Aggregate("", (current, next) => current + "\r\n\r\n" + next)
                    .Replace("},'", "},\r\n'");

                graphLabel += project.MCZs
                    .Select(q => PrepareJSONForGraphviz(q.GetInfo()))
                    .Aggregate("", (current, next) => current + "\r\n\r\n" + next)
                    .Replace("},'", "},\r\n'");

                var filename = topology == null         // Если генерируем файл с отображением топологии, то изменяем имя
                    ? project.GraphDotFilename
                    : GetFilenameWithTopology(project.GraphDotFilename);

                if (project.Graph.GenerateDotFile(filename, graphLabel, topology))
                    GeneratePDFFile(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateDotFile failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Подготовить JSON-строку для помещения в Graphviz.
        /// </summary>
        /// <param name="JSONstring>Исходная строка.</param>
        /// <returns>Обработанная строка.</returns>
        public static string PrepareJSONForGraphviz(string JSONstring)
        {
            try
            {
                return JSONstring
                    .Replace('\"', '\'')        // Заменяем кавычки для Graphviz
                    .Replace(",", ", ");        // и добавляем пробелы
            }
            catch (Exception ex)
            {
                Console.WriteLine("PrepareJSONForGraphviz failed! {0}", ex.Message);
                return JSONstring;
            }
        }

        /// <summary>
        /// Сгенерировать изменённое имя файла для графа с топологией.
        /// </summary>
        /// <param name="sourceFilename">Исходное имя файла.</param>
        /// <returns>Имя файла с суффиксом "-topology".</returns>
        public static string GetFilenameWithTopology(string sourceFilename)
        {
            try
            {
                return sourceFilename.Insert(sourceFilename.IndexOf('.'), "-topology");
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetFilenameWithTopology failed! {0}", ex.Message);
                return sourceFilename;
            }
        }

        /// <summary>
        /// Сгенерировать с помощью DOT pdf-файл графа.
        /// </summary>
        /// <param name="dotFilename">Имя dot-файла.</param>
        protected static void GeneratePDFFile(string dotFilename)
        {
            try
            {
                Console.Write("Generate the graph pdf file... ");

                var fullDotFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dotFilename);
                var fullPdfFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dotFilename.Replace("dot", "pdf"));

                System.Diagnostics.Process.Start("dot", $"-Tpdf {fullDotFilename} -o {fullPdfFilename}");

                Console.WriteLine("Done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("GeneratePDFFile failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Рассчитать топологию для графа проекта с помощью генетического алгоритма.
        /// </summary>
        /// <param name="project">Объект свойств проекта по внедрению сети.</param>
        /// <returns>Сгенерированная топология или null, если произошла ошибка.</returns>
        public static Topology CalculateTopologyWithGA(Project project)
        {
            try
            {
                Console.Write("Setup GA... ");

                var chromosome = new TopologyChromosome(project);
                var selection = new EliteSelection();
                var crossover = new UniformCrossover(0.5f);
                var mutation = new UniformMutation(true);
                var fitness = new TopologyFitness();
                var population = new Population(50, 50, chromosome);

                var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
                {
                    Termination = new GenerationNumberTermination(100)
                };

                ga.GenerationRan += (c, e) =>
                {
                    Console.WriteLine("Generations: {0}", ga.Population.GenerationsNumber);
                    Console.WriteLine("Time: {0}", ga.TimeEvolving);

                    var bc = ga.Population.BestChromosome as TopologyChromosome;

                    Console.WriteLine("Best solution found with {0} fitness.", bc.Fitness);
                };

                Console.Write("Done!\nRun GA... ");

                ga.Start();

                Console.WriteLine("Done in {0} generations!", ga.GenerationsNumber);

                var bestChromosome = ga.BestChromosome as TopologyChromosome;

                Console.WriteLine("Best solution found with {0} fitness", bestChromosome.Fitness);

                return bestChromosome.Decode();
            }
            catch (Exception ex)
            {
                Console.WriteLine("CalculateResultWithGA failed! {0}", ex.Message);
                return null;
            }
        }
    }
}
