namespace ECCONetDevTool.LedMatrixFile
{
    partial class ucLedMatrix
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
            this.dgvPatterns = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SpeedPeriod = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mode = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ScrollOnce = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Mirror = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Wrap = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPatterns)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPatterns
            // 
            this.dgvPatterns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPatterns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.SpeedPeriod,
            this.Mode,
            this.ScrollOnce,
            this.Mirror,
            this.Wrap});
            this.dgvPatterns.Location = new System.Drawing.Point(9, 9);
            this.dgvPatterns.Name = "dgvPatterns";
            this.dgvPatterns.Size = new System.Drawing.Size(868, 277);
            this.dgvPatterns.TabIndex = 100;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Step";
            this.dataGridViewTextBoxColumn1.HeaderText = "Step";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 45;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Message";
            this.dataGridViewTextBoxColumn2.HeaderText = "Message";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 250;
            // 
            // SpeedPeriod
            // 
            this.SpeedPeriod.HeaderText = "Speed / Period";
            this.SpeedPeriod.Name = "SpeedPeriod";
            this.SpeedPeriod.Width = 60;
            // 
            // Mode
            // 
            this.Mode.HeaderText = "Mode";
            this.Mode.Items.AddRange(new object[] {
            "Off",
            "Static",
            "Timed Static",
            "Scroll Left",
            "Scroll Right",
            "Scroll Up",
            "Scroll Down"});
            this.Mode.Name = "Mode";
            // 
            // ScrollOnce
            // 
            this.ScrollOnce.HeaderText = "Scroll Once";
            this.ScrollOnce.Name = "ScrollOnce";
            this.ScrollOnce.Width = 45;
            // 
            // Mirror
            // 
            this.Mirror.HeaderText = "Mirror";
            this.Mirror.Name = "Mirror";
            this.Mirror.Width = 45;
            // 
            // Wrap
            // 
            this.Wrap.HeaderText = "Wrap";
            this.Wrap.Name = "Wrap";
            this.Wrap.Width = 45;
            // 
            // ucLedMatrix
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvPatterns);
            this.Name = "ucLedMatrix";
            this.Size = new System.Drawing.Size(919, 467);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPatterns)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPatterns;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn SpeedPeriod;
        private System.Windows.Forms.DataGridViewComboBoxColumn Mode;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ScrollOnce;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Mirror;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Wrap;
    }
}
