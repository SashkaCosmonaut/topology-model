using System;

namespace TopologyModel
{
	/// <summary>
	/// Результаты вычислений по оценке проекта.
	/// </summary>
	public class CalculationResult
	{
		/// <summary>
		/// Финансовые затраты на внедрение проекта.
		/// </summary>
		public double InstallationFinancialCost { get; set; }

		/// <summary>
		/// Временные затраты на внедрение проекта.
		/// </summary>
		public TimeSpan InstallationTimeCost { get; set; }

		/// <summary>
		/// Финансовые затраты на проект во времени.
		/// </summary>
		public double PeriodFinancialCost { get; set; }
	}
}
