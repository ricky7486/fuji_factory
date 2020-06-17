using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;
using PrinterCenter.Service;
using PrinterCenter.Printer.Algorithm;
using aejw.Network;
using System.Xml.Linq;
using PrinterCenter.Log;
using PrinterCenter.File;
using PrinterCenter.Printer.JudgeWipe;
using PrinterCenter.UI;
using System.IO;
using PrinterCenter.FileClass;

//GKG 和Htgd一樣@@點解?

namespace PrinterCenter.Printer
{
    public sealed class GKG_PrinterData : IParser
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
            GKG_PrinterData data = fileobj as GKG_PrinterData;
            if (data.Barcode == barcode)
                return true;
            else
                return false;
        }

        public object Parse(string filepath)
        {
            GKG_PrinterData ret = new GKG_PrinterData();

            try
            {
                XElement GKG = XElement.Load(filepath);

                ret.ModelName = GKG.getValue("ModelName");
                ret.PrintTime = GKG.getValue("PrintTime");
                ret.SN = GKG.getValue("SN");
                ret.Barcode = GKG.getValue("Barcode");
                ret.Direction = GKG.getValue("Direction");
                ret.Thickness = GKG.getValue("Thickness");
                double thickness = 0.1;
                if (double.TryParse(GKG.getValue("StencilThickness"), out thickness) == true)
                    StencilThickness = thickness;

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

    public sealed class GKGPrinter : PrinterBase, IDisposable
    {
        private string target;
        private SharedFolderWatcher WatchedFolder;
        //private int count = 0;
        private StretchResult _StretchResult;
        private RotationResult _RotationResult;
        private CenterOffsetResult _CenterOffsetResult;
      
        public GKGPrinter(eAssignedLane_Printer lane) : base(ePrinterVendor.GKG, lane)
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
            GKG_PrinterData tmpTool = new GKG_PrinterData();
            return (GKG_PrinterData)Match(currentPanel, (IParser)tmpTool, WatchedFolder);
        }
        private List<XElement> CreateFidMark(InspectedPanel currentPanel)
        {
            List<XElement> retFidMarks = new List<XElement>();
            int index = 1;
            if (currentPanel.Panel.FiducialMarks.Count >= 2)
                foreach (var fm in currentPanel.Panel.FiducialMarks)
                {
                    var elFM = new XElement("FidMark"
                                                    , new XAttribute("Name", "PANELMARK" + index.ToString())
                                                    , new XAttribute("X", Math.Round(((fm.CadCenter.X - currentPanel.Panel.FullCadRect.X) * 0.001), 6).ToString())
                                                    , new XAttribute("Y", Math.Round(((fm.CadCenter.Y - currentPanel.Panel.FullCadRect.Y) * 0.001), 6).ToString())
                        );
                    retFidMarks.Add(elFM);
                }
            else
                return null;
            return retFidMarks;

        }
        private string filename="";
        public override bool Output(InspectedPanel currentPanel, object file)
        {
            filename = "";
            var _GKG_PrinterData = (GKG_PrinterData)file;
            string netDrive;
            if (!PrinterSFSetting.IsOutEnable)
                return false;
            else
                netDrive = WmiDiskHelper.ExtractDiskID(PrinterSFSetting.OutDriveInfo) + @"\";//Disk mapping;
            filename = String.Format("{0:yyyyMMddHHmmss}", currentPanel.InspectStartTime);
            string path = netDrive + filename + ".xml";
            
            try
            {
                bool IsWipe = _WipeReason == eWipeStencilReason.NoNeedToWipe? false:true;
                DateTime t = currentPanel.InspectStartTime;
                string str = string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}.{6:00}",
                    t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, t.Millisecond);
                XElement root = new XElement(
                   "SpiData"
                            //原WriteBasicSection
                            , new XElement("ModelName", currentPanel.Panel.ModelName)
                            , new XElement("InspectTime", str)
                            , new XElement("SN", _GKG_PrinterData.SN)
                            , new XElement("Barcode", _GKG_PrinterData.Barcode)
                            //原WriteUnitSection
                            , new XElement("Units" //寫死
                                                  , new XElement("Distance", "MM")
                                                  , new XElement("Angle", "Degree")
                                                  , new XElement("Stretch", "%")
                                          )
                            , new XElement("Direction", _GKG_PrinterData.Direction)
                            //原WriteFidMark
                            , new XElement("FidMarkList", CreateFidMark(currentPanel))
                            //原WriteCorrection
                            , new XElement("Correction"
                                                  , new XAttribute("RotCx", Math.Round((_RotationResult.Center.X * 0.001), 6).ToString())
                                                  , new XAttribute("RotCy", Math.Round((_RotationResult.Center.Y * 0.001), 6).ToString())
                                                  , new XElement("X", Math.Round((_CenterOffsetResult.Dx * 0.001), 6).ToString())
                                                  , new XElement("Y", Math.Round((_CenterOffsetResult.Dy * 0.001), 6).ToString())
                                                  , new XElement("Theta", Math.Round(_RotationResult.Theta, 6).ToString())
                                                  , new XElement("Stretch", Math.Round(_StretchResult.Stretch, 6).ToString())
                                          )
                            //原WriteWipe, 暫時not imp
                            , new XElement("Wipe", IsWipe.ToString())
                   );
                root.Save(path);
                Log4.PrinterLogger.InfoFormat("Save @ {0} [GKG]", path);
            }
            catch (Exception e)
            {
                Log4.PrinterLogger.InfoFormat("Save @ {0} Exception [GKG]", path);
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


        //Custom CSV
        public override bool CustomOutput(InspectedPanel currentPanel, object file)
        {
            bool isCustomEnable = false;
            using (IniFile ini = new IniFile())
                isCustomEnable = ini.IsSectionExist("Custom");

            if (isCustomEnable)//客製化enable
            {
                var customVM = ViewModelLocator.Atom.CustomVM;
                try
                {
                    OutputGKGCSV OutputCSV;
                    if (LaneID == eAssignedLane_Printer.Lane1 && customVM.bGKGCSVLane1)
                    {
                        OutputCSV = new OutputGKGCSV(customVM.GKGCSVLane1Path );
                        OutputCSV.Run(currentPanel);
                    }
                    else if (LaneID == eAssignedLane_Printer.Lane2 && customVM.bGKGCSVLane2)
                    {
                        OutputCSV = new OutputGKGCSV(customVM.GKGCSVLane2Path );
                        OutputCSV.Run(currentPanel);
                    }
                }
                catch (Exception e)
                {
                    Log4.PrinterLogger.InfoFormat("Save @ {0} Exception [GKG.CSV]");
                    throw new CustomOutputException(e.Message);
                }


                return true;
            }
            else
                return false;//沒有客製化ini eanble

           
        }
    }

    //移植自主程式GKG
    struct BoardDefectData
    {
        public string Fdate;
        public string BoardSN;
        public string BoardStatus;
        public string TorB;
        public string lotno;
        public string Modifier;
        public string Comp_PadName;
        public string PadStatus;
        public string ErrType;
        public double Volume;
        public double VolumeSpec;
        public double Area;
        public double AreaSpec;
        public double Height;
        public double HeightSpec;
        public double px;
        public double py;
        public double oriX;
        public double oriY;
        public double cx;
        public double cy;
    }
    public sealed class OutputGKGCSV
    {
        string folderPath = string.Empty;

        public OutputGKGCSV(string folderPath)
        {
            this.folderPath = folderPath;
        }
        List<Box> GetBoxes(Board board)
        {
            List<Box> boxList = new List<Box>();
            foreach (var component in board.Components)
                foreach (var box in component.Boxes)
                    boxList.Add(box);
            return boxList;
        }
        List<Box> GetFailBoxList(Board board)
        {
            var failBoxList = new List<Box>();

            var boxList = GetBoxes(board);
            foreach (var box in boxList)
            {
                if (box.IsSkipped)
                    continue;
                //WIN-4463修改code中對eOverallStatus.SOL_BY_RPASS改為bRpass來判斷Rpass
                //if (box.inspectResult.status != eOverallStatus.SOL_PASS && box.inspectResult.status != eOverallStatus.SOL_WARNING && box.inspectResult.status != eOverallStatus.SOL_BY_RPASS)
                if (box.Status != eOverallStatus.SOL_PASS && box.Status != eOverallStatus.SOL_WARNING && box.bRPass != true)
                    failBoxList.Add(box);
            }

            return failBoxList;
        }

        List<Box> GetPassBoxList(Board board)
        {
            var passBoxList = new List<Box>();

            var boxList = GetBoxes(board);
            foreach (var box in boxList)
            {
                if (box.IsSkipped)
                    continue;
                //WIN-4463修改code中對eOverallStatus.SOL_BY_RPASS改為bRpass來判斷Rpass
                //if (box.inspectResult.status == eOverallStatus.SOL_PASS || box.inspectResult.status == eOverallStatus.SOL_WARNING || box.inspectResult.status == eOverallStatus.SOL_BY_RPASS)
                if (box.Status == eOverallStatus.SOL_PASS || box.Status == eOverallStatus.SOL_WARNING || box.bRPass == true)
                    passBoxList.Add(box);
            }

            return passBoxList;
        }

        bool SaveCSV(string fileName, StringBuilder data)
        {
            if (fileName == string.Empty || (data == null || data.Length == 0))
                return false;

            //var fp = new FileProcess();
            bool bCreate = FileProcess.CreateFolder(folderPath);
            //fp = null;

            if (bCreate == false)
            {
                TRMessageBox.Show(string.Format("Create CSV folder failed({0})", folderPath));
                return false;
            }

            string filePath = string.Format("{0}\\{1}", folderPath, fileName);
            var file = new StreamWriter(filePath, true);
            try
            {
                file.Write(data.ToString());
                file.Flush();
                file.Close();
            }
            catch (Exception ee)
            {
                Log4.PrinterLogger.InfoFormat(ee.ToString());
                return false;
            }

            return true;
        }

        public void Run(InspectedPanel panel)
        {
            //CarrierNode carrier = ProjectManager.getInstance().GetCurrentCarrier();
            //CarrierInspData carrierInspData = DynamicDataCollection.getInstance().Get(carrier);

            //PanelInspData panelInspData = carrierInspData.GetPanelInspData(ProjectManager.getInstance().GetCurrentPanel());
            //PanelNode panel = panelInspData.Panel;

            //var boardList = new TreeAccess(panel).GetBoardList(eTreeStatus.Normal);
            var boardList = panel.Panel.Boards;
            if (boardList.Count == 0)
                return;

        
            bool bPass = panel.StripePrepare_IsPass;

            var inspectStartTime = panel.InspectStartTime;
            var strInspectStartTime = inspectStartTime.ToString("yyyy/MM/dd HH:mm:ss");

            //註冊表，要增開
            //ExportDataPara exportDataPara = MachinePara.getInstance().ExportDataTable.ExportDataPara;

           
            string loginName = panel.LoginName;

            var data = new StringBuilder();
            //normal mode and two stage: just first inspection need to write header
            //if (ProjectManager.getInstance().PrjType != ePrjType.TwoStage ||
            //    (ProjectManager.getInstance().PrjType == ePrjType.TwoStage && ProjectManager.getInstance().GetPrjTestStatus() == ePrjTestStatus.AllUnTest))
            //{
                data.AppendLine(string.Format("Model Name: {0}", panel.Panel.ModelName));
                string[] fieldName = new string[] { "Fdate", "BoardSN", "BoardStatus", "T/B", "lotno", "Modifier", "Comp_PadName", "PadStatus", "ErrType", "Volume", "VolumeSpec", "Area", "AreaSpec", "Height", "HeightSpec", "px", "py", "oriX", "oriY", "padX", "padY" };
                data.AppendLine(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}", fieldName));
            //}
            foreach (var board in boardList)
            {
                var failList = GetFailBoxList(board);
                bool bFailBoxExistInBoard = (failList.Count > 0);
                Record(/*carrier,*/ panel.Panel, bPass, strInspectStartTime, loginName, data, board, failList, false, bFailBoxExistInBoard);
                var passList = GetPassBoxList(board);
                Record(/*carrier,*/ panel.Panel, bPass, strInspectStartTime, loginName, data, board, passList, true, bFailBoxExistInBoard);
            }

            var fileName = string.Format("{0:yyyyMMddHHmmss}.csv", inspectStartTime);
            //2Stage目前沒考量進去
            //if (ProjectManager.getInstance().PrjType == ePrjType.TwoStage)
            //{
            //    if (ProjectManager.getInstance().GetPrjTestStatus() == ePrjTestStatus.AllUnTest)
            //    {
            //        InspectManager.ClosedLoop.Printer.TwoStageBoxCollectionTool.getInstance().FirstStageCSVFileName = fileName;
            //    }
            //    if (ProjectManager.getInstance().GetPrjTestStatus() == ePrjTestStatus.OneTestDone)
            //    {
            //        if (InspectManager.ClosedLoop.Printer.TwoStageBoxCollectionTool.getInstance().FirstStageCSVFileName != string.Empty)
            //            fileName = InspectManager.ClosedLoop.Printer.TwoStageBoxCollectionTool.getInstance().FirstStageCSVFileName;
            //    }
            //}
            SaveCSV(fileName, data);
        }

        static void Record(/*Carrier carrier,*/ Panel panel, bool bPass, string strInspectStartTime, string loginName, StringBuilder data, Board board, List<Box> boxList, bool isPassList, bool bFailBoxExistInBoard)
        {
            if (boxList.Count == 0)
                return;

            foreach (var box in boxList)
            {
                var boardDefectData = new BoardDefectData()
                {
                    Fdate = string.Empty,
                    BoardSN = string.Empty,
                    BoardStatus = string.Empty,
                    TorB = string.Empty,
                    lotno = string.Empty,
                    Modifier = string.Empty,
                    Comp_PadName = string.Empty,
                    PadStatus = string.Empty,
                    ErrType = string.Empty,
                    Volume = 0,
                    VolumeSpec = 0,
                    Area = 0,
                    AreaSpec = 0,
                    Height = 0,
                    HeightSpec = 0,
                    px = 0,
                    py = 0,
                    oriX = 0,
                    oriY = 0,
                    cx = 0,
                    cy = 0
                };

                boardDefectData.Fdate = strInspectStartTime;

                string barcode = board.Barcode;
                if (String.IsNullOrEmpty(barcode) == false && barcode.ToUpper().CompareTo("NONE") != 0)
                {
                    if (barcode.Contains("(=Bot=)"))
                        barcode = barcode.Replace("(=Bot=)", string.Empty);
                    else if (barcode.Contains("(=Top=)"))
                        barcode = barcode.Replace("(=Top=)", string.Empty);
                }
                else
                {
                    barcode = strInspectStartTime;
                }
                boardDefectData.BoardSN = barcode;

                string boardStatus = string.Empty;
                if (bFailBoxExistInBoard == false)
                {
                    boardStatus = "PASS";
                }
                else if (bPass)
                {
                    boardStatus = "RPASS";
                }
                else
                {
                    string boxInspectStatus = board.ConfirmStatus.ToString();
                    int lastIdxOf_OfBoxInsStatus = boxInspectStatus.LastIndexOf('_');
                    boardStatus = boxInspectStatus.Substring(lastIdxOf_OfBoxInsStatus + 1, boxInspectStatus.Length - 1 - lastIdxOf_OfBoxInsStatus);
                }
                boardDefectData.BoardStatus = boardStatus;

                boardDefectData.TorB = (board.IsTopFace) ? "T" : "B";

                boardDefectData.lotno = panel.LotID;

                boardDefectData.Modifier = loginName;

                boardDefectData.Comp_PadName = box.FullName;

                boardDefectData.PadStatus = isPassList ? "PASS" : ((box.bRPass) ? "RPASS" : "FAIL");

                boardDefectData.ErrType = box.Status.ToString();

                double volume = box.Volume_v/ 1000000000.0;
                double area = box.Area_v / 1000000.0;
                double height = box.Height_v / 1000.0;

                //double volumeSpec = box.specInfo.volume.dValue / 1000000000.0;
                double volumeSpec = box.specVolume.dValue / 1000000000.0;
                double areaSpec = box.specArea.dValue / 1000000.0;
                double heightSpec = box.specHeight.dValue / 1000.0;

                double shiftX = box.ShiftX_v / 1000.0; // mm
                double shiftY = box.ShiftY_v / 1000.0; // mm

                double volumePercentage = Math.Round(box.Volume_p, 0);
                double areaPercentage = Math.Round(box.Area_p, 0);
                double heightPercentage = Math.Round(box.Height_p, 0);

                boardDefectData.Volume = volume;
                boardDefectData.Area = area;
                boardDefectData.Height = height;

                boardDefectData.VolumeSpec = volumeSpec;
                boardDefectData.AreaSpec = areaSpec;
                boardDefectData.HeightSpec = heightSpec;

                boardDefectData.px = shiftX;
                boardDefectData.py = shiftY;

                boardDefectData.oriX = Math.Round(((panel.FullCadRect.X) * 0.001), 6);
                boardDefectData.oriY = Math.Round(((panel.FullCadRect.Y) * 0.001), 6);

                boardDefectData.cx = Math.Round(((box.CadCenter.X - panel.FullCadRect.X) * 0.001), 6);
                boardDefectData.cy = Math.Round(((box.CadCenter.Y - panel.FullCadRect.Y) * 0.001), 6);

                string[] fieldValue = new string[]
                    {
                        boardDefectData.Fdate,
                        boardDefectData.BoardSN,
                        boardDefectData.BoardStatus,
                        boardDefectData.TorB,
                        boardDefectData.lotno,
                        boardDefectData.Modifier,
                        boardDefectData.Comp_PadName,
                        boardDefectData.PadStatus,
                        boardDefectData.ErrType,
                        boardDefectData.Volume.ToString(),
                        string.Format("{0}({1}%)", boardDefectData.VolumeSpec, volumePercentage),
                        boardDefectData.Area.ToString(),
                        string.Format("{0}({1}%)", boardDefectData.AreaSpec, areaPercentage),
                        boardDefectData.Height.ToString(),
                        string.Format("{0}({1}%)", boardDefectData.HeightSpec, heightPercentage),
                        boardDefectData.px.ToString(),
                        boardDefectData.py.ToString(),
                        boardDefectData.oriX.ToString(),
                        boardDefectData.oriY.ToString(),
                        boardDefectData.cx.ToString(),
                        boardDefectData.cy.ToString()
                    };
                data.AppendLine(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}", fieldValue));
            }
        }
    }
}
