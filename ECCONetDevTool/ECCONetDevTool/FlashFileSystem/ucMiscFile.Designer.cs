namespace ECCONetDevTool.FlashFileSystem
{
    partial class ucMiscFile
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
            this.btnFind = new System.Windows.Forms.Button();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.btnTimeLogicUpdateFile = new System.Windows.Forms.Button();
            this.cbIncludeInFlashFileSystem = new System.Windows.Forms.CheckBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.cbbVolume = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbbFileType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbbFileMode = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ntbReservedSpace = new ECCONetDevTool.NumericTextBox();
            this.SuspendLayout();
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(350, 7);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(56, 23);
            this.btnFind.TabIndex = 44;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(6, 9);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(338, 20);
            this.tbFileName.TabIndex = 43;
            this.tbFileName.Text = "myfile.bin";
            this.tbFileName.TextChanged += new System.EventHandler(this.tbFileName_TextChanged);
            // 
            // btnTimeLogicUpdateFile
            // 
            this.btnTimeLogicUpdateFile.Location = new System.Drawing.Point(350, 97);
            this.btnTimeLogicUpdateFile.Name = "btnTimeLogicUpdateFile";
            this.btnTimeLogicUpdateFile.Size = new System.Drawing.Size(56, 23);
            this.btnTimeLogicUpdateFile.TabIndex = 41;
            this.btnTimeLogicUpdateFile.Text = "Update";
            this.btnTimeLogicUpdateFile.UseVisualStyleBackColor = true;
            // 
            // cbIncludeInFlashFileSystem
            // 
            this.cbIncludeInFlashFileSystem.AutoSize = true;
            this.cbIncludeInFlashFileSystem.Location = new System.Drawing.Point(187, 101);
            this.cbIncludeInFlashFileSystem.Name = "cbIncludeInFlashFileSystem";
            this.cbIncludeInFlashFileSystem.Size = new System.Drawing.Size(157, 17);
            this.cbIncludeInFlashFileSystem.TabIndex = 40;
            this.cbIncludeInFlashFileSystem.Text = "Include In Flash File System";
            this.cbIncludeInFlashFileSystem.UseVisualStyleBackColor = true;
            this.cbIncludeInFlashFileSystem.CheckedChanged += new System.EventHandler(this.cbIncludeInFlashFileSystem_CheckedChanged);
            // 
            // cbbVolume
            // 
            this.cbbVolume.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbVolume.FormattingEnabled = true;
            this.cbbVolume.Items.AddRange(new object[] {
            "0",
            "1",
            "2"});
            this.cbbVolume.Location = new System.Drawing.Point(59, 99);
            this.cbbVolume.Name = "cbbVolume";
            this.cbbVolume.Size = new System.Drawing.Size(113, 21);
            this.cbbVolume.TabIndex = 47;
            this.cbbVolume.SelectedIndexChanged += new System.EventHandler(this.cbbVolume_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 48;
            this.label1.Text = "Volume";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 52;
            this.label2.Text = "File Type";
            // 
            // cbbFileType
            // 
            this.cbbFileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbFileType.FormattingEnabled = true;
            this.cbbFileType.Items.AddRange(new object[] {
            "Binary",
            "Text"});
            this.cbbFileType.Location = new System.Drawing.Point(59, 72);
            this.cbbFileType.Name = "cbbFileType";
            this.cbbFileType.Size = new System.Drawing.Size(113, 21);
            this.cbbFileType.TabIndex = 51;
            this.cbbFileType.SelectedIndexChanged += new System.EventHandler(this.cbbFileType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 54;
            this.label3.Text = "Mode";
            // 
            // cbbFileMode
            // 
            this.cbbFileMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbFileMode.FormattingEnabled = true;
            this.cbbFileMode.Items.AddRange(new object[] {
            "Imported File",
            "Reserved Space"});
            this.cbbFileMode.Location = new System.Drawing.Point(59, 45);
            this.cbbFileMode.Name = "cbbFileMode";
            this.cbbFileMode.Size = new System.Drawing.Size(113, 21);
            this.cbbFileMode.TabIndex = 53;
            this.cbbFileMode.SelectedIndexChanged += new System.EventHandler(this.cbbFileMode_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(202, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 55;
            this.label4.Text = "Reserved Space";
            // 
            // ntbReservedSpace
            // 
            this.ntbReservedSpace.AcceptHexInput = true;
            this.ntbReservedSpace.HighlightInputError = true;
            this.ntbReservedSpace.Location = new System.Drawing.Point(292, 45);
            this.ntbReservedSpace.Name = "ntbReservedSpace";
            this.ntbReservedSpace.Size = new System.Drawing.Size(52, 20);
            this.ntbReservedSpace.TabIndex = 50;
            this.ntbReservedSpace.TextChanged += new System.EventHandler(this.ntbReservedSpace_TextChanged);
            // 
            // ucMiscFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbbFileMode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbbFileType);
            this.Controls.Add(this.ntbReservedSpace);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbbVolume);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.tbFileName);
            this.Controls.Add(this.btnTimeLogicUpdateFile);
            this.Controls.Add(this.cbIncludeInFlashFileSystem);
            this.Name = "ucMiscFile";
            this.Size = new System.Drawing.Size(410, 125);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.TextBox tbFileName;
        private System.Windows.Forms.Button btnTimeLogicUpdateFile;
        private System.Windows.Forms.CheckBox cbIncludeInFlashFileSystem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ComboBox cbbVolume;
        private System.Windows.Forms.Label label1;
        private NumericTextBox ntbReservedSpace;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbbFileType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbbFileMode;
        private System.Windows.Forms.Label label4;
    }
}
