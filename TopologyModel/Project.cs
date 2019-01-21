using System;
using System.Collections.Generic;
using TopologyModel.Enumerations;
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
		/// Множество стоимостей в месяц абонентского обслуживания технологий мобильной передачи данных на сервер.
		/// </summary>
		public Dictionary<InternetConnection, double> MobileInternetMonthlyPayment { get; set; } = new Dictionary<InternetConnection, double>();

		
		/// <summary>
		/// Массив всех участков предприятия.
		/// </summary>
		public Region[] Regions { get; set; }

		/// <summary>
		/// База данных доступного инструментария.
		/// </summary>
		public Tools AvailableTools { get; set; } = new Tools();

		/// <summary>
		/// Граф всего предприятия.
		/// </summary>
		public TopologyGraph Graph { get; set; }

		/// <summary>
		/// Имя dot-файла графа участков предприятия.
		/// </summary>
		public string GraphDotFilename { get; set; } = "graph.dot";


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

				Graph = CreateRegionsGraph(verticesMatrix);

				if (Graph == null) return false;

				if (CreateGraphDotFile())
					Console.WriteLine("Check the graph dot-file in {0}", GraphDotFilename);

				Console.WriteLine("Done!");

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
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

				foreach (var region in Regions)		// Перебираем все имеющиеся регионы
				{
					// Определяем начальные и конечные координаты участков в матрице
					var startX = region.X - 1;			// В конфигурационном файле координаты начинаются с 1
					var startY = region.Y - 1;
					var endX = startX + region.Width;
					var endY = startY + region.Height;

					for (var i = startY; i < endY; i++)			// Наполняем матрицу идентификаторами участков по координатам
						for (var j = startX; j < endX; j++)     // Создаём вершину, привязанную к учаску и задаём её координаты внутри самого участка
																// +1 из-за того, что в конфигурационном файле координаты начинаются с 1
							verticesMatrix[i, j] = new TopologyVertex(region, j - region.X + 1, i - region.Y + 1);
				}

				Console.WriteLine("Done! Result matix: ");

				for (var i = 0; i < Height; i++)				// Выводим матрицу идентификаторов регионов в вершинах для наглядности
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
		/// Создать граф всего предприятия на основе матрицы участков предприятия, перебирая каждый её элемент, 
		/// создавая на его основе вершину и генерируя связи в соответствии с имеющимися вершинами.
		/// </summary>
		/// <param name="verticesMatrix">Матрица с вершинами участков предприятия.</param>
		/// <returns>Граф с вершинами, ребрами и их весами.</returns>
		protected TopologyGraph CreateRegionsGraph(TopologyVertex[,] verticesMatrix)
		{
			Console.Write("Create regions graph... ");

			try
			{
				var resultGraph = new TopologyGraph();

				for (var i = 0; i < Height; i++)                            // Проходимся по матрице
					for (var j = 0; j < Width; j++)
					{
						var vertex = verticesMatrix[i, j];                  // Берём текущую вершину в матрице

						if (!resultGraph.ContainsVertex(vertex))            // Добавляем её в граф, если ещё ранее этого не делали
							resultGraph.AddVertex(vertex);

                        for (var yShift = -1; yShift < 2; yShift++)         // Обходим все соседние вершины для текущей вершины матрицы, какие есть
							for (var xShift = -1; xShift < 2; xShift++)
							{
								if (xShift == 0 && yShift == 0) continue;	// Пропускаем текущую ячейку

								var neighborX = j + xShift;                 // Вычисляем позицию соседней вершины
								var neighborY = i + yShift;

								// Если нет соседней вершины с такими координатами соседней позиции, пропускаем её
								if (neighborX < 0 || neighborY < 0 || neighborX >= Width || neighborY >= Height) continue;

								var neighborVertex = verticesMatrix[neighborY, neighborX];

								if (!resultGraph.ContainsVertex(neighborVertex))	// Добавляем соседнюю вершину, если её ещё нет в графе
									resultGraph.AddVertex(neighborVertex);

								// Если соседняя вершина не диагональная, то гарантировано добавляем к ней грань
								if (neighborX == 0 || neighborY == 0)
									resultGraph.AddEdge(new QuickGraph.SUndirectedTaggedEdge<TopologyVertex, float>(vertex, neighborVertex, 0));

								// Добавляем диагональную грань между вершинами, только если вершины находятся на одном участке
								else if (neighborVertex.Region == vertex.Region)
									resultGraph.AddEdge(new QuickGraph.SUndirectedTaggedEdge<TopologyVertex, float>(vertex, neighborVertex, 0));
							}
					}

				Console.WriteLine("Done!");

				return resultGraph;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed! {0}", ex.Message);
				return null;
			}
		}

		/// <summary>
		/// Создать dot-файл на основе 
		/// </summary>
		/// <returns>True, если операция выполнена успешно.</returns>
		protected bool CreateGraphDotFile()
		{
			Console.Write("Create the graph dot-file... ");

			try
			{

				Console.WriteLine("Done!");

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed! {0}", ex.Message);
				return false;
			}
		}
	}
}