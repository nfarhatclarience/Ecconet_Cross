namespace ECCONetDevTool.LedMatrixFile
{
    partial class ucLedMatrixFile
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
            this.dgvMessages = new System.Windows.Forms.DataGridView();
            this.clmEnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnBuild = new System.Windows.Forms.Button();
            this.pbxTest = new System.Windows.Forms.PictureBox();
            this.cbbMode = new System.Windows.Forms.ComboBox();
            this.cbxScrollOnce = new System.Windows.Forms.CheckBox();
            this.cbxMirror = new System.Windows.Forms.CheckBox();
            this.cbxWrap = new System.Windows.Forms.CheckBox();
            this.bntShow = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnHide = new System.Windows.Forms.Button();
            this.cbbOnlineDevices = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.gbxSendControlMessage = new System.Windows.Forms.GroupBox();
            this.cbxSequencer = new System.Windows.Forms.CheckBox();
            this.rbtnCommon = new System.Windows.Forms.RadioButton();
            this.rbtnRear = new System.Windows.Forms.RadioButton();
            this.rbtnFront = new System.Windows.Forms.RadioButton();
            this.tbxValueHex = new System.Windows.Forms.TextBox();
            this.tbxValue = new System.Windows.Forms.TextBox();
            this.btnCalcAndShow = new System.Windows.Forms.Button();
            this.btnBuildPatternFile = new System.Windows.Forms.Button();
            this.tbxExpressionCollectionFileName = new System.Windows.Forms.TextBox();
            this.btnFindPatternFile = new System.Windows.Forms.Button();
            this.openFileDialogExpression = new System.Windows.Forms.OpenFileDialog();
            this.cbxScrollStartOnScreen = new System.Windows.Forms.CheckBox();
            this.cbbJustification = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ntbIntensity = new ECCONetDevTool.NumericTextBox();
            this.ntbSpeedPeriod = new ECCONetDevTool.NumericTextBox();
            this.ntbEnum = new ECCONetDevTool.NumericTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTest)).BeginInit();
            this.gbxSendControlMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvMessages
            // 
            this.dgvMessages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMessages.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmEnum,
            this.clmText});
            this.dgvMessages.Location = new System.Drawing.Point(9, 24);
            this.dgvMessages.Name = "dgvMessages";
            this.dgvMessages.Size = new System.Drawing.Size(587, 277);
            this.dgvMessages.TabIndex = 0;
            this.dgvMessages.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvMessages_DataError);
            // 
            // clmEnum
            // 
            this.clmEnum.DataPropertyName = "MessageEnum";
            this.clmEnum.HeaderText = "Enum";
            this.clmEnum.Name = "clmEnum";
            this.clmEnum.Width = 40;
            // 
            // clmText
            // 
            this.clmText.DataPropertyName = "Text";
            this.clmText.HeaderText = "Message";
            this.clmText.Name = "clmText";
            this.clmText.Width = 500;
            // 
            // btnBuild
            // 
            this.btnBuild.Location = new System.Drawing.Point(9, 307);
            this.btnBuild.Name = "btnBuild";
            this.btnBuild.Size = new System.Drawing.Size(110, 23);
            this.btnBuild.TabIndex = 1;
            this.btnBuild.Text = "Build Message File";
            this.btnBuild.UseVisualStyleBackColor = true;
            this.btnBuild.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // pbxTest
            // 
            this.pbxTest.Location = new System.Drawing.Point(9, 403);
            this.pbxTest.Name = "pbxTest";
            this.pbxTest.Size = new System.Drawing.Size(1055, 214);
            this.pbxTest.TabIndex = 2;
            this.pbxTest.TabStop = false;
            // 
            // cbbMode
            // 
            this.cbbMode.FormattingEnabled = true;
            this.cbbMode.Items.AddRange(new object[] {
            "Off",
            "Static",
            "Timed Static",
            "Scroll Left",
            "Scroll Right",
            "Scroll Up",
            "Scroll Down"});
            this.cbbMode.Location = new System.Drawing.Point(151, 138);
            this.cbbMode.Name = "cbbMode";
            this.cbbMode.Size = new System.Drawing.Size(106, 21);
            this.cbbMode.TabIndex = 6;
            this.cbbMode.Text = "Off";
            // 
            // cbxScrollOnce
            // 
            this.cbxScrollOnce.AutoSize = true;
            this.cbxScrollOnce.Location = new System.Drawing.Point(244, 224);
            this.cbxScrollOnce.Name = "cbxScrollOnce";
            this.cbxScrollOnce.Size = new System.Drawing.Size(81, 17);
            this.cbxScrollOnce.TabIndex = 7;
            this.cbxScrollOnce.Text = "Scroll Once";
            this.cbxScrollOnce.UseVisualStyleBackColor = true;
            // 
            // cbxMirror
            // 
            this.cbxMirror.AutoSize = true;
            this.cbxMirror.Location = new System.Drawing.Point(244, 246);
            this.cbxMirror.Name = "cbxMirror";
            this.cbxMirror.Size = new System.Drawing.Size(52, 17);
            this.cbxMirror.TabIndex = 8;
            this.cbxMirror.Text = "Mirror";
            this.cbxMirror.UseVisualStyleBackColor = true;
            // 
            // cbxWrap
            // 
            this.cbxWrap.AutoSize = true;
            this.cbxWrap.Location = new System.Drawing.Point(244, 269);
            this.cbxWrap.Name = "cbxWrap";
            this.cbxWrap.Size = new System.Drawing.Size(52, 17);
            this.cbxWrap.TabIndex = 9;
            this.cbxWrap.Text = "Wrap";
            this.cbxWrap.UseVisualStyleBackColor = true;
            // 
            // bntShow
            // 
            this.bntShow.Location = new System.Drawing.Point(21, 213);
            this.bntShow.Name = "bntShow";
            this.bntShow.Size = new System.Drawing.Size(74, 23);
            this.bntShow.TabIndex = 10;
            this.bntShow.Text = "Show";
            this.bntShow.UseVisualStyleBackColor = true;
            this.bntShow.Click += new System.EventHandler(this.bntShow_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(263, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Message Enum";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(263, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Speed/Period";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(263, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Intensity";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(263, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Mode";
            // 
            // btnHide
            // 
            this.btnHide.Location = new System.Drawing.Point(21, 242);
            this.btnHide.Name = "btnHide";
            this.btnHide.Size = new System.Drawing.Size(74, 23);
            this.btnHide.TabIndex = 15;
            this.btnHide.Text = "Hide";
            this.btnHide.UseVisualStyleBackColor = true;
            this.btnHide.Click += new System.EventHandler(this.btnHide_Click);
            // 
            // cbbOnlineDevices
            // 
            this.cbbOnlineDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbOnlineDevices.FormattingEnabled = true;
            this.cbbOnlineDevices.Location = new System.Drawing.Point(56, 21);
            this.cbbOnlineDevices.Name = "cbbOnlineDevices";
            this.cbbOnlineDevices.Size = new System.Drawing.Size(258, 21);
            this.cbbOnlineDevices.TabIndex = 93;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 94;
            this.label5.Text = "Device";
            // 
            // gbxSendControlMessage
            // 
            this.gbxSendControlMessage.Controls.Add(this.label6);
            this.gbxSendControlMessage.Controls.Add(this.cbbJustification);
            this.gbxSendControlMessage.Controls.Add(this.cbxScrollStartOnScreen);
            this.gbxSendControlMessage.Controls.Add(this.cbxSequencer);
            this.gbxSendControlMessage.Controls.Add(this.rbtnCommon);
            this.gbxSendControlMessage.Controls.Add(this.rbtnRear);
            this.gbxSendControlMessage.Controls.Add(this.rbtnFront);
            this.gbxSendControlMessage.Controls.Add(this.tbxValueHex);
            this.gbxSendControlMessage.Controls.Add(this.tbxValue);
            this.gbxSendControlMessage.Controls.Add(this.btnCalcAndShow);
            this.gbxSendControlMessage.Controls.Add(this.cbbOnlineDevices);
            this.gbxSendControlMessage.Controls.Add(this.label5);
            this.gbxSendControlMessage.Controls.Add(this.btnHide);
            this.gbxSendControlMessage.Controls.Add(this.label4);
            this.gbxSendControlMessage.Controls.Add(this.label3);
            this.gbxSendControlMessage.Controls.Add(this.label2);
            this.gbxSendControlMessage.Controls.Add(this.label1);
            this.gbxSendControlMessage.Controls.Add(this.bntShow);
            this.gbxSendControlMessage.Controls.Add(this.cbxWrap);
            this.gbxSendControlMessage.Controls.Add(this.cbxMirror);
            this.gbxSendControlMessage.Controls.Add(this.cbxScrollOnce);
            this.gbxSendControlMessage.Controls.Add(this.cbbMode);
            this.gbxSendControlMessage.Controls.Add(this.ntbIntensity);
            this.gbxSendControlMessage.Controls.Add(this.ntbSpeedPeriod);
            this.gbxSendControlMessage.Controls.Add(this.ntbEnum);
            this.gbxSendControlMessage.Location = new System.Drawing.Point(666, 24);
            this.gbxSendControlMessage.Name = "gbxSendControlMessage";
            this.gbxSendControlMessage.Size = new System.Drawing.Size(398, 358);
            this.gbxSendControlMessage.TabIndex = 95;
            this.gbxSendControlMessage.TabStop = false;
            this.gbxSendControlMessage.Text = "Control Message";
            // 
            // cbxSequencer
            // 
            this.cbxSequencer.AutoSize = true;
            this.cbxSequencer.Location = new System.Drawing.Point(244, 292);
            this.cbxSequencer.Name = "cbxSequencer";
            this.cbxSequencer.Size = new System.Drawing.Size(78, 17);
            this.cbxSequencer.TabIndex = 102;
            this.cbxSequencer.Text = "Sequencer";
            this.cbxSequencer.UseVisualStyleBackColor = true;
            // 
            // rbtnCommon
            // 
            this.rbtnCommon.AutoSize = true;
            this.rbtnCommon.Checked = true;
            this.rbtnCommon.Location = new System.Drawing.Point(21, 277);
            this.rbtnCommon.Name = "rbtnCommon";
            this.rbtnCommon.Size = new System.Drawing.Size(132, 17);
            this.rbtnCommon.TabIndex = 101;
            this.rbtnCommon.TabStop = true;
            this.rbtnCommon.Text = "KeyLedMatrixMessage";
            this.rbtnCommon.UseVisualStyleBackColor = true;
            // 
            // rbtnRear
            // 
            this.rbtnRear.AutoSize = true;
            this.rbtnRear.Location = new System.Drawing.Point(21, 324);
            this.rbtnRear.Name = "rbtnRear";
            this.rbtnRear.Size = new System.Drawing.Size(155, 17);
            this.rbtnRear.TabIndex = 100;
            this.rbtnRear.Text = "KeyLedMatrixMessageRear";
            this.rbtnRear.UseVisualStyleBackColor = true;
            // 
            // rbtnFront
            // 
            this.rbtnFront.AutoSize = true;
            this.rbtnFront.Location = new System.Drawing.Point(21, 300);
            this.rbtnFront.Name = "rbtnFront";
            this.rbtnFront.Size = new System.Drawing.Size(156, 17);
            this.rbtnFront.TabIndex = 99;
            this.rbtnFront.Text = "KeyLedMatrixMessageFront";
            this.rbtnFront.UseVisualStyleBackColor = true;
            // 
            // tbxValueHex
            // 
            this.tbxValueHex.Location = new System.Drawing.Point(6, 112);
            this.tbxValueHex.Name = "tbxValueHex";
            this.tbxValueHex.Size = new System.Drawing.Size(100, 20);
            this.tbxValueHex.TabIndex = 98;
            // 
            // tbxValue
            // 
            this.tbxValue.Location = new System.Drawing.Point(7, 86);
            this.tbxValue.Name = "tbxValue";
            this.tbxValue.Size = new System.Drawing.Size(100, 20);
            this.tbxValue.TabIndex = 97;
            // 
            // btnCalcAndShow
            // 
            this.btnCalcAndShow.Location = new System.Drawing.Point(6, 57);
            this.btnCalcAndShow.Name = "btnCalcAndShow";
            this.btnCalcAndShow.Size = new System.Drawing.Size(101, 23);
            this.btnCalcAndShow.TabIndex = 96;
            this.btnCalcAndShow.Text = "Calculate";
            this.btnCalcAndShow.UseVisualStyleBackColor = true;
            this.btnCalcAndShow.Click += new System.EventHandler(this.btnCalcAndShow_Click);
            // 
            // btnBuildPatternFile
            // 
            this.btnBuildPatternFile.Location = new System.Drawing.Point(9, 348);
            this.btnBuildPatternFile.Name = "btnBuildPatternFile";
            this.btnBuildPatternFile.Size = new System.Drawing.Size(110, 23);
            this.btnBuildPatternFile.TabIndex = 96;
            this.btnBuildPatternFile.Text = "Build Pattern File";
            this.btnBuildPatternFile.UseVisualStyleBackColor = true;
            this.btnBuildPatternFile.Click += new System.EventHandler(this.btnBuildPatternFile_Click);
            // 
            // tbxExpressionCollectionFileName
            // 
            this.tbxExpressionCollectionFileName.Location = new System.Drawing.Point(125, 350);
            this.tbxExpressionCollectionFileName.Name = "tbxExpressionCollectionFileName";
            this.tbxExpressionCollectionFileName.Size = new System.Drawing.Size(390, 20);
            this.tbxExpressionCollectionFileName.TabIndex = 97;
            // 
            // btnFindPatternFile
            // 
            this.btnFindPatternFile.Location = new System.Drawing.Point(521, 348);
            this.btnFindPatternFile.Name = "btnFindPatternFile";
            this.btnFindPatternFile.Size = new System.Drawing.Size(75, 23);
            this.btnFindPatternFile.TabIndex = 98;
            this.btnFindPatternFile.Text = "Find";
            this.btnFindPatternFile.UseVisualStyleBackColor = true;
            this.btnFindPatternFile.Click += new System.EventHandler(this.btnFindPatternFile_Click);
            // 
            // openFileDialogExpression
            // 
            this.openFileDialogExpression.Filter = "Expression Collection File|*.ec";
            // 
            // cbxScrollStartOnScreen
            // 
            this.cbxScrollStartOnScreen.AutoSize = true;
            this.cbxScrollStartOnScreen.Location = new System.Drawing.Point(244, 201);
            this.cbxScrollStartOnScreen.Name = "cbxScrollStartOnScreen";
            this.cbxScrollStartOnScreen.Size = new System.Drawing.Size(131, 17);
            this.cbxScrollStartOnScreen.TabIndex = 103;
            this.cbxScrollStartOnScreen.Text = "Scroll Start On-Screen";
            this.cbxScrollStartOnScreen.UseVisualStyleBackColor = true;
            // 
            // cbbJustification
            // 
            this.cbbJustification.FormattingEnabled = true;
            this.cbbJustification.Items.AddRange(new object[] {
            "Center",
            "Left",
            "Right"});
            this.cbbJustification.Location = new System.Drawing.Point(151, 165);
            this.cbbJustification.Name = "cbbJustification";
            this.cbbJustification.Size = new System.Drawing.Size(106, 21);
            this.cbbJustification.TabIndex = 104;
            this.cbbJustification.Text = "Center";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(263, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 105;
            this.label6.Text = "Justification";
            // 
            // ntbIntensity
            // 
            this.ntbIntensity.AcceptHexInput = true;
            this.ntbIntensity.HighlightInputError = true;
            this.ntbIntensity.Location = new System.Drawing.Point(201, 111);
            this.ntbIntensity.Name = "ntbIntensity";
            this.ntbIntensity.Size = new System.Drawing.Size(56, 20);
            this.ntbIntensity.TabIndex = 5;
            // 
            // ntbSpeedPeriod
            // 
            this.ntbSpeedPeriod.AcceptHexInput = true;
            this.ntbSpeedPeriod.HighlightInputError = true;
            this.ntbSpeedPeriod.Location = new System.Drawing.Point(201, 85);
            this.ntbSpeedPeriod.Name = "ntbSpeedPeriod";
            this.ntbSpeedPeriod.Size = new System.Drawing.Size(56, 20);
            this.ntbSpeedPeriod.TabIndex = 4;
            // 
            // ntbEnum
            // 
            this.ntbEnum.AcceptHexInput = true;
            this.ntbEnum.HighlightInputError = true;
            this.ntbEnum.Location = new System.Drawing.Point(201, 59);
            this.ntbEnum.Name = "ntbEnum";
            this.ntbEnum.Size = new System.Drawing.Size(56, 20);
            this.ntbEnum.TabIndex = 3;
            // 
            // ucLedMatrixFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnFindPatternFile);
            this.Controls.Add(this.tbxExpressionCollectionFileName);
            this.Controls.Add(this.btnBuildPatternFile);
            this.Controls.Add(this.gbxSendControlMessage);
            this.Controls.Add(this.pbxTest);
            this.Controls.Add(this.btnBuild);
            this.Controls.Add(this.dgvMessages);
            this.Name = "ucLedMatrixFile";
            this.Size = new System.Drawing.Size(1080, 620);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTest)).EndInit();
            this.gbxSendControlMessage.ResumeLayout(false);
            this.gbxSendControlMessage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMessages;
        private System.Windows.Forms.Button btnBuild;
        private System.Windows.Forms.PictureBox pbxTest;
        private NumericTextBox ntbEnum;
        private NumericTextBox ntbSpeedPeriod;
        private NumericTextBox ntbIntensity;
        private System.Windows.Forms.ComboBox cbbMode;
        private System.Windows.Forms.CheckBox cbxScrollOnce;
        private System.Windows.Forms.CheckBox cbxMirror;
        private System.Windows.Forms.CheckBox cbxWrap;
        private System.Windows.Forms.Button bntShow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnHide;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmEnum;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmText;
        private System.Windows.Forms.ComboBox cbbOnlineDevices;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox gbxSendControlMessage;
        private System.Windows.Forms.Button btnCalcAndShow;
        private System.Windows.Forms.TextBox tbxValue;
        private System.Windows.Forms.Button btnBuildPatternFile;
        private System.Windows.Forms.TextBox tbxValueHex;
        private System.Windows.Forms.TextBox tbxExpressionCollectionFileName;
        private System.Windows.Forms.Button btnFindPatternFile;
        private System.Windows.Forms.OpenFileDialog openFileDialogExpression;
        private System.Windows.Forms.CheckBox cbxSequencer;
        private System.Windows.Forms.RadioButton rbtnCommon;
        private System.Windows.Forms.RadioButton rbtnRear;
        private System.Windows.Forms.RadioButton rbtnFront;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbbJustification;
        private System.Windows.Forms.CheckBox cbxScrollStartOnScreen;
    }
}
