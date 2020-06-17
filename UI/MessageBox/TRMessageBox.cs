using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System;
using System.Reflection;
using System.IO;


namespace PrinterCenter.UI
{
	public static class TRMessageBox
	{
        public static MessageBoxResult Show(string messageBoxText, bool forceShowMessageBoxInUIThread = true)
        {
			return ShowCore(null, messageBoxText, forceShowMessageBoxInUIThread:forceShowMessageBoxInUIThread);
		}

		public static MessageBoxResult Show(string messageBoxText, string caption)
		{
            return ShowCore(null, messageBoxText, caption);
		}

		public static MessageBoxResult Show(Window owner, string messageBoxText)
		{
			return ShowCore(owner, messageBoxText);
		}

		public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
		{
			return ShowCore(null, messageBoxText, caption, button);
		}

		public static MessageBoxResult Show(Window owner, string messageBoxText, string caption)
		{
			return ShowCore(owner, messageBoxText, caption);
		}

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, bool forceShowMessageBoxInUIThread = false)
		{
            return ShowCore(null, messageBoxText, caption, button, icon, forceShowMessageBoxInUIThread:forceShowMessageBoxInUIThread);
		}

		public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button)
		{
			return ShowCore(owner, messageBoxText, caption, button);
		}

		public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
		{
			return ShowCore(null, messageBoxText, caption, button, icon, defaultResult);
		}

		public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
		{
			return ShowCore(owner, messageBoxText, caption, button, icon);
		}

		//public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
		//{
		//    return ShowCore(null, messageBoxText, caption, button, icon, defaultResult, options);
		//}

		//public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
		//{
		//    return ShowCore(owner, messageBoxText, caption, button, icon, defaultResult);
		//}

		//public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
		//{
		//    return ShowCore(owner, messageBoxText, caption, button, icon, defaultResult, options);
		//}

		private static MessageBoxResult ShowCore(
			Window owner,
			string messageBoxText,
			string caption = "TR7007i",
			MessageBoxButton button = MessageBoxButton.OK,
			MessageBoxImage icon = MessageBoxImage.None,
			MessageBoxResult defaultResult = MessageBoxResult.None,
			MessageBoxOptions options = MessageBoxOptions.None,
            bool forceShowMessageBoxInUIThread = false)
		{     
			try
			{
				//Log.Info("Enter TRMessageBox.ShowCore({0})", messageBoxText);

				MessageBoxResult result = MessageBoxResult.None;
                if (Application.Current.Dispatcher.CheckAccess()) //the calling thread is the thread associated with this Dispatcher
                {
                    result = TRMessageBoxWindow.Show(owner, messageBoxText, caption, button, icon, defaultResult, options);
                }
                else if (forceShowMessageBoxInUIThread)
                {
					return (MessageBoxResult)Agent.AppInvoke(
                            new System.Func<MessageBoxResult>(() =>
                            {
                                return TRMessageBoxWindow.Show(owner, messageBoxText, caption, button, icon, defaultResult, options);
                            }));
                }
                else
                {
                    //PenNote: 如果在Task裡面使用下面的 Invoke，會和 Task.Wait()造成DeadLock，
                    //      Multi-Thread也同樣有此潛在問題，故用MessageBox取代Invoke，以防萬一
                    result = MessageBox.Show(messageBoxText, caption, button, icon, defaultResult, options);
                }

				//string msg = messageBoxText.Replace("\r\n", " ");
				//msg = msg.Replace("\n", "");
				//L4.msg.InfoFormat("{0},{1},{2},{3}", Path.GetFileName(Assembly.GetEntryAssembly().Location),
				//											caption, msg.Trim(), result.ToString());
				return result;
			}
			catch (Exception ex)//If a window has never shown, assign MsgBox owner to it throws an exception.
			{
				//Log.Info("Exception catched in TRMessageBox.ShowCore(...). Reason = {0}", ex.Message);
				return MessageBox.Show(messageBoxText, caption, button, icon, defaultResult, options);
			}
			finally
			{
				//Log.Info("Leave TRMessageBox.ShowCore(...)");
			}
		}
		
				

	}
}

