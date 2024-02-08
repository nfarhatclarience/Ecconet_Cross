/**
  ******************************************************************************
  * @file       StructParser.cs
  * @author     M. Latham, Liquid Logic
  * @version    1.0.0
  * @date       Dec 2017
  *
  * @brief      Class for parsing C-language structure symbols.  For now,
  *             only validating start and end.
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
    /// A class for parsing structs.
    /// </summary>
    public class StructParser
    {
        /// <summary>
        /// An enum for parsing enums.
        /// </summary>
        public enum States
        {
            None,
            OpeningBrace,
            ClosingBrace,
            Name,
            SemiColon
        }

        /// <summary>
        /// The parsing state.
        /// </summary>
        public States State = States.None;


        /// <summary>
        /// Parses the given symbols.
        /// </summary>
        /// <param name="symbols"></param>
        public void ParseSymbols(List<Symbol> symbols)
        {
            //  for all given symbols
            foreach (var symbol in symbols)
            {
                switch (State)
                {
                    case States.OpeningBrace:
                        if (symbol.Code == Symbol.LexicalCodes.OpenBrace)
                        {
                            State = States.ClosingBrace;
                        }
                        else
                            throw new Exception();
                        break;

                    case States.ClosingBrace:
                        if (symbol.Code == Symbol.LexicalCodes.CloseBrace)
                        {
                            State = States.Name;
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
