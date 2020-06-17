using PrinterCenterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrinterCenter.Printer.Algorithm
{
    interface ICorrectionAlgorithm
    {
        /// <summary>
        /// Calculates the specified candidate boxes.
        /// </summary>
        /// <param name="CandidateBoxes">需要計算的Boxes，自行先Filter後再傳入</param>
        /// <returns>結果物件</returns>
        object Calculate(List<Box> CandidateBoxes, InspectedPanel panel, object para);
    }
    public abstract class CorrectionAlgorithmBase : ICorrectionAlgorithm
    {
        public string Name { get; set; }


        public abstract object Calculate(List<Box> CandidateBoxes, InspectedPanel panel, object para);

    }


    public struct Point
    {
        public Point(double x,double y)
        {
            X = x;
            Y = y;
        }
        public double X;
        public double Y;
    }
}
