using aejw.Network;
using PrinterCenter.Localization;
using PrinterCenter.Log;
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

namespace PrinterCenter.UI.OneLaneSelector
{
    /// <summary>
    /// Interaction logic for ucOneLaneSetting3.xaml
    /// </summary>
    public partial class ucOneLaneSelector : UserControl
    {

        //工外部初始的ctor
        public ucOneLaneSelector(int LaneID)
        {
            InitializeComponent();
 
            OneLaneSelectorVM vm = this.DataContext as OneLaneSelectorVM;
            vm.LaneName = LaneID + 1;
            vm.LaneTitle = String.Format("@LANE".Translate() + ":", vm.LaneName);
        }
        public ucOneLaneSelector(int LaneID, OneLaneSelectorVM savedfile)
        {
            InitializeComponent();
            OneLaneSelectorVM vm = this.DataContext as OneLaneSelectorVM;
            vm.LaneName = LaneID + 1;
            vm.Vendor = savedfile.Vendor;

            vm.LaneTitle = String.Format("@LANE".Translate() + ":", vm.LaneName);
        }

    }
}
