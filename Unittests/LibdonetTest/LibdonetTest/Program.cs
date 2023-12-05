using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace ECCONetLibDemo
{
    class Program
    {
        private UsbDevice usbCanDevice;
        private UsbEndpointReader reader;
        private Thread canReadThread;
        private bool _isConnectedAndReady = false;
        private bool shouldAbortReadThread = false;

        const int VendorID = 0x2D03;
        const int ProductID = 0x0001;

        public delegate void CanFrameReceivedDelegate(UInt32 id, byte[] data);
        public event CanFrameReceivedDelegate canFrameReceivedDelegate;

        static void Main(string[] args)
        {
            Program program = new Program();
            try
            {
                program.Connect();
                program.StartReadingCanFrames();

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();

                program.StopReadingCanFrames();
                program.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error encountered: {ex.Message}");
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        public void Connect()
        {
            Console.WriteLine("Enumerating USB devices...");
            bool deviceFound = false;

            foreach (UsbRegistry usbRegistry in UsbDevice.AllDevices)
            {
                Console.WriteLine($"USB  Device: VID={usbRegistry.Vid:X4}, PID={usbRegistry.Pid:X4}");

                if (usbRegistry.Vid == VendorID && usbRegistry.Pid == ProductID)
                {
                    deviceFound = true;
                    usbRegistry.Open(out usbCanDevice);
                    bool openResult = usbRegistry.Open(out usbCanDevice);
                    Console.WriteLine($"Open result: {openResult}");
                    Console.WriteLine($"usbCanDevice is null: {usbCanDevice == null}");
                    _isConnectedAndReady = true;
                    if(usbCanDevice != null)
                        reader = usbCanDevice.OpenEndpointReader(ReadEndpointID.Ep01);
                    if (usbCanDevice == null)
                    {
                        Console.WriteLine("Failed to open Code 3 SIB device.");
                        
                    }   

                    break;

                    
                   
                }
            
            }
            if (!deviceFound)
            {  
                
            }
        

        }

       
        public void Disconnect()
        {
            if (usbCanDevice != null)
            {
                usbCanDevice.Close();
                usbCanDevice = null;
            }
            _isConnectedAndReady = false;
        }

        private void StartReadingCanFrames()
        {
            shouldAbortReadThread = false;
            canReadThread = new Thread(ReadCanFrames)
            {
                IsBackground = true
            };
            canReadThread.Start();
        }

        private void StopReadingCanFrames()
        {
            shouldAbortReadThread = true;
            canReadThread?.Join();
            reader?.Dispose();
        }

        private void ReadCanFrames()
        {
            byte[] usbData = new byte[256];
            int numBytesRead = 0;
            ErrorCode ec = ErrorCode.None;

            try
            {
                while (!shouldAbortReadThread)
                {
                    if (_isConnectedAndReady)
                    {
                        ec = reader.Read(usbData, 0, usbData.Length, 5000, out numBytesRead);

                        if (ec == ErrorCode.None && numBytesRead > 0)
                        {
                            int index = 0;
                            while (index < numBytesRead)
                            {
                                UInt32 id = (UInt32)((usbData[index] << 24) | (usbData[index + 1] << 16)
                                    | (usbData[index + 2] << 8) | usbData[index + 3]);
                                byte[] data = new byte[usbData[index + 4]];
                                for (int i = 0; i < data.Length; ++i)
                                    data[i] = usbData[index + i + 5];
                                canFrameReceivedDelegate?.Invoke(id, data);
                                index += 13;
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(20); // Pause when not connected
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("USB-CAN receive thread error: " + ex.Message);
            }
        }
    }
}
