using System.Windows;
using System.Windows.Controls;

namespace EMonitor.Frames
{
    public partial class CostCentres : Page
    {
        private CostCentersModel model;

        public CostCentres()
        {
            model = new CostCentersModel();
            this.DataContext = model;
            InitializeComponent();
        }
    }
}
