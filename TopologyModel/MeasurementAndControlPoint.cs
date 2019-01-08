using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopologyModel.Enumerations;

namespace TopologyModel
{
	public class MeasurementAndControlPoint
	{
		/// <summary>
		/// множество всех измерений потребления энергоресурсов, доступных на данной ТУУ
		/// </summary>
		public Measurement[] AllMeasurements { get; set; } = new Measurement[] { };

		/// <summary>
		/// множество всех управляющих воздействий, доступных на данной ТУУ
		/// </summary>
		public Control[] AllСontrols { get; set; } = new Control[] { };

		/// <summary>
		/// допустима ли замена уже имеющихся КУ на ТУУ на более новые
		/// </summary>
		public bool IsDeviceReplacementAvailable { get; set; }
	}
}
