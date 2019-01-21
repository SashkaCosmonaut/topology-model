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
		public Dictionary<uint, Region> Regions { get; set; }

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
				var regionsMatrix = CreateRegionsMatrix();

				if (regionsMatrix.Length == 0) return false;

				Graph = CreateRegionsGraph(regionsMatrix);

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
		protected uint[,] CreateRegionsMatrix()
		{
			Console.Write("Create regions matrix... ");

			try
			{
				var resultMatrix = new uint[Height, Width];

				foreach (var region in Regions)		// Перебираем все имеющиеся регионы
				{
					// Определяем начальные и конечные координаты участков
					var startX = region.Value.X - 1;
					var startY = region.Value.Y - 1;
					var endX = startX + region.Value.Width;
					var endY = startY + region.Value.Height;

					for (var i = startY; i < endY; i++)			// Наполняем матрицу идентификаторами участков по координатам
						for (var j = startX; j < endX; j++)
							resultMatrix[i, j] = region.Key;
				}

				Console.WriteLine("Done! Result matix: ");

				for (var i = 0; i < Height; i++)				// Выводим матрицу для наглядности
				{
					for (var j = 0; j < Width; j++)
						Console.Write("{0,3}", resultMatrix[i, j]);

					Console.WriteLine();
				}

				return resultMatrix;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed! {0}", ex.Message);
				return new uint[,] { };
			}
		}

		/// <summary>
		/// Создать граф всего предприятия на основе матрицы участков предприятия, перебирая каждый её элемент, 
		/// создавая на его основе вершину и генерируя связи в соответствии с имеющимися вершинами.
		/// </summary>
		/// <param name="regionsMatrix">Матрица с индификаторами участков предприятия.</param>
		/// <returns>Граф с вершинами, ребрами и их весами.</returns>
		protected TopologyGraph CreateRegionsGraph(uint [,] regionsMatrix)
		{
			Console.Write("Create regions graph... ");

			try
			{
				return null;
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