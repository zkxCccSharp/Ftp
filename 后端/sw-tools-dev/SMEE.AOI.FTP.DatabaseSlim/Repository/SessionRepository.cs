using SMEE.AOI.FTP.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.DatabaseSlim
{
    public class SessionRepository
    {
        public List<SessionCommand> GetAll()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.SessionCommands.ToList().OrderByDescending(P => P.CreateTime).ToList(); ;
            }
        }

        public bool Insert(SessionCommand data)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Database.EnsureCreated();
                context.SessionCommands.Add(data);
                context.SaveChanges();
            }
            return true;
        }

        public SessionCommand GetDataBySessionId(string SessionId)
        {
            // 按条件查询数据
            using (var context = new ApplicationDbContext())
            {
                return context.SessionCommands.FirstOrDefault(p => p.SessionId == SessionId);
            }
        }

        public bool Update(SessionCommand sessionCommand)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = context.SessionCommands.FirstOrDefault(e => e.SessionId == sessionCommand.SessionId);
                if (entity != null)
                {
                    // 更新实体的属性
                    if (!string.IsNullOrEmpty(sessionCommand.State))
                    {
                        entity.State = sessionCommand.State;
                    }
                    if (sessionCommand.DoneTime != null)
                    {
                        entity.DoneTime = sessionCommand.DoneTime;
                    }
                    if (!string.IsNullOrEmpty(sessionCommand.ErrorMsg))
                    {
                        entity.ErrorMsg = sessionCommand.ErrorMsg;
                    }

                    // 将更改保存到数据库
                    context.SaveChanges();
                }
                return true;
            }
        }
    }
}
