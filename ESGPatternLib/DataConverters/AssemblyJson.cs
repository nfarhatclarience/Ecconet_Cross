using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ESG.ExpressionLib.DataModels;


namespace ESG.ExpressionLib.DataConverters
{

    public static partial class ProductAssemblyConverters
    {
        /// <summary>
        /// Deserializes an assembly from a file, automatically detecting JSON or XML.
        /// </summary>
        /// <param name="pathName">The file path containing the json.</param>
        /// <returns>A product assembly, or null.</returns>
        public static ProductAssemblyNode FromFile(string pathName)
        {
            //  get the file type
            bool isXml = false;
            try
            {
                //  read product assembly
                if (File.ReadAllText(pathName).Contains("XMLSchema"))
                    isXml = true;
            }
            catch
            {
                throw new Exception("Unable to read product assembly file");
            }

            //  convert
            if (isXml)
                return FromXmlFile(pathName);
            else
                return FromJsonFile(pathName);
        }


        /// <summary>
        /// JSON-serializes an assembly to a file.
        /// </summary>
        /// <param name="assembly">The assembly to serialize.</param>
        /// <returns>An assembly as a JSON string, or an empty string on error.</returns>
        public static string ToJsonString(ProductAssemblyNode assembly)
        {
            string jsonString = string.Empty;
            try
            {
                //  get copy of assembly including the alternate IDs
                ProductAssemblyNode.SerializeAltId = true;
                var asm1 = assembly.Copy<ProductAssemblyNode>();
                ProductAssemblyNode.SerializeAltId = false;

                //  clear Id lists
                List<ComponentTreeNode> nodes = new List<ComponentTreeNode>();
                ComponentTreeNode.AllNodesOfType<ProductAssemblyNode>(asm1, nodes);
                foreach (var node in nodes)
                {
                    if (node is ProductAssemblyNode asm)
                        asm.Ids = null;
                }

                //  consolidate assemblies in type S6 output arrays
                nodes.Clear();
                ComponentTreeNode.AllNodesOfType<OutputArrayNode>(asm1, nodes);
                foreach (var node in nodes)
                {
                    if ((node is OutputArrayNode outputArrayNode) &&
                        (outputArrayNode.OutputArrayType == OutputArrayNode.OutputTypeSequencedTimerDictionary))
                    {
                        outputArrayNode.ConsolidateOutputAssemblies();
                    }
                }

                //  output assembly to file
                jsonString = JsonConvert.SerializeObject(asm1,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new Exception("Error converting product assembly to JSON string.");
            }
            return jsonString;
        }

        /// <summary>
        /// JSON-serializes an assembly to a file.
        /// </summary>
        /// <param name="assembly">The assembly to serialize.</param>
        /// <param name="pathName">The file path and name.</param>
        public static void ToJsonFile(ProductAssemblyNode assembly, string pathName)
        {
            try
            {
                File.WriteAllText(pathName, ToJsonString(assembly));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new Exception("Error writing product assembly JSON file.");
            }
        }

        /// <summary>
        /// JSON-deserializes an assembly from a string.
        /// </summary>
        /// <param name="pathName">The file path containing the json.</param>
        /// <returns>A product assembly, or null.</returns>
        public static ProductAssemblyNode FromJsonString(string json)
        {
            ProductAssemblyNode assembly = null;
            try
            {
                //  convert file text to assembly
                assembly = JsonConvert.DeserializeObject<ProductAssemblyNode>(json,
                    new JsonSerializerSettings()
                    {
                        Converters = { new JsonTreeConverter() }
                    });

                //  expand any condensed S6 output arrays back to full tree
                List<ComponentTreeNode> nodes = new List<ComponentTreeNode>();
                ComponentTreeNode.AllNodesOfType<OutputArrayNode>(assembly, nodes);
                foreach (var node in nodes)
                {
                    if ((node is OutputArrayNode outputArrayNode) &&
                        (outputArrayNode.OutputArrayType == OutputArrayNode.OutputTypeSequencedTimerDictionary))
                    {
                        outputArrayNode.ExpandOutputAssemblies();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new Exception("Error convertin JSON string to product assembly.");
            }
            return assembly;
        }

        /// <summary>
        /// JSON-deserializes an assembly from a file.
        /// </summary>
        /// <param name="pathName">The file path containing the json.</param>
        /// <returns>A product assembly, or null.</returns>
        public static ProductAssemblyNode FromJsonFile(string pathName)
        {
            ProductAssemblyNode assembly = null;
            try
            {
                assembly = FromJsonString(File.ReadAllText(pathName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new Exception("Error reading product assembly JSON file.");
            }
            return assembly;
        }
    }
}
