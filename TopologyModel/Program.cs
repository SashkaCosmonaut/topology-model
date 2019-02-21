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
        /// Количеств повторений запусков генерации топологий.
        /// </summary>
        public const int NUMBER_OF_RUNS = 1;

        /// <summary>
        /// Размер популяции в ГА.
        /// </summary>
        public const int POPULATION_SIZE = 100;

        /// <summary>
        /// Количество поколений, генерируемых ГА.
        /// </summary>
        public const int NUMBER_OF_GENERATIONS = 100;

        /// <summary>
        /// Имя папки с конфигурационными файлами.
        /// </summary>
        public const string CONFIG_DIR_NAME = "Configs";

        /// <summary>
        /// Имя папки для выходных файлов.
        /// </summary>
        public const string OUTPUT_DIR_NAME = "Output";

        /// <summary>
        /// Главная функция программы.
        /// </summary>
        /// <param name="args">Параметры командной строки.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Main starting...");

            try
            {
                MultipleRuns(Path.Combine(CONFIG_DIR_NAME, "Config.json"));

                MultipleRuns(Path.Combine(CONFIG_DIR_NAME, "Tests huge.json"));

                MultipleRuns(Path.Combine(CONFIG_DIR_NAME, "Tests.json"));

                // TODO: если не достигли желаемых значений фитнес функции по деньгам или времени, то в зависимости от приоритета (с наименьшим, т.е. большим значением)
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
        /// Выполнить множество запусков ГА для генерации нескольких результрующих топологий.
        /// </summary>
        /// <param name="configFilename">Имя конфигурационного файла проекта.</param>
        protected static void MultipleRuns(string configFilename)
        {
            var project = ReadProject(configFilename);

            if (project == null) return;

            if (!project.InitializeGraph()) return;

            GenerateGraphFile(project);

            Console.WriteLine($"Run {configFilename}...");

            for (int i = 0; i < NUMBER_OF_RUNS; i++)
            {
                Console.WriteLine($"Now running {i} of 10...");

                GenerateGraphFile(project, CalculateTopologyWithGA(project));

                Console.WriteLine("Done!");
            }

            Console.WriteLine($"{configFilename} done!");
        }

        /// <summary>
        /// Считать параметры всего проекта из JSON-файла.
        /// </summary>
        /// <param name="configFilename">Имя конфигурационного файла проекта.</param>
        /// <returns>Считанный новый объект проекта.</returns>
        public static Project ReadProject(string configFilename)
        {
            Console.Write("Reading the project config... ");

            try
            {
                using (var sr = File.OpenText(configFilename))
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

                var filename = GenerateFilename(project.GraphDotFilename, topology);

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
        /// <param name="topology">Сгенерированная топология.</param>
        /// <returns>Имя файла с суффиксом "-topology".</returns>
        public static string GenerateFilename(string sourceFilename, Topology topology)
        {
            try
            {
                return Path.Combine(OUTPUT_DIR_NAME,
                    sourceFilename.Insert(sourceFilename.IndexOf('.'),
                        $"{(topology != null ? "-topology" : "")}-{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}"));
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
                var population = new Population(POPULATION_SIZE, POPULATION_SIZE, chromosome);

                var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
                {
                    Termination = new GenerationNumberTermination(NUMBER_OF_GENERATIONS)
                };

                // Записать значения в csv файл и строить график фитнес-функции
                using (StreamWriter streamwriter = File.AppendText(Path.Combine(OUTPUT_DIR_NAME, $"fitness-{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}.csv")))
                {
                    ga.GenerationRan += (c, e) =>
                    {
                        Console.WriteLine("Generations: {0}", ga.Population.GenerationsNumber);
                        Console.WriteLine("Time: {0}", ga.TimeEvolving);

                        var bc = ga.Population.BestChromosome as TopologyChromosome;

                        streamwriter.WriteLine($"{ga.Population.GenerationsNumber};{ga.TimeEvolving:hh\\:mm\\:ss};{bc.Fitness:0.00};");

                        Console.WriteLine("Best solution found with {0} fitness.", bc.Fitness);
                    };

                    Console.Write("Done!\nRun GA... ");

                    ga.Start();
                }

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
