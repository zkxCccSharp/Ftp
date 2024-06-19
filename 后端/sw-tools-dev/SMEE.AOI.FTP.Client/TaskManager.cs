using log4net;
using SMEE.AOI.FTP.Data;
using SMEE.AOI.FTP.Data.Common;
using SMEE.AOI.FTP.Data.Database;
using SMEE.AOI.FTP.Data.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMEE.AOI.FTP.Client
{
    internal class TaskManager
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(TaskManager));

        public static int taskNumber = 0;

        private static string CreateTaskId()
        {
            return DateTime.Now.ToString("yyyyMMdd_HHmmss") + "-" + taskNumber++.ToString();
        }

        public static FtpTask CreateUploadDirectoryTask(Session session, UploadDirectoryRequest reqObj)
        {
            FtpTask task = new FtpTask()
            {
                Id = CreateTaskId(),
                State = TaskState.Start,
                OperType = OperType.UploadDirectory,
                CreateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                StartTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                AncestorTaskId = reqObj.AncestorTaskId,
                SessionId = session.Id,
                RemoteIpPort = $"{reqObj.RemoteIpConfig.Host}:{reqObj.RemoteIpConfig.Port}",
                OperParam = $"Src:{reqObj.SrcDirectory}, Dest:{reqObj.DestDirectory}, IsUpdate:{reqObj.IsUpdateDirectory}, IsOverWrite:{reqObj.IsOverWriteFile}"
            };
            InsertTask(task);
            return task;
        }

        public static FtpTask CreateUploadFileTask(Session session, UploadFileRequest reqObj)
        {
            FtpTask task = new FtpTask()
            {
                Id = CreateTaskId(),
                State = TaskState.Start,
                OperType = OperType.UploadFile,
                CreateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                StartTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                AncestorTaskId = reqObj.AncestorTaskId,
                SessionId = session.Id,
                RemoteIpPort = $"{reqObj.RemoteIpConfig.Host}:{reqObj.RemoteIpConfig.Port}",
                OperParam = $"Src:{reqObj.SrcFile}, Dest:{reqObj.DestFile}, IsOverWrite:{reqObj.IsOverWrite}"
            };
            InsertTask(task);
            return task;
        }

        public static FtpTask CreateDownloadDirectoryTask(Session session, DownloadDirectoryRequest reqObj)
        {
            FtpTask task = new FtpTask()
            {
                Id = CreateTaskId(),
                State = TaskState.Start,
                OperType = OperType.DownloadDirectory,
                CreateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                StartTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                AncestorTaskId = reqObj.AncestorTaskId,
                SessionId = session.Id,
                RemoteIpPort = $"{reqObj.RemoteIpConfig.Host}:{reqObj.RemoteIpConfig.Port}",
                OperParam = $"Local:{reqObj.LocalDirectory}, Remote:{reqObj.RemoteIpConfig}"
            };
            InsertTask(task);
            return task;
        }

        public static FtpTask CreateDownloadFileTask(Session session, DownloadFileRequest reqObj)
        {
            FtpTask task = new FtpTask()
            {
                Id = CreateTaskId(),
                State = TaskState.Start,
                OperType = OperType.DownloadFile,
                CreateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                StartTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                AncestorTaskId = reqObj.AncestorTaskId,
                SessionId = session.Id,
                RemoteIpPort = $"{reqObj.RemoteIpConfig.Host}:{reqObj.RemoteIpConfig.Port}",
                OperParam = $"Local:{reqObj.LocalFile}, Remote:{reqObj.RemoteFile}"
            };
            InsertTask(task);
            return task;
        }

        public static bool InsertTask(FtpTask task)
        {
            try
            {
                return DatabaseService.Instance.InsertFtpTask(task);
            }
            catch (Exception ex)
            {
                logger.Error($"InsertFtpTask failed, taskId:{task.Id}, sessionId:{task.SessionId}.", ex);
                return false;
            }
        }

        public static bool UpdateTask(FtpTask task, bool idSuccess)
        {
            task.DoneTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            task.State = idSuccess ? TaskState.Done : TaskState.Exception;

            try
            {
                return DatabaseService.Instance.UpdateFtpTask(task);
            }
            catch (Exception ex)
            {
                logger.Error($"UpdateTask: taskId:{task.Id}, sessionId:{task.SessionId}.", ex);
                return false;
            }
        }
    }
}
