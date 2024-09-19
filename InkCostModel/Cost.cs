using System;
using System.Collections.Generic;
using System.Text;

namespace InkCostModel
{
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
        public double CyanPagesPerCartridge { get; set; }
        public double MagentaPagesPerCartridge { get; set; }
        public double YellowPagesPerCartridge { get; set; }
        public double BlackPagesPerCartridge { get; set; }
    }
}
