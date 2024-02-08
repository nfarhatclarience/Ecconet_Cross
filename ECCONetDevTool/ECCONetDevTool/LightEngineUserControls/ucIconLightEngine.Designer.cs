namespace ECCONetDevTool.LightEngineUserControls
{
    partial class ucIconLightEngine
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
            this.tbID = new System.Windows.Forms.TextBox();
            this.ucIconLEDColor3 = new ECCONetDevTool.LightEngineUserControls.ucIconLEDColor();
            this.ucIconLEDColor2 = new ECCONetDevTool.LightEngineUserControls.ucIconLEDColor();
            this.ucIconLEDColor1 = new ECCONetDevTool.LightEngineUserControls.ucIconLEDColor();
            this.SuspendLayout();
            // 
            // tbID
            // 
            this.tbID.Location = new System.Drawing.Point(0, 0);
            this.tbID.Name = "tbID";
            this.tbID.Size = new System.Drawing.Size(66, 20);
            this.tbID.TabIndex = 3;
            // 
            // ucIconLEDColor3
            // 
            this.ucIconLEDColor3.LEDColor = null;
            this.ucIconLEDColor3.Location = new System.Drawing.Point(0, 60);
            this.ucIconLEDColor3.Name = "ucIconLEDColor3";
            this.ucIconLEDColor3.Size = new System.Drawing.Size(66, 20);
            this.ucIconLEDColor3.TabIndex = 2;
            // 
            // ucIconLEDColor2
            // 
            this.ucIconLEDColor2.LEDColor = null;
            this.ucIconLEDColor2.Location = new System.Drawing.Point(0, 40);
            this.ucIconLEDColor2.Name = "ucIconLEDColor2";
            this.ucIconLEDColor2.Size = new System.Drawing.Size(66, 20);
            this.ucIconLEDColor2.TabIndex = 1;
            // 
            // ucIconLEDColor1
            // 
            this.ucIconLEDColor1.LEDColor = null;
            this.ucIconLEDColor1.Location = new System.Drawing.Point(0, 20);
            this.ucIconLEDColor1.Name = "ucIconLEDColor1";
            this.ucIconLEDColor1.Size = new System.Drawing.Size(66, 20);
            this.ucIconLEDColor1.TabIndex = 0;
            // 
            // ucIconLightEngine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tbID);
            this.Controls.Add(this.ucIconLEDColor3);
            this.Controls.Add(this.ucIconLEDColor2);
            this.Controls.Add(this.ucIconLEDColor1);
            this.Name = "ucIconLightEngine";
            this.Size = new System.Drawing.Size(64, 78);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public ucIconLEDColor ucIconLEDColor1;
        public ucIconLEDColor ucIconLEDColor2;
        public ucIconLEDColor ucIconLEDColor3;
        public System.Windows.Forms.TextBox tbID;
    }
}
