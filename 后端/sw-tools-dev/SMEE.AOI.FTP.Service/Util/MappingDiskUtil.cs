using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Service
{
    public static class MappingDiskUtil
    {
        public static void ValidateObjectNotNull<T>(
            T obj, string name) where T : class
        {
            name = name ?? string.Empty;
            if (obj == null) throw new ArgumentNullException(
                name, $"{name} is Null!");
        }

    }
}
