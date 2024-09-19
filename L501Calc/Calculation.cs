using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility;

namespace L501Calculation
{
    public class Calculation
    {
        //public ProductUsage ProcessProductUsageDyn(string filepathProductUsageDyn)
        //{
        //    try
        //    {
        //        ProductUsageDyn usage;
        //        var serializer = new XmlSerializer(typeof(ProductUsageDyn));
        //        using (var reader = XmlReader.Create(filepathProductUsageDyn))
        //        {
        //            usage = (ProductUsageDyn)serializer.Deserialize(reader);
        //        }
        //        ProductUsage productUsage = new ProductUsage();
        //        productUsage.TotalImpressions = usage.PrintApplicationSubunit.TotalImpressions.Value;
        //        productUsage.DuplexSheets = usage.PrinterSubunit.DuplexSheets.Value;
        //        productUsage.JamEvents = usage.PrinterSubunit.JamEvents.Value;
        //        productUsage.MispickEvents = usage.PrinterSubunit.MispickEvents;
        //        productUsage.Consumables = new List<ConsumableSubUnit>();

        //        foreach (var consumableSubunit in usage.ConsumableSubunit)
        //        {
        //            ConsumableSubUnit consumable = new ConsumableSubUnit();
        //            consumable.MarkerColor = consumableSubunit.MarkerColor;
        //            consumable.Agent = new List<MarkingAgent>();
        //            foreach (var markingAgentCount in consumableSubunit.UsageByMarkingAgentCount)
        //            {
        //                MarkingAgent agent = new MarkingAgent();
        //                //if ((markingAgentCount.MarkingAgentCountType == "HPDropsCount") || (markingAgentCount.MarkingAgentCountType == "LDWDropsCount"))
        //                if (markingAgentCount.MarkingAgentCountType == "HPDropsCount")
        //                {
        //                    agent.PEID = markingAgentCount.MarkingAgentCount.PEID;
        //                    agent.Count = markingAgentCount.MarkingAgentCount.Value;
        //                    agent.Type = markingAgentCount.MarkingAgentCountType;
        //                }
        //                if (agent.Type != null)
        //                {
        //                    if (!consumable.Agent.Any(a => a.PEID == agent.PEID))
        //                        consumable.Agent.Add(agent);
        //                }
        //            }
        //            productUsage.Consumables.Add(consumable);
        //        }
        //        return productUsage;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error($"{ex.Message}. {ex.InnerException}");
        //    }
        //    return null;
        //}

        //public Cost CalculateCost(List<InkLevel> inkLevels, string printjob)
        //{
        //    Cost cost = new Cost();
        //    try
        //    {
        //        var inkLevelsGroup = new List<InkLevel>();
        //        foreach (var item in inkLevels)
        //        {
        //            if (item.PrintJob == printjob)
        //            {
        //                inkLevelsGroup.Add(item);
        //            }
        //        }
        //        var inklevelsList = (from t in inkLevelsGroup
        //                             orderby t.timestamp descending
        //                             select t).Take(8).ToList();
        //        var listTop = (from t in inklevelsList
        //                       orderby t.timestamp descending, t.PEID
        //                       select t).Take(4).ToList();

        //        var listBottom = (from t in inklevelsList
        //                          orderby t.timestamp ascending, t.PEID
        //                          select t).Take(4).ToList();
        //        List<InkLevel> SortedTopList = listTop.OrderBy(o => o.PEID).ToList();
        //        List<InkLevel> SortedBottomList = listBottom.OrderBy(o => o.PEID).ToList();

        //        PopulateListBox(SortedTopList, SortedBottomList);

        //        double colorCartridgeCost = 0;// Properties.Settings.Default.ColorCartridgeCost;
        //        double.TryParse(ColorInkCartridgeCostTextBox.Text, out colorCartridgeCost);

        //        double colorMl = Properties.Settings.Default.Color_ml;
        //        //double.TryParse(ColorCartridgeMlTextBox.Text, out colorMl);

        //        double CostPerMlColor = colorCartridgeCost / colorMl;

        //        double blackCartridgeCost = 0;// Properties.Settings.Default.BlackCartridgeCost;
        //        double.TryParse(BlackInkCartridgeCostTextBox.Text, out blackCartridgeCost);

        //        double blackMl = Properties.Settings.Default.Black_ml;
        //        //double.TryParse(BlackCartridgeMlTextBox.Text, out blackMl);

        //        double CostPerMlBlack = blackCartridgeCost / blackMl;

        //        double cyanPlUse = (SortedTopList.ElementAt(1).Count -
        //            SortedBottomList.ElementAt(1).Count) *
        //            Properties.Settings.Default.ColorConst1 * Properties.Settings.Default.ColorConst2;
        //        cost.TotalCyanMlUsed = (cyanPlUse * Properties.Settings.Default.ColorConst3);
        //        double magentaPlUse = (SortedTopList.ElementAt(2).Count -
        //            SortedBottomList.ElementAt(2).Count) *
        //            Properties.Settings.Default.ColorConst1 * Properties.Settings.Default.ColorConst2;
        //        cost.TotalMagentaMlUsed = (magentaPlUse * Properties.Settings.Default.ColorConst3);
        //        double yellowPlUse = (SortedTopList.ElementAt(3).Count -
        //            SortedBottomList.ElementAt(3).Count) *
        //            Properties.Settings.Default.ColorConst1 * Properties.Settings.Default.ColorConst2;
        //        cost.TotalYellowMlUsed = (yellowPlUse * Properties.Settings.Default.ColorConst3);

        //        cost.TotalColorMlUsed = cost.TotalCyanMlUsed + cost.TotalMagentaMlUsed + cost.TotalYellowMlUsed;

        //        cost.TotalCostOfCyan = cost.TotalCyanMlUsed * CostPerMlColor;
        //        cost.TotalCostOfMagenta = cost.TotalMagentaMlUsed * CostPerMlColor;
        //        cost.TotalCostOfYellow = cost.TotalYellowMlUsed * CostPerMlColor;
        //        cost.TotalCostOfColor = cost.TotalColorMlUsed * CostPerMlColor;

        //        double blackPlUse = (SortedTopList.ElementAt(0).Count - SortedBottomList.ElementAt(0).Count) * Properties.Settings.Default.BlackConst1 * Properties.Settings.Default.BlackConst2;
        //        cost.TotalBlackMlUsed = (blackPlUse * Properties.Settings.Default.BlackConst3);
        //        cost.TotalCostOfBlack = cost.TotalBlackMlUsed * CostPerMlBlack;

        //        cost.TotalBlackLabels = SortedTopList.ElementAt(0).TotalImpressions - SortedBottomList.ElementAt(0).TotalImpressions;
        //        if (cost.TotalBlackLabels < 1)
        //            cost.TotalBlackLabels = 1;
        //        cost.LabelCostOfBlack = cost.TotalCostOfBlack / cost.TotalBlackLabels;
        //        cost.TotalColorLabels = SortedTopList.ElementAt(1).TotalImpressions - SortedBottomList.ElementAt(1).TotalImpressions;
        //        if (cost.TotalColorLabels < 1)
        //            cost.TotalColorLabels = 1;
        //        cost.LabelCostOfColor = cost.TotalCostOfColor / cost.TotalColorLabels;

        //        cost.LabelCostOfCyan = cost.TotalCostOfCyan / cost.TotalColorLabels;
        //        cost.LabelCostOfMagenta = cost.TotalCostOfMagenta / cost.TotalColorLabels;
        //        cost.LabelCostOfYellow = cost.TotalCostOfYellow / cost.TotalColorLabels;

        //        cost.PageCyanMlUsed = cost.TotalCyanMlUsed / cost.TotalColorLabels;
        //        cost.PageMagentaMlUsed = cost.TotalMagentaMlUsed / cost.TotalColorLabels;
        //        cost.PageYellowMlUsed = cost.TotalYellowMlUsed / cost.TotalColorLabels;
        //        cost.PageColorMlUsed = cost.TotalColorMlUsed / cost.TotalColorLabels;
        //        cost.PageBlackMlUsed = cost.TotalBlackMlUsed / cost.TotalBlackLabels;

        //        cost.CyanPagesPerCartridge = colorMl / cost.PageCyanMlUsed;
        //        cost.MagentaPagesPerCartridge = colorMl / cost.PageMagentaMlUsed;
        //        cost.YellowPagesPerCartridge = colorMl / cost.PageYellowMlUsed;
        //        cost.BlackPagesPerCartridge = blackMl / cost.PageBlackMlUsed;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error($"{ex.Message}. {ex.InnerException}");
        //    }
        //    cost.Printer = selectedPrinter;
        //    cost.Printer = printjob;
        //    return cost;
        //}
    }
}
