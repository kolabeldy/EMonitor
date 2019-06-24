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
using System.Collections.Generic;

namespace EMonitor.Frames
{
    public class LossesModel : ViewModelBase
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

        private int _selectedCostCenter;
        public int SelectedCostCenter
        {
            get
            {
                return _selectedCostCenter;
            }
            set
            {
                _selectedCostCenter = value;
                IsFilterChanged = true;
                RaisePropertyChanged(() => SelectedCostCenter);
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
                //if(value == 990) { Dump(); }
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

        private ObservableCollection<ViewLossesFact> _monthUseList;
        public ObservableCollection<ViewLossesFact> MonthUseList
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
        private ObservableCollection<ViewLossesFact> _monthUseTotalList;
        public ObservableCollection<ViewLossesFact> MonthUseTotalList
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

        private ObservableCollection<ViewLossesFact> _erList;
        public ObservableCollection<ViewLossesFact> ERList
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

        public LossesModel()
        {
            Labels = new ObservableCollection<string>();
            SeriesCollection = new SeriesCollection();
            StartYear = DateTime.Now.Year;
            StartMonth = 1;
            EndYear = DateTime.Now.Year;
            EndMonth = DateTime.Now.Month - 1;
            SelectedCostCenter = 0;
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
                db.ViewLossesFacts.Load();
                db.ViewERPrices.Load();
                List<ViewERPrice> vp = db.ViewERPrices.Local.ToList();
                var filterRezult = from o in db.ViewLossesFacts.Local.ToList()
                                   join a in vp on o.IdEnergyResource equals a.Id
                                   where o.Year * 100 + o.Month >= StartYear * 100 + StartMonth
                                   where o.Year * 100 + o.Month <= EndYear * 100 + EndMonth
                                   where o.IdEnergyResource == SelectedER

                           select new
                           {
                               IdEnergyResource = o.IdEnergyResource,
                               Period = o.Period,
                               ERName = o.ERName,
                               Year = o.Year,
                               Month = o.Month,
                               UnitName = o.UnitName,
                               FactLoss = o.FactLoss,
                               FactLossCost = o.Month >= 4 && o.Month <10 ? o.FactLoss * a.CostSummer : o.FactLoss * a.CostWinter,
                               FactUse = o.FactUse,
                               FactTotal = o.FactTotal,
                               FactTotalCost = o.Month >= 4 && o.Month < 10 ? o.FactTotal * a.CostSummer : o.FactTotal * a.CostWinter,
                               Tariff = o.Month >= 4 && o.Month < 10 ? a.CostSummer : a.CostWinter,
                               ProcLoss = o.ProcLoss
                           };

                MonthUseList = new ObservableCollection<ViewLossesFact>();
                MonthUseList.Clear();

                foreach (var row in filterRezult)
                {
                    MonthUseList.Add(new ViewLossesFact()
                    {
                        IdEnergyResource = row.IdEnergyResource,
                        Period = row.Period,
                        ERName = row.ERName,
                        Year = row.Year,
                        Month = row.Month,
                        UnitName = row.UnitName,
                        FactLoss = row.FactLoss,
                        FactLossCost = row.FactLossCost,
                        FactUse = row.FactUse,
                        FactTotal = row.FactTotal,
                        FactTotalCost = row.FactTotalCost,
                        Tariff = row.Tariff,
                        ProcLoss = row.ProcLoss
                    });
                }
                double factTotal = MonthUseList.Sum(n => n.FactTotal);
                double factLoss = MonthUseList.Sum(n => n.FactLoss);
                MonthUseTotalList = new ObservableCollection<ViewLossesFact>();
                MonthUseTotalList.Clear();
                MonthUseTotalList.Add(new ViewLossesFact()
                {
                    Period = "ИТОГО:",
                    FactTotal = factTotal,
                    FactLoss = factLoss,
                    FactTotalCost = MonthUseList.Sum(n => n.FactTotalCost),
                    FactLossCost = MonthUseList.Sum(n => n.FactLossCost),
                    ProcLoss = factLoss / factTotal *100
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
                db.ViewLossesFacts.Load();

                var qry2 = from o in db.ViewLossesFacts
                           group o by new { o.IdEnergyResource } into gr
                           select new
                           {
                               IdEnergyResource = gr.Key.IdEnergyResource,
                               ERName = gr.Max(m => m.ERName)
                           };

                ERList = new ObservableCollection<ViewLossesFact>();
                ERList.Clear();
                foreach (var row in qry2)
                {
                    ERList.Add(new ViewLossesFact()
                    {
                        IdEnergyResource = row.IdEnergyResource,
                        ERName = row.IdEnergyResource + "_" + row.ERName
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
            string[] SeriesTitle = new string[] { "Всего", "Потери", "ВсегоРуб", "ПотериРуб" };
            int i = !RBCostIsChecked ? 0 : 2;
            SeriesCollection.Clear();
            LineSeries ls1 = new LineSeries
            {
                Title = SeriesTitle[i],
                LineSmoothness = 0,
                StrokeThickness = 2.5,
                Fill = System.Windows.Media.Brushes.AliceBlue,
                DataLabels = true,
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
                Fill = System.Windows.Media.Brushes.Transparent,
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                LabelPoint = point => string.Format("{0:N0}", point.Y),
                Values = new ChartValues<ObservableValue>(ValueYFill(SeriesTitle[i + 1]))
            };
            SeriesCollection.Add(ls2);

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
                    case ("Всего"):
                        foreach (ViewLossesFact newY in MonthUseList)
                        {
                            rez.Add(new ObservableValue(newY.FactTotal));
                        }
                        break;
                    case ("Потери"):
                        foreach (ViewLossesFact newY in MonthUseList)
                        {
                            rez.Add(new ObservableValue(newY.FactLoss));
                        }
                        break;
                    case ("ВсегоРуб"):
                        foreach (ViewLossesFact newY in MonthUseList)
                        {
                            rez.Add(new ObservableValue(newY.FactTotalCost));
                        }
                        break;
                    case ("ПотериРуб"):
                        foreach (ViewLossesFact newY in MonthUseList)
                        {
                            rez.Add(new ObservableValue(newY.FactLossCost));
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
                             where o.IdEnergyResource == SelectedER
                             select new { o.ERName });

            string resName = resName1.ToList()[0].ERName.ToString();
            string resCostCenter = SelectedCostCenter == 0 ? "ПАО ХИМПРОМ" : "ЦЗ-" + SelectedCostCenter.ToString();
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
            ChartCaption = string.Format("Потери '{0}' по {1} за период с: {2} по: {3}, {4}", resName, resCostCenter, date1, date2, unit);
        }

    }
}
