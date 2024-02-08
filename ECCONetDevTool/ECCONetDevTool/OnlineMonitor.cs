using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using ECCONet;
using static ECCONet.ECCONetApi;

namespace ECCONetDevTool
{
    public partial class OnlineMonitor : UserControl
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

        public OnlineMonitor()
        {
            //  initialize designer components
            InitializeComponent();

            //  restore settings
            cbxRetainOfflineDevices.Checked = Properties.Settings.Default.ODM_RetainOfflineDevices;
            cbxForcePowerState.Checked = Properties.Settings.Default.ODM_SystemPowerState;
            if (Properties.Settings.Default.TransmitDelaymS >= 1 && Properties.Settings.Default.TransmitDelaymS <= 5)
                cbbTransmitDelay.SelectedIndex = Properties.Settings.Default.TransmitDelaymS - 1;
        }

        #region Online device list changed handler
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

                //  set ECCONet API states
                if (canInterface !=  null)
                {
                    canInterface.RetainOfflineDevices = cbxRetainOfflineDevices.Checked;
                    canInterface.ShouldTransmitSystemPowerState = cbxForcePowerState.Checked;
                }
            }
        }
        #endregion

        #region Device combobox
        /// <summary>
        /// This just demonstrates how to select an available ECCONet 3.0 device.
        /// </summary>
        /// <param name="sender">The combo box sender.</param>
        /// <param name="e">The event arguments.</param>
        public void cbbOnlineDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbECCONetOnlineDevice.Text = "";
            if ((null != onlineDevices) && (0 != onlineDevices.Count)
                && (cbbOnlineDevices.SelectedIndex < onlineDevices.Count))
            {
                //  get device
                ECCONetApi.ECCONetDevice device = onlineDevices[cbbOnlineDevices.SelectedIndex];

                //  address and online time
                String s = "Address: " + device.address;
                s += "\nOnline: " + device.lastStatusTime.ToLongTimeString();
                s += "\n";

                //  model and manufacturer
                s += "\nModel: " + device.modelName;
                s += "\nManufacturer: " + device.manufacturerName;
                s += "\n";

                //  app firmware revision
                if (device.appFirmwareRevision.Equals("btldr"))
                    s += "\nRunning in bootloader mode.";
                else if (!device.appFirmwareRevision.Equals(String.Empty)
                    && !device.appFirmwareRevision.Equals("n/a"))
                    s += "\nApp Firmware Revision: " + device.appFirmwareRevision;

                //  bootloader firmware revision
                if (!device.bootloaderFirmwareRevision.Equals(String.Empty)
                    && !device.bootloaderFirmwareRevision.Equals("n/a"))
                    s += "\nBootloader Firmware Revision: " + device.bootloaderFirmwareRevision;
                s += "\n";

                //  hardware revision
                if (!device.hardwareRevision.Equals(String.Empty)
                    && !device.hardwareRevision.Equals("n/a"))
                    s += "\nHardware Revision: " + device.hardwareRevision;

                //  device GUID
                if ((null != device.guid) && (4 == device.guid.Length))
                    s += ("\nGUID: " + device.guid[3].ToString("X2") + device.guid[2].ToString("X2")
                        + device.guid[1].ToString("X2") + device.guid[0].ToString("X2"));

                //  product info file date
                if (!device.appFirmwareRevision.Equals("btldr"))
                    s += "\n\nProduct Info File Date: " + device.infoFileDate.ToString();

                //  base lighthead enumeration
                if (!device.baseLightheadEnumeration.Equals(String.Empty)
                    && !device.baseLightheadEnumeration.Equals("n/a"))
                    s += "\n\nBase Lighthead Enumeration: " + device.baseLightheadEnumeration;

                //  max lighthead enumeration
                if (!device.maxLightheadEnumeration.Equals(String.Empty)
                    && !device.maxLightheadEnumeration.Equals("n/a"))
                    s += "\nMaximum Lighthead Enumeration: " + device.maxLightheadEnumeration;

                //  write to text box
                tbECCONetOnlineDevice.Text = s;
            }
        }
        #endregion

        #region Online device list
        /// <summary>
        /// Clears the lib's list of online devices.  The list will automatically regenerate.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void btnClearOnlineDevicesList_Click(object sender, EventArgs e)
        {
            //  clear text box
            tbECCONetOnlineDevice.Text = String.Empty;

            //  clear the online devices lise
            canInterface.ClearOnlineDevicesList();
        }
        #endregion

        #region Connection selection and status

        /// <summary>
        /// Handles the check changed event.
        /// </summary>
        /// <param name="sender">The sender radion button.</param>
        /// <param name="e">The event arguments.</param>
        private void radBtnPcanUsb_CheckedChanged(object sender, EventArgs e)
        {
            canInterface.selectedCanBusInterfaceDevice = radBtnPcanUsb.Checked
                ? ECCONetApi.CANBusInterfaceDevice.PcanUsbDevice
                : ECCONetApi.CANBusInterfaceDevice.C3UsbCanDevice;
        }

        /// <summary>
        /// Handles the check changed event.
        /// </summary>
        /// <param name="sender">The sender radion button.</param>
        /// <param name="e">The event arguments.</param>
        private void radBtnUsbCan_CheckedChanged(object sender, EventArgs e)
        {
            canInterface.selectedCanBusInterfaceDevice = radBtnPcanUsb.Checked
                ? ECCONetApi.CANBusInterfaceDevice.PcanUsbDevice
                : ECCONetApi.CANBusInterfaceDevice.C3UsbCanDevice;
        }

        /// <summary>
        /// Connection status changed handler.
        /// </summary>
        /// <param name="pcanUsbIsConnected">Indicates whether the PCAN-USB device is connected.</param>
        /// <param name="usbCanIsConnected">Indicates whether the Code 3 USB-CAN devices is connected.</param>
        public void ConnectionStatusChangedHandler(bool pcanUsbIsConnected, bool usbCanIsConnected)
        {
            if (this.labelStatusPcanUsb.InvokeRequired)
            {
                ConnectionStatusChangedDelegate d = new ConnectionStatusChangedDelegate(ConnectionStatusChangedHandler);
                try
                {
                    this.Invoke(d, new object[] { pcanUsbIsConnected, usbCanIsConnected });
                }
                catch { }
            }
            else
            {
                labelStatusPcanUsb.Text = pcanUsbIsConnected ? "Status: Connected" : "Status: Not Connected";
                labelStatusUsbCan.Text = usbCanIsConnected ? "Status: Connected" : "Status: Not Connected";
            }
        }
        public void ConnectionStatusChangedHandler(bool usbCanIsConnected)
        {
            if (this.labelStatusPcanUsb.InvokeRequired)
            {
                ConnectionStatusChangedDelegate d = new ConnectionStatusChangedDelegate(ConnectionStatusChangedHandler);
                try
                {
                    this.Invoke(d, new object[] { usbCanIsConnected });
                }
                catch { }
            }
            else
            {
                
                labelStatusUsbCan.Text = usbCanIsConnected ? "Status: Connected" : "Status: Not Connected";
            }
        }
        #endregion

        #region Token received from bus
        /// <summary>
        /// Token received event.
        /// </summary>
        /// <param name="token">The token that was received.</param>
        public void CanInterface_receiveToken(Token token)
        {
            if (this.pbxPowerState.InvokeRequired)
            {
                ReceiveTokenDelegate d = new ReceiveTokenDelegate(CanInterface_receiveToken);
                try
                {
                    this.Invoke(d, new object[] { token });
                }
                catch { }
            }
            else
            {
                switch (Token.Key_WithoutPrefix(token.key))
                {
                    case Token.Keys.KeyKeyPosition:
                        pbxAcc.BackColor = ((token.value & 0x01) != 0) ? Color.Green : Color.White;
                        pbxRun.BackColor = ((token.value & 0x02) != 0) ? Color.Green : Color.White;
                        break;

                    case Token.Keys.KeySystemPowerState:
                        pbxPowerState.BackColor = (token.value != 0) ? Color.Green : Color.White;
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion

        #region System power state control

        /// <summary>
        /// User checked the force system power state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxForcePowerState_CheckedChanged(object sender, EventArgs e)
        {
            if (canInterface != null)
            {
                canInterface.ShouldTransmitSystemPowerState = cbxForcePowerState.Checked;
            }
            Properties.Settings.Default.ODM_SystemPowerState = cbxForcePowerState.Checked;
        }
        #endregion

        #region Retain offline devices
        private void cbxRetainOfflineDevices_CheckedChanged(object sender, EventArgs e)
        {
            if (canInterface != null)
            {
                canInterface.RetainOfflineDevices = cbxRetainOfflineDevices.Checked;
            }
            Properties.Settings.Default.ODM_RetainOfflineDevices = cbxRetainOfflineDevices.Checked;
        }
        #endregion

        #region Transmit delay
        //  user changed transmit rate
        private void cbbTransmitDelay_SelectedIndexChanged(object sender, EventArgs e)
        {
            var transmitDelaymS = cbbTransmitDelay.SelectedIndex + 1;
            if (transmitDelaymS >= 1 && transmitDelaymS <= 5)
            {
                Properties.Settings.Default.TransmitDelaymS = transmitDelaymS;
                if (canInterface != null && canInterface.usbCanApi != null)
                    canInterface.usbCanApi.transmitDelaymS = transmitDelaymS;
            }
        }
        #endregion

    }
}
