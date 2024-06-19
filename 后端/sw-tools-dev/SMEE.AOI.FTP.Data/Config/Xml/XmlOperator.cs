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
    public class XmlOperator : IConfigOperator
    {
        public void CreateFile(string path)
        {
            try
            {
                string dr = Path.GetDirectoryName(path);

                if (!Directory.Exists(dr))
                {
                    Directory.CreateDirectory(dr);
                }
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
                    {
                        sw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void WriteToFile<T>(T data, string path) where T : class
        {
            XmlUtil.Serialize(data, path, Encoding.UTF8);
        }

        public T ReadFromFile<T>(string path) where T : class
        {
            return XmlUtil.Deserialize<T>(path);
        }
    }
}
