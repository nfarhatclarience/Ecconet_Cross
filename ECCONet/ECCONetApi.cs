/**
  ******************************************************************************
  * @file    	CANInterface.cs
  * @copyright  © 2017 ECCO Group.  All rights reserved.
  * @author  	M. Latham, Liquid Logic, LLC
  * @version 	1.0.0
  * @date    	May 2017
  * @brief   	The ECCONet library API for system communication.
	*							
  ******************************************************************************
  * @attention
  *
  * Unless required by applicable law or agreed to in writing, software created
  * by Liquid Logic, LLC is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES
  * OR CONDITIONS OF ANY KIND, either express or implied.
  *
  ******************************************************************************
  */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using System.IO;
using System.Diagnostics;
//using ECCONet.PcanUsb; 
using ECCONet.UsbCan;

namespace ECCONet
{
    public partial class ECCONetApi
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The PC CAN address.
        /// </summary>
        public const byte PC_CAN_Address = (byte)LibConfig.MATRIX_PC_ADDRESS;

        /// <summary>
        /// The device file names.
        /// NOTE: This class is depreciated as the individual libs that create such files
        /// have their own definitions.
        /// </summary>
        public class FileName
        {
            public const string ProductInfo = "product.inf";
            public const string Equations = "equation.txt";
            public class Patterns
            {
                public const string TokenSequencer0 = "patterns.tb0";
                public const string TokenSequencer1 = "patterns.tb1";
                public const string TokenSequencer2 = "patterns.tb2";
                public const string TokenSequencer3 = "patterns.tb3";
            }
        }

        /// <summary>
        /// Device online event enumeration.
        /// </summary>
        public enum DeviceOnlineEvent
        {
            JustCameOnline,
            JustProvidedProductInfo,
            JustIndicatedNoProductInfoAvailable,
            JustWentOffline,
            NoEvent,
            JustProvidedPartialInfo
        }

        /// <summary>
        /// The ECCONet PCAN-USB device API.
        /// </summary>
        //public ECCONet_PcanUsbApi pcanUsbApi { get { return _pcanUsbApi; } }
        //ECCONet_PcanUsbApi _pcanUsbApi;

        /// <summary>
        /// The Code 3 USB-CAN device API.
        /// </summary>
        //public ECCONet_UsbDotNetCanApi usbCanApi { get { return _usbCanApi; } }
        public ECCONet_UsbDotNetCanApi usbCanApi { get { return _usbCanApi; } }

        ECCONet_UsbDotNetCanApi _usbCanApi;

        /// <summary>
        /// The ECCONet core logic.
        /// </summary>
        ECCONetCore core;

        /// <summary>
        /// The bus analyzer.
        /// </summary>
        BusAnalyzer busAnalyzer;

        /// <summary>
        /// The firmware programmer.
        /// </summary>
        FirmwareUpdate firmwareProgrammer;


        /// <summary>
        /// Constructor.
        /// </summary>
        public ECCONetApi()
        {
            Reset();
        }
        
        public void Reset()
        {
            //  disconnect from drivers and wait 100 mS
           // if (_pcanUsbApi != null)
            //    _pcanUsbApi.Disconnect();
            if (_usbCanApi != null)
                _usbCanApi.Disconnect();
            Thread.Sleep(100);

            //  create the core
            core = new ECCONetCore();

            //  this SendCanFrame method is the core's send CAN frame delegate
            core.sendFrameDelegate = this.SendCanFrame;

            //  this ReceiveToken method is the core's receive token delegate
            core.receiveTokenDelegate = this.ReceiveToken;

            //  create the bus analyzer
            busAnalyzer = new BusAnalyzer(core);

            //  create the firmware programmer
            firmwareProgrammer = new FirmwareUpdate(core);


            //  create the PCAN-USB API, requesting auto-connect
            //_pcanUsbApi = new ECCONet_PcanUsbApi(true);

            //  create the USB-CAN API, requesting auto-connect
            _usbCanApi = new ECCONet_UsbDotNetCanApi(true);
           //_usbCanApi = new ECCONet_UsbDotNetCanApi(true);

            //  this PcanUsbConnectionStatusChanged is the PCAN-USB connection status changed delegate
            //_pcanUsbApi.connectionStatusChangedDelegate = PcanUsbConnectionStatusChanged;

            //  this UsbCanConnectionStatusChanged is the USB-CAN connection status changed delegate
            _usbCanApi.connectionStatusChangedDelegate = UsbCanConnectionStatusChanged;

            //  connect the CAN frame received callback from one of the two CAN APIs
            SetCanFrameReceiveSource(_selectedCanBusInterfaceDevice);

            //  set the core debug level
            //core.consoleDebugMessageLevel = ECCONetCore.ConsoleDebugMessageLevels.FtpRequestsAndResponses;
            core.consoleDebugMessageLevel = ECCONetCore.ConsoleDebugMessageLevels.None;
        }

        /// <summary>
        /// Reconnects the USB-CAN device.  Only needed when external USB device glitches. 
        /// Normal connection and disconnection of USB cable or device power cycle does not require this method.
        /// </summary>
        public void Reconnect_USB_CAN()
        {
            if (_usbCanApi != null)
                _usbCanApi.Reconnect();
        }

        /// <summary>
        /// Disconnect from interfaces when closing.
        /// </summary>
        ~ECCONetApi()
        {
            //.Disconnect();
            _usbCanApi.Disconnect();
        }

        /// <summary>
        /// Only to be called when application closes.  This is a programmatic destructor.
        /// </summary>
        public void CloseAll()
        {
            if (busAnalyzer != null)
            {
                busAnalyzer.StopAll();
                busAnalyzer = null;
            }
            firmwareProgrammer = null;
            core = null;
        }

        #region Bus analyzer
        /// <summary>
        /// Gets a value indicating whether the bus flood is running.
        /// </summary>
        public bool IsBusFloodRunning
        {
            get => busAnalyzer.IsBusFloodRunning;
        }

        /// <summary>
        /// Floods the bus with messages at the given message size and rate.
        /// A bus flood message is a series of tokens all transmitted together.
        /// </summary>
        /// <param name="numPacketsPerMessage">The number of packets per message (min=1, max=25).</param>
        /// <param name="numMessagesPerSecond">The number of messages per second (min=1, max=50).</param>
        /// <param name="token">The token to use, or null to use the default token.</param>
        /// <returns>Returns 0 if flood started, -1 if invalid packets per message, -2 if invalid messages per second,
        ///  and -3 if no CAN bus connection.</returns>
        public int FloodBusStart(uint numPacketsPerMessage, uint numMessagesPerSecond, Token token, ECCONetApi.BusFloodDelegate callback)
        {
            //  validate CAN connection
            if (selectedCanBusInterfaceDevice == CANBusInterfaceDevice.C3UsbCanDevice)
            {
                if (!_usbCanApi.isConnectedAndReady)
                    return -3;
            }
            //else  //  PCAN-USB selected
           // {
              //  if (!_pcanUsbApi.isConnectedAndReady)
                  //  return -3;
           // }

            //  start flood
            return busAnalyzer.FloodBusStart(numPacketsPerMessage, numMessagesPerSecond, token, callback);
        }

        /// <summary>
        /// Stops the bus flood.
        /// </summary>
        public void BusFloodStop()
        {
            busAnalyzer.BusFloodStop();
        }

        /// <summary>
        /// Gets a value indicating whether a bus statistics ping test is running.
        /// </summary>
        public bool IsBusStatisticsPingTestRunning
        {
            get => busAnalyzer.IsBusStatisticsPingTestRunning;
        }

        /// <summary>
        /// A method for analyzing the bus integrity by pinging individual nodes.
        /// Note that poor-performing nodes can falsely drive up the error rate.
        /// </summary>
        /// <param name="devices">A list of ECCONet devices to be pinged.</param>
        /// <param name="duration">The test duration, minimum is 100 mS.</param>
        /// <param name="mode">The ping mode.</param>
        /// <param name="filePath">The local path of a file to write, for write mode.</param>
        /// <param name="randomFileSize">For random file write/read/delete mode, the file size.</param>
        /// <param name="progressCallback">The method to call back with the progress.</param>
        /// <param name="completeCallback">The method to call back with the results.</param>
        /// <returns>Returns 0 if test started, -1 if already busy running test, or -2 on bad test parameters.</returns>
        public int StartBusStatisticsPingTest(List<ECCONetDevice> devices, TimeSpan duration, PingTestMode mode,
            string filePath, int randomFileSize,
            BusAnalysisProgressDelegate progressCallback,
            BusAnalysisCompleteDelegate completeCallback)
        {
            return busAnalyzer.PingNodes(devices, duration, 0, mode, filePath, randomFileSize, progressCallback, completeCallback);
        }

        /// <summary>
        /// A method for analyzing the bus integrity by pinging individual nodes.
        /// Note that poor-performing nodes can falsely drive up the error rate.
        /// </summary>
        /// <param name="devices">A list of ECCONet devices to be pinged.</param>
        /// <param name="numPingsPerNode">The number of times to ping each node. A zero value uses the test duration.</param>
        /// <param name="mode">The ping mode.</param>
        /// <param name="filePath">The local path of a file to write, for write mode.</param>
        /// <param name="randomFileSize">For random file write/read/delete mode, the file size.</param>
        /// <param name="progressCallback">The method to call back with the progress.</param>
        /// <param name="completeCallback">The method to call back with the results.</param>
        /// <returns>Returns 0 if test started, -1 if already busy running test, or -2 on bad test parameters.</returns>
        public int StartBusStatisticsPingTest(List<ECCONetDevice> devices, uint numPingsPerNode, PingTestMode mode,
            string filePath, int randomFileSize,
            BusAnalysisProgressDelegate progressCallback,
            BusAnalysisCompleteDelegate completeCallback)
        {
            return busAnalyzer.PingNodes(devices, new TimeSpan(), numPingsPerNode, mode, filePath, randomFileSize, progressCallback, completeCallback);
        }

        /// <summary>
        /// Stops any node pinging that is currently running.
        /// </summary>
        public void StopBusStatisticsPingTest()
        {
            busAnalyzer.StopPinging();
        }


        /// <summary>
        /// Gets a value indicating whether the token toggle is running.
        /// </summary>
        public bool IsTokenToggleRunning
        {
            get => (busAnalyzer.IsTokenToggleRunning);
        }

        /// <summary>
        /// Periodicically sends a token with alternating values.
        /// </summary>
        /// <param name="token1">The first token to send.</param>
        /// <param name="token2">The second token to send.</param>
        /// <param name="numTokensPerSecond">The number of tokens per second to send.</param>
        /// <param name="callback">The token toggle number of tokens sent callback.</param>
        /// <returns>Returns 0 if token toggle started, -1 if busy, and -2 if bad params.</returns>
        public int TokenToggleStart(Token token1, Token token2, int numTokensPerSecond, ECCONetApi.TokenToggleDelegate callback)
        {
            return busAnalyzer.TokenToggleStart(token1, token2, numTokensPerSecond, callback);
        }

        /// <summary>
        /// Stops the token toggle.
        /// </summary>
        public void TokenToggleStop()
        {
            busAnalyzer.TokenToggleStop();
        }
        #endregion

        #region Online device monitor
        /// <summary>
        /// When true, devices that go offline remain in the list.
        /// </summary>
        public bool RetainOfflineDevices
        {
            get
            {
                if ((core != null) && (core.deviceMonitor != null))
                    return core.deviceMonitor.RetainOfflineDevices;
                return false;
            }
            set
            {
                if ((core != null) && (core.deviceMonitor != null))
                    core.deviceMonitor.RetainOfflineDevices = value;
            }
        }

        /// <summary>
        /// The maximum number of times the info getter should try to get a device's info.
        /// When set to zero, the info getter tries until info is acquired.
        /// </summary>
        public int MaximumGetInfoTries
        {
            get
            {
                if ((core != null) && (core.deviceMonitor != null))
                    return core.deviceMonitor.MaximumGetInfoTries;
                return 0;
            }
            set
            {
                if ((core != null) && (core.deviceMonitor != null))
                    core.deviceMonitor.MaximumGetInfoTries = value;
            }
        }

        /// <summary>
        /// Pauses the monitor.
        /// </summary>
        public bool Pause
        {
            get
            {
                if ((core != null) && (core.deviceMonitor != null))
                    return core.deviceMonitor.Pause;
                return false;
            }
            set
            {
                if ((core != null) && (core.deviceMonitor != null))
                    core.deviceMonitor.Pause = value;
            }
        }

        /// <summary>
        /// Enables sending dummy token, defaults to enabled.  If WinUSB tries to send but cannot, then it will try to reset itself.
        /// This is useful because the online device monitor is a passive listener unless it sees a node.
        /// </summary>
        public bool SendDummyToken
        {
            get
            {
                if ((core != null) && (core.deviceMonitor != null))
                    return core.deviceMonitor.SendDummyToken;
                return false;
            }
            set
            {
                if ((core != null) && (core.deviceMonitor != null))
                    core.deviceMonitor.SendDummyToken = value;
            }
        }

        /// <summary>
        /// Delegate for notifying subscriber of change to online device list.
        /// </summary>
        /// <param name="token">The received token.</param>
        public delegate void OnlineDeviceListChangedDelegate(List<ECCONetApi.ECCONetDevice> list);

        /// <summary>
        /// Delegate for notifying subscriber of change to online device list.
        /// </summary>
        public OnlineDeviceListChangedDelegate onlineDeviceListChangedDelegate
        {
            get { return core.deviceMonitor.onlineDeviceListChanged; }
            set { core.deviceMonitor.onlineDeviceListChanged = value;  }
        }

        /// <summary>
        /// Delegate for notifying subscriber of device online status change event.
        /// </summary>
        /// <param name="token">The received token.</param>
        public delegate void DeviceOnlineStatusEventDelegate(ECCONetApi.ECCONetDevice device, DeviceOnlineEvent deviceEvent);

        /// <summary>
        /// Delegate for notifying subscriber of device online status change event.
        /// </summary>
        public DeviceOnlineStatusEventDelegate deviceOnlineStatusEvent
        {
            get { return core.deviceMonitor.deviceOnlineStatusEvent; }
            set { core.deviceMonitor.deviceOnlineStatusEvent = value; }
        }

        /// <summary>
        /// Gets a deep copy of the online devices list.
        /// </summary>
        /// <param name="filteredByProductInfo">Set true to only get devices that responded to product info request.</param>
        /// <returns>A deep copy of the online devices list.</returns>
        public List<ECCONetApi.ECCONetDevice> GetOnlineDevicesList(bool filteredByProductInfo)
        {
            return core.deviceMonitor.GetOnlineDevicesList(filteredByProductInfo);
        }

        /// <summary>
        /// Clears the list of online devices.  The list will automatically regenerate.
        /// </summary>
        public void ClearOnlineDevicesList()
        {
            core.deviceMonitor.ClearOnlineDevicesList();
        }

        public void StopProcessManager()
        {
            core.StopProcessManager();
            //_pcanUsbApi.StopProcessManager();
            _usbCanApi.StopProcessManager();
        }

        public void StartProcessManager()
        {
            core.StartProcessManager();
          //  _pcanUsbApi.StartProcessManager();
            _usbCanApi.StartProcessManager();
        }

        #endregion

        #region Transmit system power state
        /// <summary>
        /// Indicates whether the API should automatically transmit an Active
        /// system power state once per second.
        /// </summary>
        public bool ShouldTransmitSystemPowerState
        {
            get
            {
                if (core != null)
                    return core.shouldTransmitSystemPowerState;
                return false;
            }
            set
            {
                if (core != null)
                    core.shouldTransmitSystemPowerState = value;
            }
        }
        #endregion

        #region Token send and receive
        /// <summary>
        /// The delegate method declaration for forwarding incoming tokens to the application.
        /// </summary>
        /// <param name="token">The received token.</param>
        public delegate void ReceiveTokenDelegate(Token token);

        /// <summary>
        /// The delegate for forwarding incoming tokens to the application.
        /// </summary>
        public event ReceiveTokenDelegate receiveToken;

        /// <summary>
        /// The ECCONet core's delegate that forwards incoming tokens to the bus analyzer and application.
        /// </summary>
        /// <param name="token">The token to receive.</param>
        void ReceiveToken(Token token)
        {
            //  send token to bus analyzer
            if (busAnalyzer != null)
                busAnalyzer.ReceiveToken(token);

            //  send token to firmware update
            if (firmwareProgrammer != null)
                firmwareProgrammer.ReceiveToken(token);

            //  send token to application
            receiveToken?.Invoke(token);
        }

        /// <summary>
        /// Sends a token to the CAN bus.
        /// </summary>
        /// <param name="token">A token to send.</param>
        /// <returns>Returns zero on success, else -1.</returns>
        public int SendToken(Token token)
        {
            return core.SendTokenToCanBus(token);
        }
        #endregion

        #region List of file on a server drive volume
        /// <summary>
        /// Get the list of files on a given server drive volume.
        /// </summary>
        /// <param name="serverAddress">The server network address.</param>
        /// <param name="serverAccessCode">The server access code.</param>
        /// <param name="volumeIndex">The drive volume index.</param>
        /// <param name="callback">A method to call back when the list is complete.  Can be null.</param>
        /// <returns>Returns 0 on success, -1 if error and -2 if ftp client is busy.</returns>
        public int GetFileList(byte serverAddress, UInt32 serverAccessCode, UInt16 volumeIndex,
            ECCONetApi.FileScanCompleteCallback callback)
        {
            var fileScanner = new FileScanner(core);
            return fileScanner.GetFileList(serverAddress, serverAccessCode, volumeIndex, callback);
        }
        #endregion

        #region FTP Methods

        /// <summary>
        /// FTP Server response timeout in milliseconds 
        /// </summary>
        public int FTPServerResponseTimeout
        {
            get
            {
                if (core != null)
                    return core.ftpServerResponseTimeout;
                return 0;
            }
            set
            {
                if (core != null)
                    core.ftpServerResponseTimeout = value;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the ftp client is busy.
        /// </summary>
        /// <returns></returns>
        public bool IsFtpClientBusy(byte serverAddress)
        {
            return core.FTP_ServerIsBusy(serverAddress);
        }

        /// <summary>
        /// Starts an indexed file info request from the given ECCONet device ftp server.
        /// </summary>
        /// <param name="serverAddress">The server network address.</param>
        /// <param name="volumeIndex">The drive volume index.</param>
        /// <param name="fileIndex">The file index.</param>
        /// <param name="callback">A method to call back when the transaction is complete.  Can be null.</param>
        /// <returns>Returns 0 on success, -1 if ftp client is busy or -2 if server not found.</returns>
        public int GetIndexedFileInfo(byte serverAddress, UInt16 volumeIndex,
            UInt32 fileIndex, ECCONetApi.FtpTransferCompleteDelegate callback)
        {
            var device = core.deviceMonitor.GetDeviceByAddress(serverAddress);
            if (null == device)
                return -2;
            var request = new FtpClient.FtpTransactionRequest(FtpClient.FtpTransactionType.GetIndexedFileInfo,
                serverAddress, device.serverAccessCode, volumeIndex, fileIndex, core.ftpServerResponseTimeout, callback);
            return core.FTP_TransactionRequest(request);
        }

        /// <summary>
        /// Starts a file info request from the given ECCONet device ftp server.
        /// </summary>
        /// <param name="serverAddress">The server network address.</param>
        /// <param name="filename">The file name.</param>
        /// <param name="callback">A method to call back when the transaction is complete.  Can be null.</param>
        /// <returns>Returns 0 on success, -1 if ftp client is busy or -2 if server not found.</returns>
        public int GetFileInfo(byte serverAddress, string filename, ECCONetApi.FtpTransferCompleteDelegate callback)
        {
            var device = core.deviceMonitor.GetDeviceByAddress(serverAddress);
            if (null == device)
                return -2;
            var request = new FtpClient.FtpTransactionRequest(FtpClient.FtpTransactionType.GetFileInfo,
                serverAddress, device.serverAccessCode, filename, core.ftpServerResponseTimeout, callback);
            return core.FTP_TransactionRequest(request);
        }

        /// <summary>
        /// Starts a file read from the given ECCONet device ftp server.
        /// </summary>
        /// <param name="serverAddress">The server network address.</param>
        /// <param name="filename">The file name.</param>
        /// <param name="callback">A method to call back when the transaction is complete.  Can be null.</param>
        /// <returns>Returns 0 on success, -1 if ftp client is busy or -2 if server not found.</returns>
        public int ReadFile(byte serverAddress, string filename, ECCONetApi.FtpTransferCompleteDelegate callback)
        {
            _logger.Trace($"Calling ECCONetApi.ReadFile with server address '{serverAddress}' and file '{filename}'...");

            var device = core.deviceMonitor.GetDeviceByAddress(serverAddress);
            if (null == device)
                return -2;
            var request = new FtpClient.FtpTransactionRequest(FtpClient.FtpTransactionType.ReadFile,
                serverAddress, device.serverAccessCode, filename, core.ftpServerResponseTimeout, callback);
            return core.FTP_TransactionRequest(request);
        }

        /// <summary>
        /// Starts a file write to the given ECCONet device ftp server.
        /// </summary>
        /// <param name="serverAddress">The server network address.</param>
        /// <param name="filename">The file name.</param>
        /// <param name="fileDate">The file date.</param>
        /// <param name="fileData">The file data.</param>
        /// <param name="callback">A method to call back when the transaction is complete.  Can be null.</param>
        /// <returns>Returns 0 on success, -1 if ftp client is busy or -2 if server not found.</returns>
        public int WriteFile(byte serverAddress, string filename, DateTime fileDate, byte[] fileData,
            ECCONetApi.FtpTransferCompleteDelegate callback)
        {
            _logger.Trace($"Calling ECCONetApi.WriteFile with server address '{serverAddress}' and file '{filename}' ({fileData.Length} bytes)...");

            var device = core.deviceMonitor.GetDeviceByAddress(serverAddress);
            if (null == device)
                return -2;
            var request = new FtpClient.FtpTransactionRequest(FtpClient.FtpTransactionType.WriteFile,
                serverAddress, device.serverAccessCode, filename,  fileDate,  fileData, core.ftpServerResponseTimeout, callback);
            return core.FTP_TransactionRequest(request);
        }

        /// <summary>
        /// Erases a file on the given ECCONet device ftp server.
        /// </summary>
        /// <param name="serverAddress">The server network address.</param>
        /// <param name="filename">The file name.</param>
        /// <param name="callback">A method to call back when the erase is complete.  Can be null.</param>
        /// <returns>Returns 0 on success, -1 if ftp client is busy or -2 if server not found.</returns>
        public int DeleteFile(byte serverAddress, string filename, ECCONetApi.FtpTransferCompleteDelegate callback)
        {
            _logger.Trace($"Calling ECCONetApi.DeleteFile with server address '{serverAddress}' and file '{filename}'...");

            var device = core.deviceMonitor.GetDeviceByAddress(serverAddress);
            if (null == device)
                return -2;
            var request = new FtpClient.FtpTransactionRequest(FtpClient.FtpTransactionType.DeleteFile,
                serverAddress, device.serverAccessCode, filename, core.ftpServerResponseTimeout, callback);
            return core.FTP_TransactionRequest(request);
        }
        #endregion

        #region Code 3 USB-CAN and PCAN-USB connect and disconnect
        /// <summary>
        /// Connects the selected USB hardware interfaces.  This method is not required unless Disconnect has been called,
        /// because the API starts both interfaces in autoconnect mode.
        /// Note that calling this method does not select the device to be used.
        /// <param name="deviceEnum">The USB hardware interface enumeration to be connected.</param>
        /// <param name="deviceName">For PCAN-USB, a device name, or null to select the first connected device.  Null for USB-CAN.</param>
        /// <param name="shouldAutoConnect">If true, the device will always try to stay connected.</param>
        public void Connect(ECCONetApi.CANBusInterfaceDevice deviceEnum, string deviceName, bool shouldAutoConnect)
        {
            /*
            if (ECCONetApi.CANBusInterfaceDevice.PcanUsbDevice == deviceEnum)
            {
                if (null != _pcanUsbApi)
                {
                    _pcanUsbApi.shouldAutoConnect = shouldAutoConnect;
                    if (null != deviceName)
                        _pcanUsbApi.Connect(deviceName);
                    else if (!shouldAutoConnect)
                        _pcanUsbApi.Connect();
                }
            }
            else*/
            {
                if (null != _usbCanApi)
                {
                    _usbCanApi.shouldAutoConnect = shouldAutoConnect;
                    if (!shouldAutoConnect)
                        _usbCanApi.Connect();
                }
            }
        }

        /// <summary>
        /// Closes connections to USB hardware interfaces,
        /// thus ending threads and releasing resources.
        /// </summary>
        public void Disconnect()
        {
            //  close the connections
            _usbCanApi.Disconnect();
            //_pcanUsbApi.Disconnect();
        }
        #endregion

        #region Code 3 USB-CAN and PCAN-USB connection status
        /// <summary>
        /// A connection status changed delegate method.
        /// </summary>
        /// <param name="pcanUsbIsConnected">Indicates whether the PCAN-USB device is connected.</param>
        /// <param name="usbCanIsConnected">Indicates whether the Code 3 USB-CAN devices is connected.</param>
        public delegate void ConnectionStatusChangedDelegate(bool usbCanIsConnected);

        /// <summary>
        /// The connection status changed delegate.
        /// </summary>
        public ConnectionStatusChangedDelegate connectionStatusChangedDelegate { get; set; }

        /// <summary>
        /// A callback for when the PCAN-USB connection status changed.
        /// </summary>
        /// <param name="status">The current connection status.</param>
        void PcanUsbConnectionStatusChanged(bool status)
        {
            connectionStatusChangedDelegate?.Invoke( isUsbCanConnected);
        }

        /// <summary>
        /// A callback for when the USB-CAN connection status changed.
        /// </summary>
        /// <param name="status">The current connection status.</param>
        void UsbCanConnectionStatusChanged(bool status)
        {
            connectionStatusChangedDelegate?.Invoke(isUsbCanConnected);
        }

        /// <summary>
        /// The PCAN-USB connection status.
        /// </summary>
        //public bool isPcanUsbConnected { get { return (null != _pcanUsbApi) ? _pcanUsbApi.isConnectedAndReady : false; } }

        /// <summary>
        /// The USB-CAN connection status.
        /// </summary>
        public bool isUsbCanConnected { get { return (null != _usbCanApi) ? _usbCanApi.isConnectedAndReady : false; } }
        #endregion

        #region Code 3 USB-CAN and PCAN-USB connection selection
        /// <summary>
        /// CAN bus interface device enumeration.
        /// </summary>
        public enum CANBusInterfaceDevice
        {
            PcanUsbDevice,
            C3UsbCanDevice
        }

        /// <summary>
        /// The selected CAN bus interface device.
        /// </summary>
        public CANBusInterfaceDevice selectedCanBusInterfaceDevice
        {
            get { return _selectedCanBusInterfaceDevice; }
            set { _selectedCanBusInterfaceDevice = value; SetCanFrameReceiveSource(value); }
        }
        CANBusInterfaceDevice _selectedCanBusInterfaceDevice = CANBusInterfaceDevice.C3UsbCanDevice;

        /// <summary>
        /// Sets the CAN frame receive source based on the selected CAN bus interface device. 
        /// </summary>
        /// <param name="selectedDevice">The selected device.</param>
        void SetCanFrameReceiveSource(ECCONetApi.CANBusInterfaceDevice selectedDevice)
        {  /*
            if (ECCONetApi.CANBusInterfaceDevice.PcanUsbDevice == selectedDevice)
            {
            
                if (null != _pcanUsbApi)
                    _pcanUsbApi.canFrameReceivedDelegate = ReceivedCanFrame;
                if (null != _usbCanApi)
                    _usbCanApi.canFrameReceivedDelegate = null;
            }
            else*/
            {
               // if (null != _pcanUsbApi)
                  //  _pcanUsbApi.canFrameReceivedDelegate = null;
                if (null != _usbCanApi)
                    _usbCanApi.canFrameReceivedDelegate = ReceivedCanFrame;
            }
        }
        #endregion

        #region Code 3 USB-CAN and PCAN-USB send and receive CAN frames

        /// <summary>
        /// A delegate method declaration for forwarding incoming CAN frames to the application.
        /// </summary>
        /// <param name="id">The CAN frame ID.</param>
        /// <param name="data">The CAN frame data.</param>
        public delegate void ReceiveCanFrameDelegate(UInt32 id, byte[] data);

        /// <summary>
        /// The delegate for forwarding incoming tokens to the application.
        /// </summary>
        public event ReceiveCanFrameDelegate receiveCanFrame;

        /// <summary>
        /// The selected USB device's delegate that forwards incoming CAN frames to the ECCONet core and application.
        /// </summary>
        /// <param name="id">The CAN frame ID.</param>
        /// <param name="data">The CAN frame data.</param>
        void ReceivedCanFrame(UInt32 id, byte[] data)
        {
            try
            {
                //  print incoming CAN frame to console (if debug turned on)
                printCanFrameToConsole(id, data, true);

                //  forward CAN frame to receiver
                if (core != null)
                    core.ReceiveCanFrame(id, data);

                //  forward CAN frame to application
                receiveCanFrame?.Invoke(id, data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// The ECCONet core's delegate that forwards outgoing CAN frames to the selected USB device.
        /// </summary>
        /// <param name="id">The CAN frame ID.</param>
        /// <param name="data">The CAN frame data.</param>
        public int SendCanFrame(UInt32 id, byte[] data)
        {
            int status = 0;

            //  print outgoing CAN frame to console (if debug turned on)
            printCanFrameToConsole(id, data, false);

            //  forward frame to the CAN bus
            //if (ECCONetApi.CANBusInterfaceDevice.PcanUsbDevice == _selectedCanBusInterfaceDevice)
                //status = _pcanUsbApi.SendCanFrame(id, data);
           // else
                _usbCanApi.SendCanFrame(id, data);  // no status for async send
            return status;
        }

        /// <summary>
        /// Print CAN frames to console.
        /// </summary>
        /// <param name="id">The CAN frame ID.</param>
        /// <param name="data">The CAN frame data.</param>
        void printCanFrameToConsole(UInt32 id, Byte[] data, bool incoming)
        {
            //if (incoming)
            //    return;
            try
            {
                if ((ECCONetCore.ConsoleDebugMessageLevels.Frames <= core.consoleDebugMessageLevel)
                    && (((id & 0x1f000000) == 0x1C000000) || ((id & 0x1f000000) == 0x1D000000) || ((id & 0x1f000000) == 0x1E000000)))
                {
                    string str = incoming ? "IN  <<--" : "OUT  -->>";
                    str +=
                        DateTime.Now.Second.ToString() + "." + DateTime.Now.Millisecond.ToString() + "  " +
                        id.ToString("X16") + "  " +
                        data.Length.ToString();
                    for (int i = 0; i < data.Length; ++i)
                        str += ("  " + data[i].ToString("X2"));
                    Console.WriteLine(str);

                    /* Use this to log can frames
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\can_log.txt", true))
                    {
                        file.WriteLine(str);
                        Console.WriteLine(str);
                    }
                    */
                }
            }
            catch { }
        }

        #endregion

        #region Firmware update
        /// <summary>
        /// Erases a device's app portion of flash.
        /// </summary>
        /// <param name="device">The device whose app will be erased.</param>
        public void EraseDeviceApplication(ECCONetApi.ECCONetDevice device)
        {
            firmwareProgrammer.EraseDeviceApplication(device);
        }

        /// <summary>
        /// Erases a device's flash.
        /// </summary>
        /// <param name="device">The device whose flash will be erased.</param>
        public void EraseDeviceFlash(ECCONetApi.ECCONetDevice device)
        {
            firmwareProgrammer.EraseDeviceFlash(device);
        }

        /// <summary>
        /// Programs a device via the ECCONet protocol.
        /// </summary>
        /// <param name="device">The ECCONet device to program.</param>
        /// <param name="image">The flash image.</param>
        /// <param name="progressCallback">A method to call with progress updates.</param>
        /// <param name="completeCallback">A method to call with the update result.</param>
        /// <returns>Returns result code.</returns>
        public FirmwareUpdateStatusCodes ProgramDeviceWithImage(ECCONetDevice device, byte[] image,
            FirmwareUpdateProgressDelegate progressCallback, FirmwareUpdateCompleteDelegate completeCallback)
        {
            return firmwareProgrammer.ProgramDeviceWithImage(device, image, progressCallback, completeCallback);
        }

        #endregion
    }
}


#if UNUSED_CODE


#region Event and status message enable
        /// <summary>
        /// Determines whether events are received.  Default is true.
        /// ECCONet firmware devices always receive events.
        /// </summary>
        public bool ReceiveEvents
        {
            get
            {
                if ((core != null) && (core.receiver != null))
                    return core.receiver.receiveEvents;
                return false;
            }
            set
            {
                if ((core != null) && (core.receiver != null))
                    core.receiver.receiveEvents = value;
            }
        }

        /// <summary>
        /// Determines whether current status messages are received.  Default is true.
        /// ECCONet firmware devices always receive current status messages.
        /// </summary>
        public bool ReceiveCurrentStatusMessages
        {
            get
            {
                if ((core != null) && (core.receiver != null))
                    return core.receiver.receiveCurrentStatusMessages;
                return false;
            }
            set
            {
                if ((core != null) && (core.receiver != null))
                    core.receiver.receiveCurrentStatusMessages = value;
            }
        }

        /// <summary>
        /// Determines whether expired status messages are received.  Default is false.
        /// ECCONet firmware devices always ignore expired status messages.
        /// </summary>
        public bool ReceiveExpiredStatusMessages
        {
            get
            {
                if ((core != null) && (core.receiver != null))
                    return core.receiver.receiveExpiredStatusMessages;
                return false;
            }
            set
            {
                if ((core != null) && (core.receiver != null))
                    core.receiver.receiveExpiredStatusMessages = value;
            }
        }
#endregion

#endif

