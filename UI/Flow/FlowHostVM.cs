using GalaSoft.MvvmLight;
using PrinterCenter.Service;

namespace PrinterCenter.UI.Flow
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class FlowHostVM : ViewModelBase
    {
        private IniFile _iniFile = new IniFile();
        private static bool _IsDeleteMatchedFile;
        private CheckableObservableCollection<string> _Lane1WFList;
        public CheckableObservableCollection<string> Lane1WFList     
        {
            get

            {
                if (_Lane1WFList == null)
                    _Lane1WFList = new CheckableObservableCollection<string>();
                return _Lane1WFList;
            }
            set { Set(() => Lane1WFList, ref _Lane1WFList, value); }
        }
        private CheckableObservableCollection<string> _Lane2WFList;
        public CheckableObservableCollection<string> Lane2WFList
        {
            get

            {
                if (_Lane2WFList == null)
                    _Lane2WFList = new CheckableObservableCollection<string>();
                return _Lane2WFList;
            }
            set { Set(() => Lane2WFList, ref _Lane2WFList, value); }
        }

          private CheckableObservableCollection<string> _spiInspectedData;
        public CheckableObservableCollection<string> spiInspectedData
        {
            get

            {
                if (_spiInspectedData == null)
                    _spiInspectedData = new CheckableObservableCollection<string>();
                return _spiInspectedData;
            }
            set { Set(() => spiInspectedData, ref _spiInspectedData, value); }
        }


        private bool _IsLane1WFExist;
        public bool IsLane1WFExist
        {
            get { return _IsLane1WFExist; }
            set { Set(() => IsLane1WFExist, ref _IsLane1WFExist, value); }
        }
        private bool _IsLane2WFExist;
        public bool IsLane2WFExist
        {
            get { return _IsLane2WFExist; }
            set { Set(() => IsLane2WFExist, ref _IsLane2WFExist, value); }
        }
        private string _Lane1WFDisk;
        public string Lane1WFDisk
        {
            get { return _Lane1WFDisk; }
            set { Set(() => Lane1WFDisk, ref _Lane1WFDisk, value); }
        }
        private string _Lane2WFDisk;
        public string Lane2WFDisk
        {
            get { return _Lane2WFDisk; }
            set { Set(() => Lane2WFDisk, ref _Lane2WFDisk, value); }
        }


        private string _Lane1WFPath;
        public string Lane1WFPath
        {
            get { return _Lane1WFPath; }
            set { Set(() => Lane1WFPath, ref _Lane1WFPath, value); }
        }
        private string _Lane2WFPath;
        public string Lane2WFPath
        {
            get { return _Lane2WFPath; }
            set { Set(() => Lane2WFPath, ref _Lane2WFPath, value); }
        }

        /// <summary>
        /// Initializes a new instance of the FlowHostVM class.
        /// </summary>
        public FlowHostVM()
        {
            bool.TryParse(_iniFile.Read("PrinterFile", "DeleteMatched"), out _IsDeleteMatchedFile);
        }


        public void MarkWatchedFileCheckBox(eAssignedLane_Printer lane , string file)
        {
            switch(lane)
            {
                case eAssignedLane_Printer.Lane1:
                    foreach (var f in Lane1WFList)
                    {
                        if (f.Value == file)
                        {
                            f.IsChecked = true;
                            if (_IsDeleteMatchedFile)
                                System.IO.File.Delete(Lane1WFPath+"\\"+file);
                            break;
                        }
                    }
                    break;
                case eAssignedLane_Printer.Lane2:
                    foreach (var f in Lane2WFList)
                    {
                        if (f.Value == file)
                        {
                            f.IsChecked = true;
                            if (_IsDeleteMatchedFile)
                                System.IO.File.Delete(Lane2WFPath + "\\" + file);
                            break;
                        }
                    }
                    break;
                case eAssignedLane_Printer.None:
                    break;
            }
        }
        public void MarkSPIDataCheckBox()
        {
            spiInspectedData[spiInspectedData.Count - 1].IsChecked = true;
        }
    }
}