/**
  ******************************************************************************
  * @file    	UsbCanApi.cs
  * @copyright  © 2017-2019 ECCO Group.  All rights reserved.
  * @author  	M. Latham, Liquid Logic, LLC
  * @version 	2.0.0
  * @date    	Nov 2019
  * @brief   	An API for communicating with the Code 3 USB-CAN device.
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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using JanAxelsonWinUSB;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace ECCONet.UsbCan
{

    public class ECCONet_UsbCanApi
    {
        /// <summary>
        /// A connection status changed callback delegate.
        /// </summary>
        /// <param name="isConnected">Indicates whether the Code 3 USB-CAN device is connected.</param>
        public delegate void ConnectionStatusChanged(bool isConnected);

        /// <summary>
        /// The connection status changed callback.
        /// </summary>
        public ConnectionStatusChanged connectionStatusChangedDelegate { get; set; }


        /// <summary>
        /// The USB-CAN API CAN frame received callback delegate.
        /// </summary>
        /// <param name="id">The CAN frame id.</param>
        /// <param name="data">The CAN frame data.</param>
        public delegate void CanFrameReceived(UInt32 id, byte[] data);

        /// <summary>
        /// The CAN frame received callback.
        /// </summary>
        public CanFrameReceived canFrameReceivedDelegate { get; set; }


        /// <summary>
        /// Sets the CAN transmit delay in mS.  The default is 1.0 mS.  Use higher numbers for slower devices.
        /// </summary>
        public double transmitDelaymS { get; set; }

        /// <summary>
        /// Indicates whether the API should automatically connect to
        /// the first USB-CAN device found.
        /// </summary>
        public bool shouldAutoConnect { get; set; }

        /// <summary>
        /// Indicates whether the USB-CAN device has been found and is ready for use.
        /// </summary>
        public bool isConnectedAndReady { get => _isConnectedAndReady; }
        private bool _isConnectedAndReady;

        //  USB-CAN device info
        const String DeviceInterfaceGuid = "{F08AF5A1-9FB4-4C01-BA4A-6D80546B86A7}";
        const String VendorID = "2D03";
        const String ProductID = "0001";

        //  the device managment class used for connecting (and reconnecting) to the USB-CAN device
        readonly DeviceManagement deviceManagement = new DeviceManagement();

        //  the USB-CAN device info provided by device management during initialization
        WinUsbCommunications.DeviceInfo usbCanDeviceInfo = new WinUsbCommunications.DeviceInfo();

        //  an instance of the class designed by Jan Axelson as a wrapper around the WinUsb communications driver
        WinUsbCommunications winUsbCommunications = new WinUsbCommunications();

        //  a handle to the WinUsb communications driver
        WinUsbCommunications.SafeWinUsbHandle winUsbHandle = new WinUsbCommunications.SafeWinUsbHandle();

        //  indicates whether the USB-CAN device has been detected in a system resource search
        bool usbCanPresent;

        //  the handle to the USB-CAN device
        SafeFileHandle usbCanHandle;

        //  a device remover watcher for knowing if the USB-CAN device is removed
        ManagementEventWatcher deviceRemovedWatcher;

        //	a process manager timer compatible with .NET Core
        Timer processManagerTimer;

        //  the processor manager critical section lock
        object processManagerLock;

        //  the processor manager busy
        byte processManagerBusy;

        //  a transmitter lock
        object transmitterLock;

        //  a transmitter queue
        ConcurrentQueue<byte[]> dataToSendQueue = new ConcurrentQueue<byte[]>();

        //  a flag to reconnect
        bool shouldReconnect;

        //  a thread to read CAN frames for the blocking WinUSB read pipe
        Thread canReadThread;

        //  a flag to abort the read thread
        //  does not need to be made thread-safe, as will only ever be set
        bool shouldAbortReadThread;

        //  a thread to write CAN frames
        Thread canWriteThread;

        //  a flag to abort the write thread
        //  does not need to be made thread-safe, as will only ever be set
        bool shouldAbortWriteThread;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="shouldAutoConnect">A value indicating whether this API should
        /// automatically maintain a connecton with the device.</param>
        public ECCONet_UsbCanApi(bool shouldAutoConnect)
        {
            //  save the auto-connect mode
            this.shouldAutoConnect = shouldAutoConnect;

            //  set the transmit delay to default
            this.transmitDelaymS = 1;

            //  add the device removed handler
            AddDeviceRemovedHandler();

            //  create the process manager lock
            processManagerLock = new object();

            //	create the process manager timer
            processManagerTimer = new Timer(new TimerCallback(processManagerTimerCallback), null, 0, 1000);

            //  create the transmitter lock
            transmitterLock = new object();
        }

        /// <summary>
        /// Detructor to ensure that if this object is deleted, the read and write threads end.
        /// </summary>
        ~ECCONet_UsbCanApi()
        {
            shouldAbortReadThread = true;
            shouldAbortWriteThread = true;
        }

        /// <summary>
        /// The process manager timer callback.
        /// </summary>
        /// <param name="state">The user object state.</param>
        void processManagerTimerCallback(object state)
        {
            //  if already executing a timer callback,
            //  then skip this one
            if (0 != processManagerBusy)
                return;

            //  critical code section
            lock (processManagerLock)
            {
                //  set busy flag
                processManagerBusy = 1;

                //  if USB is not ready and auto-connect enabled
                if (!_isConnectedAndReady && shouldAutoConnect)
                {
                    //  request reconnect
                    shouldReconnect = true;
                }

                //  if reconnect requested
                if (shouldReconnect)
                {
                    //  clear request and connected status
                    shouldReconnect = false;
                    _isConnectedAndReady = false;
                    Thread.Sleep(100);

                    try
                    {
                        //  debug
                        Debug.WriteLine("USB-CAN: Trying to connect.");

                        //  disconnect
                        if ((null != usbCanHandle) && (null != winUsbHandle))
                            winUsbCommunications.CloseDeviceHandle(usbCanHandle, winUsbHandle);

                        //  wait
                        Thread.Sleep(100);

                        //  new USB-CAN device info provided by device management during initialization
                        usbCanDeviceInfo = new WinUsbCommunications.DeviceInfo();

                        //  new instance of the class designed by Jan Axelson as a wrapper around the WinUsb communications driver
                        winUsbCommunications = new WinUsbCommunications();

                        //  new handle to the WinUsb communications driver
                        winUsbHandle = new WinUsbCommunications.SafeWinUsbHandle();

                        //  connect
                        Connect();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("USB-CAN: Connect failed: " + ex.Message);
                    }
                }

                //  if CAN read thread is null or aborted, then start a new thread
                if ((null == canReadThread) || (false == canReadThread.IsAlive))
                {
                    try
                    {
                        shouldAbortReadThread = false;
                        canReadThread = new Thread(ReadCanFrames);
                        canReadThread.IsBackground = true;
                        canReadThread.Start();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("USB-CAN create read thread failed failed: " + ex.Message);
                    }
                }

                //  if CAN write thread is null or aborted, then start a new thread
                if ((null == canWriteThread) || (false == canWriteThread.IsAlive))
                {
                    try
                    {
                        shouldAbortWriteThread = false;
                        canWriteThread = new Thread(WriteCanFrames);
                        canWriteThread.IsBackground = true;
                        canWriteThread.Start();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("USB-CAN create write thread failed failed: " + ex.Message);
                    }
                }

                //  clear busy flag
                processManagerBusy = 0;
            }
        }

        public void StartProcessManager()
        {
            processManagerTimer.Change(0, 1000);
        }

        public void StopProcessManager()
        {
            processManagerTimer.Change(0, 0);
            canReadThread = null;
            canWriteThread = null;
        }

        /// <summary>
        /// Requests USB-CAN reconnect.
        /// </summary>
        public void Reconnect()
        {
            shouldReconnect = true;
        }

        ///  <summary>
        ///  Initializes the USB-CAN device.
        ///  </summary>
        /// <returns>Returns 0 on success, else -1.</returns>
        public int Connect()
        {
            //  capture is connected and ready status
            bool status = _isConnectedAndReady;

            //  if the USB-CAN is not ready to use
            if (!_isConnectedAndReady)
            {
                try
                {
                    string devicePathName = "";
                    const uint pipeTimeout = 1000;

                    //  try to find the device via Guid
                    Guid winUsbDemoGuid = new Guid(DeviceInterfaceGuid);
                    bool deviceFound = deviceManagement.FindDeviceFromGuid(winUsbDemoGuid, ref devicePathName);

                    //  if the device was found
                    if (deviceFound)
                    {
                        //  try to get handle
                        usbCanHandle = winUsbCommunications.GetDeviceHandle(devicePathName);

                        //  if have handle
                        if (!usbCanHandle.IsInvalid)
                        {
                            //  indicate connected and ready, and configure the device based on the USB descriptor
                            _isConnectedAndReady = true;
                            winUsbCommunications.InitializeDevice(usbCanHandle, ref winUsbHandle, ref usbCanDeviceInfo, pipeTimeout);
                        }
                        else  //  no handle
                        {
                            //  indicate not connected and close the handle
                            _isConnectedAndReady = false;
                            winUsbCommunications.CloseDeviceHandle(usbCanHandle, winUsbHandle);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            //  if status changed
            if (status != _isConnectedAndReady)
                connectionStatusChangedDelegate?.Invoke(_isConnectedAndReady);
            return (_isConnectedAndReady ? 0 : -1);
        }

        /// <summary>
        /// Disconnects from any connected USB-CAN hardware device.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if ((null != usbCanHandle) && (null != winUsbHandle))
                    winUsbCommunications.CloseDeviceHandle(usbCanHandle, winUsbHandle);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            shouldAutoConnect = false;
            _isConnectedAndReady = false;
            shouldAbortReadThread = true;
            shouldAbortWriteThread = true;
            connectionStatusChangedDelegate?.Invoke(_isConnectedAndReady);
        }

        /// <summary>
        /// Use WMI to find the USB-CAN device by Vendor ID and Product ID.
        /// </summary>
        /// <returns>Returns a value indicating whether the USB-CAN device is (still) present.</returns>
        private Boolean IsUsbCanPresent()
        {
            bool usbCanPresent = false;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    const string deviceIdString = @"USB\VID_" + VendorID + "&PID_" + ProductID;
                    if (queryObj["PNPDeviceID"].ToString().Contains(deviceIdString))
                        usbCanPresent = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return usbCanPresent;
        }

        ///  <summary>
        ///  Create a device removed watcher.
        ///  </summary>
        private void AddDeviceRemovedHandler()
        {
            const Int32 pollingIntervalSeconds = 3;
            ManagementScope scope = new ManagementScope("root\\CIMV2");
            scope.Options.EnablePrivileges = true;

            //  try to add watcher and device removed handler
            try
            {
                var q = new WqlEventQuery();
                q.EventClassName = "__InstanceDeletionEvent";
                q.WithinInterval = new TimeSpan(0, 0, pollingIntervalSeconds);
                q.Condition = @"TargetInstance ISA 'Win32_USBControllerdevice'";
                deviceRemovedWatcher = new ManagementEventWatcher(scope, q);
                deviceRemovedWatcher.EventArrived += _deviceRemovedWatcher_EventArrived;
                deviceRemovedWatcher.Start();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                if (deviceRemovedWatcher != null)
                    deviceRemovedWatcher.Stop();
            }
        }

        /// <summary>
        /// Called on removal of any device.
        /// Calls the IsUsbCanPresent() method to know if the USB-CAN device was removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _deviceRemovedWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            usbCanPresent = IsUsbCanPresent();
            if (!usbCanPresent)
            {
                _isConnectedAndReady = false;
                connectionStatusChangedDelegate?.Invoke(_isConnectedAndReady);
            }
        }

        /// <summary>
        /// Sends a CAN frame to the bus.
        /// </summary>
        /// <param name="id">The frame id.</param>
        /// <param name="data">The frame data.</param>
        /// <returns>Returns 0 on success, else -1.</returns>
        public int SendCanFrame(UInt32 id, byte[] data)
        {
            int status = 0;

            //  critical code section
            lock (transmitterLock)
            {
                try
                {
                    //  format data and place in queue
                    byte[] dataToSend = (null == data) ? new byte[5] : new byte[5 + data.Length];
                    dataToSend[0] = (byte)((UInt32)id >> 24);
                    dataToSend[1] = (byte)((UInt32)id >> 16);
                    dataToSend[2] = (byte)((UInt32)id >> 8);
                    dataToSend[3] = (byte)id;
                    if (null == data)
                        dataToSend[4] = 0;
                    else
                    {
                        dataToSend[4] = (byte)data.Length;
                        for (int i = 0; i < data.Length; ++i)
                            dataToSend[5 + i] = data[i];
                    }
                    dataToSendQueue.Enqueue(dataToSend);

                    //  if queue is getting full, then block for user-specified transmit rate
                    if (dataToSendQueue.Count >= 100)
                    {
                        var stopWatch = new Stopwatch();
                        stopWatch.Start();
                        while (stopWatch.Elapsed.TotalMilliseconds < (transmitDelaymS + 0.2)) { }
                        stopWatch.Stop();
                    }
                }
                catch (Exception ex)
                {
                    status = -1;
                    Debug.WriteLine("USB_CAN error queueing data: " + ex.Message);
                }
            }

            //  return success
            return status;
        }

        /// <summary>
        /// Thread method to read CAN frames from queue and send to USB bus.
        /// </summary>
        private void WriteCanFrames()
        {
            byte[] dataToSend;
            UInt32 numBytesWritten = 0;
            bool success = false;
            int tries = 3;

            try
            {
                //  while thread alive
                while (!shouldAbortWriteThread)
                {
                    //  if have USB connection
                    if (_isConnectedAndReady)
                    {
                        //  while data to send
                        while (dataToSendQueue.TryDequeue(out dataToSend))
                        {
                            //  send CAN frame
                            winUsbCommunications.SendDataViaBulkTransfer(winUsbHandle, usbCanDeviceInfo,
                                (UInt32)dataToSend.Length, dataToSend, ref numBytesWritten, ref success);

                            //  if success, reset tries
                            if (success)
                            {
                                tries = 3;
                            }

                            //  else after three fails, request reconnect
                            else if (--tries <= 0)
                            {
                                tries = 3;
                                shouldReconnect = true;
                                Debug.WriteLine("USB-CAN transmit requsted reconnect.");
                            }

                            //  wait for user-specified transmit rate
                            var stopWatch = new Stopwatch();
                            stopWatch.Start();
                            while (stopWatch.Elapsed.TotalMilliseconds < (transmitDelaymS + 0.2)) { }
                            stopWatch.Stop();
                        }
                    }
                    Thread.Sleep(20);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("USB-CAN transmit thread error: " + ex.Message);
            }
        }

        /// <summary>
        /// Thread method to read CAN frames from USB bus and send to subscriber.
        /// 
        /// Note that in windows, WinUsb_ReadPipe() ignores timeout parameter and only receives one USB frame per call,
        /// so Thread.Sleep() cannot be used.
        /// </summary>
        private void ReadCanFrames()
        {
            byte[] usbData = new byte[256];
            UInt32 numBytesRead = 0;
            bool success = false;

            try
            {
                //  while thread alive
                while (!shouldAbortReadThread)
                {
                    //  if have USB connection
                    if (_isConnectedAndReady)
                    {
                        //  try to recieve frame
                        winUsbCommunications.ReceiveDataViaBulkTransfer(
                           winUsbHandle, usbCanDeviceInfo, 256, ref usbData, ref numBytesRead, ref success);

                        //  if success, format message and send to delegate
                        if (success)
                        {
                            int index = 0;
                            while (index < numBytesRead)
                            {
                                UInt32 id = (UInt32)((usbData[index] << 24) | (usbData[index + 1] << 16)
                                    | (usbData[index + 2] << 8) | usbData[index + 3]);
                                byte[] data = new byte[usbData[index + 4]];
                                for (int i = 0; i < data.Length; ++i)
                                    data[i] = usbData[index + i + 5];
                                canFrameReceivedDelegate?.Invoke(id, data);
                                index += 13;
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(20);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("USB-CAN receive thread error: " + ex.Message);
            }
        }

    }
     public class ECCONet_UsbDotNetCanApi
    {
        /// <summary>
        /// A connection status changed callback delegate.
        /// </summary>
        /// <param name="isConnected">Indicates whether the Code 3 USB-CAN device is connected.</param>
        public delegate void ConnectionStatusChanged(bool isConnected);

        /// <summary>
        /// The connection status changed callback.
        /// </summary>
        public ConnectionStatusChanged connectionStatusChangedDelegate { get; set; }


        /// <summary>
        /// The USB-CAN API CAN frame received callback delegate.
        /// </summary>
        /// <param name="id">The CAN frame id.</param>
        /// <param name="data">The CAN frame data.</param>
        public delegate void CanFrameReceived(UInt32 id, byte[] data);

        /// <summary>
        /// The CAN frame received callback.
        /// </summary>
        public CanFrameReceived canFrameReceivedDelegate { get; set; }


        /// <summary>
        /// Sets the CAN transmit delay in mS.  The default is 1.0 mS.  Use higher numbers for slower devices.
        /// </summary>
        public double transmitDelaymS { get; set; }

        /// <summary>
        /// Indicates whether the API should automatically connect to
        /// the first USB-CAN device found.
        /// </summary>
        public bool shouldAutoConnect { get; set; }

        /// <summary>
        /// Indicates whether the USB-CAN device has been found and is ready for use.
        /// </summary>
        public bool isConnectedAndReady { get => _isConnectedAndReady; }
        static private bool _isConnectedAndReady = false;
        // libdontnet stuff
        static private UsbDeviceFinder code3UsbCanDeviceFinder;
        static private UsbDevice code3UsbCanDevice;
        static private Timer usbDevicePollingTimer;

        //  USB-CAN device info
        const String DeviceInterfaceGuid = "{F08AF5A1-9FB4-4C01-BA4A-6D80546B86A7}";
        const int VendorID = 0x2D03;
        const int ProductID = 0x0001;          
    
        Timer processManagerTimer;
        const  uint pollingIntervalMilliSeconds = 1000;
        //  the processor manager critical section lock
        object processManagerLock;

        //  the processor manager busy
        byte processManagerBusy;

        //  a transmitter lock
        object transmitterLock;

        //  a transmitter queue
        ConcurrentQueue<byte[]> dataToSendQueue = new ConcurrentQueue<byte[]>();

        //  a flag to reconnect
        bool shouldReconnect;

        //  a thread to read CAN frames for the blocking WinUSB read pipe
        Thread canReadThread;
        //  a flag to abort the read thread
        //  does not need to be made thread-safe, as will only ever be set
        bool shouldAbortReadThread;
        //  a thread to write CAN frames
        Thread canWriteThread;
        //  a flag to abort the write thread
        //  does not need to be made thread-safe, as will only ever be set
        bool shouldAbortWriteThread;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="shouldAutoConnect">A value indicating whether this API should
        /// automatically maintain a connecton with the device.</param>
        public ECCONet_UsbDotNetCanApi(bool shouldAutoConnect)
        {
            //  save the auto-connect mode
            // setup the USB device Finder for the Code 3 USB-CAN device
            code3UsbCanDeviceFinder = new UsbDeviceFinder((VendorID), (ProductID));
            this.shouldAutoConnect = shouldAutoConnect;

            //  set the transmit delay to default
            this.transmitDelaymS = 1;           

            //  create the process manager lock
            processManagerLock = new object();

            //	create the process manager timer
            processManagerTimer = new Timer(processManagerTimerCallback, null, 0, pollingIntervalMilliSeconds);
            //  create the transmitter lock
            transmitterLock = new object();
        }

        /// <summary>
        /// Detructor to ensure that if this object is deleted, the read and write threads end.
        /// </summary>
        ~ECCONet_UsbDotNetCanApi()
        {
            shouldAbortReadThread = true;
            shouldAbortWriteThread = true;
        }

        /// <summary>
        /// The process manager timer callback.
        /// </summary>
        /// <param name="state">The user object state.</param>
        void processManagerTimerCallback(object state)
        {
            //  if already executing a timer callback,
            //  then skip this one
            if (0 != processManagerBusy)
                return;

            //  critical code section
            lock (processManagerLock)
            {
                //  set busy flag
                processManagerBusy = 1;

                //  if USB is not ready and auto-connect enabled
                bool deviceFound = false;
                foreach (UsbRegistry usbRegistry in UsbDevice.AllDevices)
                    {
                        if (usbRegistry.Vid == VendorID && usbRegistry.Pid == ProductID)
                            {
                                deviceFound = true;
                                break;
                            }
                            }

                        if ( !deviceFound )
                        {
                            Console.WriteLine("Code3 Device is not Found and Disconnected");
                            _isConnectedAndReady = false; 
                        }
                if (!_isConnectedAndReady && shouldAutoConnect)
                {
                    //  request reconnect
                    shouldReconnect = true;
                }

                //  if reconnect requested
                if (shouldReconnect)
                {
                    //  clear request and connected status
                    shouldReconnect = false;
                    _isConnectedAndReady = false;
                    Thread.Sleep(100);

                    try
                    {
    
                        {
                            Connect();
                        }
                        //  debug
                        Debug.WriteLine("USB-CAN: Trying to connect.");
                        //  disconnect
                        //  wait
                        Thread.Sleep(100);
                        //  connect
                        
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("USB-CAN: Connect failed: " + ex.Message);
                    }
                }

                //  if CAN read thread is null or aborted, then start a new thread
                if ((null == canReadThread) || (false == canReadThread.IsAlive))
                {
                    try
                    {
                        shouldAbortReadThread = false;
                        canReadThread = new Thread(ReadCanFrames);
                        canReadThread.IsBackground = true;
                        canReadThread.Start();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("USB-CAN create read thread failed failed: " + ex.Message);
                    }
                }

                //  if CAN write thread is null or aborted, then start a new thread
                if ((null == canWriteThread) || (false == canWriteThread.IsAlive))
                {
                    try
                    {
                        shouldAbortWriteThread = false;
                        canWriteThread = new Thread(WriteCanFrames);
                        canWriteThread.IsBackground = true;
                        canWriteThread.Start();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("USB-CAN create write thread failed failed: " + ex.Message);
                    }
                }

                //  clear busy flag
                processManagerBusy = 0;
            }
        }

        public void StartProcessManager()
        {
            processManagerTimer.Change(0, 1000);
        }

        public void StopProcessManager()
        {
            processManagerTimer.Change(0, 0);
            canReadThread = null;
            canWriteThread = null;
        }

        /// <summary>
        /// Requests USB-CAN reconnect.
        /// </summary>
        public void Reconnect()
        {
            shouldReconnect = true;
        }

        ///  <summary>
        ///  Initializes the USB-CAN device.
        ///  </summary>
        /// <returns>Returns 0 on success, else -1.</returns>
        public int Connect()
        {
            //  capture is connected and ready status
            bool status = _isConnectedAndReady;

            //  if the USB-CAN is not ready to use
            if (!_isConnectedAndReady)
            {
                try
                {
                    // added timeout to the device finder
                    //  try to find the device via Vendor and Product ID

                    code3UsbCanDevice = UsbDevice.OpenUsbDevice(code3UsbCanDeviceFinder);

                    bool deviceFound = code3UsbCanDevice != null;
                    // find the device
                    // if the device is open and ready
                    //  if the device was found
                    if (deviceFound)
                    {   
                        Console.WriteLine("Code3 Device is connected and Ready");
                        _isConnectedAndReady = true;
                    }
                    else  //  no handle
                    {
                        Console.WriteLine("Couldn't Open Code3 Device");
                        _isConnectedAndReady = false;
                        code3UsbCanDevice = null;
                    }
                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            //  if status changed
            if (status != _isConnectedAndReady)
                connectionStatusChangedDelegate?.Invoke(_isConnectedAndReady);
            return (_isConnectedAndReady ? 0 : -1);
        }

        /// <summary>
        /// Disconnects from any connected USB-CAN hardware device.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (code3UsbCanDevice != null)
                {
                    // Close the device
                    code3UsbCanDevice.Close();
                    code3UsbCanDevice = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            shouldAutoConnect = false;
            _isConnectedAndReady = false;
            shouldAbortReadThread = true;
            shouldAbortWriteThread = true;
            connectionStatusChangedDelegate?.Invoke(_isConnectedAndReady);
        }


        /// <summary>
        /// Sends a CAN frame to the bus.
        /// </summary>
        /// <param name="id">The frame id.</param>
        /// <param name="data">The frame data.</param>
        /// <returns>Returns 0 on success, else -1.</returns>
        public int SendCanFrame(UInt32 id, byte[] data)
        {
            int status = 0;

            //  critical code section
            lock (transmitterLock)
            {
                try
                {
                    //  format data and place in queue
                    byte[] dataToSend = (null == data) ? new byte[5] : new byte[5 + data.Length];
                    dataToSend[0] = (byte)((UInt32)id >> 24);
                    dataToSend[1] = (byte)((UInt32)id >> 16);
                    dataToSend[2] = (byte)((UInt32)id >> 8);
                    dataToSend[3] = (byte)id;
                    if (null == data)
                        dataToSend[4] = 0;
                    else
                    {
                        dataToSend[4] = (byte)data.Length;
                        for (int i = 0; i < data.Length; ++i)
                            dataToSend[5 + i] = data[i];
                    }
                    dataToSendQueue.Enqueue(dataToSend);

                    //  if queue is getting full, then block for user-specified transmit rate
                    if (dataToSendQueue.Count >= 100)
                    {
                        var stopWatch = new Stopwatch();
                        stopWatch.Start();
                        while (stopWatch.Elapsed.TotalMilliseconds < (transmitDelaymS + 0.2)) { }
                        stopWatch.Stop();
                    }
                }
                catch (Exception ex)
                {
                    status = -1;
                    Debug.WriteLine("USB_CAN error queueing data: " + ex.Message);
                }
            }

            //  return success
            return status;
        }

        /// <summary>
        /// Thread method to read CAN frames from queue and send to USB bus.
        /// </summary>
        private void WriteCanFrames()
        {
            byte[] dataToSend;
            UInt32 numBytesWritten = 0;
            bool success = false;
            int tries = 3;

            try
            {
                //  while thread alive
                while (!shouldAbortWriteThread)
                {
                    //  if have USB connection
                    if (_isConnectedAndReady)
                    {
                        //  while data to send
                        while (dataToSendQueue.TryDequeue(out dataToSend))
                        {
                            //  send CAN frame
                          //  winUsbCommunications.SendDataViaBulkTransfer(winUsbHandle, usbCanDeviceInfo,
                            //    (UInt32)dataToSend.Length, dataToSend, ref numBytesWritten, ref success);

                            //  if success, reset tries
                            if (success)
                            {
                                tries = 3;
                            }

                            //  else after three fails, request reconnect
                            else if (--tries <= 0)
                            {
                                tries = 3;
                                shouldReconnect = true;
                                Debug.WriteLine("USB-CAN transmit requsted reconnect.");
                            }

                            //  wait for user-specified transmit rate
                            var stopWatch = new Stopwatch();
                            stopWatch.Start();
                            while (stopWatch.Elapsed.TotalMilliseconds < (transmitDelaymS + 0.2)) { }
                            stopWatch.Stop();
                        }
                    }
                    Thread.Sleep(20);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("USB-CAN transmit thread error: " + ex.Message);
            }
        }

        /// <summary>
        /// Thread method to read CAN frames from USB bus and send to subscriber.
        /// 
        /// Note that in windows, WinUsb_ReadPipe() ignores timeout parameter and only receives one USB frame per call,
        /// so Thread.Sleep() cannot be used.
        /// </summary>
        private void ReadCanFrames()
        {
            byte[] usbData = new byte[256];
            UInt32 numBytesRead = 0;
            bool success = false;

            try
            {
                //  while thread alive
                while (!shouldAbortReadThread)
                {
                    //  if have USB connection
                    if (_isConnectedAndReady)
                    {
                        //  try to recieve frame
                       // winUsbCommunications.ReceiveDataViaBulkTransfer(
                          // winUsbHandle, usbCanDeviceInfo, 256, ref usbData, ref numBytesRead, ref success);
                      // replace with libDotnetusb
                        //UsbSetupPacket setupPacket = new UsbSetupPacket(0x80, 0x06, 0x0100, 0x0000, 0x0000);
                       // code3UsbCanDevice.ControlTransfer(ref setupPacket, usbData, 256, out numBytesRead);
                        
                        //  if success, format message and send to delegate
                        if (success)
                        {
                            int index = 0;
                            while (index < numBytesRead)
                            {
                                UInt32 id = (UInt32)((usbData[index] << 24) | (usbData[index + 1] << 16)
                                    | (usbData[index + 2] << 8) | usbData[index + 3]);
                                byte[] data = new byte[usbData[index + 4]];
                                for (int i = 0; i < data.Length; ++i)
                                    data[i] = usbData[index + i + 5];
                                canFrameReceivedDelegate?.Invoke(id, data);
                                index += 13;
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(20);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("USB-CAN receive thread error: " + ex.Message);
            }
        }

    }
}
