using System.Windows;

namespace InkCostCalculatorL501EnhancedWPF
{
    /// <summary>
    /// Interaction logic for CalibrateUnwinderDialog.xaml
    /// </summary>
    public partial class CalibrateUnwinderDialog : Window
    {
        AfiniaSerial ser;

        public CalibrateUnwinderDialog(AfiniaSerial ser)
        {
            InitializeComponent();
            this.ser = ser;
        }

        private void CalibrateButton_Click(object sender, RoutedEventArgs e)
        {
            ser.SendBasicCommand(AfiniaBasicCommand.SetProximityADCThreshold);
            this.DialogResult = true;
        }
    }
}
