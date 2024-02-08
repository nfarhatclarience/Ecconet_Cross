namespace ECCONetDevTool
{
    partial class ucECCONetFirmwareUpdate
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
            this.gbxUpdateFirmware = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.bprFirmwareProgramming = new System.Windows.Forms.ProgressBar();
            this.btnFindFile = new System.Windows.Forms.Button();
            this.btnUpdateFirmware = new System.Windows.Forms.Button();
            this.tbxFilePath = new System.Windows.Forms.TextBox();
            this.openFirmwareFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.cbxEnableFlashErase = new System.Windows.Forms.CheckBox();
            this.btnEraseAll = new System.Windows.Forms.Button();
            this.btnEraseApp = new System.Windows.Forms.Button();
            this.gbxFlashErase = new System.Windows.Forms.GroupBox();
            this.cbbOnlineDevices = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gbxUpdateFirmware.SuspendLayout();
            this.gbxFlashErase.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxUpdateFirmware
            // 
            this.gbxUpdateFirmware.Controls.Add(this.lblStatus);
            this.gbxUpdateFirmware.Controls.Add(this.bprFirmwareProgramming);
            this.gbxUpdateFirmware.Controls.Add(this.btnFindFile);
            this.gbxUpdateFirmware.Controls.Add(this.btnUpdateFirmware);
            this.gbxUpdateFirmware.Controls.Add(this.tbxFilePath);
            this.gbxUpdateFirmware.Location = new System.Drawing.Point(9, 65);
            this.gbxUpdateFirmware.Name = "gbxUpdateFirmware";
            this.gbxUpdateFirmware.Size = new System.Drawing.Size(534, 94);
            this.gbxUpdateFirmware.TabIndex = 89;
            this.gbxUpdateFirmware.TabStop = false;
            this.gbxUpdateFirmware.Text = "Update Firmware";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(10, 71);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 93;
            this.lblStatus.Text = "Status";
            this.lblStatus.Visible = false;
            // 
            // bprFirmwareProgramming
            // 
            this.bprFirmwareProgramming.Location = new System.Drawing.Point(11, 45);
            this.bprFirmwareProgramming.Name = "bprFirmwareProgramming";
            this.bprFirmwareProgramming.Size = new System.Drawing.Size(441, 23);
            this.bprFirmwareProgramming.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.bprFirmwareProgramming.TabIndex = 80;
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
            // cbxEnableFlashErase
            // 
            this.cbxEnableFlashErase.AutoSize = true;
            this.cbxEnableFlashErase.Location = new System.Drawing.Point(13, 74);
            this.cbxEnableFlashErase.Name = "cbxEnableFlashErase";
            this.cbxEnableFlashErase.Size = new System.Drawing.Size(178, 17);
            this.cbxEnableFlashErase.TabIndex = 86;
            this.cbxEnableFlashErase.Text = "Enable (When running app only)";
            this.cbxEnableFlashErase.UseVisualStyleBackColor = true;
            this.cbxEnableFlashErase.CheckedChanged += new System.EventHandler(this.cbxEnableFlashErase_CheckedChanged);
            // 
            // btnEraseAll
            // 
            this.btnEraseAll.Enabled = false;
            this.btnEraseAll.Location = new System.Drawing.Point(111, 31);
            this.btnEraseAll.Name = "btnEraseAll";
            this.btnEraseAll.Size = new System.Drawing.Size(75, 23);
            this.btnEraseAll.TabIndex = 23;
            this.btnEraseAll.Text = "Erase All";
            this.btnEraseAll.UseVisualStyleBackColor = true;
            this.btnEraseAll.Click += new System.EventHandler(this.btnEraseAll_Click);
            // 
            // btnEraseApp
            // 
            this.btnEraseApp.Enabled = false;
            this.btnEraseApp.Location = new System.Drawing.Point(13, 31);
            this.btnEraseApp.Name = "btnEraseApp";
            this.btnEraseApp.Size = new System.Drawing.Size(75, 23);
            this.btnEraseApp.TabIndex = 22;
            this.btnEraseApp.Text = "Erase App";
            this.btnEraseApp.UseVisualStyleBackColor = true;
            this.btnEraseApp.Click += new System.EventHandler(this.btnEraseApp_Click);
            // 
            // gbxFlashErase
            // 
            this.gbxFlashErase.Controls.Add(this.cbxEnableFlashErase);
            this.gbxFlashErase.Controls.Add(this.btnEraseAll);
            this.gbxFlashErase.Controls.Add(this.btnEraseApp);
            this.gbxFlashErase.Location = new System.Drawing.Point(9, 184);
            this.gbxFlashErase.Name = "gbxFlashErase";
            this.gbxFlashErase.Size = new System.Drawing.Size(215, 99);
            this.gbxFlashErase.TabIndex = 86;
            this.gbxFlashErase.TabStop = false;
            this.gbxFlashErase.Text = "Flash Erase";
            // 
            // cbbOnlineDevices
            // 
            this.cbbOnlineDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbOnlineDevices.FormattingEnabled = true;
            this.cbbOnlineDevices.Location = new System.Drawing.Point(54, 23);
            this.cbbOnlineDevices.Name = "cbbOnlineDevices";
            this.cbbOnlineDevices.Size = new System.Drawing.Size(258, 21);
            this.cbbOnlineDevices.TabIndex = 91;
            this.cbbOnlineDevices.SelectedIndexChanged += new System.EventHandler(this.cbbOnlineDevices_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 92;
            this.label1.Text = "Device";
            // 
            // ucECCONetFirmwareUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbbOnlineDevices);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gbxUpdateFirmware);
            this.Controls.Add(this.gbxFlashErase);
            this.Name = "ucECCONetFirmwareUpdate";
            this.Size = new System.Drawing.Size(1080, 620);
            this.gbxUpdateFirmware.ResumeLayout(false);
            this.gbxUpdateFirmware.PerformLayout();
            this.gbxFlashErase.ResumeLayout(false);
            this.gbxFlashErase.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox gbxUpdateFirmware;
        private System.Windows.Forms.ProgressBar bprFirmwareProgramming;
        private System.Windows.Forms.Button btnFindFile;
        private System.Windows.Forms.Button btnUpdateFirmware;
        private System.Windows.Forms.TextBox tbxFilePath;
        private System.Windows.Forms.OpenFileDialog openFirmwareFileDialog;
        private System.Windows.Forms.CheckBox cbxEnableFlashErase;
        private System.Windows.Forms.Button btnEraseAll;
        private System.Windows.Forms.Button btnEraseApp;
        private System.Windows.Forms.GroupBox gbxFlashErase;
        private System.Windows.Forms.ComboBox cbbOnlineDevices;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStatus;
    }
}
