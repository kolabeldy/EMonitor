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

        public FabricateUseModel()
        {
            Labels = new ObservableCollection<string>();
            SeriesCollection = new SeriesCollection();
            StartYear = DateTime.Now.Year;
            StartMonth = 1;
            EndYear = DateTime.Now.Year;
            EndMonth = DateTime.Now.Month - 1;
            SelectedCostCenter = 0;
            SelectedER = 958;
            IsAnalysis = true;
            IsChoiseResource = true;
            RBCostIsChecked = false;
            YearListFill();
            MonthListFill();
            ERListFill();
            CostCenterListFill();
            MonthUseListFill();
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
                db.ViewFabricateUses.Load();
                var filterRezult = from o in db.ViewFabricateUses
                           where o.Year * 100 + o.Month >= StartYear * 100 + StartMonth
                           where o.Year * 100 + o.Month <= EndYear * 100 + EndMonth
                           where o.IdProduct == SelectedER
                           group o by new { o.IdProduct, o.Period } into gr
                           orderby gr.Max(m => m.Year), gr.Max(m => m.Month)
                           select new
                            {
                               IdProduct = gr.Key.IdProduct,
                               Period = gr.Key.Period,
                               ResourceName = gr.Max(m => m.ERName),
                               Year = gr.Max(m => m.Year),
                               Month = gr.Max(m => m.Month),
                               UnitName = gr.Max(m => m.UnitName),
                               Fabricate = gr.Sum(m => m.Fabricate),
                               Fact1 = gr.Sum(m => m.Fact1),
                               Fact0 = gr.Sum(m => m.Fact0),
                               Loss = gr.Sum(m => m.Loss)
                           };

                MonthUseList = new ObservableCollection<ViewFabricateUse>();
                MonthUseList.Clear();
                foreach (var row in filterRezult)
                {
                    MonthUseList.Add(new ViewFabricateUse()
                    {
                        Year = row.Year,
                        Month = row.Month,
                        Period = Global.MonthName(row.Month) + "." + row.Year,
                        IdProduct = row.IdProduct,
                        ERName = row.ResourceName,
                        UnitName = row.UnitName,
                        Fabricate= row.Fabricate,
                        Fact1 = row.Fact1,
                        Fact0 = row.Fact0,
                        Loss = row.Loss
                    });
                }
                FabricateUseTotalList = new ObservableCollection<ViewFabricateUse>();
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
                           where o.IsActual==1
                           where o.IsPrime == 0
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
            string[] SeriesTitle = new string[] { "Потребл", "Продажа", "Потери" };
            int i = !RBCostIsChecked ? 0 : 3;
            int dlina = MonthUseList.Count();
            SeriesCollection.Clear();
            StackedAreaSeries ls1 = new StackedAreaSeries
            {
                Title = SeriesTitle[i],
                LineSmoothness = 0,
                DataLabels = dlina <= 12 ? true : false,
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
                DataLabels = dlina <= 12 ? true : false,
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
                DataLabels = dlina <= 12 ? true : false,
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
            Labels = new ObservableCollection<string>(MonthUseList.Select(x => x.Period));

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

            string unit = !RBCostIsChecked ? unitName : "руб.";
            string date1 = string.Format("{0: 00}", StartMonth) + "." + StartYear + " г.";
            string date2 = string.Format("{0: 00}", EndMonth) + "." + EndYear + " г.";
            ChartCaption = string.Format("Распределение '{0}'{1} за период с: {2} по: {3}{4}", resName, resCostCenter, date1, date2, unit);
        }
    }
}
