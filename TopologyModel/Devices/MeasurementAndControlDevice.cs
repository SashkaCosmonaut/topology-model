using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopologyModel.Enumerations;

namespace TopologyModel.Devices
{
	public class MeasurementAndControlDevice : Device
	{
		/// <summary>
		/// множество всех измерений потребления энергоресурсов, выдаваемых данным КУ
		/// </summary>
		public Measurement[] Measurements { get; set; } = new Measurement[] { };

		/// <summary>
		/// множество всех управляющих воздействий, позволяющих осуществлять данным КУ
		/// </summary>
		public Control[] Сontrols { get; set; } = new Control[] { };
	}
}
