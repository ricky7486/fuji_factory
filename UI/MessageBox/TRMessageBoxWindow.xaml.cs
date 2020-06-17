using System;
using System.ComponentModel;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Threading;


namespace PrinterCenter.UI
{
	/// <summary>
	/// Interaction logic for TRMessageBoxWindow.xaml
	/// </summary>
	public partial class TRMessageBoxWindow : Window
	{
		private TRMessageBoxWindow()
		{
			InitializeComponent();
			Closing += new CancelEventHandler(TRMessageBoxWindow_Closing);
			Loaded += new RoutedEventHandler(TRMessageBoxWindow_Loaded);
		}

		private const int GWL_STYLE = -16;
		private const int WS_SYSMENU = 0x80000;
		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
		void TRMessageBoxWindow_Loaded(object sender, RoutedEventArgs e)
		{
			//disable close button
			var hwnd = new WindowInteropHelper(this).Handle;
			SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
		}

		void TRMessageBoxWindow_Closing(object sender, CancelEventArgs e)
		{
			//Don't close window without a result 
			if (_viewModel != null &&_viewModel.Result == MessageBoxResult.None)
			{
				e.Cancel = true;
				return;
			}

			//Add other house keeping code here:


		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			//hide the close button
			var control = this.Template.FindName("ButtonClose", this) as UIElement;
			if (control != null)
			{
				control.Visibility = Visibility.Hidden;
			}
		}

		private MessageBoxViewModel _viewModel;
		[ThreadStatic]
		private static TRMessageBoxWindow _msgWindow;

        public static Cursor prevCursor;

        public static MessageBoxResult Show(
            Window owner,
            string messageBoxText,
            string caption,
            MessageBoxButton button,
            MessageBoxImage icon,
            MessageBoxResult defaultResult,
            MessageBoxOptions options)
        {
            if ((options & MessageBoxOptions.DefaultDesktopOnly) == MessageBoxOptions.DefaultDesktopOnly)
            {
                throw new NotImplementedException();
            }

            if ((options & MessageBoxOptions.ServiceNotification) == MessageBoxOptions.ServiceNotification)
            {
                throw new NotImplementedException();
            }

            //加這行的原因，請見下面的PenNote註解
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
            //PenNote: 因為_msgWindow是static，以下的psudo code會造成在第二個TRMessageBox中的dispatcher queue裡面，
            //      叫出第一個BeginInvoke裡面的TRMessageBox。而第一個TRMessageBox的結果，會覆蓋掉第二個的結果， 
            //      (因為static物件被改掉了, see  _msgWindow = new TRMessageBoxWindow())，因而產生bug。
            //public static void DoAbout() //--> to test, put it in CoreCommand.cs
            //{
            //    Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            //    {
            //        TRMessageBox.Show("1:OnlyShow");
            //    }));
            //    var result = TRMessageBox.Show("2:YesOrNo", "caption", MessageBoxButton.YesNo);
            //    return;
            //}

            _msgWindow = new TRMessageBoxWindow();                   
            if (Application.Current.MainWindow == _msgWindow)
                Application.Current.MainWindow = null; //不這樣設，之後的MainWindow秀不出來。(Test case:  App.xaml.cs在create MainWindow之前，呼叫TRMessageBox)
            if (owner == null)
                owner = Application.Current.MainWindow;
			try
			{
				_msgWindow.Owner = owner;   //需避免把owner指定為自己，會crash。（尤其自己是MainWindow的時候，在前面已防掉)
			}
			catch (Exception ex) //e.g. assign owner to a closed window.
			{
				//Log.Info("Exception catched in TRMessageBoxWindow.Show() #_msgWindow.Owner = owner #. Reason = {0}", ex.Message); 
				_msgWindow.Owner = Application.Current.MainWindow;
			}
			_msgWindow._viewModel = new MessageBoxViewModel(_msgWindow, caption, messageBoxText, button, icon, defaultResult, options);
			_msgWindow.DataContext = _msgWindow._viewModel;
            _msgWindow.Topmost = true; //always set it to topmost, otherwise if may obstacled by other topmost window.

            prevCursor = null;
            try
            {
                prevCursor = Mouse.OverrideCursor;
				Mouse.OverrideCursor = Cursors.Arrow;	
            }
            catch (Exception ex)
            {
                //Log.Error("Exception catched in TRMessageBoxWindow.Show() on setting cursor. Reason={0}", ex.Message);
            }

            _msgWindow.ShowDialog();

            try
            {
				Mouse.OverrideCursor = prevCursor;
            }
            catch (Exception ex)
            {
                //Log.Error("Exception catched in TRMessageBoxWindow.Show() on setting cursor. Reason={0}", ex.Message);
            }

            return _msgWindow._viewModel.Result;
		}

	}
}
