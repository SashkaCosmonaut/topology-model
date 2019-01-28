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
        /// Ссылка на текущий проект по генерации топологии сети.
        /// </summary>
        public Project CurrentProject { get; set; }

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
            for (var i = 0; i < GENES_FOR_SECTION; i++)
                ReplaceGene(i, GenerateGene(i));
        }

        /// <summary>
        /// Создать новую хромосому на базе текущего проекта.
        /// </summary>
        /// <returns>Новый объект хромосомы.</returns>
        public override IChromosome CreateNew()
        {
            return new TopologyChromosome(CurrentProject);
        }

        /// <summary>
        /// Клонировать данную хромосому, которая ссылается на тот же объект проекта.
        /// </summary>
        /// <returns>Новый объект-клон текущей хромосомы.</returns>
        public override IChromosome Clone()
        {
            var clone = base.Clone() as TopologyChromosome;

            clone.CurrentProject = CurrentProject;

            return clone;
        }

        /// <summary>
        /// Генерировать новый ген для хромосомы.
        /// </summary>
        /// <param name="geneIndex">Индекс гена.</param>
        /// <returns>Новый объект гена.</returns>
        public override Gene GenerateGene(int geneIndex)
        {
            // Создать ген, опираясь от порядкового номера
            return new Gene();
        }
    }
}
