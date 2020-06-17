using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;
using PrinterCenter.Service;
using PrinterCenter.Log;
using System.Xml.Linq;
using PrinterCenter.File;
using aejw.Network;
using System.IO;
using PrinterCenter.Printer.Algorithm;
using PrinterCenter.Printer.JudgeWipe;

namespace PrinterCenter.Printer
{
    /// <summary>
    /// 會有的Elements:
    /// ModelName
    /// PrintTime
    /// SN
    /// Barcode
    /// Direction
    /// Thickness
    /// StencilThickness
    /// </summary>
    public sealed class Desen_PrinterData : IParser
    {
        public string ModelName { get; set; }
        public string UnitThickness { get; set; }
        public string StencilThickness { get; set; }
        public string PrintTime { get; set; }

        public string PanelSN { get; set; }

        public string PanelBarcode { get; set; }

        public string Direction { get; set; }

        public string Thickness { get; set; }
    
        public object Parse(string filepath)
        {
            Desen_PrinterData ret = new Desen_PrinterData();

            try
            {
                XElement Desen = XElement.Load(filepath);

                //取得Print_Direction

                ret.ModelName = Desen.getValue("Print_Direction"); //需判斷是否為null

                ret.PrintTime = Desen.getValue("PrintTime");//需判斷是否為null 有特定格式...(cont.)
                /*
                 char[] separator = { '/', ' ', ':', '-' };
                string[] words = readedStr.Split(separator);
                if (words.Count() == 6)
                {
                    int year, month, day, hour, minute, second;
                    if (int.TryParse(words[0], out year) == true &&
                        int.TryParse(words[1], out month) == true &&
                        int.TryParse(words[2], out day) == true &&
                        int.TryParse(words[3], out hour) == true &&
                        int.TryParse(words[4], out minute) == true &&
                        int.TryParse(words[5], out second) == true
                        )
                    {
                        PrintTime = new DateTime(year, month, day, hour, minute, second);
                    }
                }
                 */

                ret.PanelSN = Desen.getValue("SN");

                ret.PanelBarcode = Desen.getValue("Barcode");

                ret.Thickness = Desen.getValue("Thickness");

                ret.StencilThickness = Desen.getValue("StencilThickness");

            }
            catch (Exception exception)
            {
                Log4.PrinterLogger.ErrorFormat("Desen Printer Data Pasre Exception: {0}", filepath);
                Log4.PrinterLogger.ErrorFormat("Message: {0}", exception.Message);
                return null;
            }



            return ret;
        }

        public bool IsBarcodeMatched(object fileobj, string barcode)
        {
            Desen_PrinterData data = fileobj as Desen_PrinterData;
            if (data.PanelBarcode == barcode)
                return true;
            else
                return false;
        }
    }
    public sealed class DesenPrinter : PrinterBase, IDisposable
    {
       
        //private int count = 0;
        private StretchResult _StretchResult;
        private RotationResult _RotationResult;
        private CenterOffsetResult _CenterOffsetResult;
        private SharedFolderWatcher WatchedFolder;
        private string target;
        
        public DesenPrinter(eAssignedLane_Printer lane) : base(ePrinterVendor.DESEN, lane)
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
            Desen_PrinterData tmpTool = new Desen_PrinterData();
            return (Desen_PrinterData)Match(currentPanel, (IParser)tmpTool, WatchedFolder);
        }

        public override bool Output(InspectedPanel currentPanel, object file)
        {
            var _Desen_PrinterData = (Desen_PrinterData)file;
            string netDrive;
            if (!PrinterSFSetting.IsOutEnable)
                return false;
            else
                netDrive = WmiDiskHelper.ExtractDiskID(PrinterSFSetting.OutDriveInfo) + @"\";//Disk mapping;

            string path = netDrive + String.Format("{0:yyyyMMddHHmmss}.xml", currentPanel.InspectStartTime);

            if ((Directory.Exists(path)) == false)
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));


            //原本:
            /*
                <SpiData>
                WriteBasicSection
                WriteUnitSection
                WriteDirection
                WriteFidMark
                WriteCorrection
                WriteWipe

            */
            try
            {
                DateTime t = currentPanel.InspectStartTime;
                string sInspectStartTime = string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}.{6:00}",
                    t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, t.Millisecond);

              

                XElement root = new XElement("SpiData",
                    //WriteBasicSection
                    new XElement("ModelName", currentPanel.Panel.ModelName),
                    new XElement("InspectTime", sInspectStartTime),
                    new XElement("SN", _Desen_PrinterData.PanelSN),
                    new XElement("Barcode", _Desen_PrinterData.PanelBarcode),
                    //WriteUnitSection
                    new XElement("Units",
                                new XElement("Distance",_Desen_PrinterData.UnitThickness),
                                new XElement("Angle", "Degree"),//"Degree":預設
                                new XElement("Stretch","%")//%預設
                                ),  
                    new XElement("Direction", _Desen_PrinterData.Direction),
                   //WriteFidMark
                   GreateFidMarkElement(currentPanel),
                   //WriteCorrection
                   new XElement("Correction",
                                    new XAttribute("RotCx", Math.Round((_RotationResult.Center.X * 0.001), 6).ToString()),
                                    new XAttribute("RotCy", Math.Round((_RotationResult.Center.Y * 0.001), 6).ToString()),
                                    new XElement("X", Math.Round((_CenterOffsetResult.Dx * 0.001), 6).ToString()),
                                    new XElement("Y", Math.Round((_CenterOffsetResult.Dy * 0.001), 6).ToString()),
                                    new XElement("Theta", Math.Round((_RotationResult.Theta * 0.001), 6).ToString()),
                                    new XElement("Stretch", Math.Round(_StretchResult.Stretch    , 6).ToString())
                                ),
                   //WriteWipe
                   new XElement("Wipe", _WipeReason == eWipeStencilReason.NoNeedToWipe ? false : true )
                );
                root.Save(path);
                Log4.PrinterLogger.InfoFormat("Save @ {0} [Desen]", path);
            }
            catch (Exception e)
            {
                Log4.PrinterLogger.InfoFormat("Save @ {0} Exception [Desen]",  path);
                Log4.PrinterLogger.InfoFormat("{0}", e.Message);
                throw new OutputException(e.Message);
                //return false;
            }


            return true;
        }
        private XElement GreateFidMarkElement(InspectedPanel panel)
        {
            var fmlist = panel.Panel.FiducialMarks;
            var eleStart = new XElement("FidMarkList");

            List<XElement> eleFM = new List<XElement>();
            for(int i=0; i< fmlist.Count; i++)
            {
                eleFM.Add(new XElement("FidMark",
                                new XAttribute("Name", "PANELMARK"+(i+1).ToString()),
                                new XAttribute("X", Math.Round(((fmlist[i].CadCenter.X - panel.Panel.FullCadRect.X) * 0.001), 6).ToString()),
                                new XAttribute("Y", Math.Round(((fmlist[i].CadCenter.Y - panel.Panel.FullCadRect.Y) * 0.001), 6).ToString())
                            ));
            }
            if (fmlist.Count != 0)
            {

                foreach (var ele in eleFM)
                    eleStart.Add(ele);
                   
            }
            return eleStart;


        }


        public override void UpdateHistory()
        {
            DxHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DxSetting.XAxis.Minimun + DxHistory.Count, _CenterOffsetResult.Dx));
            DyHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DySetting.XAxis.Minimun + DyHistory.Count, _CenterOffsetResult.Dy));
            ThetaHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.ThetaSetting.XAxis.Minimun + ThetaHistory.Count, _RotationResult.Theta));

        }


    }
}
