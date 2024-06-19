using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentFTP;
using log4net;
using SMEE.AOI.FTP.Core.FtpOperate;
using SMEE.AOI.FTP.Data.Config;
using SMEE.AOI.FTP.Data.ExceptionType;
using SMEE.AOI.FTP.Data.Model;

namespace SMEE.AOI.FTP.Core
{
    /// <summary>
    /// 需要提供服务器ip，用户名，密码
    /// 需要提供本地文件夹绝对路径，服务器相对路径
    /// 需要提供服务器相对路径对应的本地映射盘绝对路径
    /// </summary>
    public class FtpCore
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FtpCore));
        
        #region public Method
        
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
            return FtpUploadDirectory.UploadDirectory(remoteIpConfig, src, dest, isUpdateDirectory, isOverWriteFile);
        }

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
            return FtpUploadFile.UploadFile(remoteIpConfig, src, dest, isOverWrite);
        }

        /// <summary>
        /// 下载文件夹，镜像下载，如果本地已有此文件夹，下载后使其与远端目录中的文件完全一致
        /// </summary>
        /// <param name="remoteIpConfig"></param>
        /// <param name="local"></param>
        /// <param name="remote"></param>
        /// <returns></returns>
        public static bool DownloadDirectory(RemoteIpConfig remoteIpConfig, string local, string remote)
        {
            return FtpDownloadDirectory.DownloadDirectory(remoteIpConfig, local, remote);
        }

        /// <summary>
        /// 下载文件，如果本地有此文件，则覆盖
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool DownloadFile(RemoteIpConfig remoteIpConfig, string local, string remote)
        {
            return FtpDownloadFile.DownloadFile(remoteIpConfig, local, remote);
        }
        #endregion
        
    }
}
