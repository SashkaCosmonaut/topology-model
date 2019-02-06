using System.Collections.Generic;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс для сравнения объектов той части секции топологии, в которой находится УСПД.
    /// </summary>
    public class DataAcquisitionSectionPartComparer : IEqualityComparer<DataAcquisitionSectionPart>
    {
        /// <summary>
        /// Проверить, равны ли части секции, они равны, если одинаковы УСПД на них и одинаковы их вершины в графе.
        /// </summary>
        /// <param name="x">Один сравниваемый объект части секции.</param>
        /// <param name="y">Другой сравниваемый объект части секции.</param>
        /// <returns>Результат сравнения.</returns>
        public bool Equals(DataAcquisitionSectionPart x, DataAcquisitionSectionPart y)
        {
            return x.DAD == y.DAD && x.Vertex == y.Vertex;
        }

        /// <summary>
        /// Получить хэш-код объекта части секции, который состоит из хэш-кодов УСПД 
        /// и его вершины в графе, пропущенных через побитовую операцию исключающего ИЛИ.
        /// </summary>
        /// <param name="obj">Анализируемый объект.</param>
        /// <returns>Хэш-код объекта.</returns>
        public int GetHashCode(DataAcquisitionSectionPart obj)
        {
            return obj.DAD.GetHashCode() ^ obj.Vertex.GetHashCode();
        }
    }
}
