using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Service
{
    internal static class FileReturnRecordUtil
    {
        private const int waitReadTimeoutMS = 500;
        private const int maxRepeatCount = 5;

        private static readonly ILog logger =
            LogManager.GetLogger(typeof(FileReturnRecordUtil));

        public static bool TryGetRecord(string uid,
            out CopyTaskReturnParam returnParam)
        {
            try
            {
                for (int i = 0; ; i++)
                {
                    returnParam = GetRecordFromFile();
                    if (returnParam?.UID == uid)
                    {
                        return true;
                    }
                    if (i >= maxRepeatCount)
                    {
                        throw new Exception(
                            $"Get Record Repeat Count >= [{maxRepeatCount}]! " +
                            "Get Record Failed!");
                    }
                    Thread.Sleep(waitReadTimeoutMS);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error("Get Return Record Failed!");
                returnParam = new CopyTaskReturnParam();
                return false;
            }
        }

        private static CopyTaskReturnParam GetRecordFromFile()
        {
            try
            {
                var msg = FileUtil.ReadFileWithMutex(
                    UploadProcessInfo.PipeTaskReturnDataFile,
                    UploadProcessInfo.PipeTaskReturnMutexName);
                return JsonConvert.DeserializeObject<CopyTaskReturnParam>(msg);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error("Read Record File Failed!");
                return new CopyTaskReturnParam();
            }
        }

    }
}
