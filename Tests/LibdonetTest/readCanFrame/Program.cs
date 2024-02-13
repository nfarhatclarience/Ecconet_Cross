﻿using System;
using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.Main;
using System.Collections.ObjectModel;
using System.Text;
public class ECCONet_UsBDotNetCanApi
{
    private UsbDevice myUsbDevice;
    private UsbDeviceFinder myUsbFinder;
    private Timer devicePollingTimer;
    private bool isDeviceConnected;
    Thread canReadThread;

    const int VendorID = 0x2D03;  // Replace with your Vendor ID
    const int ProductID = 0x0001; // Replace with your Product ID
    private const byte BulkInEndpointId = 0x81; // Endpoint ID for bulk read
    private const byte BulkOutEndpointId = 0x01; // Endpoint ID for bulk write

    // print usb device configuration
    public ECCONet_UsBDotNetCanApi()
    {
        myUsbFinder = new UsbDeviceFinder(VendorID, ProductID);
        isDeviceConnected = false;
        try
        {
            myUsbDevice = UsbDevice.OpenUsbDevice(myUsbFinder);
            if (myUsbDevice == null)
            {
                throw new Exception("Device not found.");
            }
            else
            {
                Console.WriteLine("Device Found and Connected");
            }
           
           Console.WriteLine(myUsbDevice.Info.ToString());

                    for (int iConfig = 0; iConfig < myUsbDevice.Configs.Count; iConfig++)
                    {
                        UsbConfigInfo configInfo = myUsbDevice.Configs[iConfig];
                        Console.WriteLine(configInfo.ToString());

                        ReadOnlyCollection<UsbInterfaceInfo> interfaceList = configInfo.InterfaceInfoList;
                        for (int iInterface = 0; iInterface < interfaceList.Count; iInterface++)
                        {
                            UsbInterfaceInfo interfaceInfo = interfaceList[iInterface];
                            Console.WriteLine(interfaceInfo.ToString());

                            ReadOnlyCollection<UsbEndpointInfo> endpointList = interfaceInfo.EndpointInfoList;
                            for (int iEndpoint = 0; iEndpoint < endpointList.Count; iEndpoint++)
                            {
                                Console.WriteLine(endpointList[iEndpoint].ToString());
                            }
                        }
                    }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            
        }
    
    
    }
   
    // ... Additional methods
    public void readCanFrame()
            {
                UsbEndpointReader reader = myUsbDevice.OpenEndpointReader((ReadEndpointID)BulkInEndpointId);
                byte[] usbData = new byte[256];
                int numBytesRead;
                ErrorCode ecRead = reader.Read(usbData, 2000, out numBytesRead);
                if (ecRead == ErrorCode.None) 
                {
                    int index = 0;
                            while (index < numBytesRead)
                            {
                                UInt32 id = (UInt32)((usbData[index] << 24) | (usbData[index + 1] << 16)
                                    | (usbData[index + 2] << 8) | usbData[index + 3]);
                                byte[] data = new byte[usbData[index + 4]];
                                for (int i = 0; i < data.Length; ++i)
                                    data[i] = usbData[index + i + 5];
                                
                                index += 13;
                                Console.WriteLine(id);
                            }
                }
                    
                else
                {
                    Console.WriteLine("Hello");
                }
            }
    // add main method
    public static void Main(string[] args)
    {
        // Create a new instance of the class
        ECCONet_UsBDotNetCanApi myApi = new ECCONet_UsBDotNetCanApi();
        myApi.readCanFrame();
     
        // Wait for the user to press a key
        Console.ReadKey();
        UsbDevice.Exit();
    }
}

/*
using System;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System.Text;

public class UsbCommunicationExample
{
    private const int VendorId = 0xYourVendorId; // Replace with your Vendor ID
    private const int ProductId = 0xYourProductId; // Replace with your Product ID
    private const byte BulkInEndpointId = 0x81; // Endpoint ID for bulk read
    private const byte BulkOutEndpointId = 0x01; // Endpoint ID for bulk write

    public static void Main(string[] args)
    {
        // Initialize the USB device based on the specified vendor and product IDs.
        UsbDeviceFinder deviceFinder = new UsbDeviceFinder(VendorId, ProductId);
        UsbDevice myDevice = UsbDevice.OpenUsbDevice(deviceFinder);

        if (myDevice == null)
        {
            Console.WriteLine("Device not found.");
            return;
        }

        try
        {
            if (myDevice is IUsbDevice wholeDevice)
            {
                // Select the configuration and claim the interface.
                wholeDevice.SetConfiguration(1);
                wholeDevice.ClaimInterface(0);
            }

            // Open the bulk IN endpoint for reading.
            UsbEndpointReader reader = myDevice.OpenEndpointReader((ReadEndpointID)BulkInEndpointId);
            // Open the bulk OUT endpoint for writing.
            UsbEndpointWriter writer = myDevice.OpenEndpointWriter((WriteEndpointID)BulkOutEndpointId);

            // Example write operation
            byte[] writeBuffer = Encoding.Default.GetBytes("Hello USB");
            int bytesWritten;
            ErrorCode ecWrite = writer.Write(writeBuffer, 2000, out bytesWritten);
            if (ecWrite != ErrorCode.None) Console.WriteLine("Write error: " + ecWrite);

            // Example read operation
            byte[] readBuffer = new byte[1024];
            int bytesRead;
            ErrorCode ecRead = reader.Read(readBuffer, 2000, out bytesRead);
            if (ecRead != ErrorCode.None) Console.WriteLine("Read error: " + ecRead);
            else Console.WriteLine("Read data: " + Encoding.Default.GetString(readBuffer, 0, bytesRead));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            if (myDevice != null)
            {
                if (myDevice.IsOpen)
                {
                    IUsbDevice wholeUsbDevice = myDevice as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        // Release the interface and close the device.
                        wholeUsbDevice.ReleaseInterface(0);
                    }
                    myDevice.Close();
                }
                myDevice = null;

                // Free USB resources.
                UsbDevice.Exit();
            }
        }
    }
}
*/