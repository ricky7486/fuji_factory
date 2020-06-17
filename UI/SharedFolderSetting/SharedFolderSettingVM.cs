using aejw.Network;
using GalaSoft.MvvmLight;
using PrinterCenter.Log;
using PrinterCenter.Service;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace PrinterCenter.UI.SharedFolderSetting
{
    public static class SharedFolderRules
    {

        public static bool HasInputSharedFolder(ePrinterVendor vendor)
        {

            switch (vendor)
            {
                case ePrinterVendor.DEK:
                case ePrinterVendor.DESEN:
                case ePrinterVendor.EKRA:
                case ePrinterVendor.ESE:
                case ePrinterVendor.GKG:
                case ePrinterVendor.HANWHA:
                case ePrinterVendor.HTGD:
                case ePrinterVendor.INOTIS:
                case ePrinterVendor.MINAMI:
                    return true;
                case ePrinterVendor.YAMAHA:
                case ePrinterVendor.MPM:
                case ePrinterVendor.None:
                default:
                    return false; 
            }
        }
        public static bool HasOutputSharedFolder(ePrinterVendor vendor)
        {

            switch (vendor)
            {
                case ePrinterVendor.DEK:
                case ePrinterVendor.DESEN:
                case ePrinterVendor.EKRA:
                case ePrinterVendor.ESE:
                case ePrinterVendor.GKG:
                case ePrinterVendor.HANWHA:
                case ePrinterVendor.HTGD:
                case ePrinterVendor.INOTIS:
                case ePrinterVendor.MINAMI:                    
                case ePrinterVendor.YAMAHA:
                    return true;
                case ePrinterVendor.MPM:
                case ePrinterVendor.None:
                default:
                    return false;
            }
        }

    }
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SharedFolderSettingVM : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the SharedFolderSettingVM class.
        /// </summary>
        public SharedFolderSettingVM()
        {
            _IsInEnable = _IsOutEnable = false;

            _InSharedFolder= WmiDiskHelper.GetDiskNames().ToObservableCollection();
            _OutSharedFolder = WmiDiskHelper.GetDiskNames().ToObservableCollection();
        }
        private string _InDriveInfo;
        public string InDriveInfo
        {
            get { return _InDriveInfo; }
            set { Set(() => InDriveInfo, ref _InDriveInfo, value); }
        }


        private ObservableCollection<string> _InSharedFolder;
        public ObservableCollection<string> InSharedFolder
        {
            get { return _InSharedFolder; }
            set { Set(() => InSharedFolder, ref _InSharedFolder, value); }
        }


        private string _OutDriveInfo;
        public string OutDriveInfo
        {
            get { return _OutDriveInfo; }
            set { Set(() => OutDriveInfo, ref _OutDriveInfo, value); }
        }
        private ObservableCollection<string> _OutSharedFolder;
        public ObservableCollection<string> OutSharedFolder
        {
            get { return _OutSharedFolder; }
            set { Set(() => OutSharedFolder, ref _OutSharedFolder, value); }
        }
        private bool _IsOutEnable;
        public bool IsOutEnable
        {
            get { return _IsOutEnable; }
            set { Set(() => IsOutEnable, ref _IsOutEnable, value); }
        }

        private bool _IsInEnable;
        public bool IsInEnable
        {
            get { return _IsInEnable; }
            set { Set(() => IsInEnable, ref _IsInEnable, value); }
        }

        private ePrinterVendor _Vendor;
        public ePrinterVendor Vendor
        {
            get { return _Vendor; }
            set { Set(() => Vendor, ref _Vendor, value); }
        }



        private GalaSoft.MvvmLight.Command.RelayCommand _InSFSelectionChangedCommand;
        public GalaSoft.MvvmLight.Command.RelayCommand InSFSelectionChangedCommand
        {
            get { return _InSFSelectionChangedCommand ?? (_InSFSelectionChangedCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteInSFSelectionChanged, () => CanExecuteInSFSelectionChanged)); }
            set { _InSFSelectionChangedCommand = value; }
        }
        bool _canExecuteInSFSelectionChanged = true;
        public bool CanExecuteInSFSelectionChanged
        {
            get { return _canExecuteInSFSelectionChanged; }
            set { if (value != _canExecuteInSFSelectionChanged) { _canExecuteInSFSelectionChanged = value; InSFSelectionChangedCommand.RaiseCanExecuteChanged(); } }
        }
        void ExecuteInSFSelectionChanged()
        {
            Log4.PrinterLogger.InfoFormat("[A]Vendor={0} Recieve Folder = {1}",Vendor,InDriveInfo);
        }


        private GalaSoft.MvvmLight.Command.RelayCommand _OutSFSelectionChangedCommand;
        public GalaSoft.MvvmLight.Command.RelayCommand OutSFSelectionChangedCommand
        {
            get { return _OutSFSelectionChangedCommand ?? (_OutSFSelectionChangedCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteOutSFSelectionChanged, () => CanExecuteOutSFSelectionChanged)); }
            set { _OutSFSelectionChangedCommand = value; }
        }
        bool _canExecuteOutSFSelectionChanged = true;
        public bool CanExecuteOutSFSelectionChanged
        {
            get { return _canExecuteOutSFSelectionChanged; }
            set { if (value != _canExecuteOutSFSelectionChanged) { _canExecuteOutSFSelectionChanged = value; OutSFSelectionChangedCommand.RaiseCanExecuteChanged(); } }
        }
        void ExecuteOutSFSelectionChanged()
        {
            Log4.PrinterLogger.InfoFormat("[A]Vendor={0} Send Folder = {1}", Vendor, OutDriveInfo);
        }




        public SharedFolderSettingVM Clone()
        {
            return (SharedFolderSettingVM)this.MemberwiseClone();
        }

        public void UpdateVisibility(ePrinterVendor vendor)
        {
            Vendor = vendor;
            switch (vendor)
            {
                case ePrinterVendor.DEK:
                case ePrinterVendor.DESEN:
                case ePrinterVendor.EKRA:
                case ePrinterVendor.ESE:
                case ePrinterVendor.GKG:
                case ePrinterVendor.HANWHA:
                case ePrinterVendor.HTGD:
                case ePrinterVendor.INOTIS:
                case ePrinterVendor.MINAMI:
                
                   
                    IsInEnable = true;
                    IsOutEnable = true;
                    break;


   
                case ePrinterVendor.YAMAHA:
                case ePrinterVendor.FUJI:
                   
                    IsInEnable = false;
                    IsOutEnable = true;
                    break;
                case ePrinterVendor.MPM:
                case ePrinterVendor.None:
                  
                    IsInEnable = false;
                    IsOutEnable = false;
                    break;
                default:
                    break;
            }
        }


        public XElement ToXml()
        {
            XElement root = new XElement("SharedFolderSetting"
                                            , new XElement("In" , IsInEnable.ToString()
                                                                , new XAttribute("DriveLetter", WmiDiskHelper.ExtractDiskID(InDriveInfo))
                                                                , new XAttribute("DriveProvider", WmiDiskHelper.ExtractProviderName(InDriveInfo))
                                            )
                                            , new XElement("Out", IsOutEnable.ToString()
                                                                , new XAttribute("DriveLetter", WmiDiskHelper.ExtractDiskID(OutDriveInfo))
                                                                , new XAttribute("DriveProvider", WmiDiskHelper.ExtractProviderName(OutDriveInfo))
                                            )
                                            );


            return root;
        }
    }
}