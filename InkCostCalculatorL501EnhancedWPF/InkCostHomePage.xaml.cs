using InkCostModel;
using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;
//using PrinterCommandExecute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
//using System.Drawing.Printing;
using System.IO;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InkCostCalculatorL501EnhancedWPF
{
    /// <summary>
    /// Interaction logic for InkCostHomePage.xaml
    /// </summary>
    public partial class InkCostHomePage : Page
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public static Logger serialLogger = LogManager.GetLogger("serialLog");
        InkCostController inkCostController;
        string selectedPrinter = string.Empty;

        uint feedLength = 9600;
        double inchValue = 0;
        double mmValue = 0;
        string outputFilePath = string.Empty;
        double printedLength = 0;
        uint length = 0;

        AfiniaSerial ser;
        bool serialToLog = false;
        System.Windows.Forms.Timer PrinterFirmwareUpdateTimer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer PrintedLengthTimer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer FirmwareUpdateTimer = new System.Windows.Forms.Timer();

        public InkCostHomePage(InkCostController _inkCostController)
        {
            InitializeComponent();
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
                selectedPrinter = config.AppSettings.Settings["SelectedPrinter"].Value;
                inkCostController = _inkCostController;

                inkCostListView.ItemsSource = inkCostController.costCalculatorViewModel.Data;
                this.DataContext = inkCostController.costCalculatorViewModel;
                //this.Height = 320;
                //MainWindow.HeightProperty = 309;
                initiManualOperationsPage();
                inkCostController.OnPrintRunningStatusChange += InkCostController_OnPrintRunningStatusChange;
                InkCostController.logger.Debug("CostCalculatorPage Initialized");
            }
            catch (Exception ex)
            {
                InkCostController.logger.Debug("CostCalculatorPage");
                InkCostController.logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        private void InkCostController_OnPrintRunningStatusChange(object Sender, bool e)
        {
            if (e)
            {
                CutButton.Dispatcher.BeginInvoke(new Action(() => CutButton.IsEnabled = false));
                FeedButton.Dispatcher.BeginInvoke(new Action(() => FeedButton.IsEnabled = false));
            }
            else
            {
                CutButton.Dispatcher.BeginInvoke(new Action(() => CutButton.IsEnabled = true));
                FeedButton.Dispatcher.BeginInvoke(new Action(() => FeedButton.IsEnabled = true));
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            tabControl.SelectionChanged += OnSelectedIndexChanged;
            ser = new AfiniaSerial();
            //.RawResponseHandler += RawResponse;
        }
        private void RawResponse(string response)
        {
            //.logger.Debug(response);
            //(response.Contains("A F I N I A"))
            //
            //TurnDebugOn("");
            //}
        }

        static bool advancedTabWarningShown = false;
        TabItem lastTab;
        private void OnSelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedItem == PrinterUtilitiesTab)
            {
                SetTotalInchesPrintedTextBlock();
                PrintedLengthTimer.Tick += PrintedLengthTimer_Tick;
                PrintedLengthTimer.Interval = 10 * 60 * 1000;
                PrintedLengthTimer.Start();
            }
            else
            {
                PrintedLengthTimer.Stop();
            }

            if (tabControl.SelectedItem == ControllerUtilitiesTab)
            {
                if (COMPortChooser.Items.Count == 0)
                {
                    ser.FindAfiniaPorts(AddPortName, true);
                }

                if (COMPortChooser.Items.Count > 0)
                {
                    RefreshButton.Dispatcher.BeginInvoke(new Action(() => RefreshButton.Visibility = Visibility.Hidden));
                }
                else
                {
                    RefreshButton.Dispatcher.BeginInvoke(new Action(() => RefreshButton.Visibility = Visibility.Visible));
                }
            }

            if (tabControl.SelectedItem == AdvancedTab)
            {
                if (!advancedTabWarningShown)
                {
                    AdvancedPasswordDialog dialog = new AdvancedPasswordDialog();
                    dialog.ShowDialog();
                    if ((bool)dialog.DialogResult)
                    {
                        advancedTabWarningShown = true;
                        MessageBox.Show("Do not change any advanced settings unless directed by Technical Support", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                        // For some reason, on the second or so advanced tab
                        // activation, we're getting a secondary event to
                        // switch back to the previous tab. This should hide
                        // the effects of that event. We need to turn off the
                        // handler because otherwise we'll get into an infinite
                        // loop.
                        tabControl.SelectionChanged -= OnSelectedIndexChanged;
                        tabControl.SelectedItem = AdvancedTab;
                        tabControl.SelectionChanged += OnSelectedIndexChanged;
                    }
                    else
                    {
                        tabControl.SelectedItem = lastTab;
                    }
                }
            }
            else
            {
                lastTab = (TabItem)tabControl.SelectedItem;
            }
        }

        private void PrintedLengthTimer_Tick(object sender, EventArgs e)
        {
            SetTotalInchesPrintedTextBlock();
        }

        private void AddPortName(string port)
        {
            if (port != null)
            {
                COMPortChooser.Dispatcher.BeginInvoke(new Action(() => COMPortChooser.Items.Add(port)));
                COMPortChooser.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (COMPortChooser.Items.Count == 1)
                    {
                        COMPortChooser.SelectedIndex = 0;
                    }
                    else
                    {
                        COMPortChooser.SelectedIndex = -1;
                    }
                }));
            }
        }
        public string firmwareVersion = string.Empty;
        private void SetFirmwareVersionTextBox(string data)
        {
            firmwareVersion = ser.versionData;
            FirmwareVersionTextBox.Dispatcher.BeginInvoke(new Action(() => FirmwareVersionTextBox.Text = data.Substring(data.LastIndexOf('v'))));
            ser.ResponseHandler = null;
        }

        private void ExportJobListButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var path = inkCostController.outputFilePath;
                System.Windows.Forms.FolderBrowserDialog folderBrowse = new System.Windows.Forms.FolderBrowserDialog();
                folderBrowse.RootFolder = Environment.SpecialFolder.MyComputer;
                folderBrowse.SelectedPath = path;

                if (folderBrowse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    path = $"{folderBrowse.SelectedPath}\\";

                    using (StreamWriter file = new StreamWriter($"{path}jobs_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.csv", false))
                    //using (StreamWriter file = new StreamWriter($"{inkCostController.outputFilePath}jobs_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.csv", false))
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
            }
            catch (Exception ex)
            {
                InkCostController.logger.Debug("ExportJobList");
                InkCostController.logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        private void endCaptureToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            inkCostController.CaptureEndEnableToolStrip(false);

            inkCostController.InkCapture(inkCostController.manualName, true);
            inkCostController.InkCalculate(inkCostController.manualName, inkCostController.manualJobId);
            CutButton.Dispatcher.BeginInvoke(new Action(() => CutButton.IsEnabled = true));
            FeedButton.Dispatcher.BeginInvoke(new Action(() => FeedButton.IsEnabled = true));
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
                selectedPrinter = inkCostController.selectedPrinter;
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
                inkCostController.costCalculatorViewModel.ColorCost = config.AppSettings.Settings["ColorCartridgeCost"].Value;
                inkCostController.costCalculatorViewModel.BlackCost = config.AppSettings.Settings["BlackCartridgeCost"].Value;
            }
        }

        private void PrintAlignment_Click(object sender, RoutedEventArgs e)
        {
            string filename = Properties.Settings.Default.AlignmentImage;
            string name = Path.GetFileName(filename);
            var ext = Path.GetExtension(filename);
            if (ext.ToLower().Contains("jpg"))
            {
                System.Windows.Forms.PrintDialog pDialog = new System.Windows.Forms.PrintDialog();
                
                using (var pd = new System.Drawing.Printing.PrintDocument())
                {
                    try
                    {

                        pd.PrinterSettings.PrinterName = selectedPrinter;
                        pd.DocumentName = name;
                        pd.PrintPage += (_, o) =>
                        {
                            var img = System.Drawing.Image.FromFile(filename);
                            o.Graphics.DrawImage(img, new System.Drawing.Point(0, 0));
                        };
                        pDialog.PrinterSettings.PrinterName = selectedPrinter;
                        pDialog.Document = pd;
                        if (pDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            pd.Print();
                        }
                    }
                    catch (Exception ex)
                    {
                        InkCostController.logger.Debug("PrintAlignment");
                        InkCostController.logger.Error($"{ex.Message}. {ex.InnerException}");
                    }
                }

                //PrintDocument pd = new PrintDocument();
                //pd.PrintPage += PrintPage;
                //pd.Print();
            }
            else 
            {
                InkCostController.logger.Debug("PrintAlignment: Invalid file type, must use jpg files");
            }
        }
        //private void PrintPage(object o, PrintPageEventArgs e)
        //{
        //    string filename = Properties.Settings.Default.AlignmentImage;
        //    System.Drawing.Image img = System.Drawing.Image.FromFile(filename);
        //    System.Drawing.Point loc = new System.Drawing.Point(100, 100);
        //    e.Graphics.DrawImage(img, loc);
        //}

        public void initiManualOperationsPage()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
                //selectedPrinter = config.AppSettings.Settings["SelectedPrinter"].Value;
                outputFilePath = config.AppSettings.Settings["OutputFilePath"].Value;
                Inches.IsChecked = true;

                FeedLengthSpinner.Value = ConvertToInches(feedLength);
                inchValue = FeedLengthSpinner.Value;
                mmValue = Math.Round(inchValue * 25.4, 2);
                TotalInchesPrintedTextBlock.Text = "0.000";

                PrinterCommandExecute.Command.GetMech1(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);
                bool pauseState = PrinterCommandExecute.Command.ReadPauseState(outputFilePath + "mech1.xml");
                Thread.Sleep(100);
                SetPauseResumeButtonColor(pauseState);

                using (File.Create(outputFilePath + "empty.xml")) ;
                logger.Debug("ManualOperationsPage Initialized");
            }
            catch (Exception ex)
            {
                logger.Debug("ManualOperationsPage");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrinterCommandExecute.Command.Cut(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);
            }
            catch (Exception ex)
            {
                logger.Debug("CutButton");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        private void FeedButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double length = FeedLengthSpinner.Value;
                if (Inches.IsChecked == true)
                {
                    PrinterCommandExecute.Command.Feed(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath, ConvertFromInches(inchValue));
                }
                else
                {
                    PrinterCommandExecute.Command.Feed(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath, ConvertFromMM(mmValue));
                }
            }
            catch (Exception ex)
            {
                logger.Debug("FeedButton");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        private void FeedLengthScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Inches.IsChecked == true)
            {
                inchValue = Math.Round(FeedLengthSpinner.Value,2);
                mmValue = Math.Round(inchValue * 25.4, 2);
                FeedLengthSpinner.Value = inchValue;
                FeedLength.Text = FeedLengthSpinner.Value.ToString();
            }
            else
            {
                mmValue = Math.Round(FeedLengthSpinner.Value, 2);
                inchValue = mmValue / 25.4;
                FeedLengthSpinner.Value = mmValue;
                FeedLength.Text = FeedLengthSpinner.Value.ToString();
            }
        }

        private void FeedLength_KeyDown(object sender, KeyEventArgs e)
        {
            //if (!char.IsControl((char)e.Key) && !char.IsDigit((char)e.Key) && (e.Key != Key.Decimal))
            //{
            //    e.Handled = true;
            //}

            //// only allow one decimal point
            //if (((char)e.Key == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //{
            //    e.Handled = true;
            //}

            if(e.Key == Key.Enter)
            {
                if (Inches.IsChecked == true)
                {
                    inchValue = double.Parse(FeedLength.Text);
                    inchValue = Math.Round(inchValue, 2);
                    mmValue = Math.Round(inchValue * 25.4, 2);
                    FeedLengthSpinner.Value = inchValue;
                    FeedLength.Text = FeedLengthSpinner.Value.ToString();
                }
                else
                {
                    mmValue = double.Parse(FeedLength.Text);
                    mmValue = Math.Round(FeedLengthSpinner.Value, 2);
                    inchValue = mmValue / 25.4;
                    FeedLengthSpinner.Value = mmValue;
                    FeedLength.Text = FeedLengthSpinner.Value.ToString();
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //var f = (TextBox)FeedLengthSpinner.Template.FindName("spin", FeedLengthSpinner);
            //var g = f.Text;

            //if (Inches.IsChecked == true)
            //{
            //    //double.TryParse(g, out inchValue);
            //    inchValue = Math.Round(inchValue, 2);
            //    mmValue = Math.Round(inchValue * 25.4, 2);
            //    FeedLengthSpinner.Value = inchValue;
            //    //f.Text = FeedLengthSpinner.Value.ToString();
            //}
            //else
            //{
            //    mmValue = Math.Round(FeedLengthSpinner.Value, 2);
            //    inchValue = mmValue / 25.4;
            //    FeedLengthSpinner.Value = mmValue;
            //    //f.Text = FeedLengthSpinner.Value.ToString();
            //}
        }
        private void FeedLengthSpinner_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (Inches.IsChecked == true)
            {
                inchValue = Math.Round(FeedLengthSpinner.Value, 2);
                mmValue = Math.Round(inchValue * 25.4, 2);
                FeedLengthSpinner.Value = inchValue;
            }
            else
            {
                mmValue = Math.Round(FeedLengthSpinner.Value, 2);
                inchValue = mmValue / 25.4;
                FeedLengthSpinner.Value = mmValue;
            }
        }

        private void Inches_Checked(object sender, RoutedEventArgs e)
        {
            if (Inches.IsChecked == true)
            {
                printedLength = ConvertToInches(length);
                TotalInchesPrintedTextBlock.Text = printedLength.ToString("#.000");

                FeedLengthSpinner.Value = mmValue / 25.4;
                FeedLengthSpinner.Maximum = 12;
                FeedLengthSpinner.LargeChange = .1;
                FeedLengthSpinner.SmallChange = .1;
            }
        }

        private void MM_Checked(object sender, RoutedEventArgs e)
        {
            if (MM.IsChecked == true)
            {
                FeedLengthSpinner.Maximum = 304.8;
                FeedLengthSpinner.LargeChange = 2.54;
                FeedLengthSpinner.SmallChange = 2.54;
                FeedLengthSpinner.Value = inchValue * 25.4;

                printedLength = ConvertToMM(length);
                TotalInchesPrintedTextBlock.Text = printedLength.ToString("#.000");
            }
        }

        private double ConvertToInches(uint value)
        {
            return (double)((double)value / 2400.0);
        }

        private double ConvertToMM(uint value)
        {
            return (double)(((double)value / 2400.0) * 25.4);
        }

        private uint ConvertFromInches(double value)
        {
            return (uint)(value * 2400.0);
        }

        private uint ConvertFromMM(double value)
        {
            return (uint)((value * 2400.0) / 25.4);
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrinterCommandExecute.Command.GetMech1(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);
                bool pauseState = PrinterCommandExecute.Command.ReadPauseState(outputFilePath + "mech1.xml");
                logger.Debug($"PauseButton: PreExecute state = {pauseState}");

                PrinterCommandExecute.Command.Pause(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);

                Thread.Sleep(100);
                pauseState = PrinterCommandExecute.Command.ReadPauseState(outputFilePath + "mech1.xml");
                logger.Debug($"PauseButton: PostExecute Delayed state = {pauseState}");
                SetPauseResumeButtonColor(pauseState);
            }
            catch (Exception ex)
            {
                logger.Debug("PauseButton");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrinterCommandExecute.Command.GetMech1(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);
                bool pauseState = PrinterCommandExecute.Command.ReadPauseState(outputFilePath + "mech1.xml");
                logger.Debug($"ResumeButton: PreExecute state = {pauseState}");

                PrinterCommandExecute.Command.UnPause(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);

                Thread.Sleep(100);
                pauseState = PrinterCommandExecute.Command.ReadPauseState(outputFilePath + "mech1.xml");
                logger.Debug($"ResumeButton: PostExecute Delayed state = {pauseState}");
                SetPauseResumeButtonColor(pauseState);
            }
            catch (Exception ex)
            {
                logger.Debug("ResumeButton");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        private void SetPauseResumeButtonColor(bool pauseState)
        {
            var converter = new System.Windows.Media.BrushConverter();
            if (pauseState == true)
            {
                ResumeButton.Background = System.Windows.Media.Brushes.LightGreen;
                PauseButton.Background = (Brush)converter.ConvertFromString("#FFDDDDDD");
            }
            else
            {
                PauseButton.Background = System.Windows.Media.Brushes.LightGreen;
                ResumeButton.Background = (Brush)converter.ConvertFromString("#FFDDDDDD");
            }
        }

        private void SetTotalInchesPrintedTextBlock()
        {
            try
            {
                PrinterCommandExecute.Command.PrintedLength(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);

                length = PrinterCommandExecute.Command.ReadFeedLength($"{outputFilePath}mech1.xml");

                if (Inches.IsChecked == true)
                {
                    printedLength = ConvertToInches(length);
                }
                else
                {
                    printedLength = ConvertToMM(length);
                }

                TotalInchesPrintedTextBlock.Text = printedLength.ToString("#.000");
                //TotalLabelsPrintedTextBlock.Text = 
            }
            catch (Exception ex)
            {
                logger.Debug("TotalInchesPrinted");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        private void TotalInchesPrintedButton_Click(object sender, RoutedEventArgs e)
        {
            SetTotalInchesPrintedTextBlock();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
            config.AppSettings.Settings.Remove("FeedLengthInches");
            config.AppSettings.Settings.Add("FeedLengthInches", inchValue.ToString());
            config.Save(ConfigurationSaveMode.Modified);
        }

        private void BlackCostTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void BlackCostTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                double blackCartridgeCost = 0;// Properties.Settings.Default.ColorCartridgeCost;

                inkCostController.viewModel.BlackInkCartridgeCost = BlackCostTextBox.Text;
                inkCostController.costCalculatorViewModel.BlackCost = BlackCostTextBox.Text;
                double.TryParse(BlackCostTextBox.Text, out blackCartridgeCost);

                Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
                config.AppSettings.Settings.Remove("BlackCartridgeCost");
                config.AppSettings.Settings.Add("BlackCartridgeCost", blackCartridgeCost.ToString());
                config.Save(ConfigurationSaveMode.Modified);
                inkCostController.viewModel.BlackInkCartridgeCost = config.AppSettings.Settings["BlackCartridgeCost"].Value;
                inkCostController.costCalculatorViewModel.BlackCost = config.AppSettings.Settings["BlackCartridgeCost"].Value;

            }
        }

        private void ColorCostTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void ColorCostTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                double colorCartridgeCost = 0;// Properties.Settings.Default.ColorCartridgeCost;

                inkCostController.viewModel.ColorInkCartridgeCost = ColorCostTextBox.Text;
                inkCostController.costCalculatorViewModel.ColorCost = ColorCostTextBox.Text;
                double.TryParse(ColorCostTextBox.Text, out colorCartridgeCost);

                Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
                config.AppSettings.Settings.Remove("ColorCartridgeCost");
                config.AppSettings.Settings.Add("ColorCartridgeCost", colorCartridgeCost.ToString());
                config.Save(ConfigurationSaveMode.Modified);
                inkCostController.viewModel.ColorInkCartridgeCost = config.AppSettings.Settings["ColorCartridgeCost"].Value;
                inkCostController.costCalculatorViewModel.ColorCost = config.AppSettings.Settings["ColorCartridgeCost"].Value;

            }
        }

        private string[] FindCOMPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            return ports;
        }

        private void ControllerUtilitiesBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
            {
                string path = "C:\\";
                try
                {
                    path = Path.GetDirectoryName(UpdateFirmwareTextBox.Text);
                }
                catch { }

                dialog.InitialDirectory = path;
                dialog.Filter = "Firmware files | *.efm8";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    UpdateFirmwareTextBox.Text = dialog.FileName;
                }
            }
        }

        private void PrinterFirmwareUpdateBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
            {
                string path = "C:\\";
                try
                {
                    path = Path.GetDirectoryName(UpdateFirmwareTextBox.Text);
                }
                catch { }
                
                dialog.InitialDirectory = path;
                dialog.Filter = "Update files | *.fhx";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PrinterFirmwareUpdateFileTextBox.Text = dialog.FileName;
                }
            }
        }

        private void AerosolFanCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            ser.SendArgCommand(AfiniaArgCommand.SetFanPWM, 190);
        }

        private void AerosolFanCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            ser.SendArgCommand(AfiniaArgCommand.SetFanPWM, 255);
        }

        private void CalibrateUnwinderButton_Click(object sender, RoutedEventArgs e)
        {
            CalibrateUnwinderDialog dialog = new CalibrateUnwinderDialog(ser);
            dialog.ShowDialog();
        }

        private void CheckUnwinderStatusButton_Click(object sender, RoutedEventArgs e)
        {
            ser.SendBasicCommand(AfiniaBasicCommand.ToggleProximitySensor);
        }

        private void FirmwareUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(UpdateFirmwareTextBox.Text))
            {
                MessageBox.Show("Update file does not exist.", "Invalid update file", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FirmwareUpdateBusyIndicator.IsBusy = true;
            Process process = new Process();
            process.StartInfo.FileName = @"Resources\update.bat";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = "\"" + UpdateFirmwareTextBox.Text + "\"" + " -p" + ser.PortName + " -b115200";
            process.EnableRaisingEvents = true;
            process.Exited += FirmwareUpdateCheck;

            ser.SendBasicCommand(AfiniaBasicCommand.InvokeBootloader);
            ser.Close();

            FirmwareVersionTextBox.Text = "";

            try
            {
                if (process.Start())
                {
                    FirmwareUpdateTimer.Tick += FirmwareUpdateTimedOut;
                    FirmwareUpdateTimer.Interval = 1 * 60 * 1000;
                    FirmwareUpdateTimer.Start();
                }
                else
                {
                    FirmwareUpdateBusyIndicator.IsBusy = false;
                    MessageBox.Show("Could not begin firmware update.", "Firmware Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch
            {
                FirmwareUpdateTimer.Stop();
                FirmwareUpdateBusyIndicator.IsBusy = false;
                MessageBox.Show("Could not begin firmware update.", "Firmware Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void FirmwareUpdateCheck(object sender, EventArgs e)
        {
            Process process = sender as Process;
            if ((process == null) || (process.ExitCode != 0))
            {
                FirmwareUpdateTimer.Stop();
                MessageBox.Show("Firmware update failed.", "Firmware update failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                ser.Open();
                ser.ResponseHandler += FirmwareUpdateCompleted;
            }
        }

        private void FirmwareUpdateCompleted(string response)
        {
            if (Regex.IsMatch(response, @"A F I N I A +L*[0-9]+ +v[0-9]+.[0-9]+"))
            {
                FirmwareVersionTextBox.Dispatcher.BeginInvoke(new Action(() => { FirmwareVersionTextBox.Text = response.Substring(response.IndexOf("v")).Trim(); }));
                FirmwareUpdateBusyIndicator.Dispatcher.BeginInvoke(new Action(() => { FirmwareUpdateBusyIndicator.IsBusy = false; }));
                ser.ResponseHandler -= FirmwareUpdateCompleted;
                MessageBox.Show("Firmware update succeeded", "Firmware update succeeded", MessageBoxButton.OK, MessageBoxImage.None);
                TurnDebugOn(string.Empty);
            }

            FirmwareUpdateTimer.Stop();
        }

        private void FirmwareUpdateTimedOut(Object o, EventArgs e)
        {
            FirmwareUpdateTimer.Stop();

            try
            {
                foreach (Process proc in Process.GetProcessesByName("efm8load"))
                {
                    proc.Kill();
                }
            }
            catch (InvalidOperationException) { }
            catch (Win32Exception) { }

            MessageBox.Show("Firmware update timed out.", "Firmware update failed", MessageBoxButton.OK, MessageBoxImage.Error);
            FirmwareUpdateBusyIndicator.Dispatcher.BeginInvoke(new Action(() => { FirmwareUpdateBusyIndicator.IsBusy = false; }));
        }

        private void CommandSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            ser.SendString("\x1B" + CommandTextBox.Text);
        }

        private void ClearWindowButton_Click(object sender, RoutedEventArgs e)
        {
            ControllerUtilitiesTextBox.Text = string.Empty;
            AdvancedTextBox.Text = string.Empty;
        }

        private void AddLog(string text)
        {
            ControllerUtilitiesTextBox.Dispatcher.BeginInvoke(new Action(() => { ControllerUtilitiesTextBox.Text += text; }));
            AdvancedTextBox.Dispatcher.BeginInvoke(new Action(() => { AdvancedTextBox.Text += text; }));
            if (serialToLog)
            {
                serialLogger.Info(text);
            }
        }

        private void SwitchLogButton(bool logging)
        {
            StartLoggingButton.IsEnabled = !logging;
            StartLoggingButton.Visibility = logging ? Visibility.Hidden : Visibility.Visible;
            SaveLogButton.IsEnabled = logging;
            SaveLogButton.Visibility = logging ? Visibility.Visible : Visibility.Hidden;

            StartLoggingButton1.IsEnabled = !logging;
            StartLoggingButton1.Visibility = logging ? Visibility.Hidden : Visibility.Visible;
            SaveLogButton1.IsEnabled = logging;
            SaveLogButton1.Visibility = logging ? Visibility.Visible : Visibility.Hidden;
        }

        private void StartLoggingButton_Click(object sender, RoutedEventArgs e)
        {
            serialToLog = true;
            serialLogger.Info(ControllerUtilitiesTextBox.Text);
            SwitchLogButton(true);
        }

        private void SaveLogButton_Click(object sender, RoutedEventArgs e)
        {
            serialToLog = false;

            try
            {
                using (System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog())
                {
                    dialog.Filter = ("Text | *.txt");

                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Target target = LogManager.Configuration.FindTargetByName("serialLogFile");
                        while ((target != null) && (target is WrapperTargetBase))
                        {
                            target = (target as WrapperTargetBase).WrappedTarget;
                        }
                        //if (target != null)
                        //{
                            string logFileName = (target as FileTarget).FileName.ToString().Trim(new char[] { '\'' });
                            File.Copy(logFileName, dialog.FileName, true);
                        //}
                        SwitchLogButton(false);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SaveLogButton");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        private void MetallicCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            ser.SendArgCommand(AfiniaArgCommand.SetPWPL, 8);
            ser.SendArgCommand(AfiniaArgCommand.SetUnwinder, 2);
        }

        private void MetallicCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            ser.SendArgCommand(AfiniaArgCommand.SetPWPL, 3);
            ser.SendArgCommand(AfiniaArgCommand.SetUnwinder, 0);
        }

        private void UnwinderCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            ser.SendArgCommand(AfiniaArgCommand.SetUnwinder, 1);
        }

        private void UnwinderCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            ser.SendArgCommand(AfiniaArgCommand.SetUnwinder, 0);
        }

        private void FactoryDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            //ser.SendBasicCommand(AfiniaBasicCommand.FirmwareVersion);
            MessageBoxResult result = MessageBox.Show("This will restore all settings to factory defaults", "Confirm Action", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                ser.SendBasicCommand(AfiniaBasicCommand.RestoreFirmwareDefaults);
                logger.Debug("Factory Default: " + firmwareVersion);
                if (!firmwareVersion.ToLower().Contains("f502"))
                {
                    CalibrateUnwinderDialog dialog = new CalibrateUnwinderDialog(ser);
                    dialog.ShowDialog();
                }
            }
        }

        private void AutoCalibrateGAPButton_Click(object sender, RoutedEventArgs e)
        {
            ser.SendBasicCommand(AfiniaBasicCommand.CalibrateTransmissiveSensor);
        }

        private void GAPSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (GAPSensorNumberBox.Value != null)
            {
                ser.SendArgCommand(AfiniaArgCommand.SetTDAC, (double)GAPSensorNumberBox.Value);
                AutoCalibrateGAPButton.IsEnabled = false;
                GAPDisableManualButton.IsEnabled = true;
            }
        }

        private void GAPDisableManualButton_Click(object sender, RoutedEventArgs e)
        {
            AutoCalibrateGAPButton.IsEnabled = true;
            GAPDisableManualButton.IsEnabled = false;

            ser.SendArgCommand(AfiniaArgCommand.SetTDAC, 0);
        }

        private void AutoCalibrateBMSButton_Click(object sender, RoutedEventArgs e)
        {
            ser.SendBasicCommand(AfiniaBasicCommand.CalibrateReflectiveSensor);
        }

        private void BMSSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (BMSSensorNumberBox.Value != null)
            {
                ser.SendArgCommand(AfiniaArgCommand.SetRDAC, (double)BMSSensorNumberBox.Value);
                AutoCalibrateBMSButton.IsEnabled = false;
                BMSDisableManualButton.IsEnabled = true;
            }
        }

        private void BMSDisableManualButton_Click(object sender, RoutedEventArgs e)
        {
            AutoCalibrateBMSButton.IsEnabled = true;
            BMSDisableManualButton.IsEnabled = false;

            ser.SendArgCommand(AfiniaArgCommand.SetRDAC, 0);
        }

        private void PrinterFirmwareUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(PrinterFirmwareUpdateFileTextBox.Text))
            {
                MessageBox.Show("Update file does not exist", "Invalid update file", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            StartDownloading();

            // The update needs to be run as an administrator to work
            Process process = new Process();
            process.StartInfo.FileName = @"Resources\printer_fw_update.bat";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Verb = "runas";
            process.StartInfo.Arguments = PrinterFirmwareUpdateFileTextBox.Text;
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(PrinterFirmwareUpdateCompleted);

            try
            {
                if (process.Start())
                {
                    PrinterFirmwareUpdateTimer.Tick += PrinterFirmwareUpdateDownloadTimedOut;
                    PrinterFirmwareUpdateTimer.Interval = 10 * 60 * 1000;
                    PrinterFirmwareUpdateTimer.Start();
                }
                else
                {
                    SetDownloadComplete(false);
                    MessageBox.Show("Could not begin firmware update.", "Firmware update failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ShowPage2Instructions();
                    return;
                }
            }
            catch
            {
                SetDownloadComplete(false);
                MessageBox.Show("Could not begin firmware update.", "Firmware update failed", MessageBoxButton.OK, MessageBoxImage.Error);
                ShowPage2Instructions();
                return;
            }
        }

        private void PrinterFirmwareUpdateCompleted(object sender, EventArgs e)
        {
            Process process = sender as Process;

            PrinterFirmwareUpdateTimer.Stop();

            if ((process == null) || (process.ExitCode != 0))
            {
                PrinterFirmwareUpdateBusyIndicator.Dispatcher.BeginInvoke(new Action(() => { SetDownloadComplete(false); }));
                if ((process != null) && (process.ExitCode == 2))
                {
                    MessageBox.Show("Printer not connected. Check that USB cable is connected and that printer is on and in reflash mode.", "Firmware download failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Firmware download failed.", "Firmware download failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                PrinterFirmwareUpdateBusyIndicator.Dispatcher.BeginInvoke(new Action(() => { SetDownloadComplete(true); }));
                MessageBox.Show("Firmware download succeeded. See printer screen for further update status.", "Download complete");
            }
            //TurnDebugOn(string.Empty);
        }

        private void COMPortChooser_SelectionChanged(object sender, EventArgs e)
        {
            if ((COMPortChooser.SelectedValue != null) && ((string)COMPortChooser.SelectedValue != ser.PortName))
            {
                ser.Close();
                ser.PortName = (string)COMPortChooser.SelectedValue;
                ser.Open();
                ser.ResponseHandler += SetFirmwareVersionTextBox;
                ser.DataReceivedHandler = AddLog;
                ser.SendBasicCommand(AfiniaBasicCommand.FirmwareVersion);
                Thread.Sleep(500);
                ser.Close();
                ser.Open();
                //logger.Debug("ComPortChooser Changed: " + firmwareVersion);
                //AerosolFanCheckbox.Checked -= AerosolFanCheckbox_Checked;
                //AerosolFanCheckbox.IsChecked = true;
                //AerosolFanCheckbox.Checked += AerosolFanCheckbox_Checked;

                AerosolFanNorm.IsChecked = false;

                TurnDebugOn(string.Empty);
            }
        }

        private void TurnDebugOn(string text)
        {
            if (text == string.Empty || text.Trim().Equals("Monitor GPIO messages [Ignore TOF] turned OFF."))
            {
                ser.ResponseHandler = TurnDebugOn;
                ser.SendBasicCommand(AfiniaBasicCommand.ToggleDebugMessages);
            }
        }

        private void PrinterFirmwareUpdateInstructionsNavigate(object sender, RoutedEventArgs e)
        {
            Grid nextPage = null;

            if(PrinterFirmwareUpdatePage1.IsVisible)
            {
                if ((sender as Button) == PrinterFirmwareUpdateNextButton)
                {
                    nextPage = PrinterFirmwareUpdatePage2;
                }
            }
            else if (PrinterFirmwareUpdatePage2.IsVisible)
            {
                if ((sender as Button) == PrinterFirmwareUpdatePreviousButton)
                {
                    nextPage = PrinterFirmwareUpdatePage1;
                }
            }

            if (nextPage == PrinterFirmwareUpdatePage1)
            {
                ShowPage1Instructions();
            }
            else if (nextPage == PrinterFirmwareUpdatePage2)
            {
                ShowPage2Instructions();
            }
        }

        private void ShowPage1Instructions()
        {
            PrinterFirmwareUpdatePage1.Visibility = Visibility.Visible;
            PrinterFirmwareUpdatePage2.Visibility = Visibility.Hidden;
            PrinterFirmwareUpdatePreviousButton.IsEnabled = false;
            PrinterFirmwareUpdateNextButton.IsEnabled = true;
        }

        private void ShowPage2Instructions()
        {
            PrinterFirmwareUpdatePage1.Visibility = Visibility.Hidden;
            PrinterFirmwareUpdatePage2.Visibility = Visibility.Visible;
            PrinterFirmwareUpdatePreviousButton.IsEnabled = true;
            PrinterFirmwareUpdateNextButton.IsEnabled = false;
            SetNotDownloading();
        }

        private void SetNotDownloading()
        {
            PrinterFirmwareUpdateBrowseInstructionsDockPanel.Visibility = Visibility.Visible;
            PrinterFirmwareUpdateBusyIndicator.IsBusy = false;
            PrinterFirmwareDownloadCompleteLabel.Visibility = Visibility.Hidden;
        }

        private void StartDownloading()
        {
            PrinterFirmwareUpdateBrowseInstructionsDockPanel.Visibility = Visibility.Visible;
            PrinterFirmwareUpdateBusyIndicator.IsBusy = true;
            PrinterFirmwareDownloadCompleteLabel.Visibility = Visibility.Hidden;
        }

        private void SetDownloadComplete(bool success)
        {
            PrinterFirmwareUpdateBrowseInstructionsDockPanel.Visibility = Visibility.Hidden;
            PrinterFirmwareUpdateBusyIndicator.IsBusy = false;

            if (success)
            {
                PrinterFirmwareDownloadCompleteLabel.Content = "Download succeeded";
                PrinterFirmwareDownloadCompleteLabel.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                PrinterFirmwareDownloadCompleteLabel.Content = "Download failed";
                PrinterFirmwareDownloadCompleteLabel.Foreground = new SolidColorBrush(Colors.Red);
            }
            PrinterFirmwareDownloadCompleteLabel.Visibility = Visibility.Visible;
        }

        private void PrinterFirmwareUpdateDownloadTimedOut(Object o, EventArgs e)
        {
            PrinterFirmwareUpdateTimer.Stop();

            try
            {
                foreach(Process proc in Process.GetProcessesByName("USBSend"))
                {
                    proc.Kill();
                }
            }
            catch (InvalidOperationException) { }
            catch (Win32Exception) { }
        }

        private void ListParametersButton_Click(object sender, RoutedEventArgs e)
        {
            ser.SendBasicCommand(AfiniaBasicCommand.DisplayProximitySensorThresholds);
        }

        private void ControllerUtilitiesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ControllerUtilitiesScrollViewer.ScrollToEnd();
        }

        private void LockAdvancedTabButton_Click(object sender, RoutedEventArgs e)
        {
            advancedTabWarningShown = false;
            tabControl.SelectedItem = lastTab;
        }

        private void AdvancedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdvancedScrollViewer.ScrollToEnd();
        }

        private void AerosolFanMax_Checked(object sender, RoutedEventArgs e)
        {
            AerosolFanNorm.IsChecked = false;
            AerosolFanOff.IsChecked = false;
            ser.SendArgCommand(AfiniaArgCommand.SetFanPWM, 0);
        }

        private void AerosolFanNorm_Checked(object sender, RoutedEventArgs e)
        {
            AerosolFanMax.IsChecked = false;
            AerosolFanOff.IsChecked = false;
            ser.SendArgCommand(AfiniaArgCommand.SetFanPWM, 100);
        }

        private void AerosolFanOff_Checked(object sender, RoutedEventArgs e)
        {
            AerosolFanMax.IsChecked = false;
            AerosolFanNorm.IsChecked = false;
			//MessageBoxResult result = MessageBox.Show("Aerosol build up may occur inside the printer with the fan turned off", "Aerosol Fan Warning");
			ser.SendArgCommand(AfiniaArgCommand.SetFanPWM, 190);

			//if (result == MessageBoxResult.OK)
   //         {
                
   //         }
            
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FirmwareUpdateTab.IsSelected)
            {
                try
                {
                    PrinterCommandExecute.Command.GetMech1(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);
                    var result = PrinterCommandExecute.Command.ProcessMech1(outputFilePath + "mech1.xml");
                    var firmwareVersion = result.OemsiMediapathInfo.OemMediapathFirmwareVersion.Revision;
                    var index = firmwareVersion.IndexOf('.');
                    firmwareVersion = firmwareVersion.Substring(index + 1);
                    FirmwareVersionLabel.Content = $"Firmware Version: {firmwareVersion}";
                }
                catch (Exception ex)
                {
                    logger.Debug("FirmwareUpdateTab.IsSelected");
                    logger.Error($"{ex.Message}. {ex.InnerException}");
                }
            }
        }

        private void Toolbox_Click(object sender, RoutedEventArgs e)
        {
            string path = string.Empty;
            string pDriver = string.Empty;
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
                var results = searcher.Get();
                Dictionary<string, string> printers = new Dictionary<string, string>();
                foreach (var printer in results)
                {
                    if (printer.Properties["DriverName"].Value.ToString() == "Afinia Label L501 Label Printer")
                    {
                        printers.Add(printer.Properties["Name"].Value.ToString(), "Afinia Label L501 Label Printer");
                    }
                    else if (printer.Properties["DriverName"].Value.ToString() == "Afinia Label 502 Label Printer")
                    {
                        printers.Add(printer.Properties["Name"].Value.ToString(), "Afinia Label 502 Label Printer");
                    }
                }
                pDriver = printers[selectedPrinter];

                if (pDriver.ToLower().Contains("l501"))
                {
                    path = Properties.Settings.Default.L501PrinterToolBoxPath;
                }
                else
                {
                    path = Properties.Settings.Default.PrinterToolBoxPath;
                }

                logger.Debug($"ToolBox Click, using Path = {path}");
                logger.Debug($"ToolBox Click, using Driver = {pDriver}");
                Process p = new Process();
                p.StartInfo.FileName = $"{path}Toolbox.exe";
                //p.StartInfo.FileName = $"C:\\Program Files (x86)\\Afinia Label\\Afinia 502 Label Printer\\Toolbox 64-bit\\Toolbox.exe";
                p.StartInfo.Arguments = $"/printdriver \"{pDriver}\"";
                p.Start();
            }
            catch(Exception ex)
            {
                logger.Debug($"ToolBox Click, Path = {path}");
                logger.Debug($"ToolBox Click, Driver = {pDriver}");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        private void BMSSensorNumberBox_InputValidationError(object sender, Xceed.Wpf.Toolkit.Core.Input.InputValidationErrorEventArgs e)
        {
            var text = BMSSensorNumberBox.Text;
            var enteredVal = int.Parse(text);
            if (enteredVal > BMSSensorNumberBox.Maximum)
            {
                MessageBox.Show("Value out of range.  Enter a value between 76 - 250", "Error");
            }
            else if (enteredVal < BMSSensorNumberBox.Minimum)
            {
                MessageBox.Show("Value out of range.  Enter a value between 76 - 250", "Error");
            }
        }

        private void GAPSensorNumberBox_InputValidationError(object sender, Xceed.Wpf.Toolkit.Core.Input.InputValidationErrorEventArgs e)
        {
            var text = GAPSensorNumberBox.Text;
            var enteredVal = int.Parse(text);
            if (enteredVal > GAPSensorNumberBox.Maximum)
            {
                MessageBox.Show("Value out of range.  Enter a value between 126 – 2000", "Error");
            }
            else if (enteredVal < GAPSensorNumberBox.Minimum)
            {
                MessageBox.Show("Value out of range.  Enter a value between 126 – 2000", "Error");
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            //COMPortChooser.Items.Clear();
            if (COMPortChooser.Items.Count == 0)
            {
                ser.FindAfiniaPorts(AddPortName);
            }
            if(COMPortChooser.Items.Count > 0)
            {
                RefreshButton.Dispatcher.BeginInvoke(new Action(() => RefreshButton.Visibility = Visibility.Hidden));
            }
        }
    }
}
