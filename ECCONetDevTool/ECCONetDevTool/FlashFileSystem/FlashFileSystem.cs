using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using ESG.ExpressionLib.DataModels;

using VolumeIndices = ECCONetDevTool.FlashFileSystem.FlashFileVolume.VolumeIndices;

namespace ECCONetDevTool.FlashFileSystem
{
    public partial class FlashFileSystem : UserControl
    {
        /// <summary>
        /// The binary generated delegate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="name">The name of the binary flash file name.</param>
        /// <param name="binary">The bin data.</param>
        /// <param name="ec">The expression collection, or null.</param>
        /// <param name="dictionaryEntrySize">The dictionary entry size, or zero.</param>
        public delegate void BinaryGeneratedDelegate(object sender, string name, byte[] binary, object userData, uint dictionaryEntrySize);


        /// <summary>
        /// Constructor.
        /// </summary>
        public FlashFileSystem()
        {
            //  initialize designer components.
            InitializeComponent();

            //  set the volume indices
            ucFlashFileVolume0.InitializeWithVolumeIndex(0);
            ucFlashFileVolume1.InitializeWithVolumeIndex(1);
            ucFlashFileVolume2.InitializeWithVolumeIndex(2);

            //  initialize the inventory control
            ucInventoryFile.SetVolume(ucFlashFileVolume0.Volume, (uint)VolumeIndices.ProductAssembly);

            //  initialize the miscellaneous file user controls
            FlashFileVolume[] volumes = new FlashFileVolume[3] { ucFlashFileVolume0.Volume, ucFlashFileVolume1.Volume, ucFlashFileVolume2.Volume };
            ucMiscFile1.SetVolumes(volumes, (uint)VolumeIndices.Miscellaneous0);
            ucMiscFile2.SetVolumes(volumes, (uint)VolumeIndices.Miscellaneous1);
            ucMiscFile3.SetVolumes(volumes, (uint)VolumeIndices.Miscellaneous2);
            ucMiscFile4.SetVolumes(volumes, (uint)VolumeIndices.Miscellaneous3);
            ucMiscFile5.SetVolumes(volumes, (uint)VolumeIndices.Miscellaneous4);
            ucMiscFile6.SetVolumes(volumes, (uint)VolumeIndices.Miscellaneous5);
            ucMiscFile7.SetVolumes(volumes, (uint)VolumeIndices.Miscellaneous6);
            ucMiscFile8.SetVolumes(volumes, (uint)VolumeIndices.Miscellaneous7);
            ucMiscFile9.SetVolumes(volumes, (uint)VolumeIndices.Miscellaneous8);
            ucMiscFile10.SetVolumes(volumes, (uint)VolumeIndices.Miscellaneous9);
            
            //  restore settings
            RestoreSettings();
        }

        #region CAN address file
        /// <summary>
        /// Update CAN address file clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateCanAddressFile_Click(object sender, EventArgs e)
        {
            UpdateCanAddressFile();
        }

        /// <summary>
        /// Updates the CAN address file based on user controls.
        /// </summary>
        private void UpdateCanAddressFile()
        {
            //  remove any instances of CAN address file
            ucFlashFileVolume0.Volume.RemoveFile(CanAddressFile.FileName);

            //  if include file is checked
            if (cbIncludeCanAddressFile.Checked)
            {
                //  try to create a new CAN address file
                ntbCanStaticAddress.GetByteValue(out byte address);
                var canAddressFile = CanAddressFile.Create(
                    (uint)VolumeIndices.CanAddress,
                    address,
                    cbCanStaticAddress.Checked,
                    out string errorMessage);

                //  check file
                if (canAddressFile == null)
                {
                    cbIncludeCanAddressFile.Checked = false;
                    MessageBox.Show(errorMessage);
                    return;
                }

                //  add the CAN address file to the volume
                ucFlashFileVolume0.Volume.AddFile(canAddressFile);
            }

            //  update the flash volume
            ucFlashFileVolume0.UpdateFlashVolume();
        }
        #endregion

        #region Product info file
        /// <summary>
        /// Update product info file button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateProductInfoFile_Click(object sender, EventArgs e)
        {
            UpdateProductInfoFile();
        }

        /// <summary>
        /// Updates the product info file as the second file in the list.
        /// </summary>
        /// <param name="address">The CAN address.</param>
        /// <param name="isStaticAddress"></param>
        /// <returns>Returns true if the file was added.</returns>
        private void UpdateProductInfoFile()
        {
            //  remove any instances of the product info file in the volume
            ucFlashFileVolume0.Volume.RemoveFile(ProductInfoFile.FileName);

            //  if include file is checked
            if (cbIncludeProductInfoFile.Checked)
            {
                //  try to create a new product info file
                var productInfoFile = ProductInfoFile.Create(
                    (uint)VolumeIndices.ProductInfo,
                    tbModelName.Text,
                    tbManufacturerName.Text,
                    tbHardwareRevision.Text,
                    tbAppFirmwareRevision.Text,
                    tbBootloaderFirmwareRevision.Text,
                    tbBaseIndexedOutputEnum.Text,
                    tbMaxIndexedOutputEnum.Text,
                    out string errorMessage);

                //  check file
                if (productInfoFile == null)
                {
                    cbIncludeProductInfoFile.Checked = false;
                    MessageBox.Show(errorMessage);
                    return;
                }

                //  add the product info file to the volume
                ucFlashFileVolume0.Volume.AddFile(productInfoFile);
            }

            //  update the flash volume
            ucFlashFileVolume0.UpdateFlashVolume();
        }
        #endregion

        #region Save and restore settings
        /// <summary>
        /// Include product info file checkbox clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbIncludeProductInfoFile_CheckedChanged(object sender, EventArgs e)
        {
            UpdateProductInfoFile();
            Properties.Settings.Default.IncludeProductInfoFile = cbIncludeProductInfoFile.Checked;
        }

        /// <summary>
        /// Include CAN address check box check changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbIncludeCanAddressFile_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCanAddressFile();
            Properties.Settings.Default.IncludeCanAddressFile = cbIncludeCanAddressFile.Checked;
        }

        /// <summary>
        /// Static CAN address check box check changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbCanStaticAddress_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCanAddressFile();
            Properties.Settings.Default.IsCanAddressStatic = cbCanStaticAddress.Checked;
        }

        private void tbModelName_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ModelName = tbModelName.Text;
        }

        private void tbManufacturerName_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ManufacturerName = tbManufacturerName.Text;
        }

        private void tbHardwareRevision_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.HardwareRevision = tbHardwareRevision.Text;
        }

        private void tbAppFirmwareRevision_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AppFirmwareRevision = tbAppFirmwareRevision.Text;
        }

        private void tbBootloaderFirmwareRevision_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.BootloaderFirmwareRevision = tbBootloaderFirmwareRevision.Text;
        }

        private void tbBaseIndexedOutputEnum_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.BaseIndexedOutputEnum = tbBaseIndexedOutputEnum.Text;
        }

        private void tbMaxIndexedOutputEnum_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MaxIndexedOutputEnum = tbMaxIndexedOutputEnum.Text;
        }

        void RestoreSettings()
        {
            //  restore the user controls
            cbIncludeProductInfoFile.Checked = Properties.Settings.Default.IncludeProductInfoFile;
            cbIncludeCanAddressFile.Checked = Properties.Settings.Default.IncludeCanAddressFile;
            cbCanStaticAddress.Checked = Properties.Settings.Default.IsCanAddressStatic;
            tbModelName.Text = Properties.Settings.Default.ModelName;
            tbManufacturerName.Text = Properties.Settings.Default.ManufacturerName;
            tbHardwareRevision.Text = Properties.Settings.Default.HardwareRevision;
            tbAppFirmwareRevision.Text = Properties.Settings.Default.AppFirmwareRevision;
            tbBootloaderFirmwareRevision.Text = Properties.Settings.Default.BootloaderFirmwareRevision;
            tbBaseIndexedOutputEnum.Text = Properties.Settings.Default.BaseIndexedOutputEnum;
            tbMaxIndexedOutputEnum.Text = Properties.Settings.Default.MaxIndexedOutputEnum;

            //  update the files
            UpdateCanAddressFile();
            UpdateProductInfoFile();
        }
        #endregion

        private void ucInventoryFile_Load(object sender, EventArgs e)
        {

        }
    }
}



#if UNUSED

        private void cbIncludeTimeLogicFile_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTimeLogicFile();
            Properties.Settings.Default.IncludeTimeLogicFile = cbIncludeTimeLogicFile.Checked;
        }

        private void ntbTimeLogicReservedSpace_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TimeLogicReservedSpace = ntbTimeLogicReservedSpace.Text;
        }

        private void tbTimeLogicImport_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TimeLogicImportFileName = tbTimeLogicImport.Text;
        }



            cbIncludeTimeLogicFile.Checked = Properties.Settings.Default.IncludeTimeLogicFile;
            ntbTimeLogicReservedSpace.Text = Properties.Settings.Default.TimeLogicReservedSpace;
            tbTimeLogicImport.Text = Properties.Settings.Default.TimeLogicImportFileName;
            UpdateTimeLogicFile();

#region Time-Logic file
        /// <summary>
        /// Button update time-logic file clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTimeLogicUpdateFile_Click(object sender, EventArgs e)
        {
            UpdateTimeLogicFile();
        }

        /// <summary>
        /// Button find time-logic text file clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFind_Click(object sender, EventArgs e)
        {
            if (openFileDialogFindTimeLogicText.ShowDialog() == DialogResult.OK)
            {
                tbTimeLogicImport.Text = openFileDialogFindTimeLogicText.FileName;
                UpdateTimeLogicFile();
            }
        }

        /// <summary>
        /// Updates the time-logic file based on user controls.
        /// </summary>
        private void UpdateTimeLogicFile()
        {
            //  remove any instances of CAN address file
            //<<--volume.RemoveFile(TimeLogicFile.FileName);

            //  if include file is checked
            if (cbIncludeTimeLogicFile.Checked)
            {
                //  try to get imported text
                string text = string.Empty;
                if (!tbTimeLogicImport.Text.Equals(string.Empty) && File.Exists(tbTimeLogicImport.Text))
                {
                    try
                    {
                        text = File.ReadAllText(tbTimeLogicImport.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Time-Logic import file name not found.");
                    }
                }

                byte[] data = new byte[100];
                if (ntbTimeLogicReservedSpace.GetUInt32Value(out UInt32 size))
                    data = new byte[size];
                else
                    data = new byte[10];
                var timeLogicFile = new TimeLogicFile(1) { Data = data, ImportedText = text };

                //  add the CAN address file to the volume
                volume.AddFile(timeLogicFile);
            }

            //  update the flash volume
            UpdateFlashVolume();
        }
#endregion

#endif

