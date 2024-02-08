using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ECCONetDevTool.FlashFileSystem
{
    /// <summary>
    /// The CAN address file.
    /// </summary>
    public class CanAddressFile : FlashFile
    {
        /// <summary>
        /// The fixed file name.
        /// </summary>
        public static readonly string FileName = "address.can";

        /// <summary>
        /// The CanAddresFile file name.
        /// </summary>
        public override string Name => FileName;

        /// <summary>
        /// The CAN address read-only property.
        /// </summary>
        public byte Address { get => _address; }
        private byte _address;

        /// <summary>
        /// The CAN address is static read-only property.
        /// </summary>
        public bool IsStaticAddress { get => _isStaticAddress; }
        private bool _isStaticAddress;

        /// <summary>
        /// The data read-only property.
        /// </summary>
        public override byte[] Data
        {
            get
            {
                return new byte[] {
                    (byte)(IsStaticAddress ? Address : 0),
                    (byte)(IsStaticAddress ? 1 : 0)
                };
            }
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        /// <param name="volumeIndex">The file's normal zero-based position index in the volume.</param>
        private CanAddressFile(uint volumeIndex) : base(volumeIndex) { }

        /// <summary>
        /// Creates a CAN address file.
        /// </summary>
        /// <param name="address">The address in the range 0-127.</param>
        /// <param name="isStaticAddress">Indicates whether the address is static.</param>
        /// <param name="errorMessage">Information about any error creating class.</param>
        /// <returns>Returns a new CAN address file, or null if the given address was not valid.</returns>
        public static CanAddressFile Create(uint volumeIndex, byte address, bool isStaticAddress, out string errorMessage)
        {
            //  if a static address, then validate it
            if (isStaticAddress)
            {
                if ((address == 0) || (address > 128))
                {
                    errorMessage = " Static CAN address must be in the range 1-127.";
                    return null;
                }
            }
            else  //  else not static so set to zero
                address = 0;

            //  clear error message and return new CAN address file
            errorMessage = string.Empty;
            return new CanAddressFile(volumeIndex) { _address = address, _isStaticAddress = isStaticAddress };
        }

        /// <summary>
        /// Builds C-language source file string for flash file header and data.
        /// </summary>
        /// <param name="gcc">Build with gcc output.</param>
        /// <returns>Returns the binary file header and data string.</returns>
        public override string ToCSourceFileString(bool gcc)
        {
            //  get the file name
            string fileNameTitle = Name.Substring(0, 1).ToUpper() + Name.Substring(1).Replace('.', '_');

            //  build the header
            string headerAndDataString = BuildHeaderCString(gcc);

            //  build the data
            headerAndDataString += "\r\n\r\n/**";
            headerAndDataString += ("\r\n  * @brief  " + Name + " flash file data.");
            headerAndDataString += "\r\n  */";
            headerAndDataString += string.Format("\r\nconst MATRIX_CAN_ADDRESS_FILE_OBJECT {0}_FileData", fileNameTitle);
            if (gcc)
                headerAndDataString += string.Format("\r\n\t__attribute__((section(\".{0}_FileData\"))) = // 0x{1:X8}",
                    fileNameTitle, HeaderLocation);
            else
                headerAndDataString += string.Format("\r\n\t__attribute__((at(0x{0:X8}))) =", DataLocation);
            headerAndDataString += "\r\n{";
            headerAndDataString += string.Format("\r\n\t.address = {0},", Address);
            headerAndDataString += ("\r\n\t.isStatic = " + (IsStaticAddress ? "true," : "false,"));
            headerAndDataString += "\r\n};";

            //  return the header and data string
            return headerAndDataString;
        }

        /// <summary>
        /// Builds C-language header file string for flash file header and data.
        /// </summary>
        /// <returns>Returns a C string of the header.</returns>
        public override string ToCHeaderFileString()
        {
            //  get the file name
            string fileNameTitle = Name.Substring(0, 1).ToUpper() + Name.Substring(1).Replace('.', '_');

            //  build string
            string str = "\r\n//\tFactory default " + Name + " file.";
            str += string.Format("\r\nextern const FLASH_DRIVE_FILE {0}_FileHeader;", fileNameTitle);
            str += string.Format("\r\nextern const MATRIX_CAN_ADDRESS_FILE_OBJECT {0}_FileData;", fileNameTitle);
            str += string.Format("\r\n#define {0}_FileDataSize sizeof(MATRIX_CAN_ADDRESS_FILE_OBJECT)", fileNameTitle);
            return str;
        }

    }
}
