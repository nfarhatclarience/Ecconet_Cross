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
        /// Returns a copy of the given expression, with the copy's repeated sections replaced with the steps they represent.
        /// CAUTION: This method does not (yet) support nested expressions.
        /// </summary>
        /// <param name="exp">The input expression.</param>
        /// <returns>Returns a copy of the given expression, with the copy's repeated sections replaced with the steps they represent.</returns>
        public static Expression RemoveExpressionRepeatedSections(Expression exp)
        {
            //  validate input
            if (exp == null)
                return null;

            //  get deep copy of the given expression
            var expression = exp.Copy();

            //  expand repeated sections
            int rssRepeats = 0;
            int rssStartIndex = 0;
            int rssEndIndex = 0;
            for (int i = 0; i < expression.Entries.Count; ++i)
            {
                var entry = expression.Entries[i];
                if (entry is RepeatSectionStart rss)
                {
                    //  capture repeats and next step index
                    rssRepeats = rss.Repeats;
                    rssStartIndex = i;
                    expression.Entries.RemoveAt(i);
                }
                else if (entry is RepeatSectionEnd)
                {
                    //  save entry index
                    rssEndIndex = i;
                    if (rssEndIndex > rssStartIndex)
                    {
                        //  get steps in between
                        List<Entry> repeatedSectionSteps = new List<Entry>(rssEndIndex - rssStartIndex);
                        for (int n = rssStartIndex; n < rssEndIndex; ++n)
                            if (expression.Entries[n] is Step step)
                                repeatedSectionSteps.Add(step);

                        while (--rssRepeats > 0)
                        {
                            foreach (var step in repeatedSectionSteps)
                            {
                                expression.Entries.Insert(rssEndIndex, step.Copy());
                                ++rssEndIndex;
                            }
                        }
                        expression.Entries.RemoveAt(rssEndIndex);
                    }
                }
            }
            return expression;
        }

        /// <summary>
        /// Returns a copy of the given expression collection, with the copied expressions' repeated sections replaced with the steps they represent.
        /// CAUTION: This method does not (yet) support nested expressions.
        /// </summary>
        /// <param name="exp">The input expression.</param>
        /// <returns>Returns a copy of the given expression collection, with the copied expression's repeated sections
        /// replaced with the steps they represent.</returns>
        public static ExpressionCollection RemoveCollectionRepeatedSections(ExpressionCollection ec)
        {
            var expressionCollection = ec.DeepCopy();
            for (int i = 0; i < expressionCollection.Expressions.Count; ++i)
                expressionCollection.Expressions[i] = RemoveExpressionRepeatedSections(expressionCollection.Expressions[i]);
            return expressionCollection;
        }

        /// <summary>
        /// Refactors an area key of the given expression.
        /// CAUTION: This method does not (yet) support nested expressions.
        /// </summary>
        /// <param name="exp">The expression with an area to refactor.</param>
        /// <param name="oldKey">The old area key to be replaced.</param>
        /// <param name="newKey">The new area key.</param>
        public static void RefactorAreaKey(Expression exp, UInt16 oldKey, UInt16 newKey)
        {
            //  refactor the area
            foreach (Area a in exp.Areas)
            {
                if (a.Key == oldKey)
                    a.Key = newKey;
            }

            //  refactor all steps
            foreach (Entry entry in exp.Entries)
            {
                if (entry is Step step)
                {
                    foreach (Token token in step.Tokens)
                        if (token.Key == oldKey)
                            token.Key = newKey;
                }
            }
        }

        /// <summary>
        /// Returns the overall period for the given expression.
        /// CAUTION: This method does not (yet) support nested expressions.
        /// </summary>
        /// <param name="exp">The expression to measure.</param>
        /// <returns>Returns the overall period for the given expression.</returns>
        public static uint ExpressionPeriod(Expression exp)
        {
            uint period = 0;
            uint repeats = 1;
            foreach (var entry in exp.Entries)
            {
                if (entry is Step step)
                    period += (step.Period * repeats);
                else if (entry is RepeatSectionStart rss)
                    repeats = rss.Repeats;
                else if (entry is RepeatSectionEnd)
                    repeats = 1;
            }
            return period;
        }

        /// <summary>
        /// Returns statistics for the given expression.
        /// CAUTION: This method does not (yet) support nested expressions.
        /// </summary>
        /// <param name="exp">The expression to measure.</param>
        /// <param name="totalPeriod">Total expression period output.</param>
        /// <param name="minStepPeriod">Minimum step period output.</param>
        /// <param name="maxStepPeriod">Maximum step period output.</param>
        /// <param name="totalNumSteps">Total number of steps output.</param>
        public static void ExpressionStats(Expression exp, out uint totalPeriod, out uint minStepPeriod, out uint maxStepPeriod, out uint totalNumSteps)
        {
            //  preset the values
            totalPeriod = 0;
            minStepPeriod = uint.MaxValue;
            maxStepPeriod = 0;
            totalNumSteps = 0;

            //  validate input
            if (exp == null)
                return;

            //  for all steps
            uint repeats = 1;
            foreach (var entry in exp.Entries)
            {
                if (entry is Step step)
                {
                    totalPeriod += (step.Period * repeats);
                    totalNumSteps += repeats;
                    if (minStepPeriod > step.Period)
                        minStepPeriod = step.Period;
                    if (maxStepPeriod < step.Period)
                        maxStepPeriod = step.Period;
                }
                else if (entry is RepeatSectionStart rss)
                    repeats = rss.Repeats;
                else if (entry is RepeatSectionEnd)
                    repeats = 1;
            }
        }


        /// <summary>
        /// Returns statistics for the given expression collection.
        /// CAUTION: This method does not (yet) support nested expressions.
        /// </summary>
        /// <param name="exp">The expression to measure.</param>
        /// <param name="totalPeriod">Total period of all the expression output.</param>
        /// <param name="minStepPeriod">Minimum step period of all expressions output.</param>
        /// <param name="maxStepPeriod">Maximum step period of all expressions output.</param>
        /// <param name="totalNumSteps">Total number of steos of all the expression output.</param>
        public static void ExpressionCollectionStats(ExpressionCollection ec, out uint totalPeriod,
            out uint minStepPeriod, out uint maxStepPeriod, out uint totalNumSteps)
        {
            //  preset the values
            totalPeriod = 0;
            minStepPeriod = uint.MaxValue;
            maxStepPeriod = 0;
            totalNumSteps = 0;

            //  validate input
            if (ec == null)
                return;

            foreach (var exp in ec.Expressions)
            {
                //  get expression stats
                ExpressionStats(exp, out uint expTotalPeriod, out uint expMinStepPeriod,
                    out uint expMaxStepPeriod, out uint expTotalNumSteps);

                //  accumulate stats for collection
                totalPeriod += expTotalPeriod;
                totalNumSteps += expTotalNumSteps;
                if (minStepPeriod > expMinStepPeriod)
                    minStepPeriod = expMinStepPeriod;
                if (maxStepPeriod < expMaxStepPeriod)
                    maxStepPeriod = expMaxStepPeriod;
            }
        }

        /// <summary>
        /// Gets statistics on a light engine dictionary binary of one or more dictionaries.
        /// </summary>
        /// <param name="lightEngineDictionary">The light engine dictionary binary.</param>
        /// <param name="dictionaryEntrySize">The light engine dictionary entry size in bytes.</param>
        /// <param name="totalNumDictionaries">The total number of dictionaries in the binary.</param>
        /// <param name="totalUniqueSteps">The total unique steps, not including the zero key entries.</param>
        /// <param name="totalNumEntries">The total number of entries in the binary.</param>
        /// <param name="minEntries">The minimum number of entries in any one dictionary.</param>
        /// <param name="maxEntries">The maximum number of entries in any one dictionary.</param>
        public static void LightEngineDictionaryStats(byte[] lightEngineDictionary, uint entrySize,
            out uint totalNumDictionaries, out uint totalUniqueSteps,
            out uint totalNumEntries, out uint minEntries, out uint maxEntries)
        {
            //  preset the output values
            totalNumDictionaries = 0;
            totalNumEntries = 0;
            totalUniqueSteps = 0;
            minEntries = uint.MaxValue;
            maxEntries = 0;

            //  validate input
            if (lightEngineDictionary == null)
                return;

            //  unique steps and num entries
            List<UInt16> uniqueStepKeys = new List<UInt16>(500);
            int numEntries = -1;

            //  for all dictionary entries
            for (int index = 0; index < lightEngineDictionary.Length; index += (int)entrySize)
            {
                //  get the dictionary entry key
                UInt16 key = (UInt16)((lightEngineDictionary[index + 1] << 8) | lightEngineDictionary[index]);

                //  if dictionary start zero key
                if (0 == key)
                {
                    ++totalNumDictionaries;
                    if (numEntries >= 0)
                    {
                        if (minEntries > (uint)numEntries)
                            minEntries = (uint)numEntries;
                        if (maxEntries < (uint)numEntries)
                            maxEntries = (uint)numEntries;
                    }
                    numEntries = 0;
                }
                else
                {
                    if (!uniqueStepKeys.Contains(key))
                        uniqueStepKeys.Add(key);
                }
                ++numEntries;
                ++totalNumEntries;
            }

            //  get last dictionary stats
            if (minEntries > (uint)numEntries)
                minEntries = (uint)numEntries;
            if (maxEntries < (uint)numEntries)
                maxEntries = (uint)numEntries;

            //  get total unique keys
            totalUniqueSteps = (uint)uniqueStepKeys.Count;
        }

    }
}
