using System;
using System.Collections.Generic;
using System.Linq;
using TopologyModel.Enumerations;
using TopologyModel.Graphs;
using TopologyModel.Regions;
using TopologyModel.Equipments;

namespace TopologyModel
{
    /// <summary>
    /// Класс всего проекта.
    /// </summary>
    public class Project
    {
        #region ProjectProperties

        /// <summary>
        /// Цель минимизации в проекте (время, деньги или иное).
        /// </summary>
        public CostType MinimizationGoal { get; set; }

        /// <summary>
        /// Высота учитываемого размера всей территории предприятия в метрах.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// Ширина учитываемого размера всей территории предприятия в метрах.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// Бюджет всего проекта, за рамки которого затраты на реализацию проекта не должны выходить.
        /// </summary>
        public double Budget { get; set; }

        /// <summary>
        /// Планируемый период эксплуатации системы для расчёта затрат на её использование во времени.
        /// </summary>
        public uint UsageMonths { get; set; }

        /// <summary>
        /// Требуется ли использовать локальный сервер, иначе будет использоваться удалённый сервер.
        /// TODO: добавить координату сервера и его свойства, как УСПД, которые в ГА только будут выбираться, м.б. вынести в отдельный класс
        /// Сервер добавляется как отдельное УСПД, для которого создаются секции для всех КУ. Уаждому УСПД можно задать координаты и тогда это будет жестко сервер
        /// </summary>
        public bool UseLocalServer { get; set; }

        /// <summary>
        /// Допустимо ли проведение монтажных работ его сотрудниками.
        /// </summary>
        public bool UseLocalEmployee { get; set; }

        /// <summary>
        /// будет ли доступен выход в сеть Интернет из локальной сети предприятия
        /// </summary>
        public bool IsInternetAvailable { get; set; }

        /// <summary>
        /// Абонентская плата в месяц за использование и обслуживание локального сервера.
        /// </summary>
        public double LocalServerMonthlyPayment { get; set; }

        /// <summary>
        /// Абонентская плата в месяц за использование и обслуживание предоставляемого удалённого сервера.
        /// </summary>
        public double RemoteServerMonthlyPayment { get; set; }

        /// <summary>
        /// Словарь весовых коэффициентов параметров проекта, где ключ - это наименование параметра, а значение - вес.
        /// </summary>
        public Dictionary<string, double> WeightCoefficients { get; set; }

        /// <summary>
        /// Множество стоимостей в месяц абонентского обслуживания технологий мобильной передачи данных на сервер.
        /// Где ключ - это тип соединения, а значение - это стоимость.
        /// </summary>
        public Dictionary<InternetConnection, double> MobileInternetMonthlyPayment { get; set; }

        /// <summary>
        /// Имя dot-файла графа участков предприятия.
        /// </summary>
        public string GraphDotFilename { get; set; } = "unnamed.dot";

        /// <summary>
        /// Условная стоимость часа выполнения работ.
        /// </summary>
        public double CostPerHour { get; set; }

        /// <summary>
        /// Длина грани на графе в метрах.
        /// </summary>
        public double EdgeDistance { get; set; }

        #endregion

        /// <summary>
        /// Массив всех участков предприятия.
        /// </summary>
        public FacilityRegion[] Regions { get; set; }

        /// <summary>
        /// Перечень имеющихся на предприятии ТУУ
        /// </summary>
        public MeasurementAndControlZone[] MCZs { get; set; }

        /// <summary>
        /// База данных доступного инструментария.
        /// </summary>
        public EquipmentDB Equipments { get; set; }

        /// <summary>
        /// Граф всего предприятия.
        /// </summary>
        public TopologyGraph Graph { get; set; }

        /// <summary>
        /// Инициализировать граф всего предприятия.
        /// </summary>
        /// <returns>True, если операция выполнена успешно.</returns>
        public bool InitializeGraph()
        {
            try
            {
                var verticesMatrix = CreateVerticesMatrix();

                if (verticesMatrix == null || verticesMatrix.Length == 0) return false;

                Graph = new TopologyGraph(verticesMatrix, EdgeDistance, WeightCoefficients);

                Console.WriteLine("Initialize the graph... Done!");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("InitializeGraph failed! {0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Создать матрицу вершин участков для будущего графа.
        /// </summary>
        /// <returns>Матрица вершин участков бдущего графа.</returns>
        protected TopologyVertex[,] CreateVerticesMatrix()
        {
            Console.Write("Create regions matrix... ");

            try
            {
                var verticesMatrix = new TopologyVertex[Height, Width];

                foreach (var region in Regions)         // Перебираем все имеющиеся регионы
                {
                    // Определяем начальные и конечные координаты участков в матрице
                    var startX = region.X - 1;          // В конфигурационном файле координаты начинаются с 1
                    var startY = region.Y - 1;
                    var endX = startX + region.Width;
                    var endY = startY + region.Height;

                    for (var i = startY; i < endY; i++)         // Наполняем матрицу идентификаторами участков по координатам
                        for (var j = startX; j < endX; j++)
                            // Создаём вершину, привязанную к учаску и задаём её координаты внутри самого участка
                            // +1 из-за того, что в конфигурационном файле координаты начинаются с 1
                            verticesMatrix[i, j] = new TopologyVertex(region, j - region.X + 1, i - region.Y + 1, GetMCZsInVertex(j + 1, i + 1));
                }

                Console.WriteLine("Done! Result matix: ");

                for (var i = 0; i < Height; i++)                // Выводим матрицу идентификаторов регионов в вершинах для наглядности
                {
                    for (var j = 0; j < Width; j++)
                        // Делаем отступ исходя из строкового представления вершины и максимально трёхзначного идентификатора участка
                        Console.Write("{0,8}", verticesMatrix[i, j].ToString());

                    Console.WriteLine();
                }

                return verticesMatrix;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateVerticesMatrix failed! {0}", ex.Message);
                return new TopologyVertex[,] { };
            }
        }

        /// <summary>
        /// Получить массив всех мест учёта и управления в текущем узле графа по его координатам.
        /// </summary>
        /// <param name="x">Координата Х узла графа.</param>
        /// <param name="y">Координата У узла графа.</param>
        /// <returns>Массив мест учёта и управления распологающийся в указанных координатах или null, если там такого нет.</returns>
        protected MeasurementAndControlZone[] GetMCZsInVertex(uint x, uint y)
        {
            try
            {
                if (MCZs == null || !MCZs.Any()) return null;

                // Если координата узла находится в координатах ТУУ, то эта ТУУ в данном узле
                return MCZs.Where(q => x >= q.X && y >= q.Y && x <= q.X + q.Width - 1 && y <= q.Y + q.Height - 1).ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetMCZsInVertex failed! {0}", ex.Message);
                return null;
            }
        }
    }
}