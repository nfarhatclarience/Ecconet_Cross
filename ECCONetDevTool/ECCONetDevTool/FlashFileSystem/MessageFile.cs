using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ESG.ExpressionLib.DataModels;


namespace ECCONetDevTool.FlashFileSystem
{
    /// <summary>
    /// The LED Matrix message bin file class.
    /// </summary>
    public class MessageFile : BinFile
    {
        /// <summary>
        /// The fixed file name.
        /// </summary>
        public const string FileName = "messages.tbl";

        /// <summary>
        /// The ExpressionFile file name.
        /// </summary>
        public override string Name => FileName;

        /// <summary>
        /// The associated expression collection.
        /// </summary>
        public LedMatrixMessageCollection MessageCollection { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="volumeIndex">The file's normal zero-based position index in the volume.</param>
        public MessageFile(uint volumeIndex) : base(volumeIndex) { }

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

            //  add the expressions
            if (MessageCollection != null)
            {
                headerAndDataString += "\r\n\t//  Enum     Message";
                headerAndDataString += "\r\n\t//  ===========================================================";
                foreach (var msg in MessageCollection.Messages)
                {
                    string pad = "       ";
                    uint patternEnum = msg.MessageEnum;
                    while ((patternEnum /= 10) > 0)
                    {
                        if (pad.Length > 1)
                            pad = pad.Substring(1);
                    }
                    headerAndDataString += ("\r\n\t//  " + msg.MessageEnum.ToString() + pad + msg.Text);
                }
                headerAndDataString += "\r\n";
            }

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

            if (Data.Length < 6)
                return headerAndDataString;
            uint numPatterns = (Data[4] | ((uint)Data[5] << 8));
            headerAndDataString += string.Format("\r\n\t//  Number of Messages: {0} (0x{0:X4})\r\n\t", numPatterns);
            for (int n = 0; (n < 2) && (i < Data.Length); ++n, ++i)
            {
                headerAndDataString += ("0x" + Data[i].ToString("x2") + ", ");
            }
            headerAndDataString += "\r\n";

            //  message data
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


    }
}
