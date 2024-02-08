namespace ECCONetDevTool.ExpressionEdit
{
    partial class ExpressionEditor
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
            this.cbbExpressionTemplate = new System.Windows.Forms.ComboBox();
            this.btnAddTemplate = new System.Windows.Forms.Button();
            this.lblExpressionTemplateFile = new System.Windows.Forms.Label();
            this.lbxTemplateAreas = new System.Windows.Forms.ListBox();
            this.gbExpressionTemplates = new System.Windows.Forms.GroupBox();
            this.btnOpenExpressionTemplateCollection = new System.Windows.Forms.Button();
            this.lbxAreas = new System.Windows.Forms.ListBox();
            this.btnOpenCollection = new System.Windows.Forms.Button();
            this.btnSaveCollectionJson = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbSaveToLocalFile = new System.Windows.Forms.CheckBox();
            this.cbSaveToFlashFileSystem = new System.Windows.Forms.CheckBox();
            this.btnBuildExpressionBinFiles = new System.Windows.Forms.Button();
            this.btnClearLeds = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.ntbXOffset = new ECCONetDevTool.NumericTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ntbYOffset = new ECCONetDevTool.NumericTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbbTier = new System.Windows.Forms.ComboBox();
            this.lblProductAssemblyFile = new System.Windows.Forms.Label();
            this.btnOpenProductAssemblyFile = new System.Windows.Forms.Button();
            this.ntbXScale = new ECCONetDevTool.NumericTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ntbYScale = new ECCONetDevTool.NumericTextBox();
            this.cbShowIds = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveFileDialogExpressionCollection = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialogExpressionCollection = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogPatternBin = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialogLightEngineDictionaryBin = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialogProductAssembly = new System.Windows.Forms.OpenFileDialog();
            this.btnBlend = new System.Windows.Forms.Button();
            this.btnSaveCollectionXml = new System.Windows.Forms.Button();
            this.dgvExpressions = new System.Windows.Forms.DataGridView();
            this.SelectRow = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ExpressionEnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExpressionName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RegStandard = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Intensity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Repeats = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OutputPriority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InputPriority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sequencer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblExpressionFile = new System.Windows.Forms.Label();
            this.gbExpressionTemplates.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpressions)).BeginInit();
            this.SuspendLayout();
            // 
            // cbbExpressionTemplate
            // 
            this.cbbExpressionTemplate.FormattingEnabled = true;
            this.cbbExpressionTemplate.Location = new System.Drawing.Point(10, 75);
            this.cbbExpressionTemplate.Name = "cbbExpressionTemplate";
            this.cbbExpressionTemplate.Size = new System.Drawing.Size(188, 21);
            this.cbbExpressionTemplate.TabIndex = 15;
            this.cbbExpressionTemplate.SelectedIndexChanged += new System.EventHandler(this.cbbExpressionTemplate_SelectedIndexChanged);
            // 
            // btnAddTemplate
            // 
            this.btnAddTemplate.Location = new System.Drawing.Point(10, 167);
            this.btnAddTemplate.Name = "btnAddTemplate";
            this.btnAddTemplate.Size = new System.Drawing.Size(188, 23);
            this.btnAddTemplate.TabIndex = 17;
            this.btnAddTemplate.Text = "<< Add Template To Project";
            this.btnAddTemplate.UseVisualStyleBackColor = true;
            this.btnAddTemplate.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lblExpressionTemplateFile
            // 
            this.lblExpressionTemplateFile.AutoSize = true;
            this.lblExpressionTemplateFile.Location = new System.Drawing.Point(7, 46);
            this.lblExpressionTemplateFile.Name = "lblExpressionTemplateFile";
            this.lblExpressionTemplateFile.Size = new System.Drawing.Size(26, 13);
            this.lblExpressionTemplateFile.TabIndex = 60;
            this.lblExpressionTemplateFile.Text = "File:";
            // 
            // lbxTemplateAreas
            // 
            this.lbxTemplateAreas.Enabled = false;
            this.lbxTemplateAreas.FormattingEnabled = true;
            this.lbxTemplateAreas.Location = new System.Drawing.Point(10, 102);
            this.lbxTemplateAreas.Name = "lbxTemplateAreas";
            this.lbxTemplateAreas.Size = new System.Drawing.Size(188, 56);
            this.lbxTemplateAreas.TabIndex = 64;
            // 
            // gbExpressionTemplates
            // 
            this.gbExpressionTemplates.Controls.Add(this.btnOpenExpressionTemplateCollection);
            this.gbExpressionTemplates.Controls.Add(this.lbxTemplateAreas);
            this.gbExpressionTemplates.Controls.Add(this.cbbExpressionTemplate);
            this.gbExpressionTemplates.Controls.Add(this.btnAddTemplate);
            this.gbExpressionTemplates.Controls.Add(this.lblExpressionTemplateFile);
            this.gbExpressionTemplates.Location = new System.Drawing.Point(1170, 250);
            this.gbExpressionTemplates.Name = "gbExpressionTemplates";
            this.gbExpressionTemplates.Size = new System.Drawing.Size(219, 199);
            this.gbExpressionTemplates.TabIndex = 66;
            this.gbExpressionTemplates.TabStop = false;
            this.gbExpressionTemplates.Text = "Expression Templates";
            // 
            // btnOpenExpressionTemplateCollection
            // 
            this.btnOpenExpressionTemplateCollection.Location = new System.Drawing.Point(10, 19);
            this.btnOpenExpressionTemplateCollection.Name = "btnOpenExpressionTemplateCollection";
            this.btnOpenExpressionTemplateCollection.Size = new System.Drawing.Size(87, 23);
            this.btnOpenExpressionTemplateCollection.TabIndex = 65;
            this.btnOpenExpressionTemplateCollection.Text = "Open File";
            this.btnOpenExpressionTemplateCollection.UseVisualStyleBackColor = true;
            this.btnOpenExpressionTemplateCollection.Click += new System.EventHandler(this.btnOpenExpressionTemplateCollection_Click);
            // 
            // lbxAreas
            // 
            this.lbxAreas.FormattingEnabled = true;
            this.lbxAreas.Location = new System.Drawing.Point(50, 486);
            this.lbxAreas.Name = "lbxAreas";
            this.lbxAreas.Size = new System.Drawing.Size(960, 134);
            this.lbxAreas.TabIndex = 67;
            this.lbxAreas.SelectedIndexChanged += new System.EventHandler(this.lbxAreas_SelectedIndexChanged);
            // 
            // btnOpenCollection
            // 
            this.btnOpenCollection.Location = new System.Drawing.Point(1016, 271);
            this.btnOpenCollection.Name = "btnOpenCollection";
            this.btnOpenCollection.Size = new System.Drawing.Size(105, 23);
            this.btnOpenCollection.TabIndex = 68;
            this.btnOpenCollection.Text = "Open File";
            this.btnOpenCollection.UseVisualStyleBackColor = true;
            this.btnOpenCollection.Click += new System.EventHandler(this.btnOpenCollection_Click);
            // 
            // btnSaveCollectionJson
            // 
            this.btnSaveCollectionJson.Location = new System.Drawing.Point(1016, 300);
            this.btnSaveCollectionJson.Name = "btnSaveCollectionJson";
            this.btnSaveCollectionJson.Size = new System.Drawing.Size(105, 23);
            this.btnSaveCollectionJson.TabIndex = 69;
            this.btnSaveCollectionJson.Text = "Save to JSON File";
            this.btnSaveCollectionJson.UseVisualStyleBackColor = true;
            this.btnSaveCollectionJson.Click += new System.EventHandler(this.btnSaveCollectionJson_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbSaveToLocalFile);
            this.groupBox1.Controls.Add(this.cbSaveToFlashFileSystem);
            this.groupBox1.Controls.Add(this.btnBuildExpressionBinFiles);
            this.groupBox1.Location = new System.Drawing.Point(1170, 506);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(219, 99);
            this.groupBox1.TabIndex = 70;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output Files";
            // 
            // cbSaveToLocalFile
            // 
            this.cbSaveToLocalFile.AutoSize = true;
            this.cbSaveToLocalFile.Location = new System.Drawing.Point(10, 42);
            this.cbSaveToLocalFile.Name = "cbSaveToLocalFile";
            this.cbSaveToLocalFile.Size = new System.Drawing.Size(111, 17);
            this.cbSaveToLocalFile.TabIndex = 62;
            this.cbSaveToLocalFile.Text = "Save to Local File";
            this.cbSaveToLocalFile.UseVisualStyleBackColor = true;
            this.cbSaveToLocalFile.CheckedChanged += new System.EventHandler(this.cbSaveToLocalFile_CheckedChanged);
            // 
            // cbSaveToFlashFileSystem
            // 
            this.cbSaveToFlashFileSystem.AutoSize = true;
            this.cbSaveToFlashFileSystem.Checked = true;
            this.cbSaveToFlashFileSystem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSaveToFlashFileSystem.Location = new System.Drawing.Point(10, 19);
            this.cbSaveToFlashFileSystem.Name = "cbSaveToFlashFileSystem";
            this.cbSaveToFlashFileSystem.Size = new System.Drawing.Size(147, 17);
            this.cbSaveToFlashFileSystem.TabIndex = 61;
            this.cbSaveToFlashFileSystem.Text = "Save to Flash File System";
            this.cbSaveToFlashFileSystem.UseVisualStyleBackColor = true;
            this.cbSaveToFlashFileSystem.CheckedChanged += new System.EventHandler(this.cbSaveToFlashFileSystem_CheckedChanged);
            // 
            // btnBuildExpressionBinFiles
            // 
            this.btnBuildExpressionBinFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuildExpressionBinFiles.Location = new System.Drawing.Point(10, 66);
            this.btnBuildExpressionBinFiles.Name = "btnBuildExpressionBinFiles";
            this.btnBuildExpressionBinFiles.Size = new System.Drawing.Size(188, 23);
            this.btnBuildExpressionBinFiles.TabIndex = 60;
            this.btnBuildExpressionBinFiles.Text = "Build Expression Bin Files";
            this.btnBuildExpressionBinFiles.UseVisualStyleBackColor = true;
            this.btnBuildExpressionBinFiles.Click += new System.EventHandler(this.btnBuildExpressionBinFiles_Click);
            // 
            // btnClearLeds
            // 
            this.btnClearLeds.Location = new System.Drawing.Point(1016, 486);
            this.btnClearLeds.Name = "btnClearLeds";
            this.btnClearLeds.Size = new System.Drawing.Size(105, 45);
            this.btnClearLeds.TabIndex = 64;
            this.btnClearLeds.Text = "Clear Expression Area Assignments";
            this.btnClearLeds.UseVisualStyleBackColor = true;
            this.btnClearLeds.Click += new System.EventHandler(this.btnClearLeds_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.ntbXOffset);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.ntbYOffset);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.cbbTier);
            this.groupBox4.Controls.Add(this.lblProductAssemblyFile);
            this.groupBox4.Controls.Add(this.btnOpenProductAssemblyFile);
            this.groupBox4.Controls.Add(this.ntbXScale);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.ntbYScale);
            this.groupBox4.Controls.Add(this.cbShowIds);
            this.groupBox4.Location = new System.Drawing.Point(1170, 29);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(219, 156);
            this.groupBox4.TabIndex = 74;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Product Assembly";
            // 
            // ntbXOffset
            // 
            this.ntbXOffset.AcceptHexInput = true;
            this.ntbXOffset.HighlightInputError = true;
            this.ntbXOffset.Location = new System.Drawing.Point(55, 92);
            this.ntbXOffset.Name = "ntbXOffset";
            this.ntbXOffset.Size = new System.Drawing.Size(42, 20);
            this.ntbXOffset.TabIndex = 103;
            this.ntbXOffset.TabStop = false;
            this.ntbXOffset.Text = "64.0";
            this.ntbXOffset.TextChanged += new System.EventHandler(this.ntbXOffset_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 104;
            this.label2.Text = "XOffset";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(108, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 102;
            this.label3.Text = "YOffset";
            // 
            // ntbYOffset
            // 
            this.ntbYOffset.AcceptHexInput = true;
            this.ntbYOffset.HighlightInputError = true;
            this.ntbYOffset.Location = new System.Drawing.Point(152, 92);
            this.ntbYOffset.Name = "ntbYOffset";
            this.ntbYOffset.Size = new System.Drawing.Size(42, 20);
            this.ntbYOffset.TabIndex = 101;
            this.ntbYOffset.Text = "20.0";
            this.ntbYOffset.TextChanged += new System.EventHandler(this.ntbYOffset_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 127);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 100;
            this.label1.Text = "Tier";
            // 
            // cbbTier
            // 
            this.cbbTier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbTier.FormattingEnabled = true;
            this.cbbTier.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3"});
            this.cbbTier.Location = new System.Drawing.Point(55, 124);
            this.cbbTier.Name = "cbbTier";
            this.cbbTier.Size = new System.Drawing.Size(42, 21);
            this.cbbTier.TabIndex = 99;
            this.cbbTier.SelectedIndexChanged += new System.EventHandler(this.cbbTier_SelectedIndexChanged);
            // 
            // lblProductAssemblyFile
            // 
            this.lblProductAssemblyFile.AutoSize = true;
            this.lblProductAssemblyFile.Location = new System.Drawing.Point(7, 45);
            this.lblProductAssemblyFile.Name = "lblProductAssemblyFile";
            this.lblProductAssemblyFile.Size = new System.Drawing.Size(26, 13);
            this.lblProductAssemblyFile.TabIndex = 2;
            this.lblProductAssemblyFile.Text = "File:";
            // 
            // btnOpenProductAssemblyFile
            // 
            this.btnOpenProductAssemblyFile.Location = new System.Drawing.Point(10, 18);
            this.btnOpenProductAssemblyFile.Name = "btnOpenProductAssemblyFile";
            this.btnOpenProductAssemblyFile.Size = new System.Drawing.Size(87, 23);
            this.btnOpenProductAssemblyFile.TabIndex = 1;
            this.btnOpenProductAssemblyFile.Text = "Open File";
            this.btnOpenProductAssemblyFile.UseVisualStyleBackColor = true;
            this.btnOpenProductAssemblyFile.Click += new System.EventHandler(this.btnOpenProductAssemblyFile_Click);
            // 
            // ntbXScale
            // 
            this.ntbXScale.AcceptHexInput = true;
            this.ntbXScale.HighlightInputError = true;
            this.ntbXScale.Location = new System.Drawing.Point(55, 66);
            this.ntbXScale.Name = "ntbXScale";
            this.ntbXScale.Size = new System.Drawing.Size(42, 20);
            this.ntbXScale.TabIndex = 97;
            this.ntbXScale.TabStop = false;
            this.ntbXScale.Text = "64.0";
            this.ntbXScale.TextChanged += new System.EventHandler(this.ntbXScale_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 98;
            this.label7.Text = "XScale";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(108, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 96;
            this.label6.Text = "YScale";
            // 
            // ntbYScale
            // 
            this.ntbYScale.AcceptHexInput = true;
            this.ntbYScale.HighlightInputError = true;
            this.ntbYScale.Location = new System.Drawing.Point(152, 66);
            this.ntbYScale.Name = "ntbYScale";
            this.ntbYScale.Size = new System.Drawing.Size(42, 20);
            this.ntbYScale.TabIndex = 95;
            this.ntbYScale.Text = "20.0";
            this.ntbYScale.TextChanged += new System.EventHandler(this.ntbYScale_TextChanged);
            // 
            // cbShowIds
            // 
            this.cbShowIds.AutoSize = true;
            this.cbShowIds.Location = new System.Drawing.Point(111, 126);
            this.cbShowIds.Name = "cbShowIds";
            this.cbShowIds.Size = new System.Drawing.Size(72, 17);
            this.cbShowIds.TabIndex = 93;
            this.cbShowIds.Text = "Show IDs";
            this.cbShowIds.UseVisualStyleBackColor = true;
            this.cbShowIds.CheckedChanged += new System.EventHandler(this.cbShowIds_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.panel1.Location = new System.Drawing.Point(1158, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1, 616);
            this.panel1.TabIndex = 75;
            // 
            // saveFileDialogExpressionCollection
            // 
            this.saveFileDialogExpressionCollection.DefaultExt = "ec";
            this.saveFileDialogExpressionCollection.Filter = "Expression Collection files|*.ec";
            // 
            // openFileDialogExpressionCollection
            // 
            this.openFileDialogExpressionCollection.DefaultExt = "ec";
            this.openFileDialogExpressionCollection.Filter = "Expression Collection files|*.ec";
            // 
            // saveFileDialogPatternBin
            // 
            this.saveFileDialogPatternBin.FileName = "patterns.tbl";
            this.saveFileDialogPatternBin.Filter = "Pattern bin files|*.tbl";
            // 
            // saveFileDialogLightEngineDictionaryBin
            // 
            this.saveFileDialogLightEngineDictionaryBin.FileName = "lighteng.dct";
            this.saveFileDialogLightEngineDictionaryBin.Filter = "Light engine dictionary bin files|*.dct";
            // 
            // openFileDialogProductAssembly
            // 
            this.openFileDialogProductAssembly.Filter = "Product Assembly File|*.epa";
            // 
            // btnBlend
            // 
            this.btnBlend.Location = new System.Drawing.Point(1016, 399);
            this.btnBlend.Name = "btnBlend";
            this.btnBlend.Size = new System.Drawing.Size(105, 45);
            this.btnBlend.TabIndex = 68;
            this.btnBlend.Text = "Blend Checked Expressions";
            this.btnBlend.UseVisualStyleBackColor = true;
            this.btnBlend.Click += new System.EventHandler(this.btnBlend_Click);
            // 
            // btnSaveCollectionXml
            // 
            this.btnSaveCollectionXml.Location = new System.Drawing.Point(1016, 329);
            this.btnSaveCollectionXml.Name = "btnSaveCollectionXml";
            this.btnSaveCollectionXml.Size = new System.Drawing.Size(105, 23);
            this.btnSaveCollectionXml.TabIndex = 70;
            this.btnSaveCollectionXml.Text = "Save to XML File";
            this.btnSaveCollectionXml.UseVisualStyleBackColor = true;
            this.btnSaveCollectionXml.Click += new System.EventHandler(this.btnSaveCollectionXml_Click);
            // 
            // dgvExpressions
            // 
            this.dgvExpressions.AllowUserToAddRows = false;
            this.dgvExpressions.AllowUserToOrderColumns = true;
            this.dgvExpressions.AllowUserToResizeRows = false;
            this.dgvExpressions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvExpressions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SelectRow,
            this.ExpressionEnum,
            this.ExpressionName,
            this.RegStandard,
            this.Intensity,
            this.Repeats,
            this.OutputPriority,
            this.InputPriority,
            this.Sequencer});
            this.dgvExpressions.Location = new System.Drawing.Point(50, 271);
            this.dgvExpressions.MultiSelect = false;
            this.dgvExpressions.Name = "dgvExpressions";
            this.dgvExpressions.Size = new System.Drawing.Size(960, 209);
            this.dgvExpressions.TabIndex = 99;
            this.dgvExpressions.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvExpressions_DataError);
            this.dgvExpressions.SelectionChanged += new System.EventHandler(this.dgvExpressions_SelectionChanged);
            // 
            // SelectRow
            // 
            this.SelectRow.HeaderText = "";
            this.SelectRow.MinimumWidth = 20;
            this.SelectRow.Name = "SelectRow";
            this.SelectRow.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SelectRow.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.SelectRow.ToolTipText = "Check to include in Expression Blen";
            this.SelectRow.Width = 20;
            // 
            // ExpressionEnum
            // 
            this.ExpressionEnum.DataPropertyName = "ExpressionEnum";
            this.ExpressionEnum.HeaderText = "Enum";
            this.ExpressionEnum.Name = "ExpressionEnum";
            this.ExpressionEnum.ToolTipText = "The expression code.  You can use this code in the Control tab to run the express" +
    "ion.";
            this.ExpressionEnum.Width = 40;
            // 
            // ExpressionName
            // 
            this.ExpressionName.DataPropertyName = "Name";
            this.ExpressionName.HeaderText = "Expression";
            this.ExpressionName.Name = "ExpressionName";
            this.ExpressionName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ExpressionName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ExpressionName.ToolTipText = "The expression name.";
            this.ExpressionName.Width = 180;
            // 
            // RegStandard
            // 
            this.RegStandard.DataPropertyName = "RegStandard";
            this.RegStandard.HeaderText = "Regulatory Standard";
            this.RegStandard.Name = "RegStandard";
            this.RegStandard.ReadOnly = true;
            this.RegStandard.ToolTipText = "Named only if the pattern from template was built to meet a regulator standard.";
            this.RegStandard.Width = 70;
            // 
            // Intensity
            // 
            this.Intensity.DataPropertyName = "Value";
            this.Intensity.HeaderText = "Intensity";
            this.Intensity.Name = "Intensity";
            this.Intensity.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Intensity.ToolTipText = "Normally 100%, this value is multiplied by the Dim to get the final intensity.";
            this.Intensity.Width = 70;
            // 
            // Repeats
            // 
            this.Repeats.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Repeats.DataPropertyName = "Repeats";
            this.Repeats.HeaderText = "Repeats";
            this.Repeats.Name = "Repeats";
            this.Repeats.ToolTipText = "Number of pattern repeats.  Normally 0 for infinite repeats.";
            this.Repeats.Width = 72;
            // 
            // OutputPriority
            // 
            this.OutputPriority.DataPropertyName = "OutputPriority";
            this.OutputPriority.HeaderText = "Priority Level 1-6";
            this.OutputPriority.Name = "OutputPriority";
            this.OutputPriority.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.OutputPriority.ToolTipText = "Determines which expressions have light output priority.";
            this.OutputPriority.Width = 80;
            // 
            // InputPriority
            // 
            this.InputPriority.DataPropertyName = "InputPriority";
            this.InputPriority.HeaderText = "Sub Priority";
            this.InputPriority.Name = "InputPriority";
            this.InputPriority.ToolTipText = "Determines which expression has priority within the priority level.";
            this.InputPriority.Width = 70;
            // 
            // Sequencer
            // 
            this.Sequencer.DataPropertyName = "Sequencer";
            this.Sequencer.HeaderText = "Sequencer";
            this.Sequencer.Name = "Sequencer";
            this.Sequencer.ReadOnly = true;
            this.Sequencer.ToolTipText = "Automatic sequencer assignment (read-only)";
            this.Sequencer.Visible = false;
            this.Sequencer.Width = 20;
            // 
            // lblExpressionFile
            // 
            this.lblExpressionFile.AutoSize = true;
            this.lblExpressionFile.Location = new System.Drawing.Point(47, 250);
            this.lblExpressionFile.Name = "lblExpressionFile";
            this.lblExpressionFile.Size = new System.Drawing.Size(26, 13);
            this.lblExpressionFile.TabIndex = 3;
            this.lblExpressionFile.Text = "File:";
            // 
            // ExpressionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSaveCollectionXml);
            this.Controls.Add(this.dgvExpressions);
            this.Controls.Add(this.btnSaveCollectionJson);
            this.Controls.Add(this.btnOpenCollection);
            this.Controls.Add(this.lblExpressionFile);
            this.Controls.Add(this.btnBlend);
            this.Controls.Add(this.btnClearLeds);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbxAreas);
            this.Controls.Add(this.gbExpressionTemplates);
            this.Name = "ExpressionEditor";
            this.Size = new System.Drawing.Size(1394, 620);
            this.gbExpressionTemplates.ResumeLayout(false);
            this.gbExpressionTemplates.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpressions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cbbExpressionTemplate;
        private System.Windows.Forms.Button btnAddTemplate;
        private System.Windows.Forms.Label lblExpressionTemplateFile;
        private System.Windows.Forms.ListBox lbxTemplateAreas;
        private System.Windows.Forms.GroupBox gbExpressionTemplates;
        private System.Windows.Forms.Button btnOpenExpressionTemplateCollection;
        private System.Windows.Forms.ListBox lbxAreas;
        private System.Windows.Forms.Button btnOpenCollection;
        private System.Windows.Forms.Button btnSaveCollectionJson;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SaveFileDialog saveFileDialogExpressionCollection;
        private System.Windows.Forms.OpenFileDialog openFileDialogExpressionCollection;
        private System.Windows.Forms.CheckBox cbSaveToLocalFile;
        private System.Windows.Forms.CheckBox cbSaveToFlashFileSystem;
        private System.Windows.Forms.Button btnBuildExpressionBinFiles;
        private System.Windows.Forms.SaveFileDialog saveFileDialogPatternBin;
        private System.Windows.Forms.SaveFileDialog saveFileDialogLightEngineDictionaryBin;
        private System.Windows.Forms.Button btnOpenProductAssemblyFile;
        private System.Windows.Forms.OpenFileDialog openFileDialogProductAssembly;
        private System.Windows.Forms.Label lblProductAssemblyFile;
        private System.Windows.Forms.CheckBox cbShowIds;
        private System.Windows.Forms.Button btnClearLeds;
        private System.Windows.Forms.Button btnBlend;
        private NumericTextBox ntbYScale;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private NumericTextBox ntbXScale;
        private System.Windows.Forms.Button btnSaveCollectionXml;
        private System.Windows.Forms.DataGridView dgvExpressions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbbTier;
        private System.Windows.Forms.Label lblExpressionFile;
        private NumericTextBox ntbXOffset;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private NumericTextBox ntbYOffset;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SelectRow;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExpressionEnum;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExpressionName;
        private System.Windows.Forms.DataGridViewTextBoxColumn RegStandard;
        private System.Windows.Forms.DataGridViewTextBoxColumn Intensity;
        private System.Windows.Forms.DataGridViewTextBoxColumn Repeats;
        private System.Windows.Forms.DataGridViewTextBoxColumn OutputPriority;
        private System.Windows.Forms.DataGridViewTextBoxColumn InputPriority;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sequencer;
    }
}
