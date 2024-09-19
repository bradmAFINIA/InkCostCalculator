using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Management;


namespace InkCostCalculatorL501EnhancedWPF
{
    /// <summary>
    /// Interaction logic for PrinterConfigPage.xaml
    /// </summary>
    public partial class PrinterConfigPage : Window
    {
        public string selectedPrinter = string.Empty;
        public string pathToOutput = string.Empty;
        PrinterConfigViewModel printerConfigView;// = new PrinterConfigViewModel(selectedPrinter);

        public PrinterConfigPage(string _selectedPrinter, string _pathToOutput)
        {
            InitializeComponent();

            List<string> printers = GetPrinters();

            printerConfigView = new PrinterConfigViewModel(_selectedPrinter, printers);
            this.DataContext = printerConfigView;

            printerConfigView.Name = selectedPrinter = _selectedPrinter;
            printerConfigView.PathToOutput = pathToOutput = _pathToOutput;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            selectedPrinter = PrintersComboBox.SelectedValue.ToString();
            pathToOutput = PathToOutputTextBox.Text;
            DialogResult = true;
        }

        private void CancelDialogButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private List<string> GetPrinters()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
            var results = searcher.Get();
            List<string> printers = new List<string>();
            foreach (var printer in results)
            {
                //if ((printer.Properties["DriverName"].Value.ToString() == "Afinia Label L501 Label Printer") || (printer.Properties["DriverName"].Value.ToString() == "Afinia Label 502 Label Printer"))
                //{
                    printers.Add(printer.Properties["Name"].Value.ToString());
            //}
        }
            return printers;
        }
    }
}
