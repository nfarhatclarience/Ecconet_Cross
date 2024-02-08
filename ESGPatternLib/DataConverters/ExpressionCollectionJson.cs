using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ESG.ExpressionLib.DataModels;

namespace ESG.ExpressionLib.DataConverters
{
    public static partial class ExpressionConverters
    {
        /// <summary>
        /// Deserializes an expression collection from a file, automatically detecting JSON or XML.
        /// </summary>
        /// <param name="pathName">The file path containing the json.</param>
        /// <returns>An expression collection, or null.</returns>
        public static ExpressionCollection FromFile(string pathName)
        {
            //  get the file type
            bool isXml = false;
            try
            {
                //  read product assembly
                if (File.ReadAllText(pathName).Contains("XMLSchema"))
                    isXml = true;
            }
            catch
            {
                throw new Exception("Unable to read expression collection file");
            }

            //  convert
            if (isXml)
                return FromXmlFile(pathName);
            else
                return FromJsonFile(pathName);
        }


        /// <summary>
        /// JSON-serializes an expression collection to a file.
        /// </summary>
        /// <param name="ec">The expression collection.</param>
        /// <param name="pathName">The file path and name.</param>
        public static void ToJsonFile(ExpressionCollection ec, string pathName)
        {
            try
            {
                string json = JsonConvert.SerializeObject(ec,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                File.WriteAllText(pathName, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new Exception("Error writing expression collection file.");
            }
        }

        /// <summary>
        /// XML-deserializes an expression table from a file.
        /// </summary>
        /// <param name="pathName">The file path and name.</param>
        /// <returns>An expression collection.</returns>
        public static ExpressionCollection FromJsonFile(string pathName)
        {
            ExpressionCollection ec = null;
            try
            {
                //  convert file text to assembly
                string json = File.ReadAllText(pathName);
                ec = JsonConvert.DeserializeObject<ExpressionCollection>(json,
                    new JsonSerializerSettings()
                    {
                        Converters = { new JsonStepConverter() }
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new Exception("Error reading expression collection file.");
            }
            return ec;
        }

    }
}