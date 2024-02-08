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
using ECCONetDevTool.FlashFileSystem;
using ESG.BytecodeLib;


namespace ECCONetDevTool
{
    public partial class Equations : UserControl
    {
        /// <summary>
        /// The flash file volume.
        /// </summary>
        //private FlashFileVolume volume;

        /// <summary>
        /// The flash file.
        /// </summary>
        //private FlashFile file;

        /// <summary>
        /// The binary generated delegate.
        /// </summary>
        public event FlashFileSystem.FlashFileSystem.BinaryGeneratedDelegate binaryGeneratedDelegate;


        /// <summary>
        /// The user profile enum.
        /// </summary>
        public int UserProfileIndex { get; set; }

        /// <summary>
        /// The profile file names.
        /// </summary>
        private static readonly string[] ProfileNames =
        {
            FlashFileVolume.BytecodeDefaultFileName,
            FlashFileVolume.BytecodeProfile1FileName,
            FlashFileVolume.BytecodeProfile2FileName,
            FlashFileVolume.BytecodeProfile3FileName,
            FlashFileVolume.BytecodeProfile4FileName,
            FlashFileVolume.BytecodeProfile5FileName,
            FlashFileVolume.BytecodeProfile6FileName,
        };

        /// <summary>
        /// Constructor.
        /// </summary>
        public Equations()
        {
            //  initialize designer controls
            InitializeComponent();


            //  initialize list view
            lvFilePaths.View = View.Details;
            lvFilePaths.HeaderStyle = ColumnHeaderStyle.None;
            //lvFilePaths.LabelEdit = true;
            lvFilePaths.Columns.Add("Path", 360, HorizontalAlignment.Center);

            //  create or restore the settings
            RestoreSettings();
        }


        #region File selection

        /// <summary>
        /// Button select equations file clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectEquationsFile_Click(object sender, EventArgs e)
        {
            //  if user selected a file
            if (openHeaderFileDialog.ShowDialog() == DialogResult.OK)
            {
                //  show file name
                lblEquationsFilePath.Text = openHeaderFileDialog.FileName;

                //  save file name
                if (UserProfileIndex < ProfileNames.Length)
                {
                    while (Properties.Settings.Default.EquationsFileNames.Count < (UserProfileIndex + 1))
                        Properties.Settings.Default.EquationsFileNames.Add(string.Empty);
                    Properties.Settings.Default.EquationsFileNames[(int)UserProfileIndex] = openHeaderFileDialog.FileName;
                }
            }
        }
        #endregion
        
        #region Bytecode builder check boxes
        /// <summary>
        /// Checkbox include bytecode in flash file system check changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbIncludeInFlashFileSystem_CheckedChanged(object sender, EventArgs e)
        {
            //  if user profile enum valid
            if (UserProfileIndex < ProfileNames.Length)
            {
                //  make sure string collections are initialized
                CheckStringCollectionInits();

                //  save the bytecode include in file system status
                while (Properties.Settings.Default.BytecodeInclude.Count < (UserProfileIndex + 1))
                    Properties.Settings.Default.BytecodeInclude.Add(string.Empty);
                Properties.Settings.Default.BytecodeInclude[(int)UserProfileIndex] =
                    (cbIncludeInFlashFileSystem.Checked ? "true" : "false");
            }
        }

        /// <summary>
        /// Checkbox save to local file system check changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSaveToLocalFile_CheckedChanged(object sender, EventArgs e)
        {
            //  if user profile enum valid
            if (UserProfileIndex < ProfileNames.Length)
            {
                //  make sure string collections are initialized
                CheckStringCollectionInits();

                //  save the bytecode include in file system status
                while (Properties.Settings.Default.BytecodeSaveToLocalFile.Count < (UserProfileIndex + 1))
                    Properties.Settings.Default.BytecodeSaveToLocalFile.Add(string.Empty);
                Properties.Settings.Default.BytecodeSaveToLocalFile[(int)UserProfileIndex] =
                    (cbSaveToLocalFile.Checked ? "true" : "false");
            }
        }
        #endregion

        #region File paths
        /// <summary>
        /// Add file path button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddFilePath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                lvFilePaths.Items.Add(folderBrowserDialog.SelectedPath);
                SaveListViewItems();
            }
        }

        /// <summary>
        /// Remove file path(s) button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveFilePath_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvFilePaths.SelectedItems)
            {
                lvFilePaths.Items.Remove(item);
                SaveListViewItems();
            }
        }

        /// <summary>
        /// Edit item button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFilePathEdit_Click(object sender, EventArgs e)
        {
            if ((lvFilePaths.SelectedItems.Count == 1) &&
                (folderBrowserDialog.ShowDialog() == DialogResult.OK))
            {
                foreach (ListViewItem item in lvFilePaths.SelectedItems)
                {
                    item.Text = folderBrowserDialog.SelectedPath;
                    break;
                }
                SaveListViewItems();
            }
        }

        /// <summary>
        /// Saves the list view items.
        /// </summary>
        private void SaveListViewItems()
        {
            //  get file paths separated by newline
            var filePaths = string.Empty;
            foreach (ListViewItem item in lvFilePaths.Items)
                filePaths += (item.Text + "\n");

            //  save file paths
            if (UserProfileIndex < ProfileNames.Length)
            {
                while (Properties.Settings.Default.EquationsFilePaths.Count < (UserProfileIndex + 1))
                    Properties.Settings.Default.EquationsFilePaths.Add(string.Empty);
                Properties.Settings.Default.EquationsFilePaths[(int)UserProfileIndex] = filePaths;
            }
        }

        /// <summary>
        /// Restores the list view items.
        /// </summary>
        private void RestoreListViewItems()
        {
            var filePaths = Properties.Settings.Default.EquationsFilePaths[(int)UserProfileIndex].Split('\n');
            lvFilePaths.Items.Clear();
            foreach (var filePath in filePaths)
                if (filePath != string.Empty)
                    lvFilePaths.Items.Add(filePath);
        }

        /// <summary>
        /// Gets a list of the file paths.
        /// </summary>
        /// <returns>A list of the file paths.</returns>
        private List<string> GetFilePaths()
        {
            //  get file paths separated by newline
            var filePaths = new List<string>();
            foreach (ListViewItem item in lvFilePaths.Items)
                if (item.Text != string.Empty)
                    filePaths.Add(item.Text);
            return filePaths;
        }
        #endregion

        #region Build and save bytecode files
        /// <summary>
        /// Button build step method bin files clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBuildEquationBinFiles_Click(object sender, EventArgs e)
        {
            try
            {
                //  try to convert to bytecode
                var bytecode = EquationConverters.ToBytecode(lblEquationsFilePath.Text, GetFilePaths(), out string status);

                if (bytecode == null)
                {
                    //  show errors
                    tbxResults.Clear();
                    tbxResults.ForeColor = Color.DarkRed;
                    tbxResults.SelectionFont = new Font(tbxResults.Font, FontStyle.Bold);
                    tbxResults.AppendText("ERRORS:\n\n");
                    tbxResults.SelectionFont = new Font(tbxResults.Font, FontStyle.Regular);
                    tbxResults.AppendText(status);
                }
                else
                {
                    //  show results
                    tbxResults.Clear();
                    tbxResults.ForeColor = Color.Black;
                    tbxResults.SelectionFont = new Font(tbxResults.Font, FontStyle.Regular);
                    tbxResults.AppendText(status);

                    //  if user profile enumeration valid
                    if (UserProfileIndex < ProfileNames.Length)
                    {
                        //  get file name
                        var filename = ProfileNames[UserProfileIndex];

                        //  if saving to flash file system
                        if (cbIncludeInFlashFileSystem.Checked && (binaryGeneratedDelegate != null) && (bytecode != null))
                        {
                            binaryGeneratedDelegate(this, filename, bytecode, null, 0);
                        }

                        //  if saving to local files
                        if (cbSaveToLocalFile.Checked)
                        {
                            //  suggest file name
                            saveBytecodeFileDialog.FileName = filename;

                            //  save the bytecode file
                            if ((bytecode != null) && (saveBytecodeFileDialog.ShowDialog() == DialogResult.OK))
                                File.WriteAllBytes(saveBytecodeFileDialog.FileName, bytecode);
                        }
                    }
                    else
                    {
                        throw new Exception("User profile enum out of range.");
                    }
                }
            }
            catch (Exception ex)
            {
                tbxResults.Clear();
                tbxResults.ForeColor = Color.DarkRed;
                tbxResults.SelectionFont = new Font(tbxResults.Font, FontStyle.Bold);
                tbxResults.AppendText("ERRORS:\n\n");
                tbxResults.SelectionFont = new Font(tbxResults.Font, FontStyle.Regular);
                tbxResults.AppendText(ex.Message);
                return;
            }
        }
        #endregion

        #region Save and restore
        /// <summary>
        /// Make sure string collections are initialized.
        /// </summary>
        private void CheckStringCollectionInits()
        {
            if (Properties.Settings.Default.EquationsFilePaths == null)
                Properties.Settings.Default.EquationsFilePaths = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.EquationsFileNames == null)
                Properties.Settings.Default.EquationsFileNames = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.BytecodeInclude == null)
                Properties.Settings.Default.BytecodeInclude = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.BytecodeSaveToLocalFile == null)
                Properties.Settings.Default.BytecodeSaveToLocalFile = new System.Collections.Specialized.StringCollection();
        }

        /// <summary>
        /// Restore the user settings.
        /// </summary>
        void RestoreSettings()
        {
            //  if user profile enum valid
            if (UserProfileIndex < ProfileNames.Length)
            {
                //  make sure string collections are initialized
                CheckStringCollectionInits();

                //  restore the equations file paths
                if (Properties.Settings.Default.EquationsFilePaths.Count > UserProfileIndex)
                    RestoreListViewItems();

                //  restore the equations file name
                if (Properties.Settings.Default.EquationsFileNames.Count > UserProfileIndex)
                lblEquationsFilePath.Text = Properties.Settings.Default.EquationsFileNames[UserProfileIndex];

                //  restore the bytecode include in flash file system status
                if (Properties.Settings.Default.BytecodeInclude.Count > UserProfileIndex)
                    cbIncludeInFlashFileSystem.Checked = Properties.Settings.Default.BytecodeInclude[UserProfileIndex].Equals("true");

                //  restore the write to local file system status
                if (Properties.Settings.Default.BytecodeSaveToLocalFile.Count > UserProfileIndex)
                    cbSaveToLocalFile.Checked = Properties.Settings.Default.BytecodeSaveToLocalFile[UserProfileIndex].Equals("true");
            }
        }
        #endregion
    }
}
