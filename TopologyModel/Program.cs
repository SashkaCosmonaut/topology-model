using Newtonsoft.Json;
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
			var project = ReadProject();

			if (project == null) return;
		}

		/// <summary>
		/// Считать параметры всего проекта из JSON-файла.
		/// </summary>
		/// <returns>Считанный новый объект проекта.</returns>
		public static Project ReadProject()
		{
			try
			{
				using (var sr = File.OpenText("./Configs/Config.json"))
				{
					return JsonConvert.DeserializeObject<Project>(sr.ReadToEnd());
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}
	}
}
