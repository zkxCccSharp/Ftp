using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentFTP;
using log4net;
using SMEE.AOI.FTP.Data.ExceptionType;
using SMEE.AOI.FTP.Data.Model;

namespace SMEE.AOI.FTP.Core.FtpOperate
{
    internal class FtpDownloadFile
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FtpDownloadFile));

        /// <summary>
        /// 下载文件，如果本地有此文件，则覆盖
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool DownloadFile(RemoteIpConfig remoteIpConfig, string local, string remote)
        {

            if (string.IsNullOrWhiteSpace(local))
            {
                logger.Error($"DownloadFile: local filePath is null or emtry.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(remote))
            {
                logger.Error($"DownloadFile: remote filePath is null or emtry.");
                return false;
            }

            try
            {
                logger.Info($"DownloadFile ftp start ==> local:{local}, remote:{remote}");
                var result = Retry.Start(new Func<bool>(() =>
                {
                    FtpStatus state;
                    using (FtpClient client = FtpClientFactory.CreateOrGetConnect(remoteIpConfig).CreateInitFtpClient())
                    {
                        client.Connect();
                        logger.Info($"DownloadFile: ftp service connected, ip:{remoteIpConfig.Host}.");
                        var destDirc = PraseFileDirc(local);
                        if (!client.FileExists(remote))
                        {
                            throw new RetryTerminationException($"Remote file does not exists, remote:{remote}.");
                        }
                        state = client.DownloadFile(local, remote, FtpLocalExists.Overwrite);
                        client.Disconnect();
                    }
                    logger.Info($"DownloadFile ftp state:{state}.");
                    return state == FtpStatus.Success;
                }), "DownloadFile");
                logger.Info($"DownloadFile ftp end <== local:{local}, remote:{remote}, ftpResult:{result}.");
                return result;
            }
            catch (Exception ex)
            {
                logger.Error($"DownloadFile failed, local:{local}, remote:{remote}.", ex);
                return false;
            }
        }

        /// <summary>
        /// 解析文件的目录
        /// </summary>
        /// <param name="dest"></param>
        /// <returns></returns>
        private static string PraseFileDirc(string dest)
        {
            for (int i = dest.Length - 1; i >= 0; i--)
            {
                if (dest[i] == '/' || dest[i] == '\\')
                {
                    return dest.Substring(0, i);
                }
            }
            return dest;
        }
    }
}
