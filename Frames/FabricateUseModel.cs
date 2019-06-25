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
    public class FabricateUseModel : ViewModelBase
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

        private int _selectedER;
        public int SelectedER
        {
            get
            {
                return _selectedER;
            }
            set
            {
                _selectedER = value;
                IsFilterChanged = true;
                RaisePropertyChanged(() => SelectedER);
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

        private ObservableCollection<ViewFabricateUse> _monthUseList;
        public ObservableCollection<ViewFabricateUse> MonthUseList
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
        private ObservableCollection<EnergyResource> _erList;
        public ObservableCollection<EnergyResource> ERList
        {
            get
            {
                return _erList;
            }
            set
            {
                _erList = value;
                RaisePropertyChanged(() => ERList);
            }
        }

        private ObservableCollection<ViewYear> _yearList;
        public ObservableCollection<ViewYear> YearList
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

        public FabricateUseModel()
        {
            Labels = new ObservableCollection<string>();
            SeriesCollection = new SeriesCollection();
            EndYearMonthFill();

            SelectedER = 958;
            YearListFill();
            MonthListFill();

            ERList = new ObservableCollection<EnergyResource>();
            ERListFill();

            MonthUseList = new ObservableCollection<ViewFabricateUse>();
            FabricateUseTotalList = new ObservableCollection<ViewFabricateUse>();
            MonthUseListFill();

            Labels = new ObservableCollection<string>();
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
            StartYear = EndMonth == 1 ? EndYear - 1 : EndYear;
            StartMonth = 1;
        }

        private void MonthListFill()
        {
            MonthList = new ObservableCollection<MonthClass>();
            MonthList.Clear();
            for (int i = 1; i <= 12; i++)
            {
                MonthList.Add(new MonthClass() { Month = i });
            }
        }
        private ObservableCollection<ViewFabricateUse> _fabricateUseTotalList;
        public ObservableCollection<ViewFabricateUse> FabricateUseTotalList
        {
            get
            {
                return _fabricateUseTotalList;
            }
            set
            {
                _fabricateUseTotalList = value;
                RaisePropertyChanged(() => FabricateUseTotalList);
            }
        }

        private void MonthUseListFill()
        {
            using (db = new MyDBContext())
            {
                db.ViewResults.Load();
                var qrySource = from o in db.ViewFabricateUses.ToList()
                                where o.Year * 100 + o.Month >= StartYear * 100 + StartMonth
                                where o.Year * 100 + o.Month <= EndYear * 100 + EndMonth
                                where o.IdProduct == SelectedER
                                group o by new { o.IdProduct, o.Period } into gr
                                orderby gr.Max(m => m.Year), gr.Max(m => m.Month)
                                select new ViewFabricateUse
                                {
                                    IdProduct = gr.Key.IdProduct,
                                    Period = gr.Key.Period,
                                    ERName = gr.Max(m => m.ERName),
                                    Year = gr.Max(m => m.Year),
                                    Month = gr.Max(m => m.Month),
                                    UnitName = gr.Max(m => m.UnitName),
                                    Fabricate = gr.Sum(m => m.Fabricate),
                                    Fact1 = gr.Sum(m => m.Fact1),
                                    Fact0 = gr.Sum(m => m.Fact0),
                                    Loss = gr.Sum(m => m.Loss)
                                };
                MonthUseList.Clear();
                MonthUseList = Global.ObservableCollection<ViewFabricateUse>(qrySource);

                FabricateUseTotalList.Clear();
                FabricateUseTotalList.Add(new ViewFabricateUse()
                {
                    Period = "ИТОГО:",
                    Fabricate = MonthUseList.Sum(n => n.Fabricate),
                    Fact1 = MonthUseList.Sum(n => n.Fact1),
                    Fact0 = MonthUseList.Sum(n => n.Fact0),
                    Loss = MonthUseList.Sum(n => n.Loss)
                });

            }
        }
        private void YearListFill()
        {
            using (db = new MyDBContext())
            {
                db.ViewYears.Load();
                YearList = Global.ObservableCollection<ViewYear>(db.ViewYears.Local.ToList());

            }
        }
        private void ERListFill()
        {
            using (db = new MyDBContext())
            {
                db.EnergyResources.Load();

                var qry2 = from o in db.EnergyResources
                           where o.IsActual==1
                           where o.IsPrime == 0
                           select new 
                           {
                               o.Id,
                               o.Name
                           };
                ERList.Clear();
                foreach (var row in qry2)
                {
                    ERList.Add(new EnergyResource()
                    {
                        Id = row.Id,
                        Name = row.Id + "_" + row.Name
                    });
                }
            }
        }


        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> Formatter { get; set; }

        public ObservableCollection<string> Labels { get; set; }
        public void DiagrammInit()
        {
            string[] SeriesTitle = new string[] { "Потребл", "Продажа", "Потери" };
            int i = 0;
            int dlina = MonthUseList.Count();
            SeriesCollection.Clear();
            StackedAreaSeries ls1 = new StackedAreaSeries
            {
                Title = SeriesTitle[i],
                LineSmoothness = 0,
                DataLabels = true,
                StrokeThickness = 2.5,
                //Stroke = System.Windows.Media.Brushes.DeepSkyBlue,
                Fill = System.Windows.Media.Brushes.AliceBlue,
                FontSize = 10,
                Foreground = System.Windows.Media.Brushes.Black,
                FontWeight = FontWeights.Bold,
                LabelPoint = point => string.Format("{0:N0}", point.Y),
                Values = new ChartValues<ObservableValue>(ValueYFill(SeriesTitle[i]))
            };
            StackedAreaSeries ls2 = new StackedAreaSeries
            {
                Title = SeriesTitle[i + 1],
                LineSmoothness = 0,
                DataLabels = true,
                StrokeThickness = 2.5,
                //Stroke = System.Windows.Media.Brushes.Red,
                Fill = System.Windows.Media.Brushes.Pink,
                FontSize = 10,
                Foreground = System.Windows.Media.Brushes.Black,
                FontWeight = FontWeights.Bold,
                LabelPoint = point => string.Format("{0:N0}", point.Y),
                Values = new ChartValues<ObservableValue>(ValueYFill(SeriesTitle[i + 1]))
            };
            StackedAreaSeries ls3 = new StackedAreaSeries
            {
                Title = SeriesTitle[i + 2],
                LineSmoothness = 0,
                DataLabels = true,
                FontSize = 10,
                StrokeThickness = 2.5,
                Foreground = System.Windows.Media.Brushes.Black,
                Stroke = System.Windows.Media.Brushes.DarkRed,
                Fill = System.Windows.Media.Brushes.Moccasin,
                FontWeight = FontWeights.Bold,
                LabelPoint = point => string.Format("{0:N0}", point.Y),
                AreaLimit = 0,
                Values = new ChartValues<ObservableValue>(ValueYFill(SeriesTitle[i + 2]))
            };
            SeriesCollection.Add(ls1);
            SeriesCollection.Add(ls2);
            SeriesCollection.Add(ls3);

            Labels.Clear();
            foreach (var newX in MonthUseList)
            {
                Labels.Add(newX.Period.ToString());
            };


            //Labels.Clear();
            //Labels = ObservableCollection<string>(MonthUseList.Select(x => x.Period));

            ChartValues<ObservableValue> ValueYFill(string typeSeries = "")
            {
                ChartValues<ObservableValue> rez = new ChartValues<ObservableValue>();
                switch (typeSeries)
                {
                    case ("Потребл"):
                        foreach (ViewFabricateUse newY in MonthUseList)
                        {
                            rez.Add(new ObservableValue(newY.Fact1));
                        }
                        break;
                    case ("Продажа"):
                        foreach (ViewFabricateUse newY in MonthUseList)
                        {
                            rez.Add(new ObservableValue(newY.Fact0));
                        }
                        break;
                    case ("Потери"):
                        foreach (ViewFabricateUse newY in MonthUseList)
                        {
                            rez.Add(new ObservableValue(newY.Loss));
                        }
                        break;
                }
                return rez;
            }

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
            var resName1 = (from o in ERList
                            where o.Id == SelectedER
                            select new { o.Name });

            string resName = resName1.ToList()[0].Name.ToString();
            //string resCostCenter = SelectedCostCenter == 0 ? "ПАО ХИМПРОМ" : "ЦЗ-" + SelectedCostCenter.ToString();
            string resCostCenter = "";
            var unitName1 = (from o in MonthUseList
                             where o.IdProduct == SelectedER
                             select new { o.UnitName });

            //string unitName = unitName1.ToList()[0].UnitName.ToString();
            string unitName = "";

            string unit = unitName;
            string date1 = string.Format("{0: 00}", StartMonth) + "." + StartYear + " г.";
            string date2 = string.Format("{0: 00}", EndMonth) + "." + EndYear + " г.";
            ChartCaption = string.Format("Распределение '{0}'{1} за период с: {2} по: {3}{4}", resName, resCostCenter, date1, date2, unit);
        }
    }
}
