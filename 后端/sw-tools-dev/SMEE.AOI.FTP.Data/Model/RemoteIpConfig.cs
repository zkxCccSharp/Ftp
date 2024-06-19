using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Data.Model
{
    public class RemoteIpConfig
    {
        /// <summary>
        /// 服务端标识
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// ip
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 操作系统 0：Windows 1：Linux
        /// </summary>
        public int SystemType { get; set; }
    }
}
