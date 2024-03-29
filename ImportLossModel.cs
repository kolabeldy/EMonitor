﻿using EMonitor.DB;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace EMonitor
{
    public class ImportLossModel
    {
        private MyDBContext db;
        public ImportLossModel()
        {
            string fahName = "FAH0511E.xls";
            string lastPeriod = null;
            string newPeriod = null;
            using (db = new MyDBContext())
            {
                db.ViewResults.Load();
                var qry = (from o in db.ViewResults
                          orderby o.Year descending, o.Month descending
                           select new { o.Period }).Take(1);
                lastPeriod = qry.ToList()[0].Period;
                string lastMonth = lastPeriod.Substring(0, 2);
                string lastYear = lastPeriod.Substring(3, 4);
                string newMonth = lastMonth == "12" ? "01" : (Convert.ToInt32(lastMonth) + 1).ToString();
                newMonth = newMonth.Length < 2 ? "0" + newMonth : newMonth;
                string newYear = lastMonth != "12" ? lastYear : (Convert.ToInt32(lastYear) + 1).ToString();
                newPeriod = newMonth + "." + newYear;
            }

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = fahName;
            dlg.DefaultExt = ".xls"; // Default file extension
            dlg.Filter = "Книга Excel (.xls)|*.xls"; // Filter files by extension
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                Excel.Range Rng;
                Excel.Workbook xlWB;
                Excel.Worksheet xlSht;
                int iLastRow, iLastCol;
                string xlFileName;

                xlFileName = dlg.FileName;
                Excel.Application xlApp = new Excel.Application(); 

                xlWB = xlApp.Workbooks.Open(xlFileName);           
                xlSht = xlWB.ActiveSheet; 

                Rng = (Excel.Range)xlSht.Range["P1"];
                
                string nameFAH = Rng.Value != null? Rng.Value.ToString().Trim() : "";

                iLastRow = xlSht.Cells[xlSht.Rows.Count, "A"].End[Excel.XlDirection.xlUp].Row; //последняя заполненная строка в столбце А
                iLastCol = xlSht.Cells[4, xlSht.Columns.Count].End[Excel.XlDirection.xlToLeft].Column; //последний заполненный столбец в 1-й строке


                Rng = (Excel.Range)xlSht.Range["A5", xlSht.Cells[iLastRow, iLastCol]]; 
                var dataArr = (object[,])Rng.Value; //чтение данных из ячеек в массив
                //xlSht.get_Range("K1").get_Resize(dataArr.GetUpperBound(0), dataArr.GetUpperBound(1)).Value = dataArr; //выгрузка массива на лист

                if (nameFAH +".xls" != fahName || dataArr[1,4] != null)
                {
                    MessageBoxResult result2 = System.Windows.MessageBox.Show("Этот файл не содержит данных по потерям.\n\nСодержимое файла не соответствует требуемому!", "Неверный файл", MessageBoxButton.OK, MessageBoxImage.Information);
                    xlWB.Close(true);
                    xlApp.Quit();
                    return;
                }


                int j = dataArr.GetLength(0);
                int recCount = 0;
                using (db = new MyDBContext())
                {
                    for (int i = 1; i <= j; i++)
                    {
                        if (dataArr[i, 1].ToString() == newPeriod)
                        {
                            Losse eu = new Losse();
                            eu.DatePeriod = dataArr[i, 1].ToString();
                            eu.IdOrganization = Convert.ToInt32(dataArr[i, 2]);
                            eu.IdCostCenter = Convert.ToInt32(dataArr[i, 3].ToString().TrimStart('0'));
                            eu.Fact = Convert.ToDouble(dataArr[i, 7]);
                            eu.IdEnergyResource = Convert.ToInt32(dataArr[i, 16]);
                            eu.IdDepartMade = Convert.ToInt32(dataArr[i, 19].ToString().TrimStart('0'));
                            db.Losses.Add(eu);
                            recCount++;
                        }
                    }
                    db.SaveChanges();
                }
                if (recCount > 0)
                {
                    MessageBoxResult result1 = System.Windows.MessageBox.Show(string.Format("Данные потерь за период {0} успешно импортированы.\n\nДобавлено {1} записей.", 
                        newPeriod, recCount.ToString()),"Поздравляю", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBoxResult result1 = System.Windows.MessageBox.Show(string.Format("Данные потерь за период {0} не обнаружены.\n\nДобавлено {1} записей.", 
                        newPeriod, recCount.ToString()), "Нет данных", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                xlWB.Close(true);
                xlApp.Quit();
            }
        }

    }
}
