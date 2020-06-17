using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;

namespace PrinterCenter.Printer.JudgeWipe
{
   
    public sealed class JudgeWipeAvgVol : IJudgeWipeAlgorithm
    {
        public string CodeName = "W1";
        public double AvgVolUnderLimit { get; set; }
        public double AvgVolOverLimit { get; set; }
        public JudgeWipeAvgVol(double underLimit, double overLimit)
        {
            AvgVolUnderLimit = underLimit;
            AvgVolOverLimit = overLimit;
        }
        public eWipeStencilReason Judge(InspectedPanel panel, List<Box> Candidates)
        {
            eWipeStencilReason ret = eWipeStencilReason.NoNeedToWipe;
            double avgVolPercent = 0;
           
            foreach (var box in Candidates)
            {
                avgVolPercent += box.Volume_p;//box.inspectResult.volume.dPercent;
            }

            avgVolPercent /= Candidates.Count();
           
            //Overall average volume: 正常為92%~105% , 若外擴+-5%, 所以預設87%~110%為異常)
            if (avgVolPercent < AvgVolUnderLimit || avgVolPercent > AvgVolOverLimit)
                ret = eWipeStencilReason.PrintingResultsAreTooBad;


            return ret;
        }
    }
}
