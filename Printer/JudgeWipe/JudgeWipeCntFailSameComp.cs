using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;

namespace PrinterCenter.Printer.JudgeWipe
{
    public sealed class JudgeWipeCntFailSameComp : IJudgeWipeAlgorithm
    {
        public string CodeName = "W5";
        public int CntFailTimes { get; set; }
        public JudgeWipeCntFailSameComp(int cntFailTimes)
        {
            CntFailTimes = cntFailTimes;
        }
        public eWipeStencilReason Judge(InspectedPanel panel, List<Box> Candidates)
        {
            eWipeStencilReason ret = eWipeStencilReason.NoNeedToWipe;


        
            foreach (var board in panel.Panel.Boards)
            {
                foreach(var cmp in board.Components)
                {
                    if (cmp.ConfirmStatus == eConfirmStatus.SOL_FAIL)
                    {
                        if (cmp.ContinueFailCnt >= CntFailTimes)
                        {
                            ret = eWipeStencilReason.PrintingResultsAreTooBad;
                            break;
                        }
                    }
                }
                
            }
            return ret;
        }
    }
}
