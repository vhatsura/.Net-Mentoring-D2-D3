using System.Windows;

namespace MemoryLeakExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new SecondWindow();
            dlg.Show();
            Settings.OnCurrencyChanged += dlg.CurrencyChanged;  //Comment this for checking memory leak

            OpenButton.Visibility = Visibility.Collapsed;
            CheckButton.Visibility = Visibility.Visible;

            MemoryLeak.CheckObject(dlg);
        }

        private void CheckButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this, MemoryLeak.IsItDead() ? "Still here" : "I 'm dead");
        }
    }
}
