using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ECCONetDevTool.FlashFileSystem
{
    /// <summary>
    /// The base flash file system file.
    /// </summary>
    public class FlashFile
    {
        /// <summary>
        /// The file name in 8.3 format.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The file's normal zero-based position index in the volume.
        /// </summary>
        public uint VolumeIndex { get; set; }

        /// <summary>
        /// The file data.
        /// </summary>
        public virtual byte[] Data { get; set; }

        /// <summary>
        /// The header location.
        /// </summary>
        public uint HeaderLocation { get; set; }

        /// <summary>
        /// The data location.
        /// </summary>
        public uint DataLocation { get; set; }

        /// <summary>
        /// The data location offset.
        /// </summary>
        public UInt16 DataLocationOffset { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="volumeIndex">The file's normal zero-based position index in the volume.</param>
        public FlashFile(uint volumeIndex)
        {
            VolumeIndex = volumeIndex;
        }

        /// <summary>
        /// ToString() override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        private UInt16 ComputeFileCRC16(byte[] data)
        {
            //	validate input
            if ((data == null) || (data.Length == 0))
                throw new System.ArgumentNullException("data", "ComputeFileCRC bad parameter");

            //  calculate crc
            UInt16 crc = 0;
            for (int i = 0; i < data.Length; ++i)
            {
                byte byteVal = data[i];
                for (int bit = 0; bit < 8; ++bit)
                {
                    crc = (0 != ((byteVal ^ (byte)crc) & 1)) ?
                        (UInt16)((crc >> 1) ^ (UInt16)0xA001) : (UInt16)(crc >> 1);
                    byteVal >>= 1;
                }
            }
            return crc;
        }

        /// <summary>
        /// Verifies a file name for length and '.' separator.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <returns>If the filename is good, then returns the length of the filename, else 0.</returns>
        private UInt16 Validate_8_3_FileName(Byte[] filename)
        {
            //	check for null
            if (null == filename)
                return 0;

            //	check syntax
            UInt16 dot = 0;
            UInt16 len = 0;
            for (; len < filename.Length; ++len)
            {
                if (0 == filename[len])
                    break;
                if (len >= 12)
                    return 0;
                if (filename[len] == '.')
                    dot = len;
            }
            if ((1 <= dot) && (2 <= (len - dot)) && (4 >= (len - dot)))
                return len;
            return 0;
        }

        /// <summary>
        /// Calculates the FLASH file header checksums.
        /// </summary>
        /// <param name="headerChecksum">The calculated header checksum output.</param>
        /// <param name="dataChecksum">The calculated data checksum output.</param>
        private void FileHeaderChecksums(out UInt16 headerChecksum, out UInt16 dataChecksum)
        {
            //  create header data to checksum
            byte[] header = new byte[28];

            //  add the 8.3 file name with trailing zeroes
            Encoding enc = Encoding.GetEncoding("us-ascii",
                                     new EncoderExceptionFallback(),
                                     new DecoderExceptionFallback());
            byte[] fn = enc.GetBytes(Name);
            if (0 == Validate_8_3_FileName(fn))
                throw new Exception("File name not in 8.3 format.");
            for (int n = 0; n < fn.Length; ++n)
                header[n] = fn[n];

            //  add the data location
            header[12] = (byte)DataLocation;
            header[13] = (byte)(DataLocation >> 8);
            header[14] = (byte)(DataLocation >> 16);
            header[15] = (byte)(DataLocation >> 24);

            //  the timestamp is zero

            //  add the data size
            header[20] = (byte)Data.Length;
            header[21] = (byte)(Data.Length >> 8);
            header[22] = (byte)(Data.Length >> 16);
            header[23] = (byte)(Data.Length >> 24);

            //  calc and add the data checksum
            dataChecksum = ComputeFileCRC16(Data.ToArray());
            header[24] = (byte)dataChecksum;
            header[25] = (byte)(dataChecksum >> 8);

            //  add the data location offset
            header[26] = (byte)DataLocationOffset; ;
            header[27] = (byte)(DataLocationOffset >> 8);

            //  calc the header checksum
            headerChecksum = ComputeFileCRC16(header);
        }

        /// <summary>
        /// Builds a file header C string.
        /// </summary>
        /// <param name="gcc">Build with gcc output.</param>
        /// <returns>Returns a file header C string.</returns>
        public string BuildHeaderCString(bool gcc)
        {
            //  get the file name
            string fileNameTitle = Name.Substring(0, 1).ToUpper() + Name.Substring(1).Replace('.', '_');

            //  create the file header source code
            FileHeaderChecksums(out UInt16 headerChecksum, out UInt16 dataChecksum);
            string header = "/**";
            header += ("\r\n  * @brief  " + Name + " flash file header.");
            header += "\r\n  */";
            header += string.Format("\r\nconst FLASH_DRIVE_FILE {0}_FileHeader", fileNameTitle);
            if (gcc)
                header += string.Format("\r\n\t__attribute__((section(\".{0}_FileHeader\"))) = // 0x{1:X8}",
                    fileNameTitle, HeaderLocation);
            else
                header += string.Format("\r\n\t__attribute__((at(0x{0:X8}))) =", HeaderLocation);
            header += "\r\n{";
            header += "\r\n\t.key = FLASH_DRIVE_FILE_KEY_ACTIVE,";
            header += string.Format("\r\n\t.checksum = 0x{0:X4},", headerChecksum);
            header += ("\r\n\t.name = \"" + Name + "\",");
            header += "\r\n\t.timestamp = 0x00000000,";
            header += string.Format("\r\n\t.dataLocation = 0x{0:X8},", DataLocation);
            header += string.Format("\r\n\t.dataSize = {0},", Data.Length);
            header += string.Format("\r\n\t.dataChecksum = 0x{0:X4},", dataChecksum);
            header += string.Format("\r\n\t.dataLocationOffset = 0x{0:X4},", DataLocationOffset);
            header += "\r\n};";
            return header;
        }


        /// <summary>
        /// Builds C-language source file string for flash file header and data.
        /// Override in inheriting class.
        /// </summary>
        /// <param name="gcc">Build with gcc output.</param>
        /// <returns>Returns a C string of the header.</returns>
        public virtual string ToCSourceFileString(bool gcc)
        {
            return string.Empty;
        }

        /// <summary>
        /// Builds C-language header file string for flash file header and data.
        /// Override in inheriting class.
        /// </summary>
        /// <returns>Returns a C string of the header.</returns>
        public virtual string ToCHeaderFileString()
        {
            return string.Empty;
        }

        /// <summary>
        /// Builds C-language GCC linker command for flash file header and data locations.
        /// Override in inheriting class.
        /// </summary>
        /// <returns>Returns a C string of the header.</returns>
        public string ToCLinkerFileString()
        {
            //  get the file name
            string fileNameTitle = Name.Substring(0, 1).ToUpper() + Name.Substring(1).Replace('.', '_');

            string str = string.Format("\r\n -Wl,--section-start=.{0}_FileHeader=0x{1:X8}", fileNameTitle, HeaderLocation);
            str += string.Format("\r\n -Wl,--section-start=.{0}_FileData=0x{1:X8}", fileNameTitle, DataLocation);
            return str;
        }

    }
}
