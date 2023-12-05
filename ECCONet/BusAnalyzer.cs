using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;


namespace ECCONet
{
    public partial class ECCONetApi
    {
        /// <summary>
        /// The mode when running a ping test.
        /// </summary>
        public enum PingTestMode
        {
            CanAddressRequest,
            ReadInfoFile,
            WriteNamedFile,
            RandomWriteRead,
        }

        /// <summary>
        /// A callback for bus analysis progress updates.
        /// </summary>
        /// <param name="sender">The callback sender.
        /// <param name="percentComplete">The percentage of test completion, 0 to 100.</param>
        /// <param name="address">The device address about to be tested.</param>
        public delegate void BusAnalysisProgressDelegate(object sender, int percentComplete, byte address);

        /// <summary>
        /// A callback for when the bus analysis completes.
        /// </summary>
        /// <param name="sender">The callback sender.
        /// <param name="results">The full or partial results of the test.</param>
        public delegate void BusAnalysisCompleteDelegate(object sender, BusAnalysisStatistics results);

        /// <summary>
        /// A callback for the bus flood statistics.
        /// </summary>
        /// <param name="sender">The callback sender.
        /// <param name="results">The bus flood statistics.</param>
        public delegate void BusFloodDelegate(object sender, BusFloodStatistics results);

        /// <summary>
        /// A callback for number of tokens sent.
        /// </summary>
        /// <param name="sender">The callback sender.
        /// <param name="numTokensSent">The number of tokens sent.</param>
        public delegate void TokenToggleDelegate(object sender, int numTokensSent);

        /// <summary>
        /// A class for providing the bus flood statistics.
        /// </summary>
        public class BusFloodStatistics
        {
            /// <summary>
            /// The number of packets sent.
            /// </summary>
            public int NumPacketsSent;

            /// <summary>
            /// The number of messages sent.
            /// </summary>
            public int NumMessagesSent;

            /// <summary>
            /// The number of packets sent per second.
            /// </summary>
            public double ActualPacketsPerSecond;

            /// <summary>
            /// The approximate bus utilization.
            /// </summary>
            public double BusUtilizationPercent;
        }

        /// <summary>
        /// A class for providing the statistics for communications with a node or a set of nodes.
        /// </summary>
        public class BusPingNodeStatistics
        {
            /// <summary>
            /// The ECCONet device that was tested.
            /// </summary>
            public ECCONetApi.ECCONetDevice Device;
            
            /// <summary>
            /// The duration of the test.
            /// </summary>
            public TimeSpan TestDuration;

            /// <summary>
            /// The number of packets sent during the test.
            /// </summary>
            public uint PacketsSent;

            /// <summary>
            /// The number of packets received during the test.
            /// </summary>
            public uint PacketsReceived;

            /// <summary>
            /// The minimum response time in milliseconds.
            /// </summary>
            public uint MinimumResponseTime;

            /// <summary>
            /// The maximum response time in milliseconds.
            /// </summary>
            public uint MaximumResponseTime;

            /// <summary>
            /// The average response time in milliseconds.
            /// </summary>
            public double AverageResponseTime;

            /// <summary>
            /// The error rate.
            /// </summary>
            public double ErrorRate;
        }

        /// <summary>
        /// A class for providing the bus analysis statistics.
        /// </summary>
        public class BusAnalysisStatistics
        {
            /// <summary>
            /// Results for individual nodes.
            /// </summary>
            public List<BusPingNodeStatistics> Nodes;

            /// <summary>
            /// The aggreate results.
            /// </summary>
            public BusPingNodeStatistics Aggregate;
        }
    }

    /// <summary>
    /// A class for analyzing the CAN bus integrity.
    /// </summary>
    internal class BusAnalyzer
    {
        //  the core logic
        ECCONetCore core;

        #region Test Params class
        /// <summary>
        /// Class to pass test params to background worker.
        /// </summary>
        private class TestParams
        {
            /// <summary>
            /// The ECCONet devices.
            /// </summary>
            public List<ECCONetApi.ECCONetDevice> devices;

            /// <summary>
            /// The test duration.
            /// </summary>
            public TimeSpan duration;

            /// <summary>
            /// The number of pings per node.
            /// </summary>
            public uint numPingsPerNode;

            /// <summary>
            /// The ping test mode.
            /// </summary>
            public ECCONetApi.PingTestMode mode;

            /// <summary>
            /// The 8.3 file name for writing a file.
            /// </summary>
            public string fileName;

            /// <summary>
            /// The file data for writing a file.
            /// </summary>
            public byte[] fileData;

            /// <summary>
            /// The maximum random file size.
            /// </summary>
            public int randomFileSize;
        }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="core">The core logic group.</param>
        public BusAnalyzer(ECCONetCore core)
        {
            //  save the core
            this.core = core;
        }

        /// <summary>
        /// Deconstructor.
        /// </summary>
        ~BusAnalyzer()
        {
            StopAll();
        }


        public void StopAll()
        {
            BusFloodStop();
            StopPinging();
            TokenToggleStop();
        }


        #region Bus flood
        //  min and max packets per message
        const int busFloodMinPacketsPerMessage = 1;
        const int busFloodMaxPacketsPerMessage = 25;

        //  min and max messages per second
        const int busFloodMinMessagesPerSecond = 1;
        const int busFloodMaxMessagesPerSecond = 50;

        //  the bus flood callback delegate
        private ECCONetApi.BusFloodDelegate busFloodDelegate = null;

        //  the token to send
        Token floodToken = null;

        //  the number of packets per bus flood message
        uint numPacketsPerBusFloodMessage;

        //  a message timer critical section lock
        Object busFloodTimerLock = new object();

        //  a message timer busy flag
        Byte busFloodTimerBusy;

        //  the bus flood timer
        private System.Threading.Timer busFloodTimer;

        //  the bus flood callback timer
        private System.Threading.Timer busFloodCallbackTimer;

        //  the bus flood stopwatch
        private Stopwatch busFloodStopwatch;

        //  the bus flood statistics
        ECCONetApi.BusFloodStatistics busFloodStatistics = null;


        /// <summary>
        /// Gets a value indicating whether the bus flood is running.
        /// </summary>
        public bool IsBusFloodRunning
        {
            get => (busFloodTimer != null);
        }

        /// <summary>
        /// Floods the bus with messages at the given message size and rate.
        /// A bus flood message is a series of tokens all transmitted together.
        /// </summary>
        /// <param name="numPacketsPerMessage">The number of packets per message (min=1, max=25).</param>
        /// <param name="numMessagesPerSecond">The number of messages per second (min=1, max=50).</param>
        /// <param name="token">The token to use, or null to use the default token.</param>
        /// <param name="callback">A method to call back once every 500 mS with statistics, or null.</param>
        /// <returns>Returns 0 if flood started, -1 if invalid packets per message, -2 if invalid messages per second.</returns>
        public int FloodBusStart(uint numPacketsPerMessage, uint numMessagesPerSecond, Token token, ECCONetApi.BusFloodDelegate callback)
        {
            //  validate packets per message
            if ((numPacketsPerMessage < busFloodMinPacketsPerMessage) || (numPacketsPerMessage > busFloodMaxPacketsPerMessage))
                return - 1;
  
            //  validate messages per second
            if ((numMessagesPerSecond < busFloodMinMessagesPerSecond) || (numMessagesPerSecond > busFloodMaxMessagesPerSecond))
                return -2;

            //  save flood parameters
            numPacketsPerBusFloodMessage = numPacketsPerMessage;
            busFloodDelegate = callback;

            //  set the token
            if (token == null)
                token = new Token(Token.Keys.KeyIndexedTokenSequencerWithPattern, 0, 127);
            floodToken = token;

            //  create a new bus flood statistics
            busFloodStatistics = new ECCONetApi.BusFloodStatistics();

            //	start a new bus flood timer
            busFloodTimer = new System.Threading.Timer(new TimerCallback(busFloodTimerCallback), null, 0, 1000 / (int)numMessagesPerSecond);

            //	start a new bus flood callback timer
            busFloodCallbackTimer = new System.Threading.Timer(new TimerCallback(busFloodCallbackTimerCallback), null, 0, 500);

            //  start the stopwatch
            busFloodStopwatch = Stopwatch.StartNew();

            //  return success
            return 0;
        }

        /// <summary>
        /// The bus flood timer callback.
        /// </summary>
        /// <param name="state">The user object state.</param>
        void busFloodTimerCallback(object state)
        {
            //  if already executing a timer callback,
            //  then skip this one
            if (0 != busFloodTimerBusy)
                return;

            //  critical code section
            lock (busFloodTimerLock)
            {
                //  set busy flag
                busFloodTimerBusy = 1;

                //  send packets
                for (int i = 0; i < numPacketsPerBusFloodMessage; ++i)
                {
                    //  send a packet
                    core.SendTokenToCanBus(floodToken);
                }

                //  update readouts
                if (busFloodStatistics != null)
                {
                    busFloodStatistics.NumPacketsSent += (int)numPacketsPerBusFloodMessage;
                    ++busFloodStatistics.NumMessagesSent;
                }

                //  clear busy flag
                busFloodTimerBusy = 0;
            }
        }

        /// <summary>
        /// The bus flood callback timer callback.
        /// </summary>
        /// <param name="state">The user object state.</param>
        void busFloodCallbackTimerCallback(object state)
        {
            if ((busFloodStatistics != null) && (busFloodDelegate != null))
            {
                busFloodStatistics.ActualPacketsPerSecond =
                    (int)((float)busFloodStatistics.NumPacketsSent / ((float)busFloodStopwatch.ElapsedMilliseconds / 1000.0));
                busFloodStatistics.BusUtilizationPercent = (float)busFloodStatistics.ActualPacketsPerSecond * 0.97 / 10.0;
                busFloodDelegate(this, busFloodStatistics);
            }
        }

        /// <summary>
        /// Stops the bus flood.
        /// </summary>
        public void BusFloodStop()
        {
            if (busFloodTimer != null)
                busFloodTimer.Dispose();
            if (busFloodCallbackTimer != null)
                busFloodCallbackTimer.Dispose();
            if (busFloodStopwatch != null)
                busFloodStopwatch.Stop();
        }
        #endregion

        #region Node single token ping or FTP transaction
        //  the maximum reply period in mS
        private const uint maximumReplyPeriod = 30000;

        //  the analysis progress delegate.
        private ECCONetApi.BusAnalysisProgressDelegate analysisProgressDelegate = null;

        //  the analysis complete delegate.
        private ECCONetApi.BusAnalysisCompleteDelegate analysisCompleteDelegate = null;

        // a background worker to ping the nodes
        private BackgroundWorker backgroundWorker;

        //  an atomic percent complete
        private byte percentComplete = 0;

        //  the percent complete reporting timer
        private System.Threading.Timer percentCompleteTimer;

        /// <summary>
        /// Gets a value indicating whether a bus statistics ping test is running.
        /// </summary>
        public bool IsBusStatisticsPingTestRunning
        {
            get
            {
                if (backgroundWorker != null)
                    return backgroundWorker.IsBusy;
                return false;
            }
        }

        /// <summary>
        /// Stops any node pinging that is currently running.
        /// </summary>
        public void StopPinging()
        {
            if (backgroundWorker != null)
            {
                backgroundWorker.CancelAsync();
                percentCompleteTimer.Dispose();
            }
        }

        //  the percent complete timer callback
        private void PercentCompleteTimerCallback(object state)
        {
            if (serverAddress != 0)
                analysisProgressDelegate?.Invoke(this, percentComplete, serverAddress);
        }

        /// <summary>
        /// A method for analyzing the bus integrity by pinging individual nodes.
        /// Note that poor-performing nodes can falsely drive up the error rate.
        /// </summary>
        /// <param name="devices">A list of ECCONet devices to be pinged.</param>
        /// <param name="duration">The test duration, minimum is 100 mS.  Not used when a non-zero number of pings is given.</param>
        /// <param name="numPingsPerNode">The number of times to ping each node. A zero value uses the test duration.</param>
        /// <param name="mode">The ping mode.</param>
        /// <param name="filePath">The local path of a file to write, for write mode.</param>
        /// <param name="randomFileSize">For random file write/read/delete mode, the maximum file size.</param>
        /// <param name="progressCallback">A method to call back every 500 mS with the progress, or null.</param>
        /// <param name="completeCallback">The method to call back with the results, or null.</param>
        /// <returns>Returns 0 if test started, -1 if already busy running test, -2 on bad test parameters, or -3 on unable to read local file.</returns>
        public int PingNodes(List<ECCONetApi.ECCONetDevice> devices, TimeSpan duration, uint numPingsPerNode, ECCONetApi.PingTestMode mode,
            string filePath, int randomFileSize,
            ECCONetApi.BusAnalysisProgressDelegate progressCallback,
            ECCONetApi.BusAnalysisCompleteDelegate completeCallback)
        {
            //  if already running test, return false
            if ((backgroundWorker != null) && backgroundWorker.IsBusy)
                return -1;

            //  validate test params
            if (((duration.TotalSeconds < 0.1) && (numPingsPerNode == 0)) || (devices == null) || (devices.Count == 0))
                return -2;

            //  if writing to device, get local file data
            string fileName = string.Empty;
            byte[] fileData = null;
            if (mode == ECCONetApi.PingTestMode.WriteNamedFile)
            {
                //  check file exists
                if (!File.Exists(filePath))
                    return -2;

                //  check file is 8.3 format
                fileName = Path.GetFileName(filePath);
                var parts = Path.GetFileName(fileName).Split('.');
                if (parts.Length != 2 || parts[0].Length > 8 || parts[1].Length > 3)
                    return -2;

                //  try to read file
                try
                {
                    fileData = File.ReadAllBytes(filePath);
                }
                catch
                {
                    return -3;
                }
            }

            //  clear server address prior to starting progress callback
            serverAddress = 0;

            //  set the callbacks
            analysisProgressDelegate = progressCallback;
            analysisCompleteDelegate = completeCallback;

            //  start percent complete reporting
            percentComplete = 0;
            percentCompleteTimer = new Timer(new TimerCallback(PercentCompleteTimerCallback), null, 0, 500);

            //  pause the online monitor
            //core.deviceMonitor.Pause = true;

            //  setup background worker
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = false;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            //  start test
            backgroundWorker.RunWorkerAsync(new TestParams()
                { devices = devices, duration = duration, numPingsPerNode = numPingsPerNode, mode = mode,
                    fileName = fileName, fileData = fileData, randomFileSize = randomFileSize });

            //  return success
            return 0;
        }


        //  do work
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //  validate params
            if (!(e.Argument is TestParams testParams))
                return;

            //  create individual node test results
            var nodes = new List<ECCONetApi.BusPingNodeStatistics>(testParams.devices.Count);
            foreach (var device in testParams.devices)
                nodes.Add(new ECCONetApi.BusPingNodeStatistics()  { Device = device });

            //  save the results
            e.Result = nodes;


            //  if testing for a fixed number of pings per node
            if (testParams.numPingsPerNode != 0)
            {
                //  get total pings
                int totalPings = (int)(testParams.numPingsPerNode * testParams.devices.Count);

                //  for all nodes
                for (int index = 0; index < nodes.Count; ++index)
                {
                    //  get next address to test
                    var node = nodes[index];

                    //  initialize test
                    node.PacketsSent = 0;
                    node.PacketsReceived = 0;
                    node.MinimumResponseTime = maximumReplyPeriod;
                    node.MaximumResponseTime = 0;
                    Stopwatch durationTimer = Stopwatch.StartNew();
                    for (int i = 0; i < testParams.numPingsPerNode; ++i)
                    {
                        //  if cancellation pending, then return
                        if (backgroundWorker.CancellationPending)
                            return;

                        //  run a single test
                        bool status = RunSingleTest(testParams, node);

                        //  calculate progress
                        int pc = 100 * (i + (index * (int)testParams.numPingsPerNode)) / totalPings;
                        if (pc > 100)
                            pc = 100;
                        percentComplete = (byte)pc;
                    }

                    //  stop duration timer
                    durationTimer.Stop();

                    //  tabulate results
                    node.TestDuration = durationTimer.Elapsed;
                    if (node.PacketsReceived > 0)
                        node.AverageResponseTime /= (double)node.PacketsReceived;
                    else
                        node.AverageResponseTime = -1;
                    node.ErrorRate = 1.0 - ((double)node.PacketsReceived / (double)node.PacketsSent);
                }
            }
            else  //  else testing for a fixed duration
            {
                //  get the individual test period based on total period
                double individualTestPeriod = testParams.duration.TotalSeconds / (double)testParams.devices.Count;
                Stopwatch totalTestTimer = Stopwatch.StartNew();

                //  for all nodes
                for (int index = 0; index < nodes.Count; ++index)
                {
                    //  get next address to test
                    var node = nodes[index];

                    //  initialize test
                    node.PacketsSent = 0;
                    node.PacketsReceived = 0;
                    node.MinimumResponseTime = maximumReplyPeriod;
                    node.MaximumResponseTime = 0;
                    Stopwatch durationTimer = Stopwatch.StartNew();
                    while (durationTimer.Elapsed.TotalSeconds < individualTestPeriod)
                    {
                        //  if cancellation pending, then return
                        if (backgroundWorker.CancellationPending)
                            return;

                        //  run single test
                        RunSingleTest(testParams, node);

                        //  calculate progress
                        double pc = 100 * totalTestTimer.Elapsed.TotalSeconds / testParams.duration.TotalSeconds;
                        if (pc > 100.0)
                            pc = 100;
                        percentComplete = (byte)pc;  
                    }

                    //  stop timers
                    durationTimer.Stop();

                    //  tabulate results
                    node.TestDuration = durationTimer.Elapsed;
                    node.AverageResponseTime /= (double)node.PacketsReceived;
                    node.ErrorRate = 1.0 - ((double)node.PacketsReceived / (double)node.PacketsSent);
                }

                //  stop total test timer
                totalTestTimer.Stop();
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //  dispose of percent complete timer
            percentCompleteTimer.Dispose();

            //  wait for other devices to message the online monitor
            Thread.Sleep(2200);

            //  un-pause the online monitor
            //core.deviceMonitor.Pause = false;

            //  if valid results object
            if (e.Result is List<ECCONetApi.BusPingNodeStatistics> nodes)
            {
                //  get aggregate result
                var aggregate = new ECCONetApi.BusPingNodeStatistics();
                foreach (var node in nodes)
                {
                    aggregate.Device = null;
                    aggregate.PacketsSent += node.PacketsSent;
                    aggregate.PacketsReceived += node.PacketsReceived;
                    aggregate.TestDuration += node.TestDuration;
                    aggregate.AverageResponseTime += node.AverageResponseTime;
                    aggregate.ErrorRate += node.ErrorRate;
                }
                aggregate.AverageResponseTime /= (double)nodes.Count;
                aggregate.ErrorRate /= (double)nodes.Count;

                //  build statistics
                ECCONetApi.BusAnalysisStatistics busAnalysisStatistics = new ECCONetApi.BusAnalysisStatistics();
                busAnalysisStatistics.Aggregate = aggregate;
                busAnalysisStatistics.Nodes = nodes;

                //  call back
                analysisCompleteDelegate?.Invoke(this, busAnalysisStatistics);
            }
        }

        #endregion

        #region Single test

        //  an atomic server address
        private byte serverAddress = 0;

        //  an atomic callback received flag
        private byte callbackReceived = 0;

        //  the callback info
        ECCONetApi.FtpCallbackInfo callbackInfo;

        //  the random file name
        const string randFileName = "DevTool.tst";

        /// <summary>
        /// Receives tokens from the ECCONet core.
        /// </summary>
        /// <param name="token">The received token.</param>
        public void ReceiveToken(Token token)
        {
            if ((token.key == Token.Keys.KeyResponseAddressInUse) && (token.address == serverAddress))
                callbackReceived = 1;
        }

        /// <summary>
        /// The FTP server read transaction callback.
        /// </summary>
        /// <param name="callbackInfo">The callback data from the server.</param>
        private void FtpCallback(ECCONetApi.FtpCallbackInfo callbackInfo)
        {
            //  if callback from requested server
            if (callbackInfo.serverAddress == serverAddress)
            {
                callbackReceived = 1;
                this.callbackInfo = callbackInfo;
            }
        }

        /// <summary>
        /// Runs a single test with the given test params and node.
        /// </summary>
        /// <param name="testParams">The test parameters</param>
        /// <param name="node">The node being tested.</param>
        /// <returns>True if test passes, else false.</returns>
        private bool RunSingleTest(TestParams testParams, ECCONetApi.BusPingNodeStatistics node)
        {
            //  bump number of messages sent for the node
            ++node.PacketsSent;

            //  set server address and callback state
            serverAddress = node.Device.address;
            callbackReceived = 0;
            callbackInfo = null;

            //  send token or transaction request
            FtpClient.FtpTransactionRequest request = null;
            switch (testParams.mode)
            {
                case ECCONetApi.PingTestMode.CanAddressRequest:
                    {
                        Token token = new Token(Token.Keys.KeyRequestAddress, serverAddress, 0);
                        core.SendTokenToCanBus(token);
                    }
                    break;

                case ECCONetApi.PingTestMode.ReadInfoFile:
                    {
                        request = new FtpClient.FtpTransactionRequest(FtpClient.FtpTransactionType.ReadFile,
                            serverAddress, node.Device.serverAccessCode, ECCONetApi.FileName.ProductInfo, core.ftpServerResponseTimeout, FtpCallback);
                        core.FTP_TransactionRequest(request);
                    }
                    break;

                case ECCONetApi.PingTestMode.WriteNamedFile:
                    {
                        request = new FtpClient.FtpTransactionRequest(FtpClient.FtpTransactionType.WriteFile,
                            serverAddress, node.Device.serverAccessCode, testParams.fileName, DateTime.Now, testParams.fileData, core.ftpServerResponseTimeout, FtpCallback);
                        core.FTP_TransactionRequest(request);
                    }
                    break;

                case ECCONetApi.PingTestMode.RandomWriteRead:
                    {
                        //  create new random file size with random data for test
                        var r = new Random();
                        byte[] randomFileData = new byte[testParams.randomFileSize];
                        for (int idx = 0; idx < randomFileData.Length; ++idx)
                            randomFileData[idx] = (byte)r.Next(0, 255);
                        testParams.fileData = randomFileData;

                        //  write the file
                        request = new FtpClient.FtpTransactionRequest(FtpClient.FtpTransactionType.WriteFile,
                            serverAddress, node.Device.serverAccessCode, randFileName, DateTime.Now, testParams.fileData, core.ftpServerResponseTimeout, FtpCallback);
                        core.FTP_TransactionRequest(request);
                    }
                    break;

                default:
                    return false;
            }

            //  wait for response
            Stopwatch responseTimer = Stopwatch.StartNew();
            Stopwatch totalTimer = Stopwatch.StartNew();
            while (responseTimer.ElapsedMilliseconds < maximumReplyPeriod)
            {
                if (callbackReceived != 0)
                    break;
            }
            responseTimer.Stop();

            //  verify callback
            bool callbackGood = false;
            switch (testParams.mode)
            {
                case ECCONetApi.PingTestMode.CanAddressRequest:
                    callbackGood = true;
                    break;

                case ECCONetApi.PingTestMode.ReadInfoFile:
                    if (callbackInfo == null)
                        return false;
                    callbackGood = (callbackInfo.responseKey == Token.Keys.KeyResponseFileReadComplete);
                    break;

                case ECCONetApi.PingTestMode.WriteNamedFile:
                    if (callbackInfo == null)
                        return false;
                    callbackGood = (callbackInfo.responseKey == Token.Keys.KeyResponseFileWriteComplete);
                    break;

                case ECCONetApi.PingTestMode.RandomWriteRead:
                    if (callbackInfo == null || callbackInfo.responseKey != Token.Keys.KeyResponseFileWriteComplete)
                        return false;
                    callbackGood = (callbackInfo.responseKey == Token.Keys.KeyResponseFileWriteComplete);
                    break;

                default:
                    break;
            }
            if (!callbackGood)
                return false;

            //  if not reading file back for compare, then done
            if (testParams.mode != ECCONetApi.PingTestMode.RandomWriteRead)
            {
                //  tally packet and response time
                ++node.PacketsReceived;
                if (node.MinimumResponseTime > responseTimer.ElapsedMilliseconds)
                    node.MinimumResponseTime = (uint)responseTimer.ElapsedMilliseconds;
                if (node.MaximumResponseTime < responseTimer.ElapsedMilliseconds)
                    node.MaximumResponseTime = (uint)responseTimer.ElapsedMilliseconds;
                node.AverageResponseTime += responseTimer.ElapsedMilliseconds;
                return true;
            }

            //  set server address and callback state
            serverAddress = node.Device.address;
            callbackReceived = 0;
            callbackInfo = null;

            //  request file
            request = new FtpClient.FtpTransactionRequest(FtpClient.FtpTransactionType.ReadFile,
                serverAddress, node.Device.serverAccessCode, randFileName, core.ftpServerResponseTimeout, FtpCallback);
            core.FTP_TransactionRequest(request);

            //  wait for response
            responseTimer.Restart();
            while (responseTimer.ElapsedMilliseconds < maximumReplyPeriod)
            {
                if (callbackReceived != 0)
                    break;
            }
            responseTimer.Stop();

            //  validate file read
            bool fileValid = true;
            if (callbackInfo == null)
                return false;
            if (callbackInfo.responseKey == Token.Keys.KeyResponseFileReadComplete
                && callbackInfo.fileData.Length == testParams.fileData.Length)
            {
                for (int i = 0; i < callbackInfo.fileData.Length; ++i)
                {
                    if (callbackInfo.fileData[i] != testParams.fileData[i])
                    {
                        fileValid = false;
                        break;
                    }
                }
            }
            else
            {
                fileValid = false;
            }

            //  set server address and callback state
            serverAddress = node.Device.address;
            callbackReceived = 0;
            callbackInfo = null;

            //  delete the file
            request = new FtpClient.FtpTransactionRequest(FtpClient.FtpTransactionType.DeleteFile,
                serverAddress, node.Device.serverAccessCode, randFileName, core.ftpServerResponseTimeout, FtpCallback);
            core.FTP_TransactionRequest(request);

            //  wait for response
            responseTimer.Restart();
            while (responseTimer.ElapsedMilliseconds < maximumReplyPeriod)
            {
                if (callbackReceived != 0)
                    break;
            }
            responseTimer.Stop();
            totalTimer.Stop();

            //  validate file delete
            if (callbackInfo == null)
                return false;
            if (callbackInfo.responseKey != Token.Keys.KeyResponseFileDeleteComplete)
                return false;

            if (fileValid)
            {
                //  tally packet and response time
                ++node.PacketsReceived;
                if (node.MinimumResponseTime > totalTimer.ElapsedMilliseconds)
                    node.MinimumResponseTime = (uint)totalTimer.ElapsedMilliseconds;
                if (node.MaximumResponseTime < totalTimer.ElapsedMilliseconds)
                    node.MaximumResponseTime = (uint)totalTimer.ElapsedMilliseconds;
                node.AverageResponseTime += totalTimer.ElapsedMilliseconds;
            }
            return fileValid;
        }
        #endregion

        #region Token value toggle
        //  the token toggle number of tokens sent delegate
        private ECCONetApi.TokenToggleDelegate tokenToggleDelegate;

        //  min and max messages per second
        public const int TokenValueToggleMinTokensPerSecond = 1;
        public const int TokenValueToggleMaxTokensPerSecond = 50;

        //  a token toggle busy flag
        byte tokenToggleBusy;

        //  a token toggle timer critical section lock
        Object tokenToggleTimerLock = new object();

        //  the bus flood callback timer
        System.Threading.Timer tokenToggleCallbackTimer;

        //  the token to send
        Token toggleToken1;

        //  the token to send
        Token toggleToken2;

        //  the number of tokens sent
        int numTokensSent;


        /// <summary>
        /// Gets a value indicating whether the token toggle is running.
        /// </summary>
        public bool IsTokenToggleRunning
        {
            get => (tokenToggleBusy != 0);
        }

        /// <summary>
        /// Periodicically sends a token with alternating values.
        /// </summary>
        /// <param name="token1">The first token to send.</param>
        /// <param name="token2">The second token to send.</param>
        /// <param name="numTokensPerSecond">The number of tokens per second to send.</param>
        /// <param name="callback">The token toggle number of tokens sent callback.</param>
        /// <returns>Returns 0 if token toggle started, -1 if busy, and -2 if bad params.</returns>
        public int TokenToggleStart(Token token1, Token token2, int numTokensPerSecond, ECCONetApi.TokenToggleDelegate callback)
        {
            //  check busy flag
            if (tokenToggleBusy != 0)
                return -1;

            //  validate token
            if ((null == token1) || (null == token2))
                return -2;

            //  validate messages per second
            if ((numTokensPerSecond < TokenValueToggleMinTokensPerSecond) 
                || (numTokensPerSecond > TokenValueToggleMaxTokensPerSecond))
                return -2;

            //  save parameters and state
            toggleToken1 = token1;
            toggleToken2 = token2;
            tokenToggleDelegate = callback;
            numTokensSent = 0;
            tokenToggleBusy = 1;

            //  token toggle thread
            Thread tokenToggleThread = new Thread(() =>
            {
                //  calculate rate in uS
                int milliseconsPerToken = (int)(double)(1000.0 / numTokensPerSecond);
                int currentMilliseconds = 0;

                //  start a stopwatch
                var toggleStopwatch = Stopwatch.StartNew();

                //	start a new callback timer
                tokenToggleCallbackTimer = new System.Threading.Timer(new TimerCallback(tokenToggleCallbackTimerCallback), null, 0, 500);

                //  while running
                while (tokenToggleBusy != 0)
                {
                    if (toggleStopwatch.ElapsedMilliseconds > currentMilliseconds)
                    {
                        //  set next time
                        currentMilliseconds += milliseconsPerToken;

                        //  send token
                        if ((numTokensSent & 1) == 0)
                            core.SendTokenToCanBus(toggleToken1);
                        else
                            core.SendTokenToCanBus(toggleToken2);
                        ++numTokensSent;
                    }
                }

                //  stop stopwatch and timer
                if (toggleStopwatch != null)
                    toggleStopwatch.Stop();
                if (tokenToggleCallbackTimer != null)
                    tokenToggleCallbackTimer.Dispose();
            });

            //  start thread
            if (tokenToggleThread != null)
                tokenToggleThread.Start();

            //  set busy flag and return success
            return 0;
        }

        /// <summary>
        /// The token toggle callback timer callback.
        /// </summary>
        /// <param name="state">The user object state.</param>
        void tokenToggleCallbackTimerCallback(object state)
        {
            tokenToggleDelegate?.Invoke(this, numTokensSent);
        }

        /// <summary>
        /// Stops the token toggle.
        /// </summary>
        public void TokenToggleStop()
        {
            //  clear busy flag
            tokenToggleBusy = 0;

            //  stop timers
            if (tokenToggleCallbackTimer != null)
                tokenToggleCallbackTimer.Dispose();
        }

        #endregion
    }
}
