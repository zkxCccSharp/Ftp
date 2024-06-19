using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Data.Config
{
    /// <summary>
    /// 标记Config的静态数据类，绑定该类所在的文件名相对根目录的文件目录
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FileDataAttribute : Attribute
    {
        [Description("配置文件名（包含文件类型）")]
        public string FileName { get; }
        [Description("配置文件的相对路径（相对于SMEE根目录）")]
        public string RelativePath { get; }

        public FileDataAttribute(string fileName, string relativePath)
        {
            FileName = fileName;
            RelativePath = relativePath;
        }
    }
}
