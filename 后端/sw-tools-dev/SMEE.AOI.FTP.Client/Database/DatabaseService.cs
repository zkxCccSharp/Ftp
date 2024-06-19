using log4net;
using SMEE.AOI.FTP.Data.Database;
using SMEE.AOI.FTP.DatabaseSlim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Client
{
    public class DatabaseService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DatabaseService));

        public static DatabaseService Instance = new DatabaseService();

        private SessionRepository sessionRepositoryService = null;
        public SessionRepository SessionRepositoryService
        {
            get
            {
                if (sessionRepositoryService == null)
                {
                    sessionRepositoryService = new SessionRepository();
                }
                return sessionRepositoryService;
            }
        }

        private FtpTaskRepository ftpTaskRepositoryService = null;
        public FtpTaskRepository FtpTaskRepositoryService
        {
            get
            {
                if (ftpTaskRepositoryService == null)
                {
                    ftpTaskRepositoryService = new FtpTaskRepository();
                }
                return ftpTaskRepositoryService;
            }
        }

        #region SessionCommand
        public bool InsertSessionCommand(SessionCommand sessionCommand)
        {
            return SessionRepositoryService.Insert(sessionCommand);
        }

        public bool UpdateSessionCommand(SessionCommand sessionCommand)
        {
            return SessionRepositoryService.Update(sessionCommand);
        }

        public List<SessionCommand> GetAllSessionCommands()
        {
            return SessionRepositoryService.GetAll();
        }

        public SessionCommand GetSessionCommandById(string sessionId)
        {
            return SessionRepositoryService.GetDataBySessionId(sessionId);
        }
        #endregion

        #region FtpTask
        public bool InsertFtpTask(FtpTask task)
        {
            return FtpTaskRepositoryService.Insert(task);
        }

        public bool UpdateFtpTask(FtpTask task)
        {
            return FtpTaskRepositoryService.Update(task);
        }

        public List<FtpTask> GetAllFtpTasks()
        {
            return FtpTaskRepositoryService.GetAll();
        }

        public FtpTask GetFtpTaskById(string taskId)
        {
            return FtpTaskRepositoryService.GetDataByTaskId(taskId);
        }
        #endregion
    }
}
