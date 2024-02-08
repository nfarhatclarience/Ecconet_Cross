using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;


namespace ESG.ExpressionLib.DataModels
{
    /// <summary>
    /// A expression collection is a named list of expressions.
    /// </summary>
    [XmlRoot("ExpressionCollection")]
    [JsonObject("ExpressionCollection")]
    public class ExpressionCollection
    {
        /// <summary>
        /// The expression collection name.
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// The expressions in the collection.
        /// </summary>
        [XmlArrayItem("Expression", typeof(Expression))]
        [XmlArray("Expressions")]
        [JsonProperty("Expressions")]
        public BindingList<Expression> Expressions { get => _expressions; set => _expressions = value ?? _expressions; }
        private BindingList<Expression> _expressions = new BindingList<Expression>();


        /// <summary>
        /// Constructor.
        /// </summary>
        public ExpressionCollection()
        {
            Expressions = new BindingList<Expression>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expressions"></param>
        public ExpressionCollection(string name, BindingList<Expression> expressions)
        {
            this.Name = name;
            this.Expressions = expressions;
        }


        /// <summary>
        /// Removes the light engine assignments from all expressions.
        /// </summary>
        public void RemoveAllLightEngineAssignments()
        {
            foreach (Expression e in Expressions)
                e.RemoveAllOutputPaths();
        }

        /// <summary>
        /// Create a deep copy of this expression collection.
        /// </summary>
        /// <returns>A deep copy of this expression collection.</returns>
        public ExpressionCollection DeepCopy()
        {
            var ec = new ExpressionCollection();
            ec.Name = Name;
            ec.Expressions = new BindingList<Expression>();
            foreach (Expression exp in Expressions)
                ec.Expressions.Add(exp.Copy());
            return ec;
        }

    }
}
