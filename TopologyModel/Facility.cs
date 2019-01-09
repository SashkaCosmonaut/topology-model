using TopologyModel.Enumerations;

namespace TopologyModel
{
	/// <summary>
	/// Класс объекта предприятия.
	/// </summary>
	public class Facility
	{
		/// <summary>
		/// будет ли доступен выход в сеть Интернет из локальной сети предприятия
		/// </summary>
		public bool IsInternetAvailable { get; set; }

		/// <summary>
		/// множество измерений, которые необходимо производить на объекте
		/// </summary>
		public Measurement[] RequiredMeasurements { get; set; } = new Measurement[] { };

		/// <summary>
		/// множество управляющих воздействий, которые необходимо передавать на оборудование объекта
		/// </summary>
		public Control[] RequiredСontrols { get; set; } = new Control[] { };


		/// <summary>
		/// Все ли требования по покрытию измерений и управляющих воздействий покрыты.
		/// </summary>
		/// <returns>True, если да.</returns>
		public bool AllRequirementsMet()
		{
			return false;
		}
	}
}
