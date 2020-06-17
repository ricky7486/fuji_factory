using PrinterCenter.Printer.JudgeWipe;
using PrinterCenter.Service;
using PrinterCenter.UI.CommonSetting;
using PrinterCenter.UI.OneLaneSelector;
using PrinterCenter.UI.SharedFolderSetting;
using SFCData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PrinterCenter
{
    public struct CurrentInspectModeTemp
    {
        public int InspectMode;
        public int InspectResult;
    }
    public class PrinterManager
    {
        public PrinterEntity[] RemotePrinter;
        public CurrentInspectModeTemp[] CurrentInspectModeTemp= new PrinterCenter.CurrentInspectModeTemp[2];
        private static PrinterManager uniqueInstance;
        private static readonly object synclock = new Object();
        public PrinterWindow _window;
        //MonitorFolders 是根據所設定的Input(Printer給SPI)的共享資料夾實體
        //有幾個獨立的實體位置，就會有幾個獨立的watcher
        //而SharedFolderWatcher內部會有一個WatedFiles，只記錄當OnCreate後看到的檔案名稱
        //會連接到ViewModelLocator.Atom.FlowHostVM.Lane1WFList / ViewModelLocator.Atom.FlowHostVM.Lane2WFList 做顯示
        //ViewModelLocator.Atom.FlowHostVM.Lane1WFList VM為CheckBox的ListBox，內有IsChecked用來設定是否Parse過
        public List<SharedFolderWatcher> MonitorFolders = new List<SharedFolderWatcher>();
        public List<IJudgeWipeAlgorithm> JudgeWipeRoutines = new List<IJudgeWipeAlgorithm>();

        #region 傳輸方式
        public  eSFCDataExchangeMethod ExchangeMethod = eSFCDataExchangeMethod.Memory;
        public  string ExchangePath = @"\\DataExchange";
        #endregion
        public static PrinterManager getInstance()
        {
            lock (synclock)
            {
                if (uniqueInstance == null)
                {
                    uniqueInstance = new PrinterManager();
                }
                return uniqueInstance;
            }
        }

        public PrinterDuplexService PrinterDuplexServiceInstance
        {
            get { return _PrinterDuplexServiceInstance; }
            set { _PrinterDuplexServiceInstance = value; }
        }
        private PrinterDuplexService _PrinterDuplexServiceInstance;
        private PrinterManager()
        {
            _window = (PrinterWindow)Application.Current.MainWindow;

            RemotePrinter = new PrinterEntity[2] { new PrinterEntity(eAssignedLane_Printer.Lane1, ePrinterVendor.None), new PrinterEntity(eAssignedLane_Printer.Lane2, ePrinterVendor.None) };
            
            MonitorFolders.Clear();
            JudgeWipeRoutines.Clear();

            using (var ini = new IniFile())
            {
                string method = ini.Read("PrinterCenter", "Data_Exchange");
                Enum.TryParse(method, out ExchangeMethod);
                Log.Log4.PrinterLogger.InfoFormat("*ExchangeMethod={0}", ExchangeMethod.ToString());
            }
        }
        /// <summary>
        /// Service開啟的入口點*****
        /// </summary>
        /// <param name="laneID">The lane identifier.</param>
        /// <param name="vm">The vm.</param>
        /// <param name="commonvm">The commonvm.</param>
        /// <param name="sfvm">The SFVM.</param>
        /// <returns></returns>
        public bool ImportSetting(int laneID, OneLaneSelectorVM vm, CommonSettingVM commonvm, SharedFolderSettingVM sfvm)
        {
            bool ret = false;



            RemotePrinter[laneID].PrinterVendor = vm.Vendor;//此時Create物件，開始監控sharedFolder
            RemotePrinter[laneID].Printer.PrinterCommonSetting = commonvm.Clone();
            RemotePrinter[laneID].Printer.PrinterSFSetting = sfvm.Clone();
            RemotePrinter[laneID].Printer.Activate();//開始監控
            return ret;
        }
        public void EnableLane1SettingUI(bool isEnable)
        {
            if (isEnable)
                _window.tabLane1.Visibility = Visibility.Visible;
            else
                _window.tabLane1.Visibility = Visibility.Collapsed;
        }
        public void EnableLane2SettingUI(bool isEnable)
        {
            if (isEnable)
                _window.tabLane2.Visibility = Visibility.Visible;
            else
                _window.tabLane2.Visibility = Visibility.Collapsed;
        }


        public bool IsAlreadyExistWatcher(string target)
        {
            return MonitorFolders.Exists(x => x.Target == target);
        }
        /// <summary>
        /// Adds the watcher.
        /// </summary>
        /// <param name="target">網路磁碟映射</param>
        /// <param name="laneID">The lane identifier.</param>
        /// <param name="des">描述</param>
        /// <returns></returns>
        /*public Queue<string> AddWatcher(string target, eAssignedLane_Printer laneID, string des ="")
        {
            if(!IsAlreadyExistWatcher(target))
            {
                var add = new SharedFolderWatcher(target,laneID,des);
                MonitorFolders.Add(add);
                return add.WatchedFiles;
            }else
            {
                var watched = MonitorFolders.FirstOrDefault(x => x.Target == target);
                return watched.WatchedFiles;
            }
        }*/

        public SharedFolderWatcher AddWatcher(string target, eAssignedLane_Printer laneID, string des = "")
        {
            if (!IsAlreadyExistWatcher(target))
            {
                var add = new SharedFolderWatcher(target, laneID, des);
                MonitorFolders.Add(add);
                return add;
            }
            else
            {
                var watched = MonitorFolders.FirstOrDefault(x => x.Target == target);
                return watched;
            }
        }
    }
}
