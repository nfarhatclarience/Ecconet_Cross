namespace ECCONetDevTool.LightEngineUserControls
{
    partial class ucIconLEDColor
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
            this.rbLeft = new System.Windows.Forms.RadioButton();
            this.rbCenter = new System.Windows.Forms.RadioButton();
            this.rbRight = new System.Windows.Forms.RadioButton();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.pnlCenter = new System.Windows.Forms.Panel();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.pnlLeft.SuspendLayout();
            this.pnlCenter.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbLeft
            // 
            this.rbLeft.AutoCheck = false;
            this.rbLeft.AutoSize = true;
            this.rbLeft.Location = new System.Drawing.Point(3, 3);
            this.rbLeft.Name = "rbLeft";
            this.rbLeft.Size = new System.Drawing.Size(14, 13);
            this.rbLeft.TabIndex = 0;
            this.rbLeft.TabStop = true;
            this.rbLeft.UseVisualStyleBackColor = true;
            this.rbLeft.Click += new System.EventHandler(this.rbLeft_Click);
            // 
            // rbCenter
            // 
            this.rbCenter.AutoCheck = false;
            this.rbCenter.AutoSize = true;
            this.rbCenter.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbCenter.Location = new System.Drawing.Point(3, 3);
            this.rbCenter.Name = "rbCenter";
            this.rbCenter.Size = new System.Drawing.Size(14, 13);
            this.rbCenter.TabIndex = 1;
            this.rbCenter.TabStop = true;
            this.rbCenter.UseVisualStyleBackColor = true;
            this.rbCenter.Click += new System.EventHandler(this.rbCenter_Click);
            // 
            // rbRight
            // 
            this.rbRight.AutoCheck = false;
            this.rbRight.AutoSize = true;
            this.rbRight.Location = new System.Drawing.Point(3, 3);
            this.rbRight.Name = "rbRight";
            this.rbRight.Size = new System.Drawing.Size(14, 13);
            this.rbRight.TabIndex = 2;
            this.rbRight.TabStop = true;
            this.rbRight.UseVisualStyleBackColor = true;
            this.rbRight.Click += new System.EventHandler(this.rbRight_Click);
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.rbLeft);
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Margin = new System.Windows.Forms.Padding(0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(20, 20);
            this.pnlLeft.TabIndex = 3;
            // 
            // pnlCenter
            // 
            this.pnlCenter.Controls.Add(this.rbCenter);
            this.pnlCenter.Location = new System.Drawing.Point(22, 0);
            this.pnlCenter.Margin = new System.Windows.Forms.Padding(0);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Size = new System.Drawing.Size(20, 20);
            this.pnlCenter.TabIndex = 4;
            // 
            // pnlRight
            // 
            this.pnlRight.Controls.Add(this.rbRight);
            this.pnlRight.Location = new System.Drawing.Point(44, 0);
            this.pnlRight.Margin = new System.Windows.Forms.Padding(0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(20, 20);
            this.pnlRight.TabIndex = 4;
            // 
            // ucIconLEDColor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlCenter);
            this.Controls.Add(this.pnlLeft);
            this.Name = "ucIconLEDColor";
            this.Size = new System.Drawing.Size(66, 20);
            this.pnlLeft.ResumeLayout(false);
            this.pnlLeft.PerformLayout();
            this.pnlCenter.ResumeLayout(false);
            this.pnlCenter.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            this.pnlRight.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbLeft;
        private System.Windows.Forms.RadioButton rbCenter;
        private System.Windows.Forms.RadioButton rbRight;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlCenter;
        private System.Windows.Forms.Panel pnlRight;
    }
}
