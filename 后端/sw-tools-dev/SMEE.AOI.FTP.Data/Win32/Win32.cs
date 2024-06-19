using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Data
{
    public class Win32
    {
        #region Ini

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        public static extern long WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        public static extern uint GetPrivateProfileSectionNames(IntPtr lpszRetrunBuffer, uint nSize, string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        public static extern uint GetPrivateProfileSection(string lpAppName, IntPtr lpszRetrunString, uint nSize, string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        public static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder LpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        public static extern int GetLastError();

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        public static extern int FormatMessage(int dwFlags, ref IntPtr lpSource, int dwMessageId, int dwLanguageId, ref string lpBuffer, int nSize, ref IntPtr arguments);
        #endregion

    }
}
