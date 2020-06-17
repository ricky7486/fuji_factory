
using PrinterCenter.Localization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace PrinterCenter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly TextFormattingMode _TextFormattingMode = TextFormattingMode.Ideal;
        public static bool _IsDualLane;
        public static PrinterWindow _main;
        public App()
        {
            DialogWindowStyleBehaviorImpl.Init();

        }
      
    }
}
