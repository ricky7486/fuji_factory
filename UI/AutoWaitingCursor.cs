using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;

namespace PrinterCenter.UI
{
    public class AutoWaitingCursor : IDisposable
    {
        bool IsSTA // if calling from C++ COM or background thread not in STA, skip all code in this class, otherwise it will crash
        {
            get { return Thread.CurrentThread.GetApartmentState() == ApartmentState.STA; }
        }
        Dispatcher _uiDispatcher = null;
        private static int _waitingCount;

        /// <summary>
        /// 僅支援UI Thread呼叫
        /// </summary>
        public AutoWaitingCursor()
        {
            try
            {
                if (IsSTA)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    _waitingCount++;
                }
                else
                {
                    //TDebugger.Break();
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex.ToString());
                //TDebugger.Break();
            }
        }

        /// <summary>
        /// 提供給Background Thread呼叫
        /// </summary>
        /// <param name="uiDispatcherObj"></param>
        public AutoWaitingCursor(Dispatcher uiThreadDispatcher)
        {
            try
            {
                _uiDispatcher = uiThreadDispatcher;

                _uiDispatcher.BeginInvoke(new Action(() =>
                { Mouse.OverrideCursor = Cursors.Wait; }));

                _waitingCount++;
            }
            catch (Exception ex)
            {
                //Log.Error(ex.ToString());
                //TDebugger.Break();
            }
        }

        public void Dispose()
        {
            try
            {
                _waitingCount--;

                if (_waitingCount > 0) return;
                if (_waitingCount < 0)
                    //TDebugger.Break("Waiting count should not below zero");

                if (_uiDispatcher != null) //call from background thread
                {
                    _uiDispatcher.BeginInvoke(new Action(() =>
                    {
                        Mouse.OverrideCursor = null;
                        //TRMessageBoxWindow.prevCursor = null;
                    }));
                }
                else if (IsSTA) //call from UI thread
                {
                    Mouse.OverrideCursor = null;
                }
            }
            catch
            {
                //PenNote: 如果主程式正在開啟時，按Close Button關閉，有機會走到這裡
            }
        }

    }
}
