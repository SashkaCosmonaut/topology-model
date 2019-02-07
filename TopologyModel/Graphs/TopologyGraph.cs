using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TopologyModel.GA;

namespace TopologyModel.Graphs
{
    /// <summary>
    /// Класс ненаправленного графа топологии сети, который содержит вершины, привязанные 
    /// к участкам предприятия, и ненаправленные взвешенные (дробным числом) ребра.
    /// </summary>
    public class TopologyGraph : UndirectedGraph<TopologyVertex, TopologyEdge>
    {
        /// <summary>
        /// Упорядоченный массив вершин графа.
        /// </summary>
        public TopologyVertex[] VerticesArray { get; }

        /// <summary>
        /// Инициализировать граф всего предприятия на основе матрицы участков предприятия, перебирая каждый её элемент, 
        /// создавая на его основе вершину и генерируя связи в соответствии с имеющимися вершинами.
        /// </summary>
        /// <param name="verticesMatrix">Матрица с вершинами участков предприятия.</param>
        /// <param name="weightCoefficients"></param>
        /// <returns>Успешно ли инициализирован граф.</returns>
        public TopologyGraph(TopologyVertex[,] verticesMatrix, Dictionary<string, float> weightCoefficients)
        {
            Console.Write("Create regions graph... ");

            try
            {
                var height = verticesMatrix.GetLength(0);
                var width = verticesMatrix.GetLength(1);

                for (var i = 0; i < height; i++)
                    for (var j = 0; j < width; j++)
                        AddEdges(j, i, verticesMatrix, weightCoefficients);

                VerticesArray = Vertices.ToArray();

                Console.WriteLine("Done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("TopologyGraph failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Добавить грани для текущей вершины графа.
        /// </summary>
        /// <param name="x">Координата по Х текущей веершины.</param>
        /// <param name="y">Координата по Y текущей веершины.</param>
        /// <param name="verticesMatrix">Матрица всех вершин графа</param>
        protected void AddEdges(int x, int y, TopologyVertex[,] verticesMatrix, Dictionary<string, float> weightCoefficients)
        {
            try
            {
                var height = verticesMatrix.GetLength(0);
                var width = verticesMatrix.GetLength(1);

                // Обходим все соседние вершины в матрице вокруг текущей вершины матрицы, какие есть
                for (var yShift = -1; yShift < 2; yShift++)
                    for (var xShift = -1; xShift < 2; xShift++)
                    {
                        if (xShift == 0 && yShift == 0) continue;	// Пропускаем текущую вершину

                        var neighborX = x + xShift;                 // Вычисляем позицию соседней вершины
                        var neighborY = y + yShift;

                        // Если нет соседней вершины с такими координатами соседней позиции, пропускаем её
                        if (neighborX < 0 || neighborY < 0 || neighborX >= width || neighborY >= height) continue;

                        var vertex = verticesMatrix[y, x];
                        var neighborVertex = verticesMatrix[neighborY, neighborX];

                        if (!ContainsVertex(vertex))          // Если текущей вершины графа ещё нет, то добавляем её
                            AddVertex(vertex);

                        if (!ContainsVertex(neighborVertex))  // Если соседней вершины графа ещё нет, то добавляем и её
                            AddVertex(neighborVertex);

                        // Если грань между вершинами уже есть, то пропускаем
                        if (ContainsEdge(vertex, neighborVertex) || ContainsEdge(neighborVertex, vertex)) continue;

                        // Если соседняя вершина не диагональная, то гарантировано добавляем к ней грань
                        if (neighborX == x || neighborY == y)
                            AddEdge(new TopologyEdge(vertex, neighborVertex, weightCoefficients));

                        // Добавляем диагональную грань между вершинами, только если вершины ссылаются на один участок
                        else if (neighborVertex.Region == vertex.Region)
                            AddEdge(new TopologyEdge(vertex, neighborVertex, weightCoefficients));
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddEdges failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Создать dot-файл с текущим графом предприятия.
        /// </summary>
        /// <param name="filename">Имя dot-файла графа.</param>
        /// <param name="graphLabel">Метка для всего графа.</param>
        /// <param name="topology">Отображаемая по желанию предлагаемая методом топология.</param>
        /// <returns>True, если операция выполнена успешно.</returns>
        public bool GenerateDotFile(string filename, string graphLabel, Topology topology = null)
        {
            Console.Write($"Create the graph {(topology == null ? "" : "with topology")} dot-file... ");

            try
            {
                var graphviz = new GraphvizAlgorithm<TopologyVertex, TopologyEdge>(this);

                graphviz.GraphFormat.RankDirection = GraphvizRankDirection.LR;
                graphviz.GraphFormat.Label = graphLabel;


                // Собрать вершины всех УСПД и КУ и настроить отображение вершин
                var dadVertices = topology?.Sections.Select(q => q.DADPart.Vertex) ?? new TopologyVertex[] { };
                var mcdVertices = topology?.Sections.Select(q => q.MCDPart.Vertex) ?? new TopologyVertex[] { };

                SetupVerticesFormat(graphviz, dadVertices, mcdVertices);


                // Сгенерировать дополнительные и настроить отображение граней
                var topologyEdges = GetTopologyEdges(topology);

                AddEdgeRange(topologyEdges.Keys);       // Добавить дополнительные окрашенные грани путей топологии

                SetupEdgesFormat(graphviz, topologyEdges);


                graphviz.Generate(new FileDotEngine(), filename);

                RemoveEdges(topologyEdges.Keys);      // Удалить добавленные дополнительные грани

                Console.WriteLine("Done!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenerateDotFile failed! {0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Получить дополнительные грани топологии для отображения на графе.
        /// </summary>
        /// <param name="topology">Отображаемая по желанию предлагаемая методом топология.</param>
        /// <returns>Словарь, где ключ - грань топологии, а значение - её цвет.</returns>
        protected Dictionary<TopologyEdge, Color> GetTopologyEdges(Topology topology)
        {
            try
            {
                if (topology == null)
                    return new Dictionary<TopologyEdge, Color>();

                return topology.Pathes
                    .SelectMany(q => q.Value)
                    .SelectMany(q =>
                        q.Path?.ToDictionary(w => new TopologyEdge(w.Source, w.Target, labeled: false), w => q.Color)
                        ?? new Dictionary<TopologyEdge, Color>())
                    .ToDictionary(q => q.Key, q => q.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetColoredEdges failed! {0}", ex.Message);
                return new Dictionary<TopologyEdge, Color>();
            }
        }

        /// <summary>
        /// Настроить отображение вершин графа.
        /// </summary>
        /// <param name="graphviz">Объект алгоритма Graphviz для используемого графа.</param>
        /// <param name="dadVertices">Перечисление всех вершин с УСПД в топологии.</param>
        /// <param name="mcdVertices">Перечисление всех вергин с КУ в топологии.</param>
        protected void SetupVerticesFormat(GraphvizAlgorithm<TopologyVertex, TopologyEdge> graphviz,
                                           IEnumerable<TopologyVertex> dadVertices, IEnumerable<TopologyVertex> mcdVertices)
        {
            try
            {
                graphviz.FormatVertex += (sender, args) =>
                {
                    try
                    {
                        // В вершине указываем id участка и координаты внутри участка
                        args.VertexFormatter.Label = args.Vertex.ToString() + "\r\n" + args.Vertex.LaboriousnessWeight;
                        args.VertexFormatter.Group = $"{args.Vertex.Region.Id}_{args.Vertex.RegionY}";      // Группируем участки на графе

                        // Добавить наименование участка к его угловому узлу (заменить в файле на xlabel и добавить forcelabels=true)
                        args.VertexFormatter.ToolTip = (args.Vertex.RegionX == 0 && args.Vertex.RegionY == 0)
                            ? args.Vertex.Region.Name
                            : "";

                        SetVertexColors(args, dadVertices, mcdVertices);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("SetupVerticesFormat failed! {0}", ex.Message);
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("SetupVerticesFormat failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Задать цвета вершины.
        /// </summary>
        /// <param name="args">Аргументы события отрисовки вершины.</param>
        /// <param name="dadVertices">Перечисление всех вершин с УСПД в топологии.</param>
        /// <param name="mcdVertices">Перечисление всех вергин с КУ в топологии.</param>
        protected void SetVertexColors(FormatVertexEventArgs<TopologyVertex> args, IEnumerable<TopologyVertex> dadVertices, IEnumerable<TopologyVertex> mcdVertices)
        {
            try
            {
                args.VertexFormatter.Style = GraphvizVertexStyle.Filled;

                if (args.Vertex.MCZs != null && args.Vertex.MCZs.Any()) // Вершины с ТУУ окрашиваем в зелёный цвет
                    args.VertexFormatter.FillColor = Color.LightGreen;

                else if (!args.Vertex.IsInside())                       // Граничные вершины окрашиваем в жёлтый цвет
                    args.VertexFormatter.FillColor = Color.Yellow;

                else
                    args.VertexFormatter.FillColor = Color.WhiteSmoke;  // Все остальные вершины - почти белые

                HighlightVertex(args, dadVertices, mcdVertices);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SetVertexColor failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Выделить цветом на графе вершины топологии.
        /// </summary>
        /// <param name="args">Аргументы события отрисовки вершины.</param>
        /// <param name="dadVertices">Перечисление всех вершин с УСПД в топологии.</param>
        /// <param name="mcdVertices">Перечисление всех вергин с КУ в топологии.</param>
        protected void HighlightVertex(FormatVertexEventArgs<TopologyVertex> args, IEnumerable<TopologyVertex> dadVertices, IEnumerable<TopologyVertex> mcdVertices)
        {
            try
            {
                var dadFound = dadVertices.Contains(args.Vertex);
                var mcdFound = mcdVertices.Contains(args.Vertex);

                // Окрашиваем контуры вершинам в соответствии с устройствами на них
                if (dadFound)
                {
                    args.VertexFormatter.Shape = GraphvizVertexShape.Box;
                    args.VertexFormatter.StrokeColor = Color.Red;
                    args.VertexFormatter.FontColor = Color.Red;
                }

                if (mcdFound)
                {
                    args.VertexFormatter.Shape = dadFound ? GraphvizVertexShape.Hexagon : GraphvizVertexShape.Box;   // Если в вершине и счётчик, и УСПД, то форма особенная
                    args.VertexFormatter.StrokeColor = Color.Blue;
                    args.VertexFormatter.FontColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HighlightVertex failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Настроить отображение граней графа стандартно с двумя весами каждой грани.
        /// </summary>
        /// <param name="graphviz">Объект алгоритма Graphviz для используемого графа.</param>
        /// <param name="coloredEdges">Перечисление окрашенных путей в графе.</param>
        protected void SetupEdgesFormat(GraphvizAlgorithm<TopologyVertex, TopologyEdge> graphviz, Dictionary<TopologyEdge, Color> coloredEdges)
        {
            try
            {
                graphviz.FormatEdge += (sender, args) =>
                {
                    try
                    {
                        args.EdgeFormatter.Label.Value = args.Edge.ToString();      // Указываем метки граней

                        if (args.Edge.IsAcrossTheBorder())                          // Грани через участки окрашиваем в красный цвет
                            args.EdgeFormatter.StrokeColor = Color.Red;

                        if (args.Edge.IsAlongTheBorder())                           // Грани вдоль границ участков окрашиваем в оранжевый цвет
                            args.EdgeFormatter.StrokeColor = Color.Orange;

                        HighlightEdge(args, coloredEdges);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("SetupEdgesFormat failed! {0}", ex.Message);
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("SetupEdgesFormat failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Выделить цветом на графе связи топологии между вершинами графа. 
        /// </summary>
        /// <param name="args">Аргументы события отрисовки грани.</param>
        /// <param name="coloredEdges">Перечисление окрашенных путей в графе.</param>
        protected void HighlightEdge(FormatEdgeEventArgs<TopologyVertex, TopologyEdge> args, Dictionary<TopologyEdge, Color> coloredEdges)
        {
            try
            {
                if (!coloredEdges.ContainsKey(args.Edge)) return;

                args.EdgeFormatter.FontColor = coloredEdges[args.Edge];
                args.EdgeFormatter.StrokeColor = coloredEdges[args.Edge];
            }
            catch (Exception ex)
            {
                Console.WriteLine("HighlightEdge failed! {0}", ex.Message);
            }
        }
    }
}
