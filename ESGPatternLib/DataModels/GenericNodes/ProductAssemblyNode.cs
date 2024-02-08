using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ESG.ExpressionLib.DataModels
{
    /// <summary>
    /// The product assembly data model is the root tree node of a product class.
    /// </summary>
    [XmlRoot("ProductAssembly")]
    [JsonObject("ProductAssembly")]
    public class ProductAssemblyNode : ComponentTreeNode
    {
        /// <summary>
        /// The IdPair class is used for the condensed Json assembly.
        /// </summary>
        public class IdPair
        {
            /// <summary>
            /// The main Id.
            /// </summary>
            public int Id;

            /// <summary>
            /// An alternate Id, used for processing such as random to location-based Id.
            /// </summary>
            public int AltId;
        }

        #region Alt Id
        /// <summary>
        /// The node ID.
        /// </summary>
        [XmlAttribute("AltId")]
        [JsonProperty("AltId")]
        public int AltId { get; set; }

        /// <summary>
        /// Control Id serialization.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeAltId()
        {
            return SerializeAltId;
        }
        public static bool SerializeAltId = false;
        #endregion

        /// <summary>
        /// The product model name.
        /// </summary>
        [XmlAttribute("ModelName")]
        [JsonProperty("ModelName")]
        public string ModelName { get; set; }

        /// <summary>
        /// The product manufacturer name.
        /// </summary>
        [XmlAttribute("ManufacturerName")]
        [JsonProperty("ManufacturerName")]
        public string ManufacturerName { get; set; }

        /// <summary>
        /// Don't serialize the Id.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldSerializeId() { return false;  }

        /// <summary>
        /// List of assembly locations, only used for output array type "S6", else null.
        /// </summary>
        [JsonProperty("Ids")]
        public List<IdPair> Ids { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public ProductAssemblyNode()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ProductAssemblyNode(string modelName, ComponentTreeNode[] lightEngines)
        {
            ModelName = modelName;
            ChildNodes.AddRange(lightEngines);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ProductAssemblyNode(string modelName, Location location, ComponentTreeNode[] lightEngines)
        {
            ModelName = modelName;
            Location = location;
            ChildNodes.AddRange(lightEngines);
        }

        /// <summary>
        /// In the XML and JSON model files, unison output endpoints don't have to have their Ids defined,
        /// so this method sets them.
        /// </summary>
        /// <param name="node">The recursion node.</param>
        public static void EnumerateUnisonEndpoints(ComponentTreeNode node)
        {
            //  validate
            if ((node == null) || node.IsEndpoint)
                return;

            //  drill down to unison outputs
            foreach (var childnode in node.ChildNodes)
            {
                //  if child has unison outputs
                if (ComponentTreeNode.IsNodeTimerDictionaryOutputArray(childnode))
                    EnumerateUnisonChildren(childnode);
                else
                    EnumerateUnisonEndpoints(childnode);
            }
        }

        /// <summary>
        /// Enumerate the children of a UnisonOutputNode
        /// </summary>
        /// <param name="node">The recursion node.</param>
        private static void EnumerateUnisonChildren(ComponentTreeNode node)
        {
            //  validate
            if ((node == null) || node.IsEndpoint)
                return;

            int id = 0;
            foreach (var childnode in node.ChildNodes)
            {
                //  if node is endpoint
                if (childnode.IsEndpoint)
                    childnode.Id = id++;
                else
                    EnumerateUnisonChildren(childnode);
            }
        }


    }

}
