using Newtonsoft.Json;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		public TopologyGraph Graph { get; set; } = new TopologyGraph();

		/// <summary>
		/// Создать и инициализировать класс проекта по умолчанию.
		/// </summary>
		public Project()
		{
			Budget = 100000;
			UsageMonths = 12;
			UseLocalServer = false;
			UseLocalEmployee = false;
			IsInternetAvailable = false;
			LocalServerMonthlyPayment = 999999;
			RemoteServerMonthlyPayment = 2000;
			MobileInternetMonthlyPayment.Add(InternetConnection._3G, 300);

			InitializeRegions();
			InitializeGraph();
			InitializeTools();
		}

		/// <summary>
		/// Инициализировать участки всего предприятия.
		/// </summary>
		protected void InitializeRegions()
		{
			try
			{
				using (var sr = File.OpenText("./Config/Regions.json"))
				{
					Regions = JsonConvert.DeserializeObject<Region[]>(sr.ReadToEnd());
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Regions = new Region[] { };
			}
		}

		/// <summary>
		/// Инициализировать граф всего предприятия.
		/// </summary>
		protected void InitializeGraph()
		{

		}

		/// <summary>
		/// Инициализировать базу данных инструментария.
		/// </summary>
		protected void InitializeTools()
		{

		}
	}
}