using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMEE.AOI.FTP.Data.Config;

namespace SMEE.AOI.FTP.Data.ConfigModel
{
    /// <summary>
    /// 配置文件的静态初始文件，更新数据后必须更新版本号
    /// </summary>
    [FileData("SmeeFtpConfig.ini", @"Configuration\")]
    public class IniConfigData
    {
        public struct Header
        {
            [Description("版本号 更新数据后必须更新")]
            public const string VersionNo = "1.0.1";

            [Description("修改项不覆盖 0；修改项覆盖 1")]
            public const string UpdateStrategy = "0";
        }

        public struct Common
        {
            [Description("重试等待类型 0：固定 1：递增 2：随机")]
            public const string RetryWaitType = "1";

            [Description("重试次数")]
            public const string RetryTimes = "3";

            [Description("重试初次等待时间")]
            public const string RetryWaitTime = "3000";
        }

        public struct Http
        {
            [Description("SMEE下位机IP")]
            public const string IP = "172.0.0.1";
        }

        public struct Ftp
        {
            [Description("FTP服务端异常时是否开启本地映射盘拷贝，0：不开启 1：开启")]
            public const string IsMapDiskCopy = "0";

            [Description("FTP服务端本地映射盘路径")]
            public const string ServerMapDiskPath = "";

            [Description("FTP客户端DataConnectionType")]
            public const string DataConnectionType = "0";

            [Description("FTP客户端ConnectTimeout")]
            public const string ConnectTimeout = "15000";

            [Description("FTP客户端ReadTimeout")]
            public const string ReadTimeout = "600000";

            [Description("FTP客户端DataConnectionConnectTimeout")]
            public const string DataConnectionConnectTimeout = "15000";

            [Description("FTP客户端DataConnectionReadTimeout")]
            public const string DataConnectionReadTimeout = "600000";
            
        }
    }
}
