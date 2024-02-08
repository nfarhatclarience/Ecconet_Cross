using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECCONetDevTool.FlashFileSystem
{
    /// <summary>
    /// The text file class.
    /// </summary>
    public class TextFile : FlashFile
    {
        /// <summary>
        /// The binary data.
        /// </summary>
        public override byte[] Data
        {
            get
            {
                try
                {
                    Encoding enc = Encoding.GetEncoding("us-ascii",
                                                            new EncoderExceptionFallback(),
                                                            new DecoderExceptionFallback());
                    return enc.GetBytes(Text);
                }
                catch
                {
                    return new byte[0];
                }
            }
            set
            {
                try
                {
                    Text = Encoding.UTF8.GetString(value, 0, value.Length);
                }
                catch
                {
                    Text = string.Empty;
                }
            }
        }

        /// <summary>
        /// The file text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="volumeIndex">The file's normal zero-based position index in the volume.</param>
        public TextFile(uint volumeIndex) : base(volumeIndex) { }

        /// <summary>
        /// Builds C-language source file string for flash file header and data.
        /// </summary>
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
            headerAndDataString += string.Format("\r\nconst char {0}_FileData[{1}]", fileNameTitle, Data.Length);
            if (gcc)
                headerAndDataString += string.Format("\r\n\t__attribute__((section(\".{0}_FileData\"))) = // 0x{1:X8}",
                    fileNameTitle, HeaderLocation);
            else
                headerAndDataString += string.Format("\r\n\t__attribute__((at(0x{0:X8}))) =", DataLocation);
            headerAndDataString += "\r\n{";
            if (Text.Equals(string.Empty))
                headerAndDataString += "\r\n\r\n";
            else
            {
                headerAndDataString += "\r\n";
                string[] lines = Text.Split(new string[] { "\r\n" , "\n" }, StringSplitOptions.None);
                foreach (var line in lines)
                {
                    if (line == string.Empty)
                        headerAndDataString += "\r\n";
                    else
                        headerAndDataString += "\t\"" + line + "\";\r\n";
                }
            }
            headerAndDataString += "};";

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
            str += string.Format("\r\nextern const char {0}_FileData[{1}];", fileNameTitle, Data.Length);
            str += string.Format("\r\n#define {0}_FileDataSize {1}", fileNameTitle, Data.Length);
            return str;
        }
    }
}
