using EMonitor.DB;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows;
using LiveCharts.Defaults;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace EMonitor.Frames
{
    public class ChartERViewModel : ViewModelBase
    {
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
        private int _pieRadius;
        public int PieRadius
        {
            get
            {
                return _pieRadius;
            }
            set
            {
                _pieRadius = value;
                RaisePropertyChanged(() => PieRadius);
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
        private ObservableCollection<ViewResult> _monthUseERList;
        public ObservableCollection<ViewResult> MonthUseERList
        {
            get
            {
                return _monthUseERList;
            }
            set
            {
                _monthUseERList = value;
                RaisePropertyChanged(() => MonthUseERList);
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
        private double _windowWidth;
        private double _windowHeight;

        public double WindowWidth
        {
            get { return _windowWidth; }
            set
            {
                if (!Equals(_windowWidth, value))
                {
                    _windowWidth = value;
                    PieRadius = _windowWidth/WindowHeight < 0.9 ? (int)_windowWidth / 6: PieRadius;
                    RaisePropertyChanged(() => WindowWidth);
                }
            }
        }
        public double WindowHeight
        {
            get { return _windowHeight; }
            set
            {
                if (!Equals(_windowHeight, value))
                {
                    _windowHeight = value;
                    PieRadius = (int)_windowHeight / 6;
                    RaisePropertyChanged(() => WindowHeight);
                }
            }
        }

        public ChartERViewModel(ObservableCollection<ViewResult> MonthUseERList)
        {
            this.MonthUseERList = MonthUseERList;
            ProcList = new ObservableCollection<int> { 0, 1, 2, 3, 5, 8, 10 };
            Proc = 2;
            WindowWidth = 800;
            WindowHeight = 800;
            PieRadius = (int)WindowHeight/6;
            SeriesCollection1 = new SeriesCollection();
            SeriesCollection2 = new SeriesCollection();
            LabelsER = new ObservableCollection<string>();
            IsFilterChanged = false;
            DiagrammERInit();
        }
        public SeriesCollection SeriesCollection1 { get; set; }
        public SeriesCollection SeriesCollection2 { get; set; }
        public Func<double, string> FormatterER { get; set; }

        public ObservableCollection<string> LabelsER { get; set; }
        public void DiagrammERInit()
        {
            var qry2 = from o in MonthUseERList
                       select new
                       {
                           o.FactCost,
                           o.DifferenceCost,
                           o.ResourceName
                       };
            double Total = qry2.Sum(n => n.FactCost);

            var qry3 = from o in MonthUseERList
                       where o.FactCost >= Total * Proc / 100
                       orderby o.IdEnergyResource
                       select new
                       {
                           o.FactCost,
                           o.ResourceName
                       };
            var qry4 = from o in MonthUseERList
                       where o.FactCost < Total * Proc / 100
                       select new
                       {
                           o.FactCost,
                           o.ResourceName
                       };
            double Total1 = qry4.Sum(n => n.FactCost);

            SeriesCollection1.Clear();
            foreach (var newY in qry3.ToList())
            {
                PieSeries ps10 = new PieSeries
                {
                    Title = newY.ResourceName,
                    DataLabels = true,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    LabelPoint = point => string.Format("{0},\r\n {1:N0}", newY.ResourceName, point.Y),
                    Values = new ChartValues<ObservableValue> { new ObservableValue(newY.FactCost) }
                };
                SeriesCollection1.Add(ps10);

            }
            PieSeries ps2 = new PieSeries
            {
                Title = "Прочие",
                DataLabels = true,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                LabelPoint = point => string.Format("{0},\r\n {1:N0}", "Прочие", point.Y),
                Values = new ChartValues<ObservableValue> { new ObservableValue(Total1) }
            };
            SeriesCollection1.Add(ps2);


        }
        public ICommand FilterOnCommand { get { return new RelayCommand<int>(OnFilterOn); } }
        private void OnFilterOn(int numToEdit = 0)
        {
            DiagrammERInit();
            IsFilterChanged = false;
        }

    }
}
