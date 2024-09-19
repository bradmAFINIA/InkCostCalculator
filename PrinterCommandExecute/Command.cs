using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PrinterCommandExecute
{
    public static class Command
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public static void RunEXE(string printer, string commandType, string command, string outputFilePath, string swFilePath)
        {
            try
            {
                logger.Debug($"RunExe executable and path: {swFilePath}SWFWClient.exe");

                ProcessStartInfo startInfo = new ProcessStartInfo($"{swFilePath}SWFWClient.exe");
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                switch (commandType)
                {
                    case "ProductUsageDyn":
                        {
                            startInfo.Arguments = $"\"{printer}\" get \"{command}\" \"{outputFilePath}httpresponse.txt\" \"{outputFilePath}ProductUsageDyn.xml\"";
                        }
                        break;
                    case "ConsumableConfigDyn":
                        {
                            startInfo.Arguments = $"\"{printer}\" get \"{command}\" \"{outputFilePath}httpresponse.txt\" \"{outputFilePath}ConsumableConfigDyn.xml\"";
                        }
                        break;
                }
                logger.Debug($"RunExe arguments: {startInfo.Arguments}");

                Process process = Process.Start(startInfo);
                while (!process.HasExited)
                {
                    System.Threading.Thread.Sleep(100);
                }
                logger.Debug($"RunExe Complete");
            }
            catch (Exception ex)
            {
                logger.Debug("RunExe");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }
        private static void RawRunEXE(string printer, string commandType, string command, string outputFilePath, string outputFileName, string swFilePath)
        {
            try
            {
                logger.Debug($"RawRunExe executable and path: {swFilePath}SWFWClient.exe");

                ProcessStartInfo startInfo = new ProcessStartInfo($"{swFilePath}SWFWClient.exe");
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                switch (commandType)
                {
                    case "get":
                        {
                            startInfo.Arguments = $"\"{printer}\" get \"{command}\" \"{outputFilePath}gethttpresponse.txt\" \"{outputFilePath}{outputFileName}\"";
                        }
                        break;
                    case "get_verify":
                        {
                            startInfo.Arguments = $"\"{printer}\" get \"{command}\" \"{outputFilePath}getverifyhttpresponse.txt\" \"{outputFilePath}{outputFileName}\"";
                        }
                        break;
                    case "put":
                        {
                            startInfo.Arguments = $"\"{printer}\" put \"{command}\" \"{outputFilePath}puthttpresponse.txt\" \"{outputFilePath}{outputFileName}\"";
                        }
                        break;
                    case "delete":
                        {
                            //"Afinia Label L501 Label Printer" "delete" "http://L50101130.03f02c09.hpLedmUSB/OemsiMediapath/Mech/Session/1" "c:\temp\httpresponse.txt"
                            //"\"Afinia Label L501 Label Printer\" delete \"http://L50101130.03f02c09.hpLedmUSB/OemsiMediapath/Mech/Session/1\" \"C:\\temp\\deletehttpresponse.txt\" \"C:\\temp\\mech1a.xml\""
                            //"http://L50101130.03f02c09.hpLedmUSB/OemsiMediapath/Mech/Session/1"
                            //string arg = $"\"{printer}\" delete \"{command}\" \"{outputFilePath}deletehttpresponse.txt\"";
                            //startInfo.Arguments = arg;
                            startInfo.Arguments = $"\"{printer}\" delete \"{command}\" \"{outputFilePath}deletehttpresponse.txt\"";
                        }
                        break;
                    case "post":
                        {
                            startInfo.Arguments = $"\"{printer}\" post \"{command}\" \"{outputFilePath}posthttpresponse.txt\" \"{outputFilePath}{outputFileName}\"";
                        }
                        break;

                }
                logger.Debug($"RawRunExe arguments: {startInfo.Arguments}");
                Process process = Process.Start(startInfo);
                while (!process.HasExited)
                {
                    System.Threading.Thread.Sleep(100);
                }
                logger.Debug($"RawRunExe Complete");
            }
            catch (Exception ex)
            {
                logger.Debug("RawRunExe");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }
        public static void Cut(string printer, string outputFilePath, string outputFileName, string swFilePath)
        {
            try
            {
                if (File.Exists($"{outputFilePath}posthttpresponse.txt"))
                {
                    File.Delete($"{outputFilePath}posthttpresponse.txt");
                }
                if (File.Exists($"{outputFilePath}gethttpresponse.txt"))
                {
                    File.Delete($"{outputFilePath}gethttpresponse.txt");
                }
                if (File.Exists($"{outputFilePath}puthttpresponse.txt"))
                {
                    File.Delete($"{outputFilePath}puthttpresponse.txt");
                }
                if (File.Exists($"{outputFilePath}getverifyhttpresponse.txt"))
                {
                    File.Delete($"{outputFilePath}getverifyhttpresponse.txt");
                }
                if (File.Exists($"{outputFilePath}deletehttpresponse.txt"))
                {
                    File.Delete($"{outputFilePath}deletehttpresponse.txt");
                }
                if (File.Exists($"{outputFilePath}{outputFileName}"))
                {
                    File.Delete($"{outputFilePath}{outputFileName}");
                }
                using (File.Create(outputFilePath + "empty.xml")) ;
                RawRunEXE(printer, "post", "/OemsiMediapath/Mech/Session", outputFilePath, "empty.xml", swFilePath);
                while (!File.Exists($"{outputFilePath}posthttpresponse.txt"))
                {
                    Thread.Sleep(200);
                }
                string location = GetLocation($"{outputFilePath}posthttpresponse.txt");
                if (!string.IsNullOrEmpty(location))
                {
                    RawRunEXE(printer, "get", "/OemsiMediapath/Mech/Session/1", outputFilePath, outputFileName, swFilePath);
                    Thread.Sleep(200);
                    while (!File.Exists($"{outputFilePath}gethttpresponse.txt"))
                    {
                        Thread.Sleep(200);
                    }
                    if (GetOk($"{outputFilePath}gethttpresponse.txt"))
                    {
                        //Edit xml
                        EditCutMechXML($"{outputFilePath}{outputFileName}", 1);

                        RawRunEXE(printer, "put", "/OemsiMediapath/Mech/Session/1", outputFilePath, outputFileName, swFilePath);
                        Thread.Sleep(200);
                        while (!File.Exists($"{outputFilePath}puthttpresponse.txt"))
                        {
                            Thread.Sleep(200);
                        }
                        if (GetOk($"{outputFilePath}puthttpresponse.txt"))
                        {
                            RawRunEXE(printer, "get_verify", "/OemsiMediapath/Mech/Session/1", outputFilePath, outputFileName, swFilePath);//get verify
                            Thread.Sleep(200);
                            while (!File.Exists($"{outputFilePath}getverifyhttpresponse.txt"))
                            {
                                Thread.Sleep(200);
                            }
                        }
                    }
                    RawRunEXE(printer, "delete", location, outputFilePath, outputFileName, swFilePath);
                }
                Thread.Sleep(200);
                GetMech1(printer, outputFilePath, "mech1.xml", swFilePath);

                if (ReadPauseState(outputFilePath + "mech1.xml"))
                {
                    UnPause(printer, outputFilePath, outputFileName, swFilePath);
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Cut");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }
        public static void Feed(string printer, string outputFilePath, string outputFileName, string swFilePath, uint feedLength)
        {
            try
            {
                if (File.Exists($"{outputFilePath}posthttpresponse.txt"))
                {
                    File.Delete($"{outputFilePath}posthttpresponse.txt");
                }
                if (File.Exists($"{outputFilePath}gethttpresponse.txt"))
                {
                    File.Delete($"{outputFilePath}gethttpresponse.txt");
                }
                if (File.Exists($"{outputFilePath}puthttpresponse.txt"))
                {
                    File.Delete($"{outputFilePath}puthttpresponse.txt");
                }
                if (File.Exists($"{outputFilePath}getverifyhttpresponse.txt"))
                {
                    File.Delete($"{outputFilePath}getverifyhttpresponse.txt");
                }
                if (File.Exists($"{outputFilePath}deletehttpresponse.txt"))
                {
                    File.Delete($"{outputFilePath}deletehttpresponse.txt");
                }
                if (File.Exists($"{outputFilePath}{outputFileName}"))
                {
                    File.Delete($"{outputFilePath}{outputFileName}");
                }
                using (File.Create(outputFilePath + "empty.xml")) ;
                RawRunEXE(printer, "post", "/OemsiMediapath/Mech/Session", outputFilePath, "empty.xml", swFilePath);
                while (!File.Exists($"{outputFilePath}posthttpresponse.txt"))
                {
                    Thread.Sleep(200);
                }
                string location = GetLocation($"{outputFilePath}posthttpresponse.txt");
                if (!string.IsNullOrEmpty(location))
                {
                    RawRunEXE(printer, "get", "/OemsiMediapath/Mech/Session/1", outputFilePath, outputFileName, swFilePath);
                    Thread.Sleep(200);
                    while (!File.Exists($"{outputFilePath}gethttpresponse.txt"))
                    {
                        Thread.Sleep(200);
                    }
                    if (GetOk($"{outputFilePath}gethttpresponse.txt"))
                    {
                        //Edit xml
                        EditFeedMechXML($"{outputFilePath}{outputFileName}", feedLength);

                        RawRunEXE(printer, "put", "/OemsiMediapath/Mech/Session/1", outputFilePath, outputFileName, swFilePath);
                        Thread.Sleep(200);
                        while (!File.Exists($"{outputFilePath}puthttpresponse.txt"))
                        {
                            Thread.Sleep(200);
                        }
                        if (GetOk($"{outputFilePath}puthttpresponse.txt"))
                        {
                            RawRunEXE(printer, "get_verify", "/OemsiMediapath/Mech/Session/1", outputFilePath, outputFileName, swFilePath);//get verify
                            Thread.Sleep(200);
                            while (!File.Exists($"{outputFilePath}getverifyhttpresponse.txt"))
                            {
                                Thread.Sleep(200);
                            }
                        }
                    }
                    RawRunEXE(printer, "delete", location, outputFilePath, outputFileName, swFilePath);
                }
                Thread.Sleep(200);
                GetMech1(printer, outputFilePath, "mech1.xml", swFilePath);
                if (GetOk($"{outputFilePath}deletehttpresponse.txt"))
                {
                    if (ReadPauseState(outputFilePath + "mech1.xml"))
                    {
                        UnPause(printer, outputFilePath, outputFileName, swFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Feed");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }
        public static string GetLocation(string fileName)
        {
            int counter = 0;
            string line;
            string location = string.Empty;
            // Read the file and display it line by line.  
            try
            {
                System.IO.StreamReader file =
                    new System.IO.StreamReader(fileName);
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains("Location:"))
                    {
                        //"Location: http://L50101130.03f02c09.hpLedmUSB/OemsiMediapath/Mech/Session/1"
                        location = line.Replace("Location: ", "");
                        location = location.TrimStart();
                        location = location.TrimEnd();
                        break;
                    }
                    counter++;
                }

                file.Close();
            }
            catch (Exception ex)
            {
                logger.Debug("GetLocation");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return location;
        }
        public static bool GetOk(string fileName)
        {
            int counter = 0;
            string line;
            bool result = false;
            // Read the file and display it line by line.  
            try
            {
                System.IO.StreamReader file =
                    new System.IO.StreamReader(fileName);
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains("OK"))
                    {
                        result = true;
                        break;
                    }
                    counter++;
                }

                file.Close();
            }
            catch (Exception ex)
            {
                logger.Debug("GetOk");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return result;
        }
        public static bool VerifyCut(string fileName)
        {
            bool result = false;
            try
            {
                Mech mech1a = new Mech();
                mech1a = ProcessMech1a(fileName);
                if ((mech1a.MechOutput.MechOutput0 == 1) && (mech1a.MechOutput.MechOutput1 == 15))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("VerifyCut");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return result;
        }
        public static bool VerifyFeed(string fileName)
        {
            bool result = false;
            try
            {
                Mech mech1a = new Mech();
                mech1a = ProcessMech1a(fileName);
                if ((mech1a.MechOutput.MechOutput0 == 1) && (mech1a.MechOutput.MechOutput1 == 16))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("VeriyFeed");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return result;
        }
        //In the HP document, it talks about issuing a 2nd GET, to verify the results.If the flow is completed, there will be a new section in the XML, MechOutput {0 – 4}.
        //Regarding the 2nd GET, to verify if the flow has completed, I ran a test, moving the stock a longer amount and then issues the 2nd GET.
        //The XML response did not show the MechOutput values.  So we can use this 2nd GET to confirm that the flow completed, before issuing another flow request.This may help with someone just clicking the button over and over again.

        //Issues PUT command for pause and unpause.
        //•	Pause bat file(OemsiMediapathDyn_put_mech1_pause_OEM_fieldsonly.bat) calls mech1_pause_OEM_fieldsonly.xml

        //In that XML, OemRw0 is set to 1:
        //<ompdyn:OemRw0>1</ompdyn:OemRw0>

        //This will cause the printer to pause in between labels, within a job.This check is done in the firmware, AFTER each page is printed.If the printer is paused before the job starts, it will print the 1st label and then pause.
        //•	The unpause bat file (OemsiMediapathDyn_put_mech1_unpause_OEM_fieldsonly.bat) calls mech1_unpause_OEM_fieldsonly.xml

        //In that XML OemRw0 is set to 0:
        //<ompdyn:OemRw0>0</ompdyn:OemRw0>

        //Once this command is issued, printing will resume.

        //The button should show Pause when not active and then change to Resume when it is paused.Also change color, green for Pause, red for Resume, making it obvious to the user that the printer is paused.

        //Put Mech 1 Pause
        //cd c:\L501\
        //SWFWClient.exe "Afinia Label L501 Label Printer" "put" "/OemsiMediapath/OemsiMediapathDyn.xml" "c:\temp\OemsiMediapathMechSession1_httpresponse.txt" "c:\temp\mech1_pause_OEM_fieldsonly.xml"
        //Pause

        //Put Pause
        //cd c:\L501\
        //SWFWClient.exe "Afinia Label L501 Label Printer" "put" "/OemsiMediapath/OemsiMediapathDyn.xml" "c:\temp\OemsiMediapathMechSession1_httpresponse.txt" "c:\temp\mech1_pause.xml"
        //Pause

        //Get Bat
        //cd c:\L501\
        //SWFWClient.exe "Afinia Label L501 Label Printer" "get" "/OemsiMediapath/OemsiMediapathDyn.xml" "c:\temp\OemsiMediapathMechSession1_httpresponse.txt" "c:\temp\mech1.xml"
        //Pause

        public static void Pause(string printer, string outputFilePath, string outputFileName, string swFilePath)
        {
            try
            {
                GetMech1(printer, outputFilePath, outputFileName, swFilePath);
                PrinterCommandExecute.Command.EditPauseMechXML(outputFilePath + outputFileName, 1);
                RawRunEXE(printer, "put", "/OemsiMediapath/OemsiMediapathDyn.xml", outputFilePath, outputFileName, swFilePath);
            }
            catch (Exception ex)
            {
                logger.Debug("Pause");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            // /OemsiMediapath/OemsiMediapathDyn.xml pause fields only
        }
        public static void UnPause(string printer, string outputFilePath, string outputFileName, string swFilePath)
        {
            try
            {
                GetMech1(printer, outputFilePath, outputFileName, swFilePath);
                PrinterCommandExecute.Command.EditPauseMechXML(outputFilePath + outputFileName, 0);
                RawRunEXE(printer, "put", "/OemsiMediapath/OemsiMediapathDyn.xml", outputFilePath, outputFileName, swFilePath);
            }
            catch (Exception ex)
            {
                logger.Debug("UnPause");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            // /OemsiMediapath/OemsiMediapathDyn.xml unpause fields only
        }

        public static void GetMech1(string printer, string outputFilePath, string outputFileName, string swFilePath)
        {
            try
            {
                RawRunEXE(printer, "get", "/OemsiMediapath/OemsiMediapathDyn.xml", outputFilePath, outputFileName, swFilePath);
            }
            catch (Exception ex)
            {
                logger.Debug("GetMech1");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            // return ReadPauseState(outputFilePath + outputFileName);
        }
        public static bool ReadPauseState(string filepath)
        {
            bool state = false;
            try
            {
                OemsiMediapathDyn mech = new OemsiMediapathDyn();
                var serializer = new XmlSerializer(typeof(OemsiMediapathDyn));

                using (var reader = XmlReader.Create(filepath))
                {
                    mech = (OemsiMediapathDyn)serializer.Deserialize(reader);
                }
                if (mech.OemsiMediapathSettings.OemRw0 == 1)
                {
                    state = true;
                }
                else
                {
                    state = false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("ReadPauseState");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return state;
        }
        public static void PrintedLength(string printer, string outputFilePath, string outputFileName, string swFilePath)
        {
            try
            {
                RawRunEXE(printer, "get", "/OemsiMediapath/OemsiMediapathDyn.xml", outputFilePath, outputFileName, swFilePath);
            }
            catch (Exception ex)
            {
                logger.Debug("PrintedLength");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        public static void EditCutMechXML(string source, uint mech0value)
        {
            try
            {
                Mech mech1a = new Mech();
                mech1a = ProcessMech1a(source);
                mech1a.MechInput.SequenceId = 0;
                mech1a.MechInput.MechInput0 = mech0value;

                SaveMech1a(source, mech1a);
            }
            catch (Exception ex)
            {
                logger.Debug("EditCutMechXML");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        public static void EditFeedMechXML(string source, uint mech0value)
        {
            try
            {
                Mech mech1a = new Mech();
                mech1a = ProcessMech1a(source);

                mech1a.MechInput.SequenceId = 1;
                mech1a.MechInput.MechInput0 = mech0value;

                SaveMech1a(source, mech1a);
            }
            catch (Exception ex)
            {
                logger.Debug("EditFeedMechXML");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        public static uint ReadFeedLength(string filepath)
        {
            OemsiMediapathDyn mech = new OemsiMediapathDyn();
            try
            {
                var serializer = new XmlSerializer(typeof(OemsiMediapathDyn));

                using (var reader = XmlReader.Create(filepath))
                {
                    mech = (OemsiMediapathDyn)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                logger.Debug("ReadFeedLength");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return mech.OemsiMediapathSettings.OemNvm5;
        }

        public static void EditPauseMechXML(string source, uint mech0value)
        {
            try
            {
                OemsiMediapathDyn mech1 = new OemsiMediapathDyn();
                mech1 = ProcessMech1(source);

                mech1.OemsiMediapathSettings.OemRw0 = mech0value;

                SaveMech1(source, mech1);
            }
            catch (Exception ex)
            {
                logger.Debug("EditPauseMechXML");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        public static OemsiMediapathDyn ProcessMech1(string filepath)
        {
            OemsiMediapathDyn mech = new OemsiMediapathDyn();
            try
            {
                var serializer = new XmlSerializer(typeof(OemsiMediapathDyn));

                using (var reader = XmlReader.Create(filepath))
                {
                    mech = (OemsiMediapathDyn)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                logger.Debug("ProcessMech1");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return mech;
        }
        public static void SaveMech1(string filepath, OemsiMediapathDyn mech)
        {
            try
            {
                var writer = new XmlSerializer(typeof(OemsiMediapathDyn));
                using (System.IO.FileStream file = System.IO.File.Create(filepath))
                {
                    writer.Serialize(file, mech);
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SaveMech1");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        public static Mech ProcessMech1a(string filepath)
        {
            Mech mech = new Mech();
            try
            {
                var serializer = new XmlSerializer(typeof(Mech));

                using (var reader = XmlReader.Create(filepath))
                {
                    mech = (Mech)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                logger.Debug("ProcessMech1a");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
            return mech;
        }
        public static void SaveMech1a(string filepath, Mech mech)
        {
            try
            {
                var writer = new XmlSerializer(typeof(Mech));
                using (System.IO.FileStream file = System.IO.File.Create(filepath))
                {
                    writer.Serialize(file, mech);
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SaveMech1a");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        //public static void PrintAlignment()
        //{
        //    PrintDocument pd = new PrintDocument();
        //    pd.PrintPage += PrintPage;
        //    pd.Print();
        //}

        //private static void PrintPage(object o, PrintPageEventArgs e)
        //{
        //    System.Drawing.Image img = System.Drawing.Image.FromFile(Properties.Settings.Default.AlignmentImage);
        //    System.Drawing.Point loc = new System.Drawing.Point(100, 100);
        //    e.Graphics.DrawImage(img, loc);
        //}

        //private static void Page_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    System.Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
        //    config.AppSettings.Settings.Remove("FeedLengthInches");
        //    config.AppSettings.Settings.Add("FeedLengthInches", inchValue.ToString());
        //    config.Save(ConfigurationSaveMode.Modified);
        //}
    }
}
