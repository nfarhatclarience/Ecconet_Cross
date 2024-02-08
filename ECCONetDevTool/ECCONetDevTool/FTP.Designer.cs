namespace ECCONetDevTool
{
    partial class FTP
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
            this.cbbOnlineDevices = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnFileWrite = new System.Windows.Forms.Button();
            this.btnFileList = new System.Windows.Forms.Button();
            this.bntFileRead = new System.Windows.Forms.Button();
            this.btnDeleteFile = new System.Windows.Forms.Button();
            this.btnFileGetInfo = new System.Windows.Forms.Button();
            this.tbFtpResponse = new System.Windows.Forms.RichTextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.tbServerFilePathName = new System.Windows.Forms.TextBox();
            this.tbLocalFilePathName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonFileSearch = new System.Windows.Forms.Button();
            this.lblLocalPathNameError = new System.Windows.Forms.Label();
            this.lblServerFileNameError = new System.Windows.Forms.Label();
            this.lblServerAddressError = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbbOnlineDevices
            // 
            this.cbbOnlineDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbOnlineDevices.FormattingEnabled = true;
            this.cbbOnlineDevices.Location = new System.Drawing.Point(139, 28);
            this.cbbOnlineDevices.Name = "cbbOnlineDevices";
            this.cbbOnlineDevices.Size = new System.Drawing.Size(211, 21);
            this.cbbOnlineDevices.TabIndex = 37;
            this.cbbOnlineDevices.SelectedIndexChanged += new System.EventHandler(this.cbbOnlineDevices_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(54, 31);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(79, 13);
            this.label11.TabIndex = 38;
            this.label11.Text = "Server Address";
            this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnFileWrite
            // 
            this.btnFileWrite.Location = new System.Drawing.Point(189, 176);
            this.btnFileWrite.Name = "btnFileWrite";
            this.btnFileWrite.Size = new System.Drawing.Size(75, 23);
            this.btnFileWrite.TabIndex = 33;
            this.btnFileWrite.Text = "Write File";
            this.btnFileWrite.UseVisualStyleBackColor = true;
            this.btnFileWrite.Click += new System.EventHandler(this.btnFileWrite_Click);
            // 
            // btnFileList
            // 
            this.btnFileList.Location = new System.Drawing.Point(355, 176);
            this.btnFileList.Name = "btnFileList";
            this.btnFileList.Size = new System.Drawing.Size(75, 23);
            this.btnFileList.TabIndex = 36;
            this.btnFileList.Text = "File List";
            this.btnFileList.UseVisualStyleBackColor = true;
            this.btnFileList.Click += new System.EventHandler(this.btnFileList_Click);
            // 
            // bntFileRead
            // 
            this.bntFileRead.Location = new System.Drawing.Point(104, 176);
            this.bntFileRead.Name = "bntFileRead";
            this.bntFileRead.Size = new System.Drawing.Size(75, 23);
            this.bntFileRead.TabIndex = 32;
            this.bntFileRead.Text = "Read File";
            this.bntFileRead.UseVisualStyleBackColor = true;
            this.bntFileRead.Click += new System.EventHandler(this.bntFileRead_Click);
            // 
            // btnDeleteFile
            // 
            this.btnDeleteFile.Location = new System.Drawing.Point(274, 176);
            this.btnDeleteFile.Name = "btnDeleteFile";
            this.btnDeleteFile.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteFile.TabIndex = 35;
            this.btnDeleteFile.Text = "Delete File";
            this.btnDeleteFile.UseVisualStyleBackColor = true;
            this.btnDeleteFile.Click += new System.EventHandler(this.btnDeleteFile_Click);
            // 
            // btnFileGetInfo
            // 
            this.btnFileGetInfo.Location = new System.Drawing.Point(19, 176);
            this.btnFileGetInfo.Name = "btnFileGetInfo";
            this.btnFileGetInfo.Size = new System.Drawing.Size(75, 23);
            this.btnFileGetInfo.TabIndex = 31;
            this.btnFileGetInfo.Text = "Get File Info";
            this.btnFileGetInfo.UseVisualStyleBackColor = true;
            this.btnFileGetInfo.Click += new System.EventHandler(this.btnFileGetInfo_Click);
            // 
            // tbFtpResponse
            // 
            this.tbFtpResponse.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbFtpResponse.Location = new System.Drawing.Point(19, 216);
            this.tbFtpResponse.Name = "tbFtpResponse";
            this.tbFtpResponse.Size = new System.Drawing.Size(575, 401);
            this.tbFtpResponse.TabIndex = 34;
            this.tbFtpResponse.Text = "";
            // 
            // tbServerFilePathName
            // 
            this.tbServerFilePathName.Location = new System.Drawing.Point(139, 74);
            this.tbServerFilePathName.Name = "tbServerFilePathName";
            this.tbServerFilePathName.Size = new System.Drawing.Size(211, 20);
            this.tbServerFilePathName.TabIndex = 39;
            this.tbServerFilePathName.TextChanged += new System.EventHandler(this.tbServerFilePathName_TextChanged);
            // 
            // tbLocalFilePathName
            // 
            this.tbLocalFilePathName.Location = new System.Drawing.Point(139, 126);
            this.tbLocalFilePathName.Name = "tbLocalFilePathName";
            this.tbLocalFilePathName.Size = new System.Drawing.Size(374, 20);
            this.tbLocalFilePathName.TabIndex = 40;
            this.tbLocalFilePathName.TextChanged += new System.EventHandler(this.tbLocalFilePathName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 41;
            this.label1.Text = "Server File Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 42;
            this.label2.Text = "Local File Path/Name";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // buttonFileSearch
            // 
            this.buttonFileSearch.Location = new System.Drawing.Point(519, 124);
            this.buttonFileSearch.Name = "buttonFileSearch";
            this.buttonFileSearch.Size = new System.Drawing.Size(75, 23);
            this.buttonFileSearch.TabIndex = 43;
            this.buttonFileSearch.Text = "Search";
            this.buttonFileSearch.UseVisualStyleBackColor = true;
            this.buttonFileSearch.Click += new System.EventHandler(this.buttonFileSearch_Click);
            // 
            // lblLocalPathNameError
            // 
            this.lblLocalPathNameError.AutoSize = true;
            this.lblLocalPathNameError.ForeColor = System.Drawing.Color.DarkRed;
            this.lblLocalPathNameError.Location = new System.Drawing.Point(136, 110);
            this.lblLocalPathNameError.Name = "lblLocalPathNameError";
            this.lblLocalPathNameError.Size = new System.Drawing.Size(120, 13);
            this.lblLocalPathNameError.TabIndex = 46;
            this.lblLocalPathNameError.Text = "Local file does not exist.";
            this.lblLocalPathNameError.Visible = false;
            // 
            // lblServerFileNameError
            // 
            this.lblServerFileNameError.AutoSize = true;
            this.lblServerFileNameError.ForeColor = System.Drawing.Color.DarkRed;
            this.lblServerFileNameError.Location = new System.Drawing.Point(136, 58);
            this.lblServerFileNameError.Name = "lblServerFileNameError";
            this.lblServerFileNameError.Size = new System.Drawing.Size(225, 13);
            this.lblServerFileNameError.TabIndex = 47;
            this.lblServerFileNameError.Text = "Invalid server file name (must be in 8.3 format).";
            this.lblServerFileNameError.Visible = false;
            // 
            // lblServerAddressError
            // 
            this.lblServerAddressError.AutoSize = true;
            this.lblServerAddressError.ForeColor = System.Drawing.Color.DarkRed;
            this.lblServerAddressError.Location = new System.Drawing.Point(136, 12);
            this.lblServerAddressError.Name = "lblServerAddressError";
            this.lblServerAddressError.Size = new System.Drawing.Size(114, 13);
            this.lblServerAddressError.TabIndex = 48;
            this.lblServerAddressError.Text = "Please select a server.";
            this.lblServerAddressError.Visible = false;
            // 
            // FTP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblServerAddressError);
            this.Controls.Add(this.lblServerFileNameError);
            this.Controls.Add(this.lblLocalPathNameError);
            this.Controls.Add(this.buttonFileSearch);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbLocalFilePathName);
            this.Controls.Add(this.tbServerFilePathName);
            this.Controls.Add(this.cbbOnlineDevices);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.btnFileWrite);
            this.Controls.Add(this.btnFileList);
            this.Controls.Add(this.bntFileRead);
            this.Controls.Add(this.btnDeleteFile);
            this.Controls.Add(this.btnFileGetInfo);
            this.Controls.Add(this.tbFtpResponse);
            this.Name = "FTP";
            this.Size = new System.Drawing.Size(800, 620);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cbbOnlineDevices;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnFileWrite;
        private System.Windows.Forms.Button btnFileList;
        private System.Windows.Forms.Button bntFileRead;
        private System.Windows.Forms.Button btnDeleteFile;
        private System.Windows.Forms.Button btnFileGetInfo;
        private System.Windows.Forms.RichTextBox tbFtpResponse;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.TextBox tbServerFilePathName;
        private System.Windows.Forms.TextBox tbLocalFilePathName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonFileSearch;
        private System.Windows.Forms.Label lblLocalPathNameError;
        private System.Windows.Forms.Label lblServerFileNameError;
        private System.Windows.Forms.Label lblServerAddressError;
    }
}
