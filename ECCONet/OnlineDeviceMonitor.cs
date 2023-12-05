/**
  ******************************************************************************
  * @file    	OnlineDeviceMonitor2.cs
  * @copyright  ©2017-2019 ECCO Safety Group.  All rights reserved.
  * @author  	M. Latham, Liquid Logic, LLC
  * @version 	2.0.0
  * @date    	Jan 2019
  * @brief   	Maintains a list of online devices.
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
using System.Diagnostics;

namespace ECCONet
{
    public partial class ECCONetApi
    {
        #region ECCONet device
        /// <summary>
        /// An ECCONet 3.0 device product info object.
        /// </summary>
        public class ECCONetDevice
        {
            /// <summary>
            /// An ECCONet device info class.
            /// </summary>
            public class ProductInfoStatus
            {
                /// <summary>
                /// The ECCONet device info states.
                /// </summary>
                public enum Status
                {
                    Pending,
                    HaveInfo,
                    InfoFileNotAvailable,
                    HavePartialInfo,
                    NotResponsive,
                }

                /// <summary>
                /// The device product info read state.
                /// </summary>
                public Status status = Status.Pending;

                /// <summary>
                /// The number of times a product info read was attempted.
                /// </summary>
                public int getInfoTries = 0;

                /// <summary>
                /// The last time a product info read was attempted.
                /// </summary>
                public DateTime lastGetInfoTimestamp = new DateTime();

                /// <summary>
                /// Creates a deep copy of this device status.
                /// </summary>
                /// <returns>A deep copy of this device status.</returns>
                public ProductInfoStatus Copy()
                {
                    var pi = new ProductInfoStatus();
                    pi.status = status;
                    pi.getInfoTries = getInfoTries;
                    pi.lastGetInfoTimestamp = lastGetInfoTimestamp;
                    return pi;
                }
            }

            /// <summary>
            /// The product info file read status.
            /// </summary>
            public ProductInfoStatus productInfoStatus = new ProductInfoStatus();

            /// <summary>
            /// The device ECCONet address.
            /// </summary>
            public Byte address = 0;

            /// <summary>
            /// The device GUID.
            /// </summary>
            public UInt32[] guid;

            /// <summary>
            /// The device server access code, based on the hashed GUID.
            /// </summary>
            public UInt32 serverAccessCode = 0;

            /// <summary>
            /// The last time this device was hear from.
            /// If this device goes silent, it is removed from the device monitor list.
            /// </summary>
            public DateTime lastStatusTime = new DateTime();

            /// <summary>
            /// The product info file timestamp.
            /// </summary>
            public DateTime infoFileDate = new DateTime();

            /// <summary>
            /// The product name.
            /// </summary>
            public string modelName = String.Empty;

            /// <summary>
            /// The product manufacturer name.
            /// </summary>
            public string manufacturerName = String.Empty;

            /// <summary>
            /// The product hardware revision.
            /// </summary>
            public string hardwareRevision = String.Empty;

            /// <summary>
            /// The product application firmware revision.
            /// </summary>
            public string appFirmwareRevision = String.Empty;

            /// <summary>
            /// The product bootloader firmware revision.
            /// </summary>
            public string bootloaderFirmwareRevision = String.Empty;

            /// <summary>
            /// The product first indexed light head, if applicable.
            /// </summary>
            public string baseLightheadEnumeration = String.Empty;

            /// <summary>
            /// The product last indexed light head, if applicable.
            /// </summary>
            public string maxLightheadEnumeration = String.Empty;

            /// <summary>
            /// Depreciated.  Indicates that the product info file has been retrieved.
            /// The productInfoStatus.status now has the full set of states.
            /// </summary>
            public bool haveDeviceinfo = false;


            /// <summary>
            /// Creates a deep copy of this device.
            /// </summary>
            /// <returns>A deep copy of this device.</returns>
            public ECCONetDevice Copy()
            {
                ECCONetDevice device = new ECCONetDevice();
                device.productInfoStatus = productInfoStatus.Copy();
                device.address = address;
                if (null != guid)
                {
                    device.guid = new UInt32[4];
                    device.guid[0] = guid[0];
                    device.guid[1] = guid[1];
                    device.guid[2] = guid[2];
                    device.guid[3] = guid[3];
                }
                device.serverAccessCode = serverAccessCode;
                device.lastStatusTime = lastStatusTime;
                device.infoFileDate = infoFileDate;
                device.modelName = String.Copy(modelName);
                device.manufacturerName = String.Copy(manufacturerName);
                device.hardwareRevision = String.Copy(hardwareRevision);
                device.appFirmwareRevision = String.Copy(appFirmwareRevision);
                device.bootloaderFirmwareRevision = String.Copy(bootloaderFirmwareRevision);
                device.baseLightheadEnumeration = String.Copy(baseLightheadEnumeration);
                device.maxLightheadEnumeration = String.Copy(maxLightheadEnumeration);
                return device;
            }

            /// <summary>
            /// Model name and address string readonly property.
            /// </summary>
            public string ModelNameAndAddressString
            {
                get { return modelName + " / Addr " + address; }
            }

            /// <summary>
            /// ToString() override.
            /// </summary>
            /// <returns>Returns the model name and address.</returns>
            public override string ToString()
            {
                return ModelNameAndAddressString;
            }
        }
        #endregion
    }

    internal sealed class OnlineDeviceMonitor
    {
        //  the allowable gap between device status messages
        const int devicePresencePeriodmS = 3400;

        /// <summary>
        /// Delegate for notifying subscriber of change to online device list.
        /// </summary>
        public ECCONetApi.OnlineDeviceListChangedDelegate onlineDeviceListChanged;

        /// <summary>
        /// Delegate for notifying subscriber of device online status change event.
        /// </summary>
        public ECCONetApi.DeviceOnlineStatusEventDelegate deviceOnlineStatusEvent;

        /// <summary>
        /// When true, devices that go offline are retained in the list.
        /// </summary>
        public bool RetainOfflineDevices { get; set; } = false;

        /// <summary>
        /// Pauses the monitor.
        /// </summary>
        public bool Pause { get; set; } = true;

        /// <summary>
        /// The maximum number of times the info getter should try to get a device's info.
        /// When set to zero, the info getter tries until info is acquired.
        /// </summary>
        public int MaximumGetInfoTries { get; set; } = 0;

        /// <summary>
        /// Send dummy token to monitor WinUSB.
        /// </summary>
        public bool SendDummyToken { get; set; } = true;



        #region Info file data fields

        //	the product field positions in the product info file
        const int modelNameSize = 31;
        const int modelNamePosition = 0;
        const int manufacturerNameSize = 31;
        const int manufacturerNamePosition = modelNameSize + modelNamePosition;
        const int hardwareRevisionSize = 6;
        const int hardwareRevisionPosition = manufacturerNameSize + manufacturerNamePosition;
        const int appFirmwareRevisionSize = 6;
        const int appFirmwareRevisionPosition = hardwareRevisionSize + hardwareRevisionPosition;
        const int bootloaderFirmwareRevisionSize = 6;
        const int bootloaderFirmwareRevisionPosition = appFirmwareRevisionSize + appFirmwareRevisionPosition;
        const int baseLightheadEnumerationSize = 6;
        const int baseLightheadEnumerationPosition = bootloaderFirmwareRevisionSize + bootloaderFirmwareRevisionPosition;
        const int maxLightheadEnumerationSize = 6;
        const int maxLightheadEnumerationPosition = baseLightheadEnumerationSize + baseLightheadEnumerationPosition;
        const int fileInfoSize = maxLightheadEnumerationPosition + maxLightheadEnumerationSize;
        #endregion


        //  the private list of online ECCONet devices
        List<ECCONetApi.ECCONetDevice> onlineDevices = new List<ECCONetApi.ECCONetDevice>(10);

        //  the online device list lock
        Object onlineDevicesLock = new object();

        //  the online devices update counter
        //  slows down the update rate from the core clock
        int updateCounter = 0;

        //  dummy token counter slows down the dummy token rate
        int dummyTokenCounter = 0;

        //  the core logic
        ECCONetCore core;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="core">The core logic group.</param>
        public OnlineDeviceMonitor(ECCONetCore core)
        {
            //  save the core
            this.core = core;
        }

        /// <summary>
        /// Clocks the online device monitor.
        /// </summary>
        public void Clock()
        {
            //  if time to update list of devices
            if (++updateCounter >= 10)
            {
                updateCounter = 0;

                //  monitor the devices
                //  this method checks for pause
                MonitorDevices();
            }

            //  if time to send dummy token
            if (!Pause && SendDummyToken && ++dummyTokenCounter >= 100)
            {
                //  reset counter
                dummyTokenCounter = 0;

                Thread dummyTokenSend = new Thread(() =>
                {
                    //  send dummy token
                    //  if the WinUSB driver cannot send, then it will reset itself
                    core.SendTokenToCanBus(new Token(Token.Keys.KeyNull, 0, core.GetCanAddress(), 0));
                });
                dummyTokenSend.Start();
            }
        }

        #region Online device list access
        /// <summary>
        /// The online device list access mode.
        /// </summary>
        enum OnlineDeviceListAccessMode
        {
            DeviceMessageReceived,
            Monitor,
            GetDevice,
            GetListCopy,
            ClearList,
            GetFullListCopy,
        }

        /// <summary>
        /// Access for the online devices list.
        /// </summary>
        /// <param name="mode">The access mode.</param>
        /// <param name="address">The device address optional param depending on the access mode.</param>
        /// <param name="obj">An output object whose type depends on the access mode.</param>
        /// <param name="deviceEvent">An output device event that depends on the access mode.</param>
        /// <returns></returns>
        bool AccessOnlineDevices(OnlineDeviceListAccessMode mode, byte address,
            out object obj, out ECCONetApi.DeviceOnlineEvent deviceEvent)
        {
            //  make sure objects exist
            if (null == onlineDevices)
                onlineDevices = new List<ECCONetApi.ECCONetDevice>(10);
            if (null == onlineDevicesLock)
                onlineDevicesLock = new object();

            lock (onlineDevicesLock)
            {
                obj = null;
                deviceEvent = ECCONetApi.DeviceOnlineEvent.NoEvent;
                bool listChanged = false;
                bool found = false;

                switch (mode)
                {
                    case OnlineDeviceListAccessMode.DeviceMessageReceived:
                        //  if address is valid
                        if (address != 0 && address <= 127)
                        {
                            //  search for matching address already in list
                            //  if found, then update the device timestamp
                            foreach (var d in onlineDevices)
                            {
                                if (d.address == address)
                                {
                                    found = true;
                                    d.lastStatusTime = DateTime.Now;
                                    obj = d;
                                    break;
                                }
                            }

                            //  if address not in list
                            if (!found)
                            {
                                //  create new ECCONet device and add to list
                                var d = new ECCONetApi.ECCONetDevice();
                                d.address = address;
                                d.lastStatusTime = DateTime.Now;
                                d.productInfoStatus.lastGetInfoTimestamp = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(800));

                                //  ML  8/29/2018  adding special code for VBT since it does not provide device info file
                                if (d.address == LibConfig.MATRIX_VEHICLE_BUS_ADDRESS)
                                {
                                    d.manufacturerName = "CANM8 Ltd";
                                    d.modelName = "Vehicle Bus Translator";
                                    d.baseLightheadEnumeration = "n/a";
                                    d.maxLightheadEnumeration = "n/a";
                                    d.serverAccessCode = UInt32.MaxValue;
                                    d.productInfoStatus.status = ECCONetApi.ECCONetDevice.ProductInfoStatus.Status.HaveInfo;
                                }

                                //  add device to list
                                onlineDevices.Add(d);
                                listChanged = true;
                                obj = d;
                                deviceEvent = ECCONetApi.DeviceOnlineEvent.JustCameOnline;
                            }
                        }
                        break;

                    case OnlineDeviceListAccessMode.GetDevice:
                        foreach (var d in onlineDevices)
                        {
                            if (d.address == address)
                            {
                                obj = d;
                                break;
                            }
                        }
                        break;

                    case OnlineDeviceListAccessMode.Monitor:
                        //  check expiration of online devices
                        for (int i = 0; i < onlineDevices.Count; ++i)
                        {
                            if ((DateTime.Now - onlineDevices[i].lastStatusTime).TotalMilliseconds > devicePresencePeriodmS)
                            {
                                //  indicate device is no longer online
                                obj = onlineDevices[i];
                                deviceEvent = ECCONetApi.DeviceOnlineEvent.JustWentOffline;

                                if (!RetainOfflineDevices)
                                {
                                    onlineDevices.RemoveAt(i);
                                    listChanged = true;
                                }
                            }
                        }
                        break;

                    case OnlineDeviceListAccessMode.GetListCopy:
                        List<ECCONetApi.ECCONetDevice> list = new List<ECCONetApi.ECCONetDevice>(onlineDevices.Count);
                        foreach (var d in onlineDevices)
                            if (d.productInfoStatus.status == ECCONetApi.ECCONetDevice.ProductInfoStatus.Status.HaveInfo
                                ||  d.productInfoStatus.status == ECCONetApi.ECCONetDevice.ProductInfoStatus.Status.InfoFileNotAvailable)
                                list.Add(d.Copy());
                        obj = list;
                        break;

                    case OnlineDeviceListAccessMode.GetFullListCopy:
                        List<ECCONetApi.ECCONetDevice> full_list = new List<ECCONetApi.ECCONetDevice>(onlineDevices.Count);
                        foreach (var d in onlineDevices)
                            full_list.Add(d.Copy());
                        obj = full_list;
                        break;

                    case OnlineDeviceListAccessMode.ClearList:
                        onlineDevices = new List<ECCONetApi.ECCONetDevice>(10);
                        listChanged = true;
                        break;

                    default:
                        break;
                }
                return listChanged;
            }
        }
        #endregion

        #region Message received and device timeout monitor
        /// <summary>
        /// Handles a device message received event.
        /// </summary>
        /// <param name="address">The address to check.</param>
        public void DeviceMessageReceived(byte address)
        {
            //  if online device monitor not paused
            if (!Pause)
            {
                if (AccessOnlineDevices(OnlineDeviceListAccessMode.DeviceMessageReceived, address, out object obj, out ECCONetApi.DeviceOnlineEvent deviceEvent))
                {
                    onlineDeviceListChanged?.Invoke(GetOnlineDevicesList());
                }

                //  if device added or removed event then notify subscriber
                if (deviceEvent != ECCONetApi.DeviceOnlineEvent.NoEvent)
                    deviceOnlineStatusEvent?.Invoke(obj as ECCONetApi.ECCONetDevice, deviceEvent);

                //  if device and need product info and has not exceeded tries and has not requested info in at least 800ms
                if (obj is ECCONetApi.ECCONetDevice device)
                {
                    var lastTimeStamp = device.productInfoStatus.lastGetInfoTimestamp;
                    var nextTimeStamp = device.productInfoStatus.lastGetInfoTimestamp.AddMilliseconds(800);
                    
                    if (device.productInfoStatus.status != ECCONetApi.ECCONetDevice.ProductInfoStatus.Status.HaveInfo
                        && device.productInfoStatus.status != ECCONetApi.ECCONetDevice.ProductInfoStatus.Status.InfoFileNotAvailable
                        && (MaximumGetInfoTries == 0 || MaximumGetInfoTries > device.productInfoStatus.getInfoTries)
                        && nextTimeStamp > lastTimeStamp
                        && _currentAddress == 0)
                    {
                        GetDeviceInfo(device);
                    }
                }
            }
        }

        /// <summary>
        /// Monitor devices for info updates and timeouts.
        /// </summary>
        void MonitorDevices()
        {
            //  if online device monitor not paused
            if (!Pause)
            {
                if (AccessOnlineDevices(OnlineDeviceListAccessMode.Monitor, 0, out object obj, out ECCONetApi.DeviceOnlineEvent deviceEvent))
                {
                    //  notify delegate that list has changed
                    onlineDeviceListChanged?.Invoke(GetOnlineDevicesList());
                }

                //  if device added or removed event then notify subscriber
                if (deviceEvent != ECCONetApi.DeviceOnlineEvent.NoEvent)
                    deviceOnlineStatusEvent?.Invoke(obj as ECCONetApi.ECCONetDevice, deviceEvent);
            }
        }
        #endregion

        #region List content access
        /// <summary>
        /// Handles a device message received event.
        /// </summary>
        /// <param name="address">The address to check.</param>
        public ECCONetApi.ECCONetDevice GetDeviceByAddress(byte address)
        {
            AccessOnlineDevices(OnlineDeviceListAccessMode.GetDevice, address, out object obj, out ECCONetApi.DeviceOnlineEvent deviceEvent);
            return obj as ECCONetApi.ECCONetDevice;
        }

        /// <summary>
        /// Clears the list of online devices.  The list will automatically regenerate.
        /// </summary>
        public void ClearOnlineDevicesList()
        {
            if (AccessOnlineDevices(OnlineDeviceListAccessMode.ClearList, 0, out object obj, out ECCONetApi.DeviceOnlineEvent deviceEvent))
            {
                //  notify delegate that list has changed
                onlineDeviceListChanged?.Invoke(GetOnlineDevicesList());
            }
        }

        /// <summary>
        /// Gets a deep copy of the online devices list.
        /// </summary>
        /// <returns>A deep copy of the online devices list.</returns>
        public List<ECCONetApi.ECCONetDevice> GetOnlineDevicesList()
        {
            AccessOnlineDevices(OnlineDeviceListAccessMode.GetListCopy, 0, out object obj, out ECCONetApi.DeviceOnlineEvent deviceEvent);
            return (List<ECCONetApi.ECCONetDevice>)obj;
        }

        /// <summary>
        /// Gets a deep copy of the online devices list.
        /// </summary>
        /// <param name="filteredByProductInfo">Set true to only get devices that responded to product info request.</param>
        /// <returns>A deep copy of the online devices list.</returns>
        public List<ECCONetApi.ECCONetDevice> GetOnlineDevicesList(bool filteredByProductInfo)
        {
            object obj = null;

            if (filteredByProductInfo)
                AccessOnlineDevices(OnlineDeviceListAccessMode.GetListCopy, 0, out obj, out ECCONetApi.DeviceOnlineEvent deviceEvent);
            else  //  full list
                AccessOnlineDevices(OnlineDeviceListAccessMode.GetFullListCopy, 0, out obj, out ECCONetApi.DeviceOnlineEvent deviceEvent);
            return obj as List<ECCONetApi.ECCONetDevice>;
        }
        #endregion

        #region Device info reader

        // used to serialize the ftp reads for the device info file. if the _currentAddress != 0 then we know 
        // we are currently already trying to get a device info file from a device
        byte _currentAddress = 0;

        /// <summary>
        /// Gets the access code and info file for an online device.
        /// </summary>
        /// <param name="device">The device for which the reader is getting info.</param>
        /// <returns>Returns 0 for scan started, else -1 on error.</returns>
        int GetDeviceInfo(ECCONetApi.ECCONetDevice device)
        {
            //  validate device and status
            if (device == null)
                return -1;

            //  request the device file info to get access code
            var request = new FtpClient.FtpTransactionRequest(FtpClient.FtpTransactionType.GetFileInfo,
                device.address, (UInt32)(new Random().NextDouble()), ECCONetApi.FileName.ProductInfo, core.ftpServerResponseTimeout, FtpInfoCallback);
            int status = core.FTP_TransactionRequest(request);
            if (0 == status)
            {
                _currentAddress = device.address;

                //  debug
                if (core.consoleDebugMessageLevel >= ECCONetCore.ConsoleDebugMessageLevels.FtpRequestsAndResponses)
                   Debug.WriteLine($"OnlineDeviceMonitor: Requesting file info for product.inf ({device.address})");

                //  update timestamp
                device.productInfoStatus.lastGetInfoTimestamp = DateTime.Now;

                //  update number of tries
                ++device.productInfoStatus.getInfoTries;
            }
            return status;
        }


        /// <summary>
        /// The FTP server transaction FileInfoComplete callback.
        /// </summary>
        /// <param name="callback">The callback data from the server.</param>
        void FtpInfoCallback(ECCONetApi.FtpCallbackInfo callback)
        {       
            //  validate callback address
            if (callback.serverAddress == 0 || callback.serverAddress > 127)
                return;

            //  get the device associated with the callback
            var device = GetDeviceByAddress(callback.serverAddress);
            if (device == null)
                return;

            //  if device provided info on product info
            if (Token.Keys.KeyResponseFileInfoComplete == callback.responseKey)
            {
                //  add device access code, GUID and info file date
                device.serverAccessCode = callback.serverAccessCode;
                device.guid = callback.serverGuid;
                device.infoFileDate = callback.fileDate;

                //  if device responded to request but indicates info file is not available
                if (1 == callback.fileDataSize)
                {
                    //  set device product info state
                    device.productInfoStatus.status = ECCONetApi.ECCONetDevice.ProductInfoStatus.Status.InfoFileNotAvailable;

                    //  notify delegate that the list has changed
                    onlineDeviceListChanged?.Invoke(GetOnlineDevicesList());

                    //  notify delegate that device was just acquired but no product info was available
                    deviceOnlineStatusEvent?.Invoke(device, ECCONetApi.DeviceOnlineEvent.JustIndicatedNoProductInfoAvailable);
                }
                //  else device indicated that device info file is available
                else
                {
                    //  request the device info file
                    var request = new FtpClient.FtpTransactionRequest(FtpClient.FtpTransactionType.ReadFile,
                        device.address, device.serverAccessCode, ECCONetApi.FileName.ProductInfo, core.ftpServerResponseTimeout, FtpReadCallback);
                    if (0 == core.FTP_TransactionRequest(request))
                    {
                        //  debug
                        if (core.consoleDebugMessageLevel >= ECCONetCore.ConsoleDebugMessageLevels.FtpRequestsAndResponses)
                            Debug.WriteLine($"OnlineDeviceMonitor: Requesting product.inf file ({device.address})");
                    }
                }
            }
            else  //  unable to get info on product info file
            {
                //  set device product info state
                device.productInfoStatus.status = ECCONetApi.ECCONetDevice.ProductInfoStatus.Status.NotResponsive;

                //  notify delegate that the list has changed
                onlineDeviceListChanged?.Invoke(GetOnlineDevicesList());
            }
        }

        /// <summary>
        /// The FTP server transaction FileReadComplete callback.
        /// </summary>
        /// <param name="callback">The callback data from the server.</param>
        void FtpReadCallback(ECCONetApi.FtpCallbackInfo callback)
        {
            //  validate callback address
            if (callback.serverAddress == 0 || callback.serverAddress > 127)
                return;

            //  get the device associated with the callback
            var device = GetDeviceByAddress(callback.serverAddress);
            if (device == null)
                return;

            _currentAddress = 0;

            //  if device provided product info file
            //  ML 2019-10-03  include patch to not reject product info file with bad checksum 
            if (Token.Keys.KeyResponseFileReadComplete == callback.responseKey
                || Token.Keys.KeyResponseFileChecksumError == callback.responseKey)
            {
                //  if info file has full set of data
                if (fileInfoSize <= callback.fileData.Length)
                {
                    //  set device fields
                    device.modelName = System.Text.Encoding.UTF8.GetString(callback.fileData,
                        modelNamePosition, modelNameSize).TrimEnd('\0');
                    device.manufacturerName = System.Text.Encoding.UTF8.GetString(callback.fileData,
                        manufacturerNamePosition, manufacturerNameSize).TrimEnd('\0');
                    device.hardwareRevision = System.Text.Encoding.UTF8.GetString(callback.fileData,
                        hardwareRevisionPosition, hardwareRevisionSize).TrimEnd('\0');
                    device.appFirmwareRevision = System.Text.Encoding.UTF8.GetString(callback.fileData,
                        appFirmwareRevisionPosition, appFirmwareRevisionSize).TrimEnd('\0');
                    device.bootloaderFirmwareRevision = System.Text.Encoding.UTF8.GetString(callback.fileData,
                        bootloaderFirmwareRevisionPosition, bootloaderFirmwareRevisionSize).TrimEnd('\0');
                    device.baseLightheadEnumeration = System.Text.Encoding.UTF8.GetString(callback.fileData,
                        baseLightheadEnumerationPosition, baseLightheadEnumerationSize).TrimEnd('\0');
                    device.maxLightheadEnumeration = System.Text.Encoding.UTF8.GetString(callback.fileData,
                        maxLightheadEnumerationPosition, maxLightheadEnumerationSize).TrimEnd('\0');

                    //  set device product info state
                    device.productInfoStatus.status = ECCONetApi.ECCONetDevice.ProductInfoStatus.Status.HaveInfo;

                    //  notify delegate that the list has changed
                    onlineDeviceListChanged?.Invoke(GetOnlineDevicesList());

                    //  notify delegate that device was just acquired and product info is available
                    deviceOnlineStatusEvent?.Invoke(device, ECCONetApi.DeviceOnlineEvent.JustProvidedProductInfo);
                }
                else  //  else for some reason did not provide full product info file
                {
                    //  set device product info state
                    device.productInfoStatus.status = ECCONetApi.ECCONetDevice.ProductInfoStatus.Status.HavePartialInfo;

                    //  notify delegate that the list has changed
                    onlineDeviceListChanged?.Invoke(GetOnlineDevicesList());

                    //  notify delegate that device was just acquired but no product info was available
                    deviceOnlineStatusEvent?.Invoke(device, ECCONetApi.DeviceOnlineEvent.JustProvidedPartialInfo);
                }
            }
            else  //  device did not provide product info file
            {
                //  set device product info state
                device.productInfoStatus.status = ECCONetApi.ECCONetDevice.ProductInfoStatus.Status.NotResponsive;

                //  notify delegate that the list has changed
                onlineDeviceListChanged?.Invoke(GetOnlineDevicesList());
            }
        }
        #endregion
    }
}

