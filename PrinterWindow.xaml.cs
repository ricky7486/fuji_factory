using aejw.Network;
using PrinterCenter.Localization;
using PrinterCenter.Log;
using PrinterCenter.Service;
using PrinterCenter.UI;
using PrinterCenter.UI.CommonSetting;
using PrinterCenter.UI.OneLaneSelector;
using PrinterCenter.UI.SharedFolderSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace PrinterCenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class PrinterWindow : Window
    {
		private bool IsClosingPrompt = false;
		private string BindingClosing = String.Empty;

        public PrinterWindow()
        {
            MultiLanguageHelper.SettingLanguageFromRegisty();
            InitializeComponent();

            NetworkDriveViewModel ndvm = (NetworkDriveViewModel)ucNetworkDrive.DataContext;
            ndvm.NetworkDriveMappingChangedEvent += new NetworkDriveMappingChangedHandler(NetworkDrive_Changed);

            OneLaneSelectorVM.VendorChangedEvent += new VendorChangedHandler(Vendor_Changed);

			using (var ini = new IniFile())
			{
				bool PrinterClosing = Boolean.TryParse(ini.Read("Printer", "ClosingPrompt"), out IsClosingPrompt);
				bool bAlreadyExist = IsProcessAlreadyExist("PrinterCenter");//PrinterCenter
				if (bAlreadyExist)
				{
					MessageBoxResult ret = TRMessageBox.Show(this,
							"@SERVICE_OPENED".Translate(),
							"@PRINTER_CENTER".Translate(),
							MessageBoxButton.OK,
							MessageBoxImage.Stop);
					if (ret == MessageBoxResult.OK)
					{
						IsClosingPrompt = false;
						System.Windows.Application.Current.Shutdown();
					}
				}
				BindingClosing = ini.Read("Printer", "BindingClosing");
			}

            //Auto Load Settings
            if (System.IO.File.Exists("PrinterCenter.xml"))
            {
                Log4.PrinterLogger.InfoFormat("*AutoLoading \"PrinterCenter.xml\" ...");
                ViewModelLocator.Atom.PrinterWindowVM.LoadSettingFromXml("PrinterCenter.xml");
                if (ViewModelLocator.Atom.PrinterWindowVM.ExamineData())
                    ViewModelLocator.Atom.PrinterWindowVM.ExecuteOpenService();
                else
                {
                    ViewModelLocator.Atom.PrinterWindowVM.IsAutoLoadFile = true;
                    
                    Log4.PrinterLogger.InfoFormat("Examining Fail ...");
                }
                ViewModelLocator.Atom.PrinterWindowVM.stcSelectedIndex = 6;
            }

        }

		private bool IsProcessAlreadyExist(string ProcessName)
		{
			//只卡Printer.exe即可，Printer.vshost.exe只有在開發下會碰到
			Process[] ps = Process.GetProcessesByName(ProcessName);
			Log4.PrinterLogger.InfoFormat("PrinterCenter.exe ={0}", ps.Length);
			if (ps.Length > 1 /*|| psvshost.Length > 1*/)
				return true;
			else
				return false;
		}

		private bool IsProcessExist(string ProcessName)
		{
			Process[] ps = Process.GetProcessesByName(ProcessName);
			if (ps.Length == 0)//not exist
				return false;
			else
				return true;
		}

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
			if (IsProcessExist(BindingClosing)){//表示ini有設定bindingClose的app
				e.Cancel = true;
			}

			if (IsClosingPrompt)
			{
				MessageBoxResult ret = TRMessageBox.Show(this,
						"@ARE_YOU_SURE_TO_CLOSE_WINDOW".Translate(),
						"@CONFIRM".Translate(),
						MessageBoxButton.YesNo,
						MessageBoxImage.Question);
				if (ret == MessageBoxResult.No)
					e.Cancel = true;
			}

			if (!e.Cancel)
			{
				App.Current.Shutdown();
			}

			//if (IsClosingPrompt)
			//{
			//    MessageBoxResult ret = TRMessageBox.Show(this,
			//            "@ARE_YOU_SURE_TO_CLOSE_WINDOW".Translate(),
			//            "@CONFIRM".Translate(),
			//            MessageBoxButton.YesNo,
			//            MessageBoxImage.Question);
			//    if (ret == MessageBoxResult.No)
			//        e.Cancel = true;
			//}
			//if (IsProcessExist(BindingClosing))//表示ini有設定bindingClose的app
			//    e.Cancel = true;
			//else
			//    App.Current.Shutdown();

            Log4.PrinterLogger.InfoFormat(" ================== Closing ================== ");
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowBehaviorImpl.ShowMinimizeButton(this);
            Log4.PrinterLogger.InfoFormat(" ================== Opening ================== ");
        }

        private void NetworkDrive_Changed(object sender, NetworkDriveMappingChangedEventArgs e)
        {

            ucLane1_SF.RefreshLocalDrives();
            ucLane2_SF.RefreshLocalDrives();

        }

        private void Vendor_Changed(object sender, VendorChangedEventArgs e)
        {
            switch(e.LaneName)
            {
                case 1:
                    ((SharedFolderSettingVM)ucLane1_SF.DataContext).UpdateVisibility(e.Vendor);
                    break;
                case 2:
                    ((SharedFolderSettingVM)ucLane2_SF.DataContext).UpdateVisibility(e.Vendor);
                    break;
                default:
                    break;

            }
        }


    }
}
