using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ECCONetDevTool.FlashFileSystem
{
    /// <summary>
    /// The product info file.
    /// </summary>
    public class ProductInfoFile : FlashFile
    {
        //  data field sizes
        const int modelNameSize = 31;
        const int manufacturerNameSize = 31;
        const int hardwareRevisionSize = 6;
        const int appFirmwareRevisionSize = 6;
        const int bootloaderFirmwareRevisionSize = 6;
        const int baseIndexedOutputEnumerationSize = 6;
        const int maximumIndexedOutputEnumerationSize = 6;


        /// <summary>
        /// The fixed file name.
        /// </summary>
        public static readonly string FileName = "product.inf";

        /// <summary>
        /// The ProductInfoFile file name.
        /// </summary>
        public override string Name => FileName;

        /// <summary>
        /// The read-only model name property.
        /// </summary>
        public string ModelName { get => _modelName; }
        private string _modelName = string.Empty;

        /// <summary>
        /// The read-only manufacturer name property.
        /// </summary>
        public string ManufacturerName { get => _manufacturerName; }
        private string _manufacturerName = string.Empty;

        /// <summary>
        /// The read-only hardware revision property.
        /// </summary>
        public string HardwareRevision { get => _hardwareRevision; }
        private string _hardwareRevision = string.Empty;

        /// <summary>
        /// The read-only application firmware revision property.
        /// </summary>
        public string AppFirmwareRevision { get => _appFirmwareRevision; }
        private string _appFirmwareRevision = string.Empty;

        /// <summary>
        /// The read-only bootloader firmware revision property.
        /// </summary>
        public string BootloaderFirmwareRevision { get => _bootloaderFirmwareRevision; }
        private string _bootloaderFirmwareRevision = string.Empty;

        /// <summary>
        /// The read-only base indexed output revision property.
        /// </summary>
        public string BaseIndexedOutputEnumeration { get => _baseIndexedOutputEnumeration; }
        private string _baseIndexedOutputEnumeration = string.Empty;

        /// <summary>
        /// The read-only maximum indexed output revision property.
        /// </summary>
        public string MaximumIndexedOutputEnumeration { get => _maximumIndexedOutputEnumeration; }
        private string _maximumIndexedOutputEnumeration = string.Empty;


        /// <summary>
        /// The data read-only property.
        /// </summary>
        public override byte[] Data
        {
            get
            {
                //  create data and string encoding
                byte[] fileData = new byte[92];
                Encoding enc = Encoding.GetEncoding("us-ascii",
                                         new EncoderExceptionFallback(),
                                         new DecoderExceptionFallback());

                //  model name
                int fileOffset = 0;
                byte[] bytes = enc.GetBytes(ModelName);
                for (int i = 0; ((i < bytes.Length) && (i < (modelNameSize - 1))); ++i)
                    fileData[i + fileOffset] = bytes[i];

                //  manufacturer name
                fileOffset += modelNameSize;
                bytes = enc.GetBytes(ManufacturerName);
                for (int i = 0; ((i < bytes.Length) && (i < (manufacturerNameSize - 1))); ++i)
                    fileData[i + fileOffset] = bytes[i];

                //  hardware revision
                fileOffset += manufacturerNameSize;
                bytes = enc.GetBytes(HardwareRevision);
                for (int i = 0; ((i < bytes.Length) && (i < (hardwareRevisionSize - 1))); ++i)
                    fileData[i + fileOffset] = bytes[i];

                //  app firmware revision name
                fileOffset += hardwareRevisionSize;
                bytes = enc.GetBytes(AppFirmwareRevision);
                for (int i = 0; ((i < bytes.Length) && (i < (appFirmwareRevisionSize - 1))); ++i)
                    fileData[i + fileOffset] = bytes[i];

                //  bootloader firmware revision name
                fileOffset += appFirmwareRevisionSize;
                bytes = enc.GetBytes(BootloaderFirmwareRevision);
                for (int i = 0; ((i < bytes.Length) && (i < (bootloaderFirmwareRevisionSize - 1))); ++i)
                    fileData[i + fileOffset] = bytes[i];

                //  base indexed output enumeration
                fileOffset += bootloaderFirmwareRevisionSize;
                bytes = enc.GetBytes(BaseIndexedOutputEnumeration);
                for (int i = 0; ((i < bytes.Length) && (i < (baseIndexedOutputEnumerationSize - 1))); ++i)
                    fileData[i + fileOffset] = bytes[i];

                //  maximum indexed output enumeration
                fileOffset += baseIndexedOutputEnumerationSize;
                bytes = enc.GetBytes(MaximumIndexedOutputEnumeration);
                for (int i = 0; ((i < bytes.Length) && (i < (maximumIndexedOutputEnumerationSize - 1))); ++i)
                    fileData[i + fileOffset] = bytes[i];

                //  return as list
                return fileData;
            }
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        /// <param name="volumeIndex">The file's normal zero-based position index in the volume.</param>
        private ProductInfoFile(uint volumeIndex) : base(volumeIndex) { }

        /// <summary>
        /// Creates a product info file.
        /// </summary>
        /// <param name="volumeIndex">The file's normal zero-based position index in the volume.</param>
        /// <param name="modelName">The model name.</param>
        /// <param name="manufacturerName">The manufacturer name.</param>
        /// <param name="hardwareRevision">The hardware revision.</param>
        /// <param name="appFirmwareRevision">The application firmware revision.</param>
        /// <param name="bootloaderFirmwareRevision">The bootloader firmware revision.</param>
        /// <param name="baseIndexedOutputEnumeration">The base indexed output enumeration.</param>
        /// <param name="maximumIndexedOutputEnumeration">The maximum indexed output enumeration.</param>
        /// <param name="errorMessage">Information about any error creating class.</param>
        /// <returns>Returns a new product info file, or null if there is an input error.</returns>
        public static ProductInfoFile Create(
            uint volumeIndex,
            string modelName,
            string manufacturerName,
            string hardwareRevision,
            string appFirmwareRevision,
            string bootloaderFirmwareRevision,
            string baseIndexedOutputEnumeration,
            string maximumIndexedOutputEnumeration,
            out string errorMessage)
        {
            //  if a static address, then validate it
            if (modelName.Length >= modelNameSize)
            {
                errorMessage = "Model name must be 30 or fewer characters.";
                return null;
            }
            if (manufacturerName.Length >= manufacturerNameSize)
            {
                errorMessage = "Manufacturer name must be 30 or fewer characters.";
                return null;
            }
            if (hardwareRevision.Length >= hardwareRevisionSize)
            {
                errorMessage = "Hardware revision must be 5 or fewer characters.";
                return null;
            }
            if (appFirmwareRevision.Length >= appFirmwareRevisionSize)
            {
                errorMessage = "Application firmware revision must be 5 or fewer characters.";
                return null;
            }
            if (bootloaderFirmwareRevision.Length >= bootloaderFirmwareRevisionSize)
            {
                errorMessage = "Bootloader firmware revision must be 5 or fewer characters.";
                return null;
            }
            if (baseIndexedOutputEnumeration.Length >= baseIndexedOutputEnumerationSize)
            {
                errorMessage = "Base indexed output enumeration must be 5 or fewer characters.";
                return null;
            }
            if (maximumIndexedOutputEnumeration.Length >= maximumIndexedOutputEnumerationSize)
            {
                errorMessage = "Maximum indexed output enumeration must be 5 or fewer characters.";
                return null;
            }

            //  clear error message and return new CAN address file
            errorMessage = string.Empty;
            return new ProductInfoFile(volumeIndex)
            {
                _modelName = modelName,
                _manufacturerName = manufacturerName,
                _hardwareRevision = hardwareRevision,
                _appFirmwareRevision = appFirmwareRevision,
                _bootloaderFirmwareRevision = bootloaderFirmwareRevision,
                _baseIndexedOutputEnumeration = baseIndexedOutputEnumeration,
                _maximumIndexedOutputEnumeration = maximumIndexedOutputEnumeration
            };
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
            headerAndDataString += string.Format("\r\nconst MATRIX_PRODUCT_INFO_FILE_OBJECT {0}_FileData", fileNameTitle);
            if (gcc)
                headerAndDataString += string.Format("\r\n\t__attribute__((section(\".{0}_FileData\"))) = // 0x{1:X8}",
                    fileNameTitle, HeaderLocation);
            else
                headerAndDataString += string.Format("\r\n\t__attribute__((at(0x{0:X8}))) =", DataLocation);
            headerAndDataString += "\r\n{";
            headerAndDataString += "\r\n\t.modelName = \"" + ModelName + "\",";
            headerAndDataString += "\r\n\t.manufacturerName = \"" + ManufacturerName + "\",";
            headerAndDataString += "\r\n\t.hardwareRevision = \"" + HardwareRevision + "\",";
            headerAndDataString += "\r\n\t.appFirmwareRevision = \"" + AppFirmwareRevision + "\",";
            headerAndDataString += "\r\n\t.bootloaderFirmwareRevision = \"" + BootloaderFirmwareRevision + "\",";
            headerAndDataString += "\r\n\t.baseLightheadEnumeration = \"" + BaseIndexedOutputEnumeration + "\",";
            headerAndDataString += "\r\n\t.maxLightheadEnumeration = \"" + MaximumIndexedOutputEnumeration + "\",";
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
            str += string.Format("\r\nextern const MATRIX_PRODUCT_INFO_FILE_OBJECT {0}_FileData;", fileNameTitle);
            str += string.Format("\r\n#define {0}_FileDataSize sizeof(MATRIX_PRODUCT_INFO_FILE_OBJECT)", fileNameTitle);
            return str;
        }

    }
}
