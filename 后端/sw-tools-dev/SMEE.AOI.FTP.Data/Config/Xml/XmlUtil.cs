using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SMEE.AOI.FTP.Data.Config
{
    public static class XmlUtil
    {
        private const string IndentChars = "    ";

        /// <summary>
        /// The serialized object to xml file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="outputFilePath"></param>
        /// <returns></returns>
        public static void Serialize<T>(T obj, string outputFilePath, Encoding encoding) where T : class
        {
            try
            {
                // 目录不存在则创建目录
                FileInfo finfo = new FileInfo(outputFilePath);
                if (!(finfo.Directory.Exists))
                    finfo.Directory.Create();

                XmlWriterSettings settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = false,
                    Indent = true,
                    IndentChars = IndentChars,
                    Encoding = encoding
                };

                // 文件存在则覆盖原文件
                using (var xmlWriter = XmlWriter.Create(outputFilePath, settings))
                {
                    // Remove default namespace. 
                    // xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);

                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(xmlWriter, obj, namespaces);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Serialize {nameof(T)} object to {outputFilePath} failed.", ex);
            }
        }

        /// <summary>
        /// The deserialized xml file to object of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputFilePath"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string inputFilePath) where T : class
        {
            if (!File.Exists(inputFilePath))
            {
                throw new ArgumentException($"File {inputFilePath} dones not exists.");
            }

            try
            {
                using (var xmlReader = XmlReader.Create(inputFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Deserialize {inputFilePath} to {nameof(T)} failed.", ex);
            }
        }

        /// <summary>
        ///  The serialized object to xml string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize<T>(T obj, Encoding encoding) where T : class
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        OmitXmlDeclaration = false,
                        Indent = true,
                        IndentChars = IndentChars,
                        Encoding = encoding
                    };

                    using (var xmlWriter = XmlWriter.Create(ms, settings))
                    {
                        // Remove default namespace. 
                        // xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
                        var namespaces = new XmlSerializerNamespaces();
                        namespaces.Add(string.Empty, string.Empty);

                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        serializer.Serialize(xmlWriter, obj, namespaces);
                    }

                    var xml = encoding.GetString(ms.ToArray());
                    return xml;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Serialize {nameof(T)} object xml string failed.", ex);
            }
        }

        /// <summary>
        /// The deserialized xml string to object of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml, Encoding encoding) where T : class
        {
            try
            {
                using (var ms = new MemoryStream(encoding.GetBytes(xml)))
                {
                    using (var xmlReader = XmlReader.Create(ms))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        return (T)serializer.Deserialize(xmlReader);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Deserialize xml string to {nameof(T)} failed.", ex);
            }

        }
    }
}
