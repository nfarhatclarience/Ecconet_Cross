/**
  ******************************************************************************
  * @file    	Crc.cs
  * @copyright  © 2017 ECCO Group.  All rights reserved.
  * @author  	M. Latham, Liquid Logic, LLC
  * @version 	1.0.0
  * @date    	April 2017
  * @brief   	The crc calculation used in the Matrix messages.
  ******************************************************************************
  * @attention
  *
  * Unless required by applicable law or agreed to in writing, software created
  * by Liquid Logic, LLC is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES
  * OR CONDITIONS OF ANY KIND, either express or implied.
  *
  ******************************************************************************
  */

  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCONet
{
    public class Crc
    {

        /// <summary>
        /// Computes a Matrix message CRC.
        /// </summary>
        /// <param name="bytes">One or more data bytes.</param>
        /// <param name="numBytes">The number of bytes to analyze.</param>
        /// <returns>Returns the computed CRC.</returns>
        public static UInt16 ComputeCRC16(Byte[] bytes, UInt32 numBytes)
        {
            UInt16 crc;

            //	validate inputs
            if ((null == bytes) || (0 == numBytes))
                return 0;

            crc = (UInt16)LibConfig.MATRIX_MESSAGE_CRC_INIT_VALUE;
            for (int i = 0; i < numBytes; i++)
                crc = AddByteToCRC16(bytes[i], crc);
            return crc;
        }

        /// <summary>
        /// Adds a byte to a Matrix message CRC.
        /// </summary>
        /// <param name="byteVal">A byte to add to the CRC.</param>
        /// <param name="crc">A CRC accumulator.</param>
        /// <returns>The CRC accumulator with the byte added.</returns>
        public static UInt16 AddByteToCRC16(Byte byteVal, UInt16 crc)
        {
            for (int bit = 0; bit < 8; ++bit)
            {
                crc = (0 != ((byteVal ^ (Byte)crc) & 1)) ?
                    (UInt16)((crc >> 1) ^ (UInt16)LibConfig.MATRIX_MESSAGE_CRC_POLY_VALUE) : (UInt16)(crc >> 1);
                byteVal >>= 1;
            }
            return crc;
        }

        /// <summary>
        /// Determines if a Matrix message byte stream is valid.
        /// </summary>
        /// <param name="bytes">The message bytes.</param>
        /// <param name="numBytes"></param>
        /// <returns>A value that indicates whether the checksum is valid.</returns>
        public static bool IsMessageChecksumValid(Byte[] bytes, UInt32 numBytes)
        {
            UInt16 messageCrc;

            //	validate inputs
            if ((null == bytes) || (0 == numBytes))
                return false;

            /*
            //	one-frame messages do not include a checksum
            if (numBytes <= LibConfig.CAN_FRAME_MAX_NUM_BYTES)
                return true;
            */

            //	compare checksums
            messageCrc = bytes[numBytes - 2];
            messageCrc = (UInt16)((messageCrc << 8) | bytes[numBytes - 1]);
            return (messageCrc == ComputeCRC16(bytes, numBytes - 2));
        }

    }
}
