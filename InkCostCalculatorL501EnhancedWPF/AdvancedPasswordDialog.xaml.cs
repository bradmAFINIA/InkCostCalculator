using System.Windows;

namespace InkCostCalculatorL501EnhancedWPF
{
    /// <summary>
    /// Interaction logic for AdvancedPasswordDialog.xaml
    /// </summary>
    public partial class AdvancedPasswordDialog : Window
    {
        private const string PASSWORD = "8150";

        public AdvancedPasswordDialog()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (PASSWORD.Equals(AdvancedPasswordBox.Password))
            {
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Incorrect password.", "Access denied", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
