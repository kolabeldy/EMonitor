using System.Windows;
using System.Windows.Controls;

namespace EMonitor.Frames
{
    public partial class Monitor : Page
    {
        private MonitorModel model;

        public Monitor()
        {
            model = new MonitorModel();
            this.DataContext = model;
            InitializeComponent();
        }

    }
}
