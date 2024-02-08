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
    /// <summary>
    /// The delegate method declaration for area assignment changed notifications.
    /// </summary>
    /// <param name="sender">The individual LED whose assignment changed.</param>
    /// <param name="isAssigned">A value indicating whether the user assigned the LED to the current area.</param>
    public delegate void AreaAssignmentChangedDelegate(object sender, bool isAssigned);


    public partial class ucIconLEDColor : UserControl
    {
        /// <summary>
        /// The delegate for handling area assignment changed.
        /// </summary>
        public event AreaAssignmentChangedDelegate AreaAssignmentChanged;

        /// <summary>
        /// The LED color assigned to this ucIconLED.
        /// </summary>
        public ComponentTreeNode LEDColor
        {
            get => _ledColor;
            set
            {
                //  set value and configure control
                _ledColor = value;

                //  create array of LED paths
                if (_ledColor != null)
                {
                    if (_ledColor.IsEndpoint)
                    {
                        ledPaths = new string[] { _ledColor.UniqueOutputPath() };
                    }
                    else
                    {
                        ledPaths = new string[_ledColor.ChildNodes.Count];
                        for (int i = 0; i < _ledColor.ChildNodes.Count; ++i)
                            ledPaths[i] = _ledColor.ChildNodes[i].UniqueOutputPath();
                    }

                    //  configure the user controls
                    ConfigureWithLEDColor(value);
                }
            }
        }
        private ComponentTreeNode _ledColor;

        /// <summary>
        /// Private array of designer-generated radio buttons.
        /// </summary>
        private RadioButton[] radioButtons;

        /// <summary>
        /// Private array of led path strings.
        /// </summary>
        private string[] ledPaths;


        /// <summary>
        /// Constructor.
        /// </summary>
        public ucIconLEDColor()
        {
            //  initialize the designer-generated components
            InitializeComponent();

            //  create private array of designer-generated radio buttons
            radioButtons = new RadioButton[] { rbLeft, rbCenter, rbRight };
        }

        #region Configure control based on the LED color it represents.
        /// <summary>
        /// Configure the control with the given LED color.
        /// </summary>
        /// <param name="node">The LED color.</param>
        private void ConfigureWithLEDColor(ComponentTreeNode node)
        {
            if (node == null)
                return;

            //  get color
            string c = "White";
            if (node is OutputColorNode colorNode)
                c = colorNode.Color;

            //  set background color
            if (c.Equals("Amber"))
                c = "DarkOrange";
            this.BackColor = Color.FromName(c);

            //  determine which LED strings are in color and make visible
            rbLeft.Visible = false;
            rbCenter.Visible = false;
            rbRight.Visible = false;

            //  if node is an endpoint, then just use the left rb
            if (node.IsEndpoint)
            {
                rbCenter.Visible = true;
            }
            else
            {
                /*
                foreach (var childnode in node.ChildNodes)
                {
                    switch (childnode.Id)
                    {
                        case 0:
                            rbLeft.Visible = true;
                            break;
                        case 1:
                            rbCenter.Visible = true;
                            break;
                        case 2:
                            rbRight.Visible = true;
                            break;
                        default:
                            break;
                    }
                }
                */
                for (int n = 0; n < node.ChildNodes.Count; ++n)
                {
                    switch (n)
                    {
                        case 0:
                            rbLeft.Visible = true;
                            break;
                        case 1:
                            rbCenter.Visible = true;
                            break;
                        case 2:
                            rbRight.Visible = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        #endregion

        #region Update the individual LED selections for an expression area
        /// <summary>
        /// Updates the individual LED selections for an expression area.
        /// </summary>
        /// <param name="area">The given expression area.</param>
        public void UpdateForArea(Expression.Area area)
        {
            //  de-select all radio buttons
            for (int i = 0; i < radioButtons.Length; ++i)
                radioButtons[i].Checked = false;

            //  if area defined
            if ((area != null) && (LEDColor != null) && (ledPaths != null))
            {
                //  for all paths in area
                foreach (var pathValue in area.OutputPaths)
                {
                    if (_ledColor.IsEndpoint)
                    {
                        if (pathValue.Path.Equals(ledPaths[0]))
                            rbCenter.Checked = true;
                    }
                    else
                    {
                        for (int i = 0; i < LEDColor.ChildNodes.Count; ++i)
                        {
                            if (/*(i < radioButtons.Length) &&*/
                                (pathValue.Path.Equals(ledPaths[i])))
                                radioButtons[i].Checked = true;
                        }
                    }
                }
            }
        }
        #endregion

        #region Radio button clicked
        private void rbLeft_Click(object sender, EventArgs e)
        {
            rbLeft.Checked = !rbLeft.Checked;
            var node = _ledColor;
            if (!node.IsEndpoint)
            {
                //node = _ledColor.GetChildWithId(0);
                node = null;
                if ((_ledColor.ChildNodes != null) && (_ledColor.ChildNodes.Count > 0))
                    node = _ledColor.ChildNodes[0];
            }
            if (node != null)
                AreaAssignmentChanged?.Invoke(node.GetUniqueOutputPathValue(), rbLeft.Checked);
        }

        private void rbCenter_Click(object sender, EventArgs e)
        {
            rbCenter.Checked = !rbCenter.Checked;
            var node = _ledColor;
            if (!node.IsEndpoint)
            {
                //node = _ledColor.GetChildWithId(1);
                node = null;
                if ((_ledColor.ChildNodes != null) && (_ledColor.ChildNodes.Count > 1))
                    node = _ledColor.ChildNodes[1];
            }
            if (node != null)
                AreaAssignmentChanged?.Invoke(node.GetUniqueOutputPathValue(), rbCenter.Checked);
        }

        private void rbRight_Click(object sender, EventArgs e)
        {
            rbRight.Checked = !rbRight.Checked;
            var node = _ledColor;
            if (!node.IsEndpoint)
            {
                //node = _ledColor.GetChildWithId(2);
                node = null;
                if ((_ledColor.ChildNodes != null) && (_ledColor.ChildNodes.Count > 2))
                    node = _ledColor.ChildNodes[2];
            }
            if (node != null)
                AreaAssignmentChanged?.Invoke(node.GetUniqueOutputPathValue(), rbRight.Checked);
        }
        #endregion
    }
}
