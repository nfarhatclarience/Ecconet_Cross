using ECCONet;
using System;
using System.Collections.Generic;
using System.Threading;

public class DeviceDiscoveryTest
{
    private static ECCONetApi api;

    public static void Main(string[] args)
    {
        // Initialize ECCONetApi
        api = new ECCONetApi();
        api.Pause = false; // Make sure the monitor is not paused

        // Subscribe to device list changed event (optional)
        api.onlineDeviceListChangedDelegate += OnlineDeviceListChangedHandler;
        api.deviceOnlineStatusEvent += DeviceOnlineStatusEventHandler;

        // Get initial device list
        List<ECCONetApi.ECCONetDevice> initialDevices = api.GetOnlineDevicesList(false);

        // Power on or connect the device to discover

        // Wait for device detection
        Thread.Sleep(2000); // Adjust the wait time as needed

        // Get updated device list
        List<ECCONetApi.ECCONetDevice> updatedDevices = api.GetOnlineDevicesList(false);

        // Check if the device was discovered
        bool deviceDiscovered = updatedDevices.Any(d => !initialDevices.Contains(d));

        if (deviceDiscovered)
        {
            Console.WriteLine("Device discovered successfully!");
        }
        else
        {
            Console.WriteLine("Device not discovered.");
        }
    }

    private static void OnlineDeviceListChangedHandler(List<ECCONetApi.ECCONetDevice> list)
    {
        // Handle device list changes (optional)
    }
    // Event handler for device online status changes
    private static void DeviceOnlineStatusEventHandler(ECCONetApi.ECCONetDevice device, ECCONetApi.DeviceOnlineEvent deviceEvent)
    {
    if (deviceEvent == ECCONetApi.DeviceOnlineEvent.JustProvidedProductInfo && device.address == 90)
        {
        // Print device information to the terminal
        Console.WriteLine("Device Information:");
        Console.WriteLine("Model Name: " + device.modelName);
        Console.WriteLine("Manufacturer: " + device.manufacturerName);
        Console.WriteLine("Hardware Revision: " + device.hardwareRevision);
        Console.WriteLine("App Firmware Revision: " + device.appFirmwareRevision);
        Console.WriteLine("Bootloader Firmware Revision: " + device.bootloaderFirmwareRevision);
        // ... (Print other device information as needed)
        }   
}
}