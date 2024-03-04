using System;
using ECCONet.UsbCan; // Replace with the actual namespace of your USBCANAPI
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace USBCANAPIDemonstrator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize the USBCANAPI
           ECCONet_UsbDotNetCanApi usbCanApi = null;
           ECCONet_UsbDotNetCanApi usbDotNetCanApi = null; 
           Console.WriteLine("Monitoring USB-CAN device connection status...");
           Console.WriteLine("Press '1' to exit, '2' To disconnect");
           

            // Subscribe to the connection status changed event
            // condition when no argument is passed
            if (args[0] == "0")
                {
                    Console.WriteLine("Assuming Running on the client side");
                    usbCanApi = new ECCONet_UsbDotNetCanApi(shouldAutoConnect: true);
                    usbCanApi.connectionStatusChangedDelegate += ConnectionStatusChanged;
                    usbCanApi.canFrameReceivedDelegate += ReceivedCanFrame;
                }
         
           
            else
            if (args[0]== "1")
                {   
                    var canDataProcessor = new CanDataProcessor("http://127.0.0.1:8000"); // Adjust port if neededs
                    Console.WriteLine("Assuming LocalHost is the server");
                    usbDotNetCanApi = new ECCONet_UsbDotNetCanApi(shouldAutoConnect: true);
                    usbDotNetCanApi.connectionStatusChangedDelegate += ConnectionStatusChanged;
                    usbDotNetCanApi.canFrameReceivedDelegate += async (id, data) => 
                    {
                        try
                        {
                            await canDataProcessor.ProcessCanFrame(id, data);
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"Error processing CAN frame: {ex.Message}"); 
                        }
                    };
                }
                else
                if (args[0]== "2")
                {
                   // read remote host ip add from a json file saved to disk
                     // read the json file 
                    string json = System.IO.File.ReadAllText(@"remotehost.json");
                    dynamic jsonObj = JsonConvert.DeserializeObject(json);
                    string remoteHost = jsonObj["remoteHost"];
                    var canDataProcessor = new CanDataProcessor(remoteHost); // Adjust port if needed
                    Console.WriteLine("Assuming RemoteHost is the server");
                    usbDotNetCanApi = new ECCONet_UsbDotNetCanApi(shouldAutoConnect: true);
                    usbDotNetCanApi.connectionStatusChangedDelegate += ConnectionStatusChanged;
                    usbDotNetCanApi.canFrameReceivedDelegate += async (id, data) => 
                    {
                        try
                        {
                            await canDataProcessor.ProcessCanFrame(id, data);
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"Error processing CAN frame: {ex.Message}"); 
                        }
                    };

                }

            // Wait for the user to end the demonstration
            var user_input = Console.ReadLine();
           
        }
        

        private static void ConnectionStatusChanged(bool isConnected)
        {
            Console.WriteLine($"USB-CAN Connection Status: {(isConnected ? "Connected" : "Disconnected")}");
        }

       public static void ReceivedCanFrame(UInt32 id, byte[] data)
        {
            try
            {
                //  print incoming CAN frame to console (if debug turned on)
                printCanFrameToConsole(id, data, true);
            }
            catch (Exception ex)
            {
               
            }
        }

        public static void  printCanFrameToConsole(UInt32 id, Byte[] data, bool incoming)
        {
            try
            {
                
                {
                    string str = incoming ? "IN  <<--" : "OUT  -->>";
                    str +=
                        DateTime.Now.Second.ToString() + "." + DateTime.Now.Millisecond.ToString() + "  " +
                        id.ToString("X16") + "  " +
                        data.Length.ToString();
                    for (int i = 0; i < data.Length; ++i)
                        str += ("  " + data[i].ToString("X2"));
                    Console.WriteLine(str);
                }
            }
            catch { }
        }
    }

}