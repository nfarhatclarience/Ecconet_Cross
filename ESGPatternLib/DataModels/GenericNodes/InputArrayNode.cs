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
    /// An input node array class.  Examples include a set of single-wire inputs or an array of buttons.
    /// </summary>
    [XmlRoot("InputArray")]
    [JsonObject("InputArray")]
    public class InputArrayNode : ComponentTreeNode
    {
        /// <summary>
        /// Indicates what type of inputs are in the array.
        /// </summary>
        [XmlAttribute("Type")]
        [JsonProperty("InputArrayType")]
        public string InputArrayType { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public InputArrayNode()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public InputArrayNode(ComponentTreeNode[] children)
        {
            ChildNodes.AddRange(children);
        }
    }
}
