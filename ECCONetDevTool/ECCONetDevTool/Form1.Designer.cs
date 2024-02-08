namespace ECCONetDevTool
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageOnline = new System.Windows.Forms.TabPage();
            this.onlineMonitor = new ECCONetDevTool.OnlineMonitor();
            this.tabPageTokens = new System.Windows.Forms.TabPage();
            this.busMonitor = new ECCONetDevTool.BusMonitor();
            this.tabPageFTP = new System.Windows.Forms.TabPage();
            this.ftp = new ECCONetDevTool.FTP();
            this.tabPageSequencers = new System.Windows.Forms.TabPage();
            this.miscTools = new ECCONetDevTool.Misc();
            this.tabPagePatternGeneration = new System.Windows.Forms.TabPage();
            this.patternGeneration = new ECCONetDevTool.ExpressionEdit.ExpressionEditor();
            this.tabPageBytecode = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageDefault = new System.Windows.Forms.TabPage();
            this.equationsDefault = new ECCONetDevTool.Equations();
            this.tabPageProfile1 = new System.Windows.Forms.TabPage();
            this.equationsProfile1 = new ECCONetDevTool.Equations();
            this.tabPageProfile2 = new System.Windows.Forms.TabPage();
            this.equationsProfile2 = new ECCONetDevTool.Equations();
            this.tabPageProfile3 = new System.Windows.Forms.TabPage();
            this.equationsProfile3 = new ECCONetDevTool.Equations();
            this.tabPageProfile4 = new System.Windows.Forms.TabPage();
            this.equationsProfile4 = new ECCONetDevTool.Equations();
            this.tabPageProfile5 = new System.Windows.Forms.TabPage();
            this.equationsProfile5 = new ECCONetDevTool.Equations();
            this.tabPageProfile6 = new System.Windows.Forms.TabPage();
            this.equationsProfile6 = new ECCONetDevTool.Equations();
            this.tabPageFlashFileSystem = new System.Windows.Forms.TabPage();
            this.flashFileSystem = new ECCONetDevTool.FlashFileSystem.FlashFileSystem();
            this.tabPageBusStressTest = new System.Windows.Forms.TabPage();
            this.ucBusStressTester = new ECCONetDevTool.BusStressTest.ucBusStressTester();
            this.tabPageLedMatrix = new System.Windows.Forms.TabPage();
            this.ucLedMatrixFile = new ECCONetDevTool.LedMatrixFile.ucLedMatrixFile();
            this.tabPageHazCanFWUpdate = new System.Windows.Forms.TabPage();
            this.firmwwareUpdate = new ECCONetDevTool.ucHazCANFirmwareUpdate();
            this.tabPageECCONetFWUpdate = new System.Windows.Forms.TabPage();
            this.ucECCONetFirmwareUpdate = new ECCONetDevTool.ucECCONetFirmwareUpdate();
            this.tabControl.SuspendLayout();
            this.tabPageOnline.SuspendLayout();
            this.tabPageTokens.SuspendLayout();
            this.tabPageFTP.SuspendLayout();
            this.tabPageSequencers.SuspendLayout();
            this.tabPagePatternGeneration.SuspendLayout();
            this.tabPageBytecode.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageDefault.SuspendLayout();
            this.tabPageProfile1.SuspendLayout();
            this.tabPageProfile2.SuspendLayout();
            this.tabPageProfile3.SuspendLayout();
            this.tabPageProfile4.SuspendLayout();
            this.tabPageProfile5.SuspendLayout();
            this.tabPageProfile6.SuspendLayout();
            this.tabPageFlashFileSystem.SuspendLayout();
            this.tabPageBusStressTest.SuspendLayout();
            this.tabPageLedMatrix.SuspendLayout();
            this.tabPageHazCanFWUpdate.SuspendLayout();
            this.tabPageECCONetFWUpdate.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageOnline);
            this.tabControl.Controls.Add(this.tabPageTokens);
            this.tabControl.Controls.Add(this.tabPageFTP);
            this.tabControl.Controls.Add(this.tabPageSequencers);
            this.tabControl.Controls.Add(this.tabPagePatternGeneration);
            this.tabControl.Controls.Add(this.tabPageBytecode);
            this.tabControl.Controls.Add(this.tabPageFlashFileSystem);
            this.tabControl.Controls.Add(this.tabPageBusStressTest);
            this.tabControl.Controls.Add(this.tabPageLedMatrix);
            this.tabControl.Controls.Add(this.tabPageHazCanFWUpdate);
            this.tabControl.Controls.Add(this.tabPageECCONetFWUpdate);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1400, 660);
            this.tabControl.TabIndex = 29;
            // 
            // tabPageOnline
            // 
            this.tabPageOnline.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageOnline.Controls.Add(this.onlineMonitor);
            this.tabPageOnline.Location = new System.Drawing.Point(4, 22);
            this.tabPageOnline.Name = "tabPageOnline";
            this.tabPageOnline.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOnline.Size = new System.Drawing.Size(1392, 634);
            this.tabPageOnline.TabIndex = 0;
            this.tabPageOnline.Text = "Device Monitor";
            // 
            // onlineMonitor
            // 
            this.onlineMonitor.Location = new System.Drawing.Point(0, 0);
            this.onlineMonitor.Name = "onlineMonitor";
            this.onlineMonitor.Size = new System.Drawing.Size(678, 610);
            this.onlineMonitor.TabIndex = 0;
            // 
            // tabPageTokens
            // 
            this.tabPageTokens.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageTokens.Controls.Add(this.busMonitor);
            this.tabPageTokens.Location = new System.Drawing.Point(4, 22);
            this.tabPageTokens.Name = "tabPageTokens";
            this.tabPageTokens.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTokens.Size = new System.Drawing.Size(1392, 634);
            this.tabPageTokens.TabIndex = 3;
            this.tabPageTokens.Text = "Token Monitor";
            // 
            // busMonitor
            // 
            this.busMonitor.Location = new System.Drawing.Point(7, 7);
            this.busMonitor.Name = "busMonitor";
            this.busMonitor.Size = new System.Drawing.Size(1225, 620);
            this.busMonitor.TabIndex = 0;
            // 
            // tabPageFTP
            // 
            this.tabPageFTP.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageFTP.Controls.Add(this.ftp);
            this.tabPageFTP.Location = new System.Drawing.Point(4, 22);
            this.tabPageFTP.Name = "tabPageFTP";
            this.tabPageFTP.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFTP.Size = new System.Drawing.Size(1392, 634);
            this.tabPageFTP.TabIndex = 1;
            this.tabPageFTP.Text = "File Transfer";
            // 
            // ftp
            // 
            this.ftp.Location = new System.Drawing.Point(6, 6);
            this.ftp.Name = "ftp";
            this.ftp.Size = new System.Drawing.Size(850, 620);
            this.ftp.TabIndex = 0;
            // 
            // tabPageSequencers
            // 
            this.tabPageSequencers.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageSequencers.Controls.Add(this.miscTools);
            this.tabPageSequencers.Location = new System.Drawing.Point(4, 22);
            this.tabPageSequencers.Name = "tabPageSequencers";
            this.tabPageSequencers.Size = new System.Drawing.Size(1392, 634);
            this.tabPageSequencers.TabIndex = 2;
            this.tabPageSequencers.Text = "Sequencers";
            // 
            // miscTools
            // 
            this.miscTools.canInterface = null;
            this.miscTools.Location = new System.Drawing.Point(6, 7);
            this.miscTools.Name = "miscTools";
            this.miscTools.onlineDevices = null;
            this.miscTools.Size = new System.Drawing.Size(1080, 620);
            this.miscTools.TabIndex = 0;
            // 
            // tabPagePatternGeneration
            // 
            this.tabPagePatternGeneration.BackColor = System.Drawing.SystemColors.Control;
            this.tabPagePatternGeneration.Controls.Add(this.patternGeneration);
            this.tabPagePatternGeneration.Location = new System.Drawing.Point(4, 22);
            this.tabPagePatternGeneration.Name = "tabPagePatternGeneration";
            this.tabPagePatternGeneration.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePatternGeneration.Size = new System.Drawing.Size(1392, 634);
            this.tabPagePatternGeneration.TabIndex = 5;
            this.tabPagePatternGeneration.Text = "Pattern Generation";
            // 
            // patternGeneration
            // 
            this.patternGeneration.Location = new System.Drawing.Point(6, 7);
            this.patternGeneration.Name = "patternGeneration";
            this.patternGeneration.Size = new System.Drawing.Size(1380, 620);
            this.patternGeneration.TabIndex = 0;
            // 
            // tabPageBytecode
            // 
            this.tabPageBytecode.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPageBytecode.Controls.Add(this.tabControl1);
            this.tabPageBytecode.Location = new System.Drawing.Point(4, 22);
            this.tabPageBytecode.Name = "tabPageBytecode";
            this.tabPageBytecode.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBytecode.Size = new System.Drawing.Size(1392, 634);
            this.tabPageBytecode.TabIndex = 7;
            this.tabPageBytecode.Text = "Bytecode Generation";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageDefault);
            this.tabControl1.Controls.Add(this.tabPageProfile1);
            this.tabControl1.Controls.Add(this.tabPageProfile2);
            this.tabControl1.Controls.Add(this.tabPageProfile3);
            this.tabControl1.Controls.Add(this.tabPageProfile4);
            this.tabControl1.Controls.Add(this.tabPageProfile5);
            this.tabControl1.Controls.Add(this.tabPageProfile6);
            this.tabControl1.Location = new System.Drawing.Point(7, 7);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1081, 620);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageDefault
            // 
            this.tabPageDefault.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPageDefault.Controls.Add(this.equationsDefault);
            this.tabPageDefault.Location = new System.Drawing.Point(4, 22);
            this.tabPageDefault.Name = "tabPageDefault";
            this.tabPageDefault.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDefault.Size = new System.Drawing.Size(1073, 594);
            this.tabPageDefault.TabIndex = 0;
            this.tabPageDefault.Text = "Default";
            // 
            // equationsDefault
            // 
            this.equationsDefault.Location = new System.Drawing.Point(7, 7);
            this.equationsDefault.Name = "equationsDefault";
            this.equationsDefault.Size = new System.Drawing.Size(1060, 580);
            this.equationsDefault.TabIndex = 0;
            this.equationsDefault.UserProfileIndex = 0;
            // 
            // tabPageProfile1
            // 
            this.tabPageProfile1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPageProfile1.Controls.Add(this.equationsProfile1);
            this.tabPageProfile1.Location = new System.Drawing.Point(4, 22);
            this.tabPageProfile1.Name = "tabPageProfile1";
            this.tabPageProfile1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProfile1.Size = new System.Drawing.Size(1073, 594);
            this.tabPageProfile1.TabIndex = 1;
            this.tabPageProfile1.Text = "Profile 1";
            // 
            // equationsProfile1
            // 
            this.equationsProfile1.Location = new System.Drawing.Point(7, 7);
            this.equationsProfile1.Name = "equationsProfile1";
            this.equationsProfile1.Size = new System.Drawing.Size(1060, 465);
            this.equationsProfile1.TabIndex = 0;
            this.equationsProfile1.UserProfileIndex = 0;
            // 
            // tabPageProfile2
            // 
            this.tabPageProfile2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPageProfile2.Controls.Add(this.equationsProfile2);
            this.tabPageProfile2.Location = new System.Drawing.Point(4, 22);
            this.tabPageProfile2.Name = "tabPageProfile2";
            this.tabPageProfile2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProfile2.Size = new System.Drawing.Size(1073, 594);
            this.tabPageProfile2.TabIndex = 2;
            this.tabPageProfile2.Text = "Profile 2";
            // 
            // equationsProfile2
            // 
            this.equationsProfile2.Location = new System.Drawing.Point(7, 7);
            this.equationsProfile2.Name = "equationsProfile2";
            this.equationsProfile2.Size = new System.Drawing.Size(1060, 465);
            this.equationsProfile2.TabIndex = 0;
            this.equationsProfile2.UserProfileIndex = 0;
            // 
            // tabPageProfile3
            // 
            this.tabPageProfile3.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPageProfile3.Controls.Add(this.equationsProfile3);
            this.tabPageProfile3.Location = new System.Drawing.Point(4, 22);
            this.tabPageProfile3.Name = "tabPageProfile3";
            this.tabPageProfile3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProfile3.Size = new System.Drawing.Size(1073, 594);
            this.tabPageProfile3.TabIndex = 3;
            this.tabPageProfile3.Text = "Profile 3";
            // 
            // equationsProfile3
            // 
            this.equationsProfile3.Location = new System.Drawing.Point(7, 7);
            this.equationsProfile3.Name = "equationsProfile3";
            this.equationsProfile3.Size = new System.Drawing.Size(1060, 465);
            this.equationsProfile3.TabIndex = 0;
            this.equationsProfile3.UserProfileIndex = 0;
            // 
            // tabPageProfile4
            // 
            this.tabPageProfile4.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPageProfile4.Controls.Add(this.equationsProfile4);
            this.tabPageProfile4.Location = new System.Drawing.Point(4, 22);
            this.tabPageProfile4.Name = "tabPageProfile4";
            this.tabPageProfile4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProfile4.Size = new System.Drawing.Size(1073, 594);
            this.tabPageProfile4.TabIndex = 4;
            this.tabPageProfile4.Text = "Profile 4";
            // 
            // equationsProfile4
            // 
            this.equationsProfile4.Location = new System.Drawing.Point(7, 7);
            this.equationsProfile4.Name = "equationsProfile4";
            this.equationsProfile4.Size = new System.Drawing.Size(1060, 465);
            this.equationsProfile4.TabIndex = 0;
            this.equationsProfile4.UserProfileIndex = 0;
            // 
            // tabPageProfile5
            // 
            this.tabPageProfile5.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPageProfile5.Controls.Add(this.equationsProfile5);
            this.tabPageProfile5.Location = new System.Drawing.Point(4, 22);
            this.tabPageProfile5.Name = "tabPageProfile5";
            this.tabPageProfile5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProfile5.Size = new System.Drawing.Size(1073, 594);
            this.tabPageProfile5.TabIndex = 5;
            this.tabPageProfile5.Text = "Profile 5";
            // 
            // equationsProfile5
            // 
            this.equationsProfile5.Location = new System.Drawing.Point(7, 7);
            this.equationsProfile5.Name = "equationsProfile5";
            this.equationsProfile5.Size = new System.Drawing.Size(1060, 465);
            this.equationsProfile5.TabIndex = 0;
            this.equationsProfile5.UserProfileIndex = 0;
            // 
            // tabPageProfile6
            // 
            this.tabPageProfile6.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPageProfile6.Controls.Add(this.equationsProfile6);
            this.tabPageProfile6.Location = new System.Drawing.Point(4, 22);
            this.tabPageProfile6.Name = "tabPageProfile6";
            this.tabPageProfile6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProfile6.Size = new System.Drawing.Size(1073, 594);
            this.tabPageProfile6.TabIndex = 6;
            this.tabPageProfile6.Text = "Profile 6";
            // 
            // equationsProfile6
            // 
            this.equationsProfile6.Location = new System.Drawing.Point(7, 7);
            this.equationsProfile6.Name = "equationsProfile6";
            this.equationsProfile6.Size = new System.Drawing.Size(1060, 465);
            this.equationsProfile6.TabIndex = 0;
            this.equationsProfile6.UserProfileIndex = 0;
            // 
            // tabPageFlashFileSystem
            // 
            this.tabPageFlashFileSystem.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPageFlashFileSystem.Controls.Add(this.flashFileSystem);
            this.tabPageFlashFileSystem.Location = new System.Drawing.Point(4, 22);
            this.tabPageFlashFileSystem.Name = "tabPageFlashFileSystem";
            this.tabPageFlashFileSystem.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFlashFileSystem.Size = new System.Drawing.Size(1392, 634);
            this.tabPageFlashFileSystem.TabIndex = 6;
            this.tabPageFlashFileSystem.Text = "Flash File System";
            // 
            // flashFileSystem
            // 
            this.flashFileSystem.Location = new System.Drawing.Point(7, 7);
            this.flashFileSystem.Name = "flashFileSystem";
            this.flashFileSystem.Size = new System.Drawing.Size(1078, 620);
            this.flashFileSystem.TabIndex = 0;
            // 
            // tabPageBusStressTest
            // 
            this.tabPageBusStressTest.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPageBusStressTest.Controls.Add(this.ucBusStressTester);
            this.tabPageBusStressTest.Location = new System.Drawing.Point(4, 22);
            this.tabPageBusStressTest.Name = "tabPageBusStressTest";
            this.tabPageBusStressTest.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBusStressTest.Size = new System.Drawing.Size(1392, 634);
            this.tabPageBusStressTest.TabIndex = 8;
            this.tabPageBusStressTest.Text = "Bus Stress Test";
            // 
            // ucBusStressTester
            // 
            this.ucBusStressTester.Location = new System.Drawing.Point(7, 7);
            this.ucBusStressTester.Name = "ucBusStressTester";
            this.ucBusStressTester.Size = new System.Drawing.Size(1080, 620);
            this.ucBusStressTester.TabIndex = 0;
            // 
            // tabPageLedMatrix
            // 
            this.tabPageLedMatrix.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPageLedMatrix.Controls.Add(this.ucLedMatrixFile);
            this.tabPageLedMatrix.Location = new System.Drawing.Point(4, 22);
            this.tabPageLedMatrix.Name = "tabPageLedMatrix";
            this.tabPageLedMatrix.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLedMatrix.Size = new System.Drawing.Size(1392, 634);
            this.tabPageLedMatrix.TabIndex = 10;
            this.tabPageLedMatrix.Text = "LED Matrix";
            // 
            // ucLedMatrixFile
            // 
            this.ucLedMatrixFile.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ucLedMatrixFile.Location = new System.Drawing.Point(7, 7);
            this.ucLedMatrixFile.Name = "ucLedMatrixFile";
            this.ucLedMatrixFile.Size = new System.Drawing.Size(1080, 620);
            this.ucLedMatrixFile.TabIndex = 0;
            // 
            // tabPageHazCanFWUpdate
            // 
            this.tabPageHazCanFWUpdate.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageHazCanFWUpdate.Controls.Add(this.firmwwareUpdate);
            this.tabPageHazCanFWUpdate.Location = new System.Drawing.Point(4, 22);
            this.tabPageHazCanFWUpdate.Name = "tabPageHazCanFWUpdate";
            this.tabPageHazCanFWUpdate.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHazCanFWUpdate.Size = new System.Drawing.Size(1392, 634);
            this.tabPageHazCanFWUpdate.TabIndex = 4;
            this.tabPageHazCanFWUpdate.Text = "HazCAN Firmware Update";
            // 
            // firmwwareUpdate
            // 
            this.firmwwareUpdate.Location = new System.Drawing.Point(7, 7);
            this.firmwwareUpdate.Name = "firmwwareUpdate";
            this.firmwwareUpdate.Size = new System.Drawing.Size(1088, 620);
            this.firmwwareUpdate.TabIndex = 0;
            // 
            // tabPageECCONetFWUpdate
            // 
            this.tabPageECCONetFWUpdate.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPageECCONetFWUpdate.Controls.Add(this.ucECCONetFirmwareUpdate);
            this.tabPageECCONetFWUpdate.Location = new System.Drawing.Point(4, 22);
            this.tabPageECCONetFWUpdate.Name = "tabPageECCONetFWUpdate";
            this.tabPageECCONetFWUpdate.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageECCONetFWUpdate.Size = new System.Drawing.Size(1392, 634);
            this.tabPageECCONetFWUpdate.TabIndex = 9;
            this.tabPageECCONetFWUpdate.Text = "ECCONet Firmware Update";
            // 
            // ucECCONetFirmwareUpdate
            // 
            this.ucECCONetFirmwareUpdate.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ucECCONetFirmwareUpdate.Location = new System.Drawing.Point(7, 7);
            this.ucECCONetFirmwareUpdate.Name = "ucECCONetFirmwareUpdate";
            this.ucECCONetFirmwareUpdate.Size = new System.Drawing.Size(873, 620);
            this.ucECCONetFirmwareUpdate.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1424, 681);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "ECCONet Dev Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.tabControl.ResumeLayout(false);
            this.tabPageOnline.ResumeLayout(false);
            this.tabPageTokens.ResumeLayout(false);
            this.tabPageFTP.ResumeLayout(false);
            this.tabPageSequencers.ResumeLayout(false);
            this.tabPagePatternGeneration.ResumeLayout(false);
            this.tabPageBytecode.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageDefault.ResumeLayout(false);
            this.tabPageProfile1.ResumeLayout(false);
            this.tabPageProfile2.ResumeLayout(false);
            this.tabPageProfile3.ResumeLayout(false);
            this.tabPageProfile4.ResumeLayout(false);
            this.tabPageProfile5.ResumeLayout(false);
            this.tabPageProfile6.ResumeLayout(false);
            this.tabPageFlashFileSystem.ResumeLayout(false);
            this.tabPageBusStressTest.ResumeLayout(false);
            this.tabPageLedMatrix.ResumeLayout(false);
            this.tabPageHazCanFWUpdate.ResumeLayout(false);
            this.tabPageECCONetFWUpdate.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageOnline;
        private System.Windows.Forms.TabPage tabPageFTP;
        private System.Windows.Forms.TabPage tabPageSequencers;
        private FTP ftp;
        private OnlineMonitor onlineMonitor;
        private Misc miscTools;
        private System.Windows.Forms.TabPage tabPageTokens;
        private BusMonitor busMonitor;
        private System.Windows.Forms.TabPage tabPageHazCanFWUpdate;
        private ucHazCANFirmwareUpdate firmwwareUpdate;
        private System.Windows.Forms.TabPage tabPagePatternGeneration;
        private ExpressionEdit.ExpressionEditor patternGeneration;
        private System.Windows.Forms.TabPage tabPageFlashFileSystem;
        private FlashFileSystem.FlashFileSystem flashFileSystem;
        private System.Windows.Forms.TabPage tabPageBytecode;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageDefault;
        private System.Windows.Forms.TabPage tabPageProfile1;
        private Equations equationsDefault;
        private Equations equationsProfile1;
        private System.Windows.Forms.TabPage tabPageProfile2;
        private Equations equationsProfile2;
        private System.Windows.Forms.TabPage tabPageProfile3;
        private Equations equationsProfile3;
        private System.Windows.Forms.TabPage tabPageProfile4;
        private Equations equationsProfile4;
        private System.Windows.Forms.TabPage tabPageProfile5;
        private Equations equationsProfile5;
        private System.Windows.Forms.TabPage tabPageProfile6;
        private Equations equationsProfile6;
        private System.Windows.Forms.TabPage tabPageBusStressTest;
        private BusStressTest.ucBusStressTester ucBusStressTester;
        private System.Windows.Forms.TabPage tabPageECCONetFWUpdate;
        private System.Windows.Forms.TabPage tabPageLedMatrix;
        private LedMatrixFile.ucLedMatrixFile ucLedMatrixFile;
        private ucECCONetFirmwareUpdate ucECCONetFirmwareUpdate;
    }
}

