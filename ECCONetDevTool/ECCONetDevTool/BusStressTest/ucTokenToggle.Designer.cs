namespace ECCONetDevTool.BusStressTest
{
    partial class ucTokenToggle
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
            this.btnRun = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbValue2 = new System.Windows.Forms.TextBox();
            this.lblError = new System.Windows.Forms.Label();
            this.tbAddress = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbType = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tbValue1 = new System.Windows.Forms.TextBox();
            this.tbKey = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.lblMessagesPerSecond = new System.Windows.Forms.Label();
            this.ntbTokensPerSecond = new ECCONetDevTool.NumericTextBox();
            this.lblTokensSent = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(1016, 210);
            this.btnRun.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(200, 55);
            this.btnRun.TabIndex = 79;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(1016, 278);
            this.btnStop.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(200, 55);
            this.btnStop.TabIndex = 78;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbValue2);
            this.groupBox1.Controls.Add(this.lblError);
            this.groupBox1.Controls.Add(this.tbAddress);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.tbType);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.tbValue1);
            this.groupBox1.Controls.Add(this.tbKey);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Location = new System.Drawing.Point(8, 7);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox1.Size = new System.Drawing.Size(1208, 179);
            this.groupBox1.TabIndex = 75;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Token to Send";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(989, 67);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 32);
            this.label1.TabIndex = 58;
            this.label1.Text = "Value 2";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbValue2
            // 
            this.tbValue2.Location = new System.Drawing.Point(991, 105);
            this.tbValue2.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.tbValue2.Name = "tbValue2";
            this.tbValue2.Size = new System.Drawing.Size(161, 38);
            this.tbValue2.TabIndex = 59;
            this.tbValue2.TextChanged += new System.EventHandler(this.tbValue2_TextChanged);
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.ForeColor = System.Drawing.Color.DarkRed;
            this.lblError.Location = new System.Drawing.Point(1168, 111);
            this.lblError.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(24, 32);
            this.lblError.TabIndex = 57;
            this.lblError.Text = "!";
            this.lblError.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblError.Visible = false;
            // 
            // tbAddress
            // 
            this.tbAddress.Location = new System.Drawing.Point(35, 105);
            this.tbAddress.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.Size = new System.Drawing.Size(137, 38);
            this.tbAddress.TabIndex = 41;
            this.tbAddress.TextChanged += new System.EventHandler(this.tbAddress_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(32, 67);
            this.label8.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(119, 32);
            this.label8.TabIndex = 40;
            this.label8.Text = "Address";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbType
            // 
            this.tbType.Location = new System.Drawing.Point(195, 105);
            this.tbType.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.tbType.Name = "tbType";
            this.tbType.Size = new System.Drawing.Size(76, 38);
            this.tbType.TabIndex = 43;
            this.tbType.TextChanged += new System.EventHandler(this.tbType_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(803, 67);
            this.label11.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(112, 32);
            this.label11.TabIndex = 46;
            this.label11.Text = "Value 1";
            this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(192, 67);
            this.label9.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 32);
            this.label9.TabIndex = 42;
            this.label9.Text = "Type";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbValue1
            // 
            this.tbValue1.Location = new System.Drawing.Point(805, 105);
            this.tbValue1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.tbValue1.Name = "tbValue1";
            this.tbValue1.Size = new System.Drawing.Size(161, 38);
            this.tbValue1.TabIndex = 47;
            this.tbValue1.TextChanged += new System.EventHandler(this.tbValue1_TextChanged);
            // 
            // tbKey
            // 
            this.tbKey.Location = new System.Drawing.Point(293, 105);
            this.tbKey.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.tbKey.Name = "tbKey";
            this.tbKey.Size = new System.Drawing.Size(489, 38);
            this.tbKey.TabIndex = 45;
            this.tbKey.TextChanged += new System.EventHandler(this.tbKey_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(291, 67);
            this.label10.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 32);
            this.label10.TabIndex = 44;
            this.label10.Text = "Key";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblMessagesPerSecond
            // 
            this.lblMessagesPerSecond.AutoSize = true;
            this.lblMessagesPerSecond.Location = new System.Drawing.Point(37, 222);
            this.lblMessagesPerSecond.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblMessagesPerSecond.Name = "lblMessagesPerSecond";
            this.lblMessagesPerSecond.Size = new System.Drawing.Size(342, 32);
            this.lblMessagesPerSecond.TabIndex = 76;
            this.lblMessagesPerSecond.Text = "Tokens per Second (1-50)";
            this.lblMessagesPerSecond.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ntbTokensPerSecond
            // 
            this.ntbTokensPerSecond.AcceptHexInput = true;
            this.ntbTokensPerSecond.HighlightInputError = true;
            this.ntbTokensPerSecond.Location = new System.Drawing.Point(43, 261);
            this.ntbTokensPerSecond.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.ntbTokensPerSecond.Name = "ntbTokensPerSecond";
            this.ntbTokensPerSecond.Size = new System.Drawing.Size(52, 38);
            this.ntbTokensPerSecond.TabIndex = 77;
            this.ntbTokensPerSecond.TextChanged += new System.EventHandler(this.ntbTokensPerSecond_TextChanged);
            // 
            // lblTokensSent
            // 
            this.lblTokensSent.AutoSize = true;
            this.lblTokensSent.Location = new System.Drawing.Point(471, 222);
            this.lblTokensSent.Name = "lblTokensSent";
            this.lblTokensSent.Size = new System.Drawing.Size(182, 32);
            this.lblTokensSent.TabIndex = 80;
            this.lblTokensSent.Text = "Tokens Sent:";
            this.lblTokensSent.Visible = false;
            // 
            // ucTokenToggle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblTokensSent);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblMessagesPerSecond);
            this.Controls.Add(this.ntbTokensPerSecond);
            this.Name = "ucTokenToggle";
            this.Size = new System.Drawing.Size(1220, 340);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbValue2;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.TextBox tbAddress;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbType;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbValue1;
        private System.Windows.Forms.TextBox tbKey;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblMessagesPerSecond;
        private NumericTextBox ntbTokensPerSecond;
        private System.Windows.Forms.Label lblTokensSent;
    }
}
