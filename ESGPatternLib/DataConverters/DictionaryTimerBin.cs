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
        /// <summary>
        /// Builds timer dictionary bin files.
        /// </summary>
        /// <param name="stepMasterDict">A step method master step dictionary.</param>
        /// <param name="outputs">A collection of output devices with same tree structure as light engines.</param>
        /// <param name="binStepDictionaries">The resulting bin dictionary table.</param>
        private static void BuildTimerDictionaryBinFile(StepDictionary stepMasterDict, OutputArrayNode outputs, out byte[] binStepDictionaries)
        {
            //  validate inputs
            if (stepMasterDict == null)
                throw new Exception("BuildLightEngineDictionaryBinFile() given null master step dictionary.");

            //  create step bin dictionaries
            Dictionary<int, List<byte>> stepDictionaries = new Dictionary<int, List<byte>>(50);

            //  for all steps in master step dictionary
            foreach (KeyValuePair<int, StepDictionaryValue> stepDictEntry in stepMasterDict)
            {
                //  separate all of the step's output paths into groups for each output device
                Dictionary<int, List<PathValuePeriod>> outputPathLists = new Dictionary<int, List<PathValuePeriod>>(50);
                foreach (var outputPath in stepDictEntry.Value.OutputPaths)
                {
                    var rootId = outputPath.RootId;

                    //  if found new output device add to dictionary
                    if (!outputPathLists.ContainsKey(rootId))
                        outputPathLists.Add(rootId, new List<PathValuePeriod>(10));

                    //  add the output path to the list
                    outputPathLists[rootId].Add(outputPath);
                }

                //  add step's values to the step bin dictionaries
                foreach (KeyValuePair<int, List<PathValuePeriod>> pathList in outputPathLists)
                {
                    //  get the output device associated with the output path list
                    ComponentTreeNode outputDevice = null;
                    foreach (var od in outputs.ChildNodes)
                    {
                        if (od.Id == pathList.Key)
                        {
                            outputDevice = od;
                            break;
                        }
                    }

                    if (outputDevice == null)
                    {
                        _logger.Warn("No output device with Id {0} found for expression.", pathList.Key);
                        continue;
                    }

                    //  if the output device's step dictionary not created yet, then create it
                    if (!stepDictionaries.ContainsKey(pathList.Key))
                    {
                        stepDictionaries.Add(pathList.Key, new List<byte>(100));
                        byte[] zeroEntry = new byte[6];
                        zeroEntry[0] = 0;
                        zeroEntry[1] = 0;
                        zeroEntry[2] = (byte)outputDevice.Id;
                        zeroEntry[3] = (byte)(outputDevice.Id >> 8);
                        zeroEntry[4] = (byte)(outputDevice.Id >> 16);
                        zeroEntry[5] = (byte)(outputDevice.Id >> 24);
                        stepDictionaries[pathList.Key].AddRange(zeroEntry);
                    }

                    //  priority and priority reset
                    byte priority = stepDictEntry.Value.Priority;
                    if (priority > 7)
                        priority = 7;
                    priority <<= 4;
                    if (stepDictEntry.Value.IsReset)
                        priority |= 0x80;

                    //  for all of device's output paths included in step
                    byte period = 0;
                    UInt16 value = 0;
                    foreach (var outputPath in pathList.Value)
                    {
                        //  get longest on output time (they all should be the same)
                        if (period < outputPath.Period)
                            period = (byte)outputPath.Period;

                        //  get index of output device endpoint that matches step path 
                        //int outputIndex = LightEnginePathToOutputIndex(outputDevice, outputPath.Path);

                        //  get output path endpoint Id, and if valid then add to value
                        int outputIndex = outputPath.EndpointId;
                        if (outputIndex >= 0)
                        {
                            //  convert LED intensity to log index and add to value
                            UInt16 intensityIndex = LogIntensity.IntensityToIndex((byte)outputPath.Value, LogIntensity.IntensityBits.Bits2);
                            value |= (UInt16)(intensityIndex << (outputIndex * 2));
                        }
                    }

                    //  add entry to light engine step dictionary
                    byte[] bytes = new byte[6];
                    bytes[0] = (byte)stepDictEntry.Key;
                    bytes[1] = (byte)(stepDictEntry.Key >> 8);
                    bytes[2] = period;
                    bytes[3] = priority;
                    bytes[4] = (byte)(value >> 8);
                    bytes[5] = (byte)value;
                    stepDictionaries[pathList.Key].AddRange(bytes);
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

        /// <summary>
        /// Gets the step method LED index for a given path.
        /// </summary>
        /// <param name="lightEngine">The light engine to which the path belongs.</param>
        /// <param name="path">The path the the LED endpoint.</param>
        /// <returns>Returns the step method LED index for a given path.</returns>
        private static int LightEnginePathToOutputIndex(ComponentTreeNode lightEngine, string path)
        {
            //  make sure the light engine and path belong to each other
            if (PathValue.GetRootId(path) != lightEngine.Id)
                throw new Exception("LightEnginePathToOutputIndex() has incorrect combination of light engine and path.");

            try
            {
                //  create ordered list of the light engine's colors
                var colorNodes = lightEngine.ChildNodes.ToList();
                colorNodes.Sort();

                //  get path's led string
                int startIndex = path.LastIndexOf('/') + 1;
                string ledString = path.Substring(startIndex);

                //  get path's color string
                int endIndex = startIndex;
                startIndex = path.Substring(0, startIndex).LastIndexOf('/') + 1;
                string colorString = path.Substring(startIndex, endIndex - startIndex);

                //  get index
                bool colorFound = false;
                int index = 0;
                foreach (var colorNode in colorNodes)
                {
                    //  if color found, then done adding color output indices
                    if (colorString.Equals(colorNode.GetType().GetTypeInfo().GetProperty("Color").GetValue(colorNode)))
                    {
                        colorFound = true;
                        break;
                    }

                    //  add number of color outpus to index
                    if (colorNode.ChildNodes != null)
                        index += colorNode.ChildNodes.Count;
                }
                if (!colorFound)
                    index = 0;
                index += PathValue.GetEndpointId(path);
                if (index > 5)
                    index = 5;
                return index;
            }
            catch
            {
                return 0;
            }
        }


    }
}

#if UNUSED_CODE

            //  create dictionary of output endpoint paths from the given output devices (aka light engine or other similar device)
            //Dictionary<int, string[]> endpointDict = new Dictionary<int, string[]>(50);
            //foreach (var outputDevice in outputDevices)
            //    endpointDict.Add(outputDevice.Id, PathValue.UniqueOutputPaths(outputDevice));

#endif
