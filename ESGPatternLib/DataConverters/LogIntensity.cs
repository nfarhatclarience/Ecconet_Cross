using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESG.ExpressionLib.DataConverters
{
    public static class LogIntensity
    {
        /// <summary>
        /// The number of intensity bits to convert to/from log.
        /// </summary>
        public enum IntensityBits
        {
            Bits2,
            Bits3
        }

        /// <summary>
        /// Intensity map for 2 bits.
        /// </summary>
        static readonly byte[] IntensityMap2 = new byte[4] { 0, 30, 60, 100 };

        /// <summary>
        /// Intensity map for 3 bits.
        /// </summary>
        static readonly byte[] IntensityMap3 = new byte[8] { 0, 10, 20, 30, 40, 60, 80, 100 };

        /// <summary>
        /// Convert intensity to index.
        /// </summary>
        /// <param name="intensity">The intensity, 0 to 100.</param>
        /// <param name="bits">The intensity index size in bits.</param>
        /// <returns>Intensity converted to index.</returns>
        public static byte IntensityToIndex(byte intensity, IntensityBits bits)
        {
            byte index = 0;
            switch (bits)
            {
                case IntensityBits.Bits2:
                    for (; index < (IntensityMap2.Length - 1); ++index)
                        if (intensity < IntensityMap2[index + 1])
                            break;
                    break;

                case IntensityBits.Bits3:
                    for (; index < (IntensityMap3.Length - 1); ++index)
                        if (intensity < IntensityMap3[index + 1])
                            break;
                    break;

                default:
                    break;
            }
            return index;
        }

        /// <summary>
        /// Convert index to intensity.
        /// </summary>
        /// <param name="index">The intensity index.</param>
        /// <param name="bits">The intensity index size in bits.</param>
        /// <returns>Index converted to intensity.</returns>
        public static byte IndexToIntensity(byte index, IntensityBits bits)
        {
            switch (bits)
            {
                case IntensityBits.Bits2:
                    if (index >= IntensityMap2.Length)
                        index = (byte)(IntensityMap2.Length - 1);
                    return IntensityMap2[index];

                case IntensityBits.Bits3:
                    if (index >= IntensityMap3.Length)
                        index = (byte)(IntensityMap3.Length - 1);
                    return IntensityMap3[index];

                default:
                    break;
            }
            return 0;
        }


    }
}
