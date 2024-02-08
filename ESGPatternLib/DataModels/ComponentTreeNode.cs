using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ESG.ExpressionLib.DataModels
{
    /// <summary>
    /// This is the base class for product components.  
    /// Examples include button, light, light engine, siren, wire input, and wire output.
    /// </summary>
    [JsonObject("Node")]
    public abstract class ComponentTreeNode : IComparable
    {
        /// <summary>
        /// The parent node.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public ComponentTreeNode ParentNode { get; set; }

        /// <summary>
        /// A value associated with the node.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public int Value { get; set; }

        /// <summary>
        /// True if the node has no children.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsEndpoint { get => ((ChildNodes == null) || (ChildNodes.Count == 0));  }

        /// <summary>
        /// The node name.
        /// </summary>
        [XmlAttribute("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }


        #region Id
        /// <summary>
        /// The node ID.
        /// </summary>
        [XmlAttribute("Id")]
        [JsonProperty("Id")]
        public int Id
        {
            get => GetId();
            set => SetId(value);
        }
        private int _id;

        public virtual bool ShouldSerializeId()
        {
            return !(IsNodeTimerDictionaryOutputArray(ParentNode));
        }
        protected virtual int GetId()
        {
            //  if light engine type, location backing field is ID
            if (IsNodeTimerDictionaryOutputArray(ParentNode))
                return ((Location.X & 0x03ff) | (((int)(Location.Angle / 22.5f)) << 10) | (Location.Z << 14));
            return _id;
        }
        protected virtual void SetId(int id)
        {
            //  if light engine type, location backing field is ID
            if (IsNodeTimerDictionaryOutputArray(ParentNode))
            {
                Location = new Location()
                {
                    X = ((id & 0x0200) == 0) ? (id & 0x01ff) : (id & 0x01ff) - 0x0200,
                    Z = ((id >> 14) & 0x03),
                    Angle = (((id >> 10) & 0x0f) * 22.5f),
                    SerializeY = false
                };
            }
            _id = id;
        }
        #endregion

        #region Location
        /// <summary>
        /// The node location.
        /// </summary>
        [JsonProperty("Location")]
        public Location Location { get; set; }

        /// <summary>
        /// Location serializer control method.  If location is null, it will not be serialized.
        /// </summary>
        /// <returns>Returns a value indicating whether the location should be serialized.</returns>
        public virtual bool ShouldSerializeLocation()
        {
            return true;
        }
        #endregion

        #region Child nodes
        /// <summary>
        /// The child nodes.
        /// </summary>
        [XmlArrayItem("Input", typeof(InputNode))]
        [XmlArrayItem("InputArray", typeof(InputArrayNode))]
        [XmlArrayItem("Output", typeof(OutputNode))]
        [XmlArrayItem("OutputArray", typeof(OutputArrayNode))]
        [XmlArrayItem("ColorOutput", typeof(OutputColorNode))]
        [XmlArrayItem("ProductAssembly", typeof(ProductAssemblyNode))]
        [XmlArrayItem("Component", typeof(ComponentTreeNode))]
        [XmlArray("Components")]
        [JsonProperty("With")]
        public ComponentTreeNodeCollection ChildNodes { get; set; }

        /// <summary>
        /// Serializer control method.
        /// </summary>
        /// <returns>Returns a value indicating whether the child nodes should be serialized.</returns>
        public virtual bool ShouldSerializeChildNodes()
        {
            return ((ChildNodes != null) && (ChildNodes.Count > 0));
        }
        #endregion


        /// <summary>
        /// Constructor.
        /// </summary>
        public ComponentTreeNode()
        {
            ChildNodes = new ComponentTreeNodeCollection(this);
        }

        /// <summary>
        /// Returns a deep copy of this node.
        /// </summary>
        /// <returns>Returns a deep copy of the product assembly.</returns>
        public T Copy<T>()
        {
            try
            {
                //Type t = this.GetType();
                string json = JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings() { Converters = { new JsonTreeConverter() } });
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Compares two node paths to determine if matching IDs of the parent, parent's parent, and so on up the tree.
        /// </summary>
        /// <param name="otherNode"></param>
        /// <returns>True if matching IDs of the parent, parent's parent, and so on up the tree.</returns>
        public bool IsSameNodePath(ComponentTreeNode otherNode)
        {
            return PathValue.NodePath(this).Equals(PathValue.NodePath(otherNode));
        }

        /// <summary>
        /// Compares the Id of one ComponentTreeNode to another.
        /// </summary>
        /// <param name="obj">The other ComponentTreeNode.</param>
        /// <returns>An int value representing the comparison of the two node Ids.</returns>
        public virtual int CompareTo(object obj)
        {
            //  if given object is null, return 1
            if (obj == null) return 1;

            //  if given object is tree node, compare else throw an exception
            if (obj is ComponentTreeNode treeNode)
                return Id.CompareTo(treeNode.Id);
            throw new ArgumentException("ComponentTreeNode not compared to ComponentTreeNode");
        }

        /// <summary>
        /// Gets the child node that has the given Id.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <returns>The child node that has the given Id.</returns>
        public ComponentTreeNode GetChildWithId(int id)
        {
            if (ChildNodes == null)
                return null;
            foreach (var childNode in ChildNodes)
                if (childNode.Id == id)
                    return childNode;
            return null;
        }

        /// <summary>
        /// Returns an Id string for this node.
        /// </summary>
        /// <returns>Returns an Id string for this node.</returns>
        public string GetIdString()
        {
            return PathValue.NodeIdString(this);
        }

        /// <summary>
        /// Returns the endpoint portion of this node's path, unless it is part of a unison output,
        /// in which case the path will start with the unison output.
        /// </summary>
        /// <returns>Returns the endpoint portion of this node's path, unless it is part of a unison output,
        /// in which case the path will start with the unison output.</returns>
        public string UniqueOutputPath()
        {
            return PathValue.UniqueOutputPath(this);
        }

        /// <summary>
        /// Returns a PathValue object that represents this node's value and unique output path.
        /// </summary>
        /// <returns>Returns a PathValue object that represents this node's value and unique output path.</returns>
        public PathValue GetUniqueOutputPathValue()
        {
            return new PathValue(UniqueOutputPath(), Value);
        }

        /// <summary>
        /// Finds the first node in tree that matches the given node type.
        /// </summary>
        /// <param name="node">The recursion node.</param>
        /// <param name="type">The node type to find.</param>
        public static ComponentTreeNode FirstType(ComponentTreeNode node, Type type)
        {
            ComponentTreeNode result = null;

            //  validate
            if (node == null)
                return null;

            //  check node
            if (node.GetType() == type)
                result = node;
            else if (!node.IsEndpoint)
            {
                foreach (var childnode in node.ChildNodes)
                {
                    result = FirstType(childnode, type);
                    if (result != null)
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Finds the first node in tree that is an sequenced output array.
        /// </summary>
        /// <param name="node">The recursion node.</param>
        /// <param name="outputType">The node output type to find.</param>
        public static ComponentTreeNode FirstSequencedOutputArray(ComponentTreeNode node)
        {
            ComponentTreeNode result = null;

            //  validate
            if (node == null)
                return null;

            //  check node
            if ((node is OutputArrayNode arrayNode) && (arrayNode.OutputArrayType != null) && (arrayNode.OutputArrayType.StartsWith("S")))
                result = node;
            else if (!node.IsEndpoint)
            {
                foreach (var childnode in node.ChildNodes)
                {
                    result = FirstSequencedOutputArray(childnode);
                    if (result != null)
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns true if the given node is an OutputArrayNode with timer dictionary outputs.
        /// </summary>
        /// <param name="node">The node to evaluate.</param>
        /// <returns>Returns true if the given node is an OutputArrayNode with timer dictionary outputs.</returns>
        public static bool IsNodeTimerDictionaryOutputArray(ComponentTreeNode node)
        {
            return ((node is OutputArrayNode arrayNode)
                && (arrayNode.OutputArrayType == OutputArrayNode.OutputTypeSequencedTimerDictionary));
        }


        /// <summary>
        /// Finds all nodes in tree that matches the given node type.
        /// </summary>
        /// <param name="node">The recursion node.</param>
        /// <param name="nodes">The list of nodes that match type T.</param>
        public static void AllNodesOfType<T>(ComponentTreeNode node, List<ComponentTreeNode> nodes)
        {
            //  validate
            if (node == null)
                return;

            //  check node
            if (node is T)
                nodes.Add(node);
            else if (!node.IsEndpoint)
            {
                foreach (var childnode in node.ChildNodes)
                    AllNodesOfType<T>(childnode, nodes);
            }
        }


    }

}

