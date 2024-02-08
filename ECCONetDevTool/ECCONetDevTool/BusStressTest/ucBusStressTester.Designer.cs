namespace ECCONetDevTool.BusStressTest
{
    partial class ucBusStressTester
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
            this.ucNodePingStatistics = new ECCONetDevTool.BusStressTest.ucNodePingStatistics();
            this.ucBusFlood = new ECCONetDevTool.BusStressTest.ucBusFlood();
            this.gbxTokenToggle = new System.Windows.Forms.GroupBox();
            this.ucTokenToggle = new ECCONetDevTool.BusStressTest.ucTokenToggle();
            this.gbxTokenToggle.SuspendLayout();
            this.SuspendLayout();
            // 
            // ucNodePingStatistics
            // 
            this.ucNodePingStatistics.Location = new System.Drawing.Point(4, 4);
            this.ucNodePingStatistics.Name = "ucNodePingStatistics";
            this.ucNodePingStatistics.Size = new System.Drawing.Size(443, 612);
            this.ucNodePingStatistics.TabIndex = 2;
            // 
            // ucBusFlood
            // 
            this.ucBusFlood.Location = new System.Drawing.Point(488, 4);
            this.ucBusFlood.Name = "ucBusFlood";
            this.ucBusFlood.Size = new System.Drawing.Size(412, 315);
            this.ucBusFlood.TabIndex = 1;
            // 
            // gbxTokenToggle
            // 
            this.gbxTokenToggle.Controls.Add(this.ucTokenToggle);
            this.gbxTokenToggle.Location = new System.Drawing.Point(488, 326);
            this.gbxTokenToggle.Name = "gbxTokenToggle";
            this.gbxTokenToggle.Size = new System.Drawing.Size(473, 172);
            this.gbxTokenToggle.TabIndex = 3;
            this.gbxTokenToggle.TabStop = false;
            this.gbxTokenToggle.Text = "Token Toggle";
            // 
            // ucTokenToggle
            // 
            this.ucTokenToggle.Location = new System.Drawing.Point(7, 20);
            this.ucTokenToggle.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.ucTokenToggle.Name = "ucTokenToggle";
            this.ucTokenToggle.Size = new System.Drawing.Size(458, 143);
            this.ucTokenToggle.TabIndex = 0;
            // 
            // ucBusStressTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxTokenToggle);
            this.Controls.Add(this.ucNodePingStatistics);
            this.Controls.Add(this.ucBusFlood);
            this.Name = "ucBusStressTester";
            this.Size = new System.Drawing.Size(1080, 620);
            this.gbxTokenToggle.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        public ucBusFlood ucBusFlood;
        public ucNodePingStatistics ucNodePingStatistics;
        private System.Windows.Forms.GroupBox gbxTokenToggle;
        public ucTokenToggle ucTokenToggle;
    }
}
