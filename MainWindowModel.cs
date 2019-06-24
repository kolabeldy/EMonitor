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
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace EMonitor
{
    public class MainWindowModel : ViewModelBase
    {
        private MyDBContext db;
        private Excel.Application ex; 
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
                RaisePropertyChanged(() => StartMonth);
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
        private bool _isReportMonthChoise;
        public bool IsReportMonthChoise
        {
            get
            {
                return _isReportMonthChoise;
            }
            set
            {
                _isReportMonthChoise = value;
                RaisePropertyChanged(() => IsReportMonthChoise);
            }
        }

        public MainWindowModel()
        {
            MonthListFill();
            YearListFill();
            StartYear = DateTime.Now.Year;
            StartMonth = DateTime.Now.Month ==1? 12: DateTime.Now.Month - 1;
            IsReportMonthChoise = false;
        }


        public ICommand MonthReportToExcelCommand { get { return new RelayCommand<int>(ToExcelOn); } }
        private void ToExcelOn(int mes = 0)
        {
            ex = new Excel.Application();
            IsReportMonthChoise = false;
            int reportYear = StartYear;
            int reportMonth = StartMonth;
            using (db = new MyDBContext())
            {
                db.ViewResults.Load();
                var qry2 = from o in db.ViewResults
                           where o.IsMain == 1
                           where o.Year == reportYear
                           where o.Month == reportMonth
                           orderby o.IdEnergyResource
                           select new
                           {
                               o.Id,
                               o.Year,
                               o.Month,
                               o.IdDepart,
                               o.IdCategory,
                               o.DepartCategory,
                               o.IdCostCenter,
                               o.IdEnergyResource,
                               o.ResourceName,
                               o.UnitName,
                               o.Plan,
                               o.Fact,
                               o.Difference,
                               o.PlanCost,
                               o.FactCost,
                               o.DifferenceCost
                           };

                MonthUseList = new ObservableCollection<ViewResult>();
                MonthUseList.Clear();
                int rc = 0;
                foreach (var row in qry2)
                {
                    rc++;
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

                mes = mes == 0 ? DateTime.Now.Month - 1 : mes;

                ex.Workbooks.Open(AppDomain.CurrentDomain.BaseDirectory + @"repmonthbase.xlsx",
                                  Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                  Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                  Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                  Type.Missing, Type.Missing);

                ex.DisplayAlerts = false;
                Excel.Worksheet sheet = (Excel.Worksheet)ex.Worksheets.get_Item(1);

                int i = 0;
                //int rc = qry2.ToList().Count;
                object[,] arr = new object[rc, 15];

                foreach (var r in qry2.ToList())
                {
                    arr[i, 0] = r.Year;
                    arr[i, 1] = r.Month;
                    arr[i, 2] = r.IdDepart;
                    arr[i, 3] = r.IdCategory;
                    arr[i, 4] = r.DepartCategory;
                    arr[i, 5] = r.IdCostCenter;
                    arr[i, 6] = r.IdEnergyResource;
                    arr[i, 7] = r.ResourceName;
                    arr[i, 8] = r.UnitName;
                    arr[i, 9] = r.Plan;
                    arr[i, 10] = r.Fact;
                    arr[i, 11] = r.Difference;
                    arr[i, 12] = r.PlanCost;
                    arr[i, 13] = r.FactCost;
                    arr[i, 14] = r.DifferenceCost;
                    i++;
                }

                Excel.Range c1 = (Excel.Range)sheet.Cells[2, 1];
                Excel.Range c2 = (Excel.Range)sheet.Cells[2 + rc - 1, 15];
                Excel.Range range = sheet.get_Range(c1, c2);

                range.Value = arr;
                ex.Application.ActiveWorkbook.RefreshAll();
                Excel.Worksheet sh1 = ex.Application.ActiveWorkbook.Sheets.get_Item(1);
                sh1.Visible = Excel.XlSheetVisibility.xlSheetHidden;

                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Отчёт за месяц " + reportYear + "_" + reportMonth.ToString("00"); // Default file name
                dlg.DefaultExt = ".xlsx"; // Default file extension
                dlg.Filter = "Книга Excel (.xlsx)|*.xlsx"; // Filter files by extension
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    string filename = dlg.FileName;
                    ex.Application.ActiveWorkbook.SaveAs(filename + @"", Type.Missing,
                              Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange,
                              Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    ex.WindowState = Excel.XlWindowState.xlMaximized;
                    MessageBoxResult result1 = System.Windows.MessageBox.Show("Отчёт успешно сформирован",
                              "Поздравляю",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
                    //ex.Visible = true;
                }
                //ex.Quit();
            }
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

        public ICommand ReportMonthInitCommand { get { return new RelayCommand<int>(InitOn); } }
        private void InitOn(int mes = 0)
        {
            IsReportMonthChoise = true;
        }
        public ICommand ImportDBCommand { get { return new RelayCommand<int>(ImportDBOn); } }
        private void ImportDBOn(int mes = 0)
        {
            ImportDBModel import = new ImportDBModel();

        }

        public ICommand ImportLossCommand { get { return new RelayCommand<int>(ImportLossOn); } }
        private void ImportLossOn(int mes = 0)
        {
            ImportLossModel import = new ImportLossModel();

        }

    }
}
