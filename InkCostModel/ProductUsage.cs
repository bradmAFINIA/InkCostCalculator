using System;
using System.Collections.Generic;
using System.Text;

namespace InkCostModel
{
    public class ProductUsage
    {
        public uint TotalImpressions { get; set; }
        public uint DuplexSheets { get; set; }
        public uint JamEvents { get; set; }
        public uint MispickEvents { get; set; }
        public List<ConsumableSubUnit> Consumables { get; set; }
    }
}
