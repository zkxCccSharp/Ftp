using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Data.Config
{
    public interface IConfigOperator
    {
        void CreateFile(string path);

        void WriteToFile<T>(T data, string path) where T : class;

        T ReadFromFile<T>(string path) where T : class;
    }
}
