using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace PrinterCenter.UI.FujiEasyLink
{
    public class Pair
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class FujiEasyLinkVM : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the FujiEasyLinkVM class.
        /// </summary>
        public FujiEasyLinkVM()
        {
        }
        #region Lane1
        private bool _isOutputImageLane1 = false;
        public bool isOutputImageLane1
        {
            get { return _isOutputImageLane1; }
            set { Set(() => isOutputImageLane1, ref _isOutputImageLane1, value); }
        }

        private bool _isCopyLane1 = false;
        public bool isCopyLane1
        {
            get { return _isCopyLane1; }
            set { Set(() => isCopyLane1, ref _isCopyLane1, value); }
        }

        private String _ImagePathLane1="";
        public String ImagePathLane1
        {
            get { return _ImagePathLane1; }
            set { Set(() => ImagePathLane1, ref _ImagePathLane1, value); }
        }

        private String _CopyPathLane1="";
        public String CopyPathLane1
        {
            get { return _CopyPathLane1; }
            set { Set(() => CopyPathLane1, ref _CopyPathLane1, value); }
        }


        private bool _isChangeOverEnableLane1 = false;
        public bool isChangeOverEnableLane1
        {
            get { return _isChangeOverEnableLane1; }
            set { Set(() => isChangeOverEnableLane1, ref _isChangeOverEnableLane1, value); }
        }

        private bool _isKanbanLane1 = true;
        public bool isKanbanLane1
        {
            get { return _isKanbanLane1; }
            set { Set(() => isKanbanLane1, ref _isKanbanLane1, value); }
        }

        private bool _isPanelIDLane1=false;
        public bool isPanelIDLane1
        {
            get { return _isPanelIDLane1; }
            set { Set(() => isPanelIDLane1, ref _isPanelIDLane1, value); }
        }


        private String _Lane1Current;
        public String Lane1Current
        {
            get { return _Lane1Current; }
            set { Set(() => Lane1Current, ref _Lane1Current, value); }
        }


        private String _Lane1Next;
        public String Lane1Next
        {
            get { return _Lane1Next; }
            set { Set(() => Lane1Next, ref _Lane1Next, value); }
        }


        private String _Lane1XMLLocation;
        public String Lane1XMLLocation
        {
            get { return _Lane1XMLLocation; }
            set { Set(() => Lane1XMLLocation, ref _Lane1XMLLocation, value); }
        }



        #endregion

        #region Lane2

        private bool _isOutputImageLane2 = false;
        public bool isOutputImageLane2
        {
            get { return _isOutputImageLane2; }
            set { Set(() => isOutputImageLane2, ref _isOutputImageLane2, value); }
        }
        private bool _isCopyLane2 = false;
        public bool isCopyLane2
        {
            get { return _isCopyLane2; }
            set { Set(() => isCopyLane2, ref _isCopyLane2, value); }
        }

        private String _ImagePathLane2 ="";
        public String ImagePathLane2
        {
            get { return _ImagePathLane2; }
            set { Set(() => ImagePathLane2, ref _ImagePathLane2, value); }
        }

        private String _CopyPathLane2="";
        public String CopyPathLane2
        {
            get { return _CopyPathLane2; }
            set { Set(() => CopyPathLane2, ref _CopyPathLane2, value); }
        }


        private bool _isChangeOverEnableLane2 = false;
        public bool isChangeOverEnableLane2
        {
            get { return _isChangeOverEnableLane2; }
            set { Set(() => isChangeOverEnableLane2, ref _isChangeOverEnableLane2, value); }
        }

        private bool _isKanbanLane2 = true;
        public bool isKanbanLane2
        {
            get { return _isKanbanLane2; }
            set { Set(() => isKanbanLane2, ref _isKanbanLane2, value); }
        }

        private bool _isPanelIDLane2 =false;
        public bool isPanelIDLane2
        {
            get { return _isPanelIDLane2; }
            set { Set(() => isPanelIDLane2, ref _isPanelIDLane2, value); }
        }


        private String _Lane2Current;
        public String Lane2Current
        {
            get { return _Lane2Current; }
            set { Set(() => Lane2Current, ref _Lane2Current, value); }
        }

        private String _Lane2Next;
        public String Lane2Next
        {
            get { return _Lane2Next; }
            set { Set(() => Lane2Next, ref _Lane2Next, value); }
        }

        private String _Lane2XMLLocation;
        public String Lane2XMLLocation
        {
            get { return _Lane2XMLLocation; }
            set { Set(() => Lane2XMLLocation, ref _Lane2XMLLocation, value); }
        }


        #endregion


        private ObservableCollection<Pair> _MappingList = new ObservableCollection<Pair>();
        public ObservableCollection<Pair> MappingList
        {
            get { return _MappingList; }
            set { Set(() => MappingList, ref _MappingList, value); }
        }


        private RelayCommand<object> _FindFolderCommand;
        public RelayCommand<object> FindFolderCommand
        {
            get { return _FindFolderCommand ?? (_FindFolderCommand = new RelayCommand<object>(ExecuteFindFolder, CanExecuteFindFolder)); }
            set { _FindFolderCommand = value; }
        }

        public bool CanExecuteFindFolder(object obj)
        {
            if (obj == null) return false;
            else return true;
        }
        void ExecuteFindFolder(object obj)
        {
            System.Windows.Controls.Button btn = obj as System.Windows.Controls.Button;
            string name = btn.Name;

            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.  
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                switch(name)
                {
                    case "Lane1Current":
                        Lane1Current =  folderDlg.SelectedPath;
                        break;
                    case "Lane1Next":
                        Lane1Next = folderDlg.SelectedPath;
                        break;
                    case "Lane1XMLLocation":
                        Lane1XMLLocation = folderDlg.SelectedPath;
                        break;
                    case "Lane1OutputImage":
                        ImagePathLane1 = folderDlg.SelectedPath;
                        break;
                    case "Lane1Copy":
                        CopyPathLane1 = folderDlg.SelectedPath;
                        break;

                    ///==================================
                    case "Lane2Current":
                        Lane2Current = folderDlg.SelectedPath;
                        break;
                    case "Lane2Next":
                        Lane2Next = folderDlg.SelectedPath;
                        break;
                    case "Lane2XMLLocation":
                        Lane2XMLLocation = folderDlg.SelectedPath;
                        break;
                    case "Lane2OutputImage":
                        ImagePathLane2 = folderDlg.SelectedPath;
                        break;
                    case "Lane2Copy":
                        CopyPathLane2 = folderDlg.SelectedPath;
                        break;
                    default:
                        break;
                }

      
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }


    }
}