using GeneticSharp.Domain.Chromosomes;
using System;
using System.Collections.Generic;

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
        public const int GENES_FOR_SECTION = 12;

        /// <summary>
        /// Словарь функций генерации каждого гена в хромосоме, где ключ - индекс гена, а значение - создающая его функция.
        /// </summary>
        protected static Dictionary<int, Func<Gene>> GeneGenerationFuncs = new Dictionary<int, Func<Gene>>
        {

        };

        /// <summary>
        /// Ссылка на текущий проект по генерации топологии сети.
        /// </summary>
        public Project CurrentProject { get; set; }

        /// <summary>
        /// Наполняемое по результатам работы алгоритма поле предлагаемой топологии.
        /// </summary>
        private TopologySection[] _topology;

        /// <summary>
        /// Декодировать из хромосомы прелагаемую топологию сети (фенотип), состоящую из секций для каждого
        /// места учёта и управления, соединенного с УСПД через два или менее приемопередатчика.
        /// </summary>
        public TopologySection[] Topology
        {
            get {
                return _topology;
            }
        }

        /// <summary>
        /// Сформировать хромосому на основании параметров проекта. Длина хромосомы - 
        /// количество мест учёта и управления, умноженное на количество генов в одной секции хромосомы.
        /// </summary>
        /// <param name="project">Проект по генерации топологии сети.</param>
        public TopologyChromosome(Project project) : base(project.MCZs.Length * GENES_FOR_SECTION)
        {
            CurrentProject = project;

            _topology = new TopologySection[project.MCZs.Length];

            for (var i = 0; i < Length; i++)
                ReplaceGene(i, GenerateGene(i));
        }

        /// <summary>
        /// Генерировать новый ген для хромосомы.
        /// </summary>
        /// <param name="geneIndex">Индекс гена.</param>
        /// <returns>Новый объект гена.</returns>
        public override Gene GenerateGene(int geneIndex)
        {
            var geneInSecionIndex = GetGeneInSectionIndex(geneIndex);

            if (!GeneGenerationFuncs.ContainsKey(geneInSecionIndex))
                return new Gene(0);

            return GeneGenerationFuncs[geneInSecionIndex].Invoke();
        }

        /// <summary>
        /// Получить индекс секции, в которой находится ген по указанному индексу.
        /// </summary>
        /// <param name="geneIndex">Индекс гена в хромосоме.</param>
        /// <returns>Индекс секции.</returns>
        protected int GetSectionIndex(int geneIndex)
        {
            return geneIndex / GENES_FOR_SECTION;
        }

        /// <summary>
        /// Получить индекс гена внутри секции.
        /// </summary>
        /// <param name="geneIndex">Индекс гена в хромосоме.</param>
        /// <returns></returns>
        protected int GetGeneInSectionIndex(int geneIndex)
        {
            return geneIndex % GENES_FOR_SECTION;
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
