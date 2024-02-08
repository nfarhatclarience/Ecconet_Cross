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
    public partial class ucIconLightEngine : UserControl
    {
        /// <summary>
        /// The delegate for handling an area assignment changed.
        /// </summary>
        public event AreaAssignmentChangedDelegate AreaAssignmentChanged;


        /// <summary>
        /// The light engine this icon represents.
        /// </summary>
        public ComponentTreeNode OutputNode
        {
            get => _outputNode;
            set
            {
                //  set value and configure
                _outputNode = value;
                Configure(value);
            }
        }
        private ComponentTreeNode _outputNode;

        /// <summary>
        /// The currently-selected expression.
        /// </summary>
        public Expression Expression { get; set; }

        /// <summary>
        /// Private list of designer-generated ucIconLED controls
        /// </summary>
        private ucIconLEDColor[] ucLEDColors;

        /// <summary>
        /// Indicates whether to show IDs or locations.
        /// </summary>
        public bool ShowId
        {
            get => _showId;
            set
            {
                _showId = value;
                if (_outputNode != null)
                {
                    if (_showId)
                        //tbID.Text = "  0x" + _lightEngine.Id.ToString("X4");
                        tbID.Text = "  " + _outputNode.Id.ToString();
                    else
                        tbID.Text = _outputNode.Location.ToString();
                }
            }
        }
        private bool _showId;



        public ucIconLightEngine()
        {
            //  initialize controls
            InitializeComponent();

            //  create private list of designer-generated ucIconLED controls
            ucLEDColors = new ucIconLEDColor[] { ucIconLEDColor1, ucIconLEDColor2, ucIconLEDColor3 };

            //  hook up the area assignment changed events
            foreach (ucIconLEDColor color in ucLEDColors)
            {
                color.AreaAssignmentChanged += AreaAssignmentChangedHandler;
            }
        }


        /// <summary>
        /// The method to handle individual LED area assignment changed notifications.
        /// </summary>
        /// <param name="sender">The individual LED whose assignment changed.</param>
        /// <param name="isAssigned">A value indicating whether the user assigned the LED to the current area.</param>
        private void AreaAssignmentChangedHandler(object sender, bool isAssigned)
        {
            AreaAssignmentChanged?.Invoke(sender, isAssigned);
        }

        #region Update the individual LED selections for an expression area
        /// <summary>
        /// Updates the individual LED selections for an expression area.
        /// </summary>
        /// <param name="area">The given expression area.</param>
        /// <param name="lightEngine">The parent light engine.</param>
        public void UpdateForArea(Expression.Area area)
        {
            foreach (ucIconLEDColor ledColor in ucLEDColors)
                ledColor.UpdateForArea(area);
        }
        #endregion


        /// <summary>
        /// Configures the light engine icon user control when a light engine is assigned.
        /// </summary>
        /// <param name="node">The output to assign to this control.</param>
        private void Configure(ComponentTreeNode node)
        {
            if (node == null)
                return;

            //  set the light engine ID text
            if (_showId)
                tbID.Text = "  0x" + node.Id.ToString("X4");
            else
                tbID.Text = node.Location.ToString();

            //  configure each color
            for (int i = 0; i < ucLEDColors.Length; ++i)
                ucLEDColors[i].Visible = false;

            //  if this node is endpoint
            if (node.IsEndpoint)
            {
                ucLEDColors[0].Visible = true;
                ucLEDColors[0].LEDColor = node;
            }
            else
            {
                foreach (var childnode in node.ChildNodes)
                {
                    int index = childnode.Id;
                    if (index < ucLEDColors.Length)
                    {
                        ucLEDColors[index].Visible = true;
                        ucLEDColors[index].LEDColor = childnode;
                    }
                }
            }
        }

        /*
        /// <summary>
        /// User double-clicks the light engine ID.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbID_DoubleClick(object sender, EventArgs e)
        {
            formLightEngine form = new formLightEngine();
            form.LightEngine = OutputNode;
            form.Expression = Expression;
            form.Show();
        }
        */
    }
}
