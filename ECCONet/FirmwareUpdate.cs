using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECCONet
{
    public partial class ECCONetApi
    {
        /// <summary>
        /// A callback for firmware update progress updates.
        /// </summary>
        /// <param name="sender">The callback sender.
        /// <param name="device">The device being programmed.</param>
        /// <param name="percentComplete">The percentage of test completion, 0 to 100.</param>
        public delegate void FirmwareUpdateProgressDelegate(object sender, ECCONetDevice device, int percentComplete);

        /// <summary>
        /// A callback for when the firmware update completes.
        /// </summary>
        /// <param name="sender">The callback sender.
        /// <param name="device">The device being programmed.</param>
        /// <param name="result">The firmware udpate result code.</param>
        public delegate void FirmwareUpdateCompleteDelegate(object sender, ECCONetDevice device, FirmwareUpdateStatusCodes result);

        /// <summary>
        /// The firmware update result codes.
        /// </summary>
        public enum FirmwareUpdateStatusCodes
        {
            //  codes from device
            Device_Flash_Write_OK,
            Device_Refused_For_Invalid_Access_Code,
            Device_Refused_For_Invalid_Model_Name,
            Device_Refused_For_Invalid_Flash_Area,
            Device_Had_Flash_Write_Error,

            //  during programming
            No_Response_From_Device,
            Waiting,

            //  image errors
            Bad_Params,
            //  ML  2019-11-20
            //Invalid_Flash_Image_Size_Increment_Of_256,
            Invalid_Flash_Image_Size,
            Invalid_Flash_Image_File_Crc,
            Invalid_Flash_Image_Codebase_Key,
            Invalid_Flash_Image_File_Model_Name,

            //  programmer status
            Programming_Started,
            Programming_Completed,
            Programmer_Busy,
        }
    }

    /// <summary>
    /// A class to implement the HazCAN update protocol with the ECCONet device address.
    /// </summary>
    internal class FirmwareUpdate
    {
        public static class Constants
        {
            public const int FtrKeySize = 4;        // 4 bytes for footer CODEBASE key
            public const int FtrModelSize = 31;     // 31 bytes for footer model name string
            public const int FtrRsrvSize = 5;       // 5 bytes for footer reserved bytes
            public const int FtrAddrSize = 4;       // 4 bytes for footer flash address location
            public const int FtrCrcSize = 4;        // 4 bytes for footer 32-bit CRC
            public const int OverallCrcSize = 4;    // 4 bytes for overall 32-bit image CRC, which calculation includes the footer CRC bytes
            public const int MaxWriteDataSize = 256;// 256 bytes for max write segment data bytes
            public const int WriteAccessSize = 4;   // 4 bytes for write access code size
            public const int WriteDataLocSize = 4;  // 4 bytes for write data location size (flash address to write to)
            public const int WriteDataSize = 2;     // 2 bytes for write data size (number of bytes to write)
            public const int EraseDataSize = 4;     // 4 bytes for erase data size (number of bytes to erase, which could be above 64 kbytes)
        }

        //  the core logic
        ECCONetCore core;

        //  the programmer thread
        Thread programmerThread = null;

        //  programmer thread lock object
        private object programmerLock = new object();

        //  atomic programmer busy flag
        volatile private byte programmerBusy = 0;

        //  atomic programming error flag
        //volatile private byte programmerError = 0;

        //  the update progress callback delegate
        private ECCONetApi.FirmwareUpdateProgressDelegate updateProgressDelegate = null;

        //  the update complete callback delegate
        private ECCONetApi.FirmwareUpdateCompleteDelegate updateCompleteDelegate = null;

        public const int BeginFooterOffset =
            Constants.OverallCrcSize +
            Constants.FtrCrcSize +
            Constants.FtrAddrSize +
            Constants.FtrRsrvSize +
            Constants.FtrModelSize +
            Constants.FtrKeySize;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="core">The core logic group.</param>
        public FirmwareUpdate(ECCONetCore core)
        {
            //  save the core
            this.core = core;
        }

        /// <summary>
        /// Receives tokens from the ECCONet core.
        /// </summary>
        /// <param name="token">The received token.</param>
        public void ReceiveToken(Token token)
        {
            if ((token.key == Token.Keys.KeyResponseFileWriteFixedSegment) || (token.key == Token.Keys.KeyResponseFileEraseFixedSegment))
            { 
                FileWriteFixedSegmentAck = (ECCONetApi.FirmwareUpdateStatusCodes) token.value;
            }
        }


        #region Flash erase

        //  security keys
        //private const UInt32 EsgBootloaderEraseAllKey = 0x9B43FD07;
        //private const UInt32 EsgBootloaderEraseAppKey = 0xDFD94B87;

        /// <summary>
        /// Erases a device's app portion of flash.
        /// </summary>
        /// <param name="device">The device whose app will be erased.</param>
        public void EraseDeviceApplication(ECCONetApi.ECCONetDevice device)
        {
            uint code = Token.TOKEN_VALUE_ERASE_APP_FIRMWARE;
            core.SendTokenToCanBus(new Token(Token.Keys.KeyRequestEraseAppFirmware,
                (int)code ^ (int)device.serverAccessCode, device.address));
        }

        /// <summary>
        /// Erases a device's flash.
        /// </summary>
        /// <param name="device">The device whose flash will be erased.</param>
        public void EraseDeviceFlash(ECCONetApi.ECCONetDevice device)
        {
            uint code = Token.TOKEN_VALUE_ERASE_ALL_FIRMWARE;
            core.SendTokenToCanBus(new Token(Token.Keys.KeyRequestEraseAllFirmware,
                (int)code ^ (int)device.serverAccessCode, device.address));
        }
        #endregion

        #region ECCONet protocol programmer

        /// <summary>
        /// The bootloader status code returned from the device.
        /// </summary>
        volatile ECCONetApi.FirmwareUpdateStatusCodes FileWriteFixedSegmentAck = ECCONetApi.FirmwareUpdateStatusCodes.Waiting;

        /// <summary>
        /// Programs a device via the ECCONet prototcol.
        /// </summary>
        /// <param name="device">The ECCONet device to program.</param>
        /// <param name="image">The flash image.</param>
        /// <param name="progressCallback">A method to call with progress updates.</param>
        /// <param name="completeCallback">A method to call with the update result.</param>
        /// <returns>Returns result code.</returns>
        public ECCONetApi.FirmwareUpdateStatusCodes ProgramDeviceWithImage(ECCONetApi.ECCONetDevice device, byte[] image,
            ECCONetApi.FirmwareUpdateProgressDelegate progressCallback, ECCONetApi.FirmwareUpdateCompleteDelegate completeCallback)
        {
            //  if already busy programming, then just return
            if (programmerBusy != 0)
            {
                return ECCONetApi.FirmwareUpdateStatusCodes.Programmer_Busy;
            }

            //  validate inputs
            if ((device == null) || (image == null) || (image.Length == 0))
            {
                return ECCONetApi.FirmwareUpdateStatusCodes.Bad_Params;
            }

            //  RDOZIER  2021-10-07  Image must be at least one byte plus 48 byte footer and overall crc
            if (image.Length < (BeginFooterOffset + 1))
            {
                return ECCONetApi.FirmwareUpdateStatusCodes.Invalid_Flash_Image_Size;
            }

            //  validate the overall image CRC32
            if (BytesToUInt32LittleEndian(image, image.Length - Constants.OverallCrcSize) != getCRC32(image, (uint)(image.Length - Constants.OverallCrcSize)))
            {
                return ECCONetApi.FirmwareUpdateStatusCodes.Invalid_Flash_Image_File_Crc;
            }

            //  validate the image key
            if (BytesToUInt32LittleEndian(image, image.Length - BeginFooterOffset) != 0xC0DEBA5E)
            { 
                return ECCONetApi.FirmwareUpdateStatusCodes.Invalid_Flash_Image_Codebase_Key;
            }


            //  lock the programmer
            lock (programmerLock)
            {
                //  programmer is now busy
                programmerBusy = 1;

                //  set atomic error code
                //programmerError = 0;

                //  save callbacks
                updateProgressDelegate = progressCallback;
                updateCompleteDelegate = completeCallback;

                //  start the programming thread
                programmerThread = new Thread(() => ProgramDeviceViaECCONet(device, image));
                programmerThread.Start();
            }
            return ECCONetApi.FirmwareUpdateStatusCodes.Programming_Started;
        }

        /// <summary>
        /// Private thread to program device.
        /// </summary>
        /// <param name="device">The device to program.</param>
        /// <param name="image">The device image.</param>
        private void ProgramDeviceViaECCONet(ECCONetApi.ECCONetDevice device, byte[] image)
        {
            //Create a list of model names that are non LAMX of type ecconet bootloader that will be used to check below so no flash erase occurs
            List<string> nonLAMXModelNames = new List<string>();
            nonLAMXModelNames.Add("V2V Sync Module");

            //  invoke the bootloader and wait 50 mS
            uint code = Token.TOKEN_VALUE_INVOKE_BOOTLOADER;
            core.SendTokenToCanBus(new Token(Token.Keys.KeyRequestInvokeBootloader,
                (int)code ^ (int)device.serverAccessCode, device.address));
            Thread.Sleep(50);

            //  get the update info
            UInt32 dataLocation = BytesToUInt32LittleEndian(image, image.Length - (Constants.OverallCrcSize + Constants.FtrCrcSize + Constants.FtrAddrSize));

            int zeroEndIndex;
            int zeroStartIndex;

            // Detect running firmware version to determine bootloader version support
            double appFirmwareVersion = 0.0;
            if (device.appFirmwareRevision == "btldr")
            {
                appFirmwareVersion = 2.9;
            }
            else
            {
                appFirmwareVersion = Convert.ToDouble(device.appFirmwareRevision);
            }
            if (appFirmwareVersion < 3.0 || nonLAMXModelNames.Any(x => x == device.modelName))
            {
                // Disable our zero start and end indexes so it will not attempt KeyRequestFileEraseFixedSegment
                // Setting them equal to each other will disable them
                // zeroEndIndex is also used for percentComplete at the end of the function
                zeroEndIndex = image.Length - Constants.OverallCrcSize;
                zeroStartIndex = zeroEndIndex;
            }
            else // This app is joined with a 3.0 or later bootloader that supports erasing flash sections
            {
                // Start looking for zeroes at the end of file just before the footer.
                zeroEndIndex = image.Length - (BeginFooterOffset + 1);
                zeroStartIndex = zeroEndIndex;

                // Decrement zeroStartIndex until we get to the first non-zero value
                while ((image[zeroStartIndex] == 0) && (zeroStartIndex > 0))
                {
                    zeroStartIndex--;
                }
            }


            //  send update data
            int imageIndex = 0;
            int tries = 0;
            bool sendZero = false;
            int updateMessageDataSize = 0;
            while (imageIndex < (image.Length - Constants.OverallCrcSize))
            {
                //	initialize transmitter
                var tx = new Transmitter(core);
                tx.StartMessage(device.address);

                if ((imageIndex > zeroStartIndex) && (imageIndex < zeroEndIndex))
                {
                    sendZero = true;

                    //  create bin of data to encrypt
                    byte[] bin = new byte[Constants.WriteAccessSize + Constants.FtrModelSize + Constants.WriteDataLocSize + Constants.EraseDataSize];

                    //	add request zero key
                    tx.AddInt16((UInt16)Token.Keys.KeyRequestFileEraseFixedSegment);

                    //  add access code
                    UInt32ToBytesBigEndian(device.serverAccessCode, bin, 0);

                    //  add model name
                    int n = 0;
                    for (n = 0; n < Constants.FtrModelSize; ++n)
                    {
                        bin[n + Constants.WriteAccessSize] = image[n + ((image.Length - BeginFooterOffset) + Constants.WriteAccessSize)];
                    }

                    //  add data location and size in big-endian format
                    UInt32ToBytesBigEndian(dataLocation, bin, Constants.WriteAccessSize + Constants.FtrModelSize);
                    UInt32ToBytesBigEndian((uint)(zeroEndIndex - imageIndex), bin, Constants.WriteAccessSize + Constants.FtrModelSize + Constants.WriteDataLocSize);

                    //  encrypt data
                    Encrypt(device, bin);

                    //  add encrypted data
                    for (n = 0; n < bin.Length; ++n)
                    { 
                        tx.AddByte(bin[n]);
                    }
                }
                else
                {
                    sendZero = false;

                    //  get segment image data size
                    updateMessageDataSize = Math.Min(Constants.MaxWriteDataSize, (image.Length - Constants.OverallCrcSize) - imageIndex);

                    //  create bin of data to encrypt
                    byte[] bin = new byte[Constants.WriteAccessSize + Constants.FtrModelSize + Constants.WriteDataLocSize + Constants.WriteDataSize + updateMessageDataSize];

                    //	add request write key
                    tx.AddInt16((UInt16)Token.Keys.KeyRequestFileWriteFixedSegment);

                    //  add access code
                    UInt32ToBytesBigEndian(device.serverAccessCode, bin, 0);

                    //  add model name
                    int n = 0;
                    for (n = 0; n < Constants.FtrModelSize; ++n)
                    {
                        bin[n + Constants.WriteAccessSize] = image[n + ((image.Length - BeginFooterOffset) + Constants.WriteAccessSize)];
                    }

                    //  add data location and size in big-endian format
                    UInt32ToBytesBigEndian(dataLocation, bin, Constants.WriteAccessSize + Constants.FtrModelSize);
                    UInt16ToBytesBigEndian((uint)updateMessageDataSize, bin, Constants.WriteAccessSize + Constants.FtrModelSize + Constants.WriteDataLocSize);

                    //	add flash data
                    for (n = 0; n < updateMessageDataSize; ++n)
                    { 
                        bin[n + Constants.WriteAccessSize + Constants.FtrModelSize + Constants.WriteDataLocSize + Constants.WriteDataSize] = image[imageIndex + n];
                    }

                    //  encrypt data
                    Encrypt(device, bin);

                    //  add encrypted data
                    for (n = 0; n < bin.Length; ++n)
                    {
                        tx.AddByte(bin[n]);
                    }
                }



                //  set ACK flag
                FileWriteFixedSegmentAck = ECCONetApi.FirmwareUpdateStatusCodes.Waiting;

                //	finish and send message
                tx.FinishMessage();

                //  wait for ACK...
                DateTime timeout;
                if (false == sendZero)
                {
                    // Write segment only does max of 256 flash bytes at a time, so 1.5 seconds should be okay.
                    timeout = DateTime.Now.AddMilliseconds(1500);
                }
                else
                {
                    // Cypress can zero flash bytes at about 20,000 bytes per second.  We may need to erase up 
                    // to 200,000 bytes, so use 15 seconds as timeout.
                    timeout = DateTime.Now.AddMilliseconds(15000);
                }

                Thread.Sleep(20);
                while (FileWriteFixedSegmentAck == ECCONetApi.FirmwareUpdateStatusCodes.Waiting)
                {
                    if (timeout.CompareTo(DateTime.Now) < 0)
                    {
                        FileWriteFixedSegmentAck = ECCONetApi.FirmwareUpdateStatusCodes.No_Response_From_Device;
                        break;
                    }
                }

                //  capture ACK
                ECCONetApi.FirmwareUpdateStatusCodes ack = FileWriteFixedSegmentAck;

                //  if device did not respond with flash write OK
                if (ack != ECCONetApi.FirmwareUpdateStatusCodes.Device_Flash_Write_OK)
                {
                    //  if three tries with no success, then exit
                    if (++tries >= 3)
                    {
                        updateCompleteDelegate?.Invoke(this, device, ack);
                        programmerBusy = 0;
                        return;
                    }
                }
                else  //  segment programmmed properly
                {
                    //  bump image index and data location
                    if (false == sendZero)
                    {
                        imageIndex += updateMessageDataSize;
                        dataLocation += (uint)updateMessageDataSize;
                    }
                    else
                    {
                        // do dataLocation first since it uses imageIndex
                        dataLocation += (uint)(zeroEndIndex - imageIndex);
                        imageIndex += (zeroEndIndex - imageIndex);
                    }

                    //  clear tries
                    tries = 0;
                }

                //  send update
                // Use zeroStartIndex as defacto end for percentComplete indicator
                int percentComplete = (imageIndex * 100) / zeroStartIndex;
                if (percentComplete > 100)
                { 
                    percentComplete = 100;
                }
                updateProgressDelegate?.Invoke(this, device, percentComplete);
            }

            //  reboot and wait 50 mS
            code = Token.TOKEN_VALUE_SYSTEM_REBOOT;
            core.SendTokenToCanBus(new Token(Token.Keys.KeyRequestSystemReboot,
                (int)code ^ (int)device.serverAccessCode, device.address));
            Thread.Sleep(50);
            
            //  programmer done
            programmerBusy = 0;

            //  notify completed
            updateCompleteDelegate?.Invoke(this, device, ECCONetApi.FirmwareUpdateStatusCodes.Programming_Completed);
        }

        #endregion

        #region Byte array to value conversions
        /// <summary>
        /// Big-endian conversion of UInt16 value to bytes.
        /// </summary>
        /// <param name="word">The UInt32 value to convert.</param>
        /// <param name="bytes">An array of bytes to hold the result.</param>
        /// <param name="offset">The offset of the bytes to hold the result within the array.</param>
        static void UInt16ToBytesBigEndian(UInt32 word, Byte[] bytes, int offset)
        {
            bytes[offset + 0] = (byte)(word >> 8);
            bytes[offset + 1] = (byte)word;
        }

        /// <summary>
        /// Big-endian conversion of UInt32 value to bytes.
        /// </summary>
        /// <param name="word">The UInt32 value to convert.</param>
        /// <param name="bytes">An array of bytes to hold the result.</param>
        /// <param name="offset">The offset of the bytes to hold the result within the array.</param>
        static void UInt32ToBytesBigEndian(UInt32 word, Byte[] bytes, int offset)
        {
            bytes[offset + 0] = (byte)(word >> 24);
            bytes[offset + 1] = (byte)(word >> 16);
            bytes[offset + 2] = (byte)(word >> 8);
            bytes[offset + 3] = (byte)word;
        }

        /// <summary>
        /// Little-endian conversion of bytes to UInt32 value. 
        /// </summary>
        /// <param name="bytes">An array of bytes to convert.</param>
        /// <param name="index">The index of bytes to convert within the array.</param>
        /// <returns>The UInt32 value.</returns>
        static UInt32 BytesToUInt32LittleEndian(Byte[] bytes, int index)
        {
            return (UInt32)bytes[index] + ((UInt32)bytes[index + 1] << 8) +
                ((UInt32)bytes[index + 2] << 16) + ((UInt32)bytes[index + 3] << 24);
        }

        /// <summary>
        /// Little-endian conversion of bytes to UInt32 value. 
        /// </summary>
        /// <param name="bytes">An array of bytes to convert.</param>
        /// <param name="index">The index of bytes to convert within the array.</param>
        /// <returns>The UInt16 value.</returns>
        static UInt16 BytesToUInt16LittleEndian(Byte[] bytes, int index)
        {
            return (UInt16)((UInt16)bytes[index] + ((UInt16)bytes[index + 1] << 8));
        }

        /// <summary>
        /// Little-endian conversion of UInt32 value to bytes.
        /// </summary>
        /// <param name="word">The UInt32 value to convert.</param>
        /// <param name="bytes">An array of bytes to hold the result.</param>
        /// <param name="index">The index of the bytes to hold the result within the array.</param>
        static void Uint32ToBytes(UInt32 word, Byte[] bytes, int index)
        {
            bytes[index] = (byte)word;
            bytes[index + 1] = (byte)(word >> 8);
            bytes[index + 2] = (byte)(word >> 16);
            bytes[index + 3] = (byte)(word >> 24);
        }


        #endregion

        #region Encryption  --  HIGHLY CONFIDENTIAL

        /// <summary>
        /// Calculates a 32-bit CRC value of the given bytes.
        ///    
        ///  !!! THE METHODS AND CODES HEREIN ARE HIGHLY CONFIDENTIAL !!!
	    ///
        /// </summary>
        /// <param name="data">The data for which to calculate a CRC.</param>
        /// <param name="numBytesToCRC">The number of bytes to include in the calculation.</param>
        /// <returns></returns>
        private UInt32 getCRC32(Byte[] data, UInt32 numBytesToCRC)
        {
            UInt32 crc = 0xffffffff;
            for (int i = 0; i < numBytesToCRC; i++)
            {
                crc ^= ((UInt32)data[i] << 24);
                for (int j = 0; j < 8; j++)
                    crc = ((crc & 0x80000000) != 0) ? (crc << 1) ^ 0x04C11DB7 : (crc << 1);
            }
            return ~crc;
        }

        /// <summary>
        /// Encrypts a set of data.
        /// 
        ///  NOTICE:
        ///  
        ///  THIS IS A SYMMETRIC KEY SCHEME AND IS NOT NEARLY AS SECURE AS
        ///  A PUBLIC KEY SCHEME SUCH AS RSA OR AES.
        ///    
        ///  !!! THE METHODS AND CODES HEREIN ARE HIGHLY CONFIDENTIAL !!!
	    ///
        /// </summary>
        /// <param name="device">The ECCONet device that will receive the data.</param>
        /// <param name="data">The data to encrypt.</param>
        void Encrypt(ECCONetApi.ECCONetDevice device, byte[] data)
        {
            UInt16 i;
            byte[] convolutedKeys = new byte[16];

            //	get convoluted keys
            for (i = 0; i < 16; ++i)
                convolutedKeys[i] = (byte)((device.guid[i >> 2] ^ 0x90208f7f) >> ((i >> 2) << 3));

            //	for all data bytes
            for (i = 0; i < data.Length; ++i)
                data[i] ^= convolutedKeys[(i ^ (convolutedKeys[(i >> 4) & 0x0f])) & 0x0f];
        }
        #endregion
    }
}



#region UNUSED
#if HAZCAN_PROTOCOL

#region HazCAN protocol programmer

/// <summary>
/// HazCAN data type enum.
/// </summary>
private enum HazCAN_DataType : byte
{
    UpdateApp = 32,
    UpdateConfiguration,
};

/// <summary>
/// An object to pass to the programming background thread.
/// </summary>
private class DeviceFlashImage
{
    /// <summary>
    /// The device to program.
    /// </summary>
    public ECCONetApi.ECCONetDevice device;

    /// <summary>
    /// The flash image.
    /// </summary>
    public byte[] image;
}

/// <summary>
/// Programs a device via the HazCAN prototocl.
/// </summary>
/// <param name="device">The ECCONet device to program.</param>
/// <param name="image">The flash image.</param>
/// <param name="progressCallback">A method to call with progress updates.</param>
/// <param name="completeCallback">A method to call with the update result.</param>
/// <returns>Returns result code.</returns>
public ECCONetApi.FirmwareUpdateResultCodes ProgramDeviceWithImage(ECCONetApi.ECCONetDevice device, byte[] image,
    ECCONetApi.FirmwareUpdateProgressDelegate progressCallback, ECCONetApi.FirmwareUpdateCompleteDelegate completeCallback)
{
    //  if already busy programming, then just return
    if (programmerBusy != 0)
        return ECCONetApi.FirmwareUpdateResultCodes.ProgrammerBusy;

    //  validate inputs
    if ((device == null) || (image == null) || (image.Length == 0))
        return ECCONetApi.FirmwareUpdateResultCodes.BadParams;

    //  validate the CRC
    if (bytesToUInt32(image, image.Length - 4) != getCRC32(image, (uint)(image.Length - 4)))
        return ECCONetApi.FirmwareUpdateResultCodes.InvalidFlashImageCrc;

    lock (programmerLock)
    {
        //  programmer is now busy
        programmerBusy = 1;

        //  set atomic error code
        programmerError = 0;

        //  save callbacks
        updateProgressDelegate = progressCallback;
        updateCompleteDelegate = completeCallback;

        //  start the programming thread
        programmerThread = new Thread(() => ProgramDeviceViaHazCAN(device, image));
    }
    return 0;
}


volatile HazCAN_Command hazcanAck = HazCAN_Command.UpdateWait;
private void ProgramDeviceViaHazCAN(ECCONetApi.ECCONetDevice device, byte[] image)
{

    //UInt32 canAddress = bytesToUInt32(bin, bin.Length - 4 - HCNI_SIZE + hcni_offset_hcid);
    UInt32 updateAddress = bytesToUInt32(image, image.Length - 4 - HCNI_SIZE + hcni_offset_load_addr);
    UInt32 updateSize = (uint)(image.Length - 4);
    UInt32 updateKey = bytesToUInt32(image, image.Length - 4 - HCNI_SIZE + hcni_offset_crc32);

    /*
    //  check for bootloader boot compliance
    if (sendUpdateCommand(canAddress | 0x00F, 3, new Byte[] { (byte)HazCAN_Command.UpdateCheck }))
    {
        canAddress |= 0x00F;
    }
    //  else check for app boot compliance
    */

    //  send update check
    if (!sendUpdateCommand(device.address, 3, new Byte[] { (byte)HazCAN_Command.UpdateCheck }))
    {
        updateCompleteDelegate?.Invoke(this, device, ECCONetApi.FirmwareUpdateResultCodes.DeviceDidNotAckCommand);
        return;
    }

    //  try three times to request update start
    int tries = 0;
    for (; tries < 3; ++tries)
    {
        if (sendUpdateCommand(device.address, 3, new Byte[] { (byte)HazCAN_Command.UpdateBegin }))
            break;
    }
    if (tries >= 3)
    {
        updateCompleteDelegate?.Invoke(this, device, ECCONetApi.FirmwareUpdateResultCodes.DeviceDidNotAckCommand);
        return;
    }

    //  send update address
    if (!sendUpdateCommand(device.address, 3, new Byte[]
        {
                    (byte)HazCAN_Command.UpdateLoad,
                    (byte)updateAddress, (byte)(updateAddress >> 8), (byte)(updateAddress >> 16), (byte)(updateAddress >> 24)
        }))
    {
        updateCompleteDelegate?.Invoke(this, device, ECCONetApi.FirmwareUpdateResultCodes.DeviceDidNotAckCommand);
        return;
    }

    //  send update length
    if (!sendUpdateCommand(device.address, 3, new Byte[]
        {
                    (byte)HazCAN_Command.UpdateSize,
                    (byte)updateSize, (byte)(updateSize >> 8), (byte)(updateSize >> 16), (byte)(updateSize >> 24)
        }))
    {
        updateCompleteDelegate?.Invoke(this, device, ECCONetApi.FirmwareUpdateResultCodes.DeviceDidNotAckCommand);
        return;
    }

    //  send update key
    if (!sendUpdateCommand(device.address, 3, new Byte[]
    {
                (byte)HazCAN_Command.UpdateKey,
                (byte)updateKey, (byte)(updateKey >> 8), (byte)(updateKey >> 16), (byte)(updateKey >> 24)
    }))
    {
        updateCompleteDelegate?.Invoke(this, device, ECCONetApi.FirmwareUpdateResultCodes.DeviceDidNotAckCommand);
        return;
    }

    //  if device reported error
    if (programmerError != 0)
    {
        updateCompleteDelegate?.Invoke(this, device, ECCONetApi.FirmwareUpdateResultCodes.DeviceReportedError);
        return;
    }

    //  send update data
    uint numMessages = updateSize / 512;
    int binIndex = 0;
    for (int i = 0; i < numMessages; i++)
    {
        //  send 512 bytes of data
        hazcanAck = HazCAN_Command.UpdateWait;
        Byte[] data = new Byte[513];
        data[0] = (byte)HazCAN_Command.UpdateData;
        Buffer.BlockCopy(image, binIndex, data, 1, 512);
        binIndex += 512;
        sendIsoTpMessage(new IsoTpMessage(HCTP_UPDATE, device.address, data));

        //  wait for ACK...
        waitForHazCAN_Ack(3);
        if (hazcanAck != HazCAN_Command.UpdateOK)
        {
            updateCompleteDelegate?.Invoke(this, device, ECCONetApi.FirmwareUpdateResultCodes.DeviceDidNotAckDataBlock);
            return;
        }

        //  if device reported error
        if (programmerError != 0)
        {
            updateCompleteDelegate?.Invoke(this, device, ECCONetApi.FirmwareUpdateResultCodes.DeviceReportedError);
            return;
        }

        //  send update
        updateProgressDelegate?.Invoke(this, device, ((i + 1) * 100) / 56);
    }

    //  send update end
    sendIsoTpMessage(new IsoTpMessage(HCTP_UPDATE, device.address, new Byte[1] { (byte)HazCAN_Command.UpdateEnd }));

    //  notify completed
    updateCompleteDelegate?.Invoke(this, device, ECCONetApi.FirmwareUpdateResultCodes.ProgrammingCompleted);
}

//  we send message and return true if get positive ACK before timeout
bool sendUpdateCommand(UInt32 canAddress, UInt32 timeout, Byte[] data)
{
    //  Send boot-compliant check message
    hazcanAck = HazCAN_Command.UpdateWait;
    sendIsoTpMessage(new IsoTpMessage(HCTP_UPDATE, canAddress, data));
    return (waitForHazCAN_Ack(timeout) == HazCAN_Command.UpdateOK);
}

/// <summary>
/// Wait for Ack.
/// </summary>
/// <param name="timeoutSeconds">The ack timeout in seconds.</param>
/// <returns></returns>
HazCAN_Command waitForHazCAN_Ack(UInt32 timeoutSeconds)
{
    DateTime timeout = DateTime.Now.AddSeconds(timeoutSeconds);
    Thread.Sleep(10);

    while (hazcanAck == HazCAN_Command.UpdateWait)
    {
        if (timeout.CompareTo(DateTime.Now) < 0)
        {
            hazcanAck = HazCAN_Command.UpdateTimeout;
            break;
        }
    }
    return hazcanAck;
}

/// <summary>
/// Calculates the HazCAN CRC value of the given bytes.
/// </summary>
/// <param name="data"></param>
/// <param name="numBytesToCRC"></param>
/// <returns></returns>
private UInt32 getCRC32(Byte[] data, UInt32 numBytesToCRC)
{
    UInt32 crc = 0xffffffff;
    for (int i = 0; i < numBytesToCRC; i++)
    {
        crc ^= ((UInt32)data[i] << 24);
        for (int j = 0; j < 8; j++)
            crc = ((crc & 0x80000000) != 0) ? (crc << 1) ^ 0x04C11DB7 : (crc << 1);
    }
    return ~crc;
}

/// <summary>
/// Little-endian conversion of bytes to UInt32 value. 
/// </summary>
/// <param name="bytes">An array of bytes to convert.</param>
/// <param name="index">The index of bytes to convert within the array.</param>
/// <returns>The UInt32 value.</returns>
static UInt32 bytesToUInt32(Byte[] bytes, int index)
{
    return (UInt32)bytes[index] + ((UInt32)bytes[index + 1] << 8) +
        ((UInt32)bytes[index + 2] << 16) + ((UInt32)bytes[index + 3] << 24);
}

/// <summary>
/// Little-endian conversion of UInt32 value to bytes.
/// </summary>
/// <param name="word">The UInt32 value to convert.</param>
/// <param name="bytes">An array of bytes to hold the result.</param>
/// <param name="index">The index of the bytes to hold the result within the array.</param>
static void uint32ToBytes(UInt32 word, Byte[] bytes, int index)
{
    bytes[index] = (byte)word;
    bytes[index + 1] = (byte)(word >> 8);
    bytes[index + 2] = (byte)(word >> 16);
    bytes[index + 3] = (byte)(word >> 24);
}

#endregion

#endif


#if PREVIOUS_CODE


        /// <summary>
        /// Processes a received ULC token.
        /// </summary>
        /// <param name="id">The CAN frame ID.</param>
        /// <param name="data">The CAN frame data.</param>
        void processReceivedUlcToken(UInt32 id, byte[] data)
        {
            //  all ULC tokens are 7 payload bytes
            if ((data[0] & 0x0f) != 7)
                return;

            //  get device enumeration
            UInt32 senderAddress = (id >> 12) & 0x0fff;

            int esgDeviceEnum = 4;
            while (--esgDeviceEnum >= 0)
                if (senderAddress == EsgDeviceAddress[esgDeviceEnum])
                    break;
            if (esgDeviceEnum == -1)
                return;

            //  get ULC and value
            ULC ulc = (ULC)(data[2] | ((UInt16)(data[3]) << 8));
            UInt32 value = (UInt32)(data[4] | (data[5] << 8) | (data[6] << 16) | (data[7] << 24));

            //  switch on ULC
            switch (ulc)
            {
                case ULC.SP_FIRMWARE_REV:
                    UInt32 revision = value;
                    bool boot = ((revision & 0x80000000) != 0);
                    revision &= ~0x80000000;
                    EsgDeviceFirmwareRevisions[esgDeviceEnum] = revision;
                    String revNum = String.Format("{0}{1}.{2}{3}", ((revision >> 12) & 0x0f), ((revision >> 8) & 0x0f),
                        ((revision >> 4) & 0x0f), (revision & 0x0f));
                    if (revNum.StartsWith("0"))
                        revNum = revNum.Substring(1);
                    String revString = (boot ? "Boot Revision: " : "App Revision: ") + revNum;
                    revString = revString.Insert(0, "◘ " + EsgDeviceNames[esgDeviceEnum] + ": ");
                    ESGFirmwareLabels[esgDeviceEnum].Text = revString;
                    ESGFirmwareLabels[esgDeviceEnum].Enabled = true;
                    break;

                case ULC.SP_APP_CRC:
                    UInt32 CRC = value;
                    String crcString = "App CRC: " + CRC.ToString("X8");
                    ESGCRCLabels[esgDeviceEnum].Text = crcString;
                    ESGCRCLabels[esgDeviceEnum].Enabled = true;
                    break;

                case ULC.SP_UID0:
                case ULC.SP_UID1:
                case ULC.SP_UID2:
                case ULC.SP_UID3:
                    UInt16 index = (UInt16)(ulc - ULC.SP_UID0);
                    UInt32 serialNumber = value;
                    EsgSerialNumbers[esgDeviceEnum][index] = serialNumber;
                    if (index == 3)
                    {
                        String snString = "Serial Number: " +
                            EsgSerialNumbers[esgDeviceEnum][3].ToString("X8") +
                            EsgSerialNumbers[esgDeviceEnum][2].ToString("X8") +
                            EsgSerialNumbers[esgDeviceEnum][1].ToString("X8") +
                            EsgSerialNumbers[esgDeviceEnum][0].ToString("X8");
                        ESGSerialNumberLabels[esgDeviceEnum].Text = snString;
                        ESGSerialNumberLabels[esgDeviceEnum].Enabled = true;
                    }
                    break;

                case ULC.STOP:
                case ULC.LEFT_TURN:
                case ULC.RIGHT_TURN:
                case ULC.TAIL:
                    //String str = String.Format("ULC: {0} Value: {1}", ulc.ToString(), value);
                    //IncludeTextMessage(str);
                    break;

                case ULC.BITFIELD_ON:
                case ULC.BITFIELD_OFF:
                    //String str = String.Format("ULC: {0} Value: {1}", ulc.ToString(), value);
                    //IncludeTextMessage(str);
                    break;

                case ULC.CC_INDEXED_PATTERN:
                case ULC.LB_INDEXED_PATTERN:
                    byte v = (byte)(value & 0xff);
                    byte idx = (byte)(value >> 8);
                    String s = String.Format("ULC: {0} Index: {1} Value: {2}", ulc.ToString(), idx, v);
                    IncludeTextMessage(s);
                    break;

                default:
                    String str = String.Format("ULC: {0} Value: {1}", ulc.ToString(), value);
                    IncludeTextMessage(str);
                    break;
            }
        }


#region Hex file parser

        private enum HexFileRecordType
        {
            Data,
            EndOfFile,
            ExtendedSegmentAddress,
            StartSegmentAddress,
            ExtendedLinearAddress,
            StartLinearAddress
        };



        Byte[] parseHexFile(string filePath, Byte fillValue)
        {
            //  We create bin file. 
            Byte[] bin = new byte[0x8000];
            for (int i = 0; i < 0x8000; i++)
                bin[i] = fillValue;


            Byte byteCount;
            UInt16 lowerAddress;
            UInt16 upperAddress = 0;
            Byte recordType;
            Byte checksum;
            Byte hexByte;
            foreach (string s in File.ReadAllLines(filePath))
            {
                if ((s.Length < 11) || (s[0] != ':'))
                    goto error;
                if (!Byte.TryParse(s.Substring(1, 2), System.Globalization.NumberStyles.HexNumber,
                    CultureInfo.CurrentCulture, out byteCount))
                    goto error;
                if (!UInt16.TryParse(s.Substring(3, 4), System.Globalization.NumberStyles.HexNumber,
                    CultureInfo.CurrentCulture, out lowerAddress))
                    goto error;
                if (!Byte.TryParse(s.Substring(7, 2), System.Globalization.NumberStyles.HexNumber,
                    CultureInfo.CurrentCulture, out recordType))
                    goto error;
                if (!Byte.TryParse(s.Substring(9 + (byteCount * 2), 2), System.Globalization.NumberStyles.HexNumber,
                    CultureInfo.CurrentCulture, out checksum))
                    goto error;


                switch ((HexFileRecordType)recordType)
                {
                    case HexFileRecordType.Data:
                        {
                            UInt32 address = (UInt32)((upperAddress << 16) | lowerAddress);
                            for (int i = 0; i < byteCount; i++, address++)
                            {
                                if (!Byte.TryParse(s.Substring(9 + (i * 2), 2), System.Globalization.NumberStyles.HexNumber,
                                    CultureInfo.CurrentCulture, out hexByte))
                                    goto error;
                                bin[address] = hexByte;
                            }
                            lowerAddress = (UInt16)address;
                            break;
                        }

                    case HexFileRecordType.EndOfFile:
                        break;

                    case HexFileRecordType.ExtendedSegmentAddress:
                        break;

                    case HexFileRecordType.StartSegmentAddress:
                        break;

                    case HexFileRecordType.ExtendedLinearAddress:
                        if (!UInt16.TryParse(s.Substring(9, 4), System.Globalization.NumberStyles.HexNumber,
                            CultureInfo.CurrentCulture, out upperAddress))
                            goto error;
                        break;

                    case HexFileRecordType.StartLinearAddress:
                        break;
                }
            }
            return bin;

            error:
            MessageBox.Show("Error parsing hex file.", "File Error");
            return null;
        }
#endregion


        //  we send message and return true if get positive ACK before timeout
        bool sendUpdateCommand(UInt32 canAddress, UInt32 timeout, Byte[] data)
        {
            //  Send boot-compliant check message
            FileWriteFixedSegmentAck = HazCAN_Command.UpdateWait;
            sendIsoTpMessage(new IsoTpMessage(HCTP_UPDATE, canAddress, data));
            return (WaitForAck(timeout) == HazCAN_Command.UpdateOK);
        }

        /// <summary>
        /// Wait for Ack.
        /// </summary>
        /// <param name="timeoutSeconds">The ack timeout in seconds.</param>
        /// <returns></returns>
        BootloaderStatusCodes WaitForAck(UInt32 timeoutSeconds)
        {
            DateTime timeout = DateTime.Now.AddSeconds(timeoutSeconds);
            Thread.Sleep(10);

            while (FileWriteFixedSegmentAck == BootloaderStatusCodes.Waiting)
            {
                if (timeout.CompareTo(DateTime.Now) < 0)
                {
                    FileWriteFixedSegmentAck = BootloaderStatusCodes.NoResponse;
                    break;
                }
            }
            return FileWriteFixedSegmentAck;
        }

        /// <summary>
        /// An object to pass to the programming background thread.
        /// </summary>
        private class DeviceFlashImage
        {
            /// <summary>
            /// The device to program.
            /// </summary>
            public ECCONetApi.ECCONetDevice device;

            /// <summary>
            /// The flash image.
            /// </summary>
            public byte[] image;
        }



#region HazCAN definitions

        bool HazCAN_v1_5 = true;

        // ML updated for second round, August, 2015.
        /*
        const Byte  HCMD_UpdateCheck   0xB0         // check that the target node supports updating
        const Byte  HCMD_UpdateBegin   0xB1         // put target into update mode
        const Byte  HCMD_UpdateLoad    0xB2         // provide update data load address
        const Byte  HCMD_UpdateSize    0xB3         // provide update data size
        const Byte  HCMD_UpdateKey     0xB4         // provide update data cipher key
        const Byte  HCMD_UpdateData    0xB5         // a block of 512 bytes of update data, repeated until all data sent
        const Byte  HCMD_UpdateEnd     0xB6         // end of update, non-involved nodes return to normal ops, target reboots
        const Byte  HACK_UpdateOK      0xBE         // returned by target to acknowledge update commands
        const Byte  HACK_UpdateERR     0xBF         // returned by target to indicate an error has occurred
        */

        private enum HazCAN_Command : uint
        {
            ULC_Message = 0x20,
            UpdateCheck = 0xB0,
            UpdateBegin,
            UpdateLoad,
            UpdateSize,
            UpdateKey,
            UpdateData,
            UpdateEnd,
            UpdateOK = 0xBE,
            UpdateError,
            UpdateWait = 0x100,     // internal PC program use
            UpdateTimeout,          // internal PC program use
        };

        // HazCAN message type/priority codes (upper 5-bits of CAN ID) (odd numbers reserved for response messages)
        // ML revised list and updated descriptions on Aug 2015 for recent HazCAN changes
        const Byte HCTP_UPDATE = 0x04;		// ISO_TP firmware or configuration update - non-involved nodes go quiet
        const Byte HCTP_CMD = 0x10;		// ISO_TP HazCAN command
        const Byte HCTP_CMDRSP = 0x11;		// ISO_TP HazCAN command response
        const Byte HCTP_STATUS = 0x14;		// NAKED
        const Byte HCTP_STATUS1 = 0x15;		// NAKED
        const Byte HCTP_STATUS2 = 0x16;		// NAKED
        const Byte HCTP_STATUS3 = 0x17;		// NAKED

        // HazCAN hardware node IDs - lo-nybble is sub-id (default = 0), allows for 16 of each device type on bus!
        const UInt16 HCID_BROADCAST = 0x000;		// ML added for JBox v1.50 
        const UInt16 HCID_PCAN = 0x100;		// m5-pc-interface, see: www.peak-system.com/PCAN-USB.199.0.html
        const UInt16 HCID_MTC = 0x200;		// m5-touch controller
        const UInt16 HCID_MATRIX = 0x210;		// m5-matrix display
        const UInt16 HCID_JBOX = 0x220;		// m5-jbox interface
        const UInt16 HCID_LBAR = 0x300;		// m5-lightbar
        const UInt16 HCID_14DRV = 0xE00;		// ESG Axios driver board
        const UInt16 HCID_14KEY = 0xE10;		// ESG Axios keypad
        const UInt16 HCID_14DIR = 0xE20;		// ESG Safety Director
        const UInt16 HCID_EAGLET = 0xE30;		// ESG Eaglet driver board
        const UInt16 HCID_BTLDR = 0xE40;        // ESG bootloader


        // HazCAN update data types
        const Byte DTYPE_UPDATE_APP = 32;			// datatype for updating main application (as opposed to settings, messages etc.)
        const Byte DTYPE_UPDATE_CFG = 33;			// configuration update (led patterns etc.)


        //  HazCAN node/datafile information block (resides at end of firmware/config files and top of flash in device)
        //  ML: Kevin revised this August, 2015.
        //  Below are the offsets.
        const int HCNI_SIZE = 64;
        const int hcni_offset_hcd_key = 0;      // 08		"HCDKEY::"		searchable HCD key
        const int hcni_offset_hcd_des = 8;      // 24		"M5-JBOX CONFIG DATA     ", "M5-JBOX FIRMWARE        ", "M5-JBOX APP & CONFIG    "
        const int hcni_offset_hcid = 32;        // 04		CAN address
        const int hcni_offset_load_addr = 36;   // 04		flash start address
        const int hcni_offset_vers_data = 40;   // 02		hex firmware version (1234 = v12.34)
        const int hcni_offset_vers_hard = 42;   // 02		hardware version
        const int hcni_offset_vers_comp = 44;   // 02		compatilibility version
        const int hcni_offset_pad1 = 46;        // 02		0000
        const int hcni_offset_pad2 = 48;        // 04		00000000
        const int hcni_offset_pad3 = 52;        // 04		00000000
        const int hcni_offset_pad4 = 56;        // 04		00000000
        const int hcni_offset_crc32 = 60;		// 04		XXXXXXXX			crc of plaindata

#endregion

#region ESG ULC and ISO-TP definitions and classes

        //	stop, tail and turn lights
        const ushort ULC_STT_START = 0;
        const ushort ULC_STT_END = 3;

        //	light code range
        const ushort ULC_LIGHT_START = 4;
        const ushort ULC_LIGHT_END = 9999;

        //	indicator light code range
        const ushort ULC_IND_LIGHT_START = 10000;
        const ushort ULC_IND_LIGHT_END = 10499;

        //	indicator pattern code range
        const ushort ULC_IND_PATTERN_START = 10500;
        const ushort ULC_IND_PATTERN_END = 10999;

        //	Safety Director pattern code range
        const ushort ULC_SD_PATTERN_START = 11000;
        const ushort ULC_SD_PATTERN_END = 11999;

        //	12-Series lightbar pattern code range
        const ushort ULC_12S_PATTERN_START = 12000;
        const ushort ULC_12S_PATTERN_END = 12999;

        //	14-Series lightbar pattern code range
        const ushort ULC_QUAD_PATTERN_START = 14000;
        const ushort ULC_QUAD_PATTERN_END = 14999;

        //	button control code range
        const ushort ULC_CONTROL_START = 20000;
        const ushort ULC_CONTROL_END = 20999;

        //	system properties code range
        const ushort ULC_SYSTEM_PROPERTY_START = 21000;
        const ushort ULC_SYSTEM_PROPERTY_END = 21999;

        //	the ESG universal light codes
        private enum ULC : ushort
        {
            //	Stop, tail and turn lights start at 0
            STOP = ULC_STT_START,
            TAIL,
            LEFT_TURN,
            RIGHT_TURN,

            //	Lights start at 4
            BITFIELD_OFF = ULC_LIGHT_START,
            BITFIELD_ON,
            ROTATING_BEACON,
            QUADRANT_1,
            QUADRANT_2,
            QUADRANT_3,
            QUADRANT_4,
            SAFETY_DIRECTOR_1,
            SAFETY_DIRECTOR_2,
            SAFETY_DIRECTOR_3,
            SAFETY_DIRECTOR_4,
            SAFETY_DIRECTOR_5,
            SAFETY_DIRECTOR_6,
            SAFETY_DIRECTOR_7,
            SAFETY_DIRECTOR_8,
            TAKEDOWN,
            WORKLIGHT,
            ALLEY_LEFT,
            ALLEY_RIGHT,
            AUX_OUTPUT,
            SYNC_OUTPUT,
            HEAD_1,
            HEAD_2,
            HEAD_3,
            HEAD_4,
            HEAD_5,
            HEAD_6,
            HEAD_7,
            HEAD_8,
            HEAD_9,
            HEAD_10,
            HEAD_11,
            HEAD_12,
            HEAD_13,
            HEAD_14,
            HEAD_15,
            HEAD_16,
            HEAD_17,
            HEAD_18,
            HEAD_19,
            HEAD_20,
            HEAD_21,
            HEAD_22,
            HEAD_23,
            HEAD_24,
            HEAD_25,
            HEAD_26,
            HEAD_27,
            HEAD_28,

            //	Indicator lights for keypads and other control devices start at 10000
            IND_BACKLIGHT = ULC_IND_LIGHT_START,
            IND_AUX,
            IND_CRUISE,
            IND_DIMMED,
            IND_PATTERN_1,
            IND_PATTERN_2,
            IND_TAKEDOWN,
            IND_WORKLIGHT,
            IND_ALLEY_LEFT,
            IND_ALLEY_RIGHT,

            //	Indicator light patterns start at 10500
            IND_INDEXED_PATTERN = ULC_IND_PATTERN_START,
            IND_Stop,
            IND_Beacon_Flash,
            IND_Beacon_Rotate,
            IND_Reg65_Only,
            IND_All_Patterns,

            //	Safety Director patterns start at 11000
            SD_INDEXED_PATTERN = ULC_SD_PATTERN_START,
            SD_Stop,
            SD_Left,
            SD_Left_Solid,
            SD_Right,
            SD_Right_Solid,
            SD_Center_Out,
            SD_Center_Out_Solid,
            SD_Wig_Wag,
            SD_Alternating,
            SD_Quad_Flash,
            SD_Alternating_Center_Pulse,
            SD_Quad_Flash_Center_Pulse,
            SD_LeftFillAndChase,
            SD_RightFillAndChase,
            SD_CenterFillAndChase,


            //	14-Series patterns start at 14000
            LB_INDEXED_PATTERN = ULC_QUAD_PATTERN_START,
            LB_Stop,
            LB_Steady_PWM_50,
            LB_Double_75_FPM,
            LB_Title_13_Quad_65_FPM,
            LB_Title_13_Double_65_FPM,
            LB_Quint_Hold_75_FPM,
            LB_Pulse_8_Burst_75_FPM,
            LB_Reg_65_Single_120_FPM,
            LB_Reg_65_Double_120_FPM,
            LB_Reg_65_Triple_120_FPM,
            LB_Reg_65_Quad_120_FPM,
            LB_Reg_65_Burst_120_FPM,
            LB_Reg_65_Single_S_S_120_FPM,
            LB_Reg_65_Double_S_S_120_FPM,
            LB_Reg_65_Triple_S_S_120_FPM,
            LB_Reg_65_Quad_S_S_120_FPM,
            LB_Reg_65_Burst_S_S_120_FPM,
            LB_Quad_Alternate_S_S_150_FPM,
            LB_Quad_Cross_Alternate_150_FPM,
            LB_Double_Alternate_S_S_150_FPM,
            LB_Double_Cross_Alternate_150_FPM,
            LB_Quint_Hold_Alternate_S_S_150_FPM,
            LB_Quint_Hold_Cross_Alternate,
            LB_Quad_Alternate_S_S_150_FPM_Front,
            LB_Quad_Alternate_S_S_150_FPM_Rear,
            LB_Double_Alternate_S_S_150_FPM_Front,
            LB_Double_Alternate_S_S_150_FPM_Rear,
            LB_Quint_Hold_Alternate_Side_to_Side_Front,
            LB_Quint_Hold_Alternate_Side_to_Side_Rear,
            LB_CycleAll,

            //	new cp
            LB_Reg_65_Single_S_S_120_FPM_Center_Pulse,
            LB_Reg_65_Double_S_S_120_FPM_Center_Pulse,
            LB_Reg_65_Triple_S_S_120_FPM_Center_Pulse,
            LB_Reg_65_Quad_S_S_120_FPM_Center_Pulse,
            LB_Reg_65_Burst_S_S_120_FPM_Center_Pulse,
            LB_Quad_Alternate_S_S_150_FPM_Center_Pulse,
            LB_Double_Alternate_S_S_150_FPM_Center_Pulse,
            LB_Quint_Hold_Alternate_S_S_150_FPM_Center_Pulse,
            LB_Quad_Alternate_S_S_150_FPM_Front_Center_Pulse,
            LB_Quad_Alternate_S_S_150_FPM_Rear_Center_Pulse,
            LB_Double_Alternate_S_S_150_FPM_Front_Center_Pulse,
            LB_Double_Alternate_S_S_150_FPM_Rear_Center_Pulse,
            LB_Quint_Hold_Alternate_Side_to_Side_Front_Center_Pulse,
            LB_Quint_Hold_Alternate_Side_to_Side_Rear_Center_Pulse,
            LB_Quad_Alternate_S_S_Middle_75_FPM,
            LB_Quad_Alternate_S_S_Middle_75_FPM_Center_Pulse,
            LB_Fast_Rotate,
            LB_Wave_Rotate,
            LB_Rotate_Quad,
            LB_Fast_Quad,
            LB_Random,

            //	Button control codes start at 20000
            CC_INDEXED_PATTERN = ULC_CONTROL_START,
            CC_ON_OFF,
            CC_TGL_AUX_OUTPUT,
            CC_TGL_TAKEDOWN,
            CC_TGL_WORKLIGHT,
            CC_TGL_ALLEY_LEFT,
            CC_TGL_ALLEY_RIGHT,
            CC_TGL_CRUISE,
            CC_TGL_NIGHT_MODE,
            CC_ALLEY_COMBO,
            CC_SAFETY_DIR_COMBO,

            CC_QUAD_PRESET,
            CC_QUAD_PATTERN_SEL,

            CC_SAFETY_DIR_PRESET,
            CC_SAFETY_DIR_PATTERN_SEL,
            CC_SAFETY_DIR_FRONT_BACK,
            CC_SAFETY_DIR_NUM_LIGHTS,

            CC_PRESET_1,
            CC_PRESET_2,
            CC_ALLEY_LEFT,
            CC_ALLEY_RIGHT,
            CC_TAKEDOWN,
            CC_WORKLIGHT,
            CC_CRUISE,
            CC_SYNC,
            CC_AUX_OUTPUT,
            CC_KEYPAD_SHOW_QUAD_SEQ,
            CC_KEYPAD_SHOW_SAFETY_DIR_SEQ,
            CC_SET_ALL_QUADRANTS,

            CC_SD_LEFT_SOLID,
            CC_SD_RIGHT_SOLID,
            CC_SD_CENTER_OUT_SOLID,
            CC_SD_WIG_WAG,
            ULC_CC_WORKLIGHT_TAKEDOWN_COMBO,

            //	System property codes start at 21000
            SP_REINVOKE_ISP = ULC_SYSTEM_PROPERTY_START,
            SP_SET_SEND_INTERNAL_MESSAGES,
            SP_GET_FIRMWARE_REV,
            SP_FIRMWARE_REV,
            SP_ERASE_FLASH,
            SP_GET_UID,
            SP_UID0,
            SP_UID1,
            SP_UID2,
            SP_UID3,
            SP_GET_APP_CRC,
            SP_APP_CRC,
            SP_ERASE_APP,

            //	reserved ULC_NONE
            NONE = UInt16.MaxValue
        };

        //  We create a lookup table to match the dropdown.
        //
        private ULC[] ULCDropwdownLookupTable =
        {
            ULC.QUADRANT_1,
            ULC.QUADRANT_2,
            ULC.QUADRANT_3,
            ULC.QUADRANT_4,
            ULC.TAKEDOWN,
            ULC.WORKLIGHT,
            ULC.ALLEY_LEFT,
            ULC.ALLEY_RIGHT,
            ULC.AUX_OUTPUT,
            ULC.SYNC_OUTPUT,
            ULC.HEAD_1,
            ULC.HEAD_2,
            ULC.HEAD_3,
            ULC.HEAD_4,
            ULC.HEAD_5,
            ULC.HEAD_6,
            ULC.HEAD_7,
            ULC.HEAD_8,
            ULC.HEAD_9,
            ULC.HEAD_10,
            ULC.HEAD_11,
            ULC.HEAD_12,
            ULC.HEAD_13,
            ULC.HEAD_14,
            ULC.HEAD_15,
            ULC.HEAD_16,
            ULC.HEAD_17,
            ULC.HEAD_18,
            ULC.HEAD_19,
            ULC.HEAD_20,
            ULC.HEAD_21,
            ULC.HEAD_22,
            ULC.HEAD_23,
            ULC.HEAD_24,
            ULC.HEAD_25,
            ULC.HEAD_26,
            ULC.HEAD_27,
            ULC.HEAD_28,
            ULC.LB_INDEXED_PATTERN,
            ULC.LB_Stop,
            ULC.LB_Steady_PWM_50,
            ULC.LB_Double_75_FPM,
            ULC.LB_Title_13_Quad_65_FPM,
            ULC.LB_Title_13_Double_65_FPM,
            ULC.LB_Quint_Hold_75_FPM,
            ULC.LB_Pulse_8_Burst_75_FPM,
            ULC.LB_Reg_65_Single_120_FPM,
            ULC.LB_Reg_65_Double_120_FPM,
            ULC.LB_Reg_65_Triple_120_FPM,
            ULC.LB_Reg_65_Quad_120_FPM,
            ULC.LB_Reg_65_Burst_120_FPM,
            ULC.LB_Reg_65_Single_S_S_120_FPM,
            ULC.LB_Reg_65_Double_S_S_120_FPM,
            ULC.LB_Reg_65_Triple_S_S_120_FPM,
            ULC.LB_Reg_65_Quad_S_S_120_FPM,
            ULC.LB_Reg_65_Burst_S_S_120_FPM,
            ULC.LB_Quad_Alternate_S_S_150_FPM,
            ULC.LB_Quad_Cross_Alternate_150_FPM,
            ULC.LB_Double_Alternate_S_S_150_FPM,
            ULC.LB_Double_Cross_Alternate_150_FPM,
            ULC.LB_Quint_Hold_Alternate_S_S_150_FPM,
            ULC.LB_Quint_Hold_Cross_Alternate,
            ULC.LB_Quad_Alternate_S_S_150_FPM_Front,
            ULC.LB_Quad_Alternate_S_S_150_FPM_Rear,
            ULC.LB_Double_Alternate_S_S_150_FPM_Front,
            ULC.LB_Double_Alternate_S_S_150_FPM_Rear,
            ULC.LB_Quint_Hold_Alternate_Side_to_Side_Front,
            ULC.LB_Quint_Hold_Alternate_Side_to_Side_Rear,
            ULC.LB_CycleAll,
            ULC.LB_Reg_65_Single_S_S_120_FPM_Center_Pulse,
            ULC.LB_Reg_65_Double_S_S_120_FPM_Center_Pulse,
            ULC.LB_Reg_65_Triple_S_S_120_FPM_Center_Pulse,
            ULC.LB_Reg_65_Quad_S_S_120_FPM_Center_Pulse,
            ULC.LB_Reg_65_Burst_S_S_120_FPM_Center_Pulse,
            ULC.LB_Quad_Alternate_S_S_150_FPM_Center_Pulse,
            ULC.LB_Double_Alternate_S_S_150_FPM_Center_Pulse,
            ULC.LB_Quint_Hold_Alternate_S_S_150_FPM_Center_Pulse,
            ULC.LB_Quad_Alternate_S_S_150_FPM_Front_Center_Pulse,
            ULC.LB_Quad_Alternate_S_S_150_FPM_Rear_Center_Pulse,
            ULC.LB_Double_Alternate_S_S_150_FPM_Front_Center_Pulse,
            ULC.LB_Double_Alternate_S_S_150_FPM_Rear_Center_Pulse,
            ULC.LB_Quint_Hold_Alternate_Side_to_Side_Front_Center_Pulse,
            ULC.LB_Quint_Hold_Alternate_Side_to_Side_Rear_Center_Pulse,
            ULC.LB_Quad_Alternate_S_S_Middle_75_FPM,
            ULC.LB_Quad_Alternate_S_S_Middle_75_FPM_Center_Pulse,
            ULC.LB_Fast_Rotate,
            ULC.LB_Wave_Rotate,
            ULC.LB_Rotate_Quad,
            ULC.LB_Fast_Quad,
            ULC.LB_Random,
            ULC.SAFETY_DIRECTOR_1,
            ULC.SAFETY_DIRECTOR_2,
            ULC.SAFETY_DIRECTOR_3,
            ULC.SAFETY_DIRECTOR_4,
            ULC.SAFETY_DIRECTOR_5,
            ULC.SAFETY_DIRECTOR_6,
            ULC.SAFETY_DIRECTOR_7,
            ULC.SAFETY_DIRECTOR_8,
            ULC.SD_INDEXED_PATTERN,
            ULC.SD_Stop,
            ULC.SD_Left,
            ULC.SD_Left_Solid,
            ULC.SD_Right,
            ULC.SD_Right_Solid,
            ULC.SD_Center_Out,
            ULC.SD_Center_Out_Solid,
            ULC.SD_Wig_Wag,
            ULC.SD_Alternating,
            ULC.SD_Quad_Flash,
            ULC.SD_Alternating_Center_Pulse,
            ULC.SD_Quad_Flash_Center_Pulse,
            ULC.SD_LeftFillAndChase,
            ULC.SD_RightFillAndChase,
            ULC.SD_CenterFillAndChase,
        };

        private enum IsoTpFrameType : uint
        {
            Single,
            First,
            Consecutive,
            Flow
        };

        private enum IsoTpFlowType : uint
        {
            ClearToSend,
            Wait,
            Overflow_Abort,
        };


        private class IsoTpMessage
        {
            public Byte messageType;
            public UInt32 canAddress;
            public Byte[] data;

            public IsoTpMessage(Byte _messageType, UInt32 _canAddress, Byte[] _data)
            {
                messageType = _messageType;
                canAddress = _canAddress;
                data = _data;
            }
        }
#endregion

#region CAN frame and ISO-TP message handling

        /// <summary>
        /// Processes a received CAN frame. 
        /// </summary>
        /// <param name="id">The CAN frame ID.</param>
        /// <param name="data">The CAN frame data.</param>
        private void processReceivedCanFrame(UInt32 id, byte[] data)
        {
            //  get HazCAN message type based on 5 ms id bits
            Byte messageType = (byte)((uint)(id & 0x1f000000) >> 24);

            //  if an ISO-TP frame
            if ((messageType == HCTP_UPDATE) || (messageType == HCTP_CMD) || (messageType == HCTP_CMDRSP))
            {
                //  switch on ISO-TP frame type
                IsoTpFrameType frameType;
                frameType = (IsoTpFrameType)((data[0] >> 4) & 0x0f);

                switch (frameType)
                {
                    case IsoTpFrameType.Single:
                        switch (data[1])
                        {
                            case (byte)HazCAN_Command.ULC_Message:
                                //processReceivedUlcToken(id, data);  no longer supported here
                                break;

                            case (byte)HazCAN_Command.UpdateOK:
                                FileWriteFixedSegmentAck = HazCAN_Command.UpdateOK;
                                break;

                            case (byte)HazCAN_Command.UpdateError:
                                FileWriteFixedSegmentAck = HazCAN_Command.UpdateError;
                                programmerError = 1;
                                break;
                        }
                        break;

                    case IsoTpFrameType.First:
                        //  should not receive this type
                        break;

                    case IsoTpFrameType.Consecutive:
                        //  should not receive this type
                        break;

                    case IsoTpFrameType.Flow:
                        if (HazCAN_v1_5)
                            flowControl = (IsoTpFlowType)(data[0] & 0x0f);
                        else
                            flowControl = (IsoTpFlowType)((data[0] >> 4) & 0x0f);
                        break;

                    default:
                        //  ignore all others
                        break;
                }
            }
        }


        /// <summary>
        /// Sends a ULC frame.
        /// </summary>
        /// <param name="ulc"></param>
        /// <param name="value"></param>
        /// <param name="address"></param>
        private void sendUlcFrame(UInt16 ulc, UInt32 value, UInt32 address)
        {
            sendIsoTpMessage(new IsoTpMessage(HCTP_CMD, address, new Byte[]
            {
                0x20,
                (byte)(ulc & 0xff), (byte)(UInt16)(ulc >> 8),
                (byte)value, (byte)(UInt32)(value >> 8), (byte)(UInt32)(value >> 16), (byte)(UInt32)(value >> 24)
            }));
        }

        volatile private IsoTpFlowType flowControl = IsoTpFlowType.Wait;

        /// <summary>
        /// Sends an ISO-TP message to an ECCONet device.
        /// </summary>
        /// <param name="message"></param>
        private void sendIsoTpMessage(IsoTpMessage message)
        {
            //  validate inputs
            if (message == null)
                return;

            int status = 0;
            Byte[] msgData = message.data;

            //  CAN frame
            UInt32 id = ((uint)message.messageType << 24) | (message.canAddress << 12) | message.canAddress;
            byte[] data;

            //  if a single-frame message
            if (msgData.Length <= 7)
            {
                data = new byte[msgData.Length + 1];
                data[0] = (byte)(((int)IsoTpFrameType.Single << 4) | msgData.Length);
                for (int i = 0; i < msgData.Length; i++)
                    data[i + 1] = msgData[i];
                status = sendCanFrame(id, data);
            }
            else  //  a multi-frame message
            {
                int dataIndex = 0;
                int frameIndex = 0;
                flowControl = IsoTpFlowType.Wait;
                DateTime timeout = DateTime.Now.AddSeconds(3);
                while (dataIndex < msgData.Length)
                {
                    //  We send the first frame of multi-frame message.
                    if (frameIndex == 0)
                    {
                        frameIndex++;
                        data = new byte[8];

                        //  D1:D0 bits 15-4 will be the data payload length
                        data[0] = (byte)(((int)IsoTpFrameType.First << 4) | ((msgData.Length >> 8) & 0x000f));
                        data[1] = (byte)(msgData.Length & 0x00ff);
                        for (int i = 0; i < 6; i++, dataIndex++)
                            data[i + 2] = msgData[dataIndex];
                        status = sendCanFrame(id, data);
                    }
                    else  //  consecutive frames
                    {
                        //  check flow control.
                        switch (flowControl)
                        {
                            case IsoTpFlowType.ClearToSend:
                                {
                                    //  We check the number of bytes to send.
                                    int bytesToSend = msgData.Length - dataIndex;
                                    if (bytesToSend > 0)
                                    {
                                        //  We send next consecutive frame of multi-frame message.
                                        bytesToSend = (bytesToSend > 7) ? 7 : bytesToSend;
                                        data = new byte[bytesToSend + 1];
                                        data[0] = (byte)(((int)IsoTpFrameType.Consecutive << 4) | (frameIndex & 0x0f));
                                        for (int i = 0; i < bytesToSend; i++, dataIndex++)
                                            data[i + 1] = msgData[dataIndex];
                                        status = sendCanFrame(id, data);
                                        frameIndex++;
                                    }
                                    else
                                        status = -1;
                                    break;
                                }

                            case IsoTpFlowType.Wait:
                                {
                                    //  We don't wait forever...
                                    if (timeout.CompareTo(DateTime.Now) < 0)
                                        status = -1;
                                    break;
                                }

                            case IsoTpFlowType.Overflow_Abort:
                                status = -1;
                                break;
                        }
                    }
                    //  check for error.
                    if (0 != status)
                        break;
                }
            }
        }
#endregion



#region Send and Receive CAN frames
        /// <summary>
        /// Sends a CAN frame.
        /// </summary>
        /// <param name="id">The CAN frame ID.</param>
        /// <param name="data">The CAN frame data.</param>
        public int sendCanFrame(UInt32 id, byte[] data)
        {
            //  forward CAN frame to application
            if (core != null)
                return core.SendCanFrame(id, data);
            return -1;
        }

        /// <summary>
        /// Receives a CAN frame.
        /// </summary>
        /// <param name="id">The CAN frame ID.</param>
        /// <param name="data">The CAN frame data.</param>
        public void ReceiveCanFrame(UInt32 id, byte[] data)
        {
            processReceivedCanFrame(id, data);
        }


#endregion


#endif
#endregion  //  UNUSED