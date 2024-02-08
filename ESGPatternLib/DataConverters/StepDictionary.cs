using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECCONet;
using ESG.ExpressionLib.DataModels;

using Area = ESG.ExpressionLib.DataModels.Expression.Area;


namespace ESG.ExpressionLib.DataConverters
{
    public static partial class ExpressionConverters
    {
        #region Step dictionary value
        /// <summary>
        /// The StepDictionaryValue class is an expression step comprised of output priority, output priority reset
        /// and list of output paths, each comprised of path, value and period values.
        /// </summary>
        public class StepDictionaryValue
        {
            /// <summary>
            /// Adds a period to the PathValue class for use in bin generation.
            /// </summary>
            public class PathValuePeriod : PathValue
            {
                /// <summary>
                /// The step period.
                /// </summary>
                public int Period { get; set; }

                /// <summary>
                /// Returns a value indicating whether this PathValue and the given PathValue
                /// have the same path, value and period.
                /// </summary>
                /// <param name="obj">The PathValue to which to compare.</param>
                /// <returns>Returns a value indicating whether this PathValue and the given PathValue
                /// have the same path and value.</returns>
                public override bool Equals(object obj)
                {
                    if (obj is PathValuePeriod pv)
                        return (Path.Equals(pv.Path) && Value.Equals(pv.Value) && Period.Equals(pv.Period));
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

                /// <summary>
                /// Returns a deep copy of the PathValuePeriod.
                /// </summary>
                /// <returns>A deep copy of the PathValuePeriod.</returns>
                public new PathValuePeriod DeepCopy()
                {
                    return new PathValuePeriod() { Path = Path, Value = Value, Period = Period };
                }
            }

            /// <summary>
            /// The entry priority.
            /// </summary>
            public byte Priority;

            /// <summary>
            /// A value indicating whether the step is a reset value.
            /// </summary>
            public bool IsReset;

            /// <summary>
            /// The list of leds in the entry.
            /// </summary>
            public List<PathValuePeriod> OutputPaths { get => _outputPaths; set => _outputPaths = value ?? _outputPaths; }
            private List<PathValuePeriod> _outputPaths = new List<PathValuePeriod>();

            /// <summary>
            /// Constructor.
            /// </summary>
            public StepDictionaryValue() { }

            /// <summary>
            /// Constructor.
            /// </summary>
            public StepDictionaryValue(byte priority, bool isReset, List<PathValuePeriod> outputPaths)
            {
                this.Priority = priority;
                this.IsReset = isReset;
                this.OutputPaths = outputPaths;
            }

            /// <summary>
            /// Returns a value indicating whether this StepDictionaryValues and the given StepDictionaryValue
            /// have the same priority, reset and list of output paths, each path having the same path, value and period.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (obj is StepDictionaryValue value)
                {
                    //  make sure values have same priority and reset
                    if ((Priority != value.Priority) || (IsReset != value.IsReset))
                        return false;

                    //  make sure both output path lists exist
                    if ((OutputPaths == null) || (value.OutputPaths == null))
                        return false;

                    //  make sure both output path lists have same count
                    if (OutputPaths.Count != value.OutputPaths.Count)
                        return false;

                    //  compare output paths
                    for (int i = 0; i < OutputPaths.Count; ++i)
                        if (!OutputPaths[i].Equals(value.OutputPaths[i]))
                            return false;

                    return true;
                }
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

            /// <summary>
            /// Adds an output path to the step dictionary value's output path list.
            /// </summary>
            /// <param name="outputPath">The output path to add.</param>
            public void AddOutputPath(PathValuePeriod outputPath)
            {
                //  check to see if output path is already in step's output path list
                //  note that this is a path comparison, not an overall value comparison
                bool pathFound = false;
                foreach (var op in OutputPaths)
                {
                    //  if path already in list
                    if (op.PathEquals(outputPath))
                    {
                        //  flag as found and use the value and period from the higher-value path
                        pathFound = true;
                        if (op.Value < outputPath.Value)
                        {
                            op.Value = outputPath.Value;
                            op.Period = outputPath.Period;
                        }
                    }
                }

                //  if path not already in list, then add it                               
                if (!pathFound)
                    OutputPaths.Add(outputPath);
            }

            /// <summary>
            /// Creates a deep copy of the StepDictionaryValue.
            /// </summary>
            /// <returns>A deep copy of the StepDictionaryValue.</returns>
            public StepDictionaryValue DeepCopy()
            {
                var copy = new StepDictionaryValue() { IsReset = IsReset, Priority = Priority };
                copy.OutputPaths = new List<PathValuePeriod>(OutputPaths.Count);
                foreach (var outputPath in OutputPaths)
                    copy.OutputPaths.Add(outputPath.DeepCopy());
                return copy;
            }
        }
        #endregion

        #region Step dictionary
        /// <summary>
        /// The StepDictionary class holds unique StepDictionaryValues.
        /// </summary>
        public class StepDictionary : Dictionary<int, StepDictionaryValue>
        {
            /// <summary>
            /// Adds a deep copy of the given step dictionary value, unless the dictionary already contains an identical value.
            /// </summary>
            /// <param name="value">The StepDictionaryValue to add.</param>
            /// <returns>Returns the dictionary value key.</returns>
            public int AddStep(StepDictionaryValue value)
            {
                //  sort the list of output paths for this step
                value.OutputPaths.Sort();

                //  check for same combination of output paths in step dictionary
                //  step dictionary key 0 is reserved
                int stepDictKey = 0;
                foreach (KeyValuePair<int, StepDictionaryValue> item in this)
                {
                    if (value.Equals(item.Value))
                    {
                        stepDictKey = item.Key;
                        break;
                    }
                }

                //  if step is not in step dictionary, then add it
                if (stepDictKey == 0)
                {
                    stepDictKey = Count + 1;
                    Add(stepDictKey, value.DeepCopy());
                }

                //  return the step key
                return stepDictKey;
            }

            /// <summary>
            /// Adds a step dictionary entry to represents the expression areas' default values and priority reset,
            /// unless a matching entry already exists.
            /// Replaces the expression's area(s) with a StepMethodDictionaryKey token.
            /// </summary>
            /// <param name="exp">The expression.</param>
            public void AddExpressionAreasStep(Expression exp)
            {
                //  create a step dictionary entry value
                var dictValue = new StepDictionaryValue();
                dictValue.Priority = exp.OutputPriority;
                dictValue.IsReset = true;
                dictValue.OutputPaths = new List<StepDictionaryValue.PathValuePeriod>(100);

                //  for each area's output path list
                foreach (var area in exp.Areas)
                {
                    //  for each output path in area
                    foreach (var outputPath in area.OutputPaths)
                    {
                        //  create a copy with a zero period and value based on expression intensity and area default value
                        var pathCopy = new StepDictionaryValue.PathValuePeriod();
                        pathCopy.Path = string.Copy(outputPath.Path);
                        pathCopy.Value = exp.Value * area.DefaultValue / 100;
                        pathCopy.Period = 0;
                        dictValue.AddOutputPath(pathCopy);
                    }
                }

                //  add the step to the step dictionary if a matching step is not already in dictionary
                int dictKey = AddStep(dictValue);

                //  replace the expression's area(s) with a StepMethodDictionaryKey token
                exp.Areas = new BindingList<Area>() { new Area("area", 0, (UInt16)ECCONet.Token.Keys.KeyStepMethodDictionaryKey, dictKey, null) };
            }
        }
        #endregion

    }
}
