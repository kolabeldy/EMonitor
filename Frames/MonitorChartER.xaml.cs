using EMonitor.DB;
using LiveCharts;
using System.Collections.ObjectModel;
using System.Windows;

namespace EMonitor.Frames
{
    /// <summary>
    /// Логика взаимодействия для MonitorChartER.xaml
    /// </summary>
    public partial class MonitorChartER : Window
    {
        private ChartERViewModel model;

        public MonitorChartER(ObservableCollection<ViewResult> MonthUseERList)
        {
            model = new ChartERViewModel(MonthUseERList);
            this.DataContext = model;
            InitializeComponent();
        }

    }
}
