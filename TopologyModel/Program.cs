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
                            PurchasePrice = 1500,
                            Name = "Электроический счётчик полностью механический",
                            InstallationTime = new TimeSpan(1,0,0),
                            SendingProtocols = new Protocol[]
                            {
                                Protocol.Analog
                            },
                            Measurements = new Measurement[]
                            {
                                Measurement.ElectricityConsumption
                            }
                        },
                        new MeasurementAndControlDevice
                        {
                            InstallationPrice = 500,
                            IsPowerRequired = false,
                            PurchasePrice = 1500,
                            Name = "Электроический счётчик механический с импульсами",
                            InstallationTime = new TimeSpan(1,0,0),
                            SendingProtocols = new Protocol[]
                            {
                                Protocol.Analog,
                                Protocol.Impulse,
                            },
                            Measurements = new Measurement[]
                            {
                                Measurement.ElectricityConsumption
                            }
                        },
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
                        },
                        new MeasurementAndControlDevice
                        {
                            InstallationPrice = 500,
                            IsPowerRequired = false,
                            PurchasePrice = 5000,
                            Name = "Электроический счётчик с выходом в Интернет",
                            InstallationTime = new TimeSpan(1,0,0),
                            SendingProtocols = new Protocol[]
                            {
                                Protocol.ToDAD_1
                            },
                            Measurements = new Measurement[]
                            {
                                Measurement.ElectricityConsumption
                            }
                        },


                        new MeasurementAndControlDevice
                        {
                            InstallationPrice = 500,
                            IsPowerRequired = false,
                            PurchasePrice = 1500,
                            Name = "Счётчик холодной воды полностью механический",
                            InstallationTime = new TimeSpan(1,0,0),
                            SendingProtocols = new Protocol[]
                            {
                                Protocol.Analog
                            },
                            Measurements = new Measurement[]
                            {
                                Measurement.ColdWaterConsumption
                            }
                        },
                        new MeasurementAndControlDevice
                        {
                            InstallationPrice = 500,
                            IsPowerRequired = false,
                            PurchasePrice = 1500,
                            Name = "Счётчик холодной воды  механический с импульсами",
                            InstallationTime = new TimeSpan(1,0,0),
                            SendingProtocols = new Protocol[]
                            {
                                Protocol.Analog,
                                Protocol.Impulse,
                            },
                            Measurements = new Measurement[]
                            {
                                Measurement.ColdWaterConsumption
                            }
                        },
                        new MeasurementAndControlDevice
                        {
                            InstallationPrice = 500,
                            IsPowerRequired = true,
                            PurchasePrice = 3500,
                            Name = "Счётчик холодной воды с RS-485 и импульсом",
                            InstallationTime = new TimeSpan(1,0,0),
                            SendingProtocols = new Protocol[]
                            {
                                Protocol.Analog,
                                Protocol.Impulse,
                                Protocol.RS485
                            },
                            Measurements = new Measurement[]
                            {
                                Measurement.ColdWaterConsumption
                            }
                        },
                        new MeasurementAndControlDevice
                        {
                            InstallationPrice = 500,
                            IsPowerRequired = false,
                            PurchasePrice = 5000,
                            Name = "Счётчик холодной воды с выходом в Интернет",
                            InstallationTime = new TimeSpan(1,0,0),
                            SendingProtocols = new Protocol[]
                            {
                                Protocol.ToDAD_1
                            },
                            Measurements = new Measurement[]
                            {
                                Measurement.ColdWaterConsumption
                            }
                        },


                        new MeasurementAndControlDevice
                        {
                            InstallationPrice = 100,
                            IsPowerRequired = false,
                            PurchasePrice = 3000,
                            Name = "Проводной датчик температуры и влажности",
                            InstallationTime = new TimeSpan(1,0,0),
                            SendingProtocols = new Protocol[]
                            {
                                Protocol.RS485
                            },
                            Measurements = new Measurement[]
                            {
                                Measurement.Temperature,
                                Measurement.Humidity
                            }
                        },

                        new MeasurementAndControlDevice
                        {
                            InstallationPrice = 100,
                            IsPowerRequired = false,
                            PurchasePrice = 5000,
                            Name = "Беспроводной датчик температуры и влажности",
                            InstallationTime = new TimeSpan(1,0,0),
                            SendingProtocols = new Protocol[]
                            {
                                Protocol.GHz24
                            },
                            Measurements = new Measurement[]
                            {
                                Measurement.Temperature,
                                Measurement.Humidity
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
