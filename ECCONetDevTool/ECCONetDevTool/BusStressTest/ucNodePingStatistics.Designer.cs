namespace ECCONetDevTool.BusStressTest
{
    partial class ucNodePingStatistics
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
            this.gbxNodePingStatistics = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbxMaxRandomSize = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnFind = new System.Windows.Forms.Button();
            this.tbxWriteFileName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbbMode = new System.Windows.Forms.ComboBox();
            this.lblPingAddress = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label5 = new System.Windows.Forms.Label();
            this.rtbResults = new System.Windows.Forms.RichTextBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.gbxTestDuration = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rbtnTestForPeriod = new System.Windows.Forms.RadioButton();
            this.rbtnTestForPingsPerNode = new System.Windows.Forms.RadioButton();
            this.cbxPingAllNodes = new System.Windows.Forms.CheckBox();
            this.cbbOnlineDevices = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.ntbSeconds = new ECCONetDevTool.NumericTextBox();
            this.ntbPingsPerNode = new ECCONetDevTool.NumericTextBox();
            this.ntbMinutes = new ECCONetDevTool.NumericTextBox();
            this.ntbHours = new ECCONetDevTool.NumericTextBox();
            this.gbxNodePingStatistics.SuspendLayout();
            this.gbxTestDuration.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxNodePingStatistics
            // 
            this.gbxNodePingStatistics.Controls.Add(this.label8);
            this.gbxNodePingStatistics.Controls.Add(this.cbxMaxRandomSize);
            this.gbxNodePingStatistics.Controls.Add(this.label7);
            this.gbxNodePingStatistics.Controls.Add(this.btnFind);
            this.gbxNodePingStatistics.Controls.Add(this.tbxWriteFileName);
            this.gbxNodePingStatistics.Controls.Add(this.label6);
            this.gbxNodePingStatistics.Controls.Add(this.cbbMode);
            this.gbxNodePingStatistics.Controls.Add(this.lblPingAddress);
            this.gbxNodePingStatistics.Controls.Add(this.progressBar);
            this.gbxNodePingStatistics.Controls.Add(this.label5);
            this.gbxNodePingStatistics.Controls.Add(this.rtbResults);
            this.gbxNodePingStatistics.Controls.Add(this.btnRun);
            this.gbxNodePingStatistics.Controls.Add(this.btnStop);
            this.gbxNodePingStatistics.Controls.Add(this.gbxTestDuration);
            this.gbxNodePingStatistics.Controls.Add(this.cbxPingAllNodes);
            this.gbxNodePingStatistics.Controls.Add(this.cbbOnlineDevices);
            this.gbxNodePingStatistics.Controls.Add(this.label1);
            this.gbxNodePingStatistics.Location = new System.Drawing.Point(3, 3);
            this.gbxNodePingStatistics.Name = "gbxNodePingStatistics";
            this.gbxNodePingStatistics.Size = new System.Drawing.Size(434, 606);
            this.gbxNodePingStatistics.TabIndex = 61;
            this.gbxNodePingStatistics.TabStop = false;
            this.gbxNodePingStatistics.Text = "Node Ping Statistics";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(287, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 13);
            this.label8.TabIndex = 83;
            this.label8.Text = "Random File Size";
            // 
            // cbxMaxRandomSize
            // 
            this.cbxMaxRandomSize.FormattingEnabled = true;
            this.cbxMaxRandomSize.Items.AddRange(new object[] {
            "1kB",
            "2kB",
            "3kB",
            "4kB",
            "5kB",
            "6kB",
            "7kB",
            "8kB",
            "9kB",
            "10kB",
            "11kB",
            "12kB"});
            this.cbxMaxRandomSize.Location = new System.Drawing.Point(382, 52);
            this.cbxMaxRandomSize.Name = "cbxMaxRandomSize";
            this.cbxMaxRandomSize.Size = new System.Drawing.Size(41, 21);
            this.cbxMaxRandomSize.TabIndex = 82;
            this.cbxMaxRandomSize.Text = "1kB";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 94);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 13);
            this.label7.TabIndex = 81;
            this.label7.Text = "File";
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(348, 89);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(75, 23);
            this.btnFind.TabIndex = 80;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // tbxWriteFileName
            // 
            this.tbxWriteFileName.Location = new System.Drawing.Point(48, 91);
            this.tbxWriteFileName.Name = "tbxWriteFileName";
            this.tbxWriteFileName.Size = new System.Drawing.Size(258, 20);
            this.tbxWriteFileName.TabIndex = 79;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 78;
            this.label6.Text = "Mode";
            // 
            // cbbMode
            // 
            this.cbbMode.FormattingEnabled = true;
            this.cbbMode.Items.AddRange(new object[] {
            "Address Request",
            "Read Info File",
            "Write File",
            "Random Write/Read/Compare/Delete"});
            this.cbbMode.Location = new System.Drawing.Point(48, 52);
            this.cbbMode.Name = "cbbMode";
            this.cbbMode.Size = new System.Drawing.Size(223, 21);
            this.cbbMode.TabIndex = 77;
            this.cbbMode.Text = "Address Request";
            // 
            // lblPingAddress
            // 
            this.lblPingAddress.AutoSize = true;
            this.lblPingAddress.Location = new System.Drawing.Point(12, 243);
            this.lblPingAddress.Name = "lblPingAddress";
            this.lblPingAddress.Size = new System.Drawing.Size(130, 13);
            this.lblPingAddress.TabIndex = 76;
            this.lblPingAddress.Text = "Currently Pinging Address:";
            this.lblPingAddress.Visible = false;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(15, 226);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(408, 14);
            this.progressBar.TabIndex = 75;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 264);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 74;
            this.label5.Text = "Results";
            // 
            // rtbResults
            // 
            this.rtbResults.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbResults.Location = new System.Drawing.Point(15, 280);
            this.rtbResults.Name = "rtbResults";
            this.rtbResults.Size = new System.Drawing.Size(408, 320);
            this.rtbResults.TabIndex = 73;
            this.rtbResults.Text = "";
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(348, 145);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 72;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(348, 178);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 71;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // gbxTestDuration
            // 
            this.gbxTestDuration.Controls.Add(this.label4);
            this.gbxTestDuration.Controls.Add(this.label3);
            this.gbxTestDuration.Controls.Add(this.label2);
            this.gbxTestDuration.Controls.Add(this.ntbSeconds);
            this.gbxTestDuration.Controls.Add(this.ntbPingsPerNode);
            this.gbxTestDuration.Controls.Add(this.rbtnTestForPeriod);
            this.gbxTestDuration.Controls.Add(this.rbtnTestForPingsPerNode);
            this.gbxTestDuration.Controls.Add(this.ntbMinutes);
            this.gbxTestDuration.Controls.Add(this.ntbHours);
            this.gbxTestDuration.Location = new System.Drawing.Point(15, 127);
            this.gbxTestDuration.Name = "gbxTestDuration";
            this.gbxTestDuration.Size = new System.Drawing.Size(291, 93);
            this.gbxTestDuration.TabIndex = 70;
            this.gbxTestDuration.TabStop = false;
            this.gbxTestDuration.Text = "Test Duration";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(97, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 72;
            this.label4.Text = "Sec";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(53, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 13);
            this.label3.TabIndex = 71;
            this.label3.Text = "Min";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 70;
            this.label2.Text = "Hrs";
            // 
            // rbtnTestForPeriod
            // 
            this.rbtnTestForPeriod.AutoSize = true;
            this.rbtnTestForPeriod.Checked = true;
            this.rbtnTestForPeriod.Location = new System.Drawing.Point(148, 35);
            this.rbtnTestForPeriod.Name = "rbtnTestForPeriod";
            this.rbtnTestForPeriod.Size = new System.Drawing.Size(55, 17);
            this.rbtnTestForPeriod.TabIndex = 64;
            this.rbtnTestForPeriod.TabStop = true;
            this.rbtnTestForPeriod.Text = "Period";
            this.rbtnTestForPeriod.UseVisualStyleBackColor = true;
            this.rbtnTestForPeriod.CheckedChanged += new System.EventHandler(this.rbtnTestForPeriod_CheckedChanged);
            // 
            // rbtnTestForPingsPerNode
            // 
            this.rbtnTestForPingsPerNode.AutoSize = true;
            this.rbtnTestForPingsPerNode.Location = new System.Drawing.Point(148, 63);
            this.rbtnTestForPingsPerNode.Name = "rbtnTestForPingsPerNode";
            this.rbtnTestForPingsPerNode.Size = new System.Drawing.Size(99, 17);
            this.rbtnTestForPingsPerNode.TabIndex = 65;
            this.rbtnTestForPingsPerNode.Text = "Pings Per Node";
            this.rbtnTestForPingsPerNode.UseVisualStyleBackColor = true;
            this.rbtnTestForPingsPerNode.CheckedChanged += new System.EventHandler(this.rbtnTestForPingsPerNode_CheckedChanged);
            // 
            // cbxPingAllNodes
            // 
            this.cbxPingAllNodes.AutoSize = true;
            this.cbxPingAllNodes.Location = new System.Drawing.Point(315, 22);
            this.cbxPingAllNodes.Name = "cbxPingAllNodes";
            this.cbxPingAllNodes.Size = new System.Drawing.Size(61, 17);
            this.cbxPingAllNodes.TabIndex = 63;
            this.cbxPingAllNodes.Text = "Ping All";
            this.cbxPingAllNodes.UseVisualStyleBackColor = true;
            this.cbxPingAllNodes.CheckedChanged += new System.EventHandler(this.cbxPingAllNodes_CheckedChanged);
            // 
            // cbbOnlineDevices
            // 
            this.cbbOnlineDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbOnlineDevices.FormattingEnabled = true;
            this.cbbOnlineDevices.Location = new System.Drawing.Point(48, 20);
            this.cbbOnlineDevices.Name = "cbbOnlineDevices";
            this.cbbOnlineDevices.Size = new System.Drawing.Size(258, 21);
            this.cbbOnlineDevices.TabIndex = 61;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 62;
            this.label1.Text = "Node";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "product.inf";
            this.openFileDialog1.Filter = "Any File | *.*";
            // 
            // ntbSeconds
            // 
            this.ntbSeconds.AcceptHexInput = true;
            this.ntbSeconds.HighlightInputError = false;
            this.ntbSeconds.Location = new System.Drawing.Point(100, 34);
            this.ntbSeconds.Name = "ntbSeconds";
            this.ntbSeconds.Size = new System.Drawing.Size(38, 20);
            this.ntbSeconds.TabIndex = 68;
            this.ntbSeconds.TextChanged += new System.EventHandler(this.ntbSeconds_TextChanged);
            // 
            // ntbPingsPerNode
            // 
            this.ntbPingsPerNode.AcceptHexInput = true;
            this.ntbPingsPerNode.HighlightInputError = false;
            this.ntbPingsPerNode.Location = new System.Drawing.Point(100, 62);
            this.ntbPingsPerNode.Name = "ntbPingsPerNode";
            this.ntbPingsPerNode.Size = new System.Drawing.Size(38, 20);
            this.ntbPingsPerNode.TabIndex = 69;
            this.ntbPingsPerNode.TextChanged += new System.EventHandler(this.ntbPingsPerNode_TextChanged);
            // 
            // ntbMinutes
            // 
            this.ntbMinutes.AcceptHexInput = true;
            this.ntbMinutes.HighlightInputError = false;
            this.ntbMinutes.Location = new System.Drawing.Point(56, 34);
            this.ntbMinutes.Name = "ntbMinutes";
            this.ntbMinutes.Size = new System.Drawing.Size(38, 20);
            this.ntbMinutes.TabIndex = 67;
            this.ntbMinutes.TextChanged += new System.EventHandler(this.ntbMinutes_TextChanged);
            // 
            // ntbHours
            // 
            this.ntbHours.AcceptHexInput = true;
            this.ntbHours.HighlightInputError = false;
            this.ntbHours.Location = new System.Drawing.Point(12, 34);
            this.ntbHours.Name = "ntbHours";
            this.ntbHours.Size = new System.Drawing.Size(38, 20);
            this.ntbHours.TabIndex = 66;
            this.ntbHours.TextChanged += new System.EventHandler(this.ntbHours_TextChanged);
            // 
            // ucNodePingStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxNodePingStatistics);
            this.Name = "ucNodePingStatistics";
            this.Size = new System.Drawing.Size(440, 612);
            this.gbxNodePingStatistics.ResumeLayout(false);
            this.gbxNodePingStatistics.PerformLayout();
            this.gbxTestDuration.ResumeLayout(false);
            this.gbxTestDuration.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxNodePingStatistics;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox rtbResults;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.GroupBox gbxTestDuration;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private NumericTextBox ntbSeconds;
        private NumericTextBox ntbPingsPerNode;
        private System.Windows.Forms.RadioButton rbtnTestForPeriod;
        private System.Windows.Forms.RadioButton rbtnTestForPingsPerNode;
        private NumericTextBox ntbMinutes;
        private NumericTextBox ntbHours;
        private System.Windows.Forms.CheckBox cbxPingAllNodes;
        private System.Windows.Forms.ComboBox cbbOnlineDevices;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblPingAddress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbbMode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.TextBox tbxWriteFileName;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbxMaxRandomSize;
    }
}
