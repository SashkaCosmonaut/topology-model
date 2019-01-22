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
        /// Проверить, проведена ли данная грань внутри участка.
        /// </summary>
        /// <returns>True, если она внутри участка.</returns>
        public bool IsInside()
        {
            // Грань находится внутри участка, если связь не на границе и не через границу
            return !IsAlongTheBorder() && !IsAcrossTheBorder();
        }

        /// <summary>
        /// Проверить, проведена ли данная грань вдоль границы участка.
        /// </summary>
        /// <returns>True, если она вдоль границы участка.</returns>
        public bool IsAlongTheBorder()
        {
            // TODO: КАК БЫТЬ КОГДА УЗКИЙ УЧАСТОКК?

            // Грань проведена вдоль границы участка, когда обе вершины не внутри, но они на одном участке и имеют общую координату Х или Y
            return !IsAcrossTheBorder() && !Source.IsInside() && !Target.IsInside() && (Source.RegionX == Target.RegionX || Source.RegionY == Target.RegionY);
        }

        /// <summary>
        /// Проверить, проведена ли данная грань через границу между участками.
        /// </summary>
        /// <returns>True, если она через границу между участками.</returns>
        public bool IsAcrossTheBorder()
        {
            // Грань проведена через границу, когда она ведёт на другой участок
            return Source.Region != Target.Region;
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
        /// Получить направление данной грани.
        /// </summary>
        /// <returns>Направление грани: 0 - верх, 1 - вправо, 2 - вниз, 3 - влево.</returns>
        protected static uint Direction(TopologyVertex source, TopologyVertex target)
        {
            if (source.Region == target.Region) // Если вершины на одном участке, сравниваем их внутренние координаты
                return CoordinateDirection(source.RegionX, target.RegionX, source.RegionY, target.RegionY);

            // Иначе сравниваем реальные координаты самих участков
            return CoordinateDirection(source.Region.X, target.Region.X, source.Region.Y, target.Region.Y);
        }

        /// <summary>
        /// Получить направление в зависимости от координат.
        /// </summary>
        /// <param name="sourceX">Координата источника по Х.</param>
        /// <param name="targetX">Координата цели по Х.</param>
        /// <param name="sourceY">Координата источника по Y.</param>
        /// <param name="targetY">Координата цели по Y.</param>
        /// <returns>Направление грани: 0 - верх, 1 - вправо, 2 - вниз, 3 - влево.</returns>
        protected static uint CoordinateDirection(uint sourceX, uint targetX, uint sourceY, uint targetY)
        {
            if (sourceY > targetY)
                return 0;

            if (sourceY < targetY)
                return 2;

            if (sourceX > targetX)
                return 3;

            return 1;
        }

        /// <summary>
        /// Получить непроходимость радиоволн для соответствующих границ двух смежных участков.
        /// </summary>
        /// <param name="source">Исходный участок.</param>
        /// <param name="target">Целевой участок.</param>
        /// <returns>Значение непроходимости радиоволн.</returns>
        protected static ushort GetWallsBadRadioTransmittanceEstimate(TopologyVertex source, TopologyVertex target)
        {
            var sourceEstimates = source.Region.WallsBadRadioTransmittanceEstimate; // Оценки границ исходного участка

            if (sourceEstimates == null || sourceEstimates.Length == 0) return 0;

            return sourceEstimates.Length == 4                  // Если в массиве все четыре значения
                ? sourceEstimates[Direction(source, target)]    // Берём в зависимости от направления
                : sourceEstimates[0];                           // Иначе берём первое
        }
    }
}
