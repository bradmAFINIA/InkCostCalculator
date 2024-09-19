using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InkCostCalculator
{
    public partial class ConfigurationDialog : Form
    {
        public string ColorCost = string.Empty;
        public string ColorMl = string.Empty;
        public string BlackCost = string.Empty;
        public string BlackMl = string.Empty;
        public ConfigurationDialog(string colorCost, string colorMl, string blackCost, string blackMl)
        {
            InitializeComponent();
            ColorCostTextBox.Text = ColorCost = colorCost;
            ColorMlTextBox.Text = ColorMl = colorMl;
            BlackCostTextBox.Text = BlackCost = blackCost;
            BlackMlTextBox.Text = BlackMl = blackMl;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            ColorCost = ColorCostTextBox.Text;
            ColorMl = ColorMlTextBox.Text;
            BlackCost = BlackCostTextBox.Text;
            BlackMl = BlackMlTextBox.Text;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {

        }
    }
}
