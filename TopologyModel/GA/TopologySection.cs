using System;
using System.Linq;

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
            MeasurementAndControlPart.GenerateDeviceGene,
            MeasurementAndControlPart.GenerateVertexGene,
            MeasurementAndControlPart.GenerateChannelGene,
            DataAcquisitionPart.GenerateDeviceGene,
            DataAcquisitionPart.GenerateVertexGene,
            DataAcquisitionPart.GenerateChannelGene
        };

        /// <summary>
        /// Параметры выбора и расположения устройства учёта и управления, а так же исходящего КПД.
        /// </summary>
        public MeasurementAndControlPart MACPart { get; } = new MeasurementAndControlPart();

        /// <summary>
        /// Параметры выбора и расположения УСПД, а так же входящего КПД.
        /// </summary>
        public DataAcquisitionPart DADPart { get; } = new DataAcquisitionPart();

        /// <summary>
        /// Декодировать текущую секцию из генотипа.
        /// </summary>
        /// <param name="chromosome">Декодируемая хромосома.</param>
        /// <param name="sectionIndex">Индекс секции в декодируемой хромосоме.</param>
        public void Decode(TopologyChromosome chromosome, int sectionIndex)
        {
            try
            {
                var sectionGenes = chromosome.GetGenes()       // Получаем гены только данной секции
                    .Select((gene, index) => new { Value = (int)gene.Value, Index = index })
                    .Where(q => TopologyChromosome.GetSectionIndex(q.Index) == sectionIndex)
                    .Select(q => q.Value)
                    .ToArray();

                MACPart.Decode(chromosome.CurrentProject, sectionGenes[0], sectionGenes[1], sectionGenes[2]);
                DADPart.Decode(chromosome.CurrentProject, sectionGenes[3], sectionGenes[4], sectionGenes[5]);
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
                var geneInSectionIndex = TopologyChromosome.GetGeneInSectionIndex(geneIndex);

                if (geneIndex < 0 || geneIndex > GeneValueGenerationFuncs.Length)
                    return 0;

                // Вызываем соответствующую функцию для генерации гена
                return GeneValueGenerationFuncs[geneInSectionIndex].Invoke(chromosome, TopologyChromosome.GetSectionIndex(geneIndex));
            }
            catch (Exception ex)
            {
                Console.WriteLine("EncodeGeneValue failed! {0}", ex.Message);
                return 0;
            }
        }
    }
}
