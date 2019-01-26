using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TopologyModel.Enumerations;
using TopologyModel.Regions;
using TopologyModel.TopologyGraphs;

namespace TopologyModel
{
    /// <summary>
    /// Класс всего проекта.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Высота учитываемого размера всей территории предприятия.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// Ширина учитываемого размера всей территории предприятия.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// Бюджет всего проекта, за рамки которого затраты на реализацию проекта не должны выходить.
        /// </summary>
        public double Budget { get; set; }

        /// <summary>
        /// Планируемый период эксплуатации системы для расчёта затрат на её использование во времени.
        /// </summary>
        public uint UsageMonths { get; set; }

        /// <summary>
        /// Требуется ли использовать локальный сервер, иначе будет использоваться удалённый сервер.
        /// </summary>
        public bool UseLocalServer { get; set; }

        /// <summary>
        /// Допустимо ли проведение монтажных работ его сотрудниками.
        /// </summary>
        public bool UseLocalEmployee { get; set; }

        /// <summary>
        /// будет ли доступен выход в сеть Интернет из локальной сети предприятия
        /// </summary>
        public bool IsInternetAvailable { get; set; }


        /// <summary>
        /// Абонентская плата в месяц за использование и обслуживание локального сервера.
        /// </summary>
        public double LocalServerMonthlyPayment { get; set; }

        /// <summary>
        /// Абонентская плата в месяц за использование и обслуживание предоставляемого удалённого сервера.
        /// </summary>
        public double RemoteServerMonthlyPayment { get; set; }


        /// <summary>
        /// Словарь весовых коэффициентов параметров проекта, где ключ - это наименование параметра, а значение - вес.
        /// </summary>
        public Dictionary<string, float> WeightCoefficients { get; set; }

        /// <summary>
        /// Множество стоимостей в месяц абонентского обслуживания технологий мобильной передачи данных на сервер.
        /// Где ключ - это тип соединения, а значение - это стоимость.
        /// </summary>
        public Dictionary<InternetConnection, double> MobileInternetMonthlyPayment { get; set; }


        /// <summary>
        /// Массив всех участков предприятия.
        /// </summary>
        public TopologyRegion[] Regions { get; set; }

        /// <summary>
        /// перечень имеющихся на предприятии ТУУ
        /// </summary>
        public MeasurementAndControlZone[] MCZs { get; set; }

        /// <summary>
        /// База данных доступного инструментария.
        /// </summary>
        public Tools AvailableTools { get; set; }

        /// <summary>
        /// Граф всего предприятия.
        /// </summary>
        public TopologyGraph Graph { get; set; } = new TopologyGraph();

        /// <summary>
        /// Имя dot-файла графа участков предприятия.
        /// </summary>
        public string GraphDotFilename { get; set; } = "unnamed.dot";


        /// <summary>
        /// Инициализировать граф всего предприятия.
        /// </summary>
        /// <returns>True, если операция выполнена успешно.</returns>
        public bool InitializeGraph()
        {
            Console.WriteLine("Initialize the graph... ");

            try
            {
                var verticesMatrix = CreateVerticesMatrix();

                if (verticesMatrix.Length == 0) return false;

                if (!InitializeRegionsGraph(verticesMatrix)) return false;

                if (CreateGraphDotFile())
                    Console.WriteLine("Check the graph dot-file in {0}", GraphDotFilename);

                Console.WriteLine("Done!");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed! {0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Создать матрицу вершин участков для будущего графа.
        /// </summary>
        /// <returns>Матрица вершин участков бдущего графа.</returns>
        protected TopologyVertex[,] CreateVerticesMatrix()
        {
            Console.Write("Create regions matrix... ");

            try
            {
                var verticesMatrix = new TopologyVertex[Height, Width];

                foreach (var region in Regions)         // Перебираем все имеющиеся регионы
                {
                    // Определяем начальные и конечные координаты участков в матрице
                    var startX = region.X - 1;          // В конфигурационном файле координаты начинаются с 1
                    var startY = region.Y - 1;
                    var endX = startX + region.Width;
                    var endY = startY + region.Height;

                    for (var i = startY; i < endY; i++)         // Наполняем матрицу идентификаторами участков по координатам
                        for (var j = startX; j < endX; j++)     
                            // Создаём вершину, привязанную к учаску и задаём её координаты внутри самого участка
                            // +1 из-за того, что в конфигурационном файле координаты начинаются с 1
                            verticesMatrix[i, j] = new TopologyVertex(region, j - region.X + 1, i - region.Y + 1, GetMCZsInVertex(j + 1, i + 1));  
                }

                Console.WriteLine("Done! Result matix: ");

                for (var i = 0; i < Height; i++)                // Выводим матрицу идентификаторов регионов в вершинах для наглядности
                {
                    for (var j = 0; j < Width; j++)
                        // Делаем отступ исходя из строкового представления вершины и максимально трёхзначного идентификатора участка
                        Console.Write("{0,8}", verticesMatrix[i, j].ToString());

                    Console.WriteLine();
                }

                return verticesMatrix;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed! {0}", ex.Message);
                return new TopologyVertex[,] { };
            }
        }

        /// <summary>
        /// Получить массив всех мест учёта и управления в текущем узле графа по его координатам.
        /// </summary>
        /// <param name="x">Координата Х узла графа.</param>
        /// <param name="y">Координата У узла графа.</param>
        /// <returns>Массив мест учёта и управления распологающийся в указанных координатах или null, если там такого нет.</returns>
        protected MeasurementAndControlZone[] GetMCZsInVertex(uint x, uint y)
        {
            if (MCZs == null || !MCZs.Any()) return null;

            // Если координата узла находится в координатах ТУУ, то эта ТУУ в данном узле
            return MCZs.Where(q => x >= q.X && y >= q.Y && x <= q.X + q.Width - 1 && y <= q.Y + q.Height - 1).ToArray();
        }

        /// <summary>
        /// Инициализировать граф всего предприятия на основе матрицы участков предприятия, перебирая каждый её элемент, 
        /// создавая на его основе вершину и генерируя связи в соответствии с имеющимися вершинами.
        /// </summary>
        /// <param name="verticesMatrix">Матрица с вершинами участков предприятия.</param>
        /// <returns>Успешно ли инициализирован граф.</returns>
        protected bool InitializeRegionsGraph(TopologyVertex[,] verticesMatrix)
        {
            Console.Write("Create regions graph... ");

            try
            {
                for (var i = 0; i < Height; i++)                            // Проходимся по матрице
                    for (var j = 0; j < Width; j++)
                    {
                        AddEdges(j, i, verticesMatrix);
                    }

                Console.WriteLine("Done!");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed! {0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Добавить грани для текущей вершины графа.
        /// </summary>
        /// <param name="x">Координата по Х текущей веершины.</param>
        /// <param name="y">Координата по Y текущей веершины.</param>
        /// <param name="verticesMatrix">Матрица всех вершин графа</param>
        private void AddEdges(int x, int y, TopologyVertex[,] verticesMatrix)
        {
            try
            {
                // Обходим все соседние вершины в матрице вокруг текущей вершины матрицы, какие есть
                for (var yShift = -1; yShift < 2; yShift++)
                    for (var xShift = -1; xShift < 2; xShift++)
                    {
                        if (xShift == 0 && yShift == 0) continue;	// Пропускаем текущую вершину

                        var neighborX = x + xShift;                 // Вычисляем позицию соседней вершины
                        var neighborY = y + yShift;

                        // Если нет соседней вершины с такими координатами соседней позиции, пропускаем её
                        if (neighborX < 0 || neighborY < 0 || neighborX >= Width || neighborY >= Height) continue;

                        var vertex = verticesMatrix[y, x];
                        var neighborVertex = verticesMatrix[neighborY, neighborX];

                        if (!Graph.ContainsVertex(vertex))          // Если текущей вершины графа ещё нет, то добавляем её
                            Graph.AddVertex(vertex);

                        if (!Graph.ContainsVertex(neighborVertex))  // Если соседней вершины графа ещё нет, то добавляем и её
                            Graph.AddVertex(neighborVertex);

                        // Если грань между вершинами уже есть, то пропускаем
                        if (Graph.ContainsEdge(vertex, neighborVertex) || Graph.ContainsEdge(neighborVertex, vertex)) continue;

                        var newEdge = new TopologyEdge(vertex, neighborVertex, WeightCoefficients);

                        // Если соседняя вершина не диагональная, то гарантировано добавляем к ней грань
                        if (neighborX == x || neighborY == y)
                            Graph.AddEdge(newEdge);

                        // Добавляем диагональную грань между вершинами, только если вершины ссылаются на один участок
                        else if (neighborVertex.Region == vertex.Region)
                            Graph.AddEdge(newEdge);
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Создать dot-файл с текущим графом предприятия.
        /// </summary>
        /// <returns>True, если операция выполнена успешно.</returns>
        protected bool CreateGraphDotFile()
        {
            Console.Write("Create the graph dot-file... ");

            try
            {
                // Объект алгоритма Graphviz для используемого графа
                var graphviz = new GraphvizAlgorithm<TopologyVertex, TopologyEdge>(Graph);

                graphviz.GraphFormat.RankDirection = GraphvizRankDirection.LR;

                // Добавляем общую метку графу с информацией об участках
                graphviz.GraphFormat.Label = 
                    Regions.Select(q => PrepareJSONForGraphviz(q.GetInfo())).Aggregate("", (current, next) => current + "\r\n\r\n" + next) +
                    MCZs.Select(q => PrepareJSONForGraphviz(q.GetInfo())).Aggregate("", (current, next) => current + "\r\n\r\n" + next); 

                graphviz.FormatVertex += (sender, args) =>
                {
                    // В вершине указываем id участка и координаты внутри участка
                    args.VertexFormatter.Comment = args.Vertex.ToString();                                      
                    args.VertexFormatter.Group = $"{args.Vertex.Region.Id}_{args.Vertex.RegionY}";              // Группируем участки на графе

                    // Добавить наименование участка к его угловому узлу (заменить в файле на xlabel и добавить forcelabels=true)
                    args.VertexFormatter.ToolTip = (args.Vertex.RegionX == 0 && args.Vertex.RegionY == 0)
                        ? args.Vertex.Region.Name
                        : "";

                    SetVertexColor(args);
                };

                // Грани форматируем стандартно с двумя весами каждой грани
                graphviz.FormatEdge += (sender, args) =>
                {
                    args.EdgeFormatter.Label.Value = args.Edge.ToString();      // Указываем метки граней

                    if (args.Edge.IsAcrossTheBorder())                          // Грани через участки окрашиваем в красный цвет
                        args.EdgeFormatter.StrokeColor = Color.Red;

                    if (args.Edge.IsAlongTheBorder())                           // Грани вдоль границ участков окрашиваем в оранжевый цвет
                        args.EdgeFormatter.StrokeColor = Color.Orange;
                };

                graphviz.Generate(new FileDotEngine(), GraphDotFilename);       // Генерируем файл с укзанным именем

                Console.WriteLine("Done!");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed! {0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Подготовить JSON-строку для помещения в Graphviz.
        /// </summary>
        /// <param name="JSONstring>Исходная строка.</param>
        /// <returns>Обработанная строка.</returns>
        protected string PrepareJSONForGraphviz(string JSONstring)
        {
            return JSONstring
                .Replace('\"', '\'')            // Заменяем кавычки для Graphviz
                .Replace("},'", "},\r\n'")      // Добавляем переносы строк для красоты
                .Replace(",", ", ");            // ... и пробелы
        }

        /// <summary>
        /// Задать цвет фона вершины.
        /// </summary>
        /// <param name="args">Аргументы форматирования вершины.</param>
        protected void SetVertexColor(FormatVertexEventArgs<TopologyVertex> args)
        {
            args.VertexFormatter.Style = GraphvizVertexStyle.Filled;

            if (args.Vertex.MCZs != null && args.Vertex.MCZs.Any()) // Вершины с ТУУ окрашиваем в зелёный цвет
                args.VertexFormatter.FillColor = Color.LightGreen;

            else if (!args.Vertex.IsInside())                       // Граничные вершины окрашиваем в жёлтый цвет
                args.VertexFormatter.FillColor = Color.Yellow;

            else
                args.VertexFormatter.FillColor = Color.WhiteSmoke;  // Все остальные вершины - почти белые
        }
    }
}