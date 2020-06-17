using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace PrinterCenter.UI
{
    public static class Agent
    {
        static Random rand = new Random();
        public static void DoEventsWinForm()
        {
            try
            {
                //Log.AddStackSkipSelf("Enter Agent::DoEventsWinForm()");

                System.Windows.Forms.Application.DoEvents();
            }
            catch (Exception ex)
            {
                //Log.Info("Exception catched in Agent::DoEventsWinForm(), Reason={0}", ex.Message);
                throw ex;
            }
            finally
            {
               // Log.Info(" Leave Agent::DoEventsWinForm()");
            }
        }

        public static object AppInvoke(DispatcherPriority priority, Delegate method)
        {
            int id = rand.Next(9999);
            try
            {
                //Log.AddStackSkipSelf("Enter Agent::AppInvoke({0}), id={1}", priority, id);
                return Application.Current.Dispatcher.Invoke(priority, method);
            }
            catch (Exception ex)
            {
                //Log.Info("Exception catched in Agent::AppInvoke(), id={0}, Reason={1}", id, ex.Message);
                throw ex;
            }
            finally
            {
                //Log.Info(" Leave Agent::AppInvoke(), id={0}", id);
            }
        }

        public static object AppInvoke(DispatcherPriority priority, Delegate method, object arg, params object[] args)
        {
            int id = rand.Next(9999);
            try
            {
                //Log.AddStackSkipSelf("Enter Agent::AppInvoke({0}), id={1}", priority, id);
                return Application.Current.Dispatcher.Invoke(priority, method, arg, args);
            }
            catch (Exception ex)
            {
                //Log.Info("Exception catched in Agent::AppInvoke(), id={0}, Reason={1}", id, ex.Message);
                throw ex;
            }
            finally
            {
                //Log.Info(" Leave Agent::AppInvoke(), id={0}", id);
            }
        }

        public static object AppInvoke(Delegate method, params object[] args)
        {
            int id = rand.Next(9999);
            try
            {
                //Log.AddStackSkipSelf("Enter Agent::AppInvoke(priority not assigned), id={0}", id);
                return Application.Current.Dispatcher.Invoke(method, args);
            }
            catch (Exception ex)
            {
                //Log.Info("Exception catched in Agent::AppInvoke(), id={0}, Reason={1}", id, ex.Message);
                throw ex;
            }
            finally
            {
                //Log.Info(" Leave Agent::AppInvoke(), id={0}", id);
            }
        }

        public static void GarbageCollect(bool bWaitForPendingFinalizers = false)
        {


            int id = rand.Next(9999);
            try
            {
                //Log.AddStackSkipSelf("Enter Agent::GarbageCollect(), id={0}", id);

                //using (AutoStopwatch a = new AutoStopwatch("Agent.GarbageCollect(bWaitForPendingFinalizers: {0})", bWaitForPendingFinalizers))
                //{


                    GC.Collect();
                    if (bWaitForPendingFinalizers)
                        GC.WaitForPendingFinalizers();

                //}

            }
            catch (Exception ex)
            {
                //Log.Info("Exception catched in Agent::GarbageCollect(), id={0}, Reason={1}", id, ex.Message);
                throw ex;
            }
            finally
            {
                //Log.Info(" Leave Agent::GarbageCollect(), id={0}", id);
            }
        }

    }
}
