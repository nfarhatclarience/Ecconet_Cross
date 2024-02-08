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
using ECCONet;
using static ECCONet.ECCONetApi;


namespace ECCONetDevTool
{
    public partial class FTP : UserControl
    {
        /// <summary>
        /// The CAN interface object.
        /// </summary>
        public ECCONetApi canInterface;

        /// <summary>
        /// The list of online devices.
        /// This list is populated by calling ProductInfoScanner.ScanForECCONetDevices
        /// a few seconds after the devices have booted.
        /// </summary>
        List<ECCONetApi.ECCONetDevice> onlineDevices;


        public FTP()
        {
            //  initialize the user interface
            InitializeComponent();

            //  restore the user settings
            restoreUserSettings();
        }


        public void OnlineDeviceListChangedHandler(List<ECCONetApi.ECCONetDevice> list)
        {
            if (this.cbbOnlineDevices.InvokeRequired)
            {
                ECCONetApi.OnlineDeviceListChangedDelegate d =
                    new ECCONetApi.OnlineDeviceListChangedDelegate(OnlineDeviceListChangedHandler);
                try
                {
                    this.Invoke(d, new object[] { list });
                }
                catch { }
            }
            else
            {
                //  save online devices
                this.onlineDevices = list;

                //  populate dropdown
                cbbOnlineDevices.Items.Clear();
                if ((null != onlineDevices) && (0 != onlineDevices.Count))
                {
                    foreach (ECCONetApi.ECCONetDevice device in onlineDevices)
                    {
                        String s = device.modelName + " / Addr " + device.address;
                        cbbOnlineDevices.Items.Add(s);
                    }
                    cbbOnlineDevices.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Gets a file's info from an ECCONet 3.0 device.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void btnFileGetInfo_Click(object sender, EventArgs e)
        {
            byte serverAddress;
            string serverFileName;

            //  validate and get the server address and file name
            if (!getServerAddressAndFileName(out serverAddress, out serverFileName))
                return;

            //  get the file info
            canInterface.GetFileInfo(serverAddress, serverFileName, FTPServerCallback);
        }

        /// <summary>
        /// Reads a text file from an ECCONet 3.0 device.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void bntFileRead_Click(object sender, EventArgs e)
        {
            byte serverAddress;
            string serverFileName;

            //  validate and get the server address and file name
            if (!getServerAddressAndFileName(out serverAddress, out serverFileName))
                return;

            //  read the file
            canInterface.ReadFile(serverAddress, serverFileName, FTPServerCallback);
        }

        /// <summary>
        /// Writes a text file to an ECCONet 3.0 device.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void btnFileWrite_Click(object sender, EventArgs e)
        {
            byte serverAddress;
            string serverFileName;

            //  validate and get the server address and file name
            if (!getServerAddressAndFileName(out serverAddress, out serverFileName))
                return;

            //  validate the local file
            if (!File.Exists(tbLocalFilePathName.Text))
            {
                lblLocalPathNameError.Visible = true;
                return;
            }

            //  try to get the file data
            byte[] fileData = null;
            try
            {
                //  if a unicode text file, then read and convert to 7-bit ASCII
                if (serverFileName.EndsWith(".txt") || serverFileName.EndsWith(".inf"))
                {
                    Encoding enc = Encoding.GetEncoding("us-ascii",
                                                         new EncoderExceptionFallback(),
                                                         new DecoderExceptionFallback());
                    fileData = enc.GetBytes(File.ReadAllText(tbLocalFilePathName.Text));
                }
                else
                {
                    fileData = File.ReadAllBytes(tbLocalFilePathName.Text);
                }

                //  if have data
                if (null != fileData)
                {
                    //  try to write the file
                    int status = canInterface.WriteFile(serverAddress, serverFileName, DateTime.Now, fileData, FTPServerCallback);

                    if (-1 == status)
                    {
                        MessageBox.Show("FTP client is busy.", "Write File Error");
                    }
                    else if (-2 == status)
                    {
                        MessageBox.Show("FTP server (device) not found.", "Write File Error");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Write File Error");
            }
        }

        /// <summary>
        /// Deletes a text file to an ECCONet 3.0 device.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void btnDeleteFile_Click(object sender, EventArgs e)
        {
            byte serverAddress;
            string serverFileName;

            //  validate and get the server address and file name
            if (!getServerAddressAndFileName(out serverAddress, out serverFileName))
                return;

            //  delete the file
            canInterface.DeleteFile(serverAddress, serverFileName, FTPServerCallback);
        }

        /// <summary>
        /// This is the FTP server transaction callback.
        /// </summary>
        /// <param name="callback">The callback data from the server.</param>
        void FTPServerCallback(ECCONetApi.FtpCallbackInfo callback)
        {
            if (this.tbFtpResponse.InvokeRequired)
            {
                FtpTransferCompleteDelegate d = new FtpTransferCompleteDelegate(FTPServerCallback);
                this.Invoke(d, new object[] { callback });
            }
            else
            {
                switch (callback.responseKey)
                {
                    case Token.Keys.KeyResponseFileInfoComplete:
                        tbFtpResponse.Text = String.Format("FTP Client Response: {0}\n\nFile {1}: Size: {2}  File Date: {3}",
                            callback.responseKey, callback.filename, callback.fileDataSize, callback.fileDate);
                        break;

                    case Token.Keys.KeyResponseFileReadComplete:
                        //  if a text file, then get string
                        string fileText = null;
                        if (tbServerFilePathName.Text.EndsWith(".txt") || tbServerFilePathName.Text.EndsWith(".inf"))
                            fileText = System.Text.Encoding.UTF8.GetString(callback.fileData, 0, (int)callback.fileData.Length);

                        //  update ftp response text box
                        tbFtpResponse.Text = String.Format("FTP Client Response: {0}\n\nFile {1}: Size: {2}  File Date: {3}\n\n",
                            callback.responseKey, callback.filename, callback.fileDataSize, callback.fileDate);
                        if (null != fileText)
                            tbFtpResponse.Text += fileText;

                        //  try to write to local file
                        if (isPathValidRootedLocal(tbLocalFilePathName.Text))
                        {
                            //  if file already exists then prompt user to proceed
                            if (File.Exists(tbLocalFilePathName.Text))
                            {
                                if (DialogResult.No == MessageBox.Show(String.Format("Overwrite local file {0}?", tbLocalFilePathName.Text),
                                    "Overwrite File?", MessageBoxButtons.YesNo))
                                    break;
                            }
                            try
                            {
                                if (null != fileText)
                                    File.WriteAllText(tbLocalFilePathName.Text, fileText);
                                else
                                    File.WriteAllBytes(tbLocalFilePathName.Text, callback.fileData);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString(), "Read File Error");
                            }
                        }
                        break;

                    case Token.Keys.KeyResponseFileWriteComplete:
                        tbFtpResponse.Text = String.Format("FTP Client Response: {0}\n\nFile {1} write complete.", callback.responseKey, callback.filename);
                        break;

                    case Token.Keys.KeyResponseFileDeleteComplete:
                        tbFtpResponse.Text = String.Format("FTP Client Response: {0}\n\nFile {1} delete complete.", callback.responseKey, callback.filename);
                        break;

                    default:
                        tbFtpResponse.Text = String.Format("FTP Client Response: {0}\n\nFile {1}", callback.responseKey, callback.filename);
                        break;
                }
            }
        }

        private bool isPathValidRootedLocal(String path)
        {
            Uri pathUri;
            Boolean isValidUri = Uri.TryCreate(path, UriKind.Absolute, out pathUri);
            return (isValidUri && (pathUri != null) && (pathUri.IsLoopback));
        }


        #region User settings
        /// <summary>
        /// Save user settings.
        /// </summary>
        private void saveUserSettings()
        {
            Properties.Settings.Default.ServerFileName = tbServerFilePathName.Text;
            Properties.Settings.Default.LocalFilePathName = tbLocalFilePathName.Text;
        }

        /// <summary>
        /// Restore user settings.
        /// </summary>
        private void restoreUserSettings()
        {
            tbServerFilePathName.Text = Properties.Settings.Default.ServerFileName;
            tbLocalFilePathName.Text = Properties.Settings.Default.LocalFilePathName;
        }
        #endregion

        #region Drive volume 0 file list
        /// <summary>
        /// Lists the files on an ECCONet 3.0 device volume 0.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void btnFileList_Click(object sender, EventArgs e)
        {
            UInt16 volumeIndex = 0;
            try
            {
                canInterface.GetFileList(onlineDevices[cbbOnlineDevices.SelectedIndex].address,
                        onlineDevices[cbbOnlineDevices.SelectedIndex].serverAccessCode, volumeIndex,
                        FileListComplete);
            }
            catch { }
        }

        /// <summary>
        /// The product scan complete callback.
        /// </summary>
        /// <param name="onlineDevices">The list of online devices.</param>
        private void FileListComplete(ECCONetApi.FileInfo[] files)
        {
            if (this.tbFtpResponse.InvokeRequired)
            {
                FileScanCompleteCallback d = new FileScanCompleteCallback(FileListComplete);
                try
                {
                    this.Invoke(d, new object[] { files });
                }
                catch { }
            }
            else
            {
                tbFtpResponse.Text = "Name\t\tSize\tDate\n==============================================\n";
                if ((null != files) && (0 != files.Length))
                {
                    foreach (ECCONetApi.FileInfo fileInfo in files)
                    {
                        tbFtpResponse.Text += String.Format("{0}\t{1}\t{2}\n",
                            fileInfo.name, fileInfo.dataSize, fileInfo.date);
                    }
                }
                //canInterface.GetFileList(onlineDevices[cbbOnlineDevices.SelectedIndex].address,
                //    onlineDevices[cbbOnlineDevices.SelectedIndex].serverAccessCode, 1,
                //    FileListComplete1);
            }
        }

        /// <summary>
        /// The product scan complete callback.
        /// </summary>
        /// <param name="onlineDevices">The list of online devices.</param>
        private void FileListComplete1(ECCONetApi.FileInfo[] files)
        {
            if (this.tbFtpResponse.InvokeRequired)
            {
                ECCONetApi.FileScanCompleteCallback d = new ECCONetApi.FileScanCompleteCallback(FileListComplete);
                try
                {
                    this.Invoke(d, new object[] { files });
                }
                catch { }
            }
            else
            {
                if ((null != files) && (0 != files.Length))
                {
                    foreach (ECCONetApi.FileInfo fileInfo in files)
                    {
                        tbFtpResponse.Text += String.Format("{0}\t{1}\t{2}\n",
                            fileInfo.name, fileInfo.dataSize, fileInfo.date);
                    }
                }
            }
        }


        #endregion


        /// <summary>
        /// Button handler to search local directory for file name and fill text boxes.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void buttonFileSearch_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                //  populate local file path text box
                tbLocalFilePathName.Text = openFileDialog.FileName;

                //  try to populate server file name
                if (validateServerFileName(openFileDialog.SafeFileName))
                    tbServerFilePathName.Text = openFileDialog.SafeFileName;
            }
        }

        /// <summary>
        /// Validates and gets the selected server address and file name.
        /// </summary>
        /// <param name="address">Out variable, returns the server address.</param>
        /// <param name="fileName">Out variable, returns the server file name.</param>
        /// <returns>Returns true if server address and file name are valid.</returns>
        private bool getServerAddressAndFileName(out byte address, out string fileName)
        {
            address = 255;
            fileName = null;

            //  validate selected device
            if ((null == onlineDevices) || (0 == onlineDevices.Count)
                || (cbbOnlineDevices.SelectedIndex >= onlineDevices.Count))
            {
                lblServerAddressError.Visible = true;
                return false;
            }
            address = onlineDevices[cbbOnlineDevices.SelectedIndex].address;

            //  validate file name
            if (!validateServerFileName(tbServerFilePathName.Text))
            {
                lblServerFileNameError.Visible = true;
                return false;
            }
            fileName = tbServerFilePathName.Text;

            //  success
            return true;
        }

        /// <summary>
        /// Gets the server file name from the given file path and validates 8.3 format.
        /// </summary>
        /// <param name="filePath">The given path name.</param>
        /// <param name="serverFileName">Out variable, the server file name if path contains valid 8.3 format.</param>
        /// <returns>Returns true if file name in valid 8.3 format.</returns>
        private bool validateServerFileName(string fileName)
        {
            int index = 0;

            //  validate input
            if ((null == fileName) || (0 == fileName.Length))
                return false;

            //  validate total length
            if ((3 > fileName.Length) || (12 < fileName.Length))
                return false;

            //  validate extension length
            index = fileName.LastIndexOf('.');
            if ((index <= 0) || (index < (fileName.Length - 4)) || (index > (fileName.Length - 2)))
                return false;

            //  validate characters
            if (-1 != fileName.IndexOfAny(Path.GetInvalidPathChars()))
                return false;

            //  return success
            return true;
        }

        /// <summary>
        /// Gets the selected server access code.
        /// </summary>
        /// <param name="accessCode">Out variable, returns the server accessCode.</param>
        /// <returns>Returns true if server address is valid.</returns>
        private bool getServerAccessCode(out uint accessCode)
        {
            accessCode = 0;

            //  validate selected device
            if ((null == onlineDevices) || (0 == onlineDevices.Count)
                || (cbbOnlineDevices.SelectedIndex >= onlineDevices.Count))
            {
                MessageBox.Show("FTP server (device) selection not valid.", "Server Access Code Error");
                return false;
            }

            //  success
            accessCode = onlineDevices[cbbOnlineDevices.SelectedIndex].serverAccessCode;
            return true;
        }


        #region Local file path-name, server file name and selected device changed events
        /// <summary>
        /// Handles server file name changed event.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void tbServerFilePathName_TextChanged(object sender, EventArgs e)
        {
            lblServerFileNameError.Visible = false;
            Properties.Settings.Default.ServerFileName = tbServerFilePathName.Text;
        }

        /// <summary>
        /// Handles local file path-name changed event.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void tbLocalFilePathName_TextChanged(object sender, EventArgs e)
        {
            lblLocalPathNameError.Visible = false;
            Properties.Settings.Default.LocalFilePathName = tbLocalFilePathName.Text;
        }

        /// <summary>
        /// Handles server address selection changed event.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void cbbOnlineDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblServerAddressError.Visible = false;
        }
        #endregion
    }
}
