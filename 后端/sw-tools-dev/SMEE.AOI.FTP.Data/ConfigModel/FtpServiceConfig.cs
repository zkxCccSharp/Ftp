using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMEE.AOI.FTP.Data.Config;
using SMEE.AOI.FTP.Data.Model;

namespace SMEE.AOI.FTP.Data.ConfigModel
{
    [FileData("FtpServiceConfig.json", @"Configuration\")]
    public class FtpServiceConfig
    {
        public List<RemoteIpConfig> RemoteIpConfigList { get; set; } = new List<RemoteIpConfig>();
    }
}
