using Newtonsoft.Json;
using System;
using System.IO;
using TopologyModel.Enumerations;
using TopologyModel.Tools;

namespace TopologyModel
{
	/// <summary>
	/// Главный класс всей программы.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Главная функция программы.
		/// </summary>
		/// <param name="args">Параметры командной строки.</param>
		public static void Main(string[] args)
		{
			Console.WriteLine("Main starting...");

			try
			{
				var project = ReadProject();

				if (project == null) return;

				if (!project.InitializeGraph()) return;

                var tools = new ToolSet
                {
                    MCDs = new MeasurementAndControlDevice[]
                    {
                        new MeasurementAndControlDevice
                        {
                            InstallationPrice = 500,
                            IsPowerRequired = false,
                            PurchasePrice = 3500,
                            Name = "Электроический счётчик с RS-485 и импульсом",
                            InstallationTime = new TimeSpan(1,0,0),
                            SendingProtocols = new Protocol[]
                            {
                                Protocol.Analog,
                                Protocol.Impulse,
                                Protocol.RS485
                            },
                            Measurements = new Measurement[]
                            {
                                Measurement.ElectricityConsumption
                            }
                        }
                    },
                    DADs = new DataAcquisitionDevice[]
                    {

                    },
                    DCs = new DataChannel[]
                    {

                    },
                    TDs = new TransceiverDevice[]
                    {

                    }
                };

                var toolsJSON = JsonConvert.SerializeObject(tools);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Main failed! {0}", ex.Message);
			}
		}

		/// <summary>
		/// Считать параметры всего проекта из JSON-файла.
		/// </summary>
		/// <returns>Считанный новый объект проекта.</returns>
		public static Project ReadProject()
		{
			Console.Write("Reading the project config... ");

			try
			{
				using (var sr = File.OpenText("./Configs/Config.json"))
				{
					var result = JsonConvert.DeserializeObject<Project>(sr.ReadToEnd());

					Console.WriteLine(result == null ? "Failed!" : "Done!");

					return result;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed! {0}", ex.Message);
				return null;
			}
		}
	}
}
