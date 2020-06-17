using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;
using PrinterCenter.Service;
using System.Xml.Linq;
using PrinterCenter.Log;
using PrinterCenter.File;
using aejw.Network;
using PrinterCenter.Printer.Algorithm;
using PrinterCenter.Printer.JudgeWipe;

namespace PrinterCenter.Printer
{
    public struct EkraFM
    {
        public System.Windows.Point P;
        public string Name; 
    }
    public sealed class Ekra_PrinterData : IParser
    {
        public DateTime PrintTime { get; set; }
        public string  MachineName { get; set; }
        public string Reference { get; set; }
        public string ProductId { get; set; }
        public string PanelId { get; set; }
        public string PanelBarcode { get; set; }

        public string PanelStatus { get; set; }//Printed or other
        public string PrintDirection { get; set; }
        public double PanelWidth { get; set; }

        public double PanelHeight { get; set; }
        public string UnitsDistance { get; set; }
        public string UnitsAngle { get; set; }
        public string UnitsTime { get; set; }

        public List<EkraFM> EkraFMs { get; set; }

        public bool IsBarcodeMatched(object fileobj, string barcode)
        {
            Ekra_PrinterData data = fileobj as Ekra_PrinterData;
            if (data.PanelBarcode == barcode)
                return true;
            else
                return false;
        }
        public Ekra_PrinterData()
        {
            EkraFMs = new List<EkraFM>();

        }
        public object Parse(string filepath)
        {
            Ekra_PrinterData ret = new Ekra_PrinterData();
            try
            {
                XElement Ekra = XElement.Load(filepath);

                DateTime time;
                DateTime.TryParse(Ekra.getValue("DateAndTime"), out time);
                ret.PrintTime = time;
                ret.MachineName = Ekra.getValue("MachineName");
                ret.Reference = Ekra.getValue("Reference");
                ret.ProductId = Ekra.getValue("ProductId");
                ret.PanelId = Ekra.getValue("PanelId");
                ret.PanelBarcode = Ekra.getValue("PanelDmc");
                ret.PrintDirection = Ekra.getValue("PrintDirection");

                double width=0;
                double.TryParse(Ekra.getValue("Width"), out width);
                ret.PanelWidth = width;
                double height = 0;
                double.TryParse(Ekra.getValue("Height"), out height);
                ret.PanelHeight = height;

                ret.UnitsDistance = Ekra.getValue("Distance");
                ret.UnitsAngle = Ekra.getValue("Angle");
                ret.UnitsTime = Ekra.getValue("Time");

                var fmlist = Ekra.getElements("Fiducial");
                AddtoEkraFMList(ret,fmlist);

                //Fiducial
            }
            catch (Exception exception)
            {
                Log4.PrinterLogger.ErrorFormat("Ekra Printer Data Pasre Exception: {0}", filepath);
                Log4.PrinterLogger.ErrorFormat("Message: {0}", exception.Message);
                return null;
            }
            return ret;
        }
        private void AddtoEkraFMList(Ekra_PrinterData addee,List<XElement> FMs)
        {
            foreach(var fm in FMs)
            {
                EkraFM FM = new EkraFM();
                FM.Name = fm.Attribute("Name").Value;
                double Px;
                double.TryParse(fm.getAttributeValue("X", "Origin"),out Px);
                FM.P.X = Px *  1000;
                double Py;
                double.TryParse(fm.getAttributeValue("Y", "Origin"), out Py);
                FM.P.Y = Py * 1000;

                addee.EkraFMs.Add(FM);
            }
        }
    }

    public sealed class EkraPrinter : PrinterBase, IDisposable
    {
        private string target;
        private SharedFolderWatcher WatchedFolder;
        //private int count = 0;
        private StretchResult _StretchResult;
        private RotationResult _RotationResult;
        private CenterOffsetResult _CenterOffsetResult;
        private DefectStatisticResult _DefectStatisticResult;
        private EkraFiducailOffsetResult _EkraFiducailOffsetResult;
       
        public EkraPrinter(eAssignedLane_Printer lane) : base(ePrinterVendor.EKRA, lane)
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


                Ekra_PrinterData Ekradata = file as Ekra_PrinterData;
                _CenterOffsetResult = null;
                _RotationResult = null;
                _StretchResult = null;
                _DefectStatisticResult = null;
                _EkraFiducailOffsetResult = null;
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

                var input = new EkraFiducailOffsetInputPara(Ekradata, _RotationResult, _CenterOffsetResult);
                _EkraFiducailOffsetResult = (EkraFiducailOffsetResult)ekraFMAlgo.Calculate(null, null, input);
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

            Ekra_PrinterData tmpTool = new Ekra_PrinterData();
            return (Ekra_PrinterData)Match(currentPanel, (IParser)tmpTool, WatchedFolder);
        }

        public override bool Output(InspectedPanel currentPanel, object file)
        {
            var _Ekra_PrinterData = (Ekra_PrinterData)file;
            string netDrive;
            if (!PrinterSFSetting.IsOutEnable)
                return false;
            else
                netDrive = WmiDiskHelper.ExtractDiskID(PrinterSFSetting.OutDriveInfo) + @"\";//Disk mapping;
            string path = netDrive + String.Format("{0:yyyyMMddHHmmss}.xml", currentPanel.InspectStartTime);

            DateTime t = currentPanel.InspectStartTime;
            string strTime = string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}.{6:00}",
                t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, t.Millisecond);
            bool bByPass = false; //=>用在PanelStatus(原) 是使用DefectCount來判斷,初始是NonInspected
            bool bWipe = _WipeReason == eWipeStencilReason.NoNeedToWipe? false:true;
            bool bPcbResult = _DefectStatisticResult.DefectNum>0 ? true: false;//true:NG false:GOOD
            try
            {

                XElement root = new XElement(
                    "FeedbackInspResult",
                    new XElement("Message" //原WriteMessage
                                        , new XElement("DateAndTime", strTime)
                                ),
                    new XElement("Machine" //原WriteMachine
                                        , new XElement("Name", "SPI")
                                        , new XElement("Reference", _Ekra_PrinterData.Reference)
                                ),
                    new XElement("Process" //原WriteProcess
                                        , new XElement("ProductId", currentPanel.Panel.ModelName)
                                        , new XElement("PanelId", PrinterCommonSetting.MatchingBasis == UI.CommonSetting.eMatchingBasis.Sequence ? _Ekra_PrinterData.PanelId : _Ekra_PrinterData.PanelBarcode)//看情況選SN或barcode
                                        , new XElement("PanelStatus", /*By pass mode*/bByPass ? "NonInspected" : "Inspected")
                                        , new XElement("Unints"
                                                              , new XElement("Distance", _Ekra_PrinterData.UnitsDistance)
                                                              , new XElement("Angle", _Ekra_PrinterData.UnitsAngle)
                                                              , new XElement("Time", _Ekra_PrinterData.UnitsTime)
                                                              , new XElement("Stretch", "%")//SPI是百分比
                                                     )
                                        , new XElement("CorrectionFeedback"
                                                              //原WriteFiducials
                                                              , new XElement("Fiducials"
                                                                                    , new XAttribute("Count", _Ekra_PrinterData.EkraFMs.Count)
                                                                                    , CreateFiducails(_Ekra_PrinterData, _EkraFiducailOffsetResult) //WriteFiducail
                                                              )
                                                              , new XElement("Stretch", _StretchResult.Stretch)
                                                              , new XElement("PrinterSqueegeeDir", _Ekra_PrinterData.PrintDirection)
                                                              , new XElement("SpiSqueegeeDir", _Ekra_PrinterData.PrintDirection)//原作吃一樣資料，來自Printer
                                                    )
                                        
                                        , new XElement("Defect" //原WirteDefectStatistic
                                                              , new XElement("DefectNum", _DefectStatisticResult.DefectNum)
                                                              , new XElement("TotalNum", _DefectStatisticResult.TotalTestNum)
                                                              , new XElement("Volume"
                                                                                    , new XAttribute("Num", (_DefectStatisticResult.VolumeDefectOver + _DefectStatisticResult.VolumeDefectUnder).ToString())
                                                                                    , new XElement("High", _DefectStatisticResult.VolumeDefectOver)
                                                                                    , new XElement("Low", _DefectStatisticResult.VolumeDefectUnder)
                                                                            )
                                                              , new XElement("Height"
                                                                                    , new XAttribute("Num", (_DefectStatisticResult.HeightDefectOver + _DefectStatisticResult.HeightDefectUnder).ToString())
                                                                                    , new XElement("High", _DefectStatisticResult.HeightDefectOver)
                                                                                    , new XElement("Low", _DefectStatisticResult.HeightDefectUnder)
                                                                            )
                                                              , new XElement("Area"
                                                                                    , new XAttribute("Num", (_DefectStatisticResult.AreaDefectOver + _DefectStatisticResult.AreaDefectUnder).ToString())
                                                                                    , new XElement("High", _DefectStatisticResult.AreaDefectOver)
                                                                                    , new XElement("Low", _DefectStatisticResult.AreaDefectUnder)
                                                                            )
                                                              , new XElement("Bridge", _DefectStatisticResult.BridgeDefect)
                                                      )
                                        , new XElement("Command", bWipe ? "Cleaning" : "NoUse")
                                        , new XElement("PcbResult", bPcbResult?"NG":"GOOD")//由DefectCount算 "GOOD" and "NG"
                                )
                );
                root.Save(path);
                Log4.PrinterLogger.InfoFormat("Save @ {0} [Ekra]", path);
            }
            catch (Exception e)
            {
                Log4.PrinterLogger.InfoFormat("Save @ {0} Exception [Ekra]",  path);
                Log4.PrinterLogger.InfoFormat("{0}", e.Message);
                throw new OutputException(e.Message);
                //return false;
            }
            return true;
        }
        private List<XElement> CreateFiducails(Ekra_PrinterData printerData, EkraFiducailOffsetResult ekraFMOffset)
        {
            List<XElement> fiducailList = new List<XElement>();
            fiducailList.Clear();

            for (int i = 0; i < printerData.EkraFMs.Count; i++)
            {
                string name = printerData.EkraFMs[i].Name;
                System.Windows.Point origin = printerData.EkraFMs[i].P;
                System.Windows.Point offset = ekraFMOffset.FiducialsOffset[i];
                //WriteFiducial(writer, name, origin, offset);
                fiducailList.Add(
                    new XElement("Fiducial"
                                            , new XAttribute("Name", name)
                                            , new XElement("X",new XAttribute("Offset", string.Format("{0:0.000000}", offset.X * 0.001))
                                                             ,new XAttribute("Origin", string.Format("{0:0.000000}", origin.X * 0.001)) )
                                            , new XElement("Y", new XAttribute("Offset", string.Format("{0:0.000000}", offset.Y * 0.001))
                                                             , new XAttribute("Origin", string.Format("{0:0.000000}", origin.Y * 0.001)))
                                )
                    );
            }

           

            return fiducailList;
        }

        public override void UpdateHistory()
        {
            DxHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DxSetting.XAxis.Minimun + DxHistory.Count, _CenterOffsetResult.Dx));
            DyHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DySetting.XAxis.Minimun + DyHistory.Count, _CenterOffsetResult.Dy));
            ThetaHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.ThetaSetting.XAxis.Minimun + ThetaHistory.Count, _RotationResult.Theta));

        }
    }
}
