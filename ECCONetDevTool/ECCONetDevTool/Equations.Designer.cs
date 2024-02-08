namespace ECCONetDevTool
{
    partial class Equations
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
            this.openHeaderFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.gbxEquationsFilePath = new System.Windows.Forms.GroupBox();
            this.btnSelectEquationsFile = new System.Windows.Forms.Button();
            this.lblEquationsFilePath = new System.Windows.Forms.Label();
            this.gbxBuild = new System.Windows.Forms.GroupBox();
            this.cbSaveToLocalFile = new System.Windows.Forms.CheckBox();
            this.cbIncludeInFlashFileSystem = new System.Windows.Forms.CheckBox();
            this.btnBuildEquationBinFiles = new System.Windows.Forms.Button();
            this.saveBytecodeFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.lvFilePaths = new System.Windows.Forms.ListView();
            this.btnAddFilePath = new System.Windows.Forms.Button();
            this.btnRemoveFilePath = new System.Windows.Forms.Button();
            this.btnFilePathEdit = new System.Windows.Forms.Button();
            this.lblFilePaths = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tbxResults = new System.Windows.Forms.RichTextBox();
            this.gbxEquationsFilePath.SuspendLayout();
            this.gbxBuild.SuspendLayout();
            this.SuspendLayout();
            // 
            // openHeaderFileDialog
            // 
            this.openHeaderFileDialog.FileName = "equations.h";
            this.openHeaderFileDialog.Filter = "Header files|*.h";
            // 
            // gbxEquationsFilePath
            // 
            this.gbxEquationsFilePath.Controls.Add(this.btnSelectEquationsFile);
            this.gbxEquationsFilePath.Controls.Add(this.lblEquationsFilePath);
            this.gbxEquationsFilePath.Location = new System.Drawing.Point(27, 216);
            this.gbxEquationsFilePath.Name = "gbxEquationsFilePath";
            this.gbxEquationsFilePath.Size = new System.Drawing.Size(392, 70);
            this.gbxEquationsFilePath.TabIndex = 4;
            this.gbxEquationsFilePath.TabStop = false;
            this.gbxEquationsFilePath.Text = "Equations File";
            // 
            // btnSelectEquationsFile
            // 
            this.btnSelectEquationsFile.Location = new System.Drawing.Point(9, 41);
            this.btnSelectEquationsFile.Name = "btnSelectEquationsFile";
            this.btnSelectEquationsFile.Size = new System.Drawing.Size(75, 23);
            this.btnSelectEquationsFile.TabIndex = 0;
            this.btnSelectEquationsFile.Text = "Select";
            this.btnSelectEquationsFile.UseVisualStyleBackColor = true;
            this.btnSelectEquationsFile.Click += new System.EventHandler(this.btnSelectEquationsFile_Click);
            // 
            // lblEquationsFilePath
            // 
            this.lblEquationsFilePath.AutoSize = true;
            this.lblEquationsFilePath.Location = new System.Drawing.Point(6, 20);
            this.lblEquationsFilePath.Name = "lblEquationsFilePath";
            this.lblEquationsFilePath.Size = new System.Drawing.Size(83, 13);
            this.lblEquationsFilePath.TabIndex = 1;
            this.lblEquationsFilePath.Text = "No file selected.";
            // 
            // gbxBuild
            // 
            this.gbxBuild.Controls.Add(this.cbSaveToLocalFile);
            this.gbxBuild.Controls.Add(this.cbIncludeInFlashFileSystem);
            this.gbxBuild.Controls.Add(this.btnBuildEquationBinFiles);
            this.gbxBuild.Location = new System.Drawing.Point(27, 329);
            this.gbxBuild.Name = "gbxBuild";
            this.gbxBuild.Size = new System.Drawing.Size(234, 99);
            this.gbxBuild.TabIndex = 71;
            this.gbxBuild.TabStop = false;
            this.gbxBuild.Text = "Bytecode Builder";
            // 
            // cbSaveToLocalFile
            // 
            this.cbSaveToLocalFile.AutoSize = true;
            this.cbSaveToLocalFile.Location = new System.Drawing.Point(10, 42);
            this.cbSaveToLocalFile.Name = "cbSaveToLocalFile";
            this.cbSaveToLocalFile.Size = new System.Drawing.Size(111, 17);
            this.cbSaveToLocalFile.TabIndex = 62;
            this.cbSaveToLocalFile.Text = "Save to Local File";
            this.cbSaveToLocalFile.UseVisualStyleBackColor = true;
            this.cbSaveToLocalFile.CheckedChanged += new System.EventHandler(this.cbSaveToLocalFile_CheckedChanged);
            // 
            // cbIncludeInFlashFileSystem
            // 
            this.cbIncludeInFlashFileSystem.AutoSize = true;
            this.cbIncludeInFlashFileSystem.Checked = true;
            this.cbIncludeInFlashFileSystem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIncludeInFlashFileSystem.Location = new System.Drawing.Point(10, 19);
            this.cbIncludeInFlashFileSystem.Name = "cbIncludeInFlashFileSystem";
            this.cbIncludeInFlashFileSystem.Size = new System.Drawing.Size(157, 17);
            this.cbIncludeInFlashFileSystem.TabIndex = 61;
            this.cbIncludeInFlashFileSystem.Text = "Include In Flash File System";
            this.cbIncludeInFlashFileSystem.UseVisualStyleBackColor = true;
            this.cbIncludeInFlashFileSystem.CheckedChanged += new System.EventHandler(this.cbIncludeInFlashFileSystem_CheckedChanged);
            // 
            // btnBuildEquationBinFiles
            // 
            this.btnBuildEquationBinFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuildEquationBinFiles.Location = new System.Drawing.Point(10, 66);
            this.btnBuildEquationBinFiles.Name = "btnBuildEquationBinFiles";
            this.btnBuildEquationBinFiles.Size = new System.Drawing.Size(201, 23);
            this.btnBuildEquationBinFiles.TabIndex = 60;
            this.btnBuildEquationBinFiles.Text = "Compile Bytecode File";
            this.btnBuildEquationBinFiles.UseVisualStyleBackColor = true;
            this.btnBuildEquationBinFiles.Click += new System.EventHandler(this.btnBuildEquationBinFiles_Click);
            // 
            // saveBytecodeFileDialog
            // 
            this.saveBytecodeFileDialog.FileName = "equation.btc";
            this.saveBytecodeFileDialog.Filter = "Bytecode files|*.btc";
            // 
            // lvFilePaths
            // 
            this.lvFilePaths.LabelEdit = true;
            this.lvFilePaths.Location = new System.Drawing.Point(27, 73);
            this.lvFilePaths.Name = "lvFilePaths";
            this.lvFilePaths.Size = new System.Drawing.Size(392, 97);
            this.lvFilePaths.TabIndex = 72;
            this.lvFilePaths.UseCompatibleStateImageBehavior = false;
            // 
            // btnAddFilePath
            // 
            this.btnAddFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddFilePath.Location = new System.Drawing.Point(304, 44);
            this.btnAddFilePath.Name = "btnAddFilePath";
            this.btnAddFilePath.Size = new System.Drawing.Size(26, 24);
            this.btnAddFilePath.TabIndex = 73;
            this.btnAddFilePath.Text = "+";
            this.btnAddFilePath.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.btnAddFilePath.UseVisualStyleBackColor = true;
            this.btnAddFilePath.Click += new System.EventHandler(this.btnAddFilePath_Click);
            // 
            // btnRemoveFilePath
            // 
            this.btnRemoveFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveFilePath.Location = new System.Drawing.Point(336, 44);
            this.btnRemoveFilePath.Name = "btnRemoveFilePath";
            this.btnRemoveFilePath.Size = new System.Drawing.Size(26, 24);
            this.btnRemoveFilePath.TabIndex = 74;
            this.btnRemoveFilePath.Text = "-";
            this.btnRemoveFilePath.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRemoveFilePath.UseVisualStyleBackColor = true;
            this.btnRemoveFilePath.Click += new System.EventHandler(this.btnRemoveFilePath_Click);
            // 
            // btnFilePathEdit
            // 
            this.btnFilePathEdit.Location = new System.Drawing.Point(368, 44);
            this.btnFilePathEdit.Name = "btnFilePathEdit";
            this.btnFilePathEdit.Size = new System.Drawing.Size(51, 23);
            this.btnFilePathEdit.TabIndex = 2;
            this.btnFilePathEdit.Text = "Edit";
            this.btnFilePathEdit.UseVisualStyleBackColor = true;
            this.btnFilePathEdit.Click += new System.EventHandler(this.btnFilePathEdit_Click);
            // 
            // lblFilePaths
            // 
            this.lblFilePaths.AutoSize = true;
            this.lblFilePaths.Location = new System.Drawing.Point(27, 53);
            this.lblFilePaths.Name = "lblFilePaths";
            this.lblFilePaths.Size = new System.Drawing.Size(53, 13);
            this.lblFilePaths.TabIndex = 75;
            this.lblFilePaths.Text = "File Paths";
            // 
            // tbxResults
            // 
            this.tbxResults.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbxResults.Location = new System.Drawing.Point(451, 27);
            this.tbxResults.Name = "tbxResults";
            this.tbxResults.Size = new System.Drawing.Size(596, 550);
            this.tbxResults.TabIndex = 76;
            this.tbxResults.Text = "";
            // 
            // Equations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbxResults);
            this.Controls.Add(this.lblFilePaths);
            this.Controls.Add(this.btnFilePathEdit);
            this.Controls.Add(this.btnRemoveFilePath);
            this.Controls.Add(this.btnAddFilePath);
            this.Controls.Add(this.lvFilePaths);
            this.Controls.Add(this.gbxBuild);
            this.Controls.Add(this.gbxEquationsFilePath);
            this.Name = "Equations";
            this.Size = new System.Drawing.Size(1060, 580);
            this.gbxEquationsFilePath.ResumeLayout(false);
            this.gbxEquationsFilePath.PerformLayout();
            this.gbxBuild.ResumeLayout(false);
            this.gbxBuild.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openHeaderFileDialog;
        private System.Windows.Forms.GroupBox gbxEquationsFilePath;
        private System.Windows.Forms.Button btnSelectEquationsFile;
        private System.Windows.Forms.Label lblEquationsFilePath;
        private System.Windows.Forms.GroupBox gbxBuild;
        private System.Windows.Forms.CheckBox cbSaveToLocalFile;
        private System.Windows.Forms.CheckBox cbIncludeInFlashFileSystem;
        private System.Windows.Forms.Button btnBuildEquationBinFiles;
        private System.Windows.Forms.SaveFileDialog saveBytecodeFileDialog;
        private System.Windows.Forms.ListView lvFilePaths;
        private System.Windows.Forms.Button btnAddFilePath;
        private System.Windows.Forms.Button btnRemoveFilePath;
        private System.Windows.Forms.Button btnFilePathEdit;
        private System.Windows.Forms.Label lblFilePaths;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.RichTextBox tbxResults;
    }
}
