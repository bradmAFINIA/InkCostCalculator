using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;

namespace InkCostCalculatorL501EnhancedWPF
{
    public enum AfiniaBasicCommand
    {
        ToggleTransmissiveSensorType = 'A',
        InvokeBootloader = 'B',
        ForcePaperOutRecalibration = 'C',
        ToggleDebugMessages = 'D',
        ToggleUARTEcho = 'E',
        DisableUARTMessages = 'F',
        ClampPeakDetector = 'K',
        DisplayProximitySensorThresholds = 'O',
        ToggleProximitySensor = 'P',
        CalibrateReflectiveSensor = 'R',
        SetProximityADCThreshold = 'S',
        CalibrateTransmissiveSensor = 'T',
        FirmwareVersion = 'V',
        Identity = 'v',
        RestoreFirmwareDefaults = 'Z'
    }

    public enum AfiniaArgCommand
    {
        SetPaperOutGlitchDelay = 'P',
        SetFanPWM = 'F',
        SetRDAC = 'R',
        SetTDAC = 'T',
        SetPDF = 'D',
        SetPTRL = 'I',
        SetPMC = 'M',
        SetPPO = 'O',
        SetPTABLE = 'S',
        SetPWPL = 'W',
        SetMetallic = 'K',
        SetUnwinder = 'U'
    }

    public delegate void AfiniaSerialDataReceivedEventHandler(string text);

    public class AfiniaSerial: IDisposable
    {
        SerialPort ser;
        System.Timers.Timer findPortsTimer;
        private string portName;
        public string PortName
        {
            get
            {
                return portName;
            }
            set
            {
                ser.Close();
                ser.PortName = value;
                portName = value;
            }
        }

        /// <summary>
        /// Function for handling all incoming data (e.g. a logger function)
        /// </summary>
        public AfiniaSerialDataReceivedEventHandler DataReceivedHandler { get; set; }
        /// <summary>
        /// Function for handling an expected response to a sent command
        /// </summary>
        public AfiniaSerialDataReceivedEventHandler ResponseHandler { get; set; }
        //public AfiniaSerialDataReceivedEventHandler RawResponseHandler { get; set; }

        public AfiniaSerial()
        {
            ser = new SerialPort();
            ser.BaudRate = 115200;
            ser.ReadBufferSize = 1500;
            ser.ReadTimeout = 500;
            ser.NewLine = "\r";
            ser.DataReceived += serialPortDataReceivedHandler;

            findPortsTimer = new System.Timers.Timer();
            findPortsTimer.Interval = 100;
            findPortsTimer.Elapsed += FindPortsTimer_Elapsed;
        }

        /// <summary>
        /// Open the serial connection.
        /// </summary>
        public void Open()
        {
            if (portName != null)
            {
                ser.Open();
            }
        }

        /// <summary>
        /// Close the serial connection.
        /// </summary>
        public void Close()
        {
            ser.Close();
        }

        /// <summary>
        /// Send a given string over the serial port.
        /// </summary>
        /// <param name="str">String to send</param>
        public void SendString(string str)
        {
            try
            {
                ser.DiscardInBuffer();
                ser.Write(str + "\x1B");
                Thread.Sleep(50);
            }
            catch (InvalidOperationException)
            { }
        }

        /// <summary>
        /// Send a command with no arguments across the serial port.
        /// </summary>
        /// <param name="cmd">Command to send</param>
        public void SendBasicCommand(AfiniaBasicCommand cmd)
        {
            SendString("\x1B" + (char)cmd);
        }

        /// <summary>
        /// Send a command with an argument across the serial port.
        /// </summary>
        /// <param name="cmd">Command to send</param>
        /// <param name="arg">Value to set</param>
        public void SendArgCommand(AfiniaArgCommand cmd, double arg)
        {
            SendString("\x1B" + "!" + arg + (char)cmd);
        }

        private AfiniaSerialDataReceivedEventHandler foundPortHandler;

        /// <summary>
        /// Sends the first port attached to an Afinia printer to the provided
        /// callback.
        /// </summary>
        /// <param name="handler">Handler for found serial port</param>
        public void FindAfiniaPorts(AfiniaSerialDataReceivedEventHandler handler, bool sendBasicCommand=true)
        {
            foundPortHandler = handler;

            foreach (string port in FindPortsByManufacturer("FTDI"))
            {
                while (ser.IsOpen)
                {
                    Thread.Sleep(100);
                }
                ser.PortName = port;

                try
                {
                    ser.Open();

                    if (sendBasicCommand)
                    {
                        ResponseHandler = CheckIdentity;
                        findPortsTimer.Start();
                        SendBasicCommand(AfiniaBasicCommand.Identity);
                    }
                    else { 
                        foundPortHandler(port); 
                    }
                    
                }
                catch (UnauthorizedAccessException) { }
            }
        }

        private void FindPortsTimer_Elapsed(object sender, EventArgs e)
        {
            findPortsTimer.Stop();
            ser.Close();
            foundPortHandler(null);
        }

        public string versionData = string.Empty;
        /// <summary>
        /// Check that the received string matches the expected reponse from an
        /// Afinia printer identity command.
        /// </summary>
        /// <param name="data">Data sent from printer</param>
        public void CheckIdentity(string data)
        {
            findPortsTimer.Stop();
            ser.Close();
            if (data.Trim().StartsWith("A F I N I A"))
            {
                versionData = data;
                foundPortHandler(ser.PortName);
            }
            else
            {
                foundPortHandler(null);
            }
        }

        /// <summary>
        /// Return management objects for all serial ports.
        /// </summary>
        /// <returns>Management objects for all serial port</returns>
        private static ManagementObject[] FindSerialPorts()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity");
                List<ManagementObject> objects = new List<ManagementObject>();

                foreach (ManagementObject obj in searcher.Get())
                {
                    objects.Add(obj);
                }

                return objects.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ManagementObject[] { };
            }
        }

        /// <summary>
        /// Find first serial port with the given manufacturer.
        /// </summary>
        /// <param name="manufacturer">Manufacturer to search for</param>
        /// <returns>Serial port name</returns>
        private static List<string> FindPortsByManufacturer(string manufacturer)
        {
            List<string> ports = new List<string>();

            foreach (ManagementObject obj in FindSerialPorts())
            {
                try
                {
                    if (obj["Manufacturer"]!=null && obj["Manufacturer"].ToString().ToLower().Equals(manufacturer.ToLower()))
                    {
                        string comName = ParseCOMName(obj);
                        if (comName != null)
                        {
                            ports.Add(comName);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return ports;
        }

        /// <summary>
        /// Get name of serial port contained in management object.
        /// </summary>
        /// <param name="obj">Management object for given serial port</param>
        /// <returns>Serial port name</returns>
        private static string ParseCOMName(ManagementObject obj)
        {
            string name = obj["Name"].ToString();
            int startIndex = name.LastIndexOf("(");
            int endIndex = name.LastIndexOf(")");

            if ((startIndex != -1) && (endIndex != -1))
            {
                name = name.Substring(startIndex + 1, endIndex - startIndex - 1);
                return name;
            }

            return null;
        }

        /// <summary>
        /// Read the received serial port data and pass it to the appropriate handlers.
        /// </summary>
        /// <param name="sender">Serial object sending the event</param>
        /// <param name="e">Event details</param>
        private void serialPortDataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Regex regex = new Regex("^[-|]ProximitySensor=[0-9]*.*Voltage=[0-9]*mV$");

            try
            {
                while (ser.BytesToRead > 0)
                {
                    try
                    {
                        string data = ser.ReadLine();
                        string trimmed = Regex.Replace(data, @"\s+", string.Empty);

                        if ((trimmed.Length > 0) && !regex.IsMatch(trimmed))
                        {
                            DataReceivedHandler?.Invoke(data);
                            ResponseHandler?.Invoke(data);
                            //RawResponseHandler?.Invoke(data);

                        }
                    }
                    catch (TimeoutException) { }
                    catch (IOException) { }
                }
            }
            catch (InvalidOperationException) { }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    findPortsTimer.Dispose();
                    ser.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AfiniaSerial() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
