using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using ECCONetDevTool;

namespace ECCONetDevTool.FlashFileSystem
{
    public partial class ucProductAssemblyFile : UserControl
    {
        /// <summary>
        /// The flash file volume.
        /// </summary>
        private FlashFileVolume volume;

        /// <summary>
        /// The flash file.
        /// </summary>
        private FlashFile file;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ucProductAssemblyFile()
        {
            //  initialize designer controls
            InitializeComponent();
        }

        /// <summary>
        /// Set the volume and volume index.
        /// </summary>
        /// <param name="flashFileVolume">The flash file volume to which this file belongs.</param>
        /// <param name="volumeIndex">The file index to support multiple time-logic files.</param>
        public void SetVolume(FlashFileVolume flashFileVolume, uint volumeIndex)
        {
            //  save the volume
            volume = flashFileVolume;

            //  create file
            switch (volumeIndex)
            {
                case (uint)FlashFileVolume.VolumeIndices.BytecodeDefault:
                case (uint)FlashFileVolume.VolumeIndices.BytecodeUserProfile1:
                case (uint)FlashFileVolume.VolumeIndices.BytecodeUserProfile2:
                case (uint)FlashFileVolume.VolumeIndices.BytecodeUserProfile3:
                case (uint)FlashFileVolume.VolumeIndices.BytecodeUserProfile4:
                    file = new TimeLogicFile(volumeIndex);
                    lblImportFile.Text = "Import Text File";
                    break;

                case (uint)FlashFileVolume.VolumeIndices.ProductAssembly:
                    file = new ProductAssemblyFile(volumeIndex);
                    lblImportFile.Text = "Import Bin File";
                    break;

                default:
                    break;
            }

            //  restore user settings
            RestoreSettings();
        }

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
                    tbAssemblyImport.Text = openFileDialog.FileName;
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
            if ((volume == null) || (file == null))
                return;

            //  remove any instances of the product assembly file already in the volume
            volume.RemoveFile(file.Name);

            //  if include file is checked
            if (cbIncludeTimeLogicFile.Checked)
            {
                if (file is ProductAssemblyFile productAssemblyFile)
                {
                    //  try to get imported binary
                    byte[] fileData = null;
                    if ((tbAssemblyImport.Text != string.Empty) && File.Exists(tbAssemblyImport.Text))
                    {
                        try
                        {
                            //  read binary file
                            fileData = File.ReadAllBytes(tbAssemblyImport.Text);

                            //  compress the data
                            fileData = Compress(fileData);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }

                    //  set file data
                    if (fileData != null)
                        productAssemblyFile.Data = fileData;
                    else if (ntbTimeLogicReservedSpace.GetUInt32Value(out UInt32 size))
                        productAssemblyFile.Data = new byte[size];
                    else
                        productAssemblyFile.Data = new byte[10];

                    //  add the file to the volume
                    volume.AddFile(file);
                }
            }
        }
        #endregion

        #region Compress and decompress
        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }
        #endregion




        #region Save and restore

        /// <summary>
        /// Make sure string collections are initialized.
        /// </summary>
        private void CheckStringCollectionInits()
        {
            if (Properties.Settings.Default.TimeLogicFileInclude == null)
                Properties.Settings.Default.TimeLogicFileInclude = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.TimeLogicFileReservedSpace == null)
                Properties.Settings.Default.TimeLogicFileReservedSpace = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.TimeLogicFileLocations == null)
                Properties.Settings.Default.TimeLogicFileLocations = new System.Collections.Specialized.StringCollection();
        }



        /// <summary>
        /// Checkbox include time logic file check changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbIncludeTimeLogicFile_CheckedChanged(object sender, EventArgs e)
        {
            //  update the time logic file
            //UpdateTimeLogicFile();

            //  if file is present
            if (file != null)
            {
                //  make sure string collections are initialized
                CheckStringCollectionInits();

                //  save the reserved space
                while (Properties.Settings.Default.TimeLogicFileInclude.Count < (file.VolumeIndex + 1))
                    Properties.Settings.Default.TimeLogicFileInclude.Add(string.Empty);
                Properties.Settings.Default.TimeLogicFileInclude[(int)file.VolumeIndex] =
                    (cbIncludeTimeLogicFile.Checked ? "true" : "false");
            }
        }

        /// <summary>
        /// Text box reserved space text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ntbTimeLogicReservedSpace_TextChanged(object sender, EventArgs e)
        {
            //  if file is present
            if (file != null)
            {
                //  get value to validate input, indicating by text color
                ntbTimeLogicReservedSpace.GetUInt32Value(out UInt32 size);

                //  make sure string collections are initialized
                CheckStringCollectionInits();

                //  save the reserved space
                while (Properties.Settings.Default.TimeLogicFileReservedSpace.Count < (file.VolumeIndex + 1))
                    Properties.Settings.Default.TimeLogicFileReservedSpace.Add(string.Empty);
                Properties.Settings.Default.TimeLogicFileReservedSpace[(int)file.VolumeIndex] = ntbTimeLogicReservedSpace.Text;
            }
        }

        /// <summary>
        /// Text box import text file name changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbTimeLogicImport_TextChanged(object sender, EventArgs e)
        {
            //  if file is present
            if (file != null)
            {
                //  make sure string collections are initialized
                CheckStringCollectionInits();

                //  save the text file location
                while (Properties.Settings.Default.TimeLogicFileLocations.Count < (file.VolumeIndex + 1))
                    Properties.Settings.Default.TimeLogicFileLocations.Add(string.Empty);
                Properties.Settings.Default.TimeLogicFileLocations[(int)file.VolumeIndex] = tbAssemblyImport.Text;
            }
        }

        /// <summary>
        /// Restore the user settings.
        /// </summary>
        void RestoreSettings()
        {
            //  if file is present
            if (file != null)
            {
                //  make sure string collections are initialized
                CheckStringCollectionInits();

                //  save the reserved space
                if (Properties.Settings.Default.TimeLogicFileInclude.Count > file.VolumeIndex)
                    cbIncludeTimeLogicFile.Checked = Properties.Settings.Default.TimeLogicFileInclude[(int)file.VolumeIndex].Equals("true");

                //  save the reserved space
                if (Properties.Settings.Default.TimeLogicFileReservedSpace.Count > file.VolumeIndex)
                    ntbTimeLogicReservedSpace.Text = Properties.Settings.Default.TimeLogicFileReservedSpace[(int)file.VolumeIndex];

                //  save the text file location
                if (Properties.Settings.Default.TimeLogicFileLocations.Count > file.VolumeIndex)
                    tbAssemblyImport.Text = Properties.Settings.Default.TimeLogicFileLocations[(int)file.VolumeIndex];
            }

            //  update the file
            //UpdateTimeLogicFile();
        }
        #endregion

    }
}


#if UNUSED_CODE


#endif

