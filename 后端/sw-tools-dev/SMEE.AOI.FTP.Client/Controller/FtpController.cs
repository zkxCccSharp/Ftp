using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using SMEE.AOI.FTP.Client.Converter;
using SMEE.AOI.FTP.Core;
using SMEE.AOI.FTP.Data;
using SMEE.AOI.FTP.Data.Common;
using SMEE.AOI.FTP.Data.Config;
using SMEE.AOI.FTP.Data.ConfigModel;
using SMEE.AOI.FTP.Data.Request;
using SMEE.AOI.FTP.Data.Response;
using static System.Collections.Specialized.BitVector32;

namespace SMEE.AOI.FTP.Client
{
    /// <summary>
    /// Controller
    /// </summary>
    public class FtpController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FtpController));

        #region Controller

        /// <summary>
        /// 心跳
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static string Heart(Session session)
        {
            try
            {
                return GetSuccessResponseStr();
            }
            catch (Exception ex)
            {
                logger.Error("Heart failed.", ex);
                return GetErrorResponseStr(ex.Message);
            }
        }

        /// <summary>
        /// 获取上传ftp服务端的配置
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static string GetRemoteIpConfig(Session session)
        {
            try
            {
                var ftpServiceConfig = ConfigUtil.ReadJsonConfigData<FtpServiceConfig>();
                if (ftpServiceConfig == null || ftpServiceConfig.RemoteIpConfigList == null || ftpServiceConfig.RemoteIpConfigList.Count == 0)
                {
                    logger.Error(@"Please configration FtpServiceConfig,location:'..\Configuration\FtpServiceConfig.json'");
                    return null;
                }
                return ConvertResponseStr(ftpServiceConfig);
            }
            catch (Exception ex)
            {
                logger.Error("GetRemoteIpConfig failed.", ex);
                return GetErrorResponseStr(ex.Message);
            }
        }

        /// <summary>
        /// 上传整个文件夹，如果目的端有此文件夹，先删除再上传
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static string UploadDirectory(Session session)
        {
            try
            {
                var reqObj = JsonConvert.DeserializeObject<UploadDirectoryRequest>(session.RequestBody);
                if (reqObj == null || reqObj.RemoteIpConfig == null)
                {
                    throw new Exception($"UploadDirectory: UploadDirectoryRequest is null, request:{session.RequestBody}.");
                }
                var task = TaskManager.CreateUploadDirectoryTask(session, reqObj);
                var result = FtpCore.UploadDirectory(reqObj.RemoteIpConfig, reqObj.SrcDirectory, reqObj.DestDirectory, reqObj.IsUpdateDirectory, reqObj.IsOverWriteFile);
                TaskManager.UpdateTask(task, result);
                var response = ResponseConverter.UploadDirectoryResponseConvert(result);
                return ConvertResponseStr(response);
            }
            catch (Exception ex)
            {
                logger.Error("UploadDirectory failed.", ex);
                return GetErrorResponseStr(ex.Message);
            }

        }

        public static string UploadFile(Session session)
        {
            try
            {
                var reqObj = JsonConvert.DeserializeObject<UploadFileRequest>(session.RequestBody);
                if (reqObj == null || reqObj.RemoteIpConfig == null)
                {
                    throw new Exception($"UploadFile: UploadFileRequest is null, request:{session.RequestBody}.");
                }
                var task = TaskManager.CreateUploadFileTask(session, reqObj);
                var result = FtpCore.UploadFile(reqObj.RemoteIpConfig, reqObj.SrcFile, reqObj.DestFile, reqObj.IsOverWrite);
                TaskManager.UpdateTask(task, result);
                var response = ResponseConverter.UploadFileResponseConvert(result);
                return ConvertResponseStr(response);
            }
            catch (Exception ex)
            {
                logger.Error("UploadFile failed.", ex);
                return GetErrorResponseStr(ex.Message);
            }
        }

        /// <summary>
        /// 上传整个文件夹，如果目的端有此文件夹，先删除再上传
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static string DownloadDirectory(Session session)
        {
            try
            {
                var reqObj = JsonConvert.DeserializeObject<DownloadDirectoryRequest>(session.RequestBody);
                if (reqObj == null || reqObj.RemoteIpConfig == null)
                {
                    throw new Exception($"DownloadDirectory: DownloadDirectoryRequest is null, request:{session.RequestBody}.");
                }
                var task = TaskManager.CreateDownloadDirectoryTask(session, reqObj);
                var result = FtpCore.DownloadDirectory(reqObj.RemoteIpConfig, reqObj.LocalDirectory, reqObj.RemoteDirectory);
                TaskManager.UpdateTask(task, result);
                var response = ResponseConverter.DownloadDirectoryResponseConvert(result);
                return ConvertResponseStr(response);
            }
            catch (Exception ex)
            {
                logger.Error("UploadDirectory failed.", ex);
                return GetErrorResponseStr(ex.Message);
            }

        }

        public static string DownloadFile(Session session)
        {
            try
            {
                var reqObj = JsonConvert.DeserializeObject<DownloadFileRequest>(session.RequestBody);
                if (reqObj == null || reqObj.RemoteIpConfig == null)
                {
                    throw new Exception($"DownloadFile: DownloadFileRequest is null, request:{session.RequestBody}.");
                }
                var task = TaskManager.CreateDownloadFileTask(session, reqObj);
                var result = FtpCore.DownloadFile(reqObj.RemoteIpConfig, reqObj.LocalFile, reqObj.RemoteFile);
                TaskManager.UpdateTask(task, result);
                var response = ResponseConverter.DownloadFileResponseConvert(result);
                return ConvertResponseStr(response);
            }
            catch (Exception ex)
            {
                logger.Error("DownloadFile failed.", ex);
                return GetErrorResponseStr(ex.Message);
            }
        }
        #endregion

        #region Private method

        private static string ConvertResponseStr<T>(T body)
        {
            var baseResponse = new BaseResponse<T>(body);
            return JsonConvert.SerializeObject(baseResponse);
        }

        private static string GetSuccessResponseStr()
        {
            var baseResponse = new BaseResponse<object>(null);
            baseResponse.StatusCode = (int)HttpStatusCode.Success;
            return JsonConvert.SerializeObject(baseResponse);
        }

        private static string GetErrorResponseStr(string errorMessage)
        {
            var baseResponse = new BaseResponse<object>(null);
            baseResponse.StatusCode = (int)HttpStatusCode.InnerError;
            baseResponse.Error = errorMessage;
            return JsonConvert.SerializeObject(baseResponse);
        }
        #endregion

    }
}
