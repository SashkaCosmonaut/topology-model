using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using TopologyModel.TopologyGraphs;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс хромосомы топологии.
    /// </summary>
    public class TopologyChromosome : ChromosomeBase
    {
        /// <summary>
        /// Количество генов в участке хромосомы.
        /// </summary>
        public const ushort GENES_FOR_SECTION = 12;

        /// <summary>
        /// Декодировать из хромосомы прелагаемую топологию сети (фенотип), состоящую из секций для каждого
        /// места учёта и управления, соединенного с УСПД через два или менее приемопередатчика.
        /// </summary>
        public TopologySection[] Topology
        {
            get {
                return null;
            }
        }

        /// <summary>
        /// Сформировать хромосому на основании параметров проекта. Длина хромосомы - 
        /// количество мест учёта и управления, умноженное на количество генов в одной секции хромосомы.
        /// </summary>
        /// <param name="project">Проект по генерации топологии сети.</param>
        public TopologyChromosome(Project project) : base(project.MCZs.Length * GENES_FOR_SECTION)
        {
            
        }

        public override IChromosome CreateNew() => throw new System.NotImplementedException();

        public override Gene GenerateGene(int geneIndex) => throw new System.NotImplementedException();
    }
}
