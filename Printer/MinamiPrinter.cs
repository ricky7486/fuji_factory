using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;
using PrinterCenter.Printer.Algorithm;
using PrinterCenter.Service;
using System.Xml.Linq;
using PrinterCenter.File;
using PrinterCenter.Log;
using aejw.Network;

namespace PrinterCenter.Printer
{

    public sealed class Minami_PrinterData : IParser
    {
        public string ModelName { get; set; }
        public string PrintTime { get; set; }
        public string SN { get; set; }
        public string Barcode { get; set; }
        public string PrintDirection { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public double StencilThickness { get; set; }
        public bool IsBarcodeMatched(object fileobj, string barcode)
        {
            Minami_PrinterData data = fileobj as Minami_PrinterData;
            if (data.Barcode == barcode)
                return true;
            else
                return false;
        }

        public object Parse(string filepath)
        {
            Minami_PrinterData ret = new Minami_PrinterData();

            try
            {
                XElement Minami = XElement.Load(filepath);
                ret.ModelName = Minami.getValue("ModelName");
                ret.PrintTime = Minami.getValue("PrintTime");
                ret.SN = Minami.getValue("SN");
                ret.Barcode = Minami.getValue("Barcode");
                ret.PrintDirection = Minami.getValue("Direction");


                string sCol = Minami.getAttributeValue("Space", "Column");
                string sRow = Minami.getAttributeValue("Space", "Row");
                int iCol;
                int iRow;
                int.TryParse(sCol, out iCol);
                ret.Column = iCol;
                int.TryParse(sRow, out iRow);
                ret.Row = iRow;
                string sST = Minami.getValue("StencilThickness");
                double dST;
                Double.TryParse(sST, out dST);
                ret.StencilThickness = dST;

            }
            catch (Exception exception)
            {
                Log4.PrinterLogger.ErrorFormat("Minami Printer Data Pasre Exception: {0}", filepath);
                Log4.PrinterLogger.ErrorFormat("Message: {0}", exception.Message);
                return null;
            }

            return ret;
            
        }
    }

    public sealed class MinamiPrinter : PrinterBase, IDisposable
    {
        private string target;
        private SharedFolderWatcher WatchedFolder;
        //private int count = 0;
        private StretchResult _StretchResult;
        private RotationResult _RotationResult;
        private CenterOffsetResult _CenterOffsetResult;
        private DefectStatisticResult _DefectStatisticResult;

        public MinamiPrinter(eAssignedLane_Printer lane) : base(ePrinterVendor.MINAMI, lane)
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


                _CenterOffsetResult = (CenterOffsetResult)cocAlgo.Calculate(Boxes, currentPanel, null);
                _RotationResult = (RotationResult)rcAlgo.Calculate(Boxes, currentPanel, _CenterOffsetResult);
                _StretchResult = (StretchResult)sAlgo.Calculate(null, currentPanel, null);
                _DefectStatisticResult = (DefectStatisticResult)dsAlgo.Calculate(Boxes, currentPanel, null);//=>從failbox來
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
            Minami_PrinterData tmpTool = new Minami_PrinterData();
            return (Minami_PrinterData)Match(currentPanel, (IParser)tmpTool, WatchedFolder);
        }

        public override bool Output(InspectedPanel currentPanel, object file)
        {
            var _Minami_PrinterData = (Minami_PrinterData)file;
            string netDrive;
            if (!PrinterSFSetting.IsOutEnable)
                return false;
            else
                netDrive = WmiDiskHelper.ExtractDiskID(PrinterSFSetting.OutDriveInfo) + @"\";//Disk mapping;
            string path = netDrive + String.Format("{0:yyyyMMddHHmmss}.xml", currentPanel.InspectStartTime);

            try
            {
                DateTime t = currentPanel.InspectStartTime;
                string strTime = string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}.{6:00}",
                    t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, t.Millisecond);

                XElement root = new XElement("SpiData"
                                , new XElement("ModelName", _Minami_PrinterData.ModelName)
                                , new XElement("InspectTime", strTime)
                                , new XElement("SN",_Minami_PrinterData.SN)
                                , new XElement("Barcode",_Minami_PrinterData.Barcode)
                                , new XElement("Direction",_Minami_PrinterData.PrintDirection)
                                , new XElement("Board", new XAttribute("ID","1")
                                                ,new XElement("FidMarkList", CreateFidElements(currentPanel)
                                                ,new XElement("Correction" 
                                                                        , new XAttribute("RotCx", Math.Round((_RotationResult.Center.X * 0.001), 6).ToString())
                                                                        , new XAttribute("RotCy", Math.Round((_RotationResult.Center.Y * 0.001), 6).ToString())
                                                                        , new XElement("X", Math.Round((_CenterOffsetResult.Dx * 0.001), 6).ToString())
                                                                        , new XElement("Y", Math.Round((_CenterOffsetResult.Dy * 0.001), 6).ToString())
                                                                        , new XElement("Theta", Math.Round(_RotationResult.Theta, 6).ToString())
                                                                        , new XElement("Stretch", Math.Round(_StretchResult.Stretch, 6).ToString())
                                                              )               
                                                )
                                )
                    );

                root.Save(path);
                Log4.PrinterLogger.InfoFormat("Save @ {0} [Minami]",  path);
            }
            catch (Exception e)
            {
                Log4.PrinterLogger.InfoFormat("Save @ {0} Exception [Minami]",  path);
                Log4.PrinterLogger.InfoFormat("{0}", e.Message);
                throw new OutputException(e.Message);
                //return false;
            }
            return true;
        }

        //spec規定的Board就是Panel
        private List<XElement> CreateFidElements(InspectedPanel currentPanel)
        {
            List<XElement> elFidList = new List<XElement>();

            int count = 1;
            foreach(var fm in currentPanel.Panel.FiducialMarks)
            {
                var elFM = new XElement("FidMark"
                                , new XAttribute("Name" ,"MARK"+count.ToString())
                                , new XAttribute("X", fm.CadCenter.X )
                                , new XAttribute("Y", fm.CadCenter.Y)
                    );
                elFidList.Add(elFM);
                count++;
            }


            return elFidList;

        }

        public override void UpdateHistory()
        {
            DxHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DxSetting.XAxis.Minimun + DxHistory.Count, _CenterOffsetResult.Dx));
            DyHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DySetting.XAxis.Minimun + DyHistory.Count, _CenterOffsetResult.Dy));
            ThetaHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.ThetaSetting.XAxis.Minimun + ThetaHistory.Count, _RotationResult.Theta));

        }
    }
}
