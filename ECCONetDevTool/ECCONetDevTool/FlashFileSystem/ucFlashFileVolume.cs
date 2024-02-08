using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ESG.ExpressionLib.DataModels;

using VolumeIndices = ECCONetDevTool.FlashFileSystem.FlashFileVolume.VolumeIndices;

namespace ECCONetDevTool.FlashFileSystem
{
    public partial class ucFlashFileVolume : UserControl
    {
        /// <summary>
        /// The maximum number of volumes.
        /// </summary>
        const int numVolumes = 3;

        /// <summary>
        /// The FLASH file volume.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FlashFileVolume Volume { get; set; }

        /// <summary>
        /// Suppress warnings.
        /// </summary>
        private bool suppressWarnings = true;


        public ucFlashFileVolume()
        {
            //  initialize designer components
            InitializeComponent();

            //  create volume
            Volume = new FlashFileVolume();

            //  initialize list view
            lvFlashFileSystem.View = View.Details;
            lvFlashFileSystem.Columns.Add("File Name", 100, HorizontalAlignment.Center);
            lvFlashFileSystem.Columns.Add("Size", 70, HorizontalAlignment.Center);
            lvFlashFileSystem.Columns.Add("Header Location", 100, HorizontalAlignment.Center);
            lvFlashFileSystem.Columns.Add("Data Location", 100, HorizontalAlignment.Center);

            //  allow warning
            suppressWarnings = false;

        }


        /// <summary>
        /// Initialize the control with the volume index.
        /// </summary>
        /// <param name="volumeIndex">The volume index.</param>
        public void InitializeWithVolumeIndex(uint volumeIndex)
        {
            //  set the volume index
            if (Volume == null)
                Volume = new FlashFileVolume();
            if (volumeIndex >= numVolumes)
                volumeIndex = 0;
            Volume.Index = volumeIndex;

            //  restore settings
            RestoreSettings();

            //  subscribe to volume changes
            Volume.VolumeChangedDelegate += VolumeChanged;
        }

        #region Receive generated binary handler.
        /// <summary>
        /// Binary generated handler.
        /// </summary>
        /// <param name="sender">The object that generated the binary file.</param>
        /// <param name="name">The binary flash file system name.</param>
        /// <param name="binary">The generated binary.</param>
        /// <param name="ec">The expression collection, or null.</param>
        /// <param name="dictionaryEntrySize">The dictionary entry size, or zero.</param>
        public void BinaryGeneratedHandler(object sender, string name, byte[] binary, object userData, uint dictionaryEntrySize)
        {
            //  validate inputs
            if ((name == null) || (binary == null))
                throw new Exception("Flash file system received unknown binary.");

            //  get file names
            switch (name)
            {
                case ExpressionFile.FileName:
                    {
                        var file = new ExpressionFile((uint)VolumeIndices.Expression)
                        {
                            Data = binary,
                            ExpressionCollection = (userData is ExpressionCollection ec) ? ec : null,
                        };
                        Volume.AddFile(file);
                        break;
                    }

                case MessageFile.FileName:
                    {
                        var file = new MessageFile((uint)VolumeIndices.LedMatrixMessages)
                        {
                            Data = binary,
                            MessageCollection = (userData is LedMatrixMessageCollection lmc) ? lmc : null,
                        };
                        Volume.AddFile(file);
                        break;
                    }

                case LightEngineDictionaryFile.FileName:
                    {
                        var file = new LightEngineDictionaryFile((uint)VolumeIndices.LightEngineDictionary, dictionaryEntrySize)
                        {
                            Data = binary
                        };
                        Volume.AddFile(file);
                        break;
                    }

                case FlashFileVolume.BytecodeDefaultFileName:
                    {
                        var file = new BytecodeFile((uint)VolumeIndices.BytecodeDefault)
                        {
                            Name = FlashFileVolume.BytecodeDefaultFileName,
                            Data = binary
                        };
                        Volume.AddFile(file);
                        break;

                    }

                case FlashFileVolume.BytecodeProfile1FileName:
                    {
                        var file = new BytecodeFile((uint)VolumeIndices.BytecodeUserProfile1)
                        {
                            Name = FlashFileVolume.BytecodeProfile1FileName,
                            Data = binary
                        };
                        Volume.AddFile(file);
                        break;

                    }

                case FlashFileVolume.BytecodeProfile2FileName:
                    {
                        var file = new BytecodeFile((uint)VolumeIndices.BytecodeUserProfile2)
                        {
                            Name = FlashFileVolume.BytecodeProfile2FileName,
                            Data = binary
                        };
                        Volume.AddFile(file);
                        break;

                    }

                case FlashFileVolume.BytecodeProfile3FileName:
                    {
                        var file = new BytecodeFile((uint)VolumeIndices.BytecodeUserProfile3)
                        {
                            Name = FlashFileVolume.BytecodeProfile3FileName,
                            Data = binary
                        };
                        Volume.AddFile(file);
                        break;

                    }

                case FlashFileVolume.BytecodeProfile4FileName:
                    {
                        var file = new BytecodeFile((uint)VolumeIndices.BytecodeUserProfile4)
                        {
                            Name = FlashFileVolume.BytecodeProfile4FileName,
                            Data = binary
                        };
                        Volume.AddFile(file);
                        break;

                    }

                case FlashFileVolume.BytecodeProfile5FileName:
                    {
                        var file = new BytecodeFile((uint)VolumeIndices.BytecodeUserProfile5)
                        {
                            Name = FlashFileVolume.BytecodeProfile5FileName,
                            Data = binary
                        };
                        Volume.AddFile(file);
                        break;

                    }

                case FlashFileVolume.BytecodeProfile6FileName:
                    {
                        var file = new BytecodeFile((uint)VolumeIndices.BytecodeUserProfile6)
                        {
                            Name = FlashFileVolume.BytecodeProfile6FileName,
                            Data = binary
                        };
                        Volume.AddFile(file);
                        break;

                    }
            }

            //  update the volume display
            UpdateFlashVolume();
        }
        #endregion

        #region Flash volume

        /// <summary>
        /// Button update flash file volume clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateVolume_Click(object sender, EventArgs e)
        {
            UpdateFlashVolume();
        }

        /// <summary>
        /// Recalculates the volume and updates the list view.
        /// </summary>
        public void UpdateFlashVolume()
        {
            //  get volume location and size user inputs
            bool goodInput = ntbFlashVolumeBaseAddress.GetUInt32Value(out UInt32 baseAddress);
            if (!ntbFlashVolumeSize.GetUInt32Value(out UInt32 size))
                goodInput = false;

            //  if good input, then recalculate the volume
            if (goodInput)
            {
                Volume.BaseAddress = baseAddress;
                Volume.Size = size;
                int unusedMemory = Volume.RecalculateFileLocations();
            }
        }

        /// <summary>
        /// Flash volume changed handler.
        /// </summary>
        /// <param name="sender"></param>
        private void VolumeChanged(object sender, int remainingSpace)
        {
            //  set the remaining space label
            lblUnusedMemory.Text = "Unused: " + remainingSpace.ToString();

            //  show warning if files do not fit
            if (!suppressWarnings && remainingSpace < 0)
            {
                MessageBox.Show(string.Format("Files do not fit in volume {0}!", Volume.Index));
            }

            //  update the list view
            UpdateListView();
        }

        /// <summary>
        /// Updates the volume list view.
        /// </summary>
        private void UpdateListView()
        {
            lvFlashFileSystem.Items.Clear();
            foreach (var ff in Volume.FlashFiles)
            {
                lvFlashFileSystem.Items.Add(new ListViewItem(new[] { ff.Name, ff.Data.Length.ToString(),
                    "0x" + ff.HeaderLocation.ToString("x4"), "0x" + ff.DataLocation.ToString("x4") }));
            }
        }
        #endregion

        #region C file export
        /// <summary>
        /// Button export text file clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportTextFile_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;
                File.WriteAllText(fileName, Volume.ToCSourceFileString(Path.GetFileName(fileName), cbGCC.Checked));
                fileName = fileName.Replace(".c", ".h");
                File.WriteAllText(fileName, Volume.ToCHeaderFileString(Path.GetFileName(fileName), cbGCC.Checked));
            }
        }
        #endregion

        #region Save and restore settings

        /// <summary>
        /// Make sure string collections are initialized.
        /// </summary>
        private void CheckStringCollectionInits()
        {
            if (Properties.Settings.Default.FlashVolumeBaseAddress == null)
                Properties.Settings.Default.FlashVolumeBaseAddress = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.FlashVolumeSize == null)
                Properties.Settings.Default.FlashVolumeSize = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.GCCCodeCompilation == null)
                Properties.Settings.Default.GCCCodeCompilation = new System.Collections.Specialized.StringCollection();
        }

        /// <summary>
        /// The flash volume base address text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ntbFlashVolumeBaseAddress_TextChanged(object sender, EventArgs e)
        {
            //  update the flash volume
            //  only updating via "Update" button
            //UpdateFlashVolume();

            //  make sure string collections are initialized
            CheckStringCollectionInits();

            //  save the volume base address
            while (Properties.Settings.Default.FlashVolumeBaseAddress.Count < numVolumes)
                Properties.Settings.Default.FlashVolumeBaseAddress.Add(string.Empty);
            Properties.Settings.Default.FlashVolumeBaseAddress[(int)Volume.Index] = ntbFlashVolumeBaseAddress.Text;
        }

        /// <summary>
        /// The flash volume size textbox text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ntbFlashVolumeSize_TextChanged(object sender, EventArgs e)
        {
            //  update the flash volume
            //  only updating via "Update" button
            //UpdateFlashVolume();

            //  make sure string collections are initialized
            CheckStringCollectionInits();

            //  save the volume size
            while (Properties.Settings.Default.FlashVolumeSize.Count < numVolumes)
                Properties.Settings.Default.FlashVolumeSize.Add(string.Empty);
            Properties.Settings.Default.FlashVolumeSize[(int)Volume.Index] = ntbFlashVolumeSize.Text;
        }

        /// <summary>
        /// The GCC compile check changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbGCC_CheckedChanged(object sender, EventArgs e)
        {
            //  make sure string collections are initialized
            CheckStringCollectionInits();

            //  save the GCC compile setting
            while (Properties.Settings.Default.GCCCodeCompilation.Count < numVolumes)
                Properties.Settings.Default.GCCCodeCompilation.Add(string.Empty);
            Properties.Settings.Default.GCCCodeCompilation[(int)Volume.Index] = cbGCC.Checked ? "true" : "false";
        }

        /// <summary>
        /// Restore the user settings.
        /// </summary>
        void RestoreSettings()
        {
            //  make sure string collections are initialized
            CheckStringCollectionInits();

            //  restore volume base address
            if (Properties.Settings.Default.FlashVolumeBaseAddress.Count > Volume.Index)
                ntbFlashVolumeBaseAddress.Text = Properties.Settings.Default.FlashVolumeBaseAddress[(int)Volume.Index];

            //  restore volume size
            if (Properties.Settings.Default.FlashVolumeSize.Count > Volume.Index)
                ntbFlashVolumeSize.Text = Properties.Settings.Default.FlashVolumeSize[(int)Volume.Index];

            //  restore GCC compilation status
            if (Properties.Settings.Default.GCCCodeCompilation.Count > Volume.Index)
                cbGCC.Checked = Properties.Settings.Default.GCCCodeCompilation[(int)Volume.Index].Equals("true");

            //  update the flash volume
            UpdateFlashVolume();
        }

        #endregion

    }
}
