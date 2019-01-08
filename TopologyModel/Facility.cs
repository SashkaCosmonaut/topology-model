using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopologyModel.Enumerations;

namespace TopologyModel
{
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


		public bool AllRequirementsMet()
		{
			return false;
		}
	}
}
