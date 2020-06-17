using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;

namespace PrinterCenter.Printer.Algorithm
{
    public sealed class DefectStatisticResult
    {
        public DefectStatisticResult()
        {
            TotalTestNum = DefectNum = WarningNum = 0;
            VolumeDefectOver = VolumeDefectUnder = 0;
            HeightDefectOver = HeightDefectUnder = 0;
            AreaDefectOver = AreaDefectUnder = 0;
            BridgeDefect = NoPasteDefect = 0;
            VolumeWarningOver = VolumeWarningUnder = 0;
            HeightWarningOver = HeightWarningUnder = 0;
            AreaWarningOver = AreaWarningUnder = 0;
        }
        public int TotalTestNum { get; set; }
        public int DefectNum { get; set; }
        public int WarningNum { get; set; }
        public int VolumeDefectOver { get; set; }
        public int VolumeDefectUnder { get; set; }
        public int HeightDefectOver { get; set; }
        public int HeightDefectUnder { get; set; }
        public int AreaDefectOver { get; set; }
        public int AreaDefectUnder { get; set; }
        public int BridgeDefect { get; set; }
        public int NoPasteDefect { get; set; }
        public int VolumeWarningOver { get; set; }
        public int VolumeWarningUnder { get; set; }
        public int HeightWarningOver { get; set; }
        public int HeightWarningUnder { get; set; }
        public int AreaWarningOver { get; set; }
        public int AreaWarningUnder { get; set; }
    }
    public sealed class DefectStatistic : CorrectionAlgorithmBase
    {
        public override object Calculate(List<Box> CandidateBoxes, InspectedPanel panel, object para)
        {
            DefectStatisticResult result = new DefectStatisticResult();
            var failList = panel.FailList();
            var warnList = panel.WarningList();

            foreach (var warn in warnList)
            {
                if (warn.Status == eOverallStatus.SOL_HEIGHTWARNING)
                {
                    if(warn.Height_p > warn.specHeight.dOverWarningPercent)
                        result.HeightWarningOver++;
                    else if (warn.Height_p < warn.specHeight.dOverWarningPercent)
                        result.HeightWarningUnder++;
                }
                else if (warn.Status == eOverallStatus.SOL_VOLUMEWARNING)
                {
                    if (warn.Volume_p > warn.specVolume.dOverWarningPercent)
                        result.VolumeWarningOver++;
                    else if (warn.Volume_p < warn.specVolume.dOverWarningPercent)
                        result.VolumeWarningUnder++;
                }
                else if (warn.Status == eOverallStatus.SOL_AREAWARNING)
                {
                    if(warn.Area_p > warn.specArea.dOverWarningPercent)
                        result.AreaWarningOver++;
                    else if (warn.Area_p < warn.specArea.dOverWarningPercent)
                        result.AreaWarningUnder++;
                }
            }

            foreach (var fail in failList)
            {
                if (fail.Status == eOverallStatus.SOL_BY_RPASS)
                    continue;

                bool bDefect = false;
                bool bWarning = false;
                if (fail.Volume_Status == eIndividualStatus.SOL_UNDER)
                {
                    bDefect = true;
                    result.VolumeDefectUnder++;
                }
                else if (fail.Volume_Status == eIndividualStatus.SOL_OVER)
                {
                    bDefect = true;
                    result.VolumeDefectOver++;
                }
                else if (fail.Volume_Status == eIndividualStatus.SOL_WARNING)
                {
                 
                    //if (r.volume.dPercent > box.specInfo.volume.dOverWarningPercent)
                    //{
                    //    bWarning = true;
                    //    result.VolumeWarningOver++;
                    //}
                    //else if (r.volume.dPercent < box.specInfo.volume.dUnderWarningPercent)
                    //{
                    //    bWarning = true;
                    //    result.VolumeWarningUnder++;
                    //}
                    bWarning = true;
                }

                if (fail.Height_Status == eIndividualStatus.SOL_UNDER)
                {
                    bDefect = true;
                    result.HeightDefectUnder++;
                }
                else if (fail.Height_Status == eIndividualStatus.SOL_OVER)
                {
                    bDefect = true;
                    result.HeightDefectOver++;
                }
                else if (fail.Height_Status == eIndividualStatus.SOL_WARNING)
                {
                    //if (r.height.dPercent > box.specInfo.height.dOverWarningPercent)
                    //{
                    //    bWarning = true;
                    //    result.HeightWarningOver++;
                    //}
                    //else if (r.height.dPercent < box.specInfo.height.dUnderWarningPercent)
                    //{
                    //    bWarning = true;
                    //    result.HeightWarningUnder++;
                    //}
                    bWarning = true;
                }

                if (fail.Area_Status == eIndividualStatus.SOL_UNDER)
                {
                    bDefect = true;
                    result.AreaDefectUnder++;
                }
                else if (fail.Area_Status == eIndividualStatus.SOL_OVER)
                {
                    bDefect = true;
                    result.AreaDefectOver++;
                }
                else if (fail.Area_Status == eIndividualStatus.SOL_WARNING)
                {
                    //if (r.area.dPercent > box.specInfo.area.dOverWarningPercent)
                    //{
                    //    bWarning = true;
                    //    result.AreaWarningOver++;
                    //}
                    //else if (r.area.dPercent < box.specInfo.area.dUnderWarningPercent)
                    //{
                    //    bWarning = true;
                    //    result.AreaWarningUnder++;
                    //}
                    bWarning = true;
                }

                if (fail.Bridge_Status == eIndividualStatus.SOL_NG)
                {
                    bDefect = true;
                    result.BridgeDefect++;
                }

                if (fail.Volume_v == 0 && fail.Area_v == 0 && fail.Height_v == 0)
                {
                    bDefect = true;
                    result.NoPasteDefect++;
                }

                if (bDefect == true)
                    ++result.DefectNum;
                if (bWarning == true)
                    ++result.WarningNum;
            }

            return result;
            
           
        }
    }
}
