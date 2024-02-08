/**
  ******************************************************************************
  * @file       Preprocessor.cs
  * @copyright  © 2017 ECCO Group.  All rights reserved.
  * @author     M. Latham, Liquid Logic
  * @version    1.0.0
  * @date       12/1/2017
  *
  * @brief      Part of the EquationConverters class for converting plain text
  *             equations to and from the bytecode used in the ESG products'
  *             embedded controllers.
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


namespace ESG.BytecodeLib
{
    public static partial class EquationConverters
    {
        /// <summary>
        /// A class for the conditional assembly stack.
        /// </summary>
        private class ConditionalAssembly
        {
            /// <summary>
            /// Indicates whether code has been included in the if-else-endif set.
            /// </summary>
            public bool HasIncluded = false;

            /// <summary>
            /// Indicates whether the if-else-endif should be included or skipped.
            /// </summary>
            public bool IncludingText = false;
        }


        /// <summary>
        /// Preprocesses the text in the given files, producing a list of symbols with just the equations.
        /// Will throw on incorrect syntax or undefined labels.
        /// </summary>
        /// <param name="text">The text to process.</param>
        /// <param name="textSub">Optional dictionary of text substitutions.  May be null.</param>
        /// <param name="filename">Optional file name, used for error reporting.  May be null.</param>
        /// <param name="searchPaths">Optional list of file search paths, used for #include statements.  May be null.</param>
        /// <returns>A list of symbols with just the equations.</returns>
        private static List<Symbol> PreprocessText(string text, Dictionary<string, string> textSub, string filename, List<string> searchPaths)
        {
            //  create text substituion dictionary if needed
            if (textSub == null)
                textSub = new Dictionary<string, string>();

            //  set error reporting filename
            ErrorReportingFilename = filename;

            //  create a text substitution dictionary based on defines and enums
            int lineNumber = -1;
            try
            {
                //  remove comments from text
                var str = CommentRemover.Remove(text, 1);

                //  convert to symbols
                var symbols = Symbol.TextToSymbols(str, Path.GetFileName(filename), 1);

                //  create enum and struct parsers for this file
                var enumParser = new EnumParser();
                var structParser = new StructParser();

                //  create conditional assembly stack for this file
                var condAssyStack = new List<ConditionalAssembly>();

                bool includingText = true;
                for (int index = 0; index < symbols.Count; ++index)
                {
                    //  get line number and set reporting
                    lineNumber = symbols[index].LineNumber;
                    ErrorReportingLineNumber = lineNumber;

                    //  get all symbols for line
                    var lineSymbols = new List<Symbol>(30);
                    for (int n = index; n < symbols.Count; ++n)
                    {
                        if (symbols[n].LineNumber != lineNumber)
                            break;
                        lineSymbols.Add(symbols[n]);
                    }

                    //  # and typedef must be first line symbol, if found
                    for (int n = 1; n < lineSymbols.Count; ++n)
                    {
                        if ((lineSymbols[n].Code == Symbol.LexicalCodes.PpHash) ||
                            (lineSymbols[n].Code == Symbol.LexicalCodes.Typedef))
                            throw new Exception();
                    }

                    //  if a preprocessor directive
                    if (lineSymbols[0].Code == Symbol.LexicalCodes.PpHash)
                    {
                        //  remove directive
                        symbols.RemoveRange(index, lineSymbols.Count);
                        --index;

                        //  switch on directive type
                        switch (lineSymbols[1].Code)
                        {
                            case Symbol.LexicalCodes.PpInclude:
                                if (includingText)
                                {
                                    //  if a non-compiler include
                                    if (lineSymbols[2].Code == Symbol.LexicalCodes.String)
                                    {
                                        //  check syntax
                                        if (lineSymbols.Count != 3)
                                            throw new Exception();

                                        //  try to get file text
                                        var fileText = GetFileText(lineSymbols[2].Word, searchPaths);
                                        if (fileText == string.Empty)
                                            throw new Exception();

                                        //  process the included file
                                        var fn = ErrorReportingFilename;
                                        int ln = ErrorReportingLineNumber;
                                        var newSymbols = PreprocessText(fileText, textSub, lineSymbols[2].Word, searchPaths);
                                        ErrorReportingLineNumber = ln;
                                        ErrorReportingFilename = fn;
                                        symbols.InsertRange(index + 1, newSymbols);
                                        index += newSymbols.Count;
                                    }
                                }
                                break;

                            case Symbol.LexicalCodes.PpDefine:
                                //  check skipping
                                if (includingText)
                                {
                                    //  get and validate subsution key text
                                    if (!lineSymbols[2].IsLabel)
                                        throw new Exception();
                                    var key = lineSymbols[2].Word;

                                    //  get substitution value text as empty or value of remaining symbols on line
                                    var value = string.Empty;
                                    if (lineSymbols.Count > 3)
                                    {
                                        //  get lists of token symbols for line, split by the token address symbol
                                        var lists = SplitSymbolsWithTokenAddress(lineSymbols.GetRange(3, lineSymbols.Count - 3));
                                        foreach (var list in lists)
                                        {
                                            if ((list.Count == 1) && (list[0].Code == Symbol.LexicalCodes.TokenAddress))
                                                value += list[0].Word;
                                            else
                                                value += GetSymbolsIntValue(list, textSub).ToString();
                                        }
                                    }

                                    //  add to dictionary
                                    textSub.Add(key, value);
                                }
                                break;

                            case Symbol.LexicalCodes.PpUndef:
                                //  check skipping
                                if (includingText)
                                {
                                    //  validate label key and no other symbols
                                    if ((!lineSymbols[2].IsLabel) || (lineSymbols.Count > 3))
                                        throw new Exception();

                                    //  remove from substitution list
                                    textSub.Remove(lineSymbols[2].Word);
                                }
                                break;

                            case Symbol.LexicalCodes.PpIfdef:
                                if ((condAssyStack.Count > 0) && !condAssyStack.Last().IncludingText)
                                    condAssyStack.Add(new ConditionalAssembly() { HasIncluded = false, IncludingText = false });
                                else
                                {
                                    //  validate label key and no other symbols
                                    if ((!lineSymbols[2].IsLabel) || (lineSymbols.Count > 3))
                                        throw new Exception();

                                    //  determine including text and push on stack
                                    includingText = textSub.ContainsKey(lineSymbols[2].Word);
                                    condAssyStack.Add(new ConditionalAssembly() { HasIncluded = includingText, IncludingText = includingText });
                                }
                                break;

                            case Symbol.LexicalCodes.PpIfndef:
                                if ((condAssyStack.Count > 0) && !condAssyStack.Last().IncludingText)
                                    condAssyStack.Add(new ConditionalAssembly() { HasIncluded = false, IncludingText = false });
                                else
                                {
                                    //  validate label key and no other symbols
                                    if ((!lineSymbols[2].IsLabel) || (lineSymbols.Count > 3))
                                        throw new Exception();

                                    //  determine including text and push on stack
                                    includingText = !textSub.ContainsKey(lineSymbols[2].Word);
                                    condAssyStack.Add(new ConditionalAssembly() { HasIncluded = includingText, IncludingText = includingText });
                                }
                                break;

                            case Symbol.LexicalCodes.PpIf:
                                if ((condAssyStack.Count > 0) && !condAssyStack.Last().IncludingText)
                                    condAssyStack.Add(new ConditionalAssembly() { HasIncluded = false, IncludingText = false });
                                else
                                {
                                    //  need at least 3 symbols
                                    if (lineSymbols.Count < 3)
                                        throw new Exception();

                                    //  determine including text and push on stack
                                    includingText = (0 != GetSymbolsIntValue(lineSymbols.GetRange(2, lineSymbols.Count - 2), textSub));
                                    condAssyStack.Add(new ConditionalAssembly() { HasIncluded = includingText, IncludingText = includingText });
                                }
                                break;

                            case Symbol.LexicalCodes.PpElif:
                                //  must already be on conditional assy stack
                                if (condAssyStack.Count == 0)
                                    AddError("No #if for #elif", false);

                                //  if set included
                                if ((condAssyStack.Count == 1) || condAssyStack[condAssyStack.Count - 2].IncludingText)
                                {
                                    //  need at least 3 symbols
                                    if (lineSymbols.Count < 3)
                                    throw new Exception();

                                    //  set including text value
                                    var ca = condAssyStack[condAssyStack.Count - 1];
                                    includingText = false;
                                    ca.IncludingText = false;
                                    if (!ca.HasIncluded)
                                    {
                                        includingText = (0 != GetSymbolsIntValue(lineSymbols.GetRange(2, lineSymbols.Count - 2), textSub));
                                        ca.HasIncluded = includingText;
                                        ca.IncludingText = includingText;
                                    }
                                }
                                break;

                            case Symbol.LexicalCodes.PpElse:
                                //  must already be on conditional assy stack
                                if (condAssyStack.Count == 0)
                                    AddError("No #if for #else", false);

                                //  if including set
                                if ((condAssyStack.Count == 1) || condAssyStack[condAssyStack.Count - 2].IncludingText)
                                {
                                    //  must be 2 symbols
                                    if (lineSymbols.Count != 2)
                                        throw new Exception();

                                    //  set including text value
                                    var ca = condAssyStack[condAssyStack.Count - 1];
                                    includingText = !ca.HasIncluded;
                                    ca.IncludingText = includingText;
                                }
                                break;

                            case Symbol.LexicalCodes.PpEndif:
                                //  must already be on conditional assy stack
                                if (condAssyStack.Count == 0)
                                    AddError("No #if for #endif", false);

                                //  validate no other symbols on line
                                if (lineSymbols.Count != 2)
                                    throw new Exception();

                                //  pop off stack
                                condAssyStack.RemoveAt(condAssyStack.Count - 1);

                                //  if conditional assembly on stack
                                if (condAssyStack.Count > 0)
                                    includingText = condAssyStack[condAssyStack.Count - 1].IncludingText;
                                else
                                    includingText = true;

                                break;

                            default:
                                break;
                        }
                    }
                    else if (includingText)
                    {
                        //  if processing an enum
                        if (enumParser.State != EnumParser.States.None)
                        {
                            //  remove line
                            symbols.RemoveRange(index, lineSymbols.Count);
                            --index;

                            //  parser for enum
                            enumParser.ParseSymbols(lineSymbols, textSub);
                        }

                        //  else if processing a struct
                        else if (structParser.State != StructParser.States.None)
                        {
                            //  remove line
                            symbols.RemoveRange(index, lineSymbols.Count);
                            --index;

                            //  parser for enum
                            structParser.ParseSymbols(lineSymbols);
                        }

                        else
                        {
                            switch (lineSymbols[0].Code)
                            {
                                case Symbol.LexicalCodes.Typedef:
                                    //  remove line
                                    symbols.RemoveRange(index, lineSymbols.Count);
                                    --index;

                                    //  switch on directive type
                                    switch (lineSymbols[1].Code)
                                    {
                                        case Symbol.LexicalCodes.Enum:
                                            //  set parser
                                            enumParser.Key = string.Empty;
                                            enumParser.Value = 0;
                                            enumParser.State = EnumParser.States.OpeningBrace;

                                            //  if more symbols on line
                                            if (lineSymbols.Count > 2)
                                                enumParser.ParseSymbols(lineSymbols.GetRange(2, lineSymbols.Count - 2), textSub);
                                            break;

                                        case Symbol.LexicalCodes.Struct:
                                            //  set parser
                                            structParser.State = StructParser.States.OpeningBrace;

                                            //  if more symbols on line
                                            if (lineSymbols.Count > 2)
                                                structParser.ParseSymbols(lineSymbols.GetRange(2, lineSymbols.Count - 2));
                                            break;
                                    }
                                    break;

                                //  prototype declarations
                                case Symbol.LexicalCodes.Extern:
                                case Symbol.LexicalCodes.Bool:
                                case Symbol.LexicalCodes.UInt8:
                                case Symbol.LexicalCodes.UInt16:
                                case Symbol.LexicalCodes.UInt32:
                                case Symbol.LexicalCodes.Int8:
                                case Symbol.LexicalCodes.Int16:
                                case Symbol.LexicalCodes.Int32:
                                case Symbol.LexicalCodes.Int:
                                case Symbol.LexicalCodes.Short:
                                case Symbol.LexicalCodes.Unsigned:
                                    //  remove line
                                    symbols.RemoveRange(index, lineSymbols.Count);
                                    --index;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                    else  //  not including text
                    {
                        symbols.RemoveRange(index, lineSymbols.Count);
                        --index;
                    }
                }

                //  check conditional assembly stack
                if (condAssyStack.Count != 0)
                    AddError("Missing #endif", false);

                //  check enum parser
                if (enumParser.State != EnumParser.States.None)
                    AddError("Missing enumeration close", false);

                //  check struct parser
                if (structParser.State != StructParser.States.None)
                    AddError("Missing struct close", false);

                //  make text substitutions
                foreach (var symbol in symbols)
                    if ((symbol.Code == Symbol.LexicalCodes.Value) && (textSub.TryGetValue(symbol.Word, out string val)))
                        symbol.Word = val;

                //  build pre-equation constants, if any
                BuildPreEquationConstants(symbols, textSub);

                //  calculate token keys
                CalculateTokenKeys(symbols, textSub);

                //  return the remaining symbols, which should be bytecode symbols
                return symbols;
            }
            catch (Exception ex)
            {
                if (ex.Message == "An item with the same key has already been added.")
                    AddError("Duplicate symbol definition", false);
                else if (ex.Message == string.Empty)
                    throw new Exception(ex.Message);
                else
                    AddError(ex.Message, false);
                return null;
            }
        }

        /// <summary>
        /// Gets file text from the given file name and search paths.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <param name="searchPaths">The search paths.</param>
        /// <returns>Returns the file text, or string.Empty if file cannot be found.</returns>
        private static string GetFileText(string filename, List<string> searchPaths)
        {
            //  text to return
            string text = string.Empty;

            try
            {
                if (File.Exists(filename))
                {
                    text = File.ReadAllText(filename);
                }
                else if (searchPaths != null)
                {
                    //  add separator to file name if needed
                    if (!filename.StartsWith("/") && !filename.StartsWith("\\"))
                        filename = "/" + filename;

                    //  for all search paths
                    foreach (var searchPath in searchPaths)
                    {
                        //  remove path ending separator if needed
                        var fullPath = searchPath;
                        if (fullPath.EndsWith("/") || fullPath.EndsWith("\\"))
                            fullPath = fullPath.Substring(0, fullPath.Length - 1);
                        fullPath += filename;
                        if (File.Exists(fullPath))
                        {
                            text = File.ReadAllText(fullPath);
                            break;
                        }
                    }
                }
            }
            catch
            {
                //  returning string.Empty on error
            }
            return text;
        }

    }
}



#if UNUSED_CODE


#region Preprocssor directives
        /// <summary>
        /// Processes the preprocesser directives in a text file.  Throws an error on incorrect syntax or undefined symbols.
        /// </summary>
        /// <param name="text">The text to process.</param>
        /// <returns>A preprocessed text string.</returns>
        private static string ProcessPreprocessorDirectives(string text)
        {
            //  create text sub dictionary
            var textSub = new List<KeyValuePair<string, string>>();

            //  copy text
            var str = string.Copy(text);

            //  handle preprocessor directives
            int lineNumber = 1;
            for (int index = 0; index < str.Length; ++index)
            {
                //  track line number
                if (str[index] == '\n')
                    ++lineNumber;

                //  ignore white space
                if (Char.IsWhiteSpace(str[index]))
                    continue;

                //  if a preprocessor directive
                if (str[index] == '#')
                {
                    try
                    {
                        if (str.IndexOf("define") == (index + 1))
                        {
                            //  get text substitution dictionary key
                            int idx = index + "#define".Length;
                            while (Char.IsWhiteSpace(str[idx])) { ++idx; }
                            var key = new string(str.Substring(idx).TakeWhile(c => (Char.IsLetterOrDigit(c) || (c == '_'))).ToArray());

                            //  get remainder of line
                            idx += key.Length;
                            int endIndex = str.IndexOf('\n', idx);
                            if (endIndex == -1)
                                endIndex = str.Length;
                            var value = str.Substring(idx, endIndex - idx).Trim();

                            //  make text substitutions
                            //foreach (var kvp in textSub)
                            //    value.Replace(kvp.Key, kvp.Value);

                            //  add substition to dictionary
                            //textSub.Add(key, value);

                            //  insert substitution to start of test substitution list
                            textSub.Insert(0, new KeyValuePair<string, string>(key, value));

                            //  remove text, except for newline
                            str = str.Remove(index, endIndex - index);
                            --index;
                        }
                        else
                            throw new Exception(string.Format("Unrecognized preprocessor command on line {0}", lineNumber));
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        if ((msg == null) || !msg.StartsWith("Unrecognized"))
                            msg = string.Format("File #define on line {0} cannot be resolved.", lineNumber);
                        throw new Exception(msg);
                    }
                }
            }

            //  make text substitutions
            foreach (var kvp in textSub)
                str = str.Replace(kvp.Key, kvp.Value);

            //  return the pre-processed string
            return str;
        }
#endregion

#endif
