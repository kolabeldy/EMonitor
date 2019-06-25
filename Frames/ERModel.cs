using GalaSoft.MvvmLight;
using System;
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
    public class ERModel : ViewModelBase
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

        //private int _selectedCostCenter;
        //public int SelectedCostCenter
        //{
        //    get
        //    {
        //        return _selectedCostCenter;
        //    }
        //    set
        //    {
        //        _selectedCostCenter = value;
        //        IsFilterChanged = true;
        //        RaisePropertyChanged(() => SelectedCostCenter);
        //    }
        //}

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
        //private void Dump()
        //{
        //    int a = 1;
        //}

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

        private bool _isChoiseResource;
        public bool IsChoiseResource
        {
            get
            {
                return _isChoiseResource;
            }
            set
            {
                _isChoiseResource = value;
                int selER = SelectedER;
                ERListFill();
                SelectedER = selER;
                IsFilterChanged = true;
                RaisePropertyChanged(() => IsChoiseResource);
            }
        }

        private ObservableCollection<ViewResult> _monthUseList;
        public ObservableCollection<ViewResult> MonthUseList
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
        private ObservableCollection<ViewResult> _monthUseTotalList;
        public ObservableCollection<ViewResult> MonthUseTotalList
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

        private ObservableCollection<CostCenter> _costCenterList;
        public ObservableCollection<CostCenter> CostCenterList
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

        private bool _rbCostIsChecked;
        public bool RBCostIsChecked
        {
            get
            {
                return _rbCostIsChecked;
            }
            set
            {
                _rbCostIsChecked = value;
                IsFilterChanged = true;
                RaisePropertyChanged(() => RBCostIsChecked);
            }
        }

        public ERModel()
        {
            Labels = new ObservableCollection<string>();
            SeriesCollection = new SeriesCollection();
            StartYear = DateTime.Now.Year;
            StartMonth = 1;
            EndYear = DateTime.Now.Year;
            EndMonth = DateTime.Now.Month - 1;
            //SelectedCostCenter = 0;
            SelectedER = 955;
            RBCostIsChecked = false;
            YearListFill();
            MonthListFill();
            CostCenterListFill();
            MonthUseListFill();
            ERListFill();
            DiagrammInit();
            ChartCaptionFill();
            IsChoiseResource = true;
            IsFilterChanged = false;
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

        private void MonthUseListFill()
        {
            using (db = new MyDBContext())
            {
                db.ViewResults.Load();
                var filterRezult = from o in db.ViewResults
                           where o.Year * 100 + o.Month >= StartYear * 100 + StartMonth
                           where o.Year * 100 + o.Month <= EndYear * 100 + EndMonth
                           where o.IdEnergyResource == SelectedER
                           //where SelectedCostCenter == 0 ? true : o.IdCostCenter == SelectedCostCenter

                           group o by new { o.IdEnergyResource, o.Period } into gr
                           orderby gr.Max(m => m.Year), gr.Max(m => m.Month)
                           select new
                           {
                               IdEnergyResource = gr.Key.IdEnergyResource,
                               Period = gr.Key.Period,
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
                               IsMain = gr.Max(m => m.IsMain)
                           };

                MonthUseList = new ObservableCollection<ViewResult>();
                MonthUseList.Clear();

                //qry3.ToList().ForEach(x => MonthUseList.Add(x));

                foreach (var row in filterRezult)
                {
                    MonthUseList.Add(new ViewResult()
                    {
                        Year = row.Year,
                        Month = row.Month,
                        Period = Global.MonthName(row.Month) + "." + row.Year,
                        IdEnergyResource = row.IdEnergyResource,
                        ResourceName = row.ResourceName,
                        UnitName = row.UnitName,
                        Plan = row.Plan,
                        Fact = row.Fact,
                        Difference = row.Difference,
                        PlanCost = row.PlanCost,
                        FactCost = row.FactCost,
                        DifferenceCost = row.DifferenceCost
                    });
                }
                MonthUseTotalList = new ObservableCollection<ViewResult>();
                MonthUseTotalList.Clear();
                MonthUseTotalList.Add(new ViewResult()
                {
                    Period = "ИТОГО:",
                    Plan = MonthUseList.Sum(n => n.Plan),
                    Fact = MonthUseList.Sum(n => n.Fact),
                    Difference = MonthUseList.Sum(n => n.Difference),
                    PlanCost = MonthUseList.Sum(n => n.PlanCost),
                    FactCost = MonthUseList.Sum(n => n.FactCost),
                    DifferenceCost = MonthUseList.Sum(n => n.DifferenceCost)
                });

            }
        }

        private void YearListFill()
        {
            using (db = new MyDBContext())
            {
                db.ViewYears.Load();

                var qry2 = from o in db.ViewYears
                           select new
                           {
                               o.Id,
                               o.Year
                           };
                YearList = new ObservableCollection<ViewYear>();
                YearList.Clear();
                foreach (var row in qry2)
                {
                    YearList.Add(new ViewYear()
                    {
                        Id = row.Id,
                        Year = row.Year
                    });
                }
            }
        }
        private void CostCenterListFill()
        {
            using (db = new MyDBContext())
            {
                db.CostCenters.Load();

                var qry2 = from o in db.CostCenters
                           select new
                           {
                               o.Id,
                               o.IdDepart
                           };
                CostCenterList = new ObservableCollection<CostCenter>();
                CostCenterList.Clear();
                CostCenterList.Add(new CostCenter()
                {
                    Id = 0,
                    IdDepart = 0
                });

                foreach (var row in qry2)
                {
                    CostCenterList.Add(new CostCenter()
                    {
                        Id = row.Id,
                        IdDepart = row.IdDepart
                    });
                }
            }
        }
        private void ERListFill()
        {
            using (db = new MyDBContext())
            {
                db.EnergyResources.Load();

                var qry2 = from o in db.EnergyResources
                           where o.IsMain >= (IsChoiseResource ? 1 : 0)
                           select new
                           {
                               o.Id,
                               o.Name
                           };
                ERList = new ObservableCollection<EnergyResource>();
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
            int dlina = MonthUseList.Count();
            string[] SeriesTitle = new string[] { "План", "Факт", "Откл", "ПланРуб", "ФактРуб", "ОтклРуб" };
            int i = !RBCostIsChecked ? 0 : 3;
            SeriesCollection.Clear();
            LineSeries ls1 = new LineSeries
            {
                Title = SeriesTitle[i],
                LineSmoothness = 0,
                StrokeThickness = 2.5,
                //Stroke = System.Windows.Media.Brushes.DeepSkyBlue,
                Fill = System.Windows.Media.Brushes.AliceBlue,
                DataLabels = dlina <= 12 ? true : false,
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                LabelPoint = point => string.Format("{0:N0}", point.Y),
                Values = new ChartValues<ObservableValue>(ValueYFill(SeriesTitle[i]))
            };
            SeriesCollection.Add(ls1);
            LineSeries ls2 = new LineSeries
            {
                Title = SeriesTitle[i + 1],
                LineSmoothness = 0,
                DataLabels = true,
                StrokeThickness = 2.5,
                //Stroke = System.Windows.Media.Brushes.Red,
                Fill = System.Windows.Media.Brushes.Transparent,
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                LabelPoint = point => string.Format("{0:N0}", point.Y),
                Values = new ChartValues<ObservableValue>(ValueYFill(SeriesTitle[i + 1]))
            };
            SeriesCollection.Add(ls2);
            LineSeries ls3 = new LineSeries
            {
                Title = SeriesTitle[i + 2],
                LineSmoothness = 0,
                DataLabels = true,
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                LabelPoint = point => string.Format("{0:N0}", point.Y),
                AreaLimit = 0,
                Values = new ChartValues<ObservableValue>(ValueYFill(SeriesTitle[i + 2]))
            };
            SeriesCollection.Add(ls3);

            Labels.Clear();
            foreach (var newX in MonthUseList)
            {
                Labels.Add(newX.Period.ToString());
            };


            //Labels.Clear();
            //Labels = new ObservableCollection<string>(MonthUseList.Select(x => x.Period));
            Formatter = val => val.ToString("N0");

            ChartValues<ObservableValue> ValueYFill(string typeSeries = "")
            {
                ChartValues<ObservableValue> rez = new ChartValues<ObservableValue>();
                switch (typeSeries)
                {
                    case ("План"):
                        foreach (ViewResult newY in MonthUseList)
                        {
                            rez.Add(new ObservableValue(newY.Plan));
                        }
                        break;
                    case ("Факт"):
                        foreach (ViewResult newY in MonthUseList)
                        {
                            rez.Add(new ObservableValue(newY.Fact));
                        }
                        break;
                    case ("Откл"):
                        foreach (ViewResult newY in MonthUseList)
                        {
                            rez.Add(new ObservableValue(newY.Difference));
                        }
                        break;
                    case ("ПланРуб"):
                        foreach (ViewResult newY in MonthUseList)
                        {
                            rez.Add(new ObservableValue(newY.PlanCost));
                        }
                        break;
                    case ("ФактРуб"):
                        foreach (ViewResult newY in MonthUseList)
                        {
                            rez.Add(new ObservableValue(newY.FactCost));
                        }
                        break;
                    case ("ОтклРуб"):
                        foreach (ViewResult newY in MonthUseList)
                        {
                            rez.Add(new ObservableValue(newY.DifferenceCost));
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
            string unit;
            var resName1 = (from o in ERList
                             where o.Id == SelectedER
                             select new { o.Name });

            string resName = resName1.ToList()[0].Name.ToString();
            string resCostCenter = "ПАО ХИМПРОМ";
            using (db = new MyDBContext())
            {

                db.EnergyResources.Load();
                db.Units.Load();
                var unitName1 = (from o in db.EnergyResources
                                 join a in db.Units on o.Unit equals a.Id
                                 where o.Id == SelectedER
                                 select new { a.Name }).ToList();

                string unitName = unitName1[0].Name.ToString();
                unit = !RBCostIsChecked ? unitName : "руб.";
            }
            string date1 = string.Format("{0: 00}", StartMonth) + "." + StartYear + " г.";
            string date2 = string.Format("{0: 00}", EndMonth) + "." + EndYear + " г.";
            ChartCaption = string.Format("Потребление '{0}' по {1} за период с: {2} по: {3}, {4}", resName, resCostCenter, date1, date2, unit);
        }

    }
}
