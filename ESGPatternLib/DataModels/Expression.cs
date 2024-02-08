using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;


namespace ESG.ExpressionLib.DataModels
{
    /// <summary>
    /// The light expression class.
    /// </summary>
    [XmlType("Expression")]
    [JsonObject("Token")]
    public partial class Expression
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        #region Property enumerations
        /// <summary>
        /// The regulatory standard (if any) that applies to an expression.
        /// </summary>
        public enum RegulatoryStandard
        {
            None,
            SAE,
            Title13,
            ECE_R65,
        }
        #endregion

        #region Property classes

        /// <summary>
        /// Token class for expression entries.
        /// </summary>
        [XmlType("Token")]
        [JsonObject("Token")]
        public class Token
        {
            /// <summary>
            /// The token key.
            /// </summary>
            [XmlAttribute("Key")]
            [JsonProperty("Key")]
            public UInt16 Key { get; set; }

            /// <summary>
            /// The token value.
            /// </summary>
            [XmlAttribute("Value")]
            [JsonProperty("Value")]
            public Int32 Value { get; set; }

            /// <summary>
            /// Constructor.
            /// </summary>
            public Token() { }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public Token(UInt16 key, Int32 value)
            {
                this.Key = key;
                this.Value = value;
            }
        }

        /// <summary>
        /// Area class defines the lights included in an expression and their default values.
        /// </summary>
        [XmlType("Area")]
        [JsonObject("Token")]
        public class Area
        {
            /// <summary>
            /// The one-based area position in the expression's area list.
            /// This is included to show in the area's ToString() method.
            /// </summary>
            [XmlIgnore]
            [JsonIgnore]
            public int Index { get; set; }


            /// <summary>
            /// The area name.  For example, "Front-Left Quadrant".
            /// </summary>
            [XmlAttribute("Name")]
            [JsonProperty("Name")]
            public string Name { get; set; }

            /// <summary>
            /// The ECCONet area key.
            /// </summary>
            [XmlAttribute("Key")]
            [JsonProperty("Key")]
            public UInt16 Key { get; set; }

            /// <summary>
            /// The default area value.
            /// </summary>
            [XmlAttribute("DefaultValue")]
            [JsonProperty("DefaultValue")]
            public Int32 DefaultValue { get; set; }

            /// <summary>
            /// The list of light engine LEDs assigned to the area.
            /// </summary>
            [XmlArrayItem("Output", typeof(PathValue))]
            [XmlArray("Outputs")]
            [JsonProperty("Outputs")]
            public BindingList<PathValue> OutputPaths { get => _outputPaths; set => _outputPaths = value ?? _outputPaths; }
            private BindingList<PathValue> _outputPaths = new BindingList<PathValue>();

            /// <summary>
            /// Name with output paths property.
            /// </summary>
            [XmlIgnore]
            [JsonIgnore]
            public string NameWithOutputPaths
            {
                get
                {
                    //  add index and name
                    string str = /*Index.ToString() + "  " +*/ (Name ?? string.Empty);

                    //  add paths
                    bool haveComma = false;
                    foreach (var path in _outputPaths)
                    {
                        str += (haveComma ? (", " + path) : (" - " + path));
                        haveComma = true;
                    }
                    str = str.Replace("~", "");
                    return str;
                }
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            public Area() { }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="name">The area name.</param>
            /// <param name="index">The one-based position in the expression's area list.</param>
            /// <param name="key">The ECCONet area key.</param>
            /// <param name="defaultValue">The default area value.</param>
            /// <param name="ledss">The list of light engine LEDs assigned to the area.</param>
            public Area(string name, int index, UInt16 key, Int32 defaultValue, BindingList<PathValue> outputPaths)
            {
                this.Name = name;
                this.Index = index;
                this.Key = key;
                this.DefaultValue = defaultValue;
                this.OutputPaths = outputPaths;
            }

            /// <summary>
            /// Returns a string that represents this area.
            /// </summary>
            /// <returns>A string that represents this area.</returns>
            public override string ToString()
            {
                return NameWithOutputPaths;
            }
        }

        /// <summary>
        /// Base class for expression entries, which are typically steps.
        /// Other entries are for compression, which are repeat section and embedded expressions.
        /// </summary>
        [XmlType("Entry")]
        [JsonObject("Entry")]
        abstract public class Entry
        {
            /// <summary>
            /// A deep copy method.  Should be overridden in inheriting class.
            /// </summary>
            /// <returns>Returns a deep copy of the entry.</returns>
            abstract public Entry Copy();
        }

        /// <summary>
        /// Expression step entry.
        /// </summary>
        [XmlType("Step")]
        [JsonObject("Step")]
        public class Step : Entry
        {
            /// <summary>
            /// The step period in milliseconds.
            /// </summary>
            [XmlAttribute("Period")]
            [JsonProperty("Period")]
            public UInt16 Period
            {
                get => _period;
                set
                {
                    _period = value;
                }
            }
            private UInt16 _period;

            /// <summary>
            /// The step token list.  Tokens of any type can be sequenced.
            /// Typically, token key = expression area, value = intensity or volume.
            /// </summary>
            [XmlArray("Tokens")]
            [JsonProperty("Tokens")]
            public BindingList<Token> Tokens { get => _tokens; set => _tokens = value ?? _tokens; }
            private BindingList<Token> _tokens = new BindingList<Token>();

            /// <summary>
            /// Constructor.
            /// </summary>
            public Step() { }

            /// <summary>
            /// Pattern step constructor.
            /// </summary>
            public Step(UInt16 period, BindingList<Token> tokens)
            {
                this.Period = period;
                this.Tokens = tokens;
            }

            /// <summary>
            /// Returns a deep copy of the step entry.
            /// </summary>
            /// <returns>Returns a deep copy of the step entry.</returns>
            public override Entry Copy()
            {
                var step = new Step();
                step.Period = this.Period;
                foreach (var token in this.Tokens)
                    step.Tokens.Add(new Token(token.Key, token.Value));
                return step;
            }
        }

        /// <summary>
        /// Expression repeat section start.
        /// </summary>
        [XmlType("RepeatSectionStart")]
        [JsonObject("RepeatSectionStart")]
        public class RepeatSectionStart : Entry
        {
            /// <summary>
            /// The number of section repeats.  Limited to 15 in bin file.
            /// </summary>
            [XmlAttribute("Repeats")]
            [JsonProperty("Repeats")]
            public byte Repeats { get; set; }

            /// <summary>
            /// Constructor.
            /// </summary>
            public RepeatSectionStart() { }

            /// <summary>
            /// Pattern step constructor
            /// </summary>
            public RepeatSectionStart(byte repeats)
            {
                this.Repeats = repeats;
            }

            /// <summary>
            /// Returns a deep copy of the repeat section start entry.
            /// </summary>
            /// <returns>Returns a deep copy of the repeat section start entry.</returns>
            public override Entry Copy()
            {
                var rss = new RepeatSectionStart();
                rss.Repeats = this.Repeats;
                return rss;
            }
        }

        /// <summary>
        /// Expression repeat section end.
        /// </summary>
        [XmlType("RepeatSectionEnd")]
        [JsonObject("RepeatSectionEnd")]
        public class RepeatSectionEnd : Entry
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            public RepeatSectionEnd() { }

            /// <summary>
            /// Returns a deep copy of the repeat section end entry.
            /// </summary>
            /// <returns>Returns a deep copy of the repeat section end entry.</returns>
            public override Entry Copy()
            {
                return new RepeatSectionEnd();
            }
        }

        /// <summary>
        /// Nested expression.
        /// </summary>
        [XmlType("NestedExpression")]
        [JsonObject("NestedExpression")]
        public class NestedExpression : Entry
        {
            /// <summary>
            /// The nested expression enumeration.
            /// </summary>
            [XmlAttribute("ExpressionEnum")]
            [JsonProperty("ExpressionEnum")]
            public UInt16 ExpressionEnum;

            /// <summary>
            /// The number of nested expression repeats.  Limited to 15.
            /// </summary>
            [XmlAttribute("Repeats")]
            [JsonProperty("Repeats")]
            public byte Repeats { get; set; }

            /// <summary>
            /// Constructor.
            /// </summary>
            public NestedExpression() { }

            /// <summary>
            /// Pattern step constructor
            /// </summary>
            public NestedExpression(UInt16 expressionEnum, byte repeats)
            {
                this.ExpressionEnum = expressionEnum;
                this.Repeats = repeats;
            }

            /// <summary>
            /// Returns a deep copy of the nested expression entry.
            /// </summary>
            /// <returns>Returns a deep copy of the nested expression entry.</returns>
            public override Entry Copy()
            {
                var ne = new NestedExpression();
                ne.ExpressionEnum = this.ExpressionEnum;
                ne.Repeats = this.Repeats;
                return ne;
            }
        }
        #endregion


        #region Properties
        /// <summary>
        /// The expression name.
        /// </summary>
        [XmlAttribute("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// The regulatory standard (if any) that applies to the expression.
        /// </summary>
        [XmlAttribute("RegStandard")]
        [JsonProperty("RegStandard")]
        public RegulatoryStandard RegStandard { get; set; }

        /// <summary>
        /// The expression enumeration in ECCONet.  See "Patterns.cs" in the ECCONet project.
        /// </summary>
        [XmlAttribute("ExpressionEnum")]
        [JsonProperty("ExpressionEnum")]
        public UInt16 ExpressionEnum { get; set; }

        /// <summary>
        /// The number of times to repeat the expression, or zero for infinite.
        /// </summary>
        [XmlAttribute("Repeats")]
        [JsonProperty("Repeats")]
        public byte Repeats { get; set; }

        /// <summary>
        /// The expression input priority, 1-100, where 100 is the highest priority.
        /// </summary>
        [XmlAttribute("InputPriority")]
        [JsonProperty("InputPriority")]
        public byte InputPriority { get; set; }

        /// <summary>
        /// The expression output priority, 1-6, where 6 is the highest priority.
        /// </summary>
        [XmlAttribute("OutputPriority")]
        [JsonProperty("OutputPriority")]
        public byte OutputPriority { get; set; }

        /// <summary>
        /// The expression sequencer, 0-5.
        /// </summary>
        [XmlAttribute("Sequencer")]
        [JsonProperty("Sequencer")]
        public byte Sequencer { get; set; }

        /// <summary>
        /// The overall expression intensity, 0-100.
        /// </summary>
        [XmlAttribute("Value")]
        [JsonProperty("Value")]
        public byte Value { get; set; }

        /// <summary>
        /// Retrieves the total length of the pattern
        /// </summary>
        public int TotalLength
        {
            get
            {
                return Entries.OfType<Step>().Sum(x => x.Period);
            }
        }

        /// <summary>
        /// Retrieves a comma-separated list of step periods
        /// </summary>
        public string StepPeriods
        {
            get
            {
                var list = Entries.OfType<Step>().Select(x => x.Period);
                return string.Join(",",list);
            }
        }

        /// <summary>
        /// Retrieves a comma-separated list of pattern steps including keys and values
        /// </summary>
        public string PatternSteps
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                var stepList = new List<string>();
                foreach(Step entry in Entries)
                {
                    var tokenList = entry.Tokens.OrderBy(x => x.Key).Select(t => $"{t.Key}={t.Value}");
                    stepList.Add($"{entry.Period}:{string.Join(",",tokenList)}");
                }

                return string.Join("|",stepList);
            }
        }

        /// <summary>
        /// Sets whether or not the pattern was configured using transparentoffsteps
        /// </summary>
        public bool TransparentOffSteps { get; set; }

        /// <summary>
        /// The list of areas in the expression, with their default or "off" values.
        /// In the ECCONet documentation this is the first "All Off" step.
        /// </summary>
        public BindingList<Area> Areas { get => _areas; set => _areas = value ?? _areas; }
        private BindingList<Area> _areas = new BindingList<Area>();

        /// <summary>
        /// The expression entries of steps, repeat sections, and nested expressions.
        /// </summary>
        [XmlArrayItem("Entry", typeof(Entry))]
        [XmlArrayItem("Step", typeof(Step))]
        [XmlArrayItem("RepeatSectionStart", typeof(RepeatSectionStart))]
        [XmlArrayItem("RepeatSectionEnd", typeof(RepeatSectionEnd))]
        [XmlArrayItem("NestedExpression", typeof(NestedExpression))]
        [XmlArray("Entries")]
        [JsonProperty("Entries")]
        public BindingList<Entry> Entries { get => _entries; set => _entries = value ?? _entries; }
        private BindingList<Entry> _entries = new BindingList<Entry>();
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Expression() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The expression name.</param>
        /// <param name="patternEnum">The expression enumeration in ECCONet.  See "Patterns.cs" in the ECCONet project.</param>
        /// <param name="repeats">The number of times to repeat the expression, or zero for infinite.</param>
        /// <param name="priority">The expression priority, 1-3.</param>
        /// <param name="sequencer">The expression sequencer, 0-3.</param>
        /// <param name="intensity">The overall expression intensity, 0-100.</param>
        /// <param name="areas">The list of expression areas.</param>
        /// <param name="entries">The list of expression entries.</param>
        public Expression(string name, UInt16 patternEnum, byte repeats, byte priority, byte sequencer, byte intensity,
            BindingList<Area> areas, BindingList<Entry> entries)
        {
            this.Name = name;
            this.ExpressionEnum = patternEnum;
            this.Repeats = repeats;
            this.OutputPriority = priority;
            this.Sequencer = sequencer;
            this.Value = intensity;
            this.Areas = areas;
            this.Entries = entries;
        }

        /// <summary>
        /// ToString() override.
        /// </summary>
        /// <returns>Returns a string that represents this expression.</returns>
        public override string ToString()
        {
            return "P" + OutputPriority.ToString() + "." + InputPriority.ToString() + "  " + Name;
        }

        /// <summary>
        /// Returns a deep copy of the expression.
        /// </summary>
        /// <returns>A deep copy of the expression.</returns>
        public Expression Copy()
        {
            Expression newExp = null;

            try
            {
                //  serialize
                var stringwriter = new System.IO.StringWriter();
                var serializer = new XmlSerializer(typeof(Expression));
                serializer.Serialize(stringwriter, this);
                string str = stringwriter.ToString();

                //  deserialize into new object
                var stringReader = new System.IO.StringReader(str);
                newExp = (Expression)serializer.Deserialize(stringReader);

                //  update the area indices, which are only used to show a number in the area list
                int i = 0;
                foreach (var area in newExp.Areas)
                    area.Index = i++;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return newExp;
        }

        /// <summary>
        /// Removes all the areas' output paths.
        /// </summary>
        public void RemoveAllOutputPaths()
        {
            foreach (Area area in Areas)
                area.OutputPaths.Clear();
        }

        /// <summary>
        /// Get a list of the area names.
        /// </summary>
        /// <returns></returns>
        public List<string> AreaNames()
        {
            List<string> names = new List<string>(Areas.Count);
            foreach (Area a in Areas)
            {
                if (!a.Name.Equals(String.Empty))
                    names.Add(a.Name);
                else
                    names.Add(a.Key.ToString());
            }
            return names;
        }


    }



}
