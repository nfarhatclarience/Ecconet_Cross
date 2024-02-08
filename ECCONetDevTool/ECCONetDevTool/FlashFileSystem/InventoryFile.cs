using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ECCONetDevTool.FlashFileSystem
{
    /// <summary>
    /// The light bar inventory file class.
    /// </summary>
    public class ProductAssemblyFile : BinFile
    {
        /// <summary>
        /// The fixed file name.
        /// </summary>
        public static readonly string FileName = "assembly.epa";

        /// <summary>
        /// The InventoryFile file name.
        /// </summary>
        public override string Name => FileName;

        /// <summary>
        /// The imported file name, if any.
        /// </summary>
        public string ImportedFileName { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="volumeIndex">The file's normal zero-based position index in the volume.</param>
        public ProductAssemblyFile(uint volumeIndex) : base(volumeIndex) { }

    }
}
