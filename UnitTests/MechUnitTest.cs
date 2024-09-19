using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrinterCommandExecute;

namespace UnitTests
{
    [TestClass]
    public class MechUnitTest
    {
        [TestMethod]
        public void ProcessMech1TestMethod()
        {
            Command.ProcessMech1(@"C:\Users\Terry\Desktop\Microboards Enhanced L501 Ink Cost\Enhancement Project 2-14-2019\Cut_Feed files\mech1.xml");

        }

        [TestMethod]
        public void ProcessMech1aTestMethod()
        {
            Command.ProcessMech1a(@"C:\Users\Terry\Desktop\Microboards Enhanced L501 Ink Cost\Enhancement Project 2-14-2019\Cut_Feed files\mech1a.xml");
            Command.ProcessMech1a(@"C:\Users\Terry\Desktop\Microboards Enhanced L501 Ink Cost\Enhancement Project 2-14-2019\Cut_Feed files\mech1ar.xml");

        }

        [TestMethod]
        public void CutTestMethod()
        {
            Command.Cut("Afinia Label L501 Label Printer", @"C:\temp\", "mech1a.xml", @"C:\temp\");
        }

        [TestMethod]
        public void FeedTestMethod()
        {
            Command.Feed("Afinia Label L501 Label Printer", @"C:\temp\", "mech1a.xml", @"C:\temp\",2400);
        }

        [TestMethod]
        public void CutEditXMLTestMethod()
        {
            Command.EditCutMechXML(@"C:\Users\Terry\Desktop\Microboards Enhanced L501 Ink Cost\Enhancement Project 2-14-2019\Cut_Feed files\mech1a - Edit.xml",1);
 
        }
        [TestMethod]
        public void FeedEditXMLTestMethod()
        {
            Command.EditFeedMechXML(@"C:\Users\Terry\Desktop\Microboards Enhanced L501 Ink Cost\Enhancement Project 2-14-2019\Cut_Feed files\mech1a - Edit.xml", 9600);

        }
        [TestMethod]
        public void ProcessFeedEditXMLTestMethod()
        {
            var length = Command.ReadFeedLength(@"D:\temp\mech1.xml");
            double inches = (double)(length / 2400.0);
            var i = inches.ToString("#.000");
            double mm = (double)((length / 2400.0) / 25.4);
            var m = mm.ToString("#.000");
        }
        [TestMethod]
        public void PrintAlignmentTestMethod()
        {
            //Command.PrintAlignment();
        }
        [TestMethod]
        public void CheckPauseTestMethod()
        {
            PrinterCommandExecute.Command.GetMech1("Afinia Label L501 Label Printer", @"c:\temp\", "mech1.xml", @"c:\temp\");
        }

        [TestMethod]
        public void PauseTestMethod()
        {
            //bool state = PrinterCommandExecute.Command.CheckPauseState("Afinia Label L501 Label Printer", @"c:\temp\", "pause.xml", @"c:\temp\");
            PrinterCommandExecute.Command.EditPauseMechXML(@"c:\temp\pause.xml", 1);
            PrinterCommandExecute.Command.Pause("Afinia Label L501 Label Printer", @"c:\temp\", "mech1.xml", @"c:\temp\");
        }
        [TestMethod]
        public void UnPauseTestMethod()
        {
            //bool state = PrinterCommandExecute.Command.CheckPauseState("Afinia Label L501 Label Printer", @"c:\temp\", "pause.xml", @"c:\temp\");
            PrinterCommandExecute.Command.EditPauseMechXML(@"c:\temp\pause.xml", 0);
            PrinterCommandExecute.Command.UnPause("Afinia Label L501 Label Printer", @"c:\temp\", "mech1.xml", @"c:\temp\");
        }
        [TestMethod]
        public void ReadFeedLengthTestMethod()
        {
            PrinterCommandExecute.Command.PrintedLength("Afinia Label L501 Label Printer", @"c:\temp\", "mech1.xml", @"c:\temp\");

            uint length = PrinterCommandExecute.Command.ReadFeedLength(@"c:\temp\mech1.xml");

            double inches = (double)(length / 2400.0);
            var i = inches.ToString("#.000");
            double mm = (double)((length / 2400.0) * 25.4);
            var m = mm.ToString("#.000");
        }

        [TestMethod]
        public void ParseResponseTestMethod()
        {
            string location = PrinterCommandExecute.Command.GetLocation(@"c:\temp\OemsiMediapathMechSession_httpresponse.txt");
        }
    }
}
