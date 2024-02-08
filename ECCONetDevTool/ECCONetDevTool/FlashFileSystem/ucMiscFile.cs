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

namespace ECCONetDevTool.FlashFileSystem
{
    public partial class ucMiscFile : UserControl
    {
        private const int NumMiscFiles = 10;

        private enum FileMode
        {
            Binary,
            Text,
            ReservedSpace
        }

        /// <summary>
        /// The flash file volumes.
        /// </summary>
        private FlashFileVolume[] volumes;

        /// <summary>
        /// The flash file volume index.
        /// </summary>
        private uint volumeIndex;

        /// <summary>
        /// The flash file volume position index.
        /// </summary>
        private uint volumePositionIndex;

        /// <summary>
        /// The currently-selected flash file volume.
        /// </summary>
        private FlashFileVolume volume
        {
            get
            {
                if ((volumes != null) && (volumes.Length >= volumeIndex))
                    return volumes[volumeIndex];
                return null;
            }
        }

        /// <summary>
        /// The flash file.
        /// </summary>
        private FlashFile file;

        /// <summary>
        /// The file settings index
        /// </summary>
        private int fileSettingsIndex
        {
            get => (int)volumePositionIndex - (int)FlashFileVolume.VolumeIndices.Miscellaneous0;
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public ucMiscFile()
        {
            //  initialize designer controls
            InitializeComponent();
        }

        /// <summary>
        /// Set the volume and volume index.
        /// </summary>
        /// <param name="flashFileVolume">The flash file volume to which this file belongs.</param>
        public void SetVolumes(FlashFileVolume[] flashFileVolumes, uint volumePositionIndex)
        {
            //  save the volumes and the volume position index
            volumes = flashFileVolumes;
            this.volumePositionIndex = volumePositionIndex;

            //  restore user settings
            RestoreSettings();
        }


        #region Dropdown selection changed
        /// <summary>
        /// The file mode changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbbFileMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            //  save the file mode
            CheckStringCollectionInits();
            while (Properties.Settings.Default.MiscFlashFileIsReservedSpace.Count < NumMiscFiles)
                Properties.Settings.Default.MiscFlashFileIsReservedSpace.Add(string.Empty);
            Properties.Settings.Default.MiscFlashFileIsReservedSpace[fileSettingsIndex] = cbbFileMode.SelectedIndex.ToString();

            //  update the file
            UpdateFile();
        }

        /// <summary>
        /// The file type changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbbFileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //  save the file mode
            CheckStringCollectionInits();
            while (Properties.Settings.Default.MiscFlashFileIsBin.Count < NumMiscFiles)
                Properties.Settings.Default.MiscFlashFileIsBin.Add(string.Empty);
            Properties.Settings.Default.MiscFlashFileIsBin[fileSettingsIndex] = cbbFileType.SelectedIndex.ToString();

            //  update the file
            UpdateFile();
        }

        /// <summary>
        /// The selected volume changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbbVolume_SelectedIndexChanged(object sender, EventArgs e)
        {
            //  remove any instances of file in volume and delete file
            if ((file != null) && (volume != null))
            {
                volume.RemoveFile(file.Name);
                file = null;
            }

            //  save the volume index
            CheckStringCollectionInits();
            while (Properties.Settings.Default.MiscFlashFileVolume.Count < NumMiscFiles)
                Properties.Settings.Default.MiscFlashFileVolume.Add(string.Empty);
            Properties.Settings.Default.MiscFlashFileVolume[fileSettingsIndex] = cbbVolume.SelectedIndex.ToString();

            //  update the volume index and file
            volumeIndex = (uint)cbbVolume.SelectedIndex;
            UpdateFile();
        }
        #endregion

        #region Reserved space
        /// <summary>
        /// The reserved space text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ntbReservedSpace_TextChanged(object sender, EventArgs e)
        {
            //  get value to validate input, indicating by text color
            ntbReservedSpace.GetUInt32Value(out UInt32 size);

            //  make sure string collections are initialized
            CheckStringCollectionInits();

            //  save the reserved space
            while (Properties.Settings.Default.MiscFlashFileReservedSpace.Count < NumMiscFiles)
                Properties.Settings.Default.MiscFlashFileReservedSpace.Add(string.Empty);
            Properties.Settings.Default.MiscFlashFileReservedSpace[fileSettingsIndex] = ntbReservedSpace.Text;
        }
        #endregion

        #region File
        /// <summary>
        /// Button update time-logic file clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTimeLogicUpdateFile_Click(object sender, EventArgs e)
        {
            UpdateFile();
        }

        /// <summary>
        /// Button find time-logic text file clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    tbFileName.Text = openFileDialog.FileName;
                    UpdateFile();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Updates the file based on user controls.
        /// </summary>
        private void UpdateFile()
        {
            //  check volume
            if (volume == null)
                return;

            //  remove any instances of file in volume and delete file
            if (file != null)
            {
                volume.RemoveFile(file.Name);
                file = null;
            }

            //  if include file is checked
            if (cbIncludeInFlashFileSystem.Checked)
            {
                //  if an imported file
                if (cbbFileMode.SelectedIndex == 0)
                {
                    //  if a binary file
                    if (cbbFileType.SelectedIndex == 0)
                    {
                        //  try to get binary file
                        if (!tbFileName.Text.Equals(string.Empty) && File.Exists(tbFileName.Text))
                        {
                            try
                            {
                                file = new BinFile(volumePositionIndex)
                                {
                                    Name = Path.GetFileName(tbFileName.Text),
                                    Data = File.ReadAllBytes(tbFileName.Text)
                                };
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                    }
                    else  //  a text file
                    {
                        //  try to get text file
                        if (!tbFileName.Text.Equals(string.Empty) && File.Exists(tbFileName.Text))
                        {
                            try
                            {
                                file = new TextFile(volumePositionIndex)
                                {
                                    Name = Path.GetFileName(tbFileName.Text),
                                    Text = File.ReadAllText(tbFileName.Text)
                                };
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                    }
                }
                else  //  a reserved space file
                {
                    try
                    {
                        file = new BinFile(volumePositionIndex);
                        file.Name = Path.GetFileName(tbFileName.Text);
                        byte[] data = null;
                        if (ntbReservedSpace.GetUInt32Value(out UInt32 size))
                            data = new byte[size];
                        else
                            data = new byte[10];
                        file.Data = data;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                //  if file was created, add it to the volume
                if (file != null)
                    volume.AddFile(file);
            }
        }
        #endregion

        #region User controls
        /// <summary>
        /// Text box file name changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbFileName_TextChanged(object sender, EventArgs e)
        {
            //  update the file
            UpdateFile();

            //  make sure string collections are initialized
            CheckStringCollectionInits();

            //  save the file path
            while (Properties.Settings.Default.MiscFlashFileName.Count < NumMiscFiles)
                Properties.Settings.Default.MiscFlashFileName.Add("myfile.bin");
            Properties.Settings.Default.MiscFlashFileName[fileSettingsIndex] = tbFileName.Text;
        }

        /// <summary>
        /// Checkbox include in flash file system check changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbIncludeInFlashFileSystem_CheckedChanged(object sender, EventArgs e)
        {
            //  update the file
            UpdateFile();

            //  make sure string collections are initialized
            CheckStringCollectionInits();

            //  save the included status
            while (Properties.Settings.Default.MiscFlashFileIsIncluded.Count < NumMiscFiles)
                Properties.Settings.Default.MiscFlashFileIsIncluded.Add(string.Empty);
            Properties.Settings.Default.MiscFlashFileIsIncluded[fileSettingsIndex] = (cbIncludeInFlashFileSystem.Checked ? "true" : "false");
        }
        #endregion

        #region Save and restore settings
        /// <summary>
        /// Make sure string collections are initialized.
        /// </summary>
        private void CheckStringCollectionInits()
        {
            if (Properties.Settings.Default.MiscFlashFileName == null)
                Properties.Settings.Default.MiscFlashFileName = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.MiscFlashFileIsBin == null)
                Properties.Settings.Default.MiscFlashFileIsBin = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.MiscFlashFileIsReservedSpace == null)
                Properties.Settings.Default.MiscFlashFileIsReservedSpace = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.MiscFlashFileReservedSpace == null)
                Properties.Settings.Default.MiscFlashFileReservedSpace = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.MiscFlashFileVolume == null)
                Properties.Settings.Default.MiscFlashFileVolume = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.MiscFlashFileIsIncluded == null)
                Properties.Settings.Default.MiscFlashFileIsIncluded = new System.Collections.Specialized.StringCollection();
        }

        /// <summary>
        /// Restore the user settings.
        /// </summary>
        void RestoreSettings()
        {
            //  make sure string collections are initialized
            CheckStringCollectionInits();

            //  restore the file path
            if (Properties.Settings.Default.MiscFlashFileName.Count > fileSettingsIndex)
                tbFileName.Text = Properties.Settings.Default.MiscFlashFileName[fileSettingsIndex];

            //  restore the file mode, either file import or reserved space
            try
            {
                cbbFileMode.SelectedIndex = int.Parse(Properties.Settings.Default.MiscFlashFileIsReservedSpace[fileSettingsIndex]);
            }
            catch
            {
                cbbFileMode.SelectedIndex = 0;
            }


            //  restore the file type, either binary or text
            try
            {
                cbbFileType.SelectedIndex = int.Parse(Properties.Settings.Default.MiscFlashFileIsBin[fileSettingsIndex]);
            }
            catch
            {
                cbbFileType.SelectedIndex = 0;
            }

            //  restore the volume selection
            try
            {
                cbbVolume.SelectedIndex = int.Parse(Properties.Settings.Default.MiscFlashFileVolume[fileSettingsIndex]);
            }
            catch
            {
                cbbVolume.SelectedIndex = 0;
            }
            volumeIndex = (uint)cbbVolume.SelectedIndex;

            //  restore the reserved space
            if (Properties.Settings.Default.MiscFlashFileReservedSpace.Count > fileSettingsIndex)
                ntbReservedSpace.Text = Properties.Settings.Default.MiscFlashFileReservedSpace[fileSettingsIndex];

            //  restore the is included in volume status
            if (Properties.Settings.Default.MiscFlashFileIsIncluded.Count > fileSettingsIndex)
                cbIncludeInFlashFileSystem.Checked = Properties.Settings.Default.MiscFlashFileIsIncluded[fileSettingsIndex].Equals("true");

            //  update the file
            UpdateFile();
        }
        #endregion
    }
}
