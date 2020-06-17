using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PrinterCenter.Localization
{
    public static class LanguageHelper
    {
        public static string Translate(this string s)
        {
            return Application.Current.Resources[s].ToString().Replace("\\n", "\n");
        }
    }
    /// <summary>
    /// 移植自SPIBaseStructre.dll 
    /// </summary>
    public static class MultiLanguageHelper
    {
        public const int DefaultLocaleID = 1033; //default value - English
        private static int _currentLocaleID = DefaultLocaleID;
        private static Dictionary<int, string> _dicLanguageXaml = new Dictionary<int, string>()
        {

            { 1033, @"/Localization/LanguageDictionary-English.xaml" },
            { 1028, @"/Localization/LanguageDictionary-Taiwan.xaml" },
            { 2052, @"/Localization/LanguageDictionary-China.xaml" },
            { 1041, @"/Localization/LanguageDictionary-Japan.xaml" },
            { 1042, @"/Localization/LanguageDictionary-Korea.xaml" },
            { 1031, @"/Localization/LanguageDictionary-German.xaml" },
        };
        private static void ReplaceApplicationMergedDictionary()
        {

            //UpdateFontSetting(); //get language specific "DefaultFontSize" etc.  i.e.Application.Current.Resources的前三項

            Uri uriNewLanguage;
            ResourceDictionary rdNewLanguageDictionary;
            uriNewLanguage = new Uri(_dicLanguageXaml[_currentLocaleID], UriKind.Relative);
            rdNewLanguageDictionary = (ResourceDictionary)Application.LoadComponent(uriNewLanguage);
            //使用查找，避免日後有人不小心擺錯xaml位置

            //Application.Current.Resources.MergedDictionaries.RemoveAt(0);
            for (int i = 0; i < Application.Current.Resources.MergedDictionaries.Count; i++)
            {
                if (Application.Current.Resources.MergedDictionaries[i].Source.ToString().Contains("/Localization/LanguageDictionary"))
                {
                    Application.Current.Resources.MergedDictionaries.RemoveAt(i);
                    Application.Current.Resources.MergedDictionaries.Insert(i, rdNewLanguageDictionary);
                    //一定要設定Source的URI，不然下次進入if條件式，該MergedDictionaries[i].Source會變null
                    Application.Current.Resources.MergedDictionaries[i].Source = uriNewLanguage;

                    break;
                }
            }

            //Application.Current.Resources.MergedDictionaries.Insert(0, rdNewLanguageDictionary);
        }

        public static void ReloadLanguage_ReplaceVersion(int iLangID)
        {
            _currentLocaleID = iLangID;

            //GetLanguageDefaultFont(iLangID);//取得Font Setting

            ReplaceApplicationMergedDictionary();
        }

        private static int GetLocaleIDFromRegistry(string keyName = @"SOFTWARE\TRI\TR7007i\General",
                                                                                                  string valName = "LanguageLocaleID")
        {
            int localeID = DefaultLocaleID;
            var reg = Registry.LocalMachine.OpenSubKey(keyName, true);
            if (reg == null)
                reg = Registry.LocalMachine.CreateSubKey(keyName);
            if (reg != null)
            {
                bool bOK = false;
                object value = reg.GetValue(valName);
                if (value != null)
                    bOK = Int32.TryParse(value.ToString(), out localeID);
                if (!bOK)
                    reg.SetValue(valName, localeID, RegistryValueKind.DWord);
                reg.Close();
            }
            return localeID;
        }

        public static void SettingLanguageFromRegisty()
        {
            _currentLocaleID = GetLocaleIDFromRegistry();
            ReloadLanguage_ReplaceVersion(_currentLocaleID);
        }
    }
}
