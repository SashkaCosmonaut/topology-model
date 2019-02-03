using System;
using System.Linq;
using TopologyModel.Enumerations;
using TopologyModel.Regions;

namespace TopologyModel.Tools
{
    /// <summary>
    /// Класс конечного устройства.
    /// </summary>
    public class MeasurementAndControlDevice : AbstractDevice
    {
        /// <summary>
        /// множество всех измерений потребления энергоресурсов, выдаваемых данным КУ
        /// </summary>
        public Measurement[] Measurements { get; set; }

        /// <summary>
        /// множество всех управляющих воздействий, позволяющих осуществлять данным КУ
        /// </summary>
        public Control[] Controls { get; set; }

        /// <summary>
        /// Проверить, подходит ли данное устройство для места учёта и управления - покрывает ли оно 
        /// те измерения и управляющиее воздействия, которые необходимы на месте учёта.
        /// На данный момент сделано допущение, что устройство покрывает более или столько же измерений,
        /// или управляющих воздействий, сколько требуется на месте учёта и управления.
        /// </summary>
        /// <param name="mcz">Место учёта и управления.</param>
        /// <returns>True, если подходит.</returns>
        public bool IsSuitableForMCZ(MeasurementAndControlZone mcz)
        {
            try
            {
                // Вначале вобще выбираем, что необходимо ли измерять или управлять на месте
                var measurementsResult = mcz.Measurements != null && mcz.Measurements.Any();
                var controlsResult = mcz.Controls != null && mcz.Controls.Any();

                if (measurementsResult)
                    foreach (var m in mcz.Measurements.TakeWhile(q => measurementsResult))
                        measurementsResult &= Measurements.Contains(m);     // Если на месте есть измерение, которое не покрывается устройством, прерываем проверку
                else
                    measurementsResult = true;      // Если нет измерений на месте, то устройство автоматически подходит

                // Аналогично с управляющими воздействиями
                if (controlsResult)
                    foreach (var c in mcz.Controls.TakeWhile(q => controlsResult))
                        controlsResult &= Controls.Contains(c);
                else
                    controlsResult = true;

                return measurementsResult && controlsResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine("IsSuitableForMCZ failed! {0}", ex.Message);

                return false;
            }
        }
    }
}
