using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using InkCostModel;
using System.Drawing.Printing;
using System.Drawing;

namespace InkCostCalculatorL501EnhancedWPF
{
    /// <summary>
    /// Interaction logic for InkCostPage.xaml
    /// </summary>
    public partial class InkCostPage : Page
    {
        InkCostController inkCostController;
        string selectedPrinter = string.Empty;
        public InkCostPage(InkCostController _inkCostController)
        {
            InitializeComponent();
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
                selectedPrinter = config.AppSettings.Settings["SelectedPrinter"].Value;
                inkCostController = _inkCostController;
                this.DataContext = inkCostController.viewModel;
                this.Height = 320;
                //MainWindow.HeightProperty = 309;
                InkCostController.logger.Debug("InkCostPage Initialized");
            }
            catch(Exception ex)
            {
                InkCostController.logger.Debug("InkCostPage");
                InkCostController.logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        private void ExportJobListButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (StreamWriter file = new StreamWriter($"{inkCostController.outputFilePath}jobs_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.csv", false))
                {
                    file.WriteLine(Properties.Settings.Default.JobFileHeader);
                    string totalCost = string.Empty;
                    string costPerLabel = string.Empty;
                    foreach (Cost job in inkCostController.jobs)
                    {
                        totalCost = (job.TotalCostOfCyan + job.TotalCostOfMagenta + job.TotalCostOfYellow + job.TotalCostOfBlack).ToString("0.#####");
                        costPerLabel = ((job.TotalCostOfCyan + job.TotalCostOfMagenta + job.TotalCostOfYellow + job.TotalCostOfBlack) / job.TotalColorLabels).ToString("0.#####");

                        file.WriteLine($"{job.Printer},{job.PrintJob},{job.TotalColorLabels},{job.LabelCostOfCyan.ToString("0.#####")},{job.TotalCostOfCyan.ToString("0.#####")},{job.TotalCyanMlUsed.ToString("0.#####")},{job.CyanPagesPerCartridge.ToString("0.#")},{job.LabelCostOfMagenta.ToString("0.#####")},{job.TotalCostOfMagenta.ToString("0.#####")},{job.TotalMagentaMlUsed.ToString("0.#####")},{job.MagentaPagesPerCartridge.ToString("0.#")},{job.LabelCostOfYellow.ToString("0.#####")},{job.TotalCostOfYellow.ToString("0.#####")},{job.TotalYellowMlUsed.ToString("0.#####")},{job.YellowPagesPerCartridge.ToString("0.#")},{job.LabelCostOfBlack.ToString("0.#####")},{job.TotalCostOfBlack.ToString("0.#####")},{job.TotalBlackMlUsed.ToString("0.#####")},{job.BlackPagesPerCartridge.ToString("0.#")},{ totalCost},{ costPerLabel}");
                    }
                    file.Close();
                }
            }
            catch (Exception ex)
            {
                InkCostController.logger.Debug("ExportJobList");
                InkCostController.logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }
        private void ShowHideAdvancedButton_Click(object sender, RoutedEventArgs e)
        {

            if (inkCostController.viewModel.ShowHideAdvancedButton.Contains("Show"))
            {
                inkCostController.viewModel.ShowHideAdvancedButton = "Hide Advanced Info";
                inkCostController.viewModel.JobsListBoxLabelVis = Visibility.Visible;
                inkCostController.viewModel.JobsListBoxVis = Visibility.Visible;
                inkCostController.viewModel.RawLabelVis = Visibility.Visible;
                inkCostController.viewModel.RawListBoxVis = Visibility.Visible;
                inkCostController.viewModel.Width = 1001;
                inkCostController.viewModel.Height = 775;
                this.Height = 775;
            }
            else if (inkCostController.viewModel.ShowHideAdvancedButton.Contains("Hide"))
            {
                inkCostController.viewModel.ShowHideAdvancedButton = "Show Advanced Info";
                inkCostController.viewModel.JobsListBoxLabelVis = Visibility.Hidden;
                inkCostController.viewModel.JobsListBoxVis = Visibility.Hidden;
                inkCostController.viewModel.RawLabelVis = Visibility.Hidden;
                inkCostController.viewModel.RawListBoxVis = Visibility.Hidden;
                inkCostController.viewModel.Width = 1001;
                inkCostController.viewModel.Height = 325;
                this.Height = 325;
            }
        }
        private void endCaptureToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            inkCostController.CaptureEndEnableToolStrip(false);

            inkCostController.InkCapture(inkCostController.manualName, true);
            inkCostController.InkCalculate(inkCostController.manualName, inkCostController.manualJobId);
        }
        private void PrinterToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //PrinterConfig printerConfiguration = new PrinterConfig($"{inkCostController.selectedPrinter}",
            //                                             $"{inkCostController.outputFilePath}");
            PrinterConfigPage printerConfiguration = new PrinterConfigPage($"{inkCostController.selectedPrinter}",
                                             $"{inkCostController.outputFilePath}");
            //printerConfiguration.ShowDialog();
            //if (printerConfiguration.DialogResult.HasValue && printerConfiguration.DialogResult.Value)
            //if (printerConfiguration.ShowDialog() == printerConfiguration.DialogResult.HasValue && printerConfiguration.DialogResult.Value)
            if (printerConfiguration.ShowDialog() == true)
            {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
                config.AppSettings.Settings.Remove("SelectedPrinter");
                config.AppSettings.Settings.Add("SelectedPrinter", printerConfiguration.selectedPrinter);
                config.AppSettings.Settings.Remove("OutputFilePath");
                config.AppSettings.Settings.Add("OutputFilePath", printerConfiguration.pathToOutput);
                config.Save(ConfigurationSaveMode.Modified);
                inkCostController.selectedPrinter = config.AppSettings.Settings["SelectedPrinter"].Value;
                inkCostController.outputFilePath = config.AppSettings.Settings["OutputFilePath"].Value;
                inkCostController.viewModel.SelectedPrinterName = inkCostController.selectedPrinter;
                inkCostController.costCalculatorViewModel.SelectedPrinterName = inkCostController.selectedPrinter;
                inkCostController.selectedIp = inkCostController.GetPrinterIp(inkCostController.selectedPrinter);
                inkCostController.changePrinter = true;
                inkCostController.StartWatcher(config.AppSettings.Settings["SelectedPrinter"].Value.Trim());
            }
        }
        private void CostToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            double colorCartridgeCost = 0;// Properties.Settings.Default.ColorCartridgeCost;
            double.TryParse(inkCostController.viewModel.ColorInkCartridgeCost, out colorCartridgeCost);
            double colorMl = Properties.Settings.Default.Color_ml;
            double blackCartridgeCost = 0;// Properties.Settings.Default.ColorCartridgeCost;
            double.TryParse(inkCostController.viewModel.BlackInkCartridgeCost, out blackCartridgeCost);
            double blackMl = Properties.Settings.Default.Black_ml;

            //ConfigurationDialog dialog = new ConfigurationDialog(colorCartridgeCost.ToString(),
            //                                                        Properties.Settings.Default.Color_ml.ToString(),
            //                                                        blackCartridgeCost.ToString(),
            //                                                        Properties.Settings.Default.Black_ml.ToString());
            ConfigurationPage dialog = new ConfigurationPage(colorCartridgeCost.ToString(),
                                                        Properties.Settings.Default.Color_ml.ToString(),
                                                        blackCartridgeCost.ToString(),
                                                        Properties.Settings.Default.Black_ml.ToString());
            //dialog.ShowDialog();
            //if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            //if (dialog.ShowDialog() ==  dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            if (dialog.ShowDialog() == true)
            {
                double.TryParse(dialog.ColorCost, out colorCartridgeCost);
                double.TryParse(dialog.ColorMl, out colorMl);
                double.TryParse(dialog.BlackCost, out blackCartridgeCost);
                double.TryParse(dialog.BlackMl, out blackMl);

                Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
                config.AppSettings.Settings.Remove("ColorCartridgeCost");
                config.AppSettings.Settings.Add("ColorCartridgeCost", colorCartridgeCost.ToString());
                config.AppSettings.Settings.Remove("BlackCartridgeCost");
                config.AppSettings.Settings.Add("BlackCartridgeCost", blackCartridgeCost.ToString());
                config.Save(ConfigurationSaveMode.Modified);
                inkCostController.viewModel.ColorInkCartridgeCost = config.AppSettings.Settings["ColorCartridgeCost"].Value;
                inkCostController.viewModel.BlackInkCartridgeCost = config.AppSettings.Settings["BlackCartridgeCost"].Value;
            }
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            inkCostController.viewModel.SetNS(NavigationService.GetNavigationService(this));
            RawListBox.ItemsSource = inkCostController.viewModel.RawList;
            JobsListBox.ItemsSource = inkCostController.viewModel.JobList;
        }

        private void PrintAlignment_Click(object sender, RoutedEventArgs e)
        {
            string filename = Properties.Settings.Default.AlignmentImage;

            using (var pd = new System.Drawing.Printing.PrintDocument())
            {
                try
                {
                    pd.PrinterSettings.PrinterName = selectedPrinter;

                    pd.PrintPage += (_, o) =>
                    {
                        var img = System.Drawing.Image.FromFile(filename);

                        // This uses a 50 pixel margin - adjust as needed
                        o.Graphics.DrawImage(img, new System.Drawing.Point(50, 50));
                    };
                    pd.Print();
                }
                catch (Exception ex)
                {
                    InkCostController.logger.Debug("PrintAlignment");
                    InkCostController.logger.Error($"{ex.Message}. {ex.InnerException}");
                }
            }
        }
        //public void Print(string PrinterName)
        //{
        //    PrintDocument doc = new PrintDocument();
        //    doc.PrinterSettings.PrinterName = PrinterName;
        //    doc.PrintPage += new PrintPageEventHandler(PrintHandler);
        //    doc.Print();
        //}

        //private void PrintHandler(object sender, PrintPageEventArgs e)
        //{
        //    System.Drawing.Image img = System.Drawing.Image.FromFile("D:\\Foto.jpg");
        //    System.Drawing.Point loc = new System.Drawing.Point(50, 50);
        //    e.Graphics.DrawImage(img, loc);
        //}
    }
}
