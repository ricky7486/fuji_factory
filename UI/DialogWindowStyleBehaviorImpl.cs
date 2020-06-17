using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls.Primitives;
//using SPIBaseStructure;

using XStyle;
using System.Runtime.InteropServices;

namespace PrinterCenter
{
    /// <summary>
    /// 移植自SPIBaseStruture
    /// </summary>
    public class DPI
    {
        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hDc, int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

        public const int LOGPIXELSX = 88;
        public const int LOGPIXELSY = 90;

        public static double dpiX
        {
            get
            {
                try
                {
                    IntPtr hDc = GetDC(IntPtr.Zero);
                    if (hDc != IntPtr.Zero)
                    {
                        int dpiX = GetDeviceCaps(hDc, LOGPIXELSX);
                        ReleaseDC(IntPtr.Zero, hDc);
                        return dpiX;
                    }
                }
                catch (Exception ex)
                {
                    //TDebugger.Break();
                    //Log.Info("Exception catched in DPI::dpiX, reason = {0}", ex.Message);
                }
                return 96;
            }
        }

        public static double dpiY
        {
            get
            {
                try
                {
                    IntPtr hDc = GetDC(IntPtr.Zero);
                    if (hDc != IntPtr.Zero)
                    {
                        int dpiY = GetDeviceCaps(hDc, LOGPIXELSY);
                        ReleaseDC(IntPtr.Zero, hDc);
                        return dpiY;
                    }
                }
                catch (Exception ex)
                {
                    //TDebugger.Break();
                    //Log.Info("Exception catched in DPI::dpiY, reason = {0}", ex.Message);
                }
                return 96;
            }
        }

        static double _mx = -1;
        public static double MX
        {
            get
            {
                if (_mx < 0)
                    _mx = dpiX / 96;

                return _mx;
            }
        }

        static double _my = -1;
        public static double MY
        {
            get
            {
                if (_my < 0)
                    _my = dpiY / 96;

                return _my;
            }
        }


        /// <summary>
        /// Transforms device independent units (1/96 of an inch)
        /// to pixels
        /// </summary>
        /// <param name="unitX">a device independent unit value X</param>
        /// <param name="unitY">a device independent unit value Y</param>
        /// <param name="pixelX">returns the X value in pixels</param>
        /// <param name="pixelY">returns the Y value in pixels</param>
        public void TransformToPixels(double unitX,
                                      double unitY,
                                      out int pixelX,
                                      out int pixelY)
        {
            IntPtr hDc = GetDC(IntPtr.Zero);
            if (hDc != IntPtr.Zero)
            {
                int dpiX = GetDeviceCaps(hDc, LOGPIXELSX);
                int dpiY = GetDeviceCaps(hDc, LOGPIXELSY);

                ReleaseDC(IntPtr.Zero, hDc);

                pixelX = (int)(((double)dpiX / 96) * unitX);
                pixelY = (int)(((double)dpiY / 96) * unitY);
            }
            else
                throw new ArgumentNullException("Failed to get DC.");
        }



    }


    class DialogWindowStyleBehaviorImpl
    {
#if DEBUG_FAST_UI
        private static bool bShrinkOnLargeDPI = false;
#else
        private static bool bShrinkOnLargeDPI = false;
#endif
        public static void Init()
        {
            DialogWindowStyleBehavior.GridWindowRoot_Initialized = GridWindowRoot_Initialized;
            DialogWindowStyleBehavior.UpdateWindowConstraints = UpdateWindowConstraints;
            DialogWindowStyleBehavior.window_Closed = window_Closed;
            DialogWindowStyleBehavior.window_Deactivated = window_Deactivated;
            DialogWindowStyleBehavior.window_IsVisibleChanged = window_IsVisibleChanged;
            DialogWindowStyleBehavior.window_Loaded = window_Loaded;
        }

        /*static MainWindow TR7007iMainWin //區分TR7007i主程式，或者是其他的exe
        {
            get
            {
                if (!Process.GetCurrentProcess().ProcessName.Contains("TR7007i"))
                    return null;
                return MainWindow.Instance;
            }
        }*/
        static List<Window> OpenedDialogWindows = new List<Window>();
        static void window_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = sender as Window;
            if (window == null)
                return;

           

            if (/*PsudoDef.*/bShrinkOnLargeDPI && (DPI.MX > 1.05 || DPI.MY > 1.05))
            {
                Grid windowRoot = (Grid)window.Template.FindName("WindowRoot", window);
                if (windowRoot != null)
                {
                    windowRoot.LayoutTransform = new ScaleTransform(1 / DPI.MX, 1 / DPI.MY);
                    windowRoot.Margin = new Thickness(windowRoot.Margin.Left / DPI.MX,
                                                                                                    windowRoot.Margin.Top / DPI.MY,
                                                                                                    windowRoot.Margin.Right / DPI.MX,
                                                                                                    windowRoot.Margin.Bottom / DPI.MY);
                    if (window.SizeToContent == SizeToContent.Manual)
                    {
                        //以下勿放在GridWindowRoot_Initialized() 中，會造成此function一直被call，直到Width和Height變成0
                        window.Width /= DPI.MX;
                        window.Height /= DPI.MY;
                    }
                    if (window.WindowStartupLocation == WindowStartupLocation.Manual)
                    {
                        window.Left /= DPI.MX;
                        window.Top /= DPI.MY;
                    }
                }
            }
        }
        //移植從BaseClass
        private static bool IsModalWindow(Window window)
        {
            if (window == null)
                return false;

            Type type = typeof(Window);
            var field = type.GetField("_showingAsDialog", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return (bool)field.GetValue(window);
        }
        static void window_Deactivated(object sender, EventArgs e)
        {
            Window window = sender as Window;
            Border boundTop = (Border)window.Template.FindName("GroundBoundTop", window);
            if (/*BaseClass.*/IsModalWindow(window))
                boundTop.Background = Brushes.MistyRose;


            /*if (window is WinEasySpi)
                boundTop.Background = Brushes.Gray;*/
        }
        static void window_Closed(object sender, EventArgs e)
        {
            Window window = sender as Window;
            if (window != null)
            {
                TryLockUnlockMultiPanelSwitchingUI();

                if (OpenedDialogWindows.Contains(window))
                    OpenedDialogWindows.Remove(window);

                //prevent MainWindow sink below VS2010
                //PenNote: Don't put in Closing handler. It may cause Window class level hotkey failed to receive hotkey.
                //						for dual lane there are 2 screens and 2 keyboards, I don't want change the activated window.
                /*
                if (Debugger.IsAttached && !App._IsDualLane && OpenedDialogWindows.Count == 0)
                {
                    if (TR7007iMainWin != null && TR7007iMainWin.IsActive == false)
                        TR7007iMainWin.Activate();
                }
                */
                //Agent.GarbageCollect(); //Pen Marked Out : 放在 GridWindowRoot_Initialized 造成 Wizard Dialog 有時 Hang 住
                //Log.AddPhysicalMemoryUsage("Window_Closed({0})", window.Title);
            }
        }
        static void window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Window window = sender as Window;
            if (window != null)
            {
                if ((bool)e.OldValue == false && (bool)e.NewValue == true)
                {
                    //Agent.GarbageCollect(); //Pen Marked Out : 放在 GridWindowRoot_Initialized 造成 Wizard Dialog 有時 Hang 住
                    //Log.AddPhysicalMemoryUsage("Window_Show({0})", window.Title);
                }
                else if ((bool)e.OldValue == true && (bool)e.NewValue == false)
                {
                    //Agent.GarbageCollect(); //Pen Marked Out : 放在 GridWindowRoot_Initialized 造成 Wizard Dialog 有時 Hang 住
                    //Log.AddPhysicalMemoryUsage("Window_Hide({0})", window.Title);
                }
            }

            SetPanelNameToTitle();
            TryLockUnlockMultiPanelSwitchingUI();
        }

        private static void GridWindowRoot_Initialized(object sender, EventArgs e)
        {
            
            var window = (Window)((FrameworkElement)sender).TemplatedParent;

            if (window != null)
            {
                //Agent.GarbageCollect(); //Pen Marked Out : 放在 GridWindowRoot_Initialized 造成 Wizard Dialog 有時 Hang 住
                //Log.AddPhysicalMemoryUsage("WindowRoot_Initialized({0})", window.Title);

                //若不做以下處理，則SizeToContent.WidthAndHeight會造成視窗寬度和螢幕同寬。
                //		原因是為了達到平滑美觀的效果，在xaml中，GroundBoundMiddle的Stretch是設為Fill。

                if (window.SizeToContent == SizeToContent.WidthAndHeight)
                {
                    Path DecorationPath = (Path)window.Template.FindName("GroundBoundMiddle", window);
                    DecorationPath.Visibility = Visibility.Collapsed;//如此則會fit to content的寬度，不會變成與螢幕同寬。
                }

                if (!OpenedDialogWindows.Contains(window))
                    OpenedDialogWindows.Add(window);

                TryLockUnlockMultiPanelSwitchingUI();

                TextOptions.SetTextFormattingMode(window, App._TextFormattingMode);

                //PenNote: 用Dispatcher避免Window_Loaded中呼叫MessageBox造成當機 (WPF的Bug)
                //先決條件是此Window同時滿足以下三點：
                //1. 使用 Style="{StaticResource DialogWindowStyle}"
                //2. SizeToContent="WidthAndHeight"
                //3. 在此處直接設 window.ShowInTaskbar，沒有用Dispatcher
                //PenNote: 此處不設Priority，因為設成Loaded或Input優先權太低，window出現時會閃爍
                window.Dispatcher.BeginInvoke(new Action(() =>
                {
                    window.ShowInTaskbar = false;
                    if (window == Application.Current.MainWindow)
                        window.ShowInTaskbar = true;
                }));

                window.Loaded += window_Loaded;
                window.Activated += new EventHandler(window_Activated);
                window.Deactivated += new EventHandler(window_Deactivated);
                window.IsVisibleChanged += new DependencyPropertyChangedEventHandler(window_IsVisibleChanged);
                //window.Closing += new System.ComponentModel.CancelEventHandler(Window_Closing);
                window.Closed += new EventHandler(window_Closed);
                Image titleIconImage = (Image)window.Template.FindName("TitleIconImage", window);
                Grid titleLaneIndicator = (Grid)window.Template.FindName("TitleLaneIndicator", window);
                if (titleIconImage != null)
                    titleIconImage.Visibility = Visibility.Visible;
                if (titleLaneIndicator != null)
                    titleLaneIndicator.Visibility = Visibility.Hidden;


                if (App._IsDualLane)
                {
                    TextBlock laneIndicatorText = (TextBlock)window.Template.FindName("LaneIndicatorText", window);
                    /*if (App.Instance._assignedLane == eAssignedLane.Lane1)
                    {
                        laneIndicatorText.Text = "1";
                    }
                    else if (App.Instance._assignedLane == eAssignedLane.Lane2)
                    {
                        laneIndicatorText.Text = "2";
                    }*/
                    if (titleIconImage != null)
                        titleIconImage.Visibility = Visibility.Hidden;
                    if (titleLaneIndicator != null)
                        titleLaneIndicator.Visibility = Visibility.Visible;
                }

                /*if (TR7007iMainWin != null && TR7007iMainWin.multiPanelVM != null)
                {
                    TR7007iMainWin.multiPanelVM.CurrentPanelChanged += new EventHandler(OnCurrentPanelChanged);
                    window.Unloaded += new RoutedEventHandler((object objU, RoutedEventArgs argU) =>
                    {
                        TR7007iMainWin.multiPanelVM.CurrentPanelChanged -= new EventHandler(OnCurrentPanelChanged);
                    });
                    SetPanelNameToTitle();
                }*/
            } //window != null
        }

        /// <summary>
        /// Updates the window constraints based on its state.
        /// For instance, the max width and height of the window is set to prevent overlapping over the taskbar.
        /// </summary>
        /// <param name="window">Window to set properties</param>
        private static void UpdateWindowConstraints(Window window)
        {
            if (window != null)
            {
                // Make sure we don't bump the max width and height of the desktop when maximized
                GridLength borderWidth = (GridLength)window.FindResource("BorderWidth");

                if (/*PsudoDef.*/bShrinkOnLargeDPI && (DPI.MX > 1.05 || DPI.MY > 1.05))
                {
                    window.MaxWidth = SystemParameters.WorkArea.Width + borderWidth.Value * 2 / DPI.MX; ;
                    window.MaxHeight = SystemParameters.WorkArea.Height + (borderWidth.Value * 2 + 200) / DPI.MX;
                }
                else
                {
                    window.MaxWidth = SystemParameters.WorkArea.Width + borderWidth.Value * 2;
                    window.MaxHeight = SystemParameters.WorkArea.Height + borderWidth.Value * 2 + 200;
                }
            }
    
        }

        static void window_Activated(object sender, EventArgs e)
        {
            Window window = sender as Window;
            Border boundTop = (Border)window.Template.FindName("GroundBoundTop", window);
     
            if (/*BaseClass.*/IsModalWindow(window))
                boundTop.Background = Brushes.Salmon;
   
                

            /*if (window is WinEasySpi)
            {
                if (App.Instance._assignedLane == eAssignedLane.Lane1)
                    boundTop.Background = Brushes.DeepSkyBlue;
                else
                    boundTop.Background = Brushes.OliveDrab;
            }*/
        }

        static void OnCurrentPanelChanged(object sender, EventArgs e)
        {
            SetPanelNameToTitle();
        }

        static void SetPanelNameToTitle()
        {
            /*foreach (Window window in OpenedDialogWindows)
            {
                TextBlock tbPanelName = (TextBlock)window.Template.FindName("PanelName", window);
                if (tbPanelName != null)
                {
                    if (TR7007iMainWin != null && TR7007iMainWin.multiPanelVM != null
                            && TR7007iMainWin.multiPanelVM.CurrentPanel != null)
                    {
                        string sPanelName = TR7007iMainWin.multiPanelVM.CurrentPanel.Name;

                        // give user a chance to set his own PanelName when necessary
                        PropertyInfo prop = window.GetType().GetProperty("MyPanelName");
                        if (prop != null)
                        {
                            string myPanelName = prop.GetValue(window, null) as string;
                            if (myPanelName != null && myPanelName != "")
                                sPanelName = myPanelName;
                        }

                        if (Param.IsMultiPanel)
                            tbPanelName.Text = String.Format("  <{0}>", sPanelName);
                        else
                            tbPanelName.Text = "";
                    }
                    else
                    {
                        tbPanelName.Text = "";
                    }
                }
            }*/
        }

        static void TryLockUnlockMultiPanelSwitchingUI()
        {
            /*if (TR7007iMainWin != null && TR7007iMainWin.multiPanelVM != null && TR7007iMainWin.cbPanels != null)
            {
                bool bLock = false;
                foreach (Window window in App.Instance.Windows)
                {
                    if (window != TR7007iMainWin && window.IsVisible)
                    {
                        bLock = true;
                        break;
                    }
                }
                if (bLock)
                    TR7007iMainWin.cbPanels.IsEnabled = false;
                else
                    TR7007iMainWin.cbPanels.IsEnabled = true; //will validate with ui stage in the IsEnableChanged handler			
            }*/
        }
    }

    class WindowBehaviorImpl
    {
        /*
        public static ToggleButton CreateWatchButton_InAuxHost1(Window window)
        {
            if (window == null)
                //TDebugger.Break();

            ToggleButton tgButton = new ToggleButton();
            tgButton.Width = 25;
            tgButton.Height = 25;
            tgButton.SetResourceReference(Control.StyleProperty, "WatchToggleButton");

            // show-hide
            PropertyInfo prop = window.GetType().GetProperty("bAlwaysOpenOnVerifying");
            if (prop != null)
            {
                bool bWatchIt = (bool)prop.GetValue(window, null);
                tgButton.IsChecked = bWatchIt;
            }

            // add to host
            Grid gridHost = (Grid)window.Template.FindName("AuxHost1", window);
            if (gridHost != null)
                gridHost.Children.Add(tgButton);
            else
                //TDebugger.Break();

            return tgButton;
        }
        */
        public static void ShowMinimizeButton(Window window)
        {
            Button MinimizeButton = (Button)window.Template.FindName("ButtonMinimize", window);
            if (MinimizeButton != null)
                MinimizeButton.Visibility = Visibility.Visible;
        }
    }
}
