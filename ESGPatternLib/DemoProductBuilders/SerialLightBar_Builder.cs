using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESG.ExpressionLib.DataModels;


namespace ESG.ExpressionLib.Products
{
    /// <summary>
    /// Static Serial Light Bar builder.  Only used to generate demo XML file.
    /// </summary>
    public static partial class Models
    {
        /// <summary>
        /// Builds a default Serial Light Bar with 16 light engines with one color with one light each.
        /// </summary>
        public static ProductAssemblyNode BuildSerialLightBar()
        {
            //  create the output node array
            OutputArrayNode lightEngines = new OutputArrayNode("Light Engines", new ProductAssemblyNode[]
            {
                new ProductAssemblyNode("Torus",
                    new Location() { X = -5, Z = 0, Angle = 270 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Red", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
                new ProductAssemblyNode("Torus",
                    new Location() { X = -4, Z = 0, Angle = 315 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Red", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
                new ProductAssemblyNode("Torus",
                    new Location() { X = -3, Z = 0, Angle = 0 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Red", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
                new ProductAssemblyNode("Torus",
                    new Location() { X = -2, Z = 0, Angle = 0 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Red", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
                new ProductAssemblyNode("Torus",
                    new Location() { X = 0, Z = 0, Angle = 0 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Red", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
                new ProductAssemblyNode("Torus",
                    new Location() { X = 2, Z = 0, Angle = 0 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Blue", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
                new ProductAssemblyNode("Torus",
                    new Location() { X = 3, Z = 0, Angle = 0 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Blue", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
                new ProductAssemblyNode("Torus",
                    new Location() { X = 4, Z = 0, Angle = 45 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Blue", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
                new ProductAssemblyNode("Torus",
                    new Location() { X = 5, Z = 0, Angle = 90 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Blue", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
                new ProductAssemblyNode("Torus",
                    new Location() { X = 4, Z = 0, Angle = 135 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Red", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
                new ProductAssemblyNode("Torus",
                    new Location() { X = 3, Z = 0, Angle = 180 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Amber", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
                new ProductAssemblyNode("Torus",
                    new Location() { X = 2, Z = 0, Angle = 180 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Amber", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
                new ProductAssemblyNode("Torus",
                    new Location() { X = 0, Z = 0, Angle = 180 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Amber", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
                new ProductAssemblyNode("Torus",
                    new Location() { X = -2, Z = 0, Angle = 180 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Amber", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
                new ProductAssemblyNode("Torus",
                    new Location() { X = -3, Z = 0, Angle = 180 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Amber", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
                new ProductAssemblyNode("Torus",
                    new Location() { X = -4, Z = 0, Angle = 225 },
                    new ComponentTreeNode[] { new OutputColorNode(0, "Blue", new ComponentTreeNode[] { new OutputNode() { Id = 0, Value = 100 } } ) } ) { AltId = -1 },
            })
            { OutputArrayType = OutputArrayNode.OutputTypeSequencedTimerDictionary };

            //  build and return the light bar
            return new ProductAssemblyNode("SerialLightBar", new ComponentTreeNode[] { lightEngines });
        }
    }
}
