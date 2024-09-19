using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Management;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Monitors;
using NLog;
using NLog.Targets;
using InkCostModel;
using PrinterCommandExecute;

namespace InkCostCalculator
{
    public partial class Main : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public List<Cost> jobs = new List<Cost>();
        public string outputFilePath = string.Empty;
        public string selectedPrinter = string.Empty;
        string selectedIp = string.Empty;
        bool changePrinter = false;

        PrintQueueMonitor pqm = null;

        string inkLevelsPath = string.Empty;
        string logFilesPath = string.Empty;

        bool win10 = false;
        bool win8 = false;

        Dictionary<int, JobInformation> JobStates = new Dictionary<int, JobInformation>();
        Dictionary<int, TimeSpan> JobTimeSpans = new Dictionary<int, TimeSpan>();
        List<int> JobStatesDelete = new List<int>();
        List<string> LogLines = new List<string>();
        List<string> JobStatesEndTrigger = new List<string>();

        System.Timers.Timer timer = new System.Timers.Timer();
        int timerSeconds = 0;

        System.Timers.Timer timerIdle = new System.Timers.Timer();

        System.Timers.Timer timerCaptureDelay = new System.Timers.Timer();

        bool captureInProgress = false;
        List<int> pausedJobs = new List<int>();

        private bool useLast = false;
        ProductUsage lastProductUsage = new ProductUsage();
        int lastQueueCont = 0;
        //private bool loadedLastCapture = false;
        private bool okToCapture = false;

        public Main()
        {
            InitializeComponent();

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
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

            //logger.Debug("Notes for decoding status and pdwChange");
            //logger.Debug("Status Paused - 0x0001, 1");
            //logger.Debug("Status Error - 0x0002, 2");
            //logger.Debug("Status Deleting - 0x0004,4 ");
            //logger.Debug("Status Spooling - 0x0008, 8");
            //logger.Debug("Status Printing - 0x0010, 16");
            //logger.Debug("Status Offline - 0x0020, 32");
            //logger.Debug("Status Paper Out - 0x0040, 64");
            //logger.Debug("Status Printed - 0x0080, 128");
            //logger.Debug("Status Deleted - 0x0100, 256");
            //logger.Debug("Status Blocked - 0x0200, 512");
            //logger.Debug("Status User Intervention - 0x0400, 1024");
            //logger.Debug("Status Restart - 0x0800, 2048");
            //logger.Debug("Status Complete - 0x1000, 4096");
            //logger.Debug("Status Retained - 0x2000, 8192");
            //logger.Debug("Status Rendering - 0x4000, 16384");
            //logger.Debug("Change - Add Job - 0x0100, 256");
            //logger.Debug("Change - Set Job - 0x0200, 512");
            //logger.Debug("Change - Delete Job - 0x0400, 1024");
            //logger.Debug("Change - Job - 0x0800, 2048");

            logger.Info("Starting");
            var ver = System.Environment.OSVersion;
            if (ver.Version.Major >= 10)
            {
                if (ver.Version.Minor >= 0)
                    win10 = true;
            }
            else if (ver.Version.Major >= 6)
            {
                if (ver.Version.Minor >= 2)
                    win8 = true;
            }

            logger.Info($"OS Version: Major {ver.Version.Major}, Major Revision {ver.Version.MajorRevision}, Minor {ver.Version.Minor}, Minor Revision {ver.Version.MinorRevision}, build {ver.Version.Build}, Revision {ver.Version.Revision}");

            ColorInkCartridgeCostTextBox.Text = config.AppSettings.Settings["ColorCartridgeCost"].Value;
            BlackInkCartridgeCostTextBox.Text = config.AppSettings.Settings["BlackCartridgeCost"].Value;

            selectedPrinter = config.AppSettings.Settings["SelectedPrinter"].Value;
            selectedIp = GetPrinterIp(selectedPrinter);
            SelectedPrinterNameLabel.Text = selectedPrinter;
            //ColorCartridgeMlTextBox.Text = Properties.Settings.Default.Color_ml.ToString();
            //BlackCartridgeMlTextBox.Text = Properties.Settings.Default.Black_ml.ToString();

            pqm = new PrintQueueMonitor();
            StartWatcher(config.AppSettings.Settings["SelectedPrinter"].Value.Trim());
            //if (!string.IsNullOrEmpty(selectedIp))
            //{
            //    if (!win8 && !win10)
            //    {
            //        CaptureStartEnable(false);
            //        CaptureEndEnable(false);
            //    }
            //    else
            //    {
            //        CaptureStartEnable(true);
            //        CaptureEndEnable(false);
            //    }
            //}
            //else
            //{
            //CaptureStartEnable(false);
            //CaptureEndEnable(false);
            CaptureEndEnableToolStrip(false);
            //}

            ShowHideAdvancedButton.Text = "Show Advanced Info";
            JobsListBoxLabel.Visible = false;
            JobsListBox.Visible = false;
            RawLabel.Visible = false;
            RawListBox.Visible = false;
            this.Width = 1001;
            this.Height = 309;

            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            timerIdle.Interval = Properties.Settings.Default.IdleMilliSeconds;
            timerIdle.Elapsed += TimerIdle_Elapsed;

            timerCaptureDelay.Interval = Properties.Settings.Default.DelayMilliSeconds;
            timerCaptureDelay.Elapsed += TimerCaptureDelay_Elapsed;

        }

        private void TimerCaptureDelay_Elapsed(object sender, ElapsedEventArgs e)
        {
            okToCapture = true;
            timerCaptureDelay.Stop();
        }

        private void TimerIdle_Elapsed(object sender, ElapsedEventArgs e)
        {
            useLast = false;
        }

        string GetPrinterIp(string selectedPrinter)
        {
            string ip = string.Empty;
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
            return ip;
        }
        void ArchiveInkLevelFile(string pOutputFilePath)
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
        void ArchiveLogFileFile(string pOutputFilePath)
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

        void StartWatcher(string selectedPrinter)
        {
            logger.Debug("Starting Queue Monitor");
            if (!changePrinter)
            {
                pqm.OnJobStatusChange += new PrintJobStatusChanged(pqm_OnJobStatusChange);
                logger.Debug("Subscribed to Monitor Status Change");
                pqm.OnJobLogChange += new PrintJobLogChanged(pqm_OnJobLogChange);
                logger.Debug("Subscribed to Monitor Logging");
            }
            pqm.Start(selectedPrinter);
            changePrinter = false;
            useLast = false;
        }

        void StopWatcher(string selectedPrinter)
        {
            logger.Debug("Stop Watcher Method");
            pqm.OnJobStatusChange -= new PrintJobStatusChanged(pqm_OnJobStatusChange);
            logger.Debug("Unsubscribed to Monitor Status Change");
            pqm.OnJobLogChange -= new PrintJobLogChanged(pqm_OnJobLogChange);
            logger.Debug("Unsubscribed to Monitor Logging");
        }

        void pqm_OnJobLogChange(object Sender, PrintJobLogEventArgs e)
        {
            if ((e.Stat != 1) && (e.Stat != 2) && (e.PdwChange != 256))
            {
                logger.Debug($"Jobs in Queue = {e.QueueCount} Job Id = {e.JobID}, Status char = {e.Stat}, Pdw Change = {e.PdwChange.ToString()} at {DateTime.Now.ToString()}");
            }
            timerSeconds = 0;
            EnableChangePrinter(false);
        }

        void pqm_OnJobStatusChange(object Sender, PrintJobChangeEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    if (string.IsNullOrEmpty(selectedIp)) //local USB printer
                    {
                        #region USB Printer
                        if ((e.Stat != 1) && (e.Stat != 2) && (e.PdwChange != 256))
                        {
                            //if (e.QueueCount > 1)
                            //{
                            //    if (loadedLastCapture)
                            //        useLast = true;
                            //    else
                            //        useLast = false;
                            //}
                            //else
                            //    useLast = false;
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
                                                okToCapture = false;
                                                captureInProgress = true;
                                                logger.Info($"********* JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                                string printJob = $"{e.JobID}_{e.Name}_{e.JobSubmitted.Hour.ToString()}{e.JobSubmitted.Minute.ToString()}{e.JobSubmitted.Second.ToString()}";
                                                manualName = printJob;
                                                manualJobId = e.JobID;
                                                //CaptureEndEnable(true);
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
                                //if (e.QueueCount > 1)
                                //    useLast = true;
                                #region Running
                                if (JobStates[e.JobID].State == "Start")
                                {
                                    if (e.Job)
                                    {
                                        timerCaptureDelay.Start();
                                        JobStates[e.JobID].State = "Running";
                                        logger.Info($"********* JobId: {e.JobID} Job Running at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                    }
                                }
                                #endregion
                                #region End Capture
                                if ((JobStates[e.JobID].State == "Running") && okToCapture)
                                {
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
                                    if (win10)
                                    {
                                        #region win 10
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
                                //if (e.QueueCount > 1)
                                //{
                                //    if (loadedLastCapture)
                                //        useLast = true;
                                //    else
                                //        useLast = false;
                                //}
                                //else
                                //    useLast = false;
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
                                                    okToCapture = false;
                                                    captureInProgress = true;
                                                    logger.Info($"********* JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                                    string printJob = $"{e.JobID}_{e.Name}_{e.JobSubmitted.Hour.ToString()}{e.JobSubmitted.Minute.ToString()}{e.JobSubmitted.Second.ToString()}";
                                                    manualName = printJob;
                                                    manualJobId = e.JobID;
                                                    //CaptureEndEnable(true);
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
                                    //if (e.QueueCount > 1)
                                    //    useLast = true;
                                    #region Running
                                    if (JobStates[e.JobID].State == "Start")
                                    {
                                        if (e.Job)
                                        {
                                            timerCaptureDelay.Start();
                                            JobStates[e.JobID].State = "Running";
                                            logger.Info($"********* JobId: {e.JobID} Job Running at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                        }
                                    }
                                    #endregion
                                    #region End Capture
                                    if ((JobStates[e.JobID].State == "Running") && okToCapture)
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
                                //if (e.QueueCount > 1)
                                //{
                                //    if (loadedLastCapture)
                                //        useLast = true;
                                //    else
                                //        useLast = false;
                                //}
                                //else
                                //    useLast = false;
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
                                                    okToCapture = false;
                                                    captureInProgress = true;
                                                    logger.Info($"********* JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                                    string printJob = $"{e.JobID}_{e.Name}_{e.JobSubmitted.Hour.ToString()}{e.JobSubmitted.Minute.ToString()}{e.JobSubmitted.Second.ToString()}";
                                                    manualName = printJob;
                                                    manualJobId = e.JobID;
                                                    //CaptureEndEnable(true);
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
                                    //if (e.QueueCount > 1)
                                    //    useLast = true;
                                    #region Running
                                    if (JobStates[e.JobID].State == "Start")
                                    {
                                        if (e.Job)
                                        {
                                            timerCaptureDelay.Start();
                                            JobStates[e.JobID].State = "Running";
                                            logger.Info($"********* JobId: {e.JobID} Job Running at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                        }
                                    }
                                    #endregion
                                    #region End Capture
                                    if ((JobStates[e.JobID].State == "Running") && okToCapture)
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
                        #region win 10
                        if (win10)
                        {
                            if ((e.Stat != 1) && (e.Stat != 2) && (e.PdwChange != 256))
                            {
                                //if (e.QueueCount > 1)
                                //{
                                //    if (loadedLastCapture)
                                //        useLast = true;
                                //    else
                                //        useLast = false;
                                //}
                                //else
                                //    useLast = false;
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
                                                    okToCapture = false;
                                                    captureInProgress = true;
                                                    logger.Info($"********* JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                                    string printJob = $"{e.JobID}_{e.Name}_{e.JobSubmitted.Hour.ToString()}{e.JobSubmitted.Minute.ToString()}{e.JobSubmitted.Second.ToString()}";
                                                    manualName = printJob;
                                                    manualJobId = e.JobID;
                                                    //CaptureEndEnable(true);
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
                                    //if (e.QueueCount > 1)
                                    //    useLast = true;
                                    #region Running
                                    if (JobStates[e.JobID].State == "Start")
                                    {
                                        if (e.Job)
                                        {
                                            timerCaptureDelay.Start();
                                            JobStates[e.JobID].State = "Running";
                                            logger.Info($"********* JobId: {e.JobID} Job Running at {DateTime.Now.ToString()}, Change:{e.PdwChange}, Stat:{e.Stat}");
                                        }
                                    }
                                    #endregion
                                    #region End Capture
                                    if ((JobStates[e.JobID].State == "Running") && okToCapture)
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
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    logger.Error($"{ex.Message}. {ex.InnerException}");
                }
            });
        }
        private void InkCostMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            pqm.OnJobStatusChange -= new PrintJobStatusChanged(pqm_OnJobStatusChange);
            logger.Debug("Unsubscribed to Monitor Status Change");
            pqm.OnJobLogChange -= new PrintJobLogChanged(pqm_OnJobLogChange);
            logger.Debug("Unsubscribed to Monitor Logging");
        }

        private void ExportJobListButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter file = new StreamWriter($"{outputFilePath}jobs_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.csv", false))
                {
                    file.WriteLine(Properties.Settings.Default.JobFileHeader);
                    string totalCost = string.Empty;
                    string costPerLabel = string.Empty;
                    foreach (Cost job in jobs)
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
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        public void PopulateJobListBox()
        {
            logger.Info($"#########Updating Job List Box");
            try
            {
                JobsListBox.Items.Clear();
                JobsListBox.Items.Add(Properties.Settings.Default.JobFileHeader);
                string totalCost = string.Empty;
                string costPerLabel = string.Empty;
                foreach (var job in jobs)
                {
                    totalCost = (job.TotalCostOfCyan + job.TotalCostOfMagenta + job.TotalCostOfYellow + job.TotalCostOfBlack).ToString("0.#####");
                    costPerLabel = ((job.TotalCostOfCyan + job.TotalCostOfMagenta + job.TotalCostOfYellow + job.TotalCostOfBlack) / job.TotalColorLabels).ToString("0.#####");

                    JobsListBox.Items.Add($"{job.Printer},{job.PrintJob},{job.TotalColorLabels},{job.LabelCostOfCyan.ToString("0.#####")},{job.TotalCostOfCyan.ToString("0.#####")},{job.TotalCyanMlUsed.ToString("0.#####")},{job.CyanPagesPerCartridge.ToString("0.#")},{job.LabelCostOfMagenta.ToString("0.#####")},{job.TotalCostOfMagenta.ToString("0.#####")},{job.TotalMagentaMlUsed.ToString("0.#####")},{job.MagentaPagesPerCartridge.ToString("0.#")},{job.LabelCostOfYellow.ToString("0.#####")},{job.TotalCostOfYellow.ToString("0.#####")},{job.TotalYellowMlUsed.ToString("0.#####")},{job.YellowPagesPerCartridge.ToString("0.#")},{job.LabelCostOfBlack.ToString("0.#####")},{job.TotalCostOfBlack.ToString("0.#####")},{job.TotalBlackMlUsed.ToString("0.#####")},{job.BlackPagesPerCartridge.ToString("0.#")},{totalCost},{costPerLabel}");
                }
                JobsListBox.SelectedIndex = JobsListBox.Items.Count - 1;
            }
            catch (Exception ex)
            {
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        public void PopulateListBox(List<InkLevel> top, List<InkLevel> bottom)
        {
            try
            {
                RawListBox.Items.Clear();
                RawListBox.Items.Add("Current Read");
                foreach (var item in top)
                {
                    RawListBox.Items.Add($"{item.timestamp},{item.Color},{item.TotalImpressions},{item.DuplexSheets},{item.JamEvents},{item.MispickEvents},{item.PEID},{item.Count},{item.Type}");
                }
                RawListBox.Items.Add("Previous Read");
                foreach (var item in bottom)
                {
                    RawListBox.Items.Add($"{item.timestamp},{item.Color},{item.TotalImpressions},{item.DuplexSheets},{item.JamEvents},{item.MispickEvents},{item.PEID},{item.Count},{item.Type}");
                }
            }
            catch (Exception ex)
            {
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

        //delegate void CaptureStartEnableDelegate(bool enable);
        //private void CaptureStartEnable(bool enable)
        //{
        //    if (this.CaptureStartButton.InvokeRequired)
        //    {
        //        CaptureStartEnableDelegate d = new CaptureStartEnableDelegate(CaptureStartEnable);
        //        this.Invoke(d, new object[] { enable });
        //    }
        //    else
        //    {
        //        CaptureStartButton.Enabled = enable;
        //        CaptureStartButton.Visible = enable;
        //    }
        //}
        //delegate void CaptureEndEnableDelegate(bool enable);
        //private void CaptureEndEnable(bool enable)
        //{
        //    if (this.CaptureEndButton.InvokeRequired)
        //    {
        //        CaptureEndEnableDelegate d = new CaptureEndEnableDelegate(CaptureEndEnable);
        //        this.Invoke(d, new object[] { enable });
        //    }
        //    else
        //    {
        //        CaptureEndButton.Enabled = enable;
        //        CaptureEndButton.Visible = enable;
        //    }
        //}
        private void costToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double colorCartridgeCost = 0;// Properties.Settings.Default.ColorCartridgeCost;
            double.TryParse(ColorInkCartridgeCostTextBox.Text, out colorCartridgeCost);
            double colorMl = Properties.Settings.Default.Color_ml;
            double blackCartridgeCost = 0;// Properties.Settings.Default.ColorCartridgeCost;
            double.TryParse(BlackInkCartridgeCostTextBox.Text, out blackCartridgeCost);
            double blackMl = Properties.Settings.Default.Black_ml;

            ConfigurationDialog dialog = new ConfigurationDialog(colorCartridgeCost.ToString(),
                                                                    Properties.Settings.Default.Color_ml.ToString(),
                                                                    blackCartridgeCost.ToString(),
                                                                    Properties.Settings.Default.Black_ml.ToString());
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                double.TryParse(dialog.ColorCost, out colorCartridgeCost);
                double.TryParse(dialog.ColorMl, out colorMl);
                double.TryParse(dialog.BlackCost, out blackCartridgeCost);
                double.TryParse(dialog.BlackMl, out blackMl);

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                config.AppSettings.Settings.Remove("ColorCartridgeCost");
                config.AppSettings.Settings.Add("ColorCartridgeCost", colorCartridgeCost.ToString());
                config.AppSettings.Settings.Remove("BlackCartridgeCost");
                config.AppSettings.Settings.Add("BlackCartridgeCost", blackCartridgeCost.ToString());
                config.Save(ConfigurationSaveMode.Modified);
                ColorInkCartridgeCostTextBox.Text = config.AppSettings.Settings["ColorCartridgeCost"].Value;
                BlackInkCartridgeCostTextBox.Text = config.AppSettings.Settings["BlackCartridgeCost"].Value;
            }
        }

        string manualName = string.Empty;
        private int manualJobId = 0;
        //private void CaptureStartButton_Click(object sender, EventArgs e)
        //{
        //    //CaptureStartEnable(false);

        //    //manualName = $"Manual{DateTime.Now.Day}{DateTime.Now.Month}{DateTime.Now.Year}{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}";
        //    //InkCapture(manualName);

        //    //CaptureEndEnable(true);
        //}

        //private void CaptureEndButton_Click(object sender, EventArgs e)
        //{
        //    //CaptureEndEnable(false);

        //    //InkCapture(manualName, true);
        //    //InkCalculate(manualName, manualJobId);

        //    //CaptureStartEnable(true);
        //}
        delegate void CaptureEndEnableToolStripDelegate(bool enable);
        private void CaptureEndEnableToolStrip(bool enable)
        {
            if (this.menuStrip1.InvokeRequired)
            {
                CaptureEndEnableToolStripDelegate d = new CaptureEndEnableToolStripDelegate(CaptureEndEnableToolStrip);
                this.Invoke(d, new object[] { enable });
            }
            else
            {
                endCaptureToolStripMenuItem.Enabled = enable;
            }
        }
        private void endCaptureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CaptureEndEnableToolStrip(false);

            InkCapture(manualName, true);
            InkCalculate(manualName, manualJobId);
        }
        private void ShowHideAdvancedButton_Click(object sender, EventArgs e)
        {
            if (ShowHideAdvancedButton.Text.Contains("Show"))
            {
                ShowHideAdvancedButton.Text = "Hide Advanced Info";
                JobsListBoxLabel.Visible = true;
                JobsListBox.Visible = true;
                RawLabel.Visible = true;
                RawListBox.Visible = true;
                this.Width = 1001;
                this.Height = 718;
            }
            else if (ShowHideAdvancedButton.Text.Contains("Hide"))
            {
                ShowHideAdvancedButton.Text = "Show Advanced Info";
                JobsListBoxLabel.Visible = false;
                JobsListBox.Visible = false;
                RawLabel.Visible = false;
                RawListBox.Visible = false;
                this.Width = 1001;
                this.Height = 309;
            }
        }
        private void printerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrinterConfig printerConfiguration = new PrinterConfig($"{selectedPrinter}",
                                                         $"{outputFilePath}");

            if (printerConfiguration.ShowDialog() == DialogResult.OK)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                config.AppSettings.Settings.Remove("SelectedPrinter");
                config.AppSettings.Settings.Add("SelectedPrinter", printerConfiguration.SelectedPrinter);
                config.AppSettings.Settings.Remove("OutputFilePath");
                config.AppSettings.Settings.Add("OutputFilePath", printerConfiguration.PathToOutput);
                config.Save(ConfigurationSaveMode.Modified);
                selectedPrinter = config.AppSettings.Settings["SelectedPrinter"].Value;
                outputFilePath = config.AppSettings.Settings["OutputFilePath"].Value;
                SelectedPrinterNameLabel.Text = selectedPrinter;
                selectedIp = GetPrinterIp(selectedPrinter);
                changePrinter = true;
                //if (!string.IsNullOrEmpty(selectedIp))
                //{
                //    CaptureStartEnable(true);
                //    CaptureEndEnable(false);
                //}
                //else
                //{
                //    CaptureStartEnable(false);
                //    CaptureEndEnable(false);
                //}
                //StopWatcher(selectedPrinter);
                StartWatcher(config.AppSettings.Settings["SelectedPrinter"].Value.Trim());
            }
        }
        delegate void EnableChangePrinterDelegate(bool enable);
        private void EnableChangePrinter(bool enable)
        {
            if (this.menuStrip1.InvokeRequired)
            {
                EnableChangePrinterDelegate d = new EnableChangePrinterDelegate(EnableChangePrinter);
                this.Invoke(d, new object[] { enable });
            }
            else
            {
                printerToolStripMenuItem.Enabled = enable;
            }
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
        private string InkCapture(string printjob, bool captureNew)
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
                        ProductUsage productUse = ProcessProductUsageDyn($"{outputFilePath}ProductUsageDyn.xml");
                        processInkLevel(printjob, productUse);
                    }
                }
                else
                {
                    logger.Info($"#########Capturing using last value {printjob} at {DateTime.Now.ToString()}");
                    processInkLevel(printjob, lastProductUsage);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return $"{printjob}";
        }
        public void processInkLevel(string printjob, ProductUsage productUse)
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
        private void InkCalculate(string printjob, int currentJobId)
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
                    CyanCostTextBox.Text = cost.TotalCostOfCyan.ToString("0.#####");
                    MagentaCostTextBox.Text = cost.TotalCostOfMagenta.ToString("0.#####");
                    YellowCostTextBox.Text = cost.TotalCostOfYellow.ToString("0.#####");
                    BlackCostTextBox.Text = cost.TotalCostOfBlack.ToString("0.#####");

                    LabelCyanCostTextBox.Text = cost.LabelCostOfCyan.ToString("0.#####");
                    LabelMagentaCostTextBox.Text = cost.LabelCostOfMagenta.ToString("0.#####");
                    LabelYellowCostTextBox.Text = cost.LabelCostOfYellow.ToString("0.#####");
                    LabelBlackCostTextBox.Text = cost.LabelCostOfBlack.ToString("0.#####");

                    CyanMlTextBox.Text = cost.TotalCyanMlUsed.ToString("0.#####");
                    MagentaMlTextBox.Text = cost.TotalMagentaMlUsed.ToString("0.#####");
                    YellowMlTextBox.Text = cost.TotalYellowMlUsed.ToString("0.#####");
                    BlackMlTextBox.Text = cost.TotalBlackMlUsed.ToString("0.#####");

                    CyanPagesPerCartridgeTextBox.Text = cost.CyanPagesPerCartridge.ToString("0.#");
                    MagentaPagesPerCartridgeTextBox.Text = cost.MagentaPagesPerCartridge.ToString("0.#");
                    YellowPagesPerCartridgeTextBox.Text = cost.YellowPagesPerCartridge.ToString("0.#");
                    BlackPagesPerCartridgeTextBox.Text = cost.BlackPagesPerCartridge.ToString("0.#");

                    TotalLabelsTextBox.Text = cost.TotalColorLabels.ToString();
                    TotalCostTextBox.Text = (cost.TotalCostOfColor + cost.TotalCostOfBlack).ToString("0.#####");
                    CostPerLabelTextBox.Text = ((cost.TotalCostOfColor + cost.TotalCostOfBlack) / cost.TotalColorLabels).ToString("0.#####");
                    jobs.Add(cost);
                    if (jobs.Count > 20)
                    {
                        jobs.RemoveAt(0);
                    }
                    PopulateJobListBox();
                }
                else
                    logger.Info("No Ink Level xml file or path exists");

                captureInProgress = false;
                useLast = true;
                lastQueueCont = 0;
                //loadedLastCapture = true;
                //CaptureEndEnable(false);
                CaptureEndEnableToolStrip(false);
                timerIdle.Start();
            }
            catch (Exception ex)
            {
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
                productUsage.TotalImpressions = usage.PrintApplicationSubunit.TotalImpressions.Value;
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
                        //if ((markingAgentCount.MarkingAgentCountType == "HPDropsCount") || (markingAgentCount.MarkingAgentCountType == "LDWDropsCount"))
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
                double.TryParse(ColorInkCartridgeCostTextBox.Text, out colorCartridgeCost);

                double colorMl = Properties.Settings.Default.Color_ml;
                //double.TryParse(ColorCartridgeMlTextBox.Text, out colorMl);

                double CostPerMlColor = colorCartridgeCost / colorMl;

                double blackCartridgeCost = 0;// Properties.Settings.Default.BlackCartridgeCost;
                double.TryParse(BlackInkCartridgeCostTextBox.Text, out blackCartridgeCost);

                double blackMl = Properties.Settings.Default.Black_ml;
                //double.TryParse(BlackCartridgeMlTextBox.Text, out blackMl);

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
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            cost.Printer = selectedPrinter;
            cost.Printer = printjob;
            return cost;
        }

    }
}
