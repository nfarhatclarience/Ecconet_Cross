using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ECCONetDevTool.FlashFileSystem
{
    /// <summary>
    /// The bytecode flash file class.
    /// </summary>
    public class BytecodeFile : FlashFile
    {
        /// <summary>
        /// The binary data.
        /// </summary>
        public override byte[] Data { get => _data; set => _data = value ?? _data; }
        private byte[] _data = new byte[0];


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="volumeIndex">The file's normal zero-based position index in the volume.</param>
        public BytecodeFile(uint volumeIndex) : base(volumeIndex) { }

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
            headerAndDataString += string.Format("\r\nconst uint8_t {0}_FileData[{1}]", fileNameTitle, Data.Length);
            if (gcc)
                headerAndDataString += string.Format("\r\n\t__attribute__((section(\".{0}_FileData\"))) = // 0x{1:X8}",
                    fileNameTitle, HeaderLocation);
            else
                headerAndDataString += string.Format("\r\n\t__attribute__((at(0x{0:X8}))) =", DataLocation);
            headerAndDataString += "\r\n{";

            //  the file security code
            if (Data.Length < 4)
                return headerAndDataString;
            int i = 0;
            uint securityCode = (Data[0] | ((uint)Data[1] << 8) | ((uint)Data[2] << 16) | ((uint)Data[3] << 24));
            headerAndDataString += string.Format("\r\n\t//  Security Code: 0x{0:X8}\r\n\t", securityCode);
            for (int n = 0; (n < 4) && (i < Data.Length); ++n, ++i)
            {
                headerAndDataString += ("0x" + Data[i].ToString("x2") + ", ");
            }
            headerAndDataString += "\r\n";

            //  the file data
            while (i < Data.Length)
            {
                headerAndDataString += "\r\n\t";
                headerAndDataString += string.Format("/*{0:X8}*/ ", i + DataLocation);
                for (int n = 0; (n < 16) && (i < Data.Length); ++n, ++i)
                    headerAndDataString += ("0x" + Data[i].ToString("x2") + ", ");
            }
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
            str += string.Format("\r\nextern const uint8_t {0}_FileData[{1}];", fileNameTitle, Data.Length);
            str += string.Format("\r\n#define {0}_FileDataSize {1}", fileNameTitle, Data.Length);
            return str;
        }
    }
}
