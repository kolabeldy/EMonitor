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
    public class LossesStructModel : ViewModelBase
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

        public LossesStructModel()
        {
            //Labels = new ObservableCollection<string>();
            SeriesCollection = new SeriesCollection();
            StartYear = DateTime.Now.Year;
            StartMonth = 1;
            EndYear = DateTime.Now.Year;
            EndMonth = DateTime.Now.Month - 1;
            YearListFill();
            MonthListFill();
            //CostCenterListFill();
            MonthUseListFill();
            //ERListFill();
            DiagrammInit();
            ChartCaptionFill();
            //IsChoiseResource = true;
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
                           select new
                           {
                               IdEnergyResource = o.IdEnergyResource,
                               ERName = o.ERName,
                               FactTotal = o.FactTotal,
                               FactLoss = o.FactLoss,
                               FactTotalCost = o.Month >= 4 && o.Month < 10 ? o.FactTotal * a.CostSummer : o.FactTotal * a.CostWinter,
                               FactLossCost = o.Month >= 4 && o.Month <10 ? o.FactLoss * a.CostSummer : o.FactLoss * a.CostWinter,
                           };
                var filterRezult1 = from o in filterRezult
                                    group o by new { o.IdEnergyResource } into gr
                                    orderby gr.Key.IdEnergyResource
                                    select new
                                    {
                                        IdEnergyResource = gr.Key.IdEnergyResource,
                                        ERName = gr.Max(m => m.ERName),
                                        FactTotal = gr.Sum(m => m.FactTotal),
                                        FactLoss = gr.Sum(m => m.FactLoss),
                                        FactTotalCost = gr.Sum(m => m.FactTotalCost),
                                        FactLossCost = gr.Sum(m => m.FactLossCost),
                                        ProcLoss = gr.Sum(m => m.FactLoss) / gr.Sum(m => m.FactTotal) *100
                                    };

                MonthUseList = new ObservableCollection<ViewLossesFact>();
                MonthUseList.Clear();

                foreach (var row in filterRezult1)
                {
                    MonthUseList.Add(new ViewLossesFact()
                    {
                        IdEnergyResource = row.IdEnergyResource,
                        ERName = row.ERName,
                        FactTotal = row.FactTotal,
                        FactLoss = row.FactLoss,
                        FactTotalCost = row.FactTotalCost,
                        FactLossCost = row.FactLossCost,
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
                    FactTotalCost = MonthUseList.Sum(n => n.FactTotalCost),
                    FactLossCost = MonthUseList.Sum(n => n.FactLossCost),
                    ProcLoss = MonthUseList.Sum(n => n.FactLossCost) / MonthUseList.Sum(n => n.FactTotalCost) * 100
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
            double Proc = 3;
            var qry2 = from o in MonthUseList
                       select new
                       {
                           o.ERName,
                           o.FactLossCost
                       };
            double Total1 = qry2.Sum(n => n.FactLossCost);

            var qry3 = from o in MonthUseList
                       where o.FactLossCost >= Total1 * Proc / 100
                       select new
                       {
                           o.ERName,
                           o.FactLossCost
                       };
            var qry4 = from o in MonthUseList
                       where o.FactLossCost < Total1 * Proc / 100
                       select new
                       {
                           o.ERName,
                           o.FactLossCost
                       };
            double Total2 = qry4.Sum(n => n.FactLossCost);

            SeriesCollection.Clear();
            foreach (var newY in qry3.ToList())
            {
                PieSeries ps10 = new PieSeries
                {
                    Title = newY.ERName,
                    DataLabels = true,
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    LabelPoint = point => string.Format("{0},\r\n {1:N0}", newY.ERName, point.Y),
                    Values = new ChartValues<ObservableValue> { new ObservableValue(newY.FactLossCost) }
                };
                SeriesCollection.Add(ps10);
            }

            PieSeries ps2 = new PieSeries
            {
                Title = "Прочие",
                DataLabels = true,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                LabelPoint = point => string.Format("{0},\r\n {1:N0}", "Прочие", point.Y),
                Values = new ChartValues<ObservableValue> { new ObservableValue(Total2) }
            };
            SeriesCollection.Add(ps2);


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
            string date1 = string.Format("{0: 00}", StartMonth) + "." + StartYear + " г.";
            string date2 = string.Format("{0: 00}", EndMonth) + "." + EndYear + " г.";
            ChartCaption = string.Format("Структура потерь энергоресурсов за период с: {0} по: {1}, {2}", date1, date2, "руб.");
        }

    }
}
