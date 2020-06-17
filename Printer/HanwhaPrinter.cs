using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;
using PrinterCenter.Service;
using aejw.Network;
using System.Xml.Linq;
using PrinterCenter.File;
using PrinterCenter.Printer.Algorithm;
using PrinterCenter.Log;
using PrinterCenter.Printer.JudgeWipe;

namespace PrinterCenter.Printer
{
    #region Hanwha PrinterData
    public sealed class _HEADER
    {
        public _HEADER(string m, string r, __Unit u, string dd)
        {
            MachineName = m;
            Reference = r;
            Units = u;
            DefineDirection = dd;
        }
        public string MachineName { get; set; }
        public string Reference { get; set; }
        public __Unit Units { get; set; }
        public string DefineDirection { get; set; }
    }
    public sealed class __Unit
    {
        public __Unit(string d, string a, string t)
        {
            Distance = d;
            Angle = a;
            Time = t;

        }
        public string Distance { get; set; }
        public string Angle { get; set; }
        public string Time { get; set; }
    }

    public sealed class _PROCESS
    {
        public _PROCESS(string pn, string d, string t, string ps)
        {
            ProductName = pn;
            Date = d;
            Time = t;
            PanelState = ps;
        }
        public string ProductName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string PanelState { get; set; }
    }
    public sealed class _PANEL
    {
        public _PANEL(string pid, string barcode, string sdir, __PanelSize ps, string cor)
        {
            PanelId = pid;
            SqueegeeDir = sdir;
            BarcodeId = barcode;
            PanelSize = ps;
            Fiducials = new List<__Fiducial>();
            CenterOfRotation = cor;
        }
        public string PanelId { get; set; }
        public string BarcodeId { get; set; }
        public string SqueegeeDir { get; set; }
        public __PanelSize PanelSize { get; set; }
        public List<__Fiducial> Fiducials { get; set; }
        public string CenterOfRotation { get; set; }
    }
    public sealed class __PanelSize
    {
        public double width { get; set; }
        public double height { get; set; }

    }
    public sealed class __Fiducial
    {
        public string Id { get; set; }
        public double position_x { get; set; }
        public double position_y { get; set; }
    }
    #endregion
    public sealed class Hanwha_PrinterData : IParser
    {
        public _HEADER Header;
        public _PROCESS Process;
        public _PANEL Panel;



        public object Parse(string filepath)
        {
            Hanwha_PrinterData ret = new Hanwha_PrinterData();
            try
            {
                XElement Hanwha = XElement.Load(filepath);
                //取得Print_Direction

                ret.Header = new _HEADER(
                            Hanwha.getValue("MachineName"),

                            Hanwha.getValue("Reference"),
                            new __Unit(Hanwha.getValue("Distance"), Hanwha.getValue("Angle"), Hanwha.getValue("Time")),
                            Hanwha.getValue("DefineDirection")
                    );


                ret.Panel = new _PANEL(
                        Hanwha.getValue("PanelId"),
                        Hanwha.getValue("BarcodeId"),
                        Hanwha.getValue("SqueegeeDir"),
                        null,
                        Hanwha.getValue("CenterOfRotation")
                    );

                //目前只有需要PanelId
                //if (ret.Panel.PanelId == null && ret.Panel.BarcodeId == null)
                //    return null;
                //比對方式有兩種，不一定要像舊版需要強制

            }
            catch (Exception exception)
            {
                Log4.PrinterLogger.ErrorFormat("INOTIS Printer Data Pasre Exception: {0}", filepath);
                Log4.PrinterLogger.ErrorFormat("Message: {0}", exception.Message);
                return null;
            }
           
   

            return ret;
        }

        public bool IsBarcodeMatched(object fileobj, string barcode)
        {
            Hanwha_PrinterData data = fileobj as Hanwha_PrinterData;
            if (data.Panel.BarcodeId == barcode)
                return true;
            else
                return false;
        }
    }




    public sealed class HanwhaPrinter : PrinterBase, IDisposable
    {
        private string target;
        private SharedFolderWatcher WatchedFolder;
        //private int count = 0;
        private StretchResult _StretchResult;
        private RotationResult _RotationResult;
        private CenterOffsetResult _CenterOffsetResult;
        private HanwhaResult _HanwhaResult;
        public HanwhaPrinter(eAssignedLane_Printer lane) : base(ePrinterVendor.HANWHA, lane)
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
                _HanwhaResult = null;
                var Boxes = GetCandidateBoxes(currentPanel, PrinterCommonSetting);

                //Calculate
                CenterOffsetCorrectionAlgorithm cocAlgo = new CenterOffsetCorrectionAlgorithm(PrinterCommonSetting.Clone());
                RotationCorrectionAlgorithm rcAlgo = new RotationCorrectionAlgorithm(PrinterCommonSetting.Clone());
                StretchAlgorithm sAlgo = new StretchAlgorithm();
                HanwhaHAVAvg hanwhaAvg = new HanwhaHAVAvg();

                _CenterOffsetResult = (CenterOffsetResult)cocAlgo.Calculate(Boxes, currentPanel, null);
                _RotationResult = (RotationResult)rcAlgo.Calculate(Boxes, currentPanel, _CenterOffsetResult);
                _StretchResult = (StretchResult)sAlgo.Calculate(null, currentPanel, null);
                _HanwhaResult = (HanwhaResult)hanwhaAvg.Calculate(Boxes, null, null);
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

            Hanwha_PrinterData tmpTool = new Hanwha_PrinterData();
            return (Hanwha_PrinterData)Match(currentPanel, (IParser)tmpTool, WatchedFolder);
           
        }

        public override bool Output(InspectedPanel currentPanel, object file)
        {
            var _Hanwha_PrinterData = (Hanwha_PrinterData)file;
            string netDrive;
            if (!PrinterSFSetting.IsOutEnable)
                return false;
            else
                netDrive = WmiDiskHelper.ExtractDiskID(PrinterSFSetting.OutDriveInfo) + @"\";//Disk mapping;

            string path = netDrive + String.Format("{0:yyyyMMddHHmmss}.xml", currentPanel.InspectStartTime);
            try
            {
                XElement root = new XElement(
                    "ResultInsp",
                    HEADER_SECTION(currentPanel, _Hanwha_PrinterData),
                    PROCESS_SECTION(currentPanel, _Hanwha_PrinterData),
                    PANEL_SECTION(currentPanel, _Hanwha_PrinterData),
                    MEASUREMENTS_SECTION(_Hanwha_PrinterData),
                    DEFECT_SECTION(currentPanel, _Hanwha_PrinterData),
                    OPTIONAL_SECTION()
                );
                root.Save(path);
                Log4.PrinterLogger.InfoFormat("Save @ {0} [Hanwha]", path);
            }
            catch (Exception e)
            {
                Log4.PrinterLogger.InfoFormat("Save @ {0} Exception [Hanwha]", path);
                Log4.PrinterLogger.InfoFormat("{0}", e.Message);
                throw new OutputException(e.Message);
                //return false;
            }

            return true;
        }

        public override void UpdateHistory()
        {
            DxHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DxSetting.XAxis.Minimun + DxHistory.Count, _CenterOffsetResult.Dx));
            DyHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DySetting.XAxis.Minimun + DyHistory.Count, _CenterOffsetResult.Dy));
            ThetaHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.ThetaSetting.XAxis.Minimun + ThetaHistory.Count, _RotationResult.Theta));

        }








        private XElement HEADER_SECTION(InspectedPanel currentPanel,Hanwha_PrinterData data)
        {
            XElement ret = new XElement(
                "HEADER",
                new XElement("MachineName", "SPI"), //SPI fixed
                new XElement("Reference", "BOTTOM_LEFT"),//應依照象限，固定，所以請設定象限1  [Has Comfirmed]
                new XElement("Units",
                                    new XElement("Distance", "mm"), //固定 [Has Comfirmed]
                                    new XElement("Angle", "degree"),//固定 [Has Comfirmed]
                                    new XElement("Time", "seconds"),//固定 [Has Comfirmed]
                                    new XElement("Ratio", "percentage")//固定 [Has Comfirmed]
                                                        ),

                new XElement("DefineDirection", data.Header.DefineDirection)//不確定是否要按照得來的資訊 // Use as DEFINE_VERTICAL固定 [Has Comfirmed]

                );
            return ret;
        }
        private XElement PROCESS_SECTION(InspectedPanel currentPanel, Hanwha_PrinterData data)
        {
            DateTime now = DateTime.Now;
            XElement ret = new XElement(
                "PROCESS",
                new XElement("ProductName", currentPanel.Panel.ModelName),//目前用檢測的資料為主，SPEC沒有註明
                new XElement("Date", now.ToString("yyyy_MM_dd")),
                new XElement("Time", now.ToString("HH_mm_ss")),
                //new XElement("PanelState", PanelStatus == eClosedLoopSPIStatus.NonInspected ? "NOT_INSPECTED" : "INSPECTED")//有INSPECTED / NOT_INSPECTED
                new XElement("PanelState",  "INSPECTED")//有INSPECTED / NOT_INSPECTED
            );
            return ret;
        }
        private XElement PANEL_SECTION(InspectedPanel currentPanel, Hanwha_PrinterData data)
        {

            List<FiducialMark> fidList = currentPanel.Panel.FiducialMarks;
            XElement ret = new XElement(
                "PANEL",
                new XElement("PanelId", "0"),//Panel ID 取流水碼
                new XElement("BarcodeId", currentPanel.Panel.PanelBarcode),

                new XElement("Stencil", new XAttribute("thickness", "0.100")),
                new XElement("PanelSize", new XAttribute("width", currentPanel.Panel.FullCadRect.Width * 0.001), new XAttribute("height", currentPanel.Panel.FullCadRect.Height * 0.001)), //單位mm [has confirmed] SPI是um
                new XElement("Fiducials",
                                        from fid in fidList
                                        select new XElement("Fiducial", new XAttribute("id", fidList.IndexOf(fid)),
                                                                        new XAttribute("pos_x", fid.CadRect.Location.X * 0.001),//單位mm [has confirmed] SPI是um
                                                                        new XAttribute("pos_y", fid.CadRect.Location.Y * 0.001))//單位mm [has confirmed] SPI是um
                            )
                );
            return ret;
        }

        private XElement MEASUREMENTS_SECTION(Hanwha_PrinterData data)
        {
            XElement ret = new XElement(
                "MEASUREMENTS",
                new XElement("SqueegeeDir", "FORWARD"),//版的序列Hanwha只有兩種選擇 : FORWARD 1)↓Rear to Front 2)→Left to Right   和 REVERSE1)↑Front to Rear 2)←Right to Left
                new XElement("CenterOfRotation", data.Panel.CenterOfRotation),//Fixed as PANEL_CENTER [Has Confirmed]
                new XElement("Rotation", Math.Round(_RotationResult.Theta, 6)),

                new XElement("OffsetX", Math.Round((_CenterOffsetResult.Dx * 0.001), 6)), //um=>mm
                new XElement("OffsetY", Math.Round((_CenterOffsetResult.Dy * 0.001), 6)),
                //new XElement("OffsetX", Math.Round((Dx * 0.00001), 6)),//單位mm [has confirmed] SPI是um
                //new XElement("OffsetY", Math.Round((Dy * 0.00001), 6)),//單位mm [has confirmed] SPI是um
                new XElement("Height", Math.Round((_HanwhaResult.avgHeight_pct * 0.001), 6)),
                new XElement("Area", Math.Round((_HanwhaResult.avgArea_pct * 0.001), 6)),
                new XElement("Volume", Math.Round((_HanwhaResult.avgVolume_pct * 0.001), 6))
                );
            return ret;
        }

        private XElement DEFECT_SECTION(InspectedPanel currentPanel, Hanwha_PrinterData data)
        {
            //var tree = new TreeAccess(Panel);
            //int PanelIndex = ProjectManager.getInstance().GetCurrentCarrier().GetPanelIndex(Panel);

            var _boxList = GetBoxList(currentPanel);
            var _failList = FailList(currentPanel);
            XElement ret = new XElement(
                "DEFECT",
                new XElement("TotalCount", _boxList.Count),
                new XElement("DefectCount",
                                            new XAttribute("count", _failList.Count),
                                            new XElement("Height",
                                                                    new XAttribute("high", GetCount(_boxList,eOverallStatus.SOL_HEIGHTOVER)),
                                                                    new XAttribute("low",  GetCount(_boxList,eOverallStatus.SOL_HEIGHTUNDER))
                                                        ),
                                            new XElement("Volume",
                                                                    new XAttribute("heigh", GetCount(_boxList, eOverallStatus.SOL_VOLUMEOVER)),
                                                                    new XAttribute("low", GetCount(_boxList,eOverallStatus.SOL_VOLUMEUNDER))
                                                        ),
                                            new XElement("Area",
                                                                    new XAttribute("heigh", GetCount(_boxList, eOverallStatus.SOL_AREAOVER)),
                                                                    new XAttribute("low", GetCount(_boxList,eOverallStatus.SOL_AREAUNDER))
                                                        ),
                                            new XElement("Bridge",
                                                                    new XAttribute("count", GetCount(_boxList,eOverallStatus.SOL_BRIDGE))
                                                         )


                                            ),
                //new XElement("WarningCount",
                //                            new XAttribute("count", InspectResultClass.getInstance().WarningCnt[PanelIndex])
                //                            //new XElement("Height", InspectResultClass.getInstance().Fail[PanelIndex].)
                //                            )
                new XElement("DefectPosition", from box in _boxList
                                               let id = box.FullName
                                               let posx = box.CadCenter.X * 0.001       //Pad Position 
                                               let posy = box.CadCenter.Y * 0.001
                                               let sizex = box.CadRect.Width * 0.001
                                               let sizey = box.CadRect.Height * 0.001
                                               let code = ToHanwhaPadType(box.Status)
                                               select new XElement("Pad", new XAttribute("id", id), new XAttribute("posx", posx), new XAttribute("posy", posy), new XAttribute("sizex", sizex), new XAttribute("sizey", sizey), new XAttribute("code", code))
                                            )
                );
            return ret;
        }
        private List<Box> GetBoxList(InspectedPanel panel)
        {
            List<Box> retLst = new List<Box>();
            foreach(var board in panel.Panel.Boards)
            {
                foreach(var component in board.Components)
                {
                    foreach(var box in component.Boxes)
                    {
                        retLst.Add(box);
                    }
                }
            }
            return retLst;
        }
        /// <summary>
        /// 主程式段 InspectResultClass.SetResultByPriority
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        private List<Box> FailList(InspectedPanel panel)
        {
            IniFile ini = new IniFile();
            bool bWarningAsFail = false;
            Boolean.TryParse(ini.Read("Setting", "WarningAsFail"), out bWarningAsFail);
            List<Box> failList = new List<Box>();
            foreach (var board in panel.Panel.Boards)
            {
                foreach (var component in board.Components)
                {
                    foreach (var box in component.Boxes)
                    {
                        switch (box.Status)
                        {
                            case eOverallStatus.SOL_BRIDGE:
                            case eOverallStatus.SOL_HEIGHTOVER:
                            case eOverallStatus.SOL_HEIGHTUNDER:
                            case eOverallStatus.SOL_VOLUMEOVER:
                            case eOverallStatus.SOL_VOLUMEUNDER:
                            case eOverallStatus.SOL_AREAOVER:
                            case eOverallStatus.SOL_AREAUNDER:
                            case eOverallStatus.SOL_OFFSETX:
                            case eOverallStatus.SOL_OFFSETY:
                           
                            case eOverallStatus.SOL_WIDTHOVER:
                            case eOverallStatus.SOL_WIDTHUNDER:
                            case eOverallStatus.SOL_LENGTHOVER:
                            case eOverallStatus.SOL_LENGTHUNDER:
                            case eOverallStatus.SOL_MINHEIGHT:
                            case eOverallStatus.SOL_MAXHEIGHT:
                            case eOverallStatus.SOL_MINAREA:
                            case eOverallStatus.SOL_MAXAREA:
                            case eOverallStatus.SOL_MINVOLUME:
                            case eOverallStatus.SOL_MAXVOLUME:
                            case eOverallStatus.SOL_GAP:
                            case eOverallStatus.SOL_THICKNESSOVER:
                            case eOverallStatus.SOL_THICKNESSUNDER:
                            case eOverallStatus.SOL_PADSHIFT:
                                failList.Add(box);
                                break;
                            case eOverallStatus.SOL_AREAWARNING:
                            case eOverallStatus.SOL_HEIGHTWARNING:
                            case eOverallStatus.SOL_VOLUMEWARNING:
                                if (bWarningAsFail)
                                    failList.Add(box);
                                break;
                            default:
                                break;
                        }

                    }
                }
            }
            return failList;
        }
       
        private uint GetCount(List<Box> Boxes,eOverallStatus query)
        {
            uint ret = 0;
            foreach(var box in Boxes)
            {
                if (box.Status == query)
                    ret++;
            }
            return ret;
        }
        private string ToHanwhaPadType(eOverallStatus status)
        {
            switch (status)
            {
                //NP: No Paste: SPI沒有
                case eOverallStatus.SOL_HEIGHTOVER:
                    return "HE";
                case eOverallStatus.SOL_HEIGHTUNDER:
                    return "HL";
                case eOverallStatus.SOL_AREAOVER:
                    return "AE";
                case eOverallStatus.SOL_AREAUNDER:
                    return "AL";
                case eOverallStatus.SOL_VOLUMEOVER:
                    return "VE";
                case eOverallStatus.SOL_VOLUMEUNDER:
                    return "VL";

                case eOverallStatus.SOL_BRIDGE:
                    return "BR";



                default:
                    return "NOTDEFINED";



            }
        }
        private XElement OPTIONAL_SECTION()
        {
            string Cmd = "";
            //暫時沒有
            //switch (WipeStencilReason)
            //{
            //    case eWipeStencilReason.NoNeedToWipe:
            //        Cmd = "NOUSE";
            //        break;
            //    case eWipeStencilReason.PrintingResultsAreTooBad:
            //    case eWipeStencilReason.PrintingTimesAreTooMuch:
            //        Cmd = "Cleaning";
            //        break;
            //    default:
            //        Cmd = "NULL";
            //        break;
            //}
            Cmd = _WipeReason==eWipeStencilReason.NoNeedToWipe?"NOUSE":"Cleaning";
            XElement ret = new XElement(
                "OPTIONAL",
                new XElement("COMMAND", Cmd),
                new XElement("BUFFER_INFO", new XAttribute("use", "?"),
                                            new XElement("MaxBufferSlot", "?"),
                                            new XElement("FrontPcbCount", "?"),
                                            new XElement("RearPcbCount", "?")
                            )//don't know yet
                );

            return ret;
        }

    }
}
