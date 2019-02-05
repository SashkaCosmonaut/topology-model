using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System;
using System.Linq;

namespace TopologyModel.GA
{
    /// <summary>
    /// Класс фитнес-функции для топологии.
    /// </summary>
    public class TopologyFitness : IFitness
    {
        /// <summary>
        /// Оценить приспособленность хромосомы топологии.
        /// </summary>
        /// <param name="chromosome">Оцениваемая хромосома с топологией.</param>
        /// <returns>Результат оценки.</returns>
        public double Evaluate(IChromosome chromosome)
        {
            try
            {
                var topologyChromosome = chromosome as TopologyChromosome;

                if (topologyChromosome == null) return 0;

                var topology = topologyChromosome.Decode();

                var fitness = 0.0;  // Результирующее значение функции

                var project = topologyChromosome.CurrentProject;

                // 0. Считаем стоимость использования сервера
                if (project.UseLocalServer)
                    fitness += project.LocalServerMonthlyPayment * project.UsageMonths;
                else
                    fitness += project.RemoteServerMonthlyPayment * project.UsageMonths;

                // 1. Группируем секции по УСПД
                foreach (var dadGroup in topology.Sections.GroupBy(q => q.DADPart))     // 2. Перебираем группы
                {
                    // 2.1. Считаем стоимость УСПД каждой группы, чем больше УСПД в общих группах, тем ниже стоимость
                    fitness += dadGroup.Key.GetCost(project);

                    // 2.2. Считаем суммарную стоимость счётчиков в группе
                    fitness += dadGroup.Sum(q => q.MACPart.GetCost(project));
                }

                return -fitness;     // Значение общей стоимости и будет результатом фитнес функции
            }
            catch (Exception ex)
            {
                Console.WriteLine("Evaluate failed! {0}", ex.Message);
                return 0;
            }
        }
    }
}
