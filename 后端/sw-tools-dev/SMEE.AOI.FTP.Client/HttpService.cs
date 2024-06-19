using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using SMEE.AOI.FTP.IPC.HttpIPC;
using SMEE.AOI.FTP.Data.Config;
using SMEE.AOI.FTP.IPC;
using SMEE.AOI.FTP.Client.HttpIPC;

namespace SMEE.AOI.FTP.Client
{
    public class HttpService
    {

        private static readonly ILog logger = LogManager.GetLogger(typeof(HttpService));

        public static void Run()
        {
            string ip = IniHelper.ReadValueFromIni("Http", "IP", "");
            if (string.IsNullOrWhiteSpace(ip))
            {
                logger.Error(@"Run: Please configure the lower computer IP first, configuration location:'..\Configuration\SmeeFtpConfig.ini', under 'Http' node. ");
                throw new Exception("Http ip is emtry.");
            }

            int port = Helper.GetPort();
            IniHelper.SaveValueToIni("Http", "Port", port.ToString());

            string url = $"http://{ip}:{port}/";
            logger.Info($"Run: ip:{ip},port:{port},url:{url}");

            IPC.HttpIPC.HttpService.Listen(url, Route.RoutePath);
        }
    }
}
