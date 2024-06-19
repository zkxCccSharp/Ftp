using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;


namespace SMEE.AOI.FTP.Data.Config
{
    public class IniOperator
    {
        private static readonly int errMsgBuffSize = 0x400;

        public static void CreateFile(string path)
        {
            try
            {
                string dr = Path.GetDirectoryName(path);

                if (!Directory.Exists(dr))
                {
                    Directory.CreateDirectory(dr);
                }
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = new StreamWriter(path, false, Encoding.Default))
                    {
                        sw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool WriteIniValue(string section, string key, string val, string iniFilePath)
        {
            long opSt = Win32.WritePrivateProfileString(section, key, val, iniFilePath);
            return !0.Equals(opSt);
        }

        public static string[] ReadIniSections(string path, uint maxBuffSize)
        {
            string[] sections = new string[0];
            IntPtr pRetrunedString = Marshal.AllocCoTaskMem((int)maxBuffSize * sizeof(char));
            try
            {
                uint bytes = Win32.GetPrivateProfileSectionNames(pRetrunedString, maxBuffSize, path);
                if (!0.Equals(bytes))
                {
                    string local = Marshal.PtrToStringAuto(pRetrunedString, (int)bytes).ToString();
                    sections = local.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(pRetrunedString);
            }
            return sections;
        }

        public static string[] ReadIniItemsBySection(string section, string path, uint maxBuffSize)
        {
            string[] items = new string[0];
            IntPtr pRetrunedString = Marshal.AllocCoTaskMem((int)maxBuffSize * sizeof(char));
            try
            {
                while (true)
                {
                    uint bytes = Win32.GetPrivateProfileSection(section, pRetrunedString, maxBuffSize, path);
                    if (bytes.Equals(maxBuffSize - 2))
                    {
                        maxBuffSize *= 2;
                        continue;
                    }
                    else
                    {
                        string local = Marshal.PtrToStringAuto(pRetrunedString, (int)bytes).ToString();
                        items = local.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
                        break;
                    }
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(pRetrunedString);
            }
            return items;
        }

        public static string ReadIniValue(string section, string key, string defaultValue, int buffSize, string path)
        {
            StringBuilder retrunedString = null;
            int times = 0;
            int maxTimes = 10;
            while (times <= maxTimes)
            {
                retrunedString = new StringBuilder(buffSize);
                long bytesLength = Win32.GetPrivateProfileString(section, key, defaultValue, retrunedString, buffSize, path);
                if (bytesLength == buffSize - 1 || bytesLength == buffSize - 2)
                {
                    if (times == maxTimes)
                    {
                        throw new Exception($"ReadIniValue failed, retrunedString buffer overflow, buffSize:{buffSize}");
                    }
                    buffSize *= 2;
                    times++;
                    continue;
                }
                break;
            }

            return retrunedString.ToString();
        }

        public static string GetLastErrorMsg()
        {
            string errMsg = string.Empty;
            int errorCode = Win32.GetLastError();
            if (!0.Equals(errorCode))
            {
                errMsg = GetSysErrorMsg(errorCode);
            }
            return errMsg;
        }

        private static string GetSysErrorMsg(int errorCode)
        {
            IntPtr tempptr = IntPtr.Zero;
            string msg = null;
            Win32.FormatMessage(0x1300, ref tempptr, errorCode, 0, ref msg, errMsgBuffSize, ref tempptr);
            return msg;
        }
    }
}
