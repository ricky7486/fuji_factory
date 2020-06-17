using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PrinterCenter.UI;
using PrinterCenter.Log;

namespace PrinterCenter.File
{
    public static class FileProcess
    {
        static public bool CreateFile(string fullPath)
        {
            FileStream fs = null;
            try
            {
                fs = System.IO.File.Create(fullPath);
            }
            catch (Exception e)
            {
                TRMessageBox.Show(e.ToString());
            }
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
                return true;
            }
            return false;
        }

        static public bool MoveFile(string srcPath, string dstPath, bool bCopy = false)
        {
            try
            {
                ClearFileReadOnly(srcPath);
                ClearFileReadOnly(dstPath);

                if (System.IO.File.Exists(srcPath) == false)
                    using (FileStream fs = System.IO.File.Create(srcPath)) { }
                if (System.IO.File.Exists(dstPath))
                    System.IO.File.Delete(dstPath);
                if ((Directory.Exists(dstPath)) == false)
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dstPath));
                if (bCopy == true)
                    System.IO.File.Copy(srcPath, dstPath);
                else
                    System.IO.File.Move(srcPath, dstPath);
                return true;
            }
            catch (Exception ex)
            {
                Log4.PrinterLogger.InfoFormat("move {0} to {1} fail! :{2}", srcPath, dstPath, ex.Message);
                return false;
            }

        }

        static public bool DeleteFile(string filePath)
        {
            if (System.IO.File.Exists(filePath) == false)
                return false;

            ClearFileReadOnly(filePath);

            try
            {
                System.IO.File.Delete(filePath);
            }
            catch (Exception ee)
            {
                Log4.PrinterLogger.InfoFormat("{0} delete fail! {1}", filePath, ee.Message);
                return false;
            }

            return true;
        }

        static public void ClearFileReadOnly(string fileName)
        {
            if (System.IO.File.Exists(fileName) == false)
                return;

            FileInfo fi = new FileInfo(fileName);
            try
            {
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    fi.Attributes = FileAttributes.Normal;
            }
            catch (Exception e)
            {
                Log4.PrinterLogger.InfoFormat("Exception:{0}", e.Message);
            }
            fi = null;
        }

        static public bool SearchFiles(string path, string pattern, List<string> machedData)
        {
            if (Directory.Exists(path) == false)
                return false;

            machedData.Clear();
            string[] files = Directory.GetFiles(path, pattern);
            foreach (string s in files)
            {
                machedData.Add(s);
            }
            return true;
        }

        static public bool CreateFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
                return true;

            try
            {
                var info = Directory.CreateDirectory(folderPath);
                return info.Exists;
            }
            catch (IOException ee)
            {
                Log4.PrinterLogger.InfoFormat(ee.ToString());
                return false;
            }
        }
    }

}
