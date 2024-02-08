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


namespace ECCONetDevTool
{
    public partial class ucPatternSequencer : UserControl
    {
        /// <summary>
        /// The CAN interface object.
        /// </summary>
        public ECCONetApi CanInterface { get; set; }

        /// <summary>
        /// The list of online devices.
        /// This list is populated by calling ProductInfoScanner.ScanForECCONetDevices
        /// a few seconds after the devices have booted.
        /// </summary>
        public List<ECCONetApi.ECCONetDevice> onlineDevices { get; set; }

        /// <summary>
        /// The user-selected online device.
        /// </summary>
        public int SelectedOnlineDeviceIndex { get; set; }

        /// <summary>
        /// The sequencer index
        /// </summary>
        public int SequencerIndex { get; set; }



        public ucPatternSequencer()
        {
            InitializeComponent();
        }

        #region Pattern Sequencer

        /// <summary>
        /// Requests to run a pattern for the selected online device, selected sequencer, and selected pattern.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void btnRunPattern_Click(object sender, EventArgs e)
        {
            if ((null != onlineDevices) && (0 != onlineDevices.Count)
                && (SelectedOnlineDeviceIndex < onlineDevices.Count))
            {
                Int32 value = GetSequencerValues();
                if (0 <= value)
                    CanInterface.SendToken(new Token(Token.Keys.KeyIndexedTokenSequencerWithPattern,
                        value, onlineDevices[SelectedOnlineDeviceIndex].address));
            }
        }

        /// <summary>
        /// Requests to stop running a pattern for the selected online device and selected sequencer.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void btnStopPattern_Click(object sender, EventArgs e)
        {
            if ((null != onlineDevices) && (0 != onlineDevices.Count)
                && (SelectedOnlineDeviceIndex < onlineDevices.Count))
            {
                Int32 value = GetSequencerValues();
                if (0 <= value)
                    CanInterface.SendToken(new Token(Token.Keys.KeyIndexedTokenSequencerWithPattern,
                        value & 0xff, onlineDevices[SelectedOnlineDeviceIndex].address));
            }
        }

        /// <summary>
        /// Helper method converts the sequencer input boxes into a concatenated token value.
        /// </summary>
        /// <returns>The concatenated token value, or -1 if bad input.</returns>
        Int32 GetSequencerValues()
        {
            bool goodInputs = true;
            Byte intensity = 0;
            UInt16 patternEnum = 0;

            try
            {
                intensity = Convert.ToByte(textBoxIntensity.Text);
                if (100 < intensity)
                    intensity = 100;
                if ((1 <= intensity) && (9 >= intensity))
                    intensity = 10;
            }
            catch
            {
                goodInputs = false;
            }
            try
            {
                patternEnum = Convert.ToUInt16(textBoxPatternEnum.Text);
            }
            catch
            {
                goodInputs = false;
            }
            if (goodInputs)
                return (patternEnum << 16) | (intensity << 8) | SequencerIndex;
            return -1;
        }

        #endregion


    }
}
