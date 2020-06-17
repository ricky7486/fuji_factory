using GalaSoft.MvvmLight;
using PrinterCenter.Log;
using PrinterCenter.Service;
using System;

namespace PrinterCenter.UI.OneLaneSelector
{

    public class VendorChangedEventArgs : EventArgs
    {
        public VendorChangedEventArgs(ePrinterVendor vendor,int lane)
        {
            Vendor = vendor;
            LaneName = lane;
        }

        public ePrinterVendor Vendor;
        public int LaneName;
    }
    public delegate void VendorChangedHandler(object sender, VendorChangedEventArgs e);

    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class OneLaneSelectorVM : ViewModelBase
    {
        public static event VendorChangedHandler VendorChangedEvent = null;
        /// <summary>
        /// Initializes a new instance of the OneLaneViewModel3 class.
        /// </summary>
        public OneLaneSelectorVM()
        {
            _Vendor = ePrinterVendor.None;

        }

        private int _LaneName;
        public int LaneName
        {
            get { return _LaneName; }
            set { Set(() => LaneName, ref _LaneName, value); }
        }


        private String _LaneTitle;
        public String LaneTitle
        {
            get { return _LaneTitle; }
            set { Set(() => LaneTitle, ref _LaneTitle, value); }
        }


        private ePrinterVendor _Vendor;
        public ePrinterVendor Vendor
        {
            get { return _Vendor; }
            set
            {
                Set(() => Vendor, ref _Vendor, value);
                if (VendorChangedEvent != null)
                    VendorChangedEvent(this, new VendorChangedEventArgs(_Vendor,_LaneName));

            }
        }



        private GalaSoft.MvvmLight.Command.RelayCommand _VendorChangedCommand;
        public GalaSoft.MvvmLight.Command.RelayCommand VendorChangedCommand
        {
            get { return _VendorChangedCommand ?? (_VendorChangedCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteVendorChanged, () => CanExecuteVendorChanged)); }
            set { _VendorChangedCommand = value; }
        }
        bool _canExecuteVendorChanged = true;
        public bool CanExecuteVendorChanged
        {
            get { return _canExecuteVendorChanged; }
            set { if (value != _canExecuteVendorChanged) { _canExecuteVendorChanged = value; VendorChangedCommand.RaiseCanExecuteChanged(); } }
        }
        void ExecuteVendorChanged()
        {
            Log4.PrinterLogger.InfoFormat("[A] Lane{0} : Select {1}", LaneName, Vendor.ToString());
        }



    }
}