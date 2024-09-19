namespace InkCostCalculator
{
    partial class ConfigurationDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ColorCostLabel = new System.Windows.Forms.Label();
            this.ColorMlLabel = new System.Windows.Forms.Label();
            this.BlackCostLabel = new System.Windows.Forms.Label();
            this.BlackMlLabel = new System.Windows.Forms.Label();
            this.ColorCostTextBox = new System.Windows.Forms.TextBox();
            this.ColorMlTextBox = new System.Windows.Forms.TextBox();
            this.BlackCostTextBox = new System.Windows.Forms.TextBox();
            this.BlackMlTextBox = new System.Windows.Forms.TextBox();
            this.ColorCartridgeGroupBox = new System.Windows.Forms.GroupBox();
            this.BlackCartridgeGroupBox = new System.Windows.Forms.GroupBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.CancelConfigButton = new System.Windows.Forms.Button();
            this.ColorCartridgeGroupBox.SuspendLayout();
            this.BlackCartridgeGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ColorCostLabel
            // 
            this.ColorCostLabel.AutoSize = true;
            this.ColorCostLabel.Location = new System.Drawing.Point(3, 14);
            this.ColorCostLabel.Name = "ColorCostLabel";
            this.ColorCostLabel.Size = new System.Drawing.Size(28, 13);
            this.ColorCostLabel.TabIndex = 0;
            this.ColorCostLabel.Text = "Cost";
            // 
            // ColorMlLabel
            // 
            this.ColorMlLabel.AutoSize = true;
            this.ColorMlLabel.Location = new System.Drawing.Point(184, 41);
            this.ColorMlLabel.Name = "ColorMlLabel";
            this.ColorMlLabel.Size = new System.Drawing.Size(17, 13);
            this.ColorMlLabel.TabIndex = 1;
            this.ColorMlLabel.Text = "ml";
            this.ColorMlLabel.Visible = false;
            // 
            // BlackCostLabel
            // 
            this.BlackCostLabel.AutoSize = true;
            this.BlackCostLabel.Location = new System.Drawing.Point(6, 16);
            this.BlackCostLabel.Name = "BlackCostLabel";
            this.BlackCostLabel.Size = new System.Drawing.Size(28, 13);
            this.BlackCostLabel.TabIndex = 2;
            this.BlackCostLabel.Text = "Cost";
            // 
            // BlackMlLabel
            // 
            this.BlackMlLabel.AutoSize = true;
            this.BlackMlLabel.Location = new System.Drawing.Point(184, 81);
            this.BlackMlLabel.Name = "BlackMlLabel";
            this.BlackMlLabel.Size = new System.Drawing.Size(17, 13);
            this.BlackMlLabel.TabIndex = 3;
            this.BlackMlLabel.Text = "ml";
            this.BlackMlLabel.Visible = false;
            // 
            // ColorCostTextBox
            // 
            this.ColorCostTextBox.Location = new System.Drawing.Point(6, 31);
            this.ColorCostTextBox.Name = "ColorCostTextBox";
            this.ColorCostTextBox.Size = new System.Drawing.Size(100, 20);
            this.ColorCostTextBox.TabIndex = 4;
            // 
            // ColorMlTextBox
            // 
            this.ColorMlTextBox.Location = new System.Drawing.Point(171, 58);
            this.ColorMlTextBox.Name = "ColorMlTextBox";
            this.ColorMlTextBox.Size = new System.Drawing.Size(100, 20);
            this.ColorMlTextBox.TabIndex = 5;
            this.ColorMlTextBox.Visible = false;
            // 
            // BlackCostTextBox
            // 
            this.BlackCostTextBox.Location = new System.Drawing.Point(6, 33);
            this.BlackCostTextBox.Name = "BlackCostTextBox";
            this.BlackCostTextBox.Size = new System.Drawing.Size(100, 20);
            this.BlackCostTextBox.TabIndex = 6;
            // 
            // BlackMlTextBox
            // 
            this.BlackMlTextBox.Location = new System.Drawing.Point(171, 97);
            this.BlackMlTextBox.Name = "BlackMlTextBox";
            this.BlackMlTextBox.Size = new System.Drawing.Size(100, 20);
            this.BlackMlTextBox.TabIndex = 7;
            this.BlackMlTextBox.Visible = false;
            // 
            // ColorCartridgeGroupBox
            // 
            this.ColorCartridgeGroupBox.Controls.Add(this.ColorCostTextBox);
            this.ColorCartridgeGroupBox.Controls.Add(this.ColorCostLabel);
            this.ColorCartridgeGroupBox.Location = new System.Drawing.Point(49, 27);
            this.ColorCartridgeGroupBox.Name = "ColorCartridgeGroupBox";
            this.ColorCartridgeGroupBox.Size = new System.Drawing.Size(116, 63);
            this.ColorCartridgeGroupBox.TabIndex = 8;
            this.ColorCartridgeGroupBox.TabStop = false;
            this.ColorCartridgeGroupBox.Text = "Color Cartridge";
            // 
            // BlackCartridgeGroupBox
            // 
            this.BlackCartridgeGroupBox.Controls.Add(this.BlackCostLabel);
            this.BlackCartridgeGroupBox.Controls.Add(this.BlackCostTextBox);
            this.BlackCartridgeGroupBox.Location = new System.Drawing.Point(49, 97);
            this.BlackCartridgeGroupBox.Name = "BlackCartridgeGroupBox";
            this.BlackCartridgeGroupBox.Size = new System.Drawing.Size(116, 63);
            this.BlackCartridgeGroupBox.TabIndex = 9;
            this.BlackCartridgeGroupBox.TabStop = false;
            this.BlackCartridgeGroupBox.Text = "Black Cartridge";
            // 
            // SaveButton
            // 
            this.SaveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SaveButton.Location = new System.Drawing.Point(35, 166);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 10;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // CancelConfigButton
            // 
            this.CancelConfigButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelConfigButton.Location = new System.Drawing.Point(116, 166);
            this.CancelConfigButton.Name = "CancelConfigButton";
            this.CancelConfigButton.Size = new System.Drawing.Size(75, 23);
            this.CancelConfigButton.TabIndex = 11;
            this.CancelConfigButton.Text = "Cancel";
            this.CancelConfigButton.UseVisualStyleBackColor = true;
            this.CancelConfigButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ConfigurationDialog
            // 
            this.AcceptButton = this.SaveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelConfigButton;
            this.ClientSize = new System.Drawing.Size(225, 201);
            this.Controls.Add(this.CancelConfigButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.BlackMlTextBox);
            this.Controls.Add(this.ColorMlTextBox);
            this.Controls.Add(this.BlackMlLabel);
            this.Controls.Add(this.BlackCartridgeGroupBox);
            this.Controls.Add(this.ColorMlLabel);
            this.Controls.Add(this.ColorCartridgeGroupBox);
            this.Name = "ConfigurationDialog";
            this.Text = "Configuration";
            this.ColorCartridgeGroupBox.ResumeLayout(false);
            this.ColorCartridgeGroupBox.PerformLayout();
            this.BlackCartridgeGroupBox.ResumeLayout(false);
            this.BlackCartridgeGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ColorCostLabel;
        private System.Windows.Forms.Label ColorMlLabel;
        private System.Windows.Forms.Label BlackCostLabel;
        private System.Windows.Forms.Label BlackMlLabel;
        private System.Windows.Forms.TextBox ColorCostTextBox;
        private System.Windows.Forms.TextBox ColorMlTextBox;
        private System.Windows.Forms.TextBox BlackCostTextBox;
        private System.Windows.Forms.TextBox BlackMlTextBox;
        private System.Windows.Forms.GroupBox ColorCartridgeGroupBox;
        private System.Windows.Forms.GroupBox BlackCartridgeGroupBox;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button CancelConfigButton;
    }
}