using log4net;
using Newtonsoft.Json;
using SMEE.AOI.FTP.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.UploadConsole
{
    internal static class ListenUtil
    {
        private static readonly ILog logger =
            LogManager.GetLogger(typeof(ListenUtil));

        public static void CyclingListen()
        {
            while (true)
            {
                try
                {
                    var pipeMsg = PipeUtil.ReceiveMessage(
                        UploadProcessInfo.UploadExePipeServerPipeName);
                    Task.Factory.StartNew(() =>
                        ExecuteWithPipeMessage(pipeMsg));
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    logger.Error("Get Pipe Record Failed!");
                }
            }
        }
    
        private static void ExecuteWithPipeMessage(string pipeMsg)
        {
            try
            {
                var param = JsonConvert.DeserializeObject
                    <CopyTaskParam>(pipeMsg);
                ValidateParam(param);
                CopyTask.ExecuteWithTaskParam(param);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error("Execute With Pipe Msg Failed!");
                throw ex;
            }
        }

        private static void ValidateParam(CopyTaskParam copyTaskParam)
        {
            MappingDiskUtil.ValidateObjectNotNull(
                copyTaskParam, nameof(copyTaskParam));
            if (string.IsNullOrWhiteSpace(copyTaskParam.UID))
            {
                throw new Exception("Task UID Invalid!");
            }
            switch (copyTaskParam.ParamType)
            {
                case ParamTypeEnum.OnlyCheckExisted:
                case ParamTypeEnum.CopyTask:
                    break;
                default:
                    throw new Exception($"Task Type Invalid! " +
                        $"[{copyTaskParam.ParamType}]");
            }
        }

    }
}
