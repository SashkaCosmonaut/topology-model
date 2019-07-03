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
        /// Имя файла для чтения измеренных данных.
        /// </summary>
        private static string _measuredDataFilename { get; set; } = Path.Combine("Data", "ElectricityMeasured.csv");

        /// <summary>
        /// Имя файла для чтения ожидаемых данных.
        /// </summary>
        private static string _expectedDataFilename { get; set; } = Path.Combine("Data", "ElectricityExpected.csv");

        /// <summary>
        /// Имя файла для чтения потенциальных данных.
        /// </summary>
        private static string _potentialDataFilename { get; set; } = Path.Combine("Data", "ElectricityPotential.csv");

        /// <summary>
        /// Закэшированыне измеренные данные для всех объектов.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<DateTime, double>> _measuredData 
            = new Dictionary<string, Dictionary<DateTime, double>>();

        /// <summary>
        /// Закэшированыне ожидаемые данные для всех объектов.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<DateTime, double>> _expectedData 
            = new Dictionary<string, Dictionary<DateTime, double>>();

        /// <summary>
        /// Закэшированыне потенциальные данные для всех объектов.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<DateTime, double>> _potentialData 
            = new Dictionary<string, Dictionary<DateTime, double>>();

        /// <summary>
        /// Получить измеренные данные о потреблении энергоресурсов объектом.
        /// </summary>
        /// <param name="facilityName">Наименование объекта.</param>
        /// <param name="start">Начало периода измерения.</param>
        /// <param name="end">Конец периода измерения.</param>
        /// <returns>Словарь данных потребления.</returns>
        public static Dictionary<DateTime, double> GetMeasuredData(string facilityName, DateTime start, DateTime end)
        {
            return GetData(facilityName, start, end, _measuredData, _measuredDataFilename);
        }

        /// <summary>
        /// Получить ожидаемые данные о потреблении энергоресурсов объектом.
        /// </summary>
        /// <param name="facilityName">Наименование объекта.</param>
        /// <param name="start">Начало периода измерения.</param>
        /// <param name="end">Конец периода измерения.</param>
        /// <returns>Словарь данных потребления.</returns>
        public static Dictionary<DateTime, double> GetExpectedData(string facilityName, DateTime start, DateTime end)
        {
            return GetData(facilityName, start, end, _expectedData, _expectedDataFilename);
        }

        /// <summary>
        /// Получить потенциальные данные о потреблении энергоресурсов объектом.
        /// </summary>
        /// <param name="facilityName">Наименование объекта.</param>
        /// <param name="start">Начало периода измерения.</param>
        /// <param name="end">Конец периода измерения.</param>
        /// <returns>Словарь данных потребления.</returns>
        public static Dictionary<DateTime, double> GetPotentialData(string facilityName, DateTime start, DateTime end)
        {
            return GetData(facilityName, start, end, _potentialData, _potentialDataFilename);
        }

        /// <summary>
        /// Считать данные из файла или из кэша о потреблении энергоресурсов объектом.
        /// </summary>
        /// <param name="facilityName">Наименование объекта.</param>
        /// <param name="start">Начало периода измерения.</param>
        /// <param name="end">Конец периода измерения.</param>
        /// <param name="dataCache">Контейнер для кэша.</param>
        /// <param name="filename">Имя файла с данными.</param>
        /// <returns>Словарь данных потребления.</returns>
        private static Dictionary<DateTime, double> GetData(string facilityName, DateTime start, DateTime end,
            Dictionary<string, Dictionary<DateTime, double>> dataCache, string filename)
        {
            if (dataCache.Any())
                return SelectData(dataCache, facilityName, start, end);

            using (var parser = new TextFieldParser(filename))
            {
                parser.SetDelimiters(";");

                parser.TextFieldType = FieldType.Delimited;

                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();

                    if (parser.LineNumber == 2)     // Вначале считываем наименования объектов и создаём кэш для данных
                    {
                        foreach (var field in fields.Skip(1))   // Пропускаем заголовок для дат
                            dataCache.Add(field, new Dictionary<DateTime, double>());   // Наполняем словарь кэша

                        continue;
                    }

                    if (!dataCache.Any())   // Если наименования не считали и кэш не создали, что-то пошло не так
                        return new Dictionary<DateTime, double>();

                    // Первый столбец - дата
                    var date = DateTime.ParseExact(fields[0], "dd.MM.yy H:mm:ss", CultureInfo.InvariantCulture);

                    for (var i = 1; i < fields.Length; i++)     // Остальные столбцы - данные
                        dataCache.ElementAt(i - 1).Value.Add(date, double.Parse(fields[i]));
                }
            }

            return SelectData(dataCache, facilityName, start, end);
        }

        /// <summary>
        /// Выбрать данные из контейнера кэша для указанного объекта и периода времени.
        /// </summary>
        /// <param name="dataCache">Контейнер кэша.</param>
        /// <param name="facilityName">Наименование объекта.</param>
        /// <param name="start">Начало периода времени.</param>
        /// <param name="end">Конец периода времени.</param>
        /// <returns>Массив данных потребления.</returns>
        private static Dictionary<DateTime, double> SelectData(Dictionary<string, Dictionary<DateTime, double>> dataCache, 
            string facilityName, DateTime start, DateTime end)
        {
            if (dataCache.ContainsKey(facilityName)) // Если данные уже считывали, просто возвращаем
                return dataCache[facilityName].Where(q => q.Key >= start && q.Key < end).ToDictionary(q => q.Key, q => q.Value);

            return new Dictionary<DateTime, double>();
        }
    }
}
