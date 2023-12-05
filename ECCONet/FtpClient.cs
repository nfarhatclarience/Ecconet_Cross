/**
  ******************************************************************************
  * @file    	FtpClient.cs
  * @copyright  © 2017 ECCO Group.  All rights reserved.
  * @author  	M. Latham, Liquid Logic, LLC
  * @version 	1.0.0
  * @date    	April 2017
  * @brief   	Matrix file transfers over the CAN bus.
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
using System.Diagnostics;

namespace ECCONet
{
    /// <summary>
    /// The FtpTransferComplete delegate and FtpCallbackData data object are exposed in the API.
    /// </summary>
    public partial class ECCONetApi
    {
        #region Transfer complete delegate and callback info class
        /// <summary>
        /// An ftp client transfer complete delegate method.
        /// </summary>
        /// <param name="callbackData"></param>
        public delegate void FtpTransferCompleteDelegate(FtpCallbackInfo callbackData);

        /// <summary>
        /// An FTP server callback data object.
        /// </summary>
        public class FtpCallbackInfo
        {
            /// <summary>
            /// The response key code from the server.
            /// </summary>
            public Token.Keys responseKey;

            #region server

            /// <summary>
            /// The server network address.
            /// </summary>
            public byte serverAddress;

            /// <summary>
            /// The server access code.
            /// </summary>
            public UInt32 serverAccessCode;

            /// <summary>
            /// The server guid.
            /// </summary>
            public UInt32[] serverGuid;

            #endregion

            #region file

            /// <summary>
            /// The file name.
            /// </summary>
            public string filename;

            /// <summary>
            /// The file date.
            /// </summary>
            public DateTime fileDate;

            /// <summary>
            /// The file data.
            /// </summary>
            public byte[] fileData;

            /// <summary>
            /// The file data size.
            /// </summary>
            public UInt32 fileDataSize;

            /// <summary>
            /// The file data checksum.
            /// </summary>
            public UInt16 fileDataChecksum;

            #endregion
        }
        #endregion
    }

    /// <summary>
    /// The FTP client class.
    /// </summary>
    internal sealed class FtpClient
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// FTP transaction type
        /// </summary>
        public enum FtpTransactionType
        {
            GetIndexedFileInfo,
            GetFileInfo,
            ReadFile,
            WriteFile,
            DeleteFile
        }

        #region FTP transaction request class
        public class FtpTransactionRequest
        {
            /// <summary>
            /// The transaction type.
            /// </summary>
            public FtpTransactionType transactionType;

            /// <summary>
            /// The server network address.
            /// </summary>
            public byte serverAddress;

            /// <summary>
            /// The server access code.
            /// </summary>
            public UInt32 serverAccessCode;

            /// <summary>
            /// The file name used for reading and writing file by name.
            /// </summary>
            public string filename;

            /// <summary>
            /// The file index used for getting file info by index.
            /// </summary>
            public UInt32 fileIndex;

            /// <summary>
            /// The volume index used for getting file info by index.
            /// </summary>
            public UInt16 volumeIndex;

            /// <summary>
            /// The file date.
            /// </summary>
            public DateTime fileDate;

            /// <summary>
            /// The file data to write.  Null for other methods.
            /// </summary>
            public byte[] fileData;

            /// <summary>
            /// The period to wait for a server response before retry.
            /// </summary>
            public int serverResponseTimeoutPeriodMilliseconds;

            /// <summary>
            /// The number of times to retry a server request.
            /// </summary>
            public int numServerRetries = 3;

            /// <summary>
            /// A method to call back when the transaction is complete.  Can be null.
            /// </summary>
            public ECCONetApi.FtpTransferCompleteDelegate callback;

            /// <summary>
            /// Constructor.
            /// </summary>
            public FtpTransactionRequest() { }

            /// <summary>
            /// Constructor for indexed file info.
            /// </summary>
            /// <param name="transactionType">The transaction type.</param>
            /// <param name="serverAddress">The server network address.</param>
            /// <param name="serverAccessCode">The server access code.</param>
            /// <param name="volumeIndex">The drive volume index.</param>
            /// <param name="fileIndex">The file index.</param>
            /// <param name="callback">A method to call back when the transaction is complete.  Can be null.</param>
            /// <returns>Returns 0 on success, -1 if error and -2 if ftp client is busy.</returns>
            public FtpTransactionRequest(FtpTransactionType transactionType, byte serverAddress, UInt32 serverAccessCode, UInt16 volumeIndex,
                UInt32 fileIndex, int serverResponseTimeoutPeriodMilliseconds, ECCONetApi.FtpTransferCompleteDelegate callback)
            {
                // DON'T ADD LOGGING HERE - IT SEEMS TO CAUSE ISSUES WITH DETECTION

                //_logger.Trace($"Indexed file info transaction request received - transactiontype: {transactionType} - serverAddress: {serverAddress} - volumeIndex: {volumeIndex} - fileIndex: {fileIndex}");

                this.transactionType = transactionType;
                this.serverAddress = serverAddress;
                this.serverAccessCode = serverAccessCode;
                this.volumeIndex = volumeIndex;
                this.fileIndex = fileIndex;
                this.serverResponseTimeoutPeriodMilliseconds = serverResponseTimeoutPeriodMilliseconds;
                this.callback = callback;
            }

            /// <summary>
            /// Constructor for file info, file read and file delete.
            /// </summary>
            /// <param name="transactionType">The transaction type.</param>
            /// <param name="serverAddress">The server network address.</param>
            /// <param name="serverAccessCode">The server access code.</param>
            /// <param name="filename">The file name.</param>
            /// <param name="callback">A method to call back when the transaction is complete.  Can be null.</param>
            /// <returns>Returns 0 on success, -1 if error and -2 if ftp client is busy.</returns>
            public FtpTransactionRequest(FtpTransactionType transactionType, byte serverAddress, UInt32 serverAccessCode, 
                string filename, int serverResponseTimeoutPeriodMilliseconds, ECCONetApi.FtpTransferCompleteDelegate callback)
            {
                // DON'T ADD LOGGING HERE - IT SEEMS TO CAUSE ISSUES WITH DETECTION

                //_logger.Trace($"File info read Transaction Request received - Filename: {filename}, Server Address: {serverAddress}, TransactionType {transactionType}");
                this.transactionType = transactionType;
                this.serverAddress = serverAddress;
                this.serverAccessCode = serverAccessCode;
                this.filename = filename;
                this.serverResponseTimeoutPeriodMilliseconds = serverResponseTimeoutPeriodMilliseconds;
                this.callback = callback;
            }

            /// <summary>
            /// Constructor for writing a file.
            /// </summary>
            /// <param name="transactionType">The transaction type.</param>
            /// <param name="serverAddress">The server network address.</param>
            /// <param name="serverAccessCode">The server access code.</param>
            /// <param name="filename">The file name.</param>
            /// <param name="fileDate">The file date.</param>
            /// <param name="fileData">The file data to write.</param>
            /// <param name="callback">A method to call back when the transaction is complete.  Can be null.</param>
            public FtpTransactionRequest(FtpTransactionType transactionType,  byte serverAddress, UInt32 serverAccessCode, String filename,
                DateTime fileDate, byte[] buffer, int serverResponseTimeoutPeriodMilliseconds, ECCONetApi.FtpTransferCompleteDelegate callback)
            {
                // DON'T ADD LOGGING HERE - IT SEEMS TO CAUSE ISSUES WITH DETECTION

                //_logger.Trace($"FTP Write File Transaction Request received - Filename: {filename}, Server Address: {serverAddress}, TransactionType {transactionType}");

                this.transactionType = transactionType;
                this.serverAddress = serverAddress;
                this.serverAccessCode = serverAccessCode;
                this.filename = filename;
                this.fileDate = fileDate;
                this.fileData = buffer;
                this.serverResponseTimeoutPeriodMilliseconds = serverResponseTimeoutPeriodMilliseconds;
                this.callback = callback;
            }
        }
        #endregion

        /// <summary>
        /// A server data object.
        /// </summary>
        public class Server
        {
            //	server network address
            public Byte address;

            //	server access code
            public UInt32 accessCode;

            //	server guid
            public UInt32[] guid;

            //	the expected server response, or key null if idle
            public Token.Keys expectedResponse = Token.Keys.KeyNull;

            //	the server timeout timer
            public Int32 responseTimer;
        }

        /// <summary>
        /// A client file data object.
        /// </summary>
        class ClientFile
        {
            //	the name
            public string name;

            //	the date
            public DateTime date;

            //	the data
            public byte[] data;

            //	the data size
            public UInt32 dataSize;

            //	the data CRC
            public UInt16 dataChecksum;

            //	the current segment index
            public UInt16 segmentIndex;
        }

        //  the current transaction request
        FtpTransactionRequest currentTransactionRequest;

        //	the server request tries
        Int32 requestTries;

        //	the server for the current transaction request
        public Server server;

        //	the file for the current transaction request
        ClientFile file;

        //  the list of transactions to perform
        List<FtpTransactionRequest> transactionRequests;

        //  the transaction requests lock
        Object transactionRequestsLock;

        //  the core logic
        ECCONetCore core;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="core">The core logic group.</param>
        public FtpClient(ECCONetCore core)
        {
            //  save the core
            this.core = core;

            //  create the server, file and transaction requests
            server = new Server();
            file = new ClientFile();
            currentTransactionRequest = null; // new MULTI new FtpTransactionRequest();
            transactionRequests = new List<FtpTransactionRequest>(10);
            transactionRequestsLock = new object();

            //	reset the file transfer state
            server.expectedResponse = Token.Keys.KeyNull;
        }

        /// <summary>
        /// The FTP client clock.
        /// Note that the client state is determined by the expected response code from the server.
        /// An expected response code of KeyNull indicates that the FTP client is idle.
        /// </summary>
        public void Clock()
        {
            //  if transaction in process and server response has timed out
            if ((Token.Keys.KeyNull != server.expectedResponse)
                && (0 != server.responseTimer) && (0 == --server.responseTimer))
            {
                //  if more tries for client request
                if (requestTries > 0)
                {
                    //  bump tries
                    --requestTries;

                    //  if trying to start transaction, then try again
                    if (server.expectedResponse == Token.Keys.KeyResponseFileIndexedInfo
                     || server.expectedResponse == Token.Keys.KeyResponseFileInfo
                     || server.expectedResponse == Token.Keys.KeyResponseFileReadStart
                     || server.expectedResponse == Token.Keys.KeyResponseFileWriteStart
                     || server.expectedResponse == Token.Keys.KeyResponseFileDelete)
                    {
                        _logger.Debug($"Restarting transaction... Expected Response:{server.expectedResponse} ({server.address})");
                        var previousColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red; 
                        Output.WriteLine($"FTP_Client: Restarting transaction... Expected Response:{server.expectedResponse} ({server.address})");
                        Console.ForegroundColor = previousColor;
                        StartTransaction(currentTransactionRequest, true);
                    }
                    //  else if trying to read segment, then try again
                    else if (server.expectedResponse == Token.Keys.KeyResponseFileReadSegment)
                    {
                        // DON'T ADD LOGGING HERE - IT SEEMS TO CAUSE ISSUES WITH DETECTION

                        //_logger.Trace($"Requesting segment read");
                        RequestReadSegment(true);
                    }
                    //  else if trying to write segment, then try again
                    else if (server.expectedResponse == Token.Keys.KeyResponseFileWriteSegment)
                    {
                        // DON'T ADD LOGGING HERE - IT SEEMS TO CAUSE ISSUES WITH DETECTION

                        //_logger.Trace($"Requesting segment write");
                        RequestWriteSegment(true);
                    }
                    else
                    {
                        //  end the transaction
                        //  calling this may also help reset any malfunctioning server

                        // DON'T ADD LOGGING HERE - IT SEEMS TO CAUSE ISSUES WITH DETECTION

                        //_logger.Trace($"Transaction timed out");
                        EndTransaction(Token.Keys.KeyResponseFtpTransactionTimedOut);
                    }
                }
                else  //  no more retries
                {
                    //  end the transaction
                    //  calling this may also help reset any malfunctioning server

                    // DON'T ADD LOGGING HERE - IT SEEMS TO CAUSE ISSUES WITH DETECTION

                    //_logger.Trace($"Ending the transactiion");
                    EndTransaction(Token.Keys.KeyResponseFtpTransactionTimedOut);
                }
            }
        }

        #region Is busy and request transaction
        /// <summary>
        /// Returns a value indicating whether the server is busy.
        /// </summary>
        /// <returns>A value indicating whether the server is busy.</returns>
        public bool IsBusy()
        {
            _logger.Trace($"Checking if server is busy - {server.expectedResponse}");
            return server.expectedResponse != Token.Keys.KeyNull;
        }

        /// <summary>
        /// Requests a transaction.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns 0 on success, else -1 if server is busy.</returns>
        public int RequestTransaction(FtpTransactionRequest request)
        {
            if (Token.Keys.KeyNull == server.expectedResponse)
            {
                // DON'T ADD LOGGING HERE - IT SEEMS TO CAUSE ISSUES WITH DETECTION

                //_logger.Trace($"Transaction request - {currentTransactionRequest.filename}");
                currentTransactionRequest = request;
                requestTries = request.numServerRetries;
                StartTransaction(currentTransactionRequest, false);
                return 0;
            }

            _logger.Trace($"Transaction request - Server Expected Response: {server.expectedResponse} - returning -1 value");
            return -1;
        }
        #endregion

        #region Helper methods

        /// <summary>
        /// Helper method validates a transaction request file name.
        /// </summary>
        /// <param name="request">The request to check.</param>
        /// <returns>Returns true if the file name is valid, else false.</returns>
        bool IsTransactionRequestFileNameValid(FtpTransactionRequest request)
        {
            if (request == null || request.filename == null)
                return false;
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            Byte[] fn = encoding.GetBytes(request.filename);
            return ValidateFileName(fn) != 0;
        }

        /// <summary>
        /// Notifies requestor that transaction request has error.
        /// </summary>
        /// <param name="request">The transaction request.</param>
        void SendTransactionRequestErrorNotification(FtpTransactionRequest request)
        {
            //	if callback, then notify requestor
            if (null != request && null != request.callback)
            {
                _logger.Trace($"Transaction error notification - {request.filename}, Error: {Token.Keys.KeyResponseFtpClientError}");
                var callbackInfo = new ECCONetApi.FtpCallbackInfo()
                {
                    filename = request.filename,
                    responseKey = Token.Keys.KeyResponseFtpClientError
                };
                request.callback(callbackInfo);
            }

            //  log request to console (if debug level enabled)
            if (ECCONetCore.ConsoleDebugMessageLevels.FtpRequestsAndResponses <= core.consoleDebugMessageLevel)
                Output.WriteLine($"FTP Request: {Token.Keys.KeyResponseFtpClientError} ({server.address})");
        }

        /// <summary>
        /// Verifies a file name for length and '.' separator.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <returns>If the filename is good, then returns the length of the filename, else 0.</returns>
        UInt16 ValidateFileName(Byte[] filename)
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
                if (len >= LibConfig.FLASH_DRIVE_FILE_NAME_LENGTH)
                    return 0;
                if (filename[len] == '.')
                    dot = len;
            }
            if ((1 <= dot) && (2 <= (len - dot)) && (4 >= (len - dot)))
                return len;
            return 0;
        }

        /// <summary>
        /// CONFIDENTIAL INFORMATION
        /// Generates a server access code from the given guid.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        UInt32 GenerateServerAccessCode(UInt32[] guid)
        {
            UInt32 value;

            //	run hash and return code
            value = guid[0] ^ guid[3];
            value >>= (Int32)((guid[0] >> 3) & 3);
            value ^= guid[2];
            value ^= LibConfig.MATRIX_SERVER_ACCESS_POLY;
            value ^= guid[1];
            return value;
        }

        /// <summary>
        /// Creates a callback info populated with the ftp client server and file objects.
        /// </summary>
        /// <returns>A callback info populated with the ftp client server and file objects.</returns>
        ECCONetApi.FtpCallbackInfo CreateCallbackInfo(Token.Keys responseKey)
        {
            _logger.Trace($"Creating callback - {file.name}");
            var callbackInfo = new ECCONetApi.FtpCallbackInfo();

            //  response key
            callbackInfo.responseKey = responseKey;

            //  server info
            callbackInfo.serverAddress = server.address;
            callbackInfo.serverAccessCode = server.accessCode;
            if (null != server.guid)
            {
                callbackInfo.serverGuid = new UInt32[4];
                callbackInfo.serverGuid[0] = server.guid[0];
                callbackInfo.serverGuid[1] = server.guid[1];
                callbackInfo.serverGuid[2] = server.guid[2];
                callbackInfo.serverGuid[3] = server.guid[3];
            }

            //  file info
            callbackInfo.filename = String.Copy(file.name);
            callbackInfo.fileDate = file.date;
            callbackInfo.fileData = file.data;
            callbackInfo.fileDataSize = file.dataSize;
            callbackInfo.fileDataChecksum = file.dataChecksum;
            return callbackInfo;
        }

        /// <summary>
        /// Helper method does the following:
        /// - Sets the expected response.
        /// - Starts the response timer.
        /// - Sends request to server.
        /// </summary>
        /// <param name="expectedResponse">The response expected from the server.</param>
        /// <param name="tx">The transmitter used to send the message.</param>
        /// <param name="isRetry">True if the message is a retry.</param>
        void FinishRequest(Token.Keys expectedResponse, Transmitter tx, bool isRetry)
        {
            //	send request
            if (0 == tx.FinishMessage())
            {
                //  start the response timer and set the expected response 
                server.responseTimer =
                    currentTransactionRequest.serverResponseTimeoutPeriodMilliseconds / ECCONetCore.ProcessManagerTimerPeriod;
                server.expectedResponse = expectedResponse;

                //  load more retries
                if (!isRetry)
                {
                    if (currentTransactionRequest != null)
                        requestTries = currentTransactionRequest.numServerRetries;
                    else
                        requestTries = 3;
                }

                //	set the receiver sender address filter
                core.receiver.SetSenderAddressFilter(server.address, currentTransactionRequest.serverResponseTimeoutPeriodMilliseconds);
            }
            else
            {
                //	if failed to send message, then end transaction
                EndTransaction(Token.Keys.KeyResponseFtpServerError);
            }
        }
        #endregion

        #region Start Transaction
        /// <summary>
        /// Starts a transaction.
        /// </summary>
        /// <param name="request">The transaction request.</param>
        /// <param name="isRetry">True if the message is a retry.</param>
        void StartTransaction(FtpTransactionRequest request, bool isRetry)
        {
            Output.WriteLine($"Starting transaction... {request.transactionType} '{request.filename}' ({request.serverAddress})");

            if (null != request)
            {
                //  send request to handler
                switch (request.transactionType)
                {
                    case FtpTransactionType.GetIndexedFileInfo:
                        StartTransaction_GetIndexedFileInfo(request, isRetry);
                        break;

                    case FtpTransactionType.GetFileInfo:
                        StartTransaction_GetFileInfo(request, isRetry);
                        break;

                    case FtpTransactionType.ReadFile:
                        StartTransaction_ReadFile(request, isRetry);
                        break;

                    case FtpTransactionType.WriteFile:
                        StartTransaction_WriteFile(request, isRetry);
                        break;

                    case FtpTransactionType.DeleteFile:
                        StartTransaction_DeleteFile(request, isRetry);
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Starts an indexed file info request from the given ECCONet device ftp server.
        /// </summary>
        /// <param name="request">The transaction request.</param>
        /// <param name="isRetry">True if the message is a retry.</param>
        void StartTransaction_GetIndexedFileInfo(FtpTransactionRequest request, bool isRetry)
        {
            _logger.Trace($"Starting indexed file info request transaction: {request.filename}");

            //  check for errors
            bool error = (0 == request.serverAddress);
            if (error)
            {
                SendTransactionRequestErrorNotification(request);
                return;
            }

            //	initialize transaction
            server.address = request.serverAddress;
            server.accessCode = 0;

            //	initialize the transmitter
            var tx = new Transmitter(core);
            tx.StartMessage(request.serverAddress);

            //	send the request key
            tx.AddInt16((UInt16)Token.Keys.KeyRequestFileIndexedInfo);

            //	send the drive volume index
            tx.AddInt16(request.volumeIndex);

            //	send the file index
            tx.AddInt32(request.fileIndex);

            //	send the server access code
            tx.AddInt32(request.serverAccessCode);

            //	send the request
            FinishRequest(Token.Keys.KeyResponseFileIndexedInfo, tx, isRetry);

            //  log request to console (if debug level enabled)
            if (ECCONetCore.ConsoleDebugMessageLevels.FtpRequestsAndResponses <= core.consoleDebugMessageLevel)
                Output.WriteLine($"FTP Request: {Token.Keys.KeyRequestFileIndexedInfo} ({server.address})");
        }

        /// <summary>
        /// Starts a file info request from the given ECCONet device ftp server.
        /// </summary>
        /// <param name="request">The transaction request.</param>
        /// <param name="isRetry">True if the message is a retry.</param>
        void StartTransaction_GetFileInfo(FtpTransactionRequest request, bool isRetry)
        {
            _logger.Trace($"Starting file info request transaction: {request.filename}");

            //  check for errors
            bool error = (0 == request.serverAddress
                || !IsTransactionRequestFileNameValid(request));
            if (error)
            {
                SendTransactionRequestErrorNotification(request);
                return;
            }

            //	initialize transaction
            server.address = request.serverAddress;
            server.accessCode = 0;
            file.name = String.Copy(request.filename);

            //	initialize the transmitter
            var tx = new Transmitter(core);
            tx.StartMessage(request.serverAddress);

            //	send the request key
            tx.AddInt16((UInt16)Token.Keys.KeyRequestFileInfo);

            //	send the file name
            tx.AddString(request.filename);

            //	send the server access code
            tx.AddInt32(request.serverAccessCode);

            //	send the request
            FinishRequest(Token.Keys.KeyResponseFileInfo, tx, isRetry);

            //  log request to console (if debug level enabled)
            if (ECCONetCore.ConsoleDebugMessageLevels.FtpRequestsAndResponses <= core.consoleDebugMessageLevel)
                Output.WriteLine($"FTP Request: {Token.Keys.KeyRequestFileInfo} ({server.address})");
        }

        /// <summary>
        /// Starts a file read from the given ECCONet device ftp server.
        /// </summary>
        /// <param name="request">The transaction request.</param>
        /// <param name="isRetry">True if the message is a retry.</param>
        void StartTransaction_ReadFile(FtpTransactionRequest request, bool isRetry)
        {
            _logger.Trace($"Starting read file transaction: {request.filename}");

            //  check for errors
            bool error = (0 == request.serverAddress
                || !IsTransactionRequestFileNameValid(request));
            if (error)
            {
                SendTransactionRequestErrorNotification(request);
                return;
            }

            //	initialize transaction
            server.address = request.serverAddress;
            server.accessCode = request.serverAccessCode;
            file.name = String.Copy(request.filename);

            //	initialize the transmitter
            var tx = new Transmitter(core);
            tx.StartMessage(request.serverAddress);

            //	send the request key
            tx.AddInt16((UInt16)Token.Keys.KeyRequestFileReadStart);

            //	send the file name
            tx.AddString(request.filename);

            //	send the server access code
            tx.AddInt32(request.serverAccessCode);

            //	send the request
            FinishRequest(Token.Keys.KeyResponseFileReadStart, tx, isRetry);

            //  log request to console (if debug level enabled)
            if (ECCONetCore.ConsoleDebugMessageLevels.FtpRequestsAndResponses <= core.consoleDebugMessageLevel)
                Output.WriteLine($"FTP Request: {Token.Keys.KeyRequestFileReadStart} ({server.address})");
        }

        /// <summary>
        /// Starts a file write to the given ECCONet device ftp server.
        /// </summary>
        /// <param name="request">The transaction request.</param>
        /// <param name="isRetry">True if the message is a retry.</param>
        void StartTransaction_WriteFile(FtpTransactionRequest request, bool isRetry)
        {
            _logger.Trace($"Starting write file transaction: {request.filename}");

            //  check for errors
            bool error = (0 == request.serverAddress
                || !IsTransactionRequestFileNameValid(request)
                || null == request.fileData
                || 0 == request.fileData.Length);
            if (error)
            {
                SendTransactionRequestErrorNotification(request);
                return;
            }

            //	initialize transaction
            server.address = request.serverAddress;
            server.accessCode = request.serverAccessCode;
            file.name = String.Copy(request.filename);
            file.data = request.fileData;
            file.dataSize = (UInt32)request.fileData.Length;
            file.date = request.fileDate;
            if (file.date == default(DateTime))
                file.date = DateTime.Now;

            //	initialize the transmitter
            var tx = new Transmitter(core);
            tx.StartMessage(request.serverAddress);

            //	send the request key
            tx.AddInt16((UInt16)Token.Keys.KeyRequestFileWriteStart);

            //	send the file name
            tx.AddString(request.filename);

            //	send the data size
            tx.AddInt32((UInt32)request.fileData.Length);

            //	send the data crc
            tx.AddInt16(Crc.ComputeCRC16(request.fileData, (UInt32)request.fileData.Length));

            //	send the file timestamp in seconds since 00:00:00 Jan 1, 2017
            UInt32 seconds = (UInt32)request.fileDate.Subtract(new DateTime(2017, 1, 1, 0, 0, 0)).TotalSeconds;
            tx.AddInt32(seconds);

            //	send the server access code
            tx.AddInt32(request.serverAccessCode);

            //	send the request
            FinishRequest(Token.Keys.KeyResponseFileWriteStart, tx, isRetry);

            //  log request to console (if debug level enabled)
            if (ECCONetCore.ConsoleDebugMessageLevels.FtpRequestsAndResponses <= core.consoleDebugMessageLevel)
                Output.WriteLine($"FTP Request: {Token.Keys.KeyRequestFileWriteStart} ({server.address})");
        }

        /// <summary>
        /// Erases a file on the given ECCONet device ftp server.
        /// </summary>
        /// <param name="request">The transaction request.</param>
        /// <param name="isRetry">True if the message is a retry.</param>
        void StartTransaction_DeleteFile(FtpTransactionRequest request, bool isRetry)
        {
            _logger.Trace($"Starting delete file transaction: {request.filename}");

            //  check for errors
            bool error = (0 == request.serverAddress
                || !IsTransactionRequestFileNameValid(request));
            if (error)
            {
                SendTransactionRequestErrorNotification(request);
                return;
            }

            //	initialize transaction
            server.address = request.serverAddress;
            server.accessCode = request.serverAccessCode;
            file.name = String.Copy(request.filename);
            file.dataSize = 0;

            //	initialize the transmitter
            var tx = new Transmitter(core);
            tx.StartMessage(request.serverAddress);

            //	send the request key
            tx.AddInt16((UInt16)Token.Keys.KeyRequestFileDelete);

            //	send the file name
            tx.AddString(request.filename);

            //	send the server access code
            tx.AddInt32(request.serverAccessCode);

            //	send the request
            FinishRequest(Token.Keys.KeyResponseFileDelete, tx, isRetry);

            //  log request to console (if debug level enabled)
            if (ECCONetCore.ConsoleDebugMessageLevels.FtpRequestsAndResponses <= core.consoleDebugMessageLevel)
                Output.WriteLine($"FTP Request: {Token.Keys.KeyRequestFileDelete} ({server.address})");
        }
#endregion

        #region End Transaction
        /// <summary>
        /// Ends a transaction.
        /// </summary>
        /// <param name="statusKey">The final transaction status.</param>
        void EndTransaction(Token.Keys statusKey)
        {
            //	let the server know that transaction is complete
            Token token = new Token();
            token.address = server.address;
            token.key = Token.Keys.KeyRequestFileTransferComplete;
            token.value = 0;
            core.SendTokenToCanBus(token);

            //	clear expected response
            server.expectedResponse = Token.Keys.KeyNull;

            //	if transaction request
            if (null != currentTransactionRequest)
            {
                //  clear the receiver address filter (depreciated)
                core.receiver.SetSenderAddressFilter(0, currentTransactionRequest.serverResponseTimeoutPeriodMilliseconds);

                //  if callback delegate, then report statux
                var tempCallback = currentTransactionRequest.callback;
                tempCallback?.Invoke(CreateCallbackInfo(statusKey));
            }
            else
            {
                _logger.Trace($"Ending transaction but no transaction request found!");
                Debug.WriteLine("Ending transaction but no transaction request found!");
            }

            //  log transaction complete to console (if debug level enabled)
            _logger.Trace($"Transaction ended {statusKey}");
            if (ECCONetCore.ConsoleDebugMessageLevels.FtpRequestsAndResponses <= core.consoleDebugMessageLevel)
                Output.WriteLine($"FTP Transaction Ended: {statusKey} ({server.address})");
        }
        #endregion

        #region Server Response Handler
        /// <summary>
        /// Handles incoming server responses.
        /// </summary>
        /// <param name="senderAddress">The CAN address of the sender.</param>
        /// <param name="responseKey">The token response key.</param>
        /// <param name="body">An byte array of the message body.</param>
        public void ServerResponseIn(UInt16 senderAddress, Token.Keys responseKey, Byte[] body)
        {
            // DON'T ADD LOGGING HERE - IT SEEMS TO CAUSE ISSUES WITH DETECTION

            //  log response to console (if debug level enabled)
            if (ECCONetCore.ConsoleDebugMessageLevels.FtpRequestsAndResponses <= core.consoleDebugMessageLevel)
                Output.WriteLine($"FTP Response: {responseKey} with body of {body.Length} bytes ({server.address})");

            //	if not expecting a response (in idle state)
            //	or response is from an unexpected server, then ignore
            if ((Token.Keys.KeyNull == server.expectedResponse)
                || (senderAddress != server.address))
            {
                //  log error to console (if debug level enabled)
                if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                    Output.WriteLine($"FTP Response: Received response while expected response is KeyNull. ({server.address})");
                return;
            }

            //  handle response
            if (responseKey == server.expectedResponse)
            {
                switch (responseKey)
                {
                    case Token.Keys.KeyResponseFileIndexedInfo:
                        HandleFileInfoAndReadStartResponse(responseKey, body);
                        break;

                    case Token.Keys.KeyResponseFileInfo:
                        HandleFileInfoAndReadStartResponse(responseKey, body);
                        break;

                    case Token.Keys.KeyResponseFileReadStart:
                        HandleFileInfoAndReadStartResponse(responseKey, body);
                        break;

                    case Token.Keys.KeyResponseFileReadSegment:
                        HandleFileReadSegmentResponse(body);
                        break;

                    case Token.Keys.KeyResponseFileWriteStart:
                        HandleFileWriteStartResponse(body);
                        break;

                    case Token.Keys.KeyResponseFileWriteSegment:
                        HandleFileWriteSegmentResponse(body);
                        break;

                    case Token.Keys.KeyResponseFileDelete:
                        HandleFileDeleteResponse(body);
                        break;

                    //	any other expected response is ignored, but logged
                    default:
                        _logger.Trace($"Response received unsupported expected response.");
                        if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                            Output.WriteLine("FTP Response: Received unsupported expected response. ({server.address})");
                        break;
                }
            }
            else  //  unexpected response
            {
                switch (responseKey)
                {
                    //  no request retries when server responds with file not found for read or delete
                    case Token.Keys.KeyResponseFileNotFound:
                        if (server.expectedResponse == Token.Keys.KeyResponseFileIndexedInfo
                         || server.expectedResponse == Token.Keys.KeyResponseFileInfo
                         || server.expectedResponse == Token.Keys.KeyResponseFileReadStart
                         || server.expectedResponse == Token.Keys.KeyResponseFileDelete)
                        {
                            EndTransaction(responseKey);
                        }
                        break;

                    //  no request retries when server responds with disk full for write
                    case Token.Keys.KeyResponseFtpDiskFull:
                        if (server.expectedResponse == Token.Keys.KeyResponseFileWriteStart)
                        {
                            EndTransaction(responseKey);
                        }
                        break;

                    //	any other unexpected response is ignored, but logged
                    default:
                        _logger.Trace($"Received unexpected response.");
                        if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                            Output.WriteLine($"FTP Response: Received unexpected response. ({server.address})");
                        break;
                }
            }
        }


        /////////////////////////////////////////////////////////////////////////////
        //
        //	READ FILE OR FILE INFO FROM SERVER

        /// <summary>
        /// Requests that the server read a file segment.
        /// </summary>
        /// <param name="isRetry">True if the message is a retry.</param>
        void RequestReadSegment(bool isRetry)
        {
            // DON'T ADD LOGGING HERE - IT SEEMS TO CAUSE ISSUES WITH DETECTION

            //	initialize the transmitter
            var tx = new Transmitter(core);
            tx.StartMessage(server.address);

            //	send the request key
            tx.AddInt16((UInt16)Token.Keys.KeyRequestFileReadSegment);

            //	send the segment index
            tx.AddInt16(file.segmentIndex);

            //	send the server access code
            tx.AddInt32(server.accessCode);

            //	send request
            FinishRequest(Token.Keys.KeyResponseFileReadSegment, tx, isRetry);

            //  print segment request to console (if debug level enabled)
            if (ECCONetCore.ConsoleDebugMessageLevels.FtpRequestsAndResponses <= core.consoleDebugMessageLevel)
                Output.WriteLine($"FTP Request: {Token.Keys.KeyRequestFileReadSegment} ({server.address})");
        }

        /// <summary>
        /// Handles the server response for a file info or file read start request.
        /// </summary>
        /// <param name="response">The server response.</param>
        /// <param name="body">A pointer to the message body.</param>
        void HandleFileInfoAndReadStartResponse(Token.Keys response, Byte[] body)
        {
            String filename;
            UInt16 filenameLen, i, n, m;
            UInt32 timestamp;

            //  server must provide valid file name
            filenameLen = ValidateFileName(body);
            if (0 == filenameLen)
            {
                //  print error to console (if debug level enabled)
                _logger.Trace($"Read start returned bad file name.");
                if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                    Output.WriteLine($"FTP Response: Read start returned bad file name. ({server.address})");
                EndTransaction(Token.Keys.KeyResponseFtpServerError);
                return;
            }
            filename = System.Text.Encoding.UTF8.GetString(body, 0, filenameLen);

            //	if getting indexed file info, then set file name
            if (Token.Keys.KeyResponseFileIndexedInfo == response)
                file.name = String.Copy(filename);

            //  else server must provide file name that matches request
            else if (!filename.Equals(file.name))
            {
                //  print error to console (if debug level enabled)
                _logger.Trace($"Read start returned bad file name.");
                if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                    Output.WriteLine($"FTP Response: Read start returned wrong file name. ({server.address})");
                EndTransaction(Token.Keys.KeyResponseFtpServerError);
                return;
            }

            //	server response body size must includes data size, data checksum and timestamp
            if (body.Length < (filenameLen + (1 + 4 + 2 + 4)))
            {
                //  print error to console (if debug level enabled)
                _logger.Trace($"Read start body is too small.");
                if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                    Output.WriteLine($"FTP Response: Read start body is too small. ({server.address})");
                EndTransaction(Token.Keys.KeyResponseFtpServerError);
                return;
            }

            //	get the file data size, checksum and timestamp
            i = (UInt16)(filenameLen + 1);
            for (n = 0; n < 4; ++n)
            {
                file.dataSize <<= 8;
                file.dataSize |= body[i++];
            }
            for (n = 0; n < 2; ++n)
            {
                file.dataChecksum <<= 8;
                file.dataChecksum |= body[i++];
            }
            timestamp = 0;
            for (n = 0; n < 4; ++n)
            {
                timestamp <<= 8;
                timestamp |= body[i++];
            }
            file.date = new DateTime(2017, 1, 1, 0, 0, 0);
            file.date = file.date.AddSeconds(timestamp);

            //	if this was a product info file info request, and body has 16 additional guid bytes
            if ((Token.Keys.KeyResponseFileInfo == response) && filename.Equals(ECCONetApi.FileName.ProductInfo)
                && (body.Length >= (filenameLen + (1 + 4 + 2 + 4 + 16))))
            {
                //  get the guid and convert to server access code
                server.guid = new UInt32[4];
                for (m = 0; m < 4; ++m)
                {
                    for (n = 0; n < 4; ++n)
                    {
                        server.guid[m] <<= 8;
                        server.guid[m] |= body[i++];
                    }
                }
                server.accessCode = GenerateServerAccessCode(server.guid);
            }

            //  a zero data size indicates that the file was not found
            if (0 == file.dataSize)
            {
                //  print error to console (if debug level enabled)
                _logger.Trace($"Read start data size is zero.");
                if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                    Output.WriteLine($"FTP Response: Read start data size is zero. ({server.address})");
                EndTransaction(Token.Keys.KeyResponseFileNotFound);
                return;
            }

            //	if this was a file read request
            if (Token.Keys.KeyResponseFileReadStart == response)
            {
                //  create the file data buffer
                file.data = new byte[file.dataSize];

                //	clear segment index and make first segment request
                file.segmentIndex = 0;
                RequestReadSegment(false);
            }
            else  //  a file info request
            { 
                file.data = new byte[0];
                EndTransaction(Token.Keys.KeyResponseFileInfoComplete);
            }
        }

        /// <summary>
        /// Handles a file read segment server response.
        /// </summary>
        /// <param name="body">A pointer to the message body.</param>
        void HandleFileReadSegmentResponse(Byte[] body)
        {
            UInt16 segmentIndex;
            Token.Keys key;
            Int32 dataIndex, numCopyBytes;

            //	server response must have a body of at least 3 bytes
            if ((null == body) || (3 > body.Length))
            {
                //  print error to console (if debug level enabled)
                _logger.Trace($"Read segment body is too small.");
                if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                    Output.WriteLine($"FTP Response: Read segment body is too small. ({server.address})");
                EndTransaction(Token.Keys.KeyResponseFtpServerError);
                return;
            }

            //	server response must match the requested segment index
            segmentIndex = body[0];
            segmentIndex = (UInt16)((segmentIndex << 8) | body[1]);
            if (segmentIndex != file.segmentIndex)
            {
                //  print error to console (if debug level enabled)
                _logger.Trace($"Read segment returned wrong segment index.");
                if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                    Output.WriteLine($"FTP Response: Read segment returned wrong segment index. ({server.address})");
                EndTransaction(Token.Keys.KeyResponseFtpServerError);
                return;
            }

            //  get least of file bytes and requester buffer bytes remaining
            dataIndex = (Int32)(file.segmentIndex * LibConfig.MATRIX_MAX_FILE_SEGMENT_LENGTH);
            numCopyBytes = (Int32)Math.Min(body.Length - 2, file.data.Length - dataIndex);

            //  copy bytes to requestor buffer
            for (int i = 0; i < numCopyBytes; ++i)
                file.data[dataIndex++] = body[i + 2];

            //  if whole file transferred then done
            if ((0 >= (file.dataSize - dataIndex)) || (0 >= (file.data.Length - dataIndex)))
            {
                //	verify file data checksum
                if (dataIndex > file.data.Length)
                    dataIndex = file.data.Length;

                ushort checksum = Crc.ComputeCRC16(file.data, (UInt32)dataIndex);

                key = (file.dataChecksum ==
                    Crc.ComputeCRC16(file.data, (UInt32)dataIndex)) ?
                    Token.Keys.KeyResponseFileReadComplete :
                    Token.Keys.KeyResponseFileChecksumError;

                //	complete transaction with checksum result
                EndTransaction(key);
            }

            //  else more data to go, so request another segment
            else
            {
                ++file.segmentIndex;
                RequestReadSegment(false);
            }
        }


        /////////////////////////////////////////////////////////////////////////////
        //
        //	WRITE FILE TO SERVER

        /// <summary>
        /// Requests that the server write a file segment.
        /// </summary>
        /// <param name="isRetry">True if the message is a retry.</param>
        void RequestWriteSegment(bool isRetry)
        {
            // DON'T ADD LOGGING HERE - IT SEEMS TO CAUSE ISSUES WITH DETECTION

            Int32 dataIndex, numCopyBytes;

            //	get the bytes remaining to write
            dataIndex = (Int32)(file.segmentIndex * LibConfig.MATRIX_MAX_FILE_SEGMENT_LENGTH);
            numCopyBytes = (Int32)Math.Min(file.dataSize - dataIndex,
                LibConfig.MATRIX_MAX_FILE_SEGMENT_LENGTH);
            if (0 >= numCopyBytes)
            {
                EndTransaction(Token.Keys.KeyResponseFileWriteComplete);
                return;
            }

            //	initialize the transmitter
            var tx = new Transmitter(core);
            tx.StartMessage(server.address);

            //	send the request key
            tx.AddInt16((UInt16)Token.Keys.KeyRequestFileWriteSegment);

            //	send the segment index
            tx.AddInt16(file.segmentIndex);

            //	send the server access code
            tx.AddInt32(server.accessCode);

            //	send the segment data
            while (0 != numCopyBytes--)
                tx.AddByte(file.data[dataIndex++]);

            //	send request
            FinishRequest(Token.Keys.KeyResponseFileWriteSegment, tx, isRetry);

            //  print segment request to console (if debug level enabled)
            if (ECCONetCore.ConsoleDebugMessageLevels.FtpRequestsAndResponses <= core.consoleDebugMessageLevel)
                Output.WriteLine($"FTP Request: {Token.Keys.KeyRequestFileWriteSegment} ({server.address})");
        }

        /// <summary>
        /// Handles a file write start server response.
        /// </summary>
        /// <param name="body">A pointer to the message body.</param>
        void HandleFileWriteStartResponse(Byte[] body)
        {
            String filename;
            UInt16 filenameLen;

            //  server must provide valid file name
            filenameLen = ValidateFileName(body);
            if (0 == filenameLen)
            {
                //  print error to console (if debug level enabled)
                _logger.Trace($"Write start returned bad file name.");
                if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                    Output.WriteLine($"FTP Response: Write start returned bad file name. ({server.address})");
                EndTransaction(Token.Keys.KeyResponseFtpServerError);
                return;
            }

            //	server response file name must match the request file name
            filename = System.Text.Encoding.UTF8.GetString(body, 0, filenameLen);
            if (0 != String.Compare(filename, file.name))
            {
                //  print error to console (if debug level enabled)
                _logger.Trace($"Write start returned wrong file name.");
                if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                    Output.WriteLine($"FTP Response: Write start returned wrong file name. ({server.address})");
                EndTransaction(Token.Keys.KeyResponseFtpServerError);
                return;
            }

            //	clear number of bytes transferred and write the first segment
            file.segmentIndex = 0;
            RequestWriteSegment(false);
        }

        /// <summary>
        /// Handles a file write segment server response.
        /// </summary>
        /// <param name="body">A pointer to the message body.</param>
        void HandleFileWriteSegmentResponse(Byte[] body)
        {
            UInt16 segmentIndex;

            //	server response must return a body of least 2 bytes for segment index
            if ((null == body) || (2 > body.Length))
            {
                //  print error to console (if debug level enabled)
                _logger.Trace($"Write segment body is too small.");
                if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                    Output.WriteLine($"FTP Response: Write segment body is too small. ({server.address})");
                EndTransaction(Token.Keys.KeyResponseFtpServerError);
                return;
            }

            //	server response must match the requested segment index
            segmentIndex = body[0];
            segmentIndex = (UInt16)((segmentIndex << 8) | body[1]);
            if (segmentIndex != file.segmentIndex)
            {
                //  print error to console (if debug level enabled)
                _logger.Trace($"Write segment returned wrong segment index.");
                if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                    Output.WriteLine($"FTP Response: Write segment returned wrong segment index. ({server.address})");
                EndTransaction(Token.Keys.KeyResponseFtpServerError);
                return;
            }

            //	write the next segment
            ++file.segmentIndex;
            RequestWriteSegment(false);
        }


        /////////////////////////////////////////////////////////////////////////////
        //
        //	ERASE FILE ON SERVER

        /// <summary>
        /// Handles a file erase server response.
        /// </summary>
        /// <param name="body">A pointer to the message body.</param>
        void HandleFileDeleteResponse(Byte[] body)
        {
            String filename;
            UInt16 filenameLen;

            //  server must provide valid file name
            filenameLen = ValidateFileName(body);
            if (0 == filenameLen)
            {
                //  print error to console (if debug level enabled)
                _logger.Trace($"Write start returned bad file name.");
                if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                    Output.WriteLine($"FTP Response: Write start returned bad file name. ({server.address})");
                EndTransaction(Token.Keys.KeyResponseFtpServerError);
                return;
            }

            //	server response file name must match the request file name
            filename = System.Text.Encoding.UTF8.GetString(body, 0, filenameLen);
            if (0 != String.Compare(filename, file.name))
            {
                //  print error to console (if debug level enabled)
                _logger.Trace($"Write start returned wrong file name.");
                if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                    Output.WriteLine($"FTP Response: Write start returned wrong file name. ({server.address})");
                EndTransaction(Token.Keys.KeyResponseFtpServerError);
                return;
            }

            //  end transaction
            EndTransaction(Token.Keys.KeyResponseFileDeleteComplete);
        }
        #endregion

    }
}