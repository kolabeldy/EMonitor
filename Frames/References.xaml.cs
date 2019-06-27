using System.Windows;
using System.Windows.Controls;

namespace EMonitor.Frames
{
    public partial class References : Page
    {
        private ReferencesModel model;

        public References()
        {
            model = new ReferencesModel();
            this.DataContext = model;
            InitializeComponent();
        }
    }
}
