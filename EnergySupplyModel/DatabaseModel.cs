using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EnergySupplyModel
{
    /// <summary>
    /// Класс, представляющий собой источник данных, предоставляющий данные из БД.
    /// </summary>
    public static class DatabaseModel
    {
        /// <summary>
        /// Имя файла для чтения данных.
        /// </summary>
        public static string Filename { get; set; } = Path.Combine("Data", "electricity.csv");

        /// <summary>
        /// Закэшированыне данные для всех объектов.
        /// </summary>
        public static Dictionary<string, Dictionary<DateTime, double>> Data;

        /// <summary>
        /// Получить данные о потреблении энергоресурсов объектом.
        /// </summary>
        /// <param name="facilityName">Наименование объекта.</param>
        /// <param name="start">Начало периода времени.</param>
        /// <param name="end">Конец периода времени.</param>
        /// <returns>Массив данных потребления.</returns>
        public static Dictionary<DateTime, double> GetData(string facilityName, DateTime start, DateTime end)
        {
            if (Data != null)
                SelectData(facilityName, start, end);

            using (var parser = new TextFieldParser(Filename))
            {
                parser.SetDelimiters(";");

                parser.TextFieldType = FieldType.Delimited;

                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();

                    if (parser.LineNumber == 2)     // Вначале считываем наименования объектов и создаём кэш для данных
                    {
                        Data = fields.Skip(1).ToDictionary(q => q, q => new Dictionary<DateTime, double>());

                        continue;
                    }

                    if (Data == null)   // Если наименования не считали и кэш не создали, что-то пошло не так
                        return null;

                    // Первый столбец - дата
                    var date = DateTime.ParseExact(fields[0], "dd.MM.yy H:mm:ss", CultureInfo.InvariantCulture);

                    for (var i = 1; i < fields.Length; i++)     // Остальные столбцы - данные
                    {
                        Data.ElementAt(i - 1).Value.Add(date, double.Parse(fields[i]));
                    }
                }
            }

            return SelectData(facilityName, start, end);
        }

        /// <summary>
        /// Выбрать данные из кэша для указанного объекта и периода времени.
        /// </summary>
        /// <param name="facilityName">Наименование объекта.</param>
        /// <param name="start">Начало периода времени.</param>
        /// <param name="end">Конец периода времени.</param>
        /// <returns>Массив данных потребления.</returns>
        private static Dictionary<DateTime, double> SelectData(string facilityName, DateTime start, DateTime end)
        {
            if (Data != null && Data.ContainsKey(facilityName)) // Если данные уже считывали, просто возвращаем
                return Data[facilityName].Where(q => q.Key >= start && q.Key < end).ToDictionary(q => q.Key, q => q.Value);

            return null;
        }
    }
}
