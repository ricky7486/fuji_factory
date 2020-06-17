using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;

namespace PrinterCenter.Printer.JudgeWipe
{
    public sealed class JudgeWipeBridgeDefect : IJudgeWipeAlgorithm
    {
        public string CodeName = "W3";
        public JudgeWipeBridgeDefect()
        {

        }
        public eWipeStencilReason Judge(InspectedPanel panel, List<Box> Candidates)
        {
            eWipeStencilReason ret = eWipeStencilReason.NoNeedToWipe;

            foreach (var box in Candidates)
            {
                if (box.Bridge_Status != eIndividualStatus.SOL_OK)
                {
                    ret = eWipeStencilReason.PrintingResultsAreTooBad;
                    break;
                }
            }

            return ret;
        }
    }
}
