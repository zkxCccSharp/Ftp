using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Service
{
    public static class FileUtil
    {
        private const int waitIOTimeoutMS = 100;
        private const int maxRepeatCount = 5;

        public static string ReadFileWithMutex(
            string filename, string mutexName)
        {
            var res = string.Empty;
            LoopExecuteWithMutex(mutexName, () =>
            {
                if (!File.Exists(filename)) throw new IOException(
                    $"File [{filename ?? string.Empty}] is NOT Existed!");
                res = File.ReadAllText(filename);
            });
            return res;
        }

        public static void WriteFileWithMutex(
            string filename, string mutexName, string msg)
        {
            LoopExecuteWithMutex(mutexName, () =>
            {
                File.WriteAllText(filename, msg);
            });
        }

        private static void LoopExecuteWithMutex(
            string mutexName, Action action)
        {
            if (action == null) return;
            bool isCreatedNew = false;
            for (int i = 0; ; i++)
            {
                using (var mutex = new Mutex(
                    true, mutexName, out isCreatedNew))
                {
                    if (isCreatedNew)
                    {
                        action.Invoke();
                        return;
                    }
                }
                if (i >= maxRepeatCount)
                {
                    throw new Exception(
                        $"Repeat Count [{i}] >= Max Repeat Count " +
                        $"[{maxRepeatCount}]! Execute With Mutex Failed!");
                }
                Thread.Sleep(waitIOTimeoutMS);
            }
        }

    }
}
