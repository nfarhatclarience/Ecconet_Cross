/**
  ******************************************************************************
  * @file    	Codec.cs
  * @copyright  © 2017 ECCO Group.  All rights reserved.
  * @author  	M. Latham, Liquid Logic, LLC
  * @version 	1.0.0
  * @date    	April 2017
  * @brief   	Converts tokens to compressed byte streams and vice versa.
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
using System.Diagnostics;



namespace ECCONet
{
    internal sealed class Codec
    {
        /// <summary>
        /// A delegate to receive a stream byte during compression.
        /// </summary>
        /// <param name="data">The compression byte output.</param>
        public delegate void ReceiveByteDelegate(byte data);

        /// <summary>
        /// The delegate to receive a stream byte during compression.
        /// </summary>
        public ReceiveByteDelegate receiveByte = null;


        /// <summary>
        /// A delegate to receive a token during decompression.
        /// </summary>
        /// <param name="data">The compression byte output.</param>
        public delegate void ReceiveTokenDelegate(Token token);

        /// <summary>
        /// The delegate to receive a token during decompression.
        /// </summary>
        public ReceiveTokenDelegate receiveToken = null;


        /// <summary>
        /// Converts one or more tokens into a compressed byte stream.
        /// Note: Sorts the tokens in place, so token(s) must not be const array.
        /// </summary>
        /// <param name="tokens">A pointer to an array of one or more tokens.</param>
        /// <returns>Returns 0 on success, else -1.</returns>
        public int Compress(Token[] tokens)
        {
            int token, sortToken, compareToken, lastToken;
            Token tempToken = new Token();
            Byte byteVal;
            Int32 value;
            UInt32 valueSize, bit;
            UInt32 numBinaryRepeats, numAnalogRepeats;
            Token.Keys key;

            //	validate inputs
            if ((null == receiveByte) || (0 == tokens.Length))
                return -1;

            //	sort the tokens by key
            sortToken = 0;
            lastToken = tokens.Length;
            while (++sortToken < lastToken)
            {
                compareToken = sortToken;
                while (--compareToken >= 0)
                    if (tokens[compareToken].key <= tokens[sortToken].key)
                        break;
                if (++compareToken < sortToken)
                {
                    tempToken = tokens[sortToken];
                    for (int i = sortToken; i > compareToken; --i)
                        tokens[i] = tokens[i - 1];
                    tokens[compareToken] = tempToken;
                }
            }

            //	for all tokens
            token = 0;
            while (token < lastToken)
            {
                //	if no value associated with token,
                //	then just send it without compression
                valueSize = Token.Key_ValueSize((ushort)tokens[token].key);
                if (0 == valueSize)
                {
                    OutputToken(tokens[token]);
                    ++token;
                    continue;
                }

                //	check for compressible series starting with current token
                numAnalogRepeats = 0;
                numBinaryRepeats = 0;
                value = tokens[token].value;
                key = tokens[token].key + 1;
                compareToken = token + 1;
                while ((numAnalogRepeats < (LibConfig.MATRIX_MESSAGE_MAX_TOKEN_REPEATS - 1)) &&
                    (compareToken < lastToken) && (tokens[compareToken].key == key) &&
                    (Token.Key_ValueSize((ushort)tokens[compareToken].key) == valueSize))
                {
                    //	get first non-zero value for binary repeat
                    if ((0 == value) && (0 != tokens[compareToken].value))
                        value = tokens[compareToken].value;

                    //	check for non-zero value match for binary repeat
                    if ((0 == tokens[compareToken].value) || (tokens[compareToken].value == value))
                        ++numBinaryRepeats;
                    else
                        numBinaryRepeats = LibConfig.MATRIX_MESSAGE_MAX_TOKEN_REPEATS;

                    //	bump params
                    ++compareToken;
                    ++key;
                    ++numAnalogRepeats;
                }

                //	if a binary series found
                if ((0 != numBinaryRepeats) && (numBinaryRepeats < LibConfig.MATRIX_MESSAGE_MAX_TOKEN_REPEATS))
                {
                    //	send repeat, first key in sequence and the common non-zero value
                    receiveByte((Byte)(numBinaryRepeats | (Byte)Token.KeyPrefix.BinaryRepeat));
                    OutputTokenKey(tokens[token].key);
                    OutputTokenValue((UInt32)value, (UInt16)valueSize);

                    //	for tokens in series
                    bit = 0;
                    byteVal = 0;
                    ++numBinaryRepeats;
                    while (0 != numBinaryRepeats--)
                    {
                        //	set non-zero value flag and bump token
                        if (0 != tokens[token].value)
                            byteVal |= (Byte)(1 << (int)bit);
                        if (++bit >= 8)
                        {
                            receiveByte(byteVal);
                            bit = 0;
                            byteVal = 0;
                        }
                        ++token;
                    }

                    //	if partially full byte, then send it
                    if (0 != bit)
                        receiveByte(byteVal);
                }

                //	else if an analog series found
                else if (0 != numAnalogRepeats)
                {
                    //	send repeat and base token
                    receiveByte((Byte)(numAnalogRepeats | (Byte)Token.KeyPrefix.AnalogRepeat));
                    OutputToken(tokens[token]);
                    ++token;

                    //	for all others in series, send token value
                    while (0 != numAnalogRepeats--)
                    {
                        //	set value and bump token
                        OutputTokenValue((UInt32)tokens[token].value, (UInt16)valueSize);
                        ++token;
                    }
                }

                //	else no compressible series starting with current token
                else
                {
                    //	send token and bump token index
                    OutputToken(tokens[token]);
                    ++token;
                }
            }
            return 0;
        }

        /// <summary>
        /// Converts a compressed byte stream into one or more tokens.
        /// </summary>
        /// <param name="bytes">An array of bytes to decompress.</param>
        /// <param name="senderAddress">The sender address that should be given with the tokens.</param>
        /// <param name="eventIndex">The event index, used for dev tool observation.</param>
        /// <returns>Returns 0 on success, else -1.</returns>
        public int Decompress(byte[] bytes, byte senderAddress, byte eventIndex)
        {
            Token token = new Token();
            UInt32 currentByte, lastByte;
            UInt16 bitFlags, bitIndex;
            UInt16 tokenType, numRepeats, valueSize;
            Int32 value;

            //	validate inputs
            if ((null == receiveToken) || (0 == bytes.Length))
                return -1;

            try
            {
                //	for all bytes
                numRepeats = 0;
                currentByte = 0;
                lastByte = (UInt32)bytes.Length;
                while (currentByte < lastByte)
                {
                    //	token type and number of repeats
                    tokenType = (UInt16)(bytes[currentByte] & (Byte)Token.KeyPrefix.Mask);

                    //  if token is not standard or repeat, then done (sequencer compression)
                    if ((byte)Token.KeyPrefix.AnalogRepeat < tokenType)
                        return 0;

                    //	if token type is a binary or analog repeat, then get number of repeats
                    if ((tokenType == (byte)Token.KeyPrefix.BinaryRepeat) ||
                        (tokenType == (byte)Token.KeyPrefix.AnalogRepeat))
                    {
                        numRepeats = (UInt16)((bytes[currentByte++] & (LibConfig.MATRIX_MESSAGE_MAX_TOKEN_REPEATS - 1)) + 1);
                    }

                    //	token key and value size
                    token.address = senderAddress;
                    token.eventIndex = eventIndex;
                    token.key = (Token.Keys)bytes[currentByte++];
                    token.key = (Token.Keys)(((UInt16)token.key << 8) | bytes[currentByte++]);
                    valueSize = Token.Key_ValueSize((ushort)token.key);

                    //	if an analog repeat
                    if (tokenType == (Byte)Token.KeyPrefix.AnalogRepeat)
                    {
                        //	for all tokens in series
                        while (0 != numRepeats--)
                        {
                            //	output token
                            token.value = 0;
                            for (int n = 0; n < valueSize; ++n)
                            {
                                token.value <<= 8;
                                token.value |= bytes[currentByte++];
                            }
                            receiveToken(token);
                            ++token.key;
                        }
                    }

                    //	else if a binary repeat
                    else if (tokenType == (Byte)Token.KeyPrefix.BinaryRepeat)
                    {
                        //	get common non-zero value
                        value = 0;
                        for (int n = 0; n < valueSize; ++n)
                        {
                            value <<= 8;
                            value |= bytes[currentByte++];
                        }

                        //	for all tokens in series
                        bitFlags = 0;
                        bitIndex = 8;
                        while (0 != numRepeats--)
                        {
                            bitFlags >>= 1;
                            if (++bitIndex >= 8)
                            {
                                bitIndex = 0;
                                bitFlags = bytes[currentByte++];
                            }
                            token.value = (0 != (bitFlags & 1)) ? value : 0;
                            receiveToken(token);
                            ++token.key;
                        }
                    }

                    //	else a single token
                    else
                    {
                        token.value = 0;
                        for (int n = 0; n < valueSize; ++n)
                        {
                            token.value <<= 8;
                            token.value |= bytes[currentByte++];
                        }
                        receiveToken(token);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("in codec: " + ex.ToString());
            }
            return 0;
        }

        /// <summary>
        /// Adds a token key to the Matrix transmit fifo.
        /// </summary>
        /// <param name="key"></param>
        void OutputTokenKey(Token.Keys key)
        {
            receiveByte((byte)((UInt16)key >> 8));
            receiveByte((byte)key);
        }

        /// <summary>
        /// Adds a token value to the Matrix transmit fifo.
        /// </summary>
        /// <param name="value">A token value to output.</param>
        /// <param name="valueSize">The value size in bytes.</param>
        void OutputTokenValue(UInt32 value, UInt16 valueSize)
        {
            while (0 != valueSize--)
                receiveByte((Byte)((UInt32)(value >> (8 * valueSize))));
        }

        /// <summary>
        /// Adds a token to the transmit fifo.
        /// </summary>
        /// <param name="token">A token to add.</param>
        void OutputToken(Token token)
        {
            UInt16 valueSize;

            receiveByte((byte)((UInt16)token.key >> 8));
            receiveByte((byte)token.key);
            valueSize = Token.Key_ValueSize((ushort)token.key);
            OutputTokenValue((UInt32)token.value, valueSize);
        }
    }
}