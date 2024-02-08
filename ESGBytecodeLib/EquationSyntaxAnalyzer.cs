using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LexicalCodes = ESG.BytecodeLib.Symbol.LexicalCodes;


namespace ESG.BytecodeLib
{
    public static class EquationSyntaxAnalyzer
    {
        /// <summary>
        /// Test run the equations to check for errors and calculate maximum operand and operator stack depths.
        /// </summary>
        /// <param name="symbols">The equation symbols.</param>
        /// <param name="maxOperandStackDepth">The output var number of equations.</param>
        /// <param name="maxOperandStackDepth">The output var maximum operand stack depth.</param>
        /// <param name="maxOperatorStackDepth">The output var maximum operator stack depth.</param>
        public static void CheckSyntax(List<Symbol> symbols, out int numEquations, out int maxOperandStackDepth, out int maxOperatorStackDepth)
        {
            //  test run here
            numEquations = 0;
            maxOperandStackDepth = 0;
            maxOperatorStackDepth = 0;
            if (symbols.Count > 0)
            {
                //  calculation stack depths
                int calcMaxOperandStackDepth = 0;
                int calcMaxOperatorStackDepth = 0;

                //  set error reporting line number
                EquationConverters.ErrorReportingLineNumber = 0;

                //  advance past initial constants, if any
                int index = 0;
                for (; index < symbols.Count; ++index)
                {
                    var code = symbols[index].Code;
                    if ((code != LexicalCodes.Value8) && (code != LexicalCodes.Value16) && (code != LexicalCodes.Value32))
                        break;
                }

                //  first non-constant symbol must be equation start or priority equation start
                var symbol = symbols[index];
                if ((symbol.Code != LexicalCodes.EquationStart) && (symbol.Code != LexicalCodes.PriorityEquationStart))
                    //&& (symbol.Code != LexicalCodes.SuccessiveEquationStart))
                    EquationConverters.AddError("Equations must start with '$' or '$$'", symbol.FileName, symbol.LineNumber, false);

                //  for all equation symbols
                while (index < symbols.Count)
                {
                    //  set error reporting
                    EquationConverters.ErrorReportingFilename = symbols[index].FileName;
                    EquationConverters.ErrorReportingLineNumber = symbols[index].LineNumber;


                    //  get expression areas

                    //  get left-hand expression
                    int lhExpStart = index + 1;
                    int lhExpEnd = EquationConverters.IndexOfSymbol(symbols, new[] { LexicalCodes.Equals, LexicalCodes.Lambda }, lhExpStart);
                    if (lhExpEnd == -1)
                        EquationConverters.AddError("Left-hand expression must end with '=' or '=>'", false);
                    var leftHandExpression = symbols.GetRange(lhExpStart, lhExpEnd - lhExpStart);

                    //  get right-hand expression
                    int rhExpStart = lhExpEnd + 1;
                    int rhExpEnd = EquationConverters.IndexOfSymbol(symbols, LexicalCodes.EquationEnd, rhExpStart);
                    if (rhExpEnd == -1)
                        EquationConverters.AddError("Right-hand expression must end with ';'", false);

                    //  get logic and output options
                    int optStart = rhExpEnd + 1;
                    int optEnd = EquationConverters.IndexOfSymbol(symbols,
                        new[] { LexicalCodes.EquationStart, LexicalCodes.PriorityEquationStart, LexicalCodes.SuccessiveEquationStart }, optStart);
                    if (optEnd == -1)
                        optEnd = symbols.Count;


                    //  validate area contents

                    //  validate tokens in left-hand expression
                    for (int i = 0; i < leftHandExpression.Count; ++i)
                        if (leftHandExpression[i].Code == LexicalCodes.TokenKey)
                            ValidateToken(leftHandExpression, ref i, "Left-hand expression ");

                    //  run left-hand expression through calculator
                    SymbolCalculator.PerformCalculation(leftHandExpression, out calcMaxOperandStackDepth, out calcMaxOperatorStackDepth);
                    ++numEquations;

                    //  get maximum stack depths
                    if (maxOperandStackDepth < calcMaxOperandStackDepth)
                        maxOperandStackDepth = calcMaxOperandStackDepth;
                    if (maxOperatorStackDepth < calcMaxOperatorStackDepth)
                        maxOperatorStackDepth = calcMaxOperatorStackDepth;

                    //  validate right-hand expression token
                    int n = rhExpStart;
                    ValidateToken(symbols, ref n, "Right-hand expression ");
                    if ((symbols[rhExpStart].Code != LexicalCodes.TokenKey) || (symbols[rhExpEnd - 1].Code != LexicalCodes.TokenKeyClose))
                        EquationConverters.AddError("Right-hand expression may only be a single token ( missing ';' ? )", false);

                    //  validate the output options
                    if (optEnd > optStart)
                        ValidateOutputOptions(symbols.GetRange(optStart, optEnd - optStart));

                    //  if is local mapping expression
                    if (symbols[lhExpEnd].Code == LexicalCodes.Lambda)
                    {
                        //  one token only on left side of equation
                        if ((symbols[lhExpStart].Code != LexicalCodes.TokenKey) || (symbols[lhExpEnd - 1].Code != LexicalCodes.TokenKeyClose))
                            EquationConverters.AddError("Local mapping expression => Left-hand expression may only be a single token", false);

                        //  left-hand token must be local variable
                        UInt16 value = (UInt16)(symbols[lhExpStart + 1].UInt16Value & ~0xE000);
                        if ((value < 1) || (value > 199))
                            EquationConverters.AddError("Local mapping expression => Left-hand token must be local variable", false);

                        //  right-hand token must be global variable
                        value = (UInt16)(symbols[rhExpStart + 1].UInt16Value & ~0xE000);
                        if (value < 200)
                            EquationConverters.AddError("Local mapping expression => Right-hand token must be global variable", false);
                    }

                    //  set index to next expression start
                    index = optEnd;
                }
            }
        }

        /// <summary>
        /// Checks the syntax of the output options.
        /// </summary>
        /// <param name="symbols"></param>
        private static void ValidateOutputOptions(List<Symbol> symbols)
        {
            //  non-used test value
            int test = 0;

            //  status
            bool haveOutputLogic = false;
            bool haveTokenSend = false;

            //  for all given output symbols
            for (int i = 0; i < symbols.Count; ++i)
            {
                //  get the symbol
                var symbol = symbols[i];

                //  check for output option already defined
                if (haveTokenSend)
                    EquationConverters.AddError("Token send option @, only one per equation and must be listed last", false);

                //  process option
                switch (symbol.Code)
                {
                    //  logic functions with value
                    case LexicalCodes.OutputLogicActivityMonitor:
                    case LexicalCodes.OutputLogicRisingEdgeDelay:
                    case LexicalCodes.OutputLogicFallingEdgeDelay:
                        //  only one logic function per equation
                        if (haveOutputLogic)
                            EquationConverters.AddError("-Output options may only include one of activity monitor, counter, toggle or delay", false);
                        haveOutputLogic = true;

                        //  make sure value is given
                        if (symbols[++i].Code != LexicalCodes.Value)
                            EquationConverters.AddError("-Output option is missing value", false);
                        else
                        {
                            try
                            {
                                test = symbols[i].IntValue;
                                if (test > 60000)
                                    EquationConverters.AddError("-Output option time value cannot be > 60 seconds", false);
                            }
                            catch
                            {
                                EquationConverters.AddError("-Output option has invalid value", false);
                            }
                        }
                        break;

                    //  logic functions with value
                    case LexicalCodes.OutputLogicRisingEdgeUpCounter:
                    case LexicalCodes.OutputLogicFallingEdgeUpCounter:
                        //  only one logic function per equation
                        if (haveOutputLogic)
                            EquationConverters.AddError("-Output options may only include one of activity monitor, counter, toggle or delay", false);
                        haveOutputLogic = true;

                        //  make sure value is given
                        if (symbols[++i].Code != LexicalCodes.Value)
                            EquationConverters.AddError("-Output option is missing value", false);
                        else
                        {
                            try
                            {
                                test = symbols[i].IntValue;
                            }
                            catch
                            {
                                EquationConverters.AddError("-Output option has invalid value", false);
                            }
                        }
                        break;

                    //  logic functions without value
                    case LexicalCodes.OutputLogicRisingEdgeToggle:
                    case LexicalCodes.OutputLogicFallingEdgeToggle:
                        //  only one logic function per equation
                        if (haveOutputLogic)
                            EquationConverters.AddError("-Output options may only include one of activity monitor, counter, toggle or delay", false);
                        haveOutputLogic = true;

                        //  make sure value is not given
                        if (((i + 1) < symbols.Count) && (symbols[i + 1].Code == LexicalCodes.Value))
                            EquationConverters.AddError("-Output option does not require value", false);
                        break;
                    
                    //  logic that affects other tokens
                    case LexicalCodes.OutputLogicRisingEdgeSkipToggle:
                    case LexicalCodes.OutputLogicFallingEdgeSkipToggle:
                    case LexicalCodes.OutputLogicRisingEdgeVariableClear:
                    case LexicalCodes.OutputLogicFallingEdgeVariableClear:
                        //  validate token
                        ++i;
                        ValidateToken(symbols, ref i, "-Output option ");
                        break;

                    //  token output options without value
                    case LexicalCodes.OutputSendTokenOnChange:
                    case LexicalCodes.OutputSendTokenOnOutputRisingEdge:
                    case LexicalCodes.OutputSendTokenOnOutputFallingEdge:
                        //  only one token send per equation
                        haveTokenSend = true;
                        break;

                    //  token output options with value
                    case LexicalCodes.OutputSendTokenOnOutputRisingByValue:
                    case LexicalCodes.OutputSendTokenOnOutputFallingByValue:
                        //  only one token send per equation
                        haveTokenSend = true;

                        //  make sure value is given
                        if (symbols[++i].Code != LexicalCodes.Value)
                            EquationConverters.AddError("-Output option is missing value", false);
                        try
                        {
                            test = symbols[i].IntValue;
                        }
                        catch
                        {
                            EquationConverters.AddError("-Output option has invalid value", false);
                        }
                        break;

                    default:
                        EquationConverters.AddError("-Output option with unknown expression", false);
                        break;
                }
            }
        }

        /// <summary>
        /// Validates a set of token symbols.
        /// </summary>
        /// <param name="symbols">The symbols to check.</param>
        /// <param name="index">The index within the symbols at the token start.</param>
        /// <param name="errorPrefix">A prefix for any generated token error message.  May be null.</param>
        /// <returns>A non-used test value.</returns>
        public static int ValidateToken(List<Symbol> symbols, ref int index, string errorPrefix)
        {
            //  non-used test value
            int test = 0;

            //  check error prefix
            if (errorPrefix == null)
                errorPrefix = string.Empty;

            //  set error reporting
            EquationConverters.ErrorReportingFilename = symbols[index].FileName;
            EquationConverters.ErrorReportingLineNumber = symbols[index].LineNumber;

            //  token open
            if (symbols[index].Code != LexicalCodes.TokenKey)
                EquationConverters.AddError(errorPrefix + "token opening brace error", false);
            ++index;

            //  key value
            if (symbols[index].Code != LexicalCodes.Value)
                EquationConverters.AddError(errorPrefix + "token key error", false);
            try
            {
                test = symbols[index].UInt16Value;
            }
            catch
            {
                EquationConverters.AddError(errorPrefix + "token key error", false);
            }
            ++index;

            //  if next symbol is token address
            if (symbols[index].Code == LexicalCodes.TokenAddress)
            {
                //  address value
                ++index;
                if (symbols[index].Code != LexicalCodes.Value)
                    EquationConverters.AddError(errorPrefix + "token address error", false);
                try
                {
                    test = symbols[index].ByteValue;
                }
                catch
                {
                    EquationConverters.AddError(errorPrefix + "token address error", false);
                }
                ++index;
            }

            //  token close
            if (symbols[index].Code != LexicalCodes.TokenKeyClose)
                EquationConverters.AddError(errorPrefix + "token closing brace error.", false);

            //  return value
            return test;
        }


    }
}
