using log4net;
using SMEE.AOI.FTP.DatabaseSlim;
using SMEE.AOI.FTP.IPC;
using SQLitePCL;
using System.Diagnostics;
using System.IO;

namespace SMEE.AOI.FTP.Client
{
    class Program
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            //Init log4net
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));

            logger.Info("Main: Program has Strated.");

            //Init database
            Batteries.Init();
            DatabaseInit.InitDatabase();

            //Hide Window
            Win32.FreeConsole();
            logger.Info("Main: Main Form is hidden.");

            //IPC Service Run
            HttpService.Run();

            logger.Info("Main: Program has ended.");
        }

        private static bool CurrProcessRunning()
        {
            string currProcessName = Process.GetCurrentProcess().ProcessName;
            return Helper.IsProcessRunning(currProcessName);
        }
    }
}
