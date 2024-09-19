using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using API.PrintSpool;
using NLog;
using System.Management;
using System.Windows.Threading;

namespace Monitors
{
    
    public class PrintJobChangeEventArgs : EventArgs
    {
        #region private variables
        private int _jobID=0;
        private int _pdwChange = 0;
        private int _stat = 0;
        private bool _addJob = false;
        private bool _setjob = false;
        private bool _deleteJob = false;
        private bool _job = false;
        private int _queueCount = 0;
        private string _name = string.Empty;
        private DateTime _jobSubmitted = new DateTime();
        #endregion
        public int JobID { get { return _jobID;}  }
        public int PdwChange { get { return _pdwChange; } }
        public int Stat { get { return _stat; } }
        public bool AddJob { get { return _addJob; } }
        public bool Setjob { get { return _setjob; } }
        public bool DeleteJob { get { return _deleteJob; } }
        public bool Job { get { return _job; } }
        public int QueueCount { get { return _queueCount; } }
        public string Name { get { return _name; } }
        public DateTime JobSubmitted { get { return _jobSubmitted; } }

        public PrintJobChangeEventArgs(int intJobID, int stat, int pdwchange, bool addJob, bool setJob, bool deleteJob, bool job, int queueCount, string name, DateTime jobSubmitted)
           : base()
        {
            _jobID = intJobID;
            _stat = stat;
            _pdwChange = pdwchange;
            _setjob = setJob;
            _addJob = addJob;
            _deleteJob = deleteJob;
            _job = job;
            _queueCount = queueCount;
            _name = name;
            _jobSubmitted = jobSubmitted;
        }
    }
    public class PrintJobLogEventArgs : EventArgs
    {
        #region private variables
        private int _jobID = 0;
        private int _pdwChange = 0;
        private int _stat = 0;
        private int _queueCount = 0;
        private string _name = string.Empty;
        private DateTime _jobSubmitted = new DateTime();
        #endregion

        public int JobID { get { return _jobID; } }
        public int PdwChange { get { return _pdwChange; } }
        public int Stat { get { return _stat; } }
        public int QueueCount { get { return _queueCount; } }
        public string Name { get { return _name; } }
        public DateTime JobSubmitted { get { return _jobSubmitted; } }

        public PrintJobLogEventArgs(int intJobID, int stat, int pdwchange, int queueCount, string name, DateTime jobSubmitted)
           : base()
        {
            _jobID = intJobID;
            _stat = stat;
            _pdwChange = pdwchange;
            _queueCount = queueCount;
            _name = name;
            _jobSubmitted = jobSubmitted;
        }
    }
    public delegate void PrintJobStatusChanged(object Sender, PrintJobChangeEventArgs e);
    public delegate void PrintJobLogChanged(object Sender, PrintJobLogEventArgs e);
    public delegate void PrinterStatusChanged(object Sender, string e);

    public class PrintQueueMonitor  
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #region DLL Import Functions
        [DllImport("winspool.drv", EntryPoint = "OpenPrinterA",SetLastError = true, CharSet = CharSet.Ansi,ExactSpelling = true,CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter(String pPrinterName,
        out IntPtr phPrinter,
        Int32 pDefault);


        [DllImport("winspool.drv", EntryPoint = "ClosePrinter",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter
        (Int32 hPrinter);

        [DllImport("winspool.drv",
            EntryPoint = "FindFirstPrinterChangeNotification",
            SetLastError = true, 
            CharSet = CharSet.Auto,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FindFirstPrinterChangeNotification
                            (IntPtr hPrinter,
                            Int32 fwFlags,
                            Int32 fwOptions,
                             qPrinterNotifyOptions pPrinterNotifyOptions);

        [DllImport("winspool.drv",
            EntryPoint = "FindFirstPrinterChangeNotification",
            SetLastError = true,
            CharSet = CharSet.Auto,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FindFirstPrinterChangeNotification
                            ([InAttribute()] IntPtr hPrinter,
                            [InAttribute()] Int32 fwFlags,
                            [InAttribute()] Int32 fwOptions,
                            [InAttribute(), MarshalAs(UnmanagedType.LPStruct)] PRINTER_NOTIFY_OPTIONS pPrinterNotifyOptions);

        [DllImport("winspool.drv", 
            EntryPoint = "FindNextPrinterChangeNotification", 
            SetLastError = true,
            CharSet = CharSet.Auto, 
            ExactSpelling = true, 
            CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 FindNextPrinterChangeNotification
            (IntPtr hChangeObject, 
            out Int32 pdwChange, 
            IntPtr PrinterNotifyOptions, 
            ref IntPtr pPrinterNotifyInfo);


        [DllImport("winspool.drv", EntryPoint = "FindNextPrinterChangeNotification",
            SetLastError = true, 
            CharSet = CharSet.Auto,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool FindNextPrinterChangeNotification
                            (IntPtr hChangeObject,
                             out Int32 pdwChange,
                             qPrinterNotifyOptions pPrinterNotifyOptions,
                            ref IntPtr pPrinterNotifyInfo);

        [DllImport("winspool.drv", EntryPoint = "FindNextPrinterChangeNotification",
        SetLastError = true,
        CharSet = CharSet.Auto,
        ExactSpelling = true,
        CallingConvention = CallingConvention.StdCall)]
        public static extern bool FindNextPrinterChangeNotification
                            ([InAttribute()] IntPtr hChangeObject,
                             [OutAttribute()] out Int32 pdwChange,
                             [InAttribute(), MarshalAs(UnmanagedType.LPStruct)] PRINTER_NOTIFY_OPTIONS pPrinterNotifyOptions,
                            [OutAttribute()] out IntPtr lppPrinterNotifyInfo);

        //[DllImport("winspool.drv", EntryPoint = "FindFirstPrinterChangeNotification", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true)]
        //internal static extern IntPtr FindFirstPrinterChangeNotification(qSafePrinterHandle hPrinter, uint fdwFlags, System.UInt32 fdwOptions, [MarshalAs(UnmanagedType.LPStruct)] qPrinterNotifyOptions pPrinterNotifyOptions);

        //[DllImport("winspool.drv", EntryPoint = "FindNextPrinterChangeNotification", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        //internal static extern Int32 FindNextPrinterChangeNotification(qSafeWaitPrinterHandle hChange, out uint pdwChange, IntPtr PrinterNotifyOptions, ref IntPtr pPrinterNotifyInfo);

        //[DllImport("winspool.drv", EntryPoint = "FindNextPrinterChangeNotification", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        //internal static extern Int32 FindNextPrinterChangeNotification(qSafeWaitPrinterHandle hChange, out uint pdwChange, qPrinterNotifyOptions PrinterNotifyOptions, ref IntPtr pPrinterNotifyInfo);


        [DllImport("winspool.drv", EntryPoint = "FreePrinterNotifyInfo", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern System.Int32 FreePrinterNotifyInfo(IntPtr pPrinterNotifyInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        #endregion

        #region Constants
        const int PRINTER_NOTIFY_OPTIONS_REFRESH = 1;
        const UInt32 INFINITE = 0xFFFFFFFF;
        #endregion
        #region Events
        public event PrintJobStatusChanged OnJobStatusChange;
        public event PrintJobLogChanged OnJobLogChange;
        public event PrinterStatusChanged OnPrinterStatusChanged;
        
        #endregion

        #region private variables
        private IntPtr _printerHandle = IntPtr.Zero;
        private IntPtr _printerHandleJob = IntPtr.Zero;
        private ManualResetEvent _mrEvent = new ManualResetEvent(false);
        private ManualResetEvent _mrEventJob = new ManualResetEvent(false);
        private RegisteredWaitHandle _waitHandle = null;
        private RegisteredWaitHandle _waitHandleJob = null;
        private IntPtr _changeHandle = IntPtr.Zero;
        private IntPtr _changeHandleJob = IntPtr.Zero;
        private PRINTER_NOTIFY_OPTIONS _notifyOptions = new PRINTER_NOTIFY_OPTIONS();
        private PRINTER_NOTIFY_OPTIONS _notifyOptionsJob = new PRINTER_NOTIFY_OPTIONS();
        private PrintQueue pq = null;
        private PrintQueue pqJob = null;
        private string _selectedPrinter = string.Empty;
        private bool monitor = false;
        private int IntPtrSize = Marshal.SizeOf(typeof(IntPtr));
        private int IntPtrSizeJob = Marshal.SizeOf(typeof(IntPtr));
        private string shithappened = string.Empty;
        private string shithappenedJob = string.Empty;
        private bool _isNetwork = false;
        #endregion

        private Thread backgroundThread;
        private Thread backgroundThreadJob;

        #region constructor

        public PrintQueueMonitor() 
        {
            
        }
        #endregion

        #region destructor
        ~PrintQueueMonitor() 
        {
            try
            {
                Stop();
            }
            catch { }
        }
        #endregion

        #region StartMonitoring
        public string Start(string selectedPrinter, bool isNetwork)
        {
            try
            {
                //WaitHandle wh = new System.Threading.WaitHandle.EventWaitHandler();
                _selectedPrinter = selectedPrinter;
                _isNetwork = isNetwork;

                OpenPrinter(selectedPrinter, out _printerHandle, 0);
                //OpenPrinter(selectedPrinter, out _printerHandleJob, 0);
                if (_printerHandle != IntPtr.Zero)
                {
                    logger.Debug("---------------- Monitor Print Queue Monitor Starting");
                    // Create a thread
                    backgroundThread = new Thread(new ThreadStart(monitorPrinter));
                    //backgroundThreadJob = new Thread(new ThreadStart(monitorPrinterJob));
                    // Start thread
                    backgroundThread.Start();
                    //backgroundThreadJob.Start();

                }
                //logger.Debug("Print Queue Monitor Started");
            }
            catch (Exception ex)
            {
                //logger.Error("Print Queue Monitor Starting");
                //logger.Error(ex.Message);
                return $"{ex.Message},{ex.InnerException}";
            }
            return string.Empty;
        }
        #endregion
        #region StopMonitoring
        public void Stop()
        {
            try
            {
                //logger.Debug("Print Queue Monitor Stopping");
                monitor = false;
                backgroundThread.Abort();
                backgroundThreadJob.Abort();
                if (_printerHandle != null)
                {
                    _waitHandle.Unregister(_mrEvent);
                    _waitHandleJob.Unregister(_mrEventJob);
                    _mrEvent.Reset();
                    _mrEventJob.Reset();
                    if (_printerHandle != IntPtr.Zero)
                    {
                        _changeHandle = IntPtr.Zero;
                        ClosePrinter((int)_printerHandle);
                        _printerHandle = IntPtr.Zero;
                    }
                    if (_printerHandleJob != IntPtr.Zero)
                    {
                        _changeHandleJob = IntPtr.Zero;
                        ClosePrinter((int)_printerHandleJob);
                        _printerHandleJob = IntPtr.Zero;
                    }

                    _mrEvent = null;
                    _waitHandle = null;
                    _mrEventJob = null;
                    _waitHandleJob = null;

                    Thread.CurrentThread.Interrupt();
                    Thread.CurrentThread.Abort();
                }
                //logger.Debug("Print Queue Monitor Stopped");
            }
            catch (Exception ex)
            {
                //logger.Error("Print Queue Monitor Stopping");
                //logger.Error(ex.Message);
            }
        }
        #endregion

        //private void PrinterNotifyWaitCallback(object state, bool timedOut)
        private void monitorPrinterEX()
        {
            logger.Debug("---------------- Monitor Printer");
            //if (_isNetwork)
            //    _changeHandle = FindFirstPrinterChangeNotification(_printerHandle, (int)PRINTER_CHANGES.PRINTER_CHANGE_ALL, 0, null);
            //else
            //    _changeHandle = FindFirstPrinterChangeNotification(_printerHandle, 0, 0, new qPrinterNotifyOptions(false));

            //_changeHandle = FindFirstPrinterChangeNotification(_printerHandle, (int)PRINTER_CHANGES.PRINTER_CHANGE_ALL, 0, new qPrinterNotifyOptions(true));

            _notifyOptions.dwVersion = 2;
            _notifyOptions.Count = 2;
            _notifyOptions.dwFlags = 0;
            _changeHandle = FindFirstPrinterChangeNotification(_printerHandle, (int)PRINTER_CHANGES.PRINTER_CHANGE_JOB, 0, _notifyOptions);

            //if (SafePrinterHandle.PrinterInfo5.Attributes.IsNetwork)
            //    this.handle = qStatic.FindFirstPrinterChangeNotification(SafePrinterHandle, (uint)Printer_Change.ALL, 0, null);
            //else
            //    this.handle = qStatic.FindFirstPrinterChangeNotification(SafePrinterHandle, 0, 0, new qPrinterNotifyOptions(false));


            monitor = true;
            while (monitor)
            {
                logger.Debug("---------------- Monitor Printer Loop");
                var waitResult = WaitForSingleObject(_changeHandle, INFINITE);
                logger.Debug("---------------- Monitor Printer got wait");
                try
                {
                    IntPtr pni = new IntPtr();
                    int pdwChange = 0;
                    //IntPtr pnj = new IntPtr();
                    if (FindNextPrinterChangeNotification(_printerHandle, out pdwChange, IntPtr.Zero, ref pni) != 0)
                    {
                        logger.Debug("---------------- Monitor Printer find next printer change notification");
                        if (pdwChange != 0)
                        {
                            shithappened = "Server Changed : ";
                            if (GetServerChange(pdwChange, Printer_Change.ADD_FORM))
                                shithappened += "Form Added ";
                            if (GetServerChange(pdwChange, Printer_Change.ADD_JOB))
                                shithappened += "Job Added ";
                            if (GetServerChange(pdwChange, Printer_Change.ADD_PORT))
                                shithappened += "Port Added ";
                            if (GetServerChange(pdwChange, Printer_Change.ADD_PRINT_PROCESSOR))
                                shithappened += "Processor Added ";
                            if (GetServerChange(pdwChange, Printer_Change.ADD_PRINTER))
                                shithappened += "Printer Added ";
                            if (GetServerChange(pdwChange, Printer_Change.ADD_PRINTER_DRIVER))
                                shithappened += "Driver Added ";
                            if (GetServerChange(pdwChange, Printer_Change.CONFIGURE_PORT))
                                shithappened += "Port Configured ";
                            if (GetServerChange(pdwChange, Printer_Change.DELETE_FORM))
                                shithappened += "Form Deleted ";
                            if (GetServerChange(pdwChange, Printer_Change.DELETE_JOB))
                                shithappened += "Job Deleted ";
                            if (GetServerChange(pdwChange, Printer_Change.DELETE_PORT))
                                shithappened += "Port Deleted ";
                            if (GetServerChange(pdwChange, Printer_Change.DELETE_PRINT_PROCESSOR))
                                shithappened += "Processor Deleted ";
                            if (GetServerChange(pdwChange, Printer_Change.DELETE_PRINTER))
                                shithappened += "Printer Deleted ";
                            if (GetServerChange(pdwChange, Printer_Change.DELETE_PRINTER_DRIVER))
                                shithappened += "Driver Deleted ";
                            if (GetServerChange(pdwChange, Printer_Change.FAILED_CONNECTION_PRINTER))
                                shithappened += "Printer Failed Connection ";
                            if (GetServerChange(pdwChange, Printer_Change.SET_FORM))
                                shithappened += "Form Added ";
                            if (GetServerChange(pdwChange, Printer_Change.SET_JOB))
                                shithappened += "Job Set ";
                            if (GetServerChange(pdwChange, Printer_Change.SET_PRINTER))
                                shithappened += "Printer Set ";
                            if (GetServerChange(pdwChange, Printer_Change.SET_PRINTER_DRIVER))
                                shithappened += "Driver Set ";
                            if (GetServerChange(pdwChange, Printer_Change.TIMEOUT))
                                shithappened += "Temeout ";
                            if (GetServerChange(pdwChange, Printer_Change.WRITE_JOB))
                                shithappened += "Job Written ";

                            OnPrinterStatusChanged(this, shithappened);
                            logger.Debug($"---------------- Monitor sending Server Changed : {shithappened}");
                        }

                        if (pni != IntPtr.Zero)
                        {
                            logger.Debug("---------------- Monitor Printer pni!=0");
                        restart:
                            int count = Marshal.ReadInt32(pni, (IntPtrSize * 2));
                            int jobid;
                            int nnn;
                            for (int ii = 0; ii < count; ii++)
                            {
                                shithappened = "";
                                nnn = 0;
                                int j = (IntPtrSize * 5) * ii;
                                short type = Marshal.ReadInt16(pni, (j + (IntPtrSize * 3)));
                                short field = Marshal.ReadInt16(pni, (j + (IntPtrSize * 3) + 2));
                                if (type == 0) // Printer Notification
                                {
                                    switch (field)
                                    {
                                        case (short)Printer_Notify_Field_Indexes.PRINTER_NAME:
                                            shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.SHARE_NAME:
                                            shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.PORT_NAME:
                                            shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.DRIVER_NAME:
                                            shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.COMMENT:
                                            shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.LOCATION:
                                            shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.DEVMODE:
                                            //DevMode d = new DevMode(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            //new IntPtr(pbuf));
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.SEPFILE:
                                            shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.PRINT_PROCESSOR:
                                            shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.PARAMETERS:
                                            shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.DATATYPE:
                                            shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.ATTRIBUTES:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            shithappened = nnn.ToString();
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.PRIORITY:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            shithappened = nnn.ToString();
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.DEFAULT_PRIORITY:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            shithappened = nnn.ToString();
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.START_TIME:
                                            shithappened = "Start time";
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            TimeSpan span1 = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                                            nnn += span1.Hours * 60;
                                            TimeSpan ts = new TimeSpan(nnn / 60, nnn % 60, 0);
                                            shithappened = ts.ToString();
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.UNTIL_TIME:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            TimeSpan span2 = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                                            nnn += span2.Hours * 60;
                                            TimeSpan tss = new TimeSpan(nnn / 60, nnn % 60, 0);
                                            shithappened = nnn.ToString();
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.STATUS:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            shithappened = nnn.ToString();
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.CJOBS:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            shithappened = nnn.ToString();
                                            break;
                                        case (short)Printer_Notify_Field_Indexes.AVERAGE_PPM:
                                            shithappened = "Average PPM";
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            shithappened = nnn.ToString();
                                            break;
                                        default:
                                            shithappened = "";
                                            nnn = 0;
                                            break;
                                    }
                                    Printer_Notify_Field_Indexes fi = (Printer_Notify_Field_Indexes)field;

                                    OnPrinterStatusChanged(this, fi.ToString() + " " + shithappened);
                                    logger.Debug($"---------------- Monitor sending Printer : {fi.ToString()} :: {shithappened}");
                                }
                                if (type == 1)  //Job Notifiction
                                {
                                    jobid = Marshal.ReadInt32(pni, (j + (IntPtrSize * 5)));
                                    Job_Notify_Field_Indexes fi = (Job_Notify_Field_Indexes)field;
                                    shithappened = fi.ToString() + " ";
                                    switch (field)
                                    {
                                        case (short)Job_Notify_Field_Indexes.PRINTER_NAME:
                                            shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Job_Notify_Field_Indexes.MACHINE_NAME:
                                            shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Job_Notify_Field_Indexes.PORT_NAME:
                                            shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Job_Notify_Field_Indexes.USER_NAME:
                                            shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Job_Notify_Field_Indexes.NOTIFY_NAME:
                                            shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Job_Notify_Field_Indexes.DATATYPE:
                                            shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Job_Notify_Field_Indexes.PRINT_PROCESSOR:
                                            shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Job_Notify_Field_Indexes.PARAMETERS:
                                            shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Job_Notify_Field_Indexes.DRIVER_NAME:
                                            shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Job_Notify_Field_Indexes.DEVMODE:
                                            //DevMode d = new DevMode(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            shithappened += "Changed";
                                            break;
                                        case (short)Job_Notify_Field_Indexes.STATUS:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            Job_Status stat = (Job_Status)nnn;
                                            shithappened += stat.ToString();
                                            break;
                                        case (short)Job_Notify_Field_Indexes.STATUS_STRING:
                                            shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Job_Notify_Field_Indexes.DOCUMENT:
                                            shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                            break;
                                        case (short)Job_Notify_Field_Indexes.PRIORITY:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            shithappened += nnn.ToString();
                                            break;
                                        case (short)Job_Notify_Field_Indexes.POSITION:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            shithappened += nnn.ToString();
                                            break;
                                        case (short)Job_Notify_Field_Indexes.SUBMITTED:
                                            ////systemtime in pdev
                                            //IntPtr ptr1 = Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7)));
                                            //SYSTEMTIME st = new SYSTEMTIME();
                                            //Marshal.PtrToStructure(ptr1, st);
                                            //st.wHour += (short)(qStatic.hourdiv / 60);
                                            //shithappened += st.ToString();
                                            break;
                                        case (short)Job_Notify_Field_Indexes.START_TIME:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            shithappened += nnn.ToString();
                                            break;
                                        case (short)Job_Notify_Field_Indexes.UNTIL_TIME:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            shithappened += nnn.ToString();
                                            break;
                                        case (short)Job_Notify_Field_Indexes.TIME:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            shithappened += nnn.ToString();
                                            break;
                                        case (short)Job_Notify_Field_Indexes.TOTAL_PAGES:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            shithappened += nnn.ToString();
                                            break;
                                        case (short)Job_Notify_Field_Indexes.PAGES_PRINTED:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            shithappened += nnn.ToString();
                                            break;
                                        case (short)Job_Notify_Field_Indexes.TOTAL_BYTES:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            shithappened += nnn.ToString();
                                            break;
                                        case (short)Job_Notify_Field_Indexes.BYTES_PRINTED:
                                            nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                            shithappened += nnn.ToString();
                                            break;
                                        default:
                                            shithappened += "Error";
                                            nnn = 0;
                                            break;

                                    }

                                    OnPrinterStatusChanged(jobid, shithappened);
                                    logger.Debug($"---------------- Monitor sending Job : {jobid.ToString()} :: {shithappened}");
                                }
                            }
                            int fla = Marshal.ReadInt32(pni, IntPtrSize);
                            if (Convert.ToBoolean(fla & 0x00000001))
                            {
                                // some notifications had to be discarded
                                FindNextPrinterChangeNotification(_changeHandle, out pdwChange, new qPrinterNotifyOptions(true), ref pni);
                                if (Marshal.ReadInt32(pni, (IntPtrSize * 2)) > 0)
                                    goto restart;
                            }
                            FreePrinterNotifyInfo(pni);
                        }
                    }
                    //_RegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject((ManualResetEvent)_ManualResetEvent, new WaitOrTimerCallback(PrinterNotifyWaitCallback), null, -1, true);
                }
                catch (Exception ex)
                {
                    logger.Error("Handling Event Error");
                    logger.Error(ex.Message);
                    logger.Error(ex.InnerException);
                    logger.Error(ex.StackTrace);
                }
                Thread.Sleep(20);
            }
        }
        private bool GetServerChange(int change, Printer_Change fwt)
        {
            uint pdwChange = (uint)fwt;
            return (change & pdwChange) == pdwChange;
        }

        public void monitorPrinter()
        {
            //We got a valid Printer handle.  Let us register for change notification....
            string name = string.Empty;
            DateTime jobSubmitted = new DateTime();
            PrintSystemJobInfo ji = null;
            pq = new PrintQueue(new PrintServer(), _selectedPrinter);

            _notifyOptions.dwVersion = 2;
            _notifyOptions.Count = 2;
            //_notifyOptions.dwFlags = 0;
            _notifyOptions.dwFlags = 1;
            _changeHandle = FindFirstPrinterChangeNotification(_printerHandle, (int)PRINTER_CHANGES.PRINTER_CHANGE_ALL, 0, _notifyOptions);
            monitor = true;
            while (monitor)
            {
                try
                {
                    var waitResult = WaitForSingleObject(_changeHandle, INFINITE);
                    //_notifyOptions.Count = 1;
                    int pdwChange = 0;
                    IntPtr pNotifyInfo = IntPtr.Zero;
                    bool bResult = FindNextPrinterChangeNotification(_changeHandle, out pdwChange, _notifyOptions, out pNotifyInfo);
                    int queueCount = pq.NumberOfJobs;
                    //logger.Info($"---------------- Monitor received pdwChange: {pdwChange}");
                    if (pNotifyInfo != IntPtr.Zero)
                    {
                        if (bResult)
                        {
                            bool addJob = (pdwChange & 256) == 256;
                            bool setJob = (pdwChange & 512) == 512;
                            bool deleteJob = (pdwChange & 1024) == 1024;
                            bool job = (pdwChange & 2048) == 2048;
                            int printerstatus = 0;
                            #region populate Notification Information
                            //Now, let us initialize and populate the Notify Info data
                            PRINTER_NOTIFY_INFO info = (PRINTER_NOTIFY_INFO)Marshal.PtrToStructure(pNotifyInfo, typeof(PRINTER_NOTIFY_INFO));
                            int pData = (int)pNotifyInfo + Marshal.SizeOf(typeof(PRINTER_NOTIFY_INFO));

                            PRINTER_NOTIFY_INFO_DATA[] data = new PRINTER_NOTIFY_INFO_DATA[info.Count];
                            for (uint i = 0; i < info.Count; i++)
                            {
                                data[i] = (PRINTER_NOTIFY_INFO_DATA)Marshal.PtrToStructure((IntPtr)pData, typeof(PRINTER_NOTIFY_INFO_DATA));

                                int stat = (int)data[i].NotifyData.Data.cbBuf;
                                try
                                {
                                    int nnn;
                                    int j = (int)((IntPtrSize * 5) * i);
                                    short type = Marshal.ReadInt16(pNotifyInfo, (int)(j + (IntPtrSize * 3)));
                                    short field = Marshal.ReadInt16(pNotifyInfo, (int)(j + (IntPtrSize * 3) + 2));
                                    //Job_Notify_Field_Indexes fi = (Job_Notify_Field_Indexes)field;
                                    //shithappened = fi.ToString() + " ";
                                    //logger.Info($"---------------- Monitor received Notification type: {type}, field {field}");
                                    if (type == 1)
                                    {
                                        int jobid = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 5)));
                                        Job_Notify_Field_Indexes fi = (Job_Notify_Field_Indexes)field;
                                        shithappened = fi.ToString() + " ";
                                        //logger.Info($"---------------- Monitor received Job Notification jobid: {jobid}, field indexes {fi}");
                                        switch (field)
                                        {
                                            case (short)Job_Notify_Field_Indexes.STATUS:
                                                nnn = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 6)));
                                                Job_Status mystat = (Job_Status)nnn;
                                                shithappened = mystat.ToString();
                                                printerstatus = nnn;
                                                logger.Info($"---------------- Monitor received Job Notification jobId: {jobid}, status: {shithappened}, stat: {nnn}");

                                                if ((jobid > 0) && (printerstatus == 388))
                                                {
                                                    logger.Info($"---------------- Monitor send jobId: {jobid}, pdwChange: {pdwChange}, status {printerstatus}, name: {name}, time submitted: {jobSubmitted}");

                                                    if (OnJobStatusChange != null)
                                                    {
                                                        logger.Info($"---------------- Monitor sending jobId: {jobid}, pdwChange: {pdwChange}, status {printerstatus}");
                                                        //Let us raise the event
                                                        OnJobStatusChange(this, new PrintJobChangeEventArgs(jobid, printerstatus, pdwChange, addJob, setJob, deleteJob, job, queueCount, name, jobSubmitted));
                                                    }
                                                }
                                                break;
                                            case (short)Job_Notify_Field_Indexes.TOTAL_PAGES:
                                                nnn = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 6)));
                                                shithappened = nnn.ToString();
                                                logger.Info($"---------------- Monitor received Job Notification jobId: {jobid}, Total Pages: {shithappened}");
                                                break;
                                            case (short)Job_Notify_Field_Indexes.PAGES_PRINTED:
                                                nnn = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 6)));
                                                shithappened = nnn.ToString();
                                                if (nnn>=1)
                                                {
                                                    if ((jobid > 0))
                                                    {
                                                        try
                                                        {
                                                            ji = pq.GetJob(jobid);
                                                            name = ji.Name;
                                                            jobSubmitted = ji.TimeJobSubmitted;
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            ji = null;
                                                        }
                                                    }
                                                }
                                                logger.Info($"---------------- Monitor received Job Notification jobId: {jobid}, Pages Printed: {shithappened}");
                                                break;
                                            //default:
                                            //    nnn = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 6)));
                                            //    shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pNotifyInfo, (j + (IntPtrSize * 7))));
                                            //    logger.Info($"++++++++++++++++ Monitor received Job Notification default jobId: {jobid}, 7: {shithappened}, 6: {nnn}");
                                            //    break;
                                        }
                                    }
                                    if (type == 0)  //Printer Notifiction
                                    {
                                        //int jobid = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 5)));
                                        int intJobID = (int)data[i].Id;
                                        logger.Info($"++++++++++++++++ Monitor received Printer Notification j: {j}, intJobID: {intJobID}, type: {type}, field {field}");
                                        switch (field)
                                        {
                                            case (short)Printer_Notify_Field_Indexes.STATUS:
                                                nnn = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 6)));
                                                shithappened = nnn.ToString();
                                                logger.Info($"++++++++++++++++ Monitor received Printer Notification jobId: {intJobID}, status: {shithappened}, stat:{nnn}");
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.CJOBS:
                                                nnn = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 6)));
                                                shithappened = nnn.ToString();
                                                logger.Info($"++++++++++++++++ Monitor received Printer Notification jobId: {intJobID}, cJobs: {nnn}");
                                                break;
                                            default:
                                                nnn = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 6)));
                                                shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pNotifyInfo, (j + (IntPtrSize * 7))));
                                                logger.Info($"++++++++++++++++ Monitor received Printer Notification default jobId: {intJobID}, 7: {shithappened}, 6: {nnn}");
                                                break;
                                        }

                                        printerstatus = field;


                                        if (intJobID > 0)
                                        {
                                            //logger.Info($"++++++++++++++++ Monitor jobId: {intJobID}");
                                            try
                                            {
                                                ji = pq.GetJob(intJobID);
                                                name = ji.Name;
                                                jobSubmitted = ji.TimeJobSubmitted;
                                            }
                                            catch (Exception ex)
                                            {
                                                ji = null;
                                            }
                                            if (OnJobStatusChange != null)
                                            {
                                                //Let us raise the event
                                                OnJobStatusChange(this, new PrintJobChangeEventArgs(intJobID, printerstatus, pdwChange, addJob, setJob, deleteJob, job, queueCount, name, jobSubmitted));
                                            }
                                        }
                                        pData += Marshal.SizeOf(typeof(PRINTER_NOTIFY_INFO_DATA));
                                    }
                                }
                                catch { }
                            }
                            #endregion
                        }
                    }

                    FreePrinterNotifyInfo(pNotifyInfo);
                    Thread.Sleep(20);
                }
                catch (Exception ex)
                {
                    logger.Error("Handling Event Error");
                    logger.Error(ex.Message);
                    logger.Error(ex.InnerException);
                    logger.Error(ex.StackTrace);
                }
            }
        }
        public void monitorPrinterJob()
        {
            //We got a valid Printer handle.  Let us register for change notification....
            string name = string.Empty;
            DateTime jobSubmitted = new DateTime();
            PrintSystemJobInfo ji = null;
            pqJob = new PrintQueue(new PrintServer(), _selectedPrinter);

            _notifyOptionsJob.dwVersion = 2;
            _notifyOptionsJob.Count = 2;
            _notifyOptionsJob.dwFlags = 0;
            //_notifyOptions.dwFlags = 1;
            _changeHandleJob = FindFirstPrinterChangeNotification(_printerHandle, (int)PRINTER_CHANGES.PRINTER_CHANGE_ALL, 0, _notifyOptionsJob);
            monitor = true;
            while (monitor)
            {
                try
                {
                    var waitResultJob = WaitForSingleObject(_changeHandleJob, INFINITE);
                    //_notifyOptions.Count = 1;
                    int pdwChange = 0;
                    IntPtr pNotifyInfo = IntPtr.Zero;
                    bool bResult = FindNextPrinterChangeNotification(_changeHandleJob, out pdwChange, _notifyOptionsJob, out pNotifyInfo);
                    int queueCount = pqJob.NumberOfJobs;
                    //logger.Info($"---------------- Monitor received pdwChange: {pdwChange}");
                    if (pNotifyInfo != IntPtr.Zero)
                    {
                        if (bResult)
                        {
                            bool addJob = (pdwChange & 256) == 256;
                            bool setJob = (pdwChange & 512) == 512;
                            bool deleteJob = (pdwChange & 1024) == 1024;
                            bool job = (pdwChange & 2048) == 2048;
                            int printerstatus = 0;
                            #region populate Notification Information
                            //Now, let us initialize and populate the Notify Info data
                            PRINTER_NOTIFY_INFO info = (PRINTER_NOTIFY_INFO)Marshal.PtrToStructure(pNotifyInfo, typeof(PRINTER_NOTIFY_INFO));
                            int pData = (int)pNotifyInfo + Marshal.SizeOf(typeof(PRINTER_NOTIFY_INFO));

                            PRINTER_NOTIFY_INFO_DATA[] data = new PRINTER_NOTIFY_INFO_DATA[info.Count];
                            for (uint i = 0; i < info.Count; i++)
                            {
                                data[i] = (PRINTER_NOTIFY_INFO_DATA)Marshal.PtrToStructure((IntPtr)pData, typeof(PRINTER_NOTIFY_INFO_DATA));

                                int stat = (int)data[i].NotifyData.Data.cbBuf;
                                try
                                {
                                    int nnn;
                                    int j = (int)((IntPtrSize * 5) * i);
                                    short type = Marshal.ReadInt16(pNotifyInfo, (int)(j + (IntPtrSize * 3)));
                                    short field = Marshal.ReadInt16(pNotifyInfo, (int)(j + (IntPtrSize * 3) + 2));
                                    //Job_Notify_Field_Indexes fi = (Job_Notify_Field_Indexes)field;
                                    //shithappened = fi.ToString() + " ";
                                    //logger.Info($"---------------- Monitor received Notification type: {type}, field {field}");
                                    if (type == 1)
                                    {
                                        int jobid = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 5)));
                                        Job_Notify_Field_Indexes fi = (Job_Notify_Field_Indexes)field;
                                        shithappened = fi.ToString() + " ";
                                        //logger.Info($"---------------- Monitor received Job Notification jobid: {jobid}, field indexes {fi}");
                                        switch (field)
                                        {
                                            case (short)Job_Notify_Field_Indexes.STATUS:
                                                nnn = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 6)));
                                                Job_Status mystat = (Job_Status)nnn;
                                                shithappened = mystat.ToString();
                                                printerstatus = nnn;
                                                logger.Info($"---------------- Monitor received Job Notification jobId: {jobid}, status: {shithappened}, stat: {nnn}");

                                                if ((jobid > 0) && (printerstatus == 388))
                                                {
                                                    logger.Info($"---------------- Monitor send jobId: {jobid}, pdwChange: {pdwChange}, status {printerstatus}, name: {name}, time submitted: {jobSubmitted}");

                                                    if (OnJobStatusChange != null)
                                                    {
                                                        logger.Info($"---------------- Monitor sending jobId: {jobid}, pdwChange: {pdwChange}, status {printerstatus}");
                                                        //Let us raise the event
                                                        OnJobStatusChange(this, new PrintJobChangeEventArgs(jobid, printerstatus, pdwChange, addJob, setJob, deleteJob, job, queueCount, name, jobSubmitted));
                                                    }
                                                }
                                                break;
                                            case (short)Job_Notify_Field_Indexes.TOTAL_PAGES:
                                                nnn = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 6)));
                                                shithappened = nnn.ToString();
                                                logger.Info($"---------------- Monitor received Job Notification jobId: {jobid}, Total Pages: {shithappened}");
                                                break;
                                            case (short)Job_Notify_Field_Indexes.PAGES_PRINTED:
                                                nnn = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 6)));
                                                shithappened = nnn.ToString();
                                                if (nnn >= 1)
                                                {
                                                    if ((jobid > 0))
                                                    {
                                                        try
                                                        {
                                                            ji = pqJob.GetJob(jobid);
                                                            name = ji.Name;
                                                            jobSubmitted = ji.TimeJobSubmitted;
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            ji = null;
                                                        }
                                                    }
                                                }
                                                logger.Info($"---------------- Monitor received Job Notification jobId: {jobid}, Pages Printed: {shithappened}");
                                                break;
                                                //default:
                                                //    nnn = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 6)));
                                                //    shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pNotifyInfo, (j + (IntPtrSize * 7))));
                                                //    logger.Info($"++++++++++++++++ Monitor received Job Notification default jobId: {jobid}, 7: {shithappened}, 6: {nnn}");
                                                //    break;
                                        }
                                    }
                                    if (type == 0)  //Printer Notifiction
                                    {
                                        //int jobid = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 5)));
                                        int intJobID = (int)data[i].Id;
                                        logger.Info($"++++++++++++++++ Monitor received Printer Notification j: {j}, intJobID: {intJobID}, type: {type}, field {field}");
                                        switch (field)
                                        {
                                            case (short)Printer_Notify_Field_Indexes.STATUS:
                                                nnn = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 6)));
                                                shithappened = nnn.ToString();
                                                logger.Info($"++++++++++++++++ Monitor received Printer Notification jobId: {intJobID}, status: {shithappened}, stat:{nnn}");
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.CJOBS:
                                                nnn = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 6)));
                                                shithappened = nnn.ToString();
                                                logger.Info($"++++++++++++++++ Monitor received Printer Notification jobId: {intJobID}, cJobs: {nnn}");
                                                break;
                                            default:
                                                nnn = Marshal.ReadInt32(pNotifyInfo, (j + (IntPtrSize * 6)));
                                                shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pNotifyInfo, (j + (IntPtrSize * 7))));
                                                logger.Info($"++++++++++++++++ Monitor received Printer Notification default jobId: {intJobID}, 7: {shithappened}, 6: {nnn}");
                                                break;
                                        }

                                        //printerstatus = field;
                                        //if (intJobID > 0)
                                        //{
                                        //    //logger.Info($"++++++++++++++++ Monitor jobId: {intJobID}");
                                        //    try
                                        //    {
                                        //        ji = pq.GetJob(intJobID);
                                        //        name = ji.Name;
                                        //        jobSubmitted = ji.TimeJobSubmitted;
                                        //    }
                                        //    catch (Exception ex)
                                        //    {
                                        //        ji = null;
                                        //    }
                                        //    if (OnJobStatusChange != null)
                                        //    {
                                        //        //Let us raise the event
                                        //        OnJobStatusChange(this, new PrintJobChangeEventArgs(intJobID, printerstatus, pdwChange, addJob, setJob, deleteJob, job, queueCount, name, jobSubmitted));
                                        //    }
                                        //}
                                        pData += Marshal.SizeOf(typeof(PRINTER_NOTIFY_INFO_DATA));
                                    }
                                }
                                catch { }
                            }
                            #endregion
                        }
                    }

                    FreePrinterNotifyInfo(pNotifyInfo);
                    Thread.Sleep(20);
                }
                catch (Exception ex)
                {
                    logger.Error("Handling Event Error");
                    logger.Error(ex.Message);
                    logger.Error(ex.InnerException);
                    logger.Error(ex.StackTrace);
                }
            }
        }
        //public void monitorPrinter()
        //{
        //    //We got a valid Printer handle.  Let us register for change notification....
        //    _notifyOptions.dwVersion = 2;
        //    _notifyOptions.Count = 1;
        //    _notifyOptions.dwFlags = 0;
        //    _changeHandle = FindFirstPrinterChangeNotification(_printerHandle, (int)PRINTER_CHANGES.PRINTER_CHANGE_JOB, 0, _notifyOptions);
        //    monitor = true;
        //    while (monitor)
        //    {
        //        try
        //        {
        //            var waitResult = WaitForSingleObject(_changeHandle, INFINITE);
        //            _notifyOptions.Count = 1;
        //            int pdwChange = 0;
        //            IntPtr pNotifyInfo = IntPtr.Zero;
        //            bool bResult = FindNextPrinterChangeNotification(_changeHandle, out pdwChange, _notifyOptions, out pNotifyInfo);
        //            //logger.Info($"---------------- Monitor received pdwChange: {pdwChange}");
        //            if (pNotifyInfo != IntPtr.Zero)
        //            {
        //                if (bResult)
        //                {
        //                    bool addJob = (pdwChange & 256) == 256;
        //                    bool setJob = (pdwChange & 512) == 512;
        //                    bool deleteJob = (pdwChange & 1024) == 1024;
        //                    bool job = (pdwChange & 2048) == 2048;
        //                    int printerstatus = 0;
        //                    //if (deleteJob)
        //                    //{
        //                    #region populate Notification Information
        //                    //Now, let us initialize and populate the Notify Info data
        //                    PRINTER_NOTIFY_INFO info = (PRINTER_NOTIFY_INFO)Marshal.PtrToStructure(pNotifyInfo, typeof(PRINTER_NOTIFY_INFO));
        //                    int pData = (int)pNotifyInfo + Marshal.SizeOf(typeof(PRINTER_NOTIFY_INFO));

        //                    PRINTER_NOTIFY_INFO_DATA[] data = new PRINTER_NOTIFY_INFO_DATA[info.Count];
        //                    for (uint i = 0; i < info.Count; i++)
        //                    {
        //                        data[i] = (PRINTER_NOTIFY_INFO_DATA)Marshal.PtrToStructure((IntPtr)pData, typeof(PRINTER_NOTIFY_INFO_DATA));

        //                        int stat = (int)data[i].NotifyData.Data.cbBuf;
        //                        try
        //                        {
        //                            int nnn;
        //                            //var type = data[i].Type;
        //                            //int j = (IntPtrSize * 5) * ii;
        //                            short type = Marshal.ReadInt16(pNotifyInfo, (int)(i + (IntPtrSize * 3)));
        //                            short field = Marshal.ReadInt16(pNotifyInfo, (int)(i + (IntPtrSize * 3) + 2));
        //                            //logger.Info($"---------------- Monitor received type: {type}, field {field}");
        //                            if (type == 0)  //Printer Notifiction
        //                            {
        //                                logger.Info($"---------------- Monitor received Printer Notification type: {type}, field {field}");
        //                                //logger.Info($"---------------- Monitor received Printer Notify Info: {pNotifyInfo}");
        //                                printerstatus = field;
        //                                //int jobid = Marshal.ReadInt32(pNotifyInfo, (int)(i + (IntPtrSize * 5)));
        //                                //logger.Info($"---------------- Monitor received jobID: {jobid}");

        //                                //switch (field)
        //                                //{
        //                                //    case (short)Printer_Notify_Field_Indexes.STATUS:
        //                                //        //logger.Info($"---------------- Monitor received type: {type}, field {field}");
        //                                //        //logger.Info($"---------------- Monitor received Notify Info: {pNotifyInfo}");
        //                                //        //int jobid = Marshal.ReadInt32(pNotifyInfo, (int)(i + (IntPtrSize * 5)));
        //                                //        //logger.Info($"---------------- Monitor received jobID: {jobid}");

        //                                //        nnn = Marshal.ReadInt32(pNotifyInfo, (int)(i + (IntPtrSize * 6)));
        //                                //        logger.Info($"---------------- Monitor received Status: {nnn}");
        //                                //        //Job_Status mystat = (Job_Status)nnn;
        //                                //        //shithappened += stat.ToString();
        //                                //        //logger.Info($"---------------- Monitor received Status: {shithappened}");
        //                                //        break;
        //                                //}
        //                            }
        //                            //if (type == 1)
        //                            //{
        //                            //    logger.Info($"---------------- Monitor received Job Notification type: {type}, field {field}");
        //                            //    //int jobid = Marshal.ReadInt32(pNotifyInfo, (int)(i + (IntPtrSize * 5)));
        //                            //    //logger.Info($"---------------- Monitor received jobID: {jobid}");
        //                            //    switch (field)
        //                            //    {
        //                            //        case (short)Printer_Notify_Field_Indexes.STATUS:
        //                            //            //logger.Info($"---------------- Monitor received type: {type}, field {field}");
        //                            //            //logger.Info($"---------------- Monitor received Notify Info: {pNotifyInfo}");
        //                            //            //int jobid = Marshal.ReadInt32(pNotifyInfo, (int)(i + (IntPtrSize * 5)));
        //                            //            //logger.Info($"---------------- Monitor received jobID: {jobid}");

        //                            //            nnn = Marshal.ReadInt32(pNotifyInfo, (int)(i + (IntPtrSize * 6)));

        //                            //            logger.Info($"---------------- Monitor received Status: {nnn}");
        //                            //            Job_Status mystat = (Job_Status)nnn;
        //                            //            shithappened += stat.ToString();
        //                            //            logger.Info($"---------------- Monitor received Status: {shithappened}");
        //                            //break;

        //                            //            //        //case (short)Job_Notify_Field_Indexes.STATUS_STRING:
        //                            //            //        //    shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pNotifyInfo, (int)(i + (IntPtrSize * 7))));
        //                            //            //        //    logger.Info($"---------------- Monitor received StatusString: {shithappened}");
        //                            //            //        //    break;
        //                            //            //        case (short)Job_Notify_Field_Indexes.TOTAL_PAGES:
        //                            //            //            nnn = Marshal.ReadInt32(pNotifyInfo, (int)(i + (IntPtrSize * 6)));
        //                            //            //            shithappened += nnn.ToString();
        //                            //            //            logger.Info($"---------------- Monitor received Total Pages: {shithappened}");
        //                            //            //            break;
        //                            //            //        case (short)Job_Notify_Field_Indexes.PAGES_PRINTED:
        //                            //            //            nnn = Marshal.ReadInt32(pNotifyInfo, (int)(i + (IntPtrSize * 6)));
        //                            //            //            shithappened += nnn.ToString();
        //                            //            //            logger.Info($"---------------- Monitor received Pages Printed: {shithappened}");
        //                            //            //            break;
        //                            //    }
        //                            //}
        //                            //var count = info.Count;
        //                            //uint adwData = data[i].NotifyData.adwData[0];

        //                            //byte[] pBuf = new byte[stat];
        //                            //Marshal.Copy(data[i].NotifyData.Data.pBuf, pBuf, 0, stat);

        //                            //string bytesOfpBuf = BitConverter.ToString(pBuf);

        //                            //logger.Info($"---------------- Monitor count: {count.ToString()}, type: {type.ToString()}, adwData: {adwData.ToString()}, pBuf Contents: {bytesOfpBuf}");
        //                        }
        //                        catch { }

        //                        int intJobID = (int)data[i].Id;

        //                        //logger.Info($"---------------- Monitor sending  stat(data[i].NotifyData.Data.cbBuf): {stat.ToString()}");
        //                        //logger.Info($"---------------- Monitor sending pdwChange: {pdwChange}, jobId: {intJobID.ToString()}");
        //                        //                              if (stat > 0)
        //                        //                              {
        //                        if (intJobID > 0)
        //                        {
        //                            pq = new PrintQueue(new PrintServer(), _selectedPrinter);
        //                            int queueCount = pq.NumberOfJobs;
        //                            logger.Info($"---------------- Monitor jobId: {intJobID}");
        //                            string name = string.Empty;
        //                            DateTime jobSubmitted = new DateTime();
        //                            PrintSystemJobInfo ji = null;
        //                            try
        //                            {
        //                                ji = pq.GetJob(intJobID);
        //                                name = ji.Name;
        //                                jobSubmitted = ji.TimeJobSubmitted;
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                ji = null;
        //                            }
        //                            if (OnJobStatusChange != null)
        //                            {
        //                                //Let us raise the event
        //                                OnJobStatusChange(this, new PrintJobChangeEventArgs(intJobID, stat, pdwChange, addJob, setJob, deleteJob, job, queueCount, name, jobSubmitted));
        //                            }
        //                            if (OnJobLogChange != null)
        //                            {
        //                                //Let us raise the event
        //                                OnJobLogChange(this, new PrintJobLogEventArgs(intJobID, stat, pdwChange, queueCount, name, jobSubmitted));
        //                            }
        //                            //break;
        //                        }
        //                        //                              }
        //                        pData += Marshal.SizeOf(typeof(PRINTER_NOTIFY_INFO_DATA));
        //                    }
        //                    #endregion
        //                    //}
        //                }
        //            }

        //            FreePrinterNotifyInfo(pNotifyInfo);
        //            Thread.Sleep(20);
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.Error("Handling Event Error");
        //            logger.Error(ex.Message);
        //            logger.Error(ex.InnerException);
        //            logger.Error(ex.StackTrace);
        //        }
        //    }
        //}

        /***
Waits until the specified object is in the signaled state or the time-out interval elapses.

To enter an alertable wait state, use the WaitForSingleObjectEx function. To wait for multiple objects, use WaitForMultipleObjects.

Syntax
C++

Copy
DWORD WaitForSingleObject(
  [in] HANDLE hHandle,
  [in] DWORD  dwMilliseconds
);
Parameters
[in] hHandle

A handle to the object. For a list of the object types whose handles can be specified, see the following Remarks section.

If this handle is closed while the wait is still pending, the function's behavior is undefined.

The handle must have the SYNCHRONIZE access right. For more information, see Standard Access Rights.

[in] dwMilliseconds

The time-out interval, in milliseconds. If a nonzero value is specified, the function waits until the object is signaled or the interval elapses. If dwMilliseconds is zero, the function does not enter a wait state if the object is not signaled; it always returns immediately. If dwMilliseconds is INFINITE, the function will return only when the object is signaled.

Windows XP, Windows Server 2003, Windows Vista, Windows 7, Windows Server 2008 and Windows Server 2008 R2:  The dwMilliseconds value does include time spent in low-power states. For example, the timeout does keep counting down while the computer is asleep.

Windows 8, Windows Server 2012, Windows 8.1, Windows Server 2012 R2, Windows 10 and Windows Server 2016:  The dwMilliseconds value does not include time spent in low-power states. For example, the timeout does not keep counting down while the computer is asleep.

Return value
If the function succeeds, the return value indicates the event that caused the function to return. It can be one of the following values.

Return code/value	Description
WAIT_ABANDONED
0x00000080L
The specified object is a mutex object that was not released by the thread that owned the mutex object before the owning thread terminated. Ownership of the mutex object is granted to the calling thread and the mutex state is set to nonsignaled.
If the mutex was protecting persistent state information, you should check it for consistency.

WAIT_OBJECT_0
0x00000000L
The state of the specified object is signaled.
WAIT_TIMEOUT
0x00000102L
The time-out interval elapsed, and the object's state is nonsignaled.
WAIT_FAILED
(DWORD)0xFFFFFFFF
The function has failed. To get extended error information, call GetLastError.
Remarks
The WaitForSingleObject function checks the current state of the specified object. If the object's state is nonsignaled, the calling thread enters the wait state until the object is signaled or the time-out interval elapses.

The function modifies the state of some types of synchronization objects. Modification occurs only for the object whose signaled state caused the function to return. For example, the count of a semaphore object is decreased by one.

The WaitForSingleObject function can wait for the following objects:

Change notification
Console input
Event
Memory resource notification
Mutex
Process
Semaphore
Thread
Waitable timer
Use caution when calling the wait functions and code that directly or indirectly creates windows. If a thread creates any windows, it must process messages. Message broadcasts are sent to all windows in the system. A thread that uses a wait function with no time-out interval may cause the system to become deadlocked. Two examples of code that indirectly creates windows are DDE and the CoInitialize function. Therefore, if you have a thread that creates windows, use MsgWaitForMultipleObjects or MsgWaitForMultipleObjectsEx, rather than WaitForSingleObject.
Examples
For an example, see Using Mutex Objects.
         * ****/

        //        // Get change notification handle for the printer   
        //        chgObject = FindFirstPrinterChangeNotification(
        //                        hPrinter,
        //                        PRINTER_CHANGE_JOB,
        //                        0,
        //                        NULL); 

        //if (chgObject != INVALID_HANDLE_VALUE) {
        //    while (bKeepMonitoring) {
        //        // Wait for the change notification 
        //        WaitForSingleObject(chgObject, INFINITE);

        //        fcnreturn = FindNextPrinterChangeNotification(
        //                        chgObject,
        //                        pdwChange,
        //                        NULL,
        //                        NULL);

        //        if (fcnreturn) {
        //            // Check value of *pdwChange and 
        //            //  deal with the indicated change 
        //        }
        //    // Insert some mechanism to stop monitoring
        //    //  such as: 
        //    //
        //    // if (something happens) {
        //    //     bKeepMonitoring = false; 
        //    // }
        //    //
        //}
        //// Close Printer Change Notification handle when finished. 
        //FindClosePrinterChangeNotification(chgObject);
        //} else {
        //    // Unable to open printer change notification handle 
        //    dwStatus = GetLastError();
        //}

        /// <summary>
        /// Old code
        /// </summary>
        //public string Start(string selectedPrinter)
        //{
        //    try
        //    {
        //        _selectedPrinter = selectedPrinter;
        //        logger.Debug("Print Queue Monitor Starting");
        //        OpenPrinter(selectedPrinter, out _printerHandle, 0);
        //        if (_printerHandle != IntPtr.Zero)
        //        {
        //            //We got a valid Printer handle.  Let us register for change notification....
        //            //_changeHandle = FindFirstPrinterChangeNotification(_printerHandle, (int)PRINTER_CHANGES.PRINTER_CHANGE_JOB, 0, _notifyOptions);
        //            _changeHandle = FindFirstPrinterChangeNotification(_printerHandle, (int)PRINTER_CHANGES.PRINTER_CHANGE_ALL, 0, _notifyOptions);
        //            // We have successfully registered for change notification.  Let us capture the handle...
        //            _mrEvent.Handle = _changeHandle;
        //            //Now, let us wait for change notification from the printer queue....
        //            _waitHandle = ThreadPool.RegisterWaitForSingleObject(_mrEvent, new WaitOrTimerCallback(PrinterNotifyWaitCallback), _mrEvent, -1, true);
        //        }
        //        logger.Debug("Print Queue Monitor Started");
        //    }
        //    catch( Exception ex)
        //    {
        //        logger.Error("Print Queue Monitor Starting");
        //        logger.Error(ex.Message);
        //        return $"{ex.Message},{ex.InnerException}";
        //    }
        //    return string.Empty;
        //}

        #region Callback Function
        //public void PrinterNotifyWaitCallback(Object state,bool timedOut)
        //{
        //    logger.Debug("---------------- Received Message Event");
        //    try
        //    {
        //        if (_printerHandle == IntPtr.Zero)
        //        {
        //            #region reset the Event and wait for the next event
        //            _mrEvent.Reset();
        //            _waitHandle = ThreadPool.RegisterWaitForSingleObject(_mrEvent, new WaitOrTimerCallback(PrinterNotifyWaitCallback), _mrEvent, -1, true);
        //            #endregion
        //            return;
        //        }
        //        #region read notification details
        //        _notifyOptions.Count = 1;
        //        //_notifyOptions.dwFlags
        //        int pdwChange = 0;
        //        IntPtr pNotifyInfo = IntPtr.Zero;
        //        bool bResult = FindNextPrinterChangeNotification(_changeHandle, out pdwChange, _notifyOptions, out pNotifyInfo);
        //        logger.Info($"---------------- pdwChange: {pdwChange}");
        //        if (pNotifyInfo != IntPtr.Zero)
        //        {
        //            if (bResult)
        //            {
        //                bool addJob = (pdwChange & 256) == 256;
        //                bool setJob = (pdwChange & 512) == 512;
        //                bool deleteJob = (pdwChange & 1024) == 1024;
        //                bool job = (pdwChange & 2048) == 2048;
        //                #endregion

        //                #region populate Notification Information
        //                //Now, let us initialize and populate the Notify Info data
        //                PRINTER_NOTIFY_INFO info = (PRINTER_NOTIFY_INFO)Marshal.PtrToStructure(pNotifyInfo, typeof(PRINTER_NOTIFY_INFO));
        //                int pData = (int)pNotifyInfo + Marshal.SizeOf(typeof(PRINTER_NOTIFY_INFO));
        //                PRINTER_NOTIFY_INFO_DATA[] data = new PRINTER_NOTIFY_INFO_DATA[info.Count];
        //                //int count = Marshal.ReadInt32(pNotifyInfo,(PrintComponent.IntPtrSize * 2));
        //                logger.Info($"---------------- start looping pNotifyInfo.count: {info.Count.ToString()}");
        //                for (uint i = 0; i < info.Count; i++)
        //                {
        //                    logger.Info($"**** loop pNotifyInfo: {i.ToString()}");
        //                    data[i] = (PRINTER_NOTIFY_INFO_DATA)Marshal.PtrToStructure((IntPtr)pData, typeof(PRINTER_NOTIFY_INFO_DATA));
        //                    ushort t = (ushort)data[i].Type;
        //                    logger.Info($"!!!!! Type: {t.ToString()}");
        //                    //if(data[i].Type == (0x01))
        //                    //{

        //                    //}
        //                    int stat = (int)data[i].NotifyData.Data.cbBuf;
        //                    int intJobID = (int)data[i].Id;

        //                    //byte[] mypBuf = (byte[])Marshal.PtrToStructure((IntPtr)data[i].NotifyData.Data.pBuf, typeof(byte[]));
        //                    logger.Info($"!!!!! stat(data[i].NotifyData.Data.cbBuf): {stat.ToString()}");
        //                    logger.Info($"!!!!! jobId: {intJobID.ToString()}");
        //                    if (stat > 0)
        //                    {
        //                        if (intJobID > 0)
        //                        {
        //                            pq = new PrintQueue(new PrintServer(), _selectedPrinter);
        //                            int queueCount = pq.NumberOfJobs;
        //                            string name = string.Empty;
        //                            DateTime jobSubmitted = new DateTime();
        //                            PrintSystemJobInfo ji = null;
        //                            try
        //                            {
        //                                ji = pq.GetJob(intJobID);
        //                                name = ji.Name;
        //                                jobSubmitted = ji.TimeJobSubmitted;
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                ji = null;
        //                            }
        //                            if (OnJobStatusChange != null)
        //                            {
        //                                //Let us raise the event
        //                                OnJobStatusChange(this, new PrintJobChangeEventArgs(intJobID, stat, pdwChange, addJob, setJob, deleteJob, job, queueCount, name, jobSubmitted));
        //                            }
        //                            if (OnJobLogChange != null)
        //                            {
        //                                //Let us raise the event
        //                                OnJobLogChange(this, new PrintJobLogEventArgs(intJobID, stat, pdwChange, queueCount, name, jobSubmitted));
        //                            }
        //                        }
        //                    }

        //                    pData += Marshal.SizeOf(typeof(PRINTER_NOTIFY_INFO_DATA));
        //                    logger.Info($"**** done loop pNotifyInfo: {i.ToString()}");
        //                }
        //                logger.Info($"---------------- out of loop pNotifyInfo.count");
        //                #endregion
        //            }
        //        }
        //        logger.Info($"----------------End of Message Event");
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("Handling Event Error");
        //        logger.Error(ex.Message);
        //        logger.Error(ex.InnerException);
        //        logger.Error(ex.StackTrace);
        //    }
        //    finally
        //    {
        //        #region reset the Event and wait for the next event
        //        logger.Info($"----------------Reset the Event and wait for the next event");
        //        _mrEvent.Reset();
        //        _waitHandle = ThreadPool.RegisterWaitForSingleObject(_mrEvent, new WaitOrTimerCallback(PrinterNotifyWaitCallback), _mrEvent, -1, true);
        //        #endregion
        //    }
        //}
        #endregion
    }
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //public class DevMode
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    //    internal string dmDeviceName;
    //    internal short dmSpecVersion;
    //    internal short dmDriverVersion;
    //    internal short dmSize;
    //    internal short dmDriverExtra;
    //    internal int dmFields;
    //    internal short dmOrientation;
    //    internal short dmPaperSize;
    //    internal short dmPaperLength;
    //    internal short dmPaperWidth;
    //    internal short dmScale;
    //    internal short dmCopies;
    //    internal short dmDefaultSource;
    //    internal short dmPrintQuality;
    //    internal short dmColor;
    //    internal short dmDuplex;
    //    internal short dmYResolution;
    //    internal short dmTTOption;
    //    internal short dmCollate;
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    //    internal string dmFormName;
    //    internal UInt16 dmLogPixels;
    //    internal UInt32 dmBitsPerPel;
    //    internal UInt32 dmPelsWidth;
    //    internal UInt32 dmPelsHeight;
    //    internal UInt32 dmNup;
    //    internal UInt32 dmDisplayFrequency;
    //    internal UInt32 dmICMMethod;
    //    internal UInt32 dmICMIntent;
    //    internal UInt32 dmMediaType;
    //    internal UInt32 dmDitherType;
    //    internal UInt32 dmReserved1;
    //    internal UInt32 dmReserved2;
    //    internal UInt32 dmPanningWidth;
    //    internal UInt32 dmPanningHeight;




    //    internal DevMode(IntPtr DevModeHandle)
    //    {
    //        Marshal.PtrToStructure(DevModeHandle, this);
    //    }

    //    public override string ToString()
    //    {
    //        return FormName;
    //    }

    //    [Description(@"Specifies the 'friendly' name of the printer or display; for example, 'PCL/HP LaserJet' in the case of PCL/HP LaserJet. This string is unique among device drivers. Note that this name may be truncated.")]
    //    public string DeviceName
    //    {
    //        get { if (dmDeviceName == null) return ""; return dmDeviceName; }
    //    }
    //    [Description(@"Specifies the version number of the initialization data specification on which the structure is based.")]
    //    public short SpecVersion
    //    {
    //        get
    //        {
    //            return dmSpecVersion;
    //        }
    //    }
    //    [Description(@"Specifies the driver version number assigned by the driver developer.")]
    //    public short DriverVersion
    //    {
    //        get { return dmDriverVersion; }
    //    }
    //    [Description(@"Selects the orientation of the paper. This member can be either ORIENT_PORTRAIT (1) or ORIENT_LANDSCAPE (2).")]
    //    public Page_Orientation Orientation
    //    {
    //        get
    //        {
    //            if (dmOrientation == 0) return Page_Orientation.PORTRAIT;
    //            Page_Orientation p;
    //            try { p = (Page_Orientation)dmOrientation; }
    //            catch { p = Page_Orientation.PORTRAIT; }
    //            return p;
    //        }
    //        set
    //        {
    //            if ((dmOrientation != 0) && (dmOrientation != (short)value) && ((PrintComponent.SafePrinterHandle.PrinterInfoQ.Fields & 0x1) == 0x1))
    //            {
    //                dmOrientation = (short)value;
    //                UpdateDevMode(0x1);
    //            }
    //        }
    //    }
    //    [Description(@"Specifies the name of the form to use; for example, 'Letter' or 'Legal'.")]
    //    [TypeConverter(typeof(qPrintComponent.qPaperSizeConverter))]
    //    public string FormName
    //    {
    //        get { return dmFormName; }
    //        set
    //        {
    //            if (FormName != value)
    //            {
    //                int f = PrintComponent.SafePrinterHandle.PrinterInfoQ.sPaperNames.IndexOf(value);
    //                if (f != -1)
    //                {
    //                    System.Drawing.Point pt = PrintComponent.SafePrinterHandle.PrinterInfoQ.PaperSizes[f];
    //                    dmFormName = value;
    //                    dmPaperSize = (short)PrintComponent.SafePrinterHandle.PrinterInfoQ.Papers[f];
    //                    dmPaperLength = (short)(pt.Y);
    //                    dmPaperWidth = (short)(pt.X);
    //                    UpdateDevMode(2 | 4 | 8 | 0x10000);
    //                }
    //            }
    //        }
    //    }
    //    [Description(@"Selects the size of the paper to print on.")]
    //    public short PaperSize
    //    {
    //        get
    //        { return dmPaperSize; }
    //    }

    //    [Description(@"The length of the paper specified by the dmPaperSize member, either for custom paper sizes or for devices such as dot-matrix printers that can print on a page of arbitrary length. These values, along with all other values in this structure that specify a physical length, are in tenths of a millimeter.")]
    //    public short PaperLength
    //    {
    //        get { return dmPaperLength; }
    //    }
    //    [Description(@"The width of the paper specified by the dmPaperSize member, either for custom paper sizes or for devices such as dot-matrix printers that can print on a page of arbitrary length. These values, along with all other values in this structure that specify a physical length, are in tenths of a millimeter.")]
    //    public short PaperWidth
    //    {
    //        get { return dmPaperWidth; }
    //    }
    //    [Description(@"Specifies the factor by which the printed output is to be scaled. The apparent page size is scaled from the physical page size by a factor of dmScale/100. For example, a letter-sized page with a dmScale value of 50 would contain as much data as a page of 17- by 22-inches because the output text and graphics would be half their original height and width.")]
    //    public short Scale
    //    {
    //        get { return dmScale; }
    //        set
    //        {
    //            if ((value < 1001) && (value > 0) && (value != dmScale) && ((PrintComponent.SafePrinterHandle.PrinterInfoQ.Fields & 0x10) == 0x10))
    //            {
    //                dmScale = value;
    //                UpdateDevMode(0x10);
    //            }
    //        }
    //    }
    //    [Description(@"Selects the number of copies printed if the device supports multiple-page copies.")]
    //    public short Copies
    //    {
    //        get { return dmCopies; }
    //        set
    //        {
    //            if ((value != dmCopies) && ((PrintComponent.SafePrinterHandle.PrinterInfoQ.Fields & 0x100) == 0x100) && (value > 0) && (value <= PrintComponent.SafePrinterHandle.PrinterInfoQ.Copies))
    //            {
    //                dmCopies = value;
    //                UpdateDevMode(0x100);
    //            }
    //            else
    //                System.Windows.Forms.MessageBox.Show("The value has to be between 1 and " + PrintComponent.SafePrinterHandle.PrinterInfoQ.Copies.ToString());
    //        }
    //    }

    //    [Description(@"Specifies the paper source.")]
    //    [TypeConverter(typeof(qDefaultSourceConverter))]
    //    public string DefaultSource
    //    {
    //        get
    //        {
    //            string s = string.Empty;
    //            try
    //            {
    //                int i = PrintComponent.SafePrinterHandle.PrinterInfoQ.iBinNames.IndexOf(dmDefaultSource);
    //                if (i != -1)
    //                    s = PrintComponent.SafePrinterHandle.PrinterInfoQ.sBinNames[i];
    //            }
    //            catch { }
    //            return s;

    //        }
    //        [PrintingPermission(SecurityAction.Demand, Level = PrintingPermissionLevel.AllPrinting)]
    //        set
    //        {
    //            if (DefaultSource != value)
    //            {
    //                int i = PrintComponent.SafePrinterHandle.PrinterInfoQ.sBinNames.IndexOf(value);
    //                if (i != -1)
    //                {
    //                    dmDefaultSource = (short)PrintComponent.SafePrinterHandle.PrinterInfoQ.iBinNames[i];
    //                    UpdateDevMode(0x200);
    //                }
    //            }
    //        }
    //    }

    //    [Description(@"DPI")]
    //    [TypeConverter(typeof(qPrintQualityConverter))]
    //    public string PrintQuality
    //    {
    //        get { return dmPrintQuality.ToString() + " DPI"; }
    //        set
    //        {
    //            value = value.Replace(" DPI", "");
    //            if (short.TryParse(value, out dmPrintQuality))
    //            {
    //                dmYResolution = dmPrintQuality;
    //                UpdateDevMode(0x400 | 0x2000);
    //            }
    //        }
    //    }
    //    [Description(@"Switches between color and monochrome on color printers.")]
    //    public Print_Color Color
    //    {
    //        get
    //        {
    //            if (dmColor == 0) return Print_Color.MONOCHROME;
    //            Print_Color c;
    //            try { c = (Print_Color)dmColor; }
    //            catch { c = Print_Color.MONOCHROME; }
    //            return c;
    //        }
    //        set
    //        {
    //            if ((dmColor != (short)value) && ((PrintComponent.SafePrinterHandle.PrinterInfoQ.Fields & 0x800) == 0x800) && PrintComponent.SafePrinterHandle.PrinterInfoQ.CanProcessColor)
    //            {
    //                dmColor = (short)value;
    //                UpdateDevMode(0x800);
    //            }
    //            else
    //                System.Windows.Forms.MessageBox.Show("The printer does not support color");
    //        }
    //    }
    //    [Description(@"Selects duplex or double-sided printing for printers capable of duplex printing.")]
    //    public Page_Duplex Duplex
    //    {
    //        get
    //        {
    //            if (dmDuplex == 0) return Page_Duplex.SIMPLEX;
    //            Page_Duplex d;
    //            try { d = (Page_Duplex)dmDuplex; }
    //            catch { d = Page_Duplex.SIMPLEX; }
    //            return d;
    //        }
    //        set
    //        {
    //            if ((dmDuplex != (short)value) && ((PrintComponent.SafePrinterHandle.PrinterInfoQ.Fields & 0x1000) == 0x1000) && PrintComponent.SafePrinterHandle.PrinterInfoQ.CanDuplex)
    //            {
    //                dmDuplex = (short)value;
    //                UpdateDevMode(0x1000);
    //            }
    //            else
    //                System.Windows.Forms.MessageBox.Show("The printer does not support duplexing");
    //        }
    //    }
    //    [Description(@"Specifies how TrueType fonts should be printed.")]
    //    public Print_TrueType TTOption
    //    {
    //        get
    //        {
    //            if (dmTTOption == 0) return Print_TrueType.BITMAP;
    //            Print_TrueType t;
    //            try { t = (Print_TrueType)dmTTOption; }
    //            catch { t = Print_TrueType.BITMAP; }
    //            return t;
    //        }
    //    }
    //    [Description(@"Specifies whether collation should be used when printing multiple copies.")]
    //    public Printer_Collate Collate
    //    {
    //        get
    //        {
    //            if (dmCollate == 0) return Printer_Collate.FALSE;
    //            Printer_Collate c;
    //            try { c = (Printer_Collate)dmCollate; }
    //            catch { c = Printer_Collate.FALSE; }
    //            return c;
    //        }
    //        set
    //        {
    //            if ((dmCollate != (short)value) && ((PrintComponent.SafePrinterHandle.PrinterInfoQ.Fields & 0x8000) == 0x8000) && PrintComponent.SafePrinterHandle.PrinterInfoQ.CanCollate)
    //            {
    //                dmCollate = (short)value;
    //                UpdateDevMode(0x8000);
    //            }
    //            else
    //                System.Windows.Forms.MessageBox.Show("The printer does not support collating");
    //        }
    //    }

    //    [Description(@"Specifies where the NUP is done.")]
    //    public UInt32 Nup
    //    {
    //        get { return dmNup; }
    //    }
    //    [Description(@"Specifies how ICM is handled. For a non-ICM application, this member determines if ICM is enabled or disabled. For ICM applications, the system examines this member to determine how to handle ICM support.")]
    //    public Printer_ICMMethod ICMMethod
    //    {
    //        get
    //        {
    //            if (dmICMMethod == 0) return Printer_ICMMethod.NONE;
    //            Printer_ICMMethod m;
    //            try { m = (Printer_ICMMethod)dmICMMethod; }
    //            catch { m = Printer_ICMMethod.USER; }
    //            return m;
    //        }
    //    }
    //    [Description(@"Specifies which color matching method, or intent, should be used by default. This member is primarily for non-ICM applications. ICM applications can establish intents by using the ICM functions.")]
    //    public Printer_ICMIntend ICMIntent
    //    {
    //        get
    //        {
    //            if (dmICMIntent == 0) return Printer_ICMIntend.USER;
    //            Printer_ICMIntend i;
    //            try { i = (Printer_ICMIntend)dmICMIntent; }
    //            catch { i = Printer_ICMIntend.USER; }
    //            return i;
    //        }
    //    }
    //    [Description(@"Specifies the type of media being printed on. ")]
    //    public string MediaType
    //    {
    //        get
    //        {
    //            string s = string.Empty;
    //            try
    //            {
    //                int i = PrintComponent.SafePrinterHandle.PrinterInfoQ.MediaTypes.IndexOf((int)dmMediaType);
    //                if (i != -1)
    //                    s = PrintComponent.SafePrinterHandle.PrinterInfoQ.MediaNames[i];
    //            }
    //            catch { }
    //            return s;
    //        }
    //    }
    //    [Description(@"Specifies how dithering is to be done.")]
    //    public Printer_Dither DitherType
    //    {
    //        get
    //        {
    //            if (dmDitherType == 0) return Printer_Dither.NONE;
    //            Printer_Dither d;
    //            try { d = (Printer_Dither)dmDitherType; }
    //            catch { d = Printer_Dither.USER; }
    //            return d;
    //        }
    //    }


    //    private bool UpdateDevMode(int field)
    //    {
    //        if (!PrintComponent.SafePrinterHandle.canUpdate)
    //            System.Windows.Forms.MessageBox.Show("no sufficient rights to Update");
    //        if (PrintComponent.SafePrinterHandle.IsInvalid) return false;
    //        dmFields = field;
    //        IntPtr ptr1 = PrintComponent.SafePrinterHandle.PrinterInfo2.GetIntPtrField(7);
    //        Marshal.StructureToPtr(this, ptr1, true);
    //        return PrintComponent.SafePrinterHandle.PrinterInfo2.SetIntPtrField(7, ptr1);
    //    }
    //}

    [StructLayout(LayoutKind.Sequential)]
    public class qPrinterNotifyOptions
    {
        public Int32 dwVersion;
        public Int32 dwFlags;
        public Int32 Count;
        public IntPtr lpTypes;

        internal qPrinterNotifyOptions(bool refresh)
        {
            dwVersion = 2;
            if (refresh)
                dwFlags = 1;
            else
                dwFlags = 0;
            Count = 2;
            qPrinterNotifyOptionsType type1 = new qPrinterNotifyOptionsType();
            int num1 = Marshal.SizeOf(type1);
            this.lpTypes = Marshal.AllocHGlobal(num1);
            Marshal.StructureToPtr(type1, this.lpTypes, true);
        }


        ~qPrinterNotifyOptions()
        {
            if (lpTypes != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(lpTypes);
            }
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public class qPrinterNotifyOptionsType
    {
        public short wPrinterType;
        public short wPrinterReserved0;
        public int dwPrinterReserved1;
        public int dwPrinterReserved2;
        public int PrinterFieldCount;
        public IntPtr pPrinterFields;
        public short wJobType;
        public short wJobReserved0;
        public int dwJobReserved1;
        public int dwJobReserved2;
        public int JobFieldCount;
        public IntPtr pJobFields;

        public qPrinterNotifyOptionsType()
        {
            this.wPrinterType = 0;
            this.PrinterFieldCount = 20;
            this.pPrinterFields = Marshal.AllocCoTaskMem(42);

            //Printer_Notify_Field_Indexes.SERVER_NAME  //not supported
            Marshal.WriteInt16(this.pPrinterFields, 0, (short)Printer_Notify_Field_Indexes.PRINTER_NAME);
            Marshal.WriteInt16(this.pPrinterFields, 2, (short)Printer_Notify_Field_Indexes.SHARE_NAME);
            Marshal.WriteInt16(this.pPrinterFields, 4, (short)Printer_Notify_Field_Indexes.PORT_NAME);
            Marshal.WriteInt16(this.pPrinterFields, 6, (short)Printer_Notify_Field_Indexes.DRIVER_NAME);
            Marshal.WriteInt16(this.pPrinterFields, 8, (short)Printer_Notify_Field_Indexes.COMMENT);
            Marshal.WriteInt16(this.pPrinterFields, 10, (short)Printer_Notify_Field_Indexes.LOCATION);
            Marshal.WriteInt16(this.pPrinterFields, 12, (short)Printer_Notify_Field_Indexes.DEVMODE);
            Marshal.WriteInt16(this.pPrinterFields, 14, (short)Printer_Notify_Field_Indexes.SEPFILE);
            Marshal.WriteInt16(this.pPrinterFields, 16, (short)Printer_Notify_Field_Indexes.PRINT_PROCESSOR);
            Marshal.WriteInt16(this.pPrinterFields, 18, (short)Printer_Notify_Field_Indexes.PARAMETERS);
            Marshal.WriteInt16(this.pPrinterFields, 20, (short)Printer_Notify_Field_Indexes.DATATYPE);
            Marshal.WriteInt16(this.pPrinterFields, 22, (short)Printer_Notify_Field_Indexes.SECURITY_DESCRIPTOR);
            Marshal.WriteInt16(this.pPrinterFields, 24, (short)Printer_Notify_Field_Indexes.ATTRIBUTES);
            Marshal.WriteInt16(this.pPrinterFields, 26, (short)Printer_Notify_Field_Indexes.PRIORITY);
            Marshal.WriteInt16(this.pPrinterFields, 28, (short)Printer_Notify_Field_Indexes.DEFAULT_PRIORITY);
            Marshal.WriteInt16(this.pPrinterFields, 30, (short)Printer_Notify_Field_Indexes.START_TIME);
            Marshal.WriteInt16(this.pPrinterFields, 32, (short)Printer_Notify_Field_Indexes.UNTIL_TIME);
            Marshal.WriteInt16(this.pPrinterFields, 34, (short)Printer_Notify_Field_Indexes.STATUS);
            //Printer_Notify_Field_Indexes.STATUS_STRING //not supported
            Marshal.WriteInt16(this.pPrinterFields, 36, (short)Printer_Notify_Field_Indexes.CJOBS);
            //Marshal.WriteInt16(this.pPrinterFields, 38, (short)Printer_Notify_Field_Indexes.AVERAGE_PPM);
            //Printer_Notify_Field_Indexes.TOTAL_PAGES //not supported
            //Printer_Notify_Field_Indexes.PAGES_PRINTED //not supported
            //Printer_Notify_Field_Indexes.TOTAL_BYTES //not supported
            Marshal.WriteInt16(this.pPrinterFields, 38, (short)Printer_Notify_Field_Indexes.BYTES_PRINTED);//not supported
            Marshal.WriteInt16(this.pPrinterFields, 40, (short)Printer_Notify_Field_Indexes.OBJECT_GUID);


            this.wJobType = 1;
            this.JobFieldCount = 22;
            this.pJobFields = Marshal.AllocCoTaskMem(46);
            Marshal.WriteInt16(this.pJobFields, 0, (short)Job_Notify_Field_Indexes.PRINTER_NAME);
            Marshal.WriteInt16(this.pJobFields, 2, (short)Job_Notify_Field_Indexes.MACHINE_NAME);
            Marshal.WriteInt16(this.pJobFields, 4, (short)Job_Notify_Field_Indexes.PORT_NAME);
            Marshal.WriteInt16(this.pJobFields, 6, (short)Job_Notify_Field_Indexes.USER_NAME);
            Marshal.WriteInt16(this.pJobFields, 8, (short)Job_Notify_Field_Indexes.NOTIFY_NAME);
            Marshal.WriteInt16(this.pJobFields, 10, (short)Job_Notify_Field_Indexes.DATATYPE);
            Marshal.WriteInt16(this.pJobFields, 12, (short)Job_Notify_Field_Indexes.PRINT_PROCESSOR);
            Marshal.WriteInt16(this.pJobFields, 14, (short)Job_Notify_Field_Indexes.PARAMETERS);
            Marshal.WriteInt16(this.pJobFields, 16, (short)Job_Notify_Field_Indexes.DRIVER_NAME);
            Marshal.WriteInt16(this.pJobFields, 18, (short)Job_Notify_Field_Indexes.DEVMODE);
            Marshal.WriteInt16(this.pJobFields, 20, (short)Job_Notify_Field_Indexes.STATUS);
            Marshal.WriteInt16(this.pJobFields, 22, (short)Job_Notify_Field_Indexes.STATUS_STRING);
            //Job_Notify_Field_Indexes.SECURITY_DESCRIPTOR) //Not supported
            Marshal.WriteInt16(this.pJobFields, 24, (short)Job_Notify_Field_Indexes.DOCUMENT);
            Marshal.WriteInt16(this.pJobFields, 26, (short)Job_Notify_Field_Indexes.PRIORITY);
            Marshal.WriteInt16(this.pJobFields, 28, (short)Job_Notify_Field_Indexes.POSITION);
            Marshal.WriteInt16(this.pJobFields, 30, (short)Job_Notify_Field_Indexes.SUBMITTED);
            Marshal.WriteInt16(this.pJobFields, 32, (short)Job_Notify_Field_Indexes.START_TIME);
            Marshal.WriteInt16(this.pJobFields, 34, (short)Job_Notify_Field_Indexes.UNTIL_TIME);
            Marshal.WriteInt16(this.pJobFields, 36, (short)Job_Notify_Field_Indexes.TIME);
            Marshal.WriteInt16(this.pJobFields, 38, (short)Job_Notify_Field_Indexes.TOTAL_PAGES);
            Marshal.WriteInt16(this.pJobFields, 40, (short)Job_Notify_Field_Indexes.PAGES_PRINTED);
            Marshal.WriteInt16(this.pJobFields, 42, (short)Job_Notify_Field_Indexes.TOTAL_BYTES);
            Marshal.WriteInt16(this.pJobFields, 44, (short)Job_Notify_Field_Indexes.BYTES_PRINTED);
        }
        ~qPrinterNotifyOptionsType()
        {
            Marshal.FreeCoTaskMem(this.pJobFields);
            Marshal.FreeCoTaskMem(this.pPrinterFields);
        }
    }

    public enum Page_Orientation : short
    {
        PORTRAIT = 1,
        LANDSCAPE = 2
    }

    public enum Page_Duplex : short
    {
        SIMPLEX = 1,
        VERTICAL = 2,
        HORIZONTAL = 3
    }
    public enum Printer_Collate : short
    {
        FALSE = 0,
        TRUE = 1
    }
    public enum Printer_ICMIntend : short
    {
        SATURATE = 1,
        CONTRAST = 2,
        COLORMETRIC = 3,
        USER = 0x100
    }
    public enum Printer_ICMMethod : short
    {
        NONE = 1,
        SYSTEM = 2,
        DRIVER = 3,
        DEVICE = 4,
        USER = 0x100
    }
    public enum Printer_Dither : short
    {
        None = 0,
        NONE = 1,
        COARSE = 2,
        FINE = 3,
        LINEART = 4,
        GRAYSCALE = 5,
        USER = 0x100
    }
    public enum Printer_Media : short
    {
        STANDARD = 1,
        TRANSPARENCY = 2,
        GLOSSY = 3,
        USER = 0x100
    }
    public enum Paper_Source : short
    {
        FIRST = UPPER,
        UPPER = 1,
        ONLYONE = 1,
        LOWER = 2,
        MIDDLE = 3,
        MANUAL = 4,
        ENVELOPE = 5,
        ENVMANUAL = 6,
        AUTO = 7,
        TRACTOR = 8,
        SMALLFMT = 9,
        LARGEFMT = 10,
        LARGECAPACITY = 11,
        CASSETTE = 14,
        FORMSOURCE = 15,
        LAST = FORMSOURCE,
        user = 256
    }

    public enum Print_Color : short
    {
        /* color enable/disable for color printers */
        MONOCHROME = 1,
        COLOR = 2,
    }
    public enum Print_TrueType : short
    {
        BITMAP = 1,
        DOWNLOAD = 2,
        SUBDEV = 3,
        DOWNLOAD_OUTLINE = 4,
    }


    public enum PrinterControl : int
    {
        Nul = 0,
        Pause = 1,
        Resume = 2,
        Purge = 3,
        SetStatus = 4
    }

    [Flags]
    public enum Printer_Change : uint
    {
        ADD_PRINTER = 0x00000001,
        SET_PRINTER = 0x00000002,
        DELETE_PRINTER = 0x00000004,
        FAILED_CONNECTION_PRINTER = 0x00000008,
        PRINTER = 0x000000FF,
        ADD_JOB = 0x00000100,
        SET_JOB = 0x00000200,
        DELETE_JOB = 0x00000400,
        WRITE_JOB = 0x00000800,
        JOB = 0x0000FF00,
        ADD_FORM = 0x00010000,
        SET_FORM = 0x00020000,
        DELETE_FORM = 0x00040000,
        FORM = 0x00070000,
        ADD_PORT = 0x00100000,
        CONFIGURE_PORT = 0x00200000,
        DELETE_PORT = 0x00400000,
        PORT = 0x00700000,
        ADD_PRINT_PROCESSOR = 0x01000000,
        DELETE_PRINT_PROCESSOR = 0x04000000,
        PRINT_PROCESSOR = 0x07000000,
        ADD_PRINTER_DRIVER = 0x10000000,
        SET_PRINTER_DRIVER = 0x20000000,
        DELETE_PRINTER_DRIVER = 0x40000000,
        PRINTER_DRIVER = 0x70000000,
        TIMEOUT = 0x80000000,
        ALL = 0x7777FFFF
    }
    public enum Printer_Notification_Types : short
    {
        PRINTER_NOTIFY_TYPE = 0,
        JOB_NOTIFY_TYPE = 1
    }
    public enum Printer_Notify_Field_Indexes
    {
        SERVER_NAME = 0,
        PRINTER_NAME = 1,
        SHARE_NAME = 2,
        PORT_NAME = 3,
        DRIVER_NAME = 4,
        COMMENT = 5,
        LOCATION = 6,
        DEVMODE = 7,
        SEPFILE = 8,
        PRINT_PROCESSOR = 9,
        PARAMETERS = 10,
        DATATYPE = 11,
        SECURITY_DESCRIPTOR = 12,
        ATTRIBUTES = 13,
        PRIORITY = 14,
        DEFAULT_PRIORITY = 15,
        START_TIME = 16,
        UNTIL_TIME = 17,
        STATUS = 18,
        STATUS_STRING = 19,
        CJOBS = 20,
        AVERAGE_PPM = 21,
        TOTAL_PAGES = 22,
        PAGES_PRINTED = 23,
        TOTAL_BYTES = 24,
        BYTES_PRINTED = 25,
        OBJECT_GUID = 26
    }

    public enum Job_Notify_Field_Indexes
    {
        PRINTER_NAME = 0,
        MACHINE_NAME = 1,
        PORT_NAME = 2,
        USER_NAME = 3,
        NOTIFY_NAME = 4,
        DATATYPE = 5,
        PRINT_PROCESSOR = 6,
        PARAMETERS = 7,
        DRIVER_NAME = 8,
        DEVMODE = 9,
        STATUS = 10,
        STATUS_STRING = 11,
        SECURITY_DESCRIPTOR = 12,
        DOCUMENT = 13,
        PRIORITY = 14,
        POSITION = 15,
        SUBMITTED = 16,
        START_TIME = 17,
        UNTIL_TIME = 18,
        TIME = 19,
        TOTAL_PAGES = 20,
        PAGES_PRINTED = 21,
        TOTAL_BYTES = 22,
        BYTES_PRINTED = 23//,
        //ERROR = 999
    }
    [Flags]
    public enum Job_Status
    {
        BLOCKED_DEVICEQUEUE = 0x200,
        DELETED = 0x100,
        DELETING = 4,
        ERROR = 2,
        OFFLINE = 0x20,
        PAPEROUT = 0x40,
        PAUSED = 1,
        PRINTED = 0x80,
        PRINTING = 0x10,
        RESTART = 0x800,
        SPOOLING = 8,
        INTERVENTION = 0x400
    }

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

/**
pdwChange [out, optional]

A pointer to a variable whose bits are set to indicate the changes that occurred to cause the most recent notification. The bit flags that might be set correspond to those specified in the fdwFilter parameter of the FindFirstPrinterChangeNotification call. The system sets one or more of the following bit flags.

Value	                                    Hex             Meaning
PRINTER_CHANGE_ADD_FORM                     0x10000         A form was added to the server.
PRINTER_CHANGE_ADD_JOB                      0x100           A print job was sent to the printer.
PRINTER_CHANGE_ADD_PORT                     0x100000        A port or monitor was added to the server.
PRINTER_CHANGE_ADD_PRINT_PROCESSOR          0x1000000       A print processor was added to the server.
PRINTER_CHANGE_ADD_PRINTER                  0x1             A printer was added to the server.
PRINTER_CHANGE_ADD_PRINTER_DRIVER           0x10000000      A printer driver was added to the server.
PRINTER_CHANGE_CONFIGURE_PORT               0x200000        A port was configured on the server.
PRINTER_CHANGE_DELETE_FORM                  0x40000         A form was deleted from the server.
PRINTER_CHANGE_DELETE_JOB                   0x400           A job was deleted.
PRINTER_CHANGE_DELETE_PORT                  0x400000        A port or monitor was deleted from the server.
PRINTER_CHANGE_DELETE_PRINT_PROCESSOR       0x4000000       A print processor was deleted from the server.
PRINTER_CHANGE_DELETE_PRINTER               0x4             A printer was deleted.
PRINTER_CHANGE_DELETE_PRINTER_DRIVER        0x40000000      A printer driver was deleted from the server.
PRINTER_CHANGE_FAILED_CONNECTION_PRINTER    0x8             A printer connection has failed.
PRINTER_CHANGE_SET_FORM                     0x20000         A form was set on the server.
PRINTER_CHANGE_SET_JOB                      0x200           A job was set.
PRINTER_CHANGE_SET_PRINTER                  0x2             A printer was set.
PRINTER_CHANGE_SET_PRINTER_DRIVER           0x20000000      A printer driver was set.
PRINTER_CHANGE_WRITE_JOB                    0x800           Job data was written.
PRINTER_CHANGE_TIMEOUT                      0x80000000      The job timed out.
PRINTER_CHANGE_SERVER                                       Windows 7: A change occurred on the server.
**/

/**
PrinterChangeNotificationFields
PRINTER_NOTIFY_FIELD_SERVER_NAME = 0x0
PRINTER_NOTIFY_FIELD_PRINTER_NAME = 0x1
PRINTER_NOTIFY_FIELD_SHARE_NAME = 0x2
PRINTER_NOTIFY_FIELD_PORT_NAME = 0x3
PRINTER_NOTIFY_FIELD_DRIVER_NAME = 0x4
PRINTER_NOTIFY_FIELD_COMMENT = 0x5
PRINTER_NOTIFY_FIELD_LOCATION = 0x6
PRINTER_NOTIFY_FIELD_DEVMODE = 0x7
PRINTER_NOTIFY_FIELD_SEPFILE = 0x8
PRINTER_NOTIFY_FIELD_PRINT_PROCESSOR = 0x9
PRINTER_NOTIFY_FIELD_PARAMETERS = 0xA
PRINTER_NOTIFY_FIELD_DATATYPE = 0xB
PRINTER_NOTIFY_FIELD_SECURITY_DESCRIPTOR = 0xC
PRINTER_NOTIFY_FIELD_ATTRIBUTES = 0xD
PRINTER_NOTIFY_FIELD_PRIORITY = 0xE
PRINTER_NOTIFY_FIELD_DEFAULT_PRIORITY = 0xF
PRINTER_NOTIFY_FIELD_START_TIME = 0x10
PRINTER_NOTIFY_FIELD_UNTIL_TIME = 0x11
PRINTER_NOTIFY_FIELD_STATUS = 0x12
PRINTER_NOTIFY_FIELD_STATUS_STRING = 0x13
PRINTER_NOTIFY_FIELD_CJOBS = 0x14
PRINTER_NOTIFY_FIELD_AVERAGE_PPM = 0x15
PRINTER_NOTIFY_FIELD_TOTAL_PAGES = 0x16
PRINTER_NOTIFY_FIELD_PAGES_PRINTED = 0x17
PRINTER_NOTIFY_FIELD_TOTAL_BYTES = 0x18
PRINTER_NOTIFY_FIELD_BYTES_PRINTED = 0x19
PRINTER_NOTIFY_FIELD_OBJECT_GUID = 0x1A


JobChangeNotificationFields
JOB_NOTIFY_FIELD_PRINTER_NAME = 0x0
JOB_NOTIFY_FIELD_MACHINE_NAME = 0x1
JOB_NOTIFY_FIELD_PORT_NAME = 0x2
JOB_NOTIFY_FIELD_USER_NAME = 0x3
JOB_NOTIFY_FIELD_NOTIFY_NAME = 0x4
JOB_NOTIFY_FIELD_DATATYPE = 0x5
JOB_NOTIFY_FIELD_PRINT_PROCESSOR = 0x6
JOB_NOTIFY_FIELD_PARAMETERS = 0x7
JOB_NOTIFY_FIELD_DRIVER_NAME = 0x8
JOB_NOTIFY_FIELD_DEVMODE = 0x9
JOB_NOTIFY_FIELD_STATUS = 0xA
JOB_NOTIFY_FIELD_STATUS_STRING = 0xB
JOB_NOTIFY_FIELD_SECURITY_DESCRIPTOR = 0xC
JOB_NOTIFY_FIELD_DOCUMENT = 0xD
JOB_NOTIFY_FIELD_PRIORITY = 0xE
JOB_NOTIFY_FIELD_POSITION = 0xF
JOB_NOTIFY_FIELD_SUBMITTED = 0x10
JOB_NOTIFY_FIELD_START_TIME = 0x11
JOB_NOTIFY_FIELD_UNTIL_TIME = 0x12
JOB_NOTIFY_FIELD_TIME = 0x13
JOB_NOTIFY_FIELD_TOTAL_PAGES = 0x14
JOB_NOTIFY_FIELD_PAGES_PRINTED = 0x15
JOB_NOTIFY_FIELD_TOTAL_BYTES = 0x16
JOB_NOTIFY_FIELD_BYTES_PRINTED = 0x17


ChangeNotificationTypes
PRINTER_NOTIFY_TYPE = 0x0
JOB_NOTIFY_TYPE = 0x1

**/

/***
PRINTER_NOTIFY_INFO_DATA structure
Article
01/07/2021
6 minutes to read
6 contributors


The PRINTER_NOTIFY_INFO_DATA structure identifies a job or printer information field and provides the current data for that field.

The FindNextPrinterChangeNotification function returns a PRINTER_NOTIFY_INFO structure, which contains an array of PRINTER_NOTIFY_INFO_DATA structures.

Syntax
C++

Copy
typedef struct _PRINTER_NOTIFY_INFO_DATA {
  WORD  Type;
  WORD  Field;
  DWORD Reserved;
  DWORD Id;
  union {
    DWORD  adwData[2];
    struct {
      DWORD  cbBuf;
      LPVOID pBuf;
    } Data;
  } NotifyData;
} PRINTER_NOTIFY_INFO_DATA, *PPRINTER_NOTIFY_INFO_DATA; ;
Members
Type

Indicates the type of information provided. This member can be one of the following values.

Value	Meaning
JOB_NOTIFY_TYPE
0x01
Indicates that the Field member specifies a JOB_NOTIFY_FIELD_* constant.
PRINTER_NOTIFY_TYPE
0x00
Indicates that the Field member specifies a PRINTER_NOTIFY_FIELD_* constant.
Field

Indicates the field that changed. For a list of possible values, see the Remarks section.

Reserved

Reserved.

Id

Indicates the job identifier if the Type member specifies JOB_NOTIFY_TYPE. If the Type member specifies PRINTER_NOTIFY_TYPE, this member is undefined.

NotifyData

A union of data information based on the Type and Field members. For a description of the type of data associated with each field, see the Remarks section.

adwData[2]

An array of two DWORD values. For information fields that use only a single DWORD, the data is in adwData [0].

Data

cbBuf

Indicates the size, in bytes, of the buffer pointed to by pBuf.

pBuf

Pointer to a buffer that contains the field's current data.

Remarks
If the Type member specifies PRINTER_NOTIFY_TYPE, the Field member can be one of the following values.

Field	Type of data	Value
PRINTER_NOTIFY_FIELD_SERVER_NAME	Not supported.	0x00
PRINTER_NOTIFY_FIELD_PRINTER_NAME	pBuf is a pointer to a null-terminated string containing the name of the printer.	0x01
PRINTER_NOTIFY_FIELD_SHARE_NAME	pBuf is a pointer to a null-terminated string that identifies the share point for the printer.	0x02
PRINTER_NOTIFY_FIELD_PORT_NAME	pBuf is a pointer to a null-terminated string containing the name of the port that the print jobs will be printed to. If "Printer Pooling" is selected, this is a comma separated list of ports.	0x03
PRINTER_NOTIFY_FIELD_DRIVER_NAME	pBuf is a pointer to a null-terminated string containing the name of the printer's driver.	0x04
PRINTER_NOTIFY_FIELD_COMMENT	pBuf is a pointer to a null-terminated string containing the new comment string, which is typically a brief description of the printer.	0x05
PRINTER_NOTIFY_FIELD_LOCATION	pBuf is a pointer to a null-terminated string containing the new physical location of the printer (for example, "Bldg. 38, Room 1164").	0x06
PRINTER_NOTIFY_FIELD_DEVMODE	pBuf is a pointer to a DEVMODE structure that defines default printer data such as the paper orientation and the resolution.	0x07
PRINTER_NOTIFY_FIELD_SEPFILE	pBuf is a pointer to a null-terminated string that specifies the name of the file used to create the separator page. This page is used to separate print jobs sent to the printer.	0x08
PRINTER_NOTIFY_FIELD_PRINT_PROCESSOR	pBuf is a pointer to a null-terminated string that specifies the name of the print processor used by the printer.	0x09
PRINTER_NOTIFY_FIELD_PARAMETERS	pBuf is a pointer to a null-terminated string that specifies the default print-processor parameters.	0x0A
PRINTER_NOTIFY_FIELD_DATATYPE	pBuf is a pointer to a null-terminated string that specifies the data type used to record the print job.	0x0B
PRINTER_NOTIFY_FIELD_SECURITY_DESCRIPTOR	pBuf is a pointer to a SECURITY_DESCRIPTOR structure for the printer. The pointer may be NULL if there is no security descriptor.	0x0C
PRINTER_NOTIFY_FIELD_ATTRIBUTES	adwData [0] specifies the printer attributes, which can be one of the following values:
PRINTER_ATTRIBUTE_QUEUED
PRINTER_ATTRIBUTE_DIRECT
PRINTER_ATTRIBUTE_DEFAULT
PRINTER_ATTRIBUTE_SHARED
0x0D
PRINTER_NOTIFY_FIELD_PRIORITY	adwData [0] specifies a priority value that the spooler uses to route print jobs.	0x0E
PRINTER_NOTIFY_FIELD_DEFAULT_PRIORITY	adwData [0] specifies the default priority value assigned to each print job.	0x0F
PRINTER_NOTIFY_FIELD_START_TIME	adwData [0] specifies the earliest time at which the printer will print a job. (This value is specified in minutes elapsed since 12:00 A.M.)	0x10
PRINTER_NOTIFY_FIELD_UNTIL_TIME	adwData [0] specifies the latest time at which the printer will print a job. (This value is specified in minutes elapsed since 12:00 A.M.)	0x11
PRINTER_NOTIFY_FIELD_STATUS	adwData [0] specifies the printer status. For a list of possible values, see the PRINTER_INFO_2 structure.	0x12
PRINTER_NOTIFY_FIELD_STATUS_STRING	Not supported.	0x13
PRINTER_NOTIFY_FIELD_CJOBS	adwData [0] specifies the number of print jobs that have been queued for the printer.	0x14
PRINTER_NOTIFY_FIELD_AVERAGE_PPM	adwData [0] specifies the average number of pages per minute that have been printed on the printer.	0x15
PRINTER_NOTIFY_FIELD_TOTAL_PAGES	Not supported.	0x16
PRINTER_NOTIFY_FIELD_PAGES_PRINTED	Not supported.	0x17
PRINTER_NOTIFY_FIELD_TOTAL_BYTES	Not supported.	0x18
PRINTER_NOTIFY_FIELD_BYTES_PRINTED	Not supported.	0x19
PRINTER_NOTIFY_FIELD_OBJECT_GUID	This is set if the object GUID changes.	0x1A
PRINTER_NOTIFY_FIELD_FRIENDLY_NAME	This is set if the printer connection is renamed.	0x1B
If the Type member specifies JOB_NOTIFY_TYPE, the Field member can be one of the following values.

Field	Type of data	Value
JOB_NOTIFY_FIELD_PRINTER_NAME	pBuf is a pointer to a null-terminated string containing the name of the printer for which the job is spooled.	0x00
JOB_NOTIFY_FIELD_MACHINE_NAME	pBuf is a pointer to a null-terminated string that specifies the name of the machine that created the print job.	0x01
JOB_NOTIFY_FIELD_PORT_NAME	pBuf is a pointer to a null-terminated string that identifies the port(s) used to transmit data to the printer. If a printer is connected to more than one port, the names of the ports are separated by commas (for example, "LPT1:,LPT2:,LPT3:").	0x02
JOB_NOTIFY_FIELD_USER_NAME	pBuf is a pointer to a null-terminated string that specifies the name of the user who sent the print job.	0x03
JOB_NOTIFY_FIELD_NOTIFY_NAME	pBuf is a pointer to a null-terminated string that specifies the name of the user who should be notified when the job has been printed or when an error occurs while printing the job.	0x04
JOB_NOTIFY_FIELD_DATATYPE	pBuf is a pointer to a null-terminated string that specifies the type of data used to record the print job.	0x05
JOB_NOTIFY_FIELD_PRINT_PROCESSOR	pBuf is a pointer to a null-terminated string that specifies the name of the print processor to be used to print the job.	0x06
JOB_NOTIFY_FIELD_PARAMETERS	pBuf is a pointer to a null-terminated string that specifies print-processor parameters.	0x07
JOB_NOTIFY_FIELD_DRIVER_NAME	pBuf is a pointer to a null-terminated string that specifies the name of the printer driver that should be used to process the print job.	0x08
JOB_NOTIFY_FIELD_DEVMODE	pBuf is a pointer to a DEVMODE structure that contains device-initialization and environment data for the printer driver.	0x09
JOB_NOTIFY_FIELD_STATUS	adwData [0] specifies the job status. For a list of possible values, see the JOB_INFO_2 structure.	0x0A
JOB_NOTIFY_FIELD_STATUS_STRING	pBuf is a pointer to a null-terminated string that specifies the status of the print job.	0x0B
JOB_NOTIFY_FIELD_SECURITY_DESCRIPTOR	Not supported.	0x0C
JOB_NOTIFY_FIELD_DOCUMENT	pBuf is a pointer to a null-terminated string that specifies the name of the print job (for example, "MS-WORD: Review.doc").	0x0D
JOB_NOTIFY_FIELD_PRIORITY	adwData [0] specifies the job priority.	0x0E
JOB_NOTIFY_FIELD_POSITION	adwData [0] specifies the job's position in the print queue.	0x0F
JOB_NOTIFY_FIELD_SUBMITTED	pBuf is a pointer to a SYSTEMTIME structure that specifies the time when the job was submitted.	0x10
JOB_NOTIFY_FIELD_START_TIME	adwData [0] specifies the earliest time that the job can be printed. (This value is specified in minutes elapsed since 12:00 A.M.)	0x11
JOB_NOTIFY_FIELD_UNTIL_TIME	adwData [0] specifies the latest time that the job can be printed. (This value is specified in minutes elapsed since 12:00 A.M.)	0x12
JOB_NOTIFY_FIELD_TIME	adwData [0] specifies the total time, in seconds, that has elapsed since the job began printing.	0x13
JOB_NOTIFY_FIELD_TOTAL_PAGES	adwData [0] specifies the size, in pages, of the job.	0x14
JOB_NOTIFY_FIELD_PAGES_PRINTED	adwData [0] specifies the number of pages that have printed.	0x15
JOB_NOTIFY_FIELD_TOTAL_BYTES	adwData [0] specifies the size, in bytes, of the job.	0x16
JOB_NOTIFY_FIELD_BYTES_PRINTED	adwData [0] specifies the number of bytes that have been printed on this job. For this field, the change notification object is signaled when bytes are sent to the printer.	0x17
 ***/
