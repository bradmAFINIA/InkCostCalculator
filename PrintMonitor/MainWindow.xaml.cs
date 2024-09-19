using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NLog;
using NLog.Targets;
using Monitors;

namespace PrintMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public string selectedPrinter = string.Empty;
        PrinterConfigViewModel printerConfigView;
        PrintQueueMonitor pqm = null;
        List<string> printers = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            //Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
            printers = GetPrinters();
            pqm = new PrintQueueMonitor();
            printerConfigView = new PrinterConfigViewModel(selectedPrinter, printers);
            this.DataContext = printerConfigView;
            printerConfigView.Name = selectedPrinter;
        }
        public void StartWatcher(string selectedPrinter)
        {
            try
            {
                logger.Debug("Starting Queue Monitor");

                //pqm.OnJobLogChange += new PrintJobLogChanged(pqm_OnJobLogChange);
                //logger.Debug("Subscribed to Monitor Logging");
                selectedPrinter = PrintersComboBox.SelectedValue.ToString();
                pqm.Start(selectedPrinter, false);
            }
            catch (Exception ex)
            {
                logger.Debug("StartWatcher");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        public void StopWatcher(string selectedPrinter)
        {
            logger.Debug("Stop Watcher Method");
            pqm.Stop();
            //pqm.OnJobLogChange -= new PrintJobLogChanged(pqm_OnJobLogChange);
            //logger.Debug("Unsubscribed to Monitor Logging");
        }

        //void pqm_OnJobLogChange(object Sender, PrintJobLogEventArgs e)
        //{
        //logger.Debug("pqm_OnJobLogChange");
        //if ((e.Stat != 1) && (e.Stat != 2) && (e.PdwChange != 256))
        //{
        //    logger.Debug($"Jobs in Queue = {e.QueueCount} Job Id = {e.JobID}, Status char = {e.Stat}, Pdw Change = {e.PdwChange.ToString()} at {DateTime.Now.ToString()}");
        //}
        //}
        private List<string> GetPrinters()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
            var results = searcher.Get();
            List<string> printers = new List<string>();
            foreach (var printer in results)
            {
                    printers.Add(printer.Properties["Name"].Value.ToString());
            }
            return printers;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartWatcher(selectedPrinter);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopWatcher(selectedPrinter);
        }

        //public string GetPrinterIp(string selectedPrinter)
        //{
        //    string ip = string.Empty;
        //    try
        //    {
        //        var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
        //        var results = searcher.Get();

        //        foreach (var printer in results)
        //        {
        //            if (selectedPrinter == printer.Properties["Name"].Value.ToString())
        //            {
        //                var portName = printer.Properties["PortName"].Value;
        //                var searcher2 = new ManagementObjectSearcher("SELECT * FROM Win32_TCPIPPrinterPort where Name LIKE '" + portName + "'");
        //                var results2 = searcher2.Get();
        //                foreach (var printer2 in results2)
        //                {
        //                    ip = printer2.Properties["HostAddress"].Value.ToString();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Debug("GetPrinterIp");
        //        logger.Error($"{ex.Message}. {ex.InnerException}");
        //    }
        //    return ip;
        //}

        //public class PrintMonitor
        //{
        //    private Thread monitorThread;
        //    public delegate void PrintJobInit(string a);
        //    public delegate void PrintJobDone(string a);


        //    public event PrintJobInit JobInit = null;
        //    public event PrintJobDone JobDone = null;

        //    public string text;
        //    string JobID;
        //    bool done = true;

        //    public PrintMonitor()
        //    {
        //        //cambiodeestadotread();
        //    }

        //    internal void Start()
        //    {
        //        monitorThread = new Thread(new ThreadStart(WaitChangeStatus));
        //        monitorThread.Start();
        //    }

        //    public void Stop()
        //    {
        //        monitorThread.Abort();
        //        monitorThread.Interrupt();
        //    }

        //    private void WaitChangeStatus()
        //    {

        //        while (true)
        //        {
        //            string f_SearchQuery = "SELECT * FROM Win32_PrintJob";
        //            ManagementObjectSearcher f_SearchPrintJobs = new ManagementObjectSearcher(f_SearchQuery);
        //            ManagementObjectCollection f_PrntJobCollection = f_SearchPrintJobs.Get();
        //            if (f_PrntJobCollection.Count > 0)
        //            {
        //                foreach (ManagementObject f_PrntJob in f_PrntJobCollection)
        //                {
        //                    foreach (System.Management.PropertyData item in f_PrntJob.Properties)
        //                    {
        //                        if (JobID != f_PrntJob["JobId"].ToString())
        //                        {
        //                            JobID = f_PrntJob["JobId"].ToString();
        //                            JobInit?.Invoke(System.DateTime.Now.ToString() + "\n");
        //                            done = false;
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (!done)
        //                {
        //                    done = true;
        //                    text = System.DateTime.Now.ToString() + "\n";
        //                    JobDone?.Invoke(text);
        //                }
        //            }

        //        }
        //    }
        //}

    }
}
