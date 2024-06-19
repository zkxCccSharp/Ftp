using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Data.ConfigModel
{
    public class FtpClientConfig
    {
        /// <summary>
        /// 传输模式
        /// </summary>
        public int DataConnectionType { get; set; } = 0;

        public int ConnectTimeout { get; set; } = 15000;

        public int ReadTimeout { get; set; } = 15000;

        public int DataConnectionConnectTimeout { get; set; } = 15000;

        public int DataConnectionReadTimeout { get; set; } = 15000;
    }
}
