using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.UploadConsole
{
    internal static class ConsoleUtil
    {
        private const string consoleTitle =
            "SMEE.AOI.FTP.UploadConsole.Console";
        private const int redoCountOfConsoleHandler = 20;

        private static readonly ILog logger =
            LogManager.GetLogger(typeof(ConsoleUtil));

        public static void HideConsole()
        {
            try
            {
                SetConsoleTitle();
                var consoleHandler = GetConsoleHandler();
                HideConsole(consoleHandler);
            }
            catch (Exception ex)
            {
                logger.Error("Hide Console Failed!");
                logger.Error(ex);
            }
        }

        private static void SetConsoleTitle()
        {
            System.Console.Title = consoleTitle;
        }

        private static IntPtr GetConsoleHandler()
        {
            var res = FindWindow(null, consoleTitle);
            if (!IsZero(res)) return res;
            for (int i = 0; i < redoCountOfConsoleHandler; i++)
            {
                Thread.Sleep(50);
                res = FindWindow(null, consoleTitle);
                if (!IsZero(res)) return res;
            }
            throw new Exception("Get Console Handler Failed!");
        }

        private static void HideConsole(IntPtr ptr)
        {
            try
            {
                ShowWindow(ptr, 0);
            }
            catch (Exception ex)
            {
                logger.Error(
                    "Hide Console Invoke ShowWindow Failed!");
                logger.Error(ex);
                throw ex;
            }
        }

        private static bool IsZero(IntPtr ptr) =>
            ptr == IntPtr.Zero;

        /// <summary>
        /// 搜索窗口句柄
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "FindWindow",
            SetLastError = true)]
        private static extern IntPtr FindWindow(
            string lpClassName, string lpWindowName);

        /// <summary>
        /// 设置窗口隐藏或显示
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "ShowWindow",
            SetLastError = true)]
        private static extern bool ShowWindow(
            IntPtr hWnd, uint nCmdShow);


    }
}
