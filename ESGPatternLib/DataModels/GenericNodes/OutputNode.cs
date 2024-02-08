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
    /// An output node class.  Examples include single-wire outputs, dedicated LED outputs on lightbar PCAs,
    /// and light engine LEDs.
    /// </summary>
    [XmlRoot("Output")]
    [JsonObject("Output")]
    public class OutputNode : ComponentTreeNode
    {
        /// <summary>
        /// Indicates the output type.
        /// </summary>
        [XmlAttribute("Type")]
        [JsonProperty("OutputType")]
        public string OutputType { get; set; }

        /// <summary>
        /// Indicates whether should serialize output type.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeOutputType()
        {
            bool belongsToArray = false;
            ComponentTreeNode node = ParentNode;
            while (node != null)
            {
                if (node is OutputArrayNode)
                {
                    belongsToArray = true;
                    break;
                }
                node = node.ParentNode;
            }
            return !belongsToArray;
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public OutputNode() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        public OutputNode(int id,  Location location)
        {
            Id = id;
            Location = location;
        }
    }

}
