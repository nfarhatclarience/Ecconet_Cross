/**
  ******************************************************************************
  * @file       SymbolCalculator.cs
  * @copyright  © 2017 ECCO Group.  All rights reserved.
  * @author     M. Latham, Liquid Logic
  * @version    1.0.0
  * @date       12/1/2017
  *
  * @brief      Converts a math and/or logic expression into an integer result.
  *             The expression is given as a sequence of symbols.
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

using LexicalCodes = ESG.BytecodeLib.Symbol.LexicalCodes;


namespace ESG.BytecodeLib
{
    public static class SymbolCalculator
    {
        /// <summary>
        /// Operator priority dictionary, where lowest int is highest priority.
        /// </summary>
        private static Dictionary<LexicalCodes, int> OperatorPresedence = new Dictionary<LexicalCodes, int>()
        {
            { LexicalCodes.OperatorLogicalNot,               0 },
            { LexicalCodes.OperatorBitwiseInvert,            0 },
            { LexicalCodes.OperatorMultiply,                 1 },
            { LexicalCodes.OperatorDivide,                   1 },
            { LexicalCodes.OperatorModulus,                  1 },
            { LexicalCodes.OperatorAdd,                      2 },
            { LexicalCodes.OperatorSubtract,                 2 },
            { LexicalCodes.OperatorShiftLeft,                3 },
            { LexicalCodes.OperatorShiftRight,               3 },
            { LexicalCodes.OperatorIsLessThan,               4 },
            { LexicalCodes.OperatorIsLessThanOrEqual,        4 },
            { LexicalCodes.OperatorIsGreaterThan,            4 },
            { LexicalCodes.OperatorIsGreaterThanOrEqual,     4 },
            { LexicalCodes.OperatorIsEqual,                  5 },
            { LexicalCodes.OperatorIsNotEqual,               5 },
            { LexicalCodes.OperatorBitwiseAnd,               6 },
            { LexicalCodes.OperatorBitwiseXor,               7 },
            { LexicalCodes.OperatorBitwiseOr,                8 },
            { LexicalCodes.OperatorLogicalAnd,               9 },
            { LexicalCodes.OperatorLogicalOr,               10 },
            { LexicalCodes.OperatorConditionalQuestion,     11 },
            { LexicalCodes.OperatorConditionalSeparator,    11 },
            { LexicalCodes.OperatorOpenParentheses,         12 },
            { LexicalCodes.OperatorCloseParentheses,        12 },
        };

        /// <summary>
        /// Performs a calculation with the given expression symbols.
        /// </summary>
        /// <param name="symbols">The expression to calculate.</param>
        /// <returns>Returns the result of the calculation.</returns>
        public static int PerformCalculation(List<Symbol> symbols, out int operandStackDepth, out int operatorStackDepth)
        {
            //  create stacks
            var operandStack = new List<int>(50);
            var operatorStack = new List<LexicalCodes>(50);
            operandStackDepth = 0;
            operatorStackDepth = 0;


            //	while codes to analyze
            for (int i = 0; i < symbols.Count; ++i)
            {
                //  get symbol
                var symbol = symbols[i];

                //	if an open parenthesis operator
                if (symbol.Code == LexicalCodes.OperatorOpenParentheses)
                {
                    operatorStack.Add(symbol.Code);
                    if (operatorStackDepth < operatorStack.Count)
                        operatorStackDepth = operatorStack.Count;
                }

                //	else if a close parenthesis operator
                else if (symbol.Code == LexicalCodes.OperatorCloseParentheses)
                {
                    //	while backing up to left parenthesis,
                    //	pop operands, perform calc, and push result
                    while (operatorStack.Count > 0)
                    {
                        if (operatorStack[operatorStack.Count - 1] == LexicalCodes.OperatorOpenParentheses)
                        {
                            operatorStack.RemoveAt(operatorStack.Count - 1);
                            break;
                        }
                        UnwindStacks(operandStack, operatorStack);
                    }
                }

                //	else if constant value operand
                else if (symbol.Code == LexicalCodes.Value)
                {
                    //  push operand
                    operandStack.Add(symbol.IntValue);
                    if (operandStackDepth < operandStack.Count)
                        operandStackDepth = operandStack.Count;
                }

                //  else if token key
                else if (symbol.Code == LexicalCodes.TokenKey)
                {
                    //  next symbol should be token value
                    symbol = symbols[++i];
                    if (symbol.Code != LexicalCodes.Value)
                        EquationConverters.AddError("Invalid token key", symbol.FileName, symbol.LineNumber, false);

                    //  place 1 on stack, since the token value is unknown
                    operandStack.Add(1);
                    if (operandStackDepth < operandStack.Count)
                        operandStackDepth = operandStack.Count;

                    //  if next symbol is token address, then skip it and its value
                    ++i;
                    if (symbols[i].Code == LexicalCodes.TokenAddress)
                        i += 2;

                    //  next symbol should be token key close  
                    if (symbols[i].Code != LexicalCodes.TokenKeyClose)
                        EquationConverters.AddError("Invalid token key", symbol.FileName, symbol.LineNumber, false);
                }

                //	else is operator
                else
                {
                    int currentOpPrecedence = 255;
                    int prevOpPrecedence = 255;

                    //	get operator precedence
                    if (!OperatorPresedence.TryGetValue(symbol.Code, out currentOpPrecedence))
                    {
                        EquationConverters.AddError("Calculation has invalid operator", symbol.FileName, symbol.LineNumber, false);
                        return 0;
                    }

                    //  get previous operator precedense
                    if ((operatorStack.Count > 0)
                        && (!OperatorPresedence.TryGetValue(operatorStack[operatorStack.Count - 1], out prevOpPrecedence)))
                        EquationConverters.AddError("Calculation has invalid operator", symbol.FileName, symbol.LineNumber, false);

                    //	if have operator on top of stack with higher precedence than current op,
                    //	then unwind the stack
                    if (currentOpPrecedence > prevOpPrecedence)
                        UnwindStacks(operandStack, operatorStack);

                    //	push the current operator
                    operatorStack.Add(symbol.Code);
                    if (operatorStackDepth < operatorStack.Count)
                        operatorStackDepth = operatorStack.Count;
                }
            }

            //	unwind the stack
            while ((operatorStack.Count > 0) && (operandStack.Count > 0))
                UnwindStacks(operandStack, operatorStack);

            //	return the calculated value
            if (operandStack.Count == 1)
                return operandStack[0];
            return EquationConverters.AddError("Calculation error", false);
        }

        /// <summary>
        /// Unwinds the stack through one iteration, and places the result on the operand stack.
        /// </summary>
        /// <param name="operandStack">The operand stack.</param>
        /// <param name="operatorStack">The operator stack.</param>
        private static void UnwindStacks(List<int> operandStack, List<LexicalCodes> operatorStack)
        {
            LexicalCodes stackOp;
            int operand1, operand2, operand3;

            if ((operatorStack.Count > 0) && (operandStack.Count > 0))
            {
                //	pop operator
                stackOp = operatorStack[operatorStack.Count - 1];
                operatorStack.RemoveAt(operatorStack.Count - 1);

                //	if unary operator bitwise invert
                if (stackOp == LexicalCodes.OperatorBitwiseInvert)
                {
                    //  pop operand
                    operand1 = ~operandStack[operandStack.Count - 1];
                    operandStack.RemoveAt(operandStack.Count - 1);
                }

                //	if unary operator logical not
                else if (stackOp == LexicalCodes.OperatorLogicalNot)
                {
                    //  pop operand
                    operand1 = (operandStack[operandStack.Count - 1] == 0) ? 1 : 0;
                    operandStack.RemoveAt(operandStack.Count - 1);
                }

                //	else if conditional operator
                else if (LexicalCodes.OperatorConditionalSeparator == stackOp)
                {
                    if ((operandStack.Count < 3) || (operatorStack.Count < 1))
                        EquationConverters.AddError("Calculation conditional operation error", false);

                    //  pop operands
                    operand2 = operandStack[operandStack.Count - 1];
                    operandStack.RemoveAt(operandStack.Count - 1);

                    operand1 = operandStack[operandStack.Count - 1];
                    operandStack.RemoveAt(operandStack.Count - 1);

                    operand3 = operandStack[operandStack.Count - 1];
                    operandStack.RemoveAt(operandStack.Count - 1);

                    //	pop operator
                    stackOp = operatorStack[operatorStack.Count - 1];
                    operatorStack.RemoveAt(operatorStack.Count - 1);

                    //  set operand
                    if (LexicalCodes.OperatorConditionalQuestion == stackOp)
                        operand1 = (operand3 != 0) ? operand1 : operand2;
                }

                //	else other operators
                else
                {
                    if (operandStack.Count < 2)
                        EquationConverters.AddError("Calculation error", false);

                    //  pop operands
                    operand2 = operandStack[operandStack.Count - 1];
                    operandStack.RemoveAt(operandStack.Count - 1);

                    operand1 = operandStack[operandStack.Count - 1];
                    operandStack.RemoveAt(operandStack.Count - 1);

                    //  calculate operand based on code
                    operand1 = Calc(operand1, operand2, stackOp);
                }

                //  push the resulting operand
                operandStack.Add(operand1);
            }
            else  //  input error
            {
                EquationConverters.AddError("Calculation error", false);
            }
        }

        /// <summary>
        /// Perform a calculation with the given operands and operator.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="op">The operator.</param>
        /// <returns>The result of the calculation.</returns>
        private static int Calc(int leftOperand, int rightOperand, LexicalCodes op)
        {
            switch (op)
            {
                case LexicalCodes.OperatorMultiply:
                    return leftOperand * rightOperand;
                case LexicalCodes.OperatorDivide:
                    return leftOperand / rightOperand;
                case LexicalCodes.OperatorModulus:
                    return leftOperand % rightOperand;
                case LexicalCodes.OperatorAdd:
                    return leftOperand + rightOperand;
                case LexicalCodes.OperatorSubtract:
                    return leftOperand - rightOperand;
                case LexicalCodes.OperatorShiftLeft:
                    return leftOperand << rightOperand;
                case LexicalCodes.OperatorShiftRight:
                    return leftOperand >> rightOperand;
                case LexicalCodes.OperatorIsLessThan:
                    return (leftOperand < rightOperand) ? 1 : 0;
                case LexicalCodes.OperatorIsLessThanOrEqual:
                    return (leftOperand <= rightOperand) ? 1 : 0;
                case LexicalCodes.OperatorIsGreaterThan:
                    return (leftOperand > rightOperand) ? 1 : 0;
                case LexicalCodes.OperatorIsGreaterThanOrEqual:
                    return (leftOperand >= rightOperand) ? 1 : 0;
                case LexicalCodes.OperatorIsEqual:
                    return (leftOperand == rightOperand) ? 1 : 0;
                case LexicalCodes.OperatorIsNotEqual:
                    return (leftOperand != rightOperand) ? 1 : 0;
                case LexicalCodes.OperatorBitwiseAnd:
                    return leftOperand & rightOperand;
                case LexicalCodes.OperatorBitwiseXor:
                    return leftOperand ^ rightOperand;
                case LexicalCodes.OperatorBitwiseOr:
                    return leftOperand | rightOperand;
                case LexicalCodes.OperatorLogicalAnd:
                    return ((leftOperand != 0) && (rightOperand != 0)) ? 1 : 0;
                case LexicalCodes.OperatorLogicalOr:
                    return ((leftOperand != 0) || (rightOperand != 0)) ? 1 : 0;
                default:
                    return EquationConverters.AddError("Unrecognized operator", false);
            }
        }


    }
}
