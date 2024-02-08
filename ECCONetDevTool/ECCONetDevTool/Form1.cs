using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using ECCONet;
using ECCONetDevTool.FlashFileSystem;

namespace ECCONetDevTool
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// The CAN interface object.
        /// </summary>
        ECCONetApi canInterface;

        /// <summary>
        /// The array of online devices.
        /// This array is populated by calling ProductInfoScanner.ScanForECCONetDevices
        /// a few seconds after the devices have booted.
        /// </summary>
        //List<ECCONetApi.ECCONetDevice> onlineDevices;



        /// <summary>
        /// Constructor.
        /// </summary>
        public Form1()
        {
            //  initialize form
            InitializeComponent();

            //  intialize CAN interface
            canInterface = new ECCONetApi();
            //canInterface.usbCanApi.transmitDelaymS = 3;
            onlineMonitor.canInterface = canInterface;
            busMonitor.canInterface = canInterface;
            ftp.canInterface = canInterface;
            firmwwareUpdate.canInterface = canInterface;
            miscTools.canInterface = canInterface;
            ucBusStressTester.ucNodePingStatistics.canInterface = canInterface;
            ucBusStressTester.ucBusFlood.canInterface = canInterface;
            ucBusStressTester.ucTokenToggle.canInterface = canInterface;
            ucLedMatrixFile.canInterface = canInterface;
            ucECCONetFirmwareUpdate.canInterface = canInterface;

            //  monitor online devices
            canInterface.onlineDeviceListChangedDelegate = onlineMonitor.OnlineDeviceListChangedHandler;
            canInterface.onlineDeviceListChangedDelegate += ftp.OnlineDeviceListChangedHandler;
            canInterface.onlineDeviceListChangedDelegate += miscTools.OnlineDeviceListChangedHandler;
            canInterface.onlineDeviceListChangedDelegate += ucBusStressTester.ucNodePingStatistics.OnlineDeviceListChangedHandler;
            canInterface.onlineDeviceListChangedDelegate += ucLedMatrixFile.OnlineDeviceListChangedHandler;
            canInterface.onlineDeviceListChangedDelegate += ucECCONetFirmwareUpdate.OnlineDeviceListChangedHandler;

            //  online monitor USB-CAN ports
            canInterface.connectionStatusChangedDelegate = onlineMonitor.ConnectionStatusChangedHandler;

            //  bus monitor tokens
            canInterface.receiveToken += busMonitor.CanInterface_receiveToken;
            canInterface.receiveToken += onlineMonitor.CanInterface_receiveToken;

            //  firmware update CAN frames
            firmwwareUpdate.sendCanFrameDelegate += canInterface.SendCanFrame;
            canInterface.receiveCanFrame += firmwwareUpdate.ReceiveCanFrame;

            //  expression binary generated
            patternGeneration.binaryGeneratedDelegate += flashFileSystem.ucFlashFileVolume0.BinaryGeneratedHandler;

            //  LED matrix binary generated
            ucLedMatrixFile.binaryGeneratedDelegate += flashFileSystem.ucFlashFileVolume0.BinaryGeneratedHandler;

            //  equation bytecode tabs
            equationsDefault.UserProfileIndex = 0;
            equationsDefault.binaryGeneratedDelegate += flashFileSystem.ucFlashFileVolume0.BinaryGeneratedHandler;
            equationsProfile1.UserProfileIndex = 1;
            equationsProfile1.binaryGeneratedDelegate += flashFileSystem.ucFlashFileVolume0.BinaryGeneratedHandler;
            equationsProfile2.UserProfileIndex = 2;
            equationsProfile2.binaryGeneratedDelegate += flashFileSystem.ucFlashFileVolume0.BinaryGeneratedHandler;
            equationsProfile3.UserProfileIndex = 3;
            equationsProfile3.binaryGeneratedDelegate += flashFileSystem.ucFlashFileVolume0.BinaryGeneratedHandler;
            equationsProfile4.UserProfileIndex = 4;
            equationsProfile4.binaryGeneratedDelegate += flashFileSystem.ucFlashFileVolume0.BinaryGeneratedHandler;
            equationsProfile5.UserProfileIndex = 5;
            equationsProfile5.binaryGeneratedDelegate += flashFileSystem.ucFlashFileVolume0.BinaryGeneratedHandler;
            equationsProfile6.UserProfileIndex = 6;
            equationsProfile6.binaryGeneratedDelegate += flashFileSystem.ucFlashFileVolume0.BinaryGeneratedHandler;

            //  transmit delay
            if (canInterface != null && canInterface.usbCanApi != null && onlineMonitor != null && onlineMonitor.cbbTransmitDelay != null)
            {
                var transmitDelaymS = onlineMonitor.cbbTransmitDelay.SelectedIndex + 1;
                if (transmitDelaymS >= 1 && transmitDelaymS <= 5)
                    canInterface.usbCanApi.transmitDelaymS = transmitDelaymS;
            }
        }

        /// <summary>
        /// Form closing is used to close the CAN hardware connections,
        /// thus ending threads and releasing resources.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //  disconnect both USB-CAN and PCAN-USB
            canInterface.Disconnect();

            //  dispose of the CAN interface resources
            canInterface.CloseAll();

            //  save the user settings
            Properties.Settings.Default.Save();

            //  save the token key dictionary
            try
            {
                XElement xElem = new XElement("items",
                        busMonitor.keyCodeToName.Select(x => new XElement("item", new XAttribute("id", x.Key), new XAttribute("value", x.Value)))
                     );
                File.WriteAllText("ECCONetTokenKeys.xml", xElem.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}


