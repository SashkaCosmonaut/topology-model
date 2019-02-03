﻿using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TopologyModel.GA;

namespace TopologyModel.TopologyGraphs
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
                // Объект алгоритма Graphviz для используемого графа
                var graphviz = new GraphvizAlgorithm<TopologyVertex, TopologyEdge>(this);

                graphviz.GraphFormat.RankDirection = GraphvizRankDirection.LR;

                // Добавляем общую метку графу с информацией об участках
                graphviz.GraphFormat.Label = graphLabel;

                // Собираем вершины всех УСПД и КУ, а также все грани
                var dadVertices = topology?.Sections.Select(q => q.DADPart.Vertex) ?? new TopologyVertex[] { };
                var mcdVertices = topology?.Sections.Select(q => q.MACPart.Vertex) ?? new TopologyVertex[] { };
                var edges = topology?.Pathes.Values.SelectMany(q => q.Values.SelectMany(w => w)) ?? new TopologyEdge[] { };

                graphviz.FormatVertex += (sender, args) =>
                {
                    // В вершине указываем id участка и координаты внутри участка
                    args.VertexFormatter.Label = args.Vertex.ToString() + "\r\n" + args.Vertex.LaboriousnessWeight;
                    args.VertexFormatter.Group = $"{args.Vertex.Region.Id}_{args.Vertex.RegionY}";      // Группируем участки на графе

                    // Добавить наименование участка к его угловому узлу (заменить в файле на xlabel и добавить forcelabels=true)
                    args.VertexFormatter.ToolTip = (args.Vertex.RegionX == 0 && args.Vertex.RegionY == 0)
                        ? args.Vertex.Region.Name
                        : "";

                    SetVertexColor(args);

                    HighlightTopologyVertex(args, dadVertices, mcdVertices);
                };

                // Грани форматируем стандартно с двумя весами каждой грани
                graphviz.FormatEdge += (sender, args) =>
                {
                    args.EdgeFormatter.Label.Value = args.Edge.ToString();      // Указываем метки граней

                    if (args.Edge.IsAcrossTheBorder())                          // Грани через участки окрашиваем в красный цвет
                        args.EdgeFormatter.StrokeColor = Color.Red;

                    if (args.Edge.IsAlongTheBorder())                           // Грани вдоль границ участков окрашиваем в оранжевый цвет
                        args.EdgeFormatter.StrokeColor = Color.Orange;

                    HighlightTopologyEdge(args, edges);
                };

                graphviz.Generate(new FileDotEngine(), filename);       // Генерируем файл с укзанным именем

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
        /// Задать цвет фона вершины.
        /// </summary>
        /// <param name="args">Аргументы форматирования вершины.</param>
        protected void SetVertexColor(FormatVertexEventArgs<TopologyVertex> args)
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
        protected void HighlightTopologyVertex(FormatVertexEventArgs<TopologyVertex> args, IEnumerable<TopologyVertex> dadVertices, IEnumerable<TopologyVertex> mcdVertices)
        {
            try
            {
                var dadFound = dadVertices.Contains(args.Vertex);
                var mcdFound = mcdVertices.Contains(args.Vertex);

                // Окрашиваем контуры вершинам в соответствии с устройствами на них
                if (dadFound)
                {
                    args.VertexFormatter.Shape = mcdFound ? GraphvizVertexShape.Hexagon: GraphvizVertexShape.Box;   // Если в вершине и счётчик, и УСПД, то форма особенная
                    args.VertexFormatter.StrokeColor = Color.Red;
                    args.VertexFormatter.FontColor = Color.Red;
                }

                if (mcdFound)
                {
                    args.VertexFormatter.Shape = GraphvizVertexShape.Box;
                    args.VertexFormatter.StrokeColor = Color.Blue;
                    args.VertexFormatter.FontColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HighlightTopologyVertex failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Выделить цветом на графе связи топологии между вершинами графа. 
        /// </summary>
        /// <param name="args">Аргументы события отрисовки грани.</param>
        /// <param name="edges">Перечисление всех граней топологии.</param>
        protected void HighlightTopologyEdge(FormatEdgeEventArgs<TopologyVertex, TopologyEdge> args, IEnumerable<TopologyEdge> edges)
        {
            try
            {
                if (edges.Contains(args.Edge))
                {
                    args.EdgeFormatter.FontColor = Color.Blue;
                    args.EdgeFormatter.StrokeColor = Color.Blue;
                    args.EdgeFormatter.Style = GraphvizEdgeStyle.Bold;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HighlightTopologyEdge failed! {0}", ex.Message);
            }
        }
    }
}
