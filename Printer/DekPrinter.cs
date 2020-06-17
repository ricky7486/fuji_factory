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

    public sealed class Dek_PrinterData : IParser
    {
        public string Version { get; set; }
        public string Product_ID { get; set; }
        public string Panel_ID { get; set; }
        public string Panel_Status { get; set; }
        public string Inspected_Date_and_Time { get; set; }
        public string Print_Direction { get; set; }
        public int Boards_Since_Last { get; set; }
        public string UnitDistance { get; set; }
        public string UnitAngle { get; set; }
        public string UnitTime { get; set; }

        public bool IsBarcodeMatched(object fileobj, string barcode)
        {
            Dek_PrinterData data = fileobj as Dek_PrinterData;
            if (data.Panel_ID == barcode)
                return true;
            else
                return false;
        }


        public object Parse(string filepath)
        {
            Dek_PrinterData ret = new Dek_PrinterData();
            try
            {
                XElement Dek = XElement.Load(filepath);

             
                ret.Version = Dek.getAttributeValue("Message","version");
                ret.Product_ID = Dek.getValue("Product_ID");
                ret.Panel_ID = Dek.getValue("Panel_ID");
                ret.Panel_Status = Dek.getValue("Panel_Status");
                ret.Inspected_Date_and_Time = Dek.getValue("Inspected_Date_and_Time");
                ret.Print_Direction = Dek.getValue("Print_Direction");

                int bsl = 0;
                int.TryParse(Dek.getValue("Boards_Since_Last"), out bsl);
                ret.Boards_Since_Last = bsl;
               
                ret.UnitDistance = Dek.getValue("Distance");
                ret.UnitAngle = Dek.getValue("Angle");
                string time = Dek.getValue("Time");
                if (time == "yyyyMMddHHmmss")
                    UnitTime = "seconds";
                else
                    UnitTime ="No_Defined";
               

            }
            catch (Exception exception)
            {
                Log4.PrinterLogger.ErrorFormat("Dek Printer Data Pasre Exception: {0}", filepath);
                Log4.PrinterLogger.ErrorFormat("Message: {0}", exception.Message);
                return null;
            }
            return ret;
        }
    }
    public class DekPrinter : PrinterBase, IDisposable
    {
        private string target;
        private SharedFolderWatcher WatchedFolder;
        //private int count = 0;
        private StretchResult _StretchResult;
        private RotationResult _RotationResult;
        private CenterOffsetResult _CenterOffsetResult;
        private DefectStatisticResult _DefectStatisticResult;
       
        public DekPrinter(eAssignedLane_Printer lane) : base(ePrinterVendor.DEK, lane)
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
                EkraFiducailOffset ekraFMAlgo = new EkraFiducailOffset();

                _CenterOffsetResult = (CenterOffsetResult)cocAlgo.Calculate(Boxes, currentPanel, null);
                _RotationResult = (RotationResult)rcAlgo.Calculate(Boxes, currentPanel, _CenterOffsetResult);
                _StretchResult = (StretchResult)sAlgo.Calculate(null, currentPanel, null);
                _DefectStatisticResult = (DefectStatisticResult)dsAlgo.Calculate(Boxes, currentPanel, null);

                //Wipe
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
            Dek_PrinterData tmpTool = new Dek_PrinterData();
            return (Dek_PrinterData)Match(currentPanel, (IParser)tmpTool, WatchedFolder);
        }

        public override bool Output(InspectedPanel currentPanel, object file)
        {
            var _Dek_PrinterData = (Dek_PrinterData)file;
            string netDrive;
            if (!PrinterSFSetting.IsOutEnable)
                return false;
            else
                netDrive = WmiDiskHelper.ExtractDiskID(PrinterSFSetting.OutDriveInfo) + @"\";//Disk mapping;
            string path = netDrive + String.Format("{0:yyyyMMddHHmmss}.xml", currentPanel.InspectStartTime);

            DateTime t = currentPanel.InspectStartTime;
            string strTime = string.Format("{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}",
                t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
           
            //bool bWipe = false;//=>須補，目前寫死
            bool bPcbResult = _DefectStatisticResult.DefectNum > 0 ? true : false;//true:NG false:GOOD

            string reference = "Bottom_Left";
            if (PrinterCommonSetting.IsQuadrent1)
                reference = "Bottom_Left";
            else if (PrinterCommonSetting.IsQuadrent2)
                reference = "Bottom_Right";
            else if (PrinterCommonSetting.IsQuadrent3)
                reference = "Top_Right";
            else if (PrinterCommonSetting.IsQuadrent4)
                reference = "Top_Left";
            try
            {

                XElement root = new XElement(
                    "Data"
                #region 原WriteMessage
                    , new XElement("Message"
                                     , new XAttribute("version", "1.0")
                                     , new XElement("Date_and_Time", strTime)
                                )
                #endregion
                #region 原WriteEquipment
                   , new XElement("Equipment"
                                    , new XAttribute("version", "1.0")
                                    , new XAttribute("reference", reference)
                                    , new XElement("Name", "TRI_SPI")
                                )
                #endregion
                #region 原WriteProcess
                   , new XElement("Process"
                                    , new XElement("Product_ID", _Dek_PrinterData.Product_ID)
                                    , new XElement("Panel_ID"
                                                            , new XElement("Panel", PrinterCommonSetting.MatchingBasis == UI.CommonSetting.eMatchingBasis.Barcode ? _Dek_PrinterData.Panel_ID : "NO_CODE")
                                                    )
                                    , new XElement("Batch_Count", ViewModelLocator.Atom.PrinterWindowVM.SN)
                                    , new XElement("Panel_Status","Inspected")//固定，後可能需要改
                                    , new XElement("Inspected_Date_and_Time", strTime)
                                    , new XElement("Direction",_Dek_PrinterData.Print_Direction)
                                    , new XElement("Units"
                                                            , new XElement("Distance",_Dek_PrinterData.UnitDistance)
                                                            , new XElement("Angle", _Dek_PrinterData.UnitAngle)
                                                            , new XElement("Time",_Dek_PrinterData.UnitTime)
                                                            , new XElement("Stretch","%")
                                                    )
                                    , new XElement("Offset_Correction" 
                                                            , new XElement("X", Math.Round((_CenterOffsetResult.Dx * 0.001), 6).ToString())
                                                            , new XElement("Y", Math.Round((_CenterOffsetResult.Dy * 0.001), 6).ToString())
                                                            , new XElement("Theta"
                                                                                , new XAttribute("CoR_X", Math.Round((_RotationResult.Center.X * 0.001), 6).ToString())
                                                                                , new XAttribute("CoR_Y", Math.Round((_RotationResult.Center.Y * 0.001), 6).ToString())
                                                                                , Math.Round(_RotationResult.Theta, 6).ToString()
                                                                          )
                                                            , new XElement("Stretch", Math.Round(_StretchResult.Stretch, 6).ToString())
                                                    )
                                    
                                    , CreateFiducalElement(currentPanel)
                                    , new XElement("Warning"
                                                            , new XAttribute("TotalNum", _DefectStatisticResult.TotalTestNum)
                                                            , new XAttribute("DefectNum",_DefectStatisticResult.WarningNum)
                                                            , new XElement("Volume"
                                                                                  , new XAttribute("Num",_DefectStatisticResult.VolumeWarningOver+_DefectStatisticResult.VolumeWarningUnder)
                                                                                  , new XElement("High",_DefectStatisticResult.VolumeWarningOver)
                                                                                  , new XElement("Low",_DefectStatisticResult.VolumeWarningUnder) 
                                                                            )
                                                            , new XElement("Height"
                                                                                  , new XAttribute("Num",_DefectStatisticResult.HeightWarningOver+_DefectStatisticResult.HeightWarningUnder)
                                                                                  , new XAttribute("High",_DefectStatisticResult.HeightWarningOver)
                                                                                  , new XAttribute("Low",_DefectStatisticResult.HeightWarningUnder)
                                                                            )
                                                            , new XElement("Area"
                                                                                  , new XAttribute("Num",_DefectStatisticResult.AreaWarningOver+_DefectStatisticResult.AreaWarningUnder)
                                                                                  , new XElement("High",_DefectStatisticResult.AreaWarningOver)
                                                                                  , new XElement("Low",_DefectStatisticResult.AreaWarningUnder)
                                                                            )
                                                            , new XElement("Bridge","0")
                                                            , new XElement("NoPaste","0")
                                                    )
                                    , new XElement("Alarm"
                                                            , new XAttribute("TotalNum", _DefectStatisticResult.TotalTestNum)
                                                            , new XAttribute("DefectNum", _DefectStatisticResult.DefectNum)
                                                            , new XElement("Volume"
                                                                                  , new XAttribute("Num", _DefectStatisticResult.VolumeDefectOver + _DefectStatisticResult.VolumeDefectUnder)
                                                                                  , new XElement("High", _DefectStatisticResult.VolumeDefectOver)
                                                                                  , new XElement("Low", _DefectStatisticResult.VolumeDefectUnder)
                                                                            )
                                                            , new XElement("Height"
                                                                                  , new XAttribute("Num", _DefectStatisticResult.HeightDefectOver + _DefectStatisticResult.HeightDefectUnder)
                                                                                  , new XElement("High", _DefectStatisticResult.HeightDefectOver)
                                                                                  , new XElement("Low", _DefectStatisticResult.HeightDefectUnder)
                                                                            )
                                                            , new XElement("Area"
                                                                                  , new XAttribute("Num", _DefectStatisticResult.AreaDefectOver + _DefectStatisticResult.AreaDefectUnder)
                                                                                  , new XElement("High", _DefectStatisticResult.AreaDefectOver)
                                                                                  , new XElement("Low", _DefectStatisticResult.AreaDefectUnder)
                                                                            )
                                                            , new XElement("Bridge", _DefectStatisticResult.DefectNum)
                                                            , new XElement("NoPaste", _DefectStatisticResult.NoPasteDefect)
                                                    )
                                )
                #endregion
                #region 原WriteCommand
                   /*
                    * 缺Wipe的演算法
                    */
                    , CreateCmdElement()
                #endregion

                );
                root.Save(path);
                Log4.PrinterLogger.InfoFormat("Save @ {0} [Dek]", path);
            }
            catch (Exception e)
            {
                Log4.PrinterLogger.InfoFormat("Save @ {0} Exception [Dek]",  path);
                Log4.PrinterLogger.InfoFormat("{0}", e.Message);
                throw new OutputException(e.Message);
                //return false;
            }
            return true;
        }
        private XElement CreateCmdElement()
        {
            XElement ret;
            switch (_WipeReason)
            {
                case eWipeStencilReason.PrintingTimesAreTooMuch:
                    ret = new XElement("Command","Clean1");
                    return ret;
                   
                case eWipeStencilReason.PrintingResultsAreTooBad:
                    ret = new XElement("Command", "Clean2");
                    return ret;
                    
              
                case eWipeStencilReason.NoNeedToWipe:
                default:
                    return null;
            }
        }
        private XElement CreateFiducalElement(InspectedPanel currentPanel)
        {
            int count = 0;
            XElement ret = new XElement("Fiducials", new XAttribute("Count", currentPanel.Panel.FiducialMarks.Count));
            foreach(var fm in currentPanel.Panel.FiducialMarks)
            {
                var elFM = new XElement("Fiducial"
                                        ,new XAttribute("Name", "Fid "+count.ToString())
                                        ,new XElement("X"
                                                    , new XAttribute("Origin", Math.Round(fm.CadCenter.X * 0.001, 3).ToString())
                                                    , Math.Round(fm.ResultRect.X * 0.001, 3).ToString()
                                                    )
                                        ,new XElement("Y"
                                                    ,new XAttribute("Origin", Math.Round(fm.CadCenter.Y * 0.001, 3).ToString())
                                                    , Math.Round(fm.ResultRect.Y * 0.001, 3).ToString()
                                                    )
                    );
                ret.Add(elFM);
                count++;
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
