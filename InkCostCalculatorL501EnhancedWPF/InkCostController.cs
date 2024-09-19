using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.IO;
using System.Linq;
using System.Management;
using System.Timers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using NLog;
using NLog.Targets;

using Monitors;
using InkCostModel;
using PrinterCommandExecute;
using System.Windows.Navigation;
using System.Threading;
using System.Collections.ObjectModel;

namespace InkCostCalculatorL501EnhancedWPF
{
    public class InkCostController
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public InkCostViewModel viewModel = new InkCostViewModel();
        public CostCalculatorViewModel costCalculatorViewModel = new CostCalculatorViewModel();

        //public CostCalculatorViewModel costCalculatorViewModel = new CostCalculatorViewModel();
        //public ObservableCollection<CostCalculatorViewModel> costCalculatorViewModels = new ObservableCollection<CostCalculatorViewModel>();

        public List<Cost> jobs = new List<Cost>();
        public List<CostData> costCalcs = new List<CostData>();
        public string outputFilePath = string.Empty;
        public string selectedPrinter = string.Empty;
        public string selectedIp = string.Empty;
        public bool changePrinter = false;
        //public bool jobRunning = false;

        PrintQueueMonitor pqm = null;

        string inkLevelsPath = string.Empty;
        string logFilesPath = string.Empty;

        //bool win10 = false;
        //bool win8 = false;

        Dictionary<int, TimeSpan> JobTimeSpans = new Dictionary<int, TimeSpan>();
        List<int> JobStatesDelete = new List<int>();
        List<string> LogLines = new List<string>();
        List<string> JobStatesEndTrigger = new List<string>();

        System.Timers.Timer timer = new System.Timers.Timer();
        int timerSeconds = 0;

        System.Timers.Timer timerIdle = new System.Timers.Timer();

        System.Timers.Timer timerCaptureDelay = new System.Timers.Timer();

        //bool captureInProgress = false;
        List<int> pausedJobs = new List<int>();

        //private bool useLast = false;
        ProductUsage lastProductUsage = new ProductUsage();
        //int lastQueueCont = 0;

       // private bool okToCapture = false;
        public ObservableCollection<CostData> costDatas = new ObservableCollection<CostData>();

        public delegate void PrintRunninStatusChanged(object Sender, bool e);
        public event PrintRunninStatusChanged OnPrintRunningStatusChange;

        public InkCostController()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
                outputFilePath = config.AppSettings.Settings["OutputFilePath"].Value;
                inkLevelsPath = Path.GetDirectoryName($"{outputFilePath}\\InkLevels\\inklevels.xml");
                if (!Directory.Exists(inkLevelsPath))
                    Directory.CreateDirectory(inkLevelsPath);
                logFilesPath = Path.GetDirectoryName($"{outputFilePath}\\LogFiles\\log.txt");
                if (!Directory.Exists(logFilesPath))
                    Directory.CreateDirectory(logFilesPath);
                var target = (FileTarget)LogManager.Configuration.FindTargetByName("logfile");
                target.FileName = $"{outputFilePath}\\LogFiles\\log.txt";
                LogManager.ReconfigExistingLoggers();
                ArchiveInkLevelFile(outputFilePath);

                logger.Info("Starting");
                var ver = System.Environment.OSVersion;
                //if (ver.Version.Major >= 10)
                //{
                //    if (ver.Version.Minor >= 0)
                //        //win10 = true;
                //}
                //else if (ver.Version.Major >= 6)
                //{
                //    if (ver.Version.Minor >= 2)
                //        //win8 = true;
                //}

                logger.Info($"OS Version: Major {ver.Version.Major}, Major Revision {ver.Version.MajorRevision}, Minor {ver.Version.Minor}, Minor Revision {ver.Version.MinorRevision}, build {ver.Version.Build}, Revision {ver.Version.Revision}");

                viewModel.ColorInkCartridgeCost = config.AppSettings.Settings["ColorCartridgeCost"].Value;
                viewModel.BlackInkCartridgeCost = config.AppSettings.Settings["BlackCartridgeCost"].Value;

                costCalculatorViewModel.ColorCost = config.AppSettings.Settings["ColorCartridgeCost"].Value;
                costCalculatorViewModel.BlackCost = config.AppSettings.Settings["BlackCartridgeCost"].Value;
                if (File.Exists($"{outputFilePath}ProductUsageDyn.xml"))
                {
                    ProductUsage productUse = ProcessProductUsageDyn($"{outputFilePath}ProductUsageDyn.xml");
                    costCalculatorViewModel.TotalImpressions = productUse.TotalImpressions.ToString();
                }

                selectedPrinter = config.AppSettings.Settings["SelectedPrinter"].Value;
                selectedIp = GetPrinterIp(selectedPrinter);
                viewModel.SelectedPrinterName = selectedPrinter;
                costCalculatorViewModel.SelectedPrinterName = selectedPrinter;

                pqm = new PrintQueueMonitor();
                StartWatcher(config.AppSettings.Settings["SelectedPrinter"].Value.Trim());

                CaptureEndEnableToolStrip(false);

                viewModel.ShowHideAdvancedButton = "Show Advanced Info";
                viewModel.JobsListBoxLabelVis = Visibility.Hidden;
                viewModel.JobsListBoxVis = Visibility.Hidden;
                viewModel.RawLabelVis = Visibility.Hidden;
                viewModel.RawListBoxVis = Visibility.Hidden;
                viewModel.Width = 1001;
                viewModel.Height = 309;

                timer.Interval = 1000;
                timer.Elapsed += Timer_Elapsed;
                timer.Start();

                timerIdle.Interval = Properties.Settings.Default.IdleMilliSeconds;
                timerIdle.Elapsed += TimerIdle_Elapsed;

                timerCaptureDelay.Interval = Properties.Settings.Default.DelayMilliSeconds;
                //timerCaptureDelay.Elapsed += TimerCaptureDelay_Elapsed;



                logger.Debug("InkCostController Initialized");
            }
            catch (Exception ex)
            {
                logger.Debug("InkCostController");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }
        //public void Dispose()
        //{

        //    Dispose(true);
        //}
        ~InkCostController()
        {
            try
            {
                StopWatcher();
            }
            catch { }
        }
        //private void TimerCaptureDelay_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    okToCapture = true;
        //    timerCaptureDelay.Stop();
        //}

        private void TimerIdle_Elapsed(object sender, ElapsedEventArgs e)
        {
            //useLast = false;
        }

        public string GetPrinterIp(string selectedPrinter)
        {
            string ip = string.Empty;
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
                var results = searcher.Get();

                foreach (var printer in results)
                {
                    if (selectedPrinter == printer.Properties["Name"].Value.ToString())
                    {
                        var portName = printer.Properties["PortName"].Value;
                        var searcher2 = new ManagementObjectSearcher("SELECT * FROM Win32_TCPIPPrinterPort where Name LIKE '" + portName + "'");
                        var results2 = searcher2.Get();
                        foreach (var printer2 in results2)
                        {
                            ip = printer2.Properties["HostAddress"].Value.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("GetPrinterIp");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return ip;
        }
        void ArchiveInkLevelFile(string pOutputFilePath)
        {
            try
            {
                inkLevelsPath = Path.GetDirectoryName($"{pOutputFilePath}\\InkLevels\\inklevels.xml");
                if (!Directory.Exists(inkLevelsPath))
                    Directory.CreateDirectory(inkLevelsPath);

                if (File.Exists($"{inkLevelsPath}\\inklevels.xml"))
                {
                    File.Move($"{inkLevelsPath}\\inklevels.xml", $"{inkLevelsPath}\\inklevels_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.xml");
                    foreach (var fi in new DirectoryInfo(inkLevelsPath).GetFiles().OrderByDescending(x => x.LastWriteTime).Skip(5))
                        fi.Delete();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("ArchiveInkLevelFile");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }
        void ArchiveLogFileFile(string pOutputFilePath)
        {
            try
            {
                logFilesPath = Path.GetDirectoryName($"{pOutputFilePath}\\LogFiles\\log.txt");
                if (!Directory.Exists(logFilesPath))
                    Directory.CreateDirectory(logFilesPath);

                if (File.Exists($"{logFilesPath}\\log.txt"))
                {
                    File.Move($"{logFilesPath}\\log.txt", $"{logFilesPath}\\log_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.txt");
                    foreach (var fi in new DirectoryInfo(logFilesPath).GetFiles().OrderByDescending(x => x.LastWriteTime).Skip(5))
                        fi.Delete();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("ArchiveLogFile");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        public void StartWatcher(string selectedPrinter)
        {
            try
            {
                logger.Debug("Starting Queue Monitor");
                if (!changePrinter)
                {
                    pqm.OnJobStatusChange += new PrintJobStatusChanged(pqm_OnJobStatusChange);
                    logger.Debug("Subscribed to Monitor Status Change");
                    //pqm.OnJobLogChange += new PrintJobLogChanged(pqm_OnJobLogChange);
                    //logger.Debug("Subscribed to Monitor Logging");
                }

                //pqm.Start(selectedPrinter);

                if (string.IsNullOrEmpty(selectedIp))
                    pqm.Start(selectedPrinter, false);
                else
                    pqm.Start(selectedPrinter, true);

                changePrinter = false;
                //useLast = false;
            }
            catch (Exception ex)
            {
                logger.Debug("StartWatcher");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        public void StopWatcher()
        {
            pqm.Stop();
            logger.Debug("Stop Watcher Method");
            pqm.OnJobStatusChange -= new PrintJobStatusChanged(pqm_OnJobStatusChange);
            logger.Debug("Unsubscribed to Monitor Status Change");
            //pqm.OnJobLogChange -= new PrintJobLogChanged(pqm_OnJobLogChange);
            //logger.Debug("Unsubscribed to Monitor Logging");
        }

        //void pqm_OnJobLogChange(object Sender, PrintJobLogEventArgs e)
        //{
        //    logger.Debug("+++ pqm_OnJobLogChange");
        //    if ((e.Stat != 1) && (e.Stat != 2) && (e.PdwChange != 256))
        //    {
        //        logger.Debug($"+++ Jobs in Queue = {e.QueueCount} Job Id = {e.JobID}, Status char = {e.Stat}, Pdw Change = {e.PdwChange.ToString()} at {DateTime.Now.ToString()}");
        //    }
        //    timerSeconds = 0;
        //    EnableChangePrinter(false);
        //}

        void pqm_OnJobStatusChange(object Sender, PrintJobChangeEventArgs e)
        {
            logger.Info($"pqm_OnJobStatusChange JobId: {e.JobID}, Change: {e.PdwChange}, Stat: {e.Stat}");
            try
            {
                bool addJob = (e.PdwChange & 256) == 256;
                bool setJob = (e.PdwChange & 512) == 512;
                bool deleteJob = (e.PdwChange & 1024) == 1024;
                bool job = (e.PdwChange & 2048) == 2048;
                bool jRun = (e.Stat & 8192) == 8192;
                if (string.IsNullOrEmpty(selectedIp)) //local USB printer
                {
                    //logger.Debug("USB Printer");
                    #region USB Printer
                    #region commented
                    //if (addJob)
                    //{
                    //if (!captureInProgress)
                    //{
                    //    #region start capture
                    //    try
                    //    {
                    //        captureInProgress = true;
                    //        logger.Info($"********* JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                    //        string printJob = $"{e.JobID}_{e.Name}_{e.JobSubmitted.Hour.ToString()}{e.JobSubmitted.Minute.ToString()}{e.JobSubmitted.Second.ToString()}";
                    //        manualName = printJob;
                    //        manualJobId = e.JobID;
                    //        CaptureEndEnableToolStrip(true);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        logger.Error($"{ex.Message}. JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                    //    }
                    //    #endregion
                    //}
                    //}
                    //else if (deleteJob)
                    //if (captureInProgress && (setJob || job) && manualJobId!=e.JobID)
                    //{
                    //    captureInProgress = false;
                    //}
                    //if (deleteJob && !captureInProgress)
                    #endregion

                    //if ((e.Stat == 132) || (e.Stat == 8208))
                    //if (e.Stat == 8212)
                    //if (deleteJob && manualJobId != e.JobID)
                    if (e.Stat == 388 && manualJobId != e.JobID)
                    {
                        #region End Capture
                        try
                        {
                            logger.Info($"*********pqm_OnJobStatusChange JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{e.PdwChange}, Stat:{e.Stat}");
                            //captureInProgress = true;
                            string printJob = $"{e.JobID}_{e.Name}_{e.JobSubmitted.Hour.ToString()}{e.JobSubmitted.Minute.ToString()}{e.JobSubmitted.Second.ToString()}";
                            manualName = printJob;
                            manualJobId = e.JobID;
                            InkCapture(manualName, true);
                            InkCalculate(manualName, e.JobID);
                        }
                        catch (Exception ex)
                        {
                            logger.Error($"{ex.Message}. {ex.InnerException}");
                        }
                        #endregion
                    }
                    #endregion
                }
                else //network printer
                {
                    #region Network Printer
                    #region commented
                    /***
                    //if (addJob)
                    //{
                    //    if (!captureInProgress)
                    //    {
                    //        #region start capture
                    //        try
                    //        {
                    //            captureInProgress = true;
                    //            logger.Info($"********* JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                    //            string printJob = $"{e.JobID}_{e.Name}_{e.JobSubmitted.Hour.ToString()}{e.JobSubmitted.Minute.ToString()}{e.JobSubmitted.Second.ToString()}";
                    //            manualName = printJob;
                    //            manualJobId = e.JobID;
                    //            CaptureEndEnableToolStrip(true);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            logger.Error($"{ex.Message}. JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                    //        }
                    //        #endregion
                    //    }
                    //}
                    //    else if (deleteJob)
                    //if (deleteJob && !captureInProgress)
                    **/
                    #endregion
                    //if (deleteJob && manualJobId != e.JobID)
                    if (e.Stat == 388 && manualJobId != e.JobID)
                    {
                        #region End Capture
                        try
                        {
                            logger.Info($"*********pqm_OnJobStatusChange JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{e.PdwChange}, Stat:{e.Stat}");
                            //captureInProgress = true;
                            string printJob = $"{e.JobID}_{e.Name}_{e.JobSubmitted.Hour.ToString()}{e.JobSubmitted.Minute.ToString()}{e.JobSubmitted.Second.ToString()}";
                            manualName = printJob;
                            manualJobId = e.JobID;
                            InkCapture(manualName, true);
                            InkCalculate(manualName, e.JobID);
                        }
                        catch (Exception ex)
                        {
                            logger.Error($"{ex.Message}. {ex.InnerException}");
                        }
                        #endregion
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                logger.Debug("StatusChange");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        /**** orig Job Status Change
        void pqm_OnJobStatusChange(object Sender, PrintJobChangeEventArgs e)
        {
            logger.Debug("pqm_OnJobStatusChange");
            if (!JobStates.ContainsKey(e.JobID))
            {
                logger.Info($"JobId: {e.JobID}, Change: {e.PdwChange}, Stat: {e.Stat}, Job State: None");
            }
            else
            {
                logger.Info($"JobId: {e.JobID}, Change: {e.PdwChange}, Stat: {e.Stat}, Job State: {JobStates[e.JobID].State}");
            }
            try
            {

                bool addJob = (e.PdwChange & 256) == 256;
                bool setJob = (e.PdwChange & 512) == 512;
                bool deleteJob = (e.PdwChange & 1024) == 1024;
                bool job = (e.PdwChange & 2048) == 2048;
                bool jRun = (e.Stat & 8192) == 8192;
                if (string.IsNullOrEmpty(selectedIp)) //local USB printer
                {
                    //logger.Debug("USB Printer");
                    #region USB Printer
                    if ((e.Stat != 1) && (e.Stat != 2) && (e.PdwChange != 256))
                    {
                        //logger.Debug("USB Printer state layer 1");
                        if (!JobStates.ContainsKey(e.JobID))
                        {
                            logger.Info($"********* JobStates does not contain JobID");
                            #region start capture
                            if (!captureInProgress)
                            {
                                if ((e.Stat < 32768) && (e.Stat != 40))
                                {
                                    if ((e.Stat & 16) == 16)
                                    {
                                        try
                                        {
                                            //okToCapture = false;
                                            captureInProgress = true;
                                            logger.Info($"********* JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                            string printJob = $"{e.JobID}_{e.Name}_{e.JobSubmitted.Hour.ToString()}{e.JobSubmitted.Minute.ToString()}{e.JobSubmitted.Second.ToString()}";
                                            manualName = printJob;
                                            manualJobId = e.JobID;
                                            CaptureEndEnableToolStrip(true);
                                            timerIdle.Stop();

                                            if (useLast)
                                            {
                                                JobStates.Add(e.JobID, new JobInformation() { State = "Start", Name = InkCapture(printJob, false) });
                                            }
                                            else
                                            {
                                                JobStates.Add(e.JobID, new JobInformation() { State = "Start", Name = InkCapture(printJob, true) });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error($"{ex.Message}. JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                        else if (JobStates.ContainsKey(e.JobID))
                        {
                            //logger.Debug("USB Printer Job ID layer 2");
                            #region Running
                            if (JobStates[e.JobID].State == "Start")
                            {
                                if (e.Job)
                                {
                                    //timerCaptureDelay.Start();
                                    JobStates[e.JobID].State = "Running";
                                    //Let us raise the event
                                    OnPrintRunningStatusChange?.Invoke(this, true);
                                    //jobRunning = true;
                                    logger.Info($"********* JobId: {e.JobID} Job Running at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                }
                            }
                            #endregion
                            #region End Capture
                            if ((JobStates[e.JobID].State == "Running") || (JobStates[e.JobID].State == "Start"))// && okToCapture)
                            {
                                //logger.Debug("USB Printer Running layer 3");
                                //logger.Debug($"USB Printer win8 {win8.ToString()}");
                                //logger.Debug($"USB Printer win10 {win10.ToString()}");
                                logger.Info($"--------Status message--- JobId: {e.JobID}, Change:{e.PdwChange}, Stat:{e.Stat}, Job States count:{JobStates.Count.ToString()}");
                                if (!win8 && !win10)
                                {
                                    #region win 7
                                    if ((e.Setjob) && (!e.Job))
                                    {
                                        if ((e.Stat == 132) || (e.Stat == 8208))
                                        {
                                            try
                                            {
                                                JobStates[e.JobID].State = "Done";
                                                logger.Info($"********* JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{e.PdwChange}, Stat:{e.Stat}");
                                                string printjob = JobStates[e.JobID].Name;
                                                InkCapture(printjob, true);
                                                InkCalculate(printjob, e.JobID);
                                                JobStatesDelete.Add(e.JobID);
                                                if (JobStatesDelete.Count > 5)
                                                {
                                                    JobStates.Remove(JobStatesDelete[0]);
                                                    JobStatesDelete.Remove(0);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error($"{ex.Message}. {ex.InnerException}");
                                            }
                                        }
                                        if (e.Stat == 8212)
                                        {
                                            try
                                            {
                                                bool capture = false;
                                                if (lastQueueCont > e.QueueCount)
                                                    capture = true;
                                                lastQueueCont = e.QueueCount;
                                                if (capture)
                                                {
                                                    JobStates[e.JobID].State = "Done";
                                                    logger.Info($"********* JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{e.PdwChange}, Stat:{e.Stat}");
                                                    string printjob = JobStates[e.JobID].Name;
                                                    InkCapture(printjob, true);
                                                    InkCalculate(printjob, e.JobID);
                                                    JobStatesDelete.Add(e.JobID);
                                                    if (JobStatesDelete.Count > 5)
                                                    {
                                                        JobStates.Remove(JobStatesDelete[0]);
                                                        JobStatesDelete.Remove(0);
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error($"{ex.Message}. {ex.InnerException}");
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                if (win8)
                                {
                                    #region win 8
                                    //if ((e.Setjob) && (!e.Job))
                                    //if (!job)
                                    //{
                                    //logger.Debug($"USB Printer win8");
                                    //if ((e.Stat == 132) || (e.Stat == 8208))
                                    if ((e.Stat == 132) || ((e.Stat == 8208) && !job) || (deleteJob))
                                    {
                                        try
                                        {
                                            JobStates[e.JobID].State = "Done";
                                            logger.Info($"********* JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{e.PdwChange}, Stat:{e.Stat}");
                                            string printjob = JobStates[e.JobID].Name;
                                            InkCapture(printjob, true);
                                            InkCalculate(printjob, e.JobID);
                                            JobStatesDelete.Add(e.JobID);
                                            if (JobStatesDelete.Count > 5)
                                            {
                                                JobStates.Remove(JobStatesDelete[0]);
                                                JobStatesDelete.Remove(0);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error($"{ex.Message}. {ex.InnerException}");
                                        }
                                    }
                                    if (e.Stat == 8212)
                                    {
                                        try
                                        {
                                            bool capture = false;
                                            if (lastQueueCont > e.QueueCount)
                                                capture = true;
                                            lastQueueCont = e.QueueCount;
                                            if (capture)
                                            {
                                                JobStates[e.JobID].State = "Done";
                                                logger.Info($"+++++++ JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{e.PdwChange}, Stat:{e.Stat}");
                                                //logger.Debug($"SetJob:  { e.Setjob.ToString() } , Job: { e.Job.ToString() }");
                                                string printjob = JobStates[e.JobID].Name;
                                                InkCapture(printjob, true);
                                                InkCalculate(printjob, e.JobID);
                                                JobStatesDelete.Add(e.JobID);
                                                if (JobStatesDelete.Count > 5)
                                                {
                                                    JobStates.Remove(JobStatesDelete[0]);
                                                    JobStatesDelete.Remove(0);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error($"{ex.Message}. {ex.InnerException}");
                                        }
                                    }
                                    //}
                                    #endregion
                                }
                                if (win10)
                                {
                                    #region win 10
                                    //if ((e.Setjob) && (!e.Job))
                                    //if (!job)
                                    //{
                                    //logger.Debug($"USB Printer win10");
                                    //if ((e.Stat == 132) || (e.Stat == 8208))
                                    if ((e.Stat == 132) || ((e.Stat == 8208) && !job) || (deleteJob))
                                    {
                                        try
                                        {
                                            JobStates[e.JobID].State = "Done";
                                            logger.Info($"********* JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{e.PdwChange}, Stat:{e.Stat}");
                                            //logger.Debug($"SetJob:  { e.Setjob.ToString() } , Job: { e.Job.ToString() }");
                                            string printjob = JobStates[e.JobID].Name;
                                            InkCapture(printjob, true);
                                            InkCalculate(printjob, e.JobID);
                                            JobStatesDelete.Add(e.JobID);
                                            if (JobStatesDelete.Count > 5)
                                            {
                                                JobStates.Remove(JobStatesDelete[0]);
                                                JobStatesDelete.Remove(0);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error($"{ex.Message}. {ex.InnerException}");
                                        }
                                    }
                                    if (e.Stat == 8212)
                                    {
                                        try
                                        {
                                            bool capture = false;
                                            if (lastQueueCont > e.QueueCount)
                                                capture = true;
                                            lastQueueCont = e.QueueCount;
                                            if (capture)
                                            {
                                                JobStates[e.JobID].State = "Done";
                                                logger.Info($"++++++++ JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{e.PdwChange}, Stat:{e.Stat}");
                                                //logger.Debug($"SetJob:  { e.Setjob.ToString() } , Job: { e.Job.ToString() }");
                                                string printjob = JobStates[e.JobID].Name;
                                                InkCapture(printjob, true);
                                                InkCalculate(printjob, e.JobID);
                                                JobStatesDelete.Add(e.JobID);
                                                if (JobStatesDelete.Count > 5)
                                                {
                                                    JobStates.Remove(JobStatesDelete[0]);
                                                    JobStatesDelete.Remove(0);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error($"{ex.Message}. {ex.InnerException}");
                                        }
                                    }
                                    //}
                                    #endregion
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
                else //network printer
                {
                    #region Network Printer
                    #region win 7
                    if (!win8 && !win10)
                    {
                        if ((e.Stat != 1) && (e.Stat != 2) && (e.PdwChange != 256))
                        {
                            if (!JobStates.ContainsKey(e.JobID))
                            {
                                #region start capture
                                if (!captureInProgress)
                                {
                                    if ((e.Stat < 32768) && (e.Stat != 40))
                                    {
                                        if ((e.Stat & 16) == 16)
                                        {
                                            try
                                            {
                                                //okToCapture = false;
                                                captureInProgress = true;
                                                logger.Info($"********* JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                                string printJob = $"{e.JobID}_{e.Name}_{e.JobSubmitted.Hour.ToString()}{e.JobSubmitted.Minute.ToString()}{e.JobSubmitted.Second.ToString()}";
                                                manualName = printJob;
                                                manualJobId = e.JobID;
                                                CaptureEndEnableToolStrip(true);
                                                timerIdle.Stop();
                                                if (useLast)
                                                {
                                                    JobStates.Add(e.JobID, new JobInformation() { State = "Start", Name = InkCapture(printJob, false) });
                                                }
                                                else
                                                {
                                                    JobStates.Add(e.JobID, new JobInformation() { State = "Start", Name = InkCapture(printJob, true) });
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error($"{ex.Message}. JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            else if (JobStates.ContainsKey(e.JobID))
                            {
                                #region Running
                                if (JobStates[e.JobID].State == "Start")
                                {
                                    if (e.Job)
                                    {
                                        //timerCaptureDelay.Start();
                                        JobStates[e.JobID].State = "Running";
                                        logger.Info($"********* JobId: {e.JobID} Job Running at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                    }
                                }
                                #endregion
                                #region End Capture
                                if ((JobStates[e.JobID].State == "Running") || (JobStates[e.JobID].State == "Start"))// && okToCapture)
                                {
                                    if ((e.Setjob) && (!e.Job))
                                    {
                                        if ((e.Stat == 132) || (e.Stat == 8208))
                                        {
                                            try
                                            {
                                                JobStates[e.JobID].State = "Done";
                                                logger.Info($"********* JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{e.PdwChange}, Stat:{e.Stat}");
                                                string printjob = JobStates[e.JobID].Name;
                                                InkCapture(printjob, true);
                                                InkCalculate(printjob, e.JobID);
                                                JobStatesDelete.Add(e.JobID);
                                                if (JobStatesDelete.Count > 5)
                                                {
                                                    JobStates.Remove(JobStatesDelete[0]);
                                                    JobStatesDelete.Remove(0);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error($"{ex.Message}. {ex.InnerException}");
                                            }
                                        }
                                        if (e.Stat == 8212)
                                        {
                                            try
                                            {
                                                bool capture = false;
                                                if (lastQueueCont > e.QueueCount)
                                                    capture = true;
                                                lastQueueCont = e.QueueCount;
                                                if (capture)
                                                {
                                                    JobStates[e.JobID].State = "Done";
                                                    logger.Info($"********* JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{e.PdwChange}, Stat:{e.Stat}");
                                                    string printjob = JobStates[e.JobID].Name;
                                                    InkCapture(printjob, true);
                                                    InkCalculate(printjob, e.JobID);
                                                    JobStatesDelete.Add(e.JobID);
                                                    if (JobStatesDelete.Count > 5)
                                                    {
                                                        JobStates.Remove(JobStatesDelete[0]);
                                                        JobStatesDelete.Remove(0);
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error($"{ex.Message}. {ex.InnerException}");
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion
                    #region win 8
                    if (win8)
                    {
                        if ((e.Stat != 1) && (e.Stat != 2) && (e.PdwChange != 256))
                        {
                            if (!JobStates.ContainsKey(e.JobID))
                            {
                                #region start capture
                                if (!captureInProgress)
                                {
                                    if ((e.Stat < 32768) && (e.Stat != 40))
                                    {
                                        if ((e.Stat & 16) == 16)
                                        {
                                            try
                                            {
                                                //okToCapture = false;
                                                captureInProgress = true;
                                                logger.Info($"********* JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                                string printJob = $"{e.JobID}_{e.Name}_{e.JobSubmitted.Hour.ToString()}{e.JobSubmitted.Minute.ToString()}{e.JobSubmitted.Second.ToString()}";
                                                manualName = printJob;
                                                manualJobId = e.JobID;
                                                CaptureEndEnableToolStrip(true);
                                                timerIdle.Stop();
                                                if (useLast)
                                                {
                                                    JobStates.Add(e.JobID, new JobInformation() { State = "Start", Name = InkCapture(printJob, false) });
                                                }
                                                else
                                                {
                                                    JobStates.Add(e.JobID, new JobInformation() { State = "Start", Name = InkCapture(printJob, true) });
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error($"{ex.Message}. JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            else if (JobStates.ContainsKey(e.JobID))
                            {
                                #region Running
                                if (JobStates[e.JobID].State == "Start")
                                {
                                    if (e.Job)
                                    {
                                        //timerCaptureDelay.Start();
                                        JobStates[e.JobID].State = "Running";
                                        logger.Info($"********* JobId: {e.JobID} Job Running at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                    }
                                }
                                #endregion
                                #region End Capture
                                if ((JobStates[e.JobID].State == "Running") || (JobStates[e.JobID].State == "Start"))//&& okToCapture)
                                {
                                    logger.Info($"--------Status message--- JobId: {e.JobID}, Change:{e.PdwChange}, Stat:{e.Stat}, Job States count:{JobStates.Count.ToString()}");
                                    //if ((e.Setjob) && (!e.Job))
                                    //if (!job)
                                    //{
                                    //if ((e.Stat == 132) || (e.Stat == 8208))
                                    if ((e.Stat == 132) || ((e.Stat == 8208) && !job) || (deleteJob))
                                    {
                                        try
                                        {
                                            JobStates[e.JobID].State = "Done";
                                            logger.Info($"********* JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{e.PdwChange}, Stat:{e.Stat}");
                                            string printjob = JobStates[e.JobID].Name;
                                            InkCapture(printjob, true);
                                            InkCalculate(printjob, e.JobID);
                                            JobStatesDelete.Add(e.JobID);
                                            if (JobStatesDelete.Count > 5)
                                            {
                                                JobStates.Remove(JobStatesDelete[0]);
                                                JobStatesDelete.Remove(0);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error($"{ex.Message}. {ex.InnerException}");
                                        }
                                    }
                                    if (e.Stat == 8212)
                                    {
                                        try
                                        {
                                            bool capture = false;
                                            if (lastQueueCont > e.QueueCount)
                                                capture = true;
                                            lastQueueCont = e.QueueCount;
                                            if (capture)
                                            {
                                                JobStates[e.JobID].State = "Done";
                                                logger.Info($"+++++++ JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{e.PdwChange}, Stat:{e.Stat}");
                                                string printjob = JobStates[e.JobID].Name;
                                                InkCapture(printjob, true);
                                                InkCalculate(printjob, e.JobID);
                                                JobStatesDelete.Add(e.JobID);
                                                if (JobStatesDelete.Count > 5)
                                                {
                                                    JobStates.Remove(JobStatesDelete[0]);
                                                    JobStatesDelete.Remove(0);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error($"{ex.Message}. {ex.InnerException}");
                                        }
                                    }
                                    //}
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion
                    #region win 10
                    if (win10)
                    {
                        if ((e.Stat != 1) && (e.Stat != 2) && (e.PdwChange != 256))
                        {
                            if (!JobStates.ContainsKey(e.JobID))
                            {
                                #region start capture
                                if (!captureInProgress)
                                {
                                    if ((e.Stat < 32768) && (e.Stat != 40))
                                    {
                                        if ((e.Stat & 16) == 16)
                                        {
                                            try
                                            {
                                                //okToCapture = false;
                                                captureInProgress = true;
                                                logger.Info($"********* JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                                string printJob = $"{e.JobID}_{e.Name}_{e.JobSubmitted.Hour.ToString()}{e.JobSubmitted.Minute.ToString()}{e.JobSubmitted.Second.ToString()}";
                                                manualName = printJob;
                                                manualJobId = e.JobID;
                                                CaptureEndEnableToolStrip(true);
                                                timerIdle.Stop();
                                                if (useLast)
                                                {
                                                    JobStates.Add(e.JobID, new JobInformation() { State = "Start", Name = InkCapture(printJob, false) });
                                                }
                                                else
                                                {
                                                    JobStates.Add(e.JobID, new JobInformation() { State = "Start", Name = InkCapture(printJob, true) });
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error($"{ex.Message}. JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            else if (JobStates.ContainsKey(e.JobID))
                            {
                                #region Running
                                if (JobStates[e.JobID].State == "Start")
                                {
                                    if (e.Job)
                                    {
                                        //timerCaptureDelay.Start();
                                        JobStates[e.JobID].State = "Running";
                                        logger.Info($"********* JobId: {e.JobID} Job Running at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                    }
                                }
                                #endregion
                                #region End Capture
                                if ((JobStates[e.JobID].State == "Running") || (JobStates[e.JobID].State == "Start"))// && okToCapture)
                                {
                                    //if ((e.Setjob) && (!e.Job))
                                    //if (!job)
                                    //{
                                    //if ((e.Stat == 132) || (e.Stat == 8208))
                                    if ((e.Stat == 132) || ((e.Stat == 8208) && !job) || (deleteJob))
                                    {
                                        try
                                        {
                                            JobStates[e.JobID].State = "Done";
                                            logger.Info($"********* JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{e.PdwChange}, Stat:{e.Stat}");
                                            //logger.Debug($"SetJob:  { e.Setjob.ToString() } , Job: { e.Job.ToString() }");
                                            string printjob = JobStates[e.JobID].Name;
                                            InkCapture(printjob, true);
                                            InkCalculate(printjob, e.JobID);
                                            JobStatesDelete.Add(e.JobID);
                                            if (JobStatesDelete.Count > 5)
                                            {
                                                JobStates.Remove(JobStatesDelete[0]);
                                                JobStatesDelete.Remove(0);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error($"{ex.Message}. {ex.InnerException}");
                                        }
                                    }
                                    if (e.Stat == 8212)
                                    {
                                        try
                                        {
                                            bool capture = false;
                                            if (lastQueueCont > e.QueueCount)
                                                capture = true;
                                            lastQueueCont = e.QueueCount;
                                            if (capture)
                                            {
                                                JobStates[e.JobID].State = "Done";
                                                logger.Info($"+++++++ JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{e.PdwChange}, Stat:{e.Stat}");
                                                //logger.Debug($"SetJob:  { e.Setjob.ToString() } , Job: { e.Job.ToString() }");
                                                string printjob = JobStates[e.JobID].Name;
                                                InkCapture(printjob, true);
                                                InkCalculate(printjob, e.JobID);
                                                JobStatesDelete.Add(e.JobID);
                                                if (JobStatesDelete.Count > 5)
                                                {
                                                    JobStates.Remove(JobStatesDelete[0]);
                                                    JobStatesDelete.Remove(0);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error($"{ex.Message}. {ex.InnerException}");
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion
                    #endregion
                }

                //this.DataContext = inkCostController.viewModel;
            }
            catch (Exception ex)
            {
                logger.Debug("StatusChange");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            //});
        }
****/
        
        //private void InkCostMain_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    pqm.OnJobStatusChange -= new PrintJobStatusChanged(pqm_OnJobStatusChange);
        //    logger.Debug("Unsubscribed to Monitor Status Change");
        //    pqm.OnJobLogChange -= new PrintJobLogChanged(pqm_OnJobLogChange);
        //    logger.Debug("Unsubscribed to Monitor Logging");
        //}

        public void PopulateJobListBox()
        {
            logger.Info($"#########Updating Job List Box");
            try
            {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    viewModel.JobList.Clear();
                });
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    viewModel.JobList.Add(Properties.Settings.Default.JobFileHeader);
                });
                string totalCost = string.Empty;
                string costPerLabel = string.Empty;
                foreach (var job in jobs)
                {
                    totalCost = (job.TotalCostOfCyan + job.TotalCostOfMagenta + job.TotalCostOfYellow + job.TotalCostOfBlack).ToString("0.#####");
                    costPerLabel = ((job.TotalCostOfCyan + job.TotalCostOfMagenta + job.TotalCostOfYellow + job.TotalCostOfBlack) / job.TotalColorLabels).ToString("0.#####");

                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        viewModel.JobList.Add($"{job.Printer},{job.PrintJob},{job.TotalColorLabels},{job.LabelCostOfCyan.ToString("0.#####")},{job.TotalCostOfCyan.ToString("0.#####")},{job.TotalCyanMlUsed.ToString("0.#####")},{job.CyanPagesPerCartridge.ToString("0.#")},{job.LabelCostOfMagenta.ToString("0.#####")},{job.TotalCostOfMagenta.ToString("0.#####")},{job.TotalMagentaMlUsed.ToString("0.#####")},{job.MagentaPagesPerCartridge.ToString("0.#")},{job.LabelCostOfYellow.ToString("0.#####")},{job.TotalCostOfYellow.ToString("0.#####")},{job.TotalYellowMlUsed.ToString("0.#####")},{job.YellowPagesPerCartridge.ToString("0.#")},{job.LabelCostOfBlack.ToString("0.#####")},{job.TotalCostOfBlack.ToString("0.#####")},{job.TotalBlackMlUsed.ToString("0.#####")},{job.BlackPagesPerCartridge.ToString("0.#")},{totalCost},{costPerLabel}");
                    });

                    //var uiContext = SynchronizationContext.Current;
                    //uiContext.Send(x => viewModel.JobList.Add($"{job.Printer},{job.PrintJob},{job.TotalColorLabels},{job.LabelCostOfCyan.ToString("0.#####")},{job.TotalCostOfCyan.ToString("0.#####")},{job.TotalCyanMlUsed.ToString("0.#####")},{job.CyanPagesPerCartridge.ToString("0.#")},{job.LabelCostOfMagenta.ToString("0.#####")},{job.TotalCostOfMagenta.ToString("0.#####")},{job.TotalMagentaMlUsed.ToString("0.#####")},{job.MagentaPagesPerCartridge.ToString("0.#")},{job.LabelCostOfYellow.ToString("0.#####")},{job.TotalCostOfYellow.ToString("0.#####")},{job.TotalYellowMlUsed.ToString("0.#####")},{job.YellowPagesPerCartridge.ToString("0.#")},{job.LabelCostOfBlack.ToString("0.#####")},{job.TotalCostOfBlack.ToString("0.#####")},{job.TotalBlackMlUsed.ToString("0.#####")},{job.BlackPagesPerCartridge.ToString("0.#")},{totalCost},{costPerLabel}"), null);

                    //viewModel.JobList.Add($"{job.Printer},{job.PrintJob},{job.TotalColorLabels},{job.LabelCostOfCyan.ToString("0.#####")},{job.TotalCostOfCyan.ToString("0.#####")},{job.TotalCyanMlUsed.ToString("0.#####")},{job.CyanPagesPerCartridge.ToString("0.#")},{job.LabelCostOfMagenta.ToString("0.#####")},{job.TotalCostOfMagenta.ToString("0.#####")},{job.TotalMagentaMlUsed.ToString("0.#####")},{job.MagentaPagesPerCartridge.ToString("0.#")},{job.LabelCostOfYellow.ToString("0.#####")},{job.TotalCostOfYellow.ToString("0.#####")},{job.TotalYellowMlUsed.ToString("0.#####")},{job.YellowPagesPerCartridge.ToString("0.#")},{job.LabelCostOfBlack.ToString("0.#####")},{job.TotalCostOfBlack.ToString("0.#####")},{job.TotalBlackMlUsed.ToString("0.#####")},{job.BlackPagesPerCartridge.ToString("0.#")},{totalCost},{costPerLabel}");
                }
                //    JobsListBox.SelectedIndex = JobsListBox.Items.Count - 1;
            }
            catch (Exception ex)
            {
                logger.Debug("PopulateJobListBox");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        public void PopulateCostCalcListView()
        {
            logger.Info($"#########Updating Cost Calc List View");
            try
            {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    costCalculatorViewModel.Data.Clear();
                });
                foreach (var calc in costCalcs)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        costCalculatorViewModel.Data.Add(calc);
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Debug("PopulateCostCalcListView");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        public void PopulateListBox(List<InkLevel> top, List<InkLevel> bottom)
        {
            try
            {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    viewModel.RawList.Clear();
                });
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    viewModel.RawList.Add("Current Read");
                });
                foreach (var item in top)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        viewModel.RawList.Add($"{item.timestamp},{item.Color},{item.TotalImpressions},{item.DuplexSheets},{item.JamEvents},{item.MispickEvents},{item.PEID},{item.Count},{item.Type}");
                    });

                    //var uiContext = SynchronizationContext.Current;
                    //uiContext.Send(x => viewModel.RawList.Add($"{item.timestamp},{item.Color},{item.TotalImpressions},{item.DuplexSheets},{item.JamEvents},{item.MispickEvents},{item.PEID},{item.Count},{item.Type}"), null);

                    //viewModel.RawList.Add($"{item.timestamp},{item.Color},{item.TotalImpressions},{item.DuplexSheets},{item.JamEvents},{item.MispickEvents},{item.PEID},{item.Count},{item.Type}");
                }
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    viewModel.RawList.Add("Previous Read");
                });
                foreach (var item in bottom)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        viewModel.RawList.Add($"{item.timestamp},{item.Color},{item.TotalImpressions},{item.DuplexSheets},{item.JamEvents},{item.MispickEvents},{item.PEID},{item.Count},{item.Type}");
                    });

                    //var uiContext = SynchronizationContext.Current;
                    //uiContext.Send(x => viewModel.RawList.Add($"{item.timestamp},{item.Color},{item.TotalImpressions},{item.DuplexSheets},{item.JamEvents},{item.MispickEvents},{item.PEID},{item.Count},{item.Type}"), null);

                    //viewModel.RawList.Add($"{item.timestamp},{item.Color},{item.TotalImpressions},{item.DuplexSheets},{item.JamEvents},{item.MispickEvents},{item.PEID},{item.Count},{item.Type}");
                }
            }
            catch (Exception ex)
            {
                logger.Debug("PopulateListBox");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        public List<InkLevel> FillTable(ProductUsage productUse, string printJob, string printer)
        {
            try
            {
                DataTable consumableTable = new DataTable("Consumable");
                consumableTable.Columns.Add("Printer", typeof(string));
                consumableTable.Columns.Add("PrintJob", typeof(string));

                consumableTable.Columns.Add("TotalImpressions", typeof(uint));
                consumableTable.Columns.Add("DuplexSheets", typeof(uint));
                consumableTable.Columns.Add("JamEvents", typeof(uint));
                consumableTable.Columns.Add("MispickEvents", typeof(uint));
                consumableTable.Columns.Add("Color", typeof(string));
                consumableTable.Columns.Add("PEID", typeof(uint));
                consumableTable.Columns.Add("Count", typeof(uint));
                consumableTable.Columns.Add("Type", typeof(string));

                foreach (var consumable in productUse.Consumables)
                {
                    foreach (var agent in consumable.Agent)
                    {
                        consumableTable.Rows.Add(printer, printJob, productUse.TotalImpressions, productUse.DuplexSheets, productUse.JamEvents, productUse.MispickEvents, consumable.MarkerColor, agent.PEID, agent.Count, agent.Type);
                    }
                }
                var result = from a in consumableTable.AsEnumerable()
                             select new InkLevel()
                             {
                                 Printer = a.Field<string>("Printer"),
                                 PrintJob = a.Field<string>("PrintJob"),
                                 TotalImpressions = a.Field<uint>("TotalImpressions"),
                                 DuplexSheets = a.Field<uint>("DuplexSheets"),
                                 JamEvents = a.Field<uint>("JamEvents"),
                                 MispickEvents = a.Field<uint>("MispickEvents"),
                                 Color = a.Field<string>("Color"),
                                 PEID = a.Field<uint>("PEID"),
                                 Count = a.Field<uint>("Count"),
                                 Type = a.Field<string>("Type"),
                                 timestamp = DateTime.Now
                             };
                return result.ToList();
            }
            catch (Exception ex)
            {
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return null;
        }

        public string manualName = string.Empty;
        public int manualJobId = 0;

        delegate void CaptureEndEnableToolStripDelegate(bool enable);
        public void CaptureEndEnableToolStrip(bool enable)
        {
            //if (this.menuStrip1.InvokeRequired)
            //{
            //    CaptureEndEnableToolStripDelegate d = new CaptureEndEnableToolStripDelegate(CaptureEndEnableToolStrip);
            //    this.Invoke(d, new object[] { enable });
            //}
            //else
            //{
            //    endCaptureToolStripMenuItem.Enabled = enable;
            //}
        }

        delegate void EnableChangePrinterDelegate(bool enable);
        private void EnableChangePrinter(bool enable)
        {
            //if (this.menuStrip1.InvokeRequired)
            //{
            //    EnableChangePrinterDelegate d = new EnableChangePrinterDelegate(EnableChangePrinter);
            //    this.Invoke(d, new object[] { enable });
            //}
            //else
            //{
            //    printerToolStripMenuItem.Enabled = enable;
            //}
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerSeconds++;
            if (timerSeconds >= 20)
            {
                EnableChangePrinter(true);
            }
        }
        public void WriteFile(string path, List<InkLevel> results)
        {
            try
            {
                var xmlperson = ObjectToXMLGeneric<List<InkLevel>>(results);
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(xmlperson);
                xdoc.Save(path);
            }
            catch (Exception ex)
            {
                logger.Debug("WriteFile");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }
        public List<InkLevel> ReadFile(string path)
        {
            try
            {
                if (File.Exists($"{inkLevelsPath}\\inklevels.xml"))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(path);
                    string xmlcontents = doc.InnerXml;
                    List<InkLevel> inkLevels = new List<InkLevel>();
                    inkLevels = XMLToObject<List<InkLevel>>(xmlcontents);

                    return inkLevels;
                }
                else
                    logger.Debug("No Ink Level xml file or path exists");
            }
            catch (Exception ex)
            {
                logger.Debug("ReadFile");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return null;
        }
        public String ObjectToXMLGeneric<T>(T filter)
        {
            string xml = null;
            try
            {
                using (StringWriter sw = new StringWriter())
                {

                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    xs.Serialize(sw, filter);
                    try
                    {
                        xml = sw.ToString();

                    }
                    catch (Exception e)
                    {
                        logger.Error($"{e.Message}. {e.InnerException}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return xml;
        }
        public T XMLToObject<T>(string xml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));

                using (var textReader = new StringReader(xml))
                {
                    using (var xmlReader = XmlReader.Create(textReader))
                    {
                        return (T)serializer.Deserialize(xmlReader);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return default(T);
        }
        public string InkCapture(string printjob, bool captureNew)
        {
            try
            {
                string selected = selectedPrinter;
                if (!string.IsNullOrEmpty(selectedIp))
                    selected = selectedIp;
                if (captureNew)
                {
                    logger.Info($"#########Capturing New {printjob} at {DateTime.Now.ToString()}");
                    Command.RunEXE(selected, "ProductUsageDyn", Properties.Settings.Default.ProductUsageDyn, outputFilePath, Properties.Settings.Default.SWFWClientPath);
                    System.Threading.Thread.Sleep(1000);
                    if (File.Exists($"{outputFilePath}ProductUsageDyn.xml"))
                    {
                        logger.Info($"#########The {outputFilePath}ProductUsageDyn.xml file exists");
                        ProductUsage productUse = ProcessProductUsageDyn($"{outputFilePath}ProductUsageDyn.xml");
                        logger.Info($"#########TotalImpressions =  {productUse.TotalImpressions.ToString()}");
                        costCalculatorViewModel.TotalImpressions = productUse.TotalImpressions.ToString();
                        processInkLevel(printjob, productUse);
                    }
                }
                else
                {
                    logger.Info($"#########Capturing using last value {printjob} at {DateTime.Now.ToString()}");
                    processInkLevel(printjob, lastProductUsage);
                }
                //Let us raise the event
                OnPrintRunningStatusChange?.Invoke(this, false);
                //jobRunning = false;
            }
            catch (Exception ex)
            {
                logger.Debug("InkCapture");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return $"{printjob}";
        }
        public void processInkLevel(string printjob, ProductUsage productUse)
        {
            try
            {
                List<InkLevel> inkLevels = new List<InkLevel>();
                if (File.Exists($"{inkLevelsPath}\\inklevels.xml"))
                    inkLevels = ReadFile($"{inkLevelsPath}\\inklevels.xml");
                else
                    logger.Info("No Ink Level xml file or path exists");

                List<InkLevel> results = FillTable(productUse, $"{printjob}", selectedPrinter);

                foreach (var row in results)
                {
                    inkLevels.Add(row);
                }
                WriteFile($"{inkLevelsPath}\\inklevels.xml", inkLevels);
                lastProductUsage = productUse;
            }
            catch (Exception ex)
            {
                logger.Debug("ProcessInkLevel");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }
        public void InkCalculate(string printjob, int currentJobId)
        {
            logger.Info($"#########Calculating {printjob} at {DateTime.Now.ToString()}");
            try
            {
                if (File.Exists($"{inkLevelsPath}\\inklevels.xml"))
                {
                    List<InkLevel> inkLevels = ReadFile($"{inkLevelsPath}\\inklevels.xml");
                    Cost cost = CalculateCost(inkLevels, printjob);
                    cost.PrintJob = printjob;
                    cost.Printer = selectedPrinter;
                    viewModel.CyanCost = cost.TotalCostOfCyan.ToString("0.#####");
                    viewModel.MagentaCost = cost.TotalCostOfMagenta.ToString("0.#####");
                    viewModel.YellowCost = cost.TotalCostOfYellow.ToString("0.#####");
                    viewModel.BlackCost = cost.TotalCostOfBlack.ToString("0.#####");

                    viewModel.LabelCyanCost = cost.LabelCostOfCyan.ToString("0.#####");
                    viewModel.LabelMagentaCost = cost.LabelCostOfMagenta.ToString("0.#####");
                    viewModel.LabelYellowCost = cost.LabelCostOfYellow.ToString("0.#####");
                    viewModel.LabelBlackCost = cost.LabelCostOfBlack.ToString("0.#####");

                    viewModel.CyanMl = cost.TotalCyanMlUsed.ToString("0.#####");
                    viewModel.MagentaMl = cost.TotalMagentaMlUsed.ToString("0.#####");
                    viewModel.YellowMl = cost.TotalYellowMlUsed.ToString("0.#####");
                    viewModel.BlackMl = cost.TotalBlackMlUsed.ToString("0.#####");

                    viewModel.CyanPagesPerCartridge = cost.CyanPagesPerCartridge.ToString("0.#");
                    viewModel.MagentaPagesPerCartridge = cost.MagentaPagesPerCartridge.ToString("0.#");
                    viewModel.YellowPagesPerCartridge = cost.YellowPagesPerCartridge.ToString("0.#");
                    viewModel.BlackPagesPerCartridge = cost.BlackPagesPerCartridge.ToString("0.#");

                    viewModel.TotalLabels = cost.TotalColorLabels.ToString();
                    viewModel.TotalCost = (cost.TotalCostOfColor + cost.TotalCostOfBlack).ToString("0.#####");
                    viewModel.CostPerLabel = ((cost.TotalCostOfColor + cost.TotalCostOfBlack) / cost.TotalColorLabels).ToString("0.#####");
                    jobs.Add(cost);
                    if (jobs.Count > 20)
                    {
                        jobs.RemoveAt(0);
                    }
                    PopulateJobListBox();

                    if (costCalcs.Count>20)
                    {
                        costCalcs.RemoveAt(20);
                    }
                    string timestamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                    //ml values
                    costCalcs.Add(new CostData
                    {
                        BlackMl = cost.PageBlackMlUsed.ToString("0.#######"),
                        CyanMl = cost.PageCyanMlUsed.ToString("0.#######"),
                        MagentaMl = cost.PageMagentaMlUsed.ToString("0.#######"),
                        YellowMl = cost.PageYellowMlUsed.ToString("0.#######"),
                        TotalLabels = cost.TotalColorLabels.ToString(),
                        CostPerLabel = ((cost.TotalCostOfColor + cost.TotalCostOfBlack) / cost.TotalColorLabels).ToString("0.#####"),
                        Timestamp = timestamp
                    });

                    ////Page Cost or Label Cost)
                    //costCalcs.Add(new CostData
                    //{
                    //    BlackMl = viewModel.LabelBlackCost,
                    //    CyanMl = viewModel.LabelCyanCost,
                    //    MagentaMl = viewModel.LabelMagentaCost,
                    //    YellowMl = viewModel.LabelYellowCost,
                    //    TotalLabels = viewModel.TotalLabels,
                    //    CostPerLabel = viewModel.CostPerLabel,
                    //    Timestamp = timestamp
                    //});

                    ////Total Cost
                    //costCalcs.Add(new CostData
                    //{
                    //    BlackMl = viewModel.BlackCost,
                    //    CyanMl = viewModel.CyanCost,
                    //    MagentaMl = viewModel.MagentaCost,
                    //    YellowMl = viewModel.YellowCost,
                    //    TotalLabels = viewModel.TotalLabels,
                    //    CostPerLabel = viewModel.CostPerLabel,
                    //    Timestamp = timestamp
                    //});
                    PopulateCostCalcListView();
                }
                else
                    logger.Info("No Ink Level xml file or path exists");

                //captureInProgress = false;
                //useLast = true;
                //lastQueueCont = 0;
                CaptureEndEnableToolStrip(false);
                timerIdle.Start();
            }
            catch (Exception ex)
            {
                logger.Debug("InkCalculate");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }
        public ProductUsage ProcessProductUsageDyn(string filepathProductUsageDyn)
        {
            try
            {
                ProductUsageDyn usage;
                var serializer = new XmlSerializer(typeof(ProductUsageDyn));
                using (var reader = XmlReader.Create(filepathProductUsageDyn))
                {
                    usage = (ProductUsageDyn)serializer.Deserialize(reader);
                }
                ProductUsage productUsage = new ProductUsage();
                productUsage.TotalImpressions = usage.PrinterSubunit.TotalImpressions.Value;
                //productUsage.TotalImpressions = usage.PrintApplicationSubunit.TotalImpressions.Value;
                productUsage.DuplexSheets = usage.PrinterSubunit.DuplexSheets.Value;
                productUsage.JamEvents = usage.PrinterSubunit.JamEvents.Value;
                productUsage.MispickEvents = usage.PrinterSubunit.MispickEvents;
                productUsage.Consumables = new List<ConsumableSubUnit>();

                foreach (var consumableSubunit in usage.ConsumableSubunit)
                {
                    ConsumableSubUnit consumable = new ConsumableSubUnit();
                    consumable.MarkerColor = consumableSubunit.MarkerColor;
                    consumable.Agent = new List<MarkingAgent>();
                    foreach (var markingAgentCount in consumableSubunit.UsageByMarkingAgentCount)
                    {
                        MarkingAgent agent = new MarkingAgent();
                        if (markingAgentCount.MarkingAgentCountType == "HPDropsCount")
                        {
                            agent.PEID = markingAgentCount.MarkingAgentCount.PEID;
                            agent.Count = markingAgentCount.MarkingAgentCount.Value;
                            agent.Type = markingAgentCount.MarkingAgentCountType;
                        }
                        if (agent.Type != null)
                        {
                            if (!consumable.Agent.Any(a => a.PEID == agent.PEID))
                                consumable.Agent.Add(agent);
                        }
                    }
                    productUsage.Consumables.Add(consumable);
                }
                return productUsage;
            }
            catch (Exception ex)
            {
                logger.Debug("ProcessProductUseageDyn");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return null;
        }
        public Cost CalculateCost(List<InkLevel> inkLevels, string printjob)
        {
            Cost cost = new Cost();
            try
            {
                var inkLevelsGroup = new List<InkLevel>();
                foreach (var item in inkLevels)
                {
                    if (item.PrintJob == printjob)
                    {
                        inkLevelsGroup.Add(item);
                    }
                }
                var inklevelsList = (from t in inkLevelsGroup
                                     orderby t.timestamp descending
                                     select t).Take(8).ToList();
                var listTop = (from t in inklevelsList
                               orderby t.timestamp descending, t.PEID
                               select t).Take(4).ToList();

                var listBottom = (from t in inklevelsList
                                  orderby t.timestamp ascending, t.PEID
                                  select t).Take(4).ToList();
                List<InkLevel> SortedTopList = listTop.OrderBy(o => o.PEID).ToList();
                List<InkLevel> SortedBottomList = listBottom.OrderBy(o => o.PEID).ToList();

                PopulateListBox(SortedTopList, SortedBottomList);

                double colorCartridgeCost = 0;// Properties.Settings.Default.ColorCartridgeCost;
                double.TryParse(viewModel.ColorInkCartridgeCost, out colorCartridgeCost);

                double colorMl = Properties.Settings.Default.Color_ml;

                double CostPerMlColor = colorCartridgeCost / colorMl;

                double blackCartridgeCost = 0;// Properties.Settings.Default.BlackCartridgeCost;
                double.TryParse(viewModel.BlackInkCartridgeCost, out blackCartridgeCost);

                double blackMl = Properties.Settings.Default.Black_ml;

                double CostPerMlBlack = blackCartridgeCost / blackMl;

                double cyanPlUse = (SortedTopList.ElementAt(1).Count -
                    SortedBottomList.ElementAt(1).Count) *
                    Properties.Settings.Default.ColorConst1 * Properties.Settings.Default.ColorConst2;
                cost.TotalCyanMlUsed = (cyanPlUse * Properties.Settings.Default.ColorConst3);
                double magentaPlUse = (SortedTopList.ElementAt(2).Count -
                    SortedBottomList.ElementAt(2).Count) *
                    Properties.Settings.Default.ColorConst1 * Properties.Settings.Default.ColorConst2;
                cost.TotalMagentaMlUsed = (magentaPlUse * Properties.Settings.Default.ColorConst3);
                double yellowPlUse = (SortedTopList.ElementAt(3).Count -
                    SortedBottomList.ElementAt(3).Count) *
                    Properties.Settings.Default.ColorConst1 * Properties.Settings.Default.ColorConst2;
                cost.TotalYellowMlUsed = (yellowPlUse * Properties.Settings.Default.ColorConst3);

                cost.TotalColorMlUsed = cost.TotalCyanMlUsed + cost.TotalMagentaMlUsed + cost.TotalYellowMlUsed;

                cost.TotalCostOfCyan = cost.TotalCyanMlUsed * CostPerMlColor;
                cost.TotalCostOfMagenta = cost.TotalMagentaMlUsed * CostPerMlColor;
                cost.TotalCostOfYellow = cost.TotalYellowMlUsed * CostPerMlColor;
                cost.TotalCostOfColor = cost.TotalColorMlUsed * CostPerMlColor;

                double blackPlUse = (SortedTopList.ElementAt(0).Count - SortedBottomList.ElementAt(0).Count) * Properties.Settings.Default.BlackConst1 * Properties.Settings.Default.BlackConst2;
                cost.TotalBlackMlUsed = (blackPlUse * Properties.Settings.Default.BlackConst3);
                cost.TotalCostOfBlack = cost.TotalBlackMlUsed * CostPerMlBlack;

                cost.TotalBlackLabels = SortedTopList.ElementAt(0).TotalImpressions - SortedBottomList.ElementAt(0).TotalImpressions;
                if (cost.TotalBlackLabels < 1)
                    cost.TotalBlackLabels = 1;
                cost.LabelCostOfBlack = cost.TotalCostOfBlack / cost.TotalBlackLabels;
                cost.TotalColorLabels = SortedTopList.ElementAt(1).TotalImpressions - SortedBottomList.ElementAt(1).TotalImpressions;
                if (cost.TotalColorLabels < 1)
                    cost.TotalColorLabels = 1;
                cost.LabelCostOfColor = cost.TotalCostOfColor / cost.TotalColorLabels;

                cost.LabelCostOfCyan = cost.TotalCostOfCyan / cost.TotalColorLabels;
                cost.LabelCostOfMagenta = cost.TotalCostOfMagenta / cost.TotalColorLabels;
                cost.LabelCostOfYellow = cost.TotalCostOfYellow / cost.TotalColorLabels;

                cost.PageCyanMlUsed = cost.TotalCyanMlUsed / cost.TotalColorLabels;
                cost.PageMagentaMlUsed = cost.TotalMagentaMlUsed / cost.TotalColorLabels;
                cost.PageYellowMlUsed = cost.TotalYellowMlUsed / cost.TotalColorLabels;
                cost.PageColorMlUsed = cost.TotalColorMlUsed / cost.TotalColorLabels;
                cost.PageBlackMlUsed = cost.TotalBlackMlUsed / cost.TotalBlackLabels;

                cost.CyanPagesPerCartridge = colorMl / cost.PageCyanMlUsed;
                cost.MagentaPagesPerCartridge = colorMl / cost.PageMagentaMlUsed;
                cost.YellowPagesPerCartridge = colorMl / cost.PageYellowMlUsed;
                cost.BlackPagesPerCartridge = blackMl / cost.PageBlackMlUsed;
            }
            catch (Exception ex)
            {
                logger.Debug("CalculateCost");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            cost.Printer = selectedPrinter;
            cost.Printer = printjob;
            return cost;
        }
    }
}
