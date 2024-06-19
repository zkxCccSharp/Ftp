using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Data.Config
{
    internal class ReflectHelper
    {
        public static Dictionary<string, Dictionary<string, string>> GetNestedTypesFields<T>(T t)
        {
            var dicTypeAndItems = new Dictionary<string, Dictionary<string, string>>();
            var types = t.GetType().GetNestedTypes();
            foreach (var type in types)
            {
                var dicFieldNameAndValue = new Dictionary<string, string>();
                foreach (var item in type.GetFields())
                {
                    if (item.FieldType == typeof(string))
                    {
                        dicFieldNameAndValue.Add(item.Name, item.GetValue(item).ToString());
                    }
                }
                dicTypeAndItems.Add(type.Name, dicFieldNameAndValue);
            }
            return dicTypeAndItems;
        }
    }
}
