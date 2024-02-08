using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ECCONetDevTool.FlashFileSystem
{
    /// <summary>
    /// The light engine dictionary bin file class.
    /// </summary>
    public class LightEngineDictionaryFile : BinFile
    {
        /// <summary>
        /// The fixed file name.
        /// </summary>
        public const string FileName = "lighteng.dct";

        /// <summary>
        /// The LightEngineDictionaryFile file name.
        /// </summary>
        public override string Name => FileName;

        /// <summary>
        /// The dictionary entry size in bytes.
        /// </summary>
        public uint DictionaryEntrySize { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="volumeIndex">The file's normal zero-based position index in the volume.</param>
        public LightEngineDictionaryFile(uint volumeIndex, uint entrySize) : base(volumeIndex)
        {
            DictionaryEntrySize = entrySize;
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

            //  dictionary data
            while (i < Data.Length)
            {
                if (DictionaryEntrySize == 4)
                {
                    bool isZeroKey = ((Data[i] == 0) && (Data[i + 1] == 0));
                    uint lightEngineId = (((uint)Data[i + 2] << 8) | Data[i + 3]);
                    if (isZeroKey)
                        headerAndDataString += string.Format("\r\n\t//  ID 0x{0:X4}", lightEngineId);
                    headerAndDataString += "\r\n\t";
                    for (int n = 0; (n < 4) && (i < Data.Length); ++n, ++i)
                    {
                        headerAndDataString += ("0x" + Data[i].ToString("x2") + ", ");
                    }
                }
                else if (DictionaryEntrySize == 6)
                {
                    bool isZeroKey = ((Data[i] == 0) && (Data[i + 1] == 0));
                    uint lightEngineId = (Data[i + 2] | ((uint)Data[i + 3] << 8) | ((uint)Data[i + 4] << 16)  | ((uint)Data[i + 5] << 24));
                    if (isZeroKey)
                        headerAndDataString += string.Format("\r\n\t//  ID 0x{0:X4}", lightEngineId);
                    headerAndDataString += "\r\n\t";
                    for (int n = 0; (n < 6) && (i < Data.Length); ++n, ++i)
                    {
                        headerAndDataString += ("0x" + Data[i].ToString("x2") + ", ");
                    }
                }
                else
                {
                    break;
                }
            }
            headerAndDataString += "\r\n};";

            //  return the header and data string
            return headerAndDataString;
        }


    }
}
