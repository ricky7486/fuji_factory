using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;

namespace PrinterCenter.Printer.Algorithm
{
    public sealed class EkraFiducailOffsetInputPara
    {

        public EkraFiducailOffsetInputPara(Ekra_PrinterData data, RotationResult rResult, CenterOffsetResult cResult)
        {
            FM = new List<System.Windows.Point>();
            foreach (var fm in data.EkraFMs)
            {
                var point = new System.Windows.Point(fm.P.X, fm.P.Y);
                FM.Add(point);
            }
            Theta = rResult.Theta;
            Center = new System.Windows.Point(rResult.Center.X, rResult.Center.Y);
            Dx = cResult.Dx;
            Dy = cResult.Dy;
        }
        public List<System.Windows.Point> FM { get; set; }
        public double Theta { get; set; }
        public double Dx { get; set; }
        public double Dy { get; set; }

        public System.Windows.Point Center { get; set; }

    }
    public sealed class EkraFiducailOffsetResult
    {
        public List<System.Windows.Point> FiducialsOffset { get; set; }
        public EkraFiducailOffsetResult()
        {
            FiducialsOffset = new List<System.Windows.Point>();
        }
    }
    /// <summary>
    /// Ekra 使用
    /// 移植原程式 EkraPrinter UpdateFiducialsOffset
    /// </summary>
    /// <seealso cref="PrinterCenter.Printer.Algorithm.CorrectionAlgorithmBase" />
    public sealed class EkraFiducailOffset : CorrectionAlgorithmBase
    {
        

        public override object Calculate(List<Box> CandidateBoxes, InspectedPanel panel, object para)
        {
            EkraFiducailOffsetResult ret = new EkraFiducailOffsetResult();
            ret.FiducialsOffset.Clear();
            EkraFiducailOffsetInputPara data = para as EkraFiducailOffsetInputPara;
            double x = 0, y = 0;
            //foreach (System.Windows.Point point in data.EkraFMs)
            foreach(var point in data.FM)
            {
                double angle = data.Theta * Math.PI / 180;
                x = point.X + data.Dx;
                y = point.Y + data.Dy;

                double newX = data.Center.X + ((x - data.Center.X) * Math.Cos(angle)) - ((y - data.Center.Y) * Math.Sin(angle));
                double newY = data.Center.Y + ((x - data.Center.X) * Math.Sin(angle)) + ((y - data.Center.Y) * Math.Cos(angle));

                System.Windows.Point offset = new System.Windows.Point(point.X - newX, point.Y - newY);
                ret.FiducialsOffset.Add(offset);
            }

            return ret;
        }
    }
}
