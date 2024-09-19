using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InkCostCalculator
{
    public partial class PrinterConfig : Form
    {
        BindingList<string> comboSource = new BindingList<string>();

        public string SelectedPrinter = string.Empty;
        public string PathOutputProductUseageDyn = string.Empty;
        public string PathOutputConsumableConfigDyn = string.Empty;
        public string PathHttpResponse = string.Empty;
        public string PathSWFWClient = string.Empty;
        public string PathToOutput = string.Empty;

        public PrinterConfig(string selectedPrinter,
            string pathToOutput)
        {
            InitializeComponent();

            SelectedPrinter = selectedPrinter;
            PathToOutputTextBox.Text = PathToOutput = pathToOutput;

            comboSource.Add("Please Select Printer");
            List<string> printers = GetPrinters();
            foreach (string printer in printers)
            {
                comboSource.Add(printer);
            }
            PrintersComboBox.DataSource = comboSource;

            int index = PrintersComboBox.FindString(SelectedPrinter);
            if (index > 0)
            {
                PrintersComboBox.SelectedIndex = index;
            }
            else
            {
                PrintersComboBox.SelectedIndex = 0;
            }
        }
        private List<string> GetPrinters()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
            var results = searcher.Get();
            List<string> printers = new List<string>();
            foreach (var printer in results)
            {
                if (printer.Properties["DriverName"].Value.ToString() == "Afinia Label L301 Label Printer")
                {
                    printers.Add(printer.Properties["Name"].Value.ToString());
                }
            }
            return printers;
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            SelectedPrinter = PrintersComboBox.SelectedValue.ToString();
            PathToOutput = PathToOutputTextBox.Text;
        }

        private void CancelDialogButton_Click(object sender, EventArgs e)
        {

        }
    }
}
