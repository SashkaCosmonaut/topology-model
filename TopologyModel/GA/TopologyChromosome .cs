using GeneticSharp.Domain.Chromosomes;
using System;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс хромосомы топологии, состоящей из секций для подключения каждого места учёта и управления.
    /// </summary>
    public class TopologyChromosome : ChromosomeBase
    {
        /// <summary>
        /// Количество генов в секции топологии хромосомы.
        /// </summary>
        public const int GENES_FOR_SECTION = 5;

        /// <summary>
        /// Ссылка на текущий проект с параметрами для генерации топологии сети.
        /// </summary>
        public Project CurrentProject { get; set; }

        /// <summary>
        /// Наполняемое по результатам работы алгоритма поле предлагаемой топологии - результат работы метода.
        /// </summary>
        private TopologySection[] _topology;

        /// <summary>
        /// Декодировать из генотипа хромосомы прелагаемую топологию сети (фенотип), состоящую из секций 
        /// для каждого места учёта и управления, соединенного с УСПД - результат работы метода.
        /// </summary>
        public TopologySection[] Topology
        {
            get {
                try
                {
                    // Поочерёдно декодируем каждую секцию
                    for (var sectionIndex = 0; sectionIndex < Length / GENES_FOR_SECTION; sectionIndex++)
                    {
                        if (_topology[sectionIndex] == null)
                            _topology[sectionIndex] = new TopologySection();

                        _topology[sectionIndex].Decode(this, sectionIndex);
                    }

                    return _topology;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Topology failed! {0}", ex.Message);
                    return _topology;
                }
            }
        }

        /// <summary>
        /// Сформировать хромосому на основании параметров проекта. Длина хромосомы - 
        /// количество мест учёта и управления, умноженное на количество генов в одной секции хромосомы.
        /// </summary>
        /// <param name="project">Проект по генерации топологии сети.</param>
        public TopologyChromosome(Project project) : base(project.MCZs.Length * GENES_FOR_SECTION)
        {
            try
            {
                CurrentProject = project;

                _topology = new TopologySection[project.MCZs.Length];

                for (var geneIndex = 0; geneIndex < Length; geneIndex++)
                    ReplaceGene(geneIndex, GenerateGene(geneIndex));
            }
            catch (Exception ex)
            {
                Console.WriteLine("TopologyChromosome failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Генерировать новый ген для хромосомы.
        /// </summary>
        /// <param name="geneIndex">Индекс гена.</param>
        /// <returns>Новый объект гена.</returns>
        public override Gene GenerateGene(int geneIndex)
        {
            try
            {
                return new Gene(TopologySection.GenerateGeneValue(this, geneIndex));
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenerateGene failed! {0}", ex.Message);
                return new Gene();
            }
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
    }
}
