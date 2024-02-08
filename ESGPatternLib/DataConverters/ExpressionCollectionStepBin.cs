using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECCONet;
using ESG.ExpressionLib.DataModels;

using Keys = ECCONet.Token.Keys;
using Area = ESG.ExpressionLib.DataModels.Expression.Area;
using Step = ESG.ExpressionLib.DataModels.Expression.Step;
using Token = ESG.ExpressionLib.DataModels.Expression.Token;
using PathValuePeriod = ESG.ExpressionLib.DataConverters.ExpressionConverters.StepDictionaryValue.PathValuePeriod;


namespace ESG.ExpressionLib.DataConverters
{
    /// <summary>
    /// Class to convert an expression collection into a light bar bin files.
    /// The bin file are a pattern table and light engine dictionary to be distributed to the light engines.
    /// </summary>
    public static partial class ExpressionConverters
    {
        /// <summary>
        /// Converts an expression collection into a step method expression collection and a step method master step dictionary.
        /// CAUTION: This method does not yet support nested expressions.
        /// </summary>
        /// <param name="ec">The expression collection to convert.</param>
        /// <param name="usingOutputTimers">True if output devices will be using output on timers.</param>
        /// <param name="stepExpColl">The resulting step method expression collection.</param>
        /// <param name="stepMasterDict">The resulting step method master step dictionary.</param>
        private static void ECtoStepMethodECandDictionary(ExpressionCollection ec, bool usingOutputTimers,
            out ExpressionCollection stepExpColl, out StepDictionary stepMasterDict)
        {
            //  validate inputs
            if (ec == null)
                throw new Exception("ConvertExpCollToStepMethodExpCollAndStepDictionary() given null expression collection.");

            //  create a copy of the expression collection with the repeated sections replaced with individual steps
            //  the area tokens in each expression's steps will be replaced with a KeyStepMethodDictionaryKey token
            stepExpColl = RemoveCollectionRepeatedSections(ec);

            //  create a new dictionary of steps
            stepMasterDict = new StepDictionary();

            //  for each expression in collection
            foreach (Expression exp in stepExpColl.Expressions)
            {
                //  create a dictionary of each area's output paths for quick indexing with step token key
                Dictionary<ushort, List<PathValue>> areaOutputPaths = new Dictionary<ushort, List<PathValue>>(exp.Areas.Count);
                foreach (Area area in exp.Areas)
                    areaOutputPaths.Add(area.Key, area.OutputPaths.ToList());

                //  add a step dictionary entry to represents the expression areas' default values, unless a matching entry already exists
                //  this will replace the expression's area(s) with a StepMethodDictionaryKey token
                stepMasterDict.AddExpressionAreasStep(exp);

                //  for all steps in expression
                for (int index = 0; index < exp.Entries.Count(); ++index)
                {
                    var entry = exp.Entries[index];
                    if (entry is Step step)
                    {
                        //  get composite list of all output paths for this step
                        //  each step token represents one area with a specific output value
                        var dictValue = new StepDictionaryValue() { Priority = exp.OutputPriority, IsReset = false };
                        foreach (var area in step.Tokens)
                        {
                            //  make sure token key is included in all-off step
                            if (!areaOutputPaths.ContainsKey(area.Key))
                                throw new Exception(string.Format("Building bin files, expression {0} contains token not included in expression areas.", exp.Name));

                            //  get output paths that token represents
                            var outputPaths = areaOutputPaths[area.Key];

                            //  if not using timers
                            if (!usingOutputTimers)
                            {
                                //  add all output paths to dictionary value, factoring expression and token intensity value
                                foreach (var outputPath in outputPaths)
                                    dictValue.AddOutputPath(new PathValuePeriod()
                                    {
                                        Path = string.Copy(outputPath.Path),
                                        Value = (exp.Value * area.Value * outputPath.Value / 10000),
                                        Period = 0
                                    });
                            }
                            else  //  using timers
                            {
                                if ((area.Value > 0) || (exp.Entries.Count == 1))
                                {
                                    //  perform look-ahead to determine how long area should be on
                                    //  does not matter if next step found with same area has same value
                                    uint period = step.Period;
                                    bool nextMatchingAreaFound = false;
                                    for (int n = index + 1; n < exp.Entries.Count(); ++n)
                                    {
                                        //  if expression entry is a step
                                        if (exp.Entries[n] is Step stp)
                                        {
                                            //  check to see if step has same area token
                                            foreach (var tk in stp.Tokens)
                                            {
                                                if (exp.TransparentOffSteps)
                                                {
                                                    if (tk.Key == area.Key || tk.Value < 100)
                                                    {
                                                        nextMatchingAreaFound = true;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    if (tk.Key == area.Key)
                                                    {
                                                        nextMatchingAreaFound = true;
                                                        break;
                                                    }
                                                }
                                            }

                                            //  if matching area found, then end of timer period, else add step's period and keep looking
                                            if (nextMatchingAreaFound)
                                                break;
                                            else
                                                period += stp.Period;
                                        }
                                    }

                                    //  add all output paths to dictionary value, factoring expression and token intensity value
                                    foreach (var outputPath in outputPaths)
                                        dictValue.AddOutputPath(new PathValuePeriod()
                                        {
                                            Path = string.Copy(outputPath.Path),
                                            Value = (exp.Value * area.Value * outputPath.Value / 10000),
                                            Period = (int)period
                                        });
                                }
                            }
                        }

                        //  if step has output paths
                        if (dictValue.OutputPaths.Count > 0)
                        {
                            //  if not using output timers
                            if (!usingOutputTimers)
                            {
                                //  add dictionary value to step dictionary and get the key
                                //  if value already exists, then just returns the key of the existing value
                                int dictKey = stepMasterDict.AddStep(dictValue);

                                //  replace the expression step token(s) with a StepMethodDictionaryKey token
                                step.Tokens = new BindingList<Token>() { new Token((UInt16)Keys.KeyStepMethodDictionaryKey, dictKey) };
                            }
                            else  //  using timers
                            {
                                //  if a single-step steady/cut and period is <= 200 mS
                                if ((exp.Entries.Count == 1) && (dictValue.OutputPaths[0].Period <= 200))
                                {
                                    //  add 55 mS overlap
                                    foreach (var pathValuePeriod in dictValue.OutputPaths)
                                        pathValuePeriod.Period += 55;

                                    //  add dictionary value to step dictionary and get the key
                                    //  if value already exists, then just returns the key of the existing value
                                    int dictKey = stepMasterDict.AddStep(dictValue);

                                    //  replace the expression step token(s) with a StepMethodDictionaryKey token
                                    step.Tokens = new BindingList<Token>() { new Token((UInt16)Keys.KeyStepMethodDictionaryKey, dictKey) };
                                }

                                //  else if multi-step and period is <= 255
                                else if ((exp.Entries.Count > 1) && (dictValue.OutputPaths[0].Period <= 255))
                                {
                                    //  add dictionary value to step dictionary and get the key
                                    //  if value already exists, then just returns the key of the existing value
                                    int dictKey = stepMasterDict.AddStep(dictValue);

                                    //  replace the expression step token(s) with a StepMethodDictionaryKey token
                                    step.Tokens = new BindingList<Token>() { new Token((UInt16)Keys.KeyStepMethodDictionaryKey, dictKey) };
                                }

                                //  else step needs to be broken into sub-steps because timers are one-byte
                                else
                                {
                                    //  get period and number of sub-steps required for total on period
                                    //  timers are set to 240 mS max plus 15 mS overlap
                                    int period = dictValue.OutputPaths[0].Period;
                                    int numSubSteps = period / 240 + (((period % 240) > 0) ? 1 : 0);

                                    //  get sub-period and remainder
                                    int subPeriod = period / numSubSteps;
                                    int remainder = period % subPeriod;

                                    //  all sub-steps will be sub-period plus 15 mS overlap
                                    foreach (var pathValuePeriod in dictValue.OutputPaths)
                                        pathValuePeriod.Period = subPeriod + 15;

                                    //  add dictionary value to step dictionary and get the key
                                    //  if value already exists, then just returns the key of the existing value
                                    int dictKey = stepMasterDict.AddStep(dictValue);

                                    //  for all sub-steps
                                    for (int i = 0; i < numSubSteps; ++i)
                                    {
                                        //  if last sub-step
                                        if (i == (numSubSteps - 1))
                                        {
                                            //  adjust step already in expression to match sub-period
                                            step.Period = (UInt16)(subPeriod + 15);
                                            step.Tokens = new BindingList<Token>() { new Token((UInt16)Keys.KeyStepMethodDictionaryKey, dictKey) };
                                        }
                                        //  else if second to last sub-step
                                        else if (i == (numSubSteps - 2))
                                        {
                                            //  add step to expression with period adjusted to get correct overall period
                                            exp.Entries.Insert(index, new Expression.Step((UInt16)(subPeriod - 15 + remainder),
                                                new BindingList<Token>() { new Token((UInt16)Keys.KeyStepMethodDictionaryKey, dictKey) }));
                                            ++index;
                                        }
                                        //  else any other sub-step
                                        else
                                        {
                                            //  add step to expression with sub-step period
                                            exp.Entries.Insert(index, new Expression.Step((UInt16)subPeriod,
                                                new BindingList<Token>() { new Token((UInt16)Keys.KeyStepMethodDictionaryKey, dictKey) }));
                                            ++index;
                                        }
                                    }
                                }
                            }
                        }
                        else  //  no output paths for this step
                        {
                            //  if previous step exists, then add this expression step's period to previous step
                            if (index > 0)
                            {
                                var prevEntry = exp.Entries[index - 1];
                                if (prevEntry is Step prevStep)
                                    prevStep.Period += step.Period;
                                //Console.Write("Time added to prev step, ");
                            }

                            //  remove the expression step
                            exp.Entries.RemoveAt(index);
                            --index;
                            //Console.Write("step removed, ");
                        }
                    }
                }

                //  <stats> get and print total period
                //Console.WriteLine("Expression period {0}", ExpressionPeriod(exp));
            }
        }


    }
}



#if LBC_LE_NO_TIMERS
        /// <summary>
        /// Converts an expression collection into the simplified 4-byte step method
        /// binary pattern table and light engine dictionaries.
        /// Note: The expression collection must include the area LED assignments,
        /// and each LED must have the LightEngineID and Color fields set. 
        /// </summary>
        /// <param name="_ec">The expression collection to convert.</param>
        /// <param name="lightBar">The target light bar.</param>
        /// <param name="binPatternTable">The binary pattern table output.</param>
        /// <param name="binLEDictionaries">The binary light engine dictionary output.</param>
        /// <returns>Returns true on success.</returns>
        public static bool BuildLbcLeFiles(ExpressionCollection _ec, LightBar lightBar,
            out byte[] binPatternTable, out byte[] binLEDictionaries)
        {
            binPatternTable = null;
            binLEDictionaries = null;

            //  validate inputs
            if ((_ec == null) || (lightBar == null))
                return false;

            //  create a copy of the expression collection
            //  the area tokens in each expression's steps will be replaced with a KeyStepMethodDictionaryKey token
            ExpressionCollection ec = _ec.Copy();

            //  create a new dictionary of steps
            Dictionary<int, StepDictionaryValue> stepDict = new Dictionary<int, StepDictionaryValue>(100);

            //  for each expression in collection
            foreach (Expression exp in ec.Expressions)
            {
                //  create a dictionary of each area's LEDs for quick indexing with step token key
                Dictionary<ushort, List<LED>> areaLEDs = new Dictionary<ushort, List<LED>>(exp.Areas.Count);
                foreach (Area area in exp.Areas)
                    areaLEDs.Add(area.Key, area.LEDs.ToList<LED>());

                //  add a step dictionary entry to represents the expression areas' default values, unless a matching entry already exists
                //  this will replace the expression's area(s) with a StepMethodDictionaryKey token
                AddExpressionAreasToStepDictionary(exp, stepDict);

                //  for all steps in expression               
                foreach (Entry entry in exp.Entries)
                {
                    if (entry is Step step)
                    {
                        //  each step token represents one area with a specific LED intensity
                        //  get composite list of all LEDs in step
                        var value = new StepDictionaryValue() { Priority = exp.OutputPriority, IsReset = false };
                        foreach (Token token in step.Tokens)
                        {
                            //  for each LED in area represented by token
                            foreach (LED led in areaLEDs[token.Key])
                            {
                                //  check to see if LED is already in step's LED list
                                bool found = false;
                                for (int i = 0; i < value.LEDs.Count; ++i)
                                {
                                    //  if found
                                    if (value.LEDs[i].IsSamePath(led))
                                    {
                                        //  flag LED found
                                        found = true;

                                        //  use higher of the two token-factored LED intensities
                                        byte intensity = (byte)(token.Value * led.Intensity / 100);
                                        if (value.LEDs[i].Intensity < intensity)
                                            value.LEDs[i].Intensity = intensity;
                                        break;
                                    }
                                }
                                //  if LED not already in list, then add it                                
                                if (!found)
                                {
                                    //  calculate the token-factored LED intensity
                                    byte intensity = (byte)(token.Value * led.Intensity / 100);
                                    value.LEDs.Add(new LED(led.Id, intensity, led.LightEngineId, led.Color));
                                }
                            }
                        }

                        //  sort the combined list of LEDs
                        var sortedLEDs = value.LEDs.OrderBy(x => x.LightEngineId).ThenBy(x => x.Color).ThenBy(x => x.Id).ToList();
                        value.LEDs = sortedLEDs;

                        //  check for same step already in step dictionary
                        //  step dictionary key 0 is reserved
                        int stepDictKey = 0;
                        foreach (KeyValuePair<int, StepDictionaryValue> item in stepDict)
                        {
                            if (value.Equals(item.Value))
                            {
                                stepDictKey = item.Key;
                                break;
                            }
                        }

                        //  if composite list is not in dictionary, then add it
                        if (stepDictKey == 0)
                        {
                            stepDictKey = stepDict.Count + 1;
                            stepDict.Add(stepDictKey, value);
                        }

                        //  replace the step token(s) with a StepMethodDictionaryKey token
                        step.Tokens = new BindingList<Token>() { new Token((UInt16)Keys.KeyStepMethodDictionaryKey, stepDictKey) };
                    }
                }
            }

            //  convert expression collection into ECCONet 3.0 pattern table
            binPatternTable = ExpressionConverters.ToPatternBinary(ec, PatternBinType.StepMethodDitionaryKeys);

            //  create pattern dictionaries for each light engine used in any pattern
            Dictionary<UInt32, List<byte>> lightEngineDictionaries = new Dictionary<uint, List<byte>>(50);
            foreach (KeyValuePair<int, StepDictionaryValue> stepDictEntry in stepDict)
            {
                //  distribute step LEDs into lists for each included light engine
                Dictionary<UInt32, List<LED>> lightEngineLEDLists = new Dictionary<uint, List<LED>>(50);
                foreach (LED led in stepDictEntry.Value.LEDs)
                {
                    //  if found new light engine add to dictionary
                    if (!lightEngineLEDLists.ContainsKey(led.LightEngineId))
                        lightEngineLEDLists.Add(led.LightEngineId, new List<LED>(4));

                    //  add the LED to its light engine
                    lightEngineLEDLists[led.LightEngineId].Add(led);
                }

                //  add light engine values to the light engine dictionaries
                foreach (KeyValuePair<UInt32, List<LED>> lightEngineLEDList in lightEngineLEDLists)
                {
                    //  get the actual light engine data model that has the listed LEDs
                    LightEngine lightEngine = null;
                    foreach (LightEngine le in lightBar.LightEngines)
                    {
                        if (le.ShortId == lightEngineLEDList.Key)
                        {
                            lightEngine = le;
                            break;
                        }
                    }

                    //  check for valid light engine found (should always be the case)
                    if (!LightEngine.ValidateLightEngine(lightEngine))
                    {
                        binPatternTable = null;
                        binLEDictionaries = null;
                        return false;
                    }
                    else
                    {
                        //  if the light engine's step dictionary not created yet, then create it
                        if (!lightEngineDictionaries.ContainsKey(lightEngineLEDList.Key))
                        {
                            //  create new light engine dictionary
                            lightEngineDictionaries.Add(lightEngineLEDList.Key, new List<byte>(100));
                            byte[] zeroEntry = new byte[4];
                            zeroEntry[0] = 0;
                            zeroEntry[1] = 0;
                            zeroEntry[2] = (byte)(lightEngine.ShortId >> 8);
                            zeroEntry[3] = (byte)lightEngine.ShortId;
                            lightEngineDictionaries[lightEngineLEDList.Key].AddRange(zeroEntry);
                        }

                        //  create entry in the light engine's step dictionary
                        UInt16 key = (UInt16)stepDictEntry.Key;
                        UInt16 value = 0;

                        //  add priority and reset
                        UInt16 priority = stepDictEntry.Value.Priority;
                        if (priority > 7)
                            priority = 7;
                        value |= (UInt16)(priority << 12);
                        if (stepDictEntry.Value.IsReset)
                            value |= 0x8000;

                        //  add individual LED intensities
                        foreach (LED led in lightEngineLEDList.Value)
                        {
                            //  find the light engine LED color data model that matches the color name ("Red", "Blue", etc.)
                            //  if there is no color name match, then default to the light engine's first color
                            LightEngine.LEDColor ledColor = null;
                            foreach (LightEngine.LEDColor c in lightEngine.LedColors)
                            {
                                if (c.Equals(led.Color))
                                {
                                    ledColor = c;
                                    break;
                                }
                            }
                            if (ledColor == null)
                                ledColor = lightEngine.LedColors[0];

                            //  get LED string within color
                            //  if LED string is not valid for the color, then default to first LED string
                            int ledId = led.Id;
                            if (ledId >= ledColor.LEDs.Count)
                                ledId = 0;

                            //  get dictionary entry intensity position 
                            int position = 0;
                            int colorID = 0;
                            while (colorID < ledColor.Id)
                                position += ledColor.LEDs.Count;
                            position += ledId;
                            if (position > 5)
                                position = 5;

                            //  convert LED intensity to log index and add to value
                            UInt16 intensityIndex = LogIntensity.IntensityToIndex(led.Intensity, LogIntensity.IntensityBits.Bits2);
                            value |= (UInt16)(intensityIndex << (position * 2));
                        }

                        //  add entry to light engine step dictionary
                        byte[] bytes = new byte[4];
                        bytes[0] = (byte)(key >> 8);
                        bytes[1] = (byte)key;
                        bytes[2] = (byte)(value >> 8);
                        bytes[3] = (byte)value;
                        lightEngineDictionaries[lightEngineLEDList.Key].AddRange(bytes);
                    }
                }
            }

            //  concatenate all light engine dictionaries into one bin file
            List<byte> binDict = new List<byte>(4096);
            foreach (var value in lightEngineDictionaries.Values)
                binDict.AddRange(value);
            binLEDictionaries = binDict.ToArray();

            //  print statistics (if in debug mode)
            ExpressionTest.PrintExpressionAndBinFileStats(ec, binPatternTable, binLEDictionaries, 4);

            //  return success
            return true;
        }
#endif

#if AREA_OVERLAP_6_BYTE_METHOD
/// <summary>
/// Converts an expression collection into the simplified step method
/// binary pattern table and light engine dictionaries.
/// Note: The expression collection must include the area LED assignments,
/// and each LED must have the LightEngineID and Color fields set. 
/// </summary>
/// <param name="_ec">The expression collection to convert.</param>
/// <param name="binPatternTable">The binary pattern table output.</param>
/// <param name="binLEDictionaries">The binary light engine dictionary output.</param>
public static void BuildLbcLeFiles(ExpressionCollection _ec,
    out byte[] binPatternTable, out byte[] binLEDictionaries,
    out byte[] upperLEDicitonaries, out byte[] lowerLEDictionaries)
{
    binPatternTable = null;
    binLEDictionaries = null;
    upperLEDicitonaries = null;
    lowerLEDictionaries = null;


    //  create a copy of the expression collection
    //  the area tokens in each expression's steps will be replaced with a KeyStepMethodDictionaryKey token
    ExpressionCollection ec = _ec.Copy();

    //  create a new dictionary of steps
    Dictionary<int, StepDictionaryValue> stepDict = new Dictionary<int, StepDictionaryValue>(100);

    //  initialize a dictionary key
    int stepDictKey = 1;  //  0 is reserved for dictionary start

    //  for all expressions in collection
    foreach (Expression exp in ec.Expressions)
    {
        //  create a dictionary of each area's LEDs sorted
        Dictionary<ushort, List<LED>> areaLEDs = new Dictionary<ushort, List<LED>>(exp.Areas.Count);
        foreach (Area area in exp.Areas)
            areaLEDs.Add(area.Key, area.LEDs.ToList<LED>());

        //  AREA

        //  get composite list of all LEDs in areas
        var value = new StepDictionaryValue();
        value.Priority = exp.Priority;
        value.IsReset = true;
        value.LEDs = new List<LED>(100);
        foreach (List<LED> leds in areaLEDs.Values)
        {
            //  for each LED in area represented by token
            foreach (LED led in leds)
            {
                //  check to see if LED is already in step's LED list
                bool found = false;
                for (int i = 0; i < value.LEDs.Count; ++i)
                {
                    //  if found
                    if (value.LEDs[i].IsSamePath(led))
                    {
                        //  flag found and use higher of two LED intensities
                        found = true;
                        if (value.LEDs[i].Intensity < led.Intensity)
                            value.LEDs[i].Intensity = led.Intensity;
                        break;
                    }
                }
                //  if LED not already in list, then add it  //<<-- area default for intensity                              
                if (!found)
                    value.LEDs.Add(new LED(led.Id, 0, led.LightEngineId, led.Color));
            }
        }

        //  now that have all LEDs in areas, sort them
        List<LED> sortedLEDs = value.LEDs.OrderBy(x => x.LightEngineId).ThenBy(x => x.Color).ThenBy(x => x.Id).ToList();
        value.LEDs = sortedLEDs;

        //  check for same step already in step dictionary
        int stepDictKey1 = stepDictKey;
        foreach (KeyValuePair<int, StepDictionaryValue> item in stepDict)
        {
            if (value.Equals(item.Value))
            {
                stepDictKey1 = item.Key;
                break;
            }
        }

        //  if step is not in step dictionary, then add it
        if (stepDictKey1 == stepDictKey)
        {
            stepDict.Add(stepDictKey, value);
            ++stepDictKey;
        }

        //  replace the area(s) with a StepMethodDictionaryKey token
        exp.Areas = new BindingList<Area>() { new Area("area", 0, (UInt16)Keys.KeyStepMethodDictionaryKey, stepDictKey1, null) };

        //  END AREA


        //  for all steps in expression
        foreach (Entry entry in exp.Entries)
        {
            if (entry is Step step)
            {
                //  each step token represents one area with a specific LED intensity
                //  get composite list of all LEDs in step
                value = new StepDictionaryValue();
                value.Priority = exp.Priority;
                value.LEDs = new List<LED>(100);
                foreach (Token t in step.Tokens)
                {
                    //  for each LED in area represented by token
                    foreach (LED led in areaLEDs[t.Key])
                    {
                        //  check to see if LED is already in step's LED list
                        bool found = false;
                        for (int i = 0; i < value.LEDs.Count; ++i)
                        {
                            //  if found
                            if (value.LEDs[i].IsSamePath(led))
                            {
                                //  flag found and use higher of two LED intensities
                                found = true;
                                if (value.LEDs[i].Intensity < led.Intensity)
                                    value.LEDs[i].Intensity = led.Intensity;
                                break;
                            }
                        }
                        //  if LED not already in list, then add it                                
                        if (!found)
                            value.LEDs.Add(new LED(led.Id, (byte)t.Value, led.LightEngineId, led.Color));
                    }
                }

                //  now that have all LEDs in step, sort them
                sortedLEDs = value.LEDs.OrderBy(x => x.LightEngineId).ThenBy(x => x.Color).ThenBy(x => x.Id).ToList();
                value.LEDs = sortedLEDs;

                //  check for same composite list already in step dictionary
                stepDictKey1 = stepDictKey;
                foreach (KeyValuePair<int, StepDictionaryValue> item in stepDict)
                {
                    if (value.Equals(item.Value))
                    {
                        stepDictKey1 = item.Key;
                        break;
                    }
                }

                //  if composite list is not in dictionary, then add it
                if (stepDictKey1 == stepDictKey)
                {
                    stepDict.Add(stepDictKey, value);
                    ++stepDictKey;
                }

                //  replace the step token(s) with a StepMethodDictionaryKey token
                step.Tokens = new BindingList<Token>() { new Token((UInt16)Keys.KeyStepMethodDictionaryKey, stepDictKey1) };
            }
        }
    }

    //  convert expression collection into ECCONet 3.0 pattern table
    binPatternTable = ExpressionCollectionConvert.ToBin(ec);

    //  create pattern dictionaries for each light engine used in any pattern
    Dictionary<UInt32, List<byte>> lightEngineDictionaries = new Dictionary<uint, List<byte>>(50);
    foreach (KeyValuePair<int, StepDictionaryValue> stepDictEntry in stepDict)
    {
        //  consolidate step LEDs into light engines
        Dictionary<UInt32, List<LED>> lightEngines = new Dictionary<uint, List<LED>>(50);
        foreach (LED led in stepDictEntry.Value.LEDs)
        {
            //  if found new light engine add to dictionary
            if (!lightEngines.ContainsKey(led.LightEngineId))
                lightEngines.Add(led.LightEngineId, new List<LED>(4));

            //  add the LED to its light engine
            lightEngines[led.LightEngineId].Add(led);
        }

        //  add light engine values to the light engine dictionaries
        foreach (KeyValuePair<UInt32, List<LED>> lightEngine in lightEngines)
        {
            //  if light engine step dictionary not created yet, then create it
            if (!lightEngineDictionaries.ContainsKey(lightEngine.Key))
            {
                //  create new light engine dictionary
                lightEngineDictionaries.Add(lightEngine.Key, new List<byte>(100));
                byte[] zeroEntry = new byte[6];
                zeroEntry[0] = 0;
                zeroEntry[1] = 0;
                zeroEntry[2] = (byte)(lightEngine.Key >> 24);
                zeroEntry[3] = (byte)(lightEngine.Key >> 16);
                zeroEntry[4] = (byte)(lightEngine.Key >> 8);
                zeroEntry[5] = (byte)lightEngine.Key;
                lightEngineDictionaries[lightEngine.Key].AddRange(zeroEntry);
            }

            //  create entry for light engine step dictionary
            UInt16 key = (UInt16)stepDictEntry.Key;
            UInt16 color0 = 0;
            UInt16 color1 = 0;
            UInt16 color2 = 0;
            foreach (LED led in lightEngine.Value)
            {
                //  convert LED intensity to log index
                byte intensityIndex = LogIntensity.IntensityToIndex(led.Intensity);
                UInt16 color = 0;
                switch (led.Id)
                {
                    case LightEngine.LEDId.Left: color |= (UInt16)intensityIndex; break;
                    case LightEngine.LEDId.Center: color |= (UInt16)(intensityIndex << 3); break;
                    case LightEngine.LEDId.Right: color |= (UInt16)(intensityIndex << 6); break;
                }
                color |= (UInt16)(stepDictEntry.Value.Priority << 9);
                if (stepDictEntry.Value.IsReset)
                    color |= (UInt16)(1 << 11);

                //<<-- need conversions based on light engine and color
                color0 |= color;
            }

            //  add entry to light engine step dictionary
            byte[] bytes = new byte[6];
            bytes[0] = (byte)(key >> 4);
            bytes[1] = (byte)(key << 4);
            bytes[1] |= (byte)(color2 >> 8);
            bytes[2] = (byte)(color2);
            bytes[3] = (byte)(color1 >> 4);
            bytes[4] = (byte)(color1 << 4);
            bytes[4] |= (byte)(color0 >> 8);
            bytes[5] = (byte)(color0);
            lightEngineDictionaries[lightEngine.Key].AddRange(bytes);
        }
    }

    //  concatenate all light engine dictionaries into one bin file
    List<byte> binDict = new List<byte>(4096);
    List<byte> upperDict = new List<byte>(4096);
    List<byte> lowerDict = new List<byte>(4096);
    //foreach (var value in lightEngineDictionaries.Values)
    //    binDict.AddRange(value);
    foreach (KeyValuePair<UInt32, List<byte>> item in lightEngineDictionaries)
    {
        binDict.AddRange(item.Value);
        if (item.Key <= 32)
            upperDict.AddRange(item.Value);
        else
            lowerDict.AddRange(item.Value);
    }
    binLEDictionaries = binDict.ToArray();
    upperLEDicitonaries = upperDict.ToArray();
    lowerLEDictionaries = lowerDict.ToArray();
}
#endif

#if NON_OVERLAP_6_BYTE_METHOD
        /// <summary>
        /// Converts an expression collection into the simplified step method
        /// binary pattern table and light engine dictionaries.
        /// Note: The expression collection must include the area LED assignments,
        /// and each LED must have the LightEngineID and Color fields set. 
        /// </summary>
        /// <param name="_ec">The expression collection to convert.</param>
        /// <param name="binPatternTable">The binary pattern table output.</param>
        /// <param name="binLEDictionaries">The binary light engine dictionary output.</param>
        public static void BuildLbcLeFiles(ExpressionCollection _ec,
            out byte[] binPatternTable, out byte[] binLEDictionaries)
        {
            binPatternTable = null;
            binLEDictionaries = null;


            //  create a copy of the expression collection
            //  the area tokens in each expression's steps will be replaced with a KeyStepMethodDictionaryKey token
            ExpressionCollection ec = _ec.Copy();

            //  create a new dictionary of steps
            Dictionary<int, StepDictionaryValue> stepDict = new Dictionary<int, StepDictionaryValue>(100);

            //  initialize a dictionary key
            int stepDictKey = 1;  //  0 is reserved for dictionary start

            //  for all expressions in collection
            foreach (Expression exp in ec.Expressions)
            {
                //  create a dictionary of each area's LEDs sorted
                Dictionary<ushort, List<LED>> areaLEDs = new Dictionary<ushort, List<LED>>(exp.Areas.Count);
                foreach (Area area in exp.Areas)
                    areaLEDs.Add(area.Key, area.LEDs.OrderBy(x => x.LightEngineId).ThenBy(x => x.Color).ThenBy(x => x.Id).ToList());

                //  for all steps in expression
                foreach (Entry entry in exp.Entries)
                {
                    if (entry is Step step)
                    {
                        //  each step token represents one area with a specific LED intensity
                        //  get composite list of all LEDs in step
                        var value = new StepDictionaryValue();
                        value.Priority = exp.Priority;
                        value.LEDs = new List<LED>(100);
                        foreach (Token t in step.Tokens)
                            foreach (LED led in areaLEDs[t.Key])
                                value.LEDs.Add(new LED(led.Id, (byte)t.Value, led.LightEngineId, led.Color));

                        //  check for same composite list already in step dictionary
                        int stepDictKey1 = stepDictKey;
                        foreach (KeyValuePair<int, StepDictionaryValue> item in stepDict)
                        {
                            if (value.Equals(item.Value))
                            {
                                stepDictKey1 = item.Key;
                                break;
                            }
                        }

                        //  if composite list is not in dictionary, then add it
                        if (stepDictKey1 == stepDictKey)
                        {
                            stepDict.Add(stepDictKey, value);
                            ++stepDictKey;
                        }

                        //  replace the step token(s) with a StepMethodDictionaryKey token
                        step.Tokens = new BindingList<Token>() { new Token((UInt16)Keys.KeyStepMethodDictionaryKey, stepDictKey1) };
                    }
                }
            }

            //  convert expression collection into ECCONet 3.0 pattern table
            binPatternTable = ExpressionCollectionConvert.ToBin(ec);

            //  create pattern dictionaries for each light engine used in any pattern
            Dictionary<UInt32, List<byte>> lightEngineDictionaries = new Dictionary<uint, List<byte>>(50);
            foreach (KeyValuePair<int, StepDictionaryValue> stepDictEntry in stepDict)
            {
                //  consolidate step LEDs into light engines
                Dictionary<UInt32, List<LED>> lightEngines = new Dictionary<uint, List<LED>>(50);
                foreach (LED led in stepDictEntry.Value.LEDs)
                {
                    //  if found new light engine add to dictionary
                    if (!lightEngines.ContainsKey(led.LightEngineId))
                        lightEngines.Add(led.LightEngineId, new List<LED>(4));

                    //  add the LED to its light engine
                    lightEngines[led.LightEngineId].Add(led);
                }

                //  add light engine values to the light engine dictionaries
                foreach (KeyValuePair<UInt32, List<LED>> lightEngine in lightEngines)
                {
                    //  if light engine step dictionary not created yet, then create it
                    if (!lightEngineDictionaries.ContainsKey(lightEngine.Key))
                    {
                        //  create new light engine dictionary
                        lightEngineDictionaries.Add(lightEngine.Key, new List<byte>(100));
                        byte[] zeroEntry = new byte[6];
                        zeroEntry[0] = 0;
                        zeroEntry[1] = 0;
                        zeroEntry[2] = (byte)(lightEngine.Key >> 24);
                        zeroEntry[3] = (byte)(lightEngine.Key >> 16);
                        zeroEntry[4] = (byte)(lightEngine.Key >> 8);
                        zeroEntry[5] = (byte)lightEngine.Key;
                        lightEngineDictionaries[lightEngine.Key].AddRange(zeroEntry);
                    }

                    //  create entry for light engine step dictionary
                    UInt16 key = (UInt16)stepDictEntry.Key;
                    UInt16 color0 = 0;
                    UInt16 color1 = 0;
                    UInt16 color2 = 0;
                    foreach (LED led in lightEngine.Value)
                    {
                        //  convert LED intensity to log index
                        byte intensityIndex = LogIntensity.IntensityToIndex(led.Intensity);
                        UInt16 color = 0;
                        switch (led.Id)
                        {
                            case LightEngine.LEDId.Left:   color |= (UInt16)intensityIndex; break;
                            case LightEngine.LEDId.Center: color |= (UInt16)(intensityIndex << 3); break;
                            case LightEngine.LEDId.Right:  color |= (UInt16)(intensityIndex << 6); break;
                        }
                        color |= (UInt16)(stepDictEntry.Value.Priority << 9);
                        if (stepDictEntry.Value.IsReset)
                            color |= (UInt16)(1 << 11);

                        //<<-- need conversions based on light engine and color
                        color0 |= color;
                    }

                    //  add entry to light engine step dictionary
                    byte[] bytes = new byte[6];
                    bytes[0] = (byte)(key >> 4);
                    bytes[1] = (byte)(key << 4);
                    bytes[1] |= (byte)(color2 >> 8);
                    bytes[2] = (byte)(color2);
                    bytes[3] = (byte)(color1 >> 4);
                    bytes[4] = (byte)(color1 << 4);
                    bytes[4] |= (byte)(color0 >> 8);
                    bytes[5] = (byte)(color0);
                    lightEngineDictionaries[lightEngine.Key].AddRange(bytes);
                }
            }

            //  concatenate all light engine dictionaries into one bin file
            List<byte> binDict = new List<byte>(4096);
            foreach (var value in lightEngineDictionaries.Values)
                binDict.AddRange(value);
            binLEDictionaries = binDict.ToArray();
        }
#endif




#if UNUSED_CODE

        /// <summary>
        /// Builds expression bin files for independent outputs.
        /// </summary>
        /// <param name="ec">The expression collection.</param>
        /// <param name="binPatternTable"></param>
        /// <param name="binDictionaries"></param>
        public static void BuildExpressionCollectionStepMethodBinFiles(ExpressionCollection ec, out byte[] binPatternTable, out byte[] binDictionaries)
        {
            //  convert the expression collection into these:
            //
            //  - a step method expression collection (only contains KeyStepMethodDictionaryKey tokens)
            //  - a step method master step dictionary (the master file for the light engine steps)
            //
            ECtoStepMethodECandDictionary(ec, false, out ExpressionCollection stepExpColl, out StepDictionary stepMasterDict);

            //  convert the step method expression collection into a bin file
            binPatternTable = ExpressionConverters.ToPatternBinary(stepExpColl, PatternBinType.StepMethodDitionaryKeys);

            //  convert the step method master dictionary into a bin file
            //  note that the light engines are required to know what dictionary bits to set
            BuildDictionaryBinFile(stepMasterDict, out binDictionaries);

            //  print statistics (if in debug mode)
            ExpressionTest.PrintExpressionAndBinFileStats(ec, binPatternTable, binDictionaries, 5);
        }



        /// <summary>
        /// Returns a value indicating whether the given step has the same LEDs (path and intensity) as this step.
        /// Note: This compare assumes that both step's list of LEDs have been sorted.
        /// </summary>
        /// <param name="step">The step to compare.</param>
        /// <returns>A value indicating whether the given step has the same LEDs (path and intensity) as this step.</returns>
        public static bool IsSameLEDsWithSameIntensities(List<LED> ledsA, List<LED> ledsB)
        {
            //  make sure both lists exist
            if ((ledsA == null) || (ledsB == null))
                return false;

            //  make sure both lists have same count
            if (ledsA.Count != ledsA.Count)
                return false;

            //  compare LEDs
            for (int i = 0; i < ledsA.Count; ++i)
                if (!ledsA[i].Equals(ledsB[i]))
                    return false;
            return true;
        }

        private class Step
        {
            /// <summary>
            /// The step ID.
            /// </summary>
            public int Id;

            /// <summary>
            /// The LEDs for this step.
            /// </summary>
            public List<LED> LEDs;


            /// <summary>
            /// Returns a value indicating whether the given step has the same LEDs (path and intensity) as this step.
            /// Note: This compare assumes that both step's list of LEDs have been sorted.
            /// </summary>
            /// <param name="step">The step to compare.</param>
            /// <returns>A value indicating whether the given step has the same LEDs (path and intensity) as this step.</returns>
            public bool HasSameLEDsWithSameIntensities(List<LED> leds)
            {
                //  make sure both lists exist
                if ((LEDs == null) || (leds == null))
                    return false;

                //  make sure both lists have same count
                if (LEDs.Count != leds.Count)
                    return false;

                //  compare LEDs
                for (int i = 0; i < LEDs.Count; ++i)
                    if (!LEDs[i].Equals(leds[i]))
                        return false;
                return true;
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            public Step() { }

            /// <summary>
            /// Constructor.
            /// </summary>
            public Step(int id, List<LED> leds)
            {
                this.Id = id;
                this.LEDs = leds;
            }
        }



                            else  //  using timers
                            {
                                //  if area is on or a steady or cut single-step expression
                                if ((area.Value > 0) || (exp.Entries.Count == 1))
                                {
                                    //  perform look-ahead to determine how long area should be on
                                    //  does not matter if next step found with same area has same value
                                    uint period = step.Period;
                                    bool periodFound = false;
                                    for (int n = index + 1; n < exp.Entries.Count(); ++n)
                                    {
                                        var ent = exp.Entries[n];
                                        if (ent is Step stp)
                                        {
                                            foreach (var tk in stp.Tokens)
                                            {
                                                if (tk.Key == area.Key)
                                                {
                                                    periodFound = true;
                                                    break;
                                                }
                                                else
                                                    period += stp.Period;
                                            }
                                        }
                                        if (periodFound)
                                            break;
                                    }

                                    //  add all output paths to dictionary value, factoring expression and token intensity value
                                    foreach (var outputPath in outputPaths)
                                        dictValue.AddOutputPath(new PathValuePeriod()
                                        {
                                            Path = string.Copy(outputPath.Path),
                                            Value = (exp.Value * area.Value * outputPath.Value / 10000),
                                            Period = (int)period
                                        });
                                }
                            }

#endif

