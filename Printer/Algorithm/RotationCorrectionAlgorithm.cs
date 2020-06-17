using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;
using PrinterCenter.UI.CommonSetting;

namespace PrinterCenter.Printer.Algorithm
{
    public sealed class RotationResult
    {
        public double Theta;
        public Point Center;
    }
    public sealed class RotationCorrectionAlgorithm : CorrectionAlgorithmBase
    {
        private CommonSettingVM Settings;
        /// <summary>
        /// CommonSettingVM
        /// 原先Printer計算流程: PrinterCorrection > WipeAlogrithm(MPM) > 建立MergedData > Update
        /// Update步驟內將 "PrinterCorrection" 算好的資料，再根據註冊表做轉換
        /// 原先註冊表 1.bSolderMoveToPad 2.bCCWRotate 3.nQuandarnt
        /// </summary>
        /// <param name="setting">The setting.</param>
        public RotationCorrectionAlgorithm(CommonSettingVM setting)
        {
            Name = "Rotation Algorithm";
            Settings = setting;
        }

        public override object Calculate(List<Box> CandidateBoxes, InspectedPanel panel, object para)
        {
            //Point shiftedC = (Point)para;
            Point shiftedC = (para as CenterOffsetResult).ShiftedCenter;
            Point _padC = (para as CenterOffsetResult)._PadCenter;
            Point _solderC = (para as CenterOffsetResult)._SolderCenter;

            RotationResult result = new RotationResult();
            //0. 原點
            Point origin0;

            origin0.X = panel.Panel.FullCadRect.X;
            origin0.Y = panel.Panel.FullCadRect.Y;
            //4. 以板子1/2為pad中心, 計算整板旋轉角 <= 原本註解
            Point originC;
            originC.X = panel.Panel.FullCadRect.Width * 0.5;
            originC.Y = panel.Panel.FullCadRect.Height * 0.5;



            

            double X0, Y0, X1, Y1;
            double Sum1 = 0, Sum2 = 0;
            foreach (Box box in CandidateBoxes)
            {
                //double padX, padY;
                Point pad;

                pad.X = box.CadRect.X + box.CadRect.Width * 0.5;
                pad.Y = box.CadRect.Y + box.CadRect.Height * 0.5;
                pad.X -= origin0.X;
                pad.Y -= origin0.Y;

                //double solderX, solderY;
                Point solder;
                solder.X = pad.X + box.ShiftGlobalX;
                solder.Y = pad.Y + box.ShiftGlobalY;

                X0 = solder.X - shiftedC.X;
                Y0 = solder.Y - shiftedC.Y;

              
                X1 = pad.X - originC.X;//pad -> solder
                Y1 = pad.Y - originC.Y;//pad -> solder

                Sum1 += X1 * Y0 - X0 * Y1;
                Sum2 += X1 * X0 + Y1 * Y0;
            }

            if (Sum2 == 0)
                return result;//return null會造成後續寫檔錯誤

            //原本的計算在這邊被改變，但return後還是會再改一次
            //Dx = solderCx - padCx;
            //Dy = solderCy - padCy;

            result.Theta = Math.Atan(-Sum1 / Sum2) * 180 / Math.PI;

            result.Center.X = shiftedC.X;
            result.Center.Y = shiftedC.Y;


            //最後轉檔base on CommonSettingVM
            if (Settings.IsMoveStencil)
            {
                result.Theta = -result.Theta;
                result.Center.X = _padC.X;
                result.Center.Y = _padC.Y;//[原註解]padCxCy must be the centerOfRotate when rotate solder to pad.
            }
            else
            {
                result.Center.X = _solderC.X;
                result.Center.Y = _solderC.Y;//[原註解]solderCxy must be the centerOfRotate when rotate pad to solder.
            }


            // [原註解]一般認知的情況下+Theta角為逆時針方向，isCCWRotate應為true。
            if (Settings.IsCCWRotate == false)
                result.Theta *= -1;

   


            return result;
        }

   
    }
}
