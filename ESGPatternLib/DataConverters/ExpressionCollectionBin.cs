using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESG.ExpressionLib.DataModels;
using ECCONet;

using Token = ESG.ExpressionLib.DataModels.Expression.Token;
using Area = ESG.ExpressionLib.DataModels.Expression.Area;
using Entry = ESG.ExpressionLib.DataModels.Expression.Entry;
using Step = ESG.ExpressionLib.DataModels.Expression.Step;
using RepeatedSectionStart = ESG.ExpressionLib.DataModels.Expression.RepeatSectionStart;
using RepeatedSectionEnd = ESG.ExpressionLib.DataModels.Expression.RepeatSectionEnd;
using NestedExpression = ESG.ExpressionLib.DataModels.Expression.NestedExpression;


namespace ESG.ExpressionLib.DataConverters
{
    public static partial class ExpressionConverters
    {
        //  file keys
        const uint tokenPatternFileKey = 0x4865433B;
        const uint stepDictionaryFileKey = 0x38B1E2BA;

        /// <summary>
        /// Builds expression collection step method bin files.  Throws an exception on error.
        /// </summary>
        /// <param name="ec">The expression collection.</param>
        /// <param name="outputs">The outputs for which the expression is mapped.</param>
        /// <param name="binPatternTable">A return value of the bin pattern table.</param>
        /// <param name="binDictionaries">A return value of the bin master dictionary, or null if outputs do not use dictionary.</param>
        public static void BuildExpressionCollectionStepMethodBinFiles(ExpressionCollection ec, OutputArrayNode outputs, 
            out byte[] binPatternTable, out byte[] binDictionaries)
        {
            //  default method outputs
            binPatternTable = null;
            binDictionaries = null;

            //  intermediate conversion results
            ExpressionCollection stepExpColl = null;
            StepDictionary stepMasterDict = null;

            //  convert the project expression into binary files
            switch (outputs.OutputArrayType)
            {
                case OutputArrayNode.OutputTypeSequenced:

                    //  convert the step method expression collection into a bin file
                    binPatternTable = ExpressionConverters.ToPatternBinary(ec, PatternBinType.AnyToken);
                    break;


                case OutputArrayNode.OutputTypeSequencedDictionary:

                    //  get intermediate conversion results
                    ECtoStepMethodECandDictionary(ec, false, out stepExpColl, out stepMasterDict);

                    //  convert the step method expression collection into a bin file
                    binPatternTable = ExpressionConverters.ToPatternBinary(stepExpColl, PatternBinType.StepMethodDictionaryKeys);

                    //  convert the step method master dictionary into a bin file
                    BuildDictionaryBinFile(stepMasterDict, out binDictionaries);
                    break;


                case OutputArrayNode.OutputTypeSequencedTimerDictionary:

                    //  get intermediate conversion results
                    ECtoStepMethodECandDictionary(ec, true, out stepExpColl, out stepMasterDict);

                    //  convert the step method expression collection into a bin file
                    binPatternTable = ExpressionConverters.ToPatternBinary(stepExpColl, PatternBinType.StepMethodDictionaryKeys);

                    //  convert the step method master dictionary into a bin file
                    //  note that the light engines are required to know what dictionary bits to set
                    BuildTimerDictionaryBinFile(stepMasterDict, outputs, out binDictionaries);

                    //  print statistics (if in debug mode)
                    ExpressionTest.PrintExpressionAndBinFileStats(ec, binPatternTable, binDictionaries, 6);
                    break;


                default:
                    throw new Exception("Expression Bin conversion encountered unknown output array type.");
            }

        }


        #region Pattern table bin conversion
        /// <summary>
        /// The pattern bin type.
        /// </summary>
        public enum PatternBinType
        {
            /// <summary>
            /// For general sequencing of one or more tokens of any type.
            /// </summary>
            AnyToken = 0x00,

            /// <summary>
            /// Single-token patterns use a common token key, so a smaller table is created with just the token values.
            /// This is the type used for the step method, where token key = KeyStepMethodDictionaryKey and value = the dictionary key.
            /// </summary>
            StepMethodDictionaryKeys = 0x20,
            LedMatrixKeys = 0x40,
        }


        #region To bin helper methods

        /// <summary>
        /// Adds token's bytes to the bin.
        /// </summary>
        /// <param name="bytes">The binary being constructed.</param>
        /// <param name="token">The token to convert.</param>
        /// <param name="binType">The pattern bin type.</param>
        private static void TokenToBin(List<byte> bytes, Token token, PatternBinType binType)
        {
            //  validate inputs
            if ((null == bytes) || (null == token))
                return;

            //  if sequencing any types of tokens
            if (binType == PatternBinType.AnyToken)
            {
                //  add the token key
                bytes.Add((byte)(token.Key >> 8));
                bytes.Add((byte)(token.Key & 0xff));
            }

            //  add the token value
            int numBytes = ECCONet.Token.Key_ValueSize(token.Key);
            while (--numBytes >= 0)
                bytes.Add((byte)(token.Value >> (numBytes * 8)));
        }

        /// <summary>
        /// Adds list of tokens' bytes to bin.
        /// </summary>
        /// <param name="bytes">The binary being constructed.</param>
        /// <param name="tokens">The tokens to add.</param>
        /// <param name="binType">The pattern bin type.</param>
        private static void TokensToBin(List<byte> bytes, BindingList<Token> tokens, PatternBinType binType)
        {
            //  validate inputs
            if ((null == bytes) || (null == tokens))
                return;

            foreach (Token token in tokens)
                TokenToBin(bytes, token, binType);
        }

        /// <summary>
        /// Adds an expresion start entry to the bin.
        /// </summary>
        /// <param name="bytes">The binary being constructed.</param>
        /// <param name="expressionEnum">The ECCONet expression enumeration.</param>
        /// <param name="numRepeats">The number of expression repeats, limited to 15, and zero = infinite.</param>
        /// <param name="binType">The pattern bin type.</param>
        private static void ExpressionStartToBin(List<byte> bytes, UInt16 expressionEnum, byte numRepeats, PatternBinType binType)
        {
            //  validate inputs
            if (null == bytes)
                return;

            //  number of repeats must be limited to 15
            if (numRepeats > 15) numRepeats = 15;

            //  add the pattern start header and the pattern enumeration
            bytes.Add((byte)((numRepeats & 0xf0) | (int)ECCONet.Token.KeyPrefix.PatternPrefix_PatternWithRepeats));
            bytes.Add((byte)((expressionEnum >> 8) | (int)binType));
            bytes.Add((byte)(expressionEnum & 0xff));
        }

        /// <summary>
        /// Adds expresion areas all-off/priority-reset entry to the bin.
        /// </summary>
        /// <param name="bytes">The binary being constructed.</param>
        /// <param name="areas">The expression areas to add.</param>
        /// <param name="binType">The pattern bin type.</param>
        private static void ExpressionAreasToBin(List<byte> bytes, BindingList<Area> areas, PatternBinType binType)
        {
            //  validate inputs
            if ((null == bytes) || (null == areas))
                return;

            //  add the entry header
            bytes.Add((byte)(ECCONet.Token.KeyPrefix.PatternPrefix_PatternStepWithAllOff));

            //  add the area tokens
            switch (binType)
            {
                case PatternBinType.AnyToken:
                    {
                        //  create list of tokens
                        List<Token> areaTokens = new List<Token>();

                        //  for all areas
                        foreach (Area area in areas)
                        {
                            //  if area has output paths
                            if ((area.OutputPaths != null) && (area.OutputPaths.Count > 0))
                            {
                                foreach (var pathValue in area.OutputPaths)
                                    //TokenToBin(bytes, new Token((UInt16)pathValue.EndpointId, (area.DefaultValue * pathValue.Value) / 100), binType);
                                    areaTokens.Add(new Token((UInt16)pathValue.EndpointId, (area.DefaultValue * pathValue.Value) / 100));
                            }
                            else
                                //TokenToBin(bytes, new Token(area.Key, area.DefaultValue), binType);
                                areaTokens.Add(new Token(area.Key, area.DefaultValue));
                        }

                        //  compress tokens and add to bin
                        Compress(areaTokens.ToArray(), bytes);
                    }
                    break;

                case PatternBinType.LedMatrixKeys:
                case PatternBinType.StepMethodDictionaryKeys:
                    {
                        //  add the step dictionary all-off token
                        foreach (Area area in areas)
                            TokenToBin(bytes, new Token(area.Key, area.DefaultValue), binType);
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Adss an expression step entry to the bin.
        /// </summary>
        /// <param name="bytes">The binary being constructed.</param>
        /// <param name="step">The expression step to convert.</param>
        /// <param name="exp">The expression.</param>
        /// <param name="binType">The pattern bin type.</param>
        private static void ExpressionStepToBin(List<byte> bytes, Step step, Expression exp, PatternBinType binType)
        {
            //  validate inputs
            if ((null == bytes) || (null == step))
                return;

            //  add the entry header
            bytes.Add((byte)((step.Period >> 8) | (byte)ECCONet.Token.KeyPrefix.PatternPrefix_PatternStepWithPeriod));
            bytes.Add((byte)(step.Period & 0xff));

            //  add the step tokens
            switch (binType)
            {
                case PatternBinType.AnyToken:
                    {
                        //  create list of step tokens
                        List<Token> stepTokens = new List<Token>();

                        //  for each token in step
                        foreach (var token in step.Tokens)
                        {
                            //  check to see if token represents area with output paths
                            bool tokensSubstituted = false;
                            foreach (var area in exp.Areas)
                            {
                                //  if area token key matches step token key
                                if (area.Key == token.Key)
                                {
                                    //  if area has output paths
                                    if ((area.OutputPaths != null) && (area.OutputPaths.Count > 0))
                                    {
                                        foreach (var outputPath in area.OutputPaths)
                                            //TokenToBin(bytes, new Token((UInt16)outputPath.EndpointId, (outputPath.Value * token.Value) / 100), binType);
                                            stepTokens.Add(new Token((UInt16)outputPath.EndpointId, (outputPath.Value * token.Value) / 100));
                                        tokensSubstituted = true;
                                    }
                                    break;
                                }
                            }
                            if (!tokensSubstituted)
                                //TokenToBin(bytes, token, binType);
                                stepTokens.Add(token);
                        }
                        //  compress tokens and add to bin
                        Compress(stepTokens.ToArray(), bytes);
                    }
                    break;

                case PatternBinType.LedMatrixKeys:
                case PatternBinType.StepMethodDictionaryKeys:
                    {
                        //  add the dictionary step token
                        foreach (var token in step.Tokens)
                            TokenToBin(bytes, token, binType);
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Adds an expression repeat section start entry to bin.
        /// </summary>
        /// <param name="bytes">The binary being constructed.</param>
        /// <param name="repeats">The number of times to repeat a section, limited to 15.</param>
        private static void ExpressionRepeatStartToBin(List<byte> bytes, byte numRepeats)
        {
            //  validate inputs
            if (null == bytes)
                return;

            //  limit repeats to 15
            if (numRepeats > 15) numRepeats = 15;

            //  add the entry header and number of repeats
            bytes.Add((byte)(numRepeats | (byte)ECCONet.Token.KeyPrefix.PatternPrefix_PatternSectionStartWithRepeats));
        }

        /// <summary>
        /// Adds an expression repeat section end entry to bin.
        /// </summary>
        /// <param name="bytes">The binary being constructed.</param>
        /// <param name="repeats">The number of times to repeat a section, limited to 15.</param>
        private static void ExpressionRepeatEndToBin(List<byte> bytes)
        {
            //  validate inputs
            if (null == bytes)
                return;

            //  add the entry header
            bytes.Add((byte)ECCONet.Token.KeyPrefix.PatternPrefix_PatternSectionEnd);
        }

        /// <summary>
        /// Adds a nested expression entry to bin.
        /// </summary>
        /// <param name="bytes">The binary being constructed.</param>
        /// <param name="expressionEnum">The ECCONet expression enumeration.</param>
        /// <param name="numRepeats">The number of expression repeats, limited to 15, and zero = infinite.</param>
        /// <returns>An expression start entry.</returns>
        private static void NestedExpressionToBin(List<byte> bytes, UInt16 expressionEnum, byte numRepeats)
        {
            //  validate inputs
            if (null == bytes)
                return;

            //  limit repeats to 15
            if (numRepeats > 15) numRepeats = 15;

            //  add the entry header and number of repeats
            bytes.Add((byte)((numRepeats & 0xf0) | (int)ECCONet.Token.KeyPrefix.PatternPrefix_PatternStepWithRepeatsOfNestedPattern));
            bytes.Add((byte)(expressionEnum >> 8));
            bytes.Add((byte)(expressionEnum & 0xff));
        }
        #endregion


        /// <summary>
        /// Converts an expression collection to an ECCONet pattern bin file.
        /// </summary>
        /// <param name="ec">The expression collection to convert.</param>
        /// <returns>An ECCONet pattern bin file.</returns>
        public static byte[] ToPatternBinary(ExpressionCollection ec, PatternBinType binType)
        {
            //  create list of bytes
            List<byte> bytes = new List<byte>(4096);

            //  add file key in little-endian format
            bytes.Add((byte)(tokenPatternFileKey & 0xff));
            bytes.Add((byte)((tokenPatternFileKey >> 8) & 0xff));
            bytes.Add((byte)((tokenPatternFileKey >> 16) & 0xff));
            bytes.Add((byte)((tokenPatternFileKey >> 24) & 0xff));

            //  add number of patterns
            bytes.Add((byte)(ec.Expressions.Count >> 8));
            bytes.Add((byte)ec.Expressions.Count);

            //  for all expressions in collection
            foreach (Expression exp in ec.Expressions)
            {
                //  add expression start header with pattern enum and repeats
                ExpressionStartToBin(bytes, exp.ExpressionEnum, exp.Repeats, binType);

                //  add expression areas header with tokens
                ExpressionAreasToBin(bytes, exp.Areas, binType);

                //  for all expression entries
                foreach (Entry entry in exp.Entries)
                {
                    if (entry is Step step)
                        ExpressionStepToBin(bytes, step, exp, binType);
                    else if (entry is RepeatedSectionStart rss)
                        ExpressionRepeatStartToBin(bytes, rss.Repeats);
                    else if (entry is RepeatedSectionEnd rse)
                        ExpressionRepeatEndToBin(bytes);
                    else if (entry is NestedExpression ne)
                        NestedExpressionToBin(bytes, ne.ExpressionEnum, ne.Repeats);
                }
            }

            //  add null pattern here to end table
            ExpressionStartToBin(bytes, 0, 0, binType);

            //  return the array of bytes
            return bytes.ToArray();
        }
        #endregion


        /// <summary>
        /// Converts one or more tokens into a compressed byte stream.
        /// Note: Sorts the tokens in place, so token(s) must not be const array.
        /// </summary>
        /// <param name="tokens">A pointer to an array of one or more tokens.</param>
        /// <returns>Returns 0 on success, else -1.</returns>
        static int Compress(Token[] tokens, List<byte> bytes)
        {
            int token, sortToken, compareToken, lastToken;
            Token tempToken = new Token();
            Byte byteVal;
            Int32 value;
            UInt32 valueSize, bit;
            UInt32 numBinaryRepeats, numAnalogRepeats;
            UInt16 key;

            //	validate inputs
            if ((null == tokens) || (0 == tokens.Length))
                return -1;

            //	sort the tokens by key
            sortToken = 0;
            lastToken = tokens.Length;
            while (++sortToken < lastToken)
            {
                compareToken = sortToken;
                while (--compareToken >= 0)
                    if (tokens[compareToken].Key <= tokens[sortToken].Key)
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
                valueSize = ECCONet.Token.Key_ValueSize(tokens[token].Key);
                if (0 == valueSize)
                {
                    OutputToken(tokens[token], bytes);
                    ++token;
                    continue;
                }

                //	check for compressible series starting with current token
                numAnalogRepeats = 0;
                numBinaryRepeats = 0;
                value = tokens[token].Value;
                key = (UInt16)(tokens[token].Key + 1);
                compareToken = token + 1;
                while ((numAnalogRepeats < (LibConfig.MATRIX_MESSAGE_MAX_TOKEN_REPEATS - 1))
                    && (compareToken < lastToken) && (tokens[compareToken].Key == key)
                    && (ECCONet.Token.Key_ValueSize(tokens[compareToken].Key) == valueSize))
                {
                    //	get first non-zero value for binary repeat
                    if ((0 == value) && (0 != tokens[compareToken].Value))
                        value = tokens[compareToken].Value;

                    //	check for non-zero value match for binary repeat
                    if ((0 == tokens[compareToken].Value) || (tokens[compareToken].Value == value))
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
                    bytes.Add((Byte)(numBinaryRepeats | (Byte)ECCONet.Token.KeyPrefix.BinaryRepeat));
                    OutputTokenKey(tokens[token].Key, bytes);
                    OutputTokenValue((UInt32)value, (UInt16)valueSize, bytes);

                    //	for tokens in series
                    bit = 0;
                    byteVal = 0;
                    ++numBinaryRepeats;
                    while (0 != numBinaryRepeats--)
                    {
                        //	set non-zero value flag and bump token
                        if (0 != tokens[token].Value)
                            byteVal |= (Byte)(1 << (int)bit);
                        if (++bit >= 8)
                        {
                            bytes.Add(byteVal);
                            bit = 0;
                            byteVal = 0;
                        }
                        ++token;
                    }

                    //	if partially full byte, then send it
                    if (0 != bit)
                        bytes.Add(byteVal);
                }

                //	else if an analog series found
                else if (0 != numAnalogRepeats)
                {
                    //	send repeat and base token
                    bytes.Add((Byte)(numAnalogRepeats | (Byte)ECCONet.Token.KeyPrefix.AnalogRepeat));
                    OutputToken(tokens[token], bytes);
                    ++token;

                    //	for all others in series, send token value
                    while (0 != numAnalogRepeats--)
                    {
                        //	set value and bump token
                        OutputTokenValue((UInt32)tokens[token].Value, (UInt16)valueSize, bytes);
                        ++token;
                    }
                }

                //	else no compressible series starting with current token
                else
                {
                    //	send token and bump token index
                    OutputToken(tokens[token], bytes);
                    ++token;
                }
            }
            return 0;
        }


        /// <summary>
        /// Adds a token to the transmit fifo.
        /// </summary>
        /// <param name="token">A token to add.</param>
        static void OutputToken(Token token, List<byte> bytes)
        {
            UInt16 valueSize;

            bytes.Add((byte)((UInt16)token.Key >> 8));
            bytes.Add((byte)token.Key);
            valueSize = ECCONet.Token.Key_ValueSize(token.Key);
            OutputTokenValue((UInt32)token.Value, valueSize, bytes);
        }

        /// <summary>
        /// Adds a token key to the Matrix transmit fifo.
        /// </summary>
        /// <param name="key"></param>
        static void OutputTokenKey(UInt16 key, List<byte> bytes)
        {
            bytes.Add((byte)((UInt16)key >> 8));
            bytes.Add((byte)key);
        }

        /// <summary>
        /// Adds a token value to the Matrix transmit fifo.
        /// </summary>
        /// <param name="value">A token value to output.</param>
        /// <param name="valueSize">The value size in bytes.</param>
        static void OutputTokenValue(UInt32 value, UInt16 valueSize, List<byte> bytes)
        {
            while (0 != valueSize--)
                bytes.Add((Byte)((UInt32)(value >> (8 * valueSize))));
        }

    }
}
