using log4net;
using Newtonsoft.Json;
using SMEE.AOI.FTP.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.UploadConsole
{
    internal static class CopyTask
    {
        private static readonly ILog logger =
            LogManager.GetLogger(typeof(CopyTask));
        
        public static void ExecuteWithTaskParam(
            CopyTaskParam copyTaskParam)
        {
            try
            {
                MappingDiskUtil.ValidateObjectNotNull(
                    copyTaskParam, nameof(copyTaskParam));
                ExecuteWithTask(copyTaskParam, GetReturnParam);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error("Execute With Copy Task Failed!");
                throw ex;
            }
        }

        private static void ExecuteWithTask(CopyTaskParam copyTaskParam,
            Func<CopyTaskParam, CopyTaskReturnParam> funcOfGetReturnParam)
        {
            try
            {
                MappingDiskUtil.ValidateObjectNotNull(
                    copyTaskParam, nameof(copyTaskParam));
                MappingDiskUtil.ValidateObjectNotNull(
                    funcOfGetReturnParam, nameof(funcOfGetReturnParam));
                var returnParam = funcOfGetReturnParam.Invoke(copyTaskParam);
                MappingDiskUtil.ValidateObjectNotNull(
                    returnParam, nameof(returnParam));
                var sendMsg = JsonConvert.SerializeObject(returnParam);
                FileUtil.WriteFileWithMutex(
                    UploadProcessInfo.PipeTaskReturnDataFile,
                    UploadProcessInfo.PipeTaskReturnMutexName,
                    sendMsg);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error("ExecuteWithTask Failed!");
                throw ex;
            }
        }

        private static CopyTaskReturnParam GetReturnParam(
            CopyTaskParam taskParam)
        {
            try
            {
                MappingDiskUtil.ValidateObjectNotNull(
                    taskParam, nameof(taskParam));
                switch (taskParam.ParamType)
                {
                    case ParamTypeEnum.OnlyCheckExisted:
                        return GetCheckReturnParam(taskParam);
                    case ParamTypeEnum.CopyTask:
                        return GetCopyReturnParam(taskParam);
                    default:
                        throw new Exception($"Task Type Invalid! " +
                            $"[{taskParam.ParamType}]");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error("GetReturnParam Failed!");
                throw ex;
            }
        }

        private static CopyTaskReturnParam GetCheckReturnParam(
            CopyTaskParam taskParam)
        {
            MappingDiskUtil.ValidateObjectNotNull(
                taskParam, nameof(taskParam));
            return new CopyTaskReturnParam()
            {
                UID = taskParam.UID,
                ParamType = ParamTypeEnum.OnlyCheckExisted,
                IsSuccessful = true,
                ExceptionMessage = string.Empty,
            };
        }

        private static CopyTaskReturnParam GetCopyReturnParam(
            CopyTaskParam taskParam)
        {
            MappingDiskUtil.ValidateObjectNotNull(
                taskParam, nameof(taskParam));
            try
            {
                CyclingCopy(taskParam);
                return new CopyTaskReturnParam()
                {
                    UID = taskParam.UID,
                    ParamType = ParamTypeEnum.CopyTask,
                    IsSuccessful = true,
                    ExceptionMessage = string.Empty,
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error("Copy Failed!");
                return new CopyTaskReturnParam()
                {
                    UID = taskParam.UID,
                    ParamType = ParamTypeEnum.CopyTask,
                    IsSuccessful = false,
                    ExceptionMessage = ex.Message,
                };
            }
        }

        private static void CyclingCopy(CopyTaskParam taskParam)
        {
            MappingDiskUtil.ValidateObjectNotNull(
                taskParam, nameof(taskParam));
            MappingDiskBusiness.CopyCycling(
                new FileInfo(taskParam.SourceFullFilename),
                new FileInfo(taskParam.TargetFullFilename),
                taskParam.MaxRepeatCount);
        }

    }
}
