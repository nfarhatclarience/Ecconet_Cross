/**
  ******************************************************************************
  * @file       EquationConverters.cs
  * @copyright  © 2017 ECCO Group.  All rights reserved.
  * @author     M. Latham, Liquid Logic
  * @version    1.0.0
  * @date       12/1/2017
  *
  * @brief      Symbol class for converting plain text equations to and from
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
using System.Text;
using System.Threading.Tasks;
using System.Globalization;



namespace ESG.BytecodeLib
{
    /// <summary>
    /// A symbol class to represent lexical expressions.
    /// </summary>
    public class Symbol
    {
        /// <summary>
        /// Lexical token codes, used to represent identifiers, keywords, literals, operators, and punctuators.
        /// </summary>
        public enum LexicalCodes : byte
        {
            None,
            Value,      //  signed 32-bit in equations
            Value8,     //  const 8-bit unsigned value before equations start
            Value16,    //  const 16-bit unsigned value before equations start
            Value32,    //  const 32-bit unsigned value before equations start
            String,

            //  equation and token delineation
            EquationStart,
            PriorityEquationStart,
            SuccessiveEquationStart,
            EquationEnd,
            Equals,
            Lambda,
            TokenKey,
            TokenKeyClose,
            TokenAddress,

            //  math and logic operators
            OperatorLogicalNot,
            OperatorBitwiseInvert,
            OperatorMultiply,
            OperatorDivide,
            OperatorModulus,
            OperatorAdd,
            OperatorSubtract,
            OperatorShiftLeft,
            OperatorShiftRight,
            OperatorIsLessThan,
            OperatorIsLessThanOrEqual,
            OperatorIsGreaterThan,
            OperatorIsGreaterThanOrEqual,
            OperatorIsEqual,
            OperatorIsNotEqual,
            OperatorBitwiseAnd,
            OperatorBitwiseXor,
            OperatorBitwiseOr,
            OperatorLogicalAnd,
            OperatorLogicalOr,
            OperatorConditionalQuestion,
            OperatorConditionalSeparator,
            OperatorOpenParentheses,
            OperatorCloseParentheses,

            //  logic functions
            OutputLogicActivityMonitor,
            OutputLogicRisingEdgeUpCounter,
            OutputLogicFallingEdgeUpCounter,
            OutputLogicRisingEdgeToggle,
            OutputLogicFallingEdgeToggle,
            OutputLogicRisingEdgeSkipToggle,
            OutputLogicFallingEdgeSkipToggle,
            OutputLogicRisingEdgeVariableClear,
            OutputLogicFallingEdgeVariableClear,
            OutputLogicRisingEdgeDelay,
            OutputLogicFallingEdgeDelay,

            //  token output options
            OutputSendTokenOnChange,
            OutputSendTokenOnOutputRisingEdge,
            OutputSendTokenOnOutputFallingEdge,
            OutputSendTokenOnOutputRisingByValue,
            OutputSendTokenOnOutputFallingByValue,

            //  preprocessor symbols
            PpHash,
            OpenBrace,
            CloseBrace,
            Comma,

            //  C-language keywords
            PpInclude,
            PpDefine,
            PpUndef,
            PpIfdef,
            PpIfndef,
            PpIf,
            PpElif,
            PpElse,
            PpEndif,
            Typedef,
            Struct,
            Enum,
            Bool,
            UInt8,
            UInt16,
            UInt32,
            Int8,
            Int16,
            Int32,
            Int,
            Short,
            Unsigned,
            Extern,
        }

        /// <summary>
        /// Symbol lexical token to code conversion dictionary.
        /// </summary>
        public static Dictionary<string, LexicalCodes> SymbolLexicon = new Dictionary<string, LexicalCodes>()
        {
            //  values
            { "V8",          LexicalCodes.Value8 },
            { "V16",         LexicalCodes.Value16 },
            { "V32",         LexicalCodes.Value32 },

            //  equation and token delineation
            { "$",          LexicalCodes.EquationStart },
            { "$$",         LexicalCodes.PriorityEquationStart },
            { "+$",         LexicalCodes.SuccessiveEquationStart },
            { ";",          LexicalCodes.EquationEnd },
            { "=",          LexicalCodes.Equals },
            { "=>",         LexicalCodes.Lambda },
            { "[",          LexicalCodes.TokenKey },
            { "]",          LexicalCodes.TokenKeyClose },
            { "-a",         LexicalCodes.TokenAddress },

            //  math and logical operators
            { "!",          LexicalCodes.OperatorLogicalNot },
            { "~",          LexicalCodes.OperatorBitwiseInvert },
            { "*",          LexicalCodes.OperatorMultiply },
            { "/",          LexicalCodes.OperatorDivide },
            { "%",          LexicalCodes.OperatorModulus },
            { "+",          LexicalCodes.OperatorAdd },
            { "-",          LexicalCodes.OperatorSubtract },
            { "<<",         LexicalCodes.OperatorShiftLeft },
            { ">>",         LexicalCodes.OperatorShiftRight },
            { "<",          LexicalCodes.OperatorIsLessThan },
            { "<=",         LexicalCodes.OperatorIsLessThanOrEqual },
            { ">",          LexicalCodes.OperatorIsGreaterThan },
            { ">=",         LexicalCodes.OperatorIsGreaterThanOrEqual },
            { "==",         LexicalCodes.OperatorIsEqual },
            { "!=",         LexicalCodes.OperatorIsNotEqual },
            { "&",          LexicalCodes.OperatorBitwiseAnd },
            { "^",          LexicalCodes.OperatorBitwiseXor },
            { "|",          LexicalCodes.OperatorBitwiseOr },
            { "&&",         LexicalCodes.OperatorLogicalAnd },
            { "||",         LexicalCodes.OperatorLogicalOr },
            { "?",          LexicalCodes.OperatorConditionalQuestion },
            { ":",          LexicalCodes.OperatorConditionalSeparator },
            { "(",          LexicalCodes.OperatorOpenParentheses },
            { ")",          LexicalCodes.OperatorCloseParentheses },

            //  logic functions
            { "-am",        LexicalCodes.OutputLogicActivityMonitor },
            { "-rctr",      LexicalCodes.OutputLogicRisingEdgeUpCounter },
            { "-fctr",      LexicalCodes.OutputLogicFallingEdgeUpCounter },
            { "-rt",        LexicalCodes.OutputLogicRisingEdgeToggle },
            { "-ft",        LexicalCodes.OutputLogicFallingEdgeToggle },
            { "-rskt",      LexicalCodes.OutputLogicRisingEdgeSkipToggle },
            { "-fskt",      LexicalCodes.OutputLogicFallingEdgeSkipToggle },
            { "-rc",        LexicalCodes.OutputLogicRisingEdgeVariableClear },
            { "-fc",        LexicalCodes.OutputLogicFallingEdgeVariableClear },
            { "-rd",        LexicalCodes.OutputLogicRisingEdgeDelay },
            { "-fd",        LexicalCodes.OutputLogicFallingEdgeDelay },

            //  token output options
            { "-c@",        LexicalCodes.OutputSendTokenOnChange },
            { "-r@",        LexicalCodes.OutputSendTokenOnOutputRisingEdge },
            { "-f@",        LexicalCodes.OutputSendTokenOnOutputFallingEdge },
            { "-rv@",       LexicalCodes.OutputSendTokenOnOutputRisingByValue },
            { "-fv@",       LexicalCodes.OutputSendTokenOnOutputFallingByValue },

            //  preprocessor
            { "#",          LexicalCodes.PpHash },
            { "{",          LexicalCodes.OpenBrace },
            { "}",          LexicalCodes.CloseBrace },
            { ",",          LexicalCodes.Comma },
        };

        /// <summary>
        /// Keyword lexical token to code conversion dictionary.
        /// </summary>
        public static Dictionary<string, LexicalCodes> KeywordLexicon = new Dictionary<string, LexicalCodes>()
        {
            { "include",    LexicalCodes.PpInclude },
            { "define",     LexicalCodes.PpDefine },
            { "undef",      LexicalCodes.PpUndef },
            { "ifdef",      LexicalCodes.PpIfdef },
            { "ifndef",     LexicalCodes.PpIfndef },
            { "if",         LexicalCodes.PpIf },
            { "elif",       LexicalCodes.PpElif },
            { "else",       LexicalCodes.PpElse },
            { "endif",      LexicalCodes.PpEndif },
            { "typedef",    LexicalCodes.Typedef },
            { "struct",     LexicalCodes.Struct },
            { "enum",       LexicalCodes.Enum },
            { "bool",       LexicalCodes.Bool },
            { "BOOL",       LexicalCodes.Bool },
            { "uint8_t",    LexicalCodes.UInt8 },
            { "uint16_t",   LexicalCodes.UInt16 },
            { "uint32_t",   LexicalCodes.UInt32 },
            { "int8_t",     LexicalCodes.Int8 },
            { "int16_t",    LexicalCodes.Int16 },
            { "int32_t",    LexicalCodes.Int32 },
            { "int",        LexicalCodes.Int },
            { "short",      LexicalCodes.Short },
            { "unsigned",   LexicalCodes.Unsigned },
            { "extern",     LexicalCodes.Extern },
        };


        /// <summary>
        /// The token code.
        /// </summary>
        public LexicalCodes Code = LexicalCodes.Value;

        /// <summary>
        /// The text expression associated with the symbol.
        /// </summary>
        public string Word = string.Empty;

        /// <summary>
        /// The file name where the symbol exists.
        /// </summary>
        public string FileName = string.Empty;

        /// <summary>
        /// The file line number where the symbol exists.
        /// </summary>
        public int LineNumber = 0;

        /// <summary>
        /// The integer value of the word.
        /// Accessing this property on non-valid integer representations will throw an error.
        /// </summary>
        public int IntValue
        {
            get
            {
                try
                {
                    //  parse for number
                    if (Word.StartsWith("0x") || Word.StartsWith("0X"))
                    {
                        return Int32.Parse(Word.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                    }
                    return Int32.Parse(Word);
                }
                catch
                {
                    EquationConverters.AddError(string.Format("Unable to resolve \"{0}\"", Word), false);
                    return 0;
                }
            }
        }

        /// <summary>
        /// The symbol int value as four little-endian bytes.
        /// </summary>
        public byte[] IntValueBytesLittleEndian
        {
            get
            {
                var value = IntValue;
                return new Byte[4]
                {
                    (byte)value,
                    (byte)(value >> 8),
                    (byte)(value >> 16),
                    (byte)(value >> 24)
                };
            }
        }

        /// <summary>
        /// The symbol int value as four big-endian bytes.
        /// </summary>
        public byte[] IntValueBytesBigEndian
        {
            get
            {
                var value = IntValue;
                return new Byte[4]
                {
                    (byte)(value >> 24),
                    (byte)(value >> 16),
                    (byte)(value >> 8),
                    (byte)value
                };
            }
        }

        /// <summary>
        /// The UInt16 value of the word.
        /// Accessing this property on non-valid UInt16 representations will throw an error.
        /// </summary>
        public UInt16 UInt16Value
        {
            get
            {
                try
                {
                    //  parse for number
                    if (Word.StartsWith("0x") || Word.StartsWith("0X"))
                    {
                        return UInt16.Parse(Word.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                    }
                    return UInt16.Parse(Word);
                }
                catch
                {
                    EquationConverters.AddError(string.Format("Unable to resolve \"{0}\"", Word), false);
                    return 0;
                }
            }
        }

        /// <summary>
        /// The symbol UInt16 value as two little-endian bytes.
        /// </summary>
        public byte[] UInt16ValueBytesLittleEndian
        {
            get
            {
                var value = UInt16Value;
                return new Byte[2]
                {
                    (byte)value,
                    (byte)(value >> 8)
                };
            }
        }

        /// <summary>
        /// The symbol UInt16 value as two big-endian bytes.
        /// </summary>
        public byte[] UInt16ValueBytesBigEndian
        {
            get
            {
                var value = UInt16Value;
                return new Byte[2]
                {
                    (byte)(value >> 8),
                    (byte)value
                };
            }
        }

        /// <summary>
        /// The byte value of the word.
        /// Accessing this property on non-valid byte representations will throw an error.
        /// </summary>
        public byte ByteValue
        {
            get
            {
                try
                {
                    //  parse for number
                    if (Word.StartsWith("0x") || Word.StartsWith("0X"))
                    {
                        return byte.Parse(Word.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                    }
                    return byte.Parse(Word);
                }
                catch
                {
                    EquationConverters.AddError(string.Format("Unable to resolve \"{0}\"", Word), false);
                    return 0;
                }
            }
        }

        /// <summary>
        /// Returns a value indicating whether the word is a label.
        /// </summary>
        public bool IsLabel
        {
            get
            {
                if (Word == string.Empty)
                    return false;

                bool firstChar = false;
                foreach (var c in Word)
                {
                    if (!firstChar)
                    {
                        firstChar = true;
                        if (!(Char.IsLetter(c) || (c == '_')))
                            return false;
                    }
                    else
                    {
                        if (!(Char.IsLetterOrDigit(c) || (c == '_')))
                            return false;
                    }
                }
                return true;
            }
        }


        /// <summary>
        /// Converts C-style text to list of symbols.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <param name="filename">The name of the file, used for error reporting.</param>
        /// <param name="lineNumber">The starting line number of the text, used for error reporting.</param>
        /// <returns>A list of symbols.</returns>
        public static List<Symbol> TextToSymbols(string text, string filename, int lineNumber)
        {
            var symbols = new List<Symbol>(1000);
            var label = string.Empty;
            var word = string.Empty;
            EquationConverters.ErrorReportingLineNumber = lineNumber;


            //  convert text into lines
            var lines = new List<string>(text.Split('\n'));

            //  for all lines of source code
            foreach (var line in lines)
            {
                int index = 0;
                int endIndex = line.Length;
                while (index < line.Length)
                {
                    //  search for start of string
                    endIndex = line.IndexOf('\"', index);
                    if (endIndex == -1)
                        endIndex = line.Length;

                    //  if have non-quote text, then convert to symbols
                    if ((endIndex - index) > 1)
                        symbols.AddRange(ToSymbols(line.Substring(index, endIndex - index), filename, lineNumber));
                    index = endIndex;

                    //  if start of string found
                    if (endIndex != line.Length)
                    {
                        //  search for end of string
                        do
                        {
                            endIndex = line.IndexOf("\"", endIndex + 1);
                            if (endIndex == -1)
                                EquationConverters.AddError("File ends with unclosed quote", false);

                        } while (line[endIndex - 1] == '\\');
                        
                        //  add string symbol
                        ++index;
                        if (endIndex > index)
                        {
                            var quoted = line.Substring(index, endIndex - index);
                            symbols.Add(new Symbol() { Code = LexicalCodes.String, Word = quoted, FileName = filename, LineNumber = lineNumber });
                        }
                        index = endIndex + 1;
                    }
                }

                //  bump the line number
                ++lineNumber;
            }

            //  return list of symbols
            return symbols;
        }

        /// <summary>
        /// Converts text into symbols.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <param name="filename">The file name, used for error reporting.</param>
        /// <param name="lineNumber">The line number, used for error reporting.</param>
        /// <returns></returns>
        private static List<Symbol> ToSymbols(string text, string filename, int lineNumber)
        {
            //  put whitespace around text symbols
            for (int index = 0; index < text.Length; ++index)
            {
                //  try to find largest lexical symbol string
                var code = LexicalCodes.None;
                for (int n = 5; n > 0; --n)
                {
                    //  check for remaining characters
                    if ((text.Length - index) < n)
                        continue;

                    //  if found, then surround with whitespace
                    if (SymbolLexicon.TryGetValue(text.Substring(index, n), out code))
                    {
                        text = text.Insert(index, " ");
                        text = text.Insert(index + 1 + n, " ");
                        index += (n + 1);
                        break;
                    }
                }
            }

            //  separate text into array of strings
            var strings = text.Split(new char[] { ' ', '\t', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            //  convert text to symbols
            var symbols = new List<Symbol>(10);
            foreach (var str in strings)
            {
                var code = LexicalCodes.None;
                if (SymbolLexicon.TryGetValue(str, out code) || KeywordLexicon.TryGetValue(str, out code))
                    symbols.Add(new Symbol() { Code = code, Word = str, FileName = filename, LineNumber = lineNumber });
                else
                    symbols.Add(new Symbol() { Code = LexicalCodes.Value, Word = str, FileName = filename, LineNumber = lineNumber });
            }
            return symbols;
        }

    }
}


#if PREV_CODE

        /// <summary>
        /// Converts C-style text to list of symbols.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <param name="filename">The name of the file, used for error reporting.</param>
        /// <param name="lineNumber">The starting line number of the text, used for error reporting.</param>
        /// <returns>A list of symbols.</returns>
        public static List<Symbol> TextToSymbols(string text, string filename, int lineNumber)
        {
            var symbols = new List<Symbol>(1000);
            var label = string.Empty;
            var word = string.Empty;
            EquationConverters.ErrorReportingLineNumber = lineNumber;

            //  for all characters in text
            for (int index = 0; index < text.Length; ++index)
            {
                //  current character
                var currentChar = text[index];

                //  track line number
                if (currentChar == '\n')
                {
                    ++lineNumber;
                    EquationConverters.ErrorReportingLineNumber = lineNumber;
                }

                var code = LexicalCodes.None;
                bool delimiter = false;

                //  if is white space
                if (Char.IsWhiteSpace(currentChar))
                {
                    delimiter = true;
                }

                //  else if string in quotes
                else if ((currentChar == '"') && ((index == 0) || (text[index - 1] != '\\')))
                {
                    //  get closing quote
                    int endIndex = index;
                    do
                    {
                        endIndex = text.IndexOf("\"", endIndex + 1);
                        if (endIndex == -1)
                            EquationConverters.ThrowExceptionWithError("File ends with unclosed quote");

                    } while (text[endIndex - 1] == '\\');

                    //  add string symbol
                    ++index;
                    if (endIndex > index)
                    {
                        var quoted = text.Substring(index, endIndex - index);
                        symbols.Add(new Symbol() { Code = LexicalCodes.String, Word = quoted, FileName = filename, LineNumber = lineNumber });
                    }

                    //  account for newlines within quotes
                    int numLines = 0;
                    for (int n = index; n < endIndex; ++n)
                    {
                        if (text[n] == '\n')
                            ++numLines;
                    }
                    index = endIndex;
                    delimiter = true;
                }
                else
                {
                    //  try to convert largest lexical string into code
                    for (int n = 8; n > 0; --n)
                    {
                        //  check for remaining characters
                        if ((text.Length - index) < n)
                            continue;

                        //  try to get the key
                        word = text.Substring(index, n);
                        if (SymbolLexicon.TryGetValue(word, out code))
                        {
                            //  indicate delimiter found and bump index past symbol
                            delimiter = true;
                            index += (n - 1);
                            break;
                        }
                    }
                }

                int t = 0;
                if (lineNumber == 157)
                    ++t;

                //  non-symbol text
                if (!delimiter)
                    label += currentChar;
                else if (label != string.Empty)
                {
                    symbols.Add(new Symbol() { Code = LexicalCodes.Value, Word = label, FileName = filename, LineNumber = lineNumber });
                    label = string.Empty;
                }

                //  known symbols
                if (code != LexicalCodes.None)
                    symbols.Add(new Symbol() { Code = code, Word = word, FileName = filename, LineNumber = lineNumber });
            }

            //  check for last label
            if (label != string.Empty)
                symbols.Add(new Symbol() { Code = LexicalCodes.Value, Word = label, FileName = filename, LineNumber = lineNumber });

            //  return list of symbols
            return symbols;
        }

#endif



#if UNUSED_CODE

        /// <summary>
        /// Converts C-style text to list of symbols.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <param name="lineNumber">The starting line number of the text.</param>
        /// <returns>A list of symbols.</returns>
        public static List<Symbol> TextToSymbols(string text, int lineNumber)
        {
            var symbols = new List<Symbol>(1000);

            //  for all characters in text
            for (int index = 0; index < text.Length; ++index)
            {
                //  current character
                var currentChar = text[index];

                //  track line number
                if (currentChar == '\n')
                    ++lineNumber;

                //  ignore white space
                if (Char.IsWhiteSpace(currentChar))
                    continue;

                //  try to convert largest lexical string into code
                var lexCode = LexicalCodes.None;
                string word = string.Empty;
                for (int n = 8; n > 0; --n)
                {
                    //  check for remaining characters
                    if ((text.Length - index) < n)
                        continue;

                    //  try to get the key
                    word = text.Substring(index, n);
                    if (Lexicon.TryGetValue(word, out lexCode))
                    {
                        index += (n - 1);
                        break;
                    }
                }

                //  process code
                switch (lexCode)
                {
                    case LexicalCodes.None:
                        try
                        {
                            word = new string(text.Substring(index).TakeWhile(c => (Char.IsLetterOrDigit(c) || (c == '_') || (c == '.'))).ToArray());
                            if (word.Length == 0)
                                throw new Exception(string.Format("Syntax error: Unrecognized symbol on line {0}", lineNumber));
                            symbols.Add(new Symbol() { Code = lexCode, Word = word, LineNumber = lineNumber });
                            index += (word.Length - 1);
                        }
                        catch (Exception ex)
                        {
                            if ((ex.Message != null) && ex.Message.StartsWith("Syntax"))
                                throw new Exception(ex.Message);
                            throw new Exception(string.Format("Syntax error on line {0}", lineNumber));
                        }
                        break;

                        /*
                    case LexicalCodes.TokenKey:
                        try
                        {
                            var word = new string(text.Substring(index + 1).TakeWhile(c => (Char.IsLetterOrDigit(c) || (c == '-'))).ToArray());
                            symbols.Add(new Symbol() { Code = lexCode, Word = word, LineNumber = lineNumber });
                            index += (1 + word.Length);
                            if (text[index] != ']')
                                throw new Exception(string.Format("Syntax error: Missing closing token key bracket on line {0}", lineNumber));
                        }
                        catch (Exception ex)
                        {
                            //  if specific syntax exception
                            if ((ex.Message != null) && (ex.Message.StartsWith("Syntax")))
                                throw new Exception(ex.Message);

                            //  else throw general syntax exception
                            throw new Exception(string.Format("Syntax error on line {0}", lineNumber));
                        }
                        break;
                        */

                    default:
                        symbols.Add(new Symbol() { Code = lexCode, Word = word, LineNumber = lineNumber });
                        break;
                }
            }

            //  return list of symbols
            return symbols;
        }

#endif
