using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ESG.ExpressionLib.DataModels;

using Step = ESG.ExpressionLib.DataModels.Expression.Step;
using Token = ESG.ExpressionLib.DataModels.Expression.Token;


namespace ESG.ExpressionLib.DataConverters
{
    public class ExpressionTest
    {

        /// <summary>
        /// Prints an area timing.
        /// </summary>
        /// <param name="exp">The expression with the area.</param>
        /// <param name="areaKey">The area key.</param>
        [Conditional("DEBUG")]
        public static void PrintExpressionAreaPeriodsAndValues(Expression exp, UInt16 areaKey)
        {
            //  validate test input
            if ((exp == null) || (exp.Areas.Count == 0) || (exp.Entries.Count == 0))
            {
                Console.WriteLine("PrintExpressionAreaPeriodsAndValues: Invalid expression.");
                return;
            }

            //  get total expression time
            uint totalTime = ExpressionConverters.ExpressionPeriod(exp);

            //  get dictionary of area steps
            var expressionAreaSteps = ExpressionConverters.DeinterlacedExpression(exp);
            Debug.Assert(expressionAreaSteps != null);
            if (expressionAreaSteps == null)
                return;

            //  get area steps for given key
            List<ExpressionConverters.AreaStep> stepInfo = expressionAreaSteps[areaKey];
            Debug.Assert((stepInfo != null) && (stepInfo.Count > 0));
            if ((stepInfo == null) || (stepInfo.Count == 0))
                return;

            //  get steps accumulated time
            uint accumulatedTime = 0;
            foreach (var areaStepInfo in stepInfo)
                accumulatedTime += areaStepInfo.Period;

            //  compare times
            Debug.Assert(Math.Abs(accumulatedTime - totalTime) < 3);

            //  print results
            Console.WriteLine("===============================================================");
            Console.WriteLine("Total expression period: {0} mS", totalTime);
            Console.WriteLine("Total accumulated step period: {0} mS", accumulatedTime);
            foreach (var si in stepInfo)
                Console.WriteLine("Key {0}, \tValue {1}, \tPeriod {2}, {3}\tTime {4}",
                    areaKey, si.Value, si.Period, (si.Period < 99) ? "\t" : "", si.ExpressionTime);
        }


        /// <summary>
        /// Prints a step method expression collection and bin file statistics.
        /// </summary>
        /// <param name="ec">The expression collection.</param>
        /// <param name="patternBin">The pattern binary.</param>
        /// <param name="dictionaryBin">The dictionary binary that includes one or more light engine dictionaries.</param>
        /// <param name="dictionaryEntrySize">The light engine dictionary entry size in bytes.</param>
        [Conditional("DEBUG")]
        public static void PrintExpressionAndBinFileStats(ExpressionCollection ec,
            byte[] patternBin, byte[] dictionaryBin, uint dictionaryEntrySize)
        {
            //  statistics
            //  ===========================================================

            //  get expression stats
            ExpressionConverters.ExpressionCollectionStats(ec, out uint totalPeriod, out uint minStepPeriod,
                out uint maxStepPeriod, out uint totalNumSteps);

            //  get light engine dictionary stats
            ExpressionConverters.LightEngineDictionaryStats(dictionaryBin, dictionaryEntrySize,
                out uint totalNumDictionaries, out uint totalUniqueSteps,
                out uint totalNumDictionaryEntries, out uint minDictionaryEntries, out uint maxDictionaryEntries);

            //  print the stats
            Console.WriteLine("");
            Console.WriteLine("===============================================================================");
            Console.WriteLine("EXPRESSIONS:");
            foreach (Expression exp in ec.Expressions)
                Console.WriteLine(exp.ToString());
            Console.WriteLine("Step periods mS: min = {0}, max = {1}, avg = {2:0.0}",
                minStepPeriod, maxStepPeriod, totalPeriod / totalNumSteps);
            Console.WriteLine("");
            Console.WriteLine("LBC AND LE BIN WITH TIMERS FILE STATISTICS:");
            Console.WriteLine("Total unique steps: {0}", totalUniqueSteps);
            Console.WriteLine("Participating Light Engines: {0}", totalNumDictionaries);
            Console.WriteLine("Light Bar Controller pattern table bytes: {0}", patternBin.Length);
            Console.WriteLine("Light Engine combined dictionary bytes: {0}", dictionaryBin.Length);
            Console.WriteLine("Individual Light Engine dictionary bytes: min = {0}, max = {1}, avg = {2:0.000}",
                minDictionaryEntries * dictionaryEntrySize, maxDictionaryEntries * dictionaryEntrySize,
                (float)(totalNumDictionaryEntries * dictionaryEntrySize) / totalNumDictionaries);
            Console.WriteLine("===============================================================================");
        }


    }
}



#if UNUSED_CODE

    public class ExpressionTest
    {
        /// <summary>
        /// Contains step info.
        /// </summary>
        private class StepInfo
        {
            //  the area key
            public UInt16 key;

            //  the area value
            public int value;

            //  the step time
            public UInt16 period;

            //  the accumulated time
            public uint accumulatedTime;
        }



        /// <summary>
        /// Prints an area timing.
        /// </summary>
        /// <param name="exp">The expression with the area.</param>
        /// <param name="areaKey">The area key.</param>
        public static void PrintExpressionAreaPeriodsAndValues(Expression exp, UInt16 areaKey)
        {
            //  validate input
            if ((exp == null) || (exp.Areas.Count == 0) || (exp.Entries.Count == 0))
            {
                Console.WriteLine("PrintExpressionAreaPeriodsAndValues: Invalid expression.");
                return;
            }

            //  get total expression time
            uint totalTime = ExpressionConverters.ExpressionPeriod(exp);
            Console.WriteLine("===============================================================");
            Console.WriteLine("Total expression period: {0} mS", totalTime);

            //  accumulated time and step info list
            uint accumulatedTime = 0;
            List<StepInfo> stepInfo = new List<StepInfo>(exp.Entries.Count);

            //  for all expression entries
            foreach (var entry in exp.Entries)
            {
                if (entry is Step step)
                {
                    //  for all tokens in step
                    foreach (var token in step.Tokens)
                    {
                        if (token.Key == areaKey)
                        {
                            if (stepInfo.Count > 0)
                                stepInfo[stepInfo.Count - 1].period = (UInt16)(accumulatedTime - stepInfo[stepInfo.Count - 1].accumulatedTime);
                            stepInfo.Add(new StepInfo() { key = areaKey, value = token.Value, accumulatedTime = accumulatedTime });
                        }
                    }
                    accumulatedTime += step.Period;
                }
            }

            //  add last time period
            if (stepInfo.Count > 0)
                stepInfo[stepInfo.Count - 1].period = (UInt16)(accumulatedTime - stepInfo[stepInfo.Count - 1].accumulatedTime);

            //  print results
            Console.WriteLine("Total accumulated period: {0} mS", accumulatedTime);
            foreach (var si in stepInfo)
            {
                Console.WriteLine("Key {0}, \tValue {1}, \tPeriod {2}, \t\tTime {3}", areaKey, si.value, si.period, si.accumulatedTime);
            }

            //  check total time
            Debug.Assert(Math.Abs(accumulatedTime - totalTime) < 3);
        }



    }

#endif
