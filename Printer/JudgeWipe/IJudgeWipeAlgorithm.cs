using PrinterCenterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrinterCenter.Printer.JudgeWipe
{
    public  enum eWipeStencilReason
    {
        NoNeedToWipe = 0,
        PrintingTimesAreTooMuch = 1,
        PrintingResultsAreTooBad = 2,
    }

    public interface IJudgeWipeAlgorithm
    {
        eWipeStencilReason Judge(InspectedPanel panel, List<Box> Candidates);
    }
    public static class JudgeWipeAlgorithmFactory
    {
        public static IJudgeWipeAlgorithm CreateAlgorithm(string codeName,object[] param)
        {
            switch(codeName)
            {
                case "W1":
                    return new JudgeWipeAvgVol((double)param[0], (double)param[1]);
                case "W2":
                    return new JudgeWipePeakofSinglePad((double)param[0], (double)param[1]);
                case "W3":
                    return new JudgeWipeBridgeDefect();
                case "W4":
                    return new JudgeWipeCntFailSamePad((int)param[0]);
                case "W5":
                    return new JudgeWipeCntFailSameComp((int)param[0]);
                default:
                    return null;

            }
        }
      
    }
    public static class JudgeWipeHelper
    {
        public static eWipeStencilReason JudgeWipeByPriorityStrategy(InspectedPanel currentPanel, List<Box> boxes)
        {
            eWipeStencilReason ret = eWipeStencilReason.NoNeedToWipe;
            var wipeRoutines = PrinterManager.getInstance().JudgeWipeRoutines;
            foreach (var wipe in wipeRoutines)
            {
                ret = wipe.Judge(currentPanel, boxes);
                if (ret != eWipeStencilReason.NoNeedToWipe)
                    break;

            }
            return ret;
        }
    }

}
