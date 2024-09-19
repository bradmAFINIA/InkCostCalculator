using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkCostCalculatorL501EnhancedWPF
{
    public class CostCalculatorViewModel : ObservableObject
    {
        public ObservableCollection<CostData> Data = new ObservableCollection<CostData>();

        private string selectedPrinterName;
        public string SelectedPrinterName
        {
            get { return selectedPrinterName; }
            set
            {
                selectedPrinterName = value;
                RaisePropertyChangedEvent("SelectedPrinterName");
            }
        }
        private string colorCost;
        public string ColorCost
        {
            get { return colorCost; }
            set
            {
                colorCost = value;
                RaisePropertyChangedEvent("ColorCost");
            }
        }
        private string blackCost;
        public string BlackCost
        {
            get { return blackCost; }
            set
            {
                blackCost = value;
                RaisePropertyChangedEvent("BlackCost");
            }
        }
        private string totalImpressions;
        public string TotalImpressions
        {
            get { return totalImpressions; }
            set
            {
                totalImpressions = value;
                RaisePropertyChangedEvent("TotalImpressions");
            }
        }
    }
    public class CostData
    {
        private string cyanMl = "0.00";
        public string CyanMl
        {
            get { return cyanMl; }
            set
            {
                cyanMl = value;
            }
        }

        private string magentaMl = "0.00";
        public string MagentaMl
        {
            get { return magentaMl; }
            set
            {
                magentaMl = value;
            }
        }

        private string yellowMl = "0.00";
        public string YellowMl
        {
            get { return yellowMl; }
            set
            {
                yellowMl = value;
            }
        }

        private string blackMl = "0.00";
        public string BlackMl
        {
            get { return blackMl; }
            set
            {
                blackMl = value;
            }
        }

        private string totalLabels = "0";
        public string TotalLabels
        {
            get { return totalLabels; }
            set
            {
                totalLabels = value;
            }
        }

        private string costPerLabel = "0.00";
        public string CostPerLabel
        {
            get { return costPerLabel; }
            set
            {
                costPerLabel = value;
            }
        }

        private string timestamp = "1-1-1900 00:00:00";
        public string Timestamp
        {
            get { return timestamp; }
            set
            {
                timestamp = value;
            }
        }
    }
}
