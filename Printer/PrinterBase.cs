using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using PrinterCenter.Log;
using PrinterCenter.Printer.JudgeWipe;
using PrinterCenter.Service;
using PrinterCenter.UI.CommonSetting;
using PrinterCenter.UI.SharedFolderSetting;
using PrinterCenterData;

namespace PrinterCenter.Printer
{
    public abstract class PrinterBase
    {
        //記數放在Template pattern此處
        //protected int Accumulation;

        public eWipeStencilReason _WipeReason = eWipeStencilReason.NoNeedToWipe;

        //public Queue<InspectedPanel> InspectedPanels = new Queue<InspectedPanel>();
        public EventQueue<InspectedPanel> InspectedPanels = new EventQueue<InspectedPanel>();

        public PrinterBase(ePrinterVendor vendor, eAssignedLane_Printer laneID)
        {
            LaneID = laneID;
            Vendor = vendor;
            InspectedPanels.Clear();
            PrinterCommonSetting = new CommonSettingVM();
            PrinterSFSetting = new SharedFolderSettingVM();
            DxHistory = new ObservableCollection<KeyValuePair<double, double>>();
            DyHistory = new ObservableCollection<KeyValuePair<double, double>>();
            ThetaHistory = new ObservableCollection<KeyValuePair<double, double>>();
            DxHistory.Clear();
            DyHistory.Clear();
            ThetaHistory.Clear();
            //Accumulation = 0;//= AccDx = AccDy = AccTheta = 0;
        }

        ~PrinterBase()
        {
            double dxLimit = ViewModelLocator.Atom.ChartHostVM.DxSetting.XAxis.Maximun - ViewModelLocator.Atom.ChartHostVM.DxSetting.XAxis.Minimun + 1;
            double dyLimit = ViewModelLocator.Atom.ChartHostVM.DySetting.XAxis.Maximun - ViewModelLocator.Atom.ChartHostVM.DySetting.XAxis.Minimun + 1;
            double thLimit = ViewModelLocator.Atom.ChartHostVM.ThetaSetting.XAxis.Maximun - ViewModelLocator.Atom.ChartHostVM.ThetaSetting.XAxis.Minimun + 1;

            //DX
            BackupAChart(DxHistory, dxLimit, "Chart_Dx", ".dx", true);
            //Dy
            BackupAChart(DyHistory, dxLimit, "Chart_Dy", ".dy", true);
            //Theta
            BackupAChart(ThetaHistory, thLimit, "Chart_Theta", ".th", true);
        }

        public ObservableCollection<KeyValuePair<double, double>> DxHistory { get; set; }

        public ObservableCollection<KeyValuePair<double, double>> DyHistory { get; set; }

        public eAssignedLane_Printer LaneID { get; private set; }
        public CommonSettingVM PrinterCommonSetting { get; set; }
        public SharedFolderSettingVM PrinterSFSetting { get; set; }
        public ObservableCollection<KeyValuePair<double, double>> ThetaHistory { get; set; }
        public ePrinterVendor Vendor { get; private set; }

        /// <summary>
        /// Gets the candidate boxes. 每個繼承PrinterBase在實作 Caculate的時候會使用到此function 為UI介面挑選box的candidate選項
        /// </summary>
        /// <param name="currentPanel">The current panel.</param>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        public static List<Box> GetCandidateBoxes(InspectedPanel currentPanel, CommonSettingVM vm)
        {
            List<Box> Boxes = new List<Box>();

            //var BadBoards = (from board in currentPanel.Panel.Boards
            //                 where board.IsHasBadMark == true
            //                 select board).ToList();

            foreach (var board in currentPanel.Panel.Boards)
            {
                if (board.IsSkipped || board.IsHasBadMark) { continue; }

                foreach (var component in board.Components)
                {
                    foreach (var box in component.Boxes)
                    {
                        bool _IsPassHFilter = false;
                        bool _IsPassAFilter = false;
                        bool _IsPassVFilter = false;
                        //H
                        if (vm.IsHeightFilter)
                        {
                            if (box.Height_p >= vm.HeightRange.LowerBound
                                && box.Height_p <= vm.HeightRange.UpperBound)
                            {
                                _IsPassHFilter = true;
                            }
                            else
                                _IsPassHFilter = false;
                        }
                        else
                            _IsPassHFilter = true; //表示不用Filter
                        //A
                        if (vm.IsAreaFilter)
                        {
                            if (box.Area_p >= vm.AreaRange.LowerBound
                                && box.Area_p <= vm.AreaRange.UpperBound)
                            {
                                _IsPassAFilter = true;
                            }
                            else
                                _IsPassAFilter = false;
                        }
                        else
                            _IsPassAFilter = true; //表示不用Filter
                        //V
                        if (vm.IsVolumeFilter)
                        {
                            if (box.Volume_p >= vm.VolumeRange.LowerBound
                                && box.Volume_p <= vm.VolumeRange.UpperBound)
                            {
                                _IsPassVFilter = true;
                            }
                            else
                                _IsPassVFilter = false;
                        }
                        else
                            _IsPassVFilter = true; //表示不用Filter

                        if (_IsPassHFilter && _IsPassAFilter && _IsPassVFilter)
                            Boxes.Add(box);
                    }
                }
            }
            return Boxes;
        }

        /// <summary>
        /// 放置需要啟動的功能 例如SharedFolder的監控
        /// </summary>
        public abstract void Activate();

        /// <summary> Template Design Pattern
        /// 1. Match
        /// 2. Calculate
        /// 3. BackupRoutine => 看前一次是否滿了，滿了則先清
        /// 4. UpdateHistory

        public virtual void BackupRoutine(int dxLimit, int dyLimit, int thLimit)
        {
            //Dx
            if (DxHistory.Count == dxLimit)//達到上限
                BackupAChart(DxHistory, dxLimit, "Chart_Dx", ".dx");

            //Dy
            if (DyHistory.Count == dyLimit)//達到上限
                BackupAChart(DyHistory, dxLimit, "Chart_Dy", ".dy");

            //Theta
            if (ThetaHistory.Count == thLimit)//達到上限
                BackupAChart(ThetaHistory, thLimit, "Chart_Theta", ".th");
        }

        public abstract void Calculate(InspectedPanel currentPanel, object file);

        /// <summary>
        /// FujiChangeOver用
        /// </summary>
        /// <returns><c>true</c> if [is need write comp image]; otherwise, <c>false</c>.</returns>
        public virtual bool CheckCurrentSharedFolder()
        {
            Log4.PrinterLogger.InfoFormat("CheckCurrentSharedFolder()-default return true");
            return true;
        }

        public virtual bool CustomOutput(InspectedPanel currentPanel, object file)
        {
            return false;//無custom
        }

        //清除DxHistory、DyHistory、ThetaHistory並做成檔案
        public bool GenerateToFile(ObservableCollection<KeyValuePair<double, double>> target, string filePath, bool clearTarget = true)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    for (int i = 0; i < target.Count; i++)
                    {
                        sw.WriteLine(String.Format("{0}={1}", target[i].Key, target[i].Value));
                    }
                }
                if (clearTarget)
                    target.Clear();
                return true;
            }
            catch (Exception e)
            {
                if (clearTarget)
                    target.Clear();
                return false;
            }
        }

        /// <summary>
        /// Fuji EasyLink 出大圖用
        /// </summary>
        /// <returns></returns>
        public virtual string GetWriteCompImagePath()
        {
            Log4.PrinterLogger.InfoFormat("GetWriteCompImagePath()-default return string.Empty");
            return string.Empty;
        }

        /// <summary>
        /// Fuji EasyLink 出大圖用
        /// </summary>
        /// <returns><c>true</c> if [is need write comp image]; otherwise, <c>false</c>.</returns>
        public virtual bool IsNeedWriteCompImage()
        {
            Log4.PrinterLogger.InfoFormat("IsNeedWriteCompImage()-default return false");
            return false;
        }

        /// <summary>
        /// Matches this instance.
        /// </summary>
        /// <returns>為回傳的Vendor給予的資料 需自行轉</returns>
        public abstract object Match(InspectedPanel currentPanel);

        /// <summary>
        /// FujiChangeOver用
        /// </summary>
        /// <returns></returns>
        public virtual bool MoveToNextSharedFolder()
        {
            Log4.PrinterLogger.InfoFormat("MoveToNextSharedFolder()-default return true");
            return true;
        }

        /// <summary>
        /// FujiChangeOver用
        /// </summary>
        /// <returns><c>true</c> if [is need write data to next folder]; otherwise, <c>false</c>.</returns>
		//public virtual bool NeedCreateTheDataToNext()
		//{
		//    Log4.PrinterLogger.InfoFormat("NeedCreateTheDataToNext()-default return true");
		//    return true;
		//}

        public abstract bool Output(InspectedPanel currentPanel, object file);

        /// 5. Output </summary>
        public virtual void StartProcess()
        {
            try
            {
                Log4.PrinterLogger.InfoFormat("StartProcess()");
                var currentPanel = InspectedPanels.Dequeue();
                Log4.PrinterLogger.InfoFormat(" -Dequeue [{0}] SPI Data InspectTime = {1}", LaneID.ToString(), currentPanel.InspectStartTime);
                Log4.PrinterLogger.InfoFormat(" 1.Match()");

                var file = Match(currentPanel);
                Log4.PrinterLogger.InfoFormat(" 2.Calculate()");
                Calculate(currentPanel, file);

                double dxLimit = ViewModelLocator.Atom.ChartHostVM.DxSetting.XAxis.Maximun - ViewModelLocator.Atom.ChartHostVM.DxSetting.XAxis.Minimun + 1;
                double dyLimit = ViewModelLocator.Atom.ChartHostVM.DySetting.XAxis.Maximun - ViewModelLocator.Atom.ChartHostVM.DySetting.XAxis.Minimun + 1;
                double thLimit = ViewModelLocator.Atom.ChartHostVM.ThetaSetting.XAxis.Maximun - ViewModelLocator.Atom.ChartHostVM.ThetaSetting.XAxis.Minimun + 1;

                Log4.PrinterLogger.InfoFormat(" 3.BackupRoutine()");
                BackupRoutine((int)dxLimit, (int)dyLimit, (int)thLimit); //計算出Dx、Dy、Theta的累加器

                Log4.PrinterLogger.InfoFormat(" 4.UpdateHistory()");
                //累加
                //Accumulation++;

                UpdateHistory();

                Log4.PrinterLogger.InfoFormat(" 5.Output()");
                Output(currentPanel, file);

                bool bCus = CustomOutput(currentPanel, file);
                if (bCus)
                    Log4.PrinterLogger.InfoFormat(" 6.CustomOutput() DONE");

                ViewModelLocator.Atom.PrinterWindowVM.SN++;
            }
            catch (Exception e)
            {
                if (e is MatchException)
                    Log4.PrinterLogger.ErrorFormat("[!] MatchException:" + e.Message);
                else
                    if (e is CaculateException)
                        Log4.PrinterLogger.ErrorFormat("[!] CaculateException:" + e.Message);
                    else
                        if (e is UpdateHistoryException)
                            Log4.PrinterLogger.ErrorFormat("[!] UpdateHistoryException:" + e.Message);
                        else
                            if (e is OutputException)
                                Log4.PrinterLogger.ErrorFormat("[!] OutputException:" + e.Message);
                            else
                                if (e is CustomOutputException)
                                    Log4.PrinterLogger.ErrorFormat("[!] CustomOutputException:" + e.Message);
            }
        }

        public abstract void UpdateHistory();

        protected object Match(InspectedPanel currentPanel, IParser parser, SharedFolderWatcher watcher)
        {
            //bool IsDone = false;
            object _ParsedData = null;
            if (PrinterCommonSetting.MatchingBasis == UI.CommonSetting.eMatchingBasis.Sequence)
            {
                #region SN

                //while (!IsDone)
                //{
                if (watcher.WatchedFiles.Count > 0)
                {
                    //正常情況，都是取第一個(抽板狀況除外)
                    var file = watcher.WatchedFiles.Dequeue();
                    Log4.PrinterLogger.InfoFormat(" -(1).Pick a file({0})", file);
                    _ParsedData = parser.Parse(watcher.Target + file);
                    if (_ParsedData != null) //parse ok
                    {
                        Log4.PrinterLogger.InfoFormat(" -(2).Parse a file OK({0})", file);
                        //UI狀態打V
                        ViewModelLocator.Atom.FlowHostVM.MarkWatchedFileCheckBox(LaneID, file);

                        ViewModelLocator.Atom.FlowHostVM.MarkSPIDataCheckBox();
                        Log4.PrinterLogger.InfoFormat(" -(3).Done {0}", file);
                        //IsDone = true;
                    }
                    else
                    {
                        //IsDone = true;
                        Log4.PrinterLogger.InfoFormat(" -(2).Parse a file NG({0}) - break", file);
                        throw new MatchException("[s] Parse a file NG - break");
                    }
                }
                else
                {
                    //IsDone = true;
                    //正常流程應該要先有檔案，才會有檢測資料
                    Log4.PrinterLogger.InfoFormat(" -(1).No files - break");
                    throw new MatchException("[s] No Files - break");
                }

                //}

                #endregion SN
            }
            else
            {
                #region Barcode

                bool IsFoundBarcode = false;
                //while (!IsDone)
                //{
                if (watcher.WatchedFiles.Count > 0)
                {
                    for (int i = 0; i < watcher.WatchedFiles.Count; i++)
                    {
                        _ParsedData = parser.Parse(watcher.Target + watcher.WatchedFiles[i]);
                        if (_ParsedData != null)
                        {
                            if (parser.IsBarcodeMatched(_ParsedData, currentPanel.Panel.PanelBarcode))
                            {
                                Log4.PrinterLogger.InfoFormat(" -(3).Find Barcode = {0} break {1}", currentPanel.Panel.PanelBarcode, watcher.WatchedFiles[i]);

                                ViewModelLocator.Atom.FlowHostVM.MarkWatchedFileCheckBox(LaneID, watcher.WatchedFiles[i]);
                                ViewModelLocator.Atom.FlowHostVM.MarkSPIDataCheckBox();

                                Log4.PrinterLogger.InfoFormat(" -(4).Done {0}", watcher.WatchedFiles[i]);
                                //執行dequeue
                                watcher.WatchedFiles.Dequeue();
                                IsFoundBarcode = true;
                                //IsDone = true;
                                break;
                            }
                            else
                            {
                                Log4.PrinterLogger.InfoFormat(" -(3).Find Barcode = {0} NG {0}", currentPanel.Panel.PanelBarcode, watcher.WatchedFiles[i]);
                                continue;
                            }
                        }
                        else
                        {
                            //IsDone = true;
                            Log4.PrinterLogger.InfoFormat(" -(2).Parse file NG {0}", watcher.WatchedFiles[i]);
                        }
                    }
                    if (!IsFoundBarcode)
                    {
                        Log4.PrinterLogger.InfoFormat(" -(3). Reach to the end, Still can't find file coresponding to Barcode = {0}", currentPanel.Panel.PanelBarcode);
                        //IsDone = true;
                        throw new MatchException("[b] Reach to the end, Still can't find file coresponding to Barcode");
                    }
                }
                else
                {
                    //IsDone = true;
                    //正常流程應該要先有檔案，才會有檢測資料
                    Log4.PrinterLogger.InfoFormat(" -(1).No files - break");
                    throw new MatchException("[b] No files - break");
                }
                //}

                #endregion Barcode
            }

            return _ParsedData;
        }

        private void BackupAChart(ObservableCollection<KeyValuePair<double, double>> target, double limit, string iniSection, string extensionFileName, bool forceBackup = false)
        {
            using (IniFile ini = new IniFile())
            {
                bool clearTarget = true;
                string path;
                if (target.Count == limit || forceBackup)//達到上限 或 強制備份
                {
                    //產生back檔

                    clearTarget = true;
                    Boolean.TryParse(ini.Read(iniSection, "ClearTarget"), out clearTarget);
                    path = ini.Read(iniSection, "Path");
                    path = Path.Combine(path, DateTime.Now.ToString("yyyyMMdd_HHmmss") + extensionFileName);
                    GenerateToFile(target, path, clearTarget);
                }
            }
        }
    }

    #region 自定義Exception

    public class CaculateException : Exception
    {
        public CaculateException()
        {
        }

        public CaculateException(string message)
            : base(message) { }

        public CaculateException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class CustomOutputException : Exception
    {
        public CustomOutputException()
        {
        }

        public CustomOutputException(string message)
            : base(message) { }

        public CustomOutputException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class MatchException : Exception
    {
        public MatchException()
        {
        }

        public MatchException(string message)
            : base(message) { }

        public MatchException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class OutputException : Exception
    {
        public OutputException()
        {
        }

        public OutputException(string message)
            : base(message) { }

        public OutputException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class UpdateHistoryException : Exception
    {
        public UpdateHistoryException()
        {
        }

        public UpdateHistoryException(string message)
            : base(message) { }

        public UpdateHistoryException(string message, Exception inner)
            : base(message, inner) { }
    }

    #endregion 自定義Exception
}