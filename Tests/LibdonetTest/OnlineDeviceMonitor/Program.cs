using System;
using System.Threading;
using ECCONet;

namespace ListOnlineDevices
{
    class Program
    {
        static ECCONetApi canInterface;

        static void Main(string[] args)
        {
            // Initialize the ECCONet API
            canInterface = new ECCONetApi();

            // Subscribe to the connection status changed event
            canInterface.connectionStatusChangedDelegate += ConnectionStatusChanged;

            // Subscribe to the online device list changed event
            canInterface.onlineDeviceListChangedDelegate += OnlineDeviceListChanged;

            // Start the process manager (for monitoring connections and online devices)
            canInterface.StartProcessManager();

            Console.WriteLine("Monitoring USB-CAN device connection status...");

            // Wait for the user to end the program
            Console.ReadLine();
        }

        private static void ConnectionStatusChanged(bool isConnected)
        {
            Console.WriteLine($"USB-CAN Connection Status: {(isConnected ? "Connected" : "Disconnected")}");
        }

        private static void OnlineDeviceListChanged(List<ECCONetApi.ECCONetDevice> list)
        {
            Console.WriteLine("\nOnline Devices:");
            if (list.Count > 0)
            {
                foreach (var device in list)
                {
                    Console.WriteLine($"- {device.modelName} (Address: {device.address})");
                }
            }
            else
            {
                Console.WriteLine("- No online devices found.");
            }
        }
    }
}