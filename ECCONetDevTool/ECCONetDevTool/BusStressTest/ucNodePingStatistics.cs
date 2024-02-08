using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ECCONet;

namespace ECCONetDevTool.BusStressTest
{
    public partial class ucNodePingStatistics : UserControl
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


        /// <summary>
        /// Constructor.
        /// </summary>
        public ucNodePingStatistics()
        {
            //  initialize UI component
            InitializeComponent();

            //  restore the settings
            restoreSettings();
        }

        #region Online devices
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
        #endregion

        #region Test run and stop
        //  user click the run test button
        private void btnRun_Click(object sender, EventArgs e)
        {
            //  get list of devices to ping
            List<ECCONetApi.ECCONetDevice> devices;
            if (cbxPingAllNodes.Checked)
            {
                //  validate online devices
                if ((null == onlineDevices) || (0 == onlineDevices.Count))
                {
                    MessageBox.Show("No devices to ping.");
                    return;
                }

                //  get devices
                devices = new List<ECCONetApi.ECCONetDevice>(onlineDevices.Count);
                foreach (var device in onlineDevices)
                    devices.Add(device.Copy());
            }
            else
            {
                //  validate selected device
                if ((null == onlineDevices) || (0 == onlineDevices.Count)
                    || (cbbOnlineDevices.SelectedIndex >= onlineDevices.Count))
                {
                    MessageBox.Show("Selected node not valid.");
                    return;
                }
                devices = new List<ECCONetApi.ECCONetDevice>() { onlineDevices[cbbOnlineDevices.SelectedIndex].Copy() };
            }

            //  if testing for a specified period
            if (rbtnTestForPeriod.Checked)
            {
                //  get hours, minutes, and seconds
                if (!ntbHours.GetUInt32Value(out uint hours))
                {
                    MessageBox.Show("Hours not valid.");
                    return;
                }
                if (!ntbMinutes.GetUInt32Value(out uint minutes))
                {
                    MessageBox.Show("Minutes not valid.");
                    return;
                }
                if (!ntbSeconds.GetUInt32Value(out uint seconds))
                {
                    MessageBox.Show("Seconds not valid.");
                    return;
                }

                //  create time span
                TimeSpan duration = new TimeSpan((int)hours, (int)minutes, (int)seconds);

                //  start test
                rtbResults.Text = string.Empty;
                lastPingedAddress = 0;
                ECCONetApi.PingTestMode mode = (ECCONetApi.PingTestMode)cbbMode.SelectedIndex;
                int maxRandomFileSize = (cbxMaxRandomSize.SelectedIndex + 1) * 1000;
                canInterface.StartBusStatisticsPingTest(devices, duration, mode, tbxWriteFileName.Text, maxRandomFileSize,
                    BusAnalysisProgressCallback, BusAnalysisCompleteCallback);
            }
            else  //  running for specified number of pings
            {
                //  get hours, minutes, and seconds
                if (!ntbPingsPerNode.GetUInt32Value(out uint pingsPerNode))
                {
                    MessageBox.Show("Pings per node not valid.");
                    return;
                }

                //  start test
                rtbResults.Text = string.Empty;
                lastPingedAddress = 0;
                ECCONetApi.PingTestMode mode = (ECCONetApi.PingTestMode)cbbMode.SelectedIndex;
                int maxRandomFileSize = (cbxMaxRandomSize.SelectedIndex + 1) * 1000;
                canInterface.StartBusStatisticsPingTest(devices, pingsPerNode, mode, tbxWriteFileName.Text, maxRandomFileSize,
                    BusAnalysisProgressCallback, BusAnalysisCompleteCallback);
            }
        }

        //  user clicked the stop button
        private void btnStop_Click(object sender, EventArgs e)
        {
            //  just return if not running
            if (!canInterface.IsBusStatisticsPingTestRunning)
                return;

            //  if user verifies, then stop test
            if (MessageBox.Show("Abort test?", "Test Still Running", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                canInterface.StopBusStatisticsPingTest();
            }
        }

        //  user clicked the find file button
        private void btnFind_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                tbxWriteFileName.Text = openFileDialog1.FileName;
            }
        }
        #endregion


        #region Test callbacks
        //  last pinged address tracker
        private byte lastPingedAddress = 0;

        /// <summary>
        /// A callback for bus analysis progress updates.
        /// </summary>
        /// <param name="sender">The callback sender.
        /// <param name="percentComplete">The percentage of test completion, 0 to 100.</param>
        /// <param name="address">The device address about to be tested.</param>
        public void BusAnalysisProgressCallback(object sender, int percentComplete, byte address)
        {
            if (this.cbbOnlineDevices.InvokeRequired)
            {
                ECCONetApi.BusAnalysisProgressDelegate d = 
                    new ECCONetApi.BusAnalysisProgressDelegate(BusAnalysisProgressCallback);
                try
                {
                    this.Invoke(d, new object[] { sender, percentComplete, address });
                }
                catch { }
            }
            else
            {
                //  set pinging address
                //lblPingAddress.Visible = true;
                //lblPingAddress.Text = string.Format("Currently pinging address {0}", address);
                if (lastPingedAddress != address)
                    rtbResults.Text += string.Format("Pinging address {0}...\n", address);
                lastPingedAddress = address;

                //  set progress bar
                progressBar.Value = percentComplete;
            }
        }

        /// <summary>
        /// A callback for when the bus analysis completes.
        /// </summary>
        /// <param name="sender">The callback sender.
        /// <param name="results">The full or partial results of the test.</param>
        public void BusAnalysisCompleteCallback(object sender, ECCONetApi.BusAnalysisStatistics results)
        {
            if (this.cbbOnlineDevices.InvokeRequired)
            {
                ECCONetApi.BusAnalysisCompleteDelegate d =
                    new ECCONetApi.BusAnalysisCompleteDelegate(BusAnalysisCompleteCallback);
                try
                {
                    this.Invoke(d, new object[] { sender, results });
                }
                catch { }
            }
            else
            {
                //  set pinging address
                lblPingAddress.Visible = false;

                //  set progress bar
                progressBar.Value = 100;

                //  show results
                rtbResults.Text = string.Empty;

                //  if just one node
                if (results.Nodes.Count == 1)
                {
                    if (cbbMode.SelectedIndex == 0)
                    {
                        rtbResults.Text = string.Format("{0}  \nAddress {1}\n",
                            results.Nodes[0].Device.modelName, results.Nodes[0].Device.address);
                        rtbResults.Text += string.Format("Test period: {0:%hh\\:mm\\:ss\\.ff}\n",
                            results.Nodes[0].TestDuration);
                        rtbResults.Text += string.Format("Packets sent: {0}  Packets received: {1}\n",
                            results.Nodes[0].PacketsSent, results.Nodes[0].PacketsReceived);
                        rtbResults.Text += string.Format("Min, Max, Avg response time mS: {0:0.0}   {1:0.0}   {2:0.0}\n",
                            results.Nodes[0].MinimumResponseTime, results.Nodes[0].MaximumResponseTime, results.Nodes[0].AverageResponseTime);
                        rtbResults.Text += string.Format("Error rate: {0:0.00}%\n", results.Nodes[0].ErrorRate * 100);
                    }
                    else if (cbbMode.SelectedIndex == 1)
                    {
                        rtbResults.Text = string.Format("{0}  \nAddress {1}\n",
                            results.Nodes[0].Device.modelName, results.Nodes[0].Device.address);
                        rtbResults.Text += string.Format("Test period: {0:%hh\\:mm\\:ss\\.ff}\n",
                            results.Nodes[0].TestDuration);
                        rtbResults.Text += string.Format("FTP read file requests sent: {0}  Files read: {1}\n",
                            results.Nodes[0].PacketsSent, results.Nodes[0].PacketsReceived);
                        rtbResults.Text += string.Format("Min, Max, Avg response time mS: {0:0.0}   {1:0.0}   {2:0.0}\n",
                            results.Nodes[0].MinimumResponseTime, results.Nodes[0].MaximumResponseTime, results.Nodes[0].AverageResponseTime);
                        rtbResults.Text += string.Format("Error rate: {0:0.00}%\n", results.Nodes[0].ErrorRate * 100);
                    }
                    else if (cbbMode.SelectedIndex == 2)
                    {
                        rtbResults.Text = string.Format("{0}  \nAddress {1}\n",
                            results.Nodes[0].Device.modelName, results.Nodes[0].Device.address);
                        rtbResults.Text += string.Format("Test period: {0:%hh\\:mm\\:ss\\.ff}\n",
                            results.Nodes[0].TestDuration);
                        rtbResults.Text += string.Format("FTP write file requests sent: {0}  Files written: {1}\n",
                            results.Nodes[0].PacketsSent, results.Nodes[0].PacketsReceived);
                        rtbResults.Text += string.Format("Min, Max, Avg response time mS: {0:0.0}   {1:0.0}   {2:0.0}\n",
                            results.Nodes[0].MinimumResponseTime, results.Nodes[0].MaximumResponseTime, results.Nodes[0].AverageResponseTime);
                        rtbResults.Text += string.Format("Error rate: {0:0.00}%\n", results.Nodes[0].ErrorRate * 100);
                    }
                    else if (cbbMode.SelectedIndex == 3)
                    {
                        rtbResults.Text = string.Format("{0}  \nAddress {1}\n",
                            results.Nodes[0].Device.modelName, results.Nodes[0].Device.address);
                        rtbResults.Text += string.Format("Test period: {0:%hh\\:mm\\:ss\\.ff}\n",
                            results.Nodes[0].TestDuration);
                        rtbResults.Text += string.Format("FTP write/read/delete requests sent: {0}  Files written: {1}\n",
                            results.Nodes[0].PacketsSent, results.Nodes[0].PacketsReceived);
                        rtbResults.Text += string.Format("Min, Max, Avg response time mS: {0:0.0}   {1:0.0}   {2:0.0}\n",
                            results.Nodes[0].MinimumResponseTime, results.Nodes[0].MaximumResponseTime, results.Nodes[0].AverageResponseTime);
                        rtbResults.Text += string.Format("Error rate: {0:0.00}%\n", results.Nodes[0].ErrorRate * 100);
                    }
                }

                //  else if more than one node to report
                else if (results.Nodes.Count > 1)
                {
                    if (cbbMode.SelectedIndex == 0)
                    {
                        rtbResults.Text = "Summary\n";
                        rtbResults.Text += "====================================================\n";
                        rtbResults.Text += string.Format("Total packets sent: {0}  Total packets received: {1}\n",
                            results.Aggregate.PacketsSent, results.Aggregate.PacketsReceived);
                        rtbResults.Text += string.Format("Total test period: {0:%hh\\:mm\\:ss\\.ff}  Average response time mS: {1:0.0}\n",
                            results.Aggregate.TestDuration, results.Aggregate.AverageResponseTime);
                        rtbResults.Text += string.Format("Avgerage node error rate: {0:0.00}%\n", results.Aggregate.ErrorRate * 100);

                        for (int i = 0; i < results.Nodes.Count; ++i)
                        {
                            rtbResults.Text += string.Format("\n{0}  \nAddress {1}\n",
                                results.Nodes[i].Device.modelName, results.Nodes[i].Device.address);
                            rtbResults.Text += string.Format("Test period: {0:%hh\\:mm\\:ss\\.ff}\n",
                                results.Nodes[i].TestDuration);
                            rtbResults.Text += string.Format("Packets sent: {0}  Packets received: {1}\n",
                                results.Nodes[i].PacketsSent, results.Nodes[i].PacketsReceived);
                            rtbResults.Text += string.Format("Min, Max, Avg response time mS: {0:0.0}   {1:0.0}   {2:0.0}\n",
                                results.Nodes[i].MinimumResponseTime, results.Nodes[i].MaximumResponseTime, results.Nodes[i].AverageResponseTime);
                            rtbResults.Text += string.Format("Error rate: {0:0.00}%\n", results.Nodes[i].ErrorRate * 100);
                        }
                    }
                    else if (cbbMode.SelectedIndex == 1)
                    {
                        rtbResults.Text = "Summary\n";
                        rtbResults.Text += "====================================================\n";
                        rtbResults.Text += string.Format("Total FTP read file requests sent: {0}  Total files read: {1}\n",
                            results.Aggregate.PacketsSent, results.Aggregate.PacketsReceived);
                        rtbResults.Text += string.Format("Total test period: {0:%hh\\:mm\\:ss\\.ff}  Average response time mS: {1:0.0}\n",
                            results.Aggregate.TestDuration, results.Aggregate.AverageResponseTime);
                        rtbResults.Text += string.Format("Avgerage node error rate: {0:0.00}%\n", results.Aggregate.ErrorRate * 100);

                        for (int i = 0; i < results.Nodes.Count; ++i)
                        {
                            rtbResults.Text += string.Format("\n{0}  \nAddress {1}\n",
                                results.Nodes[i].Device.modelName, results.Nodes[i].Device.address);
                            rtbResults.Text += string.Format("Test period: {0:%hh\\:mm\\:ss\\.ff}\n",
                                results.Nodes[i].TestDuration);
                            rtbResults.Text += string.Format("FTP read file requests sent: {0}  Files read: {1}\n",
                                results.Nodes[i].PacketsSent, results.Nodes[i].PacketsReceived);
                            rtbResults.Text += string.Format("Min, Max, Avg response time mS: {0:0.0}   {1:0.0}   {2:0.0}\n",
                                results.Nodes[i].MinimumResponseTime, results.Nodes[i].MaximumResponseTime, results.Nodes[i].AverageResponseTime);
                            rtbResults.Text += string.Format("Error rate: {0:0.00}%\n", results.Nodes[i].ErrorRate * 100);
                        }

                    }
                    else if (cbbMode.SelectedIndex == 2)
                    {
                        rtbResults.Text = "Summary\n";
                        rtbResults.Text += "====================================================\n";
                        rtbResults.Text += string.Format("Total FTP write file requests sent: {0}  Total files written: {1}\n",
                            results.Aggregate.PacketsSent, results.Aggregate.PacketsReceived);
                        rtbResults.Text += string.Format("Total test period: {0:%hh\\:mm\\:ss\\.ff}  Average response time mS: {1:0.0}\n",
                            results.Aggregate.TestDuration, results.Aggregate.AverageResponseTime);
                        rtbResults.Text += string.Format("Avgerage node error rate: {0:0.00}%\n", results.Aggregate.ErrorRate * 100);

                        for (int i = 0; i < results.Nodes.Count; ++i)
                        {
                            rtbResults.Text += string.Format("\n{0}  \nAddress {1}\n",
                                results.Nodes[i].Device.modelName, results.Nodes[i].Device.address);
                            rtbResults.Text += string.Format("Test period: {0:%hh\\:mm\\:ss\\.ff}\n",
                                results.Nodes[i].TestDuration);
                            rtbResults.Text += string.Format("FTP write file requests sent: {0}  Files written: {1}\n",
                                results.Nodes[i].PacketsSent, results.Nodes[i].PacketsReceived);
                            rtbResults.Text += string.Format("Min, Max, Avg response time mS: {0:0.0}   {1:0.0}   {2:0.0}\n",
                                results.Nodes[i].MinimumResponseTime, results.Nodes[i].MaximumResponseTime, results.Nodes[i].AverageResponseTime);
                            rtbResults.Text += string.Format("Error rate: {0:0.00}%\n", results.Nodes[i].ErrorRate * 100);
                        }

                    }
                    else if (cbbMode.SelectedIndex == 3)
                    {
                        rtbResults.Text = "Summary\n";
                        rtbResults.Text += "====================================================\n";
                        rtbResults.Text += string.Format("Total FTP write/read/delete requests sent: {0}  Total files written: {1}\n",
                            results.Aggregate.PacketsSent, results.Aggregate.PacketsReceived);
                        rtbResults.Text += string.Format("Total test period: {0:%hh\\:mm\\:ss\\.ff}  Average response time mS: {1:0.0}\n",
                            results.Aggregate.TestDuration, results.Aggregate.AverageResponseTime);
                        rtbResults.Text += string.Format("Avgerage node error rate: {0:0.00}%\n", results.Aggregate.ErrorRate * 100);

                        for (int i = 0; i < results.Nodes.Count; ++i)
                        {
                            rtbResults.Text += string.Format("\n{0}  \nAddress {1}\n",
                                results.Nodes[i].Device.modelName, results.Nodes[i].Device.address);
                            rtbResults.Text += string.Format("Test period: {0:%hh\\:mm\\:ss\\.ff}\n",
                                results.Nodes[i].TestDuration);
                            rtbResults.Text += string.Format("FTP write/read/delete requests sent: {0}  Files written: {1}\n",
                                results.Nodes[i].PacketsSent, results.Nodes[i].PacketsReceived);
                            rtbResults.Text += string.Format("Min, Max, Avg response time mS: {0:0.0}   {1:0.0}   {2:0.0}\n",
                                results.Nodes[i].MinimumResponseTime, results.Nodes[i].MaximumResponseTime, results.Nodes[i].AverageResponseTime);
                            rtbResults.Text += string.Format("Error rate: {0:0.00}%\n", results.Nodes[i].ErrorRate * 100);
                        }

                    }
                }
            }
        }
        #endregion

        #region Settings save and restore
        /// <summary>
        /// Restore the settings.
        /// </summary>
        private void restoreSettings()
        {
            cbxPingAllNodes.Checked = Properties.Settings.Default.NPS_PingAllNodes;
            ntbHours.Text = Properties.Settings.Default.NPS_Hours;
            ntbMinutes.Text = Properties.Settings.Default.NPS_Minutes;
            ntbSeconds.Text = Properties.Settings.Default.NPS_Seconds;
            ntbPingsPerNode.Text = Properties.Settings.Default.NPS_PingsPerNode;
            rbtnTestForPeriod.Checked = Properties.Settings.Default.NPS_TestForPeriod;
            rbtnTestForPingsPerNode.Checked = !rbtnTestForPeriod.Checked;
        }

        //  user checked Ping All Nodes
        private void cbxPingAllNodes_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.NPS_PingAllNodes = cbxPingAllNodes.Checked;
        }

        //  user typed in the Hours text box
        private void ntbHours_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.NPS_Hours = ntbHours.Text;
        }

        //  user typed in the Minutes text box
        private void ntbMinutes_TextChanged(object sender, EventArgs e)
        {
            //  validate input
            if (ntbMinutes.GetUInt32Value(out uint minutes))
            {
                if (minutes > 59)
                    ntbMinutes.Text = "59";
            }
            Properties.Settings.Default.NPS_Minutes = ntbMinutes.Text;
        }

        //  user typed in the Seconds box
        private void ntbSeconds_TextChanged(object sender, EventArgs e)
        {
            //  validate input
            if (ntbMinutes.GetUInt32Value(out uint seconds))
            {
                if (seconds > 59)
                    ntbSeconds.Text = "59";
            }
            Properties.Settings.Default.NPS_Seconds = ntbSeconds.Text;
        }

        //  user typed in the Pings Per Node box
        private void ntbPingsPerNode_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.NPS_PingsPerNode = ntbPingsPerNode.Text;
        }

        //  user checked the Test For Period radio button
        private void rbtnTestForPeriod_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.NPS_TestForPeriod = rbtnTestForPeriod.Checked;
        }

        //  user checked the Pings Per Node radio button
        private void rbtnTestForPingsPerNode_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.NPS_TestForPeriod = !rbtnTestForPingsPerNode.Checked;
        }

        #endregion


    }
}
