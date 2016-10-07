using System.Windows;

namespace MemoryLeakExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SecondWindow : Window
    {
        public SecondWindow()
        {
            InitializeComponent();
        }

        public void CurrencyChanged(string newCurrency)
        {
            // Change the view to reflect the change here
        }
    }
}
