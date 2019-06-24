using EMonitor.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace EMonitor.Frames
{
    public enum OperationClass { Profit, Cost };
    public static class Global
    {
        public const int ItemProfits = 2;
        public const int ItemCosts = 7;
        public const int ItemIdStartBalance = 4;
        public const int ItemIdProfitBalanceCorrection = 5;
        public const int ItemIdProfitTransaction = 6;
        public const int ItemIdCostBalanceCorrection = 9;
        public const int ItemIdCostTransaction = 10;

        public const string FullCodeProfits = "2.";
        public const string FullCodeCosts = "7.";

        public const string FullNameCosts ="Расходы\\";
        public const string NameCosts = "Расходы";

        public const string FullNameProfits = "Доходы\\";
        public const string NameProfits = "Доходы";


        static public int MainWindowWidth = 1050;
        static public int MainWindowHeight = 750;

        public static ObservableCollection<T> ObservableCollection<T>(this IEnumerable<T> qrySource)
        {
            if (qrySource == null)
            {
                throw new ArgumentNullException("qrySource");
            }
            return new ObservableCollection<T>(qrySource);
        }

        static public string MonthName(int monthNom)
        {
            string rez = null;
            switch (monthNom)
            {
                case 1:
                    rez = "янв";
                    break;
                case 2:
                    rez = "фев";
                    break;
                case 3:
                    rez = "мар";
                    break;
                case 4:
                    rez = "апр";
                    break;
                case 5:
                    rez = "май";
                    break;
                case 6:
                    rez = "июн";
                    break;
                case 7:
                    rez = "июл";
                    break;
                case 8:
                    rez = "авг";
                    break;
                case 9:
                    rez = "сен";
                    break;
                case 10:
                    rez = "окт";
                    break;
                case 11:
                    rez = "ноя";
                    break;
                case 12:
                    rez = "дек";
                    break;
            }
            return rez;
        }

        static public int MonthNumber(string monthName)
        {
            int rez = 0;
            switch (monthName)
            {
                case "янв":
                    rez = 1;
                    break;
                case "фев":
                    rez = 2;
                    break;
                case "мар":
                    rez = 3;
                    break;
                case "апр":
                    rez = 4;
                    break;
                case "май":
                    rez = 5;
                    break;
                case "июн":
                    rez = 6;
                    break;
                case "июл":
                    rez = 7;
                    break;
                case "авг":
                    rez = 8;
                    break;
                case "сен":
                    rez = 9;
                    break;
                case "окт":
                    rez = 10;
                    break;
                case "ноя":
                    rez = 11;
                    break;
                case "дек":
                    rez = 12;
                    break;
            }
            return rez;
        }
        static public string NumericToString(TextBox strSource)
        {
            strSource.Text = Regex.Replace(strSource.Text, "[^0-9,]+", "");
            Regex regex = new Regex("[^0-9,]+");
            bool handle = regex.IsMatch(strSource.Text);
            if (handle)
            {
                StringBuilder dd = new StringBuilder();
                int i = -1;
                int cursor = -1;
                foreach (char item in strSource.Text)
                {
                    i++;
                    if (char.IsDigit(item) || item == (",").ToCharArray()[0])
                        dd.Append(item);
                    else if (cursor == -1)
                        cursor = i;
                }
                strSource.Text = dd.ToString();

                if (i == -1)
                    strSource.SelectionStart = strSource.Text.Length;
                else
                    strSource.SelectionStart = cursor;
            }
            if (strSource.Text.Length > 0 & new Regex(",").Matches(strSource.Text).Count <= 1)
            {
                return strSource.Text;
            }
            else return "0";
        }

        static public int RetYearFromPeriod(string period)
        {
            string[] parts = period.Split(new char[] { ' ' });
            return Convert.ToInt32(parts[0]);
        }
        static public int RetMonthFromPeriod(string period)
        {
            string[] parts = period.Split(new char[] { ' ' });
            return MonthNumber(parts[1]);
        }

        static public DateTime StartDate { get; set; }
        static public DateTime EndDate { get; set; }



        static public void ToolBarLoaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            if (toolBar.Template.FindName("OverflowGrid", toolBar) is FrameworkElement overflowGrid)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            if (toolBar.Template.FindName("MainPanelBorder", toolBar) is FrameworkElement mainPanelBorder)
            {
                mainPanelBorder.Margin = new Thickness();
            }

        }




        //*******************************************************************
        // Отключение клавиши Backspace для навигации по страницам
        //*******************************************************************
        //KeyGesture backKeyGesture = null;
        //foreach (var gesture in NavigationCommands.BrowseBack.InputGestures)
        //{
        //    KeyGesture keyGesture = gesture as KeyGesture;
        //    if ((keyGesture != null) &&
        //       (keyGesture.Key == Key.Back) &&
        //       (keyGesture.Modifiers == ModifierKeys.None))
        //    {
        //        backKeyGesture = keyGesture;
        //    }
        //}

        //if (backKeyGesture != null)
        //{
        //    NavigationCommands.BrowseBack.InputGestures.Remove(backKeyGesture);
        //}



    }
}
