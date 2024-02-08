using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESG.ExpressionLib.DataModels;

namespace ECCONetDevTool.LightEngineUserControls
{
    public partial class ucLightEngineSingleLED : UserControl
    {
        /// <summary>
        /// The light engine this icon represents.
        /// </summary>
        public ComponentTreeNode LightEngine
        {
            get => _lightEngine;
            set
            {
                //  set value and configure
                _lightEngine = value;
                //Configure(value);
            }
        }
        private ComponentTreeNode _lightEngine;

        public ucLightEngineSingleLED()
        {
            InitializeComponent();
        }
        /*
        /// <summary>
        /// Configures the light engine icon user control when a light engine is assigned.
        /// </summary>
        /// <param name="lightEngine">The light engine to assign to this control.</param>
        private void Configure(LightEngine lightEngine)
        {
            if (lightEngine == null)
                return;

            //  set the light engine ID text
            tbID.Text = lightEngine.Id.ToString();

            //  set the background color
            this.BackColor =             //  set background color
            this.BackColor = Color.FromName(ledColor.Color);




            //  in the light engine, colors are in a list to remain dynamic

            //  configure each color
            for (int i = 0; i < ucLEDColors.Length; ++i)
                ucLEDColors[i].Visible = false;
            foreach (LightEngine.LEDColor ledColor in lightEngine.LedColors)
            {
                int index = (int)ledColor.Id;
                if (index < ucLEDColors.Length)
                {
                    ucLEDColors[index].Visible = true;
                    ucLEDColors[index].LEDColor = ledColor;
                }
            }
        }
        */
    }
}
