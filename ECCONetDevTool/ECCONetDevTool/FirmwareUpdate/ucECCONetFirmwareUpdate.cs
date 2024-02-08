using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using ECCONet;
using System.Globalization;



namespace ECCONetDevTool
{
    public partial class ucECCONetFirmwareUpdate : UserControl
    {
        /// <summary>
        /// The CAN interface object.
        /// </summary>
        public ECCONetApi canInterface;

        /// <summary>
        /// The list of online devices.
        /// This list is populated by calling ProductInfoScanner.ScanForECCONetDevices
        /// a few seconds after the devices have booted.
        /// </summary>
        List<ECCONetApi.ECCONetDevice> onlineDevices;


        public ucECCONetFirmwareUpdate()
        {
            //  initialize UI components
            InitializeComponent();

            //  default file bin path
            tbxFilePath.Text = Properties.Settings.Default.FWU_FilePath;
        }


        #region Online device list
        public void OnlineDeviceListChangedHandler(List<ECCONetApi.ECCONetDevice> list)
        {
            if (this.cbbOnlineDevices.InvokeRequired)
            {
                ECCONetApi.OnlineDeviceListChangedDelegate d =
                    new ECCONetApi.OnlineDeviceListChangedDelegate(OnlineDeviceListChangedHandler);
                try
                {
                    this.Invoke(d, new object[] { list });
                }
                catch { }
            }
            else
            {
                //  save online devices
                this.onlineDevices = list;

                //  populate dropdown
                cbbOnlineDevices.Items.Clear();
                if ((null != onlineDevices) && (0 != onlineDevices.Count))
                {
                    foreach (ECCONetApi.ECCONetDevice device in onlineDevices)
                    {
                        String s = device.modelName + " / Addr " + device.address;
                        cbbOnlineDevices.Items.Add(s);
                    }
                    cbbOnlineDevices.SelectedIndex = 0;
                }

                //  enable or disable flash erase group box
                if ((null != onlineDevices) && (0 != onlineDevices.Count)
                    && (cbbOnlineDevices.SelectedIndex != -1)
                    && (cbbOnlineDevices.SelectedIndex < onlineDevices.Count))
                {
                    var device1 = onlineDevices[cbbOnlineDevices.SelectedIndex];
                    if (device1 != null)
                    {
                        //  app firmware revision
                        bool isBootloader = device1.appFirmwareRevision.Equals("btldr");
                        gbxFlashErase.Enabled = !isBootloader;
                        if (isBootloader)
                            cbxEnableFlashErase.Checked = false;
                    }
                }
            }

        }

        /// <summary>
        /// The selected device changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbbOnlineDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((null != onlineDevices) && (0 != onlineDevices.Count)
                && (cbbOnlineDevices.SelectedIndex != -1)
                && (cbbOnlineDevices.SelectedIndex < onlineDevices.Count))
            {
                //  enable or disable flash erase group box
                var device1 = onlineDevices[cbbOnlineDevices.SelectedIndex];
                if (device1 != null)
                {
                    //  app firmware revision
                    bool isBootloader = device1.appFirmwareRevision.Equals("btldr");
                    gbxFlashErase.Enabled = !isBootloader;
                    if (isBootloader)
                        cbxEnableFlashErase.Checked = false;
                }
            }
        }

        /// <summary>
        /// Gets the currently-selected device.
        /// </summary>
        /// <returns>The currently-selected device, or null if not available.</returns>
        private ECCONetApi.ECCONetDevice SelectedDevice()
        {
            //  validate selected device
            if ((null == onlineDevices) || (0 == onlineDevices.Count)
                || (cbbOnlineDevices.SelectedIndex >= onlineDevices.Count))
            {
                MessageBox.Show("Selected node not valid.");
                return null;
            }
            return onlineDevices[cbbOnlineDevices.SelectedIndex];
        }
        #endregion

        #region FLASH erase
        /// <summary>
        /// Handle flash erase enable check changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxEnableFlashErase_CheckedChanged(object sender, EventArgs e)
        {
            btnEraseApp.Enabled = cbxEnableFlashErase.Checked;
            btnEraseAll.Enabled = cbxEnableFlashErase.Checked;
        }

        /// <summary>
        /// Button to erase the app portion of flash.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEraseApp_Click(object sender, EventArgs e)
        {
            //  validate device
            var device = onlineDevices[cbbOnlineDevices.SelectedIndex];
            if (device == null)
            {
                MessageBox.Show("Online device not found", "Online device Not Found");
                return;
            }

            //  confirm erase all with user
            DialogResult result = MessageBox.Show(
                "Are you sure you want to erase the " + device.modelName + " application firmware?\n",
                "CAUTION: Firmware Erase of " + device.modelName.ToUpper(), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                //  erase device application FLASH
                canInterface.EraseDeviceApplication(device);
            }
        }

        /// <summary>
        /// Button to erase all flash on device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEraseAll_Click(object sender, EventArgs e)
        {
            //  validate device
            var device = onlineDevices[cbbOnlineDevices.SelectedIndex];
            if (device == null)
            {
                MessageBox.Show("Online device not found", "Online device Not Found");
                return;
            }

            //  confirm erase all with user
            DialogResult result = MessageBox.Show(
                "Are you sure you want to erase the " + device.modelName + " entire firmware?\n" +
                "After erasure you can no longer reprogram it via CAN " +
                "and you MUST cycle board power in order to reprogram it via SWD.",
                "CAUTION: Firmware Erase of " + device.modelName.ToUpper(), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                //  erase all device processor FLASH
                canInterface.EraseDeviceFlash(device);
            }
        }

        #endregion

        #region Firmware update

        /// <summary>
        /// User clicked button to update firmware.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateFirmware_Click(object sender, EventArgs e)
        {
            UpdateFirmware();
        }

        /// <summary>
        /// User changed the file path text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxFilePath_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.FWU_FilePath = tbxFilePath.Text;
        }

        /// <summary>
        /// User clicked find file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFindFile_Click(object sender, EventArgs e)
        {
            if (openFirmwareFileDialog.ShowDialog() == DialogResult.OK)
                tbxFilePath.Text = openFirmwareFileDialog.FileName;
        }

        /// <summary>
        /// Updates the firmware of the selected device.
        /// </summary>
        private void UpdateFirmware()
        {
            try
            {
                //  validate device
                var device = onlineDevices[cbbOnlineDevices.SelectedIndex];
                if (device == null)
                {
                    MessageBox.Show("Online device not found", "Online device Not Found");
                    return;
                }

                //  validate file path
                if (!File.Exists(tbxFilePath.Text))
                {
                    MessageBox.Show("FLASH firmware image file not found", "File Not Found");
                    return;
                }

                //  get image
                byte[] image = File.ReadAllBytes(tbxFilePath.Text);

                //  request programming
                var code = canInterface.ProgramDeviceWithImage(device, image, UpdateProgressCallback, UpdateCompleteCallback);

                //  update status label
                lblStatus.Text = code.ToString().Replace('_', ' ');
                if (code == ECCONetApi.FirmwareUpdateStatusCodes.Programming_Started)
                {
                    lblStatus.ForeColor = Color.DarkGray;

                    //  disable firmware update button
                    btnUpdateFirmware.Enabled = false;
                }
                else
                {
                    lblStatus.ForeColor = Color.Red;
                }
                lblStatus.Visible = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
        }
  

        /// <summary>
        /// The progress callback.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="device">The device being programmed.</param>
        /// <param name="percentComplete">The percent complete.</param>
        private void UpdateProgressCallback(object sender, ECCONetApi.ECCONetDevice device, int percentComplete)
        {
            if (this.cbbOnlineDevices.InvokeRequired)
            {
                ECCONetApi.FirmwareUpdateProgressDelegate d =
                    new ECCONetApi.FirmwareUpdateProgressDelegate(UpdateProgressCallback);
                try
                {
                    this.Invoke(d, new object[] { sender, device, percentComplete });
                }
                catch { }
            }
            else
            {
                //  set progress bar and hide status
                bprFirmwareProgramming.Value = percentComplete;
                lblStatus.Visible = false;
            }
        }

        private void UpdateCompleteCallback(object sender, ECCONetApi.ECCONetDevice device, ECCONetApi.FirmwareUpdateStatusCodes code)
        {
            if (this.cbbOnlineDevices.InvokeRequired)
            {
                ECCONetApi.FirmwareUpdateCompleteDelegate d =
                    new ECCONetApi.FirmwareUpdateCompleteDelegate(UpdateCompleteCallback);
                try
                {
                    this.Invoke(d, new object[] { sender, device, code });
                }
                catch { }
            }
            else
            {
                //  enable programming button
                btnUpdateFirmware.Enabled = true;

                //  show result
                lblStatus.Text = code.ToString().Replace('_', ' ');
                lblStatus.ForeColor = (code == ECCONetApi.FirmwareUpdateStatusCodes.Programming_Completed) ?
                    Color.DarkGreen : Color.Red;
                lblStatus.Visible = true;
            }
        }
        #endregion

    }
}


#if UNUSED_CODE

        /// <summary>
        /// A delegate method declaration for forwarding outgoing CAN frames to the selected USB device.
        /// </summary>
        /// <param name="id">The CAN frame ID.</param>
        /// <param name="data">The CAN frame data.</param>
        /// <returns>Returns 0 on success, else error code.</returns>
        public delegate int SendCanFrameDelegate(UInt32 id, byte[] data);

        /// <summary>
        /// The delegate for forwarding outgoing CAN frames to the selected USB device.
        /// </summary>
        public event SendCanFrameDelegate sendCanFrameDelegate;


#region Hex file parser

private enum HexFileRecordType
{
    Data,
    EndOfFile,
    ExtendedSegmentAddress,
    StartSegmentAddress,
    ExtendedLinearAddress,
    StartLinearAddress
};



Byte[] parseHexFile(string filePath, Byte fillValue)
{
    //  We create bin file. 
    Byte[] bin = new byte[0x8000];
    for (int i = 0; i < 0x8000; i++)
        bin[i] = fillValue;


    Byte byteCount;
    UInt16 lowerAddress;
    UInt16 upperAddress = 0;
    Byte recordType;
    Byte checksum;
    Byte hexByte;
    foreach (string s in File.ReadAllLines(filePath))
    {
        if ((s.Length < 11) || (s[0] != ':'))
            goto error;
        if (!Byte.TryParse(s.Substring(1, 2), System.Globalization.NumberStyles.HexNumber,
            CultureInfo.CurrentCulture, out byteCount))
            goto error;
        if (!UInt16.TryParse(s.Substring(3, 4), System.Globalization.NumberStyles.HexNumber,
            CultureInfo.CurrentCulture, out lowerAddress))
            goto error;
        if (!Byte.TryParse(s.Substring(7, 2), System.Globalization.NumberStyles.HexNumber,
            CultureInfo.CurrentCulture, out recordType))
            goto error;
        if (!Byte.TryParse(s.Substring(9 + (byteCount * 2), 2), System.Globalization.NumberStyles.HexNumber,
            CultureInfo.CurrentCulture, out checksum))
            goto error;


        switch ((HexFileRecordType)recordType)
        {
            case HexFileRecordType.Data:
                {
                    UInt32 address = (UInt32)((upperAddress << 16) | lowerAddress);
                    for (int i = 0; i < byteCount; i++, address++)
                    {
                        if (!Byte.TryParse(s.Substring(9 + (i * 2), 2), System.Globalization.NumberStyles.HexNumber,
                            CultureInfo.CurrentCulture, out hexByte))
                            goto error;
                        bin[address] = hexByte;
                    }
                    lowerAddress = (UInt16)address;
                    break;
                }

            case HexFileRecordType.EndOfFile:
                break;

            case HexFileRecordType.ExtendedSegmentAddress:
                break;

            case HexFileRecordType.StartSegmentAddress:
                break;

            case HexFileRecordType.ExtendedLinearAddress:
                if (!UInt16.TryParse(s.Substring(9, 4), System.Globalization.NumberStyles.HexNumber,
                    CultureInfo.CurrentCulture, out upperAddress))
                    goto error;
                break;

            case HexFileRecordType.StartLinearAddress:
                break;
        }
    }
    return bin;

    error:
    MessageBox.Show("Error parsing hex file.", "File Error");
    return null;
}
#endregion

#endif

