using System.Windows.Controls;

namespace EMonitor.Frames
{
    /// <summary>
    /// Логика взаимодействия для Trend.xaml
    /// </summary>
    public partial class Trend : Page
    {
        private TrendModel model;
        public Trend()
        {
            model = new TrendModel();
            this.DataContext = model;
            InitializeComponent();
        }
    }
}
