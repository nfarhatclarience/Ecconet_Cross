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

namespace ECCONetDevTool.ExpressionEdit
{
    public partial class ExpressionListView : ListView
    {
        private ListViewItem listViewItem;
        private int subItemIndex = 0;
        private System.Windows.Forms.TextBox editBox = new System.Windows.Forms.TextBox();
        private System.Windows.Forms.ComboBox comboBox = new System.Windows.Forms.ComboBox();
        private System.Windows.Forms.ComboBox comboBoxExpressions = new System.Windows.Forms.ComboBox();

        /// <summary>
        /// The expression collection.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ExpressionCollection ExpressionCollection { get; set; }

        /// <summary>
        /// The expression collection.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ExpressionCollection ExpressionTemplateCollection
        {
            get => _expressionTemplateCollection;
            set
            {
                _expressionTemplateCollection = value;
                if (comboBoxExpressions != null)
                {
                    comboBoxExpressions.Items.Clear();
                    comboBoxExpressions.Items.AddRange(value.Expressions.ToArray());
                }
            }
        }
        private ExpressionCollection _expressionTemplateCollection;



        private enum columnIndices
        {
            Enum,
            Expression,
            OutputPriority,
            InputPriority
        }

        public ExpressionListView()
        {
            //  initialize the columns
            View = View.Details;
            Columns.Add("Enum", 50, HorizontalAlignment.Center);
            Columns.Add("Expression", 250, HorizontalAlignment.Center);
            Columns.Add("Output", 50, HorizontalAlignment.Center);
            Columns.Add("Input", 50, HorizontalAlignment.Center);

            //  customize control
            FullRowSelect = true;
            TabIndex = 0;
            View = System.Windows.Forms.View.Details;
            MouseDown += ExpressionListView_MouseDown;
            GridLines = true;
            this.Size = new System.Drawing.Size(0, 0);
            this.TabIndex = 0;

            //  intialize the eidt box
            InitEditBox();

            //  intialize the combo box
            InitComboBox();

            InitExpressionComboBox();

            ExpressionCollection = new ExpressionCollection();

        }


        #region Combo box
        private void InitComboBox()
        {
            //  combo box
            comboBox.Items.Add(1);
            comboBox.Items.Add(2);
            comboBox.Items.Add(3);
            comboBox.Items.Add(4);
            comboBox.Items.Add(5);
            comboBox.Items.Add(6);
            comboBox.Size = new System.Drawing.Size(0, 0);
            comboBox.Location = new System.Drawing.Point(0, 0);
            Controls.Add(comboBox);

            comboBox.SelectedIndexChanged += new System.EventHandler(this.CmbSelected);
            comboBox.LostFocus += new System.EventHandler(this.CmbFocusOver);
            comboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CmbKeyPress);
            comboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            comboBox.BackColor = Color.SkyBlue;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Hide();
        }

        private void CmbKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 || e.KeyChar == 27)
            {
                comboBox.Hide();
            }
        }

        private void CmbSelected(object sender, System.EventArgs e)
        {
            int sel = comboBox.SelectedIndex;
            if (sel >= 0)
            {
                string itemSel = comboBox.Items[sel].ToString();
                listViewItem.SubItems[subItemIndex].Text = itemSel;
            }
        }

        private void CmbFocusOver(object sender, System.EventArgs e)
        {
            comboBox.Hide();
        }
        #endregion


        #region Expressions combo box
        private void InitExpressionComboBox()
        {
            //  combo box
            comboBoxExpressions.Size = new System.Drawing.Size(0, 0);
            comboBoxExpressions.Location = new System.Drawing.Point(0, 0);
            Controls.Add(comboBoxExpressions);

            comboBoxExpressions.SelectedIndexChanged += new System.EventHandler(ComboboxExpressionsSelected);
            comboBoxExpressions.LostFocus += new System.EventHandler(ComboboxExpressionsFocusOver);
            comboBoxExpressions.KeyPress += new System.Windows.Forms.KeyPressEventHandler(ComboboxExpressionsKeyPress);
            comboBoxExpressions.BackColor = Color.SkyBlue;
            comboBoxExpressions.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxExpressions.Hide();
        }

        private void ComboboxExpressionsKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 || e.KeyChar == 27)
            {
                comboBoxExpressions.Hide();
            }
        }

        private void ComboboxExpressionsSelected(object sender, System.EventArgs e)
        {
            int sel = comboBoxExpressions.SelectedIndex;
            if (sel >= 0)
            {
                string itemSel = comboBoxExpressions.Items[sel].ToString();
                listViewItem.SubItems[subItemIndex].Text = itemSel;
            }
        }

        private void ComboboxExpressionsFocusOver(object sender, System.EventArgs e)
        {
            comboBoxExpressions.Hide();
        }
        #endregion

        #region Edit box
        private void InitEditBox()
        {
            editBox.Size = new System.Drawing.Size(0, 0);
            editBox.Location = new System.Drawing.Point(0, 0);
            Controls.Add(editBox);

            editBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EditOver);
            editBox.LostFocus += new System.EventHandler(this.FocusOver);
            editBox.BackColor = Color.LightYellow;
            editBox.BorderStyle = BorderStyle.Fixed3D;
            editBox.Hide();
            editBox.Text = "";
        }

        private void EditOver(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                listViewItem.SubItems[subItemIndex].Text = editBox.Text;
                editBox.Hide();
            }

            if (e.KeyChar == 27)
                editBox.Hide();
        }

        private void FocusOver(object sender, System.EventArgs e)
        {
            listViewItem.SubItems[subItemIndex].Text = editBox.Text;
            editBox.Hide();
        }
        #endregion

        #region Edit actions
        /// <summary>
        /// Gets the clicked list view item and the x,y coordinates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpressionListView_MouseDown(object sender, MouseEventArgs e)
        {
            //  if a double-click
            if (e.Clicks == 2)
            {
                //  check the column clicked
                int xLeft = 0;
                int xRight = 0;
                for (int i = 0; i < Columns.Count; i++)
                {
                    xRight += Columns[i].Width;
                    if ((e.X > xLeft) && (e.X < xRight))
                    {
                        subItemIndex = i;
                        break;
                    }
                    xLeft = xRight;
                }

                //  get the list view item clicked
                listViewItem = GetItemAt(e.X, e.Y);
                //if ((listViewItem == null) && (subItemIndex != (int)columnIndices.Expression))
                //    return;

                if (listViewItem == null)
                {
                    listViewItem = new ListViewItem(new[] { "1", "Expression", "1", "1" });
                    this.Items.Add(listViewItem);
                }

                //  debug
                Console.WriteLine("SUB ITEM SELECTED = " + listViewItem.SubItems[subItemIndex].Text);

                //  handle edit start
                switch (Columns[subItemIndex].Text)
                {
                    case "Expression":
                        {
                            Rectangle r = new Rectangle(xLeft, listViewItem.Bounds.Y, xRight, listViewItem.Bounds.Bottom);
                            comboBoxExpressions.Size = new System.Drawing.Size(xRight - xLeft, listViewItem.Bounds.Bottom - listViewItem.Bounds.Top);
                            comboBoxExpressions.Location = new System.Drawing.Point(xLeft, listViewItem.Bounds.Y);
                            comboBoxExpressions.Show();
                            comboBoxExpressions.Text = listViewItem.SubItems[subItemIndex].Text;
                            comboBoxExpressions.SelectAll();
                            comboBoxExpressions.Focus();
                        }
                        break;

                    case "Output":
                        {
                            Rectangle r = new Rectangle(xLeft, listViewItem.Bounds.Y, xRight, listViewItem.Bounds.Bottom);
                            comboBox.Size = new System.Drawing.Size(xRight - xLeft, listViewItem.Bounds.Bottom - listViewItem.Bounds.Top);
                            comboBox.Location = new System.Drawing.Point(xLeft, listViewItem.Bounds.Y);
                            comboBox.Show();
                            comboBox.Text = listViewItem.SubItems[subItemIndex].Text;
                            comboBox.SelectAll();
                            comboBox.Focus();
                        }
                        break;

                    default:
                        {
                            Rectangle r = new Rectangle(xLeft, listViewItem.Bounds.Y, xRight, listViewItem.Bounds.Bottom);
                            editBox.Size = new System.Drawing.Size(xRight - xLeft, listViewItem.Bounds.Bottom - listViewItem.Bounds.Top);
                            editBox.Location = new System.Drawing.Point(xLeft, listViewItem.Bounds.Y);
                            editBox.Show();
                            editBox.Text = listViewItem.SubItems[subItemIndex].Text;
                            editBox.SelectAll();
                            editBox.Focus();
                        }
                        break;
                }
            }
        }

        #endregion
    }
}
