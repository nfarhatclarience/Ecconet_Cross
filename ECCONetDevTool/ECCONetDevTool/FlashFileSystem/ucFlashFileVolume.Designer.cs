namespace ECCONetDevTool.FlashFileSystem
{
    partial class ucFlashFileVolume
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
            this.btnUpdateVolume = new System.Windows.Forms.Button();
            this.cbGCC = new System.Windows.Forms.CheckBox();
            this.lblUnusedMemory = new System.Windows.Forms.Label();
            this.btnExportTextFile = new System.Windows.Forms.Button();
            this.ntbFlashVolumeSize = new ECCONetDevTool.NumericTextBox();
            this.ntbFlashVolumeBaseAddress = new ECCONetDevTool.NumericTextBox();
            this.lvFlashFileSystem = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // btnUpdateVolume
            // 
            this.btnUpdateVolume.Location = new System.Drawing.Point(164, 14);
            this.btnUpdateVolume.Name = "btnUpdateVolume";
            this.btnUpdateVolume.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateVolume.TabIndex = 38;
            this.btnUpdateVolume.Text = "Update";
            this.btnUpdateVolume.UseVisualStyleBackColor = true;
            this.btnUpdateVolume.Click += new System.EventHandler(this.btnUpdateVolume_Click);
            // 
            // cbGCC
            // 
            this.cbGCC.AutoSize = true;
            this.cbGCC.Checked = true;
            this.cbGCC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGCC.Location = new System.Drawing.Point(380, 268);
            this.cbGCC.Name = "cbGCC";
            this.cbGCC.Size = new System.Drawing.Size(48, 17);
            this.cbGCC.TabIndex = 40;
            this.cbGCC.Text = "GCC";
            this.cbGCC.UseVisualStyleBackColor = true;
            this.cbGCC.CheckedChanged += new System.EventHandler(this.cbGCC_CheckedChanged);
            // 
            // lblUnusedMemory
            // 
            this.lblUnusedMemory.AutoSize = true;
            this.lblUnusedMemory.Location = new System.Drawing.Point(278, 19);
            this.lblUnusedMemory.Name = "lblUnusedMemory";
            this.lblUnusedMemory.Size = new System.Drawing.Size(47, 13);
            this.lblUnusedMemory.TabIndex = 39;
            this.lblUnusedMemory.Text = "Unused:";
            // 
            // btnExportTextFile
            // 
            this.btnExportTextFile.Location = new System.Drawing.Point(233, 264);
            this.btnExportTextFile.Name = "btnExportTextFile";
            this.btnExportTextFile.Size = new System.Drawing.Size(141, 23);
            this.btnExportTextFile.TabIndex = 37;
            this.btnExportTextFile.Text = "Export C Source File";
            this.btnExportTextFile.UseVisualStyleBackColor = true;
            this.btnExportTextFile.Click += new System.EventHandler(this.btnExportTextFile_Click);
            // 
            // ntbFlashVolumeSize
            // 
            this.ntbFlashVolumeSize.AcceptHexInput = true;
            this.ntbFlashVolumeSize.HighlightInputError = true;
            this.ntbFlashVolumeSize.Location = new System.Drawing.Point(85, 16);
            this.ntbFlashVolumeSize.Name = "ntbFlashVolumeSize";
            this.ntbFlashVolumeSize.Size = new System.Drawing.Size(69, 20);
            this.ntbFlashVolumeSize.TabIndex = 36;
            this.ntbFlashVolumeSize.Text = "0x2000";
            this.ntbFlashVolumeSize.TextChanged += new System.EventHandler(this.ntbFlashVolumeSize_TextChanged);
            // 
            // ntbFlashVolumeBaseAddress
            // 
            this.ntbFlashVolumeBaseAddress.AcceptHexInput = true;
            this.ntbFlashVolumeBaseAddress.HighlightInputError = true;
            this.ntbFlashVolumeBaseAddress.Location = new System.Drawing.Point(5, 16);
            this.ntbFlashVolumeBaseAddress.Name = "ntbFlashVolumeBaseAddress";
            this.ntbFlashVolumeBaseAddress.Size = new System.Drawing.Size(69, 20);
            this.ntbFlashVolumeBaseAddress.TabIndex = 35;
            this.ntbFlashVolumeBaseAddress.Text = "0x6000";
            this.ntbFlashVolumeBaseAddress.TextChanged += new System.EventHandler(this.ntbFlashVolumeBaseAddress_TextChanged);
            // 
            // lvFlashFileSystem
            // 
            this.lvFlashFileSystem.Location = new System.Drawing.Point(5, 42);
            this.lvFlashFileSystem.Name = "lvFlashFileSystem";
            this.lvFlashFileSystem.Size = new System.Drawing.Size(430, 215);
            this.lvFlashFileSystem.TabIndex = 33;
            this.lvFlashFileSystem.UseCompatibleStateImageBehavior = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(83, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 42;
            this.label2.Text = "Size";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 41;
            this.label1.Text = "Base Address";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileName = "file_system.c";
            this.saveFileDialog1.Filter = "C files|*.c";
            // 
            // ucFlashFileVolume
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnUpdateVolume);
            this.Controls.Add(this.cbGCC);
            this.Controls.Add(this.lblUnusedMemory);
            this.Controls.Add(this.btnExportTextFile);
            this.Controls.Add(this.ntbFlashVolumeSize);
            this.Controls.Add(this.ntbFlashVolumeBaseAddress);
            this.Controls.Add(this.lvFlashFileSystem);
            this.Name = "ucFlashFileVolume";
            this.Size = new System.Drawing.Size(440, 290);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUpdateVolume;
        private System.Windows.Forms.CheckBox cbGCC;
        private System.Windows.Forms.Label lblUnusedMemory;
        private System.Windows.Forms.Button btnExportTextFile;
        private NumericTextBox ntbFlashVolumeSize;
        private NumericTextBox ntbFlashVolumeBaseAddress;
        private System.Windows.Forms.ListView lvFlashFileSystem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}
