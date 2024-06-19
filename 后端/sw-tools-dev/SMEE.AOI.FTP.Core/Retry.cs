using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using SMEE.AOI.FTP.Data.Config;
using SMEE.AOI.FTP.Data.ExceptionType;

namespace SMEE.AOI.FTP.Core
{
    internal class Retry
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Retry));

        private static int randomMinWaitTime = 0;
        private static int randomMaxWaitTime = 10;

        private static RetryWaitType retryWaitType = RetryWaitType.Increase;
        private static int retryTimes = 3;
        private static int retryWaitTime = 3 * 1000;

        public Retry()
        {
            try
            {
                GetRetryConfig();
            }
            catch (Exception ex)
            {
                throw new Exception("Retry ", ex);
            }
        }

        /// <summary>
        /// 对有返回值程序体添加重试机制和异常处理机制，如果重试后仍然失败执行绑定的异常处理程序
        /// </summary>
        /// <typeparam name="T">程序体返回类型</typeparam>
        /// <param name="func">程序体</param>
        /// <param name="funcName">程序名</param>
        public static bool Start(Func<bool> func, string funcName)
        {
            return Start(func, funcName, retryTimes, retryWaitType, retryWaitTime);
        }

        /// <summary>
        /// 对有返回值程序体添加重试机制和异常处理机制，如果重试后仍然失败执行绑定的异常处理程序
        /// </summary>
        /// <typeparam name="T">程序体返回类型</typeparam>
        /// <param name="func">程序体</param>
        /// <param name="funcName">程序名</param>
        /// <param name="times">程序体重试次数</param>
        /// <param name="waitType">重试等待策略</param>
        /// <param name="retryWaitTime">重试等待时间</param>
        public static bool Start(Func<bool> func, string funcName, int times, RetryWaitType waitType, int retryWaitTime)
        {
            int currTime = 1;
            while (currTime <= times)
            {
                try
                {
                    if (func.Invoke())
                    {
                        logger.Info($"Retry: [{funcName}] execute {currTime} time success.");
                        return true;
                    }
                    else
                    {
                        throw new Exception($"[{funcName}] result is false.");
                    }
                }
                catch (RetryTerminationException ex)
                {
                    logger.Error($"Retry: [{funcName}] execute {currTime} time termination, reason:{ex.Reason}.", ex);
                    return false;
                }
                catch (Exception ex)
                {
                    logger.Error($"Retry: [{funcName}] execute {currTime} time exception.", ex);
                    currTime++;
                    if (currTime <= times)
                    {
                        Thread.Sleep(retryWaitTime);
                        retryWaitTime = GetWaitTime(waitType, retryWaitTime);
                    }
                }
            }
            return false;
        }

        private static int GetWaitTime(RetryWaitType waitType, int waitTime)
        {
            switch (waitType)
            {
                case RetryWaitType.Fixed:
                    break;
                case RetryWaitType.Random:
                    waitTime = new Random().Next(randomMinWaitTime, randomMaxWaitTime);
                    break;
                case RetryWaitType.Increase:
                    waitTime *= 2;
                    break;
                default:
                    break;
            }
            return waitTime;
        }

        private static void GetRetryConfig()
        {
            try
            {
                retryWaitType = (RetryWaitType)Convert.ToInt32(IniHelper.ReadValueFromIni("Common", "RetryWaitType", "1"));
            }
            catch
            {
                logger.Error($"SmeeFtpConfig.ini [Common] [RetryWaitType] must is number. retryWaitType:{retryWaitType}.");
            }
            try
            {
                retryTimes = Convert.ToInt32(IniHelper.ReadValueFromIni("Common", "RetryTimes", "3"));
            }
            catch
            {
                logger.Error($"SmeeFtpConfig.ini [Common] [RetryTimes] must is number. retryTimes:{retryTimes}.");
            }
            try
            {
                retryWaitTime = Convert.ToInt32(IniHelper.ReadValueFromIni("Common", "RetryWaitTime", "3000"));
            }
            catch
            {
                logger.Error($"SmeeFtpConfig.ini [Common] [RetryWaitTime] must is number. retryWaitTime:{retryWaitTime}.");
            }
        }
    }

    public enum RetryWaitType
    {
        /// <summary>
        /// 固定等待时间
        /// </summary>
        Fixed = 0,
        /// <summary>
        /// 递增等待时间，每次等待时间增长一倍
        /// </summary>
        Increase = 1,
        /// <summary>
        /// 随机等待时间
        /// </summary>
        Random = 2
    }
}
