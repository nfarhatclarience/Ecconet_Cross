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
using System.Xml.Linq;
using System.Diagnostics;
using ECCONet;


namespace ECCONetDevTool.BusStressTest
{
    public partial class ucTokenToggle : UserControl
    {
        const string inputStatusString = "IS";
        const string outputStatusString = "OS";
        const string commandString = "CMD";

        const int minMessagesPerSecond = 1;
        const int maxMessagesPerSecond = 50;

        /// <summary>
        /// The CAN interface object.
        /// </summary>
        public ECCONetApi canInterface;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ucTokenToggle()
        {
            //  initialize designer components
            InitializeComponent();

            //  restore the settings
            restoreSettings();

            //  populate the dictionaries
            populateDictionaries();

            //  populate the send token line
            populateSendTokenLines();
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

        #region Token
        class SendTokenLine
        {
            //  members
            public TextBox address;
            public TextBox keyPrefix;
            public TextBox key;
            public TextBox value1;
            public TextBox value2;
            public Label inputError;


            public SendTokenLine(TextBox address, TextBox keyPrefix, TextBox key, TextBox value1, TextBox value2, Label inputError)
            {
                this.address = address;
                this.keyPrefix = keyPrefix;
                this.key = key;
                this.value1 = value1;
                this.value2 = value2;
                this.inputError = inputError;
            }

            /// <summary>
            /// Converts the text box inputs to tokens.
            /// </summary>
            /// <returns></returns>
            public bool TryParse(Dictionary<string, int> keyPrefixNameToCode, Dictionary<string, int> keyNameToCode, out Token token1, out Token token2)
            {
                token1 = new Token();
                token2 = new Token();
                int kp;
                int k;
                bool goodInput = true;

                //  check for unused
                if (address.Text.Trim().Equals(String.Empty)
                    && keyPrefix.Text.Trim().Equals(String.Empty)
                    && key.Text.Trim().Equals(String.Empty)
                    && value1.Text.Trim().Equals(String.Empty)
                    && value2.Text.Trim().Equals(String.Empty))
                {
                    inputError.Visible = false;
                    return false;
                }

                //  address
                if (address.Text.StartsWith("0x"))
                    goodInput = Byte.TryParse(address.Text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out token1.address);
                else
                    goodInput = Byte.TryParse(address.Text, out token1.address);
                if (!goodInput || (0x7f < token1.address))
                {
                    address.ForeColor = Color.DarkRed;
                    inputError.Visible = true;
                    return false;
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
                    inputError.Visible = true;
                    return false;
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
                    inputError.Visible = true;
                    return false;
                }
                key.ForeColor = Color.Black;
                token1.key = (Token.Keys)((kp << 8) | k);

                //  value0
                if (value1.Text.StartsWith("0x"))
                    goodInput = Int32.TryParse(value1.Text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out token1.value);
                else
                    goodInput = Int32.TryParse(value1.Text, out token1.value);
                if (!goodInput)
                {
                    value1.ForeColor = Color.DarkRed;
                    inputError.Visible = true;
                    return false;
                }
                value1.ForeColor = Color.Black;

                //  value1
                if (value2.Text.StartsWith("0x"))
                    goodInput = Int32.TryParse(value2.Text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out token2.value);
                else
                    goodInput = Int32.TryParse(value2.Text, out token2.value);
                if (!goodInput)
                {
                    value2.ForeColor = Color.DarkRed;
                    inputError.Visible = true;
                    return false;
                }
                value2.ForeColor = Color.Black;

                //  copy token address and key
                token2.address = token1.address;
                token2.key = token1.key;
                inputError.Visible = false;

                //  return success
                return true;
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
            sendTokenLines.Add(new SendTokenLine(tbAddress, tbType, tbKey, tbValue1, tbValue2, lblError));
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

            //  validate the input line
            var t = sendTokenLines[0];
            if (t == null)
            {
                MessageBox.Show("Unknown token error.");
                return;
            }
            if (!t.TryParse(keyPrefixNameToCode, keyNameToCode, out Token token1, out Token token2))
            {
                MessageBox.Show("Invalid token.");
                return;
            }

            //  validate messages per second text box number
            if (!ntbTokensPerSecond.GetUInt32Value(out uint messagesPerSecond))
            {
                MessageBox.Show("Invalid messages per second.");
                return;
            }

            //  start the token toggle
            int result = canInterface.TokenToggleStart(token1, token2, (int)messagesPerSecond, NumTokensSentCallback);

            //  validate packets per message
            if (result == -1)
            {
                MessageBox.Show("Token toggle already running.");
                return;
            }

            //  validate messages per second
            if (result == -2)
            {
                MessageBox.Show("Invalid parameters.");
                return;
            }

            //  validate active CAN interface
            if (result == -3)
            {
                MessageBox.Show("No active CAN interface.");
                return;
            }
        }

        /// <summary>
        /// User clicked the stop button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            canInterface.TokenToggleStop();
        }

        /// <summary>
        /// Callback from api.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="numTokensSent"></param>
        private void NumTokensSentCallback(object sender, int numTokensSent)
        {
            //  check thread
            if (lblTokensSent.InvokeRequired)
            {
                ECCONetApi.TokenToggleDelegate d =
                    new ECCONetApi.TokenToggleDelegate(NumTokensSentCallback);
                try
                {
                    this.Invoke(d, new object[] { sender, numTokensSent });
                }
                catch { }
            }
            else
            {
                if (numTokensSent > 0)
                {
                    lblTokensSent.Text = string.Format("Tokens Sent: {0}", numTokensSent);
                    lblTokensSent.Visible = true;
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
            //  token address
            tbAddress.Text = Properties.Settings.Default.TT_Address;
            //  token key prefix
            tbType.Text = Properties.Settings.Default.TT_KeyPrefix;
            //  token key
            tbKey.Text = Properties.Settings.Default.TT_Key;
            //  value1 value
            tbValue1.Text = Properties.Settings.Default.TT_Value1;
            //  value2 value
            tbValue2.Text = Properties.Settings.Default.TT_Value2;
            //  messages per second value
            ntbTokensPerSecond.Text = Properties.Settings.Default.TT_TokensPerSecond;
        }

        //  address
        private void tbAddress_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                sendTokenLines[0].TryParse(keyPrefixNameToCode, keyNameToCode, out Token token1, out Token token2);
                Properties.Settings.Default.TT_Address = tbAddress.Text;
            }
        }

        //  key prefix
        private void tbType_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                sendTokenLines[0].TryParse(keyPrefixNameToCode, keyNameToCode, out Token token1, out Token token2);
                Properties.Settings.Default.TT_KeyPrefix = tbType.Text;
            }
        }

        //  key
        private void tbKey_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                sendTokenLines[0].TryParse(keyPrefixNameToCode, keyNameToCode, out Token token1, out Token token2);
                Properties.Settings.Default.TT_Key = tbKey.Text;
            }
        }

        //  value 1
        private void tbValue1_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                sendTokenLines[0].TryParse(keyPrefixNameToCode, keyNameToCode, out Token token1, out Token token2);
                Properties.Settings.Default.TT_Value1 = tbValue1.Text;
            }
        }

        //  value 2
        private void tbValue2_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                sendTokenLines[0].TryParse(keyPrefixNameToCode, keyNameToCode, out Token token1, out Token token2);
                Properties.Settings.Default.TT_Value2 = tbValue2.Text;
            }
        }

        //  messages per second value
        private void ntbTokensPerSecond_TextChanged(object sender, EventArgs e)
        {
            //  save the setting
            Properties.Settings.Default.TT_TokensPerSecond = ntbTokensPerSecond.Text;
        }
        #endregion

    }
}
