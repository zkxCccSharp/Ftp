using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentFTP;
using log4net;
using SMEE.AOI.FTP.Data.ExceptionType;
using SMEE.AOI.FTP.Data.Model;

namespace SMEE.AOI.FTP.Core.FtpOperate
{
    internal static class FtpDownloadDirectory
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FtpDownloadDirectory));

        /// <summary>
        /// 下载文件夹，镜像下载，如果本地已有此文件夹，下载后使其与远端目录中的文件完全一致
        /// </summary>
        /// <param name="remoteIpConfig"></param>
        /// <param name="local"></param>
        /// <param name="remote"></param>
        /// <returns></returns>
        public static bool DownloadDirectory(RemoteIpConfig remoteIpConfig, string local, string remote)
        {
            if (string.IsNullOrWhiteSpace(local))
            {
                logger.Error($"DownloadDirectory: local filePath is null or emtry.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(remote))
            {
                logger.Error($"DownloadDirectory: remote filePath is null or emtry.");
                return false;
            }

            try
            {
                logger.Info($"DownloadDirectory ftp start ==> local:{local}, remote:{remote}");
                var result = Retry.Start(new Func<bool>(() =>
                {
                    List<FtpResult> results;
                    using (FtpClient client = FtpClientFactory.CreateOrGetConnect(remoteIpConfig).CreateInitFtpClient())
                    {
                        client.Connect();
                        logger.Info($"DownloadDirectory: ftp service connected, ip:{remoteIpConfig.Host}.");
                        if (!client.DirectoryExists(remote))
                        {
                            throw new RetryTerminationException($"Remote directory does not exists, remote:{remote}.");
                        }
                        results = client.DownloadDirectory(local, remote, FtpFolderSyncMode.Mirror, FtpLocalExists.Overwrite);
                        client.Disconnect();
                    }
                    return CheckFtpResult(results);
                }), "DownloadDirectory");
                logger.Info($"DownloadDirectory ftp end <== local:{local}, remote:{remote}, ftpResult:{result}.");
                return result;
            }
            catch (Exception ex)
            {
                logger.Error($"DownloadDirectory failed, local:{local}, remote:{remote}.", ex);
                return false;
            }
        }

        private static bool CheckFtpResult(List<FtpResult> results)
        {
            if (results == null)
            {
                return false;
            }

            var fails = results.FindAll(p => p.IsFailed);
            if (fails == null || fails.Count == 0)
            {
                return true;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < fails.Count; i++)
            {
                sb.AppendLine($"fail {i}: localPath:{fails[i].LocalPath}, name:{fails[i].Name}, exMessage:{fails[i].Exception.Message}, ex:{fails[i].Exception}.");
            }
            logger.Error($"FtpResult list has failed, message:\n {sb.ToString()}");
            return false;
        }
    }
}
