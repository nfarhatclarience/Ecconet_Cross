using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics;
using ECCONet;


namespace ECCONetDevTool.BusStressTest
{
    public partial class ucBusFlood : UserControl
    {
        const string inputStatusString = "IS";
        const string outputStatusString = "OS";
        const string commandString = "CMD";

        const int minPacketsPerMessage = 1;
        const int maxPacketsPerMessage = 25;

        const int minMessagesPerSecond = 1;
        const int maxMessagesPerSecond = 50;

        /// <summary>
        /// The CAN interface object.
        /// </summary>
        public ECCONetApi canInterface;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ucBusFlood()
        {
            //  initialize designer components
            InitializeComponent();

            //  restore the settings
            restoreSettings();

            //  populate the dictionaries
            populateDictionaries();

            //  populate the send token line
            populateSendTokenLines();

            //  set labels
            lblPacketsPerMessage.Text = string.Format("Packets per Message ({0} - {1})", minPacketsPerMessage, maxPacketsPerMessage);
            lblMessagesPerSecond.Text = string.Format("Messages per Second ({0} - {1})", minMessagesPerSecond, maxMessagesPerSecond);
        }

        #region Dictionaries
        /// <summary>
        /// The key code to name dictionary.
        /// </summary>
        public Dictionary<int, string> keyCodeToName;

        /// <summary>
        /// The key name to code dictionary.
        /// </summary>
        Dictionary<string, int> keyNameToCode;

        /// <summary>
        /// The key prefix name to code dictionary.
        /// </summary>
        Dictionary<string, int> keyPrefixNameToCode;

        /// <summary>
        /// Populates the default token key names from the ECCONet library.
        /// </summary>
        private void populateDictionaries()
        {
            //  build dictionary from token key enumerations
            keyCodeToName = new Dictionary<int, string>(200);
            Array values = Enum.GetValues(typeof(Token.Keys));
            foreach (Token.Keys val in values)
            {
                string name = Enum.GetName(typeof(Token.Keys), val);
                if (3 < name.Length)
                    keyCodeToName.Add((int)val, name.Substring(3));
            }

            //  if custom token key file available, then try to get and convert to dictionary
            if (File.Exists("CustomECCONetTokenKeys.xml"))
            {
                try
                {
                    //  try to read in custom keys
                    XElement xElem = XElement.Parse(File.ReadAllText("CustomECCONetTokenKeys.xml"));
                    Dictionary<int, string> customKeys = xElem.Descendants("item")
                                        .ToDictionary(x => (int)x.Attribute("id"), x => (string)x.Attribute("value"));

                    //  add custom keys to dictionary
                    foreach (KeyValuePair<int, string> customKey in customKeys)
                    {
                        if (!keyCodeToName.ContainsKey(customKey.Key))
                            keyCodeToName.Add(customKey.Key, customKey.Value);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            //  create reverse token key dictionary
            keyNameToCode = new Dictionary<string, int>(200);
            foreach (int key in keyCodeToName.Keys)
                keyNameToCode.Add(keyCodeToName[key], key);

            //  create token key prefix name to code dictionary
            keyPrefixNameToCode = new Dictionary<string, int>(3);
            keyPrefixNameToCode.Add(inputStatusString, (int)Token.KeyPrefix.InputStatus);
            keyPrefixNameToCode.Add(outputStatusString, (int)Token.KeyPrefix.OutputStatus);
            keyPrefixNameToCode.Add(commandString, (int)Token.KeyPrefix.Command);
        }
        #endregion

        #region Custom token
        class SendTokenLine
        {
            //  members
            public TextBox address;
            public TextBox keyPrefix;
            public TextBox key;
            public TextBox value;
            public Label inputError;
            public Token token;

            public SendTokenLine(TextBox address, TextBox keyPrefix, TextBox key, TextBox value, Label inputError)
            {
                this.address = address;
                this.keyPrefix = keyPrefix;
                this.key = key;
                this.value = value;
                this.inputError = inputError;
            }

            /// <summary>
            /// Converts the text box inputs to a token, or null if the text box data is not valid.
            /// </summary>
            /// <returns></returns>
            public Token ToToken(Dictionary<string, int> keyPrefixNameToCode, Dictionary<string, int> keyNameToCode)
            {
                Token token = new Token();
                int kp;
                int k;
                bool goodInput = true;

                //  address
                if (address.Text.StartsWith("0x"))
                    goodInput = Byte.TryParse(address.Text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out token.address);
                else
                    goodInput = Byte.TryParse(address.Text, out token.address);
                if (!goodInput || (0x7f < token.address))
                {
                    address.ForeColor = Color.DarkRed;
                    return null;
                }
                address.ForeColor = Color.Black;

                //  key prefix
                goodInput = keyPrefixNameToCode.TryGetValue(keyPrefix.Text, out kp);
                if (!goodInput)
                {
                    goodInput = Int32.TryParse(keyPrefix.Text, out kp);
                }
                if (!goodInput)
                {
                    keyPrefix.ForeColor = Color.DarkRed;
                    return null;
                }
                keyPrefix.ForeColor = Color.Black;

                //  key
                int maxTokenValue = Math.Abs(~((int)Token.KeyPrefix.Mask << 8));
                goodInput = keyNameToCode.TryGetValue(key.Text, out k);
                if (!goodInput)
                {
                    if (key.Text.StartsWith("0x"))
                        goodInput = Int32.TryParse(key.Text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out k);
                    else
                        goodInput = Int32.TryParse(key.Text, out k);
                }
                if (!goodInput ||
                    (0 > kp) || ((int)Token.KeyPrefix.InputStatus < kp) ||
                    (0 > k) || (maxTokenValue < k))
                {
                    key.ForeColor = Color.DarkRed;
                    return null;
                }
                key.ForeColor = Color.Black;
                token.key = (Token.Keys)((kp << 8) | k);

                //  value
                if (value.Text.StartsWith("0x"))
                    goodInput = Int32.TryParse(value.Text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out token.value);
                else
                    goodInput = Int32.TryParse(value.Text, out token.value);
                if (!goodInput)
                {
                    value.ForeColor = Color.DarkRed;
                    return null;
                }
                value.ForeColor = Color.Black;

                //  return token
                return token;
            }
        }


        /// <summary>
        /// List of send token lines.
        /// </summary>
        const int numSendTokenLines = 1;
        List<SendTokenLine> sendTokenLines = new List<SendTokenLine>(numSendTokenLines);

        /// <summary>
        /// Populate the list of send token lines.
        /// </summary>
        private void populateSendTokenLines()
        {
            sendTokenLines.Add(new SendTokenLine(tbAddress, tbType, tbKey, tbValue, lblError));
        }

        /// <summary>
        /// Evaluates an input line.
        /// </summary>
        private void evaluateInputLine(SendTokenLine line)
        {
            if (line.address.Text.Trim().Equals(String.Empty) &&
                line.keyPrefix.Text.Trim().Equals(String.Empty) &&
                line.key.Text.Trim().Equals(String.Empty) &&
                line.value.Text.Trim().Equals(String.Empty))
            {
                line.token = null;
                line.inputError.Visible = false;
            }
            else
            {
                line.token = line.ToToken(keyPrefixNameToCode, keyNameToCode);
                line.inputError.Visible = (null == line.token);
            }
        }
        #endregion

        #region Run and stop
        //  user clicked the run button
        private void btnRun_Click(object sender, EventArgs e)
        {
            //  validate CAN interface
            if (canInterface == null)
            {
                MessageBox.Show("CAN interface error.");
                return;
            }

            Token token = null;
            if (!cbxUseDefaultToken.Checked)
            {
                //  validate the input line
                var t = sendTokenLines[0];
                if (t == null)
                {
                    MessageBox.Show("Unknown token error.");
                    return;
                }
                evaluateInputLine(t);
                if (lblError.Visible || (t.token == null))
                {
                    MessageBox.Show("Invalid token.");
                    return;
                }
                token = t.token;
            }

            //  validate packets per message text box number
            if (!ntbPacketsPerMessage.GetUInt32Value(out uint packetsPerMessage))
            {
                MessageBox.Show("Invalid packets per message.");
                return;
            }

            //  validate messages per second text box number
            if (!ntbMessagesPerSecond.GetUInt32Value(out uint messagesPerSecond))
            {
                MessageBox.Show("Invalid messages per second.");
                return;
            }

            //  start the bus flood
            int result = canInterface.FloodBusStart(packetsPerMessage, messagesPerSecond, token, BusFloodCallback);

            //  validate packets per message
            if (result == -1)
            {
                MessageBox.Show("Invalid packets per message.");
                return;
            }

            //  validate messages per second
            if (result == -2)
            {
                MessageBox.Show("Invalid messages per second.");
                return;
            }

            //  validate active CAN interface
            if (result == -3)
            {
                MessageBox.Show("No active CAN interface to flood.");
                return;
            }
        }

        //  user clicked the stop button
        private void btnStop_Click(object sender, EventArgs e)
        {
            canInterface.BusFloodStop();
        }
        #endregion

        #region Callback

        /// <summary>
        /// The bus flood timer callback.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="results"></param>
        void BusFloodCallback(object sender, ECCONetApi.BusFloodStatistics results)
        {
            //  validate results
            if (results == null)
                return;

            //  check thread
            if (lblNumPackets.InvokeRequired)
            {
                ECCONetApi.BusFloodDelegate d =
                    new ECCONetApi.BusFloodDelegate(BusFloodCallback);
                try
                {
                    this.Invoke(d, new object[] { sender, results });
                }
                catch { }
            }
            else
            {
                lblNumPackets.Text = string.Format("Packets Sent: {0}", results.NumPacketsSent);
                lblNumMessages.Text = string.Format("Messages Sent: {0}", results.NumMessagesSent);
                lblActualPacketPerSecond.Text = string.Format("Actual Packets Per Second: {0}", results.ActualPacketsPerSecond);
                lblApproxBusUtilization.Text = string.Format("Approx. Bus Utilization: {0}%", (int)results.BusUtilizationPercent);
            }
        }

        #endregion



        #region Settings save and restore
        /// <summary>
        /// Restore the settings.
        /// </summary>
        private void restoreSettings()
        {
            //  send token address
            tbAddress.Text = Properties.Settings.Default.BST_Address;
            //  send token key prefix
            tbType.Text = Properties.Settings.Default.BST_KeyPrefix;
            //  send token key
            tbKey.Text = Properties.Settings.Default.BST_Key;
            //  send token value
            tbValue.Text = Properties.Settings.Default.BST_Value;
            //  packets per message value
            ntbPacketsPerMessage.Text = Properties.Settings.Default.BST_PacketsPerMessage;
            //  messages per second value
            ntbMessagesPerSecond.Text = Properties.Settings.Default.BST_MessagesPerSecond;
            //  use default token
            cbxUseDefaultToken.Checked = Properties.Settings.Default.BST_UseDefaultToken;
        }

        //  address
        private void tbAddress_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[0]);
                Properties.Settings.Default.BST_Address = tbAddress.Text;
            }
        }

        //  key prefix
        private void tbType_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[0]);
                Properties.Settings.Default.BST_KeyPrefix = tbType.Text;
            }
        }

        //  key
        private void tbKey_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[0]);
                Properties.Settings.Default.BST_Key = tbKey.Text;
            }
        }

        //  value
        private void tbValue_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[0]);
                Properties.Settings.Default.BST_Value = tbValue.Text;
            }
        }

        //  packets per message value
        private void ntbPacketsPerMessage_TextChanged(object sender, EventArgs e)
        {
            //  save the setting
            Properties.Settings.Default.BST_PacketsPerMessage = ntbPacketsPerMessage.Text;
        }

        //  messages per second value
        private void ntbMessagesPerSecond_TextChanged(object sender, EventArgs e)
        {
            //  save the setting
            Properties.Settings.Default.BST_MessagesPerSecond = ntbMessagesPerSecond.Text;
        }

        //  user clicked use default token checkbox
        private void cbxUseDefaultToken_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.BST_UseDefaultToken = cbxUseDefaultToken.Checked;
        }
        #endregion

    }
}



#if PREVIOUS_CODE
        //  message params
        private Stopwatch stopWatch;
        private Token token = null;
        private uint packetsPerMessage = 1;
        private uint numPackets = 0;
        private uint numMessages = 0;

        //	a message timer compatible with .NET Core
        System.Threading.Timer messageTimer;

        //  a message timer critical section lock
        Object messageTimerLock = new object();

        //  a message timer busy flag
        Byte messageTimerBusy;

        /// <summary>
        /// The process manager timer callback.
        /// </summary>
        /// <param name="state">The user object state.</param>
        void messageTimerCallback(object state)
        {
            //  if already executing a timer callback,
            //  then skip this one
            if (0 != messageTimerBusy)
                return;

            //  critical code section
            lock (messageTimerLock)
            {
                //  set busy flag
                messageTimerBusy = 1;

                if ((packetsPerMessage >= minPacketsPerMessage) && (packetsPerMessage <= maxPacketsPerMessage))
                {
                    //  send packets
                    for (int i = 0; i < packetsPerMessage; ++i)
                    {
                        //  send a packet
                        if (null != canInterface)
                            canInterface.SendToken(token);
                    }
                }

                //  update readouts
                numPackets += packetsPerMessage;
                ++numMessages;

                //  clear busy flag
                messageTimerBusy = 0;
            }
        }


        //  timer message tick
        private void tmrMessage_Tick(object sender, EventArgs e)
        {
            lblNumPackets.Text = string.Format("Packets: {0}", numPackets);
            lblNumMessages.Text = string.Format("Messages: {0}", numMessages);
            UpdateBusUtilizationLabel();
        }


        //  user clicked the run button
        private void btnRun_Click(object sender, EventArgs e)
        {
            //  validate CAN interface
            if (canInterface == null)
            {
                MessageBox.Show("CAN interface error.");
                return;
            }

            //  validate the input line
            var t = sendTokenLines[0];
            if (t == null)
            {
                MessageBox.Show("Unknown token error.");
                return;
            }
            evaluateInputLine(t);
            if (lblError.Visible || (t.token == null))
            {
                MessageBox.Show("Invalid token.");
                return;
            }

            //  validate packets per message
            if (!ntbPacketsPerMessage.GetUInt32Value(out packetsPerMessage)
                || (packetsPerMessage < minPacketsPerMessage) || (packetsPerMessage > maxPacketsPerMessage))
            {
                MessageBox.Show("Invalid packets per message.");
                return;
            }

            //  validate messages per second
            if (!ntbMessagesPerSecond.GetUInt32Value(out uint messagesPerSecond)
                || ((messagesPerSecond < minMessagesPerSecond) || (messagesPerSecond > maxMessagesPerSecond)))
            {
                MessageBox.Show("Invalid messages per second.");
                return;
            }

            //  setup to run bus flood
            token = t.token;
            stopWatch = new Stopwatch();
            stopWatch.Start();
            numPackets = 0;
            numMessages = 0;
            lblNumPackets.Text = "Packets: 0";
            lblNumMessages.Text = "Messages: 0";
            tmrMessage.Start();

            //	create the process manager timer
            messageTimer = new System.Threading.Timer(new TimerCallback(messageTimerCallback), null, 0, 1000 / (int)messagesPerSecond);

            //  debug
            Debug.WriteLine(string.Format("Starting bus flood."));
        }

        //  user clicked the stop button
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (tmrMessage != null)
                tmrMessage.Stop();
            if (stopWatch != null)
                stopWatch.Stop();
            if (messageTimer != null)
                messageTimer.Dispose();
            UpdateBusUtilizationLabel();
        }

            //  update bus utilization label
            if (stopWatch != null)
            {
                //TimeSpan ts = stopWatch.Elapsed;
                //lblActualPacketPerSecond.Visible = true;
                //lblApproxBusUtilization.Visible = true;

                int packetsPerSecond = (int)((float)numPackets / ((float)stopWatch.ElapsedMilliseconds / 1000.0));
                lblActualPacketPerSecond.Text = string.Format("Actual Packets Per Second: {0}", packetsPerSecond);
                lblApproxBusUtilization.Text = string.Format("Approx. Bus Utilization: {0}%", (int)((float)packetsPerSecond * 0.97 / 10.0));
            }
            /*
            else
            {
                lblActualPacketPerSecond.Visible = false;
                lblApproxBusUtilization.Visible = false;
            }
            */

        /// <summary>
        /// User checked or unchecked the run checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbRun_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRun.Checked)
            {
                var t = sendTokenLines[0];
                evaluateInputLine(t);
                token = t.token;
                if ((null != token) && (null != canInterface)
                    && (ntbPacketsPerMessage.GetUInt32Value(out packetsPerMessage))
                    && ((packetsPerMessage >= minPacketsPerMessage) && (packetsPerMessage <= maxPacketsPerMessage))
                    && (ntbMessagesPerSecond.GetUInt32Value(out uint messagesPerSecond))
                    && ((messagesPerSecond >= minMessagesPerSecond) && (messagesPerSecond <= maxMessagesPerSecond)))
                {
                    stopWatch = new Stopwatch();
                    stopWatch.Start();
                    numPackets = 0;
                    numMessages = 0;
                    lblNumPackets.Text = "Packets: 0";
                    lblNumMessages.Text = "Messages: 0";
                    tmrMessage.Interval = 1000 / (int)messagesPerSecond;
                    tmrMessage.Start();
                }
                else
                {
                    cbRun.Checked = false;
                }
            }
            else
            {
                tmrMessage.Stop();
                stopWatch.Stop();
                UpdateBusUtilizationLabel();
            }
        }
#endif

