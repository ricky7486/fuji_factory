using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using PrinterCenter.Log;
using PrinterCenterData;
using IPCData;

namespace PrinterCenter.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name
    //       "PrinterDuplexService" in both code and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Reentrant, IncludeExceptionDetailInFaults = true)]
    public class PrinterDuplexService : IPrinterDuplexService
    {
        //回乎通道  雙向溝通
        public IPrinterDuplexServiceCallback _Callback;

        public PrinterWindow _window = null;

        public PrinterDuplexService()
        {
            _window = (PrinterWindow)App.Current.MainWindow;
            _Callback = OperationContext.Current.GetCallbackChannel<IPrinterDuplexServiceCallback>();
            PrinterManager.getInstance().PrinterDuplexServiceInstance = this;
        }

        public IPrinterDuplexServiceCallback Callback
        {
            get
            {
                return _Callback;
            }
        }

        /// <summary>
        /// Fuji Changover
        /// </summary>
        /// <param name="Lane">The lane.</param>
        /// <returns></returns>
        public bool CheckCurrentSharedFolder(eAssignedLane_Printer Lane)
        {
            if (PrinterManager.getInstance() == null)
                return false;
            else
            {
                Log4.PrinterLogger.InfoFormat("[SOA]CheckCurrentSharedFolder({0})", Lane.ToString());
                switch (Lane)
                {
                    case eAssignedLane_Printer.None:
                        return false;
                    case eAssignedLane_Printer.Lane1:
                        return PrinterManager.getInstance().RemotePrinter[0].Printer.CheckCurrentSharedFolder();
                    case eAssignedLane_Printer.Lane2:
                        return PrinterManager.getInstance().RemotePrinter[1].Printer.CheckCurrentSharedFolder();
                }
                return false;
            }
        }

        public ePrinterVendor GetPrinterCurrentVendor(eAssignedLane_Printer Lane)
        {
            Log4.PrinterLogger.InfoFormat("[SOA]GetPrinterCurrentVendor({0})", Lane.ToString());
            return PrinterManager.getInstance().RemotePrinter[(int)Lane - 1].Printer.Vendor;
        }

        public string GetWriteCompImagePath(eAssignedLane_Printer Lane)
        {
            if (PrinterManager.getInstance() == null)
                return string.Empty;
            else
            {
                Log4.PrinterLogger.InfoFormat("[SOA]GetWriteCompImagePath({0})", Lane.ToString());
                switch (Lane)
                {
                    case eAssignedLane_Printer.None:
                        return string.Empty;
                    case eAssignedLane_Printer.Lane1:
                        return PrinterManager.getInstance().RemotePrinter[0].Printer.GetWriteCompImagePath();
                    case eAssignedLane_Printer.Lane2:
                        return PrinterManager.getInstance().RemotePrinter[1].Printer.GetWriteCompImagePath();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Fuji EasyLink
        /// </summary>
        /// <param name="Lane">The lane.</param>
        /// <returns>
        /// <c>true</c> if [is need write comp image] [the specified lane]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNeedWriteCompImage(eAssignedLane_Printer Lane)
        {
            if (PrinterManager.getInstance() == null)
                return false;
            else
            {
                Log4.PrinterLogger.InfoFormat("[SOA]IsNeedWriteCompImage({0})", Lane.ToString());
                switch (Lane)
                {
                    case eAssignedLane_Printer.None:
                        return false;
                    case eAssignedLane_Printer.Lane1:
                        return PrinterManager.getInstance().RemotePrinter[0].Printer.IsNeedWriteCompImage();
                    case eAssignedLane_Printer.Lane2:
                        return PrinterManager.getInstance().RemotePrinter[1].Printer.IsNeedWriteCompImage();
                }
                return false;
            }
        }

        /// <summary>
        /// Fuji Changeover
        /// </summary>
        /// <param name="Lane">The lane.</param>
        /// <returns></returns>
        public bool MoveToNextSharedFolder(eAssignedLane_Printer Lane)
        {
            if (PrinterManager.getInstance() == null)
                return false;
            else
            {
                Log4.PrinterLogger.InfoFormat("[SOA]MoveToNextSharedFolder({0})", Lane.ToString());
                switch (Lane)
                {
                    case eAssignedLane_Printer.None:
                        return false;
                    case eAssignedLane_Printer.Lane1:
                        return PrinterManager.getInstance().RemotePrinter[0].Printer.MoveToNextSharedFolder();
                    case eAssignedLane_Printer.Lane2:
                        return PrinterManager.getInstance().RemotePrinter[1].Printer.MoveToNextSharedFolder();
                }
                return false;
            }
        }

        /// <summary>
        /// Fuji Changover
        /// </summary>
        /// <param name="Lane">The lane.</param>
        /// <returns></returns>
		//Fuji Changover
		//public bool NeedCreateTheDataToNext(eAssignedLane_Printer Lane, int size)   
		//{
		//    if (PrinterManager.getInstance() == null)
		//        return false;
		//    else
		//    {
		//        InspectedPanel panel = new InspectedPanel();
		//        try
		//        {
		//            #region MMF

		//            using (var mmf = MemoryMappedFile.CreateOrOpen("PrinterCenter_" + Lane.ToString(), size, MemoryMappedFileAccess.ReadWrite))
		//            {
		//                using (var mmvStream = mmf.CreateViewStream(0, size))
		//                {
		//                    BinaryFormatter binFormatter = new BinaryFormatter();
		//                    byte[] buffer = new byte[size];
		//                    mmvStream.Read(buffer, 0, size);
		//                    panel = (InspectedPanel)binFormatter.Deserialize(new MemoryStream(buffer));
		//                }
		//            }

		//            #endregion MMF
		//        }
		//        catch (Exception e)
		//        {
		//            Log4.PrinterLogger.ErrorFormat("NeedCreate MMF Exception");
		//            Log4.PrinterLogger.ErrorFormat(e.Message);
		//            return false;
		//        }

		//        Log4.PrinterLogger.InfoFormat("NeedCreateTheDataToNext({0},{1})", Lane.ToString(), size);

		//        switch (Lane)
		//        {
		//            case eAssignedLane_Printer.None:
		//                break;

		//            case eAssignedLane_Printer.Lane1:
		//                ViewModelLocator.Atom.FlowHostVM.spiInspectedData.Add("[" + eAssignedLane_Printer.Lane1.ToString() + "]" + panel.InspectStartTime);
		//                Log4.PrinterLogger.InfoFormat(" -Push [{0}] NeedCreate CurrentInspectModeTemp.InspectMode={1} .InspectResult={2}", Lane.ToString(), panel.InspectMode, panel.InspectResult);
		//                PrinterManager.getInstance().CurrentInspectModeTemp[0].InspectMode = panel.InspectMode;
		//                PrinterManager.getInstance().CurrentInspectModeTemp[0].InspectResult = panel.InspectResult;
		//                Log4.PrinterLogger.InfoFormat(" -Enqueue [{0}] NeedCreate SPI Data InspectTime = {1}", Lane.ToString(), panel.InspectStartTime);
		//                PrinterManager.getInstance().RemotePrinter[0].Printer.InspectedPanels.Enqueue(panel);
		//                //PrinterManager.getInstance().RemotePrinter[0].Printer.StartProcess();
		//                break;

		//            case eAssignedLane_Printer.Lane2:
		//                ViewModelLocator.Atom.FlowHostVM.spiInspectedData.Add("[" + eAssignedLane_Printer.Lane2.ToString() + "]" + panel.InspectStartTime);
		//                Log4.PrinterLogger.InfoFormat(" -Push [{0}] NeedCreate CurrentInspectModeTemp.InspectMode={1} .InspectResult={2}", Lane.ToString(), panel.InspectMode, panel.InspectResult);
		//                PrinterManager.getInstance().CurrentInspectModeTemp[1].InspectMode = panel.InspectMode;
		//                PrinterManager.getInstance().CurrentInspectModeTemp[1].InspectResult = panel.InspectResult;
		//                Log4.PrinterLogger.InfoFormat(" -Enqueue [{0}] NeedCreate SPI Data InspectTime = {1}", Lane.ToString(), panel.InspectStartTime);
		//                PrinterManager.getInstance().RemotePrinter[1].Printer.InspectedPanels.Enqueue(panel);
		//                //PrinterManager.getInstance().RemotePrinter[1].Printer.StartProcess();
		//                break;
		//        }
		//        return true;
		//    }
		//}
		//*/

		public bool SendCurrentInspectedPanelData(eAssignedLane_Printer Lane, int size)
        {
            if (PrinterManager.getInstance() == null)
                return false;
            else
            {
                InspectedPanel panel = new InspectedPanel();
                try
                {
                    #region MMF

                    using (var mmf = MemoryMappedFile.CreateOrOpen("PrinterCenter_" + Lane.ToString(), size, MemoryMappedFileAccess.ReadWrite))
                    {
                        using (var mmvStream = mmf.CreateViewStream(0, size))
                        {
                            BinaryFormatter binFormatter = new BinaryFormatter();
                            byte[] buffer = new byte[size];

                            mmvStream.Read(buffer, 0, size);
                            panel = (InspectedPanel)binFormatter.Deserialize(new MemoryStream(buffer));
                        }
                    }

                    #endregion MMF
                }
                catch (Exception e)
                {
                    Log4.PrinterLogger.ErrorFormat("MMF Exception");
                    Log4.PrinterLogger.ErrorFormat(e.Message);
                    return false;
                }

                Log4.PrinterLogger.InfoFormat("[D]SendCurrentInspectedPanelData({0},{1})", Lane.ToString(), size);

                switch (Lane)
                {
                    case eAssignedLane_Printer.None:
                        break;

                    case eAssignedLane_Printer.Lane1:
                        ViewModelLocator.Atom.FlowHostVM.spiInspectedData.Add("[" + eAssignedLane_Printer.Lane1.ToString() + "]" + panel.InspectStartTime);
                        Log4.PrinterLogger.InfoFormat(" -Push [{0}] CurrentInspectModeTemp.InspectMode={1} .InspectResult={2}", Lane.ToString(), panel.InspectMode, panel.InspectResult);
                        PrinterManager.getInstance().CurrentInspectModeTemp[0].InspectMode = panel.InspectMode;
                        PrinterManager.getInstance().CurrentInspectModeTemp[0].InspectResult = panel.InspectResult;
                        Log4.PrinterLogger.InfoFormat(" -Enqueue [{0}] SPI Data InspectTime = {1}", Lane.ToString(), panel.InspectStartTime);
                        PrinterManager.getInstance().RemotePrinter[0].Printer.InspectedPanels.Enqueue(panel);
                        //PrinterManager.getInstance().RemotePrinter[0].Printer.StartProcess();
                        break;

                    case eAssignedLane_Printer.Lane2:
                        ViewModelLocator.Atom.FlowHostVM.spiInspectedData.Add("[" + eAssignedLane_Printer.Lane2.ToString() + "]" + panel.InspectStartTime);
                        Log4.PrinterLogger.InfoFormat(" -Push [{0}] CurrentInspectModeTemp.InspectMode={1} .InspectResult={2}", Lane.ToString(), panel.InspectMode, panel.InspectResult);
                        PrinterManager.getInstance().CurrentInspectModeTemp[1].InspectMode = panel.InspectMode;
                        PrinterManager.getInstance().CurrentInspectModeTemp[1].InspectResult = panel.InspectResult;
                        Log4.PrinterLogger.InfoFormat(" -Enqueue [{0}] SPI Data InspectTime = {1}", Lane.ToString(), panel.InspectStartTime);
                        PrinterManager.getInstance().RemotePrinter[1].Printer.InspectedPanels.Enqueue(panel);
                        //PrinterManager.getInstance().RemotePrinter[1].Printer.StartProcess();
                        break;
                }
                return true;
            }
        }

        public void StartProcess(eAssignedLane_Printer Lane,string filename)
        {
            if (PrinterManager.getInstance() == null)
                return;
            else
            {
                Log4.PrinterLogger.InfoFormat("[SOA]StartProcess({0},{1})", Lane.ToString(), filename);
                //如果檔案是用XML則需要enqueue
                if(PrinterManager.getInstance().ExchangeMethod == SFCData.eSFCDataExchangeMethod.XML)
                {
                    InspectedPanel panel = null;
                    while (panel == null)
                    {
                        panel = null;
                        try
                        {
                            panel = IPCHelper.DeserializeFromXML<InspectedPanel>(filename);
                        }
                        catch (Exception ex)
                        {
                            //Log4.SFCLogger.InfoFormat("Pause {0} ms", iPause);
                            //Thread.Sleep(iPause);
                            //Log4.SFCLogger.ErrorFormat("DeserializeFromXML File {0} Exception:{1}", AssemblyPath + "\\DataExchange\\" + e.Name, ex.Message);

                        }

                    }
                    switch (Lane)
                    {
                        case eAssignedLane_Printer.None:
                            break;

                        case eAssignedLane_Printer.Lane1:
                            ViewModelLocator.Atom.FlowHostVM.spiInspectedData.Add("[" + eAssignedLane_Printer.Lane1.ToString() + "]" + panel.InspectStartTime);
                            Log4.PrinterLogger.InfoFormat(" -Push [{0}] CurrentInspectModeTemp.InspectMode={1} .InspectResult={2}", Lane.ToString(), panel.InspectMode, panel.InspectResult);
                            PrinterManager.getInstance().CurrentInspectModeTemp[0].InspectMode = panel.InspectMode;
                            PrinterManager.getInstance().CurrentInspectModeTemp[0].InspectResult = panel.InspectResult;
                            Log4.PrinterLogger.InfoFormat(" -Enqueue [{0}] SPI Data InspectTime = {1}", Lane.ToString(), panel.InspectStartTime);
                            PrinterManager.getInstance().RemotePrinter[0].Printer.InspectedPanels.Enqueue(panel);
            
                            break;

                        case eAssignedLane_Printer.Lane2:
                            ViewModelLocator.Atom.FlowHostVM.spiInspectedData.Add("[" + eAssignedLane_Printer.Lane2.ToString() + "]" + panel.InspectStartTime);
                            Log4.PrinterLogger.InfoFormat(" -Push [{0}] CurrentInspectModeTemp.InspectMode={1} .InspectResult={2}", Lane.ToString(), panel.InspectMode, panel.InspectResult);
                            PrinterManager.getInstance().CurrentInspectModeTemp[1].InspectMode = panel.InspectMode;
                            PrinterManager.getInstance().CurrentInspectModeTemp[1].InspectResult = panel.InspectResult;
                            Log4.PrinterLogger.InfoFormat(" -Enqueue [{0}] SPI Data InspectTime = {1}", Lane.ToString(), panel.InspectStartTime);
                            PrinterManager.getInstance().RemotePrinter[1].Printer.InspectedPanels.Enqueue(panel);
            
                            break;
                    }
                }
                switch (Lane)
                {
                    case eAssignedLane_Printer.None:
                        break;

                    case eAssignedLane_Printer.Lane1:
                        PrinterManager.getInstance().RemotePrinter[0].Printer.StartProcess();
                        break;

                    case eAssignedLane_Printer.Lane2:
                        PrinterManager.getInstance().RemotePrinter[1].Printer.StartProcess();
                        break;
                }
            }
        }
    }
}