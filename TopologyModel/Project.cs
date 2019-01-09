using System;
using System.Collections.Generic;
using System.Linq;
using TopologyModel.Enumerations;

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
		/// Множество рассматриваемых участков всех объектов всего предприятия.
		/// </summary>
		public Region StartRegion { get; set; }

		/// <summary>
		/// Множество объектов предприятия.
		/// </summary>
		public Facility[] Facilities { get; set; } = new Facility[] { };

		/// <summary>
		/// База данных доступного инструментария.
		/// </summary>
		public Tools AvailableTools { get; set; }

		/// <summary>
		/// Рассчитать затраты на проект.
		/// </summary>
		/// <returns>Полученные результаты расчета проекта.</returns>
		public CalculationResult Calculate()
		{
			var result = new CalculationResult();

			do
			{
				RecalculateResult(ref result);
			}
			while (!IsCalculationFinished(result));

			return result;
		}

		/// <summary>
		/// Заново рассчитать затраты на проект.
		/// </summary>
		/// <param name="currentResult">Обновляемый объект результатов.</param>
		protected void RecalculateResult(ref CalculationResult currentResult)
		{
			currentResult.InstallationFinancialCost = GetCurrentInstallationFinancialCost();
			currentResult.InstallationTimeCost = GetCurrentInstallationTimeCost();
			currentResult.PeriodFinancialCost = GetCurrentPeriodFinancialCost();
		}

		/// <summary>
		/// Рассчитать начальную стоимость проекта.
		/// </summary>
		protected double GetCurrentInstallationFinancialCost()
		{
			return 0;
		}

		/// <summary>
		/// Рассчитать затраты во времени на проект.
		/// </summary>
		protected TimeSpan GetCurrentInstallationTimeCost()
		{
			return new TimeSpan(); ;
		}

		/// <summary>
		/// Рассчитать затраченное время на реализацию проекта.
		/// </summary>
		protected double GetCurrentPeriodFinancialCost()
		{
			return 0;
		}

		/// <summary>
		/// Проверить, завершён ли расчет.
		/// </summary>
		/// <param name="currentResult">Текущий полученный результат.</param>
		/// <returns>True, если расчет завершён.</returns>
		protected bool IsCalculationFinished(CalculationResult currentResult)
		{
			return 
				Facilities.All(f => f.AllRequirementsMet()) &&
				currentResult.InstallationFinancialCost < Budget;
		}
	}
}
