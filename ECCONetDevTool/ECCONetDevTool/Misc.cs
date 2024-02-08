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
using static ECCONet.ECCONetApi;


namespace ECCONetDevTool
{
    public partial class Misc : UserControl
    {
        /// <summary>
        /// The CAN interface object.
        /// </summary>
        public ECCONetApi canInterface
        {
            get => _canInterface;
            set
            {
                _canInterface = value;
                ucPatternSequencer1.CanInterface = value;
                ucPatternSequencer2.CanInterface = value;
                ucPatternSequencer3.CanInterface = value;
                ucPatternSequencer4.CanInterface = value;
                ucPatternSequencer5.CanInterface = value;
                ucPatternSequencer6.CanInterface = value;
                ucPatternSequencer7.CanInterface = value;
                ucPatternSequencer8.CanInterface = value;
                ucPatternSequencer9.CanInterface = value;
                ucPatternSequencer10.CanInterface = value;
            }
        }
        private ECCONetApi _canInterface;

        /// <summary>
        /// The list of online devices.
        /// This list is populated by calling ProductInfoScanner.ScanForECCONetDevices
        /// a few seconds after the devices have booted.
        /// </summary>
        public List<ECCONetApi.ECCONetDevice> onlineDevices
        {
            get => _onlineDevices;
            set
            {
                _onlineDevices = value;
                ucPatternSequencer1.onlineDevices = value;
                ucPatternSequencer2.onlineDevices = value;
                ucPatternSequencer3.onlineDevices = value;
                ucPatternSequencer4.onlineDevices = value;
                ucPatternSequencer5.onlineDevices = value;
                ucPatternSequencer6.onlineDevices = value;
                ucPatternSequencer7.onlineDevices = value;
                ucPatternSequencer8.onlineDevices = value;
                ucPatternSequencer9.onlineDevices = value;
                ucPatternSequencer10.onlineDevices = value;
            }
        }
        private List<ECCONetApi.ECCONetDevice> _onlineDevices;

        /// <summary>
        /// The light stepper enumeration.
        /// </summary>
        Token.Keys lightStepperEnum;


        /// <summary>
        /// Constructor.
        /// </summary>
        public Misc()
        {
            //  initialize designer components
            InitializeComponent();

            //  assign sequencer indices
            ucPatternSequencer1.SequencerIndex = 0;
            ucPatternSequencer2.SequencerIndex = 1;
            ucPatternSequencer3.SequencerIndex = 2;
            ucPatternSequencer4.SequencerIndex = 3;
            ucPatternSequencer5.SequencerIndex = 4;
            ucPatternSequencer6.SequencerIndex = 5;
            ucPatternSequencer7.SequencerIndex = 6;
            ucPatternSequencer8.SequencerIndex = 7;
            ucPatternSequencer9.SequencerIndex = 8;
            ucPatternSequencer10.SequencerIndex = 9;
        }

        #region Online device list changed handler
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

        #region Device selection changed handler
        /// <summary>
        /// Device selection changed handler.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void cbbOnlineDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((null != onlineDevices) && (0 != onlineDevices.Count)
                && (cbbOnlineDevices.SelectedIndex < onlineDevices.Count))
            {
                //  set the base and max light enums
                ECCONetApi.ECCONetDevice device = onlineDevices[cbbOnlineDevices.SelectedIndex];
                tbBaseEnum.Text = device.baseLightheadEnumeration;
                tbMaxEnum.Text = device.maxLightheadEnumeration;
                labelCurrentEnum.Text = "Current Lighthead Enum: " + lightStepperEnum.ToString();
            }

            //  pattern sequencers
            ucPatternSequencer1.SelectedOnlineDeviceIndex = cbbOnlineDevices.SelectedIndex;
            ucPatternSequencer2.SelectedOnlineDeviceIndex = cbbOnlineDevices.SelectedIndex;
            ucPatternSequencer3.SelectedOnlineDeviceIndex = cbbOnlineDevices.SelectedIndex;
            ucPatternSequencer4.SelectedOnlineDeviceIndex = cbbOnlineDevices.SelectedIndex;
            ucPatternSequencer5.SelectedOnlineDeviceIndex = cbbOnlineDevices.SelectedIndex;
            ucPatternSequencer6.SelectedOnlineDeviceIndex = cbbOnlineDevices.SelectedIndex;
            ucPatternSequencer7.SelectedOnlineDeviceIndex = cbbOnlineDevices.SelectedIndex;
            ucPatternSequencer8.SelectedOnlineDeviceIndex = cbbOnlineDevices.SelectedIndex;
            ucPatternSequencer9.SelectedOnlineDeviceIndex = cbbOnlineDevices.SelectedIndex;
            ucPatternSequencer10.SelectedOnlineDeviceIndex = cbbOnlineDevices.SelectedIndex;
        }
        #endregion

        #region Light Stepper
        /// <summary>
        /// Turns off the last stepped light and resets the light stepper enum.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void btnLightStepReset_Click(object sender, EventArgs e)
        {
            if ((null != onlineDevices) && (0 != onlineDevices.Count)
                && (cbbOnlineDevices.SelectedIndex < onlineDevices.Count))
            {
                //  turn off most recent light
                canInterface.SendToken(new Token(lightStepperEnum, 0,
                onlineDevices[cbbOnlineDevices.SelectedIndex].address));
            }
            lightStepperEnum = 0;
            labelCurrentEnum.Text = "Current Lighthead Enum: " + lightStepperEnum.ToString();
        }


        /// <summary>
        /// Turns off the last stepped light and turns on the next stepped light.
        /// </summary>
        /// <param name="sender">The button sender.</param>
        /// <param name="e">The event arguments.</param>
        private void btnLightStepperNext_Click(object sender, EventArgs e)
        {
            Int16 baseEnum = GetLightStepperBaseEnum();
            Int16 maxEnum = GetLightStepperMaxEnum();
            sbyte intensity = GetLightStepperIntensity();
            if ((0 > baseEnum) || (0 > maxEnum) || (0 > intensity))
                return;

            if ((null != onlineDevices) && (0 != onlineDevices.Count)
                && (cbbOnlineDevices.SelectedIndex < onlineDevices.Count))
            {
                //  if first step
                if (0 == lightStepperEnum)
                    lightStepperEnum = (Token.Keys)baseEnum;
                else
                {
                    //  turn off most recent light and bump the enum
                    canInterface.SendToken(new Token(lightStepperEnum, 0,
                    onlineDevices[cbbOnlineDevices.SelectedIndex].address));
                    ++lightStepperEnum;
                    if (lightStepperEnum > (Token.Keys)maxEnum)
                        lightStepperEnum = (Token.Keys)baseEnum;
                }

                //  show the current enum
                labelCurrentEnum.Text = "Current Lighthead Enum: " + lightStepperEnum.ToString();

                //  turn on next light
                canInterface.SendToken(new Token(lightStepperEnum, intensity,
                onlineDevices[cbbOnlineDevices.SelectedIndex].address));
            }

        }

        /// <summary>
        /// Helper method converts the light stepper intensity input box into a value.
        /// </summary>
        /// <returns>The light intensity, or -1 if bad input.</returns>
        sbyte GetLightStepperIntensity()
        {
            bool goodInput = true;
            sbyte intensity = 0;

            try
            {
                intensity = Convert.ToSByte(tbLightIntensity.Text);
                if (100 < intensity)
                    intensity = 100;
                if ((1 <= intensity) && (9 >= intensity))
                    intensity = 10;
            }
            catch
            {
                goodInput = false;
            }
            if (goodInput)
                return intensity;
            return -1;
        }

        /// <summary>
        /// Helper method converts the light stepper base enumeration input box into a value.
        /// </summary>
        /// <returns>The light intensity, or -1 if bad input.</returns>
        Int16 GetLightStepperBaseEnum()
        {
            bool goodInput = true;
            Int16 baseEnum = 0;

            try
            {
                baseEnum = Convert.ToInt16(tbBaseEnum.Text);
            }
            catch
            {
                goodInput = false;
            }
            if (goodInput)
                return baseEnum;
            return -1;
        }

        /// <summary>
        /// Helper method converts the light stepper base enumeration input box into a value.
        /// </summary>
        /// <returns>The light intensity, or -1 if bad input.</returns>
        Int16 GetLightStepperMaxEnum()
        {
            bool goodInput = true;
            Int16 maxEnum = 0;

            try
            {
                maxEnum = Convert.ToInt16(tbMaxEnum.Text);
            }
            catch
            {
                goodInput = false;
            }
            if (goodInput)
                return maxEnum;
            return -1;
        }

        #endregion

        #region Flash erase
        private void btnEraseApp_Click(object sender, EventArgs e)
        {
            canInterface.SendToken(new Token(Token.Keys.KeyRequestEraseAppFirmware,
                (int)Token.TOKEN_VALUE_ERASE_APP_FIRMWARE ^ (int)onlineDevices[cbbOnlineDevices.SelectedIndex].serverAccessCode,
                onlineDevices[cbbOnlineDevices.SelectedIndex].address));
        }

        private void btnEraseAll_Click(object sender, EventArgs e)
        {
            uint code = Token.TOKEN_VALUE_ERASE_ALL_FIRMWARE;
            canInterface.SendToken(new Token(Token.Keys.KeyRequestEraseAllFirmware,
                (int)code ^ (int)onlineDevices[cbbOnlineDevices.SelectedIndex].serverAccessCode,
                onlineDevices[cbbOnlineDevices.SelectedIndex].address));
        }

        #endregion

    }
}
