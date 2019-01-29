using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TopologyModel.TopologyGraphs
{
	/// <summary>
	/// Класс ненаправленного графа топологии сети, который содержит вершины, привязанные 
	/// к участкам предприятия, и ненаправленные взвешенные (дробным числом) ребра.
	/// </summary>
	public class TopologyGraph : UndirectedGraph<TopologyVertex, TopologyEdge>
	{
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

                Console.WriteLine("Done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed! {0}", ex.Message);
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

                        var newEdge = new TopologyEdge(vertex, neighborVertex, weightCoefficients);

                        // Если соседняя вершина не диагональная, то гарантировано добавляем к ней грань
                        if (neighborX == x || neighborY == y)
                            AddEdge(newEdge);

                        // Добавляем диагональную грань между вершинами, только если вершины ссылаются на один участок
                        else if (neighborVertex.Region == vertex.Region)
                            AddEdge(newEdge);
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
        public bool CreateDotFile(string filename, string graphLabel)
        {
            Console.Write("Create the graph dot-file... ");

            try
            {
                // Объект алгоритма Graphviz для используемого графа
                var graphviz = new GraphvizAlgorithm<TopologyVertex, TopologyEdge>(this);

                graphviz.GraphFormat.RankDirection = GraphvizRankDirection.LR;

                // Добавляем общую метку графу с информацией об участках
                graphviz.GraphFormat.Label = graphLabel;

                graphviz.FormatVertex += (sender, args) =>
                {
                    // В вершине указываем id участка и координаты внутри участка
                    args.VertexFormatter.Comment = args.Vertex.ToString();
                    args.VertexFormatter.Group = $"{args.Vertex.Region.Id}_{args.Vertex.RegionY}";      // Группируем участки на графе

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

                graphviz.Generate(new FileDotEngine(), filename);       // Генерируем файл с укзанным именем

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
