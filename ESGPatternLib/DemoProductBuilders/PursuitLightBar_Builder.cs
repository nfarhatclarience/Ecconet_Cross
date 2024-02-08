using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using ESG.ExpressionLib.DataModels;



namespace ESG.ExpressionLib.Products
{
    /// <summary>
    /// Static Pursuit light bar builder.  Only used to generate demo XML file.
    /// </summary>
    public static partial class Models
    {
        /// <summary>
        /// Builds a default Serial Light Bar with 16 light engines with one color with one light each.
        /// </summary>
        public static ProductAssemblyNode BuildPursuitLightBar()
        {
            //  create the output node array
            OutputArrayNode lightEngines = new OutputArrayNode("LED Outputs", new OutputNode[]
            {
                //  upper tier
                new OutputColorNode() { Id = 532, Color = "Red",   Location = new Location() { X = -5, Z = 1, Angle = 270 } },
                new OutputColorNode() { Id = 531, Color = "Red",   Location = new Location() { X = -4, Z = 1, Angle = 315 } },
                new OutputColorNode() { Id = 530, Color = "Red",   Location = new Location() { X = -3, Z = 1, Angle = 0 } },
                new OutputColorNode() { Id = 529, Color = "Red",   Location = new Location() { X = -2, Z = 1, Angle = 0 } },
                new OutputColorNode() { Id = 501, Color = "White", Location = new Location() { X = 0,  Z = 1, Angle = 0 } },
                new OutputColorNode() { Id = 520, Color = "Blue",  Location = new Location() { X = 2,  Z = 1, Angle = 0 } },
                new OutputColorNode() { Id = 519, Color = "Blue",  Location = new Location() { X = 3,  Z = 1, Angle = 0 } },
                new OutputColorNode() { Id = 518, Color = "Blue",  Location = new Location() { X = 4,  Z = 1, Angle = 45 } },
                new OutputColorNode() { Id = 517, Color = "Blue",  Location = new Location() { X = 5,  Z = 1, Angle = 90 } },
                new OutputColorNode() { Id = 502, Color = "Blue",  Location = new Location() { X = 4,  Z = 1, Angle = 135 } },
                new OutputColorNode() { Id = 503, Color = "Blue",  Location = new Location() { X = 3,  Z = 1, Angle = 180 } },
                new OutputColorNode() { Id = 504, Color = "Blue",  Location = new Location() { X = 2,  Z = 1, Angle = 180 } },
                new OutputColorNode() { Id = 516, Color = "White", Location = new Location() { X = 0,  Z = 1, Angle = 180 } },
                new OutputColorNode() { Id = 513, Color = "Red",   Location = new Location() { X = -2, Z = 1, Angle = 180 } },
                new OutputColorNode() { Id = 514, Color = "Red",   Location = new Location() { X = -3, Z = 1, Angle = 180 } },
                new OutputColorNode() { Id = 515, Color = "Red",   Location = new Location() { X = -4, Z = 1, Angle = 225 } },


                //  lower tier
                new OutputColorNode() { Id = 564, Color = "White", Location = new Location() { X = -5, Z = 0, Angle = 270 } },
                new OutputColorNode() { Id = 563, Color = "White", Location = new Location() { X = -4, Z = 0, Angle = 315 } },
                new OutputColorNode() { Id = 562, Color = "White", Location = new Location() { X = -3, Z = 0, Angle = 0 } },
                new OutputColorNode() { Id = 561, Color = "White", Location = new Location() { X = -2, Z = 0, Angle = 0 } },
                new OutputColorNode() { Id = 560, Color = "White", Location = new Location() { X = -1, Z = 0, Angle = 0 } },
                new OutputColorNode() { Id = 553, Color = "White", Location = new Location() { X = 1,  Z = 0, Angle = 0 } },
                new OutputColorNode() { Id = 552, Color = "White", Location = new Location() { X = 2,  Z = 0, Angle = 0 } },
                new OutputColorNode() { Id = 551, Color = "White", Location = new Location() { X = 3,  Z = 0, Angle = 0 } },
                new OutputColorNode() { Id = 550, Color = "White", Location = new Location() { X = 4,  Z = 0, Angle = 45 } },
                new OutputColorNode() { Id = 549, Color = "White", Location = new Location() { X = 5,  Z = 0, Angle = 90 } },
                new OutputColorNode() { Id = 534, Color = "White", Location = new Location() { X = 4,  Z = 0, Angle = 135 } },
                new OutputColorNode() { Id = 535, Color = "Amber", Location = new Location() { X = 3,  Z = 0, Angle = 180 } },
                new OutputColorNode() { Id = 536, Color = "Amber", Location = new Location() { X = 2,  Z = 0, Angle = 180 } },
                new OutputColorNode() { Id = 537, Color = "Amber", Location = new Location() { X = 1,  Z = 0, Angle = 180 } },
                new OutputColorNode() { Id = 544, Color = "Amber", Location = new Location() { X = -1, Z = 0, Angle = 180 } },
                new OutputColorNode() { Id = 545, Color = "Amber", Location = new Location() { X = -2, Z = 0, Angle = 180 } },
                new OutputColorNode() { Id = 546, Color = "Amber", Location = new Location() { X = -3, Z = 0, Angle = 180 } },
                new OutputColorNode() { Id = 547, Color = "White", Location = new Location() { X = -4, Z = 0, Angle = 225 } },
            });
            lightEngines.OutputArrayType = OutputArrayNode.OutputTypeSequencedDictionary;

            //  build and return the light bar
            return new ProductAssemblyNode("PursuitLightBar", new ComponentTreeNode[] { lightEngines });
        }
    }
}
