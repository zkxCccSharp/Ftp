using SMEE.AOI.FTP.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.DatabaseSlim
{
    public class FtpTaskRepository
    {
        public List<FtpTask> GetAll()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.FtpTasks.ToList().OrderByDescending(P=>P.CreateTime).ToList();
            }
        }

        public bool Insert(FtpTask data)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Database.EnsureCreated();
                context.FtpTasks.Add(data);
                context.SaveChanges();
            }
            return true;
        }

        public FtpTask GetDataByTaskId(string taskId)
        {
            // 按条件查询数据
            using (var context = new ApplicationDbContext())
            {
                return context.FtpTasks.FirstOrDefault(p => p.Id == taskId);
            }
        }

        public bool Update(FtpTask ftpTask)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = context.FtpTasks.FirstOrDefault(e => e.Id == ftpTask.Id);
                if (entity != null)
                {
                    //更新信息
                    entity.State = ftpTask.State;
                    if (!string.IsNullOrEmpty(ftpTask.DoneTime))
                    {
                        entity.DoneTime = ftpTask.DoneTime;
                    }
                    // 将更改保存到数据库
                    context.SaveChanges();
                }
                return true;
            }
        }
    }
}
