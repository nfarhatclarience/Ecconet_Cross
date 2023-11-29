using System;
using LibUsbDotNet;
using LibUsbDotNet.Main;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enumerating USB devices...");

        foreach (UsbRegistry usbRegistry in UsbDevice.AllDevices)
        {
            Console.WriteLine($"Device: VID={usbRegistry.Vid:X4}, PID={usbRegistry.Pid:X4}");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
