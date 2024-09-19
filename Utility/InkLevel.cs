using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
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
}
