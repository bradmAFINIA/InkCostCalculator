using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace InkCostCalculatorL501EnhancedWPF
{
    public class InkCostViewModel : ObservableObject
    {
        NavigationService ns;
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public InkCostViewModel()
        {
            _canExecute = true;
        }
        public void SetNS(NavigationService _ns)
        {
            ns = _ns;
        }
        private ICommand _manualCommand;
        public ICommand ManualCommand
        {
            get
            {
                return _manualCommand ?? (_manualCommand = new CommandHandler(() => MyAction(), _canExecute));
            }
        }
        private bool _canExecute;
        public void MyAction()
        {
            string some = string.Empty;
            try
            {
                logger.Debug("Navigating Manual Operations");
                ns.Navigate(new Uri("ManualOperationsPage.xaml", UriKind.Relative));
            }
            catch (Exception ex)
            {
                logger.Debug("NavigateManualOperations");
                logger.Error($"{ex.Message}. {ex.InnerException}");
            }
        }

        public ObservableCollection<string> RawList = new ObservableCollection<string>();

        public ObservableCollection<string> JobList = new ObservableCollection<string>();

        private string colorInkCartridgeCost = "0.00";
        public string ColorInkCartridgeCost
        {
            get { return colorInkCartridgeCost; }
            set { colorInkCartridgeCost = value;
                RaisePropertyChangedEvent("ColorInkCartridgeCost");
            }
        }

        private string blackInkCartridgeCost = "0.00";
        public string BlackInkCartridgeCost
        {
            get { return blackInkCartridgeCost; }
            set
            {
                blackInkCartridgeCost = value;
                RaisePropertyChangedEvent("BlackInkCartridgeCost");
            }
        }

        private string cyanCost = "0.00";
        public string CyanCost
        {
            get { return cyanCost; }
            set
            {
                cyanCost = value;
                RaisePropertyChangedEvent("CyanCost");
            }
        }

        private string magentaCost = "0.00";
        public string MagentaCost
        {
            get { return magentaCost; }
            set
            {
                magentaCost = value;
                RaisePropertyChangedEvent("MagentaCost");
            }
        }

        private string yellowCost = "0.00";
        public string YellowCost
        {
            get { return yellowCost; }
            set
            {
                yellowCost = value;
                RaisePropertyChangedEvent("YellowCost");
            }
        }

        private string blackCost = "0.00";
        public string BlackCost
        {
            get { return blackCost; }
            set
            {
                blackCost = value;
                RaisePropertyChangedEvent("BlackCost");
            }
        }

        private string labelCyanCost = "0.00";
        public string LabelCyanCost
        {
            get { return labelCyanCost; }
            set
            {
                labelCyanCost = value;
                RaisePropertyChangedEvent("LabelCyanCost");
            }
        }

        private string labelMagentaCost = "0.00";
        public string LabelMagentaCost
        {
            get { return labelMagentaCost; }
            set
            {
                labelMagentaCost = value;
                RaisePropertyChangedEvent("LabelMagentaCost");
            }
        }

        private string labelYellowCost = "0.00";
        public string LabelYellowCost
        {
            get { return labelYellowCost; }
            set
            {
                labelYellowCost = value;
                RaisePropertyChangedEvent("LabelYellowCost");
            }
        }

        private string labelBlackCost = "0.00";
        public string LabelBlackCost
        {
            get { return labelBlackCost; }
            set
            {
                labelBlackCost = value;
                RaisePropertyChangedEvent("LabelBlackCost");
            }
        }

        private string cyanMl = "0.00";
        public string CyanMl
        {
            get { return cyanMl; }
            set
            {
                cyanMl = value;
                RaisePropertyChangedEvent("CyanMl");
            }
        }

        private string magentaMl = "0.00";
        public string MagentaMl
        {
            get { return magentaMl; }
            set
            {
                magentaMl = value;
                RaisePropertyChangedEvent("MagentaMl");
            }
        }

        private string yellowMl = "0.00";
        public string YellowMl
        {
            get { return yellowMl; }
            set
            {
                yellowMl = value;
                RaisePropertyChangedEvent("YellowMl");
            }
        }

        private string blackMl = "0.00";
        public string BlackMl
        {
            get { return blackMl; }
            set
            {
                blackMl = value;
                RaisePropertyChangedEvent("BlackMl");
            }
        }

        private string cyanPagesPerCartridge = "0.00";
        public string CyanPagesPerCartridge
        {
            get { return cyanPagesPerCartridge; }
            set
            {
                cyanPagesPerCartridge = value;
                RaisePropertyChangedEvent("CyanPagesPerCartridge");
            }
        }

        private string magentaPagesPerCartridge = "0.00";
        public string MagentaPagesPerCartridge
        {
            get { return magentaPagesPerCartridge; }
            set
            {
                magentaPagesPerCartridge = value;
                RaisePropertyChangedEvent("MagentaPagesPerCartridge");
            }
        }

        private string yellowPagesPerCartridge = "0.00";
        public string YellowPagesPerCartridge
        {
            get { return yellowPagesPerCartridge; }
            set
            {
                yellowPagesPerCartridge = value;
                RaisePropertyChangedEvent("YellowPagesPerCartridge");
            }
        }

        private string blackPagesPerCartridge = "0.00";
        public string BlackPagesPerCartridge
        {
            get { return blackPagesPerCartridge; }
            set
            {
                blackPagesPerCartridge = value;
                RaisePropertyChangedEvent("BlackPagesPerCartridge");
            }
        }

        private string totalLabels = "0";
        public string TotalLabels
        {
            get { return totalLabels; }
            set
            {
                totalLabels = value;
                RaisePropertyChangedEvent("TotalLabels");
            }
        }

        private string totalCost = "0.00";
        public string TotalCost
        {
            get { return totalCost; }
            set
            {
                totalCost = value;
                RaisePropertyChangedEvent("TotalCost");
            }
        }

        private string costPerLabel = "0.00";
        public string CostPerLabel
        {
            get { return costPerLabel; }
            set
            {
                costPerLabel = value;
                RaisePropertyChangedEvent("CostPerLabel");
            }
        }

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

        private string showHideAdvancedButton;
        public string ShowHideAdvancedButton
        {
            get { return showHideAdvancedButton; }
            set
            {
                showHideAdvancedButton = value;
                RaisePropertyChangedEvent("ShowHideAdvancedButton");
            }
        }

        private Visibility jobsListBoxLabelVis;
        public Visibility JobsListBoxLabelVis
        {
            get { return jobsListBoxLabelVis; }
            set
            {
                jobsListBoxLabelVis = value;
                RaisePropertyChangedEvent("JobsListBoxLabelVis");
            }
        }

        private Visibility jobsListBoxVis;
        public Visibility JobsListBoxVis
        {
            get { return jobsListBoxVis; }
            set
            {
                jobsListBoxVis = value;
                RaisePropertyChangedEvent("JobsListBoxVis");
            }
        }

        private Visibility rawLabelVis;
        public Visibility RawLabelVis
        {
            get { return rawLabelVis; }
            set
            {
                rawLabelVis = value;
                RaisePropertyChangedEvent("RawLabelVis");
            }
        }

        private Visibility rawListBoxVis;
        public Visibility RawListBoxVis
        {
            get { return rawListBoxVis; }
            set
            {
                rawListBoxVis = value;
                RaisePropertyChangedEvent("RawListBoxVis");
            }
        }

        private int width;
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                RaisePropertyChangedEvent("Width");
            }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                RaisePropertyChangedEvent("Height");
            }
        }

        
    }
    public class ItemLine
    {
        private string line = string.Empty;
        public string Line { get; set; }
    }
    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
