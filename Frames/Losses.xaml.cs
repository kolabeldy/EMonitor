using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EMonitor.Frames
{
    /// <summary>
    /// Логика взаимодействия для ER.xaml
    /// </summary>
    public partial class Losses : Page
    {
        private LossesModel model;
        public Losses()
        {
            model = new LossesModel();
            this.DataContext = model;
            InitializeComponent();
        }
    }
}
