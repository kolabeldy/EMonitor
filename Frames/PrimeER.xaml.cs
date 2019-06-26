using System.Windows;
using System.Windows.Controls;

namespace EMonitor.Frames
{
    public partial class PrimeER : Page
    {
        private PrimeERModel model;

        public PrimeER()
        {
            model = new PrimeERModel();
            this.DataContext = model;
            InitializeComponent();
        }
    }
}
