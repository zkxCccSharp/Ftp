using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.IPC
{
    public class Helper
    {
        private const int startPort = 23333;
        private const int endPort = 24333;

        public static int GetPort()
        {
            for (int port = startPort; port <= endPort; port++)
            {
                if (IsPostAvailable(port))
                {
                    return port;
                }
            }
            return -1;
        }

        private static bool IsPostAvailable(int port)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] activeEndpoints = ipGlobalProperties.GetActiveTcpListeners();

            foreach (var endPoint in activeEndpoints)
            {
                if (endPoint.Port == port)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsProcessRunning(string processName)
        {
            Process[] processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                if (string.Equals(process.ProcessName, processName,StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsProcessRunning(int pid)
        {
            Process[] processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                if (process.Id == pid)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
