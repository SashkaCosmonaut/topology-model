using System;
using System.Linq;
using TopologyModel.Enumerations;
using TopologyModel.Regions;

namespace TopologyModel.Equipments
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
                var measurementsResult = mcz.Measurements != null && mcz.Measurements.Any()
                    ? Measurements!= null && Measurements.Any() && mcz.Measurements.All(m => Measurements.Contains(m))  // Устройство подхдит, если содержит все измерения места
                    : true;                                                                                             // Если нет измерений на месте, то устройство автоматически подходит    

                var controlsResult = mcz.Controls != null && mcz.Controls.Any()
                    ? Controls != null && Controls.Any() && mcz.Controls.All(c => Controls.Contains(c)) // Устройство подхдит, если поддерживает все управляющие воздействия места
                    : true;                                                                             // Если воздействий на месте не требуется, то устройство автоматически подходит    

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
