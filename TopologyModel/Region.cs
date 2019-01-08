using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopologyModel
{
	public class Region
	{
		/// <summary>
		/// наличие на данном участке проводного подключения к локальной сети предприятия 
		/// </summary>
		public bool HasLan { get; set; }

		/// <summary>
		/// экспертная оценка недоступности (слабости сигнала) на данном участке беспроводного подключения к локальной сети объекта предприятия
		/// </summary>
		public bool HasWiFi { get; set; }

		/// <summary>
		/// наличие на данном участке питающей электрической сети 220 В
		/// </summary>
		public bool HasPower { get; set; }

		/// <summary>
		/// экспертная оценка доступности на данном участке различных радиоволн извне участка
		/// </summary>
		public ushort MobileInternetSignalEstimate { get; set; }    // Вес проводной связи сквозь стену


		/// <summary>
		/// экспертная оценка агрессивности окружающей среды на участке (включая его стены)
		/// </summary>
		public ushort AggressivenessEstimate { get; set; }          // Вес проводной связи внутри, на стене, сквозь стену

		/// <summary>
		/// экспертная оценка от 1 до 10 трудоемкости проведения монтажных работ внутри участка, вдоль стены участка или проведения связи (кабелей) на соседний участок через стену
		/// </summary>
		public ushort LaboriousnessEstimate { get; set; }           // Вес проводной связи внутри, на стене, сквозь стену

		/// <summary>
		/// экспертная оценка не пригодности участка (включая стены) для размещения на нём КУ, УСПД, сервера или приемопередатчиков
		/// </summary>
		public ushort UnavailabilityEstimate { get; set; }          // Вес проводной связи внутри, на стене, сквозь стену

		/// <summary>
		/// непроходимость радиоволн на самом участке (наличие металлоконструкций, электромагнитных помех, экранов)
		/// </summary>
		public ushort BadTransmittanceEstimate { get; set; }		// Вес беспроводной связи внутри и вдоль стен

		
		/// <summary>
		/// связанные регионы
		/// </summary>
		public Region[] AdjacentRegions { get; set; } = new Region[] { };

		/// <summary>
		/// Объект данных участков.
		/// </summary>
		public Facility ParentFacility { get; set; }



		/// <summary>
		/// перечень имеющихся на участке ТУУ
		/// </summary>
		public MeasurementAndControlPoint[] MCPs { get; set; } = new MeasurementAndControlPoint[] { };


		public Tools InstalledTools { get; set; }

		public double NodeWeight()
		{
			return 0;
		}

		public double ConnectionWeight(Region other)
		{
			return 0;
		}
	}
}
