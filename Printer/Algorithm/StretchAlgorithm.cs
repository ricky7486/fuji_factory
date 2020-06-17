using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;

namespace PrinterCenter.Printer.Algorithm
{
    public sealed class StretchResult
    {
        public double Stretch;
    }
    /// <summary>
    /// 移植自BusinessLogicClass.cs 
    /// void CalculateStretch()
    /// </summary>
    /// <seealso cref="PrinterCenter.Printer.Algorithm.CorrectionAlgorithmBase" />
    public sealed class StretchAlgorithm : CorrectionAlgorithmBase
    {
        public override object Calculate(List<Box> CandidateBoxes, InspectedPanel panel, object para)
        {
            StretchResult result = new StretchResult();
            List<FiducialMark> markList = panel.Panel.FiducialMarks;

            result.Stretch = 0;
            if (markList.Count >= 2)
            {
                System.Windows.Rect r;

                r = markList[0].IdealRect;
                double originX1 = r.X + r.Width * 0.5;
                double originY1 = r.Y + r.Height * 0.5;
                r = markList[1].IdealRect;
                double originX2 = r.X + r.Width * 0.5;
                double originY2 = r.Y + r.Height * 0.5;

                r = markList[0].ResultRect;
                double realX1 = r.X + r.Width * 0.5;
                double realY1 = r.Y + r.Height * 0.5;
                r = markList[1].ResultRect;
                double realX2 = r.X + r.Width * 0.5;
                double realY2 = r.Y + r.Height * 0.5;

                double distOrigin = Math.Sqrt((originX2 - originX1) * (originX2 - originX1) + (originY2 - originY1) * (originY2 - originY1));
                double distReal = Math.Sqrt((realX2 - realX1) * (realX2 - realX1) + (realY2 - realY1) * (realY2 - realY1));

                result.Stretch = (distReal - distOrigin) / distOrigin;
                return result;
            }
            else
                return result;//return null會造成後續寫檔錯誤
         
        }
    }
}
