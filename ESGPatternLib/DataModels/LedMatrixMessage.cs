using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;


namespace ESG.ExpressionLib.DataModels
{
    /// <summary>
    /// A data model for a LED Matrix message.
    /// </summary>
    [XmlType("Message")]
    [JsonObject("Message")]
    public class LedMatrixMessage
    {
        /// <summary>
        /// The message enumeration.
        /// </summary>
        [XmlAttribute("Enum")]
        [JsonProperty("Enum")]
        public UInt16 MessageEnum { get; set; }

        /// <summary>
        /// The message text.
        /// </summary>
        [XmlAttribute("Text")]
        [JsonProperty("Text")]
        public string Text { get => _text; set => _text = value ?? _text; }
        private string _text = string.Empty;


        /// <summary>
        /// Returns a deep copy of this message.
        /// </summary>
        /// <returns>Returns a deep copy of this message.</returns>
        public LedMatrixMessage DeepCopy()
        {
            return new LedMatrixMessage() { MessageEnum = MessageEnum, Text = Text };
        }

    }
}
