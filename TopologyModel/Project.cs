using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopologyModel.Devices;
using TopologyModel.Enumerations;

namespace TopologyModel
{
	public class Project
	{
		/// <summary>
		/// бюджет всего проекта, за рамки которого затраты на реализацию проекта не должны выходить 
		/// </summary>
		public double Budget { get; set; }

		/// <summary>
		/// планируемый период эксплуатации системы для расчёта затрат на её использование во времени
		/// </summary>
		public uint UsageMonths { get; set; }

		/// <summary>
		/// требуется ли использовать локальный сервер, иначе будет использоваться удалённый сервер
		/// </summary>
		public bool UseLocalServer { get; set; }

		/// <summary>
		/// допустимо ли проведение монтажных работ его сотрудниками
		/// </summary>
		public bool UseLocalEmployee { get; set; }


		/// <summary>
		/// абонентская плата в месяц за использование и обслуживание локального сервера 
		/// </summary>
		public double LocalServerMonthlyPayment { get; set; }

		/// <summary>
		/// абонентская плата в месяц за использование и обслуживание предоставляемого удалённого сервера
		/// </summary>
		public double RemoteServerMonthlyPayment { get; set; }


		/// <summary>
		/// множество стоимостей в месяц абонентского обслуживания
		/// </summary>
		public Dictionary<InternetConnection, double> MobileInternetMonthlyPayment { get; set; } = new Dictionary<InternetConnection, double>();

		/// <summary>
		/// множество рассматриваемых участков всех объектов всего предприятия
		/// </summary>
		public Region StartRegion { get; set; }


		public Facility[] Facilities { get; set; } = new Facility[] { };


		public Tools AvailableTools { get; set; }


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

		protected void RecalculateResult(ref CalculationResult currentResult)
		{
			currentResult.InstallationFinancialCost = GetCurrentInstallationFinancialCost();
			currentResult.InstallationTimeCost = GetCurrentInstallationTimeCost();
			currentResult.PeriodFinancialCost = GetCurrentPeriodFinancialCost();
		}

		protected double GetCurrentInstallationFinancialCost()
		{
			return 0;
		}

		protected TimeSpan GetCurrentInstallationTimeCost()
		{
			return new TimeSpan(); ;
		}

		protected double GetCurrentPeriodFinancialCost()
		{
			return 0;
		}

		protected bool IsCalculationFinished(CalculationResult currentResult)
		{
			return 
				Facilities.All(f => f.AllRequirementsMet()) &&
				currentResult.InstallationFinancialCost < Budget;
		}
	}
}
