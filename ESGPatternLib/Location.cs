using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ESG.ExpressionLib
{
    /// <summary>
    /// The Location ID class is used to provide physical location and orientation, and integer IDs for IO devices like light heads.
    /// The Y location is not used for light bars.
    /// </summary>
    [XmlType("LocationId")]
    public class Location
    {
        /// <summary>
        /// The X position in cm.
        /// </summary>
        [XmlAttribute("X")]
        [JsonProperty("X")]
        public int X
        {
            get => _x;
            set
            {
                _x = value;
                if (_x < -511)
                    _x = -511;
                if (_x > 511)
                    _x = 511;
            }
        }
        private int _x;

        #region Y is not used for light bars, but is reserved for other types such as programmable keypad button matrices
        /// <summary>
        /// The Y position in cm.  Not used for light bars.
        /// </summary>
        [XmlAttribute("Y")]
        [JsonProperty("Y")]
        public int Y
        {
            get => _y;
            set
            {
                _y = value;
                if (_y < -511)
                    _y = -511;
                if (_y > 511)
                    _y = 511;
            }
        }
        private int _y;

        /// <summary>
        /// Indicates whether to include Y in serialization.
        /// Lightbars will set this to false.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool SerializeY { get => _serializeY; set => _serializeY = value; }
        private bool _serializeY = true;

        /// <summary>
        /// Serializer control method.
        /// </summary>
        /// <returns>Returns a value indicating whether the Id should be serialized as a location.</returns>
        public bool ShouldSerializeY()
        {
            return SerializeY;
        }
        #endregion

        /// <summary>
        /// The z position in cm.  For light bars this is the tier.
        /// </summary>
        [XmlAttribute("Z")]
        [JsonProperty("Z")]
        public int Z
        {
            get => _z;
            set
            {
                _z = value;
                if (_z < -511)
                    _z = -511;
                if (_z > 511)
                    _z = 511;
            }
        }
        private int _z;

        /// <summary>
        /// The angle in degrees.  Front lights are 0 degreee, rear are 180.
        /// </summary>
        [XmlAttribute("Angle")]
        [JsonProperty("Angle")]
        public float Angle
        {
            get => _angle;
            set
            {
                _angle = value;
                while (_angle >= 360.0f)
                    _angle -= 360.0f;
            }
        }
        private float _angle;

        /*
        /// <summary>
        /// The ID in short form for light heads and light engines.
        /// </summary>
        [XmlIgnore]
        public UInt16 LightHeadId
        {
            get { return (UInt16)((UInt16)(X & 0x03ff) | (((uint)((float)Angle / 22.5)) << 10) | ((uint)Z << 14)); }
        }
        */

        /// <summary>
        /// Constructor.
        /// </summary>
        public Location() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Location(int x, int y, int z, int angle)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Angle = angle;
        }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns>The string representation of the location ID.</returns>
        public override string ToString()
        {
            return string.Format("T{0} X{1} A{2:0}", Z, X, Angle);
        }

        /*
        /// <summary>
        /// To light head ID string.
        /// </summary>
        /// <returns>The string representation of the location short ID.</returns>
        public string ToLightHeadIdString()
        {
            return "0x" + Id.ToString("X4");
        }
        */
    }
}
