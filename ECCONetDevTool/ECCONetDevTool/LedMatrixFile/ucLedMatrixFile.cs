using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using ESG.ExpressionLib.DataModels;
using ESG.ExpressionLib.DataConverters;
using ECCONet;


namespace ECCONetDevTool.LedMatrixFile
{
    public partial class ucLedMatrixFile : UserControl
    {

        //  4-byte security key
        //  2-byte number of messages
        //
        //  2-byte pattern key
        //  2-byte pattern enum
        //  2-byte pattern size (does not include header)
        //  pattern
        //
        //  2-byte pattern key
        //  2-byte pattern enum
        //  2-byte pattern size (does not include header)
        //  pattern
        //  ...
        //  ...
        //  2-byte pattern key
        //  2-byte pattern enum zero


        /// <summary>
        /// The binary generated delegate.
        /// </summary>
        public event FlashFileSystem.FlashFileSystem.BinaryGeneratedDelegate binaryGeneratedDelegate;

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


        /*
        /// <summary>
        /// A LED Matrix message class.
        /// </summary>
        public class LedMatrixMessage
        {
            /// <summary>
            /// The message enumeration
            /// </summary>
            public UInt16 MessageEnum { get; set; }

            /// <summary>
            /// The message text.
            /// </summary>
            public string Text { get; set; }
        }

        /// <summary>
        /// The messages in the data grid view.
        /// </summary>
        private BindingList<LedMatrixMessage> messages;
        */

        //  the messages
        private LedMatrixMessageCollection messageCollection;





        public ucLedMatrixFile()
        {
            //  initialize designer components
            InitializeComponent();

            //  bind the messages
            messageCollection = new LedMatrixMessageCollection();

            /*
            messageCollection.Messages.Add(new LedMatrixMessage() { MessageEnum = 1, Text = "CODE 3 WELCOMES YOU TO NAPFM 2018" });
            messageCollection.Messages.Add(new LedMatrixMessage() { MessageEnum = 2, Text = "CODE 3" });
            messageCollection.Messages.Add(new LedMatrixMessage() { MessageEnum = 3, Text = "ESG" });
            messageCollection.Messages.Add(new LedMatrixMessage() { MessageEnum = 4, Text = "COMMANDER LIGHTBAR" });
            messageCollection.Messages.Add(new LedMatrixMessage() { MessageEnum = 5, Text = "STOP" });
            messageCollection.Messages.Add(new LedMatrixMessage() { MessageEnum = 6, Text = "FOLLOW" });
            */

            messageCollection.Messages.Add(new LedMatrixMessage() { MessageEnum = 1, Text = "STOPP" });
            messageCollection.Messages.Add(new LedMatrixMessage() { MessageEnum = 2, Text = "POLIZEI" });
            messageCollection.Messages.Add(new LedMatrixMessage() { MessageEnum = 3, Text = "BITTE" });
            messageCollection.Messages.Add(new LedMatrixMessage() { MessageEnum = 4, Text = "FOLGEN" });
            messageCollection.Messages.Add(new LedMatrixMessage() { MessageEnum = 5, Text = "UNFALL" });
            dgvMessages.DataSource = messageCollection.Messages;
        }


        #region Online device list
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
        /// Gets the currently-selected device.
        /// </summary>
        /// <returns>The currently-selected device, or null if not available.</returns>
        private ECCONetApi.ECCONetDevice SelectedDevice()
        {
            //  validate selected device
            if ((null == onlineDevices) || (0 == onlineDevices.Count)
                || (cbbOnlineDevices.SelectedIndex >= onlineDevices.Count))
            {
                MessageBox.Show("Selected node not valid.");
                return null;
            }
            return onlineDevices[cbbOnlineDevices.SelectedIndex];
        }
        #endregion





        private void btnBuild_Click(object sender, EventArgs e)
        {
            CompileMessages();
        }

        //  the file key
        const uint matrixMessageFileKey = 0x083FB876;
        const uint matrixMessageKey = 0x9D86;


        private void CompileMessages()
        {
            //  validate messages
            if ((messageCollection == null) || (messageCollection.Messages == null) || (messageCollection.Messages.Count == 0))
                return;

            //  create binary
            List<byte> bytes = new List<byte>(1000);

            //  add file key in little-endian format
            bytes.Add((byte)(matrixMessageFileKey & 0xff));
            bytes.Add((byte)((matrixMessageFileKey >> 8) & 0xff));
            bytes.Add((byte)((matrixMessageFileKey >> 16) & 0xff));
            bytes.Add((byte)((matrixMessageFileKey >> 24) & 0xff));

            //  add number of messages in little-endian format
            bytes.Add((byte)(messageCollection.Messages.Count & 0xff));
            bytes.Add((byte)((messageCollection.Messages.Count >> 8) & 0xff));

            //  for all messages
            foreach (var message in messageCollection.Messages)
            {
                AddMessageBytesWithHandCodedFonts(message, bytes);
            }

            //  finish file with zero enum
            //
            //  add message key in little-endian format
            bytes.Add((byte)(matrixMessageKey & 0xff));
            bytes.Add((byte)((matrixMessageKey >> 8) & 0xff));

            //  add message enum in little-endian format
            bytes.Add(0);
            bytes.Add(0);

            try
            {
                /*
                //  if saving to local files
                if (cbSaveToLocalFile.Checked)
                {
                    //  save the pattern bin file
                    if ((binPatternTable != null) && (saveFileDialogPatternBin.ShowDialog() == DialogResult.OK))
                        File.WriteAllBytes(saveFileDialogPatternBin.FileName, binPatternTable);

                    //  save the dictionary bin file
                    if ((binDictionaries != null) && (saveFileDialogLightEngineDictionaryBin.ShowDialog() == DialogResult.OK))
                        File.WriteAllBytes(saveFileDialogLightEngineDictionaryBin.FileName, binDictionaries);
                }
                */

                //  if saving to flash file system
                if (/*cbSaveToFlashFileSystem.Checked &&*/ (binaryGeneratedDelegate != null))
                {
                    //  send pattern bin to delegate
                    if (bytes != null)
                        binaryGeneratedDelegate(this, "messages.tbl", bytes.ToArray(), messageCollection, 0);
                }
            }
            catch { }
        }

        /// <summary>
        /// Handle enumeration errors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvMessages_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Message enumeration not valid.");
        }

        private void bntShow_Click(object sender, EventArgs e)
        {
            ShowMessage();
        }


        private void ShowMessage()
        {
            if (null == canInterface)
            {
                MessageBox.Show("No can interface!");
                return;
            }

            ECCONetApi.ECCONetDevice device = SelectedDevice();
            if (device == null)
            {
                MessageBox.Show("No device selected!");
                return;
            }

            if (CalculateValue(out int value))
            {
                Token token = null;
                if (rbtnCommon.Checked)
                    token = new Token(Token.Keys.KeyLedMatrixMessage, value, device.address);
                else if (rbtnFront.Checked)
                    token = new Token(Token.Keys.KeyLedMatrixMessageFront, value, device.address);
                else if (rbtnRear.Checked)
                    token = new Token(Token.Keys.KeyLedMatrixMessageRear, value, device.address);
                canInterface.SendToken(token);
            }
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            HideMessage();
        }


        private void HideMessage()
        {
            if (null == canInterface)
            {
                MessageBox.Show("No can interface!");
                return;
            }

            ECCONetApi.ECCONetDevice device = SelectedDevice();
            if (device == null)
            {
                MessageBox.Show("No device selected!");
                return;
            }

            Token token = null;
            if (rbtnCommon.Checked)
                token = new Token(Token.Keys.KeyLedMatrixMessage, 0, device.address);
            else if (rbtnFront.Checked)
                token = new Token(Token.Keys.KeyLedMatrixMessageFront, 0, device.address);
            else if (rbtnRear.Checked)
                token = new Token(Token.Keys.KeyLedMatrixMessageRear, 0, device.address);
            canInterface.SendToken(token);
        }

        private void btnCalcAndShow_Click(object sender, EventArgs e)
        {
            if (CalculateValue(out int value))
            {
                tbxValue.Text = value.ToString();
                tbxValueHex.Text = string.Format("{0:X8}", value);
            }
        }


        private bool CalculateValue(out int value)
        {
            value = 0;

            if ((!ntbEnum.GetUInt32Value(out uint messageEnum)) || (messageEnum >= 1024))
            {
                MessageBox.Show("Enum not valid");
                return false;
            }

            if ((!ntbSpeedPeriod.GetUInt32Value(out uint speedPeriod)) || (speedPeriod >= 32))
            {
                MessageBox.Show("Speed / Period not valid");
                return false;
            }

            if ((!ntbIntensity.GetUInt32Value(out uint intensity)) || (intensity > 100))
            {
                MessageBox.Show("Intensity not valid");
                return false;
            }

            //  get other settings
            int showMode = cbbMode.SelectedIndex;
            int justification = cbbJustification.SelectedIndex;
            int scrollStartOnScreen = cbxScrollStartOnScreen.Checked ? 1 : 0;
            int scrollOnce = cbxScrollOnce.Checked ? 1 : 0;
            int mirror = cbxMirror.Checked ? 1 : 0;
            int wrap = cbxWrap.Checked ? 1 : 0;
            int sequencer = cbxSequencer.Checked ? 1 : 0;

            //  create value
            value = (int)messageEnum & 0x03ff;
            value |= (scrollStartOnScreen << 10);
            value |= (justification << 11);
            value |= (showMode << 13);
            value |= ((int)intensity << (13 + 3));
            value |= ((int)speedPeriod << (13 + 3 + 7));
            value |= (scrollOnce << (13 + 3 + 7 + 5));
            value |= (mirror << (13 + 3 + 7 + 5 + 1));
            value |= (wrap << (13 + 3 + 7 + 5 + 1 + 1));
            value |= (sequencer << (13 + 3 + 7 + 5 + 1 + 1 + 1));

            //  return success
            return true;
        }

        private void btnBuildPatternFile_Click(object sender, EventArgs e)
        {
            BuildPatternFile();
        }

        private void BuildPatternFile()
        { 
            try
            {
                //  read expression file
                ExpressionCollection ec = ExpressionConverters.FromFile(tbxExpressionCollectionFileName.Text);
                if (ec == null)
                {
                    MessageBox.Show("Expression file not valid.");
                    return;
                }

                //  convert to binary file
                byte[] bytes = ExpressionConverters.ToPatternBinary(ec, ExpressionConverters.PatternBinType.LedMatrixKeys);

                if (binaryGeneratedDelegate != null)
                {
                    //  send pattern bin to delegate
                    if (bytes != null)
                        binaryGeneratedDelegate(this, "patterns.tbl", bytes, ec, 0);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void btnFindPatternFile_Click(object sender, EventArgs e)
        {
            if (openFileDialogExpression.ShowDialog() == DialogResult.OK)
                tbxExpressionCollectionFileName.Text = openFileDialogExpression.FileName;
        }

        #region Show big
        /// <summary>
        /// Shows the message bytes in 8x format.
        /// </summary>
        /// <param name="messageBytes"></param>
        private void ShowBig(List<byte> messageBytes)
        {
            int height = 8;
            int width = messageBytes.Count;

            //  show big
            Bitmap big = new Bitmap(width * 8, height * 8);
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    var pixelColor = ((messageBytes[x] & (0x80 >> y)) == 0) ? Color.White : Color.Black;
                    int x2 = x * 8;
                    int y2 = y * 8;
                    for (int x1 = 0; x1 < 8; ++x1)
                    {
                        for (int y1 = 0; y1 < 8; ++y1)
                        {
                            big.SetPixel(x1 + x2, y1 + y2, pixelColor);
                        }
                    }
                }
            }
            pbxTest.Image = big;
        }
        #endregion


        #region Message bytes with Arial Narrow
        /// <summary>
        /// Adds a message's byte with Arial Narrow font.
        /// </summary>
        /// <param name="message">The message to add.</param>
        /// <param name="bytes">The bytes file.</param>
        private void AddMessageBytesWithArialNarrowFont(LedMatrixMessage message, List<byte>bytes)
        {
            string text = message.Text;
            Bitmap bitmap = new Bitmap(1, 1);
            Font font = new Font("Arial Narrow", 11, FontStyle.Regular, GraphicsUnit.Pixel);
            Graphics graphics = Graphics.FromImage(bitmap);
            int width = (int)graphics.MeasureString(text, font).Width;
            int height = (int)graphics.MeasureString(text, font).Height;
            bitmap = new Bitmap(bitmap, new Size(width, height));
            graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            graphics.DrawString(text, font, new SolidBrush(Color.FromArgb(0, 0, 0)), 0, 0);
            graphics.Flush();
            graphics.Dispose();

            //  find x start and end
            int xStart = 0;
            bool found = false;
            for (; xStart < bitmap.Width; ++xStart)
            {
                for (int y = 0; y < bitmap.Height; ++y)
                {
                    if (bitmap.GetPixel(xStart, y).G < 128)
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }

            int xEnd = bitmap.Width - 1;
            found = false;
            for (; xEnd >= 0; --xEnd)
            {
                for (int y = 0; y < bitmap.Height; ++y)
                {
                    if (bitmap.GetPixel(xEnd, y).G < 128)
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }
            ++xEnd;

            //  set y start and end
            int yStart = 4;
            int yEnd = bitmap.Height - 1;

            /*  To test other fonts
            xStart = 0;
            xEnd = bitmap.Width - 1;
            yStart = 0;
            yEnd = bitmap.Height - 1;
            */

            //  get array of bytes
            List<byte> messageBytes = new List<byte>(1000);
            for (int x = xStart; x < xEnd; ++x)
            {
                byte b = 0;
                int bitIndex = 7;
                for (int y = yStart; y < yEnd; ++y, --bitIndex)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    if (pixel.G < 128)
                        b |= (byte)(1 << bitIndex);
                }
                messageBytes.Add(b);
            }

            //  patch
            //if (message.Text == "FOLGEN")
            //    messageBytes.RemoveAt(0);

            //  add message key in little-endian format
            bytes.Add((byte)(matrixMessageKey & 0xff));
            bytes.Add((byte)((matrixMessageKey >> 8) & 0xff));

            //  add message enum in little-endian format
            bytes.Add((byte)(message.MessageEnum & 0xff));
            bytes.Add((byte)((message.MessageEnum >> 8) & 0xff));

            //  add message length in little-endian format
            bytes.Add((byte)(messageBytes.Count & 0xff));
            bytes.Add((byte)((messageBytes.Count >> 8) & 0xff));

            //  add message bytes to bytes
            bytes.AddRange(messageBytes);

            //  keep bytes on 2-byte boundaryies
            if ((bytes.Count & 0x01) != 0)
                bytes.Add(0);

            //  show big
            Bitmap big = new Bitmap(width * 8, height * 8);
            for (int x = 0; x < bitmap.Width; ++x)
            {
                for (int y = 0; y < bitmap.Height; ++y)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    int x2 = x * 8;
                    int y2 = y * 8;
                    for (int x1 = 0; x1 < 8; ++x1)
                    {
                        for (int y1 = 0; y1 < 8; ++y1)
                        {
                            big.SetPixel(x1 + x2, y1 + y2, pixel);
                        }
                    }
                }
            }

            //  draw box around big
            xStart *= 8;
            xEnd *= 8;
            if (xEnd >= big.Width)
                xEnd = big.Width - 1;
            yStart *= 8;
            yEnd *= 8;
            if (yEnd >= big.Height)
                yEnd = big.Height - 1;

            for (int x = xStart; x < xEnd; ++x)
            {
                big.SetPixel(x, yStart, Color.Black);
                big.SetPixel(x, yEnd, Color.Black);
            }
            for (int y = yStart; y < yEnd; ++y)
            {
                big.SetPixel(xStart, y, Color.Black);
                big.SetPixel(xEnd, y, Color.Black);
            }

            pbxTest.Image = big;
        }
        #endregion


        #region Message bytes with hand-coded fonts
        /// <summary>
        /// Adds a message's byte with hand-coded fonts.
        /// </summary>
        /// <param name="message">The message to add.</param>
        /// <param name="bytes">The bytes file.</param>
        private void AddMessageBytesWithHandCodedFonts(LedMatrixMessage message, List<byte>bytes)
        {
            //  get array of font bytes
            List<byte> messageBytes = new List<byte>(1000);

            foreach (char c in message.Text.ToCharArray())
            {
                //  find font
                dotFont font = null;
                foreach (dotFont f in fontTable)
                {
                    if (f.Char == c)
                    {
                        font = f;
                        break;
                    }
                }
                if (font == null)
                    continue;

                //  add font
                for (int width = 0; width < font.Size.Width; ++width)
                {
                    int bit = 0;
                    byte b = 0;
                    int index = width;
                    for (; bit < 8; ++bit, index += font.Size.Width)
                    {
                        if (font.Bytes[index] > 0)
                            b |= (byte)(0x80 >> bit);
                    }
                    messageBytes.Add(b);
                }

                //  add space
                messageBytes.Add(0);
            }
            //  remove last space
            messageBytes.RemoveAt(messageBytes.Count - 1);

            //  add message key in little-endian format
            bytes.Add((byte)(matrixMessageKey & 0xff));
            bytes.Add((byte)((matrixMessageKey >> 8) & 0xff));

            //  add message enum in little-endian format
            bytes.Add((byte)(message.MessageEnum & 0xff));
            bytes.Add((byte)((message.MessageEnum >> 8) & 0xff));

            //  add message length in little-endian format
            bytes.Add((byte)(messageBytes.Count & 0xff));
            bytes.Add((byte)((messageBytes.Count >> 8) & 0xff));

            //  add message bytes to bytes
            bytes.AddRange(messageBytes);

            //  keep bytes on 2-byte boundaryies
            if ((bytes.Count & 0x01) != 0)
                bytes.Add(0);

            //  show message bytes
            ShowBig(messageBytes);
        }

        /// <summary>
        /// A dot-font class for building fonts.
        /// </summary>
        private class dotFont
        {
            public char Char;
            public Size Size;

            public byte[] Bytes;
        }

        /// <summary>
        /// A hand-code font list!
        /// </summary>
        private List<dotFont> fontTable = new List<dotFont> {
            new dotFont() { Char = 'A', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 0, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 1, 1, 1, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1 } },

            new dotFont() { Char = 'B', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 1, 1, 1, 0 } },

            new dotFont() { Char = 'C', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 0, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 1,
              0, 1, 1, 1, 0 } },

            new dotFont() { Char = 'D', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 1, 1, 1, 0 } },

            new dotFont() { Char = 'E', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 1, 1, 1, 1,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 1, 1, 1, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 1, 1, 1, 1 } },

            new dotFont() { Char = 'F', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 1, 1, 1, 1,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 1, 1, 1, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0 } },

            new dotFont() { Char = 'G', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 0, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 1, 1,
              1, 0, 0, 0, 1,
              0, 1, 1, 1, 1 } },

            new dotFont() { Char = 'H', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 1, 1, 1, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1 } },

            /* I 5 wide
            new dotFont() { Char = 'I', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 1, 1, 1, 1,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              1, 1, 1, 1, 1 } }, */

            /* I 3 wide
            new dotFont() { Char = 'I', Size = { Width = 3, Height = 8 }, Bytes = new byte[]
            { 1, 1, 1,
              0, 1, 0,
              0, 1, 0,
              0, 1, 0,
              0, 1, 0,
              0, 1, 0,
              0, 1, 0,
              1, 1, 1 } }, */

            // I 1 wide
            new dotFont() { Char = 'I', Size = { Width = 1, Height = 8 }, Bytes = new byte[]
            { 1,
              1,
              1,
              1,
              1,
              1,
              1,
              1 } },


            new dotFont() { Char = 'J', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 1, 1, 1, 1,
              0, 0, 0, 0, 1,
              0, 0, 0, 0, 1,
              0, 0, 0, 0, 1,
              0, 0, 0, 0, 1,
              0, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              0, 1, 1, 1, 0 } },

            new dotFont() { Char = 'K', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 1, 0,
              1, 1, 1, 0, 0,
              1, 0, 0, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1 } },

            // L 5 wide
            new dotFont() { Char = 'L', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 1, 1, 1, 1 } },

            /* L 4 wide
            new dotFont() { Char = 'L', Size = { Width = 4, Height = 8 }, Bytes = new byte[]
            { 1, 0, 0, 0,
              1, 0, 0, 0,
              1, 0, 0, 0,
              1, 0, 0, 0,
              1, 0, 0, 0,
              1, 0, 0, 0,
              1, 0, 0, 0,
              1, 1, 1, 1 } }, */

            new dotFont() { Char = 'M', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 0, 0, 0, 1,
              1, 1, 0, 1, 1,
              1, 1, 0, 1, 1,
              1, 0, 1, 0, 1,
              1, 0, 1, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1 } },

            new dotFont() { Char = 'N', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 0, 0, 0, 1,
              1, 1, 0, 0, 1,
              1, 1, 0, 0, 1,
              1, 0, 1, 0, 1,
              1, 0, 1, 0, 1,
              1, 0, 0, 1, 1,
              1, 0, 0, 1, 1,
              1, 0, 0, 0, 1 } },

            new dotFont() { Char = 'O', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 0, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              0, 1, 1, 1, 0 } },

            new dotFont() { Char = 'P', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 1, 1, 1, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0 } },

            new dotFont() { Char = 'Q', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 0, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 1, 0,
              0, 1, 1, 0, 1 } },

            new dotFont() { Char = 'R', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1 } },

            new dotFont() { Char = 'S', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 0, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 0,
              0, 1, 1, 1, 0,
              0, 0, 0, 0, 1,
              0, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              0, 1, 1, 1, 0 } },

            new dotFont() { Char = 'T', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 1, 1, 1, 1,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0 } },

            new dotFont() { Char = 'U', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              0, 1, 1, 1, 0 } },

            new dotFont() { Char = 'V', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              0, 1, 0, 1, 0,
              0, 1, 0, 1, 0,
              0, 1, 0, 1, 0,
              0, 0, 1, 0, 0 } },

            new dotFont() { Char = 'W', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 1, 0, 1,
              1, 0, 1, 0, 1,
              1, 0, 1, 0, 1,
              0, 1, 0, 1, 0,
              0, 1, 0, 1, 0 } },

            new dotFont() { Char = 'X', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              0, 1, 0, 1, 0,
              0, 0, 1, 0, 0,
              0, 1, 0, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1 } },

            new dotFont() { Char = 'Y', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              0, 1, 0, 1, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0 } },

            /* Z 5 wide
            new dotFont() { Char = 'Z', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 1, 1, 1, 1,
              0, 0, 0, 0, 1,
              0, 0, 0, 1, 0,
              0, 0, 1, 0, 0,
              0, 1, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 1, 1, 1, 1 } }, */

            //  Z 6 wide
            new dotFont() { Char = 'Z', Size = { Width = 6, Height = 8 }, Bytes = new byte[]
            { 1, 1, 1, 1, 1, 1,
              0, 0, 0, 0, 0, 1,
              0, 0, 0, 0, 1, 0,
              0, 0, 0, 1, 0, 0,
              0, 0, 1, 0, 0, 0,
              0, 1, 0, 0, 0, 0,
              1, 0, 0, 0, 0, 0,
              1, 1, 1, 1, 1, 1 } },

            new dotFont() { Char = '0', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 0, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 1, 1,
              1, 0, 1, 0, 1,
              1, 1, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              0, 1, 1, 1, 0 } },

            new dotFont() { Char = '1', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 1, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              0, 0, 1, 0, 0,
              1, 1, 1, 1, 1 } },

            new dotFont() { Char = '2', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 0, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              0, 0, 0, 0, 1,
              0, 0, 0, 1, 0,
              0, 0, 1, 0, 0,
              0, 1, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 1, 1, 1, 1 } },

            new dotFont() { Char = '3', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 0, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              0, 0, 0, 0, 1,
              0, 1, 1, 1, 0,
              0, 0, 0, 0, 1,
              0, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              0, 1, 1, 1, 0 } },

            new dotFont() { Char = '4', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 0, 0, 0, 1, 0,
              0, 0, 1, 1, 0,
              0, 1, 0, 1, 0,
              1, 1, 1, 1, 1,
              0, 0, 0, 1, 0,
              0, 0, 0, 1, 0,
              0, 0, 0, 1, 0,
              0, 0, 0, 1, 0 } },

            new dotFont() { Char = '5', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 1, 1, 1, 1,
              1, 0, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 1, 1, 1, 0,
              0, 0, 0, 0, 1,
              0, 0, 0, 0, 1,
              0, 0, 0, 0, 1,
              1, 1, 1, 1, 0 } },

            new dotFont() { Char = '6', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 0, 0, 1, 1, 0,
              0, 1, 0, 0, 0,
              1, 0, 0, 0, 0,
              1, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              0, 1, 1, 1, 0 } },

            new dotFont() { Char = '7', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 1, 1, 1, 1, 1,
              0, 1, 0, 0, 1,
              0, 0, 0, 1, 0,
              0, 0, 1, 0, 0,
              0, 1, 0, 0, 0,
              0, 1, 0, 0, 0,
              0, 1, 0, 0, 0,
              0, 1, 0, 0, 0 } },

            new dotFont() { Char = '8', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 0, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              0, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              0, 1, 1, 1, 0 } },

            new dotFont() { Char = '9', Size = { Width = 5, Height = 8 }, Bytes = new byte[]
            { 0, 1, 1, 1, 0,
              1, 0, 0, 0, 1,
              1, 0, 0, 0, 1,
              0, 1, 1, 1, 1,
              0, 0, 0, 0, 1,
              0, 0, 0, 0, 1,
              0, 0, 0, 1, 0,
              0, 1, 1, 0, 0 } },

            new dotFont() { Char = ' ', Size = { Width = 3, Height = 8 }, Bytes = new byte[]
            { 0, 0, 0,
              0, 0, 0,
              0, 0, 0,
              0, 0, 0,
              0, 0, 0,
              0, 0, 0,
              0, 0, 0,
              0, 0, 0, } },

            new dotFont() { Char = '-', Size = { Width = 3, Height = 8 }, Bytes = new byte[]
            { 0, 0, 0,
              0, 0, 0,
              0, 0, 0,
              0, 0, 0,
              1, 1, 1,
              0, 0, 0,
              0, 0, 0,
              0, 0, 0, } },

            new dotFont() { Char = '.', Size = { Width = 3, Height = 8 }, Bytes = new byte[]
            { 0, 
              0, 
              0, 
              0, 
              0, 
              0, 
              0, 
              1, } },
            };

        #endregion






    }
}
