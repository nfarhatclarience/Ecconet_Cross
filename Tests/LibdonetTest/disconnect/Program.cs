using LibUsbDotNet;
using LibUsbDotNet.Main;
using System; // Add missing using statement

namespace LibdonetTest.disconnect
{
    class Program
    {
        private static UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder(0x04D8, 0x0053);
        private static UsbDevice MyUsbDevice;
        private static Timer usbDevicePollingTimer; // Change 'Time' to 'Timer'
        private static bool isDeviceConnected = false;
        static void Main(string[] args)
        {
            MyUsbFinder = new UsbDeviceFinder(0x2D03, 0x0001);
            
            usbDevicePollingTimer = new Timer(TimerCallback, null, 0, 3000);
        
            {
                var user_input = Console.ReadLine();
                if (user_input == "1")
                {
                    break;
                }
            }
            Console.WriteLine("Monitoring USB-CAN device connection status...");
        }
        private static void TimerCallback(Object state)
        {
            UsbDevice device = UsbDevice.OpenUsbDevice(MyUsbFinder);
            bool currentlyConnected = (device != null);

            if (currentlyConnected != isDeviceConnected)
            {
                isDeviceConnected = currentlyConnected;
                ConnectionStatusChanged(isDeviceConnected);
            }

            device?.Close();
        }

        private static void ConnectionStatusChanged(bool isConnected)
        {
            Console.WriteLine($"USB-CAN Connection Status: {(isConnected ? "Connected" : "Disconnected")}");
        }
    }
}