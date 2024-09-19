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
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using System.Globalization;

using Monitors;
using NLog;
using NLog.Targets;
using System.Management;

namespace InkCostCalculator
{
    public partial class InkCostMain : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public List<Cost> jobs = new List<Cost>();
        public string outputFilePath = string.Empty;
        public string selectedPrinter = string.Empty;
        bool changePrinter = false;

        PrintQueueMonitor pqm = null;

        string inkLevelsPath = string.Empty;
        string logFilesPath = string.Empty;

        bool win10 = false;
        bool win8 = false;

        Dictionary<int, JobInformation> JobStates = new Dictionary<int, JobInformation>();
        List<int> JobStatesDelete = new List<int>();
        List<string> LogLines = new List<string>();

        System.Timers.Timer timer = new System.Timers.Timer();
        int timerSeconds = 0;

        System.Timers.Timer timerIdle = new System.Timers.Timer();

        bool captureInProgress = false;
        List<int> pausedJobs = new List<int>();

        private bool useLast = false;
        ProductUsage lastProductUsage = new ProductUsage();
        int lastQueueCont = 0;

        public InkCostMain()
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
            SelectedPrinterNameLabel.Text = selectedPrinter;
            //ColorCartridgeMlTextBox.Text = Properties.Settings.Default.Color_ml.ToString();
            //BlackCartridgeMlTextBox.Text = Properties.Settings.Default.Black_ml.ToString();
            pqm = new PrintQueueMonitor();


            StartWatcher(config.AppSettings.Settings["SelectedPrinter"].Value.Trim());

            CaptureEndEnableToolStrip(false);

            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            timerIdle.Interval = Properties.Settings.Default.IdleMilliSeconds;
            timerIdle.Elapsed += TimerIdle_Elapsed;
        }

        private void TimerIdle_Elapsed(object sender, ElapsedEventArgs e)
        {
            useLast = false;
        }

        void ArchiveInkLevelFile(string pOutputFilePath)
        {
            if (File.Exists($"{inkLevelsPath}\\inklevels.xml"))
            {
                File.Move($"{inkLevelsPath}\\inklevels.xml", $"{inkLevelsPath}\\inklevels_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.xml");
                foreach (var fi in new DirectoryInfo(inkLevelsPath).GetFiles().OrderByDescending(x => x.LastWriteTime).Skip(5))
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
            //if (string.IsNullOrEmpty(selectedIp))
            //    pqm.Start(selectedPrinter, false);
            //else
            //    pqm.Start(selectedPrinter, true);
            pqm.Start(selectedPrinter,false);

            changePrinter = false;
            useLast = false;
        }

        void StopWatcher(string selectedPrinter)
        {
            pqm.OnJobStatusChange -= new PrintJobStatusChanged(pqm_OnJobStatusChange);
            pqm.OnJobLogChange -= new PrintJobLogChanged(pqm_OnJobLogChange);
        }

        //private bool loadedLastCapture = false;
        void pqm_OnJobLogChange(object Sender, PrintJobLogEventArgs e)
        {
            if ((e.Stat != 1) && (e.Stat != 2) && (e.PdwChange != 256))
            {
                int result = e.PdwChange & ~256;
                logger.Debug($"Jobs in Queue = {e.QueueCount} Job Id = {e.JobID}, Status char = {e.Stat}, Pdw Change = {result.ToString()} at {DateTime.Now.ToString()}");
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

                        int result = e.PdwChange & ~256;
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
                                            captureInProgress = true;
                                            logger.Info($"********* JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{result}, Stat:{e.Stat}");
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
                                            logger.Error($"{ex.Message}. JobId: {e.JobID} Start Capture at {DateTime.Now.ToString()}, Change:{result}, Stat:{e.Stat}");
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
                                    JobStates[e.JobID].State = "Running";
                                    logger.Info($"********* JobId: {e.JobID} Job Running at {DateTime.Now.ToString()}, Change:{result}, Stat:{e.Stat}");
                                }
                            }
                            #endregion
                            #region End Capture
                            if (JobStates[e.JobID].State == "Running")
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
                                                logger.Info($"********* JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{result}, Stat:{e.Stat}");

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
                                                logger.Info($"********* JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{result}, Stat:{e.Stat}");

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
                                                logger.Info($"********* JobId: {e.JobID} Job Completed at {DateTime.Now.ToString()}, Capture Job End, Change:{result}, Stat:{e.Stat}");

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
                catch (Exception ex)
                {
                    logger.Error($"{ex.Message}. {ex.InnerException}");
                }
            });
        }

        private void InkCostMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            pqm.OnJobStatusChange -= new PrintJobStatusChanged(pqm_OnJobStatusChange);
        }

        private void ExportJobListButton_Click(object sender, EventArgs e)
        {
            try
            {
                var path = outputFilePath;
                FolderBrowserDialog folderBrowse = new FolderBrowserDialog();
                //Environment.SpecialFolder.
                folderBrowse.SelectedPath = path;

                if (folderBrowse.ShowDialog() == DialogResult.OK)
                {
                    path = folderBrowse.SelectedPath;
                }
                using (StreamWriter file = new StreamWriter($"{path}jobs_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.csv", false))
                //using (StreamWriter file = new StreamWriter($"{outputFilePath}jobs_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.csv", false))
                {
                    file.WriteLine(Properties.Settings.Default.JobFileHeader);
                    foreach (Cost job in jobs)
                    {
                        file.WriteLine($"{job.Printer},{job.PrintJob},{job.LabelCostOfColor.ToString("0.#####")},{job.TotalCostOfColor.ToString("0.#####")},{job.TotalColorLabels},{job.TotalColorMlUsed.ToString("0.#####")},{job.LabelCostOfBlack.ToString("0.#####")},{job.TotalCostOfBlack.ToString("0.#####")},{job.TotalBlackLabels},{job.TotalBlackMlUsed.ToString("0.#####")}");
                    }
                    file.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        private string InkCapture(string printjob, bool captureNew)
        {
            try
            {
                if (captureNew)
                {
                    logger.Info($"#########Capturing New {printjob} at {DateTime.Now.ToString()}");
                    RunEXE(selectedPrinter, "ProductUsageDyn");
                    System.Threading.Thread.Sleep(1000);
                    if (File.Exists($"{outputFilePath}ProductUsageDyn.xml"))
                    {
                        ProductUsage productUse = ProcessProductUsageDyn($"{outputFilePath}ProductUsageDyn.xml");
                        processInkLevel(printjob, productUse);
                    }
                    else
                        logger.Info("No Product Usage xml file or path exists");
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
            catch(Exception ex)
            {
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }
        
        private void InkCalculate(object _printjob, int currentJobId)
        {
            string printjob = (string)_printjob;
            logger.Info($"#########Calculating {printjob} at {DateTime.Now.ToString()}");
            try
            {
                if (File.Exists($"{inkLevelsPath}\\inklevels.xml"))
                {
                    List<InkLevel> inkLevels = ReadFile($"{inkLevelsPath}\\inklevels.xml");
                    Cost cost = CalculateCost(inkLevels, printjob);
                    cost.PrintJob = printjob;
                    cost.Printer = selectedPrinter;
                    ColorCostTextBox.Text = cost.TotalCostOfColor.ToString("0.#####");
                    BlackCostTextBox.Text = cost.TotalCostOfBlack.ToString("0.#####");

                    TotalLabelsTextBox.Text = cost.TotalColorLabels.ToString();

                    LabelColorCostTextBox.Text = cost.LabelCostOfColor.ToString("0.#####");
                    LabelBlackCostTextBox.Text = cost.LabelCostOfBlack.ToString("0.#####");

                    ColorMlTextBox.Text = cost.TotalColorMlUsed.ToString("0.#####");
                    BlackMlTextBox.Text = cost.TotalBlackMlUsed.ToString("0.#####");

                    TotalCostTextBox.Text = (cost.TotalCostOfColor + cost.TotalCostOfBlack).ToString("0.#####");
                    if (cost.ColorPagesPerCartridge < cost.BlackPagesPerCartridge)
                    {
                        ColorPagesPerCartridgeTextBox.Text = cost.ColorPagesPerCartridge.ToString("0.#");
                    }
                    else
                    {
                        ColorPagesPerCartridgeTextBox.Text = cost.BlackPagesPerCartridge.ToString("0.#");
                    }
                    
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
                //loadedLastCapture = true;
                useLast = true;
                lastQueueCont = 0;
                CaptureEndEnableToolStrip(false);
                timerIdle.Start();
            }
            catch (Exception ex)
            {
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        public void RunEXE(string printer, string commandType)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo($"{Properties.Settings.Default.SWFWClientPath}SWFWClient.exe");
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            switch (commandType)
            {
                case "ProductUsageDyn":
                    {
                        startInfo.Arguments = $"\"{printer}\" get \"{Properties.Settings.Default.ProductUsageDyn}\" \"{outputFilePath}httpresponse.txt\" \"{outputFilePath}ProductUsageDyn.xml\"";
                    }
                    break;
                case "ConsumableConfigDyn":
                    {
                        startInfo.Arguments = $"\"{printer}\" get \"{Properties.Settings.Default.ConsumabeConfigDyn}\" \"{outputFilePath}httpresponse.txt\" \"{outputFilePath}ConsumableConfigDyn.xml\"";
                    }
                    break;
            }
            Process process = Process.Start(startInfo);
            while (!process.HasExited)
            {
                System.Threading.Thread.Sleep(100);
            }
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
                                     select t).Take(14).ToList();

                var listTop = (from t in inklevelsList
                               orderby t.timestamp descending, t.PEID
                               select t).Take(7).ToList();

                var listBottom = (from t in inklevelsList
                                  orderby t.timestamp ascending, t.PEID
                                  select t).Take(7).ToList();
                List<InkLevel> SortedTopList = listTop.OrderBy(o => o.PEID).ToList();
                List<InkLevel> SortedBottomList = listBottom.OrderBy(o => o.PEID).ToList();

                PopulateListBox(SortedTopList, SortedBottomList);

                double colorCartridgeCost = 0;// Properties.Settings.Default.ColorCartridgeCost;
                double.TryParse(ColorInkCartridgeCostTextBox.Text, out colorCartridgeCost);

                //ColorCartridgeMlTextBox.Text = Properties.Settings.Default.Color_ml.ToString();
                //BlackCartridgeMlTextBox.Text = Properties.Settings.Default.Black_ml.ToString();
                double colorMl = Properties.Settings.Default.Color_ml;
                //double.TryParse(ColorCartridgeMlTextBox.Text, out colorMl);

                double CostPerMlColor = colorCartridgeCost / colorMl;

                double blackCartridgeCost = 0;// Properties.Settings.Default.BlackCartridgeCost;
                double.TryParse(BlackInkCartridgeCostTextBox.Text, out blackCartridgeCost);

                double blackMl = Properties.Settings.Default.Black_ml;
                //double.TryParse(BlackCartridgeMlTextBox.Text, out blackMl);

                double CostPerMlBlack = blackCartridgeCost / blackMl;

                double cyanPlUse = ((SortedTopList.ElementAt(1).Count + SortedTopList.ElementAt(4).Count) -
                    (SortedBottomList.ElementAt(1).Count + SortedBottomList.ElementAt(4).Count)) *
                    Properties.Settings.Default.ColorConst1 * Properties.Settings.Default.ColorConst2;
                cost.TotalCyanMlUsed = (cyanPlUse * Properties.Settings.Default.ColorConst3);
                double magentaPlUse = ((SortedTopList.ElementAt(2).Count + SortedTopList.ElementAt(5).Count) -
                    (SortedBottomList.ElementAt(2).Count + SortedBottomList.ElementAt(5).Count)) *
                    Properties.Settings.Default.ColorConst1 * Properties.Settings.Default.ColorConst2;
                cost.TotalMagentaMlUsed = (magentaPlUse * Properties.Settings.Default.ColorConst3);
                double yellowPlUse = ((SortedTopList.ElementAt(3).Count + SortedTopList.ElementAt(6).Count) -
                    (SortedBottomList.ElementAt(3).Count + SortedBottomList.ElementAt(6).Count)) *
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

                cost.PageCyanMlUsed = cost.TotalCyanMlUsed / cost.TotalColorLabels;
                cost.PageMagentaMlUsed = cost.TotalMagentaMlUsed / cost.TotalColorLabels;
                cost.PageYellowMlUsed = cost.TotalYellowMlUsed / cost.TotalColorLabels;
                cost.PageColorMlUsed = cost.TotalColorMlUsed / cost.TotalColorLabels;
                cost.PageBlackMlUsed = cost.TotalBlackMlUsed / cost.TotalBlackLabels;

                cost.CyanPagesPerCartridge = (colorMl / 3) / cost.PageCyanMlUsed;
                cost.MagentaPagesPerCartridge = (colorMl / 3) / cost.PageMagentaMlUsed;
                cost.YellowPagesPerCartridge = (colorMl / 3) / cost.PageYellowMlUsed;

                if ((cost.CyanPagesPerCartridge < cost.MagentaPagesPerCartridge) & (cost.CyanPagesPerCartridge < cost.YellowPagesPerCartridge))
                {
                    cost.ColorPagesPerCartridge = cost.CyanPagesPerCartridge;
                }
                else if ((cost.MagentaPagesPerCartridge < cost.CyanPagesPerCartridge) & (cost.MagentaPagesPerCartridge < cost.YellowPagesPerCartridge))
                {
                    cost.ColorPagesPerCartridge = cost.MagentaPagesPerCartridge;
                }
                else if ((cost.YellowPagesPerCartridge < cost.MagentaPagesPerCartridge) & (cost.YellowPagesPerCartridge < cost.CyanPagesPerCartridge))
                {
                    cost.ColorPagesPerCartridge = cost.YellowPagesPerCartridge;
                }
                else
                    cost.ColorPagesPerCartridge = cost.YellowPagesPerCartridge;

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

        public void PopulateJobListBox()
        {
            logger.Info($"#########Updating Job List Box");
            //logger.Info("###################################");
            //logger.Info("");
            try
            {
                JobsListBox.Items.Clear();
                JobsListBox.Items.Add(Properties.Settings.Default.JobFileHeader);
                foreach (var job in jobs)
                {
                    JobsListBox.Items.Add($"{job.Printer},{job.PrintJob},{job.LabelCostOfColor.ToString("0.#####")},{job.TotalCostOfColor.ToString("0.#####")},{job.TotalColorLabels},{job.TotalColorMlUsed.ToString("0.#####")},{job.LabelCostOfBlack.ToString("0.#####")},{job.TotalCostOfBlack.ToString("0.#####")},{job.TotalBlackLabels},{job.TotalBlackMlUsed.ToString("0.#####")}");
                }
                JobsListBox.SelectedIndex = JobsListBox.Items.Count - 1;
            }
            catch (Exception ex)
            {
                logger.Error($"{ex.Message}. {ex.InnerException}");
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
                        if ((markingAgentCount.MarkingAgentCountType == "HPDropsCount") || (markingAgentCount.MarkingAgentCountType == "LDWDropsCount"))
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
        public class JobInformation
        {
            public string State { get; set; }
            public string Name { get; set; }
        }
        public class Cost
        {
            public string Printer { get; set; }
            public string PrintJob { get; set; }

            public double TotalCostOfCyan { get; set; }
            public double TotalCostOfMagenta { get; set; }
            public double TotalCostOfYellow { get; set; }
            public double TotalCostOfColor { get; set; }
            public double TotalCostOfBlack { get; set; }
            public double LabelCostOfCyan { get; set; }
            public double LabelCostOfMagenta { get; set; }
            public double LabelCostOfYellow { get; set; }
            public double LabelCostOfColor { get; set; }
            public double LabelCostOfBlack { get; set; }
            public double TotalBlackLabels { get; set; }
            public double TotalCyanLabels { get; set; }
            public double TotalMagentaLabels { get; set; }
            public double TotalYellowLabels { get; set; }
            public double TotalColorLabels { get; set; }
            public double TotalCyanMlUsed { get; set; }
            public double TotalMagentaMlUsed { get; set; }
            public double TotalYellowMlUsed { get; set; }
            public double TotalColorMlUsed { get; set; }
            public double TotalBlackMlUsed { get; set; }
            public double PageColorMlUsed { get; set; }
            public double PageCyanMlUsed { get; set; }
            public double PageMagentaMlUsed { get; set; }
            public double PageYellowMlUsed { get; set; }
            public double PageBlackMlUsed { get; set; }
            public double ColorPagesPerCartridge { get; set; }
            public double CyanPagesPerCartridge { get; set; }
            public double MagentaPagesPerCartridge { get; set; }
            public double YellowPagesPerCartridge { get; set; }
            public double BlackPagesPerCartridge { get; set; }
        }

        [Serializable()]
        public class InkLevel
        {
            public string Printer { get; set; }
            public string PrintJob { get; set; }
            public uint TotalImpressions { get; set; }
            public uint DuplexSheets { get; set; }
            public uint JamEvents { get; set; }
            public uint MispickEvents { get; set; }
            public string Color { get; set; }
            public uint PEID { get; set; }
            public uint Count { get; set; }
            public string Type { get; set; }
            public DateTime timestamp { get; set; }
        }
        public class ProductUsage
        {
            public uint TotalImpressions { get; set; }
            public uint DuplexSheets { get; set; }
            public uint JamEvents { get; set; }
            public uint MispickEvents { get; set; }
            public List<ConsumableSubUnit> Consumables { get; set; }
        }
        public class ConsumableSubUnit
        {
            public string MarkerColor { get; set; }
            public List<MarkingAgent> Agent { get; set; }
        }
        public class MarkingAgent
        {
            public uint PEID { get; set; }
            public uint Count { get; set; }
            public string Type { get; set; }
        }
        public class ConsumableInfo
        {
            public string ID { get; set; }
            public string LabelCode { get; set; }
            public string SelectabilityNumber { get; set; }
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
                changePrinter = true;
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
                triggerEndCaptureToolStripMenuItem.Enabled = enable;
            }
        }
        private void endCaptureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CaptureEndEnableToolStrip(false);

            InkCapture(manualName, true);
            InkCalculate(manualName, manualJobId);
        }
    }
}
