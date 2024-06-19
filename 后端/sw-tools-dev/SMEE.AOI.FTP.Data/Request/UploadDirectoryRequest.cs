using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMEE.AOI.FTP.Data.Model;

namespace SMEE.AOI.FTP.Data.Request
{
    public class UploadDirectoryRequest : BaseRequest
    {
        public RemoteIpConfig RemoteIpConfig { get; set; }
        public string SrcDirectory { get; set; }
        public string DestDirectory { get; set; }
        public bool IsUpdateDirectory { get; set; }
        public bool IsOverWriteFile { get; set; }
    }
}
