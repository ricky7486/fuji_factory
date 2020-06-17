using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;

namespace PrinterCenter.Printer.JudgeWipe
{
    public sealed class JudgeWipeCntFailSamePad : IJudgeWipeAlgorithm
    {
        public string CodeName = "W4";
        public int CntFailTimes { get; set; }
        public JudgeWipeCntFailSamePad(int cntFailTimes)
        {
            CntFailTimes = cntFailTimes;
        }
        public eWipeStencilReason Judge(InspectedPanel panel, List<Box> Candidates)
        {
            eWipeStencilReason ret = eWipeStencilReason.NoNeedToWipe;

          
            foreach (var box in panel.FailList())
            {
                if (box.Status == eOverallStatus.SOL_BY_RPASS)
                    continue;

                if (box.ContinueFailCnt >= CntFailTimes)
                {
                    ret = eWipeStencilReason.PrintingResultsAreTooBad;
                    break;
                }
            }
            return ret;
        }
    }
}
