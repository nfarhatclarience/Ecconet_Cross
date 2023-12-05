/**
  ******************************************************************************
  * @file    	FileScanner.cs
  * @copyright  © 2017 ECCO Group.  All rights reserved.
  * @author  	M. Latham, Liquid Logic, LLC
  * @version 	1.0.0
  * @date    	May 2017
  * @brief   	Scans devices for product info.
  ******************************************************************************
  * @attention
  *
  * Unless required by applicable law or agreed to in writing, software created
  * by Liquid Logic, LLC is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES
  * OR CONDITIONS OF ANY KIND, either express or implied.
  *
  ******************************************************************************
  */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECCONet
{
    public partial class ECCONetApi
    {
        /// <summary>
        /// The ECCONet 3.0 device scan complete callback.
        /// </summary>
        /// <param name="onlineDevices">The list of online devices.</param>
        public delegate void FileScanCompleteCallback(FileInfo[] volumeFiles);

        /// <summary>
        /// An ECCONet 3.0 file info object.
        /// </summary>
        public class FileInfo
        {
            /// <summary>
            /// The file name.
            /// </summary>
            public string name;

            /// <summary>
            /// The file date.
            /// </summary>
            public DateTime date;

            /// <summary>
            /// The file data size.
            /// </summary>
            public UInt32 dataSize;

            /// <summary>
            /// The file data CRC checksum.
            /// </summary>
            public UInt16 dataChecksum;

            /// <summary>
            /// Constructor.
            /// </summary>
            public FileInfo() { }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="name">The file name.</param>
            /// <param name="date">The file date.</param>
            /// <param name="dataSize">The file data size.</param>
            /// <param name="dataChecksum">The file data checksum.</param>
            public FileInfo(string name, DateTime date, UInt32 dataSize, UInt16 dataChecksum)
            {
                //  save members
                this.name = name;
                this.date = date;
                this.dataSize = dataSize;
                this.dataChecksum = dataChecksum;
            }
        };
    }

    /// <summary>
    /// A class to get the complete list of files on a given server drive volume.
    /// </summary>
    internal sealed class FileScanner
    {
        //  the private array of files during scan
        List<ECCONetApi.FileInfo> files;

        //  the private index of files during scan
        int fileIndex;

        //  the tries counter of files during scan
        int tries;

        //  the scan server address
        byte serverAddress;

        //  the scan server access code
        UInt32 serverAccessCode;

        //  the scan drive volume index
        UInt16 volumeIndex;
            
        //  the scan callback
        ECCONetApi.FileScanCompleteCallback callback;

        //  the core logic
        ECCONetCore core;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="core">The core logic group.</param>
        public FileScanner(ECCONetCore core)
        {
            this.core = core;
        }

        /// <summary>
        /// Get the list of files on a given server drive volume.
        /// </summary>
        /// <param name="serverAddress">The server network address.</param>
        /// <param name="serverAccessCode">The server access code.</param>
        /// <param name="volumeIndex">The drive volume index.</param>
        /// <param name="callback">A method to call back when the list is complete.  Can be null.</param>
        /// <returns>Returns 0 on success, -1 if error and -2 if ftp client is busy.</returns>
        public int GetFileList(byte serverAddress, UInt32 serverAccessCode, UInt16 volumeIndex, ECCONetApi.FileScanCompleteCallback callback)
        {
            //  save params
            this.serverAddress = serverAddress;
            this.serverAccessCode = serverAccessCode;
            this.volumeIndex = volumeIndex;
            this.callback = callback;

            //  create new list
            files = new List<ECCONetApi.FileInfo>(10);

            //  start scan process
            //  note that this does not user worker thread because the FTP client already has callback
            fileIndex = 0;
            tries = 0;
            var request = new FtpClient.FtpTransactionRequest(FtpClient.FtpTransactionType.GetIndexedFileInfo,
                serverAddress, serverAccessCode, volumeIndex, (UInt32)fileIndex, core.ftpServerResponseTimeout, FTPServerFileInfoCallback);
            return core.FTP_TransactionRequest(request);
        }

        /// <summary>
        /// The FTP server transaction callback.
        /// </summary>
        /// <param name="callback">The callback data from the server.</param>
        void FTPServerFileInfoCallback(ECCONetApi.FtpCallbackInfo callback)
        {
            //  if file found at index
            if (Token.Keys.KeyResponseFileInfoComplete == callback.responseKey)
            {
                //  add the file to the list
                files.Add(new ECCONetApi.FileInfo(callback.filename, callback.fileDate, callback.fileDataSize,
                    callback.fileDataChecksum));

                //  clear tries, bump index
                tries = 0;
                ++fileIndex;
            }
            else if (Token.Keys.KeyResponseFileNotFound == callback.responseKey)
            {
                this.callback?.Invoke(files.ToArray());
                return;
            }


            //  if more than three tries, then return with null list
            if (3 < ++tries)
                this.callback?.Invoke(null);

            //  else get file info
            else
            {
                var request = new FtpClient.FtpTransactionRequest(FtpClient.FtpTransactionType.GetIndexedFileInfo,
                    serverAddress, serverAccessCode, volumeIndex, (UInt32)fileIndex, core.ftpServerResponseTimeout, FTPServerFileInfoCallback);
                if (0 != core.FTP_TransactionRequest(request))
                    this.callback?.Invoke(null);
            }
        }

    }
}
