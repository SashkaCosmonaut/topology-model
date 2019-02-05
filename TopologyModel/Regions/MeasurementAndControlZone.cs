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
        /// Экспертная оценка приоритета покрытия ТУУ сетью, от 0 и далее, 0 - наивысшая
        /// </summary>
        public uint Priority { get; set; }

        /// <summary>
        /// Множество всех измерений потребления энергоресурсов, доступных на данной ТУУ
        /// TODO: как быть, когда надо ставить несколько счётчиков на несколько измерений?
        /// </summary>
        public Measurement[] Measurements { get; set; }

        /// <summary>
        /// Множество всех управляющих воздействий, доступных на данной ТУУ
        /// </summary>
        public Control[] Controls { get; set; }

        /// <summary>
        /// Допустима ли замена уже имеющихся КУ на ТУУ на более новые
        /// TODO: добавить поле с установленным уже оборудованием, тогда в ГА выбирается только оно, 
        /// а в зависимости от этого флага в новом гене выбирается что-то новое или берётся постоянно только имеющееся оборудование
        /// </summary>
        public bool IsDeviceReplacementAvailable { get; set; }

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
                Controls,
                IsDeviceReplacementAvailable
            });
        }
    }
}
