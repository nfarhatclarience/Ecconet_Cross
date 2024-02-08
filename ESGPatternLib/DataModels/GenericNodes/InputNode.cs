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
    /// An input node class.  Examples include single-wire inputs and buttons.
    /// </summary>
    [XmlRoot("Input")]
    [JsonObject("Input")]
    public class InputNode : ComponentTreeNode
    {
        /// <summary>
        /// Indicates the input type.
        /// </summary>
        [XmlAttribute("Type")]
        [JsonProperty("InputType")]
        public string InputType { get; set; }

        /// <summary>
        /// Indicates whether should serialize input type.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeInputType()
        {
            bool belongsToArray = false;
            ComponentTreeNode node = ParentNode;
            while (node != null)
            {
                if (node is InputArrayNode)
                {
                    belongsToArray = true;
                    break;
                }
                node = node.ParentNode;
            }
            return !belongsToArray;
        }

    }
}
