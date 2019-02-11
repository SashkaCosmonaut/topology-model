using System.Collections.Generic;

namespace TopologyModel.Graphs
{
    /// <summary>
    /// Элемент кэша путей в графе.
    /// </summary>
    public class TopologyPathesCacheItem
    {
        /// <summary>
        /// Вершина графа - источник пути.
        /// </summary>
        public TopologyVertex Source { get; set; }

        /// <summary>
        /// Вершина пути - цель пути.
        /// </summary>
        public TopologyVertex Target { get; set; }

        /// <summary>
        /// Является ли путь беспроводным.
        /// </summary>
        public bool IsWireless { get; set; }

        /// <summary>
        /// Перечисление граней графа, составляющих путь.
        /// </summary>
        public IEnumerable<TopologyEdge> Path { get; set; }

        /// <summary>
        /// Создать и проинициализировать элемент кэша путей в графе.
        /// </summary>
        /// <param name="source">Вершина графа - источник пути.</param>
        /// <param name="target">Вершина пути - цель пути.</param>
        /// <param name="isWireless">Является ли путь беспроводным.</param>
        /// <param name="path">Перечисление граней графа, составляющих путь.</param>
        public TopologyPathesCacheItem(TopologyVertex source, TopologyVertex target, bool isWireless, IEnumerable<TopologyEdge> path)
        {
            Source = source;
            Target = target;
            IsWireless = isWireless;
            Path = path;
        }
    }
}
