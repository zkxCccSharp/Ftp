using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMEE.AOI.FTP.Data.Model;

namespace SMEE.AOI.FTP.Data.Request
{
    public class DownloadFileRequest : BaseRequest
    {
        public RemoteIpConfig RemoteIpConfig { get; set; }
        public string LocalFile { get; set; }
        public string RemoteFile { get; set; }
    }
}
