using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMEE.AOI.FTP.Data.Model;

namespace SMEE.AOI.FTP.Data.Request
{
    public class UploadFileRequest : BaseRequest
    {
        public RemoteIpConfig RemoteIpConfig { get; set; }
        public string SrcFile { get; set; }
        public string DestFile { get; set; }
        public bool IsOverWrite { get; set; }
    }
}
