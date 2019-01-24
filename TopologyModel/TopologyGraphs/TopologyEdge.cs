using QuickGraph;
using System;
using System.Collections.Generic;

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
                // Если внутри участка, то вес беспроводной связи - соответствующая оценка, между участками - среднее от значений границ смежных участков
                WirelessWeight = IsAcrossTheBorder()
                    ? (GetWallsBadRadioTransmittanceEstimate(Source, Target) + GetWallsBadRadioTransmittanceEstimate(Target, Source)) / 2
                    : Source.Region.InsideBadRadioTransmittanceEstimate;
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
            return !IsAcrossTheBorder() &&
                ((Source.RegionX == Target.RegionX && (Source.RegionX == 0 || Source.RegionX == Source.Region.Width - 1)) ||
                  (Source.RegionY == Target.RegionY && (Source.RegionY == 0 || Source.RegionY == Source.Region.Height - 1)));
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
            return $"{WiredWeight:0.0}, {WirelessWeight:0.0}";
        }

        /// <summary>
        /// Получить непроходимость радиоволн для границы, исходящей из одного исходного в уелевой.
        /// </summary>
        /// <param name="source">Исходный участок.</param>
        /// <param name="target">Целевой участок.</param>
        /// <returns>Значение непроходимости радиоволн через границу из исходного участка.</returns>
        protected static ushort GetWallsBadRadioTransmittanceEstimate(TopologyVertex source, TopologyVertex target)
        {
            var estimates = source.Region.WallsBadRadioTransmittanceEstimate; // Оценки границ исходного участка

            if (estimates == null || estimates.Length == 0) return 0;

            return estimates.Length == 4                  // Если в массиве все четыре значения
                ? estimates[GetStraightDirection(source, target)]    // Берём в зависимости от направления грани
                : estimates[0];                           // Иначе берём первое
        }

        /// <summary>
        /// Получить направление грани, расположенной между участками (или внутри, но только не диагональные).
        /// </summary>
        /// <returns>Направление грани: 0 - верх, 1 - вправо, 2 - вниз, 3 - влево.</returns>
        protected static uint GetStraightDirection(TopologyVertex source, TopologyVertex target)
        {
            if (source.Region == target.Region) // Если вершины на одном участке, сравниваем их внутренние координаты
                return GetStraightDirectionByCoordinates(source.RegionX, target.RegionX, source.RegionY, target.RegionY);

            // Иначе сравниваем реальные координаты самих участков
            return GetStraightDirectionByCoordinates(source.Region.X, target.Region.X, source.Region.Y, target.Region.Y);
        }

        /// <summary>
        /// Получить направление в зависимости от координат.
        /// </summary>
        /// <param name="sourceX">Координата источника по Х.</param>
        /// <param name="targetX">Координата цели по Х.</param>
        /// <param name="sourceY">Координата источника по Y.</param>
        /// <param name="targetY">Координата цели по Y.</param>
        /// <returns>Направление грани: 0 - верх, 1 - вправо, 2 - вниз, 3 - влево.</returns>
        protected static uint GetStraightDirectionByCoordinates(uint sourceX, uint targetX, uint sourceY, uint targetY)
        {
            if (sourceY > targetY)
                return 0;

            if (sourceY < targetY)
                return 2;

            if (sourceX > targetX)
                return 3;

            return 1;
        }
    }
}
