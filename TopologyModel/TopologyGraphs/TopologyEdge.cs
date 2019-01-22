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
                var sourceRegion = Source.Region;
                var targetRegion = Target.Region;
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
            // Грань находится внутри участка, если хотя бы одна из вершин находится во внутренней части участка
            return Source.IsInside() || Target.IsInside();
        }

        /// <summary>
        /// Проверить, проведена ли данная грань вдоль границы участка.
        /// </summary>
        /// <returns>True, если она вдоль границы участка.</returns>
        public bool IsAlongTheBorder()
        {
            // Грань проведена вдоль границы участка, когда обе вершины не внутри, но они на одном участке и имеют общую координату Х или Y
            return Source.Region == Target.Region && !Source.IsInside() && !Target.IsInside() && (Source.RegionX == Target.RegionX || Source.RegionY == Target.RegionY);
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
    }
}
