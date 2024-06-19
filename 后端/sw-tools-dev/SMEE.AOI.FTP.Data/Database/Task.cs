using SMEE.AOI.FTP.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Data.Database
{
    public class FtpTask
    {
        public string Id { get; set; }
        public TaskState State { get; set; }
        public string CreateTime { get; set; }
        public string StartTime { get; set; }
        public string DoneTime { get; set; }
        public OperType OperType { get; set; }
        public string RemoteIpPort { get; set; }
        public string OperParam { get; set; }
        public string ErrorMsg { get; set; }
        public string AncestorTaskId { get; set; }
        public string SessionId { get; set; }
    }
}
