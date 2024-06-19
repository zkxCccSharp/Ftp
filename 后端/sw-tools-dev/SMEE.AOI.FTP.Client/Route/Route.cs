using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using SMEE.AOI.FTP.Data;
using SMEE.AOI.FTP.Data.Database;

namespace SMEE.AOI.FTP.Client.HttpIPC
{
    public class Route
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Route));

        /// <summary>
        /// 手动路由
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static string RoutePath(string localPath, Session session)
        {
            logger.Info($"Into route, localPath:{localPath}, session id:{session.Id}.");
            string responseString = string.Empty;
            localPath = localPath.ToLower();
            switch (localPath)
            {
                case "/heart":
                    responseString = FtpController.Heart(session);
                    break;
                case "/remoteipconfig":
                    responseString = FtpController.GetRemoteIpConfig(session);
                    break;
                case "/uploaddirectory":
                    DatabaseService.Instance.InsertSessionCommand(CreateSessionCommand(session, localPath, "start"));
                    responseString = FtpController.UploadDirectory(session);
                    DatabaseService.Instance.UpdateSessionCommand(CreateSessionCommand(session, localPath, "done", DateTime.Now));
                    break;
                case "/uploadfile":
                    DatabaseService.Instance.InsertSessionCommand(CreateSessionCommand(session, localPath, "start"));
                    responseString = FtpController.UploadFile(session);
                    DatabaseService.Instance.UpdateSessionCommand(CreateSessionCommand(session, localPath, "done", DateTime.Now));
                    break;
                case "/downloaddirectory":
                    DatabaseService.Instance.InsertSessionCommand(CreateSessionCommand(session, localPath, "start"));
                    responseString = FtpController.DownloadDirectory(session);
                    DatabaseService.Instance.UpdateSessionCommand(CreateSessionCommand(session, localPath, "done", DateTime.Now));
                    break;
                case "/downloadfile":
                    DatabaseService.Instance.InsertSessionCommand(CreateSessionCommand(session, localPath, "start"));
                    responseString = FtpController.DownloadFile(session);
                    DatabaseService.Instance.UpdateSessionCommand(CreateSessionCommand(session, localPath, "done", DateTime.Now));
                    break;
                case "/alltask":
                    responseString = UIController.AllTask(session);
                    break;
                case "/task":
                    responseString = UIController.Task(session); //GetAllTaskFake();// UIController.Task(session);
                    break;
                case "/retrytask":
                    responseString = RetryTask(); //GetAllTaskFake();// UIController.Task(session);
                    break;
                case "basic-api/task":
                    responseString = UIController.Task(session);
                    break;
                default:
                    responseString = "Pleast choose the correct route.";
                    break;
            }
            return responseString ?? string.Empty;
        }

        static int count = 0;
        private static string RetryTask()
        {
            if (count > 100)
            {
                count = 0;
            }
            Random random = new Random();
            int i = random.Next(1, 20);
            return (count + i).ToString();
        }

        private static string GetTaskFake(string body)
        {
            List<task> tasks = new List<task>();
            for (int i = 0; i < 5; i++)
            {
                task task = new task();
                task.id = i.ToString();
                task.name = "zkx";
                task.age = "20";
                task.no = (100 + i).ToString();
                task.beginTime = DateTime.Now;
                task.endTime = DateTime.Now;
                task.address = body;
                tasks.Add(task);
            }
            return JsonConvert.SerializeObject(tasks);
        }

        private static string GetAllTaskFake()
        {
            List<FtpTask> tasks = new List<FtpTask>();
            for (int i = 0; i < 5; i++)
            {
                FtpTask task = new FtpTask();
                task.Id = i.ToString();
                task.RemoteIpPort = (100 + i).ToString();
                task.CreateTime = DateTime.Now.ToString();
                task.DoneTime = DateTime.Now.ToString();
                task.OperParam = "rode1";
                tasks.Add(task);
            }
            return JsonConvert.SerializeObject(tasks);
        }
        class task
        {
            public string id { get; set; }
            public string name { get; set; }
            public string age { get; set; }
            public string no { get; set; }
            public string address { get; set; }
            public string addressaa { get; set; }
            public DateTime beginTime { get; set; }
            public DateTime endTime { get; set; }
        }

        private static SessionCommand CreateSessionCommand(Session session, string localPath, string state, DateTime? dateTime = null)
        {
            return new SessionCommand()
            {
                SessionId = session.Id,
                CreateTime = DateTime.Now,
                StartTime = DateTime.Now,
                OperType = localPath,
                OperParam = session.RequestBody,
                State = state,
                DoneTime = dateTime
            };
        }
    }
}
