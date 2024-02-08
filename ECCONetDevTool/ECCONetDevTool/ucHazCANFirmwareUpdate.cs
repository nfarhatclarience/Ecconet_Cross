using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using ECCONet;
using HazCAN_Bootloader;



namespace ECCONetDevTool
{
    public partial class ucHazCANFirmwareUpdate : UserControl
    {
        /// <summary>
        /// The CAN interface object.
        /// </summary>
        public ECCONetApi canInterface;

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
        
        /// <summary>
        /// The HazCAN protocol bootloader.
        /// </summary>
        private HazCAN_Bootloader.Bootloader bootloader;
        
        /// <summary>
        /// UI class to update firmware using the HazCAN firmware update protocol.
        /// </summary>
        public ucHazCANFirmwareUpdate()
        {
            //  initialize UI components
            InitializeComponent();

            //  create the bootloader
            bootloader = new Bootloader();
            bootloader.sendCanFrameDelegate += SendCanFrame;

            //  restore saved bin file path
            tbxFilePath.Text = Properties.Settings.Default.BinFilePath;
        }
        
        #region Send and Receive CAN frames
        /// <summary>
        /// Sends a CAN frame to the selected USB-CAN device.
        /// </summary>
        /// <param name="id">The CAN frame ID.</param>
        /// <param name="data">The CAN frame data.</param>
        public int SendCanFrame(UInt32 id, byte[] data)
        {
            //  forward CAN frame to application
            if (null != sendCanFrameDelegate)
                return sendCanFrameDelegate(id, data);
            return -1;
        }

        /// <summary>
        /// Receives a CAN frame from the selected USB-CAN device.
        /// </summary>
        /// <param name="id">The CAN frame ID.</param>
        /// <param name="data">The CAN frame data.</param>
        public void ReceiveCanFrame(UInt32 id, byte[] data)
        {
            if (bootloader != null)
                bootloader.ReceiveCanFrame(id, data);
        }
        #endregion

        #region Scan for HazCAN protocol compliant devices
        /// <summary>
        /// User clicked the Scan button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnScanHazCAN_Click(object sender, EventArgs e)
        {
            ScanForDevices();
        }

        /// <summary>
        /// Scan to see if any devices in the list are present.
        /// </summary>
        private void ScanForDevices()
        {
            //  make sure bootloader exists
            if (bootloader == null)
                return;

            //  disable controls
            btnScanHazCAN.Enabled = false;
            gbxUpdateFirmware.Enabled = false;

            //  clear results and set status
            rtbBusScan.Text = string.Empty;
            lblScanning.Text = "Scanning";
            lblScanning.Visible = true;

            //  start scan
            bootloader.StartBusScan(ScanComplete,true);
        }

        /// <summary>
        /// Scan complete.
        /// </summary>
        public void ScanComplete(object sender, BindingList<HazCAN_Device> discoveredDevices)
        {
            if (btnScanHazCAN.InvokeRequired)
            {
                Bootloader.BusScanCompleteDelegate d = new Bootloader.BusScanCompleteDelegate(ScanComplete);
                try
                {
                    this.Invoke(d, new object[] { sender, discoveredDevices });
                }
                catch { }
            }
            else
            {
                //  set status and show resutls
                lblScanning.Text = "Scan Complete";
                rtbBusScan.Text = string.Empty;
                foreach (var device in discoveredDevices)
                    rtbBusScan.Text += (device.InfoString() + "\n");

                //  enable controls
                gbxUpdateFirmware.Enabled = true;
                btnScanHazCAN.Enabled = true;
            }
        }
        #endregion

        #region Program device

        /// <summary>
        /// Button to open and open file dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFindFile_Click(object sender, EventArgs e)
        {
            DialogResult result = openFirmwareFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                tbxFilePath.Text = openFirmwareFileDialog.FileName;
            }
        }

        /// <summary>
        /// Button to update the firmware.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateFirmware_Click(object sender, EventArgs e)
        {
            ProgramDeviceViaHazCAN();
        }

        /// <summary>
        /// Bin file path changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxFilePath_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.BinFilePath = tbxFilePath.Text;
        }

        /// <summary>
        /// Programs a device using the HazCAN protocal.  The device to be programmed is determined by the selected bin file.
        /// </summary>
        private void ProgramDeviceViaHazCAN()
        {
            //  validate bootloader
            if (bootloader == null)
                return;

            try
            {
                //  check the bin file directory
                String filePath = tbxFilePath.Text;
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Please select the bin file.", "Bin File Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }

                //  read the bin file
                Byte[] bin = File.ReadAllBytes(filePath);

                int result = bootloader.ProgramDevice(bin, ProgrammingProgressDelegate, ProgrammingCompleteDelegate);
                if (result == -1)
                {
                    MessageBox.Show("Already busy programming.  Please try again in a few seconds.", "Error");
                    return;
                }
                else if (result == -2)
                {
                    MessageBox.Show("Bin file does not have a valid CRC.", "Bin File Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }

                //  set UI controls
                lblPassFail.Text = "";
                lblPassFail.Visible = false;
                bprFirmwareProgramming.Value = 0;
                gbxUpdateFirmware.Enabled = false;
                bprFirmwareProgramming.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        /// <summary>
        /// A programming progress delegate.
        /// </summary>
        /// <param name="sender">The callback sender.
        /// <param name="percentComplete">The percent of programming complete.</param>
        public void ProgrammingProgressDelegate(object sender, int percentComplete)
        {
            if (bprFirmwareProgramming.InvokeRequired)
            {
                Bootloader.ProgrammingProgressDelegate d = new Bootloader.ProgrammingProgressDelegate(ProgrammingProgressDelegate);
                try
                {
                    this.Invoke(d, new object[] { sender, percentComplete });
                }
                catch { }
            }
            else
            {
                //  show progress
                bprFirmwareProgramming.Value = percentComplete;
            }

        }

        /// <summary>
        /// A programming complete delegate.
        /// </summary>
        /// <param name="sender">The callback sender.
        /// <param name="result">The programming result code.</param>
        public void ProgrammingCompleteDelegate(object sender, Bootloader.ProgramResultCodes result)
        {
            if (lblPassFail.InvokeRequired)
            {
                Bootloader.ProgrammingCompleteDelegate d = new Bootloader.ProgrammingCompleteDelegate(ProgrammingCompleteDelegate);
                try
                {
                    this.Invoke(d, new object[] { sender, result });
                }
                catch { }
            }
            else
            {
                //  show result
                lblPassFail.Text = result.ToString().Replace("_", " ");
                lblPassFail.ForeColor = (result == Bootloader.ProgramResultCodes.Programming_Completed) ?
                    Color.Green : Color.DarkRed;
                lblPassFail.Visible = true;

                //  turn the group box back on
                gbxUpdateFirmware.Enabled = true;
            }
        }
        #endregion
    }
}


#if UNUSED_CODE


#region Info messages
        delegate void SetTextCallback(string text);
        int messageCtr = 0;
        /// <summary>
        /// Includes a new line of text into the information Listview
        /// </summary>
        /// <param name="strMsg">Text to be included</param>
        private void IncludeTextMessage(string strMsg)
        {
            if (lbxInfo.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(IncludeTextMessage);
                this.Invoke(d, new object[] { strMsg });
            }
            else
            {
                ++messageCtr;
                string msg = messageCtr.ToString();
                while (msg.Length < 6)
                    msg += " ";
                msg += strMsg;
                lbxInfo.Items.Add(msg);
                lbxInfo.SelectedIndex = lbxInfo.Items.Count - 1;
            }
        }

        /// <summary>
        /// Button to clear the info box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInfoClear_Click(object sender, EventArgs e)
        {
            lbxInfo.Items.Clear();
            messageCtr = 0;
        }
#endregion

#endif