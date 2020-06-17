using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;


namespace PrinterCenter
{
    #region IniFile
    public class IniFile : IDisposable
    {
        public string IniPath { get { return sPath; } }
        string sPath;
        //string EXE = Assembly.GetExecutingAssembly().GetName().Name;
        string EXE = Assembly.GetExecutingAssembly().Location;


        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileSectionNamesA")]
        static extern int GetPrivateProfileSectionNames(byte[] lpszReturnBuffer, int nSize, string lpFileName);

        


        public IniFile(string IniPath = null)
        {
            //Path = new FileInfo(IniPath ?? EXE + ".ini").FullName.ToString();
            sPath = IniPath;
        }
        public IniFile()
        {
            string sDir = Path.GetDirectoryName(EXE);
            string sFileName = Path.GetFileNameWithoutExtension(EXE);

            sPath = sDir + "\\" + sFileName + ".ini";
        }
        public List<string> GetSectionNames()
        {
            byte[] buffer = new byte[1024];
            GetPrivateProfileSectionNames(buffer, buffer.Length, sPath);
            string allSections = System.Text.Encoding.Default.GetString(buffer);
            string[] sectionNames = allSections.Split('\0');
            List<string> ret = new List<string>();
            foreach(var section in sectionNames)
            {
                if(section !=  String.Empty)
                {
                    ret.Add(section);
                }
            }

            return ret;
        }
        public bool IsSectionExist(string section)
        {
            var Sections = GetSectionNames();
            return Sections.Exists(x => x == section);
        }

        public string Read(string Section, string Key)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section, Key, "", RetVal, 255, sPath);
            return RetVal.ToString();
        }

        public void Write(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, sPath);
        }

        public void DeleteKey(string Section, string Key)
        {
            Write(Section, Key, null);
        }

        public void DeleteSection(string Section)
        {
            Write(Section, null, null);
        }

        public bool KeyExists(string Section, string Key)
        {
            return Read(Section, Key).Length > 0;
        }

        public void Dispose()
        {
         
        }
    }
    #endregion
}
