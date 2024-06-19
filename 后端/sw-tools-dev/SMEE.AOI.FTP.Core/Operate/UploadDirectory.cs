using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentFTP;
using log4net;
using SMEE.AOI.FTP.Data.Config;
using SMEE.AOI.FTP.Data.ExceptionType;
using SMEE.AOI.FTP.Data.Model;

namespace SMEE.AOI.FTP.Core.FtpOperate
{
    internal class FtpUploadDirectory
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FtpUploadDirectory));

        /// <summary>
        /// 上传整个文件夹
        /// </summary>
        /// <param name="remoteIpConfig">ftp服务配置</param>
        /// <param name="src">源路径</param>
        /// <param name="dest">目标路径</param>
        /// <param name="isUpdateDirectory">远端存在相同目录时，更新还是镜像（删除再创建）</param>
        /// <param name="isOverWriteFile">isUpdateDirectory选择更新时，存在相同文件覆盖还是跳过</param>
        /// <returns></returns>
        public static bool UploadDirectory(RemoteIpConfig remoteIpConfig, string src, string dest, bool isUpdateDirectory, bool isOverWriteFile)
        {
            if (string.IsNullOrWhiteSpace(dest))
            {
                logger.Error($"UploadDirectory: dest directory is null or emtry.");
                return false;
            }

            if (!Directory.Exists(src))
            {
                logger.Error($"UploadDirectory: src directory does not exists, src:{src}.");
                return false;
            }

            logger.Info($"UploadDirectory ftp start ==> src:{src}, dest:{dest}");
            try
            {
                List<FtpResult> failResults = null;
                
                var isSuccess = Retry.Start(()=> 
                {
                    var onceResult = UploadDirectoryOnce(remoteIpConfig, src, dest, isUpdateDirectory, isOverWriteFile, out failResults);
                    if (!onceResult && failResults == null)
                    {
                        //还没有进行传输，可能是连接失败，整体重试
                        return false;
                    }
                    if(!onceResult && failResults != null)
                    {
                        //已经进行传输，有内部成功的文件，不再整体重试，后面单独重试
                        throw new RetryTerminationException("UploadDirectoryOnce has successed and failed, ");
                    }
                    return true;
                }, "UploadDirectoryOnce");

                //传输时失败，对失败列表重试
                if (!isSuccess && failResults != null && failResults.Count > 0)
                {
                    Retry.Start(()=> {
                        var failDirResults = new List<FtpResult>();
                        foreach (var item in failResults)
                        {
                            if (item.Type == FtpObjectType.File)
                            {
                                using (FtpClient client = FtpClientFactory.CreateOrGetConnect(remoteIpConfig).CreateInitFtpClient())
                                {
                                    client.Connect();
                                    var destDirc = PraseFileDirc(item.RemotePath);
                                    if (!client.DirectoryExists(destDirc))
                                    {
                                        client.CreateDirectory(destDirc);
                                    }
                                    FtpRemoteExists ftpRemoteExists = FtpRemoteExists.Skip;
                                    var fileResult = client.UploadFile(item.LocalPath, item.RemotePath, ftpRemoteExists);
                                    if (fileResult == FtpStatus.Failed)
                                    {
                                        logger.Error($"UploadDirectory: retry file src:{item.LocalPath} failed, dest:{item.RemotePath}, state:{fileResult}.");
                                    }
                                    client.Disconnect();
                                    item.IsFailed = (fileResult == FtpStatus.Failed);
                                    item.IsSuccess = !item.IsFailed;
                                }
                            }
                            else if (item.Type == FtpObjectType.Directory)
                            {
                                item.IsSuccess = UploadDirectoryOnce(remoteIpConfig, src, dest, isUpdateDirectory, isOverWriteFile, out failDirResults);
                                if (!item.IsSuccess && failDirResults != null && failDirResults.Count > 0)
                                {
                                    //内部文件传输失败，将目录移除重试列表
                                    item.IsSuccess = true;
                                }
                                item.IsFailed = !item.IsSuccess;
                            }
                            else
                            {
                                logger.Error($"UploadDirectory: retry item src:{src} does not exists.");
                            }
                        }
                        if (failDirResults != null)
                        {
                            failResults.AddRange(failDirResults);
                        }
                        failResults = failResults.FindAll(p => p.IsFailed);
                        logger.Info($"UploadDirectory: retry fails count:{failResults.Count}.");
                        return failResults.Count == 0;
                    }, "UploadDirectoryFails");
                    
                    if (failResults.Count > 0)
                    {
                        WriteFtpFailResults(failResults);
                    }
                    else
                    {
                        isSuccess = true;
                    }
                }

                logger.Info($"UploadDirectory ftp end <== src:{src}, dest:{dest}, ftpResult:{isSuccess}.");
                return isSuccess;
            }
            catch (Exception ex)
            {
                logger.Error($"UploadDirectory failed, src:{src}, dest:{dest}.", ex);
                return false;
            }
        }

        /// <summary>
        /// 上传文件夹一次，无重试
        /// <returns></returns>
        private static bool UploadDirectoryOnce(RemoteIpConfig remoteIpConfig, string src, string dest, bool isUpdateDirectory, bool isOverWriteFile, out List<FtpResult> failResults)
        {
            failResults = null;

            List<FtpResult> results = null;
            using (FtpClient client = FtpClientFactory.CreateOrGetConnect(remoteIpConfig).CreateInitFtpClient())
            {
                client.Connect();
                logger.Info($"UploadDirectory: ftp service connected, ip:{remoteIpConfig.Host}.");
                if (!isUpdateDirectory)
                {
                    if (client.DirectoryExists(dest))
                    {
                        logger.Info($"UploadDirectory: dest directory exists, not update directory, delete directory, dest:{dest}.");
                        client.DeleteDirectory(dest);
                    }
                    client.CreateDirectory(dest);
                }
                FtpFolderSyncMode ftpFolderSyncMode = isUpdateDirectory ? FtpFolderSyncMode.Update : FtpFolderSyncMode.Mirror;
                FtpRemoteExists ftpRemoteExists = FtpRemoteExists.NoCheck;
                if (ftpFolderSyncMode == FtpFolderSyncMode.Update)
                {
                    ftpRemoteExists = isOverWriteFile ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip;
                }
                results = client.UploadDirectory(src, dest, ftpFolderSyncMode, ftpRemoteExists);
                failResults = results.FindAll(p => p.IsFailed);
                client.Disconnect();
            }

            return CheckFtpResult(results);
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

            WriteFtpFailResults(fails);
            return false;
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

        private static void WriteFtpFailResults(List<FtpResult> fails)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < fails.Count; i++)
            {
                sb.AppendLine($"fail {i}: localPath:{fails[i].LocalPath}, name:{fails[i].Name}, exMessage:{fails[i].Exception.Message}, ex:{fails[i].Exception}.");
            }
            logger.Error($"FtpResult list has failed, message:\n {sb.ToString()}");
        }

        /// <summary>
        /// 服务器路径转换为本地映射盘路径
        /// </summary>
        /// <param name="dest"></param>
        /// <returns></returns>
        private static string GetLocalDir(string dest)
        {
            var serverMapDiskPath = IniHelper.ReadValueFromIni("Ftp", "ServerMapDiskPath", "");
            return Path.Combine(serverMapDiskPath, dest);
        }

        /// <summary>
        /// 本地文件夹拷贝
        /// </summary>
        /// <param name="src">源文件夹</param>
        /// <param name="dest">目标文件夹</param>
        /// <param name="isOverWrite">存在相同文件时，是否覆盖</param>
        private static void LocalCopyDirectory(string src, string dest, bool isOverWrite)
        {
            logger.Info($"LocalCopyDirectory: [Enter]: src{src}, dest{dest}.");
            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }
            string[] files = Directory.GetFiles(src);
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string destPath = Path.Combine(dest, fileName);
                File.Copy(file, dest, isOverWrite);
            }

            string[] dirs = Directory.GetDirectories(src);
            foreach (var subdir in dirs)
            {
                string dirName = new DirectoryInfo(subdir).Name;
                string destPath = Path.Combine(dest, dirName);
                LocalCopyDirectory(subdir, destPath, isOverWrite);
            }
            logger.Info($"LocalCopyDirectory: [Leave].");
        }
    }
}
