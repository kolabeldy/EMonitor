using EMonitor.DB;
using LiveCharts;
using System.Collections.ObjectModel;
using System.Windows;

namespace EMonitor.Frames
{
    /// <summary>
    /// Логика взаимодействия для MonitorChartCC.xaml
    /// </summary>
    public partial class MonitorChartCC : Window
    {
        private ChartCCViewModel model;

        public MonitorChartCC(ObservableCollection<ViewResult> MonthUseCCList)
        {
            model = new ChartCCViewModel(MonthUseCCList);
            this.DataContext = model;
            InitializeComponent();
        }

    }
}
