using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECCONetDevTool
{
    public class NumericTextBox : TextBox 
    {
        /// <summary>
        /// Set true to turn text red on input error.
        /// </summary>
        public bool HighlightInputError { get; set; }


        /// <summary>
        /// Set true to accept hex input.
        /// </summary>
        public bool AcceptHexInput { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public NumericTextBox() : base()
        {
            HighlightInputError = true;
            AcceptHexInput = true;
        }



        /// <summary>
        /// Gets the Byte value represented by the text box text.
        /// </summary>
        /// <param name="value">The value output.</param>
        /// <returns>Returns true if parsing is succcessful.</returns>
        public bool GetByteValue(out Byte value)
        {
            value = 0;
            bool goodInput = false;

            //  get value
            if (AcceptHexInput && Text.StartsWith("0x"))
                goodInput = Byte.TryParse(Text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out value);
            else
                goodInput = Byte.TryParse(Text, out value);

            //  set control color
            if (HighlightInputError)
                ForeColor = goodInput ? Color.Black : Color.DarkRed;

            //  return input status
            return goodInput;
        }



        /// <summary>
        /// Get the UInt32 value represented by the text box text.
        /// </summary>
        /// <param name="value">The value output.</param>
        /// <returns>Returns true if parsing is succcessful.</returns>
        public bool GetUInt32Value(out UInt32 value)
        {
            value = 0;
            bool goodInput = false;

            //  get value
            if (AcceptHexInput && Text.StartsWith("0x"))
                goodInput = UInt32.TryParse(Text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out value);
            else
                goodInput = UInt32.TryParse(Text, out value);

            //  set control color
            if (HighlightInputError)
                ForeColor = goodInput ? Color.Black : Color.DarkRed;

            //  return input status
            return goodInput;
        }


        /// <summary>
        /// Get the Single value represented by the text box text.
        /// </summary>
        /// <param name="value">The value output.</param>
        /// <returns>Returns true if parsing is succcessful.</returns>
        public bool GetSingleValue(out float value)
        {
            value = 0;
            bool goodInput = false;

            //  get value
            goodInput = Single.TryParse(Text, out value);

            //  set control color
            if (HighlightInputError)
                ForeColor = goodInput ? Color.Black : Color.DarkRed;

            //  return input status
            return goodInput;
        }


        /// <summary>
        /// Get the Double value represented by the text box text.
        /// </summary>
        /// <param name="value">The value output.</param>
        /// <returns>Returns true if parsing is succcessful.</returns>
        public bool GetDoubleValue(out double value)
        {
            value = 0;
            bool goodInput = false;

            //  get value
            goodInput = Double.TryParse(Text, out value);

            //  set control color
            if (HighlightInputError)
                ForeColor = goodInput ? Color.Black : Color.DarkRed;

            //  return input status
            return goodInput;
        }









    }
}
