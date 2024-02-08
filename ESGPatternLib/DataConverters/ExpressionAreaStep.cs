using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESG.ExpressionLib.DataModels;

using Area = ESG.ExpressionLib.DataModels.Expression.Area;
using Entry = ESG.ExpressionLib.DataModels.Expression.Entry;
using Step = ESG.ExpressionLib.DataModels.Expression.Step;
using RepeatSectionStart = ESG.ExpressionLib.DataModels.Expression.RepeatSectionStart;
using RepeatSectionEnd = ESG.ExpressionLib.DataModels.Expression.RepeatSectionEnd;
using Token = ESG.ExpressionLib.DataModels.Expression.Token;

namespace ESG.ExpressionLib.DataConverters
{
    public static partial class ExpressionConverters
    {
        /// <summary>
        /// Contains step info.
        /// </summary>
        public class AreaStep : IComparable
        {
            /// <summary>
            /// The area step key.
            /// </summary>
            public int Key;

            /// <summary>
            /// The area step value.
            /// </summary>
            public int Value;

            /// <summary>
            /// The area step period.
            /// </summary>
            public UInt16 Period;

            /// <summary>
            ///  The accumulated expression time.
            /// </summary>
            public uint ExpressionTime;


            /// <summary>
            /// Compares the keys of this AreaStep and the given AreaStep.
            /// </summary>
            /// <param name="obj">The AreaStep for which to compare.</param>
            /// <returns>Returns the compare result.</returns>
            public int CompareTo(object obj)
            {
                if (obj == null)
                    return 1;
                if (obj is AreaStep areaStep)
                    return Key.CompareTo(areaStep.Key);
                throw new Exception("Trying to compare AreaStep to object that is not an AreaStep.");
            }

            /// <summary>
            /// Returns a value indicating whether this AreaStep has the same Key, Value and Period as the given AreaStep.
            /// </summary>
            /// <param name="obj">The AreaStep for which to compare.</param>
            /// <returns>Returns a value indicating whether this AreaStep has the same Key, Value and Period as the given AreaStep.</returns>
            public override bool Equals(object obj)
            {
                if (obj is AreaStep areaStep)
                    return ((Key == areaStep.Key) && (Value == areaStep.Value) && (Period == areaStep.Period));
                return false;
            }

            /// <summary>
            /// Returns the hash code.
            /// </summary>
            /// <returns>Returns the hash code.</returns>
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }


        /// <summary>
        /// An expression is a chronological series of interlaced area events, embodied in a data model, XML or JSON.
        /// This method de-interlaces the area events to provide separate chronological series for each of an expression's areas.
        /// </summary>
        /// <param name="exp">Returns a dictionary of an expression's step information, organized by expression areas.</param>
        /// <returns></returns>
        public static Dictionary<UInt16, List<AreaStep>> DeinterlacedExpression(Expression exp)
        {
            Dictionary<UInt16, List<AreaStep>> dict = new Dictionary<UInt16, List<AreaStep>>(500);

            foreach (var area in exp.Areas)
            {
                //  create new dictionary entry
                List<AreaStep> stepInfo = new List<AreaStep>(exp.Entries.Count);
                dict.Add(area.Key, stepInfo);

                //  accumulated time and step info list
                uint expressionTime = 0;

                //  for all expression entries
                foreach (var entry in exp.Entries)
                {
                    if (entry is Step step)
                    {
                        //  for all tokens in step
                        foreach (var token in step.Tokens)
                        {
                            if (token.Key == area.Key)
                            {
                                if (stepInfo.Count > 0)
                                    stepInfo[stepInfo.Count - 1].Period = (UInt16)(expressionTime - stepInfo[stepInfo.Count - 1].ExpressionTime);
                                stepInfo.Add(new AreaStep() { Key = token.Key, Value = token.Value, ExpressionTime = expressionTime });
                            }
                        }
                        expressionTime += step.Period;
                    }
                }

                //  add last time period
                if (stepInfo.Count > 0)
                    stepInfo[stepInfo.Count - 1].Period = (UInt16)(expressionTime - stepInfo[stepInfo.Count - 1].ExpressionTime);
            }

            //  return the expression area step info dictionary
            return dict;
        }
    }
}
