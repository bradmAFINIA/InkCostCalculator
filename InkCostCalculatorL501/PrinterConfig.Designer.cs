namespace InkCostCalculator
{
    partial class PrinterConfig
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
            this.SaveButton = new System.Windows.Forms.Button();
            this.CancelDialogButton = new System.Windows.Forms.Button();
            this.PrintersComboBox = new System.Windows.Forms.ComboBox();
            this.PrintersLabel = new System.Windows.Forms.Label();
            this.PathOutputProductUseageDynLabel = new System.Windows.Forms.Label();
            this.PathOutputConsumableConfigDynLabel = new System.Windows.Forms.Label();
            this.PathHttpResponseLabel = new System.Windows.Forms.Label();
            this.PathSWFWClientLabel = new System.Windows.Forms.Label();
            this.PathOutputProductUseageDynTextBox = new System.Windows.Forms.TextBox();
            this.PathOutputConsumableConfigDynTextBox = new System.Windows.Forms.TextBox();
            this.PathHttpResponseTextBox = new System.Windows.Forms.TextBox();
            this.PathSWFWClientTextBox = new System.Windows.Forms.TextBox();
            this.PathToOutputTextBox = new System.Windows.Forms.TextBox();
            this.PathToOutputLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SaveButton.Location = new System.Drawing.Point(246, 130);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 0;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // CancelDialogButton
            // 
            this.CancelDialogButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelDialogButton.Location = new System.Drawing.Point(328, 130);
            this.CancelDialogButton.Name = "CancelDialogButton";
            this.CancelDialogButton.Size = new System.Drawing.Size(75, 23);
            this.CancelDialogButton.TabIndex = 1;
            this.CancelDialogButton.Text = "Cancel";
            this.CancelDialogButton.UseVisualStyleBackColor = true;
            this.CancelDialogButton.Click += new System.EventHandler(this.CancelDialogButton_Click);
            // 
            // PrintersComboBox
            // 
            this.PrintersComboBox.FormattingEnabled = true;
            this.PrintersComboBox.Location = new System.Drawing.Point(21, 30);
            this.PrintersComboBox.Name = "PrintersComboBox";
            this.PrintersComboBox.Size = new System.Drawing.Size(654, 21);
            this.PrintersComboBox.TabIndex = 2;
            // 
            // PrintersLabel
            // 
            this.PrintersLabel.AutoSize = true;
            this.PrintersLabel.Location = new System.Drawing.Point(21, 11);
            this.PrintersLabel.Name = "PrintersLabel";
            this.PrintersLabel.Size = new System.Drawing.Size(42, 13);
            this.PrintersLabel.TabIndex = 3;
            this.PrintersLabel.Text = "Printers";
            // 
            // PathOutputProductUseageDynLabel
            // 
            this.PathOutputProductUseageDynLabel.AutoSize = true;
            this.PathOutputProductUseageDynLabel.Location = new System.Drawing.Point(690, 57);
            this.PathOutputProductUseageDynLabel.Name = "PathOutputProductUseageDynLabel";
            this.PathOutputProductUseageDynLabel.Size = new System.Drawing.Size(182, 13);
            this.PathOutputProductUseageDynLabel.TabIndex = 4;
            this.PathOutputProductUseageDynLabel.Text = "Path To Output Product Useage Dyn";
            this.PathOutputProductUseageDynLabel.Visible = false;
            // 
            // PathOutputConsumableConfigDynLabel
            // 
            this.PathOutputConsumableConfigDynLabel.AutoSize = true;
            this.PathOutputConsumableConfigDynLabel.Location = new System.Drawing.Point(693, 96);
            this.PathOutputConsumableConfigDynLabel.Name = "PathOutputConsumableConfigDynLabel";
            this.PathOutputConsumableConfigDynLabel.Size = new System.Drawing.Size(192, 13);
            this.PathOutputConsumableConfigDynLabel.TabIndex = 5;
            this.PathOutputConsumableConfigDynLabel.Text = "Path to Output Consumable Config Dyn";
            this.PathOutputConsumableConfigDynLabel.Visible = false;
            // 
            // PathHttpResponseLabel
            // 
            this.PathHttpResponseLabel.AutoSize = true;
            this.PathHttpResponseLabel.Location = new System.Drawing.Point(690, 135);
            this.PathHttpResponseLabel.Name = "PathHttpResponseLabel";
            this.PathHttpResponseLabel.Size = new System.Drawing.Size(115, 13);
            this.PathHttpResponseLabel.TabIndex = 6;
            this.PathHttpResponseLabel.Text = "Path to Http Response";
            this.PathHttpResponseLabel.Visible = false;
            // 
            // PathSWFWClientLabel
            // 
            this.PathSWFWClientLabel.AutoSize = true;
            this.PathSWFWClientLabel.Location = new System.Drawing.Point(690, 11);
            this.PathSWFWClientLabel.Name = "PathSWFWClientLabel";
            this.PathSWFWClientLabel.Size = new System.Drawing.Size(125, 13);
            this.PathSWFWClientLabel.TabIndex = 7;
            this.PathSWFWClientLabel.Text = "Path to SWFWClient exe";
            this.PathSWFWClientLabel.Visible = false;
            // 
            // PathOutputProductUseageDynTextBox
            // 
            this.PathOutputProductUseageDynTextBox.Location = new System.Drawing.Point(693, 73);
            this.PathOutputProductUseageDynTextBox.Name = "PathOutputProductUseageDynTextBox";
            this.PathOutputProductUseageDynTextBox.Size = new System.Drawing.Size(654, 20);
            this.PathOutputProductUseageDynTextBox.TabIndex = 8;
            this.PathOutputProductUseageDynTextBox.Visible = false;
            // 
            // PathOutputConsumableConfigDynTextBox
            // 
            this.PathOutputConsumableConfigDynTextBox.Location = new System.Drawing.Point(693, 112);
            this.PathOutputConsumableConfigDynTextBox.Name = "PathOutputConsumableConfigDynTextBox";
            this.PathOutputConsumableConfigDynTextBox.Size = new System.Drawing.Size(654, 20);
            this.PathOutputConsumableConfigDynTextBox.TabIndex = 9;
            this.PathOutputConsumableConfigDynTextBox.Visible = false;
            // 
            // PathHttpResponseTextBox
            // 
            this.PathHttpResponseTextBox.Location = new System.Drawing.Point(693, 151);
            this.PathHttpResponseTextBox.Name = "PathHttpResponseTextBox";
            this.PathHttpResponseTextBox.Size = new System.Drawing.Size(654, 20);
            this.PathHttpResponseTextBox.TabIndex = 10;
            this.PathHttpResponseTextBox.Visible = false;
            // 
            // PathSWFWClientTextBox
            // 
            this.PathSWFWClientTextBox.Location = new System.Drawing.Point(690, 27);
            this.PathSWFWClientTextBox.Name = "PathSWFWClientTextBox";
            this.PathSWFWClientTextBox.Size = new System.Drawing.Size(654, 20);
            this.PathSWFWClientTextBox.TabIndex = 11;
            this.PathSWFWClientTextBox.Visible = false;
            // 
            // PathToOutputTextBox
            // 
            this.PathToOutputTextBox.Location = new System.Drawing.Point(21, 73);
            this.PathToOutputTextBox.Name = "PathToOutputTextBox";
            this.PathToOutputTextBox.Size = new System.Drawing.Size(654, 20);
            this.PathToOutputTextBox.TabIndex = 13;
            // 
            // PathToOutputLabel
            // 
            this.PathToOutputLabel.AutoSize = true;
            this.PathToOutputLabel.Location = new System.Drawing.Point(21, 57);
            this.PathToOutputLabel.Name = "PathToOutputLabel";
            this.PathToOutputLabel.Size = new System.Drawing.Size(64, 13);
            this.PathToOutputLabel.TabIndex = 12;
            this.PathToOutputLabel.Text = "Output Path";
            // 
            // PrinterConfig
            // 
            this.AcceptButton = this.SaveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelDialogButton;
            this.ClientSize = new System.Drawing.Size(708, 190);
            this.Controls.Add(this.PathToOutputTextBox);
            this.Controls.Add(this.PathToOutputLabel);
            this.Controls.Add(this.PathSWFWClientTextBox);
            this.Controls.Add(this.PathHttpResponseTextBox);
            this.Controls.Add(this.PathOutputConsumableConfigDynTextBox);
            this.Controls.Add(this.PathOutputProductUseageDynTextBox);
            this.Controls.Add(this.PathSWFWClientLabel);
            this.Controls.Add(this.PathHttpResponseLabel);
            this.Controls.Add(this.PathOutputConsumableConfigDynLabel);
            this.Controls.Add(this.PathOutputProductUseageDynLabel);
            this.Controls.Add(this.PrintersLabel);
            this.Controls.Add(this.PrintersComboBox);
            this.Controls.Add(this.CancelDialogButton);
            this.Controls.Add(this.SaveButton);
            this.Name = "PrinterConfig";
            this.Text = "Printer Configuration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button CancelDialogButton;
        private System.Windows.Forms.ComboBox PrintersComboBox;
        private System.Windows.Forms.Label PrintersLabel;
        private System.Windows.Forms.Label PathOutputProductUseageDynLabel;
        private System.Windows.Forms.Label PathOutputConsumableConfigDynLabel;
        private System.Windows.Forms.Label PathHttpResponseLabel;
        private System.Windows.Forms.Label PathSWFWClientLabel;
        private System.Windows.Forms.TextBox PathOutputProductUseageDynTextBox;
        private System.Windows.Forms.TextBox PathOutputConsumableConfigDynTextBox;
        private System.Windows.Forms.TextBox PathHttpResponseTextBox;
        private System.Windows.Forms.TextBox PathSWFWClientTextBox;
        private System.Windows.Forms.TextBox PathToOutputTextBox;
        private System.Windows.Forms.Label PathToOutputLabel;
    }
}