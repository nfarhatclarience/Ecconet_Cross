namespace ECCONetDevTool.BusStressTest
{
    partial class ucBusFlood
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
            this.gbxBusFlood = new System.Windows.Forms.GroupBox();
            this.cbxUseDefaultToken = new System.Windows.Forms.CheckBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblError = new System.Windows.Forms.Label();
            this.tbAddress = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbType = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.tbKey = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.lblApproxBusUtilization = new System.Windows.Forms.Label();
            this.lblPacketsPerMessage = new System.Windows.Forms.Label();
            this.lblActualPacketPerSecond = new System.Windows.Forms.Label();
            this.ntbPacketsPerMessage = new ECCONetDevTool.NumericTextBox();
            this.lblNumPackets = new System.Windows.Forms.Label();
            this.lblMessagesPerSecond = new System.Windows.Forms.Label();
            this.lblNumMessages = new System.Windows.Forms.Label();
            this.ntbMessagesPerSecond = new ECCONetDevTool.NumericTextBox();
            this.gbxBusFlood.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxBusFlood
            // 
            this.gbxBusFlood.Controls.Add(this.cbxUseDefaultToken);
            this.gbxBusFlood.Controls.Add(this.btnRun);
            this.gbxBusFlood.Controls.Add(this.btnStop);
            this.gbxBusFlood.Controls.Add(this.groupBox1);
            this.gbxBusFlood.Controls.Add(this.lblApproxBusUtilization);
            this.gbxBusFlood.Controls.Add(this.lblPacketsPerMessage);
            this.gbxBusFlood.Controls.Add(this.lblActualPacketPerSecond);
            this.gbxBusFlood.Controls.Add(this.ntbPacketsPerMessage);
            this.gbxBusFlood.Controls.Add(this.lblNumPackets);
            this.gbxBusFlood.Controls.Add(this.lblMessagesPerSecond);
            this.gbxBusFlood.Controls.Add(this.lblNumMessages);
            this.gbxBusFlood.Controls.Add(this.ntbMessagesPerSecond);
            this.gbxBusFlood.Location = new System.Drawing.Point(3, 3);
            this.gbxBusFlood.Name = "gbxBusFlood";
            this.gbxBusFlood.Size = new System.Drawing.Size(406, 309);
            this.gbxBusFlood.TabIndex = 60;
            this.gbxBusFlood.TabStop = false;
            this.gbxBusFlood.Text = "Bus Flood";
            // 
            // cbxUseDefaultToken
            // 
            this.cbxUseDefaultToken.AutoSize = true;
            this.cbxUseDefaultToken.Checked = true;
            this.cbxUseDefaultToken.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxUseDefaultToken.Location = new System.Drawing.Point(217, 120);
            this.cbxUseDefaultToken.Name = "cbxUseDefaultToken";
            this.cbxUseDefaultToken.Size = new System.Drawing.Size(145, 17);
            this.cbxUseDefaultToken.TabIndex = 75;
            this.cbxUseDefaultToken.Text = "Use Default Flood Token";
            this.cbxUseDefaultToken.UseVisualStyleBackColor = true;
            this.cbxUseDefaultToken.CheckedChanged += new System.EventHandler(this.cbxUseDefaultToken_CheckedChanged);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(217, 155);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 74;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(217, 187);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 73;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblError);
            this.groupBox1.Controls.Add(this.tbAddress);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.tbType);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.tbValue);
            this.groupBox1.Controls.Add(this.tbKey);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Location = new System.Drawing.Point(6, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(394, 75);
            this.groupBox1.TabIndex = 51;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Token to Send";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.ForeColor = System.Drawing.Color.DarkRed;
            this.lblError.Location = new System.Drawing.Point(371, 47);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(11, 13);
            this.lblError.TabIndex = 57;
            this.lblError.Text = "!";
            this.lblError.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblError.Visible = false;
            // 
            // tbAddress
            // 
            this.tbAddress.Location = new System.Drawing.Point(13, 44);
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.Size = new System.Drawing.Size(54, 20);
            this.tbAddress.TabIndex = 41;
            this.tbAddress.TextChanged += new System.EventHandler(this.tbAddress_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 40;
            this.label8.Text = "Address";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbType
            // 
            this.tbType.Location = new System.Drawing.Point(73, 44);
            this.tbType.Name = "tbType";
            this.tbType.Size = new System.Drawing.Size(31, 20);
            this.tbType.TabIndex = 43;
            this.tbType.TextChanged += new System.EventHandler(this.tbType_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(301, 28);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(34, 13);
            this.label11.TabIndex = 46;
            this.label11.Text = "Value";
            this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(72, 28);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 13);
            this.label9.TabIndex = 42;
            this.label9.Text = "Type";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbValue
            // 
            this.tbValue.Location = new System.Drawing.Point(302, 44);
            this.tbValue.Name = "tbValue";
            this.tbValue.Size = new System.Drawing.Size(63, 20);
            this.tbValue.TabIndex = 47;
            this.tbValue.TextChanged += new System.EventHandler(this.tbValue_TextChanged);
            // 
            // tbKey
            // 
            this.tbKey.Location = new System.Drawing.Point(110, 44);
            this.tbKey.Name = "tbKey";
            this.tbKey.Size = new System.Drawing.Size(186, 20);
            this.tbKey.TabIndex = 45;
            this.tbKey.TextChanged += new System.EventHandler(this.tbKey_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(109, 28);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(25, 13);
            this.label10.TabIndex = 44;
            this.label10.Text = "Key";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblApproxBusUtilization
            // 
            this.lblApproxBusUtilization.AutoSize = true;
            this.lblApproxBusUtilization.Location = new System.Drawing.Point(16, 279);
            this.lblApproxBusUtilization.Name = "lblApproxBusUtilization";
            this.lblApproxBusUtilization.Size = new System.Drawing.Size(115, 13);
            this.lblApproxBusUtilization.TabIndex = 58;
            this.lblApproxBusUtilization.Text = "Approx. Bus Utilization:";
            // 
            // lblPacketsPerMessage
            // 
            this.lblPacketsPerMessage.AutoSize = true;
            this.lblPacketsPerMessage.Location = new System.Drawing.Point(18, 120);
            this.lblPacketsPerMessage.Name = "lblPacketsPerMessage";
            this.lblPacketsPerMessage.Size = new System.Drawing.Size(140, 13);
            this.lblPacketsPerMessage.TabIndex = 48;
            this.lblPacketsPerMessage.Text = "Packets per Message (1-10)";
            this.lblPacketsPerMessage.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblActualPacketPerSecond
            // 
            this.lblActualPacketPerSecond.AutoSize = true;
            this.lblActualPacketPerSecond.Location = new System.Drawing.Point(16, 262);
            this.lblActualPacketPerSecond.Name = "lblActualPacketPerSecond";
            this.lblActualPacketPerSecond.Size = new System.Drawing.Size(140, 13);
            this.lblActualPacketPerSecond.TabIndex = 57;
            this.lblActualPacketPerSecond.Text = "Actual Packets per Second:";
            // 
            // ntbPacketsPerMessage
            // 
            this.ntbPacketsPerMessage.AcceptHexInput = true;
            this.ntbPacketsPerMessage.HighlightInputError = true;
            this.ntbPacketsPerMessage.Location = new System.Drawing.Point(19, 136);
            this.ntbPacketsPerMessage.Name = "ntbPacketsPerMessage";
            this.ntbPacketsPerMessage.Size = new System.Drawing.Size(22, 20);
            this.ntbPacketsPerMessage.TabIndex = 50;
            this.ntbPacketsPerMessage.TextChanged += new System.EventHandler(this.ntbPacketsPerMessage_TextChanged);
            // 
            // lblNumPackets
            // 
            this.lblNumPackets.AutoSize = true;
            this.lblNumPackets.Location = new System.Drawing.Point(16, 228);
            this.lblNumPackets.Name = "lblNumPackets";
            this.lblNumPackets.Size = new System.Drawing.Size(49, 13);
            this.lblNumPackets.TabIndex = 56;
            this.lblNumPackets.Text = "Packets:";
            // 
            // lblMessagesPerSecond
            // 
            this.lblMessagesPerSecond.AutoSize = true;
            this.lblMessagesPerSecond.Location = new System.Drawing.Point(18, 174);
            this.lblMessagesPerSecond.Name = "lblMessagesPerSecond";
            this.lblMessagesPerSecond.Size = new System.Drawing.Size(143, 13);
            this.lblMessagesPerSecond.TabIndex = 52;
            this.lblMessagesPerSecond.Text = "Messages per Second (1-50)";
            this.lblMessagesPerSecond.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblNumMessages
            // 
            this.lblNumMessages.AutoSize = true;
            this.lblNumMessages.Location = new System.Drawing.Point(16, 245);
            this.lblNumMessages.Name = "lblNumMessages";
            this.lblNumMessages.Size = new System.Drawing.Size(58, 13);
            this.lblNumMessages.TabIndex = 55;
            this.lblNumMessages.Text = "Messages:";
            // 
            // ntbMessagesPerSecond
            // 
            this.ntbMessagesPerSecond.AcceptHexInput = true;
            this.ntbMessagesPerSecond.HighlightInputError = true;
            this.ntbMessagesPerSecond.Location = new System.Drawing.Point(19, 190);
            this.ntbMessagesPerSecond.Name = "ntbMessagesPerSecond";
            this.ntbMessagesPerSecond.Size = new System.Drawing.Size(22, 20);
            this.ntbMessagesPerSecond.TabIndex = 53;
            this.ntbMessagesPerSecond.TextChanged += new System.EventHandler(this.ntbMessagesPerSecond_TextChanged);
            // 
            // ucBusFlood
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxBusFlood);
            this.Name = "ucBusFlood";
            this.Size = new System.Drawing.Size(412, 315);
            this.gbxBusFlood.ResumeLayout(false);
            this.gbxBusFlood.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxBusFlood;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.TextBox tbAddress;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbType;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbValue;
        private System.Windows.Forms.TextBox tbKey;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblApproxBusUtilization;
        private System.Windows.Forms.Label lblPacketsPerMessage;
        private System.Windows.Forms.Label lblActualPacketPerSecond;
        private NumericTextBox ntbPacketsPerMessage;
        private System.Windows.Forms.Label lblNumPackets;
        private System.Windows.Forms.Label lblMessagesPerSecond;
        private System.Windows.Forms.Label lblNumMessages;
        private NumericTextBox ntbMessagesPerSecond;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.CheckBox cbxUseDefaultToken;
    }
}
