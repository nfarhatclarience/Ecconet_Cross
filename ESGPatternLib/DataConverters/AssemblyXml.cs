using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using ESG.ExpressionLib.DataModels;

namespace ESG.ExpressionLib.DataConverters
{
    public static partial class ProductAssemblyConverters
    {
        /// <summary>
        /// XML-serializes this assembly to the given file.
        /// </summary>
        /// <param name="pathName"></param>
        public static void ToXmlFile(ProductAssemblyNode assembly, string pathName)
        {
            using (FileStream fs = new FileStream(pathName, FileMode.Create))
            {
                try
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(ProductAssemblyNode));
                    xSer.Serialize(fs, assembly);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw new Exception("Error writing product assembly file.");
                }
            }
        }

        /// <summary>
        /// XML-deserializes a product from the given file.
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        public static ProductAssemblyNode FromXmlFile(string pathName)
        {
            ProductAssemblyNode assembly = null;
            using (FileStream fs = new FileStream(pathName, FileMode.Open))
            {
                try
                {
                    XmlRootAttribute xRoot = new XmlRootAttribute
                    {
                        ElementName = "ProductAssembly",
                        IsNullable = true
                    };
                    XmlSerializer xSer = new XmlSerializer(typeof(ProductAssemblyNode), xRoot);
                    assembly = (ProductAssemblyNode)xSer.Deserialize(fs);

                    //  in the XML and JSON model files, unison output endpoints (such as LEDs)
                    //  don't have to have their Ids defined, so they are set here
                    //ProductAssemblyNode.EnumerateUnisonEndpoints(assembly);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw new Exception("Error reading product assembly file.");
                }
            }
            return assembly;
        }



    }
}


#if UNUSED_CODE

        /// <summary>
        /// XML-serializes this assembly to the given file.
        /// </summary>
        /// <param name="pathName"></param>
        public static void ToXmlFile(ProductAssembly assembly, string pathName)
        {
            using (FileStream fs = new FileStream(pathName, FileMode.Create))
            {
                try
                {
                    //  if assembly is a light bar
                    if (assembly is LightBar)
                    {
                        XmlSerializer xSer = new XmlSerializer(typeof(LightBar));
                        xSer.Serialize(fs, assembly);
                    }
                    else
                    {
                        XmlSerializer xSer = new XmlSerializer(typeof(ProductAssembly));
                        xSer.Serialize(fs, assembly);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// XML-deserializes a product from the given file.
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        public static ProductAssembly FromXmlFile(string pathName)
        {
            ProductAssembly assembly = null;
            string assemblyType = string.Empty;

            //  if valid pathName
            if (File.Exists(pathName))
            {
                //  try to get the assembly type
                try
                {
                    //  get assembly type
                    string[] lines = File.ReadAllLines(pathName);
                    int endIndex = lines[1].IndexOf(" ");
                    if ((endIndex != -1) && (endIndex > 2))
                        assemblyType = lines[1].Substring(1, endIndex - 1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                switch (assemblyType)
                {
                    case "ProductAssembly":
                        using (FileStream fs = new FileStream(pathName, FileMode.Open))
                        {
                            try
                            {
                                XmlRootAttribute xRoot = new XmlRootAttribute
                                {
                                    ElementName = "ProductAssembly",
                                    IsNullable = true
                                };
                                XmlSerializer xSer = new XmlSerializer(typeof(ProductAssembly), xRoot);
                                assembly = (ProductAssembly)xSer.Deserialize(fs);

                                //  in the XML and JSON model files, unison output endpoints (such as LEDs)
                                //  don't have to have their Ids defined, so they are set here
                                ProductAssembly.EnumerateUnisonEndpoints(assembly);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                        break;

                    case LightBar.AssemblyTypeName:
                        using (FileStream fs = new FileStream(pathName, FileMode.Open))
                        {
                            try
                            {
                                XmlRootAttribute xRoot = new XmlRootAttribute
                                {
                                    ElementName = "LightBar",
                                    IsNullable = true
                                };
                                XmlSerializer xSer = new XmlSerializer(typeof(ProductAssembly), xRoot);
                                assembly = (ProductAssembly)xSer.Deserialize(fs);

                                //  in the XML and JSON model files, LEDs properties don't have to be defined,
                                //  so they are set here
                                if (assembly is LightBar lightBar)
                                    lightBar.EnumerateLEDs();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            return assembly;
        }


#endif
