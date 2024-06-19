1.程序根目录下 Configuration\SmeeFtpConfig.ini 文件中，[Http]节点下配置下位机IP，[Ftp]节点下配置ServerMapDiskPath，值为上传ftp服务器映射本地磁盘的绝对路径
2.程序根目录下 Configuration\FtpServiceConfig.json 文件中，配置上传ftp服务器的名称、ip、用户名、密码，具体格式参照数据结构SMEE.AOI.FTP.Data.ConfigModel.FtpServiceConfig
3.需要将SMEE.AOI.FTP.Launcher.exe放在SMEE.AOI.FTP.Client.exe的目录中，才能使用SMEE.AOI.FTP.Launcher.exe启动SMEE.AOI.FTP.Client.exe