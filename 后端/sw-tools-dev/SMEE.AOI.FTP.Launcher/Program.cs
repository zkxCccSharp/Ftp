using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Microsoft.Win32;

namespace SMEE.AOI.FTP.Launcher
{
    class Program
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Program));
        
        private const string ftpClientName = "SMEE.AOI.FTP.Client.exe";
        private const int time = 3 * 1000;
        private static int pid = -1;

        static void Main(string[] args)
        {
            //Init log4net
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4netLauncher.config"));

            logger.Info("Main: Start.");

            //隐藏窗体
            Win32.FreeConsole();
            logger.Info("Main: Main Form is hidden.");

            string currAppDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string processPath = Path.Combine(currAppDirectory, ftpClientName);
            logger.Info($"Main: FtpClient path:{processPath}.");

            //设为开机自启动(当前用户)
            string currProcessName = Process.GetCurrentProcess().ProcessName;
            string currPrcessFullPath = Process.GetCurrentProcess().MainModule.FileName;
            SetPowerOnAutoRunByUser(currProcessName, currPrcessFullPath);
            
            FtpClientRun(processPath);
            
            logger.Info("Main: Exit.");
        }

        private static void FtpClientRun(string processPath)
        {
            try
            {
                while (true)
                {
                    if (!IsProcessRunning(pid))
                    {
                        logger.Warn($"FtpClientRun: FtpClient not running.");
                        var process = Process.Start(processPath);
                        pid = process.Id;
                        logger.Info($"FtpClientRun: FtpClient is running, pid:{pid}, path:{processPath}");
                    }
                    Thread.Sleep(time);
                }
            }
            catch (Exception ex)
            {
                logger.Info($"FtpClientRun failed." ,ex);
            }
        }

        private static bool IsProcessRunning(int pid)
        {
            Process[] processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                if (process.Id == pid)
                {
                    return true;
                }
            }
            return false;
        }

        private static void SetPowerOnAutoRunByUser(string processName, string processFullName)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            registryKey.SetValue(processName, processFullName);
        }

        private static void SetPowerOnAutoRunByAdmnin(string processName, string processFullName)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            registryKey.SetValue(processName, processFullName);
        }
    }
}
