using log4net;
using Newtonsoft.Json;
using SMEE.AOI.FTP.Client.Converter;
using SMEE.AOI.FTP.Data;
using SMEE.AOI.FTP.Data.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Client
{
    /// <summary>
    /// Controller
    /// </summary>
    public class UIController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(UIController));

        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static string AllTask(Session session)
        {
            try
            {
                var respose = DatabaseService.Instance.GetAllFtpTasks();
                return JsonConvert.SerializeObject(respose);
            }
            catch (Exception ex)
            {
                logger.Error("AllTask failed.", ex);
                return GetErrorResponseStr(ex.Message);
            }
        }

        public static string Task(Session session)
        {
            try
            {
                var respose = DatabaseService.Instance.GetAllFtpTasks();
                //var respose = DatabaseService.Instance.GetAllSessionCommands();
                return JsonConvert.SerializeObject(respose);
            }
            catch (Exception ex)
            {
                logger.Error("Task failed.", ex);
                return GetErrorResponseStr(ex.Message);
            }
        }


        #region Private method

        private static string ConvertResponseStr<T>(T body)
        {
            var baseResponse = new BaseResponse<T>(body);
            return JsonConvert.SerializeObject(baseResponse);
        }

        private static string GetSuccessResponseStr()
        {
            var baseResponse = new BaseResponse<object>(null);
            baseResponse.StatusCode = (int)HttpStatusCode.Success;
            return JsonConvert.SerializeObject(baseResponse);
        }

        private static string GetErrorResponseStr(string errorMessage)
        {
            var baseResponse = new BaseResponse<object>(null);
            baseResponse.StatusCode = (int)HttpStatusCode.InnerError;
            baseResponse.Error = errorMessage;
            return JsonConvert.SerializeObject(baseResponse);
        }
        #endregion
    }
}
