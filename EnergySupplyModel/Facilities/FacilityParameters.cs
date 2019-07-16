using EnergySupplyModel.Enumerations;
using System;
using System.Collections.Generic;

namespace EnergySupplyModel.Facilities
{
    /// <summary>
    /// Параметры объекта предприятия, описывающие потребляемые им энергоресурсы, производимые изделия и т.п.
    /// </summary>
    public class FacilityParameters
    {
        /// <summary>
        /// Постоянное потребление данного объекте в виде словаря, где ключ - тип энергоресурса, а значение - объем потребления.
        /// </summary>
        public Dictionary<EnergyResourceType, double> ConstantConsumption { get; set; }

        /// <summary>
        /// Производительность объекта, которая задаётся как словарь, в котором ключ - это тип продукции, а значение - 
        /// это словарь, в котором ключ - это тип энергоресурса, а значение - объем его потребления для изготовления единицы продукции.
        /// </summary>
        public Dictionary<ProductType, Dictionary<EnergyResourceType, double>> Productivity { get; set; }

        /// <summary>
        /// Функция задания производственного плана для текущего объекта предприятия, которая возвращает количество
        /// произведённых единиц продукции различных типов в определённый момент времени. Для составного объекта может не задаваться.
        /// Может браться из файла или БД, аналогично тому, как берутся данные потребления со средств измерения.
        /// </summary>
        public Func<DateTime, Dictionary<ProductType, int>> ProductionPlan { get; set; }
    }
}
