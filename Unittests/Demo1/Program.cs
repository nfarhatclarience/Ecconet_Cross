using System;
using ECCONet.UsbCan; // Replace with the actual namespace of your USBCANAPI

namespace USBCANAPIDemonstrator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the USBCANAPI
           ECCONet_UsbCanApi usbCanApi = null;
           ECCONet_UsbDotNetCanApi usbDotNetCanApi = null; 
           Console.WriteLine("Monitoring USB-CAN device connection status...");
           Console.WriteLine("Press '1' to exit, '2' To disconnect");
            // Subscribe to the connection status changed event
            if (args[0] == "1")
                {   
                    Console.WriteLine("Using ECCONet_UsbCanApi");
                    usbCanApi = new ECCONet_UsbCanApi(shouldAutoConnect: true);
                    usbCanApi.connectionStatusChangedDelegate += ConnectionStatusChanged;
                }
            else
                {   
                    Console.WriteLine("Using ECCONet_UsbDotNetCanApi");
                    usbDotNetCanApi = new ECCONet_UsbDotNetCanApi(shouldAutoConnect: true);
                    usbDotNetCanApi.connectionStatusChangedDelegate += ConnectionStatusChanged;
                }

            // Wait for the user to end the demonstration
            var user_input = Console.ReadLine();
           
        }
        

        private static void ConnectionStatusChanged(bool isConnected)
        {
            Console.WriteLine($"USB-CAN Connection Status: {(isConnected ? "Connected" : "Disconnected")}");
        }
    }

}