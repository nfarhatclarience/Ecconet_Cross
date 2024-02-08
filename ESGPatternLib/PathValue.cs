using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Serialization;
using Newtonsoft.Json;

using ESG.ExpressionLib.DataModels;


namespace ESG.ExpressionLib
{
    /// <summary>
    /// The path-value class represents an I/O component's assembly tree node path and it's input or output value.
    /// </summary>
    [XmlRoot("Output")]
    public class PathValue : IComparable
    {
        /// <summary>
        /// The I/O component's assembly tree node path.
        /// </summary>
        [XmlAttribute("Path")]
        [JsonProperty("Path")]
        public string Path { get; set; }

        /// <summary>
        /// The I/O component's value.
        /// </summary>
        [XmlAttribute("Value")]
        [JsonProperty("Value")]
        public int Value { get; set; }

        #region Root and endpoint
        /// <summary>
        /// The path root if available, else string.empty.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public string Root { get => GetRoot(Path); }

        /// <summary>
        /// The path root Id if available, else -1.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public int RootId { get => GetRootId(Path);  }

        /// <summary>
        /// The endpoint if available, else string empty.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public string Endpoint { get => GetEndpoint(Path); }

        /// <summary>
        /// The endpoint Id if available, else -1.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public int EndpointId { get => GetEndpointId(Path); }
        #endregion


        /// <summary>
        /// Constructor.
        /// </summary>
        public PathValue() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The I/O component's assembly tree node path.</param>
        /// <param name="value">The I/O component's value.</param>
        public PathValue(string path, int value)
        {
            Path = path;
            Value = value;
        }

        /// <summary>
        /// ToString() override returns path.
        /// </summary>
        /// <returns>Returns the path.</returns>
        public override string ToString()
        {
            return Path;
        }

        /// <summary>
        /// Returns a value indicating whether the paths are equal.
        /// </summary>
        /// <param name="pathValue">The path to which to compare.</param>
        /// <returns>Returns a value indicating whether the paths are equal.</returns>
        public bool PathEquals(PathValue pathValue)
        {
            return Path.Equals(pathValue.Path);
        }

        /// <summary>
        /// CompareTo() for PathValue sorting compares paths.
        /// </summary>
        /// <param name="obj">The object to which to compare.</param>
        /// <returns>Returns a sort value.</returns>
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            if (obj is PathValue pv)
                return Path.CompareTo(pv.Path);
            throw new Exception("Comparing to object that is not a PathValue");
        }

        /// <summary>
        /// Returns a value indicating whether this PathValue and the given PathValue
        /// have the same path and value.
        /// </summary>
        /// <param name="obj">The PathValue to which to compare.</param>
        /// <returns>Returns a value indicating whether this PathValue and the given PathValue
        /// have the same path and value.</returns>
        public override bool Equals(object obj)
        {
            if (obj is PathValue pv)
                return (Path.Equals(pv.Path) && Value.Equals(pv.Value));
            return false;
        }

        /// <summary>
        /// Returns the hash code.
        /// </summary>
        /// <returns>Returns the hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a deep copy of this PathValue.
        /// </summary>
        /// <returns>Returns a deep copy of this PathValue.</returns>
        public PathValue DeepCopy()
        {
            return new PathValue(string.Copy(Path), Value);
        }


        //  static methods

        #region Id string for node
        /// <summary>
        /// Returns the node Id string, substituting the color if node has color property and parent does not.
        /// </summary>
        /// <param name="node">The node from which to get the Id string.</param>
        /// <returns>Returns the local node Id string, substituting the color if node has color property and parent does not.</returns>
        public static string NodeIdString(ComponentTreeNode node)
        {
            /*
            //  validate inputs
            if (node == null)
                return string.Empty;
            if ((node.ParentNode != null) && (node.ParentNode.GetType().GetTypeInfo().GetProperty("Color") != null))
                return node.Id.ToString();
            else if (node.GetType().GetTypeInfo().GetProperty("Color") is PropertyInfo pi)
                //return node.Id.ToString();
                return (string)pi.GetValue(node);
            return node.Id.ToString();
            */

            //  validate inputs
            if (node == null)
                return string.Empty;
            if (node is OutputColorNode colorNode)
            {
                if ((colorNode.ChildNodes != null) && (colorNode.ChildNodes.Count > 0))
                    return colorNode.Color;
                else
                    return colorNode.Color + "-" + node.Id.ToString();
            }
            return node.Id.ToString();

        }
        #endregion

        #region Full path for node and node's endpoints
        /// <summary>
        /// Returns the node's path.
        /// </summary>
        /// <param name="node">The node from which to get the path.</param>
        /// <returns>Returns the node's path.</returns>
        public static string NodePath(ComponentTreeNode node)
        {
            //  validate inputs
            if (node == null)
                return string.Empty;

            string nodePath = string.Empty;
            string separator = "/";
            do
            {
                nodePath = separator + NodeIdString(node) + nodePath;
                node = node.ParentNode;

            } while (node != null);
            if (nodePath.StartsWith("/") && (nodePath.Length > 0))
                nodePath = nodePath.Substring(1);
            return nodePath;
        }

        /// <summary>
        /// Gets the paths for all endpoints of given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>The paths for all endpoints of given node.</returns>
        public static string[] EndpointFullPaths(ComponentTreeNode node)
        {
            //  validate
            if (node == null)
                return new string[0];

            //  if node is endpoint, return the node path
            if (node.IsEndpoint)
                return new string[] { NodePath(node) };

            //  for all node children, gather the node paths
            List<string> localPaths = new List<string>(100);
            foreach (var childNode in node.ChildNodes)
                localPaths.AddRange(EndpointFullPaths(childNode));
            return localPaths.ToArray();
        }
        #endregion

        #region Unique output path for node and node's endpoints 
        /// <summary>
        /// Returns the endpoint portion of the given node's path, unless it is part of a timer dictionary output array,
        /// in which case the path will start with the top-level output component.
        /// </summary>
        /// <param name="testNode">The node from which to get the path.</param>
        /// <returns>Returns the endpoint portion of the given node's path, unless it is part of a unison output,
        /// in which case the path will start with the unison output.</returns>
        public static string UniqueOutputPath(ComponentTreeNode node)
        {
            //  validate inputs
            if (node == null)
                return string.Empty;

            var testNode = node;
            bool unisonNodeFound = false;
            string localPath = string.Empty;
            do
            {
                if ((testNode.ParentNode is OutputArrayNode parent) && (parent.OutputArrayType == OutputArrayNode.OutputTypeSequencedTimerDictionary))
                {
                    unisonNodeFound = true;
                    localPath = NodeIdString(testNode) + localPath;
                    break;
                }

                //  add path
                localPath = "/" + NodeIdString(testNode) + localPath;
                testNode = testNode.ParentNode;

            } while (testNode != null);

            //  if unison parent node found, then return the path
            if (unisonNodeFound)
                return localPath;

            //  otherwise, return just the node's Id to string
            return NodeIdString(node);
        }

        /// <summary>
        /// For all endpoint nodes, returns the endpoint portion of the given node's path,
        /// unless it is part of a timer dictionary output array, in which case the path will start with the top-level output component.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>The unique output paths for all endpoints of given node.</returns>
        public static string[] UniqueOutputPaths(ComponentTreeNode node)
        {
            //  if node is endpoint, return the node path
            if (node.IsEndpoint)
                return new string[] { UniqueOutputPath(node) };

            //  for all node children, gather the node paths
            List<string> localPaths = new List<string>(100);
            foreach (var childNode in node.ChildNodes)
                localPaths.AddRange(UniqueOutputPaths(childNode));
            return localPaths.ToArray();
        }
        #endregion

        #region Root string and Root Id
        /// <summary>
        /// Returns the root for the given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Returns the root for the given path.</returns>
        public static string GetRoot(string path)
        {
            if ((path == null) || (path.Length == 0))
                return string.Empty;
            int index = path.IndexOf("/");
            if (index == -1)
                return path;
            return path.Substring(0, index);
        }

        /// <summary>
        /// Returns the root Id from the given path.
        /// Throws an exception if the path's root Id is not parsible.
        /// </summary>
        /// <param name="path">The path from which to get the root Id.</param>
        /// <returns>Returns the root Id from the given path.</returns>
        public static int GetRootId(string path)
        {
            if (int.TryParse(GetRoot(path), out int id))
                return id;
            return -1;

            /*
            try
            {
                //  start index
                //int startIndex = path.IndexOf('~') + 1;
                //if (startIndex == 0)
                //    startIndex = path.LastIndexOf('/') + 1;

                //  end index
                //int endIndex = path.IndexOf('/', startIndex);
                //if (endIndex == -1)
                //    endIndex = path.Length;

                //  return result
                //return int.Parse(path.Substring(startIndex, endIndex - startIndex));
                return int.Parse(GetRoot(path));
            }
            catch
            {
                throw new Exception("Error parsing root Id from path.");
            }
            */
        }
        #endregion

        #region Endpoint and EndpointId
        /// <summary>
        /// Returns the root for the given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Returns the root for the given path.</returns>
        public static string GetEndpoint(string path)
        {
            if ((path == null) || (path.Length == 0))
                return string.Empty;
            return path.Substring(path.LastIndexOf('/') + 1).Substring(path.LastIndexOf('-') + 1);
        }


        /// <summary>
        /// Returns the endpoint Id from the given path.
        /// Throws an exception if the path's endpoint Id is not parsible.
        /// </summary>
        /// <param name="path">The path from which to get the root Id.</param>
        /// <returns>Returns the root Id from the given path.</returns>
        public static int GetEndpointId(string path)
        {
            if (int.TryParse(GetEndpoint(path), out int id))
                return id;
            return -1;

            /*
            try
            {
                //  return result
                return int.Parse(GetEndpoint(path));
            }
            catch
            {
                throw new Exception("Error parsing endpoint Id from path.");
            }
            */
        }
        #endregion

    }
}
