using System.Windows.Controls;

namespace EMonitor.Frames
{
    /// <summary>
    /// Логика взаимодействия для FabricateUse.xaml
    /// </summary>
    public partial class FabricateUse : Page
    {
        private FabricateUseModel model;

        public FabricateUse()
        {
            model = new FabricateUseModel();
            this.DataContext = model;
            InitializeComponent();
        }
    }
}
