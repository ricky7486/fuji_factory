using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;

namespace PrinterCenter.Printer.Algorithm
{
    public sealed class HanwhaResult
    {
        public HanwhaResult()
        {
            avgHeight_pct = avgArea_pct = avgVolume_pct = 0;
            avgHeight_val = avgArea_val = avgVolume_val;
        }
        public double avgHeight_pct { get; set; }
        public double avgArea_pct { get; set; }
        public double avgVolume_pct { get; set; }

        public double avgHeight_val { get; set; }
        public double avgArea_val { get; set; }
        public double avgVolume_val { get; set; }
    }
    public sealed class HanwhaHAVAvg : CorrectionAlgorithmBase
    {
        public override object Calculate(List<Box> CandidateBoxes, InspectedPanel panel, object para)
        {
            HanwhaResult result = new HanwhaResult();
            foreach (Box box in CandidateBoxes)
            {


                //[Rex]
                result.avgArea_pct += box.Area_p; 
                result.avgHeight_pct += box.Height_p;
                result.avgVolume_pct += box.Volume_p; 
                result.avgArea_val += box.Area_v; 
                result.avgHeight_val += box.Height_v; 
                result.avgVolume_val += box.Volume_v; 
            }
            //[Rex]求出值整版 H.A.V 平均 both % and value
            result.avgArea_pct /= CandidateBoxes.Count;
            result.avgHeight_pct /= CandidateBoxes.Count;
            result.avgVolume_pct /= CandidateBoxes.Count;
            result.avgArea_val /= CandidateBoxes.Count;
            result.avgHeight_val /= CandidateBoxes.Count;
            result.avgVolume_val /= CandidateBoxes.Count;


            return result;
        }
    }
}
