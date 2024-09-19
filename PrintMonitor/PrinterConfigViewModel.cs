using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PrintMonitor
{
    public class PrinterConfigViewModel : ObservableObject
    {
        public PrinterConfigViewModel(string name, List<string> printers)
        {
            _name = name;
            _printerEntry = name;
            List<PrinterEntry> list = new List<PrinterEntry>()
                                             {
                                                 new PrinterEntry("Please Select Printer")
                                             };
            foreach(string printer in printers)
            {
                list.Add(new PrinterEntry(printer));
            }
            _printerEntries = new System.Windows.Data.CollectionView(list);
        }
        private readonly System.Windows.Data.CollectionView _printerEntries;
        public System.Windows.Data.CollectionView PrinterEntries
        {
            get { return _printerEntries; }
        }
        private string _printerEntry;
        public string PrinterEntry
        {
            get { return _printerEntry; }
            set
            {
                if (_printerEntry == value) return;
                _printerEntry = value;
                RaisePropertyChangedEvent("PrinterEntry");
            }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                RaisePropertyChangedEvent("Name");
            }
        }

        private string pathToOutput;
        public string PathToOutput
        {
            get { return pathToOutput; }
            set
            {
                pathToOutput = value;
                RaisePropertyChangedEvent("PathToOutput");
            }
        }
    }

    public class PrinterEntry
    {
        public string Name { get; set; }
        public PrinterEntry(string name)
        {
            Name = name;
        }
    }
}
