using System;
using System.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;

public class ECCONet_UsBDotNetCanApi
{
    private UsbDevice myUsbDevice;
    private UsbDeviceFinder myUsbFinder;
    private Timer devicePollingTimer;
    private bool isDeviceConnected;

    const int VendorID = 0x2D03;  // Replace with your Vendor ID
    const int ProductID = 0x0001; // Replace with your Product ID

    public ECCONet_UsBDotNetCanApi()
    {
        myUsbFinder = new UsbDeviceFinder(VendorID, ProductID);
        isDeviceConnected = false;
        devicePollingTimer = new Timer(DevicePollingTimerCallback, null, 0, 3000); // Poll every 3 seconds
    }
    public bool Connect()
    {   
        try
        {
            myUsbDevice = UsbDevice.OpenUsbDevice(myUsbFinder);
            if (myUsbDevice == null)
            {
                throw new Exception("Device not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
        return true;
    }
    private void DevicePollingTimerCallback(object state)
    {
        UsbDevice device = UsbDevice.OpenUsbDevice(myUsbFinder);
        bool currentlyConnected = (device != null);

        if (currentlyConnected != isDeviceConnected)
        {
            isDeviceConnected = currentlyConnected;
            OnDeviceConnectionChanged(isDeviceConnected);
        }

        device?.Close();
    }

    protected void OnDeviceConnectionChanged(bool isConnected)
    {
        // Handle the event: for example, raise an event or execute a callback
        // ...
    }

    // ... Additional methods
    // add main method
    public static void Main(string[] args)
    {
        // Create a new instance of the class
        ECCONet_UsBDotNetCanApi myApi = new ECCONet_UsBDotNetCanApi();
        if(myApi.Connect())
        {
            Console.WriteLine("Device connected");
        }
        else
        {
            Console.WriteLine("Device not connected");
        }
        // Wait for the user to press a key
        Console.ReadKey();
    }
}
