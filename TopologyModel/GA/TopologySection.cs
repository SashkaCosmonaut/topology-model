using System;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс части (секции), на которые разбита топология, 
    /// для связи одного места учёта и управления с сервером.
    /// </summary>
    public class TopologySection
    {
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
        /// <param name="sectionChromosomePart">Часть хромосомы декодируемой секции.</param>
        public void Decode(Project project, int[] sectionGenes)
        {
            try
            {
                MACPart.Decode(project, sectionGenes[0], sectionGenes[1], sectionGenes[2]);
                DADPart.Decode(project, sectionGenes[3], sectionGenes[4], sectionGenes[5]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("TopologySection Decode failed! {0}", ex.Message);
            }
        }
    }
}
