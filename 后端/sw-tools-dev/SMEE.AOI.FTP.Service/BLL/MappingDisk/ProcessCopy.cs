using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Service
{
    internal static class ProcessCopy
    {
        private const int waitConsoleTimeoutMS = 1000;

        private static readonly ILog logger =
            LogManager.GetLogger(typeof(ProcessCopy));

        public static void StartCopyProcessIfNotFound(
            Action<string> funcOfCreateUserProcess)
        {
            if (!IsUploadProcessExecuted())
            {
                logger.Warn("Test Upload Process NOT Existed! " +
                    "Create New Process.");
                ProcessUtil.StartNewProcess(funcOfCreateUserProcess);
                Thread.Sleep(waitConsoleTimeoutMS);
                if (!IsUploadProcessExecuted()) throw new Exception(
                    "Test UploadProcessExecuted Failed!");
            }
        }
        
        private static bool IsUploadProcessExecuted()
        {
            try
            {
                var checkParam = CreateCheckCopyTask();
                var pipeReturnRecord = new CopyTaskReturnParam();
                PipeUtil.SendMessage(
                    UploadProcessInfo.UploadExePipeServerPipeName,
                    JsonConvert.SerializeObject(checkParam), 100);
                Thread.Sleep(100);
                if (FileReturnRecordUtil.TryGetRecord(checkParam.UID,
                    out pipeReturnRecord))
                {
                    CopyTaskReturnParam.Validate(pipeReturnRecord);
                    return pipeReturnRecord.ParamType ==
                        ParamTypeEnum.OnlyCheckExisted;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        private static CopyTaskParam CreateCheckCopyTask()
        {
            return new CopyTaskParam()
            {
                UID = Guid.NewGuid().ToString(),
                SourceFullFilename = string.Empty,
                TargetFullFilename = string.Empty,
                MaxRepeatCount = 0,
                ParamType = ParamTypeEnum.OnlyCheckExisted,
            };
        }

        public static CopyTaskParam CreateNewCopyTask(
            FileInfo sourceFile, FileInfo targetFile, int maxRepeatCount)
        {
            MappingDiskUtil.ValidateObjectNotNull(sourceFile, "sourceFileInfo");
            MappingDiskUtil.ValidateObjectNotNull(targetFile, "targetFileInfo");
            return new CopyTaskParam()
            {
                UID = Guid.NewGuid().ToString(),
                SourceFullFilename = sourceFile.FullName,
                TargetFullFilename = targetFile.FullName,
                MaxRepeatCount = maxRepeatCount,
                ParamType = ParamTypeEnum.CopyTask,
            };
        }

        public static void SendCopyTask(
            CopyTaskParam copyTaskParam, int waitUploadMS)
        {
            MappingDiskUtil.ValidateObjectNotNull(
                copyTaskParam, nameof(copyTaskParam));
            var pipeMsg = JsonConvert.SerializeObject(copyTaskParam);
            PipeUtil.SendMessage(
                UploadProcessInfo.UploadExePipeServerPipeName,
                pipeMsg, waitUploadMS);
        }

        public static void WaitForCopyCompleted(
            string uid, int waitUploadMS)
        {
            var pipeReturnRecord = new CopyTaskReturnParam();
            var startTime = DateTime.Now;
            while (true)
            {
                if (FileReturnRecordUtil.TryGetRecord(uid, out pipeReturnRecord))
                {
                    CopyTaskReturnParam.Validate(pipeReturnRecord);
                    return;
                }
                if ((DateTime.Now - startTime).TotalMilliseconds >
                    waitUploadMS)
                {
                    throw new TimeoutException("Wait Copy Record Timeout > " +
                        $"[{waitUploadMS}] ms!");
                }
                Thread.Sleep(20);
            }
        }
        
    }

    public static class UploadProcessInfo
    {
        public const string UploadExeName = "SMEE.AOI.FTP.UploadConsole.exe";
        public const string UploadExePipeServerPipeName = "UploadConsolePipeServerPipe";
        public const string PipeTaskReturnDataFile = "UploadConsolePipeTaskReturn.json";
        public const string PipeTaskReturnMutexName = "UploadConsoleTaskReturnMutex";

    }

}
