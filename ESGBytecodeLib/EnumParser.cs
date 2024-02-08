/**
  ******************************************************************************
  * @file       EnumParser.cs
  * @author     M. Latham, Liquid Logic
  * @version    1.0.0
  * @date       Dec 2017
  *
  * @brief      Class for parsing C-language enumeration symbols.
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

namespace ESG.BytecodeLib
{
    /// <summary>
    /// A class for parsing enums.
    /// </summary>
    public class EnumParser
    {
        /// <summary>
        /// An enum for parsing enums.
        /// </summary>
        public enum States
        {
            None,
            OpeningBrace,
            Label,
            PostLabel,
            ValueSymbols,
            Comma,
            Name,
            SemiColon
        }

        /// <summary>
        /// The parsing state.
        /// </summary>
        public States State = States.None;

        /// <summary>
        /// The enumeration key.
        /// </summary>
        public string Key = string.Empty;

        /// <summary>
        /// The enumeration value.
        /// </summary>
        public int Value = 0;

        /// <summary>
        /// Value string for when using equals directive.
        /// </summary>
        public string ValueString = string.Empty;

        //  value symbols
        private List<Symbol> ValueSymbols = new List<Symbol>();


        /// <summary>
        /// Parses the given symbols, using text substitutions for values when a match is found.
        /// </summary>
        /// <param name="symbols">The symbols to parse.</param>
        /// <param name="textSub">The text substituion dictionary.</param>
        public void ParseSymbols(List<Symbol> symbols, Dictionary<string, string> textSub)
        {
            //  for all given symbols
            foreach (var symbol in symbols)
            {
                switch (State)
                {
                    case States.OpeningBrace:
                        if (symbol.Code == Symbol.LexicalCodes.OpenBrace)
                        {
                            State = States.Label;
                        }
                        else
                            throw new Exception();
                        break;

                    case States.Label:
                        if ((symbol.Code == Symbol.LexicalCodes.Value) && symbol.IsLabel)
                        {
                            Key = symbol.Word;
                            State = States.PostLabel;
                        }
                        else if (symbol.Code == Symbol.LexicalCodes.CloseBrace)
                        {
                            State = States.Name;
                        }
                        else
                            throw new Exception();
                        break;

                    case States.PostLabel:
                        if (symbol.Code == Symbol.LexicalCodes.Equals)
                        {
                            ValueSymbols.Clear();
                            ValueString = string.Empty;
                            State = States.ValueSymbols;
                        }
                        else if (symbol.Code == Symbol.LexicalCodes.Comma)
                        {
                            textSub.Add(Key, Value.ToString());
                            ++Value;
                            State = States.Label;
                        }
                        else if (symbol.Code == Symbol.LexicalCodes.CloseBrace)
                        {
                            State = States.Name;
                        }
                        else
                            throw new Exception();
                        break;

                    case States.ValueSymbols:
                        if (symbol.Code == Symbol.LexicalCodes.Comma)
                        {
                            Value = EquationConverters.GetSymbolsIntValue(ValueSymbols, textSub);
                            textSub.Add(Key, Value.ToString());
                            ++Value;
                            State = States.Label;
                        }
                        else if (symbol.Code == Symbol.LexicalCodes.CloseBrace)
                        {
                            Value = EquationConverters.GetSymbolsIntValue(ValueSymbols, textSub);
                            textSub.Add(Key, Value.ToString());
                            State = States.Name;
                        }
                        else
                        {
                            //  make substitutions with previous symbols (if match) and save to value symbol
                            if (textSub.TryGetValue(symbol.Word, out string val))
                                symbol.Word = val;

                            //  add to list of value symbols and value string
                            ValueSymbols.Add(symbol);
                            ValueString += symbol.Word;
                        }
                        break;

                    case States.Name:
                        if ((symbol.Code == Symbol.LexicalCodes.Value) && symbol.IsLabel)
                        {
                            State = States.SemiColon;
                        }
                        else
                            throw new Exception();
                        break;

                    case States.SemiColon:
                        if (symbol.Code == Symbol.LexicalCodes.EquationEnd)
                            State = States.None;
                        else
                            throw new Exception();
                        break;

                    default:
                        break;
                }
            }
        }

    }
}
