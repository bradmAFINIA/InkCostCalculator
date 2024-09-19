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

namespace InkCostCalculatorL501EnhancedWPF
{
    /// <summary>
    /// Interaction logic for ConfigurationPage.xaml
    /// </summary>
    public partial class ConfigurationPage : Window
    {
        public string ColorCost = string.Empty;
        public string ColorMl = string.Empty;
        public string BlackCost = string.Empty;
        public string BlackMl = string.Empty;
        public ConfigurationPage(string colorCost, string colorMl, string blackCost, string blackMl)
        {
            InitializeComponent();
            ColorCostTextBox.Text = ColorCost = colorCost;
            BlackCostTextBox.Text = BlackCost = blackCost;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ColorCost = ColorCostTextBox.Text;
            BlackCost = BlackCostTextBox.Text;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
