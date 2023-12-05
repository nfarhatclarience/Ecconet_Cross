/**
  ******************************************************************************
  * @file    	LibConfig.cs
  * @copyright  © 2017 ECCO Group.  All rights reserved.
  * @author  	M. Latham, Liquid Logic, LLC
  * @version 	1.0.0
  * @date    	April 2017
  * @brief   	Matrix library configuration.
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
    public static class LibConfig
    {
        //	The industry-standard CAN frame maximum number of data bytes.
        public const UInt32 CAN_FRAME_MAX_NUM_BYTES = 8;

        //	The industry-standard CAN frame maximum number of devices.
        public const UInt32 CAN_FRAME_MAX_NUM_DEVICES = 120;

        //  The flash drive file name length.
        public const UInt32 FLASH_DRIVE_FILE_NAME_LENGTH = 12;

        //	The number of repeats in token compression.
        public const UInt32 MATRIX_MESSAGE_MAX_TOKEN_REPEATS = 32;

        //	The portion of the CAN identifier that indicates the message body frames and last frame.
        //	Choosing these allows coexistence with HazCAN.

        //  optimized
        public const UInt32 MATRIX_MESSAGE_FRAME_TYPE_SINGLE = 0x1C;
        public const UInt32 MATRIX_MESSAGE_FRAME_TYPE_BODY = 0x1D;
        public const UInt32 MATRIX_MESSAGE_FRAME_TYPE_LAST = 0x1E;

        //	CAN Identifier Bit Widths
        public const UInt32 MATRIX_CAN_ID_FRAME_INDEX_BIT_WIDTH = 5;
        public const UInt32 MATRIX_CAN_ID_FRAME_TYPE_BIT_WIDTH = 5;
        public const UInt32 MATRIX_CAN_ID_EVENT_FLAG_BIT_WIDTH = 1;
        public const UInt32 MATRIX_CAN_ID_ADDRESS_BIT_WIDTH = 7;

        //	CAN Identifier Shifts
        public const Int32 MATRIX_CAN_ID_FRAME_INDEX_SHIFT = 0;
        public const Int32 MATRIX_CAN_ID_DEST_ADDRESS_SHIFT = 5;
        public const Int32 MATRIX_CAN_ID_EVENT_FLAG_SHIFT = 12;
        public const Int32 MATRIX_CAN_ID_SOURCE_ADDRESS_SHIFT = 17;
        public const Int32 MATRIX_CAN_ID_FRAME_TYPE_SHIFT = 24;

        //	CAN Identifier Masks
        public const UInt32 MATRIX_CAN_ID_FRAME_INDEX_MASK = 0x1F;
        public const UInt32 MATRIX_CAN_ID_FRAME_TYPE_MASK = 0x1F;
        public const UInt32 MATRIX_CAN_ID_EVENT_FLAG_MASK = 0x01;
        public const UInt32 MATRIX_CAN_ID_ADDRESS_MASK = 0x7F;


        // CAN bus addresses 0-127
        public const UInt32 MATRIX_CAN_BROADCAST_ADDRESS = 0;                // broadcast address
        public const UInt32 MATRIX_CAN_MIN_STANDARD_ADDRESS = 1;             // self-assigned range is 1-120
        public const UInt32 MATRIX_CAN_MAX_STANDARD_ADDRESS = 120;           //  
        public const UInt32 MATRIX_CAN_MIN_RESERVED_ADDRESS = 121;           // reserved range is 121-127
        public const UInt32 MATRIX_CAN_MAX_RESERVED_ADDRESS = 127;           //  
        public const UInt32 MATRIX_VEHICLE_BUS_ADDRESS = 121;                // reserved: vehicle bus gateway 
        public const UInt32 MATRIX_PC_ADDRESS = 126;                         // reserved: PC USB-CAN

        // device internal address 128-255
        public const UInt32 MATRIX_EQUATION_PROCESSOR_NETWORK_ADDRESS = 132; // reserved: equation processor
        public const UInt32 MATRIX_TOKEN_SEQUENCER_0_NETWORK_ADDRESS = 133;  // reserved: sequencers 1-6 (0-5)
        public const UInt32 MATRIX_TOKEN_SEQUENCER_1_NETWORK_ADDRESS = 134;
        public const UInt32 MATRIX_TOKEN_SEQUENCER_2_NETWORK_ADDRESS = 135;
        public const UInt32 MATRIX_TOKEN_SEQUENCER_3_NETWORK_ADDRESS = 136;
        public const UInt32 MATRIX_TOKEN_SEQUENCER_4_NETWORK_ADDRESS = 137;
        public const UInt32 MATRIX_TOKEN_SEQUENCER_5_NETWORK_ADDRESS = 138;

        //  The CAN frame expiration period
        //  10,000 ticks per millisecond
        public const Int64 CAN_FRAME_EXPIRATION_PERIOD_TICKS = 500 * 10000;

        //	Cashe up to 1024 frames in message stream.
        public const UInt32 MR_STREAM_BUFFER_FRONT_SIZE = 1024;

        //	Circular queue of 1024 frames for ansynchronous CAN callbacks.
        public const UInt32 MR_STREAM_BUFFER_BACK_SIZE = 1024;

        //	File crc.
        public const UInt32 MATRIX_MESSAGE_CRC_INIT_VALUE = 0;
        public const UInt32 MATRIX_MESSAGE_CRC_POLY_VALUE = 0xA001;

        //	FTP params
        public const UInt32 MATRIX_MAX_FILE_NAME_LENGTH = 12;
        public const UInt32 MATRIX_MAX_FILE_SEGMENT_LENGTH = 256;
        public const Int32  MATRIX_MAX_FILE_SEGMENT_LENGTH_SHIFT = 8;
        public const UInt32 MATRIX_SERVER_ACCESS_POLY = 0x5EB9417D;

    }
}
