using EMonitor.Frames;
using System.Windows;
using System.Windows.Controls;

namespace EMonitor
{
    public partial class MainWindow : Window
    {
        private MainWindowModel model;
        public MainWindow()
        {
            model = new MainWindowModel();
            this.DataContext = model;
            var sWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            var sHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            InitializeComponent();
            if (sWidth < 1300) mainWin.WindowState = WindowState.Maximized;
            BtnMonitor.IsChecked = true;
        }

        static public void ToolBarLoaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            if (toolBar.Template.FindName("OverflowGrid", toolBar) is FrameworkElement overflowGrid)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            if (toolBar.Template.FindName("MainPanelBorder", toolBar) is FrameworkElement mainPanelBorder)
            {
                mainPanelBorder.Margin = new Thickness();
            }

        }
        private void BtnCostCenters_Checked(object sender, RoutedEventArgs e)
        {
            frameReports.Content = null;
            frameReports.Content = new CostCentres();
        }

        private void BtnEnergyResource_Checked(object sender, RoutedEventArgs e)
        {
            frameReports.Content = null;
            frameReports.Content = new ER();
        }

        private void BtnTrends_Checked(object sender, RoutedEventArgs e)
        {
            frameReports.Content = null;
            frameReports.Content = new Trend();
        }

        private void BtnFavorit_Checked(object sender, RoutedEventArgs e)
        {
            frameReports.Content = null;
            frameReports.Content = new Favorit();
        }

        private void BtnDevelopmentUse_Checked(object sender, RoutedEventArgs e)
        {
            frameReports.Content = null;
            frameReports.Content = new FabricateUse();
        }

        private void BtnLoadToDB_Checked(object sender, RoutedEventArgs e)
        {
            frameReports.Content = null;
            //frameReports.Content = new LoadMonthToDB();
        }

        private void BtnLoadPeriodToDB_Checked(object sender, RoutedEventArgs e)
        {
            frameReports.Content = null;
            //frameReports.Content = new LoadPeriodToDB();
        }

        private void BtnSpravER_Checked(object sender, RoutedEventArgs e)
        {
            frameReports.Content = null;
        }

        private void BtnSpravTariff_Checked(object sender, RoutedEventArgs e)
        {
            frameReports.Content = null;
        }

        private void BtnSpravDepart_Checked(object sender, RoutedEventArgs e)
        {
            frameReports.Content = null;
        }

        private void BtnSpravCostCenter_Checked(object sender, RoutedEventArgs e)
        {
            frameReports.Content = null;
        }

        private void BtnSpravNormResource_Checked(object sender, RoutedEventArgs e)
        {
            frameReports.Content = null;
        }

        private void BtnMonitor_Checked(object sender, RoutedEventArgs e)
        {
            frameReports.Content = null;
            frameReports.Content = new Monitor();
        }

        private void BtnLosses_Checked(object sender, RoutedEventArgs e)
        {
            frameReports.Content = null;
            frameReports.Content = new Losses();
        }

        private void BtnLossesStruct_Checked(object sender, RoutedEventArgs e)
        {
            frameReports.Content = null;
            frameReports.Content = new LossesStruct();
        }
    }
}
