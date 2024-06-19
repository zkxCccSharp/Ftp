using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using SMEE.AOI.FTP.Data;

namespace SMEE.AOI.FTP.IPC
{
    public static class SessionManager
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SessionManager));

        public static void Enter(Session session)
        {
            logger.Info($"Session id:{session.Id}, state:[Start].");
        }

        public static void Out(Session session, double runTime, bool isSuccess)
        {
            string state = isSuccess ? "Done" : "Exception";
            logger.Info($"Session id:{session.Id}, run time:{runTime.ToString("f2")}, state:[{state}].");
        }
    }
}
