using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using ESG.ExpressionLib.DataModels;

using PathValuePeriod = ESG.ExpressionLib.DataConverters.ExpressionConverters.StepDictionaryValue.PathValuePeriod;


namespace ESG.ExpressionLib.DataConverters
{
    public static partial class ExpressionConverters
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Builds dictionary bin files for output arrays whose outputs operate independently.
        /// </summary>
        /// <param name="stepMasterDict">A step method master step dictionary.</param>
        /// <param name="binStepDictionaries">The resulting bin dictionary table.</param>
        private static void BuildDictionaryBinFile(StepDictionary stepMasterDict, out byte[] binStepDictionaries)
        {
            //  validate inputs
            if (stepMasterDict == null)
                throw new Exception("BuildLightEngineDictionaryBinFile() given null master step dictionary.");

            //  create step bin dictionaries
            Dictionary<int, List<byte>> stepDictionaries = new Dictionary<int, List<byte>>(50);

            //  for all steps in master step dictionary
            foreach (KeyValuePair<int, StepDictionaryValue> stepDictEntry in stepMasterDict)
            {
                foreach (var outputPath in stepDictEntry.Value.OutputPaths)
                {
                    //  if path is for a unison output
                    if (outputPath.Path.Contains('/'))
                        throw new Exception("BuildDictionaryBinFile() given unison output path.");
                    int outputId = outputPath.EndpointId;

                    //  if the output's step dictionary not created yet, then create it
                    if (!stepDictionaries.ContainsKey(outputId))
                    {
                        stepDictionaries.Add(outputId, new List<byte>(100));
                        byte[] zeroEntry = new byte[4];
                        zeroEntry[0] = 0;
                        zeroEntry[1] = 0;
                        zeroEntry[2] = (byte)outputId;
                        zeroEntry[3] = (byte)(outputId >> 8);
                        stepDictionaries[outputId].AddRange(zeroEntry);
                    }

                    //  priority and priority reset
                    byte priority = stepDictEntry.Value.Priority;
                    if (priority > 7)
                        priority = 7;
                    priority <<= 4;
                    if (stepDictEntry.Value.IsReset)
                        priority |= 0x80;

                    //  add entry to step dictionary
                    byte[] bytes = new byte[4];
                    bytes[0] = (byte)stepDictEntry.Key;
                    bytes[1] = (byte)(stepDictEntry.Key >> 8);
                    bytes[2] = priority;
                    bytes[3] = (byte)outputPath.Value;
                    stepDictionaries[outputId].AddRange(bytes);
                }
            }

            //  concatenate all light engine dictionaries into one bin file
            List<byte> binDict = new List<byte>(4096);

            //  add file key in little-endian format
            binDict.Add((byte)(stepDictionaryFileKey & 0xff));
            binDict.Add((byte)((stepDictionaryFileKey >> 8) & 0xff));
            binDict.Add((byte)((stepDictionaryFileKey >> 16) & 0xff));
            binDict.Add((byte)((stepDictionaryFileKey >> 24) & 0xff));

            //  add each dictionary
            foreach (var value in stepDictionaries.Values)
                binDict.AddRange(value);
            binStepDictionaries = binDict.ToArray();
        }

    }
}


