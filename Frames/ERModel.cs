﻿using GalaSoft.MvvmLight;
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
using Excel = Microsoft.Office.Interop.Excel;

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
            EndYearMonthFill();

            SelectedER = 955;
            RBCostIsChecked = false;
            YearListFill();
            MonthListFill();

            CostCenterList = new List<CostCenter>();
            CostCenterListFill();

            MonthUseList = new ObservableCollection<ViewResult>();
            MonthUseTotalList = new ObservableCollection<ViewResult>();
            MonthUseListFill();

            ERList = new ObservableCollection<EnergyResource>();
            ERListFill();
            DiagrammInit();
            ChartCaptionFill();
            IsChoiseResource = true;
            IsFilterChanged = false;
        }
        private void CostCenterListFill()
        {
            using (db = new MyDBContext())
            {
                db.CostCenters.Load();
                CostCenterList = db.CostCenters.Local.ToList();
            }
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
            StartYear = EndMonth == 1? EndYear - 1 : EndYear;
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

        private void MonthUseListFill()
        {
            using (db = new MyDBContext())
            {
                db.ViewResults.Load();
                var qrySource = from o in db.ViewResults.ToList()
                                where o.Year * 100 + o.Month >= StartYear * 100 + StartMonth
                                where o.Year * 100 + o.Month <= EndYear * 100 + EndMonth
                                where o.IdEnergyResource == SelectedER
                                where SelectedCostCenter == 0 ? true : o.IdCostCenter == SelectedCostCenter
                                group o by new { o.IdEnergyResource, o.Period } into gr
                                orderby gr.Max(m => m.Year), gr.Max(m => m.Month)
                                select new ViewResult
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
                MonthUseList.Clear();
                MonthUseList = Global.ObservableCollection<ViewResult>(qrySource);

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
                YearList = db.ViewYears.Local.ToList();
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

            string period;
            if (StartYear == EndYear && StartMonth == EndMonth)
            {
                period = string.Format("за {0}", date1);
            }
            else
            {
                period = string.Format("за период с: {0} по: {1}", date1, date2);
            }

            ChartCaption = string.Format("Потребление '{0}' по {1} {2}, {3}", resName, resCostCenter, period, unit);
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
            object[] arrCaption = new object[] { "Код", "Ресурс", "ЕдИзм", "План", "Факт", "Откл", "ПланРуб", "ФактРуб", "ОтклРуб" };
            object[] arrCaption1 = new object[] { "Ресурс", "ФактРуб" };
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

                //var qry3 = from o in MonthUseList
                //           where o.FactCost >= Total * Proc / 100
                //           orderby o.FactCost descending
                //           select new
                //           {
                //               o.FactCost,
                //               o.ResourceName
                //           };
                //var qry4 = from o in MonthUseList
                //           where o.FactCost < Total * Proc / 100
                //           select new
                //           {
                //               o.FactCost,
                //               o.ResourceName
                //           };
                //double Total1 = qry4.Sum(n => n.FactCost);
                //int j = 0;
                //ChartList.Clear();
                //foreach (var newY in qry3.ToList())
                //{
                //    ViewResult r = new ViewResult();
                //    r.ResourceName = newY.ResourceName;
                //    r.FactCost = newY.FactCost;
                //    ChartList.Add(r);
                //    j++;
                //}
                //ViewResult s = new ViewResult();
                //s.ResourceName = "Прочие";
                //s.FactCost = Total1;
                //ChartList.Add(s);

                //len3 = ChartList.Count();
                //len4 = 2;


                //i = 0;

                //foreach (var l in ChartList)
                //{
                //    arrChart[i, 0] = l.ResourceName;
                //    arrChart[i, 1] = l.FactCost;
                //    i++;
                //}

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
                rangeCapt.Value = ChartCaption;

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

                //rangeCapt = sheet.get_Range(c1, c2);
                //rangeCapt.Value = "Структура фактического потребления, руб.";

                //c1 = (Excel.Range)sheet.Cells[3, 1];
                //c2 = (Excel.Range)sheet.Cells[3, 2];
                //rangeCaption = sheet.get_Range(c1, c2);
                //rangeCaption.Value = arrCaption1;

                //c1 = (Excel.Range)sheet.Cells[4, 1];
                //c2 = (Excel.Range)sheet.Cells[4 + len3 - 1, 2];
                //range = sheet.get_Range(c1, c2);
                //range.Value = arrChart;

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
