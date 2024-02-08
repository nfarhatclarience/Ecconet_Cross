using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LexicalCodes = ESG.BytecodeLib.Symbol.LexicalCodes;


namespace ESG.BytecodeLib
{
    public static partial class EquationConverters
    {
        /// <summary>
        /// Gets the first symbol that matches the given code.
        /// </summary>
        /// <param name="symbols">The symbols within which to search.</param>
        /// <param name="code">The code to match.</param>
        /// <param name="startIndex">The start index within the given symbols.</param>
        /// <returns>Returns the index of the first match, or -1 on no match.</returns>
        public static int IndexOfSymbol(List<Symbol> symbols, LexicalCodes code, int startIndex)
        {
            for (; startIndex < symbols.Count; ++startIndex)
                if (symbols[startIndex].Code == code)
                    return startIndex;
            return -1;
        }

        /// <summary>
        /// Gets the first symbol that matches any of the given codes.
        /// </summary>
        /// <param name="symbols">The symbols within which to search.</param>
        /// <param name="codes">The codes to match.</param>
        /// <param name="startIndex">The start index within the given symbols.</param>
        /// <returns>Returns the index of the first match, or -1 on no match.</returns>
        public static int IndexOfSymbol(List<Symbol> symbols, LexicalCodes[] codes, int startIndex)
        {
            for (; startIndex < symbols.Count; ++startIndex)
            {
                var symbol = symbols[startIndex];
                foreach (var code in codes)
                    if (symbol.Code == code)
                    return startIndex;
            }
            return -1;
        }

        /// <summary>
        /// Gets the symbols on a line.
        /// </summary>
        /// <param name="symbols">The symbols from which to get the symbols on a line.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns>The symbols on a line.</returns>
        private static List<Symbol> SymbolsOnLine(List<Symbol> symbols, int lineNumber)
        {
            var symList = new List<Symbol>(30);

            foreach (var symbol in symbols)
            {
                if (symbol.LineNumber == lineNumber)
                    symList.Add(symbol);
                if (symbol.LineNumber > lineNumber)
                    break;
            }
            return symList;
        }


        /// <summary>
        /// Convert symbols into integer value.  Will throw if conversion fails. 
        /// </summary>
        /// <param name="symbols">The symbols to evaluate.</param>
        /// <param name="textSub">The text substitutions.</param>
        /// <returns>The integer value of the symbols.</returns>
        public static int GetSymbolsIntValue(List<Symbol> symbols, Dictionary<string, string> textSub)
        {
            //  get concatenation of all symbol argument words on line, making substitutions
            foreach (var symbol in symbols)
            {
                //  make substitutions with previous symbols and get token address index (if present)
                if ((symbol.Code == Symbol.LexicalCodes.Value) && (textSub.TryGetValue(symbol.Word, out string val)))
                    symbol.Word = val;
            }

            //  get symbols from string value
            if (symbols.Count == 1)
                return symbols[0].IntValue;
            return SymbolCalculator.PerformCalculation(symbols, out int operandStackDepth, out int operatorStackDepth);
        }


        /// <summary>
        /// Splits a list of symbols into separate lists split with token address symbol.
        /// </summary>
        /// <param name="symbols"></param>
        /// <returns>Separate lists split with token address symbol.</returns>
        private static List<List<Symbol>> SplitSymbolsWithTokenAddress(List<Symbol> symbols)
        {
            var list = new List<List<Symbol>>(3);

            //  get index of token address symbol, if any
            int tokenAddressIndex = -1;
            for (int i = 0; i < symbols.Count; ++i)
            {
                if (symbols[i].Code == Symbol.LexicalCodes.TokenAddress)
                {
                    if (tokenAddressIndex == -1)
                        tokenAddressIndex = i;
                    else
                        throw new Exception();
                }
            }

            //  add symbols to lists
            if (tokenAddressIndex == -1)
                list.Add(symbols);
            else
            {
                //  get symbols before token
                if (tokenAddressIndex > 0)
                    list.Add(symbols.GetRange(0, tokenAddressIndex));

                //  get token
                list.Add(symbols.GetRange(tokenAddressIndex, 1));

                //  get symbols after token
                ++tokenAddressIndex;
                if (tokenAddressIndex < symbols.Count)
                    list.Add(symbols.GetRange(tokenAddressIndex, symbols.Count - tokenAddressIndex));
            }

            //  return list
            return list;
        }

        /// <summary>
        /// Calculates any math or logic within the token key brackets.
        /// </summary>
        /// <param name="symbols">The list of symbols to parse.</param>
        /// <param name="textSub">The list of text substitutions.</param>
        private static void CalculateTokenKeys(List<Symbol> symbols, Dictionary<string, string> textSub)
        {
            //  collapse token key equations
            int tokenStartIndex = -1;
            for (int i = 0; i < symbols.Count; ++i)
            {
                //  get symbol
                var symbol = symbols[i];

                //  if a token start
                if (symbol.Code == LexicalCodes.TokenKey)
                {
                    tokenStartIndex = i + 1;
                }
                else if (symbol.Code == LexicalCodes.TokenKeyClose)
                {
                    //  verify have token start and token between brackets
                    if ((tokenStartIndex == -1) || (tokenStartIndex == i))
                        AddError("Token key error", symbol.FileName, symbol.LineNumber, false);

                    //  get symbols to convert and remove from list
                    int count = i - tokenStartIndex;
                    var tokenSymbols = symbols.GetRange(tokenStartIndex, count);
                    symbols.RemoveRange(tokenStartIndex, count);
                    i -= count;

                    //  get lists of token symbols for line, split by the token address symbol
                    var lists = SplitSymbolsWithTokenAddress(tokenSymbols);
                    foreach (var list in lists)
                    {
                        if ((list.Count == 1) && (list[0].Code == LexicalCodes.TokenAddress))
                        {
                            symbols.Insert(tokenStartIndex, list[0]);
                            ++tokenStartIndex;
                            ++i;
                        }
                        else
                        {
                            var value = GetSymbolsIntValue(list, textSub).ToString();
                            symbols.Insert(tokenStartIndex, new Symbol() { Code = Symbol.LexicalCodes.Value, Word = value, LineNumber = list[0].LineNumber });
                            ++tokenStartIndex;
                            ++i;
                        }
                    }

                    //  clear token key start index
                    tokenStartIndex = -1;
                }
            }

            //  verify last closing bracket
            if (tokenStartIndex != -1)
                AddError("Missing token key bracket", symbols[symbols.Count - 1].FileName, symbols[symbols.Count - 1].LineNumber, false);
        }

        /// <summary>
        /// Combines pre-equation constant symbols with their value.
        /// </summary>
        /// <param name="symbols">The list of symbols to parse.</param>
        /// <param name="textSub">The list of text substitutions.</param>
        private static void BuildPreEquationConstants(List<Symbol> symbols, Dictionary<string, string> textSub)
        {
            //  collapse token key equations
            int address = 0;
            int lineNumber = -1;
            for (int index = 0; index < symbols.Count; ++index)
            {
                //  get all symbols for line
                lineNumber = symbols[index].LineNumber;
                var lineSymbols = new List<Symbol>(30);
                for (int n = index; n < symbols.Count; ++n)
                {
                    if (symbols[n].LineNumber != lineNumber)
                        break;
                    lineSymbols.Add(symbols[n]);
                }
                if (lineSymbols.Count == 0)
                    continue;

                // split the line between the first and remaining symbols
                var symbol = lineSymbols[0];
                lineSymbols.RemoveAt(0);

                switch (symbol.Code)
                {
                    case LexicalCodes.Value8:
                        //  validate that the V8 was not found after equations started
                        if (address < 0)
                        {
                            AddError("Const symbol table must preceed equations", symbol.FileName, symbol.LineNumber, true);
                            break;
                        }

                        //  validate that there is a value for the V8
                        if (lineSymbols.Count == 0)
                        {
                            AddError("Missing V8 value", symbol.FileName, symbol.LineNumber, false);
                            break;
                        }

                        //  get the value for the V8
                        symbol.Word = GetSymbolsIntValue(lineSymbols, textSub).ToString();

                        //  remove the symbols
                        symbols.RemoveRange(index + 1, lineSymbols.Count);

                        //  bump the byte address
                        ++address;
                        break;

                    case LexicalCodes.Value16:
                        //  validate that the V16 was not found after equations started
                        if (address < 0)
                        {
                            AddError("Const symbol table must preceed equations", symbol.FileName, symbol.LineNumber, true);
                            break;
                        }

                        //  validate that there is a value for the V16
                        if (lineSymbols.Count == 0)
                        {
                            AddError("Missing V16 value", symbol.FileName, symbol.LineNumber, false);
                            break;
                        }

                        //  if not on 16-bit address, insert dummy byte
                        if ((address & 0x01) != 0)
                        {
                            symbols.Insert(index, new Symbol() { Code = LexicalCodes.Value8, Word = "0", FileName = symbol.FileName, LineNumber = symbol.LineNumber });
                            ++index;
                            ++address;
                        }

                        //  get the value for the V16
                        symbol.Word = GetSymbolsIntValue(lineSymbols, textSub).ToString();

                        //  remove the symbols
                        symbols.RemoveRange(index + 1, lineSymbols.Count);

                        //  bump the byte address
                        address += 2;
                        break;

                    case LexicalCodes.Value32:
                        //  validate that the V32 was not found after equations started
                        if (address < 0)
                        {
                            AddError("Const symbol table must preceed equations", symbol.FileName, symbol.LineNumber, true);
                            break;
                        }

                        //  validate that there is a value for the V32
                        if (lineSymbols.Count == 0)
                        {
                            AddError("Missing V32 value", symbol.FileName, symbol.LineNumber, false);
                            break;
                        }

                        //  while not on 32-bit address, insert dummy bytes
                        while ((address & 0x03) != 0)
                        {
                            symbols.Insert(index, new Symbol() { Code = LexicalCodes.Value8, Word = "0", FileName = symbol.FileName, LineNumber = symbol.LineNumber });
                            ++index;
                            ++address;
                        }

                        //  get the value for the V32
                        symbol.Word = GetSymbolsIntValue(lineSymbols, textSub).ToString();

                        //  remove the symbols
                        symbols.RemoveRange(index + 1, lineSymbols.Count);

                        //  bump the byte address
                        address += 4;
                        break;

                    default:
                        //  if any initial constants have been added
                        if (address > 0)
                        {
                            if (address > 65535)
                                AddError("Const symbol table must be < 65532 bytes", symbol.FileName, symbol.LineNumber, true);

                            //  while not on 32-bit address, insert dummy bytes
                            while ((address & 0x03) != 0)
                            {
                                symbols.Insert(index, new Symbol() { Code = LexicalCodes.Value8, Word = "0", FileName = symbol.FileName, LineNumber = symbol.LineNumber });
                                ++index;
                                ++address;
                            }

                            //  add "cafe" and 16-bit table size to beginning of symbols
                            symbols.Insert(0, new Symbol() { Code = LexicalCodes.Value16, Word = "0xfeca", FileName = symbol.FileName, LineNumber = symbol.LineNumber });
                            symbols.Insert(1, new Symbol() { Code = LexicalCodes.Value16, Word = address.ToString(), FileName = symbol.FileName, LineNumber = symbol.LineNumber });
                            index += 2;
                        }

                        //  invalidate address
                        address = -1;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the number of unique tokens in the given list of symbols.
        /// </summary>
        /// <param name="symbols">The symbols to parse.</param>
        /// <returns>The number of unique tokens.</returns>
        private static int GetNumUniqueTokens(List<Symbol> symbols)
        {
            //  get number of tokens
            var keyAddresses = new List<int>();
            for (int i = 0; i < symbols.Count; ++i)
            {
                var symbol = symbols[i];

                if (symbol.Code == LexicalCodes.TokenKey)
                {
                    //  next symbol should be value
                    symbol = symbols[++i];
                    if (symbol.Code != LexicalCodes.Value)
                        AddError("Invalid token key", false);
                    int value = (int)symbol.UInt16Value << 16;

                    //  if next symbol is token address
                    if (symbols[i + 1].Code == LexicalCodes.TokenAddress)
                    {
                        //  next symbol should be value
                        ++i;
                        symbol = symbols[++i];
                        if (symbol.Code != LexicalCodes.Value)
                            AddError("Invalid token address", false);
                        value |= (int)symbol.ByteValue << 8;
                    }

                    //  if new unique token, add to list
                    if (!keyAddresses.Contains(value))
                        keyAddresses.Add(value);
                }
            }
            return keyAddresses.Count;
        }

    }
}
