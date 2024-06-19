using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Data.Database
{
    public class SessionCommand
    {
        public string SessionId { get; set; }
        public string AncestorSessionId { get; set; }
        public string State { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? DoneTime { get; set; }
        public string OperType { get; set; }
        public string OperParam { get; set; }
        public string ErrorMsg { get; set; }

    }
}
