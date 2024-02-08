namespace ECCONetDevTool.FlashFileSystem
{
    partial class ucProductAssemblyFile
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
            this.lblImportFile = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btnFind = new System.Windows.Forms.Button();
            this.tbAssemblyImport = new System.Windows.Forms.TextBox();
            this.btnTimeLogicUpdateFile = new System.Windows.Forms.Button();
            this.cbIncludeTimeLogicFile = new System.Windows.Forms.CheckBox();
            this.ntbTimeLogicReservedSpace = new ECCONetDevTool.NumericTextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // lblImportFile
            // 
            this.lblImportFile.AutoSize = true;
            this.lblImportFile.Location = new System.Drawing.Point(0, 79);
            this.lblImportFile.Name = "lblImportFile";
            this.lblImportFile.Size = new System.Drawing.Size(55, 13);
            this.lblImportFile.TabIndex = 37;
            this.lblImportFile.Text = "Import File";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(0, 49);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(87, 13);
            this.label12.TabIndex = 35;
            this.label12.Text = "Reserved Space";
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(271, 44);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(45, 23);
            this.btnFind.TabIndex = 39;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // tbTimeLogicImport
            // 
            this.tbAssemblyImport.Location = new System.Drawing.Point(107, 76);
            this.tbAssemblyImport.Name = "tbTimeLogicImport";
            this.tbAssemblyImport.Size = new System.Drawing.Size(209, 20);
            this.tbAssemblyImport.TabIndex = 38;
            this.tbAssemblyImport.TextChanged += new System.EventHandler(this.tbTimeLogicImport_TextChanged);
            // 
            // btnTimeLogicUpdateFile
            // 
            this.btnTimeLogicUpdateFile.Location = new System.Drawing.Point(107, 6);
            this.btnTimeLogicUpdateFile.Name = "btnTimeLogicUpdateFile";
            this.btnTimeLogicUpdateFile.Size = new System.Drawing.Size(75, 23);
            this.btnTimeLogicUpdateFile.TabIndex = 36;
            this.btnTimeLogicUpdateFile.Text = "UpdateFile";
            this.btnTimeLogicUpdateFile.UseVisualStyleBackColor = true;
            this.btnTimeLogicUpdateFile.Click += new System.EventHandler(this.btnTimeLogicUpdateFile_Click);
            // 
            // cbIncludeTimeLogicFile
            // 
            this.cbIncludeTimeLogicFile.AutoSize = true;
            this.cbIncludeTimeLogicFile.Location = new System.Drawing.Point(3, 10);
            this.cbIncludeTimeLogicFile.Name = "cbIncludeTimeLogicFile";
            this.cbIncludeTimeLogicFile.Size = new System.Drawing.Size(97, 17);
            this.cbIncludeTimeLogicFile.TabIndex = 34;
            this.cbIncludeTimeLogicFile.Text = "Include In Files";
            this.cbIncludeTimeLogicFile.UseVisualStyleBackColor = true;
            this.cbIncludeTimeLogicFile.CheckedChanged += new System.EventHandler(this.cbIncludeTimeLogicFile_CheckedChanged);
            // 
            // ntbTimeLogicReservedSpace
            // 
            this.ntbTimeLogicReservedSpace.AcceptHexInput = true;
            this.ntbTimeLogicReservedSpace.HighlightInputError = true;
            this.ntbTimeLogicReservedSpace.Location = new System.Drawing.Point(107, 46);
            this.ntbTimeLogicReservedSpace.Name = "ntbTimeLogicReservedSpace";
            this.ntbTimeLogicReservedSpace.Size = new System.Drawing.Size(57, 20);
            this.ntbTimeLogicReservedSpace.TabIndex = 33;
            this.ntbTimeLogicReservedSpace.Text = "0";
            this.ntbTimeLogicReservedSpace.TextChanged += new System.EventHandler(this.ntbTimeLogicReservedSpace_TextChanged);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Product Assembly files|*.epa";
            // 
            // ucProductAssemblyFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblImportFile);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.tbAssemblyImport);
            this.Controls.Add(this.btnTimeLogicUpdateFile);
            this.Controls.Add(this.cbIncludeTimeLogicFile);
            this.Controls.Add(this.ntbTimeLogicReservedSpace);
            this.Name = "ucProductAssemblyFile";
            this.Size = new System.Drawing.Size(319, 107);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblImportFile;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.TextBox tbAssemblyImport;
        private System.Windows.Forms.Button btnTimeLogicUpdateFile;
        private System.Windows.Forms.CheckBox cbIncludeTimeLogicFile;
        private ECCONetDevTool.NumericTextBox ntbTimeLogicReservedSpace;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}
