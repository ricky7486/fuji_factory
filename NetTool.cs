using PrinterCenter.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PrinterCenter
{
    /// <summary>
    /// 移植自SPIBaseStruture
    /// </summary>
    public class NetTool
    {

        public static bool Ping(string strIP)
        {
            IPAddress ip = null;
            if (IPAddress.TryParse(strIP, out ip) == false)
                return false;

            try
            {
                if (new Ping().Send(ip, 500).Status == IPStatus.Success)
                    return true;
            }
            catch (Exception ex)
            {
                TRMessageBox.Show(ex.Message);
            }
            return false;
        }

        private static int SHARED_FOLDER_CONNECTION_WAIT_TIMEOUT = 2000;

        public static bool IsFileExist(string filePath)
        {
            var isFileExist = false;
            var task = Task.Factory.StartNew(() => { isFileExist = System.IO.File.Exists(filePath); });
            task.Wait(SHARED_FOLDER_CONNECTION_WAIT_TIMEOUT); //若路徑在遠端，連線時間超過timeout值就視為無法連線

            return isFileExist;
        }

        public static bool IsDirectoryExist(string dirPath)
        {
            var isDirExist = false;
            var task = Task.Factory.StartNew(() => { isDirExist = Directory.Exists(dirPath); });
            task.Wait(SHARED_FOLDER_CONNECTION_WAIT_TIMEOUT); //若路徑在遠端，連線時間超過timeout值就視為無法連線

            return isDirExist;
        }
        //從Printer移過來，歸類nettool function
        public static bool IsIP(String IpStr)
        {
            bool bIP = false;
            try
            {
                IPAddress.Parse(IpStr);
                bIP = true;
            }
            catch
            {
                bIP = false;
            }
            return bIP;
        }

        #region get remove or local disk space
        public enum DisplayUnit
        {
            BYTE,
            KBYTE,
            MBYTE,
            GBYTE
        }
        /// <summary>
        /// GetFreeSpace = GetTotalFreeSpace
        /// GetTotalSpace
        /// 
        /// usage:
        /// GetFreeSpace(@"\\192.168.168.49\Temp2",DisplayUnit.GBYTE);
        /// GetFreeSpace(@"C:");
        /// </summary>
        /// <param name="folderName"></param>
        /// specific the path
        /// <param name="unit"></param>
        /// chose Byte/KB/MB/GB to be the unit of output
        /// <returns></returns>
        /// -1: error
        /// others: the size base on unit
        public static long GetFreeSpace(string folderName, DisplayUnit unit = DisplayUnit.BYTE)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException("folderName");
            }

            if (!folderName.EndsWith("\\"))
            {
                folderName += '\\';
            }

            long free = 0, total = 0, totalfree = 0;

            if (GetDiskFreeSpaceEx(folderName, ref free, ref total, ref totalfree))
            {
                switch (unit)
                {
                    case DisplayUnit.BYTE:
                        return free;
                    case DisplayUnit.KBYTE:
                        return free / 1024;
                    case DisplayUnit.MBYTE:
                        return free / (1024 * 1024);
                    case DisplayUnit.GBYTE:
                        return free / (1024 * 1024 * 1024);

                    default:
                        return free;
                }
            }
            else
            {
                return -1;
            }
        }
        public static long GetTotalSpace(string folderName, DisplayUnit unit = DisplayUnit.BYTE)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException("folderName");
            }

            if (!folderName.EndsWith("\\"))
            {
                folderName += '\\';
            }

            long free = 0, total = 0, totalfree = 0;

            if (GetDiskFreeSpaceEx(folderName, ref free, ref total, ref totalfree))
            {
                switch (unit)
                {
                    case DisplayUnit.BYTE:
                        return total;
                    case DisplayUnit.KBYTE:
                        return total / 1024;
                    case DisplayUnit.MBYTE:
                        return total / (1024 * 1024);
                    case DisplayUnit.GBYTE:
                        return total / (1024 * 1024 * 1024);

                    default:
                        return total;
                }
            }
            else
            {
                return -1;
            }
        }
        public static long GetTotalFreeSpace(string folderName, DisplayUnit unit = DisplayUnit.BYTE)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException("folderName");
            }

            if (!folderName.EndsWith("\\"))
            {
                folderName += '\\';
            }

            long free = 0, total = 0, totalfree = 0;

            if (GetDiskFreeSpaceEx(folderName, ref free, ref total, ref totalfree))
            {
                switch (unit)
                {
                    case DisplayUnit.BYTE:
                        return totalfree;
                    case DisplayUnit.KBYTE:
                        return totalfree / 1024;
                    case DisplayUnit.MBYTE:
                        return totalfree / (1024 * 1024);
                    case DisplayUnit.GBYTE:
                        return totalfree / (1024 * 1024 * 1024);

                    default:
                        return totalfree;
                }
            }
            else
            {
                return -1;
            }
        }
        //public static long
        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("Kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool GetDiskFreeSpaceEx
        (
            string lpszPath,                    // Must name a folder, must end with '\'.
            ref long lpFreeBytesAvailable,
            ref long lpTotalNumberOfBytes,
            ref long lpTotalNumberOfFreeBytes
        );
        #endregion
    }
}
