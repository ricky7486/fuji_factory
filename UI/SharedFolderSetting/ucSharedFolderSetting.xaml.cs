using aejw.Network;
using System;
using System.Collections.Generic;
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

namespace PrinterCenter.UI.SharedFolderSetting
{
    /// <summary>
    /// Interaction logic for ucSharedFolderSetting.xaml
    /// </summary>
    public partial class ucSharedFolderSetting : UserControl
    {
        public ucSharedFolderSetting()
        {
            InitializeComponent();       
        }
        //外部(Mainwindow)內的分頁改變了磁碟映射後，需要刷新物件的可用選擇
        //提供PrinterWindow呼叫
        public void RefreshLocalDrives()
        {
            var vm = this.DataContext as SharedFolderSettingVM;
            vm.InSharedFolder = WmiDiskHelper.GetDiskNames().ToObservableCollection();
            vm.OutSharedFolder= WmiDiskHelper.GetDiskNames().ToObservableCollection();
        }


    }
}
