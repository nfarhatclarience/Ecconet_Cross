using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
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
        /// Finds a common period for a list of expressions.
        /// CAUTION: This method does not (yet) support nested expressions.
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        /// <param name="periodTolerance">The allowable individual period deviation (0.01 equals 1%).</param>
        /// <param name="maxPeriod">The maximum common expression period in milliseconds.</param>
        /// <returns>Returns a common period for the list of expressions, or zero on error.</returns>
        public static uint FindBestCommonPeriod(List<Expression> expressions, double periodTolerance, uint maxPeriod)
        {
#if DEBUG
            Console.WriteLine("");
            Console.WriteLine("===============================================================================");
            Console.WriteLine("EXPRESSION PERIODS (PRE-FACTOR):");
#endif
            //  get list of periods
            List<double> periods = new List<double>(expressions.Count);
            double longestInputPeriod = double.MaxValue;
            foreach (var exp in expressions)
            {
                double period = ExpressionPeriod(exp);
                periods.Add(period);
                if (longestInputPeriod > period)
                    longestInputPeriod = period;
#if DEBUG
                Console.WriteLine("Period: {0:0.0}", period);
#endif
            }

            //  check longest input period
            if (longestInputPeriod > maxPeriod)
                return 0;
            
            //  for all resolutions
            double bestPeriod = 0;
            double bestPeriodMaxError = double.MaxValue;
            bool meetsTolerance = false;
            double shortestPeriodThatMeetsTolerance = double.MaxValue;
            double shortestPeriodTolerance = 0;
            for (double period = longestInputPeriod; period <= maxPeriod; ++period)
            {
                double maxPeriodExpressionError = 0;
                foreach (var p in periods)
                {
                    //  note that an integer number of this expression period 'p'
                    //  could either almost fit or almost not fit within the given test 'period'
                    //
                    //  for example, the expression could fit 3.9 times (almost fit) or
                    //  fit 4.1 times (almost not fit)
                    //
                    //  in either case the absolute percentage error is evaluated

                    //  calculate the expression period error
                    double quotient = period / p;
                    double factor = quotient / Math.Round(quotient);
                    double error_pcnt = (factor >= 1) ? factor - 1 : 1 - factor;
                    
                    //  get max expression error for period
                    if (maxPeriodExpressionError < error_pcnt)
                        maxPeriodExpressionError = error_pcnt;
                }

                //  capture the period that produces the lowest maximum individual expression error
                if (bestPeriodMaxError > maxPeriodExpressionError)
                {
                    bestPeriodMaxError = maxPeriodExpressionError;
                    bestPeriod = period;
                }

                //  capture the shortest period that meets the tolerance requirement
                if (!meetsTolerance && (maxPeriodExpressionError <= periodTolerance))
                {
                    meetsTolerance = true;
                    shortestPeriodThatMeetsTolerance = period;
                    shortestPeriodTolerance = maxPeriodExpressionError;
                }
            }
#if DEBUG
            //  stats
            uint shortestPeriod = (uint)Math.Round((bestPeriod < shortestPeriodThatMeetsTolerance) ?
                bestPeriod : shortestPeriodThatMeetsTolerance);
            double spt = (bestPeriod < shortestPeriodThatMeetsTolerance) ?
                bestPeriodMaxError : shortestPeriodTolerance;
            Console.WriteLine("");
            Console.WriteLine("===============================================================================");
            Console.WriteLine("EXPRESSION BLEND TIMING:");
            Console.WriteLine("Inputs: Period tolerance: {0:0.00000}, Max allowable period: {1}", periodTolerance, maxPeriod);
            Console.WriteLine("Period: {0} mS, Worst case error: {1:0.000}%", shortestPeriod, spt * 100);
#endif
            //  return the best result
            return (uint)Math.Round((bestPeriod < shortestPeriodThatMeetsTolerance) ?
                bestPeriod : shortestPeriodThatMeetsTolerance);
        }

        /// <summary>
        /// Factors the overall expression period by re-timing the steps.
        /// This method replaces repeated sections with steps to avoid accumulated timing errors.
        /// CAUTION: This method does not (yet) support nested expressions.
        /// </summary>
        /// <param name="_exp">The expression to factor.</param>
        /// <param name="factor">The ratio of new period / old period.</param>
        /// <param name="stepResolution_ms">The step period resolution in milliseconds.</param>
        /// <returns>Returns a new time-factored expression.</returns>
        public static Expression FactorExpressionPeriod(Expression _exp, double factor, uint stepResolution_ms)
        {
            //  validate input
            if (_exp == null)
                return null;

            //  get deep copy of expression with repeated sections replaced with steps
            var exp = RemoveExpressionRepeatedSections(_exp);

            // keep master times to avoid accumulative error
            uint originalTime = 0;
            uint factoredTime = 0;
            foreach (var entry in exp.Entries)
            {
                if (entry is Step step)
                {
                    originalTime += step.Period;
                    uint targetTime = (uint)Math.Round(originalTime * factor / stepResolution_ms) * stepResolution_ms;
                    step.Period = (UInt16)(targetTime - factoredTime);
                    factoredTime += step.Period;
                }
            }

            //  return the new factored expression
            return exp;
        }

        /// <summary>
        /// Blends two or more expressions by expanding repeated sections, making areas unique, and adjusting timings.
        /// The resulting expression will have no area LED assignments.
        /// CAUTION: This method does not (yet) support nested expressions.
        /// </summary>
        /// <param name="_expressions">A ist of expressions to blend.</param>
        /// <param name="periodTolerance">The allowable individual period deviation (0.01 equals 1%).</param>
        /// <param name="maxPeriod_ms">The maximum blended expression period in milliseconds.</param>
        /// <param name="stepResolution_ms">The step period resolution in milliseconds.</param>
        /// <returns>A new merged expression.</returns>
        public static Expression Blend(List<Expression> _expressions, double periodTolerance, uint maxPeriod_ms, uint stepResolution_ms)
        {
            //  validate inputs
            if ((_expressions == null) || (_expressions.Count < 2))
                throw new Exception("Expression blending requires two or more expressions to blend.");
            
            //  determine the common overall period for each of the expressions
            uint bestPeriod = FindBestCommonPeriod(_expressions, periodTolerance, maxPeriod_ms);
            //  if best period cannot be found, return null 
            if (bestPeriod == 0)
                return null;

            //  create new list of time-factored expressions
            List<Expression> expressions = new List<Expression>(_expressions.Count);
            foreach (var _exp in _expressions)
            {
                //  calculate the expression period factor
                double quotient = (double)bestPeriod / ExpressionPeriod(_exp);
                double factor = quotient / Math.Round(quotient);

                //  get new time-factored expression
                expressions.Add(FactorExpressionPeriod(_exp, factor, stepResolution_ms));
            }

            //  make unique area keys for expressions and clear any LED assignments
            MakeUniqueAreaKeys(expressions);

            //  consolidate the expressions into one
            var blendedExpression = ConsolidateExpressions(expressions, bestPeriod, stepResolution_ms);
#if DEBUG
            //  stats
            //for (UInt16 key = 500; key < 508; ++key)
            //    ExpressionTest.PrintExpressionAreaPeriodsAndValues(blendedExpression, key);
            Console.WriteLine("");
            Console.WriteLine("===============================================================================");
            Console.WriteLine("EXPRESSION BLEND:");
            Console.WriteLine("Total expression entries: {0}, Step resolution mS: {1}", blendedExpression.Entries.Count, stepResolution_ms);
#endif
            //  return the blended expression
            return blendedExpression;
        }

        /// <summary>
        /// Makes all the areas key unique for the given list of expressions.
        /// Re-assigns token keys that match changed area keys.
        /// </summary>
        /// <param name="expressions">The list of expressions.</param>
        private static void MakeUniqueAreaKeys(List<Expression> expressions)
        {
            //  validate inputs
            if ((expressions == null) || (expressions.Count == 0))
                return;

            //  vars
            List<UInt16> keys = new List<UInt16>(40);
            UInt16 maxKey = 0;
            UInt16 areaIndex = 0;

            //  for all expressions in list
            foreach (var exp in expressions)
            {
                //  for all expression areas
                foreach (var area in exp.Areas)
                {
                    //  set area index incrementally
                    area.Index = ++areaIndex;

                    //  track highest used key
                    if (maxKey < area.Key)
                        maxKey = area.Key;

                    //  if this area key already exists in the expressions' areas
                    if (keys.Contains(area.Key))
                    {
                        //  create a new key for area and assign to all same keys in expression's step entries
                        UInt16 newKey = ++maxKey;
                        foreach (var entry in exp.Entries)
                        {
                            if (entry is Step step)
                            {
                                foreach (var token in step.Tokens)
                                    if (token.Key == area.Key)
                                        token.Key = newKey;
                            }
                        }
                        area.Key = newKey;
                    }
                    keys.Add(area.Key);

                    //  eliminate any previously-assigned output paths
                    area.OutputPaths.Clear();
                }
            }
        }

        /// <summary>
        /// Returns an expression from a consolidated list of expressions.
        /// Note that this private method operates on expressions that have been factored via FactorExpressionPeriod().
        /// </summary>
        /// <param name="expressions">The list of expressions to consolidate.</param>
        /// <param name="totalPeriod_ms">The total period of the consolidated expression, in milliseconds.</param>
        /// <param name="minStepPeriod_ms">The step period resolution of the consolidated expression, in milliseconds.</param>
        /// <returns>Returns an expression from a consolidated list of expressions.</returns>
        private static Expression ConsolidateExpressions(List<Expression> expressions, uint totalPeriod_ms, uint minStepPeriod_ms)
        {
            //  create a new compound output expression
            Expression consolidatedExpression = new Expression();

            //  create list of expression cycles within total period
            uint[] expressionCycles = new uint[expressions.Count];
            for (int i = 0; i < expressions.Count; ++i)
                expressionCycles[i] = (uint)Math.Round((double)totalPeriod_ms / ExpressionPeriod(expressions[i]));

            //  add areas to consolidated expression
            foreach (var exp in expressions)
            {
                foreach (var area in exp.Areas)
                    consolidatedExpression.Areas.Add(area);
            }

            //  create arrays for expression entry indices and step times
            int[] expressionEntryIndices = new int[expressions.Count];
            uint[] expressionStepTimes = new uint[expressions.Count];

            //  add steps to expression
            uint time = 0;
            uint prevStepTime = 0;
            Step prevStep = null;
            for (; time < totalPeriod_ms; time += minStepPeriod_ms)
            {
                //  new step
                Step step = null;

                //  for all expressions
                for (int i = 0; i < expressions.Count; ++i)
                {
                    //  get expression and if time for next step
                    var exp = expressions[i];
                    if (expressionStepTimes[i] == time)
                    {
                        //  get current expression entry
                        int entryIndex = expressionEntryIndices[i];
                        if (entryIndex < exp.Entries.Count)
                        {
                            var entry = exp.Entries[entryIndex];
                            if (entry is Step expressionStep)
                            {
                                //  if new step not created for this time, then create it
                                if (step == null)
                                    step = new Step();

                                //  set default period
                                step.Period = 6;

                                //  add tokens to step
                                foreach (var token in expressionStep.Tokens)
                                    step.Tokens.Add(token);

                                //  set next step time
                                expressionStepTimes[i] += expressionStep.Period;
                            }

                            //  bump to next entry
                            if (++expressionEntryIndices[i] >= exp.Entries.Count)
                            {
                                if (--expressionCycles[i] > 0)
                                    expressionEntryIndices[i] = 0;
                                else
                                    expressionEntryIndices[i] = exp.Entries.Count;
                            }
                        }
                    }
                }

                //  if have new step
                if (step != null)
                {
                    //  add it to the blended expression (without the period set)
                    consolidatedExpression.Entries.Add(step);

                    //  set the period for the previous step
                    if (prevStep != null)
                        prevStep.Period = (UInt16)(time - prevStepTime);

                    //  record the previous step
                    prevStep = step;
                    prevStepTime = time;
                }
            }

            //  set time of last step
            if (prevStep != null)
            {
                if (totalPeriod_ms > prevStepTime)
                    prevStep.Period = (UInt16)(totalPeriod_ms - prevStepTime);
                else
                    prevStep.Period = (UInt16)minStepPeriod_ms;
            }

            //  return consolidated expression
            return consolidatedExpression;
        }

    }
}


#if UNUSED_CODE

        /// <summary>
        /// Finds a common period for a list of expressions.
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        /// <param name="msMinResolution">The minimum overall expression period resolution in mS.</param>
        /// <returns>Returns a common period for a list of expressions</returns>
        public static void FindCommonPeriod(List<Expression> expressions, uint msMinResolution,
            out uint leastCommonMultiple, out List<uint> finalPeriods)
        {
            /*
            //  get list of unique periods at the given resolution
            List<uint> periods = new List<uint>(expressions.Count);
            foreach (var exp in expressions)
            {
                uint period = ExpressionPeriod(exp);
                period /= msResolution;
                if ((period > 0) && !periods.Contains(period))
                    periods.Add(period);
            }
            */

            //  <<-- TEST
            msMinResolution = 25;
            //List<uint> periods = new List<uint>() { 4, 7, 12, 21, 42 };
            List<uint> periods = new List<uint>() { 1000, 800, 750, 521, 500, 400, 250, 200, 160 };
            finalPeriods = new List<uint>();
            List<uint> samplePeriods = new List<uint>();

            //  set default output values
            leastCommonMultiple = UInt32.MaxValue;
            uint resolution = 1;

            //  for all resolutions
            uint msResolution = 1;
            for (; msResolution <= msMinResolution; ++msResolution)
            {
                //  get list of periods at current resolution
                List<uint> coarsePeriods = new List<uint>(periods.Count);
                List<uint> actualPeriods = new List<uint>(periods.Count);
                foreach (var period in periods)
                {
                    //  get coarsened short and long periods
                    uint shortPeriod = (period / msResolution) * msResolution;
                    if (shortPeriod < (uint)((float)period * 0.9))
                        shortPeriod = (uint)((float)period * 0.9);
                    uint longPeriod = shortPeriod + msResolution;
                    if (longPeriod > (uint)((float)period * 1.1))
                        longPeriod = (uint)((float)period * 1.1);
                    uint coarsePeriod = ((period - shortPeriod) <= (longPeriod - period)) ? shortPeriod : longPeriod;
                    if (coarsePeriod > 0)
                    {
                        coarsePeriods.Add(coarsePeriod);
                        actualPeriods.Add(coarsePeriod);
                    }
                }

                //  get list of factors
                List<uint> factors = new List<uint>(100);
                bool done = false;
                uint currentFactor = 2;
                while (!done)
                {
                    done = true;
                    bool divided = false;
                    for (int i = 0; i < coarsePeriods.Count; ++i)
                    {
                        if (((coarsePeriods[i] / currentFactor) * currentFactor) == coarsePeriods[i])
                        {
                            divided = true;
                            coarsePeriods[i] /= currentFactor;
                        }
                        if (coarsePeriods[i] != 1)
                            done = false;
                    }
                    if (divided)
                        factors.Add(currentFactor);
                    else
                        ++currentFactor;
                }

                //  multiply factors to get common multiple
                uint commonPeriod = 1;
                foreach (var f in factors)
                    commonPeriod *= f;
                if (leastCommonMultiple > commonPeriod)
                {
                    leastCommonMultiple = commonPeriod;
                    resolution = msResolution;
                    finalPeriods = actualPeriods;
                }
            }
            //leastCommonMultiple *= resolution;
        }

        /// <summary>
        /// Factors the overall expression period by re-timing the steps.
        /// </summary>
        /// <param name="_exp">The expression to factor.</param>
        /// <param name="factor">The ratio of new period / old period.</param>
        /// <returns>Returns a new time-factored expression.</returns>
        public static Expression FactorExpressionPeriod(Expression _exp, double factor)
        {
            //  validate input
            if (_exp == null)
                return null;

            //  get deep copy of expression
            var exp = _exp.Copy();

            // keep master times to avoid accumulative error
            uint originalTime = 0;
            uint factoredTime = 0;
            uint rssRepeats = 0;
            uint rssOriginalTime = 0;
            uint rssFactoredTime = 0;
            foreach (var entry in exp.Entries)
            {
                if (entry is Step step)
                {
                    originalTime += step.Period;
                    step.Period = (UInt16)(Math.Round(originalTime * factor) - factoredTime);
                    factoredTime += step.Period;
                }
                else if (entry is RepeatSectionStart rss)
                {
                    //  capture repeats and original time
                    rssRepeats = rss.Repeats;
                    rssOriginalTime = originalTime;
                    rssFactoredTime = factoredTime;
                }
                else if (entry is RepeatSectionEnd rse)
                {
                    //  advance original time and factored time by the number of repeats
                    originalTime += ((originalTime - rssOriginalTime) * (rssRepeats - 1));
                    factoredTime += ((factoredTime - rssFactoredTime) * (rssRepeats - 1));
                }
            }

            //  return the new factored expression
            return exp;
        }

#endif

#if UNUSED_CODE

        /// <summary>
        /// Finds a common period for a list of expressions.
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        /// <param name="maxError_mS">The maximum allowable milliseconds period error for any expression.</param>
        /// <param name="maxError_pcnt">The maximum allowable percent period error for any expression.</param>
        /// <param name="maxPeriod">The maximum common expression period in milliseconds.</param>
        /// <returns>Returns a common period for the list of expressions, or zero on error.</returns>
        public static uint FindBestCommonPeriod(List<Expression> expressions, uint maxError_mS, double maxError_pcnt, uint maxPeriod)
        {
            /*
            //  get list of periods
            List<double> periods = new List<double>(expressions.Count);
            uint longestInputPeriod = uint.MaxValue;
            foreach (var exp in expressions)
            {
                double period = ExpressionPeriod(exp);
                periods.Add(period);
                if (longestInputPeriod > period)
                    longestInputPeriod = period;
            }
            */

            //  <<-- TEST
            maxError_mS = 25;
            maxError_pcnt = 0.05;
            maxPeriod = 15000;
            List<double> periods = new List<double>() { 1000, 800, 750, 521, 500, 400, 250, 200, 160 };
            double longestInputPeriod = 1000;

            //  check longest input period
            if (longestInputPeriod > maxPeriod)
                return 0;

            //  for all resolutions
            double bestPeriod = 0;
            double maxTotalError = double.MaxValue;
            for (double period = longestInputPeriod; period <= maxPeriod; ++period)
            {
                bool periodOK = true;
                double totalError = 0;
                foreach (var p in periods)
                {
                    //  get error
                    double error_mS = period % p;
                    double error_pcnt = error_mS / p;
                    if (error_pcnt > 0.5)
                        error_pcnt = 1.0 - error_pcnt;

                    //  if error too great, then continue to next period
                    if ((error_mS > maxError_mS) || (error_pcnt > maxError_pcnt))
                    {
                        periodOK = false;
                        break;
                    }
                    totalError += error_mS;
                }

                if (periodOK && (maxTotalError > totalError))
                {
                    maxTotalError = totalError;
                    bestPeriod = period;
                }
            }
            return (uint)Math.Round(bestPeriod);
        }
#endif


