using GeneticSharp.Domain.Randomizations;
using System;
using System.Linq;
using TopologyModel.Equipments;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс части (секции), на которые разбита топология, 
    /// для связи одного места учёта и управления с сервером.
    /// </summary>
    public class TopologySection
    {
        /// <summary>
        /// Массив функций генерации каждого соответсвтующего гена в хромосоме в зависимости от его расположения в секции, где порядковы номер функции 
        /// соответствует гену в секции, а значение - создающая его функция, возвращающая целочисленное значение гена.
        /// </summary>
        protected static Func<TopologyChromosome, int, int>[] GeneValueGenerationFuncs = new Func<TopologyChromosome, int, int>[]
        {
            MeasurementAndControlSectionPart.GenerateDeviceGene,
            MeasurementAndControlSectionPart.GenerateVertexGene,
            DataAcquisitionSectionPart.GenerateDeviceGene,
            DataAcquisitionSectionPart.GenerateVertexGene,
            GenerateDataChannelGene,
        };

        /// <summary>
        /// Параметры выбора и расположения устройства учёта и управления, а так же исходящего КПД.
        /// </summary>
        public MeasurementAndControlSectionPart MCDPart { get; } = new MeasurementAndControlSectionPart();

        /// <summary>
        /// Параметры выбора и расположения УСПД, а так же входящего КПД.
        /// </summary>
        public DataAcquisitionSectionPart DADPart { get; } = new DataAcquisitionSectionPart();

        /// <summary>
        /// Канал передачи данных, который используется для передачи данных между КУ и УСПД.
        /// </summary>
        public DataChannel Channel { get; protected set; }

        /// <summary>
        /// Декодировать текущую секцию из генотипа.
        /// </summary>
        /// <param name="chromosome">Декодируемая хромосома.</param>
        /// <param name="sectionIndex">Индекс секции в декодируемой хромосоме.</param>
        public void Decode(TopologyChromosome chromosome, int sectionIndex)
        {
            try
            {
                var sectionGenes = chromosome.GetGenes()       // Получаем значения генов только данной секции
                    .Select((gene, index) => new { Value = (int)gene.Value, Index = index })
                    .Where(q => GetSectionIndex(q.Index) == sectionIndex)
                    .Select(q => q.Value)
                    .ToArray();

                if (sectionGenes.Length < GeneValueGenerationFuncs.Length)
                    throw new Exception("Incorrect section size!");

                // 0-й ген - тип КУ, 1-й ген - узел КУ, 2-й ген - тип УСПД, 3-й ген - узел УСПД, 4-й ген - КПД
                MCDPart.Decode(chromosome.CurrentProject, sectionGenes[0], sectionGenes[1]);
                DADPart.Decode(chromosome.CurrentProject, sectionGenes[2], sectionGenes[3]);

                Channel = chromosome.CurrentProject.Equipments.DCs[sectionGenes[4]];
            }
            catch (Exception ex)
            {
                Console.WriteLine("TopologySection Decode failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Сгенерировать новое значение гена секции в зависимости от индекса гена.
        /// </summary>
        /// <param name="chromosome">Хромосома, для гена которой предназначено данное значение.</param>
        /// <param name="geneIndex">Индекс гена в хромосоме.</param>
        /// <returns>Новое случайное значение гена для хромосомы.</returns>
        public static int GenerateGeneValue(TopologyChromosome chromosome, int geneIndex)
        {
            try
            {
                var geneInSectionIndex = GetGeneInSectionIndex(geneIndex);

                if (geneInSectionIndex >= GeneValueGenerationFuncs.Length)
                    return 0;

                // Вызываем соответствующую функцию для генерации гена
                return GeneValueGenerationFuncs[geneInSectionIndex].Invoke(chromosome, GetSectionIndex(geneIndex));
            }
            catch (Exception ex)
            {
                Console.WriteLine("EncodeGeneValue failed! {0}", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Сгенерировать новый ген, представляющий случайный КПД для связьи устройства учёта и управления и УСПД, 
        /// выбирается такой, чтобы подходил к устройству и к КПД.
        /// </summary>
        /// <param name="chromosome">Текущая хромосома.</param>
        /// <param name="sectionIndex">Индекс секции, для которой генерируется ген.</param>
        /// <returns>Целочисленное значение случайного гена, соответствующее индексу в массиве доступных каналов передачи данных.</returns>
        protected static int GenerateDataChannelGene(TopologyChromosome chromosome, int sectionIndex)
        {
            try
            {
                return RandomizationProvider.Current.GetInt(0, chromosome.CurrentProject.Equipments.DCs.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenerateDataChannelGene failed! {0}", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Получить индекс секции, в которой находится ген по указанному индексу.
        /// </summary>
        /// <param name="geneIndex">Индекс гена в хромосоме.</param>
        /// <returns>Индекс секции.</returns>
        public static int GetSectionIndex(int geneIndex)
        {
            return geneIndex / TopologyChromosome.GENES_IN_SECTION;
        }

        /// <summary>
        /// Получить индекс гена внутри секции.
        /// </summary>
        /// <param name="geneIndex">Индекс гена в хромосоме.</param>
        /// <returns>Индекс гена внутри секции.</returns>
        public static int GetGeneInSectionIndex(int geneIndex)
        {
            return geneIndex % TopologyChromosome.GENES_IN_SECTION;
        }
    }
}
