using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using ESG.ExpressionLib.DataModels;

namespace ESG.ExpressionLib.DataConverters
{
    public static partial class ExpressionConverters
    {
        /// <summary>
        /// XML-serializes an expression collection to a file.
        /// </summary>
        /// <param name="ec">The expression collection.</param>
        /// <param name="pathName">The file path and name.</param>
        public static void ToXmlFile(ExpressionCollection ec, string pathName)
        {
            using (FileStream fs = new FileStream(pathName, FileMode.Create))
            {
                try
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(ExpressionCollection));
                    xSer.Serialize(fs, ec);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw new Exception("Error writing expression collection file.");
                }
            }
        }

        /// <summary>
        /// XML-deserializes an expression table from a file.
        /// </summary>
        /// <param name="pathName">The file path and name.</param>
        /// <returns>An expression collection.</returns>
        public static ExpressionCollection FromXmlFile(string pathName)
        {
            ExpressionCollection ec = null;

            //  if valid pathName
            if (File.Exists(pathName))
            {
                using (FileStream fs = new FileStream(pathName, FileMode.Open))
                {
                    try
                    {
                        XmlRootAttribute xRoot = new XmlRootAttribute
                        {
                            ElementName = "ExpressionCollection",
                            IsNullable = true
                        };
                        XmlSerializer xSer = new XmlSerializer(typeof(ExpressionCollection), xRoot);
                        ec = (ExpressionCollection)xSer.Deserialize(fs);


                        //  update the area indices, which are only used to show a number in the area list
                        foreach (var exp in ec.Expressions)
                        {
                            int i = 0;
                            foreach (var area in exp.Areas)
                                area.Index = i++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        throw new Exception("Error reading expression collection file.");
                    }
                }
            }
            return ec;
        }

    }
}
