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
    public class JsonTreeConverter : JsonConverter
    {
        //  enumeration of branch type
        private enum BranchType
        {
            Input,
            Output
        }

        //  indicate current branch type
        private BranchType currentBranchType;


        //  do not deserialize top node here, because that would be recursive
        private bool haveTopNode = false;


        public override bool CanConvert(Type objectType)
        {
            if ((objectType == typeof(ProductAssemblyNode)) && !haveTopNode)
            {
                haveTopNode = true;
                return false;
            }
            return (objectType == typeof(ComponentTreeNode));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //  get json object
            try
            {
                JObject jo = JObject.Load(reader);

                //  types of component tree nodes
                if (jo["ModelName"] != null)
                {
                    return jo.ToObject<ProductAssemblyNode>(serializer);
                }
                else if (jo["InputType"] != null)
                {
                    currentBranchType = BranchType.Input;
                    return jo.ToObject<InputNode>(serializer);
                }
                else if (jo["InputArrayType"] != null)
                {
                    currentBranchType = BranchType.Input;
                    return jo.ToObject<InputArrayNode>(serializer);
                }
                else if (jo["OutputType"] != null)
                {
                    currentBranchType = BranchType.Output;
                    return jo.ToObject<OutputNode>(serializer);
                }
                else if (jo["OutputArrayType"] != null)
                {
                    currentBranchType = BranchType.Output;
                    return jo.ToObject<OutputArrayNode>(serializer);
                }
                else if (jo["Color"] != null)
                {
                    currentBranchType = BranchType.Output;
                    return jo.ToObject<OutputColorNode>(serializer);
                }
                else if (currentBranchType == BranchType.Input)
                {
                    return jo.ToObject<InputNode>(serializer);
                }
                else
                {
                    return jo.ToObject<OutputNode>(serializer);
                }
            }
            catch { }
            return null;
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
