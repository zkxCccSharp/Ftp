using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Service
{
    public class CopyTaskParam
    {
        public ParamTypeEnum ParamType { get; set; } = ParamTypeEnum.Unknown;

        public string UID { get; set; } = string.Empty;

        public string SourceFullFilename { get; set; } = string.Empty;

        public string TargetFullFilename { get; set; } = string.Empty;

        public int MaxRepeatCount { get; set; } = 0;

    }
}
