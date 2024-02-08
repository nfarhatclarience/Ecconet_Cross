using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ECCONetDevTool.FlashFileSystem
{
    /// <summary>
    /// The Time-Logic FLASH file.
    /// </summary>
    public class TimeLogicFile : FlashFile
    {
        /// <summary>
        /// The default equations fixed file name.
        /// </summary>
        public static readonly string FileName = "equation.txt";

        /// <summary>
        /// The UserProfile 1 fixed file name.
        /// </summary>
        public static readonly string FileName_UserProfile1 = "eq_user1.txt";

        /// <summary>
        /// The UserProfile 2 fixed file name.
        /// </summary>
        public static readonly string FileName_UserProfile2 = "eq_user2.txt";

        /// <summary>
        /// The UserProfile 3 fixed file name.
        /// </summary>
        public static readonly string FileName_UserProfile3 = "eq_user3.txt";

        /// <summary>
        /// The UserProfile 4 fixed file name.
        /// </summary>
        public static readonly string FileName_UserProfile4 = "eq_user4.txt";

        /// <summary>
        /// The TimeLogicFile file name.
        /// </summary>
        public override string Name
        {
            get
            {
                switch (VolumeIndex)
                {
                    case (uint)FlashFileVolume.VolumeIndices.BytecodeUserProfile1:
                        return FileName_UserProfile1;
                    case (uint)FlashFileVolume.VolumeIndices.BytecodeUserProfile2:
                        return FileName_UserProfile2;
                    case (uint)FlashFileVolume.VolumeIndices.BytecodeUserProfile3:
                        return FileName_UserProfile3;
                    case (uint)FlashFileVolume.VolumeIndices.BytecodeUserProfile4:
                        return FileName_UserProfile4;
                    default:
                        return FileName;
                }
            }
        }

        /// <summary>
        /// The binary data.
        /// </summary>
        public override byte[] Data { get => _data; set => _data = value ?? _data; }
        private byte[] _data = new byte[0];

        /// <summary>
        /// The imported text, if any.
        /// </summary>
        public string ImportedText { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="volumeIndex">The file's normal zero-based position index in the volume.</param>
        public TimeLogicFile(uint volumeIndex) : base(volumeIndex) { }

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
            if (ImportedText.Equals(string.Empty))
                headerAndDataString += "\r\n\r\n";
            else
            {
                headerAndDataString += "\r\n";
                headerAndDataString += ImportedText;
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
            str += string.Format("\r\nextern const char {0}_FileData[{1}];", fileNameTitle, Data.Length);
            str += string.Format("\r\n#define {0}_FileDataSize {1}", fileNameTitle, Data.Length);
            return str;
        }
    }
}
