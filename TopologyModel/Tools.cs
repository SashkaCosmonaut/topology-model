using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopologyModel.Devices;

namespace TopologyModel
{
	public class Tools
	{
		public MeasurementAndControlDevice[] MCDs { get; set; } = new MeasurementAndControlDevice[] { };

		public DataChannel[] DCs { get; set; } = new DataChannel[] { };

		public TransceiverDevice[] TDs { get; set; } = new TransceiverDevice[] { };

		public DataAcquisitionDevice[] DADs { get; set; } = new DataAcquisitionDevice[] { };
	}
}
