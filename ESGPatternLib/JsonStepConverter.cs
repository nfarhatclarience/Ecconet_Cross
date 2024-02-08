using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ESG.ExpressionLib.DataModels;


namespace ESG.ExpressionLib
{
    public class JsonStepConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Expression.Entry));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //  get json object
            JObject jo = JObject.Load(reader);

            //  types of expression entries
            if (jo["Tokens"] != null)
                return jo.ToObject<Expression.Step>(serializer);

            else if (jo["ExpressionEnum"] != null)
                return jo.ToObject<Expression.NestedExpression>(serializer);

            else if (jo["Repeats"] != null)
                return jo.ToObject<Expression.RepeatSectionStart>(serializer);

            else
                return jo.ToObject<Expression.RepeatSectionEnd>(serializer);
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }
}
