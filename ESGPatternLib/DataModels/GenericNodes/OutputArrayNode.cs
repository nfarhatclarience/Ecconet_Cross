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
    /// An output node array class.  Examples include a set of single-wire outputs or an array of light heads.
    /// </summary>
    [XmlRoot("OutputArray")]
    [JsonObject("OutputArray")]
    public class OutputArrayNode : ComponentTreeNode
    {
        /// <summary>
        /// Standard non-sequenced outputs.
        /// </summary>
        public const string OutputTypeStandard = "NS";

        /// <summary>
        /// Sequenced outputs that have token per output.
        /// </summary>
        public const string OutputTypeSequenced = "S0";

        /// <summary>
        /// Sequenced outputs that have 4-byte dictionary entries.
        /// </summary>
        public const string OutputTypeSequencedDictionary = "S4";

        /// <summary>
        /// Sequenced outputs that have 6-byte timer dictionary entries.
        /// </summary>
        public const string OutputTypeSequencedTimerDictionary = "S6";


        /// <summary>
        /// Indicates what type of outputs are in the array.
        /// </summary>
        [XmlAttribute("Type")]
        [JsonProperty("OutputArrayType")]
        public string OutputArrayType { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public OutputArrayNode()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public OutputArrayNode(string name, ComponentTreeNode[] children)
        {
            Name = name;
            ChildNodes.AddRange(children);
        }


        #region Type S6 consolidation and expansion
        /// <summary>
        /// Consolidates the output array node children if they are assemblies, with each unique assembly have a locations list.
        /// </summary>
        public void ConsolidateOutputAssemblies()
        {
            //  only consolidate OutputTypeSequencedTimerDictionary
            if (this.OutputArrayType != OutputTypeSequencedTimerDictionary)
                return;

            //  for all product assemblies in output array
            for (int i = 0; i < ChildNodes.Count; ++i)
            {
                if (ChildNodes[i] is ProductAssemblyNode assy)
                {
                    ProductAssemblyNode matchingAssy = null;
                    for (int n = 0; n < i; ++n)
                    {
                        if (ChildNodes[n] is ProductAssemblyNode assy1)
                        {
                            if (AreNodesEqualIgnoringLocation(assy, assy1))
                            {
                                matchingAssy = assy1;
                                break;
                            }
                        }
                    }

                    //  if match not found
                    if (matchingAssy == null)
                    {
                        assy.Ids = new List<ProductAssemblyNode.IdPair>();
                        assy.Ids.Add(new ProductAssemblyNode.IdPair() { AltId = assy.AltId, Id = assy.Id });
                        assy.Location = null;
                    }
                    else
                    {
                        matchingAssy.Ids.Add(new ProductAssemblyNode.IdPair() { AltId = assy.AltId, Id = assy.Id });
                        ChildNodes.Remove(assy);
                        --i;
                    }
                }

            }
        }


        /// <summary>
        /// Expands the output array node children if they are assemblies, converting the Id lists into individual assemblies.
        /// </summary>
        public void ExpandOutputAssemblies()
        {
            //  only consolidate OutputTypeSequencedTimerDictionary
            if (this.OutputArrayType != OutputTypeSequencedTimerDictionary)
                return;

            //  for all product assemblies in output array
            for (int i = 0; i < ChildNodes.Count; ++i)
            {
                if ((ChildNodes[i] is ProductAssemblyNode assy) && (assy.Ids != null))
                {
                    for (int n = 0; n < assy.Ids.Count; ++n)
                    {
                        var idPair = assy.Ids[n];
                        if (n == 0)
                        {
                            assy.Id = idPair.Id;
                            assy.AltId = idPair.AltId;
                        }
                        else
                        {
                            //  copy assembly, nullify Ids and copy the Id pair, and add to output array
                            var newAssy = assy.Copy<ProductAssemblyNode>();
                            ChildNodes.Add(newAssy);
                            newAssy.Ids = null;
                            newAssy.Id = idPair.Id;
                            newAssy.AltId = idPair.AltId;
                        }
                    }

                    //  nullify assy Ids
                    assy.Ids = null;
                }
            }
        }

        /// <summary>
        /// Returns a value indicating whether the two assemblies are equal, ignoring locations.
        /// </summary>
        /// <param name="a">The A node.</param>
        /// <param name="b">The B node.</param>
        /// <returns>A value indicating whether the two assemblies are equal, ignoring location.</returns>
        private bool AreNodesEqualIgnoringLocation(ProductAssemblyNode a, ProductAssemblyNode b)
        {
            try
            {
                var asmA = a.Copy<ProductAssemblyNode>();
                asmA.Location = null;
                asmA.Ids = null;
                asmA.Id = 0;
                asmA.AltId = 0;
                string jsonA = JsonConvert.SerializeObject(asmA, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                var asmB = b.Copy<ProductAssemblyNode>();
                asmB.Location = null;
                asmB.Ids = null;
                asmB.Id = 0;
                asmB.AltId = 0;
                string jsonB = JsonConvert.SerializeObject(asmB, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                //  return
                return (jsonA == jsonB);
            }
            catch
            {
                return false;
            }
        }
        #endregion

    }

}
