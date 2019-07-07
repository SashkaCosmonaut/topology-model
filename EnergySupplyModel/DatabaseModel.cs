using EnergySupplyModel.Enumerations;
using EnergySupplyModel.Input;
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
        private static readonly List<DataSet> DataCache = new List<DataSet>();

        /// <summary>
        /// Получить измеренные данные о потреблении энергоресурсов объектом.
        /// </summary>
        /// <param name="dataSource">Источник данных.</param>
        /// <param name="parameters">Параметры времени и даты для запроса данных.</param>
        /// <returns>Словарь данных потребления.</returns>
        public static DataSet GetMeasuredData(DataSource dataSource, InputDateTimeParameters parameters)
        {
            // Пытаемся найти данные указанного источника в кэше
            var resultData = DataCache.SingleOrDefault(q => q.DataSource.Equals(dataSource));

            if (resultData != null)
                return SelectData(resultData, parameters); ;

            var newData = new List<DataSet>();    // Новый блок кэша данных, который будет добавлен к общему кэшу

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
                            var data = new DataSet      // Создаем новые объекты данных для кэша
                            {
                                DataSource = new DataSource
                                {
                                    EnergyResourceType = dataSource.EnergyResourceType,
                                    FacilityName = field,
                                    TimeInterval = dataSource.TimeInterval
                                }
                            };

                            if (data.DataSource.Equals(dataSource))   // Запоминаем объект данных для нужного объекта, чтобы потом не искать
                                resultData = data;

                            newData.Add(data);     // Сохраняем новые данные в кэш
                        }

                        continue;
                    }

                    if (!newData.Any())   // Если наименования не считали и кэш не создали, что-то пошло не так
                        return null;

                    // Первый столбец - дата
                    var dateTime = DateTime.ParseExact(fields[0], "dd.MM.yy H:mm:ss", CultureInfo.InvariantCulture);

                    for (var i = 1; i < fields.Length; i++)     // Остальные столбцы - данные
                        newData.ElementAt(i - 1).Add(dateTime, 
                            new DataItem
                            {
                                ItemValue = double.Parse(fields[i]),
                                TimeStamp = dateTime                    // Тут должны быть добавлены метаданные
                            });
                }
            }

            DataCache.AddRange(newData);   // Сохраняем новые данные в общем кэше

            return SelectData(resultData, parameters);
        }

        /// <summary>
        /// Выбрать данные для заданного периода времени.
        /// </summary>
        /// <param name="data">Данные для выборки.</param>
        /// <param name="parameters">Параметры выборки данных.</param>
        /// <returns>Массив данных потребления за указанный период времени.</returns>
        private static DataSet SelectData(DataSet data, InputDateTimeParameters parameters)
        {
            var result = new DataSet { DataSource = data.DataSource };

            foreach (var item in data.Where(q => q.Key >= parameters.Start && q.Key < parameters.End))
                result.Add(item.Key, item.Value);

            return result;
        }
    }
}
