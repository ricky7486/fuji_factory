using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using aejw.Network;
using PrinterCenter.Log;

namespace PrinterCenter.UI
{
    public class NetworkDriveMappingChangedEventArgs : EventArgs
    {
        public NetworkDriveMappingChangedEventArgs(string disk)
        {
            Disk = disk;

        }

        public string Disk;
    }
    public delegate void NetworkDriveMappingChangedHandler(object sender, NetworkDriveMappingChangedEventArgs e);

    public class NetworkDriveViewModel
    {
        public event NetworkDriveMappingChangedHandler NetworkDriveMappingChangedEvent = null;
        public NetDriveModel NetDriveM { get; set; }
        public NetworkDriveViewModel()
        {
            NetDriveM = new NetDriveModel();

            
        }
        private ICommand _Create;
        public ICommand Create
        {
            get
            {
                if (this._Create == null)
                {
                    this._Create = 
                    new RelayCommand(
                        () => this.CreateExecute(),
                        () => this.CanCreate);
                    
                }
                return this._Create;
            }
        }
        private ICommand _Diagnosis;
        public ICommand Diagnosis
        {
            get
            {
                if (this._Diagnosis == null)
                {
                    this._Diagnosis =
                    new RelayCommand(
                        () => this.DiagnosisExecute(),
                        () => this.CanDiagnosis);

                }
                return this._Diagnosis;
            }
        }


        private void CreateExecute()
        {
            Log4.PrinterLogger.Info("[A][Tool Page]Press Create button.");
            NetworkDriveWrapper.MappingNetDrive(WmiDiskHelper.ExtractDiskID( NetDriveM.DriveInfo ), NetDriveM.ShareFolder, NetDriveM.UserName, NetDriveM.Password);
            if(NetworkDriveMappingChangedEvent!= null)
                NetworkDriveMappingChangedEvent(this, new NetworkDriveMappingChangedEventArgs(WmiDiskHelper.ExtractDiskID(NetDriveM.DriveInfo)));
        }
        public bool CanCreate
        {
            get
            {
                return true;
            }
        }


        private void DiagnosisExecute()
        {
            Log4.PrinterLogger.Info("[A][Tool Page]Press Diagnosis button.");
            NetworkDriveWrapper.ShowNetDriveDisconnectionDialog(App.Current.MainWindow);
            if (NetworkDriveMappingChangedEvent != null)
                NetworkDriveMappingChangedEvent(this, new NetworkDriveMappingChangedEventArgs(WmiDiskHelper.ExtractDiskID(NetDriveM.DriveInfo)));
        }
        public bool CanDiagnosis
        {
            get
            {
                return true;
            }
        }
    }
}
