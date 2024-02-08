using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Serialization;
using Newtonsoft.Json;


namespace ESG.ExpressionLib.DataModels
{
    /// <summary>
    /// A data model for a LED Matrix message collection.
    /// </summary>
    public class LedMatrixMessageCollection
    {
        /// <summary>
        /// The LED Matrix message collection name.
        /// </summary>
        [XmlAttribute("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// The LED Matrix messages in the collection.
        /// </summary>
        [XmlArrayItem("Message", typeof(Expression))]
        [XmlArray("Messages")]
        [JsonProperty("Messages")]
        public BindingList<LedMatrixMessage> Messages { get => _messages; set => _messages = value ?? _messages; }
        private BindingList<LedMatrixMessage> _messages = new BindingList<LedMatrixMessage>();


        /// <summary>
        /// Constructor.
        /// </summary>
        public LedMatrixMessageCollection()
        {
            _messages = new BindingList<LedMatrixMessage>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the collection.</param>
        /// <param name="messages">The messages in the collection.</param>
        public LedMatrixMessageCollection(string name, BindingList<LedMatrixMessage> messages)
        {
            Name = name;
            _messages = messages;
        }

        /// <summary>
        /// Create a deep copy of this LED Matrix message collection.
        /// </summary>
        /// <returns>A deep copy of this LED Matrix message collection.</returns>
        public LedMatrixMessageCollection DeepCopy()
        {
            var lmc = new LedMatrixMessageCollection();
            lmc.Name = Name;
            lmc.Messages = new BindingList<LedMatrixMessage>();
            foreach (var msg in Messages)
                lmc.Messages.Add(msg.DeepCopy());
            return lmc;
        }

    }
}
