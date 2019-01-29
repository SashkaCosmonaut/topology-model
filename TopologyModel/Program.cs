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

                var chromosome = new TopologyChromosome(project);

                var topology = chromosome.Topology;

                CreateDotFile(project);
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
        public static void CreateDotFile(Project project)
        {
            try
            {
                var graphLabel =
                    project.Regions.Select(q => PrepareJSONForGraphviz(q.GetInfo())).Aggregate("", (current, next) => current + "\r\n\r\n" + next) +
                    project.MCZs.Select(q => PrepareJSONForGraphviz(q.GetInfo())).Aggregate("", (current, next) => current + "\r\n\r\n" + next);

                if (project.Graph.GenerateDotFile(project.GraphDotFilename, graphLabel))
                    Console.WriteLine("Check the graph dot-file in {0}", project.GraphDotFilename);
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
            return JSONstring
                .Replace('\"', '\'')            // Заменяем кавычки для Graphviz
                .Replace("},'", "},\r\n'")      // Добавляем переносы строк для красоты
                .Replace(",", ", ");            // ... и пробелы
        }
    }
}
