/**
  ******************************************************************************
  * @file       CommentRemover.cs
  * @author     M. Latham, Liquid Logic
  * @version    1.0.0
  * @date       Dec 2017
  *
  * @brief      Class for removing comments from C language text.
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
    public static class CommentRemover
    {
        /// <summary>
        /// Removes all C-style comments from the given text.  Will throw on commenting errors.
        /// </summary>
        /// <param name="text">The text from which to remove comments.</param>
        /// <returns>A text with no comments.</returns>
        public static string Remove(string text)
        {
            return Remove(text, 1);
        }
            
        /// <summary>
        /// Removes all C-style comments from the given text.  Will throw on commenting errors.
        /// </summary>
        /// <param name="text">The text from which to remove comments.</param>
        /// <param name="lineNumber">The starting line number of the text.</param>
        /// <returns>A text with no comments.</returns>
        public static string Remove(string text, int lineNumber)
        {
            //  make a copy of the given text
            var str = string.Copy(text);
  
            //  set line number
            EquationConverters.ErrorReportingLineNumber = lineNumber;

            try
            {
                //  for all characters in text
                for (int index = 0; index < str.Length; ++index)
                {
                    //  track line number
                    if (str[index] == '\n')
                    {
                        ++lineNumber;
                        EquationConverters.ErrorReportingLineNumber = lineNumber;
                    }

                    //  ignore white space
                    if (Char.IsWhiteSpace(str[index]))
                        continue;

                    //  if a comment start
                    if (str[index] == '/')
                    {
                        int endIndex = -1;

                        //  if a single-line comment
                        if (str[index + 1] == '/')
                        {
                            //  remove text, except for newline
                            endIndex = str.IndexOf('\n', index + 1);
                            if (endIndex == -1)
                                endIndex = str.Length;
                            str = str.Remove(index, endIndex - index);
                            --index;
                        }

                        //  else if delimited comments
                        else if (str[index + 1] == '*')
                        {
                            //  get end of comment index
                            endIndex = str.IndexOf("*/", index + 1);
                            if (endIndex == -1)
                                EquationConverters.AddError("File ends with unclosed comment", false);
                            endIndex += 2;

                            //  get number of newlines within comment
                            int numLines = 0;
                            for (int n = index; n < endIndex; ++n)
                            {
                                if (str[n] == '\n')
                                    ++numLines;
                            }

                            //  remove text, except for newlines
                            str = str.Remove(index, endIndex - index);
                            for (int n = 0; n < numLines; ++n)
                                str = str.Insert(index, "\r\n");
                            --index;
                        }
                    }

                    //  else if a string
                    else if (str[index] == '"')
                    {
                        //  get end of quotes
                        int endIndex = index;
                        do
                        {
                            endIndex = str.IndexOf("\"", endIndex + 1);
                            if (endIndex == -1)
                                EquationConverters.AddError("File ends with unclosed quote", false);

                        } while (str[endIndex - 1] == '\\');

                        //  get number of newlines within quotes
                        int numLines = 0;
                        for (int n = index; n < endIndex; ++n)
                        {
                            if (str[n] == '\n')
                                ++numLines;
                        }
                        index = endIndex;
                    }
                }
            }
            catch (Exception ex)
            {
                if ((ex.Message != null) && ex.Message.StartsWith("__"))
                    throw new Exception(ex.Message);
                EquationConverters.AddError("File ends with unclosed comment", false);
            }

            //  return the text without comments
            return str;
        }

    }
}
