namespace ECCONetDevTool
{
    partial class FormBlendResults
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
            this.lblTotalPeriod = new System.Windows.Forms.Label();
            this.lblMinStepPeriod = new System.Windows.Forms.Label();
            this.lblMaxStepPeriod = new System.Windows.Forms.Label();
            this.lblTotalNumSteps = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblTotalPeriod
            // 
            this.lblTotalPeriod.AutoSize = true;
            this.lblTotalPeriod.Location = new System.Drawing.Point(12, 9);
            this.lblTotalPeriod.Name = "lblTotalPeriod";
            this.lblTotalPeriod.Size = new System.Drawing.Size(70, 13);
            this.lblTotalPeriod.TabIndex = 0;
            this.lblTotalPeriod.Text = "Total Period: ";
            // 
            // lblMinStepPeriod
            // 
            this.lblMinStepPeriod.AutoSize = true;
            this.lblMinStepPeriod.Location = new System.Drawing.Point(12, 37);
            this.lblMinStepPeriod.Name = "lblMinStepPeriod";
            this.lblMinStepPeriod.Size = new System.Drawing.Size(88, 13);
            this.lblMinStepPeriod.TabIndex = 1;
            this.lblMinStepPeriod.Text = "Min Step Period: ";
            // 
            // lblMaxStepPeriod
            // 
            this.lblMaxStepPeriod.AutoSize = true;
            this.lblMaxStepPeriod.Location = new System.Drawing.Point(12, 66);
            this.lblMaxStepPeriod.Name = "lblMaxStepPeriod";
            this.lblMaxStepPeriod.Size = new System.Drawing.Size(91, 13);
            this.lblMaxStepPeriod.TabIndex = 2;
            this.lblMaxStepPeriod.Text = "Max Step Period: ";
            // 
            // lblTotalNumSteps
            // 
            this.lblTotalNumSteps.AutoSize = true;
            this.lblTotalNumSteps.Location = new System.Drawing.Point(12, 96);
            this.lblTotalNumSteps.Name = "lblTotalNumSteps";
            this.lblTotalNumSteps.Size = new System.Drawing.Size(119, 13);
            this.lblTotalNumSteps.TabIndex = 3;
            this.lblTotalNumSteps.Text = "Total Number of Steps: ";
            // 
            // FormBlendResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 117);
            this.Controls.Add(this.lblTotalNumSteps);
            this.Controls.Add(this.lblMaxStepPeriod);
            this.Controls.Add(this.lblMinStepPeriod);
            this.Controls.Add(this.lblTotalPeriod);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormBlendResults";
            this.Text = "Blend Results";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTotalPeriod;
        private System.Windows.Forms.Label lblMinStepPeriod;
        private System.Windows.Forms.Label lblMaxStepPeriod;
        private System.Windows.Forms.Label lblTotalNumSteps;
    }
}