using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;

namespace PrinterCenter.Printer.JudgeWipe
{
    public sealed class JudgeWipePeakofSinglePad : IJudgeWipeAlgorithm
    {
        public string CodeName = "W2";
        public double PeakOfSinglePadUnderLimit { get; set; }
        public double PeakOfSinglePadOverLimit { get; set; }
        public JudgeWipePeakofSinglePad(double underLimit, double overLimit)
        {
            PeakOfSinglePadUnderLimit = underLimit;
            PeakOfSinglePadOverLimit = overLimit;
        }
        public eWipeStencilReason Judge(InspectedPanel panel, List<Box> Candidates)
        {
            eWipeStencilReason ret = eWipeStencilReason.NoNeedToWipe;
            foreach(var box in Candidates)
            {


                var value = box.Volume_p; //原: box.inspectResult.volume.dPercent;
                //Maximum/Minimum peak of single pad volume: 正常為55%~140% , 若超出異常)
                if (value < PeakOfSinglePadUnderLimit || value > PeakOfSinglePadOverLimit)
                {
                    ret = eWipeStencilReason.PrintingResultsAreTooBad;
                    break;
                }
            }
            return ret;
        }
    }
}
