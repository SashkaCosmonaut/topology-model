using EnergySupplyModel.Enumerations;
using EnergySupplyModel.Materials;
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
        /// Словарь имен файлов чтения данных для каждого типа данных.
        /// </summary>
        private static readonly Dictionary<EnergyResourceType, string> Filenames = new Dictionary<EnergyResourceType, string>
        {
            { EnergyResourceType.Electricity, Path.Combine("Data", "Electricity.csv") },
            { EnergyResourceType.ColdWater, Path.Combine("Data", "Water.csv") }
        };

        /// <summary>
        /// Кэш считанных данных из файлов.
        /// </summary>
        private static readonly List<Data> DataCache = new List<Data>();

        /// <summary>
        /// Получить измеренные данные о потреблении энергоресурсов объектом.
        /// </summary>
        /// <param name="dataSource">Источник данных.</param>
        /// <param name="start">Начало периода измерения.</param>
        /// <param name="end">Конец периода измерения.</param>
        /// <returns>Словарь данных потребления.</returns>
        public static Data GetMeasuredData(DataSource dataSource, DateTime start, DateTime end)
        {
            // Пытаемся найти данные указанного источника в кэше
            var resultData = DataCache.SingleOrDefault(q => q.DataSource.Equals(dataSource));

            if (resultData != null)
                return resultData;

            var newData = new List<Data>();    // Новый блок кэша данных, который будет добавлен к общему кэшу

            using (var parser = new TextFieldParser(Filenames[dataSource.EnergyResourceType]))
            {
                parser.SetDelimiters(";");
                parser.TextFieldType = FieldType.Delimited;

                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();

                    if (parser.LineNumber == 2)     // Вначале считываем наименования объектов и создаём кэш для каждого источника
                    {
                        foreach (var field in fields.Skip(1))   // Пропускаем заголовок для дат
                        {
                            var data = new Data      // Создаем новые объекты данных для кэша
                            {
                                DataSource = new DataSource
                                {
                                    EnergyResourceType = dataSource.EnergyResourceType,
                                    FacilityName = field,
                                    TimeInterval = dataSource.TimeInterval
                                }
                            };

                            if (field == dataSource.FacilityName)   // Запоминаем объект данных для нужного объекта, чтобы потом не искать
                                resultData = data;

                            newData.Add(data);     // Сохраняем новые данные в кэш
                        }

                        continue;
                    }

                    if (!newData.Any())   // Если наименования не считали и кэш не создали, что-то пошло не так
                        return null;

                    // Первый столбец - дата
                    var datetime = DateTime.ParseExact(fields[0], "dd.MM.yy H:mm:ss", CultureInfo.InvariantCulture);

                    for (var i = 1; i < fields.Length; i++)     // Остальные столбцы - данные
                        newData.ElementAt(i - 1).Add(datetime, 
                            new DataItem
                            {
                                ItemValue = double.Parse(fields[i]),
                                TimeStamp = datetime                    // Тут должны быть добавлены метаданные
                            });
                }
            }

            DataCache.AddRange(newData);   // Сохраняем новые данные в общем кэше

            return resultData;
        }
    }
}
