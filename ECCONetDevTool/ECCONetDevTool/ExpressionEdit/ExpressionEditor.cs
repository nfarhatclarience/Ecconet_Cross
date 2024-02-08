using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using ECCONetDevTool.LightEngineUserControls;

using ESG.ExpressionLib;
using ESG.ExpressionLib.DataModels;
using ESG.ExpressionLib.DataConverters;




namespace ECCONetDevTool.ExpressionEdit
{

    public partial class ExpressionEditor : UserControl
    {
        /// <summary>
        /// The binary generated delegate.
        /// </summary>
        public event FlashFileSystem.FlashFileSystem.BinaryGeneratedDelegate binaryGeneratedDelegate;

        /// <summary>
        /// The light engine user controls.
        /// </summary>
        private const string ucLightEngineCollectionName = "Light Engines";

        /// <summary>
        /// The current project expression collection
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
        private ExpressionCollection ProjectExpressionCollection
        {
            get => _projectExpressionCollection;
            set
            {
                //  set the collection
                _projectExpressionCollection = value;
                _projectExpressionCollection.Expressions.ListChanged += ProjectExpressions_ListChanged;
                ProjectExpressions_ListChanged(null, null);
                dgvExpressions.DataSource = ProjectExpressionCollection.Expressions;
                dgvExpressions.Columns["Sequencer"].Visible = false;
            }

        }
        private ExpressionCollection _projectExpressionCollection;

        /// <summary>
        /// The currently-selected light bar.
        /// </summary>
        private ProductAssemblyNode ProjectProductAssembly;


        /// <summary>
        /// The currently-selected expression.
        /// </summary>
        private Expression selectedExpression
        {
            get
            {
                try
                {
                    var items = dgvExpressions.SelectedRows;
                    if ((items != null) && (items.Count > 0) && (items[0].DataBoundItem is Expression exp))
                        return exp;
                }
                catch { }
                return null;
            }
        }

        /// <summary>
        /// The currently-selected area.
        /// </summary>
        private Expression.Area selectedArea
        {
            get { return lbxAreas.SelectedItem as Expression.Area; }
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public ExpressionEditor()
        {
            //  initialize designer components
            InitializeComponent();

            //  restore the user settings
            RestoreSettings();

            if (!DesignMode)
            {
                //  try to open the product assembly
                OpenProductAssembly(Properties.Settings.Default.ProjectProductAssemblyFileName);

                //  try to open the expression template collection
                OpenExpressionTemplateCollection(Properties.Settings.Default.ProjectExpressionTemplateCollectionFileName);

                //  try to open the expression collection
                OpenExpressionCollection(Properties.Settings.Default.ProjectExpressionCollectionFileName);

                //  converter tests
                DataModelConverterTests();

                //  populate the light engine user controls
                if (ProjectProductAssembly != null)
                    PopulateOutputIcons(ProjectProductAssembly);
            }
        }



        #region Converter tests
        /// <summary>
        /// Data model JSON and XML converter tests.
        /// </summary>
        private void DataModelConverterTests()
        {
            /*
            //  Pursuit Light bar JSON test
            ProductAssemblyNode lb0 = Models.BuildPursuitLightBar();
            ProductAssemblyConverters.ToJsonFile(lb0, "PursuitLightBar_Json.epa");
            ProductAssemblyNode lb1 = ProductAssemblyConverters.FromJsonFile("PursuitLightBar_Json.epa");

            //  Pursuit Light bar XML test
            ProductAssemblyNode lb2 = Models.BuildPursuitLightBar();
            ProductAssemblyConverters.ToXmlFile(lb2, "PursuitLightBar_Xml.epa");
            ProductAssemblyNode lb3 = ProductAssemblyConverters.FromXmlFile("PursuitLightBar_Xml.epa");
            */

            /*
            //  Serial Light bar JSON test
            ProductAssemblyNode lb0 = Models.BuildSerialLightBar();
            ProductAssemblyConverters.ToJsonFile(lb0, "SerialLightBar_Json.epa");
            ProductAssemblyNode lb1 = ProductAssemblyConverters.FromJsonFile("SerialLightBar_Json.epa");
            
            //  Serial Light bar XML test
            ProductAssemblyNode lb2 = Models.BuildSerialLightBar();
            ProductAssemblyConverters.ToXmlFile(lb2, "SerialLightBar_Xml.epa");
            ProductAssemblyNode lb3 = ProductAssemblyConverters.FromXmlFile("SerialLightBar_Xml.epa");
            */

            /*
            //  Serial Siren bar JSON test
            ProductAssemblyNode lb0 = ProductAssemblyConverters.FromXmlFile("SerialSiren.epa");
            ProductAssemblyConverters.ToJsonFile(lb0, "SerialSiren_Json.epa");
            ProductAssemblyNode lb1 = ProductAssemblyConverters.FromJsonFile("SerialSiren_Json.epa");

            //  Serial Siren bar XML test
            ProductAssemblyNode lb2 = ProductAssemblyConverters.FromXmlFile("SerialSiren.epa");
            ProductAssemblyConverters.ToXmlFile(lb2, "SerialSiren_Xml.epa");
            ProductAssemblyNode lb3 = ProductAssemblyConverters.FromXmlFile("SerialSiren_Xml.epa");
            */

            /*
            //  Expression collection JSON test
            //ExpressionCollection ec0 = NamedQuadFlashPatterns.Build();
            ExpressionCollection ec0 = ExpressionConverters.FromXmlFile("ExpressionTemplateCollection.ec");
            ExpressionConverters.ToJsonFile(ec0, "ExpressionTemplateCollection_Json.ec");
            ExpressionCollection ec1 = ExpressionConverters.FromJsonFile("ExpressionTemplateCollection_Json.ec");

            //  Expression collection XML test
            //ExpressionCollection ec2 = NamedQuadFlashPatterns.Build();
            ExpressionCollection ec2 = ExpressionConverters.FromXmlFile("ExpressionTemplateCollection.ec");
            ExpressionConverters.ToXmlFile(ec2, "ExpressionTemplateCollection_Xml.ec");
            ExpressionCollection ec3 = ExpressionConverters.FromXmlFile("ExpressionTemplateCollection_Xml.ec");
            */

            /*
            //  bytecode converter test
            var equations = File.ReadAllText("equations.txt");
            try
            {
                var bytecode = EquationConverters.ToBytecode(new List<string> { equations });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            */

        }
        #endregion

        #region Populate light engine controls

        /// <summary>
        /// Tier selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbbTier_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateOutputIcons(ProjectProductAssembly);
        }

        /// <summary>
        /// Updates the light engine ID diplsay to be the location or the short ID.
        /// </summary>
        /// <param name="showId">True to show the short ID.</param>
        private void UpdateLightEngineIdDisplays(bool showId)
        {
            var control = this.Controls[ucLightEngineCollectionName];
            if (control != null)
            {
                foreach (var ctrl in control.Controls)
                    if (ctrl is ucIconLightEngine le)
                        le.ShowId = showId;
            }
        }

        /// <summary>
        /// Populates the light engine user controls.
        /// </summary>
        /// <param name="assembly">The light bar that the user controls represent.</param>
        private void PopulateOutputIcons(ProductAssemblyNode assembly)
        {
            //  remove any previous light engine user controls
            this.Controls.RemoveByKey(ucLightEngineCollectionName);

            //  create new control to hold light engines
            Control ucLightEngines = new Control(assembly.ModelName);
            ucLightEngines.Name = ucLightEngineCollectionName;
            ucLightEngines.Size = new Size(1150, 300);
            ucLightEngines.Location = new Point(0, 0);

            //  find the first sequenced output array
            OutputArrayNode outputArray = (OutputArrayNode)ComponentTreeNode.FirstType(assembly, typeof(OutputArrayNode));
            if (outputArray == null)
                return;

            //  create user control for light engine
            foreach (var childnode in outputArray.ChildNodes)
            {
                if (childnode.Location.Z.ToString() == (string)cbbTier.SelectedItem)
                {
                    ucIconLightEngine ucLE = new ucIconLightEngine();
                    ucLE.OutputNode = childnode;
                    ucLE.AreaAssignmentChanged += AreaAssignmentChangedHandler;
                    ucLightEngines.Controls.Add(ucLE);
                }
            }

            //  add the control
            Controls.Add(ucLightEngines);

            //  set the light engine user control locations
            UpdateLightEngineIconLocations();

            //  update the light engine ID displays
            UpdateLightEngineIdDisplays(cbShowIds.Checked);
        }

        /// <summary>
        /// Updates the light engine icon sizes and locations.
        /// </summary>
        private void UpdateLightEngineIconLocations()
        {
            //  get the controls
            var lightEngines = Controls[ucLightEngineCollectionName];
            if (lightEngines != null)
            {
                //  set params
                if (!ntbXScale.GetSingleValue(out float xScale))
                    xScale = 64.0f;
                if (!ntbYScale.GetSingleValue(out float yScale))
                    yScale = 20.0f;
                if (!ntbXOffset.GetSingleValue(out float xOffset))
                    xOffset = 0f;
                if (!ntbYOffset.GetSingleValue(out float yOffset))
                    yOffset = 0f;
                SizeF locationSpread = new SizeF(xScale, yScale);
                Point locationCenter = new Point(350 + (int)xOffset, 100 + (int)yOffset);

                //  for all light engines
                foreach (Control ctl in lightEngines.Controls)
                {
                    if (ctl is ucIconLightEngine ucLE)
                    {
                        if (ucLE.OutputNode is ComponentTreeNode node)
                        {
                            double y = 0;
                            if ((node.ParentNode is OutputArrayNode parentNode) && "S0".Equals(parentNode.OutputArrayType))
                                y = node.Location.Y;
                            else  //  S4 or S6 output array, such as Light Engine, then get the light engine y based on the angle
                                y = Math.Cos(node.Location.Angle * Math.PI / 180) * 4;

                            //  set size
                            if (node.IsEndpoint)
                                ucLE.Size = new Size(60, 40);
                            else
                                ucLE.Size = new Size(60, (node.ChildNodes.Count * 20) + 20);
                            ucLE.tbID.Location = new Point(ucLE.Size.Width / 2 - 34, 0);
                            ucLE.ucIconLEDColor1.Location = new Point(ucLE.Size.Width / 2 - 33, 20);
                            ucLE.ucIconLEDColor2.Location = new Point(ucLE.Size.Width / 2 - 33, 40);
                            ucLE.ucIconLEDColor3.Location = new Point(ucLE.Size.Width / 2 - 33, 60);

                            //  set locatoin
                            ucLE.Location = new Point(
                                (int)(float)(node.Location.X * locationSpread.Width) + locationCenter.X,
                                (int)(float)(y * -locationSpread.Height) + locationCenter.Y);

                        }
                    }
                }
            }
        }
        #endregion

        #region LED area assignment message event handler
        /// <summary>
        /// The method to handle individual LED area assignment changed notifications.
        /// </summary>
        /// <param name="sender">The individual LED whose assignment changed.</param>
        /// <param name="isAssigned">A value indicating whether the user assigned the LED to the current area.</param>
        private void AreaAssignmentChangedHandler(object sender, bool isAssigned)
        {
            //  if inputs are valid
            if ((ProjectProductAssembly != null) && (selectedArea != null) && (sender is PathValue outputPath))
            {
                //  remove all matching paths in area
                BindingList<PathValue> outputPaths = selectedArea.OutputPaths;
                int i = 0;
                while (i < outputPaths.Count)
                {
                    if (outputPath.PathEquals(outputPaths[i]))
                        outputPaths.RemoveAt(i);
                    else
                        ++i;
                }

                //  if path is assigned to area, add the path
                if (isAssigned)
                {
                    //  get copy of path, and for this DevTool set intensity to 100%
                    var pathCopy = outputPath.DeepCopy();
                    pathCopy.Value = 100;
                    outputPaths.Add(pathCopy);
                }

                //  update the expression areas listbox
                UpdateExpressionAreaListBox();
            }
        }
        #endregion

        #region Expression templates

        private void btnOpenExpressionTemplateCollection_Click(object sender, EventArgs e)
        {
            if (openFileDialogExpressionCollection.ShowDialog() == DialogResult.OK)
            {
                OpenExpressionTemplateCollection(openFileDialogExpressionCollection.FileName);
            }
        }

        /// <summary>
        /// Open an expression template collection with the given name.
        /// </summary>
        /// <param name="filename">The file name to open.</param>
        private void OpenExpressionTemplateCollection(string filename)
        {
            try
            {
                //  open from XML or JSON file
                var ec = ExpressionConverters.FromFile(filename);
                if (ec != null)
                {
                    ExpressionTemplateCollection = ec;
                    lblExpressionTemplateFile.Text = Path.GetFileName(filename);
                    Properties.Settings.Default.ProjectExpressionTemplateCollectionFileName = filename;
                    if (dgvExpressions.Columns[1] is DataGridViewComboBoxColumn column)
                    {
                        column.DataSource = ec.Expressions;
                        column.ValueMember = "Name";
                        column.DisplayMember = "Name";
                    }
                }
            }
            catch
            {
                //  dev
            }
        }

        /// <summary>
        /// The current expression template collection.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ExpressionCollection ExpressionTemplateCollection
        {
            get => _expressionTemplateCollection;
            set
            {
                //  set the collection
                _expressionTemplateCollection = value;

                //  bind to expression templates combo box
                //  (Expression ToString() override is also just Name)
                cbbExpressionTemplate.DataSource = value.Expressions;
                cbbExpressionTemplate.DisplayMember = "Name";

                //  show expression template collection name
                lblExpressionTemplateFile.Text = value.Name;

                //  update the expression area listbox
                UpdateExpressionTemplateAreaListBox();
            }

        }
        ExpressionCollection _expressionTemplateCollection;

        /// <summary>
        /// The currently-selected expression template.
        /// </summary>
        private Expression selectedExpressionTemplate
        {
            get { return cbbExpressionTemplate.SelectedItem as Expression; }
        }

        /// <summary>
        /// Expression selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbbExpressionTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateExpressionTemplateAreaListBox();
        }

        /// <summary>
        /// Add template button pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddExpressionTemplateToProject();
        }

        /// <summary>
        /// Updates the template area list for the currently-selected expression template.
        /// </summary>
        private void UpdateExpressionTemplateAreaListBox()
        {
            lbxTemplateAreas.DataSource = null;
            lbxTemplateAreas.DisplayMember = "NameWithLEDPaths";
            if (selectedExpressionTemplate is Expression exp)
            {
                lbxTemplateAreas.SelectionMode = SelectionMode.One;
                lbxTemplateAreas.DataSource = exp.Areas;
                //tbNewTemplateName.Text = exp.Name;
                lbxTemplateAreas.SelectionMode = SelectionMode.None;
            }
        }

        /// <summary>
        /// Adds the selected expression template to the project.
        /// </summary>
        private void AddExpressionTemplateToProject()
        {
            if ((cbbExpressionTemplate.SelectedItem is Expression exp) && (ProjectExpressionCollection != null))
            { 
                //  get copy of expression template
                Expression expCopy = exp.Copy();
                //expCopy.Name = tbNewTemplateName.Text;

                //  remove any area light engine assignments
                expCopy.RemoveAllOutputPaths();

                //  add to project expressions
                ProjectExpressionCollection.Expressions.Add(expCopy);
            }
        }
        #endregion

        #region Project product assembly

        /// <summary>
        /// Button open product assembly file clicked,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenProductAssemblyFile_Click(object sender, EventArgs e)
        {
            if (openFileDialogProductAssembly.ShowDialog() == DialogResult.OK)
            {
                OpenProductAssembly(openFileDialogProductAssembly.FileName);
            }
        }

        /// <summary>
        /// Open the product assembly with the given file name.
        /// </summary>
        /// <param name="filename">The file name of the product assembly.</param>
        private void OpenProductAssembly(string filename)
        {
            try
            {
                //  get product assembly
                ProjectProductAssembly = ProductAssemblyConverters.FromFile(filename);

                if (ProjectProductAssembly != null)
                {
                    //  test only
                    //ProductAssemblyConverters.ToJsonFile(ProjectProductAssembly, "C:/Users/Eng4/Desktop/MyJsonTest.epa");

                    //  show the assembly and save user settings
                    lblProductAssemblyFile.Text = Path.GetFileName(filename);
                    Properties.Settings.Default.ProjectProductAssemblyFileName = filename;

                    //  find the first sequenced output array
                    OutputArrayNode outputArray = (OutputArrayNode)ComponentTreeNode.FirstType(ProjectProductAssembly, typeof(OutputArrayNode));
                    if ((outputArray != null) && (outputArray.ChildNodes.Count > 0))
                    {
                        //  set the tier, which will populate the output icons
                        int z = outputArray.ChildNodes[0].Location.Z;
                        if ((z >= 0) && (z <= 3))
                            cbbTier.SelectedIndex = z;
                        else
                            cbbTier.SelectedIndex = 0;
                    }

                    //  populate the icons
                    PopulateOutputIcons(ProjectProductAssembly);
                }
            }
            catch
            {
                //  dev console in lib already has messages
                //  GUI would handle this exception
            }
        }
        #endregion

        #region Project expression collection

        /// <summary>
        /// An expression was added or deleted from project expressions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectExpressions_ListChanged(object sender, ListChangedEventArgs e)
        {
            //  update the selected expression's areas listbox
            UpdateExpressionAreaListBox();
        }


        /// <summary>
        /// Expression in data grid view was clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExpressions_SelectionChanged(object sender, EventArgs e)
        {
            UpdateExpressionAreaListBox();
        }


        /// <summary>
        /// Button to clear all LEDs was clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearLeds_Click(object sender, EventArgs e)
        {
            if (selectedExpression is Expression exp)
            {
                exp.RemoveAllOutputPaths();
                UpdateExpressionAreaListBox();
            }
        }

        /// <summary>
        /// Button to remove expression was clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            //  remove the expression from the collection
            if ((selectedExpression != null) && (ProjectExpressionCollection != null))
                ProjectExpressionCollection.Expressions.Remove(selectedExpression);
        }


        /// <summary>
        /// Updates the expression area list for the currently-selected expression.
        /// </summary>
        void UpdateExpressionAreaListBox()
        {
            lbxAreas.DataSource = null;
            lbxAreas.DisplayMember = null;  // use ToString() "NameWithLEDPaths";
            if (selectedExpression != null)
                lbxAreas.DataSource = selectedExpression.Areas;

            //  update the LED selections
            UpdateLightBarForExpressionArea();
        }
        #endregion

        #region Project expression areas

        private void lbxAreas_SelectedIndexChanged(object sender, EventArgs e)
        {
            //  update the LED selection display
            UpdateLightBarForExpressionArea();
        }

        /// <summary>
        /// Updates the light bar to show the LEDs included in the selected expression selected area.
        /// </summary>
        private void UpdateLightBarForExpressionArea()
        {
            if (this.Controls[ucLightEngineCollectionName] is Control ucLightEngines)
            {
                foreach (Control control in ucLightEngines.Controls)
                {
                    if (control is ucIconLightEngine ucLE)
                        ucLE.UpdateForArea(selectedArea);
                }
            }
        }
        #endregion

        #region Project expression intensity

        /// <summary>
        /// Intensity map.
        /// </summary>
        static readonly byte[] IntensityMap = new byte[8] { 0, 10, 20, 30, 40, 60, 80, 100 };

        /// <summary>
        /// Convert intensity to index.
        /// </summary>
        /// <param name="intensity"></param>
        /// <returns></returns>
        private int IntensityToIndex(byte intensity)
        {
            int index = 0;
            for (; index < (IntensityMap.Length - 1); ++index)
                if (intensity < IntensityMap[index + 1])
                    break;
            return index;
        }

        /// <summary>
        /// Convert index to intensity.
        /// </summary>
        /// <param name="intensity"></param>
        /// <returns></returns>
        private byte IndexToIntensity(int index)
        {
            if (index >= IntensityMap.Length)
                index = IntensityMap.Length - 1;
            return IntensityMap[index];
        }

        /* not used anymore
        /// <summary>
        /// Update the intensity combobox with the current expression's intensity. 
        /// </summary>
        private void UpdateIntensityComboBox()
        {
            //  if valid selection
            if (selectedExpression is Expression exp)
                cbbIntensity.SelectedIndex = IntensityToIndex(exp.Value);
        }

        private void cbbIntensity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedExpression is Expression exp)
            {
                exp.Value = IndexToIntensity(cbbIntensity.SelectedIndex);
            }
        }
        */
        #endregion

        #region Open and save collection
        private void btnSaveCollectionJson_Click(object sender, EventArgs e)
        {
            if ((ProjectExpressionCollection != null) && (ProjectExpressionCollection.Expressions.Count > 0))
            //if ((lvExpressions.Items.Count > 0) && (ProjectExpressionCollection != null))
            {
                if (saveFileDialogExpressionCollection.ShowDialog() == DialogResult.OK)
                {
                    //  save to JSON format
                    try
                    {
                        ExpressionConverters.ToJsonFile(ProjectExpressionCollection, saveFileDialogExpressionCollection.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void btnSaveCollectionXml_Click(object sender, EventArgs e)
        {
            if ((ProjectExpressionCollection != null) && (ProjectExpressionCollection.Expressions.Count > 0))
            //if ((lvExpressions.Items.Count > 0) && (ProjectExpressionCollection != null))
            {
                if (saveFileDialogExpressionCollection.ShowDialog() == DialogResult.OK)
                {
                    //  save to XML format
                    try
                    {
                        ExpressionConverters.ToXmlFile(ProjectExpressionCollection, saveFileDialogExpressionCollection.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }


        private void btnOpenCollection_Click(object sender, EventArgs e)
        {
            if (openFileDialogExpressionCollection.ShowDialog() == DialogResult.OK)
            {
                OpenExpressionCollection(openFileDialogExpressionCollection.FileName);
            }
        }

        private void OpenExpressionCollection(string filename)
        {
            try
            {
                //  open from XML or JSON file
                ProjectExpressionCollection = ExpressionConverters.FromFile(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //  if no expression collection, then start a new one
            if (ProjectExpressionCollection == null)
                ProjectExpressionCollection = new ExpressionCollection("New Expression Collection", new BindingList<Expression>());

            //  show the file name and save the settings
            lblExpressionFile.Text = Path.GetFileName(filename);
            Properties.Settings.Default.ProjectExpressionCollectionFileName = filename;
        }
        #endregion

        #region Build and save binary files
        /// <summary>
        /// Button build step method bin files clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBuildExpressionBinFiles_Click(object sender, EventArgs e)
        {
            try
            {
                //  try to find the first available output array
                OutputArrayNode outputArray = (OutputArrayNode)ComponentTreeNode.FirstType(ProjectProductAssembly, typeof(OutputArrayNode));
                if (outputArray == null)
                {
                    MessageBox.Show("Cannot build bin files for device that does not have an output array.");
                    return;
                }

                //  convert the expression and outputs to bin files
                ExpressionConverters.BuildExpressionCollectionStepMethodBinFiles(ProjectExpressionCollection, outputArray,
                    out byte[] binPatternTable, out byte[] binDictionaries);


                //  show process complete
                if (binPatternTable == null)
                {
                    MessageBox.Show("Unknown error generating bin files.");
                }
                else
                { 
                    if (binDictionaries == null)
                        MessageBox.Show("Expressions pattern table bin file built successfully.");
                    else
                        MessageBox.Show("Expressions pattern table and dictionary bin files built successfully.");
                }

                //  if saving to local files
                if (cbSaveToLocalFile.Checked)
                {
                    //  save the pattern bin file
                    if ((binPatternTable != null) && (saveFileDialogPatternBin.ShowDialog() == DialogResult.OK))
                        File.WriteAllBytes(saveFileDialogPatternBin.FileName, binPatternTable);

                    //  save the dictionary bin file
                    if ((binDictionaries != null) && (saveFileDialogLightEngineDictionaryBin.ShowDialog() == DialogResult.OK))
                        File.WriteAllBytes(saveFileDialogLightEngineDictionaryBin.FileName, binDictionaries);
                }

                //  if saving to flash file system
                if (cbSaveToFlashFileSystem.Checked && (binaryGeneratedDelegate != null))
                {
                    //  determine if processing unison outputs
                    bool areUnisonOutputs = ComponentTreeNode.IsNodeTimerDictionaryOutputArray(outputArray);

                    //  send pattern bin to delegate
                    if (binPatternTable != null)
                        binaryGeneratedDelegate(this, "patterns.tbl", binPatternTable, ProjectExpressionCollection, 0);

                    //  send dictionary bin to delegate
                    if (binDictionaries != null)
                        binaryGeneratedDelegate(this, "lighteng.dct", binDictionaries, null, (uint)(areUnisonOutputs ? 6 : 4));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        #endregion

        #region Save and restore settings
        /// <summary>
        /// User clicked save to flash file system checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSaveToFlashFileSystem_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.BinSaveToFlashFileSystem = cbSaveToFlashFileSystem.Checked;
        }

        /// <summary>
        /// User clicked save to local file checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSaveToLocalFile_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.BinSaveToLocalFile = cbSaveToLocalFile.Checked;
        }

        /// <summary>
        /// Show light engine IDs checkbox clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbShowIds_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowLightEngineIds = cbShowIds.Checked;
            UpdateLightEngineIdDisplays(cbShowIds.Checked);
        }

        /// <summary>
        /// User changed the display x scale.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ntbXScale_TextChanged(object sender, EventArgs e)
        {
            UpdateLightEngineIconLocations();
            Properties.Settings.Default.LeIconXScale = ntbXScale.Text;
        }

        /// <summary>
        /// User changed the display y scale.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ntbYScale_TextChanged(object sender, EventArgs e)
        {
            UpdateLightEngineIconLocations();
            Properties.Settings.Default.LeIconYScale = ntbYScale.Text;
        }

        /// <summary>
        /// User changed the display x offset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ntbXOffset_TextChanged(object sender, EventArgs e)
        {
            UpdateLightEngineIconLocations();
            Properties.Settings.Default.LeIconXOffset = ntbXOffset.Text;
        }

        /// <summary>
        /// User changed the display y offset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ntbYOffset_TextChanged(object sender, EventArgs e)
        {
            UpdateLightEngineIconLocations();
            Properties.Settings.Default.LeIconYOffset = ntbYOffset.Text;
        }

        /// <summary>
        /// Restore the pattern generation settings.
        /// </summary>
        private void RestoreSettings()
        {
            cbSaveToFlashFileSystem.Checked = Properties.Settings.Default.BinSaveToFlashFileSystem;
            cbSaveToLocalFile.Checked = Properties.Settings.Default.BinSaveToLocalFile;
            cbShowIds.Checked = Properties.Settings.Default.ShowLightEngineIds;
            ntbXScale.Text = Properties.Settings.Default.LeIconXScale;
            ntbYScale.Text = Properties.Settings.Default.LeIconYScale;
            ntbXOffset.Text = Properties.Settings.Default.LeIconXOffset;
            ntbYOffset.Text = Properties.Settings.Default.LeIconYOffset;
        }
        #endregion

        #region Blend test
        private void btnTest_Click(object sender, EventArgs e)
        {
            var ec = ExpressionConverters.FromXmlFile("NineExpressionTest.ec");
            if (ec != null)
            {
                List<Expression> expressions = new List<Expression>(ec.Expressions);
                var blendedExpression = ExpressionConverters.Blend(expressions, 0.037, 12000, 1);
                blendedExpression.Name = "Nine Expressions Blended";
                var blendedEc = new ExpressionCollection("Nine Expressions Blended", new BindingList<Expression>() { blendedExpression });
                ExpressionConverters.ToXmlFile(blendedEc, "NineExpressionBlended.ec");
            }

            ///uint bestPeriod = ExpressionConverters.FindBestCommonPeriod(null, 15000);
        }



        #endregion

        #region Blend expressions
        private void btnBlend_Click(object sender, EventArgs e)
        {
            var form = new FormNameBlend();
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    List<Expression> expressions = new List<Expression>(10);

                    for (int i = 0; i < dgvExpressions.Rows.Count; ++i)
                    {
                        if (dgvExpressions.Rows[i].Cells["SelectRow"] is DataGridViewCheckBoxCell checkbox)
                        {
                            if (Convert.ToBoolean(checkbox.Value) == true)
                            {
                                //  add expression to list
                                if (dgvExpressions.Rows[i].DataBoundItem is Expression expression)
                                    expressions.Add(expression);

                                //  remove list view item
                                dgvExpressions.Rows.RemoveAt(i);
                                --i;
                            }
                        }
                    }

                    //  blend list of expressions and add to list view as first list
                    Expression exp = ExpressionConverters.Blend(expressions, 0.04, 12000, 1);
                    exp.Name = form.tbExpressionName.Text;
                    exp.InputPriority = 1;
                    exp.OutputPriority = 1;
                    ProjectExpressionCollection.Expressions.Insert(0, exp);
                    //UpdateExpressionsListView();

                    //  show expression results
                    var resultsForm = new FormBlendResults();
                    resultsForm.ShowResults(exp);
                    resultsForm.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion

        #region Data grid view data error event
        private void dgvExpressions_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }
        #endregion

    }
}

#if UNUSED_CODE


            /*
            //  initialize list view
            lvExpressions.View = View.Details;
            lvExpressions.Columns.Add("Description", 230, HorizontalAlignment.Center);
            lvExpressions.Columns.Add("Output Priority", 80, HorizontalAlignment.Center);
            lvExpressions.Columns.Add("Input Priority", 70, HorizontalAlignment.Center);
            */


        /// <summary>
        /// An expression was added or deleted from project expressions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectExpressions_ListChanged(object sender, ListChangedEventArgs e)
        {
            //  update the selected expression's areas listbox, priority combobox and intensity combobox
            //UpdateExpressionsListView();
            UpdateExpressionAreaListBox();
            //UpdatePriorityComboBoxes();
            //UpdateIntensityComboBox();
        }

        /*  no longer used
        /// <summary>
        /// Expression in list box was clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvExpressions_SelectedIndexChanged(object sender, EventArgs e)
        {
            //  keep highlighting
            foreach (ListViewItem item in lvExpressions.Items)
            {
                if (item.Selected)
                {
                    item.ForeColor = Color.White;
                    item.BackColor = Color.DodgerBlue;
                }
                else
                {
                    item.ForeColor = Color.Black;
                    item.BackColor = Color.White;
                }
            }

            //  update the selected expression's areas listbox, priority combobox and intensity combobox
            UpdateExpressionAreaListBox();
            //UpdatePriorityComboBoxes();
            //UpdateIntensityComboBox();

            //  keep highlighting
            foreach (ListViewItem item in lvExpressions.Items)
            {
                if (item.Selected)
                    item.Focused = true;
            }
        }
        */

        /*
        /// <summary>
        /// Updates the expressions list for the currently-selected expression collection.
        /// </summary>
        void UpdateExpressionsListBox()
        {
            if (ProjectExpressionCollection is ExpressionCollection ec)
            {
                lbxExpressions.DataSource = null;
                lbxAreas.DisplayMember = null;  // to use ToString() override
                lbxExpressions.DataSource = ec.Expressions;
            }
        }
        */

        /* not used anymore
        /// <summary>
        /// Updates the expressions list for the currently-selected expression collection.
        /// </summary>
        void UpdateExpressionsListView()
        {
            lvExpressions.Items.Clear();
            try
            {
                if ((ProjectExpressionCollection != null) &&
                    (ProjectExpressionCollection.Expressions != null) && (ProjectExpressionCollection.Expressions.Count > 0))
                {
                    foreach (var exp in ProjectExpressionCollection.Expressions)
                    {
                        lvExpressions.Items.Add(new ListViewItem(
                            new[] { exp.Name, exp.OutputPriority.ToString(), exp.InputPriority.ToString() })
                        { Tag = exp });
                    }
                    lvExpressions.Items[0].Focused = true;
                    lvExpressions.Items[0].Selected = true;
                }
            }
            catch
            {
                // dev
            }
        }
        */

        /// <summary>
        /// Gets the Y position based on the given angle.
        /// </summary>
        /// <param name="angle">The light engine ID angle.</param>
        /// <returns>Returns the Y position based on the given angle.</returns>
        private int GetLightEngineY(float angle)
        {
            float radians = angle * Math.PI / 180;

            int y = 0;
            if ((angle < 45.0) || (angle > 315.0))
                y = 4;
            else if ((angle < 90.0) || (angle > 270.0))
                y = 3;
            else if ((angle > 135.0) && (angle < 225.0))
                y = -4;
            else if ((angle > 90.0) && (angle < 270.0))
                y = -3;
            return y;
        }


        private void UpdateLightEngineIconLocations()
        {
            //  get the controls
            var lightEngines = Controls[ucLightEngineCollectionName];
            if (lightEngines != null)
            {
                //  set params
                if (!ntbXScale.GetSingleValue(out float xScale))
                    xScale = 64.0f;
                if (!ntbYScale.GetSingleValue(out float yScale))
                    yScale = 20.0f;
                SizeF locationSpread = new SizeF(xScale, yScale);
                Point locationCenter = new Point(350, 120);

                /*
                //  y offsets
                int yTier3_Offset = 0;   // unused
                int yTier2_Offset = 0;   //  unused
                int yTier1_Offset = -20;
                int yTier0_Offset = 20;

                //  x offsets
                int xTier3_Offset = -32;
                int xTier2_Offset = -32;
                int xTier1_Offset = -32;
                int xTier0_Offset = -32;
                */

                //  for all light engines
                foreach (Control ctl in lightEngines.Controls)
                {
                    if (ctl is ucIconLightEngine ucLE)
                    {
                        if (ucLE.OutputNode is ComponentTreeNode node)
                        {
                            //  get the light engine y based on the angle
                            int y = GetLightEngineY(node.Location.Angle);

                            //  create user control for light engine
                            if (node.IsEndpoint)
                                ucLE.Size = new Size(60, 40);
                            else
                                ucLE.Size = new Size(60, (node.ChildNodes.Count * 20) + 20);
                            ucLE.tbID.Location = new Point(ucLE.Size.Width / 2 - 34, 0);
                            ucLE.ucIconLEDColor1.Location = new Point(ucLE.Size.Width / 2 - 33, 20);
                            ucLE.ucIconLEDColor2.Location = new Point(ucLE.Size.Width / 2 - 33, 40);
                            ucLE.ucIconLEDColor3.Location = new Point(ucLE.Size.Width / 2 - 33, 60);

                            //  calculate location offsets
                            int xOffset = 0;
                            int yOffset = 0;
                            /*
                            switch (node.Location.Z)
                            {
                                case 3: xOffset = xTier3_Offset; yOffset = yTier3_Offset; break;
                                case 2: xOffset = xTier2_Offset; yOffset = yTier2_Offset; break;
                                case 1: xOffset = xTier1_Offset; yOffset = yTier1_Offset; break;
                                case 0: xOffset = xTier0_Offset; yOffset = yTier0_Offset; break;
                            }
                            */

                            //  x offset zero or sign
                            if (node.Location.X == 0)
                                xOffset = 0;
                            else if (node.Location.X < 0)
                                xOffset = -xOffset;

                            //  y offset zero or sign
                            //if (y == 0)
                            //    //yOffset -= yTier1_Offset;
                            if (y < 0)
                                yOffset = -yOffset;

                            ucLE.Location = new Point(
                                (int)(float)(node.Location.X * locationSpread.Width) + locationCenter.X + xOffset,
                                (int)(float)(y * -locationSpread.Height) + locationCenter.Y - yOffset);

                            //  hook up the area assignment changed events
                            ucLE.AreaAssignmentChanged += AreaAssignmentChangedHandler;
                        }
                    }
                }
            }
        }

#region Blend expressions
        private void btnBlend_Click(object sender, EventArgs e)
        {
            var form = new FormNameBlend();
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    List<Expression> expressions = new List<Expression>(10);

                    for (int i = 0; i < dgvExpressions.Rows.Count; ++i)
                    {
                        if ((dgvExpressions.Rows[i].Cells["Select"] is DataGridViewCheckBoxCell checkbox)
                            && (checkbox.Value == checkbox.TrueValue))
                        {
                            //  add expression to list
                            if (dgvExpressions.Rows[i].DataBoundItem is Expression expression)
                                expressions.Add(expression);

                            //  remove list view item
                            dgvExpressions.Rows.RemoveAt(i);
                            --i;
                        }
                    }

                    /*
                    for (int i = 0; i < lvExpressions.Items.Count; ++i)
                    {
                        if (lvExpressions.Items[i].Checked)
                        {
                            //  add expression to list
                            if (lvExpressions.Items[i].Tag is Expression expression)
                                expressions.Add(expression);

                            //  remove list view item
                            lvExpressions.Items.RemoveAt(i);
                            --i;
                        }
                    }
                    */

                    //  blend list of expressions and add to list view as first list
                    Expression exp = ExpressionConverters.Blend(expressions, 0.04, 12000, 1);
                    exp.Name = form.tbExpressionName.Text;
                    exp.InputPriority = 1;
                    exp.OutputPriority = 1;
                    ProjectExpressionCollection.Expressions.Insert(0, exp);
                    //UpdateExpressionsListView();

                    //  show expression results
                    var resultsForm = new FormBlendResults();
                    resultsForm.ShowResults(exp);
                    resultsForm.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
#endregion



#region Project expression input and output priority

        /* not used anymore
        /// <summary>
        /// Update the input and output priority comboboxes with the current expression's priorities. 
        /// </summary>
        private void UpdatePriorityComboBoxes()
        {
            //  if valid selection
            if (selectedExpression is Expression exp)
            {
                //  input priority in the range 0-100, where 100 is highest priority
                byte priority = exp.InputPriority;
                if (priority < 1) priority = 1;
                if (priority > 100) priority = 100;
                cbbInputPriority.SelectedIndex = priority - 1;

                //  output priority in the range 1-6, where 6 is the highest priority
                priority = exp.OutputPriority;
                if (priority < 1) priority = 1;
                if (priority > 6) priority = 6;
                cbbOutputPriority.SelectedIndex = priority - 1;
            }
        }

        /// <summary>
        /// Output priority selected index changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbbInputPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            //  get selected items
            var indices = lvExpressions.SelectedIndices;
            if ((indices != null) && (indices.Count > 0))
            {
                int index = indices[0];
                if (lvExpressions.Items[index].Tag is Expression exp)
                {
                    //  update the expression input priority
                    exp.InputPriority = (byte)(cbbInputPriority.SelectedIndex + 1);

                    //  update text in list view
                    lvExpressions.Items[index].SubItems[2].Text = exp.InputPriority.ToString();
                }
            }
        }
        */

        /* not used anymore
        /// <summary>
        /// Output priority selected index changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbbOutputPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            //  get selected items
            var indices = lvExpressions.SelectedIndices;
            if ((indices != null) && (indices.Count > 0))
            {
                int index = indices[0];
                if (lvExpressions.Items[index].Tag is Expression exp)
                {
                    //  update the expression output priority
                    exp.OutputPriority = (byte)(cbbOutputPriority.SelectedIndex + 1);

                    //  update text in list view
                    lvExpressions.Items[index].SubItems[1].Text = exp.OutputPriority.ToString();
                }
            }

            
            if (selectedExpression is Expression exp)
            {
                exp.OutputPriority = (byte)(cbbOutputPriority.SelectedIndex + 1);
                UpdateExpressionsListView();
            }
            
        }
        */

#endregion


        /*
        /// <summary>
        /// Set the project expresssions collection.
        /// </summary>
        /// <param name="ec"></param>
        private void SetProjectExpressionCollection(ExpressionCollection ec)
        {
            //  validate collection
            if ((ec == null) || (ec.Expressions == null) || (ec.Expressions.Count == 0))
                return;

            //  replace collection in list box
            lbxExpressions.Items.Clear();
            foreach (Expression exp in ec.Expressions)
                lbxExpressions.Items.Add(exp);

            //  set the selected item
            if (lbxExpressions.Items.Count > 0)
                lbxExpressions.SelectedIndex = 0;

            //  update the expression area list
            UpdateExpressionAreaList();
        }
        */

        /*
        /// <summary>
        /// Updates all the light engine icons with the currently-selected expression.
        /// This allows the icons to know the expression when they are clicked.
        /// </summary>
        void UpdateLightEngineIconExpressions()
        {
            if ((cbbExpressionTemplate.Items.Count > 0) && (ucLightEngines != null))
            {
                foreach (ucIconLightEngine icon in ucLightEngines)
                    icon.Expression = cbbExpressionTemplate.SelectedItem as Expression;
            }
        }
        */




#endif


