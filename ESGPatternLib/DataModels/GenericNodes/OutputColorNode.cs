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
    /// An output color node array class.  Examples include light heads with specific colors.
    /// </summary>
    [XmlRoot("ColorOutput")]
    [JsonObject("ColorOutput")]
    public class OutputColorNode : OutputNode
    {
        /// <summary>
        /// The color of this output, for example "Red", "Blue", "Amber", etc.
        /// </summary>
        [XmlAttribute("Color")]
        [JsonProperty("Color")]
        public string Color { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public OutputColorNode() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        public OutputColorNode(ComponentTreeNode[] children)
        {
            ChildNodes.AddRange(children);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public OutputColorNode(int id, string color, ComponentTreeNode[] children)
        {
            //_childNodes = new ComponentTreeNodeCollection<LED>(this);
            Id = id;
            Color = color;
            ChildNodes.AddRange(children);
        }

    }
}
