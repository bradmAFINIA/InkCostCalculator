using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
//using System.Windows.Forms.CommonDialog;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InkCostCalculatorL501EnhancedWPF
{
    /// <summary>
    /// Interaction logic for ManualOperationsPage.xaml
    /// </summary>
    public partial class ManualOperationsPage : Page
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        uint feedLength = 9600;
        double inchValue = 0;
        double mmValue = 0;
        string selectedPrinter = string.Empty;
        string outputFilePath = string.Empty;
        double printedLength = 0;
        uint length = 0;

        public ManualOperationsPage()
        {
            InitializeComponent();
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
                selectedPrinter = config.AppSettings.Settings["SelectedPrinter"].Value;
                outputFilePath = config.AppSettings.Settings["OutputFilePath"].Value;
                Inches.IsChecked = true;

                FeedLengthSpinner.Value = ConvertToInches(feedLength);
                inchValue = FeedLengthSpinner.Value;
                mmValue = Math.Round(inchValue * 25.4, 2);
                TotalInchesPrintedTextBox.Text = "0.000";

                PrinterCommandExecute.Command.GetMech1(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);
                //PrinterCommandExecute.Command.EditPauseMechXML(outputFilePath + "mech1.xml", 0);
                //PrinterCommandExecute.Command.Pause(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);
                bool pauseState = PrinterCommandExecute.Command.ReadPauseState(outputFilePath + "mech1.xml");
                if (pauseState == true)
                {
                    PrinterCommandExecute.Command.Pause(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);
                    Pause_UnPauseButton.Background = System.Windows.Media.Brushes.Red;
                    Pause_UnPauseButton.Content = "Unpause";
                }
                else
                {
                    PrinterCommandExecute.Command.UnPause(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);
                    Pause_UnPauseButton.Background = System.Windows.Media.Brushes.Green;
                    Pause_UnPauseButton.Content = "Pause";
                }
                //Pause_UnPauseButton.Background = System.Windows.Media.Brushes.Green;
                //Pause_UnPauseButton.Content = "Pause";

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
                inchValue = FeedLengthSpinner.Value;
                mmValue = Math.Round(inchValue * 25.4,2);
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
                TotalInchesPrintedTextBox.Text = printedLength.ToString("#.000");

                FeedLengthSpinner.Value = mmValue /25.4;
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
                TotalInchesPrintedTextBox.Text = printedLength.ToString("#.000");
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

        private void Pause_UnPauseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrinterCommandExecute.Command.GetMech1(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);
                //if (Pause_UnPauseButton.Background == System.Windows.Media.Brushes.Green)
                //{
                //    PrinterCommandExecute.Command.Pause(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);
                //    Pause_UnPauseButton.Background = System.Windows.Media.Brushes.Red;
                //    Pause_UnPauseButton.Content = "Unpause";
                //}
                //else
                //{
                //    PrinterCommandExecute.Command.UnPause(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);
                //    Pause_UnPauseButton.Background = System.Windows.Media.Brushes.Green;
                //    Pause_UnPauseButton.Content = "Pause";
                //}

                bool pauseState = PrinterCommandExecute.Command.ReadPauseState(outputFilePath + "mech1.xml");
                if (pauseState == true)
                {
                    PrinterCommandExecute.Command.Pause(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);
                    Pause_UnPauseButton.Background = System.Windows.Media.Brushes.Red;
                    Pause_UnPauseButton.Content = "Unpause";
                }
                else
                {
                    PrinterCommandExecute.Command.UnPause(selectedPrinter, outputFilePath, "mech1.xml", Properties.Settings.Default.SWFWClientPath);
                    Pause_UnPauseButton.Background = System.Windows.Media.Brushes.Green;
                    Pause_UnPauseButton.Content = "Pause";
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Pause_UnPauseButton");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        private void TotalInchesPrintedButton_Click(object sender, RoutedEventArgs e)
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
                TotalInchesPrintedTextBox.Text = printedLength.ToString("#.000");
            }
            catch (Exception ex)
            {
                logger.Debug("TotalInchesPrinted");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
            config.AppSettings.Settings.Remove("FeedLengthInches");
            config.AppSettings.Settings.Add("FeedLengthInches", inchValue.ToString());
            config.Save(ConfigurationSaveMode.Modified);
        }

        #region printing to be deleted
        //private void sendPrint()
        //{
        //    string filename = Properties.Settings.Default.AlignmentImage;
        //    //Task<string> obTask = Task.Run(() => ( 
        //    using (var pd = new System.Drawing.Printing.PrintDocument())
        //    {
        //        pd.PrintPage += (_, o) =>
        //        {
        //            var img = System.Drawing.Image.FromFile(filename);

        //            // This uses a 50 pixel margin - adjust as needed
        //            o.Graphics.DrawImage(img, new System.Drawing.Point(50, 50));
        //        };
        //        pd.Print();
        //    }
        //}

        //private void PrintAlignmentPatternButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //string filename = Properties.Settings.Default.AlignmentImage;
        //    ////Task<string> obTask = Task.Run(() => ( 
        //    //using (var pd = new System.Drawing.Printing.PrintDocument())
        //    //{
        //    //    pd.PrintPage += (_, o) =>
        //    //    {
        //    //        var img = System.Drawing.Image.FromFile(filename);

        //    //        // This uses a 50 pixel margin - adjust as needed
        //    //        o.Graphics.DrawImage(img, new System.Drawing.Point(50, 50));
        //    //    };
        //    //    pd.Print();
        //    //}
        //    ////));

        //    Thread thread = new Thread(new ThreadStart(sendPrint));
        //    thread.Start();
        //    //PrintAlignment();

        //    //FileInfo value = new FileInfo(Properties.Settings.Default.AlignmentImage);
        //    //using (Process p = new Process())
        //    //{
        //    //    p.StartInfo.FileName = value.FullName;
        //    //    //p.StartInfo.Arguments = "\"" + selectedPrinter + "\"";
        //    //    p.StartInfo.Verb = "Print";
        //    //    p.Start();
        //    //}

        //    //ProcessStartInfo info = new ProcessStartInfo();
        //    ////Process p;

        //    //// Set process setting to be hidden
        //    //info.Verb = "Print";
        //    //info.FileName = value.FullName;
        //    //info.CreateNoWindow = false;
        //    //info.WindowStyle = ProcessWindowStyle.Normal;
        //    //info.Arguments = "\"" + selectedPrinter + "\"";
        //    //info.UseShellExecute = true;
        //    //// Start hidden process
        //    //using (Process p = new Process())
        //    //{
        //    //    p.StartInfo = info;
        //    //    p.Start();

        //    //    //// Give the process some time
        //    //    ////p.WaitForInputIdle();
        //    //    //Thread.Sleep(1000);

        //    //    //// Close it
        //    //    //if (p.CloseMainWindow() == false)
        //    //    //{
        //    //    //    p.Close();
        //    //    //}
        //    //}

        //    //System.Windows.Forms.PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
        //    //printDialog1.PrinterSettings.PrinterName = selectedPrinter;
        //    //if (printDialog1.ShowDialog() == DialogResult.OK)
        //    //{
        //    //    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(Properties.Settings.Default.AlignmentImage);
        //    //    info.Arguments = "\"" + printDialog1.PrinterSettings.PrinterName + "\"";
        //    //    info.CreateNoWindow = true;
        //    //    info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        //    //    info.UseShellExecute = true;
        //    //    info.Verb = "PrintTo";
        //    //    System.Diagnostics.Process.Start(info);
        //    //}


        //    //BitmapImage myBitmapImage = new BitmapImage();
        //    //myBitmapImage.BeginInit();
        //    //myBitmapImage.UriSource = new Uri(Properties.Settings.Default.AlignmentImage);
        //    //myBitmapImage.EndInit();
        //    //AlignmentImage.Source = myBitmapImage;
        //    //System.Windows.Controls.PrintDialog myPrintDialog = new System.Windows.Controls.PrintDialog();

        //    //if (myPrintDialog.ShowDialog() == true)
        //    //{
        //    //    myPrintDialog.PrintVisual(AlignmentImage, "Image Print");
        //    //}
        //}

        //public void PrintAlignment()
        //{
        //    BitmapImage bi = new BitmapImage();
        //    bi.BeginInit();
        //    bi.UriSource = new Uri(Properties.Settings.Default.AlignmentImage);
        //    bi.EndInit();
        //    var vis = new DrawingVisual();
        //    using (var dc = vis.RenderOpen())
        //    {
        //        dc.DrawImage(bi, new Rect { Width = bi.Width, Height = bi.Height });
        //    }
        //    System.Windows.Controls.PrintDialog printDlg = new System.Windows.Controls.PrintDialog();
        //   // using (PrintDocument pd = new PrintDocument())
        //    //{
        //        //pd.PrinterSettings.PrinterName = selectedPrinter;
        //        //pd.PrinterSettings.Copies = 1;
        //        ////pd.PrinterSettings.
        //        //printDlg.Document = pd;
        //        printDlg.PageRangeSelection = PageRangeSelection.AllPages;
        //        printDlg.UserPageRangeEnabled = true;

        //        // Display the dialog. This returns true if the user presses the Print button.
        //        Nullable<Boolean> print = printDlg.ShowDialog();
        //        if (print == true)
        //        {
        //            printDlg.PrintVisual(vis, "My Image");
        //        //PrintDocument pd = new PrintDocument();
        //        //pd.PrintPage += (sender, args) =>
        //        //{
        //        //    System.Drawing.Image img = System.Drawing.Image.FromFile(Properties.Settings.Default.AlignmentImage);
        //        //    ////System.Drawing.Point loc = new System.Drawing.Point(100, 100);
        //        //    //float x = 0.0F;
        //        //    //float y = 0.0F;
        //        //    ////float width = 150.0F;
        //        //    ////float height = 150.0F;
        //        //    //System.Drawing.Rectangle srcRect = new System.Drawing.Rectangle(0, 0, 100, 100);
        //        //    //GraphicsUnit units = GraphicsUnit.Pixel;

        //        //    // Draw original image to screen.
        //        //    args.Graphics.DrawImage(img, args.MarginBounds);
        //        //    //args.Graphics.DrawImage(img, x, y, srcRect, units);
        //        //    //System.Drawing.Point loc = new System.Drawing.Point(Properties.Settings.Default.AlignmentImageXSize, Properties.Settings.Default.AlignmentImageYSize);
        //        //    //e.Graphics.DrawImage(img, loc);
        //        //};
        //    }
        //        //pd.Print();
        //    //}
        //}

        //private void PrintPage(object o, PrintPageEventArgs e)
        //{
        //    System.Drawing.Image img = System.Drawing.Image.FromFile(Properties.Settings.Default.AlignmentImage);
        //    //System.Drawing.Point loc = new System.Drawing.Point(100, 100);
        //    float x = 0.0F;
        //    float y = 0.0F;
        //    //float width = 150.0F;
        //    //float height = 150.0F;
        //    System.Drawing.Rectangle srcRect = new System.Drawing.Rectangle(0, 0, 150, 150);
        //    GraphicsUnit units = GraphicsUnit.Pixel;

        //    // Draw original image to screen.
        //    e.Graphics.DrawImage(img, x, y, srcRect, units);
        //    //System.Drawing.Point loc = new System.Drawing.Point(Properties.Settings.Default.AlignmentImageXSize, Properties.Settings.Default.AlignmentImageYSize);
        //   //e.Graphics.DrawImage(img, loc);
        //}
        //public sealed class PrintDialog : System.Windows.Forms.CommonDialog
        //{
        //    public override void Reset()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    protected override bool RunDialog(IntPtr hwndOwner)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        #endregion
    }
}
