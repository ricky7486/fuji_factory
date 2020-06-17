using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenter.Service;
using PrinterCenter.UI.SharedFolderSetting;
using PrinterCenter.File;
using System.IO;
using PrinterCenter.UI;
using aejw.Network;
using System.Xml.Linq;
using PrinterCenter.Log;
using System.Windows.Controls;
using PrinterCenter.UI.Chart;
using PrinterCenterData;
using System.Threading.Tasks;
using PrinterCenter.Printer.Algorithm;

namespace PrinterCenter.Printer
{
    public sealed class INOTIS_PrinterData :IParser
    {
        public string sPrint_Direction { get; set; }
        public string sPanel_ID { get; set; }
        //public static INOTIS_PrinterData Parse(string filepath)
        public object Parse(string filepath)
        {
            INOTIS_PrinterData ret = new INOTIS_PrinterData();
            try
            {
                XElement INOTIS = XElement.Load(filepath);

                //取得Print_Direction

                ret.sPrint_Direction = INOTIS.getValue("Print_Direction");

                if (ret.sPrint_Direction == null)
                {
                    Log4.PrinterLogger.Info("INOTIS Printer Data Pasre Fail on: 'Print_Direction' ");

                    return null;
                }
                

                //取得Barcode = Panel_ID
                ret.sPanel_ID = INOTIS.getValue("Panel_ID");
                if (ret.sPanel_ID == null)
                {
                    Log4.PrinterLogger.Info("INOTIS Printer Data Pasre Fail on: 'Panel_ID' ");
                    return null;
                }

            }
            catch (Exception exception)
            {
                Log4.PrinterLogger.ErrorFormat("INOTIS Printer Data Pasre Exception: {0}",filepath);
                Log4.PrinterLogger.ErrorFormat("Message: {0}", exception.Message);
                return null;
            }
           
            return ret;
        }

        public bool IsBarcodeMatched(object fileobj,string barcode)
        {
            INOTIS_PrinterData data = fileobj as INOTIS_PrinterData;
            if (data.sPanel_ID == barcode)
                return true;
            else
                return false;
        }
    }
    public sealed class INOTISPrinter : PrinterBase,IDisposable
    {
        //private int count = 0;
        private StretchResult _StretchResult;
        private RotationResult _RotationResult;
        private CenterOffsetResult _CenterOffsetResult;
        public INOTISPrinter(ePrinterVendor vendor,eAssignedLane_Printer lane) : base(vendor,lane)
        {
            //InspectedPanels.EnqueueEvent += new EventQueueHandler<InspectedPanel>(InspectedPanelsEnqueue<InspectedPanel>);
        }
        public INOTISPrinter(eAssignedLane_Printer lane):this(ePrinterVendor.INOTIS,lane)
        {
            
            //WatchedFiles.Clear();
        }

        private SharedFolderWatcher WatchedFolder;
        public void Dispose()
        {
        
        }
        private string target;
        public override void Activate()
        {
            //InDriveInfo Z:(\\IP\Folder)格式，WmiDiskHelper.ExtractDiskID取 "Z:"
            //但當選取的InDriveInfo是 D:格式，WmiDiskHelper.ExtractDiskID取"D:"
            //可是當送給DirectoryWatcher時 因D:本質是Disk Volume而不是Folder
            //而Z:是以建立共享資料夾的虛擬網路硬碟 (可以想成Z:是一個代號)，本質上他還是個Folder，所以可以監控
            //因此，為了避免選成Disk，故加一個@"\"


            target = WmiDiskHelper.ExtractDiskID(PrinterSFSetting.InDriveInfo) + @"\";//Disk mapping
            var des = WmiDiskHelper.ExtractProviderName(PrinterSFSetting.InDriveInfo);//ui顯示用
            WatchedFolder = PrinterManager.getInstance().AddWatcher(target, LaneID, des);
        

        }
      
   
        public override object Match(InspectedPanel currentPanel)
        {
         
            INOTIS_PrinterData tmpTool = new INOTIS_PrinterData();
            return (INOTIS_PrinterData)Match(currentPanel, (IParser)tmpTool, WatchedFolder);
          
        }

        public override void Calculate(InspectedPanel currentPanel,object file)
        {
            try
            {
                var Boxes = GetCandidateBoxes(currentPanel, PrinterCommonSetting);

                //Calculate
                CenterOffsetCorrectionAlgorithm cocAlgo = new CenterOffsetCorrectionAlgorithm(PrinterCommonSetting.Clone());
                RotationCorrectionAlgorithm rcAlgo = new RotationCorrectionAlgorithm(PrinterCommonSetting.Clone());
                StretchAlgorithm sAlgo = new StretchAlgorithm();


                _CenterOffsetResult = (CenterOffsetResult)cocAlgo.Calculate(Boxes, currentPanel, null);
                _RotationResult = (RotationResult)rcAlgo.Calculate(Boxes, currentPanel, _CenterOffsetResult);
                _StretchResult = (StretchResult)sAlgo.Calculate(null, currentPanel, null);
            }
            catch (Exception e)
            {
                throw new CaculateException(e.Message);
            }


        }
        /// <summary>
        /// 畫上Chart
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void UpdateHistory()
        {


            DxHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DxSetting.XAxis.Minimun + DxHistory.Count, _CenterOffsetResult.Dx));
            DyHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DySetting.XAxis.Minimun + DyHistory.Count, _CenterOffsetResult.Dy));
            ThetaHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.ThetaSetting.XAxis.Minimun + ThetaHistory.Count, _RotationResult.Theta));

        }

        public override bool Output(InspectedPanel currentPanel, object file)
        {


            string netDrive;
            if (!PrinterSFSetting.IsOutEnable)
                return false;
            else
                netDrive = WmiDiskHelper.ExtractDiskID(PrinterSFSetting.OutDriveInfo) + @"\";//Disk mapping;

            string path = netDrive + String.Format("{0:yyyyMMddHHmmss}.xml", currentPanel.InspectStartTime);
            INOTIS_PrinterData _INOTIS_PrinterData = file as INOTIS_PrinterData;
            try
            {
                XElement root = new XElement("Data",
                new XElement(
                    "Process",
                    new XElement("Direction", _INOTIS_PrinterData.sPrint_Direction),
                    new XElement("Offset_Correction",
                        new XElement("X", Math.Round((_CenterOffsetResult.Dx * 0.001), 6)),
                        new XElement("Y", Math.Round((_CenterOffsetResult.Dy * 0.001), 6)),
                        new XElement("Theta", new XAttribute("CoR_X", Math.Round((_RotationResult.Center.X * 0.001), 6)),
                                              new XAttribute("CoR_Y", Math.Round((_RotationResult.Center.Y * 0.001), 6)), 
                                              Math.Round(_RotationResult.Theta, 6)),
                        new XElement("Stretch", Math.Round(_StretchResult.Stretch, 6))
                                )//Offset_Correction
                   )//Process
                );
                root.Save(path);
                Log4.PrinterLogger.InfoFormat("Save @ {0} [INOTIS]",  path);
            }
            catch (Exception e)
            {
                Log4.PrinterLogger.InfoFormat("Save @ {0} Exception [INOTIS]", path);
                Log4.PrinterLogger.InfoFormat("{0}", e.Message);
                throw new OutputException(e.Message);
                //return false;
            }

            return true;
            
        }


     
    }
}
