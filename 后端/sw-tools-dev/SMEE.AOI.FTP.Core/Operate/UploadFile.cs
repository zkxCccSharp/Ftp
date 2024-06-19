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
    internal class FtpUploadFile
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FtpUploadFile));

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="remoteIpConfig"></param>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="isOverWrite">存在相同文件时是否覆盖</param>
        /// <returns></returns>
        public static bool UploadFile(RemoteIpConfig remoteIpConfig, string src, string dest, bool isOverWrite)
        {

            if (string.IsNullOrWhiteSpace(dest))
            {
                logger.Error($"UploadFile: dest filePath is null or emtry.");
                return false;
            }

            if (!File.Exists(src))
            {
                logger.Error($"UploadFile: src filePath does not exists, src:{src}.");
                return false;
            }

            try
            {
                logger.Info($"UploadFile ftp start ==> src:{src}, dest:{dest}");
                var result = Retry.Start(new Func<bool>(() =>
                {
                    FtpStatus state;
                    using (FtpClient client = FtpClientFactory.CreateOrGetConnect(remoteIpConfig).CreateInitFtpClient())
                    {
                        client.Connect();
                        logger.Info($"UploadFile: ftp service connected, ip:{remoteIpConfig.Host}.");
                        //重试时再次校验，防止上传过程中文件变化
                        if (!File.Exists(src))
                        {
                            throw new RetryTerminationException($"Local file does not exists, src:{src}.");
                        }
                        var destDirc = PraseFileDirc(dest);
                        if (!string.IsNullOrEmpty(destDirc) || !client.DirectoryExists(destDirc))
                        {
                            client.CreateDirectory(destDirc);
                        }
                        FtpRemoteExists ftpRemoteExists = isOverWrite ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip;
                        state = client.UploadFile(src, dest, ftpRemoteExists);
                        client.Disconnect();
                    }
                    logger.Info($"UploadFile ftp state:{state}.");
                    return state != FtpStatus.Failed;
                }), "UploadFile");
                logger.Info($"UploadFile ftp end <== src:{src}, dest:{dest}, ftpResult:{result}.");
                return result;
            }
            catch (Exception ex)
            {
                logger.Error($"UploadFile failed, src:{src}, dest:{dest}.", ex);
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
            if (dest.Contains('/') || dest.Contains('\\'))
            {
                for (int i = dest.Length - 1; i >= 0; i--)
                {
                    if (dest[i] == '/' || dest[i] == '\\')
                    {
                        return dest.Substring(0, i);
                    }
                }
            }
            return string.Empty;
        }
    }
}
