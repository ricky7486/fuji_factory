using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;
using PrinterCenter.UI.CommonSetting;

namespace PrinterCenter.Printer.Algorithm
{

    public sealed class CenterOffsetResult
    {
        //public Point Center;//Theta用
        //public Point Origin;//可從資料得到，故不放在此
        public Point _PadCenter; //過渡資料
        public Point _SolderCenter;//過渡資料
        public Point ShiftedCenter;
        //public Point TotalCenter;
        public double Dx;
        public double Dy;
    }
    public sealed class CenterOffsetCorrectionAlgorithm : CorrectionAlgorithmBase
    {
        private CommonSettingVM Settings;
        /// <summary>
        /// CommonSettingVM
        /// 原先Printer計算流程: PrinterCorrection > WipeAlogrithm(MPM) > 建立MergedData > Update
        /// Update步驟內將 "PrinterCorrection" 算好的資料，再根據註冊表做轉換
        /// 原先註冊表 1.bSolderMoveToPad 2.bCCWRotate 3.nQuandarnt
        /// </summary>
        /// <param name="setting"></param>
        public CenterOffsetCorrectionAlgorithm(CommonSettingVM setting)
        {
            Name = "Center Offset Algorithm";
            Settings = setting;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CandidateBoxes">經過Filter後的Candidates</param>
        /// <param name="panel">取整版資訊時要的，例如原點</param>
        /// <returns></returns>
        public override object Calculate(List<Box> CandidateBoxes, InspectedPanel panel, object para)
        {
            CenterOffsetResult result = new CenterOffsetResult();



            //0. 原點
            Point origin0;

            origin0.X = panel.Panel.FullCadRect.X;
            origin0.Y = panel.Panel.FullCadRect.Y;
            //4. 以板子1/2為pad中心, 計算整板旋轉角 <= 原本註解
            Point originC;
            originC.X = panel.Panel.FullCadRect.Width * 0.5;
            originC.Y = panel.Panel.FullCadRect.Height * 0.5;

            //1. 求出整板PAD中心與錫膏中心
            Point padC;
            padC.X = padC.Y = 0;

            Point solderC;
            solderC.X = solderC.Y = 0;

            foreach (var box in CandidateBoxes)
            {
                Point cadC;
                cadC.X = box.CadRect.X + box.CadRect.Width * 0.5;
                cadC.Y = box.CadRect.Y + box.CadRect.Height * 0.5;

                padC.X += cadC.X;
                padC.Y += cadC.Y;

                solderC.X += cadC.X + box.ShiftGlobalX;
                solderC.Y += cadC.Y + box.ShiftGlobalY;
            }

            padC.X /= CandidateBoxes.Count;
            padC.Y /= CandidateBoxes.Count;

            padC.X -= origin0.X;
            padC.Y -= origin0.Y;

            solderC.X /= CandidateBoxes.Count;
            solderC.Y /= CandidateBoxes.Count;

            solderC.X -= origin0.X;
            solderC.Y -= origin0.Y;

            //**結果 [PadCenter和SolderCenter為過度資料，可以簡化輸出資料]
            //全板pad中心
            result._PadCenter.X = padC.X; 
            result._PadCenter.Y = padC.Y;
            //全板錫膏中心
            result._SolderCenter.X = solderC.X;
            result._SolderCenter.Y = solderC.Y;

            //3. 求出整板偏移量
            double totalDx = solderC.X - padC.X;
            double totalDy = solderC.Y - padC.Y;

            //4. 以板子1/2為pad中心, 計算整板旋轉角
            //double totalCx, totalCy;

            //原程式呼叫到public virtual void Run(BoardNode board = null) 都是null
            //if (board == null)
            //{
            //    totalCx = panel.Panel.FullCadRect.Width * 0.5;
            //    totalCy = panel.Panel.FullCadRect.Height * 0.5;
            //}
            //else
            //{
            //    totalCx = board.CadRect.Width * 0.5;
            //    totalCy = board.CadRect.Height * 0.5;
            //}
            //totalCx = panel.Panel.FullCadRect.Width * 0.5;
            //totalCy = panel.Panel.FullCadRect.Height * 0.5;

            //TotalCenter可從全板資料中得到，故不再撰寫
            //result.TotalCenter.X = panel.Panel.FullCadRect.Width * 0.5;
            //result.TotalCenter.Y = panel.Panel.FullCadRect.Height * 0.5;

            //double shiftPanelCx = totalCx + totalDx;
            //double shiftPanelCy = totalCy + totalDy;

            //**求得偏移後的整版中心
            result.ShiftedCenter.X = originC.X + totalDx; 
            result.ShiftedCenter.Y = originC.Y + totalDy;

            //原本算角度的參數(boxList, 原本origin0 , 整版偏移後的中心, 原本中心) =>原本origin0和 原本中心都可從資料得到
            //Calcualte(boxList, OriginX, OriginY, shiftPanelCx, shiftPanelCy, totalCx, totalCy);//原本這邊算Theta



            //**結果Dx&Dy
            result.Dx = totalDx;
            result.Dy = totalDy;


            //最後轉檔
            if (Settings.IsMoveStencil)//移動鋼板 ， 原 isSolderMoveToPad=true
            {
                result.Dx = padC.X - solderC.X;
                result.Dy = padC.Y - solderC.Y;//soder+correction = pad
           
                //result.Cx = padC.X;
                //result.Cy = padC.Y;//padCxCy must be the centerOfRotate when rotate solder to pad.
            }
            else//pad -> solder
            {
                result.Dx = solderC.X - padC.X;
                result.Dy = solderC.Y - padC.Y;//pad+correction = solder
   
                //result.Cx = solderC.X;
                //result.Cy = solderC.Y;//solderCxy must be the centerOfRotate when rotate pad to solder.
            }
            //象限影響 DX、DY [原BussinessLogicClass GetCorrectionResult]
            if(Settings.IsQuadrent1)
            {
                ;//Not thing need to do
            }else if(Settings.IsQuadrent2)
            {
                result.Dx *= -1;

            }else if(Settings.IsQuadrent3)
            {
                result.Dx *= -1;
                result.Dy *= -1;
            }else if(Settings.IsQuadrent4)
            {
                result.Dy *= -1;
            }

            return result;
        }

   
       
    }
}
