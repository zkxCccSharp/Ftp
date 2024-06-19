using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.UploadConsole
{
    class Program
    {
        private static readonly ILog logger =
            LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            LogStartInfo();
            ConsoleUtil.HideConsole();
            var programMutexName = GetMutexName();
            bool isCreatedNew = false;
            using (var mutex = new Mutex(true,
                programMutexName, out isCreatedNew))
            {
                if (isCreatedNew)
                {
                    RealMain(args);
                }
                else
                {
                    logger.Warn("UploadConsole Program " +
                        "has been Executed!");
                }
            }
        }

        private static void LogStartInfo()
        {
            var info = new StringBuilder(string.Empty);
            info.AppendLine();
            info.AppendLine("------------------------------");
            info.AppendLine($"Start At : {DateTime.Now:yyyyMMdd HH:mm:ss}");
            info.AppendLine("------------------------------");
            logger.Info(info.ToString());
        }

        private static string GetMutexName()
        {
            return typeof(Program).FullName.Replace('.', '_');
        }

        private static void RealMain(string[] args)
        {
            try
            {
                ListenUtil.CyclingListen();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error("Upload Console has Error! Exit.");
                throw ex;
            }
        }

    }
}
