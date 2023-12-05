/**
  ******************************************************************************
  * @file    	Transmitter.cs
  * @copyright  © 2017 ECCO Group.  All rights reserved.
  * @author  	M. Latham, Liquid Logic, LLC
  * @version 	1.0.0
  * @date    	April 2017
  * @brief   	Transmits Matrix messages over the CAN bus.
  *
  *				Buffering the compressed outgoing messages allows a look-ahead
  *				for the last message packet, and is efficient use of memory as
  *				compared to buffering raw tokens or whole CAN frames.
  *
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
using System.Threading;
using System.Diagnostics;



namespace ECCONet
{
    /// <summary>
    /// A transmitter.
    /// 
    /// NOTE:
    ///   Although would seem obvious for the .Net lib to just build messages in byte lists
    ///   and then send to the connected CAN transmitter, for the purpose of system debug,
    ///   the .Net code is as close to the device firmware as possible.
    ///   
    ///   In particular, the firmware transmitter builds messages in a way that uses a fixed
    ///   amount of microcontroller RAM.  This .Net implementation instances a transmitter
    ///   for each message, allowing multiple processes to create their own messages without
    ///   emitting interleaved CAN frames from the same node address.
    /// 
    /// </summary>
    internal sealed class Transmitter
    {
        //	a two-frame byte fifo for look-ahead function
        const UInt32 MATRIX_TRANSMITTER_FIFO_SIZE = (2 * LibConfig.CAN_FRAME_MAX_NUM_BYTES);

        //	message id-address field
        UInt32 idAddress;

        //	a two-frame byte fifo for look-ahead function
        Byte[] fifo;

        //	message bytes fifo index
        UInt32 fifoIndex;

        //	message sent bytes counter
        UInt16 numBytesSent;

        //	message bytes crc accumulator
        UInt16 crc;

        //	cyclic frame index
        static UInt16 frameIndex = 0;

        //  the core logic
        ECCONetCore core;

        /// <summary>
        /// A tx CAN frame for multi-thread buffering.
        /// </summary>
        class TxCanFrame
        {
            /// <summary>
            /// The CAN frame ID field.
            /// </summary>
            public UInt32 id;

            /// <summary>
            /// The CAN frame data.
            /// </summary>
            public byte[] data;
        }

        //  a list of CAN frames to transmit
        List<TxCanFrame> txFrames;

        //  a lock for sending the frames
        static object txLock = new object();


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="core">The core logic group.</param>
        public Transmitter(ECCONetCore core)
        {
            //  save core
            this.core = core;

            //  fifo
            fifo = new Byte[MATRIX_TRANSMITTER_FIFO_SIZE];

            //  the message to send
            txFrames = new List<TxCanFrame>();

            //	reset the frame index
            //frameIndex = 0;
        }

        /// <summary>
        /// Resets the transmitter and starts a new message.
        /// </summary>
        /// <param name="destinationAddress">The destination address, or zero for broadcast.</param>
        public void StartMessage(UInt16 destinationAddress)
        {
            StartMessageWithAddressAndKey(destinationAddress, Token.Keys.KeyNull);
        }


        /// <summary>
        /// Resets the transmitter for a new message for the given type.
        /// </summary>
        /// <param name="destinationAddress">The destination address, or zero for broadcast.</param>
        /// <param name="key">A token key used to determine the message type.  Use KeyNull for normal messages.</param>
        public void StartMessageWithAddressAndKey(UInt16 destinationAddress, Token.Keys key)
        {
            //	reset the fifo
            fifoIndex = 0;
            crc = 0;
            numBytesSent = 0;
            idAddress = 0;

            //	add the destination address
            idAddress |=
                ((UInt32)(destinationAddress & LibConfig.MATRIX_CAN_ID_ADDRESS_MASK)
                << LibConfig.MATRIX_CAN_ID_DEST_ADDRESS_SHIFT);

            //	add this device source address
            idAddress |=
                ((UInt32)(core.GetCanAddress() & LibConfig.MATRIX_CAN_ID_ADDRESS_MASK)
                << LibConfig.MATRIX_CAN_ID_SOURCE_ADDRESS_SHIFT);

            //	add the first frame id
            idAddress |= ((UInt32)LibConfig.MATRIX_MESSAGE_FRAME_TYPE_BODY
                << LibConfig.MATRIX_CAN_ID_FRAME_TYPE_SHIFT);

            //	if a command token
            if (Token.Key_IsCommand((ushort)key))
            {
                //	add event index zero
                AddByte(0);
            }
            //	else if an input our output event
            else if ((Token.KeyPrefix.InputStatus == Token.Key_GetPrefix(key))
                || (Token.KeyPrefix.OutputStatus == Token.Key_GetPrefix(key)))
            {
                //	set event flag and add the current event index
                idAddress |= (1 << LibConfig.MATRIX_CAN_ID_EVENT_FLAG_SHIFT);
                AddByte(core.eventIndex.GetEventIndex());
            }
            //
            //	else any other type of message
            else
            {
                //	add the current event index
                AddByte(core.eventIndex.GetEventIndex());
            }
        }

        /// <summary>
        /// Adds an 8-bit value to the transmit fifo and accumulates the crc.
        /// If the fifo is then full, sends a CAN frame.
        /// </summary>
        /// <param name="value">The value to add to the fifo.</param>
        public void AddByte(Byte value)
        {
            //	accumulate crc
            crc = Crc.AddByteToCRC16(value, crc);

            //	add the byte to the fifo
            fifo[fifoIndex] = value;

            //	if fifo is full, then send CAN frame
            if (MATRIX_TRANSMITTER_FIFO_SIZE <= ++fifoIndex)
            {
                MatrixTransmitter_SendFrame();
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Adds a 16-bit value to the transmit fifo and accumulates the crc.
        /// If the fifo is then full, sends a CAN frame.
        /// </summary>
        /// <param name="value">The value to add to the fifo</param>
        public void AddInt16(UInt16 value)
        {
            AddByte((Byte)((UInt16)(value >> 8)));
            AddByte((Byte)value);
        }

        /// <summary>
        /// Adds a 32-bit value to the transmit fifo and accumulates the crc.
        /// If the fifo is then full, sends a CAN frame.
        /// </summary>
        /// <param name="value">The value to add to the fifo</param>
        public void AddInt32(UInt32 value)
        {
            AddByte((Byte)((UInt32)(value >> 24)));
            AddByte((Byte)((UInt32)(value >> 16)));
            AddByte((Byte)((UInt32)(value >> 8)));
            AddByte((Byte)value);
        }

        /// <summary>
        /// Adds a string up to 256 characters to the transmit fifo and accumulates the crc.
        /// If the fifo is then full, sends a CAN frame.
        /// </summary>
        /// <param name="str">The string to add to the fifo.</param>
        public void AddString(String str)
        {
            char[] strb = str.ToCharArray();
            for (int i = 0; ((i < str.Length) && (i < 256)); i++)
                AddByte((Byte)strb[i]);
            AddByte(0);
        }

        /// <summary>
        /// Sends any message bytes remaining in the transmit fifo.
        /// </summary>
        /// <returns>Returns zero on success, else -1.</returns>
        public int FinishMessage()
        {
            UInt32 numBytesToSend, crcVal;
            int status;
            bool isSingleFrame;

            //	determine if is single frame message
            isSingleFrame = (8 >= (numBytesSent + fifoIndex));

            //	if more than one frame to send, then add checksum
            if (!isSingleFrame)
            {
                //	get crc in local variable because AddByte updates the crc
                crcVal = crc;
                AddByte((Byte)((UInt16)(crcVal >> 8)));
                AddByte((Byte)crcVal);
            }

            //	while bytes in fifo
            status = 0;
            while (0 != fifoIndex)
            {
                //	get number of bytes to send
                numBytesToSend = (fifoIndex <= LibConfig.CAN_FRAME_MAX_NUM_BYTES) ?
                    fifoIndex : LibConfig.CAN_FRAME_MAX_NUM_BYTES;

                //	flag last frame
                if (fifoIndex == numBytesToSend)
                {
                    idAddress &= 0x00ffffff;
                    if (isSingleFrame)
                        idAddress |= ((UInt32)LibConfig.MATRIX_MESSAGE_FRAME_TYPE_SINGLE
                            << LibConfig.MATRIX_CAN_ID_FRAME_TYPE_SHIFT);
                    else
                        idAddress |= ((UInt32)LibConfig.MATRIX_MESSAGE_FRAME_TYPE_LAST
                            << LibConfig.MATRIX_CAN_ID_FRAME_TYPE_SHIFT);

                }

                //	send the frame
                status = MatrixTransmitter_SendFrame();
                if (0 != status)
                    return status;
            }

            //  added for .Net threading
            SendCanFrames(core, txFrames);

            //  return the status
            return status;
        }

        /// <summary>
        /// Sends a CAN frame from the fifo.
        /// </summary>
        /// <returns>Returns 0 on success, else -1.</returns>
        int MatrixTransmitter_SendFrame()
        {
            // see static method below - int status = -1;

            //	get number of bytes to send
            UInt16 numBytesToSend =
                (UInt16)((fifoIndex <= LibConfig.CAN_FRAME_MAX_NUM_BYTES) ?
                fifoIndex : LibConfig.CAN_FRAME_MAX_NUM_BYTES);
            if (0 == numBytesToSend)
                return -1;

            //  get data
            byte[] data = new byte[numBytesToSend];
            for (int i = 0; i < numBytesToSend; i++)
                data[i] = fifo[i];

            //  send the frame
            //  see static method below - status = core.SendCanFrame(idAddress | frameIndex, data);
            txFrames.Add(new TxCanFrame() { id = idAddress, data = data });

            //	update number of bytes sent and the frame index
            numBytesSent += numBytesToSend;
            // see static method below - frameIndex = (UInt16)((UInt32)(frameIndex + 1) & LibConfig.MATRIX_CAN_ID_FRAME_INDEX_MASK);

            //	move index and shift any remaining bytes in fifo
            if (fifoIndex >= numBytesToSend)
                fifoIndex -= numBytesToSend;
            else
                fifoIndex = 0;
            for (int i = 0; i < fifoIndex; ++i)
                fifo[i] = fifo[i + numBytesToSend];

            // see static method below - return status
            return 0;
        }

        /// <summary>
        /// A static locked method to send a message's CAN frames.
        /// Added for .Net multi-threading.
        /// </summary>
        /// <param name="core">The ECCONet core.</param>
        /// <param name="frames">The frames to send.</param>
        static void SendCanFrames(ECCONetCore core, List<TxCanFrame>frames)
        {
            //  validate inputs
            if ((core == null) || (frames == null) || (frames.Count == 0))
                return;

            //  validate lock
            if (txLock == null)
                txLock = new object();

            lock (txLock)
            {
                foreach (var frame in frames)
                {
                    //  send the frame and bump the frame index
                    core.SendCanFrame(frame.id | frameIndex, frame.data);
                    frameIndex = (UInt16)((UInt32)(frameIndex + 1) & LibConfig.MATRIX_CAN_ID_FRAME_INDEX_MASK);
                }
            }
        }

    }

}
