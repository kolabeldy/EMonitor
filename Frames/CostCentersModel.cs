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
using Excel = Microsoft.Office.Interop.Excel;

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
                IsFilterChanged = true;
                RaisePropertyChanged(() => ChartCaption);
            }
        }
        private string cCaption;

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
                IsFilterChanged = true;
                RaisePropertyChanged(() => IsChoiseResource);
            }
        }

        private List<int> _procList;
        public List<int> ProcList
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

        public List<ViewResult> ChartList { get; set; }

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

        public CostCentersModel()
        {
            ProcList = new List<int> { 0, 1, 2, 3, 5, 8, 10 };
            Proc = 3;
            SeriesCollection = new SeriesCollection();
            SeriesCollection1 = new SeriesCollection();
            Labels = new ObservableCollection<string>();
            ChartList = new List<ViewResult>();
            EndYearMonthFill();

            SelectedCostCenter = 0;
            IsChoiseResource = true;

            YearList = new List<ViewYear>();
            YearListFill();

            MonthList = new ObservableCollection<MonthClass>();
            MonthListFill();

            CostCenterList = new List<CostCenter>();
            CostCenterListFill();

            MonthUseList = new ObservableCollection<ViewResult>();
            MonthUseTotalList = new ObservableCollection<ViewResult>();
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
                var qrySource = from o in db.ViewResults.ToList()
                                where o.IsMain >= (IsChoiseResource ? 1 : 0)
                                where o.Year * 100 + o.Month >= StartYear * 100 + StartMonth
                                where o.Year * 100 + o.Month <= EndYear * 100 + EndMonth
                                where SelectedCostCenter == 0 ? true : o.IdCostCenter == SelectedCostCenter
                                group o by new { o.IdEnergyResource } into gr
                                orderby gr.Sum(m => m.FactCost) descending
                                select new ViewResult
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
                MonthUseList.Clear();
                MonthUseList = Global.ObservableCollection<ViewResult>(qrySource);

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
            ChartCaption = string.Format("Потребление энергоресурсов {0} {1} {2} ({3} > {4}%)", 
                resCostCenter, period, unit, selres, Proc);
            cCaption = string.Format("Потребление энергоресурсов {0} {1} {2} ({3})",
                resCostCenter, period, unit, selres);
        }
        public ICommand ExportCommand { get { return new RelayCommand<int>(OnExport); } }
        private void OnExport(int numToEdit = 0)
        {
            int len1 = MonthUseList.Count();
            int len2 = 9;
            int len3;
            int len4;

            object[,] arrRes = new object[len1, len2];
            object[,] arrChart = new object[len1, 2];
            object[] arrCaption = new object[]{ "Код", "Ресурс", "ЕдИзм", "План", "Факт", "Откл", "ПланРуб", "ФактРуб", "ОтклРуб" };
            object[] arrCaption1 = new object[] {"Ресурс", "ФактРуб"};
            DBListToArray();
            ArrToExcel();

            void DBListToArray()
            {
                int i = 0;

                foreach (var r in MonthUseList)
                {
                    arrRes[i, 0] = r.IdEnergyResource;
                    arrRes[i, 1] = r.ResourceName;
                    arrRes[i, 2] = r.UnitName;
                    arrRes[i, 3] = r.Plan;
                    arrRes[i, 4] = r.Fact;
                    arrRes[i, 5] = r.Difference;
                    arrRes[i, 6] = r.PlanCost;
                    arrRes[i, 7] = r.FactCost;
                    arrRes[i, 8] = r.DifferenceCost;
                    i++;
                }
                var qry2 = from o in MonthUseList
                           select new
                           {
                               o.FactCost,
                               o.DifferenceCost,
                               o.ResourceName
                           };
                double Total = qry2.Sum(n => n.FactCost);

                var qry3 = from o in MonthUseList
                           where o.FactCost >= Total * Proc / 100
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
                int j = 0;
                ChartList.Clear();
                foreach (var newY in qry3.ToList())
                {
                    ViewResult r = new ViewResult();
                    r.ResourceName = newY.ResourceName;
                    r.FactCost = newY.FactCost;
                    ChartList.Add(r);
                    j++;
                }
                ViewResult s = new ViewResult();
                s.ResourceName = "Прочие";
                s.FactCost = Total1;
                ChartList.Add(s);

                len3 = ChartList.Count();
                len4 = 2;

                
                i = 0;

                foreach (var l in ChartList)
                {
                    arrChart[i, 0] = l.ResourceName;
                    arrChart[i, 1] = l.FactCost;
                    i++;
                }

            }


            void ArrToExcel()
            {
                Excel.Application ex = new Excel.Application();

                //ex.SheetsInNewWorkbook = 1;
                //Excel.Workbook workBook = ex.Workbooks.Add(Type.Missing);

                ex.Workbooks.Open(AppDomain.CurrentDomain.BaseDirectory + @"tmpcostcenters.xlsx",
                  Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                  Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                  Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                  Type.Missing, Type.Missing);


                ex.DisplayAlerts = true;
                Excel.Worksheet sheet = (Excel.Worksheet)ex.Worksheets.get_Item(1);
                sheet.Name = "ЦентрыЗатрат";

                Excel.Range c1 = (Excel.Range)sheet.Cells[1, 1];
                Excel.Range c2 = (Excel.Range)sheet.Cells[1, 1];

                Excel.Range rangeCapt = sheet.get_Range(c1, c2);
                rangeCapt.Value = cCaption;

                c1 = (Excel.Range)sheet.Cells[3, 1];
                c2 = (Excel.Range)sheet.Cells[3, 9];
                Excel.Range rangeCaption = sheet.get_Range(c1, c2);
                rangeCaption.Value = arrCaption;

                c1 = (Excel.Range)sheet.Cells[4, 1];
                c2 = (Excel.Range)sheet.Cells[4 + len1 - 1, 9];
                Excel.Range range = sheet.get_Range(c1, c2);
                range.Value = arrRes;

                sheet = (Excel.Worksheet)ex.Worksheets.get_Item(2);
                c1 = (Excel.Range)sheet.Cells[1, 1];
                c2 = (Excel.Range)sheet.Cells[1, 1];

                rangeCapt = sheet.get_Range(c1, c2);
                rangeCapt.Value = "Структура фактического потребления, руб.";

                c1 = (Excel.Range)sheet.Cells[3, 1];
                c2 = (Excel.Range)sheet.Cells[3, 2];
                rangeCaption = sheet.get_Range(c1, c2);
                rangeCaption.Value = arrCaption1;

                c1 = (Excel.Range)sheet.Cells[4, 1];
                c2 = (Excel.Range)sheet.Cells[4 + len3 - 1, 2];
                range = sheet.get_Range(c1, c2);
                range.Value = arrChart;

                ex.Visible = true;
                ex.DisplayAlerts = true;

                //Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                //dlg.FileName = "CostCenters01"; // Default file name
                //dlg.DefaultExt = ".xlsx"; // Default file extension
                //dlg.Filter = "Книга Excel (.xlsx)|*.xlsx"; // Filter files by extension
                //dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //Nullable<bool> result = dlg.ShowDialog();

                //if (result == true)
                //{
                //    string filename = dlg.FileName;
                //    ex.Application.ActiveWorkbook.SaveAs(filename + @"", Type.Missing,
                //              Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange,
                //              Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                //    ex.WindowState = Excel.XlWindowState.xlMaximized;
                //    MessageBoxResult result1 = System.Windows.MessageBox.Show("Отчёт успешно сформирован",
                //              "Поздравляю",
                //              MessageBoxButton.OK,
                //              MessageBoxImage.Information);
                //    //ex.Visible = true;
                //}
                //ex.Quit();

            }
        }

    }
}
