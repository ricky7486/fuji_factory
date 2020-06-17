using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Linq;
using System.Text;

namespace PrinterCenter.UI
{
    public class NetDriveModel : INotifyPropertyChanged
    {
        public NetDriveModel()
        {
            
        }
        private string _DriveInfo;
        public string DriveInfo
        {
            get
            {
                return _DriveInfo;
            }
            set
            {
                if(_DriveInfo != value)
                {
                    _DriveInfo = value;
                    RaisePropertyChanged("DriveInfo");
                }
            }
        }
        private string _ShareFolder;
        public string ShareFolder
        {
            get { return _ShareFolder; }
            set
            {
                if (_ShareFolder != value)
                {
                    _ShareFolder = value;
                    RaisePropertyChanged("ShareFolder");
                }
            }
        }
        private string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                if (_UserName != value)
                {
                    _UserName = value;
                    RaisePropertyChanged("UserName");
                }
            }
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                if (_Password != value)
                {
                    _Password = value;
                    RaisePropertyChanged("Password");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
