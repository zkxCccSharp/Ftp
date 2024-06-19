using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMEE.AOI.FTP.Data.Model;

namespace SMEE.AOI.FTP.Data.Request
{
    public class DownloadDirectoryRequest : BaseRequest
    {
        public RemoteIpConfig RemoteIpConfig { get; set; }
        public string LocalDirectory { get; set; }
        public string RemoteDirectory { get; set; }
    }
}
