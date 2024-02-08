/**
  ******************************************************************************
  * @file       EquationConverters.cs
  * @author     M. Latham, Liquid Logic
  * @version    1.0.0
  * @date       Dec 2017
  *
  * @brief      Class for converting plain text equations to and from
  *             the bytecode used in the ESG products' embedded controllers.
  *
  ******************************************************************************
  * @attention
  * Unless required by applicable law or agreed to in writing, software
  * created by Liquid Logic LLC is delivered "as is" without warranties
  * or conditions of any kind, either express or implied.
  *
  ******************************************************************************
  */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using LexicalCodes = ESG.BytecodeLib.Symbol.LexicalCodes;


namespace ESG.BytecodeLib
{
    public static partial class EquationConverters
    {
        //  bytecode file key
        const uint bytecodeFileKey = 0x1C3D5C47;

        //  preprocessor constants
        private const string define = "#define";





        /// <summary>
        /// Compiles the given plain text equations to time-logic bytecode.
        /// Will throw on syntax or missing label errors.
        /// </summary>
        /// <param name="text">The text to convert into bytecode.</param>
        /// <param name="status">The compiler result status text.</param>
        /// <returns>Returns a bytecode representation of the equations, or null if any errors were found.</returns>
        public static byte[] ToBytecode(string text, out string status)
        {
            return CompileBytecode(text, null, null, out status);
        }

        /// <summary>
        /// Compiles the given plain text equations to time-logic bytecode.
        /// Will throw on syntax or missing label errors.
        /// </summary>
        /// <param name="filename">The name of the file to process.</param>
        /// <param name="searchPaths">An optional list of file search paths. May be null.</param>
        /// <param name="status">The compiler result status text.</param>
        /// <returns>Returns a bytecode representation of the equations, or null if any errors were found.</returns>
        public static byte[] ToBytecode(string filename, List<string> searchPaths, out string status)
        {
            //  get the file text
            var text = GetFileText(filename, searchPaths);
            if (text == string.Empty)
                throw new Exception("The bytecode input file cannot be found.");

            //  convert text to bytecode
            return CompileBytecode(text, filename, searchPaths, out status);
        }

        /// <summary>
        /// Compiles the given plain text equations to time-logic bytecode.
        /// Will throw on syntax or missing label errors.
        /// </summary>
        /// <param name="text">The text to convert into bytecode.</param>
        /// <param name="filename">Optional file name, used for error reporting.  May be null.</param>
        /// <param name="searchPaths">Optional list of file search paths, used for #include statements.  May be null.</param>
        /// <param name="status">The compiler result status text.</param>
        /// <returns>Returns a bytecode representation of the equations, or null if any errors were found.</returns>
        private static byte[] CompileBytecode(string text, string filename, List<string> searchPaths, out string status)
        {
            //  create bytecode list
            var bytecode = new List<byte>(1000);

            //  add file key in little-endian format
            bytecode.Add((byte)(bytecodeFileKey & 0xff));
            bytecode.Add((byte)((bytecodeFileKey >> 8) & 0xff));
            bytecode.Add((byte)((bytecodeFileKey >> 16) & 0xff));
            bytecode.Add((byte)((bytecodeFileKey >> 24) & 0xff));

            //  reset error reporting
            ResetErrorReporting();

            try
            {
                //  preprocess the text
                var symbols = PreprocessText(text, null, filename, searchPaths);

                //  check the equation syntax
                EquationSyntaxAnalyzer.CheckSyntax(symbols, out int numEquations, out int maxOperandStackDepth, out int maxOperatorStackDepth);
                if ((status = ErrorReportingList) != string.Empty)
                    return null;

                //  for all symbols
                for (int i = 0; i < symbols.Count; ++i)
                {
                    //  get next symbol
                    var symbol = symbols[i];

                    //  switch on symbol code
                    switch (symbol.Code)
                    {
                        //  if a token key
                        case LexicalCodes.TokenKey:
                            //  add code
                            bytecode.Add((byte)symbol.Code);

                            //  add big endian key value
                            bytecode.AddRange(symbols[++i].UInt16ValueBytesBigEndian);

                            //  get token close or token address
                            symbol = symbols[++i];

                            //  if token address
                            if (symbol.Code == LexicalCodes.TokenAddress)
                            {
                                //  add code
                                bytecode.Add((byte)symbol.Code);

                                //  add address
                                bytecode.Add(symbols[++i].ByteValue);

                                //  advance to token close
                                ++i;
                            }
                            break;
                        
                        //  if a pre-equation 8-bit value
                        case LexicalCodes.Value8:
                            //  add byte value
                            bytecode.Add(symbol.ByteValue);
                            break;

                        //  if a pre-equation 16-bit value
                        case LexicalCodes.Value16:
                            //  add 16-bit value
                            bytecode.AddRange(symbol.UInt16ValueBytesLittleEndian);
                            break;

                        //  if a pre-equation 32-bit value
                        case LexicalCodes.Value32:
                            //  add 32-bit value
                            bytecode.AddRange(symbol.IntValueBytesLittleEndian);
                            break;

                        //  if a constant value
                        case LexicalCodes.Value:
                            //  add code
                            bytecode.Add((byte)LexicalCodes.Value);

                            //  add big endian key value
                            bytecode.AddRange(symbol.IntValueBytesBigEndian);
                            break;

                        default:
                            //  add code
                            bytecode.Add((byte)symbol.Code);
                            break;
                    }
                }

                //  get number of unique tokens
                int numUniqueTokens = GetNumUniqueTokens(symbols);

                //  check error reporting
                if (ErrorReportingList == string.Empty)
                {
                    //  get status
                    status = "Bytecode compiled successfully.\n\n";
                    status += string.Format("Number of equations: {0}\n\n", numEquations);
                    status += string.Format("Number of unique tokens: {0}\n\n", numUniqueTokens);
                    status += string.Format("Max operand stack depth: {0}\n", maxOperandStackDepth);
                    status += string.Format("Max operator stack depth: {0}\n\n", maxOperatorStackDepth);
                    status += FromBytecode(bytecode.ToArray());
                }
                else
                {
                    status = ErrorReportingList;
                }
            }
            catch
            {
                //  flag error and must abort
                if (ErrorReportingList == string.Empty)
                    ErrorReportingList += "Unknown error compiling bytecode.";
                status = ErrorReportingList;
            }

            //  return the bytecode
            if (ErrorReportingList != string.Empty)
                return null;
            return bytecode.ToArray();
        }

        /// <summary>
        /// Converts the given time-logic bytecode to plain text equations.
        /// </summary>
        /// <param name="bytecode">The bytecode to convert.</param>
        /// <returns>Returns the equations.</returns>
        public static string FromBytecode(byte[] bytecode)
        {
            return FromBytecode(bytecode, false, false);
        }

        /// <summary>
        /// Converts the given time-logic bytecode to plain text equations.
        /// </summary>
        /// <param name="bytecode">The bytecode to convert.</param>
        /// <param name="hexConstants">True to create constant value strings in hexadecimal format.</param>
        /// <param name="hexTokenKeys">True to create token key strings in hexadecimal format.</param>
        /// <returns>Returns the equations.</returns>
        public static string FromBytecode(byte[] bytecode, bool hexConstants, bool hexTokenKeys)
        {
            var output = string.Empty;
            int index = 0;

            //  validate bytecode
            if ((bytecode == null) || (bytecode.Length < 6))
                throw new Exception("Bytecode file does not contain at least 6 bytes");

            //  skip security key
            index = 4;

            //  check for initial constants
            int constLength = 0;
            if ((bytecode[4] == 0xca) && (bytecode[5] == 0xfe))
            {
                //  length of initial constants, always in 4-byte sizes
                constLength = bytecode[6] | (bytecode[7] << 8);
                if ((constLength & 0x03) != 0)
                    throw new Exception("Bytecode initial constant length not in 4-byte increments");
                else
                {
                    output += string.Format("Initial constants, length 0x{0:X4}\n", constLength);
                    output += "Non-packed little-endian format\n";
                    output += "========================\n";
                }

                //  for all bytecode after security key
                var lastConst = constLength + 8;
                for (index = 8; index < lastConst; index += 4)
                    output += string.Format("0x{0:X2} 0x{1:X2} 0x{2:X2} 0x{3:X2}\n",
                        bytecode[index], bytecode[index + 1], bytecode[index + 2], bytecode[index + 3]);
            }
            else
            {
                output += "No initial constants table.\n";
            }

            //  for all bytecode after security key
            output += "\nEquations\n========================";
            for (; index < bytecode.Length; ++index)
            {
                switch ((LexicalCodes)bytecode[index])
                {
                    case LexicalCodes.Value:
                        try
                        {
                            //  read constant big endian
                            int value = (int)bytecode[index + 1] << 24;
                            value |= ((int)bytecode[index + 2] << 16);
                            value |= ((int)bytecode[index + 3] << 8);
                            value |= bytecode[index + 4];
                            if (hexConstants)
                                output += ("0x" + value.ToString("x8"));
                            else
                                output += value.ToString();
                            index += 4;
                        }
                        catch
                        {
                            throw new Exception("Error parsing bytecode constant value");
                        }
                        break;

                    case LexicalCodes.TokenKey:
                        try
                        {
                            //  add open bracket
                            output += "[";
                            ++index;

                            //  read token key big endian
                            UInt16 value = (UInt16)((int)bytecode[index] << 8);
                            value |= bytecode[index + 1];
                            if (hexTokenKeys)
                                output += ("0x" + value.ToString("x4"));
                            else
                                output += value.ToString();
                            index += 2;

                            //  if token address
                            if ((LexicalCodes)bytecode[index] == LexicalCodes.TokenAddress)
                            {
                                output += Symbol.SymbolLexicon.FirstOrDefault(x => x.Value == LexicalCodes.TokenAddress).Key;
                                ++index;

                                //  read token address
                                if (hexTokenKeys)
                                    output += ("0x" + bytecode[index].ToString("x2"));
                                else
                                    output += bytecode[index].ToString();
                            }
                            output += "]";

                            //  bump back to last code
                            --index;
                        }
                        catch
                        {
                            throw new Exception("Error parsing bytecode token key");
                        }
                        break;

                    case LexicalCodes.EquationStart:
                        output += "\n$";
                        break;

                    case LexicalCodes.PriorityEquationStart:
                        output += "\n$$";
                        break;

                    case LexicalCodes.SuccessiveEquationStart:
                        output += "\n+$";
                        break;

                    default:
                        output += Symbol.SymbolLexicon.FirstOrDefault(x => x.Value == (LexicalCodes)bytecode[index]).Key;
                        break;
                }
            }

            //  return equation
            if (output.StartsWith("\n"))
                output = output.Substring(1);
            return output;
        }

        /// <summary>
        /// The error reporting file name.
        /// </summary>
        public static string ErrorReportingFilename = string.Empty;

        /// <summary>
        /// The error reporting line number.
        /// </summary>
        public static int ErrorReportingLineNumber = -1;

        /// <summary>
        /// The error list.
        /// </summary>
        public static string ErrorReportingList = string.Empty;

        /// <summary>
        /// The number of errors reported.
        /// </summary>
        public static int ErrorReportingNumErrors = 0;

        /// <summary>
        /// Resets the error reporting.
        /// </summary>
        public static void ResetErrorReporting()
        {
            ErrorReportingNumErrors = 0;
            ErrorReportingList = string.Empty;
        }


        /// <summary>
        /// Adds an error string.
        /// </summary>
        /// <param name="error">The error description.</param>
        /// <param name="abort">True to abort and throw new error.</param>
        /// <returns>The integer zero.</returns>
        public static int AddError(string error, bool abort)
        {
            //  add the error using static file name and line number
            return AddError(error, ErrorReportingFilename, ErrorReportingLineNumber, abort);
        }

        /// <summary>
        /// Adds an error string.
        /// </summary>
        /// <param name="symbol">The symbol related to the error.</param>
        /// <param name="error">The error description.</param>
        /// <param name="abort">True to abort and throw new error.</param>
        /// <returns>The integer zero.</returns>
        public static int AddError(Symbol symbol, string error, bool abort)
        {
            //  add the error using static file name and line number
            return AddError(error, symbol.FileName, symbol.LineNumber, abort);
        }

        /// <summary>
        /// Adds an error string.
        /// </summary>
        /// <param name="error">The error description.</param>
        /// <param name="fileName">The filename.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="abort">True to abort and throw new error.</param>
        /// <returns>The integer zero.</returns>
        public static int AddError(string error, string fileName, int lineNumber, bool abort)
        {
            //  compose error string
            string errorStr = string.Empty;

            if ((error == null) || (error == string.Empty))
                errorStr = "Unknown error occcurred.\n";

            if ((fileName != null) && (fileName != string.Empty))
                errorStr = string.Format("At {0} line {1}: {2}.\n", Path.GetFileName(fileName), lineNumber, error);
            else
                errorStr = string.Format("At line {0}: {1}.\n", lineNumber, error);

            //  add error to list
            ErrorReportingList += errorStr;
            if (abort)
                throw new Exception(string.Empty);
            else if (++ErrorReportingNumErrors > 30)
            {
                ErrorReportingList += "Too many errors, aborting.\n";
                throw new Exception(string.Empty);
            }
            return 0;
        }


    }
}


#if UNUSED_CODE

        /// <summary>
        /// Trys to resolve all equations within token key brackets.  Will throw if not resolvable.
        /// </summary>
        /// <param name="text">The text in which to resolve token key equations.</param>
        /// <returns>A text with token key equations resolved.</returns>
        private static string ResolveTokenKeyEquations(string text)
        {
            //  get copy of text
            var str = string.Copy(text);

            //  convert all token key expressions to numbers
            int lineNumber = 1;
            for (int index = 0; index < str.Length; ++index)
            {
                //  track line number
                if (str[index] == '\n')
                    ++lineNumber;

                //  ignore white space
                if (Char.IsWhiteSpace(str[index]))
                    continue;

                //  if a token start
                if (str[index] == '[')
                {
                    //  get token key string
                    int endIndex = str.IndexOf(']', index);
                    if (endIndex == -1)
                        throw new Exception(string.Format("Token key syntax error on line {0}.", lineNumber));
                    var tokenKeyString = str.Substring(index + 1, endIndex - (index + 1));

                    //  get symbols
                    var symbols = Symbol.TextToSymbols(tokenKeyString, lineNumber);

                    //  if multiple symbols found, convert to int result and replace token key string
                    if (symbols.Count > 1)
                    {
                        //  perform calculation
                        int value = SymbolCalculator.PerformCalculation(symbols);

                        //  replace key word
                        tokenKeyString = value.ToString();
                        str = str.Remove(index + 1, endIndex - (index + 1));
                        str = str.Insert(index + 1, tokenKeyString);
                    }

                    //  bump index to ']' char
                    index += (1 + tokenKeyString.Length);
                }
            }

            //  return the text with token key equations resolved
            return str;
        }

        private class LexicalToken
        {
            /// <summary>
            /// The text characters.
            /// </summary>
            string Chars;

            /// <summary>
            /// The lexical code.
            /// </summary>
            int Code;

            /// <summary>
            /// The priority, if this is an operator.
            /// </summary>
            int OperatorPriority;

            /// <summary>
            /// The value, if this is a constant value.
            /// </summary>
            Int32 Value;




        }



        /// <summary>
        /// MTL token private class.
        /// </summary>
        private class MtlToken
        {
            //	this field is for the time-logic equation processor
            //	added here because there was a spare byte in token
            byte flags;

            //	For incoming tokens, the CAN address of the sender.
            //	For outgoing tokens, the CAN address of the recipient.
            byte address;

            //	the private or ESG public key
            UInt16 key;

            //	The value associated with the key.
            Int32 value;
            
            //	the zero-nonzero timestamp
            //	this value is never zero for output tokens
            UInt32 timestamp;
        }


        /// <summary>
        /// Converts given text to signed integer value.  Throws on parsing error.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>Returns the value.</returns>
        private static int TextToInt(string text)
        {
            int value = 0;
            if ((text[0] == '0') && ((text[1] == 'x') || (text[1] == 'X')))
            {
                value = (int)UInt32.Parse(text.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            }
            else
            {
                value = Int32.Parse(text);
            }
            return value;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="equations"></param>
        /// <param name="index"></param>
        /// <param name="textSub"></param>
        private static void ParseDefine(string equations, ref int index, Dictionary<string, string> textSub)
        {
            //  get label and bump pointer
            index += define.Length;
            while (Char.IsWhiteSpace(equations[index])) { ++index; }
            var key = new string(equations.Substring(index).TakeWhile(c => (Char.IsLetterOrDigit(c) || (c == '_'))).ToArray());
            index += key.Length;

            //  get remaining line
            int endIndex = equations.IndexOf('\n');
            var line = equations.Substring(index, endIndex - index);
            index = endIndex;


            while (Char.IsWhiteSpace(equations[index])) { ++index; }
            var value = new string(equations.Substring(index).TakeWhile(c => Char.IsLetterOrDigit(c)).ToArray());
            index += value.Length;
            textSub.Add(key, value);

        }




        /// <summary>
        /// Parse for number.  Will throw on error.
        /// </summary>
        /// <param name="equations">The text from which to parse.</param>
        /// <param name="index">The index within the text.</param>
        /// <param name="textSub">Text substitutions.</param>
        /// <returns></returns>
        private static int ParseInt(string equations, ref int index, Dictionary<string, string> textSub)
        {
            string digits = string.Empty;
            int value;

            //  skip leading whitespace
            while (Char.IsWhiteSpace(equations[index])) { ++index; }

            //  if starts with letter or underscore, make substitution
            if (Char.IsLetter(equations[index]) || (equations[index] == '_'))
            {
                //  get substitution
                var label = new string(equations.Substring(index).TakeWhile(c => (Char.IsLetterOrDigit(c) || (c == '_'))).ToArray());
                digits = textSub[label];
            }
            //  else get number 
            else
            {
                digits = new string(equations.Substring(index).TakeWhile(c => (Char.IsLetterOrDigit(c) || (c == '-'))).ToArray());
            }
            index += digits.Length;

            //  parse for number
            if ((digits[0] == '0') && ((digits[1] == 'x') || (digits[1] == 'X')))
            {
                value = Int32.Parse(digits.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            }
            else
            {
                value = Int32.Parse(digits);
            }
            return value;
        }


        /// <summary>
        /// Makes word symbol text substitutions for #defines and enums,
        /// and removes the #defines and enums from the symbol list.
        /// </summary>
        /// <param name="symbols"></param>
        private static void MakeWordSubstitutions(List<Symbol> symbols)
        {
            var textSub = new Dictionary<string, string>();

            for (int i = 0; i < symbols.Count; ++i)
            {
                //  get next symbol
                var symbol = symbols[i];

                //  switch on symbol code
                switch (symbol.Code)
                {
                    //  preprocessor #define
                    case LexicalCodes.PreprocessorDefine:
                        try
                        {
                            //  get list of symbols on same line
                            var defineSymbols = new List<Symbol>(5);





                            //  get label
                            index += Lexicon.FirstOrDefault(x => x.Value == lexCode).Key.Length;
                            while (Char.IsWhiteSpace(equations[index])) { ++index; }
                            var key = new string(equations.Substring(index).TakeWhile(c => (Char.IsLetterOrDigit(c) || (c == '_'))).ToArray());


                            index += key.Length;
                            while (Char.IsWhiteSpace(equations[index])) { ++index; }
                            var value = new string(equations.Substring(index).TakeWhile(c => Char.IsLetterOrDigit(c)).ToArray());
                            index += value.Length;
                            textSub.Add(key, value);
                        }
                        catch
                        {
                            throw new Exception("Equations file ends with unclosed comment.");
                        }
                        break;
                }

            }
        }
        /// <summary>
        /// Converts the given plain text equations to time-logic bytecode.
        /// </summary>
        /// <param name="text">The plain text equations to convert.</param>
        /// <returns>Returns a bytecode representation of the equations.</returns>
        public static byte[] ToBytecode(string text)
        {
            var output = new List<byte>(1000);
            var textSub = new Dictionary<string, string>();

            //  preprocess the text
            var symbols = PreprocessText(new List<string>() { text });

            //  for all symbols
            for (int i = 0; i < symbols.Count; ++i)
            {
                //  get next symbol
                var symbol = symbols[i];

                //  switch on symbol code
                switch (symbol.Code)
                {
                    //  preprocessor #define
                    case LexicalCodes.PreprocessorDefine:
                        try
                        {
                            //  get label
                            index += Lexicon.FirstOrDefault(x => x.Value == lexCode).Key.Length;
                            while (Char.IsWhiteSpace(equations[index])) { ++index; }
                            var key = new string(equations.Substring(index).TakeWhile(c => (Char.IsLetterOrDigit(c) || (c == '_'))).ToArray());


                            index += key.Length;
                            while (Char.IsWhiteSpace(equations[index])) { ++index; }
                            var value = new string(equations.Substring(index).TakeWhile(c => Char.IsLetterOrDigit(c)).ToArray());
                            index += value.Length;
                            textSub.Add(key, value);
                        }
                        catch
                        {
                            throw new Exception("Equations file ends with unclosed comment.");
                        }
                        break;


                    //  ignore single line comments
                    case LexicalCodes.SingleLineComment:
                        singleLineComment = true;
                        break;

                    //  ignore area line comments
                    case LexicalCodes.CommentStart:
                        areaComment = true;
                        break;

                    //  if no code, is possibly a constant
                    case LexicalCodes.None:
                        try
                        {
                            //  get the int value
                            int constant = ParseInt(equations, ref index, textSub);

                            //  add constant number big endian
                            output.Add((byte)LexicalCodes.ConstantValue);
                            output.Add((byte)(constant >> 24));
                            output.Add((byte)(constant >> 16));
                            output.Add((byte)(constant >> 8));
                            output.Add((byte)constant);
                        }
                        catch
                        {
                            throw new Exception(string.Format("Equations has error in constant value, line {0}.", lineNumber));
                        }
                        break;

                    case LexicalCodes.OperatorOpenTokenKey:
                        try
                        {
                            //  get int value and check bounds
                            ++index;
                            int key = ParseInt(equations, ref index, textSub);
                            if ((key < UInt16.MinValue) || (key > UInt16.MaxValue))
                                throw new Exception();

                            //  verify close token character
                            if (equations[index] != ']')
                                throw new Exception();

                            //  add key big endian
                            output.Add((byte)LexicalCodes.OperatorOpenTokenKey);
                            output.Add((byte)(key >> 8));
                            output.Add((byte)key);
                        }
                        catch
                        {
                            throw new Exception(string.Format("Equations has error in token key, line {0}.", lineNumber));
                        }
                        break;

                    default:
                        output.Add((byte)lexCode);
                        break;
                }

            }
            //  return the bytecode
            return output.ToArray();
        }

#endif

