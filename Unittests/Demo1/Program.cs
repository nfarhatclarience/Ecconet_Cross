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
            // condition when no argument is passed
            if (args.Length == 0)
                {
                    Console.WriteLine("Using ECCONet_UsbCanApi");
                    usbCanApi = new ECCONet_UsbCanApi(shouldAutoConnect: true);
                    usbCanApi.connectionStatusChangedDelegate += ConnectionStatusChanged;
                    usbCanApi.canFrameReceivedDelegate += ReceivedCanFrame;
                }
         
           
            else
                {   
                    Console.WriteLine("Using ECCONet_UsbDotNetCanApi");
                    usbDotNetCanApi = new ECCONet_UsbDotNetCanApi(shouldAutoConnect: true);
                    usbDotNetCanApi.connectionStatusChangedDelegate += ConnectionStatusChanged;
                    usbDotNetCanApi.canFrameReceivedDelegate += ReceivedCanFrame;
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

                //  forward CAN frame to receiver
               
                    //ReceiveCanFrame(id, data);

                //  forward CAN frame to application
                //ReceivedCanFrame?.Invoke(id, data);
            }
            catch (Exception ex)
            {
               
            }
        }

        public static void  printCanFrameToConsole(UInt32 id, Byte[] data, bool incoming)
        {
            //if (incoming)
            //    return;
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

                    /* Use this to log can frames
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\can_log.txt", true))
                    {
                        file.WriteLine(str);
                        Console.WriteLine(str);
                    }
                    */
                }
            }
            catch { }
        }
    }

}