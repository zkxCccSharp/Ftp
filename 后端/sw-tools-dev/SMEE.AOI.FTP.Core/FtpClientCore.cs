using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using log4net;
using SMEE.AOI.FTP.Data.Config;
using SMEE.AOI.FTP.Data.ConfigModel;
using SMEE.AOI.FTP.Data.Model;

namespace SMEE.AOI.FTP.Core
{
    /// <summary>
    /// 执行单个连接的文件操作
    /// </summary>
    internal class FtpClientCore 
    {

        private static readonly ILog logger = LogManager.GetLogger(typeof(FtpClientCore));

        private RemoteIpConfig remote;
        private FtpClientConfig clientConfig;
        //private FluentLogger fluentLogger;
        
        public FtpClientCore(RemoteIpConfig remoteIpConfig)
        {
            try
            {
                remote = remoteIpConfig;
                clientConfig = GetFtpClientConfig();
                //fluentLogger = new FluentLogger("FluentFtp", "log4netFluent.config", true);
                //StartTaskDeal();
            }
            catch (Exception ex)
            {
                throw new Exception("FtpClientCore ", ex);
            }
        }

        public FtpClient CreateInitFtpClient()
        {
            var client = new FtpClient(remote.Host, remote.Port, remote.Username, remote.Password);
            client.DataConnectionType = (FtpDataConnectionType)clientConfig.DataConnectionType;
            client.ConnectTimeout = clientConfig.ConnectTimeout;
            client.ReadTimeout = clientConfig.ReadTimeout;
            client.DataConnectionConnectTimeout = clientConfig.DataConnectionConnectTimeout;
            client.DataConnectionReadTimeout = clientConfig.DataConnectionReadTimeout;

            return client;
        }

        private FtpClientConfig GetFtpClientConfig()
        {
            var clientConfig = new FtpClientConfig();

            #region DataConnectionType
            int connectionType = 0;
            try
            {
                connectionType = Convert.ToInt32(IniHelper.ReadValueFromIni("Ftp", "FtpDataConnectionType", "0"));
                clientConfig.DataConnectionType = connectionType;
            }
            catch
            {
                logger.Error($"SmeeFtpConfig.ini [Ftp] [DataConnectionType] must is number. connectTimeout:{connectionType}.");
            }
            #endregion

            #region ConnectTimeout
            int connectTimeout = 0;
            try
            {
                connectTimeout = Convert.ToInt32(IniHelper.ReadValueFromIni("Ftp", "ConnectTimeout", "15000"));
                clientConfig.ConnectTimeout = connectTimeout;
            }
            catch
            {
                logger.Error($"SmeeFtpConfig.ini [Ftp] [ConnectTimeout] must is number. connectTimeout:{connectTimeout}.");
            }
            #endregion

            #region ReadTimeout
            int readTimeout = 0;
            try
            {
                readTimeout = Convert.ToInt32(IniHelper.ReadValueFromIni("Ftp", "ReadTimeout", "15000"));
                clientConfig.ReadTimeout = readTimeout;
            }
            catch 
            {
                logger.Error($"SmeeFtpConfig.ini [Ftp] [ReadTimeout] must is number. readTimeout:{readTimeout}.");
            }
            #endregion

            #region DataConnectionConnectTimeout
            int dataConnectionConnectTimeout = 0;
            try
            {
                dataConnectionConnectTimeout = Convert.ToInt32(IniHelper.ReadValueFromIni("Ftp", "DataConnectionConnectTimeout", "15000"));
                clientConfig.DataConnectionConnectTimeout = dataConnectionConnectTimeout;
            }
            catch
            {
                logger.Error($"SmeeFtpConfig.ini [Ftp] [DataConnectionConnectTimeout] must is number. dataConnectionConnectTimeout:{dataConnectionConnectTimeout}.");
            }
            #endregion

            #region DataConnectionReadTimeout
            int dataConnectionReadTimeout = 0;
            try
            {
                dataConnectionReadTimeout = Convert.ToInt32(IniHelper.ReadValueFromIni("Ftp", "DataConnectionReadTimeout", "15000"));
                clientConfig.DataConnectionReadTimeout = dataConnectionReadTimeout;
            }
            catch
            {
                logger.Error($"SmeeFtpConfig.ini [Ftp] [DataConnectionReadTimeout] must is number. dataConnectionReadTimeout:{dataConnectionReadTimeout}.");
            }
            #endregion

            return clientConfig;
        }
    }
}
