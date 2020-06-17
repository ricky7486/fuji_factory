using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;
using PrinterCenter.Service;
using aejw.Network;
using PrinterCenter.Printer.Algorithm;
using System.IO;
using PrinterCenter.Log;

namespace PrinterCenter.Printer
{
    public sealed class YamahaPrinter : PrinterBase, IDisposable
    {
        //private int count = 0;

        private RotationResult _RotationResult;
        private CenterOffsetResult _CenterOffsetResult;
        private PadHAVAvgResult _PadHAVAvgResult;
    
        public YamahaPrinter(eAssignedLane_Printer lane) : base(ePrinterVendor.YAMAHA, lane)
        {

        }
        public void Dispose()
        {

        }
        public override void Calculate(InspectedPanel currentPanel, object file)
        {
            try
            {
                var Boxes = GetCandidateBoxes(currentPanel, PrinterCommonSetting);

                //Calculate
                CenterOffsetCorrectionAlgorithm cocAlgo = new CenterOffsetCorrectionAlgorithm(PrinterCommonSetting.Clone());
                RotationCorrectionAlgorithm rcAlgo = new RotationCorrectionAlgorithm(PrinterCommonSetting.Clone());

                PadHAVAvg yamahaAlgo = new PadHAVAvg();

                _CenterOffsetResult = (CenterOffsetResult)cocAlgo.Calculate(Boxes, currentPanel, null);
                _RotationResult = (RotationResult)rcAlgo.Calculate(Boxes, currentPanel, _CenterOffsetResult);

                _PadHAVAvgResult = (PadHAVAvgResult)yamahaAlgo.Calculate(Boxes, null, null);//YAMAHA需要額外的avg統計
            }
            catch (Exception e)
            {
                throw new CaculateException(e.Message);
            }

        }



        public override object Match(InspectedPanel currentPanel)
        {
            return null;
        }

        public override bool Output(InspectedPanel currentPanel, object file)
        {
            string netDrive;
            if (!PrinterSFSetting.IsOutEnable)
                return false;
            else
                netDrive = WmiDiskHelper.ExtractDiskID(PrinterSFSetting.OutDriveInfo) + @"\";//Disk mapping;

            string path = netDrive + String.Format("{0:yyyyMMddHHmmss}.csv", currentPanel.InspectStartTime);
            //撰寫SPEC定義的
            //CSV format
            //Rows starting with "//" are skipped as comments.
            //Data rows are defined as below
            //Column 1: Character string which indicates Board ID(Up to 256 Char)
            //Column 2: Correction value X(float, rounded to 3 decimal places)
            //Column 3: Correction value Y(float, rounded to 3 decimal places)
            //Column 4: Correction value R(float, rounded to 3 decimal places)
            //Column 5: Total number of pads(Positive integer)
            //Column 6: Number of NG pads(Positive integer)
            //Column 7: Average area ratio(Positive integer)
            //Column 8: Average volume ratio(Positive integer)
            //Column 9: Average height(Positive integer)

            //Sample "Board ID",1.020,2.0e-2,-0.0023e3,1000,10,120,120,0

            try
            {

                if (!System.IO.File.Exists(path))
                {
                    string dir = Path.GetDirectoryName(path);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    System.IO.File.Create(path).Close();
                   

                }
                //Log.Info("CSV file write start.");
                using (System.IO.TextWriter tw = new StreamWriter(path))
                {
                    string line = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                        //GetBarcode(Panel),
                        currentPanel.Panel.PanelBarcode,
                        _CenterOffsetResult.Dx,
                        _CenterOffsetResult.Dy,
                        _RotationResult.Theta,
                        _PadHAVAvgResult.totalPads,
                        _PadHAVAvgResult.totalNGPads,
                        _PadHAVAvgResult.avgArea_pct,
                        _PadHAVAvgResult.avgVolume_pct,
                        _PadHAVAvgResult.avgHeight_pct
                        );
                    tw.WriteLine(line);

                }
            }
            catch (Exception e)
            {

                Log4.PrinterLogger.Error(e.Message);
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
        /// <summary>
        /// YAMAHA 不送檔
        /// </summary>
        public override void Activate()
        {
   
        }

    }
}
