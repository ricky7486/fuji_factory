using aejw.Network;
using GalaSoft.MvvmLight;
using PrinterCenter.File;
using PrinterCenter.Localization;
using PrinterCenter.Log;
using PrinterCenter.Service;
using PrinterCenter.UI;
using PrinterCenter.UI.CommonSetting;
using PrinterCenter.UI.OneLaneSelector;
using PrinterCenter.UI.SharedFolderSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace PrinterCenter
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class PrinterWindowVM : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the PrinterWindowVM class.
        /// </summary>
        private PrinterWindow _window;
        private bool IsServiceBeenOpened = false;
        public PrinterWindowVM()
        {
            _window = (PrinterWindow)Application.Current.MainWindow;
            using (IniFile ini = new IniFile())
            {
          


                Boolean.TryParse(ini.Read("ToolBar", "Setting"), out _SettingVisible);
                Boolean.TryParse(ini.Read("ToolBar", "Chart"), out _ChartVisible);
                Boolean.TryParse(ini.Read("ToolBar", "Flow"), out _FlowVisible);
                Boolean.TryParse(ini.Read("ToolBar", "Wipe"), out _WipeVisible);
                Boolean.TryParse(ini.Read("ToolBar", "Doctor"), out _DoctorVisible);

                string initPage = ini.Read("ToolBar", "Default");
                switch(initPage)
                {
                    case "Chart":
                        _ChartIsCheck = true;
                        break;
                    case "Flow":
                        _FlowIsCheck = true;
                        break;
                    case "Wipe":
                        _WipeIsCheck = true;
                        break;
                    case "Doctor":
                        _DoctorIsCheck = true;
                        break;
                    case "Setting":
                    default:
                        _SettingIsCheck = true;
                        break;
                }

                isCustomizationVisible = ini.IsSectionExist("Custom");
                isFujiEasyLinkVisible = ini.IsSectionExist("FujiEasyLink");

            }
        }

        #region 左側工具箱
        private bool _SettingVisible;
        public bool SettingVisible
        {
            get { return _SettingVisible; }
            set { Set(() => SettingVisible, ref _SettingVisible, value); }
        }

        private bool _SettingIsCheck;
        public bool SettingIsCheck
        {
            get { return _SettingIsCheck; }
            set { Set(() => SettingIsCheck, ref _SettingIsCheck, value); }
        }


        private bool _ChartVisible;
        public bool ChartVisible
        {
            get { return _ChartVisible; }
            set { Set(() => ChartVisible, ref _ChartVisible, value); }
        }
        private bool _ChartIsCheck;
        public bool ChartIsCheck
        {
            get { return _ChartIsCheck; }
            set { Set(() => ChartIsCheck, ref _ChartIsCheck, value); }
        }

        private bool _FlowVisible;
        public bool FlowVisible
        {
            get { return _FlowVisible; }
            set { Set(() => FlowVisible, ref _FlowVisible, value); }
        }
        private bool _FlowIsCheck;
        public bool FlowIsCheck
        {
            get { return _FlowIsCheck; }
            set { Set(() => FlowIsCheck, ref _FlowIsCheck, value); }
        }

        private bool _WipeVisible;
        public bool WipeVisible
        {
            get { return _WipeVisible; }
            set { Set(() => WipeVisible, ref _WipeVisible, value); }
        }
        private bool _WipeIsCheck;
        public bool WipeIsCheck
        {
            get { return _WipeIsCheck; }
            set { Set(() => WipeIsCheck, ref _WipeIsCheck, value); }
        }

        private bool _DoctorVisible;
        public bool DoctorVisible
        {
            get { return _DoctorVisible; }
            set { Set(() => DoctorVisible, ref _DoctorVisible, value); }
        }
        private bool _DoctorIsCheck;
        public bool DoctorIsCheck
        {
            get { return _DoctorIsCheck; }
            set { Set(() => DoctorIsCheck, ref _DoctorIsCheck, value); }
        }

        #endregion
        private String _ServiceBtnContent = "@OPEN_SERVICE".Translate();
        public String ServiceBtnContent
        {
            get { return _ServiceBtnContent; }
            set { Set(() => ServiceBtnContent, ref _ServiceBtnContent, value); }
        }

        //Customization Tab

        private bool _isCustomizationVisible =false ;
        public bool isCustomizationVisible
        {
            get { return _isCustomizationVisible; }
            set { Set(() => isCustomizationVisible, ref _isCustomizationVisible , value); }
        }

        private bool _IsAutoLoadFile = false;
        public bool IsAutoLoadFile
        {
            get { return _IsAutoLoadFile; }
            set { Set(() => IsAutoLoadFile, ref _IsAutoLoadFile, value); }
        }

        #region OpenService
        private GalaSoft.MvvmLight.Command.RelayCommand _OpenServiceCommand;
        public GalaSoft.MvvmLight.Command.RelayCommand OpenServiceCommand
        {
            get { return _OpenServiceCommand ?? (_OpenServiceCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteOpenService, () => CanExecuteOpenService)); }
            set { _OpenServiceCommand = value; }
        }
        bool _canExecuteOpenService = true;
        public bool CanExecuteOpenService
        {
            get { return _canExecuteOpenService; }
            set { if (value != _canExecuteOpenService) { _canExecuteOpenService = value; OpenServiceCommand.RaiseCanExecuteChanged(); } }
        }


        public void ExecuteOpenService()
        {
            Log4.PrinterLogger.Info("[A] Press Open Service button...");
            if (!IsServiceBeenOpened)
            {
                //將資料導入到PrinterManager
                Log4.PrinterLogger.Info("   Importing Settings...");
                if (ViewModelLocator.Atom.LaneSelectorHostVM.LaneContainer.Count == 1)
                {
                    Log4.PrinterLogger.Info("   Importing Settings... Lane1");
                    PrinterManager.getInstance().ImportSetting(0
                        , (OneLaneSelectorVM)ViewModelLocator.Atom.LaneSelectorHostVM.LaneContainer[0].DataContext
                        , (CommonSettingVM)_window.ucLane1_Common.DataContext
                        , (SharedFolderSettingVM)_window.ucLane1_SF.DataContext);
                }
                else if (ViewModelLocator.Atom.LaneSelectorHostVM.LaneContainer.Count == 2)
                {
                    Log4.PrinterLogger.Info("   Importing Settings... Lane1");
                    PrinterManager.getInstance().ImportSetting(0
                        , (OneLaneSelectorVM)ViewModelLocator.Atom.LaneSelectorHostVM.LaneContainer[0].DataContext
                        , (CommonSettingVM)_window.ucLane1_Common.DataContext
                        , (SharedFolderSettingVM)_window.ucLane1_SF.DataContext);
                    Log4.PrinterLogger.Info("   Importing Settings... Lane2");
                    PrinterManager.getInstance().ImportSetting(1
                        , (OneLaneSelectorVM)ViewModelLocator.Atom.LaneSelectorHostVM.LaneContainer[1].DataContext
                        , (CommonSettingVM)_window.ucLane2_Common.DataContext
                        , (SharedFolderSettingVM)_window.ucLane2_SF.DataContext);

                }
                //取資料
                if (!ExamineData())
                {
                    IsServiceBeenOpened = false;
                    return;
                }

                try
                {
                    //開啟service
                    PrinterServiceHost.Instance().OpenHost(null);
                    IsServiceBeenOpened = true;
                    Log4.PrinterLogger.Info("================== Service is Opened ==================");
                    ServiceBtnContent = "@CLOSE_SERVICE".Translate();
                }
                catch (Exception e)
                {
                    Log4.PrinterLogger.Info("[X] Service Open Failed!");
                    IsServiceBeenOpened = false;
                }

                //BtnOpenServiceVisible = !IsServiceBeenOpened;

                //清空前面的檢測log 訊息
                _window.lbStatusReporter.Items.Clear();

                if (IsServiceBeenOpened)//已開啟
                {
                    _window.lbStatusReporter.Items.Add("@SERVICE_OPENED".Translate());
                    _window.lbStatusReporter.Items.Add("@LAST_COMMENT".Translate());
                    _window.lbStatusReporter.Items.Add("@LAST_COMMENT2".Translate());
                }
                //Refresh Chart
                _window.ucChartHost.RefreshSelectionChanged();
            }
        }
        #endregion
      
        #region CloseService

        private GalaSoft.MvvmLight.Command.RelayCommand _CloseServiceCommand;
        public GalaSoft.MvvmLight.Command.RelayCommand CloseServiceCommand
        {
            get { return _CloseServiceCommand ?? (_CloseServiceCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteCloseService, () => CanExecuteCloseService)); }
            set { _CloseServiceCommand = value; }
        }
        bool _canExecuteCloseService = true;
        public bool CanExecuteCloseService
        {
            get { return _canExecuteCloseService; }
            set { if (value != _canExecuteCloseService) { _canExecuteCloseService = value; CloseServiceCommand.RaiseCanExecuteChanged(); } }
        }
        public void ExecuteCloseService()
        {
            Log4.PrinterLogger.Info("[A] Press Close Service button...");
            if (IsServiceBeenOpened)
            {
                PrinterServiceHost.Instance().CloseHost(null);
                IsServiceBeenOpened = false;
                ServiceBtnContent = "@OPEN_SERVICE".Translate();
                //清空前面的檢測log 訊息
                _window.lbStatusReporter.Items.Clear();
            }

        }

        #endregion


        #region 換頁
        private int _stcSelectedIndex;
        public int stcSelectedIndex
        {
            get { return _stcSelectedIndex; }
            set
            {
                Set(() => stcSelectedIndex, ref _stcSelectedIndex, value);
                if (_stcSelectedIndex == 6) // 0:Tool 1:General 2:Lane1 3:Lane2 4:Custom 5:FujiEasyLink 6:Final
                    ExecuteFinalTabItemSelected();
            }
        }
        private GalaSoft.MvvmLight.Command.RelayCommand _FinalTabItemSelectedCommand;
        public GalaSoft.MvvmLight.Command.RelayCommand FinalTabItemSelectedCommand
        {
            get { return _FinalTabItemSelectedCommand ?? (_FinalTabItemSelectedCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteFinalTabItemSelected, () => CanExecuteFinalTabItemSelected)); }
            set { _FinalTabItemSelectedCommand = value; }
        }
        bool _canExecuteFinalTabItemSelected = true;
        public bool CanExecuteFinalTabItemSelected
        {
            get { return _canExecuteFinalTabItemSelected; }
            set { if (value != _canExecuteFinalTabItemSelected) { _canExecuteFinalTabItemSelected = value; FinalTabItemSelectedCommand.RaiseCanExecuteChanged(); } }
        }
        void ExecuteFinalTabItemSelected()
        {
            Log4.PrinterLogger.Info("[A] Press Fial TabItem");
            //Examine data
            if (!IsServiceBeenOpened)//服務沒開啟才需要檢查資料
            {
                Log4.PrinterLogger.Info("--- Entering ExamineData procedure ---");
                bool isOK = ExamineData();
                Log4.PrinterLogger.Info("--- Leaving ExamineData procedure ---");
                //_window.btnOpenService.Visibility = isOK ? Visibility.Visible : Visibility.Hidden;
                //BtnOpenServiceVisible = isOK;
            }
        }
        #endregion

        #region 檢查資料function
        public bool ExamineData()
        {
            
            _window.lbStatusReporter.Items.Clear();
            _window.lbStatusReporter.Items.Add("@EXAMINE_SETTING_DATA".Translate());

            bool retLane1 = false;
            bool retLane2 = false;

            if (ViewModelLocator.Atom.LaneSelectorHostVM.LaneContainer.Count == 1)
            {
                retLane1 = CheckEachValues(0, (OneLaneSelectorVM)ViewModelLocator.Atom.LaneSelectorHostVM.LaneContainer[0].DataContext, (CommonSettingVM)_window.ucLane1_Common.DataContext, (SharedFolderSettingVM)_window.ucLane1_SF.DataContext);
                if (retLane1)
                    return true;
                else
                    return false;
            }
            else if (ViewModelLocator.Atom.LaneSelectorHostVM.LaneContainer.Count == 2)
            {

                retLane1 = CheckEachValues(0, (OneLaneSelectorVM)ViewModelLocator.Atom.LaneSelectorHostVM.LaneContainer[0].DataContext, (CommonSettingVM)_window.ucLane1_Common.DataContext, (SharedFolderSettingVM)_window.ucLane1_SF.DataContext);
                retLane2 = CheckEachValues(1, (OneLaneSelectorVM)ViewModelLocator.Atom.LaneSelectorHostVM.LaneContainer[1].DataContext, (CommonSettingVM)_window.ucLane2_Common.DataContext, (SharedFolderSettingVM)_window.ucLane2_SF.DataContext);

                if (retLane1 && retLane2)
                    return true;
                else
                    return false;
            }
            else
            {
                _window.lbStatusReporter.Items.Add("@THERE_IS_NO_DATA".Translate());
                Log4.PrinterLogger.Info("There is no Data!");
                return false;
            }

        }
        //以下檢查站時不包含每個Printer的Rule
        private bool CheckEachValues(int laneID, OneLaneSelectorVM vm, CommonSettingVM commonvm, SharedFolderSettingVM sfvm)
        {
            bool _bAllGood = true;
            //1.檢查CommonSetting


            //2.檢查SharedFolderSetting
            _window.lbStatusReporter.Items.Add(String.Format("@LANE_DATA".Translate() + ":", laneID + 1));
            Log4.PrinterLogger.InfoFormat("Check Lane {0} Data:",laneID+1);
            //Input SharedFolder
            if (SharedFolderRules.HasInputSharedFolder(vm.Vendor))
                _bAllGood = CheckSharedFolder(sfvm.InDriveInfo, "@RECIEVE");

            //Output SharedFolder
            if (SharedFolderRules.HasOutputSharedFolder(vm.Vendor))
                _bAllGood = CheckSharedFolder(sfvm.OutDriveInfo, "@SEND");

            if (_bAllGood)
            {
                _window.lbStatusReporter.Items.Add("@PASS".Translate());
                Log4.PrinterLogger.Info(" RESULT = PASS!");
            }


            return _bAllGood;
        }
        private bool CheckSharedFolder(string folder, string type)
        {
            Log4.PrinterLogger.Info("Checking "+ type + " SharedFolder...");
            bool ret = true;
            string diskID = WmiDiskHelper.ExtractDiskID(folder);
            string provider = WmiDiskHelper.ExtractProviderName(folder);
            if (String.IsNullOrEmpty(diskID) || String.IsNullOrWhiteSpace(diskID))
            {
                _window.lbStatusReporter.Items.Add("(" + type + ")" + "@DISK".Translate() + " " + "@INVALID".Translate());
                Log4.PrinterLogger.Info(type+" SharedFolder Invalid!");
                ret = false;
            }
            if (String.IsNullOrEmpty(provider) || String.IsNullOrWhiteSpace(provider))
            {
                _window.lbStatusReporter.Items.Add("(" + type + ")" + "@SHARED_FOLDER".Translate() + " " + "@INVALID".Translate());
                Log4.PrinterLogger.Info(type+" SharedFolder Invalid!");
                ret = false;
            }
            return ret;
        }

        #endregion
        #region Refresh SN


        private long _SN = 0;
        public long SN
        {
            get { return _SN; }
            set { Set(() => SN, ref _SN, value); }
        }

        private bool _SNEnable = false;
        public bool SNEnable
        {
            get { return _SNEnable; }
            set { Set(() => SNEnable, ref _SNEnable, value); }
        }


        private GalaSoft.MvvmLight.Command.RelayCommand _SNEditCommand;
        public GalaSoft.MvvmLight.Command.RelayCommand SNEditCommand
        {
            get { return _SNEditCommand ?? (_SNEditCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteSNEdit, () => CanExecuteSNEdit)); }
            set { _SNEditCommand = value; }
        }
        bool _canExecuteSNEdit = true;
        public bool CanExecuteSNEdit
        {
            get { return _canExecuteSNEdit; }
            set { if (value != _canExecuteSNEdit) { _canExecuteSNEdit = value; SNEditCommand.RaiseCanExecuteChanged(); } }
        }
        void ExecuteSNEdit()
        {
            SNEnable = !SNEnable;
        }


        #endregion


        //FujiEasyLink Tab

        private bool _isFujiEasyLinkVisible = false;
        public bool isFujiEasyLinkVisible
        {
            get { return _isFujiEasyLinkVisible; }
            set { Set(() => isFujiEasyLinkVisible, ref _isFujiEasyLinkVisible, value); }
        }


        #region OpenFile
        private GalaSoft.MvvmLight.Command.RelayCommand _OpenFileCommand;
        public GalaSoft.MvvmLight.Command.RelayCommand OpenFileCommand
        {
            get { return _OpenFileCommand ?? (_OpenFileCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteOpenFile, () => CanExecuteOpenFile)); }
            set { _OpenFileCommand = value; }
        }
        bool _canExecuteOpenFile = true;
        public bool CanExecuteOpenFile
        {
            get { return _canExecuteOpenFile; }
            set { if (value != _canExecuteOpenFile) { _canExecuteOpenFile = value; OpenFileCommand.RaiseCanExecuteChanged(); } }
        }
        public void ExecuteOpenFile()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Xml Files (*.xml)| *.xml";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    LoadSettingFromXml(ofd.FileName);
                }
                catch (Exception ex)
                {
                    Log4.PrinterLogger.ErrorFormat("Exception={0}", ex.Message);
                }
            }
            else
            {
                return;
            }
        }
        public bool LoadSettingFromXml(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    LoadSettings(path);
                }
            }
            catch (Exception ex)
            {
                Log4.PrinterLogger.ErrorFormat("Exception={0}", ex.Message);
                return false;
            }
            return true;
        }

        private void LoadSettings(string filename)
        {
            XElement po = XElement.Load(filename);
            List<XElement> _lstLane =
                (from el in po.Elements()
                 select el).ToList();

            //_window._ucLaneSettingHost.ClearAllLaneSettings();
            ViewModelLocator.Atom.LaneSelectorHostVM.LaneContainer.Clear();

            foreach (XElement el in _lstLane)
            {
                ViewModelLocator.Atom.LaneSelectorHostVM.AddOneLaneSetting(ExtractOneLaneDataFromXml(el));
                //Load Common&SF setting VM for each Lane
                XElement common = el.getElement("CommonSetting");
                XElement sfsetting = el.getElement("SharedFolderSetting");
                if(el.getAttributeValue("ID") == "1")
                {
                    //load入第一軌
                    _window.ucLane1_Common.DataContext = common.ToCommonSettingVM();
                    _window.ucLane1_SF.DataContext = sfsetting.ToSharedFolderSettingVM();
                }
                else if (el.getAttributeValue("ID") == "2")
                {
                    //load入第二軌
                    _window.ucLane2_Common.DataContext = common.ToCommonSettingVM();
                    _window.ucLane2_SF.DataContext = sfsetting.ToSharedFolderSettingVM();
                }
            }
        }
        private OneLaneSelectorVM ExtractOneLaneDataFromXml(XElement secLane)
        {
            OneLaneSelectorVM olm = new OneLaneSelectorVM();
            ePrinterVendor vendor;
            Enum.TryParse(secLane.getValue("Vendor"), true, out vendor);// #改Enum

            olm.Vendor = vendor;// #改Enum
            return olm;
        }
        #endregion
        #region Save File

        private GalaSoft.MvvmLight.Command.RelayCommand _SaveFileCommand;
        public GalaSoft.MvvmLight.Command.RelayCommand SaveFileCommand
        {
            get { return _SaveFileCommand ?? (_SaveFileCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteSaveFile, () => CanExecuteSaveFile)); }
            set { _SaveFileCommand = value; }
        }
        bool _canExecuteSaveFile = true;
        public bool CanExecuteSaveFile
        {
            get { return _canExecuteSaveFile; }
            set { if (value != _canExecuteSaveFile) { _canExecuteSaveFile = value; SaveFileCommand.RaiseCanExecuteChanged(); } }
        }
        void ExecuteSaveFile()
        {

            List <OneLaneSelectorVM> _lstOneLaneModel = new List<OneLaneSelectorVM>();
            foreach (var item in ViewModelLocator.Atom.LaneSelectorHostVM.LaneContainer)
            {
                _lstOneLaneModel.Add( (OneLaneSelectorVM)(item as ucOneLaneSelector).DataContext);
            }

            TRMessageBox.Show("@AUTOLOADFILENAME_HINT".Translate());
            System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog();
            ofd.Filter = "Xml Files (*.xml)| *.xml";

            //string pathName = "PlacerCenter.xml";
            ofd.Filter = "Xml Files (*.xml)| *.xml";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {

                    XElement root =new XElement("PRINTERCENTER");
                    CommonSettingVM lane1CommonVM = (CommonSettingVM)_window.ucLane1_Common.DataContext;
                    SharedFolderSettingVM lane1SFVM = (SharedFolderSettingVM)_window.ucLane1_SF.DataContext;
                    CommonSettingVM lane2CommonVM = (CommonSettingVM)_window.ucLane2_Common.DataContext;
                    SharedFolderSettingVM lane2SFVM = (SharedFolderSettingVM)_window.ucLane2_SF.DataContext;



                    List<XElement> LaneList = new List<XElement>(); 

                    foreach(var lane in _lstOneLaneModel)
                    {
                        var iLaneID = _lstOneLaneModel.IndexOf(lane) + 1;
                        var sVendor = lane.Vendor;
                        if(iLaneID == 1)
                        {
                            var el = new XElement("LANE", new XAttribute("ID", iLaneID)
                                                      , new XElement("Vendor", sVendor)
                                                      , lane1CommonVM.ToXml()
                                                      , lane1SFVM.ToXml()
                                               );
                            LaneList.Add(el);
                        }
                        else if(iLaneID == 2)
                        {
                            var el = new XElement("LANE", new XAttribute("ID", iLaneID)
                                                      , new XElement("Vendor", sVendor)
                                                      , lane2CommonVM.ToXml()
                                                      , lane2SFVM.ToXml()
                                               );
                            LaneList.Add(el);
                        }

                    }

                    root.Add(LaneList);

                   
                    root.Save(ofd.FileName);

                }
                catch (Exception ex)
                {
                    Log4.PrinterLogger.ErrorFormat("Exception={0}", ex.Message);
                    return;
                }
            }
        }
        #endregion


    }
}