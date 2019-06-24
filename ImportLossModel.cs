using EMonitor.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace EMonitor
{
    public class ImportLossModel
    {
        private MyDBContext db;
        public ImportLossModel()
        {
            string fahName = "FAHXXXXX.xls";
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
                Excel.Application xlApp = new Excel.Application(); //создаём приложение Excel


                xlWB = xlApp.Workbooks.Open(xlFileName); //открываем наш файл           
                xlSht = xlWB.ActiveSheet; //или так xlSht = xlWB.ActiveSheet //активный лист

                Rng = (Excel.Range)xlSht.Range["I1"];
                string nameFAH = Rng.Value.ToString().Trim();
                if(nameFAH != fahName)
                {
                    MessageBoxResult result2 = System.Windows.MessageBox.Show("Это не " + fahName, "Неверный файл", MessageBoxButton.OK, MessageBoxImage.Information);
                    xlWB.Close(true);
                    xlApp.Quit();
                    return;
                }

                iLastRow = xlSht.Cells[xlSht.Rows.Count, "A"].End[Excel.XlDirection.xlUp].Row; //последняя заполненная строка в столбце А
                iLastCol = xlSht.Cells[4, xlSht.Columns.Count].End[Excel.XlDirection.xlToLeft].Column; //последний заполненный столбец в 1-й строке


                Rng = (Excel.Range)xlSht.Range["A4", xlSht.Cells[iLastRow, iLastCol]]; //пример записи диапазона ячеек в переменную Rng
                var dataArr = (object[,])Rng.Value; //чтение данных из ячеек в массив
                //xlSht.get_Range("K1").get_Resize(dataArr.GetUpperBound(0), dataArr.GetUpperBound(1)).Value = dataArr; //выгрузка массива на лист

                int j = dataArr.GetLength(0);
                int recCount = 0;
                using (db = new MyDBContext())
                {
                    for (int i = 1; i <= j; i++)
                    {
                        if (dataArr[i, 15].ToString() == "Ф" && dataArr[i, 1].ToString() == newPeriod)
                        {
                            EnergyMonthUse eu = new EnergyMonthUse();
                            eu.Period = dataArr[i, 1].ToString();
                            eu.IdDepartMade = Convert.ToInt32(dataArr[i, 2].ToString().TrimStart('0'));
                            eu.IdEnergyResource = Convert.ToInt32(dataArr[i, 3]);
                            eu.IdOrganization = dataArr[i, 5] != null ? Convert.ToInt32(dataArr[i, 5]) : 0;
                            eu.IdCostCenter = dataArr[i, 6] != null ? Convert.ToInt32(dataArr[i, 6].ToString().TrimStart('0')) : 0;
                            eu.IdProduct = Convert.ToInt32(dataArr[i, 7]);
                            eu.Fact = dataArr[i, 10] != null ? Convert.ToDouble(dataArr[i, 10]) : 0;
                            eu.Plan = dataArr[i, 11] != null ? Convert.ToDouble(dataArr[i, 11]) : 0;
                            eu.Fabricate = dataArr[i, 14] != null ? Convert.ToDouble(dataArr[i, 14]) : 0;
                            db.EnergyMonthUses.Add(eu);
                            recCount++;
                        }
                    }
                    db.SaveChanges();
                }
                if (recCount > 0)
                {
                    MessageBoxResult result1 = System.Windows.MessageBox.Show(string.Format("Данные потерь за период {0} успешно импортированы.\n\nДобавлено {1} записей.", newPeriod, recCount.ToString()),
                                                                    "Поздравляю", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBoxResult result1 = System.Windows.MessageBox.Show(string.Format("Данные потерь за период {0} не обнаружены.\n\nДобавлено {1} записей.", newPeriod, recCount.ToString()),
                                                                    "Нет данных", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                xlWB.Close(true);
                xlApp.Quit();
            }
        }

    }
}
