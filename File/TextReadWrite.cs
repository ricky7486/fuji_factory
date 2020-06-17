using PrinterCenter.Log;
using PrinterCenter.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// 移植自SPIBaseStruture
/// </summary>
namespace PrinterCenter.FileClass
{
    public abstract class FileBehavior
    {
        private String strFilePath = "";
        public String FilePath
        {
            get { return strFilePath; }
            set { strFilePath = value; }
        }

        public abstract bool Run(bool bAppend = false);
    }
    public abstract class TextBehavior : FileBehavior
    {
        private List<String> strList = null;
        public List<String> StrList
        {
            get { return strList; }
            set { strList = value; }
        }

        public void Dispose()
        {
            if (strList != null)
            {
                strList.Clear();
                strList = null;
            }
        }
    }
    public class TextReader : TextBehavior
    {
        public TextReader(String strFilePath)
        {
            FilePath = strFilePath;
        }

        public override bool Run(bool bAppend = false)
        {
            if (!System.IO.File.Exists(FilePath))
                return false;

            if (StrList == null)
                StrList = new List<String>();
            else
                StrList.Clear();

            using (StreamReader reader = System.IO.File.OpenText(FilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    StrList.Add(line);
            }
            return true;
        }
    }

    public class TextWriter : TextBehavior
    {
        public TextWriter(String strFilePath)
        {
            FilePath = strFilePath;
            if (StrList == null)
                StrList = new List<string>();
        }

        public override bool Run(bool bAppend = false)
        {
            if (string.IsNullOrEmpty(FilePath))
                return false;
            if (StrList == null || StrList.Count < 1)
                return false;

            StreamWriter streamWriter = null;
            try
            {
                streamWriter = new StreamWriter(FilePath, bAppend);
                for (int i = 0; i < StrList.Count; i++)
                {
                    streamWriter.WriteLine(StrList[i]);
                }
            }
            catch (Exception ex)
            {
                TRMessageBox.Show(
                    ex.Message,
                    "PrinterCenter",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return false;
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Flush();
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
            }
            return true;
        }
    }


  
}
