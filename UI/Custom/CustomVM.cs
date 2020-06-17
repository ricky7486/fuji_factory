using GalaSoft.MvvmLight;
using System;

namespace PrinterCenter.UI.Custom
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    struct GKGCSV
    {
        bool enable;
        string path;
    }
    public class CustomVM : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the CustomVM class.
        /// </summary>
        
        public CustomVM()
        {
            //using (IniFile ini = new IniFile())
            //{
            //    CustomVisible = ini.IsSectionExist("Custom");

            //}
        }


        private bool _CustomVisible =true;
        public bool CustomVisible
        {
            get { return _CustomVisible; }
            set { Set(() => CustomVisible, ref _CustomVisible, value); }
        }

        #region GKG Lane1 & Lane2


        private bool _bGKGCSVLane1=false;
        public bool bGKGCSVLane1
        {
            get { return _bGKGCSVLane1; }
            set { Set(() => bGKGCSVLane1, ref _bGKGCSVLane1, value); }
        }

        private String _GKGCSVLane1Path;
        public String GKGCSVLane1Path
        {
            get { return _GKGCSVLane1Path; }
            set { Set(() => GKGCSVLane1Path, ref _GKGCSVLane1Path, value); }
        }


        private bool _bGKGCSVLane2=false;
        public bool bGKGCSVLane2
        {
            get { return _bGKGCSVLane2; }
            set { Set(() => bGKGCSVLane2, ref _bGKGCSVLane2, value); }
        }

        private String _GKGCSVLane2Path;
        public String GKGCSVLane2Path
        {
            get { return _GKGCSVLane2Path; }
            set { Set(() => GKGCSVLane2Path, ref _GKGCSVLane2Path, value); }
        }


        #endregion
    }
}