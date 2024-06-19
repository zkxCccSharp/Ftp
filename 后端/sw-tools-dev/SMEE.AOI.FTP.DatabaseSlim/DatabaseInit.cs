using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.DatabaseSlim
{
    public class DatabaseInit
    {
        public static void InitDatabase()
        {
            using (var context = new ApplicationDbContext())
            {
                context.Database.Migrate();
            }
        }
    }
}
