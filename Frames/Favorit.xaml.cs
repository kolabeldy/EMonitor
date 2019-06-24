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
    /// Логика взаимодействия для Favorit.xaml
    /// </summary>
    public partial class Favorit : Page
    {
        private FavoritModel model;

        public Favorit()
        {
            model = new FavoritModel();
            this.DataContext = model;
            InitializeComponent();
        }
    }
}
