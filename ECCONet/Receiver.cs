/**
  ******************************************************************************
  * @file    	Receiver.cs
  * @copyright  © 2017 ECCO Group.  All rights reserved.
  * @author  	M. Latham, Liquid Logic, LLC
  * @version 	1.0.0
  * @date    	April 2017
  * @brief   	Receives Matrix messages on the CAN bus.
	*
	*			Incoming CAN frames arrive asynchronously via CAN API callback
	*			that places them in the back region of a stream buffer.
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
using System.Threading.Tasks;
using System.Timers;
using System.Diagnostics;


namespace ECCONet
{
    internal sealed class Receiver
    {
        /// <summary>
        /// The Matrix receiver frame flag enumeration.
        /// </summary>
        public enum FrameFlags
        {
            MR_FRAME_FLAG_NONE,
            MR_FRAME_FLAG_SINGLE = (int)LibConfig.MATRIX_MESSAGE_FRAME_TYPE_SINGLE,
            MR_FRAME_FLAG_BODY = (int)LibConfig.MATRIX_MESSAGE_FRAME_TYPE_BODY,
            MR_FRAME_FLAG_LAST = (int)LibConfig.MATRIX_MESSAGE_FRAME_TYPE_LAST,
        }

        /// <summary>
        /// A received CAN frame data object.
        /// </summary>
        public class MatrixCanFrame
        {
            //	the sender address
            //	an address of zero indicates an empty frame
            public Byte senderAddress;

            //	the sender frame index and last frame flag
            public Byte frameIndex;

            //	the number of frame data bytes
            public Byte dataSize;

            //	frame flags
            public FrameFlags frameFlags;

            //	event flag
            public bool isEvent;

            //	the frame data
            public Byte[] data;

            //  timestamp for debug
            public DateTime dateTime;

            /// <summary>
            /// Constructor.
            /// </summary>
            public MatrixCanFrame()
            {
                data = new byte[LibConfig.CAN_FRAME_MAX_NUM_BYTES];
                frameFlags = Receiver.FrameFlags.MR_FRAME_FLAG_NONE;
            }

            /// <summary>
            /// Creates a deep copy of the frame.
            /// </summary>
            /// <returns>A deep copy of the frame.</returns>
            public MatrixCanFrame DeepCopy()
            {
                MatrixCanFrame frame = new MatrixCanFrame()
                {
                    senderAddress = senderAddress,
                    frameIndex = frameIndex,
                    dataSize = dataSize,
                    frameFlags = frameFlags,
                    dateTime = dateTime,
                    isEvent = isEvent
                };
                for (int i = 0; i < LibConfig.CAN_FRAME_MAX_NUM_BYTES; ++i)
                    frame.data[i] = data[i];
                return frame;
            }

            /// <summary>
            /// To string override.
            /// </summary>
            /// <returns>The CAN frame string.</returns>
            public override string ToString()
            {
                return string.Format("addr:{0}  len:{1}  idx:{2:00}  mS:{3:0.0}  flag:{4}",
                    senderAddress, dataSize, frameIndex, (Single)((Int64)((dateTime.Ticks / 1000) % 10000))/10.0, frameFlags);
            }
        };


        //	incoming stream buffer
        MatrixCanFrame[] streamBuffer;

        //	incoming stream buffer
        MatrixCanFrame[] rxBuffer;

        //	the incoming frame write index
        UInt16 rxBufferWriteIndex;

        //	the incoming frame read index
        UInt16 rxBufferReadIndex;

        //	the sender address filter
        //	this is normally zero to receive messages from all senders
        Byte senderAddressFilter;

        //	the address filter timer
        UInt32 senderAddressFilterTimer;

        //  the rx callback lock
        Object rxCallbackLock;

        //  the rx buffer lock
        Object rxBufferLock;

        //  the core logic
        ECCONetCore core;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="core">The core logic group.</param>
        public Receiver(ECCONetCore core)
        {
            //  save the core
            this.core = core;

            //  rx callback and buffer lock
            rxCallbackLock = new Object();
            rxBufferLock = new Object();

            //  new stream buffer
            streamBuffer = new MatrixCanFrame[LibConfig.MR_STREAM_BUFFER_FRONT_SIZE];
            for (int i = 0; i < LibConfig.MR_STREAM_BUFFER_FRONT_SIZE; i++)
                streamBuffer[i] = new MatrixCanFrame();

            //  new rx buffer
            rxBuffer = new MatrixCanFrame[LibConfig.MR_STREAM_BUFFER_BACK_SIZE];
            for (int i = 0; i < LibConfig.MR_STREAM_BUFFER_BACK_SIZE; i++)
                rxBuffer[i] = new MatrixCanFrame();

            //	the the stream indices
            rxBufferWriteIndex = 0;
            rxBufferReadIndex = 0;

            //	clear the sender address filter
            senderAddressFilter = 0;
            senderAddressFilterTimer = 0;
        }

        /// <summary>
        /// The receiver clock.
        /// </summary>
        public void Clock()
        {
            //  check for expiration of server address filter
            if ((0 != senderAddressFilterTimer) && (0 == --senderAddressFilterTimer))
            {
                senderAddressFilter = 0;
            }

            //	validate uint rxBufferReadIndex as less than the back buffer size
            if (rxBufferReadIndex >= LibConfig.MR_STREAM_BUFFER_BACK_SIZE)
                rxBufferReadIndex = 0;

            //	check for received frames
            UInt16 numNewFrames = (UInt16)(rxBufferWriteIndex - rxBufferReadIndex);
            while (numNewFrames > LibConfig.MR_STREAM_BUFFER_BACK_SIZE)
                numNewFrames += (UInt16)LibConfig.MR_STREAM_BUFFER_BACK_SIZE;

            //	if new frames to process
            if (0 < numNewFrames)
            {
                //	shift bytes in front buffer to make room
                int i;
                for (i = 0; i < (LibConfig.MR_STREAM_BUFFER_FRONT_SIZE - numNewFrames); ++i)
                    streamBuffer[i] = streamBuffer[i + numNewFrames];

                //	copy received bytes into front buffer
                while (i < LibConfig.MR_STREAM_BUFFER_FRONT_SIZE)
                {
                    //  copy frame
                    streamBuffer[i++] = rxBuffer[rxBufferReadIndex].DeepCopy();
                    if (++rxBufferReadIndex >= LibConfig.MR_STREAM_BUFFER_BACK_SIZE)
                        rxBufferReadIndex = 0;
                }

                //	process the messages in the stream
                ProcessMessagesInStream(numNewFrames);
            }

            //	process the messages in the stream
            if (0 != numNewFrames)
                ProcessMessagesInStream(numNewFrames);
        }

        /// <summary>
        /// Set the receiver sender address filter.
        /// The sender address filter is normally zero to receive message from all senders,
        /// and is periodically reset to broadcast by the timer callback.
        /// </summary>
        /// <param name="senderAddressFilterIn">The node address to filter.</param>
        /// <param name="timeoutPeriodMilliseconds">The filter timeout period.</param>
        public void SetSenderAddressFilter(Byte senderAddressFilterIn, int timeoutPeriodMilliseconds)
        {
            //	set the address
            senderAddressFilter = senderAddressFilterIn;
            senderAddressFilterTimer =
                (UInt32)(timeoutPeriodMilliseconds / ECCONetCore.ProcessManagerTimerPeriod);
        }

        /// <summary>
        /// Processes the messages in the stream.
        /// </summary>
        /// <param name="numNewFrames">The number of new frames in the stream.</param>
        void ProcessMessagesInStream(UInt16 numNewFrames)
        {
            int messageFrame, nextMessageFrame, frame, lastFrame;
            UInt16 frameIndex, numMessageBytes, numMessageFrames;
            bool isCompleteMessage;
            Token.Keys key;

            //  remove unprocessed frames
            RemoveUnprocessedFrames();

            //	sort the new frames
            SortNewFrames(numNewFrames);

            //	starting with oldest frames, search for complete messages
            messageFrame = 0;
            lastFrame = (int)LibConfig.MR_STREAM_BUFFER_FRONT_SIZE;
            while (messageFrame < lastFrame)
            {
                //	advance to used frame
                if (Receiver.FrameFlags.MR_FRAME_FLAG_NONE == streamBuffer[messageFrame].frameFlags)
                {
                    ++messageFrame;
                    continue;
                }

                //	advance to the next message start
                numMessageFrames = 0;
                numMessageBytes = 0;
                frameIndex = streamBuffer[messageFrame].frameIndex;
                nextMessageFrame = messageFrame;
                isCompleteMessage = false;
                while (nextMessageFrame < lastFrame)
                {
                    //	if not next frame in sequence, then next message
                    if ((frameIndex != streamBuffer[nextMessageFrame].frameIndex) ||
                        (streamBuffer[nextMessageFrame].senderAddress != streamBuffer[messageFrame].senderAddress))
                        break;

                    //	if a single-frame message, then done
                    if (Receiver.FrameFlags.MR_FRAME_FLAG_SINGLE == streamBuffer[nextMessageFrame].frameFlags)
                    {
                        numMessageFrames = 1;
                        numMessageBytes = streamBuffer[nextMessageFrame].dataSize;
                        ++nextMessageFrame;
                        isCompleteMessage = true;
                        break;
                    }

                    //	accumulate the frame count and data size
                    ++numMessageFrames;
                    numMessageBytes += streamBuffer[nextMessageFrame].dataSize;

                    //	if the last frame in message, then done
                    if (Receiver.FrameFlags.MR_FRAME_FLAG_LAST == streamBuffer[nextMessageFrame].frameFlags)
                    {
                        ++nextMessageFrame;
                        isCompleteMessage = (numMessageFrames > 1);
                        break;
                    }

                    //  bump the frame index and the message frame
                    ++nextMessageFrame;
                    frameIndex = (Byte)((frameIndex + 1) & LibConfig.MATRIX_CAN_ID_FRAME_INDEX_MASK);
                }

                //	if have a complete message
                if (isCompleteMessage)
                {
                    //  if core logic
                    if (null != core)
                    {
                        //	concatenate message bytes
                        Byte[] data = new Byte[numMessageBytes];
                        int byteIndex = -1;
                        for (frame = messageFrame; frame < nextMessageFrame; ++frame)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                if (++byteIndex >= numMessageBytes)
                                    break;
                                data[byteIndex] = streamBuffer[frame].data[i];
                            }
                        }

                        //	if message checksum is valid
                        if ((numMessageFrames == 1) || Crc.IsMessageChecksumValid(data, numMessageBytes))
                        {
                            //  remove checksum from message
                            if (LibConfig.CAN_FRAME_MAX_NUM_BYTES < numMessageBytes)
                                numMessageBytes -= 2;

                            //  if message has at least the event index and a token key
                            if (3 <= numMessageBytes)
                            {
                                //  copy data to message
                                Byte[] message = new Byte[numMessageBytes];
                                for (int i = 0; i < numMessageBytes; ++i)
                                    message[i] = data[i];

                                //	get key
                                key = (Token.Keys)(data[1] & ~0xE0);
                                key = (Token.Keys)(((UInt16)key << 8) | data[2]);


                                //  if is the firmware update response key
                                if ((key == Token.Keys.KeyResponseFileWriteFixedSegment) || 
                                    (key == Token.Keys.KeyResponseFileEraseFixedSegment))
                                {
                                    if (data.Length >= 4)
                                    {
                                        Token token = new Token(key, data[3], streamBuffer[messageFrame].senderAddress);
                                        core.ReceiveTokenFromCanBus(token);
                                    }
                                }

                                //	else if ftp response, then send to ftp client
                                else if (Token.Key_IsFtpResponse((ushort)key))
                                {
                                    Byte[] body = new Byte[numMessageBytes - 3];
                                    for (int i = 0; i < (numMessageBytes - 3); ++i)
                                        body[i] = message[i + 3];
                                    core.FTP_ServerResponseIn(streamBuffer[messageFrame].senderAddress, key, body);
                                }
                                else
                                {
                                    //	capture event index
                                    core.eventIndex.NewEventIndex(data[0]);

                                    //  if an event or message index has not expired
                                    if (streamBuffer[messageFrame].isEvent || Token.Key_IsCommand((ushort)key) || !core.eventIndex.IsEventIndexExpired(data[0]))
                                    {
                                        //  decompress token stream
                                        Byte[] stream = new Byte[numMessageBytes - 1];
                                        for (int i = 0; i < (numMessageBytes - 1); ++i)
                                            stream[i] = message[i + 1];
                                        core.codec.receiveToken = core.ReceiveTokenFromCanBus;
                                        core.codec.Decompress(stream, streamBuffer[messageFrame].senderAddress, data[0]);
                                    }
                                }
                            }
                        }
                        else  //  bad checksum
                        {
                            if (ECCONetCore.ConsoleDebugMessageLevels.Errors <= core.consoleDebugMessageLevel)
                                Debug.WriteLine("Receiver: Bad checksum. Check CAN wiring termination.");
                        }
                    }

                    //	erase message in message stream
                    for (frame = nextMessageFrame - 1; frame >= numMessageFrames; --frame)
                        streamBuffer[frame] = streamBuffer[frame - numMessageFrames];
                    for (; frame >= 0; --frame)
                        streamBuffer[frame] = new MatrixCanFrame();
                }

                //	next message
                messageFrame = nextMessageFrame;
            }
        }

        /// <summary>
        /// Removes unprocessed frames in stream.
        /// </summary>
        private void RemoveUnprocessedFrames()
        {
            var now = DateTime.Now.Ticks;
            for (int i = 0; i < LibConfig.MR_STREAM_BUFFER_FRONT_SIZE; ++i)
            {
                if ((streamBuffer[i].frameFlags != FrameFlags.MR_FRAME_FLAG_NONE)
                    && ((now - streamBuffer[i].dateTime.Ticks) >= LibConfig.CAN_FRAME_EXPIRATION_PERIOD_TICKS))
                {
                    //  remove the frame
                    for (int n = i; n > 0; --n)
                        streamBuffer[n] = streamBuffer[n - 1];
                    streamBuffer[0] = new MatrixCanFrame();
                }
            }
        }

        /// <summary>
        /// Sorts the new frames into the proper location within the stream back buffer.
        /// </summary>
        /// <param name="numNewFrames">The number of new frames received.</param>
        private void SortNewFrames(UInt16 numNewFrames)
        {
            int newFrame, compareFrame, lastFrame;
            MatrixCanFrame tempFrame = new MatrixCanFrame();
            bool matchFound;
            Int16 n;

            //	for all new frames
            newFrame = (int)(LibConfig.MR_STREAM_BUFFER_FRONT_SIZE - numNewFrames - 1);
            lastFrame = (int)LibConfig.MR_STREAM_BUFFER_FRONT_SIZE;
            while (++newFrame < lastFrame)
            {
                //  try to find most recent frame from same source
                compareFrame = newFrame;
                while ((--compareFrame >= 0)
                    && (streamBuffer[compareFrame].frameFlags != FrameFlags.MR_FRAME_FLAG_NONE))
                {
                    //  if have one or more frames from same source
                    if (streamBuffer[compareFrame].senderAddress == streamBuffer[newFrame].senderAddress)
                    {
                        //  find correct position within frames
                        n = 15;
                        matchFound = false;
                        ++compareFrame;
                        while ((--compareFrame >= 0)
                            && (streamBuffer[compareFrame].frameFlags != FrameFlags.MR_FRAME_FLAG_NONE)
                            && (streamBuffer[compareFrame].senderAddress == streamBuffer[newFrame].senderAddress)
                            && (--n >= 0))
                        {
                            //	if frame found with same index as new frame, then done
                            if (streamBuffer[newFrame].frameIndex == streamBuffer[compareFrame].frameIndex)
                            {
                                matchFound = true;
                                break;
                            }

                            //	else if frame found with previous index to new frame, then done
                            else if ((((streamBuffer[newFrame].frameIndex - streamBuffer[compareFrame].frameIndex)
                                & LibConfig.MATRIX_CAN_ID_FRAME_INDEX_MASK)
                                < ((LibConfig.MATRIX_CAN_ID_FRAME_INDEX_MASK + 1) / 2)))
                                break;
                        }

                        //	if same frame index found, then replace previous frame with new one
                        if (matchFound)
                        {
                            //  replace frame
                            streamBuffer[compareFrame] = streamBuffer[newFrame];
                            for (int i = newFrame; i > 0; --i)
                                streamBuffer[i] = streamBuffer[i - 1];
                            streamBuffer[0] = new MatrixCanFrame();
                        }

                        //	else if frame has new location, then move into place
                        else if (++compareFrame < newFrame)
                        {
                            //  move frames into place
                            tempFrame = streamBuffer[newFrame];
                            for (int i = newFrame; i > compareFrame; --i)
                                streamBuffer[i] = streamBuffer[i - 1];
                            streamBuffer[compareFrame] = tempFrame;
                        }

                        //  done
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// The ECCONet CAN reciever method to handle incoming can frames.
        /// </summary>
        /// <param name="id">The CAN frame ID.</param>
        /// <param name="data">The CAN frame data.</param>
        public void ReceiveCanFrame(UInt32 id, Byte[] data)
        {
            UInt32 frameType, sourceAddress, destinationAddress;

            //  if no core, return
            if (null == core)
                return;

            lock (rxCallbackLock)
            {
                //  frame type filter
                //
                //	if not an ECCONet message, then return
                frameType = (UInt32)(id >> LibConfig.MATRIX_CAN_ID_FRAME_TYPE_SHIFT)
                & LibConfig.MATRIX_CAN_ID_FRAME_TYPE_MASK;
                if ((frameType != LibConfig.MATRIX_MESSAGE_FRAME_TYPE_BODY)
                    && (frameType != LibConfig.MATRIX_MESSAGE_FRAME_TYPE_LAST)
                    && (frameType != LibConfig.MATRIX_MESSAGE_FRAME_TYPE_SINGLE))
                    return;

                //  source address
                //
                //  not filtering traffic by source address
                sourceAddress = (id >> LibConfig.MATRIX_CAN_ID_SOURCE_ADDRESS_SHIFT)
                    & LibConfig.MATRIX_CAN_ID_ADDRESS_MASK;

                //  send source address to online device monitor except for
                //  skip single-frame messages with event index 0 (address negotiation, command, etc.)
                if (frameType != LibConfig.MATRIX_MESSAGE_FRAME_TYPE_SINGLE || (data.Length > 0 && data[0] != 0))
                {
                    core.deviceMonitor.DeviceMessageReceived((byte)sourceAddress);
                }

                //  destination address
                //
                //	filter out non-broadcast messages and messages not send to this device
                destinationAddress = (id >> LibConfig.MATRIX_CAN_ID_DEST_ADDRESS_SHIFT)
                    & LibConfig.MATRIX_CAN_ID_ADDRESS_MASK;
                if ((destinationAddress != LibConfig.MATRIX_CAN_BROADCAST_ADDRESS)
                    && (destinationAddress != core.GetCanAddress()))
                    return;

                //  create new frame
                MatrixCanFrame frame = new MatrixCanFrame();
                frame.dateTime = DateTime.Now;
                frame.frameIndex = (Byte)((id >> LibConfig.MATRIX_CAN_ID_FRAME_INDEX_SHIFT)
                    & LibConfig.MATRIX_CAN_ID_FRAME_INDEX_MASK);
                frame.isEvent = (0 != ((id >> LibConfig.MATRIX_CAN_ID_EVENT_FLAG_SHIFT)
                    & LibConfig.MATRIX_CAN_ID_EVENT_FLAG_MASK));
                frame.frameFlags = (FrameFlags)frameType;
                frame.senderAddress = (byte)sourceAddress;
                frame.dataSize = (Byte)Math.Min(data.Length, LibConfig.CAN_FRAME_MAX_NUM_BYTES);
                for (int n = 0; n < frame.dataSize; ++n)
                    frame.data[n] = data[n];

                //	patch for VBT
                //	note that if the VBT gets updated, this patch becomes redundant
                if ((frameType == LibConfig.MATRIX_MESSAGE_FRAME_TYPE_SINGLE)
                    && (sourceAddress == LibConfig.MATRIX_VEHICLE_BUS_ADDRESS))
                    frame.isEvent = true;

                //	validate uint rxBufferWriteIndex as less than the back buffer size
                if (rxBufferWriteIndex >= LibConfig.MR_STREAM_BUFFER_BACK_SIZE)
                    rxBufferWriteIndex = 0;

                //  add new frame to queue and bump write index
                rxBuffer[rxBufferWriteIndex] = frame;
                if (++rxBufferWriteIndex >= LibConfig.MR_STREAM_BUFFER_BACK_SIZE)
                    rxBufferWriteIndex = 0;

                //  debug
                if (core.consoleDebugMessageLevel >= ECCONetCore.ConsoleDebugMessageLevels.Frames)
                {
                    Debug.Write(frame.frameIndex.ToString("x2") + "\t");
                    for (int i = 0; i < frame.dataSize; ++i)
                        Debug.Write(frame.data[i].ToString("x2") + " ");
                    Debug.Write("\n");
                }
            }
        }

    }
}

