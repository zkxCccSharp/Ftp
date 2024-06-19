using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SMEE.AOI.FTP.Data.Common;
using SMEE.AOI.FTP.Data.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SMEE.AOI.FTP.DatabaseSlim
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<SessionCommand> SessionCommands { get; set; }
        public DbSet<FtpTask> FtpTasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=DatabaseSQLite.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region SessionCommand

            modelBuilder.Entity<SessionCommand>()
                .HasKey(sc => sc.SessionId); 

            modelBuilder.Entity<SessionCommand>()
               .Property(sc => sc.AncestorSessionId)
               .IsRequired(false);
            modelBuilder.Entity<SessionCommand>()
               .Property(sc => sc.State)
               .IsRequired();
            modelBuilder.Entity<SessionCommand>()
               .Property(sc => sc.CreateTime)
               .HasColumnType("datetime2(0)")
               .IsRequired(false);
            modelBuilder.Entity<SessionCommand>()
               .Property(sc => sc.StartTime)
               .HasColumnType("datetime2(0)")
               .IsRequired(false);
            modelBuilder.Entity<SessionCommand>()
               .Property(sc => sc.DoneTime)
               .HasColumnType("datetime2(0)")
               .IsRequired(false);
            modelBuilder.Entity<SessionCommand>()
               .Property(sc => sc.OperType)
               .IsRequired();
            modelBuilder.Entity<SessionCommand>()
               .Property(sc => sc.OperParam)
               .IsRequired(false);
            modelBuilder.Entity<SessionCommand>()
               .Property(sc => sc.ErrorMsg)
               .IsRequired(false);

            #endregion

            #region FtpTask

            modelBuilder.Entity<FtpTask>()
                .HasKey(sc => sc.Id);

            modelBuilder.Entity<FtpTask>()
               .Property(sc => sc.AncestorTaskId)
               .IsRequired(false);
            modelBuilder.Entity<FtpTask>()
               .Property(sc => sc.State)
               .HasConversion(EnumToStringConverter<TaskState>());
            modelBuilder.Entity<FtpTask>()
               .Property(sc => sc.CreateTime)
               .IsRequired(false);
            modelBuilder.Entity<FtpTask>()
               .Property(sc => sc.StartTime)
               .IsRequired(false);
            modelBuilder.Entity<FtpTask>()
               .Property(sc => sc.DoneTime)
               .IsRequired(false);
            modelBuilder.Entity<FtpTask>()
               .Property(sc => sc.OperType)
                .HasConversion(EnumToStringConverter<OperType>());
            modelBuilder.Entity<FtpTask>()
               .Property(sc => sc.RemoteIpPort)
               .IsRequired(false);
            modelBuilder.Entity<FtpTask>()
               .Property(sc => sc.OperParam)
               .IsRequired(false);
            modelBuilder.Entity<FtpTask>()
               .Property(sc => sc.ErrorMsg)
               .IsRequired(false);
            modelBuilder.Entity<FtpTask>()
               .Property(sc => sc.SessionId)
               .IsRequired(false);

            #endregion
        }

        private ValueConverter<TEnum, string> EnumToStringConverter<TEnum>() where TEnum : struct, Enum
        {
            return new ValueConverter<TEnum, string>(
                v => v.ToString(),
                v => (TEnum)Enum.Parse(typeof(TEnum), v));
        }
    }
}
