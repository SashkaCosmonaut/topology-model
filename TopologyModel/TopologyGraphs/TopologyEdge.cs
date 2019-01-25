using System;
using System.Collections.Generic;
using QuickGraph;

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
        public float WiredWeight { get; protected set; }

        /// <summary>
        /// Вес грани для беспроводной связи, строящийся на основе пераметров смежных участков.
        /// </summary>
        public float WirelessWeight { get; protected set; }

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
                CalculateWeights(weightCoefficients);
            }
            catch (Exception ex)
            {
                Console.WriteLine("TopologyEdge failed! {1}", ex.Message);
            }
        }

        /// <summary>
        /// Рассчитать веса данной грани.
        /// </summary>
        /// <param name="weightCoefficients">Весовые коэффициенты для рассчётов весов граней.</param>
        protected void CalculateWeights(Dictionary<string, float> weightCoefficients)
        {
            try
            {
                WirelessWeight = GetWirelessWeight();

                if (IsAcrossTheBorder())
                    WiredWeight = (GetWiredWeightAcrossTheBorder(Source, Target) + GetWiredWeightAcrossTheBorder(Target, Source)) / 2;

                else if (IsAlongTheBorder())
                    WiredWeight = GetWiredWeightAlongTheBorder(Source, Target);

                else
                    WiredWeight = GetWiredWeightInside();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Edge weight calculation failed! {1}", ex.Message);
            }
        }

        /// <summary>
        /// Проверить, проведена ли данная грань через границу между участками.
        /// </summary>
        /// <returns>True, если она через границу между участками.</returns>
        public bool IsAcrossTheBorder()
        {
            return Source.Region != Target.Region;  // Грань проведена через границу, когда она ведёт на другой участок
        }

        /// <summary>
        /// Проверить, проведена ли данная грань вдоль границы участка.
        /// </summary>
        /// <returns>True, если она вдоль границы участка.</returns>
        public bool IsAlongTheBorder()
        {
            // Грань вдоль границы, когда её узлы в одном участке и они вдоль левой или правой границы или вдоль верхней или нижней границы
            return !IsAcrossTheBorder() && GetBoundaryBorderIndex(Source, Target) < 5;
        }

        /// <summary>
        /// Проверить, проведена ли данная грань внутри участка.
        /// </summary>
        /// <returns>True, если она внутри участка.</returns>
        public bool IsInside()
        {
            return !IsAlongTheBorder() && !IsAcrossTheBorder(); // Грань находится внутри участка, если она не вдоль границы и не через границу
        }

        /// <summary>
        /// Получить беспроводной вес грани. Если беспроводная связь внутри участка, то берётся 
        /// соответствующая оценка, а между участками - среднее от значений границ смежных участков.
        /// </summary>
        /// <param name="edge">Грань, вес которой рассчитывается.</param>
        /// <returns>Значение беспроводного веса грани.</returns>
        protected float GetWirelessWeight()
        {
            return IsAcrossTheBorder()
                ? (GetEstimate(Source.Region.WallsBadRadioTransmittanceEstimate, Source, Target) +
                   GetEstimate(Target.Region.WallsBadRadioTransmittanceEstimate, Target, Source)) / 2
                : Source.Region.InsideBadRadioTransmittanceEstimate;
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
            return GetEstimate(source.Region.WallsBadWiredTransmittanceEstimate, source, target) +
                   GetWiredWeightAlongTheBorder(source, target);
        }

        /// <summary>
        /// Получить проводной вес грани вдоль границы участка путём сложения экспертных оценок.
        /// </summary>
        /// <param name="source">Исходный узел грани.</param>
        /// <param name="target">Целевой узел грани.</param>
        /// <returns>Значение проводного веса.</returns>
        protected static float GetWiredWeightAlongTheBorder(TopologyVertex source, TopologyVertex target)
        {
            return 0.4f * GetEstimate(source.Region.WallsUnavailabilityEstimate, source, target) +
                   0.3f * GetEstimate(source.Region.WallsLaboriousnessEstimate, source, target) +
                   0.3f * GetEstimate(source.Region.WallsAggressivenessEstimate, source, target);
        }

        /// <summary>
        /// Получить проводной вес внутри участка.
        /// </summary>
        /// <returns>Значение проводного веса.</returns>
        protected static float GetWiredWeightInside()
        {
            return 0;
        }

        /// <summary>
        /// Получить экспертную оценку грани из массива оценок в зависимости от индекса границы и количества оценок в массиве.
        /// </summary>
        /// <param name="estimates">Массив экспертных оценок для границ участка.</param>
        /// <param name="source">Исходный узел грани.</param>
        /// <param name="target">Целевой узел грани.</param>
        /// <returns>Значение оценки из массива.</returns>
        protected static ushort GetEstimate(ushort[] estimates, TopologyVertex source, TopologyVertex target)
        {
            if (estimates == null || estimates.Length == 0)
                throw new ArgumentNullException(nameof(estimates), "Empty estimates array!");

            return estimates.Length == 4                            // Если в массиве все четыре значения
                ? estimates[GetBordeIndex(source, target)]   // Берём в зависимости от узла-источника и узла-цели
                : estimates[0];                                     // Иначе берём первое значение
        }

        /// <summary>
        /// Получить индекс границы, вдоль которой проложена грань, или через которую она проложена.
        /// </summary>
        /// <param name="source">Исходный узел грани.</param>
        /// <param name="target">Целевой узел грани.</param>
        /// <returns>Направление грани: 0 - верх, 1 - вправо, 2 - вниз, 3 - влево.</returns>
        protected static ushort GetBordeIndex(TopologyVertex source, TopologyVertex target)
        {
            ushort result = 5;

            if (!source.IsInside() && !target.IsInside())
            {
                // Если узлы на одном участке, получаем границу, вдоль которой проведена грань
                if (source.Region == target.Region)
                    result = GetBoundaryBorderIndex(source, target);

                // Иначе получаем границу, через которую проведена грань
                else
                    result = GetCrossedBorderIndex(source, target);
            }

            if (result == 5)
                throw new ArgumentException("Incorrect vertex coordinates in the number of border calculation (edge is inside)!");

            return result;
        }

        /// <summary>
        /// Получить индекс границы от 0 до 3 участка, вдоль которой проведена грань.
        /// </summary>
        /// <param name="source">Исходный узел грани.</param>
        /// <param name="target">Целевой узел грани.</param>
        /// <returns>Индекс границы: 0 - вверху, 1 - справа, 2 - снизу, 3 - слева, 5 - ошибка, грань внутри участка или идёт через границу участка.</returns>
        protected static ushort GetBoundaryBorderIndex(TopologyVertex source, TopologyVertex target)
        {
            if (source.RegionX == target.RegionX && source.RegionX == 0)
                return 3;

            if (source.RegionX == target.RegionX && source.RegionX == source.Region.Width - 1)
                return 1;

            if (source.RegionY == target.RegionY && source.RegionY == 0)
                return 0;

            if (source.RegionY == target.RegionY && source.RegionY == source.Region.Height - 1)
                return 2;

            return 5;
        }

        /// <summary>
        /// Получить индекс границы от 0 до 3 на исходном участке, через которую проходит грань.
        /// </summary>
        /// <param name="source">Исходный узел грани.</param>
        /// <param name="target">Целевой узел грани.</param>
        /// <returns>Индекс границы: 0 - верх, 1 - вправо, 2 - вниз, 3 - влево, 5 - ошибка, исходная и целевая грани находятся на одном участке.</returns>
        protected static ushort GetCrossedBorderIndex(TopologyVertex source, TopologyVertex target)
        {
            if (source.Region.Y > target.Region.Y)
                return 0;

            if (source.Region.Y < target.Region.Y)
                return 2;

            if (source.Region.X > target.Region.X)
                return 3;

            if (source.Region.X < target.Region.X)
                return 1;

            return 5;
        }
    }
}
