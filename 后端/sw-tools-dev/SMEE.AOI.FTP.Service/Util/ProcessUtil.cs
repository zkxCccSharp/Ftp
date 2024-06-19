using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Service
{
    internal static class ProcessUtil
    {
        private static readonly ILog logger =
            LogManager.GetLogger(typeof(ProcessUtil));

        public static void StartNewProcess(
            Action<string> funcOfCreateUserProcess)
        {
            try
            {
                MappingDiskUtil.ValidateObjectNotNull(
                    funcOfCreateUserProcess,
                    nameof(funcOfCreateUserProcess));
                var consoleExeFileName = GetConsoleExeFileName();
                funcOfCreateUserProcess.Invoke(consoleExeFileName);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error("StartNewProcess Failed!");
                throw ex;
            }
        }

        private static string GetConsoleExeFileName()
        {
            try
            {
                var res = UploadProcessInfo.UploadExeName;
                if (!File.Exists(res)) throw new IOException(
                    "Can NOT Find Console EXE File!");
                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error("GetConsoleExeFullName Failed!");
                throw ex;
            }
        }

    }
}
