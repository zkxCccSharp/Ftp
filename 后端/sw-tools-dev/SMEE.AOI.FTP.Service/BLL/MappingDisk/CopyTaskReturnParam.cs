using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Service
{
    public class CopyTaskReturnParam : IUIDRecord
    {
        public static void Validate(CopyTaskReturnParam copyTaskReturnParam)
        {
            if (copyTaskReturnParam == null) throw new ArgumentNullException(
                "returnPipeRecord", "Return Record is Null!");
            switch (copyTaskReturnParam.ParamType)
            {
                case ParamTypeEnum.OnlyCheckExisted:
                case ParamTypeEnum.CopyTask:
                    break;
                default:
                    throw new Exception("Unknown Type Record Parameter!");
            }
            if (!copyTaskReturnParam.IsSuccessful)
            {
                var errMsg = copyTaskReturnParam.ExceptionMessage;
                if (string.IsNullOrWhiteSpace(errMsg))
                {
                    errMsg = "Unknown Exception!";
                }
                throw new Exception(errMsg);
            }
        }

        public ParamTypeEnum ParamType { get; set; } = ParamTypeEnum.Unknown;

        public string UID { get; set; } = string.Empty;

        public bool IsSuccessful { get; set; } = false;

        public string ExceptionMessage { get; set; } = string.Empty;

    }

    public interface IUIDRecord
    {
        string UID { get; }
    }

}
