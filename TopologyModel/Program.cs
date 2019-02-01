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

                CreateDotFile(project);             // Сгенерировать исходный граф без отрисовки топологии

                var chromosome = new TopologyChromosome(project);

                var topology = chromosome.Decode();

                CreateDotFile(project, topology);   // Сгенерировать результирующий граф с топологией
            }
            catch (Exception ex)
            {
                Console.WriteLine("Main failed! {0}", ex.Message);
            }
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
        public static void CreateDotFile(Project project, Topology topology = null)
        {
            try
            {
                var graphLabel =
                    project.Regions.Select(q => PrepareJSONForGraphviz(q.GetInfo())).Aggregate("", (current, next) => current + "\r\n\r\n" + next) +
                    project.MCZs.Select(q => PrepareJSONForGraphviz(q.GetInfo())).Aggregate("", (current, next) => current + "\r\n\r\n" + next);

                var filename = topology == null         // Если генерируем файл с отображением топологии, то изменяем имя
                    ? project.GraphDotFilename
                    : GetFilenameWithTopology(project.GraphDotFilename);

                if (project.Graph.GenerateDotFile(filename, graphLabel, topology))
                    Console.WriteLine("Check the graph dot-file in {0}", filename);
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
                    .Replace('\"', '\'')            // Заменяем кавычки для Graphviz
                    .Replace("},'", "},\r\n'")      // Добавляем переносы строк для красоты
                    .Replace(",", ", ");            // ... и пробелы
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
    }
}
