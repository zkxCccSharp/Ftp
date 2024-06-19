using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using SMEE.AOI.FTP.IPC.HttpIPC;
using SMEE.AOI.FTP.Data.Response;

namespace SMEE.AOI.FTP.Service
{
    public class Helper
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HttpService));
        private const int FindCount = 10;

        public static int FindFtpPort(string ip)
        {
            int port = 23333;
            int maxPort = port + FindCount;
            while (port <= maxPort)
            {
                string url = $"http://{ip}:{port}/Heart";
                try
                {
                    string resStr = HttpClient.PostUrl(url, "", true);
                    var res = JsonConvert.DeserializeObject<BaseResponse<object>>(resStr);
                    if (res.StatusCode == (int)HttpStatusCode.Success)
                    {
                        return port;
                    }
                }
                catch(Exception ex)
                {
                    //连接失败
                    logger.Debug($"FindFtpPort ex, port:{port}.", ex);
                }
                port++;
            }
            return -1;
        }

        public static bool TryConnect(string ip, int port)
        {
            string url = $"http://{ip}:{port}/Heart";
            try
            {
                string resStr = HttpClient.PostUrl(url, "", true);
                var res = JsonConvert.DeserializeObject<BaseResponse<object>>(resStr);
                if (res.StatusCode == (int)HttpStatusCode.Success)
                {
                    return true;
                }
            }
            catch
            {
                //连接失败
                return false;
            }
            return false;
        }
    }
}
