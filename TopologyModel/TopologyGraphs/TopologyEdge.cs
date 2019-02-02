using System;
using System.Collections.Generic;
using QuickGraph;
using TopologyModel.Enumerations;

namespace TopologyModel.TopologyGraphs
{
    /// <summary>
    /// Класс грани графа топологии.
    /// </summary>
    public class TopologyEdge : UndirectedEdge<TopologyVertex>
    {
        /// <summary>
        /// Вес грани для проводной связи, строящийся на основе пераметров смежных участков.
        /// </summary>
        public float WiredWeight { get; }

        /// <summary>
        /// Вес грани для беспроводной связи, строящийся на основе пераметров смежных участков.
        /// </summary>
        public float WirelessWeight { get; }

        /// <summary>
        /// Создать новую грань графа.
        /// </summary>
        /// <param name="source">Вершина графа - источник грани.</param>
        /// <param name="target">Вершина графа - приемник грани.</param>
        /// <param name="weightCoefficients">Весовые коэффициенты для рассчётов весов граней.</param>
        public TopologyEdge(TopologyVertex source, TopologyVertex target, Dictionary<string, float> weightCoefficients = null) : base(source, target)
        {
            try
            {
                // Если связь между участками, берётся среднее от значений границ смежных участков
                WirelessWeight = IsAcrossTheBorder()    
                    ? (Source.Region.GetBadRadioTransmittanceEstimate(GetLocation(Source, Target)) +
                       Target.Region.GetBadRadioTransmittanceEstimate(GetLocation(Target, Source))) / 2
                    : Source.Region.GetBadRadioTransmittanceEstimate(LocationInRegion.Inside);          // Иначе берётся оценка внутри участка

                WiredWeight = IsAcrossTheBorder()
                    ? (GetWiredWeightAcrossTheBorder(Source, Target) + GetWiredWeightAcrossTheBorder(Target, Source)) / 2
                    : GetWiredWeight(Source, Target);
            }
            catch (Exception ex)
            {
                Console.WriteLine("TopologyEdge failed! {0}", ex.Message);
            }
        }


        /// <summary>
        /// Рассчитать проводной вес грани через границу исходного участка в целевой путём сложения трудоемкости проведения кабелей,
        /// оценок недоступности, трудоемкости стены и агрессивности среды на участке.
        /// </summary>
        /// <param name="source">Исходный участок.</param>
        /// <param name="target">Целевой участок.</param>
        /// <returns>Дробное значение веса грани.</returns>
        protected static float GetWiredWeightAcrossTheBorder(TopologyVertex source, TopologyVertex target)
        {
            return source.Region.GetBadWiredTransmittanceEstimate(GetLocation(source, target)) + GetWiredWeight(source, target);
        }

        /// <summary>
        /// Получить проводной вес грани вдоль границы участка путём сложения экспертных оценок.
        /// </summary>
        /// <param name="source">Исходный узел грани.</param>
        /// <param name="target">Целевой узел грани.</param>
        /// <returns>Значение проводного веса.</returns>
        protected static float GetWiredWeight(TopologyVertex source, TopologyVertex target)
        {
            var location = GetLocation(source, target);

            return 0.4f * source.Region.GetUnavailabilityEstimate(location) +
                   0.3f * source.Region.GetLaboriousnessEstimate(location) +
                   0.3f * source.Region.GetAggressivenessEstimate(location);
        }

        /// <summary>
        /// Сравнить две грани. Используется базовый метод.
        /// </summary>
        /// <param name="obj">Объект другой грани.</param>
        /// <returns>Результат сравнения граней.</returns>
        public override bool Equals(object obj) => base.Equals(obj);

        /// <summary>
        /// Получить хеш-код объекта грани. Используется базовый метод.
        /// </summary>
        /// <returns>Хэш-код объекта грани.</returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Получить строковое представление грани.
        /// </summary>
        /// <returns>Строка с весами грани через запятую, округлёнными до одного знака после запятой.</returns>
        public override string ToString()
        {
            return $"{WiredWeight:0.0}; {WirelessWeight:0.0}";
        }

        /// <summary>
        /// Проверить, проведена ли данная грань через границу между участками.
        /// </summary>
        /// <returns>True, если она через границу между участками.</returns>
        public bool IsAcrossTheBorder()
        {
            return IsAcrossTheBorder(Source, Target);
        }

        /// <summary>
        /// Проверить, проведена ли данная грань вдоль границы участка.
        /// </summary>
        /// <returns>True, если она вдоль границы участка.</returns>
        public bool IsAlongTheBorder()
        {
            return IsAlongTheBorder(Source, Target);
        }

        /// <summary>
        /// Проверить, проведена ли данная грань внутри участка.
        /// </summary>
        /// <returns>True, если она внутри участка.</returns>
        public bool IsInside()
        {
            return GetInRegionLocation(Source, Target) == LocationInRegion.Inside; // Грань находится внутри участка, если она не вдоль границы и не через границу
        }

        /// <summary>
        /// Проверить, проведена ли грань через границу между участками.
        /// </summary>
        /// <param name="source">Исходный узел грани.</param>
        /// <param name="target">Целевой узел грани.</param>
        /// <returns>True, если она через границу между участками.</returns>
        public static bool IsAcrossTheBorder(TopologyVertex source, TopologyVertex target)
        {
            return source.Region != target.Region;  // Грань проведена через границу, когда она ведёт на другой участок
        }

        /// <summary>
        /// Проверить, проведена ли грань вдоль границы участка.
        /// </summary>
        /// <param name="source">Исходный узел грани.</param>
        /// <param name="target">Целевой узел грани.</param>
        /// <returns>True, если она вдоль границы участка.</returns>
        public static bool IsAlongTheBorder(TopologyVertex source, TopologyVertex target)
        {
            return !IsAcrossTheBorder(source, target) && GetLocation(source, target) != LocationInRegion.Inside;    // Грань располагается вдоль границы, когда она не через границу и не внутри
        }

        /// <summary>
        /// Получить месторасположение грани, вдоль или через какую границу она проходит или внутри.
        /// </summary>
        /// <param name="source">Исходный узел грани.</param>
        /// <param name="target">Целевой узел грани.</param>
        /// <returns>Значение из перечисления месторасположений.</returns>
        public static LocationInRegion GetLocation(TopologyVertex source, TopologyVertex target)
        {
            if (IsAcrossTheBorder(source, target))
                return GetCrossBorderLocation(source, target);

            return GetInRegionLocation(source, target);
        }

        /// <summary>
        /// Получить, через какую границу участка пересечена грань.
        /// </summary>
        /// <param name="source">Исходный узел грани.</param>
        /// <param name="target">Целевой узел грани.</param>
        /// <returns>Значение из перечисления месторасположений.</returns>
        protected static LocationInRegion GetCrossBorderLocation(TopologyVertex source, TopologyVertex target)
        {
            if (source.Region.Y > target.Region.Y)
                return LocationInRegion.TopBorder;

            if (source.Region.Y < target.Region.Y)
                return LocationInRegion.BottomBorder;

            if (source.Region.X > target.Region.X)
                return LocationInRegion.LeftBorder;

            if (source.Region.X < target.Region.X)
                return LocationInRegion.RightBorder;

            return LocationInRegion.Inside;
        }

        /// <summary>
        /// Получить месторасположение связи, вдоль какой границы она проведена.
        /// </summary>
        /// <param name="source">Исходный узел грани.</param>
        /// <param name="target">Целевой узел грани.</param>
        /// <returns>Значение из перечисления месторасположений.</returns>
        protected static LocationInRegion GetInRegionLocation(TopologyVertex source, TopologyVertex target)
        {
            if (source.RegionX == target.RegionX)
            {
                if (source.RegionX == 0)
                    return LocationInRegion.LeftBorder;

                if (source.RegionX == source.Region.Width - 1)
                    return LocationInRegion.RightBorder;
            }

            if (source.RegionY == target.RegionY)
            {
                if (source.RegionY == 0)
                    return LocationInRegion.TopBorder;

                if (source.RegionY == source.Region.Height - 1)
                    return LocationInRegion.BottomBorder;
            }

            return LocationInRegion.Inside;
        }
    }
}
