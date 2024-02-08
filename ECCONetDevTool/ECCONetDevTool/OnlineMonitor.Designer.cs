namespace ECCONetDevTool
{
    partial class OnlineMonitor
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
            this.groupBoxInterface = new System.Windows.Forms.GroupBox();
            this.labelStatusUsbCan = new System.Windows.Forms.Label();
            this.labelStatusPcanUsb = new System.Windows.Forms.Label();
            this.radBtnUsbCan = new System.Windows.Forms.RadioButton();
            this.radBtnPcanUsb = new System.Windows.Forms.RadioButton();
            this.groupBoxOnlineDevices = new System.Windows.Forms.GroupBox();
            this.cbxRetainOfflineDevices = new System.Windows.Forms.CheckBox();
            this.tbECCONetOnlineDevice = new System.Windows.Forms.RichTextBox();
            this.cbbOnlineDevices = new System.Windows.Forms.ComboBox();
            this.btnClearOnlineDevicesList = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cbxForcePowerState = new System.Windows.Forms.CheckBox();
            this.groupBoxSystemPowerState = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pbxPowerState = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pbxRun = new System.Windows.Forms.PictureBox();
            this.pbxAcc = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbbTransmitDelay = new System.Windows.Forms.ComboBox();
            this.groupBoxInterface.SuspendLayout();
            this.groupBoxOnlineDevices.SuspendLayout();
            this.groupBoxSystemPowerState.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxPowerState)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxRun)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxAcc)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxInterface
            // 
            this.groupBoxInterface.Controls.Add(this.labelStatusUsbCan);
            this.groupBoxInterface.Controls.Add(this.labelStatusPcanUsb);
            this.groupBoxInterface.Controls.Add(this.radBtnUsbCan);
            this.groupBoxInterface.Controls.Add(this.radBtnPcanUsb);
            this.groupBoxInterface.Location = new System.Drawing.Point(17, 18);
            this.groupBoxInterface.Name = "groupBoxInterface";
            this.groupBoxInterface.Size = new System.Drawing.Size(151, 117);
            this.groupBoxInterface.TabIndex = 20;
            this.groupBoxInterface.TabStop = false;
            this.groupBoxInterface.Text = "Selected Interface";
            // 
            // labelStatusUsbCan
            // 
            this.labelStatusUsbCan.AutoSize = true;
            this.labelStatusUsbCan.Location = new System.Drawing.Point(22, 44);
            this.labelStatusUsbCan.Name = "labelStatusUsbCan";
            this.labelStatusUsbCan.Size = new System.Drawing.Size(115, 13);
            this.labelStatusUsbCan.TabIndex = 19;
            this.labelStatusUsbCan.Text = "Status: Not Connected";
            // 
            // labelStatusPcanUsb
            // 
            this.labelStatusPcanUsb.AutoSize = true;
            this.labelStatusPcanUsb.Location = new System.Drawing.Point(20, 90);
            this.labelStatusPcanUsb.Name = "labelStatusPcanUsb";
            this.labelStatusPcanUsb.Size = new System.Drawing.Size(115, 13);
            this.labelStatusPcanUsb.TabIndex = 18;
            this.labelStatusPcanUsb.Text = "Status: Not Connected";
            // 
            // radBtnUsbCan
            // 
            this.radBtnUsbCan.AutoSize = true;
            this.radBtnUsbCan.Checked = true;
            this.radBtnUsbCan.Location = new System.Drawing.Point(8, 24);
            this.radBtnUsbCan.Name = "radBtnUsbCan";
            this.radBtnUsbCan.Size = new System.Drawing.Size(109, 17);
            this.radBtnUsbCan.TabIndex = 17;
            this.radBtnUsbCan.TabStop = true;
            this.radBtnUsbCan.Text = "Code 3 USB-CAN";
            this.radBtnUsbCan.UseVisualStyleBackColor = true;
            this.radBtnUsbCan.CheckedChanged += new System.EventHandler(this.radBtnUsbCan_CheckedChanged);
            // 
            // radBtnPcanUsb
            // 
            this.radBtnPcanUsb.AutoSize = true;
            this.radBtnPcanUsb.Location = new System.Drawing.Point(6, 70);
            this.radBtnPcanUsb.Name = "radBtnPcanUsb";
            this.radBtnPcanUsb.Size = new System.Drawing.Size(79, 17);
            this.radBtnPcanUsb.TabIndex = 16;
            this.radBtnPcanUsb.Text = "PCAN-USB";
            this.radBtnPcanUsb.UseVisualStyleBackColor = true;
            this.radBtnPcanUsb.CheckedChanged += new System.EventHandler(this.radBtnPcanUsb_CheckedChanged);
            // 
            // groupBoxOnlineDevices
            // 
            this.groupBoxOnlineDevices.Controls.Add(this.cbxRetainOfflineDevices);
            this.groupBoxOnlineDevices.Controls.Add(this.tbECCONetOnlineDevice);
            this.groupBoxOnlineDevices.Controls.Add(this.cbbOnlineDevices);
            this.groupBoxOnlineDevices.Controls.Add(this.btnClearOnlineDevicesList);
            this.groupBoxOnlineDevices.Controls.Add(this.label3);
            this.groupBoxOnlineDevices.Location = new System.Drawing.Point(17, 153);
            this.groupBoxOnlineDevices.Name = "groupBoxOnlineDevices";
            this.groupBoxOnlineDevices.Size = new System.Drawing.Size(358, 407);
            this.groupBoxOnlineDevices.TabIndex = 21;
            this.groupBoxOnlineDevices.TabStop = false;
            this.groupBoxOnlineDevices.Text = "Online Devices";
            // 
            // cbxRetainOfflineDevices
            // 
            this.cbxRetainOfflineDevices.AutoSize = true;
            this.cbxRetainOfflineDevices.Location = new System.Drawing.Point(20, 384);
            this.cbxRetainOfflineDevices.Name = "cbxRetainOfflineDevices";
            this.cbxRetainOfflineDevices.Size = new System.Drawing.Size(132, 17);
            this.cbxRetainOfflineDevices.TabIndex = 24;
            this.cbxRetainOfflineDevices.Text = "Retain Offline Devices";
            this.cbxRetainOfflineDevices.UseVisualStyleBackColor = true;
            this.cbxRetainOfflineDevices.CheckedChanged += new System.EventHandler(this.cbxRetainOfflineDevices_CheckedChanged);
            // 
            // tbECCONetOnlineDevice
            // 
            this.tbECCONetOnlineDevice.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbECCONetOnlineDevice.Location = new System.Drawing.Point(20, 54);
            this.tbECCONetOnlineDevice.Name = "tbECCONetOnlineDevice";
            this.tbECCONetOnlineDevice.Size = new System.Drawing.Size(315, 324);
            this.tbECCONetOnlineDevice.TabIndex = 12;
            this.tbECCONetOnlineDevice.Text = "";
            // 
            // cbbOnlineDevices
            // 
            this.cbbOnlineDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbOnlineDevices.FormattingEnabled = true;
            this.cbbOnlineDevices.Location = new System.Drawing.Point(69, 20);
            this.cbbOnlineDevices.Name = "cbbOnlineDevices";
            this.cbbOnlineDevices.Size = new System.Drawing.Size(185, 21);
            this.cbbOnlineDevices.TabIndex = 8;
            this.cbbOnlineDevices.SelectedIndexChanged += new System.EventHandler(this.cbbOnlineDevices_SelectedIndexChanged);
            // 
            // btnClearOnlineDevicesList
            // 
            this.btnClearOnlineDevicesList.Location = new System.Drawing.Point(260, 19);
            this.btnClearOnlineDevicesList.Name = "btnClearOnlineDevicesList";
            this.btnClearOnlineDevicesList.Size = new System.Drawing.Size(75, 23);
            this.btnClearOnlineDevicesList.TabIndex = 23;
            this.btnClearOnlineDevicesList.Text = "Reset";
            this.btnClearOnlineDevicesList.UseVisualStyleBackColor = true;
            this.btnClearOnlineDevicesList.Click += new System.EventHandler(this.btnClearOnlineDevicesList_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Devices";
            // 
            // cbxForcePowerState
            // 
            this.cbxForcePowerState.AutoSize = true;
            this.cbxForcePowerState.Location = new System.Drawing.Point(21, 25);
            this.cbxForcePowerState.Name = "cbxForcePowerState";
            this.cbxForcePowerState.Size = new System.Drawing.Size(103, 17);
            this.cbxForcePowerState.TabIndex = 22;
            this.cbxForcePowerState.Text = "Force Power On";
            this.cbxForcePowerState.UseVisualStyleBackColor = true;
            this.cbxForcePowerState.CheckedChanged += new System.EventHandler(this.cbxForcePowerState_CheckedChanged);
            // 
            // groupBoxSystemPowerState
            // 
            this.groupBoxSystemPowerState.Controls.Add(this.label4);
            this.groupBoxSystemPowerState.Controls.Add(this.pbxPowerState);
            this.groupBoxSystemPowerState.Controls.Add(this.label2);
            this.groupBoxSystemPowerState.Controls.Add(this.label1);
            this.groupBoxSystemPowerState.Controls.Add(this.pbxRun);
            this.groupBoxSystemPowerState.Controls.Add(this.pbxAcc);
            this.groupBoxSystemPowerState.Controls.Add(this.cbxForcePowerState);
            this.groupBoxSystemPowerState.Location = new System.Drawing.Point(189, 18);
            this.groupBoxSystemPowerState.Name = "groupBoxSystemPowerState";
            this.groupBoxSystemPowerState.Size = new System.Drawing.Size(186, 117);
            this.groupBoxSystemPowerState.TabIndex = 23;
            this.groupBoxSystemPowerState.TabStop = false;
            this.groupBoxSystemPowerState.Text = "System Power State";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(111, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Power State";
            // 
            // pbxPowerState
            // 
            this.pbxPowerState.BackColor = System.Drawing.Color.White;
            this.pbxPowerState.Location = new System.Drawing.Point(131, 68);
            this.pbxPowerState.Name = "pbxPowerState";
            this.pbxPowerState.Size = new System.Drawing.Size(20, 20);
            this.pbxPowerState.TabIndex = 27;
            this.pbxPowerState.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(73, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Run";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Acc";
            // 
            // pbxRun
            // 
            this.pbxRun.BackColor = System.Drawing.Color.White;
            this.pbxRun.Location = new System.Drawing.Point(76, 68);
            this.pbxRun.Name = "pbxRun";
            this.pbxRun.Size = new System.Drawing.Size(20, 20);
            this.pbxRun.TabIndex = 24;
            this.pbxRun.TabStop = false;
            // 
            // pbxAcc
            // 
            this.pbxAcc.BackColor = System.Drawing.Color.White;
            this.pbxAcc.Location = new System.Drawing.Point(21, 68);
            this.pbxAcc.Name = "pbxAcc";
            this.pbxAcc.Size = new System.Drawing.Size(20, 20);
            this.pbxAcc.TabIndex = 23;
            this.pbxAcc.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(403, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(145, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "USB-CAN Transmit Delay mS";
            // 
            // cbbTransmitDelay
            // 
            this.cbbTransmitDelay.FormattingEnabled = true;
            this.cbbTransmitDelay.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.cbbTransmitDelay.Location = new System.Drawing.Point(406, 53);
            this.cbbTransmitDelay.Name = "cbbTransmitDelay";
            this.cbbTransmitDelay.Size = new System.Drawing.Size(45, 21);
            this.cbbTransmitDelay.TabIndex = 26;
            this.cbbTransmitDelay.Text = "1";
            this.cbbTransmitDelay.SelectedIndexChanged += new System.EventHandler(this.cbbTransmitDelay_SelectedIndexChanged);
            // 
            // OnlineMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbbTransmitDelay);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBoxSystemPowerState);
            this.Controls.Add(this.groupBoxInterface);
            this.Controls.Add(this.groupBoxOnlineDevices);
            this.Name = "OnlineMonitor";
            this.Size = new System.Drawing.Size(749, 573);
            this.groupBoxInterface.ResumeLayout(false);
            this.groupBoxInterface.PerformLayout();
            this.groupBoxOnlineDevices.ResumeLayout(false);
            this.groupBoxOnlineDevices.PerformLayout();
            this.groupBoxSystemPowerState.ResumeLayout(false);
            this.groupBoxSystemPowerState.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxPowerState)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxRun)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxAcc)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxInterface;
        private System.Windows.Forms.Label labelStatusUsbCan;
        private System.Windows.Forms.Label labelStatusPcanUsb;
        private System.Windows.Forms.RadioButton radBtnUsbCan;
        private System.Windows.Forms.RadioButton radBtnPcanUsb;
        private System.Windows.Forms.GroupBox groupBoxOnlineDevices;
        private System.Windows.Forms.RichTextBox tbECCONetOnlineDevice;
        private System.Windows.Forms.ComboBox cbbOnlineDevices;
        private System.Windows.Forms.Button btnClearOnlineDevicesList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbxForcePowerState;
        private System.Windows.Forms.GroupBox groupBoxSystemPowerState;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pbxPowerState;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pbxRun;
        private System.Windows.Forms.PictureBox pbxAcc;
        private System.Windows.Forms.CheckBox cbxRetainOfflineDevices;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.ComboBox cbbTransmitDelay;
    }
}
