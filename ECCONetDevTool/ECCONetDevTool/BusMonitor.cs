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
using System.Xml.Serialization;

using ECCONet;
using static ECCONet.ECCONetApi;
using static ECCONet.Token.Keys;

namespace ECCONetDevTool
{
    public partial class BusMonitor : UserControl
    {
        const string inputStatusString = "IS";
        const string outputStatusString = "OS";
        const string commandString = "CMD";

        #region LineEntry class
        /// <summary>
        /// The line entry class for filtering and sorting.
        /// </summary>
        class LineEntry
        {
            //  direction
            public enum Direction
            {
                In,
                Out
            }

            //  members
            public Direction direction;
            public DateTime dateTime;
            public byte eventIndex;
            public byte address;
            public byte keyPrefix;
            public UInt16 key;
            public Int32 value;
            public int count;
            public int lineNumber;

            /// <summary>
            /// Constructor from token.
            /// </summary>
            /// <param name="token">The token.</param>
            /// <param name="token">The token direction, in or out.</param>
            public LineEntry(Token token, Direction dir)
            {
                direction = dir;
                dateTime = DateTime.Now;
                eventIndex = token.eventIndex;
                address = token.address;
                keyPrefix = (byte)(((int)token.key >> 8) & (int)Token.KeyPrefix.Mask);
                key = (UInt16)((int)token.key & ~((int)Token.KeyPrefix.Mask << 8));
                value = token.value;
                count = 1;
                lineNumber = 0;
            }

            /// <summary>
            /// A string representation of the line entry.
            /// </summary>
            public string ToString(Dictionary<int, string> keyCodeToName, bool showValueInHex)
            {
                //  stringify address, prefix and key
                string str = "";
                try
                {
                    //  direction 6 chars total
                    if (Direction.In == direction)
                        str += " In   ";
                    else
                        str += " Out  ";

                    //  timestamp 9 chars + 2 spaces
                    str += dateTime.ToString("mm:ss:fff  ");

                    //  event index up to 3 chars + 3 spaces
                    str += eventIndex.ToString();
                    while (27 > str.Length)
                        str += " ";

                    //  address up to 3 chars + 6 spaces
                    str += address.ToString();
                    while (36 > str.Length)
                        str += " ";

                    //  value up to 12 spaces
                    if (showValueInHex)
                        str += ("0x" + value.ToString("x4"));
                    else
                        str += value.ToString();
                    while (48 > str.Length)
                        str += " ";

                    //  token key prefix 6 spaces
                    switch ((Token.KeyPrefix)(keyPrefix))
                    {
                        case Token.KeyPrefix.InputStatus:
                            str += (inputStatusString + "    ");
                            break;

                        case Token.KeyPrefix.OutputStatus:
                            str += (outputStatusString + "    ");
                            break;

                        case Token.KeyPrefix.Command:
                            str += (commandString + "   ");
                            break;

                        default:
                            str += "???   ";
                            break;
                    }

                    //  key up to 4 chars + 1 spaces
                    str += key.ToString();
                    while (59 > str.Length)
                        str += " ";

                    //  named and index key up to 37 chars + 3 spaces
                    uint oneByteInputs = (uint)key - Token.Region_Base__Indexed_One_Byte_Inputs;
                    uint oneByteOutputs = (uint)key - Token.Region_Base__Indexed_One_Byte_Outputs;
                    string keyName = "";
                    if ((key >= Token.Region_Base__Indexed_Sequencer_Three_Byte)
                        && (key < (Token.Region_Base__Indexed_Sequencer_Three_Byte + Token.Region_Size__Indexed_Sequencer_Three_Byte)))
                        str += ("Indexed_Sequencer_" + (key - Token.Region_Base__Indexed_Sequencer_Three_Byte).ToString());
                    else if (keyCodeToName.TryGetValue(key, out keyName))
                        str += keyName;
                    else if ((key >= Token.Region_Size__Indexed_One_Byte_Inputs)
                        && (key < (Token.Region_Base__Indexed_One_Byte_Inputs + Token.Region_Size__Indexed_One_Byte_Inputs)))
                        str += ("Indexed_One_Byte_Input_" + (key - Token.Region_Base__Indexed_One_Byte_Inputs).ToString());
                    else if ((key >= Token.Region_Base__Indexed_One_Byte_Outputs)
                        && (key < (Token.Region_Base__Indexed_One_Byte_Outputs + Token.Region_Size__Indexed_One_Byte_Outputs)))
                        str += ("Indexed_One_Byte_Output_" + (key - Token.Region_Base__Indexed_One_Byte_Outputs).ToString());
                    else
                        str += (key).ToString();
                    while (101 > str.Length)
                        str += " ";

                    //  repeats
                    //if (0 < repeats)
                        str += count.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Token Parsing Error");
                }
                return str;
            }

        }
        #endregion

        #region LineEntryFilter class
        /// <summary>
        /// A line filter class.
        /// </summary>
        class LineEntryFilter
        {
            //  the list of items to filter
            public List<int> filter = new List<int>();

            //  a value indicating wheter the filter is exclusive
            public bool isExclusive = false;

            /// <summary>
            /// Default constructor.
            /// </summary>
            public LineEntryFilter()
            {
            }

            /// <summary>
            /// Creates a line filter instance with given comma-separated numeric string.
            /// </summary>
            /// <param name="filterString">The comma-separated filter string.  If starts with '-' then is exclusive.</param>
            public LineEntryFilter(string filterString)
            {
                int value;
                if (filterString.StartsWith("-"))
                {
                    isExclusive = true;
                    filterString = filterString.Substring(1);
                }
                foreach (string s in filterString.Split(',').Select(sValue => sValue.Trim()).ToArray())
                    if (Int32.TryParse(s, out value))
                        filter.Add(value);
            }

            /// <summary>
            /// Creates a line filter instance with given comma-separated text string and dictionary.
            /// </summary>
            /// <param name="filterString">The comma-separated text string.  If starts with '-' then is exclusive.</param>
            /// <param name="dict">A dictionary that contains potential matches to the string elements.
            public LineEntryFilter(string filterString, Dictionary<string, int> dictionary)
            {
                int value;
                if (filterString.StartsWith("-"))
                {
                    isExclusive = true;
                    filterString = filterString.Substring(1);
                }
                foreach (string s in filterString.Split(',').Select(sValue => sValue.Trim()).ToArray())
                {
                    if (dictionary.TryGetValue(s, out value))
                        filter.Add(value);
                    else if (Int32.TryParse(s, out value))
                        filter.Add(value);
                }
            }

            /// <summary>
            /// Returns a value indicating whether the given value passes the filter.
            /// </summary>
            /// <param name="value">The value to check.</param>
            /// <returns>True if the given value passes the filter.</returns>
            public bool PassesFilter(int value)
            {
                return ((0 == filter.Count) || (isExclusive && !filter.Contains(value)) || (!isExclusive && filter.Contains(value)));
            }
        }
        #endregion


        /// <summary>
        /// The CAN interface object.
        /// </summary>
        public ECCONetApi canInterface;


        /// <summary>
        /// Constructor.
        /// </summary>
        public BusMonitor()
        {
            //  initialize UI component
            InitializeComponent();

            //  restore the settings
            restoreSettings();

            //  populate the dictionaries
            populateDictionaries();

            //  populate the send token lines
            populateSendTokenLines();

            //  set the filters
            setFilters();
        }

        #region Token received from bus
        /// <summary>
        /// Token received event.
        /// </summary>
        /// <param name="token">The token that was received.</param>
        public void CanInterface_receiveToken(Token token)
        {
            if (this.tbTokens.InvokeRequired)
            {
                ReceiveTokenDelegate d = new ReceiveTokenDelegate(CanInterface_receiveToken);
                try
                {
                    this.Invoke(d, new object[] { token });
                }
                catch { }
            }
            else
            {
                addTokenToDisplay(token, LineEntry.Direction.In);
            }
        }
        #endregion

        #region Add token to display
        /// <summary>
        /// Token received event.
        /// </summary>
        /// <param name="token">The token that was received.</param>
        private void addTokenToDisplay(Token token, LineEntry.Direction dir)
        {
            //  if not running, just return
            if (!cbRun.Checked)
                    return;

            //  create new line entry from token
            LineEntry newEntry = new LineEntry(token, dir);

            //  if the new entry passes the filters
            if (passesFilters(newEntry))
            {
                //  keep lists in sync, and they should already be in sync
                //  separate lists are used for speed
                if (lineEntries.Count != textBoxLines.Count)
                {
                    textBoxLines = new List<string>(100);
                    foreach (var le in lineEntries)
                        textBoxLines.Add(le.ToString(keyCodeToName, cbShowHex.Checked));
                }

                //  line replaced status
                bool lineReplaced = false;

                //  if an overlay criteria is checked
                if (cbEnAddressOverlay.Checked || cbEnKeyPrefixOverlay.Checked || cbEnKeyOverlay.Checked || cbEnValueOverlay.Checked)
                {
                    int i = 0;
                    for (i = 0; i < lineEntries.Count; ++i)
                    {
                        LineEntry le = lineEntries[i];
                        bool meetsOverlayCriteria = true;
                        if (cbEnAddressOverlay.Checked && (newEntry.address != le.address))
                            meetsOverlayCriteria = false;
                        if (cbEnKeyPrefixOverlay.Checked && (newEntry.keyPrefix != le.keyPrefix))
                            meetsOverlayCriteria = false;
                        if (cbEnKeyOverlay.Checked && (newEntry.key != le.key))
                            meetsOverlayCriteria = false;
                        if (cbEnValueOverlay.Checked && (newEntry.value != le.value))
                            meetsOverlayCriteria = false;
                        if (meetsOverlayCriteria)
                        {
                            /* auto-clear method
                            //  if first match, then replace line
                            if (!lineReplaced)
                            {
                                lineEntries[i] = newEntry;
                                textBoxLines[i] = newEntry.ToString();
                            }
                            else  // remove line
                            {
                                lineEntries.RemoveAt(i);
                                textBoxLines.RemoveAt(i);
                            }
                            //  flag line replaced
                            lineReplaced = true;
                            */

                            lineReplaced = true;
                            newEntry.count = lineEntries[i].count + 1;
                            lineEntries[i] = newEntry;
                            textBoxLines[i] = newEntry.ToString(keyCodeToName, cbShowHex.Checked);
                            break;
                        }
                    }
                }

                //  if line was not replaced, then add line to list
                if (!lineReplaced)
                {
                    lineEntries.Add(newEntry);
                    textBoxLines.Add(newEntry.ToString(keyCodeToName, cbShowHex.Checked));
                }

                //  update text box
                tbTokens.Lines = textBoxLines.ToArray();

            }
        }
        #endregion

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
                    foreach(KeyValuePair<int, string> customKey in customKeys)
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

        #region Line entry lists
        /// <summary>
        /// The list of token lines.
        /// </summary>
        List<LineEntry> lineEntries = new List<LineEntry>(1024);

        /// <summary>
        /// The list of text box lines.
        /// </summary>
        List<string> textBoxLines = new List<string>(1024);


        /// <summary>
        /// Clear button handler clears the token display.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            clearLineEntryLists();
        }

        /// <summary>
        /// Clears the token lists.
        /// </summary>
        private void clearLineEntryLists()
        {
            lineEntries = new List<LineEntry>(100);
            textBoxLines = new List<string>(100);
            tbTokens.Text = String.Empty;
        }
        #endregion

        #region Filters

        /// <summary>
        /// Address filter.
        /// </summary>
        LineEntryFilter addressFilter = new LineEntryFilter();
        bool addressFilterEn = false;

        /// <summary>
        /// Key prefixes filter.
        /// </summary>
        LineEntryFilter keyPrefixFilter = new LineEntryFilter();
        bool keyPrefixFilterEn = false;

        /// <summary>
        /// Key filter.
        /// </summary>
        LineEntryFilter keyFilter = new LineEntryFilter();
        bool keyFilterEn = false;

        /// <summary>
        /// Value filter.
        /// </summary>
        LineEntryFilter valueFilter = new LineEntryFilter();
        bool valueFilterEn = false;


        /// <summary>
        /// Sets the filters based on the filter text box values.
        /// </summary>
        private void setFilters()
        {
            //  address filter
            addressFilter = new LineEntryFilter(tbAddressFilter.Text);
            addressFilterEn = cbEnAddressFilter.Checked;

            //  prefix filter
            keyPrefixFilter = new LineEntryFilter(tbPrefixFilter.Text, keyPrefixNameToCode);
            keyPrefixFilterEn = cbEnKeyPrefixFilter.Checked;

            //  key filter
            keyFilter = new LineEntryFilter(tbKeyFilter.Text, keyNameToCode);
            keyFilterEn = cbEnKeyFilter.Checked;

            //  value filter
            valueFilter = new LineEntryFilter(tbValueFilter.Text);
            valueFilterEn = cbEnValueFilter.Checked;
        }

        /// <summary>
        /// Clear the filter text boxes and the filters.
        /// </summary>
        private void clearFilters()
        {
            //  clear the text boxes
            tbAddressFilter.Text = String.Empty;
            tbPrefixFilter.Text = String.Empty;
            tbKeyFilter.Text = String.Empty;
            tbValueFilter.Text = String.Empty;

            //  set the filters
            setFilters();
        }

        /// <summary>
        /// Returns a value indicating whether the given line entry passes the current filter criteria.
        /// </summary>
        /// <param name="entry">The line entry.</param>
        /// <returns>A value indicating whether a line entry passes the current filter criteria.</returns>
        private bool passesFilters(LineEntry entry)
        {
            if (addressFilterEn && !addressFilter.PassesFilter(entry.address))
                return false;
            if (keyPrefixFilterEn && !keyPrefixFilter.PassesFilter(entry.keyPrefix))
                return false;
            if (keyFilterEn && !keyFilter.PassesFilter(entry.key))
                return false;
            if (valueFilterEn && !valueFilter.PassesFilter(entry.value))
                return false;
            return true;
        }


        /// <summary>
        /// Applies the filters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplyAddressFilter_Click(object sender, EventArgs e)
        {
            //  clear the tokens text box
            clearLineEntryLists();

            //  set the filters
            setFilters();
        }

        /// <summary>
        /// Clears the filters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFilterClear_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Clear all filters?", "Confirm", MessageBoxButtons.YesNo))
                clearFilters();
        }

        /// <summary>
        /// Filter help message box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFilterHelp_Click(object sender, EventArgs e)
        {

        }

        //  address filter control events
        private void tbAddressFilter_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TkFilterAddress = tbAddressFilter.Text;
        }
        private void cbEnAddressFilter_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TkFilterAddressEn = cbEnAddressFilter.Checked;
        }

        //  key prefix filter control events
        private void tbPrefixFilter_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TkFilterKeyPrefix = tbPrefixFilter.Text;
        }
        private void cbEnKeyPrefixFilter_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TkFilterKeyPrefixEn = cbEnKeyPrefixFilter.Checked;
        }

        //  key filter control events
        private void tbKeyFilter_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TkFilterKey = tbKeyFilter.Text;
        }
        private void cbEnKeyFilter_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TkFilterKeyEn = cbEnKeyFilter.Checked;
        }

        //  value filter control events
        private void tbValueFilter_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TkFilterValue = tbValueFilter.Text;
        }
        private void cbEnValueFilter_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TkFilterValueEn = cbEnValueFilter.Checked;
        }
        #endregion

        #region Overlay
        /// <summary>
        /// Address overlay checked clears the token lists.
        /// </summary>
        /// <param name="sender">The checkbox sender.</param>
        /// <param name="e">The event arguments.</param>
        private void cbEnAddressOverlay_CheckedChanged(object sender, EventArgs e)
        {
            clearLineEntryLists();
            Properties.Settings.Default.TkOvAddressEn = cbEnAddressOverlay.Checked;
        }

        /// <summary>
        /// Key prefix overlay checked clears the token lists.
        /// </summary>
        /// <param name="sender">The checkbox sender.</param>
        /// <param name="e">The event arguments.</param>
        private void cbEnKeyPrefixOverlay_CheckedChanged(object sender, EventArgs e)
        {
            clearLineEntryLists();
            Properties.Settings.Default.TkOvKeyPrefixEn = cbEnKeyPrefixOverlay.Checked;
        }

        /// <summary>
        /// Key overlay checked clears the token lists.
        /// </summary>
        /// <param name="sender">The checkbox sender.</param>
        /// <param name="e">The event arguments.</param>
        private void cbEnKeyOverlay_CheckedChanged(object sender, EventArgs e)
        {
            clearLineEntryLists();
            Properties.Settings.Default.TkOvKeyEn = cbEnKeyOverlay.Checked;
        }

        /// <summary>
        /// Value overlay checked clears the token lists.
        /// </summary>
        /// <param name="sender">The checkbox sender.</param>
        /// <param name="e">The event arguments.</param>
        private void cbEnValueOverlay_CheckedChanged(object sender, EventArgs e)
        {
            clearLineEntryLists();
            Properties.Settings.Default.TkOvValueEn = cbEnValueOverlay.Checked;
        }
        #endregion

        #region Send tokens

        class SendTokenLine
        {
            //  members
            public TextBox address;
            public TextBox keyPrefix;
            public TextBox key;
            public TextBox value;
            public CheckBox enabled;
            public Label inputError;
            public Token token;

            public SendTokenLine(TextBox address, TextBox keyPrefix, TextBox key, TextBox value, CheckBox enabled, Label inputError)
            {
                this.address = address;
                this.keyPrefix = keyPrefix;
                this.key = key;
                this.value = value;
                this.enabled = enabled;
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

            /// <summary>
            /// Returns a value indicating whether the line is enabled.
            /// </summary>
            public bool Enabled { get { return enabled.Checked; } }
        }






        /// <summary>
        /// List of send token lines.
        /// </summary>
        const int numSendTokenLines = 10;
        List<SendTokenLine> sendTokenLines = new List<SendTokenLine>(numSendTokenLines);

        /// <summary>
        /// Populate the list of send token lines.
        /// </summary>
        private void populateSendTokenLines()
        {
            sendTokenLines.Add(new SendTokenLine(tbS1Address, tbS1Type, tbS1Key, tbS1Value, cbS1Enable, lblS1Error));
            sendTokenLines.Add(new SendTokenLine(tbS2Address, tbS2Type, tbS2Key, tbS2Value, cbS2Enable, lblS2Error));
            sendTokenLines.Add(new SendTokenLine(tbS3Address, tbS3Type, tbS3Key, tbS3Value, cbS3Enable, lblS3Error));
            sendTokenLines.Add(new SendTokenLine(tbS4Address, tbS4Type, tbS4Key, tbS4Value, cbS4Enable, lblS4Error));
            sendTokenLines.Add(new SendTokenLine(tbS5Address, tbS5Type, tbS5Key, tbS5Value, cbS5Enable, lblS5Error));
            sendTokenLines.Add(new SendTokenLine(tbS6Address, tbS6Type, tbS6Key, tbS6Value, cbS6Enable, lblS6Error));
            sendTokenLines.Add(new SendTokenLine(tbS7Address, tbS7Type, tbS7Key, tbS7Value, cbS7Enable, lblS7Error));
            sendTokenLines.Add(new SendTokenLine(tbS8Address, tbS8Type, tbS8Key, tbS8Value, cbS8Enable, lblS8Error));
            sendTokenLines.Add(new SendTokenLine(tbS9Address, tbS9Type, tbS9Key, tbS9Value, cbS9Enable, lblS9Error));
            sendTokenLines.Add(new SendTokenLine(tbS10Address, tbS10Type, tbS10Key, tbS10Value, cbS10Enable, lblS10Error));
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

        /// <summary>
        /// Sends the enabled tokens to the bus and the display.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void btnSendToken_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; ++i)
            {
                var t = sendTokenLines[i];
                evaluateInputLine(t);
                if (t.Enabled && (null != t.token))
                {
                    if (null != canInterface)
                        canInterface.SendToken(t.token);
                    t.token.address = PC_CAN_Address;
                    addTokenToDisplay(t.token, LineEntry.Direction.Out);
                }
            }
        }

        /// <summary>
        /// Sends the enabled tokens to the bus and the display.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void btnSendToken2_Click(object sender, EventArgs e)
        {
            for (int i = 5; i < 10; ++i)
            {
                var t = sendTokenLines[i];
                evaluateInputLine(t);
                if (t.Enabled && (null != t.token))
                {
                    if (null != canInterface)
                        canInterface.SendToken(t.token);
                    addTokenToDisplay(t.token, LineEntry.Direction.Out);
                }
            }
        }

        /// <summary>
        /// Clears the enabled send token entry text boxes.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void btnSendClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; ++i)
            {
                var t = sendTokenLines[i];
                if (t.Enabled)
                {
                    t.address.Text = String.Empty;
                    t.keyPrefix.Text = String.Empty;
                    t.key.Text = String.Empty;
                    t.value.Text = String.Empty;
                }
            }
        }

        /// <summary>
        /// Clears the enabled send token entry text boxes.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void btnSendClear2_Click(object sender, EventArgs e)
        {
            for (int i = 5; i < 10; ++i)
            {
                var t = sendTokenLines[i];
                if (t.Enabled)
                {
                    t.address.Text = String.Empty;
                    t.keyPrefix.Text = String.Empty;
                    t.key.Text = String.Empty;
                    t.value.Text = String.Empty;
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
            //  send token address
            tbS1Address.Text = Properties.Settings.Default.S1Address;
            tbS2Address.Text = Properties.Settings.Default.S2Address;
            tbS3Address.Text = Properties.Settings.Default.S3Address;
            tbS4Address.Text = Properties.Settings.Default.S4Address;
            tbS5Address.Text = Properties.Settings.Default.S5Address;
            tbS6Address.Text = Properties.Settings.Default.S6Address;
            tbS7Address.Text = Properties.Settings.Default.S7Address;
            tbS8Address.Text = Properties.Settings.Default.S8Address;
            tbS9Address.Text = Properties.Settings.Default.S9Address;
            tbS10Address.Text = Properties.Settings.Default.S10Address;
            //  send token key prefix
            tbS1Type.Text = Properties.Settings.Default.S1KeyPrefix;
            tbS2Type.Text = Properties.Settings.Default.S2KeyPrefix;
            tbS3Type.Text = Properties.Settings.Default.S3KeyPrefix;
            tbS4Type.Text = Properties.Settings.Default.S4KeyPrefix;
            tbS5Type.Text = Properties.Settings.Default.S5KeyPrefix;
            tbS6Type.Text = Properties.Settings.Default.S6KeyPrefix;
            tbS7Type.Text = Properties.Settings.Default.S7KeyPrefix;
            tbS8Type.Text = Properties.Settings.Default.S8KeyPrefix;
            tbS9Type.Text = Properties.Settings.Default.S9KeyPrefix;
            tbS10Type.Text = Properties.Settings.Default.S10KeyPrefix;
            //  send token key
            tbS1Key.Text = Properties.Settings.Default.S1Key;
            tbS2Key.Text = Properties.Settings.Default.S2Key;
            tbS3Key.Text = Properties.Settings.Default.S3Key;
            tbS4Key.Text = Properties.Settings.Default.S4Key;
            tbS5Key.Text = Properties.Settings.Default.S5Key;
            tbS6Key.Text = Properties.Settings.Default.S6Key;
            tbS7Key.Text = Properties.Settings.Default.S7Key;
            tbS8Key.Text = Properties.Settings.Default.S8Key;
            tbS9Key.Text = Properties.Settings.Default.S9Key;
            tbS10Key.Text = Properties.Settings.Default.S10Key;
            //  send token value
            tbS1Value.Text = Properties.Settings.Default.S1Value;
            tbS2Value.Text = Properties.Settings.Default.S2Value;
            tbS3Value.Text = Properties.Settings.Default.S3Value;
            tbS4Value.Text = Properties.Settings.Default.S4Value;
            tbS5Value.Text = Properties.Settings.Default.S5Value;
            tbS6Value.Text = Properties.Settings.Default.S6Value;
            tbS7Value.Text = Properties.Settings.Default.S7Value;
            tbS8Value.Text = Properties.Settings.Default.S8Value;
            tbS9Value.Text = Properties.Settings.Default.S9Value;
            tbS10Value.Text = Properties.Settings.Default.S10Value;
            //  send token enabled
            cbS1Enable.Checked = Properties.Settings.Default.S1Enabled;
            cbS2Enable.Checked = Properties.Settings.Default.S2Enabled;
            cbS3Enable.Checked = Properties.Settings.Default.S3Enabled;
            cbS4Enable.Checked = Properties.Settings.Default.S4Enabled;
            cbS5Enable.Checked = Properties.Settings.Default.S5Enabled;
            cbS6Enable.Checked = Properties.Settings.Default.S6Enabled;
            cbS7Enable.Checked = Properties.Settings.Default.S7Enabled;
            cbS8Enable.Checked = Properties.Settings.Default.S8Enabled;
            cbS9Enable.Checked = Properties.Settings.Default.S9Enabled;
            cbS10Enable.Checked = Properties.Settings.Default.S10Enabled;

            //  address filter
            tbAddressFilter.Text = Properties.Settings.Default.TkFilterAddress;
            cbEnAddressFilter.Checked = Properties.Settings.Default.TkFilterAddressEn;
            //  key prefix filter
            tbPrefixFilter.Text = Properties.Settings.Default.TkFilterKeyPrefix;
            cbEnKeyPrefixFilter.Checked = Properties.Settings.Default.TkFilterKeyPrefixEn;
            //  key filter
            tbKeyFilter.Text = Properties.Settings.Default.TkFilterKey;
            cbEnKeyFilter.Checked = Properties.Settings.Default.TkFilterKeyEn;
            //  value filter
            tbValueFilter.Text = Properties.Settings.Default.TkFilterValue;
            cbEnValueFilter.Checked = Properties.Settings.Default.TkFilterValueEn;

            //  overlay
            cbEnAddressOverlay.Checked = Properties.Settings.Default.TkOvAddressEn;
            cbEnKeyPrefixOverlay.Checked = Properties.Settings.Default.TkOvKeyPrefixEn;
            cbEnKeyOverlay.Checked = Properties.Settings.Default.TkOvKeyEn;
            cbEnValueOverlay.Checked = Properties.Settings.Default.TkOvValueEn;
        }

        //  address
        private void tbS1Address_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[0]);
                Properties.Settings.Default.S1Address = tbS1Address.Text;
            }
        }

        private void tbS2Address_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[1]);
                Properties.Settings.Default.S2Address = tbS2Address.Text;
            }
        }

        private void tbS3Address_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[2]);
                Properties.Settings.Default.S3Address = tbS3Address.Text;
            }
        }

        private void tbS4Address_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[3]);
                Properties.Settings.Default.S4Address = tbS4Address.Text;
            }
        }

        private void tbS5Address_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[4]);
                Properties.Settings.Default.S5Address = tbS5Address.Text;
            }
        }

        private void tbS6Address_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[5]);
                Properties.Settings.Default.S6Address = tbS6Address.Text;
            }
        }

        private void tbS7Address_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[6]);
                Properties.Settings.Default.S7Address = tbS7Address.Text;
            }
        }

        private void tbS8Address_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[7]);
                Properties.Settings.Default.S8Address = tbS8Address.Text;
            }
        }

        private void tbS9Address_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[8]);
                Properties.Settings.Default.S9Address = tbS9Address.Text;
            }
        }

        private void tbS10Address_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[9]);
                Properties.Settings.Default.S10Address = tbS10Address.Text;
            }
        }


        //  key prefix
        private void tbS1Type_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[0]);
                Properties.Settings.Default.S1KeyPrefix = tbS1Type.Text;
            }
        }

        private void tbS2Type_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[1]);
                Properties.Settings.Default.S2KeyPrefix = tbS2Type.Text;
            }
        }

        private void tbS3Type_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[2]);
                Properties.Settings.Default.S3KeyPrefix = tbS3Type.Text;
            }
        }

        private void tbS4Type_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[3]);
                Properties.Settings.Default.S4KeyPrefix = tbS4Type.Text;
            }
        }

        private void tbS5Type_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[4]);
                Properties.Settings.Default.S5KeyPrefix = tbS5Type.Text;
            }
        }

        private void tbS6Type_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[5]);
                Properties.Settings.Default.S6KeyPrefix = tbS6Type.Text;
            }
        }

        private void tbS7Type_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[6]);
                Properties.Settings.Default.S7KeyPrefix = tbS7Type.Text;
            }
        }

        private void tbS8Type_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[7]);
                Properties.Settings.Default.S8KeyPrefix = tbS8Type.Text;
            }
        }

        private void tbS9Type_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[8]);
                Properties.Settings.Default.S9KeyPrefix = tbS9Type.Text;
            }
        }

        private void tbS10Type_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[9]);
                Properties.Settings.Default.S10KeyPrefix = tbS10Type.Text;
            }
        }


        //  key
        private void tbS1Key_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[0]);
                Properties.Settings.Default.S1Key = tbS1Key.Text;
            }
        }

        private void tbS2Key_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[1]);
                Properties.Settings.Default.S2Key = tbS2Key.Text;
            }
        }

        private void tbS3Key_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[2]);
                Properties.Settings.Default.S3Key = tbS3Key.Text;
            }
        }

        private void tbS4Key_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[3]);
                Properties.Settings.Default.S4Key = tbS4Key.Text;
            }
        }

        private void tbS5Key_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[4]);
                Properties.Settings.Default.S5Key = tbS5Key.Text;
            }
        }

        private void tbS6Key_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[5]);
                Properties.Settings.Default.S6Key = tbS6Key.Text;
            }
        }

        private void tbS7Key_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[6]);
                Properties.Settings.Default.S7Key = tbS7Key.Text;
            }
        }

        private void tbS8Key_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[7]);
                Properties.Settings.Default.S8Key = tbS8Key.Text;
            }
        }

        private void tbS9Key_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[8]);
                Properties.Settings.Default.S9Key = tbS9Key.Text;
            }
        }

        private void tbS10Key_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[9]);
                Properties.Settings.Default.S10Key = tbS10Key.Text;
            }
        }


        //  value
        private void tbS1Value_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[0]);
                Properties.Settings.Default.S1Value = tbS1Value.Text;
            }
        }

        private void tbS2Value_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[1]);
                Properties.Settings.Default.S2Value = tbS2Value.Text;
            }
        }

        private void tbS3Value_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[2]);
                Properties.Settings.Default.S3Value = tbS3Value.Text;
            }
        }

        private void tbS4Value_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[3]);
                Properties.Settings.Default.S4Value = tbS4Value.Text;
            }
        }

        private void tbS5Value_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[4]);
                Properties.Settings.Default.S5Value = tbS5Value.Text;
            }
        }

        private void tbS6Value_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[5]);
                Properties.Settings.Default.S6Value = tbS6Value.Text;
            }
        }

        private void tbS7Value_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[6]);
                Properties.Settings.Default.S7Value = tbS7Value.Text;
            }
        }

        private void tbS8Value_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[7]);
                Properties.Settings.Default.S8Value = tbS8Value.Text;
            }
        }

        private void tbS9Value_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[8]);
                Properties.Settings.Default.S9Value = tbS9Value.Text;
            }
        }

        private void tbS10Value_TextChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[9]);
                Properties.Settings.Default.S10Value = tbS10Value.Text;
            }
        }


        //  enable
        private void cbS1Enable_CheckedChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[0]);
                Properties.Settings.Default.S1Enabled = cbS1Enable.Checked;
            }
        }

        private void cbS2Enable_CheckedChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[1]);
                Properties.Settings.Default.S2Enabled = cbS2Enable.Checked;
            }
        }

        private void cbS3Enable_CheckedChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[2]);
                Properties.Settings.Default.S3Enabled = cbS3Enable.Checked;
            }
        }

        private void cbS4Enable_CheckedChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[3]);
                Properties.Settings.Default.S4Enabled = cbS4Enable.Checked;
            }
        }

        private void cbS5Enable_CheckedChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[4]);
                Properties.Settings.Default.S5Enabled = cbS5Enable.Checked;
            }
        }

        private void cbS6Enable_CheckedChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[5]);
                Properties.Settings.Default.S6Enabled = cbS6Enable.Checked;
            }
        }

        private void cbS7Enable_CheckedChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[6]);
                Properties.Settings.Default.S7Enabled = cbS7Enable.Checked;
            }
        }

        private void cbS8Enable_CheckedChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[7]);
                Properties.Settings.Default.S8Enabled = cbS8Enable.Checked;
            }
        }

        private void cbS9Enable_CheckedChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[8]);
                Properties.Settings.Default.S9Enabled = cbS9Enable.Checked;
            }
        }

        private void cbS10Enable_CheckedChanged(object sender, EventArgs e)
        {
            if ((null != sendTokenLines) && (numSendTokenLines == sendTokenLines.Count))
            {
                evaluateInputLine(sendTokenLines[9]);
                Properties.Settings.Default.S10Enabled = cbS10Enable.Checked;
            }
        }


        #endregion
    }
}


#if UNUSED_CODE

        /// <summary>
        /// The CAN interface object.
        /// </summary>
        public ECCONetApi canInterface;
        {
            get => _canInterface;
            set
            {
                _canInterface = value;
                if (null != _canInterface)
                {
                    _canInterface.ReceiveEvents = cbxShowEvents.Checked;
                    _canInterface.ReceiveCurrentStatusMessages = cbxShowCurrentStatusMessages.Checked;
                    _canInterface.ReceiveExpiredStatusMessages = cbxShowExpiredStatusMessages.Checked;
                }
            }
        }
        private ECCONetApi _canInterface;

        private void cbxShowEvents_CheckedChanged(object sender, EventArgs e)
        {
            if (null != canInterface)
                canInterface.ReceiveEvents = cbxShowEvents.Checked;
            Properties.Settings.Default.TM_ShowEvents = cbxShowEvents.Checked;
        }

        private void cbxShowCurrentStatusMessages_CheckedChanged(object sender, EventArgs e)
        {
            if (null != canInterface)
                canInterface.ReceiveCurrentStatusMessages = cbxShowCurrentStatusMessages.Checked;
            Properties.Settings.Default.TM_ShowCurrentStatusMessages = cbxShowCurrentStatusMessages.Checked;

        }

        private void cbxShowExpiredStatusMessages_CheckedChanged(object sender, EventArgs e)
        {
            if (null != canInterface)
                canInterface.ReceiveExpiredStatusMessages = cbxShowExpiredStatusMessages.Checked;
            Properties.Settings.Default.TM_ShowExpiredStatusMessages = cbxShowExpiredStatusMessages.Checked;
        }

            //  event and status message filtering
            cbxShowEvents.Checked = Properties.Settings.Default.TM_ShowEvents;
            cbxShowCurrentStatusMessages.Checked = Properties.Settings.Default.TM_ShowCurrentStatusMessages;
            cbxShowExpiredStatusMessages.Checked = Properties.Settings.Default.TM_ShowExpiredStatusMessages;

#endif
