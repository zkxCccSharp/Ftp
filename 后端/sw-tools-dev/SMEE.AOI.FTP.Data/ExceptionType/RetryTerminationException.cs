using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Data.ExceptionType
{
    /// <summary>
    /// 重试终止异常
    /// </summary>
    public class RetryTerminationException : Exception
    {
        /// <summary>
        /// 终止原因
        /// </summary>
        public string Reason { get; set; }

        public RetryTerminationException(string reason)
        {
            Reason = reason;
        }
    }
}
