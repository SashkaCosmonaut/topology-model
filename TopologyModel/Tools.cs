using TopologyModel.Devices;

namespace TopologyModel
{
	/// <summary>
	/// БД инструментов для использования в проекте.
	/// </summary>
	public class Tools
	{
		public MeasurementAndControlDevice[] MCDs { get; set; } = new MeasurementAndControlDevice[] { };

		public DataChannel[] DCs { get; set; } = new DataChannel[] { };

		public TransceiverDevice[] TDs { get; set; } = new TransceiverDevice[] { };

		public DataAcquisitionDevice[] DADs { get; set; } = new DataAcquisitionDevice[] { };
	}
}
