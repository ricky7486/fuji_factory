using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;
using PrinterCenter.Service;
using PrinterCenter.Printer.Algorithm;
using aejw.Network;
using PrinterCenter.Log;
using System.Xml.Linq;
using PrinterCenter.File;
using PrinterCenter.Printer.JudgeWipe;

namespace PrinterCenter.Printer
{
    public struct EseFM
    {
        public System.Windows.Point P;
        public string Name;
    }
    public sealed class Ese_PrinterData : IParser
    {
        //HEADER
        public string MachineName { get; set; }
        public string Reference { get; set; }
        public string UnitDistance { get; set; }
        public string UnitAngle { get; set; }
        public string UnitTime { get; set; }
        public string DefineDirection { get; set; }
        //PROPCESS
        public string ProductName { get; set; }
        public string Date { get; set; }
        public string PrintTime { get; set; }
        public string PanelState { get; set; }
        //PANEL
        public string BarcodeId { get; set; }
        public string SqueegeeDir { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public List<EseFM> EseFMs { get; set; }
        public string CenterOfRotation { get; set; }

        //OPTIONAL
        public double PrintSpeed { get; set; }
        public double PrintPressure { get; set; }
        public Ese_PrinterData()
        {
            EseFMs = new List<EseFM>();
        }
        public bool IsBarcodeMatched(object fileobj, string barcode)
        {
            Ese_PrinterData data = fileobj as Ese_PrinterData;
            if (data.BarcodeId == barcode)
                return true;
            else
                return false;
        }

        public object Parse(string filepath)
        {
            Ese_PrinterData ret = new Ese_PrinterData();



            try
            {
                XElement GKG = XElement.Load(filepath);
                //取得四大Element，因為有重複的Element，直接下去使用Extension內的getValue會錯誤
                XElement _HEADER = GKG.getElement("HEADER");
                XElement _PROCESS = GKG.getElement("PROCESS");
                XElement _PANEL = GKG.getElement("PANEL");
                XElement _OPTIONAL = GKG.getElement("OPTIONAL");
                //HEADER
                ret.MachineName = _HEADER.getValue("MachineName");
                ret.Reference = _HEADER.getValue("Reference");
                ret.UnitDistance = _HEADER.getValue("Distance");
                ret.UnitAngle = _HEADER.getValue("Angle");
                ret.UnitTime = _HEADER.getValue("Time");
                ret.DefineDirection = _HEADER.getValue("DefineDirection");
                //PROCESS
                ret.ProductName = _PROCESS.getValue("ProductName");
                ret.Date = _PROCESS.getValue("Date");
                ret.PrintTime = _PROCESS.getValue("Time");
                ret.PanelState = _PROCESS.getValue("PanelState");
                //PANEL
                ret.BarcodeId = _PANEL.getValue("BarcodeId");
                ret.SqueegeeDir = _PANEL.getValue("SqueegeeDir");
                string sWidth, sHeight;
                double width, height;
                sWidth = _PANEL.getAttributeValue("PanelSize", "width");
                Double.TryParse(sWidth, out width);
                ret.Width = width;
                sHeight = _PANEL.getAttributeValue("PanelSize", "height");
                Double.TryParse(sHeight, out height);
                ret.Height = height;
                //PANEL-Fid
                List<XElement> elFMs = _PANEL.getElements("Fiducial");
                foreach(var fm in elFMs)
                {
                    string name = fm.getAttributeValue("id");
                    string x = fm.getAttributeValue("pox_x");
                    string y = fm.getAttributeValue("pox_y");
                    double dX, dY;
                    Double.TryParse(x, out dX);
                    Double.TryParse(y, out dY);
                    var eseFM = new EseFM();
                    eseFM.Name = name;
                    eseFM.P.X = dX;
                    eseFM.P.Y = dY;
                    ret.EseFMs.Add(eseFM);
                }
                 
                ret.CenterOfRotation = _PANEL.getValue("CenterOfRotation");

                //OPTIONAL
                string sPrintSpeed, sPrintPressure;
                double printspeed, printpressure;
                sPrintSpeed = _PANEL.getValue("PrintSpeed");
                Double.TryParse(sPrintSpeed, out printspeed);
                ret.PrintSpeed = printspeed;
                sPrintPressure = _PANEL.getValue("PrintPressure");
                Double.TryParse(sPrintPressure, out printpressure);
                ret.PrintPressure = printpressure;
            }
            catch (Exception exception)
            {
                Log4.PrinterLogger.ErrorFormat("GKG Printer Data Pasre Exception: {0}", filepath);
                Log4.PrinterLogger.ErrorFormat("Message: {0}", exception.Message);
                return null;
            }
            return ret;
        }
    }
    public sealed class EsePrinter : PrinterBase, IDisposable
    {
        private string target;
        private SharedFolderWatcher WatchedFolder;
        //private int count = 0;
        private StretchResult _StretchResult;
        private RotationResult _RotationResult;
        private CenterOffsetResult _CenterOffsetResult;
        private DefectStatisticResult _DefectStatisticResult;
        private PadHAVAvgResult _PadHAVAvgResult;
      

        public EsePrinter(eAssignedLane_Printer lane) : base(ePrinterVendor.ESE, lane)
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
                _DefectStatisticResult = null;

                var Boxes = GetCandidateBoxes(currentPanel, PrinterCommonSetting);

                //Calculate
                CenterOffsetCorrectionAlgorithm cocAlgo = new CenterOffsetCorrectionAlgorithm(PrinterCommonSetting.Clone());
                RotationCorrectionAlgorithm rcAlgo = new RotationCorrectionAlgorithm(PrinterCommonSetting.Clone());
                StretchAlgorithm sAlgo = new StretchAlgorithm();
                DefectStatistic dsAlgo = new DefectStatistic();
                //Yamaha該方法和Ese要計算 each box的HAV percentage 平均功能一致，故重用
                PadHAVAvg padAvgAlgo = new PadHAVAvg();

                _CenterOffsetResult = (CenterOffsetResult)cocAlgo.Calculate(Boxes, currentPanel, null);
                _RotationResult = (RotationResult)rcAlgo.Calculate(Boxes, currentPanel, _CenterOffsetResult);
                _StretchResult = (StretchResult)sAlgo.Calculate(null, currentPanel, null);
                _DefectStatisticResult = (DefectStatisticResult)dsAlgo.Calculate(Boxes, currentPanel, null);//=>從failbox來
                _PadHAVAvgResult = (PadHAVAvgResult)padAvgAlgo.Calculate(Boxes, null, null);//YAMAHA需要額外的avg統計
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
            Ese_PrinterData tmpTool = new Ese_PrinterData();
            return (Ese_PrinterData)Match(currentPanel, (IParser)tmpTool, WatchedFolder);
        }

        public override bool Output(InspectedPanel currentPanel, object file)
        {
            var _Ese_PrinterData = (Ese_PrinterData)file;
            string netDrive;
            if (!PrinterSFSetting.IsOutEnable)
                return false;
            else
                netDrive = WmiDiskHelper.ExtractDiskID(PrinterSFSetting.OutDriveInfo) + @"\";//Disk mapping;
            string path = netDrive + String.Format("{0:yyyy-MM-dd-HH_mm_ss}.xml", currentPanel.InspectStartTime);

            try
            {
                bool IsWipe = _WipeReason == eWipeStencilReason.NoNeedToWipe?false:true;
                //DateTime t = currentPanel.InspectStartTime;
                //string str = string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}.{6:00}",
                //    t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, t.Millisecond);

                DateTime printTime;
                DateTime.TryParse(_Ese_PrinterData.PrintTime,out printTime);
                if (printTime == DateTime.MinValue)
                {
                    printTime = currentPanel.InspectStartTime; // If no information of printer, It will be SPI base
                }
                string strDate = printTime.ToString("yyyy_MM_dd");
                string strTime = printTime.ToString("HH_mm_ss");

                XElement root = new XElement(
                            "ResultInsp"
                                        //原WriteHeaderSection
                                        , new XElement("HEADER"
                                                            , new XElement("MachineName", "SPI")
                                                            , new XElement("Reference", "BOTTOM_LEFT")
                                                            //原WriteHeaderUnitsSection
                                                            , new XElement("Units"
                                                                                , new XElement("Distance", "mm")
                                                                                , new XElement("Angle", "degree")
                                                                                , new XElement("Time", "seconds")
                                                                                , new XElement("Ratio", "percentage")
                                                            )
                                                            , new XElement("DefineDirection", _Ese_PrinterData.DefineDirection)
                                        )
                                        //原WriteProcessSection
                                        , new XElement("PROCESS"
                                                            , new XElement("ProductName", "")//原本都是空白 奇怪@@
                                                            , new XElement("Date", strDate)
                                                            , new XElement("Time", strTime)
                                                            , new XElement("PanelState", "INSPECTED")
                                        )
                                        //WritePanelSection
                                        , new XElement("PANEL"
                                                            , new XElement("PanelId", ViewModelLocator.Atom.PrinterWindowVM.SN)//sn*
                                                            , new XElement("BarcodeId",_Ese_PrinterData.BarcodeId)
                                                            , new XElement("Stencil",   new XAttribute("thickness",(currentPanel.Panel.StencilThick*0.001).ToString() ) )
                                                            , new XElement("PanelSize", new XAttribute("width",(currentPanel.Panel.FullCadRect.Width*0.001).ToString())
                                                                                      , new XAttribute("height",(currentPanel.Panel.FullCadRect.Height*0.01).ToString())
                                                                            )
                                                            //WritePanelFiducialsSection
                                                            ,new XElement("Fiducials", CreateFiducailElements(currentPanel) )
                 
                                        )
                                        //WriteMeasurementsSection
                                        , new XElement("MEASUREMENTS"
                                                            , new XElement("SqueegeeDir", _Ese_PrinterData.SqueegeeDir)
                                                            , new XElement("CenterOfRotation"
                                                                                            , new XAttribute("RotCx", Math.Round((_RotationResult.Center.X * 0.001), 6).ToString())
                                                                                            , new XAttribute("RotCy", Math.Round((_RotationResult.Center.Y * 0.001), 6).ToString())
                                                                            )
                                                            , new XElement("Rotation", Math.Round(_RotationResult.Theta, 6).ToString())
                                                            , new XElement("OffsetX",  Math.Round((_CenterOffsetResult.Dx * 0.001), 6).ToString())
                                                            , new XElement("OffsetY",  Math.Round((_CenterOffsetResult.Dy * 0.001), 6).ToString())
                                                            , new XElement("Height", _PadHAVAvgResult.avgHeight_pct.ToString("F3"))
                                                            , new XElement("Area",   _PadHAVAvgResult.avgArea_pct.ToString("F3"))
                                                            , new XElement("Volume", _PadHAVAvgResult.avgVolume_pct.ToString("F3"))

                                        )
                                        //WriteDefectSection
                                        , new XElement("DEFECT"
                                                //WriteDefectTotalCountSection
                                                            , new XElement("TotalCount", GetTotalBoxCount(currentPanel).ToString())
                                                //WriteDefectDefectCountSection
                                                            , new XElement("DefectCount", new XAttribute("count",(currentPanel.FailList().Count).ToString())
                                                                                        , new XElement("Height" , new XAttribute("high",_DefectStatisticResult.HeightDefectOver)
                                                                                                                , new XAttribute("low",_DefectStatisticResult.HeightDefectUnder)
                                                                                        )
                                                                                        , new XElement("Volume" , new XAttribute("high",_DefectStatisticResult.VolumeDefectOver)
                                                                                                                , new XAttribute("low",_DefectStatisticResult.VolumeDefectUnder)
                                                                                        )
                                                                                        , new XElement("Area" , new XAttribute("high",_DefectStatisticResult.AreaDefectOver)
                                                                                                              , new XAttribute("low",_DefectStatisticResult.AreaDefectUnder)
                                                                                        )
                                                                                        , new XElement("Bridge", new XAttribute("count",_DefectStatisticResult.BridgeDefect))  
                                                            )

                                                            //WriteDefectWarningCountSection
                                                            , new XElement("WarningCount", new XAttribute("count", (currentPanel.WarningList().Count).ToString() )
                                                                                         , new XElement("Height", new XAttribute("count", (_DefectStatisticResult.HeightWarningOver + _DefectStatisticResult.HeightWarningUnder).ToString()))
                                                                                         , new XElement("Volume", new XAttribute("count", (_DefectStatisticResult.VolumeWarningOver + _DefectStatisticResult.VolumeWarningUnder).ToString()))
                                                                                         , new XElement("Area", new XAttribute("count", (_DefectStatisticResult.AreaWarningOver + _DefectStatisticResult.AreaWarningUnder).ToString()))
                                                            )
                                                            , new XElement("OPTIONAL", new XElement("COMMAND", IsWipe? "CLEANING":string.Empty))
                                        )



                   );
                root.Save(path);
                Log4.PrinterLogger.InfoFormat("Save @ {0} [ESE]", path);
            }
            catch (Exception e)
            {
                Log4.PrinterLogger.InfoFormat("Save @ {0} Exception [ESE]",  path);
                Log4.PrinterLogger.InfoFormat("{0}", e.Message);
                throw new OutputException(e.Message);
                //return false;
            }

            return true;
        }
        private int GetTotalBoxCount(InspectedPanel panel)
        {
            int count = 0;
            foreach(var b in panel.Panel.Boards)
            {
                foreach(var c in b.Components)
                {
                    count += c.Boxes.Count();
                }
            }
            return count;
        }
        private List<XElement> CreateFiducailElements(InspectedPanel panel)
        {
            List<XElement> ret = new List<XElement>();
            int count = 1;
            if (panel.Panel.FiducialMarks.Count >= 2)
            {
                foreach (var fm in panel.Panel.FiducialMarks)
                {
                    var elFM = new XElement("Fiducial"
                                        , new XAttribute("id", "PANELMARK" + (count + 1).ToString())
                                        , new XAttribute("pos_x", Math.Round(((fm.CadCenter.X - panel.Panel.FullCadRect.X) * 0.001), 6).ToString())
                                        , new XAttribute("pos_y", Math.Round(((fm.CadCenter.Y - panel.Panel.FullCadRect.Y) * 0.001), 6).ToString())
                        );
                    count++;
                    ret.Add(elFM);
                }
                    
            }

            return ret;

        }
        public override void UpdateHistory()
        {
            DxHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DxSetting.XAxis.Minimun + DxHistory.Count, _CenterOffsetResult.Dx));
            DyHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DySetting.XAxis.Minimun + DyHistory.Count, _CenterOffsetResult.Dy));
            ThetaHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.ThetaSetting.XAxis.Minimun + ThetaHistory.Count, _RotationResult.Theta));

        }
    }
}
