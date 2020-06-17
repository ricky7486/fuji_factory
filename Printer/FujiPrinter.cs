using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using aejw.Network;
using PrinterCenter.Localization;
using PrinterCenter.Log;
using PrinterCenter.Printer.Algorithm;
using PrinterCenter.Printer.JudgeWipe;
using PrinterCenter.Service;
using PrinterCenter.UI;
using PrinterCenterData;

namespace PrinterCenter.Printer
{
    public class FujiPrinter : PrinterBase, IDisposable
    {
        public static bool TransferTamp;
        private CenterOffsetResult _CenterOffsetResult;
        private DefectStatisticResult _DefectStatisticResult;
        private RotationResult _RotationResult;
        private StretchResult _StretchResult;
        private int AreaExceeded;
        private int AreaInsufficient;
        private bool bIsMoveStatus = false;
        private int Bridging;
        private string FUJI_ChangeOverMode = "";
        private int HeightExceeded;
        private int HeightInsufficient;
        private int InspectionPassed;
        private string MachineConfigFile = "";
        private string NextMC = "";
        private string NextMC2 = "";
        private string NextPath = "";
        private string OwnMC = "";
        private string OwnPath = "";
        private int PositionDeviation;
        private int RetryCount = 0;
        private int RetryInterval = 0;
        private string sPreMC = "";
        private string sPreMC2 = "";
        private string sProductionMethod = "";

        //Fix or Random
        //PanelID=>Every panel  ,ProgramID=>KanbaMode; Mode=PanelID or ProgramID or PanelID-ProgramID
        private string tempFileName = "";

        private int UnDefinedError;
        private int VolumeExceeded;
        private int VolumeInsufficient;

        public FujiPrinter(eAssignedLane_Printer lane)
            : base(ePrinterVendor.FUJI, lane)
        {
            InspectionPassed = 0;
            VolumeExceeded = 1;
            VolumeInsufficient = 2;
            PositionDeviation = 3;
            Bridging = 4;
            HeightExceeded = 7;
            HeightInsufficient = 8;
            AreaExceeded = 9;
            AreaInsufficient = 10;
            UnDefinedError = 13;
        }

        public override void Activate()
        {
        }

        public override void Calculate(InspectedPanel currentPanel, object file)
        {
            try
            {
                _CenterOffsetResult = null;
                _RotationResult = null;
                _StretchResult = null;
                _DefectStatisticResult = null;

                var Boxes = GetCandidateBoxes(currentPanel, PrinterCommonSetting);

                //Calculate
                CenterOffsetCorrectionAlgorithm cocAlgo = new CenterOffsetCorrectionAlgorithm(PrinterCommonSetting.Clone());
                RotationCorrectionAlgorithm rcAlgo = new RotationCorrectionAlgorithm(PrinterCommonSetting.Clone());
                StretchAlgorithm sAlgo = new StretchAlgorithm();
                DefectStatistic dsAlgo = new DefectStatistic();

                _CenterOffsetResult = (CenterOffsetResult)cocAlgo.Calculate(Boxes, currentPanel, null);
                _RotationResult = (RotationResult)rcAlgo.Calculate(Boxes, currentPanel, _CenterOffsetResult);
                _StretchResult = (StretchResult)sAlgo.Calculate(null, currentPanel, null);
                _DefectStatisticResult = (DefectStatisticResult)dsAlgo.Calculate(Boxes, currentPanel, null);

                //Wipe
                _WipeReason = JudgeWipeHelper.JudgeWipeByPriorityStrategy(currentPanel, Boxes);
            }
            catch (Exception e)
            {
                throw new CaculateException(e.Message);
            }
        }

        public override bool CheckCurrentSharedFolder()
        {
            //AutoLoad
            if (LaneID == eAssignedLane_Printer.Lane1 && ViewModelLocator.Atom.FujiEasyLinkVM.isChangeOverEnableLane1)
            {
                /////////////////////////////////////////////////////////////////////////////
                //InspectedPanel currentPanel = this.InspectedPanels.Dequeue();
                string StationID = PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.GetValue("StationID");
                double ConveyorWidth = double.Parse(PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.GetValue("SuggestConveyorWidthUm"));
                string PanelIDSurface = PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.GetValue("WholePanelSide");
                string PanelBarcode = PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.GetValue("PanelBarcode");
                string OldName = PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.GetValue("FileName");
                /////////////////////////////////////////////////////////////////////////////

                string FileName = ViewModelLocator.Atom.FujiEasyLinkVM.Lane1Current;
                bool KanbanMethod1 = ViewModelLocator.Atom.FujiEasyLinkVM.isKanbanLane1;
                bool PanelIDMethod1 = ViewModelLocator.Atom.FujiEasyLinkVM.isPanelIDLane1;

				bool FUJIFolder = CheckFUJIFolder(FileName);
                int FSFDefinition = ReadFUJI_FSFDefinition(FileName, ref sProductionMethod, ref FUJI_ChangeOverMode, ref RetryCount, ref RetryInterval);
                bool MachineConfig = ReadMachineConfigFile(FileName, ref sPreMC, ref sPreMC2, ref OwnPath, ref OwnMC, ref NextPath, ref NextMC, ref NextMC2, ref MachineConfigFile, StationID);
                

                if (ViewModelLocator.Atom.FujiEasyLinkVM.isPanelIDLane1)
                {
					bool ReadModal = ReadModalFile(2, OwnMC, OwnPath, ConveyorWidth, PanelIDSurface, ref PanelBarcode, sPreMC, RetryInterval, RetryCount);
                    bool WaitAutoLoading = FUJIAutoLoad(OldName, PanelBarcode);
                }
                else if (ViewModelLocator.Atom.FujiEasyLinkVM.isKanbanLane1)
                {
					bool ReadModal = ReadModalFile(1, OwnMC, OwnPath, ConveyorWidth, PanelIDSurface, ref PanelBarcode, sPreMC, RetryInterval, RetryCount);
                    bool WaitAutoLoading = FUJIAutoLoad(OldName, PanelBarcode);
                }
                //if (MachineConfig == true && FUJIFolder == true && WaitAutoLoading == true) { return true; }  else { return false; }
            }
            else if (LaneID == eAssignedLane_Printer.Lane2 && ViewModelLocator.Atom.FujiEasyLinkVM.isChangeOverEnableLane2)
            {
                /////////////////////////////////////////////////////////////////////////////
                //InspectedPanel currentPanel = this.InspectedPanels.Dequeue();
                string StationID = PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.GetValue("StationID");
                double ConveyorWidth = double.Parse(PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.GetValue("SuggestConveyorWidthUm"));
                string PanelIDSurface = PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.GetValue("WholePanelSide");
                string PanelBarcode = PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.GetValue("PanelBarcode");
                string OldName = PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.GetValue("FileName");
                ///////////////////////////////////////////////////////////////////////////

                string FileName2 = ViewModelLocator.Atom.FujiEasyLinkVM.Lane2Current;
                bool KanbanMethod2 = ViewModelLocator.Atom.FujiEasyLinkVM.isKanbanLane2;
                bool PanelIDMethod2 = ViewModelLocator.Atom.FujiEasyLinkVM.isPanelIDLane2;

				bool FUJIFolder = CheckFUJIFolder(FileName2);
                int FSFDefinition = ReadFUJI_FSFDefinition(FileName2, ref sProductionMethod, ref FUJI_ChangeOverMode, ref RetryCount, ref RetryInterval);
                bool MachineConfig = ReadMachineConfigFile(FileName2, ref sPreMC, ref sPreMC2, ref OwnPath, ref OwnMC, ref NextPath, ref NextMC, ref NextMC2, ref MachineConfigFile, StationID);
                

                if (ViewModelLocator.Atom.FujiEasyLinkVM.isPanelIDLane2)
                {
					bool ReadModal = ReadModalFile(2, OwnMC, OwnPath, ConveyorWidth, PanelIDSurface, ref PanelBarcode, sPreMC2, RetryInterval, RetryCount);
                    bool WaitAutoLoading = FUJIAutoLoad(OldName, PanelBarcode);
                }
                else if (ViewModelLocator.Atom.FujiEasyLinkVM.isKanbanLane2)
                {
					bool ReadModal = ReadModalFile(1, OwnMC, OwnPath, ConveyorWidth, PanelIDSurface, ref PanelBarcode, sPreMC2, RetryInterval, RetryCount);
                    bool WaitAutoLoading = FUJIAutoLoad(OldName, PanelBarcode);
                }
                //if (MachineConfig == true && FUJIFolder == true && WaitAutoLoading == true) { return true; }  else { return false; }
            }

            return true;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public override string GetWriteCompImagePath()
        {
            if (LaneID == eAssignedLane_Printer.Lane1)//此程式是Lane幾?
            {
                Log4.PrinterLogger.InfoFormat("[Fuji]GetWriteCompImagePath()- return {0}", ViewModelLocator.Atom.FujiEasyLinkVM.ImagePathLane1);
                return ViewModelLocator.Atom.FujiEasyLinkVM.ImagePathLane1;
            }
            else if (LaneID == eAssignedLane_Printer.Lane2)
            {
                Log4.PrinterLogger.InfoFormat("[Fuji]GetWriteCompImagePath()- return {0}", ViewModelLocator.Atom.FujiEasyLinkVM.ImagePathLane2);
                return ViewModelLocator.Atom.FujiEasyLinkVM.ImagePathLane2;
            }
            else
            {
                Log4.PrinterLogger.InfoFormat("[Fuji]GetWriteCompImagePath()- return string.Empty");
                return string.Empty;
            }
        }

        public override bool IsNeedWriteCompImage()
        {
            bool bIsComfirnStatus = false;
            if (PrinterManager.getInstance().CurrentInspectModeTemp[(int)LaneID - 1].InspectMode == 0
                && PrinterManager.getInstance().CurrentInspectModeTemp[(int)LaneID - 1].InspectResult == 1)
            {
                bIsComfirnStatus = true;
            }
            else if (PrinterManager.getInstance().CurrentInspectModeTemp[(int)LaneID - 1].InspectMode == 1
                && PrinterManager.getInstance().CurrentInspectModeTemp[(int)LaneID - 1].InspectResult == 1)
            {
                bIsComfirnStatus = true;
            }

            if (LaneID == eAssignedLane_Printer.Lane1)//此程式是Lane幾?
            {
                if (ViewModelLocator.Atom.FujiEasyLinkVM.isOutputImageLane1 && bIsComfirnStatus)
                {
                    Log4.PrinterLogger.InfoFormat("[Fuji]IsNeedWriteCompImage()- return true");
                    return true;
                }
            }
            else if (LaneID == eAssignedLane_Printer.Lane2)
            {
                if (ViewModelLocator.Atom.FujiEasyLinkVM.isOutputImageLane2 && bIsComfirnStatus)
                {
                    Log4.PrinterLogger.InfoFormat("[Fuji]IsNeedWriteCompImage()- return true");
                    return true;
                }
            }
            else
            {
                Log4.PrinterLogger.InfoFormat("[Fuji]IsNeedWriteCompImage()- return false");
                return false;
            }
            Log4.PrinterLogger.InfoFormat("[Fuji]IsNeedWriteCompImage()- return false");
            return false;
        }

        public override object Match(InspectedPanel currentPanel)
        {
            return null;
        }

        public override bool MoveToNextSharedFolder()
        {
            if (LaneID == eAssignedLane_Printer.Lane1 && ViewModelLocator.Atom.FujiEasyLinkVM.isChangeOverEnableLane1)
            {
                //string FileName = ViewModelLocator.Atom.FujiEasyLinkVM.Lane1Next;
                bool KanbanMethod1 = ViewModelLocator.Atom.FujiEasyLinkVM.isKanbanLane1;
                bool PanelIDMethod1 = ViewModelLocator.Atom.FujiEasyLinkVM.isPanelIDLane1;
                bool ReNameModal = ReNameModalFile(MachineConfigFile, NextPath, NextMC, tempFileName, RetryInterval, RetryCount);

                if (!ReNameModal)
                {
                    TRMessageBox.Show("Read Fuji Move ModalFile Error ", "@TR7007I".Translate(),
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                    return false; ;//離開檢測流程
                }
            }
            else if (LaneID == eAssignedLane_Printer.Lane2 && ViewModelLocator.Atom.FujiEasyLinkVM.isChangeOverEnableLane2)
            {
                //string FileName = ViewModelLocator.Atom.FujiEasyLinkVM.Lane2Next;
                bool KanbanMethod2 = ViewModelLocator.Atom.FujiEasyLinkVM.isKanbanLane2;
                bool PanelIDMethod2 = ViewModelLocator.Atom.FujiEasyLinkVM.isPanelIDLane2;
                bool ReNameModal = ReNameModalFile(MachineConfigFile, NextPath, NextMC2, tempFileName, RetryInterval, RetryCount);

                if (!ReNameModal)
                {
                    TRMessageBox.Show("Read Fuji Move ModalFile Error ", "@TR7007I".Translate(),
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                    return false; ;//離開檢測流程
                }
            }
            return true;
        }

        public override bool Output(InspectedPanel currentPanel, object file)
        {
            string netDrive;
            if (!PrinterSFSetting.IsOutEnable) { return false; }
            else
            {
                netDrive = WmiDiskHelper.ExtractDiskID(PrinterSFSetting.OutDriveInfo) + @"\";//Disk mapping;
                Log4.PrinterLogger.Info("[FujiTxt_OutPut_Path] = " + netDrive);
            }

            ReadErrorCodeSetting(netDrive);

            StringBuilder OKCompList = new StringBuilder();
            StringBuilder FCCompList = new StringBuilder();//Rpass comp
            StringBuilder NGCompList = new StringBuilder();
            StringBuilder OKPadList = new StringBuilder();
            StringBuilder FCPadList = new StringBuilder();//Rpass pad
            StringBuilder NGPadList = new StringBuilder();

            DateTime NowTime;
            double unitRatio = 0.001;
            bool bValue = true;
            var sBasic = new StringBuilder();
            OKCompList.Clear();
            FCCompList.Clear();
            NGCompList.Clear();
            OKPadList.Clear();
            FCPadList.Clear();
            NGPadList.Clear();
            int NGCompQty = 0;
            int FCCompQty = 0;
            int OKCompQty = 0;
            int NGPadQty = 0;
            int FCPadQty = 0;
            int OKPadQty = 0;

            string sBoardID = "";
            string sTBFlage = "";
            string sTB = "";
            string sImageFileNamePath = "";
            bool bIsComfirnStatus = false;
            // when Panel Barcode not Assign barcode , use board barcode
            if (currentPanel.Panel.PanelBarcode == "NONE")
            {
                Log4.PrinterLogger.Info("[Fuji_PanelBarcode]PanelBarcode = NONE");
                var substractBoardBarcode = currentPanel.Panel.Boards.FirstOrDefault(x => x.Barcode != "NONE");
                if (substractBoardBarcode != null)
                    currentPanel.Panel.PanelBarcode = substractBoardBarcode.Barcode;
            }

            try
            {
                //NowTime = DateTime.Now.ToUniversalTime();//GMT
                ////create folder & create file
                //string sTempFolder = "";
                //string sFolder="";
                //string sFileName="";

                //if (currentPanel.InspectMode == 0 && currentPanel.ConfirmStatus == 0)
                //{
                //        //Comfirn Mode & Inspect
                //        sTempFolder = string.Format(@"{0}\TMP_{1}_{2}", netDrive, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                //        sFolder = string.Format(@"{0}\{1}_{2}", netDrive, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                //        sFileName = string.Format(@"{0}\M_{1}_{2}.txt", sTempFolder, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                //}
                //else if (currentPanel.InspectMode == 0 && currentPanel.InspectResult == 1)
                //{
                //        //Comfirn Mode & Inspect
                //        sTempFolder = string.Format(@"{0}\TMP_{1}_{2}", netDrive, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                //        sFolder = string.Format(@"{0}\{1}_{2}", netDrive, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                //        sFileName = string.Format(@"{0}\H_{1}_{2}.txt", sTempFolder, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                //        bIsComfirnStatus = true;
                //        bIsMoveStatus = bIsComfirnStatus;
                //}
                //else
                //{
                //        //Auto Mode
                //        sTempFolder = string.Format(@"{0}\TMP_{1}_{2}", netDrive, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                //        sFolder = string.Format(@"{0}\{1}_{2}", netDrive, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                //        sFileName = string.Format(@"{0}\{1}_{2}.txt", sTempFolder, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                //}
                //if (!Directory.Exists(sTempFolder))
                //    Directory.CreateDirectory(sTempFolder);

                //if (System.IO.File.Exists(sFileName))
                //    System.IO.File.Delete(sFileName);

                if (currentPanel.InspectMode == 0 && currentPanel.InspectResult == 1)
                {
                    bIsComfirnStatus = true;
                    bIsMoveStatus = bIsComfirnStatus;
                }

                foreach (Board pboard in currentPanel.Panel.Boards)
                {
                    sTBFlage = (pboard.IsTopFace == true) ? "TOP" : "BOTTOM";
                    if (sTBFlage == "TOP")
                        sTB = "T";
                    else
                        sTB = "B";

                    if (pboard.Barcode.CompareTo("NONE") == 0)
                        sBoardID += "" + ","; // board barcde NONE
                    else
                        sBoardID += pboard.Barcode + ",";

                    foreach (Component comp in pboard.Components)
                    {
                        bool bCompFail = false;
                        bool bCompRpass = false;
                        int nCompErrorCategory = 0;
                        foreach (var box in comp.Boxes)
                        {
                            string sBox = string.Format("{0},{1},{2},{3},{4:0.000},{5:0.000},{6:0.000},{7:0.000},{8:0.00},{9:0.000},{10:0.00},{11:0.000},{12:0.000},{13:0.000},",
                                      box.PinNum,
                                      pboard.BoardSequence,
                                      comp.Name,
                                      GetBoxInspectStatus(box),//ErrorCategory
                                      (box.CadCenter.X - currentPanel.Panel.FullCadRect.X) * unitRatio, // 170811 fuji
                                      box.ShiftGlobalX * unitRatio,
                                      (box.CadCenter.Y - currentPanel.Panel.FullCadRect.Y) * unitRatio,
                                      box.ShiftGlobalY * unitRatio,
                                      box.Volume_p,
                                      box.Volume_v,
                                      box.Area_p,
                                      box.Area_v,
                                      box.Height_v * unitRatio,
                                      box.Height_v * unitRatio);

							if (box.Status == eOverallStatus.SOL_PASS || box.Status == eOverallStatus.SOL_WARNING && box.Status != eOverallStatus.SOL_NOT_TEST)
                            {
                                OKPadQty++;
                                OKPadList.AppendLine(sBox);
                                // OKPadQty
                            }
							else if (box.Status == eOverallStatus.SOL_BY_RPASS && box.Status != eOverallStatus.SOL_NOT_TEST)
                            {
                                bCompRpass = true;
                                FCPadQty++;
                                FCPadList.AppendLine(sBox);
                            }
							else if (box.Status != eOverallStatus.SOL_NOT_TEST)
							{
								bCompFail = true;
								NGPadQty++;
								NGPadList.AppendLine(sBox);
								nCompErrorCategory = GetBoxInspectStatus(box);
							}
                        }
                        //2019/10/2前
                        /*if (LaneID == eAssignedLane_Printer.Lane1)
                        {
                            //string PathFolder = string.Format("{0}_{1:yyyyMMddHHmmssff}", currentPanel.Panel.ModelName, currentPanel.InspectStartTime.ToUniversalTime());
                            //sImageFileNamePath = Path.Combine(ViewModelLocator.Atom.FujiEasyLinkVM.ImagePathLane1, PathFolder, currentPanel.Panel.PanelBarcode, sImageFileName);
                            string PathFolder = string.Format("{0}_{1}", currentPanel.Panel.ModelName, currentPanel.Panel.PanelBarcode);
                            string sImageFileName = string.Format("R_{0}_{1}_{2}_{3}_SPI.jpg", string.Format("{0:yyyyMMddHHmmssff}", currentPanel.InspectStartTime.ToUniversalTime()), currentPanel.Panel.PanelBarcode, pboard.BoardSequence, comp.Name);                            
                            sImageFileNamePath = Path.Combine(ViewModelLocator.Atom.FujiEasyLinkVM.ImagePathLane1, PathFolder, currentPanel.InspectStartTime.ToUniversalTime().ToString("yyyyMMddHHmmssff"), sImageFileName);
                        }
                        else if (LaneID == eAssignedLane_Printer.Lane2)
                        {
                            //string PathFolder = string.Format("{0}_{1:yyyyMMddHHmmssff}", currentPanel.Panel.ModelName, currentPanel.InspectStartTime.ToUniversalTime());
                            string PathFolder = string.Format("{0}_{1}", currentPanel.Panel.ModelName, currentPanel.Panel.PanelBarcode);
                            string sImageFileName = string.Format("R_{0}_{1}_{2}_{3}_SPI.jpg", string.Format("{0:yyyyMMddHHmmssff}", currentPanel.InspectStartTime.ToUniversalTime()), currentPanel.Panel.PanelBarcode, pboard.BoardSequence, comp.Name);
                            sImageFileNamePath = Path.Combine(ViewModelLocator.Atom.FujiEasyLinkVM.ImagePathLane2, PathFolder, currentPanel.InspectStartTime.ToUniversalTime().ToString("yyyyMMddHHmmssff"), sImageFileName);
                        }*/
                        //2019/10/2
                        if (LaneID == eAssignedLane_Printer.Lane1)
                        {
                            //string PathFolder = string.Format("{0}_{1:yyyyMMddHHmmssff}", currentPanel.Panel.ModelName, currentPanel.InspectStartTime.ToUniversalTime());
                            //sImageFileNamePath = Path.Combine(ViewModelLocator.Atom.FujiEasyLinkVM.ImagePathLane1, PathFolder, currentPanel.Panel.PanelBarcode, sImageFileName);
                            //string PathFolder = string.Format("{0}_{1}", currentPanel.Panel.ModelName, currentPanel.Panel.PanelBarcode);
                            string sImageFileName = string.Format("R_{0}_{1}_{2}_{3}_SPI.jpg", string.Format("{0:yyyyMMddHHmmssff}", currentPanel.InspectStartTime.ToUniversalTime()), currentPanel.Panel.PanelBarcode, pboard.BoardSequence, comp.Name);
                            //sImageFileNamePath = Path.Combine(ViewModelLocator.Atom.FujiEasyLinkVM.ImagePathLane1, PathFolder, currentPanel.InspectStartTime.ToUniversalTime().ToString("yyyyMMddHHmmssff"), sImageFileName);
                        }
                        else if (LaneID == eAssignedLane_Printer.Lane2)
                        {
                            //string PathFolder = string.Format("{0}_{1:yyyyMMddHHmmssff}", currentPanel.Panel.ModelName, currentPanel.InspectStartTime.ToUniversalTime());
                            string PathFolder = string.Format("{0}_{1}", currentPanel.Panel.ModelName, currentPanel.Panel.PanelBarcode);
                            string sImageFileName = string.Format("R_{0}_{1}_{2}_{3}_SPI.jpg", string.Format("{0:yyyyMMddHHmmssff}", currentPanel.InspectStartTime.ToUniversalTime()), currentPanel.Panel.PanelBarcode, pboard.BoardSequence, comp.Name);
                            sImageFileNamePath = Path.Combine(ViewModelLocator.Atom.FujiEasyLinkVM.ImagePathLane2, PathFolder, currentPanel.InspectStartTime.ToUniversalTime().ToString("yyyyMMddHHmmssff"), sImageFileName);
                        }

                        //if (Para.bOutAllImage == false)
                        //{
                        //    sImageFileName = "";
                        //}

                        string sComp = string.Format("{0},{1},{2},{3}",
                                     pboard.BoardSequence,
                                     comp.Name,
                                     nCompErrorCategory,//Comp ErrorCategory
                                     sImageFileNamePath//sImageFileName
                                     );

                        //string sImageFilePath = string.Format(@"{0}\{1}", sTempFolder, sImageFileName);

                        //[TODO]暫時取消 DS
                        //if (Para.bOutAllImage == true && bIsComfirnStatus == false)
                        //    WriteCompImage(comp, sImageFilePath);

                        if (bCompFail == true)
                        {
                            NGCompQty++;
                            NGCompList.AppendLine(sComp);
                        }
                        else if (bCompRpass == true)
                        {
                            FCCompQty++;
                            FCCompList.AppendLine(sComp);
                        }
						else
                        {
                            OKCompQty++;
                            OKCompList.AppendLine(sComp);
                        }
                    }
                }
                //Get PrintGapX,PrintGapY,PrintGapQ
                //PrinterCandidateCondition candidateCondition = new PrinterCandidateCondition();
                //PrinterCorrectionAlgorithm correctionAlg = new PrinterCorrectionAlgorithm(candidateCondition, pPanel);
                //correctionAlg.Run();
                //沿用APC default value Caculate x,y,theta
                //bool isSolderMoveToPad = MachinePara.getInstance().APCTable.APCPara.bSolderMoveToPad;
                //bool isCCWRotate = MachinePara.getInstance().APCTable.APCPara.bCCWRotate;

                //int PrinterQuadrant = MachinePara.getInstance().PrinterTable.PrinterPara.PrinterQuadrant;
                //int PrinterQuadrant2 = MachinePara.getInstance().PrinterTable.PrinterPara.PrinterQuadrant2;
                //var result = correctionAlg.GetCorrectionResult(isSolderMoveToPad, isCCWRotate, PrinterQuadrant);
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //var result2 = correctionAlg.GetCorrectionResult(isSolderMoveToPad, isCCWRotate, PrinterQuadrant2);
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //eAssignedLane lane = GlobalManager.GetGlobalMgr().LaneID;

                sBoardID = sBoardID.Substring(0, sBoardID.Length - 1);//
                int lane = (LaneID == eAssignedLane_Printer.Lane2) ? 2 : 1;//GlobalManager.GetGlobalMgr().LaneID + 1; // +1 means : cardinality 1

                sBasic.AppendLine("[BasicInfo]");
                sBasic.AppendLine(string.Format("PanelID={0}", currentPanel.Panel.PanelBarcode));
                sBasic.AppendLine(string.Format("BoardQty={0}", currentPanel.Panel.Boards.Count));
                sBasic.AppendLine(string.Format("BoardID={0}", sBoardID));
                sBasic.AppendLine(string.Format("MachineName={0}", currentPanel.StationID));
                sBasic.AppendLine(string.Format("LaneNo={0}", lane));
                sBasic.AppendLine(string.Format("MachineType={0}", "SPI"));
                sBasic.AppendLine(string.Format("StartTime={0}", string.Format("{0:yyyyMMddHHmmssff}", currentPanel.InspectStartTime.ToUniversalTime())));
                sBasic.AppendLine(string.Format("EndTime={0}", string.Format("{0:yyyyMMddHHmmssff}", currentPanel.InspectEndTime.ToUniversalTime())));
                sBasic.AppendLine(string.Format("ProgramName={0}_{1}", currentPanel.Panel.ModelName, sTB));
                sBasic.AppendLine(string.Format("Side={0}", sTBFlage));
                sBasic.AppendLine(string.Format("Revision={0}", currentPanel.SolutionVersionInfo_OriginalCreateVersionNo));
                //
                sBasic.AppendLine("[Result]");
                int nInspectResult = 0;//0->ok,1->Rpass,2->fail
                if (NGCompQty > 0)
                    nInspectResult = 2;
                else if (FCCompQty > 0)
                    nInspectResult = 1;

                sBasic.AppendLine(string.Format("InspectResult={0}", nInspectResult));
                sBasic.AppendLine(string.Format("NGCompQty={0}", NGCompQty));
                sBasic.AppendLine(string.Format("FCCompQty={0}", FCCompQty));
                sBasic.AppendLine(string.Format("OKCompQty={0}", OKCompQty));
                sBasic.AppendLine(string.Format("NGPadQty={0}", NGPadQty));
                sBasic.AppendLine(string.Format("FCPadQty={0}", FCPadQty));
                sBasic.AppendLine(string.Format("OKPadQty={0}", OKPadQty));
                //sBasic.AppendLine(string.Format("PrintGapX={0:0.000}", result.Cx * unitRatio));//Panel Center
                //sBasic.AppendLine(string.Format("PrintGapY={0:0.000}", result.Cy * unitRatio));
                sBasic.AppendLine(string.Format("PrintGapX={0:0.000}", _CenterOffsetResult.Dx * unitRatio));//Panel Center
                sBasic.AppendLine(string.Format("PrintGapY={0:0.000}", _CenterOffsetResult.Dy * unitRatio));

                sBasic.AppendLine(string.Format("PrintGapQ={0:0.0000}", _RotationResult.Theta));
                var content = sBasic.ToString();
                var sb = new StringBuilder();
                sb.Append(content);
                sb.AppendLine("[NGCompList]");
                if (NGCompList.Length > 0)
                    sb.Append(NGCompList.ToString());
                sb.AppendLine("[FCCompList]");
                if (FCCompList.Length > 0)
                    sb.Append(FCCompList.ToString());
                sb.AppendLine("[OKCompList]");
                if (OKCompList.Length > 0)
                    sb.Append(OKCompList.ToString());

                sb.AppendLine("[NGPadList]");
                if (NGPadList.Length > 0)
                    sb.Append(NGPadList.ToString());
                sb.AppendLine("[FCPadList]");
                if (FCPadList.Length > 0)
                    sb.Append(FCPadList.ToString());
                sb.AppendLine("[OKPadList]");
                if (OKPadList.Length > 0)
                    sb.Append(OKPadList.ToString());

                var Allcontent = sb.ToString();
                sb = null;
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				NowTime = currentPanel.InspectStartTime.ToUniversalTime();//DateTime.Now.ToUniversalTime();//GMT
                //create folder & create file
                string sTempFolder = "";
                string sFolder = "";
                string sFileName = "";
                string CopysFileName = "";

                if (currentPanel.InspectMode == 0 && currentPanel.ConfirmStatus == 1 && currentPanel.InspectResult == 0)
                {
                    //Comfirn Mode & Inspect
                    sTempFolder = string.Format(@"{0}\TMP_{1}_{2}", netDrive, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                    sFolder = string.Format(@"{0}\{1}_{2}", netDrive, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                    sFileName = string.Format(@"{0}\M_{1}_{2}.txt", sTempFolder, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                    if (LaneID == eAssignedLane_Printer.Lane1)//此程式是Lane幾?
                    {
                        if (ViewModelLocator.Atom.FujiEasyLinkVM.isCopyLane1)
                        {
                            //2019/10/2
                            Log4.PrinterLogger.InfoFormat("Confirm Inspect 1");
                            CopysFileName = string.Format(@"{0}\M_{1}_{2}.txt", ViewModelLocator.Atom.FujiEasyLinkVM.CopyPathLane1, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                            //CopysFileName = "不知道他要表達什麼.txt";
                            System.IO.File.WriteAllText(CopysFileName, Allcontent, Encoding.Default);
                        }
                    }
                    else if (LaneID == eAssignedLane_Printer.Lane2)
                    {
                        if (ViewModelLocator.Atom.FujiEasyLinkVM.isCopyLane2)
                        {
                            //2019/10/2
                            CopysFileName = string.Format(@"{0}\M_{1}_{2}.txt", ViewModelLocator.Atom.FujiEasyLinkVM.CopyPathLane2, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                            //CopysFileName = "不知道他要表達什麼.txt";
                            System.IO.File.WriteAllText(CopysFileName, Allcontent, Encoding.Default);
                        }
                    }
                }
                else if (currentPanel.InspectMode == 0 && currentPanel.InspectResult == 1)
                {
                    //Comfirn Mode & Inspect
                    sTempFolder = string.Format(@"{0}\TMP_{1}_{2}", netDrive, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                    sFolder = string.Format(@"{0}\{1}_{2}", netDrive, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                    sFileName = string.Format(@"{0}\H_{1}_{2}.txt", sTempFolder, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                    if (LaneID == eAssignedLane_Printer.Lane1)//此程式是Lane幾?
                    {
                        if (ViewModelLocator.Atom.FujiEasyLinkVM.isCopyLane1)
                        {
                            //2019/10/2
                            Log4.PrinterLogger.InfoFormat("Confirm Inspect 2");
                            CopysFileName = string.Format(@"{0}\H_{1}_{2}.txt", ViewModelLocator.Atom.FujiEasyLinkVM.CopyPathLane1, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                            //CopysFileName = "不知道他要表達什麼.txt";
                            System.IO.File.WriteAllText(CopysFileName, Allcontent, Encoding.Default);
                        }
                    }
                    else if (LaneID == eAssignedLane_Printer.Lane2)
                    {
                        if (ViewModelLocator.Atom.FujiEasyLinkVM.isCopyLane2)
                        {
                            //2019/10/2
                            CopysFileName = string.Format(@"{0}\H_{1}_{2}.txt", ViewModelLocator.Atom.FujiEasyLinkVM.CopyPathLane2, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                            //CopysFileName = "不知道他要表達什麼.txt";
                            System.IO.File.WriteAllText(CopysFileName, Allcontent, Encoding.Default);
                        }
                    }
                }
                else
                {
                    //Auto Mode
                    sTempFolder = string.Format(@"{0}\TMP_{1}_{2}", netDrive, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                    sFolder = string.Format(@"{0}\{1}_{2}", netDrive, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                    sFileName = string.Format(@"{0}\{1}_{2}.txt", sTempFolder, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                    if (LaneID == eAssignedLane_Printer.Lane1)//此程式是Lane幾?
                    {
                        if (ViewModelLocator.Atom.FujiEasyLinkVM.isCopyLane1)
                        {
                            Log4.PrinterLogger.InfoFormat("Auto");
                            CopysFileName = string.Format(@"{0}\{1}_{2}.txt", ViewModelLocator.Atom.FujiEasyLinkVM.CopyPathLane1, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                            //CopysFileName = "不知道他要表達什麼.txt";
                            System.IO.File.WriteAllText(CopysFileName, Allcontent, Encoding.Default);
                        }
                    }
                    else if (LaneID == eAssignedLane_Printer.Lane2)
                    {
                        if (ViewModelLocator.Atom.FujiEasyLinkVM.isCopyLane2)
                        {
                            CopysFileName = string.Format(@"{0}\{1}_{2}.txt", ViewModelLocator.Atom.FujiEasyLinkVM.CopyPathLane2, string.Format("{0:yyyyMMddHHmmssff}", NowTime), currentPanel.Panel.PanelBarcode);
                            System.IO.File.WriteAllText(CopysFileName, Allcontent, Encoding.Default);
                        }
                    }
                }

                if (!Directory.Exists(sTempFolder))
                    Directory.CreateDirectory(sTempFolder);

                //RexNote 出圖機制更改
                //if (LaneID == eAssignedLane_Printer.Lane1)//此程式是Lane幾?
                //{
                //    if (ViewModelLocator.Atom.FujiEasyLinkVM.isOutputImageLane1 && bIsComfirnStatus)
                //        PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.WriteCompImageCommand(LaneID,
                //            ViewModelLocator.Atom.FujiEasyLinkVM.ImagePathLane1);

                //}
                //else if (LaneID == eAssignedLane_Printer.Lane2)
                //{
                //    if (ViewModelLocator.Atom.FujiEasyLinkVM.isOutputImageLane2 && bIsComfirnStatus)
                //        PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.WriteCompImageCommand(LaneID,
                //            ViewModelLocator.Atom.FujiEasyLinkVM.ImagePathLane2);
                //}

                if (System.IO.File.Exists(sFileName))
                    System.IO.File.Delete(sFileName);

                System.IO.File.WriteAllText(sFileName, Allcontent, Encoding.Default);
                /////////////////////////////////////////////////////////////////////////////////////////////////////
                //if (LaneID == eAssignedLane_Printer.Lane1)//此程式是Lane幾?
                //{
                //    if (ViewModelLocator.Atom.FujiEasyLinkVM.isCopyLane1 )
                //        System.IO.File.WriteAllText(ViewModelLocator.Atom.FujiEasyLinkVM.CopyPathLane1, Allcontent, Encoding.Default);
                //}
                //else if (LaneID == eAssignedLane_Printer.Lane2)
                //{
                //    if (ViewModelLocator.Atom.FujiEasyLinkVM.isCopyLane2)
                //        System.IO.File.WriteAllText(ViewModelLocator.Atom.FujiEasyLinkVM.CopyPathLane2, Allcontent, Encoding.Default);
                //}
                ////////////////////////////////////////////////////////////////////////////////////////////////////////
                System.Threading.Thread.Sleep(2000);
                //Move Folder
                Directory.Move(sTempFolder, sFolder);
            }
            catch (Exception ex)
            {
                Log4.PrinterLogger.InfoFormat("Exception catched in Fuji SendTextFile, Reason={0}", ex.Message);
                bValue = false;
            }
            return bValue;
        }

        public override void UpdateHistory()
        {
            DxHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DxSetting.XAxis.Minimun + DxHistory.Count, _CenterOffsetResult.Dx));
            DyHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DySetting.XAxis.Minimun + DyHistory.Count, _CenterOffsetResult.Dy));
            ThetaHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.ThetaSetting.XAxis.Minimun + ThetaHistory.Count, _RotationResult.Theta));
        }

        private int GetBoxInspectStatus(Box box)
        {
            int nValue = 0;
            if (box.Status == eOverallStatus.SOL_PASS || box.Status == eOverallStatus.SOL_WARNING || box.Status == eOverallStatus.SOL_BY_RPASS)
                nValue = InspectionPassed;
            else if (box.Status == eOverallStatus.SOL_BRIDGE)
                nValue = Bridging;
            else if (box.Status == eOverallStatus.SOL_OFFSETX || box.Status == eOverallStatus.SOL_OFFSETY)
                nValue = PositionDeviation;
            else if (box.Status == eOverallStatus.SOL_VOLUMEOVER)
                nValue = VolumeExceeded;
            else if (box.Status == eOverallStatus.SOL_VOLUMEUNDER)
                nValue = VolumeInsufficient;
            else if (box.Status == eOverallStatus.SOL_AREAOVER)
                nValue = AreaExceeded;
            else if (box.Status == eOverallStatus.SOL_AREAUNDER)
                nValue = AreaInsufficient;
            else if (box.Status == eOverallStatus.SOL_HEIGHTOVER)
                nValue = HeightExceeded;
            else if (box.Status == eOverallStatus.SOL_HEIGHTUNDER)
                nValue = HeightInsufficient;
            else
                nValue = UnDefinedError; //other

            return nValue;
        }

        private bool ReadErrorCodeSetting(string netDrive)
        {
            Log4.PrinterLogger.Info("ReadErrorCodeSetting--In");
            int ChackPos = 0;
            string FileName;
            if (netDrive != "" || netDrive != null)
            {
                if (int.TryParse(netDrive.Substring(0, 1), out ChackPos))
                {
                    FileName = string.Format(@"\\{0}\SETTING\Definition.ini", netDrive);
                }
                else
                {
                    FileName = string.Format(@"{0}\SETTING\Definition.ini", netDrive);
                }
                
            }
            else
            {
                Log4.PrinterLogger.Info("[Fuji_ReadErrorCode]ReadErrorCodeSetting--FalseOut");
                return false;
            }

            if (System.IO.File.Exists(FileName) == false) 
            {
                Log4.PrinterLogger.Info("[Fuji_ReadErrorCode]ReadErrorCodeSetting Not File--FalseOut");
                return false; 
            }
                
            char[] separator = { '=' };
            try
            {
                using (StreamReader fileStream = new StreamReader(FileName))
                {
                    while (!fileStream.EndOfStream)
                    {
                        string sline = "";
                        sline = fileStream.ReadLine();
                        if (sline == "")
                            break;
                        string[] items = sline.Split(separator);
                        if (items.Length == 2)
                        {
                            if (items[0] == "InspectionPassed")
                                InspectionPassed = Int32.Parse(items[1]);
                            else if (items[0] == "VolumeExceeded")
                                VolumeExceeded = Int32.Parse(items[1]);
                            else if (items[0] == "VolumeInsufficient")
                                VolumeInsufficient = Int32.Parse(items[1]);
                            else if (items[0] == "PositionDeviation")
                                PositionDeviation = Int32.Parse(items[1]);
                            else if (items[0] == "Bridging")
                                Bridging = Int32.Parse(items[1]);
                            else if (items[0] == "HeightExceeded")
                                HeightExceeded = Int32.Parse(items[1]);
                            else if (items[0] == "HeightInsufficient")
                                HeightInsufficient = Int32.Parse(items[1]);
                            else if (items[0] == "AreaExceeded")
                                AreaExceeded = Int32.Parse(items[1]);
                            else if (items[0] == "AreaInsufficient")
                                AreaInsufficient = Int32.Parse(items[1]);
                            else if (items[0] == "UnDefinedError")
                                UnDefinedError = Int32.Parse(items[1]);
                        }
                    }
                    fileStream.Close();
                }
            }
            catch (Exception ex)
            {
                Log4.PrinterLogger.InfoFormat("Exception catched in Fuji ReadErrorCodeSetting, Reason={0}", ex.Message);
            }
            return true;
        }

        #region 確認SharedFolder資料
        private static Mutex mutex = new Mutex();
        public bool FUJIAutoLoad(string OldName, string PanelBarcode)//Retry
        {
            bool mutexFail = mutex.WaitOne();

            Log4.PrinterLogger.Info("Into MThread Mutex");

            bool rtn = true;
            string sFilePath = "";
            bool MathName = LoadXmlBarcode(OldName, PanelBarcode, out sFilePath);

			if (MathName)
			{
				TransferTamp = true;
				PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.AutoLoadXmlFile(false, sFilePath, false, true);
				PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.PLCStart();
			}
			else
			{
				TransferTamp = true;
			}
            //sFilePath = "";
            //OldName = "";

            mutex.ReleaseMutex();
            Log4.PrinterLogger.Info("OutPut MThread Mutex");
            return rtn;

            //string sFilePath = ViewModelLocator.Atom.FujiEasyLinkVM.Lane1XMLLocation;
        }

        //        //System.Windows.Forms.Application.DoEvents();
        //        //System.Threading.Thread.Sleep(200);
        //        //nWaitCount++;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4.PrinterLogger.InfoFormat("Exception catched in WaitAutoLoading_Fuji(), Reason={0}", ex.Message);
        //        return false;
        //    }
        //    return bValue;
        //}//等待前站送板訊號
        public bool LoadXmlBarcode(string OldName, string PanelBarcode, out string sFilePath)
        {
            //string sFilePath;
            if (FindBarcodeRule(PanelBarcode, out sFilePath) == true)
            {
                string sFileName = "";
                //do work here
                string sfn = sFilePath.Substring(sFilePath.LastIndexOf("\\"), sFilePath.Length - sFilePath.LastIndexOf("\\"));
                sfn = sfn.Replace("\\", "");

                if (sfn.Contains(".xmlx") == true) { sFileName = sfn.Replace(".xmlx", ""); }
                else if (sfn.Contains(".xml") == true) { sFileName = sfn.Replace(".xml", ""); }

                if (sFileName == OldName.Replace(".xml", "")) { return false; } // 相同檔案 不切換程式
                else { return true; }
            }
            else
            {
                return false;
            }
        }

        private bool CheckFUJIFolder(string path)
        {
            string sFileFolder = "";
            int ChackPos = 0;
            try
            {
                if (int.TryParse(path.Substring(0, 1), out ChackPos))
                {
                    sFileFolder = string.Format(@"\\{0}\SPI", path);
                }
                else
                {
                    sFileFolder = string.Format(@"{0}\SPI", path);
                }

                if (Directory.Exists(sFileFolder))
                {
                    return true;
                }
                else
                {
                    Log4.PrinterLogger.InfoFormat("Exception catched in check Folder, Reason= False");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log4.PrinterLogger.InfoFormat("Exception catched in Fuji ReadFSFDefinition{0}, Reason={1}", sFileFolder, ex.Message);
                return false;
            }
        }

        //        //int lane = (LaneID == eAssignedLane_Printer.Lane2) ? 2 : 1;
        //        //if (PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.BufferBoardOn()) //前站送板訊號
        //        //{
        //            Log4.PrinterLogger.InfoFormat("To return true,WaitAutoLoading_Fuji=>ApcBufferBoardOn() == true");
        //            //Panel Mode or All
        //            if (model == 1)
        //            {
        //                bValue = true;
        //                //break;
        //            }
        //            if (model == 2 || model == 3)
        //            {
        //                Log4.PrinterLogger.InfoFormat("WaitAutoLoading_Fuji=>autoloadPrj(2)");
        //                if (ReadModal == true)
        //                {
        //                    Log4.PrinterLogger.InfoFormat("WaitAutoLoading_Fuji=>autoloadPrj(2)==>FUJIAutoLoad");
        //                    bValue = FUJIAutoLoad();
        //                    Log4.PrinterLogger.InfoFormat("WaitAutoLoading_Fuji=>autoloadPrj(2)==>FUJIAutoLoad={0}", bValue);
        //                    //break;
        //                }
        //                else
        //                {
        //                    System.Windows.MessageBoxResult msgBoxResult = PrinterCenter.UI.TRMessageBox.Show("No PanelID File,  Continuse Inspecting ! ",
        //                     "",
        //                    System.Windows.MessageBoxButton.YesNo,
        //                    System.Windows.MessageBoxImage.Question);
        //                    if (msgBoxResult == System.Windows.MessageBoxResult.Yes)
        //                    {
        //                        Log4.PrinterLogger.InfoFormat("Fuji No PanelID File,Continuse Inspecting==Yes");
        //                        bValue = true;
        //                        //break;
        //                    }
        //                    else
        //                    {
        //                        Log4.PrinterLogger.InfoFormat("Fuji No PanelID File,Continuse Inspecting==No");
        //                        bValue = false;
        //                        //break;
        //                    }
        //                }
        //            }
        //        //}
        private bool FindBarcodeRule(string sLoadBarcode, out string sFilePath)
        {
            bool bRetval = false;
            sFilePath = "";
            if (sLoadBarcode == "" || sLoadBarcode == null || sLoadBarcode == "NONE") { return bRetval; }

            var NewFileName = ViewModelLocator.Atom.FujiEasyLinkVM.MappingList;
            for (int i = 0; i < NewFileName.Count; i++)
            {
                var dataBarcodeRule = NewFileName[i].Key;
                var dataXmlFileName = NewFileName[i].Value;
                //if (Mapping(sLoadBarcode, dataBarcodeRule) == true)
                if (sLoadBarcode == dataBarcodeRule)
                {
                    sFilePath = dataXmlFileName;
                    bRetval = true;
                    break;
                }
            }
            return bRetval;
        }

        private int ReadFUJI_FSFDefinition(string path, ref string sProductionMethod, ref string FUJI_ChangeOverMode, ref  int nRetryCount, ref int nRetryInterval)
        {
            int nValue = -1;
            int ChackPos = 0;
            string FileName = "";
            //2019/10/2
            string FileNameDrive = "";
            string FSFnetDrive = "";

            try
            {
                //2019/10/2
                FSFnetDrive = WmiDiskHelper.ExtractDiskID(path) + @"\";
                Log4.PrinterLogger.Info("[FujiReadFuji_FSFDefinition1] = " + FSFnetDrive);
                FileNameDrive = String.Format(@"{0}\FSFDefinition.ini",FSFnetDrive);
                Log4.PrinterLogger.Info("[FujiReadFuji_FSFDefinition2] = " + FileNameDrive);

                if (int.TryParse(path.Substring(0, 1), out ChackPos))
                {
                    //2019/10/2
                    //FileName = string.Format(@"\\{0}\SPI\FSFDefinition.ini", path);
                    FileName = string.Format(@"{0}\FSFDefinition.ini", path);
                    Log4.PrinterLogger.Info("[FujiReadFuji_FSFDefinition] = " + FileName);
                }
                else
                {
                    //2019/10/2
                    //FileName = string.Format(@"{0}\SPI\FSFDefinition.ini", path);
                    FileName = string.Format(@"{0}\FSFDefinition.ini", path);
                    Log4.PrinterLogger.Info("[FujiReadFuji_FSFDefinition] = " + FileName);
                }
            }
            catch (Exception ex)
            {
                Log4.PrinterLogger.InfoFormat("Exception catched in Fuji ReadFSFDefinition{0}, Reason={1}", FileName, ex.Message);
                return nValue;
            }

            try
            {
                if (System.IO.File.Exists(FileName) == false)
                {
                    //2019/10/2
                    //Log4.PrinterLogger.Info("[Fuji_FSFDefinition]ReadFUJI_FSFDefinition Not File");
                    //return nValue;
                    Log4.PrinterLogger.Info("[Fuji_FSFDefinition]ReadFUJI_FSFDefinition Not File1");
                    FileName = FileNameDrive;
                    if(System.IO.File.Exists(FileName) == false)
                    {
                        Log4.PrinterLogger.Info("[Fuji_FSFDefinition]ReadFUJI_FSFDefinition Not File2");
                        return nValue;
                    }

                }

                char[] separator = { '=' };
                using (StreamReader fileStream = new StreamReader(FileName))
                {
                    while (!fileStream.EndOfStream)
                    {
                        string sline = "";
                        sline = fileStream.ReadLine();
                        if (sline == "")
                            break;
                        string[] items = sline.Split(separator);
                        if (items.Length == 2)
                        {
                            if (items[0] == "RetryCount")
                                nRetryCount = Int32.Parse(items[1]);
                            else if (items[0] == "RetryInterval")
                                nRetryInterval = Int32.Parse(items[1]);
                            else if (items[0] == "ProductionMethod")
                                sProductionMethod = items[1];
                            else if (items[0] == "Mode")
                                FUJI_ChangeOverMode = items[1];
                        }
                    }
                    fileStream.Close();
                }

                if (FUJI_ChangeOverMode == "ProgramID")
                    nValue = 1;
                else if (FUJI_ChangeOverMode == "PanelID")
                    nValue = 2;
                else if (FUJI_ChangeOverMode == "PanelID-ProgramID")
                    nValue = 3;
                else
                    nValue = 0; // default
            }
            catch (Exception ex)
            {
                Log4.PrinterLogger.InfoFormat("Exception catched in Fuji ReadFSFDefinition, Reason={0}", ex.Message);
                return nValue;
            }
            return nValue;
        }

        private bool ReadMachineConfigFile(string path, ref string sPreMC, ref string sPreMC2, ref string OwnPath, ref string OwnMC, ref string NextPath, ref string NextMC, ref string NextMC2, ref string MachineConfigFile, string StationID)
        {
            string FileName = "";
            //2019/10/2
            string FileNameDrive = "";
            string FSFnetDrive = "";
            int ChackPos = 0;
            //InspectedPanel currentPanel = new InspectedPanel();
            int lane = (LaneID == eAssignedLane_Printer.Lane2) ? 2 : 1;
            MachineConfigFile = StationID + string.Format("@{0}config.ini", lane); ;

            try
            {
                //2019/10/2
                FSFnetDrive = WmiDiskHelper.ExtractDiskID(path) + @"\";
                Log4.PrinterLogger.Info("[FujiReadFuji_ReadMachineConfigFile1] = " + FSFnetDrive);
                FileNameDrive = String.Format(@"{0}\{1}", FSFnetDrive,MachineConfigFile);
                Log4.PrinterLogger.Info("[FujiReadFuji_ReadMachineConfigFile2] = " + FileNameDrive);


                if (int.TryParse(path.Substring(0, 1), out ChackPos))
                {
                    //2019/10/2
                    //FileName = string.Format(@"\\{0}\{1}", path, MachineConfigFile);
                    FileName = string.Format(@"{0}\{1}", path, MachineConfigFile);
                    Log4.PrinterLogger.Info("[FujiReadFuji_ReadMachineConfigFile3] = " + FileName);
                }
                else
                {
                    //2019/10/2
                    //FileName = string.Format(@"{0}\SPI\{1}", path, MachineConfigFile);
                    FileName = string.Format(@"{0}\{1}", path, MachineConfigFile);
                    Log4.PrinterLogger.Info("[FujiReadFuji_ReadMachineConfigFile4] = " + FileName);
                }
            }
            catch (Exception ex)
            {
                Log4.PrinterLogger.InfoFormat("Exception catched in Fuji ReadMachineConfig{0}, Reason={1}", FileName, ex.Message);
                return false;
            }

            try
            {
                if (System.IO.File.Exists(FileName) == false)
                {
                    Log4.PrinterLogger.Info("[Fuji_ReadMachineConfigFile]ReadMachineConfigFile1 Not File");
                    //2019/10/2
                    FileName = FileNameDrive;
                    if(System.IO.File.Exists(FileName) == false)
                    {
                        Log4.PrinterLogger.Info("[Fuji_ReadMachineConfigFile]ReadMachineConfigFile2 Not File");
                        return false;
                    }
                    //return false;
                }

                char[] separator = { '=' };
                using (StreamReader fileStream = new StreamReader(FileName))
                {
                    while (!fileStream.EndOfStream)
                    {
                        string sline = "";
                        sline = fileStream.ReadLine();
                        if (sline == "")
                            break;
                        string[] items = sline.Split(separator);
                        if (items.Length == 2)
                        {
                            if (items[0] == "PreMC")
                                sPreMC = items[1];
                            else if (items[0] == "PreMC2")
                                sPreMC2 = items[1];
                            else if (items[0] == "OwnPath")//
                                OwnPath = items[1];
                            else if (items[0] == "OwnMC") //MachineName
                                OwnMC = items[1];
                            else if (items[0] == "NextPath")
                                NextPath = items[1];
                            else if (items[0] == "NextMC")
                                NextMC = items[1];
                            else if (items[0] == "NextMC2")
                                NextMC2 = items[1];
                        }
                    }
                    fileStream.Close();
                }
            }
            catch (Exception ex)
            {
                Log4.PrinterLogger.InfoFormat("Exception catched in Fuji ReadMachineConfigFile, Reason={0}", ex.Message);
                return false;
            }
            return true;
        }

        private bool ReadModalFile(int ChangeOverMode, string OwnMC, string OwnPath, 
			double ConveyorWidth, string PanelIDSurface, ref string PanelBarcode, string PreMC,int nRetryInterval, int nRetryCount)
        {
            int lane = (LaneID == eAssignedLane_Printer.Lane2) ? 2 : 1;
			int nWaitCount = 0;
            string FileName = "";
			if (PanelIDSurface == "T") { PanelIDSurface = "Top"; }
			else { PanelIDSurface = "Bottom"; }

            //string MachineConfigFile = "";

            //InspectedPanel currentPanel = new InspectedPanel();
            //var ConveyorWidth = currentPanel.SuggestConveyorWidthUm;
            //var PanelIDSurface = currentPanel.Panel.WholePanelSide;
            //var PanelBarcode = currentPanel.Panel.PanelBarcode;

            if (ChangeOverMode == 2) //PanelID =>2
            {
                //2019/10/2
                MachineConfigFile = OwnMC + string.Format("@{0}.txt", lane);
                FileName = string.Format(@"{0}\{1}\{2}", OwnPath, OwnMC, MachineConfigFile);
                tempFileName = FileName;
                //MachineConfigFile = PreMC + string.Format("@{0}.txt", lane);
                //FileName = string.Format(@"{0}\{1}\{2}", OwnPath, OwnMC, MachineConfigFile);
                //tempFileName = string.Format(@"{0}\{1}\temp_{2}", OwnPath, OwnMC, MachineConfigFile);
            }
            else if (ChangeOverMode == 1) //Kanba=>1
            {
                //MachineConfigFile = string.Format("ProgramID@{0}.txt", lane);
                //2019/10/2
                MachineConfigFile = OwnMC + string.Format("@{0}.txt", lane);
                //MachineConfigFile = PreMC + string.Format("@{0}.txt", lane);
                FileName = string.Format(@"{0}\{1}\{2}", OwnPath, OwnMC, MachineConfigFile);

                //tempFileName = string.Format(@"{0}\{1}\temp_{2}", OwnPath, OwnMC, MachineConfigFile);
                tempFileName = FileName;
            }

            //if (PanelBarcode == "NONE") { PrinterCenter.UI.TRMessageBox.Show("PanelBarcode = NONE;Please Check PanelBarcode"); }
            try
            {
                if (System.IO.File.Exists(FileName) == false)
                {
                    //2019/10/2
					//while (true)
					//{
					//	nWaitCount++;
					//	if (nWaitCount > nRetryCount)
					//	{
							using (StreamWriter sw = new StreamWriter(FileName))//為第一站自行產生檔案丟至下站
							{
								sw.WriteLine(PanelBarcode);
								sw.WriteLine("[PanelInfo]");
								sw.WriteLine("SizeY=" + (ConveyorWidth / 1000).ToString("f3"));
								sw.WriteLine("PanelIDSurface=" + PanelIDSurface);
							}
							Log4.PrinterLogger.Info("[Fuji_ReadModalFile]ReadModalFile Not File => Make One");
						//	break;
						//}

					//	if (System.IO.File.Exists(FileName)) { break; }

					//	if (PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.ESCFlow())//
					//	{
					//		Log4.PrinterLogger.InfoFormat("ReadModalFile_Fuji InEscape() is true, going to break and return false");
					//		break;
					//	}

					//	System.Windows.Forms.Application.DoEvents();
					//	System.Threading.Thread.Sleep(nRetryInterval);
					//}
                }

                FileStream LockFileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                using (StreamReader fileStream = new StreamReader(LockFileStream))
                {
                    while (!fileStream.EndOfStream)
                    {
                        string sline = "";
                        sline = fileStream.ReadLine();
                        if (sline != "")
                        {
                            PanelBarcode = sline;
                            Log4.PrinterLogger.InfoFormat("Fuji ReadModalFile, PanelID={0}", sline);
                            break;
                        }
                    }
                    fileStream.Close();
                }

				if (ChangeOverMode == 2 || ChangeOverMode == 1 || ChangeOverMode == 3) //PanelID =>2
                {
                    //2019/10/2
                    //if (System.IO.File.Exists(tempFileName))
                    //{
                    //    Log4.PrinterLogger.Info("[Fuji_ReadModalFile]ReadModalFile Have The Same File");
                    //    string Dt = System.IO.File.GetLastWriteTime(tempFileName).ToString("yyyyMMddhhmmss");
                    //    string RenameT = tempFileName + "_" + Dt;
                    //    System.IO.File.Move(tempFileName, RenameT);
                    //}
                    if (TransferTamp == true)
                    {
                        //2019/10/2
                        Log4.PrinterLogger.Info("[Fuji_ReadModelFile]ReadModalFile Rename to temp_file");
                        //System.IO.File.Move(FileName, tempFileName);
                        TransferTamp = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log4.PrinterLogger.InfoFormat("Exception catched in Fuji CheckModalFile, Reason={0}", ex.Message);
                return false;
            }
            return true;
        }

        private bool ReNameModalFile(string MachineConfigFile, string NextPath, string NextMC, string tempFileName, int nRetryInterval, int nRetryCount)
        {
            bool bValue = true;
            int lane = (LaneID == eAssignedLane_Printer.Lane2) ? 2 : 1;

            //2019/10/2
            //string FileName = string.Format(@"{0}\{1}\{2}", NextPath, NextMC, MachineConfigFile);
            //string FolderName = string.Format(@"{0}\{1}", NextPath, NextMC);
            string FileName = "";
            string FolderName = "";
            int ChackPos = 0;
            //2019/10/2
            //if (NextMC == "" || NextPath == "")
            //{
            //    if (System.IO.File.Exists(FileName) == true) { System.IO.File.Delete(FileName); }
            //    return true;
            //}
            try
            {
                if(int.TryParse(NextPath.Substring(0,1), out ChackPos))
                {
                    FileName = String.Format(@"{0}\{1}\{2}",NextPath,NextMC,MachineConfigFile);
                    FolderName = String.Format(@"{0}\{1}", NextPath, NextMC);
                    Log4.PrinterLogger.InfoFormat("Fuji ReNameModal FileName ={0}, FolderName ={1}", FileName,FolderName);
                }else
                {
                    FileName = String.Format(@"{0}\{1}\{2}", NextPath, NextMC, MachineConfigFile);
                    FolderName = String.Format(@"{0}\{1}", NextPath, NextMC);
                    Log4.PrinterLogger.InfoFormat("Fuji ReNameModal FileName ={0}, FolderName ={1}", FileName, FolderName);
                }

            }catch(Exception e)
            {

            }

            int nWaitCount = 0;
            string sErrorMessage = "";
            try
            {
                while (true)
                {
                    nWaitCount++;
                    if (nWaitCount > nRetryCount) { break; }

                    if (Directory.Exists(FolderName))
                    {
                        if (System.IO.File.Exists(FileName))//File is not exist is right, exist is error
                        {
                            System.IO.File.Delete(FileName);
                            bValue = true;
                            
                            Log4.PrinterLogger.InfoFormat("Fuji ReNameModal FileName ={0} the same", FileName);
                            break;
                        }
                        else
                        {
                            bValue = true;
                            Log4.PrinterLogger.InfoFormat("Fuji ReNameModal FileName ={0} not same", FileName);
                            break;
                        }
                    }
                    else
                    {
                        bValue = false;
                        Log4.PrinterLogger.ErrorFormat("ReNameModalFile_Fuji is Folder not Exist Error");
                        sErrorMessage = string.Format("{0} is Folder not Exist Error", FolderName);
                    }

                    if (PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.ESCFlow())//
                    {
                        Log4.PrinterLogger.InfoFormat("ReNameModalFile_Fuji InEscape() is true, going to break and return false");
                        break;
                    }

                    System.Windows.Forms.Application.DoEvents();
                    System.Threading.Thread.Sleep(nRetryInterval);
                }

                if (bValue == false)
                {
                    PrinterCenter.UI.TRMessageBox.Show(sErrorMessage, "@TR7007I".Translate()
                        , System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Stop);////
                }
                //2019/10/2原本
                //if (System.IO.File.Exists(tempFileName) == false) { return false; }
                //System.IO.File.Move(tempFileName, FileName);
                //2019/10/2現在
                if (System.IO.File.Exists(tempFileName) == false)
                {
                    Log4.PrinterLogger.ErrorFormat("Fuji ReNameModal tempFileName={0} not have", tempFileName);
                    return false;
                }
                try
                {
                    FileName = String.Format(@"{0}\{1}\{2}",NextPath,NextMC,NextMC+"@1.txt");//轉檔名
                    System.IO.File.Move(tempFileName, FileName);
                    Log4.PrinterLogger.InfoFormat("Fuji ReNameModal tempFileName={0} Move Great = {1}", tempFileName, FileName);
                }
                catch
                {
                    Log4.PrinterLogger.ErrorFormat("Fuji ReNameModal tempFileName={0} Move Fail", tempFileName);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log4.PrinterLogger.InfoFormat("Exception catched in Fuji CheckModalFile, Reason={0}", ex.Message);
                return false;
            }
            return bValue;
        }

        //public bool autoloadPrj(int ChangeOverMode, string OwnMC, string OwnPath)//Kanba=>1, PanelID=>2
        //{
        //	bool bValue = true;
        //	if (ReadModalFile(ChangeOverMode, OwnMC, OwnPath) == false) //Get PanelID
        //	{
        //		bValue = false;
        //	}
        //	return bValue;
        //}

        //public bool AutoLoading_Fuji()
        //{
        //    Log4.PrinterLogger.InfoFormat("HoldPanel.StageFrontConv(), ∵ closedLoop.WaitAutoLoading_Fuji() => Enable");
        //    //closedLoop.Placer.PanelID = ""; //clean PanelID
        //    InspectedPanel currentPanel = new InspectedPanel();
        //    currentPanel.Panel.PanelBarcode = "";
        //    if (WaitAutoLoading_Fuji() == false)
        //    {
        //        Log4.PrinterLogger.InfoFormat("To return false from HoldPanel.StageFrontConv(), ∵ closedLoop.WaitAutoLoading_Fuji() == false");
        //        return false;
        //    }
        //    return true;
        //}

        //public bool WaitAutoLoading_Fuji(bool ReadModal, int model, string PanelBarcode)
        //{
        //    bool bValue = false;
        //    //InspectedPanel currentPanel = new InspectedPanel();
        //    Log4.PrinterLogger.InfoFormat("HoldPanel.StageFrontConv(), ∵ closedLoop.WaitAutoLoading_Fuji() => Enable");

        //    PanelBarcode = ""; //clean PanelID
        //    //int nWaitCount = 0;

        //    try
        //    {
        //        Log4.PrinterLogger.InfoFormat("Enter WaitAutoLoading_Fuji() 註：等待收到前站送板訊號後，繼續流程");

        //        //while (true)
        //        //{
        //        //if (PrinterManager.getInstance().PrinterDuplexServiceInstance.Callback.ESCFlow())
        //        //{
        //        //    Log4.PrinterLogger.InfoFormat("WaitAutoLoading_Fuji InEscape() is true, going to break and return false");
        //        //    //break;
        //        //}

        //        //Kanba Mode or All
        //        if (model == 1 || model == 3)
        //        {
        //            Log4.PrinterLogger.InfoFormat("WaitAutoLoading_Fuji=>autoloadPrj(1)");
        //            if (ReadModal == true)
        //            {
        //                Log4.PrinterLogger.InfoFormat("WaitAutoLoading_Fuji=>autoloadPrj(1)==FUJIAutoLoad");

        //                bValue = FUJIAutoLoad();
        //                Log4.PrinterLogger.InfoFormat("WaitAutoLoading_Fuji=>autoloadPrj(1)==>FUJIAutoLoad={0}", bValue);
        //                //break; // Kanba Modal 讀取後不跳離 繼續等待 直到前站送板訊號
        //            }
        //        }
        ////Mapping File
        //private bool Mapping(string sBarcode, string sBarcodeRule)
        //{
        //    bool bRetVal = true;
        //    bool bstarMark = false; //判斷是否有"*"
        //    StringBuilder sbBarcodeRule = new StringBuilder(sBarcodeRule);
        //    StringBuilder sbBarcode = new StringBuilder(sBarcode);
        //    int ctr;
        //    for (ctr = 0; ctr < sBarcodeRule.Length; ctr++) //Barcode Lengrh
        //    {
        //        if (ctr > (sBarcode.Length - 1))
        //        {
        //            bRetVal = false;
        //            break;
        //        }

        //        char chBarcodeRule = sbBarcodeRule[ctr];
        //        char chBarcode = sbBarcode[ctr];
        //        if (chBarcodeRule.ToString() == "?")
        //            continue;
        //        if (chBarcodeRule.ToString() == "*")
        //        {
        //            bstarMark = true;
        //            break;
        //        }
        //        if (chBarcodeRule.ToString() != chBarcode.ToString())
        //        {
        //            bRetVal = false;
        //            break;
        //        }
        //    }

        //    if (bstarMark == false && sBarcodeRule.Length != sBarcode.Length)
        //    {
        //        bRetVal = false;
        //    }
        //    sbBarcodeRule = null;
        //    sbBarcode = null;
        //    return bRetVal;
        //}

        //    public string GetBarcode(PanelNode panel)
        //    {
        //        string sBarcod = "NONE";
        //        TreeAccess ta = new TreeAccess(panel);
        //        foreach (BoardNode board in ta.GetBoardList(eTreeStatus.All))
        //        {
        //            if (board.Barcode.Barcode != "NONE")
        //            {
        //                sBarcod = board.Barcode.Barcode;
        //                break;
        //            }
        //        }

        //        return sBarcod;
        //    }

        #endregion 確認SharedFolder資料
    }
}

