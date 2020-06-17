using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;
using PrinterCenter.Service;
using PrinterCenter.Printer.Algorithm;
using System.Xml.Linq;
using PrinterCenter.File;
using PrinterCenter.Log;
using aejw.Network;
using PrinterCenter.Printer.JudgeWipe;

namespace PrinterCenter.Printer
{
    public sealed class Htgd_PrinterData : IParser
    {
        public string ModelName { get; set; }
        public string PrintTime { get; set; }
        public string SN { get; set; }
        public string Barcode { get; set; }
        public string Direction { get; set; }
        public string Thickness { get; set; }
        public double StencilThickness { get; set; }
        public bool IsBarcodeMatched(object fileobj, string barcode)
        {
            Htgd_PrinterData data = fileobj as Htgd_PrinterData;
            if (data.Barcode == barcode)
                return true;
            else
                return false;
        }

        public object Parse(string filepath)
        {
            Htgd_PrinterData ret = new Htgd_PrinterData();
            try
            {
                XElement Htgd = XElement.Load(filepath);

                ret.ModelName = Htgd.getValue("ModelName");
                ret.PrintTime = Htgd.getValue("PrintTime");
                ret.SN = Htgd.getValue("SN");
                ret.Barcode = Htgd.getValue("Barcode");
                ret.Direction = Htgd.getValue("Direction");
                ret.Thickness = Htgd.getValue("Thickness");
                double thickness = 0.1;
                if (double.TryParse(Htgd.getValue("StencilThickness"), out thickness) == true)
                    StencilThickness = thickness;

            }
            catch (Exception exception)
            {
                Log4.PrinterLogger.ErrorFormat("Htgd Printer Data Pasre Exception: {0}", filepath);
                Log4.PrinterLogger.ErrorFormat("Message: {0}", exception.Message);
                return null;
            }
            return ret;
        }
    }

    public sealed class HtgdPrinter : PrinterBase, IDisposable
    {
        private string target;
        private SharedFolderWatcher WatchedFolder;
        //private int count = 0;
        private StretchResult _StretchResult;
        private RotationResult _RotationResult;
        private CenterOffsetResult _CenterOffsetResult;
        //private DefectStatisticResult _DefectStatisticResult;
        public HtgdPrinter(eAssignedLane_Printer lane) : base(ePrinterVendor.HTGD, lane)
        {
        }
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

        public override void Calculate(InspectedPanel currentPanel, object file)
        {
            try
            {

                _CenterOffsetResult = null;
                _RotationResult = null;
                _StretchResult = null;


                var Boxes = GetCandidateBoxes(currentPanel, PrinterCommonSetting);

                //Calculate
                CenterOffsetCorrectionAlgorithm cocAlgo = new CenterOffsetCorrectionAlgorithm(PrinterCommonSetting.Clone());
                RotationCorrectionAlgorithm rcAlgo = new RotationCorrectionAlgorithm(PrinterCommonSetting.Clone());
                StretchAlgorithm sAlgo = new StretchAlgorithm();



                _CenterOffsetResult = (CenterOffsetResult)cocAlgo.Calculate(Boxes, currentPanel, null);
                _RotationResult = (RotationResult)rcAlgo.Calculate(Boxes, currentPanel, _CenterOffsetResult);
                _StretchResult = (StretchResult)sAlgo.Calculate(null, currentPanel, null);
                _WipeReason = JudgeWipeHelper.JudgeWipeByPriorityStrategy(currentPanel, Boxes);
            }
            catch (Exception e)
            {
                throw new CaculateException(e.Message);
            }
        }

        public void Dispose()
        {
         
        }

        public override object Match(InspectedPanel currentPanel)
        {
            Htgd_PrinterData tmpTool = new Htgd_PrinterData();
            return (Htgd_PrinterData)Match(currentPanel, (IParser)tmpTool, WatchedFolder);
        }

        public override bool Output(InspectedPanel currentPanel, object file)
        {
            var _Htgd_PrinterData = (Htgd_PrinterData)file;
            string netDrive;
            if (!PrinterSFSetting.IsOutEnable)
                return false;
            else
                netDrive = WmiDiskHelper.ExtractDiskID(PrinterSFSetting.OutDriveInfo) + @"\";//Disk mapping;
            string path = netDrive + String.Format("{0:yyyyMMddHHmmss}.xml", currentPanel.InspectStartTime);

            try
            {
                bool IsWipe = _WipeReason==eWipeStencilReason.NoNeedToWipe?false:true;
                DateTime t = currentPanel.InspectStartTime;
                string str = string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}.{6:00}",
                    t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, t.Millisecond);
                XElement root = new XElement(
                   "SpiData"
                            //原WriteBasicSection
                            , new XElement("ModelName", currentPanel.Panel.ModelName)
                            , new XElement("InspectTime", str)
                            , new XElement("SN", _Htgd_PrinterData.SN)
                            , new XElement("Barcode", _Htgd_PrinterData.Barcode)
                            //原WriteUnitSection
                            , new XElement("Units" //寫死
                                                  , new XElement("Distance","MM")
                                                  , new XElement("Angle", "Degree")
                                                  , new XElement("Stretch", "%")
                                          )
                            , new XElement("Direction", _Htgd_PrinterData.Direction)
                            //原WriteFidMark
                            , new XElement("FidMarkList", CreateFidMark(currentPanel))
                            //原WriteCorrection
                            , new XElement("Correction"
                                                  , new XAttribute("RotCx", Math.Round((_RotationResult.Center.X * 0.001), 6).ToString())
                                                  , new XAttribute("RotCy", Math.Round((_RotationResult.Center.Y * 0.001), 6).ToString())
                                                  , new XElement("X", Math.Round((_CenterOffsetResult.Dx * 0.001), 6).ToString())
                                                  , new XElement("Y", Math.Round((_CenterOffsetResult.Dy * 0.001), 6).ToString())
                                                  , new XElement("Theta", Math.Round(_RotationResult.Theta, 6).ToString() )
                                                  , new XElement("Stretch", Math.Round(_StretchResult.Stretch, 6).ToString())
                                          )
                            //原WriteWipe, 暫時not imp
                            , new XElement("Wipe", IsWipe.ToString())
                   );
                root.Save(path);
                Log4.PrinterLogger.InfoFormat("Save @ {0} [Htgd]", path);
            }
            catch (Exception e)
            {
                Log4.PrinterLogger.InfoFormat("Save @ {0} Exception [Htgd]",  path);
                Log4.PrinterLogger.InfoFormat("{0}", e.Message);
                throw new OutputException(e.Message);
                //return false;
            }

            return true;

        }
        private List<XElement> CreateFidMark(InspectedPanel currentPanel)
        {
            List<XElement> retFidMarks = new List<XElement>();
            int index = 1;
            if (currentPanel.Panel.FiducialMarks.Count >= 2)
                foreach (var fm in currentPanel.Panel.FiducialMarks)
                {
                    var elFM = new XElement("FidMark"
                                                    , new XAttribute("Name","PANELMARK"+index.ToString())
                                                    , new XAttribute("X", Math.Round(((fm.CadCenter.X - currentPanel.Panel.FullCadRect.X) * 0.001), 6).ToString())
                                                    , new XAttribute("Y", Math.Round(((fm.CadCenter.Y - currentPanel.Panel.FullCadRect.Y) * 0.001), 6).ToString())
                        );
                    retFidMarks.Add(elFM);
                }
            else
                return null;
            return retFidMarks;

        }
        public override void UpdateHistory()
        {
            DxHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DxSetting.XAxis.Minimun + DxHistory.Count, _CenterOffsetResult.Dx));
            DyHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DySetting.XAxis.Minimun + DyHistory.Count, _CenterOffsetResult.Dy));
            ThetaHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.ThetaSetting.XAxis.Minimun + ThetaHistory.Count, _RotationResult.Theta));

        }
    }
}
