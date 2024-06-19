using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMEE.AOI.FTP.Data.Model;

namespace SMEE.AOI.FTP.Core
{
    internal class FtpClientFactory
    {
        private static Dictionary<string, FtpClientCore> dicHostAndClient = new Dictionary<string, FtpClientCore>();
        
        public static FtpClientCore CreateOrGetConnect(RemoteIpConfig remoteIpConfig)
        {
            if (!dicHostAndClient.ContainsKey(remoteIpConfig.Host))
            {
                dicHostAndClient.Add(remoteIpConfig.Host, new FtpClientCore(remoteIpConfig));
            }
            return dicHostAndClient[remoteIpConfig.Host];
        }

    }
}
