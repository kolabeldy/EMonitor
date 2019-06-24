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
    public class CostCentersModel : ViewModelBase
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

        private int oldProc;
        private int _proc;
        public int Proc
        {
            get
            {
                return _proc;
            }
            set
            {
                oldProc = _proc;
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

        private bool _rb_Fact_IsChecked;
        public bool RB_Fact_IsChecked
        {
            get
            {
                return _rb_Fact_IsChecked;
            }
            set
            {
                _rb_Fact_IsChecked = value;
                DiagrammInit();
                RaisePropertyChanged(() => RB_Fact_IsChecked);
            }
        }
        private bool _rb_Diff_IsChecked = false;
        public bool RB_Diff_IsChecked
        {
            get
            {
                return _rb_Diff_IsChecked;
            }
            set
            {
                _rb_Diff_IsChecked = value;
                RaisePropertyChanged(() => RB_Diff_IsChecked);
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
                IsFilterChanged = true;
                RaisePropertyChanged(() => IsChoiseResource);
            }
        }

        private ObservableCollection<int> _procList;
        public ObservableCollection<int> ProcList
        {
            get
            {
                return _procList;
            }
            set
            {
                _procList = value;
                RaisePropertyChanged(() => ProcList);
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

        public CostCentersModel()
        {
            ProcList = new ObservableCollection<int> { 0, 1, 2, 3, 5, 8, 10 };
            Proc = 3;
            oldProc = 2;
            SeriesCollection = new SeriesCollection();
            SeriesCollection1 = new SeriesCollection();
            Labels = new ObservableCollection<string>();
            SeriesCollection = new SeriesCollection();
            StartYear = DateTime.Now.Year;
            StartMonth = 1;
            EndYear = DateTime.Now.Year;
            EndMonth = DateTime.Now.Month - 1;
            SelectedCostCenter = 0;
            IsAnalysis = true;
            IsChoiseResource = true;
            RBCostIsChecked = true;
            YearListFill();
            MonthListFill();
            CostCenterListFill();
            MonthUseListFill();
            RB_Fact_IsChecked = true;
            //DiagrammInit();
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
                    var filterRezult = from o in db.ViewResults
                                       where o.IsMain >= (IsChoiseResource ? 1 : 0)
                                       where o.Year * 100 + o.Month >= StartYear * 100 + StartMonth
                                       where o.Year * 100 + o.Month <= EndYear * 100 + EndMonth
                                       where SelectedCostCenter == 0 ? true : o.IdCostCenter == SelectedCostCenter
                                       group o by new { o.IdEnergyResource } into gr
                                       orderby gr.Sum(m => m.FactCost) descending
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
                                           IsMain = gr.Max(m => m.IsMain)
                                       };
                    MonthUseList = new ObservableCollection<ViewResult>();
                    MonthUseList.Clear();
                    foreach (var row in filterRezult)
                    {
                        MonthUseList.Add(new ViewResult()
                        {
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
                           where o.FactCost>=Total * Proc / 100
                           orderby o.FactCost descending
                           select new
                           {
                               o.FactCost,
                               o.ResourceName
                           };
                var qry4 = from o in MonthUseList
                           where o.FactCost < Total * Proc / 100
                           select new
                           {
                               o.FactCost,
                               o.ResourceName
                           };
                double Total1 = qry4.Sum(n => n.FactCost);

                var qry5 = from o in MonthUseList
                           where Math.Abs(o.DifferenceCost) >= TotalDiff * Proc / 100
                           orderby o.DifferenceCost descending
                           select new
                           {
                               o.DifferenceCost,
                               o.ResourceName
                           };
                var qry6 = from o in MonthUseList
                           where Math.Abs(o.DifferenceCost) < TotalDiff * Proc / 100
                           //orderby o.DifferenceCost descending
                           select new
                           {
                               o.DifferenceCost,
                               o.ResourceName
                           };
                double Total2 = qry6.Sum(n => n.DifferenceCost);

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
            ps1.Values.Add(new ObservableValue(Total2));
            SeriesCollection1.Add(ps1);

            Labels.Clear();
            foreach (var newX in qry5.ToList())
            {
                Labels.Add(newX.ResourceName.ToString());
            };
            Labels.Add("Прочие");

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
            string resCostCenter = SelectedCostCenter == 0 ? "ПАО ХИМПРОМ" : "ЦЗ-" + SelectedCostCenter.ToString();
            string selres = !IsChoiseResource ? "все ресурсы" : "избранные ресурсы";
            string unit = !RBCostIsChecked ? ", в физических величинах" : ", в рублях";
            string date1 = string.Format("{0: 00}", StartMonth) + "." + StartYear + " г.";
            string date2 = string.Format("{0: 00}", EndMonth) + "." + EndYear + " г.";
            ChartCaption = string.Format("Фактическое потребление энергоресурсов {0} за период с: {1} по: {2}{3} ({4} > {5}%)", 
                resCostCenter, date1, date2, unit, selres, Proc);
        }

    }
}
