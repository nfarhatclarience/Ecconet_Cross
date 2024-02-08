namespace ECCONetDevTool
{
    partial class ucPatternSequencer
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
            this.textBoxIntensity = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxPatternEnum = new System.Windows.Forms.TextBox();
            this.btnStopPattern = new System.Windows.Forms.Button();
            this.btnRunPattern = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxIntensity
            // 
            this.textBoxIntensity.Location = new System.Drawing.Point(88, 22);
            this.textBoxIntensity.Name = "textBoxIntensity";
            this.textBoxIntensity.Size = new System.Drawing.Size(56, 20);
            this.textBoxIntensity.TabIndex = 28;
            this.textBoxIntensity.Text = "10";
            this.textBoxIntensity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(85, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 29;
            this.label6.Text = "Intensity 0~100";
            // 
            // textBoxPatternEnum
            // 
            this.textBoxPatternEnum.Location = new System.Drawing.Point(9, 22);
            this.textBoxPatternEnum.Name = "textBoxPatternEnum";
            this.textBoxPatternEnum.Size = new System.Drawing.Size(64, 20);
            this.textBoxPatternEnum.TabIndex = 23;
            this.textBoxPatternEnum.Text = "1";
            this.textBoxPatternEnum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnStopPattern
            // 
            this.btnStopPattern.Location = new System.Drawing.Point(261, 19);
            this.btnStopPattern.Name = "btnStopPattern";
            this.btnStopPattern.Size = new System.Drawing.Size(75, 23);
            this.btnStopPattern.TabIndex = 27;
            this.btnStopPattern.Text = "Stop";
            this.btnStopPattern.UseVisualStyleBackColor = true;
            this.btnStopPattern.Click += btnStopPattern_Click;
            // 
            // btnRunPattern
            // 
            this.btnRunPattern.Location = new System.Drawing.Point(180, 19);
            this.btnRunPattern.Name = "btnRunPattern";
            this.btnRunPattern.Size = new System.Drawing.Size(75, 23);
            this.btnRunPattern.TabIndex = 26;
            this.btnRunPattern.Text = "Run";
            this.btnRunPattern.UseVisualStyleBackColor = true;
            this.btnRunPattern.Click += btnRunPattern_Click;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Pattern Enum";
            // 
            // ucPatternSequencer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxIntensity);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxPatternEnum);
            this.Controls.Add(this.btnStopPattern);
            this.Controls.Add(this.btnRunPattern);
            this.Controls.Add(this.label5);
            this.Name = "ucPatternSequencer";
            this.Size = new System.Drawing.Size(346, 50);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxIntensity;
        private System.Windows.Forms.TextBox textBoxPatternEnum;
        private System.Windows.Forms.Button btnStopPattern;
        private System.Windows.Forms.Button btnRunPattern;
    }
}
