using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using SMEE.AOI.FTP.IPC.HttpIPC;
using SMEE.AOI.FTP.Data.Config;
using SMEE.AOI.FTP.Data.ConfigModel;
using SMEE.AOI.FTP.Data.Model;
using SMEE.AOI.FTP.Data.Request;
using SMEE.AOI.FTP.Data.Response;

namespace SMEE.AOI.FTP.Service
{
    internal class FtpService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FtpService));

        private static FtpServiceConfig ftpServiceConfig;
        //上一次连接成功的ip
        private static int port = -1;

        #region Public method

        public static bool UploadDirectory(string ip, string ftpServerName, string src, string dest, bool isUpdateDirectory, bool isOverWriteFile)
        {
            #region Connect
            string baseUrl = "";
            if (!TryConnect(ip, out baseUrl))
            {
                logger.Error("UploadDirectory: Unable connect with ftpClient, please check lower computer ftpClient or check ftpClient log.");
                return false;
            }
            string routeName = "UploadDirectory";
            string url = baseUrl + routeName;
            #endregion

            #region data
            var remoteIpConfig = GetRemoteIpConfigByName(baseUrl, ftpServerName);
            if (remoteIpConfig == null)
            {
                logger.Error($"UploadDirectory: RemoteIpConfig is null, please check lower computer remote FTP service config.");
                return false;
            }
            var request = new UploadDirectoryRequest();
            request.RemoteIpConfig = remoteIpConfig;
            request.SrcDirectory = src;
            request.DestDirectory = dest;
            request.IsUpdateDirectory = isUpdateDirectory;
            request.IsOverWriteFile = isOverWriteFile;
            string resp = JsonConvert.SerializeObject(request);
            #endregion

            #region IPC
            string resStr = HttpClient.PostUrl(url, resp, false);
            #endregion

            #region Result
            var res = JsonConvert.DeserializeObject<BaseResponse<UploadDirectoryResponse>>(resStr);
            if (res == null || res.Body == null)
            {
                logger.Error($"UploadDirectory: parse result is null, response string:{resStr}.");
                return false;
            }
            return res.Body.IsSuccess;
            #endregion
        }

        public static bool UploadFile(string ip, string ftpServerName, string src, string dest, bool isOverWrite)
        {
            #region Connect
            string baseUrl = "";
            if (!TryConnect(ip, out baseUrl))
            {
                logger.Error("UploadFile: Unable connect with ftpClient, please check lower computer ftpClient or check ftpClient log.");
                return false;
            }
            string routeName = "UploadFile";
            string url = baseUrl + routeName;
            #endregion

            #region data
            var remoteIpConfig = GetRemoteIpConfigByName(baseUrl, ftpServerName);
            var request = new UploadFileRequest();
            request.RemoteIpConfig = remoteIpConfig;
            request.SrcFile = src;
            request.DestFile = dest;
            request.IsOverWrite = isOverWrite;
            string resp = JsonConvert.SerializeObject(request);
            #endregion

            #region IPC
            string resStr = HttpClient.PostUrl(url, resp, false);
            #endregion

            #region Result
            var res = JsonConvert.DeserializeObject<BaseResponse<UploadFileResponse>>(resStr);
            if (res == null || res.Body == null)
            {
                logger.Error($"UploadFile: parse result is null, response string:{resStr}.");
                return false;
            }
            return res.Body.IsSuccess;
            #endregion
        }

        public static bool DownloadDirectory(string ip, string ftpServerName, string local, string remote)
        {
            #region Connect
            string baseUrl = "";
            if (!TryConnect(ip, out baseUrl))
            {
                logger.Error("DownloadDirectory: Unable connect with ftpClient, please check lower computer ftpClient or check ftpClient log.");
                return false;
            }
            string routeName = "DownloadDirectory";
            string url = baseUrl + routeName;
            #endregion

            #region data
            var remoteIpConfig = GetRemoteIpConfigByName(baseUrl, ftpServerName);
            var request = new DownloadDirectoryRequest();
            request.RemoteIpConfig = remoteIpConfig;
            request.LocalDirectory = local;
            request.RemoteDirectory = remote;
            string resp = JsonConvert.SerializeObject(request);
            #endregion

            #region IPC
            string resStr = HttpClient.PostUrl(url, resp, false);
            #endregion

            #region Result
            var res = JsonConvert.DeserializeObject<BaseResponse<DownloadDirectoryResponse>>(resStr);
            if (res == null || res.Body == null)
            {
                logger.Error($"DownloadDirectory: parse result is null, response string:{resStr}.");
                return false;
            }
            return res.Body.IsSuccess;
            #endregion
        }

        public static bool DownloadFile(string ip, string ftpServerName, string local, string remote)
        {
            #region Connect
            string baseUrl = "";
            if (!TryConnect(ip, out baseUrl))
            {
                logger.Error("DownloadFile: Unable connect with ftpClient, please check lower computer ftpClient or check ftpClient log.");
                return false;
            }
            string routeName = "DownloadFile";
            string url = baseUrl + routeName;
            #endregion

            #region data
            var remoteIpConfig = GetRemoteIpConfigByName(baseUrl, ftpServerName);
            var request = new DownloadFileRequest();
            request.RemoteIpConfig = remoteIpConfig;
            request.LocalFile = local;
            request.RemoteFile = remote;
            string resp = JsonConvert.SerializeObject(request);
            #endregion

            #region IPC
            string resStr = HttpClient.PostUrl(url, resp, false);
            #endregion

            #region Result
            var res = JsonConvert.DeserializeObject<BaseResponse<DownloadFileResponse>>(resStr);
            if (res == null || res.Body == null)
            {
                logger.Error($"DownloadFile: parse result is null, response string:{resStr}.");
                return false;
            }
            return res.Body.IsSuccess;
            #endregion
        }
        
        public static string GetFtpServereIP(string ip, string ftpServerName)
        {
            #region Connect
            string baseUrl = "";
            if (!TryConnect(ip, out baseUrl))
            {
                string errMsg = "GetFtpServereInfo: Unable connect with ftpClient, please check lower computer ftpClient or check ftpClient log.";
                logger.Error(errMsg);
                throw new Exception(errMsg);
            }
            #endregion

            var remoteIpConfig = GetRemoteIpConfigByName(baseUrl, ftpServerName);
            return remoteIpConfig.Host;
        }
        #endregion

        #region Private

        private static RemoteIpConfig GetRemoteIpConfigByName(string baseUrl, string ftpServerName)
        {
            RemoteIpConfig remoteIpConfig = null;
            if (ftpServiceConfig != null && ftpServiceConfig.RemoteIpConfigList != null && ftpServiceConfig.RemoteIpConfigList.Count > 0)
            {
                remoteIpConfig = ftpServiceConfig.RemoteIpConfigList.Find(p => p.ServiceName == ftpServerName);

                if (remoteIpConfig != null)
                {
                    return remoteIpConfig;
                }
            }

            #region IPC
            string routeName = "RemoteIpConfig";
            string url = baseUrl + routeName;
            string resStr = HttpClient.PostUrl(url, "", false);
            #endregion

            #region Result
            var res = JsonConvert.DeserializeObject<BaseResponse<FtpServiceConfig>>(resStr);
            if (res == null || res.Body == null || res.Body.RemoteIpConfigList == null || res.Body.RemoteIpConfigList.Count == 0)
            {
                throw new Exception($"GetRemoteIpConfigByName: parse result is null, response string:{resStr}.");
            }

            ftpServiceConfig = res.Body;

            remoteIpConfig = res.Body.RemoteIpConfigList.Find(p => p.ServiceName == ftpServerName);
            if (remoteIpConfig == null)
            {
                throw new Exception($"GetRemoteIpConfigByName: RemoteIpConfig is null, please check lower computer remote FTP service config, ftpServerName:{ftpServerName}.");
            }
            return remoteIpConfig;
            #endregion
        }

        private static bool TryConnect(string ip, out string baseUrl)
        {
            baseUrl = "";
            if (port == -1 || !Helper.TryConnect(ip, port))
            {
                port = Helper.FindFtpPort(ip);
            }
            if (port == -1)
            {
                logger.Info("Unsuccessful access to the FTP client of the lower computer, please check whether the configured is correct, or lower computer FTP client has started.");
                return false;
            }
            baseUrl = $"http://{ip}:{port}/";
            logger.Info($"Try Connect success, base url:{baseUrl}.");
            return true;
        }

        #endregion

    }
}
