using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс хромосомы топологии, состоящей из секций для каждого места учёта и управления.
    /// </summary>
    public class TopologyChromosome : ChromosomeBase
    {
        /// <summary>
        /// Количество генов для секции топологии в хромосоме.
        /// </summary>
        public const int GENES_FOR_SECTION = 6;

        /// <summary>
        /// Словарь функций генерации каждого гена в хромосоме в зависимости от его расположения в секции, где ключ - 
        /// индекс гена в секции, а значение - создающая его функция, возвращающая целочисленное значение гена.
        /// </summary>
        protected static Dictionary<int, Func<TopologyChromosome, int, int>> GeneValueGenerationFuncs = new Dictionary<int, Func<TopologyChromosome, int, int>>
        {
            { 0, GenerateMCDeviceGene },
            { 1, GenerateMCVertexGene },
            { 2, GenerateMCChannelGene },
            { 3, GenerateDADeviceGene },
            { 4, GenerateDAVertexGene },
            { 5, GenerateDAChannelGene }
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
                // Поочерёдно декодируем каждую секцию
                for (var sectionIndex = 0; sectionIndex < Length / GENES_FOR_SECTION; sectionIndex++)
                    _topology[sectionIndex] = DecodeSection(sectionIndex);

                return _topology;
            }
        }

        /// <summary>
        /// Раскодировать фенотип секции из генотипа.
        /// </summary>
        /// <param name="sectionIndex">Номер секции топологии графа.</param>
        /// <returns>Раскодированный фенотип в виде секции топологии сети.</returns>
        protected TopologySection DecodeSection(int sectionIndex)
        {
            var resultSection = _topology[sectionIndex] ?? new TopologySection();

            var sectionGenes = GetGenes()
                .Select((gene, index) => new { Value = (int)gene.Value, Index = index })
                .Where(q => GetSectionIndex(q.Index) == sectionIndex)
                .Select(q => q.Value)
                .ToArray();

            resultSection.Decode(CurrentProject, sectionGenes);

            return resultSection;
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
                var geneInSecionIndex = GetGeneInSectionIndex(geneIndex);

                if (!GeneValueGenerationFuncs.ContainsKey(geneInSecionIndex))
                    return new Gene(0);

                return new Gene(GeneValueGenerationFuncs[geneInSecionIndex].Invoke(this, GetSectionIndex(geneIndex)));
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenerateGene failed! {0}", ex.Message);
                return new Gene();
            }
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
        /// <returns>Индекс гена внутри секции.</returns>
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

        /// <summary>
        /// Генерировать новый ген, представляющий случайное устройство учёта и управления.
        /// Из всех доступных устройств, выбираются только те, которые подходят для данного места учёта и управления.
        /// Ген является индексом в массиве всех доступных устройств.
        /// </summary>
        /// <param name="chromosome">Текущая хромосома.</param>
        /// <param name="sectionIndex">Индекс секции, для которой генерируется ген.</param>
        /// <returns>Целочисленное значение случайного гена, соответствующее индексу в массиве доступных устройств.</returns>
        protected static int GenerateMCDeviceGene(TopologyChromosome chromosome, int sectionIndex)
        {
            try
            {
                var suitableMCDs = chromosome.CurrentProject.AvailableTools.MCDs
                    .Select((mcd, index) => new { MCD = mcd, Index = index })                           // Запоминаем индекс устройства в массиве всех доступных устройств
                    .Where(q => q.MCD.IsSuitableForMCZ(chromosome.CurrentProject.MCZs[sectionIndex]))   // Выбираем только те устройства, которые подходят для места
                    .ToArray();

                var randomIndex = RandomizationProvider.Current.GetInt(0, suitableMCDs.Count());    // Выбираем из массива выбранных устройств случайное

                return suitableMCDs[randomIndex].Index;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenerateMCDevice failed! {0}", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Сгенерировать новый ген, представляющий случайную вершину графа, который находится на выбранном 
        /// месте учёта и управления. Ген является индексом в массиве вершин графа, на которых
        /// находится данное место учёта и управления.
        /// </summary>
        /// <param name="chromosome">Текущая хромосома.</param>
        /// <param name="sectionIndex">Индекс секции, для которой генерируется ген.</param>
        /// <returns>Целочисленное значение случайного гена, соответствующее индексу в массиве вершин графа.</returns>
        protected static int GenerateMCVertexGene(TopologyChromosome chromosome, int sectionIndex)
        {
            try
            {
                var currentMCZ = chromosome.CurrentProject.MCZs[sectionIndex];    // Текущее место учёта и управления

                var mczVertices = chromosome.CurrentProject.Graph.VerticesArray
                    .Select((vertex, index) => new { Vertex = vertex, Index = index })  // Запоминаем индекс в массиве каждого места
                    .Where(q => q.Vertex.MCZs.Contains(currentMCZ)).ToArray();          // Берём те вершины графа, где располагается данное место

                var randomIndex = RandomizationProvider.Current.GetInt(0, mczVertices.Count());

                return mczVertices[randomIndex].Index;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenerateMCDVertexGene failed! {0}", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Сгенерировать новый ген, представляющий случайный КПД для устройства учёта и управления, выбирается 
        /// такой, чтобы подходил к устройству.
        /// </summary>
        /// <param name="chromosome">Текущая хромосома.</param>
        /// <param name="sectionIndex">Индекс секции, для которой генерируется ген.</param>
        /// <returns>Целочисленное значение случайного гена, соответствующее индексу в массиве доступных каналов передачи данных.</returns>
        protected static int GenerateMCChannelGene(TopologyChromosome chromosome, int sectionIndex)
        {
            try
            {
                // Декодируем устройство из гена, которое выбрано в данной секции (оно идёт первым хромосоме)
                var device = chromosome.CurrentProject.AvailableTools.MCDs[(int)chromosome.GetGene(sectionIndex * GENES_FOR_SECTION).Value];

                var availableChannels = chromosome.CurrentProject.AvailableTools.DCs
                    .Select((channel, index) => new { Channel = channel, Index = index })
                    .Where(q => device.SendingProtocols.Contains(q.Channel.Protocol))   // Выбираем те КПД, которые совместимы с данным устройством
                    .ToArray();

                var randomIndex = RandomizationProvider.Current.GetInt(0, availableChannels.Count());

                return availableChannels[randomIndex].Index;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenerateMCTransmission failed! {0}", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Сгенерировать новый ген, представляющий случайное устройство сбора и передачи данных.
        /// </summary>
        /// <param name="chromosome">Текущая хромосома.</param>
        /// <param name="sectionIndex">Индекс секции, для которой генерируется ген.</param>
        /// <returns>Целочисленное значение случайного гена, соответствующее индексу в массиве УСПД.</returns>
        protected static int GenerateDADeviceGene(TopologyChromosome chromosome, int sectionIndex)
        {
            try
            {
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenerateDADeviceGene failed! {0}", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Сгенерировать новый ген, представляющий случайную вершину графа, для расположения в ней УСПД.
        /// </summary>
        /// <param name="chromosome">Текущая хромосома.</param>
        /// <param name="sectionIndex">Индекс секции, для которой генерируется ген.</param>
        /// <returns>Целочисленное значение случайного гена, соответствующее индексу в массиве вершин графа.</returns>
        protected static int GenerateDAVertexGene(TopologyChromosome chromosome, int sectionIndex)
        {
            try
            {
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenerateDAVertexGene failed! {0}", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Сгенерировать новый ген, представляющий случайный КПД, совместимый с выбранным УСПД в секции.
        /// </summary>
        /// <param name="chromosome">Текущая хромосома.</param>
        /// <param name="sectionIndex">Индекс секции, для которой генерируется ген.</param>
        /// <returns>Целочисленное значение случайного гена, соответствующее индексу в массиве каналов передачи данных.</returns>
        protected static int GenerateDAChannelGene(TopologyChromosome chromosome, int sectionIndex)
        {
            try
            {
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenerateDAChannelGene failed! {0}", ex.Message);
                return 0;
            }
        }
    }
}
