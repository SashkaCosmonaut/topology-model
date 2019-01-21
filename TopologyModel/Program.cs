﻿using Newtonsoft.Json;
using System;
using System.IO;

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
