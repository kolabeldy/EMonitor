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
    public class FavoritModel : ViewModelBase
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

        private bool _isAnalysis;
        public bool IsAnalysis
        {
            get
            {
                return _isAnalysis;
            }
            set
            {
                _isAnalysis = value;
                IsFilterChanged = true;
                RaisePropertyChanged(() => IsAnalysis);
            }
        }

        private bool _isActual;
        public bool IsActual
        {
            get
            {
                return _isActual;
            }
            set
            {
                _isActual = value;
                IsFilterChanged = true;
                RaisePropertyChanged(() => IsActual);
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

        private ObservableCollection<int> _costCenterList;
        public ObservableCollection<int> CostCenterList
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

        public FavoritModel()
        {
            Labels = new ObservableCollection<string>();
            SeriesCollection = new SeriesCollection();
            SeriesCollection1 = new SeriesCollection();
            StartYear = DateTime.Now.Year;
            StartMonth = 1;
            EndYear = DateTime.Now.Year;
            EndMonth = DateTime.Now.Month - 1;
            SelectedCostCenter = 0;
            SelectedER = 955;
            IsAnalysis = true;
            IsChoiseResource = true;
            RBCostIsChecked = false;
            YearListFill();
            MonthListFill();
            ERListFill();
            MonthUseListFill();
            CostCenterListFill();
            DiagrammInit();
            ChartCaptionFill();
            IsFilterChanged = false;
            //int a = 2;
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
        private void MonthUseListFill()
        {
            using (db = new MyDBContext())
            {
                db.ViewResults.Load();
                db.CostCenters.Load();
                var filterRezult = from o in db.ViewResults
                                   join a in db.CostCenters on o.IdCostCenter equals a.Id
                                   join b in db.EnergyResources on o.IdEnergyResource equals b.Id
                                   where o.Year * 100 + o.Month >= StartYear * 100 + StartMonth
                                   where o.Year * 100 + o.Month <= EndYear * 100 + EndMonth
                                   where o.IdEnergyResource == SelectedER
                                   where a.IsMain >= (IsAnalysis?1:0)
                                   where b.IsActual== 1
                                   group o by new { o.IdEnergyResource, o.IdCostCenter } into gr
                                   orderby gr.Sum(m => m.IdCostCenter)
                                   select new
                                   {
                                       IdEnergyResource = gr.Key.IdEnergyResource,
                                       IdCostCenter = gr.Key.IdCostCenter,
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
                foreach (var row in filterRezult)
                {
                    MonthUseList.Add(new ViewResult()
                    {
                        Year = row.Year,
                        Month = row.Month,
                        IdCostCenter = row.IdCostCenter,
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

            var qry2 = from o in MonthUseList
                       select new { o.IdCostCenter };

            CostCenterList = new ObservableCollection<int>();
            CostCenterList.Clear();
            foreach (var row in qry2)
            {
                CostCenterList.Add(row.IdCostCenter);
            }
        }
        private void ERListFill()
        {
            using (db = new MyDBContext())
            {
                db.EnergyResources.Load();

                var qry2 = from o in db.EnergyResources
                           where o.IsMain >= (IsChoiseResource ? 1 : 0)
                           where o.IsActual == 1
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
        public SeriesCollection SeriesCollection1 { get; set; }
        public Func<double, string> Formatter { get; set; }

        public ObservableCollection<string> Labels { get; set; }
        public void DiagrammInit()
        {
            int Proc = 3;
            var qry2 = from o in MonthUseList
                       select new
                       {
                           o.Fact,
                           o.Difference,
                           o.IdCostCenter
                       };
            double Total = qry2.Sum(n => n.Fact);
            double TotalDiff = qry2.Sum(n => Math.Abs(n.Difference));

            var qry3 = from o in MonthUseList
                       where o.Fact >= Total * Proc / 100
                       orderby o.Fact descending
                       select new
                       {
                           o.Fact,
                           o.IdCostCenter
                       };
            var qry4 = from o in MonthUseList
                       where o.Fact < Total * Proc / 100
                       select new
                       {
                           o.Fact,
                           o.IdCostCenter
                       };
            double Total1 = qry4.Sum(n => n.Fact);
            SeriesCollection.Clear();
            foreach (var newY in qry3.ToList())
            {
                PieSeries ps10 = new PieSeries
                {
                    Title = newY.IdCostCenter.ToString(),
                    DataLabels = true,
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    LabelPoint = point => string.Format("ЦЗ-{0},\r\n {1:N0}", newY.IdCostCenter, point.Y),
                    Values = new ChartValues<ObservableValue> { new ObservableValue(newY.Fact) }
                };
                SeriesCollection.Add(ps10);

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
            SeriesCollection.Add(ps2);

            var qry5 = from o in MonthUseList
                       where Math.Abs(o.Difference) >= TotalDiff * Proc / 100
                       orderby o.Difference descending
                       select new
                       {
                           o.Difference,
                           o.IdCostCenter
                       };
            var qry6 = from o in MonthUseList
                       where Math.Abs(o.Difference) < TotalDiff * Proc / 100
                       select new
                       {
                           o.Difference,
                           o.IdCostCenter
                       };
            double Total2 = qry6.Sum(n => n.Difference);

            SeriesCollection1.Clear();
            ColumnSeries ps1 = new ColumnSeries()
            {
                //LabelsPosition = BarLabelPosition.Parallel,
                //Fill = System.Windows.Media.Brushes.PowderBlue,
                DataLabels = true,
                LabelPoint = point => string.Format("{0:N0}",  point.Y),
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Values = new ChartValues<ObservableValue>()
            };

            foreach (var newY in qry5.ToList())
            {
                ps1.Values.Add(new ObservableValue(newY.Difference));
            }
            ps1.Values.Add(new ObservableValue(Total2));
            SeriesCollection1.Add(ps1);

            Labels.Clear();
            foreach (var newX in qry5.ToList())
            {
                Labels.Add(newX.IdCostCenter.ToString());
            };
            Labels.Add("Прочие");



        }
        public ICommand FilterOnCommand { get { return new RelayCommand<int>(OnFilterOn); } }
        private void OnFilterOn(int numToEdit = 0)
        {
            MonthUseListFill();
            CostCenterListFill();
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
            string resCostCenter = IsAnalysis ? "основным ЦЗ" : "по всем ЦЗ";
            var unitName1 = (from o in MonthUseList
                             where o.IdEnergyResource == SelectedER
                             select new { o.UnitName });
            string unitName = unitName1.Count() != 0 ? unitName1.ToList()[0].UnitName.ToString() : unitName = "";

            string unit = !RBCostIsChecked ? unitName : "руб.";
            string date1 = string.Format("{0: 00}", StartMonth) + "." + StartYear + " г.";
            string date2 = string.Format("{0: 00}", EndMonth) + "." + EndYear + " г.";
            ChartCaption = string.Format("Потребление '{0}' по {1} за период с: {2} по: {3}, {4}", resName, resCostCenter, date1, date2, unit);
        }

    }
}
