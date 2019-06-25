using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MonitorModel : ViewModelBase
    {
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

        private int _proc;
        public int Proc
        {
            get
            {
                return _proc;
            }
            set
            {
                _proc = value;
                IsFilterChanged = true;
                RaisePropertyChanged(() => Proc);
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
        public List<int> ProcList { get; set; }

        private ObservableCollection<ViewResult> _monthUseERList;
        public ObservableCollection<ViewResult> MonthUseERList
        {
            get
            {
                return _monthUseERList;
            }
            set
            {
                _monthUseERList = value;
                RaisePropertyChanged(() => MonthUseERList);
            }
        }

        private ObservableCollection<ViewResult> _monthUseCCList;
        public ObservableCollection<ViewResult> MonthUseCCList
        {
            get
            {
                return _monthUseCCList;
            }
            set
            {
                _monthUseCCList = value;
                RaisePropertyChanged(() => MonthUseCCList);
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
        private List<MonthClass> _monthList;
        public List<MonthClass> MonthList
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
        public MonitorModel()
        {
            ProcList = new List<int> { 0, 1, 2, 3, 5, 8, 10 };
            Proc = 5;

            SeriesCollection1 = new SeriesCollection();
            SeriesCollection2 = new SeriesCollection();
            LabelsER = new ObservableCollection<string>();
            SeriesCollection3 = new SeriesCollection();
            SeriesCollection4 = new SeriesCollection();
            LabelsCC = new ObservableCollection<string>();

            EndYearMonthFill();

            YearListFill();
            MonthListFill();

            MonthUseERList = new ObservableCollection<ViewResult>();
            MonthUseERListFill();

            MonthUseCCList = new ObservableCollection<ViewResult>();
            MonthUseTotalCCList = new List<ViewResult>();
            MonthUseCCListFill();

            DiagrammERInit();
            DiagrammCCInit();

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
            MonthList = new List<MonthClass>();
            MonthList.Clear();
            for (int i = 1; i <= 12; i++)
            {
                MonthList.Add(new MonthClass() { Month = i });
            }
        }
        public List<ViewResult> MonthUseTotalCCList { get; set; }
        private double _totalDifferenceCost;
        public double TotalDifferenceCost
        {
            get
            {
                return _totalDifferenceCost;
            }
            set
            {
                _totalDifferenceCost = value;
                RaisePropertyChanged(() => TotalDifferenceCost);
            }
        }

        private void MonthUseERListFill()
        {
            using (db = new MyDBContext())
            {
                db.ViewResults.Load();
                var qrySource = from o in db.ViewResults.ToList()
                                where o.Year * 100 + o.Month >= StartYear * 100 + StartMonth
                                where o.Year * 100 + o.Month <= EndYear * 100 + EndMonth
                                group o by new { o.IdEnergyResource } into gr
                                orderby gr.Sum(m => m.FactCost) descending
                                select new ViewResult
                                {
                                    IdEnergyResource = gr.Key.IdEnergyResource,
                                    ResourceName = gr.Max(m => m.ResourceName),
                                    FactCost = gr.Sum(m => m.FactCost),
                                    DifferenceCost = gr.Sum(m => m.DifferenceCost)
                                };
                MonthUseERList.Clear();
                MonthUseERList = Global.ObservableCollection<ViewResult>(qrySource);
            }
        }

        private void MonthUseCCListFill()
        {
            using (db = new MyDBContext())
            {
                db.ViewResults.Load();
                var qrySource = from o in db.ViewResults.ToList()
                                where o.Year * 100 + o.Month >= StartYear * 100 + StartMonth
                                where o.Year * 100 + o.Month <= EndYear * 100 + EndMonth
                                group o by new { o.IdCostCenter } into gr
                                orderby gr.Sum(m => m.IdCostCenter)
                                select new ViewResult
                                {
                                    IdCostCenter = gr.Key.IdCostCenter,
                                    FactCost = gr.Sum(m => m.FactCost),
                                    DifferenceCost = gr.Sum(m => m.DifferenceCost)
                                };
                MonthUseCCList.Clear();
                MonthUseCCList = Global.ObservableCollection<ViewResult>(qrySource);
            }
            MonthUseTotalCCList.Clear();
            MonthUseTotalCCList.Add(new ViewResult()
            {
                DifferenceCost = MonthUseCCList.Sum(n => n.DifferenceCost)
            });

            TotalDifferenceCost = MonthUseTotalCCList[0].DifferenceCost;
        }

        private void YearListFill()
        {
            using (db = new MyDBContext())
            {
                db.ViewYears.Load();
                YearList = db.ViewYears.Local.ToList();
            }
        }
        public SeriesCollection SeriesCollection1 { get; set; }
        public SeriesCollection SeriesCollection2 { get; set; }
        public Func<double, string> FormatterER { get; set; }

        public ObservableCollection<string> LabelsER { get; set; }
        public void DiagrammERInit()
        {
            var qry2 = from o in MonthUseERList
                        select new
                        {
                            o.FactCost,
                            o.DifferenceCost,
                            o.ResourceName
                        };
            double Total = qry2.Sum(n => n.FactCost);
            double TotalDiff = qry2.Sum(n => Math.Abs(n.DifferenceCost));

                var qry3 = from o in MonthUseERList
                           where o.FactCost>=Total * Proc / 100
                           orderby o.FactCost descending
                           select new
                           {
                               o.FactCost,
                               o.ResourceName
                           };
                var qry4 = from o in MonthUseERList
                           where o.FactCost < Total * Proc / 100
                           select new
                           {
                               o.FactCost,
                               o.ResourceName
                           };
                double Total1 = qry4.Sum(n => n.FactCost);

                var qry5 = from o in MonthUseERList
                           where Math.Abs(o.DifferenceCost) >= TotalDiff * Proc / 100
                           orderby o.DifferenceCost descending
                           select new
                           {
                               o.DifferenceCost,
                               o.ResourceName
                           };
                var qry6 = from o in MonthUseERList
                           where Math.Abs(o.DifferenceCost) < TotalDiff * Proc / 100
                           //orderby o.DifferenceCost descending
                           select new
                           {
                               o.DifferenceCost,
                               o.ResourceName
                           };
                double Total2 = qry6.Sum(n => n.DifferenceCost);

                SeriesCollection1.Clear();
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
                    SeriesCollection1.Add(ps10);

                }
                PieSeries ps2 = new PieSeries
                {
                    Title = "Прочие",
                    DataLabels = true,
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    LabelPoint = point => string.Format("{0},\r\n {1:N0}", "Прочие", point.Y),
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Total1) }
                };
                SeriesCollection1.Add(ps2);

                SeriesCollection2.Clear();
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
            ps1.Values.Add(new ObservableValue(Total2));
            SeriesCollection2.Add(ps1);

            LabelsER.Clear();
            foreach (var newX in qry5.ToList())
            {
                LabelsER.Add(newX.ResourceName.ToString());
            };
            LabelsER.Add("Прочие");

            //Labels = new ObservableCollection<string>(MonthUseList.Select(x => x.ResourceName));
            FormatterER = val => val.ToString("N0");

        }

        public Func<double, string> FormatterCC { get; set; }
        public SeriesCollection SeriesCollection3 { get; set; }
        public SeriesCollection SeriesCollection4 { get; set; }
        public ObservableCollection<string> LabelsCC { get; set; }
        public void DiagrammCCInit()
        {
            var qry2 = from o in MonthUseCCList
                       select new
                       {
                           o.FactCost,
                           o.DifferenceCost,
                           o.IdCostCenter
                       };
            double Total = qry2.Sum(n => n.FactCost);
            double TotalDiff = qry2.Sum(n => Math.Abs(n.DifferenceCost));

            var qry3 = from o in MonthUseCCList
                       where o.FactCost >= Total * Proc / 100
                       orderby o.FactCost descending
                       select new
                       {
                           o.FactCost,
                           o.IdCostCenter
                       };
            var qry4 = from o in MonthUseCCList
                       where o.FactCost < Total * Proc / 100
                       select new
                       {
                           o.FactCost,
                           o.IdCostCenter
                       };
            double Total1 = qry4.Sum(n => n.FactCost);
            SeriesCollection3.Clear();
            foreach (var newY in qry3.ToList())
            {
                PieSeries ps10 = new PieSeries
                {
                    Title = newY.IdCostCenter.ToString(),
                    DataLabels = true,
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    LabelPoint = point => string.Format("ЦЗ-{0},\r\n {1:N0}", newY.IdCostCenter, point.Y),
                    Values = new ChartValues<ObservableValue> { new ObservableValue(newY.FactCost) }
                };
                SeriesCollection3.Add(ps10);

            }
            PieSeries ps2 = new PieSeries
            {
                Title = "Прочие",
                DataLabels = true,
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                LabelPoint = point => string.Format("{0},\r\n {1:N0}", "Прочие", point.Y),
                Values = new ChartValues<ObservableValue> { new ObservableValue(Total1) }
            };
            SeriesCollection3.Add(ps2);

            var qry5 = from o in MonthUseCCList
                       where Math.Abs(o.DifferenceCost) >= TotalDiff * Proc / 100
                       orderby o.DifferenceCost descending
                       select new
                       {
                           o.DifferenceCost,
                           o.IdCostCenter
                       };
            var qry6 = from o in MonthUseCCList
                       where Math.Abs(o.DifferenceCost) < TotalDiff * Proc / 100
                       select new
                       {
                           o.DifferenceCost,
                           o.IdCostCenter
                       };
            double Total2 = qry6.Sum(n => n.DifferenceCost);

            SeriesCollection4.Clear();
            ColumnSeries ps1 = new ColumnSeries()
            {
                //LabelsPosition = BarLabelPosition.Parallel,
                //Fill = System.Windows.Media.Brushes.PowderBlue,
                DataLabels = true,
                LabelPoint = point => string.Format("{0:N0}", point.Y),
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Values = new ChartValues<ObservableValue>()
            };

            foreach (var newY in qry5.ToList())
            {
                ps1.Values.Add(new ObservableValue(newY.DifferenceCost));
            }
            ps1.Values.Add(new ObservableValue(Total2));
            SeriesCollection4.Add(ps1);

            LabelsCC.Clear();
            foreach (var newX in qry5.ToList())
            {
                LabelsCC.Add(newX.IdCostCenter.ToString());
            };
            LabelsCC.Add("Прочие");
        }

        public ICommand FilterOnCommand { get { return new RelayCommand<int>(OnFilterOn); } }
        private void OnFilterOn(int numToEdit = 0)
        {
            MonthUseERListFill();
            DiagrammERInit();
            MonthUseCCListFill();
            DiagrammCCInit();
            ChartCaptionFill();
            IsFilterChanged = false;
        }
        private void ChartCaptionFill()
        {
            string date1 = string.Format("{0: 00}", StartMonth) + "." + StartYear + " г.";
            string date2 = string.Format("{0: 00}", EndMonth) + "." + EndYear + " г.";
            string period;
            if(StartYear == EndYear && StartMonth == EndMonth)
            {
                period = string.Format("за {0}", date1);
            }
            else
            {
                period = string.Format("за период с: {0} по: {1}", date1, date2);
            }
            ChartCaption = string.Format("Фактическое потребление энергоресурсов ПАО ХИМПРОМ {0}, руб. (порог для Прочих - {1}%)", period, Proc);
        }
        public ICommand ChartERViewCommand { get { return new RelayCommand<int>(OnChartERView); } }
        private void OnChartERView(int numToEdit = 0)
        {
            MonitorChartER ChartERWindow = new MonitorChartER(MonthUseERList);
            ChartERWindow.ShowDialog();
        }
        public ICommand ChartCCViewCommand { get { return new RelayCommand<int>(OnChartCCView); } }
        private void OnChartCCView(int numToEdit = 0)
        {
            MonitorChartCC ChartCCWindow = new MonitorChartCC(MonthUseCCList);
            ChartCCWindow.ShowDialog();
        }

    }

}
