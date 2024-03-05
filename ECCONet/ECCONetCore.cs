using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
//using System.Timers;

namespace ECCONet
{
    /// <summary>
    /// The ECCONet core functionality class.
    /// </summary>
public class ECCONetCore
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        //  the receiver
        public Receiver receiver { get; set; }

        //  the transmitter
        //public Transmitter transmitter { get; set; }

        //  the event index
        public EventIndex eventIndex { get; set; }

        //  the codec
        public Codec codec { get; set; }

        //  the ftp client
        //public FtpClient ftpClient { get; set; }

        //  a list of ftp clients
        List<FtpClient> ftpClientList = new List<FtpClient>(10);

        //  ftp client list lock
        object ftpClientListLock = new object();


        //  the online devices monitor
        public OnlineDeviceMonitor deviceMonitor { get; set; }

        /// <summary>
        /// Indicates whether the core should automatically transmit an Active
        /// system power state once per second.
        /// </summary>
        public bool shouldTransmitSystemPowerState { get; set; }

        /// <summary>
        /// The console debug messages level (see consoleDebugMessageLevels enum).
        /// </summary>
        public ConsoleDebugMessageLevels consoleDebugMessageLevel { get; set; }

        /// <summary>
        /// FTP server response timeout in number of milliseconds
        /// </summary>
        public int ftpServerResponseTimeout { get; set; } = 4000;

        /// <summary>
        /// The console debug message levels.
        /// </summary>
        public enum ConsoleDebugMessageLevels
        {
            None,
            Errors,
            FtpRequestsAndResponses,
            Frames
        }

        //	a process manager timer compatible with .NET Core
        Timer processManagerTimer;

        //  the processor manager critical section lock
        Object processManagerLock;

        //  the processor manager busy
        Byte processManagerBusy;

        //  the beacon timer
        //int beaconTimer = 0;

        //  the system power state timer
        int powerStateTimer = 0;

        //  the processor manager timer period
        public const Int32 ProcessManagerTimerPeriod = 15;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ECCONetCore()
        {
            //  create the modules
            eventIndex = new EventIndex();
            codec = new Codec();
            receiver = new Receiver(this);
            //transmitter = new Transmitter(this);
            //ftpClient = new FtpClient(this);
            deviceMonitor = new OnlineDeviceMonitor(this);

            //  enable system power state beacon
            shouldTransmitSystemPowerState = false;

            //  create clock lock
            processManagerLock = new Object();

            //	create the process manager timer
            processManagerTimer = new Timer(new TimerCallback(processManagerTimerCallback), null, 0, ProcessManagerTimerPeriod);
        }
   
        /// <summary>
        /// The process manager timer callback.
        /// </summary>
        /// <param name="state">The user object state.</param>
        void processManagerTimerCallback(object state)
        {
            //  if already executing a timer callback,
            //  then skip this one
            if (0 != processManagerBusy)
                return;

            //  critical code section
            lock (processManagerLock)
            {
                try
                {
                    //  set busy flag
                    processManagerBusy = 1;

                    //  clock the modules
                    receiver.Clock();
                    deviceMonitor.Clock();
                    AccessFtpClients(FtpClientListAccessMode.Clock, null, 0, 0, null);

                    //  transmit the power state
                    if (shouldTransmitSystemPowerState && (++powerStateTimer >= (400 / 15)))
                    {
                        //  clear timer
                        powerStateTimer = 0;

                        //	send as status message
                        var tx = new Transmitter(this);
                        tx.StartMessageWithAddressAndKey(0, Token.Keys.KeyNull);
                        tx.AddInt16((UInt16)Token.Keys.KeySystemPowerState + ((int)Token.KeyPrefix.InputStatus << 8));
                        tx.AddByte(1);
                        tx.FinishMessage();

                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                //  clear busy flag
                processManagerBusy = 0;
            }
        }

        public void StartProcessManager()
        {
            processManagerTimer.Change(0, ProcessManagerTimerPeriod);
        }

        public void StopProcessManager()
        {
            processManagerTimer.Change(0, 0);
        }

        /// <summary>
        /// Gets the CAN address of this device, which is always 126.
        /// </summary>
        /// <returns>The CAN address of this device, which is always 126.</returns>
        public Byte GetCanAddress()
        {
            return ECCONetApi.PC_CAN_Address;
        }

        #region Core send and receive token

        /// <summary>
        /// A delegate for the application to receive a token.
        /// </summary>
        /// <param name="token">The received token.</param>
        public delegate void ReceiveTokenDelegate(Token token);

        /// <summary>
        /// The delegate for the application to receive a token.
        /// </summary>
        public ReceiveTokenDelegate receiveTokenDelegate { get; set; }

        /// <summary>
        /// Receives a token from the CAN bus and sends to the application.
        /// </summary>
        /// <param name="token">The token to receive.</param>
        public void ReceiveTokenFromCanBus(Token token)
        {
            receiveTokenDelegate?.Invoke(token);
        }

        /// <summary>
        /// Sends a token over the CAN bus.
        /// </summary>
        /// <param name="token">A token to send.</param>
        /// <returns>Returns zero on success, else -1.</returns>
        public int SendTokenToCanBus(Token token)
        {
            bool isInputEvent;

            //  validate token
            if (token == null)
            {
                Debug.WriteLine("Null token sent to transmitter!");
                return -1;
            }

            //	get input event status
            isInputEvent = (Token.KeyPrefix.InputStatus == Token.Key_GetPrefix(token.key));

            //	bump the event index for input events
            if (isInputEvent)
            {
                eventIndex.NextEventIndex();
                //Debug.WriteLine("Input status sent, event index {0}", core.eventIndex.GetEventIndex());
            }

            //	send the token once, or three times for input events
            for (int i = 0; i < (isInputEvent ? 3 : 1); ++i)
            {
                //	initialize the transmitter
                var tx = new Transmitter(this);
                tx.StartMessageWithAddressAndKey(token.address, token.key);

                //  add the token key to the byte stream
                tx.AddInt16((UInt16)token.key);

                //  add the token value to the byte stream
                //  get token size
                int valueSize = token.ValueSize;
                while (--valueSize >= 0)
                    tx.AddByte((byte)(token.value >> (8 * valueSize)));

                //	send remaining part of message in fifo
                tx.FinishMessage();
            }
            return 0;
        }
        #endregion

        #region Core send and receive CAN frames

        /// <summary>
        /// A delegate to send a frame to the CAN bus.
        /// </summary>
        /// <param name="id">The frame id.</param>
        /// <param name="data">The frame data.</param>
        public delegate int SendFrameDelegate(UInt32 id, byte[] data);

        /// <summary>
        /// The delegate to send a frame to the CAN bus.
        /// </summary>
        public SendFrameDelegate sendFrameDelegate { get; set; }

        /// <summary>
        /// Sends a CAN frame.
        /// </summary>
        /// <param name="id">The frame id.</param>
        /// <param name="data">The frame data.</param>
        public int SendCanFrame(UInt32 id, byte[] data)
        {
            if (null != sendFrameDelegate)
                return sendFrameDelegate(id, data);
            return -1;
        }

        /// <summary>
        /// Receives a CAN frame.
        /// </summary>
        /// <param name="id">The frame id.</param>
        /// <param name="data">The frame data.</param>
        public void ReceiveCanFrame(UInt32 id, byte[] data)
        {
            receiver.ReceiveCanFrame(id, data);
        }

        #endregion

        #region Core FTP client handler
        /// <summary>
        /// The FTP client list access mode.
        /// </summary>
        enum FtpClientListAccessMode
        {
            RequestTransaction,
            HandleServerResponse,
            CheckIsBusy,
            Clock,
        }

        /// <summary>
        /// Access the list of FTP clients.
        /// </summary>
        /// <param name="mode">Access mode.</param>
        /// <param name="request">FTP transaction request, optional with mode.</param>
        /// <param name="serverAddress">Server address, optional with mode.</param>
        /// <param name="responseKey">Server response key, optional with mode.</param>
        /// <param name="body">Server response body, optional with mode.</param>
        /// <returns>Returns 0 on success, else -1.</returns>
        int AccessFtpClients(FtpClientListAccessMode mode, FtpClient.FtpTransactionRequest request, UInt16 serverAddress, Token.Keys responseKey, Byte[] body)
        {
            // DON'T ADD LOGGING HERE - IT SEEMS TO CAUSE ISSUES WITH DETECTION

            //  make sure lock exists
            if (null == ftpClientListLock)
                ftpClientListLock = new object();

            lock (ftpClientListLock)
            {
                bool found = false;
                int status = 0;

                //  make sure list is initialized
                if (ftpClientList == null)
                    ftpClientList = new List<FtpClient>(10);

                switch (mode)
                {
                    case FtpClientListAccessMode.RequestTransaction:
                        //  validate request
                        if (request == null)
                        {
                            _logger.Error("FTP Request Transaction is null - status set to -1");
                            status = -1;
                        }
                        else
                        {
                            //  if already have client for server, add request
                            found = false;
                            foreach (var client in ftpClientList)
                            {
                                if (client.server.address == request.serverAddress)
                                {
                                    found = true;
                                    status = client.RequestTransaction(request);
                                    _logger.Trace($"FTP Request Transaction - status set to {status}");
                                    break;
                                }
                            }
                            //  else no client found with server address
                            if (!found)
                            {
                                var newClient = new FtpClient(this);
                                ftpClientList.Add(newClient);
                                status = newClient.RequestTransaction(request);
                                _logger.Trace($"FTP Request Transaction - status set to {status}");
                            }
                        }
                        break;

                    case FtpClientListAccessMode.HandleServerResponse:
                        //  validate body
                        if (body == null)
                        {
                            _logger.Error($"FTP Response Transaction body is null - status set to -1");
                            status = -1;
                        }
                        else
                        {
                            //  handle server response by matching client
                            foreach (var client in ftpClientList)
                            {
                                if (client.server.address == serverAddress)
                                {
                                    client.ServerResponseIn(serverAddress, responseKey, body);
                                    _logger.Trace($"FTP Response Transaction - response key is {responseKey}, body is {body.Length} bytes");
                                    break;
                                }
                            }
                        }
                        break;

                    case FtpClientListAccessMode.CheckIsBusy:
                        //  check for existing client and if busy
                        found = false;
                        foreach (var client in ftpClientList)
                        {
                            if (client.server.address == serverAddress)
                            {
                                status = client.IsBusy() ? -1 : 0;
                                if (status == -1)
                                {
                                    _logger.Error($"FTP Transaction Mode - status set to {status}");
                                }
                                else
                                {
                                    _logger.Trace($"FTP Transaction Mode - status set to {status}");
                                }
                                break;
                            }
                        }
                        break;

                    case FtpClientListAccessMode.Clock:
                        foreach (var client in ftpClientList)
                            client.Clock();
                        break;

                    default:
                        break;
                }
                return status;
            }
        }

        /// <summary>
        /// Adds a transaction request to list of transactions to perform.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns 0 on success, else -1 if list is full.</returns>
        public int FTP_TransactionRequest(FtpClient.FtpTransactionRequest request)
        {
            // DON'T ADD LOGGING HERE - IT SEEMS TO CAUSE ISSUES WITH DETECTION

            return AccessFtpClients(FtpClientListAccessMode.RequestTransaction, request, 0, 0, null);
        }

        /// <summary>
        /// Handles incoming server responses.
        /// </summary>
        /// <param name="senderAddress">The CAN address of the sender.</param>
        /// <param name="responseKey">The token response key.</param>
        /// <param name="body">An byte array of the message body.</param>
        public void FTP_ServerResponseIn(UInt16 senderAddress, Token.Keys responseKey, Byte[] body)
        {
            // DON'T ADD LOGGING HERE - IT SEEMS TO CAUSE ISSUES WITH DETECTION

            //_logger.Trace($"FTP transaction response received: {responseKey}");
            AccessFtpClients(FtpClientListAccessMode.HandleServerResponse, null, senderAddress, responseKey, body);
        }

        /// <summary>
        /// Handles incoming server responses.
        /// </summary>
        /// <param name="serverAddress">The CAN address of the server.</param>
        public bool FTP_ServerIsBusy(byte serverAddress)
        {
            // DON'T ADD LOGGING HERE - IT SEEMS TO CAUSE ISSUES WITH DETECTION

            //_logger.Trace($"FTP transaction server is busy: Address ({serverAddress})");
            return (AccessFtpClients(FtpClientListAccessMode.CheckIsBusy, null, serverAddress, 0, null) != 0);
        }
        #endregion
    }
}
