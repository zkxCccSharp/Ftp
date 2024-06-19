using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace SMEE.AOI.FTP.Service
{
    public class FtpAPI
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FtpAPI));

        /// <summary>
        /// 上传整个文件夹
        /// </summary>
        /// <param name="lowerComputerIP">下位机ip（FTPClient所在机器）</param>
        /// <param name="ftpServerName">ftp服务名称</param>
        /// <param name="src">源路径</param>
        /// <param name="dest">目标路径</param>
        /// <param name="isUpdateDirectory">远端存在相同目录时，更新还是镜像（删除再创建); true: 更新, false: 镜像</param>
        /// <param name="isOverWriteFile">isUpdateDirectory选择更新时，存在相同文件覆盖还是跳过; true: 覆盖, false: 跳过</param>
        /// <returns></returns>
        public static bool UploadDirectory(string lowerComputerIP, string ftpServerName, string src, string dest, bool isUpdateDirectory, bool isOverWriteFile)
        {
            try
            {
                return FtpService.UploadDirectory(lowerComputerIP, ftpServerName, src, dest, isUpdateDirectory, isOverWriteFile);
            }
            catch (Exception ex)
            {
                logger.Error("UploadDirectory failed.", ex);
                return false;
            }
        }

        /// <summary>
        /// 上传文件夹
        /// </summary>
        /// <param name="lowerComputerIP">下位机ip（FTPClient所在机器）</param>
        /// <param name="ftpServerName">ftp服务名称</param>
        /// <param name="src">源路径</param>
        /// <param name="dest">目标路径</param>
        /// <param name="isOverWrite">存在相同文件时是否覆盖; true: 覆盖, false: 跳过</param>
        /// <returns></returns>
        public static bool UploadFile(string lowerComputerIP, string ftpServerName, string src, string dest, bool isOverWrite)
        {
            try
            {
                return FtpService.UploadFile(lowerComputerIP, ftpServerName, src, dest, isOverWrite);
            }
            catch (Exception ex)
            {
                logger.Error("UploadFile failed.", ex);
                return false;
            }
        }

        /// <summary>
        /// 下载文件夹，镜像下载，如果本地已有此文件夹，下载后使其与远端目录中的文件完全一致
        /// </summary>
        /// <param name="lowerComputerIP">下位机ip（FTPClient所在机器）</param>
        /// <param name="ftpServerName">ftp服务名称</param>
        /// <param name="loacl"></param>
        /// <param name="remote"></param>
        /// <returns></returns>
        public static bool DownloadDirectory(string lowerComputerIP, string ftpServerName, string loacl, string remote)
        {
            try
            {
                return FtpService.DownloadDirectory(lowerComputerIP, ftpServerName, loacl, remote);
            }
            catch (Exception ex)
            {
                logger.Error("UploadDirectory failed.", ex);
                return false;
            }
        }

        /// <summary>
        /// 下载文件，如果本地有此文件，则覆盖
        /// </summary>
        /// <param name="lowerComputerIP">下位机ip（FTPClient所在机器）</param>
        /// <param name="ftpServerName">ftp服务名称</param>
        /// <param name="loacl"></param>
        /// <param name="remote"></param>
        /// <returns></returns>
        public static bool DownloadFile(string lowerComputerIP, string ftpServerName, string loacl, string remote)
        {
            try
            {
                return FtpService.DownloadFile(lowerComputerIP, ftpServerName, loacl, remote);
            }
            catch (Exception ex)
            {
                logger.Error("UploadFile failed.", ex);
                return false;
            }
        }

        /// <summary>
        /// 获取指定ftp服务端的ip
        /// </summary>
        /// <param name="lowerComputerIP">下位机ip（FTPClient所在机器）</param>
        /// <param name="ftpServerName">ftp服务名称</param>
        /// <returns>ftp服务端的ip</returns>
        public static string GetFtpServereIP(string lowerComputerIP, string ftpServerName)
        {
            try
            {
                return FtpService.GetFtpServereIP(lowerComputerIP, ftpServerName);
            }
            catch (Exception ex)
            {
                logger.Error("GeFtpServereIP failed.", ex);
                throw ex;
            }
        }
    }
}
