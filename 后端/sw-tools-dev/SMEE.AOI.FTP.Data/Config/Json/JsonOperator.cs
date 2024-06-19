using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SMEE.AOI.FTP.Data.Config
{
    public class JsonOperator : IConfigOperator
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
                    using (StreamWriter sw = new StreamWriter(path, false, Encoding.Default))
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
            var json = JsonConvert.SerializeObject(data);
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(json);
            }
        }

        public T ReadFromFile<T>(string path) where T : class
        {
            var res = default(T);
            var jsonSb = new StringBuilder();
            var line = string.Empty;
            using (StreamReader reader = new StreamReader(path))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    jsonSb.Append(line);
                }
                res = JsonConvert.DeserializeObject<T>(jsonSb.ToString());
            }
            return res;
        }
    }
}
