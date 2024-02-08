using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECCONetDevTool.FlashFileSystem
{
    /// <summary>
    /// Delegate to notify receiver that the volume changed.
    /// </summary>
    /// <param name="sender">The flash file volume.</param>
    public delegate void FlashFileVolumeChangedDelegate(object sender, int remainingSpace);

    /// <summary>
    /// The flash file volume class.
    /// </summary>
    public class FlashFileVolume
    {
        //  known base file names
        public const string BytecodeDefaultFileName = "equation.btc";
        public const string BytecodeProfile1FileName = "eq_user1.btc";
        public const string BytecodeProfile2FileName = "eq_user2.btc";
        public const string BytecodeProfile3FileName = "eq_user3.btc";
        public const string BytecodeProfile4FileName = "eq_user4.btc";
        public const string BytecodeProfile5FileName = "eq_user5.btc";
        public const string BytecodeProfile6FileName = "eq_user6.btc";

        /// <summary>
        /// The volume indices of the files.
        /// </summary>
        public enum VolumeIndices : uint
        {
            CanAddress,
            ProductInfo,
            ProductAssembly,
            Expression,
            LightEngineDictionary,
            LedMatrixMessages,
            BytecodeDefault,
            BytecodeUserProfile1,
            BytecodeUserProfile2,
            BytecodeUserProfile3,
            BytecodeUserProfile4,
            BytecodeUserProfile5,
            BytecodeUserProfile6,
            Miscellaneous0,
            Miscellaneous1,
            Miscellaneous2,
            Miscellaneous3,
            Miscellaneous4,
            Miscellaneous5,
            Miscellaneous6,
            Miscellaneous7,
            Miscellaneous8,
            Miscellaneous9,
        }

        /// <summary>
        /// Delegate to notify receiver that the volume changed.
        /// </summary>
        public event FlashFileVolumeChangedDelegate VolumeChangedDelegate;

        /// <summary>
        /// The volume index.
        /// </summary>
        public uint Index { get; set; }

        /// <summary>
        /// The volume base address.
        /// </summary>
        public uint BaseAddress
        {
            get => _baseAddress;
            set
            {
                _baseAddress = value;
                RecalculateFileLocations();
            }
        }
        private uint _baseAddress;

        /// <summary>
        /// The volume size.
        /// </summary>
        public uint Size
        {
            get => _size;
            set
            {
                _size = value;
                RecalculateFileLocations();
            }
        }
        private uint _size;

        /// <summary>
        /// The list of files in the volume.
        /// </summary>
        public List<FlashFile> FlashFiles { get => _flashFiles; set => _flashFiles = value ?? _flashFiles; }
        private List<FlashFile> _flashFiles = new List<FlashFile>();


        /// <summary>
        /// Adds a file to the volume.
        /// </summary>
        /// <param name="file">The file to add.</param>
        public void AddFile(FlashFile file)
        {
            //  validate inputs
            if ((file == null) || (file.Name == null) || (file.Name.Equals(string.Empty)))
                return;

            //  remove any instances of file
            RemoveFile(file.Name);

            //  add the file
            int index = 0;
            for (; index < FlashFiles.Count; ++index)
                if (file.VolumeIndex <= FlashFiles[index].VolumeIndex)
                    break;
            FlashFiles.Insert(index, file);

            //  recalculate the locations
            RecalculateFileLocations();
        }

        /// <summary>
        /// Removes all instances of the named file from the volume.
        /// </summary>
        /// <param name="name">The name of the file to remove.</param>
        public void RemoveFile(string name)
        {
            //  validate inputs (string.Empty is OK)
            if (name == null)
                return;

            //  remove any instances of file
            for (int i = 0; i < FlashFiles.Count; ++i)
            {
                var ff = FlashFiles[i];
                if (ff.Name.Equals(name))
                {
                    FlashFiles.RemoveAt(i);
                    --i;
                }
            }

            //  recalculate the locations
            RecalculateFileLocations();
        }

        /// <summary>
        /// Recalculates the file header and data locations.
        /// </summary>
        /// <returns>Returns the amount of unused memory.</returns>
        public int RecalculateFileLocations()
        {
            //  set header and base address start
            uint headerAddress = BaseAddress;
            uint dataAddress = BaseAddress + Size;

            //  calculate and set the files' header and data locations
            foreach (var flashFile in FlashFiles)
            {
                flashFile.HeaderLocation = headerAddress;
                flashFile.DataLocation = (uint)((dataAddress - flashFile.Data.Length) & 0xfffffffc);

                //  update header and data address
                headerAddress += 0x20;
                dataAddress = flashFile.DataLocation;
            }

            //  calculate remaining space
            int remainingSpace = (int)dataAddress - (int)(headerAddress + 0x20);

            //  notify subscriber that volume changed
            VolumeChangedDelegate?.Invoke(this, remainingSpace);

            //  return true if all files fit in volume
            return remainingSpace;
        }

        /// <summary>
        /// Builds C-language source file string for flash file header and data.
        /// </summary>
        /// <param name="name">The name of the C file.</param>
        /// <param name="gcc">Build with gcc output.</param>
        /// <returns>Returns a C string of the volume.</returns>
        public string ToCSourceFileString(string name, bool gcc)
        {
            //  get header name
            string headerName = name.Replace(".c", ".h");

            //  make sure all the files are updated
            RecalculateFileLocations();

            //  create the file header
            string fileText = string.Empty;
            fileText += "/**";
            fileText += "\r\n  * *****************************************************************************";
            fileText += "\r\n  * @file       " + name;
            fileText += "\r\n  * @copyright  © 2017 ECCO Group.  All rights reserved.";
            fileText += "\r\n  * @author     ";
            fileText += "\r\n  * @version    ";
            fileText += "\r\n  * @date       " + DateTime.Now.ToShortDateString();
            fileText += "\r\n  * @brief      Default FLASH file system volume 0 files.";
            fileText += "\r\n  * *****************************************************************************";
            fileText += "\r\n  */";
            fileText += "\r\n";
            fileText += "\r\n#include <stdint.h>";
            fileText += "\r\n#include <stdbool.h>";
            fileText += "\r\n#include \"" + headerName + "\"";
            fileText += "\r\n\r\n";

            //  add the files
            foreach (var file in FlashFiles)
            {
                fileText += file.ToCSourceFileString(gcc);
                fileText += "\r\n\r\n";
            }
            fileText += "\r\n";

            //  return the file string
            return fileText;
        }

        /// <summary>
        /// Builds C-language header file string for flash file header and data.
        /// </summary>
        /// <param name="name">The name of the C file.</param>
        /// <param name="gcc">Build with gcc output.</param>
        /// <returns>Returns a C string of the volume.</returns>
        public string ToCHeaderFileString(string name, bool gcc)
        {
            //  make sure all the files are updated
            RecalculateFileLocations();

            //  create the file header
            string fileText = string.Empty;
            fileText += "/**";
            fileText += "\r\n  * *****************************************************************************";
            fileText += "\r\n  * @file       " + name;
            fileText += "\r\n  * @copyright  © 2017 ECCO Group.  All rights reserved.";
            fileText += "\r\n  * @author     ";
            fileText += "\r\n  * @version    ";
            fileText += "\r\n  * @date       " + DateTime.Now.ToShortDateString();
            fileText += "\r\n  * @brief      Default FLASH file system volume 0 files.";
            fileText += "\r\n  * *****************************************************************************";
            fileText += "\r\n  */";
            fileText += "\r\n";
            fileText += "\r\n#include <stdint.h>";
            fileText += "\r\n#include <stdbool.h>";
            fileText += "\r\n#include \"matrix_lib_interface.h\"";
            fileText += "\r\n";
            fileText += "\r\n#ifndef __DEFAULT_FLASH_FILES_H";
            fileText += "\r\n#define __DEFAULT_FLASH_FILES_H";
            fileText += "\r\n\r\n";

            //  add the files
            foreach (var file in FlashFiles)
            {
                fileText += file.ToCHeaderFileString();
                fileText += "\r\n\r\n";
            }

            //  add the gcc linker statements, if creating gcc file
            if (gcc)
            {
                fileText += "\r\n\r\n\r\n/*  GCC linker commands\r\n";
                foreach (var file in FlashFiles)
                    fileText += file.ToCLinkerFileString();
                fileText += "\r\n\r\n*/";
            }

            //  add the endif
            fileText += "\r\n\r\n#endif  //  __DEFAULT_FLASH_FILES_H";
            fileText += "\r\n";

            //  return the file string
            return fileText;
        }


    }
}
