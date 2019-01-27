using TopologyModel.Enumerations;

namespace TopologyModel.Tools
{
	/// <summary>
	/// Класс канала передачи данных.
	/// </summary>
	public class DataChannel : AbstractTool
	{
		/// <summary>
		/// является ли канал передачи данных беспроводным 
		/// </summary>
		public bool IsWireless { get; set; }

		/// <summary>
		/// стандарт передачи данных, используемый в данном КПД
		/// </summary>
		public Protocol Protocol { get; set; }

		/// <summary>
		/// топология, допустимая для использования в данном КПД
		/// </summary>
		public Topology Topology { get; set; }

		/// <summary>
		/// максимальная дальность передачи данных по данному КПД
		/// </summary>
		public double MaxRange { get; set; }

		/// <summary>
		/// максимально допустимое количество устройств, которые могут передавать данные через данный КПД
		/// </summary>
		public uint MaxDevicesConnected { get; set; }
	}
}
