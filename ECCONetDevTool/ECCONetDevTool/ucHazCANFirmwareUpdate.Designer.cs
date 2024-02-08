namespace ECCONetDevTool
{
    partial class ucHazCANFirmwareUpdate
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnScanHazCAN = new System.Windows.Forms.Button();
            this.gbxUpdateFirmware = new System.Windows.Forms.GroupBox();
            this.lblPassFail = new System.Windows.Forms.Label();
            this.bprFirmwareProgramming = new System.Windows.Forms.ProgressBar();
            this.btnFindFile = new System.Windows.Forms.Button();
            this.btnUpdateFirmware = new System.Windows.Forms.Button();
            this.tbxFilePath = new System.Windows.Forms.TextBox();
            this.openFirmwareFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.rtbBusScan = new System.Windows.Forms.RichTextBox();
            this.lblScanning = new System.Windows.Forms.Label();
            this.gbxUpdateFirmware.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnScanHazCAN
            // 
            this.btnScanHazCAN.Location = new System.Drawing.Point(609, 374);
            this.btnScanHazCAN.Name = "btnScanHazCAN";
            this.btnScanHazCAN.Size = new System.Drawing.Size(77, 23);
            this.btnScanHazCAN.TabIndex = 52;
            this.btnScanHazCAN.Text = "Scan";
            this.btnScanHazCAN.UseVisualStyleBackColor = true;
            this.btnScanHazCAN.Click += new System.EventHandler(this.btnScanHazCAN_Click);
            // 
            // gbxUpdateFirmware
            // 
            this.gbxUpdateFirmware.Controls.Add(this.lblPassFail);
            this.gbxUpdateFirmware.Controls.Add(this.bprFirmwareProgramming);
            this.gbxUpdateFirmware.Controls.Add(this.btnFindFile);
            this.gbxUpdateFirmware.Controls.Add(this.btnUpdateFirmware);
            this.gbxUpdateFirmware.Controls.Add(this.tbxFilePath);
            this.gbxUpdateFirmware.Location = new System.Drawing.Point(15, 425);
            this.gbxUpdateFirmware.Name = "gbxUpdateFirmware";
            this.gbxUpdateFirmware.Size = new System.Drawing.Size(534, 98);
            this.gbxUpdateFirmware.TabIndex = 84;
            this.gbxUpdateFirmware.TabStop = false;
            this.gbxUpdateFirmware.Text = "Update Firmware";
            // 
            // lblPassFail
            // 
            this.lblPassFail.AutoSize = true;
            this.lblPassFail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassFail.ForeColor = System.Drawing.Color.Green;
            this.lblPassFail.Location = new System.Drawing.Point(8, 71);
            this.lblPassFail.Name = "lblPassFail";
            this.lblPassFail.Size = new System.Drawing.Size(42, 13);
            this.lblPassFail.TabIndex = 81;
            this.lblPassFail.Text = "Passed";
            this.lblPassFail.Visible = false;
            // 
            // bprFirmwareProgramming
            // 
            this.bprFirmwareProgramming.Location = new System.Drawing.Point(11, 45);
            this.bprFirmwareProgramming.Name = "bprFirmwareProgramming";
            this.bprFirmwareProgramming.Size = new System.Drawing.Size(381, 23);
            this.bprFirmwareProgramming.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.bprFirmwareProgramming.TabIndex = 80;
            this.bprFirmwareProgramming.Visible = false;
            // 
            // btnFindFile
            // 
            this.btnFindFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnFindFile.Location = new System.Drawing.Point(458, 16);
            this.btnFindFile.Name = "btnFindFile";
            this.btnFindFile.Size = new System.Drawing.Size(65, 23);
            this.btnFindFile.TabIndex = 76;
            this.btnFindFile.Text = "File";
            this.btnFindFile.UseVisualStyleBackColor = true;
            this.btnFindFile.Click += new System.EventHandler(this.btnFindFile_Click);
            // 
            // btnUpdateFirmware
            // 
            this.btnUpdateFirmware.Location = new System.Drawing.Point(458, 45);
            this.btnUpdateFirmware.Name = "btnUpdateFirmware";
            this.btnUpdateFirmware.Size = new System.Drawing.Size(65, 23);
            this.btnUpdateFirmware.TabIndex = 65;
            this.btnUpdateFirmware.Text = "Update";
            this.btnUpdateFirmware.UseVisualStyleBackColor = true;
            this.btnUpdateFirmware.Click += new System.EventHandler(this.btnUpdateFirmware_Click);
            // 
            // tbxFilePath
            // 
            this.tbxFilePath.Location = new System.Drawing.Point(11, 17);
            this.tbxFilePath.Name = "tbxFilePath";
            this.tbxFilePath.Size = new System.Drawing.Size(441, 20);
            this.tbxFilePath.TabIndex = 64;
            this.tbxFilePath.TextChanged += new System.EventHandler(this.tbxFilePath_TextChanged);
            // 
            // openFirmwareFileDialog
            // 
            this.openFirmwareFileDialog.Filter = "Bin files|*.bin";
            // 
            // rtbBusScan
            // 
            this.rtbBusScan.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbBusScan.Location = new System.Drawing.Point(15, 17);
            this.rtbBusScan.Name = "rtbBusScan";
            this.rtbBusScan.Size = new System.Drawing.Size(671, 351);
            this.rtbBusScan.TabIndex = 86;
            this.rtbBusScan.Text = "";
            // 
            // lblScanning
            // 
            this.lblScanning.AutoSize = true;
            this.lblScanning.Location = new System.Drawing.Point(15, 374);
            this.lblScanning.Name = "lblScanning";
            this.lblScanning.Size = new System.Drawing.Size(52, 13);
            this.lblScanning.TabIndex = 87;
            this.lblScanning.Text = "Scanning";
            this.lblScanning.Visible = false;
            // 
            // ucHazCANFirmwareUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblScanning);
            this.Controls.Add(this.rtbBusScan);
            this.Controls.Add(this.btnScanHazCAN);
            this.Controls.Add(this.gbxUpdateFirmware);
            this.Name = "ucHazCANFirmwareUpdate";
            this.Size = new System.Drawing.Size(1086, 620);
            this.gbxUpdateFirmware.ResumeLayout(false);
            this.gbxUpdateFirmware.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnScanHazCAN;
        private System.Windows.Forms.GroupBox gbxUpdateFirmware;
        private System.Windows.Forms.Label lblPassFail;
        private System.Windows.Forms.ProgressBar bprFirmwareProgramming;
        private System.Windows.Forms.Button btnFindFile;
        private System.Windows.Forms.Button btnUpdateFirmware;
        private System.Windows.Forms.TextBox tbxFilePath;
        private System.Windows.Forms.OpenFileDialog openFirmwareFileDialog;
        private System.Windows.Forms.RichTextBox rtbBusScan;
        private System.Windows.Forms.Label lblScanning;
    }
}
