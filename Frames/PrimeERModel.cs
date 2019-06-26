using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using EMonitor.DB;
using System.Collections.ObjectModel;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using LiveCharts.Defaults;

namespace EMonitor.Frames
{
    public class PrimeERModel : ViewModelBase
    {
        public class PrimeERFull
        {
            public int Id { get; set; }
            public string Period { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public int IdEnergyResource { get; set; }
            public string ResourceName { get; set; }
            public string UnitName { get; set; }
            public double Plan { get; set; }
            public double Fact { get; set; }
            public double Difference { get; set; }
            public double Loss { get; set; }
            public double Total { get; set; }
            public double PlanCost { get; set; }
            public double FactCost { get; set; }
            public double DifferenceCost { get; set; }
            public double LossCost { get; set; }
            public double TotalCost { get; set; }
        }

        private MyDBContext db;
        public class MonthClass
        {
            public int Month { get; set; }
        }
        private int _startYear;
        public int StartYear
        {
            get
            {
                return _startYear;
            }
            set
            {
                _startYear = value;
                IsFilterChanged = true;
                RaisePropertyChanged(() => StartYear);
            }
        }

        private int _startMonth;
        public int StartMonth
        {
            get
            {
                return _startMonth;
            }
            set
            {
                _startMonth = value;
                IsFilterChanged = true;
                RaisePropertyChanged(() => StartMonth);
            }
        }

        private int _endYear;
        public int EndYear
        {
            get
            {
                return _endYear;
            }
            set
            {
                _endYear = value;
                IsFilterChanged = true;
                RaisePropertyChanged(() => EndYear);
            }
        }
        private int _endMonth;
        public int EndMonth
        {
            get
            {
                return _endMonth;
            }
            set
            {
                _endMonth = value;
                IsFilterChanged = true;
                RaisePropertyChanged(() => EndMonth);
            }
        }
        public int SelectedCostCenter { get; set; }
        private string _chartCaption;
        public string ChartCaption
        {
            get
            {
                return _chartCaption;
            }
            set
            {
                _chartCaption = value;
                IsFilterChanged = true;
                RaisePropertyChanged(() => ChartCaption);
            }
        }

        private bool _isFilterChanged;
        public bool IsFilterChanged
        {
            get
            {
                return _isFilterChanged;
            }
            set
            {
                _isFilterChanged = value;
                RaisePropertyChanged(() => IsFilterChanged);
            }
        }


        private ObservableCollection<PrimeERFull> _monthUseList;
        public ObservableCollection<PrimeERFull> MonthUseList
        {
            get
            {
                return _monthUseList;
            }
            set
            {
                _monthUseList = value;
                RaisePropertyChanged(() => MonthUseList);
            }
        }


        private List<CostCenter> _costCenterList;
        public List<CostCenter> CostCenterList
        {
            get
            {
                return _costCenterList;
            }
            set
            {
                _costCenterList = value;
                RaisePropertyChanged(() => CostCenterList);
            }
        }

        private List<ViewYear> _yearList;
        public List<ViewYear> YearList
        {
            get
            {
                return _yearList;
            }
            set
            {
                _yearList = value;
                RaisePropertyChanged(() => YearList);
            }
        }
        private ObservableCollection<MonthClass> _monthList;
        public ObservableCollection<MonthClass> MonthList
        {
            get
            {
                return _monthList;
            }
            set
            {
                _monthList = value;
                RaisePropertyChanged(() => MonthList);
            }
        }

        public PrimeERModel()
        {
            SeriesCollection = new SeriesCollection();
            SeriesCollection1 = new SeriesCollection();
            Labels = new ObservableCollection<string>();

            EndYearMonthFill();

            SelectedCostCenter = 0;

            YearList = new List<ViewYear>();
            YearListFill();

            MonthList = new ObservableCollection<MonthClass>();
            MonthListFill();

            CostCenterList = new List<CostCenter>();
            CostCenterListFill();

            MonthUseList = new ObservableCollection<PrimeERFull>();
            MonthUseTotalList = new ObservableCollection<PrimeERFull>();
            MonthUseListFill();

            DiagrammInit();
            ChartCaptionFill();
            IsFilterChanged = false;
        }

        private void EndYearMonthFill()
        {
            using (db = new MyDBContext())
            {
                db.ViewResults.Load();
                var qry = (from o in db.ViewResults
                           orderby o.Year descending, o.Month descending
                           select new { o.Year, o.Month }).Take(1);
                EndYear = qry.ToList()[0].Year;
                EndMonth = qry.ToList()[0].Month;
            }
            StartYear = EndYear;
            StartMonth = EndMonth;
        }

        private void MonthListFill()
        {
            MonthList.Clear();
            for (int i = 1; i <= 12; i++)
            {
                MonthList.Add(new MonthClass() { Month = i });
            }
        }

        private ObservableCollection<PrimeERFull> _monthUseTotalList;
        public ObservableCollection<PrimeERFull> MonthUseTotalList
        {
            get
            {
                return _monthUseTotalList;
            }
            set
            {
                _monthUseTotalList = value;
                RaisePropertyChanged(() => MonthUseTotalList);
            }
        }

        private void MonthUseListFill()
        {
            using (db = new MyDBContext())
            {
                db.ViewResults.Load();
                db.EnergyResources.Load();
                db.ViewLossesFacts.Load();

                var qrySource = from o in db.ViewResults.Local.ToList()
                                where o.Year * 100 + o.Month >= StartYear * 100 + StartMonth
                                where o.Year * 100 + o.Month <= EndYear * 100 + EndMonth
                                join a in db.EnergyResources.ToList() on o.IdEnergyResource equals a.Id
                                where a.IsPrime == 1
                                group o by new { o.IdEnergyResource } into gr
                                //orderby gr.Sum(m => m.FactCost) descending
                                select new 
                                {
                                    IdEnergyResource = gr.Key.IdEnergyResource,
                                    ResourceName = gr.Max(m => m.ResourceName),
                                    Year = gr.Max(m => m.Year),
                                    Month = gr.Max(m => m.Month),
                                    UnitName = gr.Max(m => m.UnitName),
                                    Plan = gr.Sum(m => m.Plan),
                                    Fact = gr.Sum(m => m.Fact),
                                    Difference = gr.Sum(m => m.Difference),
                                    PlanCost = gr.Sum(m => m.PlanCost),
                                    FactCost = gr.Sum(m => m.FactCost),
                                    DifferenceCost = gr.Sum(m => m.DifferenceCost),
                                };
                var qry1 = from o in db.ViewLossesFacts
                           where o.Year * 100 + o.Month >= StartYear * 100 + StartMonth
                           where o.Year * 100 + o.Month <= EndYear * 100 + EndMonth
                           group o by new { o.IdEnergyResource } into gr
                           select new
                          {
                               IdEnergyResource = gr.Key.IdEnergyResource,
                               Year = gr.Max(m => m.Year),
                               Month = gr.Max(m => m.Month),
                               Loss = gr.Sum(m => m.FactLoss),
                               Total = gr.Sum(m => m.FactTotal),
                           };


                var qry = from o in qrySource.ToList()
                          join b in qry1.ToList() on new { o.IdEnergyResource, o.Year, o.Month } equals new { b.IdEnergyResource, b.Year, b.Month }
                          select new PrimeERFull
                          {
                              IdEnergyResource = o.IdEnergyResource,
                              ResourceName = o.ResourceName,
                              Year = o.Year,
                              Month = o.Month,
                              UnitName = o.UnitName,
                              Plan = o.Plan,
                              Fact = o.Fact,
                              Difference = o.Difference,
                              PlanCost = o.PlanCost,
                              FactCost = o.FactCost,
                              DifferenceCost = o.DifferenceCost,
                              Loss = b.Loss,
                              Total = b.Total,
                              LossCost = b.Loss * o.FactCost / o.Fact,
                              TotalCost = b.Total * o.FactCost / o.Fact
                          };

                MonthUseList.Clear();
                MonthUseList = Global.ObservableCollection<PrimeERFull>(qry);


                MonthUseTotalList.Clear();
                MonthUseTotalList.Add(new PrimeERFull()
                {
                    Period = "ИТОГО:",
                    PlanCost = MonthUseList.Sum(n => n.PlanCost),
                    FactCost = MonthUseList.Sum(n => n.FactCost),
                    DifferenceCost = MonthUseList.Sum(n => n.DifferenceCost),
                    TotalCost = MonthUseList.Sum(n => n.TotalCost),
                    LossCost = MonthUseList.Sum(n => n.LossCost)
                });

            }
        }
        private void YearListFill()
        {
            using (db = new MyDBContext())
            {
                db.ViewYears.Load();
                YearList = db.ViewYears.Local.ToList();
            }
        }
        private void CostCenterListFill()
        {
            using (db = new MyDBContext())
            {
                db.CostCenters.Load();
                CostCenterList = db.CostCenters.Local.ToList();
            }
        }

        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection SeriesCollection1 { get; set; }

        public Func<double, string> Formatter { get; set; }

        public ObservableCollection<string> Labels { get; set; }
        public void DiagrammInit()
        {
            var qry2 = from o in MonthUseList
                        select new
                        {
                            o.FactCost,
                            o.DifferenceCost,
                            o.ResourceName
                        };
            double Total = qry2.Sum(n => n.FactCost);
            double TotalDiff = qry2.Sum(n => Math.Abs(n.DifferenceCost));

                var qry3 = from o in MonthUseList
                           orderby o.FactCost descending
                           select new
                           {
                               o.FactCost,
                               o.ResourceName
                           };

                var qry5 = from o in MonthUseList
                           orderby o.DifferenceCost descending
                           select new
                           {
                               o.DifferenceCost,
                               o.ResourceName
                           };

                SeriesCollection.Clear();
                foreach (var newY in qry3.ToList())
                {
                    PieSeries ps10 = new PieSeries
                    {
                        Title = newY.ResourceName,
                        DataLabels = true,
                        FontSize = 10,
                        FontWeight = FontWeights.Bold,
                        LabelPoint = point => string.Format("{0},\r\n {1:N0}", newY.ResourceName, point.Y),
                        Values = new ChartValues<ObservableValue> { new ObservableValue(newY.FactCost) }
                    };
                    SeriesCollection.Add(ps10);

                }

                SeriesCollection1.Clear();
            ColumnSeries ps1 = new ColumnSeries()
            {
                //LabelsPosition = BarLabelPosition.Parallel,
                //Fill = System.Windows.Media.Brushes.PowderBlue,
                DataLabels = true,
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Values = new ChartValues<ObservableValue>()
            };

            foreach (var newY in qry5.ToList())
            {
                ps1.Values.Add(new ObservableValue(newY.DifferenceCost));
            }
            SeriesCollection1.Add(ps1);

            Labels.Clear();
            foreach (var newX in qry5.ToList())
            {
                Labels.Add(newX.ResourceName.ToString());
            };

            //Labels = new ObservableCollection<string>(MonthUseList.Select(x => x.ResourceName));
            Formatter = val => val.ToString("N0");

        }
        public ICommand FilterOnCommand { get { return new RelayCommand<int>(OnFilterOn); } }
        private void OnFilterOn(int numToEdit = 0)
        {
            MonthUseListFill();
            DiagrammInit();
            ChartCaptionFill();
            IsFilterChanged = false;
        }
    private void ChartCaptionFill()
        {
            string resCostCenter = "ПАО ХИМПРОМ";
            string unit = ", в рублях";
            string date1 = string.Format("{0: 00}", StartMonth) + "." + StartYear + " г.";
            string date2 = string.Format("{0: 00}", EndMonth) + "." + EndYear + " г.";
            string period;
            if (StartYear == EndYear && StartMonth == EndMonth)
            {
                period = string.Format("за {0}", date1);
            }
            else
            {
                period = string.Format("за период с: {0} по: {1}", date1, date2);
            }
            ChartCaption = string.Format("Фактическое потребление первичных энергоресурсов {0} {1} {2})", 
                resCostCenter, period, unit);
        }

    }
}
