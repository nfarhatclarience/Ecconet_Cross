using System;
using ECCONet.UsbCan; // Replace with the actual namespace of your USBCANAPI

namespace USBCANAPIDemonstrator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the USBCANAPI
            var usbCanApi = new ECCONet_UsbDotNetCanApi(shouldAutoConnect: true);

            // Subscribe to the connection status changed event
            usbCanApi.connectionStatusChangedDelegate += ConnectionStatusChanged;

            Console.WriteLine("Monitoring USB-CAN device connection status...");
            Console.WriteLine("Press 'Enter' to exit.");

            // Wait for the user to end the demonstration
            Console.ReadLine();
        }

        private static void ConnectionStatusChanged(bool isConnected)
        {
            Console.WriteLine($"USB-CAN Connection Status: {(isConnected ? "Connected" : "Disconnected")}");
        }
    }
}
