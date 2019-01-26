using Newtonsoft.Json;
using TopologyModel.Enumerations;

namespace TopologyModel.Regions
{
    /// <summary>
    /// Класс места учёта и управления (или только точки учёта и управления).
    /// </summary>
    public class MeasurementAndControlZone : AbstractRegion
	{
        /// <summary>
        /// Глобальный автоприращиваемый идентификатор.
        /// </summary>
        private static uint GlobalId = 0;

        /// <summary>
        /// экспертная оценка приоритета покрытия ТУУ сетью, от 0 и далее, 0 - наивысшая
        /// </summary>
        public uint Priority { get; set; }

		/// <summary>
		/// множество всех измерений потребления энергоресурсов, доступных на данной ТУУ
		/// </summary>
		public Measurement[] Measurements { get; set; }

		/// <summary>
		/// множество всех управляющих воздействий, доступных на данной ТУУ
		/// </summary>
		public Control[] Сontrols { get; set; }

		/// <summary>
		/// допустима ли замена уже имеющихся КУ на ТУУ на более новые
		/// </summary>
		public bool IsDeviceReplacementAvailable { get; set; }

        /// <summary>
        /// Создать и проинициализировать место учёта и управления по умолчанию.
        /// </summary>
        public MeasurementAndControlZone()
        {
            Id = ++GlobalId;
        }

        /// <summary>
        /// Получить информацию об основных свойствах участка.
        /// </summary>
        /// <returns>Строка с JSON-объектом свойств участка.</returns>
        public override string GetInfo()
        {
            return JsonConvert.SerializeObject(new
            {
                Name,
                Priority,
                Measurements,
                Сontrols,
                IsDeviceReplacementAvailable
            });
        }
    }
}
