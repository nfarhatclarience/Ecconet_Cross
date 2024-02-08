using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESG.ExpressionLib.DataModels;
using ESG.ExpressionLib.DataConverters;


namespace ECCONetDevTool
{
    public partial class FormBlendResults : Form
    {
        public FormBlendResults()
        {
            InitializeComponent();
        }


        public void ShowResults(Expression exp)
        {
            this.Name = exp.Name + " Results";

            ExpressionConverters.ExpressionStats(exp,
                out uint totalPeriod, out uint minStepPeriod, out uint maxStepPeriod, out uint totalNumSteps);

            lblTotalPeriod.Text += totalPeriod.ToString();
            lblMinStepPeriod.Text += minStepPeriod.ToString();
            lblMaxStepPeriod.Text += maxStepPeriod.ToString();
            lblTotalNumSteps.Text += totalNumSteps.ToString();
        }

    }
}
