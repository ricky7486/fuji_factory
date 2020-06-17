using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterCenterData;
using PrinterCenter.Service;
using PrinterCenter.FileClass;
using PrinterCenter.Log;
using System.IO;
using System.Net;
using PrinterCenter.Localization;
using PrinterCenter.UI;
using PrinterCenter.Printer.Algorithm;
using PrinterCenter.Printer.JudgeWipe;
using aejw.Network;
using PrinterCenter.File;

/// <summary>
/// [Rex Note]
/// MPM 使用PClientExe.exe共三種command
/// 1. Status的查詢
///     IIPrtStatusRequest [A/B]
///     檔案[A/B]IIPrtStatusRequest.txt
/// 2. 發送SPI檢測資料
///     IIPrtBoardInspectedData [A/B] [SN] [1/0] [modelName] [barodStatus] [inspectLow] [dataPath] [dx] [dy] [theta] [cx] [cy]
/// 3. 清鋼板指令
///     IIPrtWipeNow [A/B] [inspectLow] 0 1 
/// </summary>
/// 原本的架構是:abstract WriteFileToMergedFolder()為撰寫SPI計算過後準備給Printer的輸出檔案，並放到輸出檔案夾(備份區)
///             virtual SendToPrinter()為將輸出檔案放到sharedfolder中
///             後人多用WriteFileToMergedFolder直接寫到sharedfolder中，因此SendToPrinter使用virtual的方式
///             MPM部分發送給Printer是透過PClientExe.exe 故會多一到手續
///             為: 將給sharedfolder的檔案，複製到PClinetExe.exe所存在的資料夾WorkingPath中，透過PClientExe.exe
///                 來將資料丟給MPM Printer
/// 又，原本架構為大多繼承自ShareFolderClosedLoop 但MPM繼承自PrinterClosedLoop          
namespace PrinterCenter.Printer
{
    public sealed class MPM_PrinterData : IParser
    {
        #region Data
        public string PrinterAddress { get; set; }//印刷機主機名稱:埠號 //ex: 127.0.0.1:1550 or TRI-NB:1550
        public long BoardNumber { get; set; }//流水號 //ex: 3
        public bool IsBoardIdProvided { get; set; }//是否使用Barcode(true:使用, false:不使用) //ex:1
        public string BoardId { get; set; }//印刷機輸出的Barcode字串 //ex: BarCode_10_10_10
        public string ProcessProgramName { get; set; }//印刷機的程式名稱 //ex: Test_PP
        public int NumFids { get; set; }//傳入的標記點數量	//ex: 2


        public List<double> FidPosXList { get; set; }//標記點X座標 //ex: 0.2000,1.2000
        public List<double> FidPosYList { get; set; }//標記點Y座標 //ex: 0.3000,1.3000

        public int BoardStatus { get; set; }//板子印刷狀態 //ex: 0:Error ; 1:Printed

        public bool IsStrokeFrontToRear { get; set; }//前刮刀或後刮刀 //ex: 0:從後到前刷 ; 1:從前到後刷  
        public long ProcessedTimeStampLow { get; set; }//印刷時間(低位元)//ex: 1316500259
        public long ProcessedTimeStampHigh { get; set; }//印刷時間(高位元)//ex: 0
        public int PackagedDataLength { get; set; }//封包長度//ex: 20
        public List<string> PackagedData { get; set; }//封包資料//ex: 41 42 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
        #endregion
        public bool IsBarcodeMatched(object fileobj, string barcode)
        {
            MPM_PrinterData data = fileobj as MPM_PrinterData;
            if (data.BoardId == barcode)
                return true;
            else
                return false;
        }

        public object Parse(string filepath)
        {
            MPM_PrinterData ret = new MPM_PrinterData();

            try
            {

                PrinterCenter.FileClass.TextReader reader = new PrinterCenter.FileClass.TextReader(filepath);
                reader.Run();

                long lValue;
                int nValue;
                string sValue = string.Empty;
                string sLebel = string.Empty;
                char[] separator = { '=' };

                foreach (string s in reader.StrList)
                {
                    string[] words = s.Split(separator);
                    if (words.Count() != 2)
                        continue;

                    sLebel = words[0];
                    sValue = words[1];

                    switch (sLebel)
                    {
                        case "PrinterAddress":
                            ret.PrinterAddress = sValue;
                            break;
                        case "BoardNumber":
                            if (long.TryParse(sValue, out lValue) == true)
                                ret.BoardNumber = lValue;
                            else
                                ret.BoardNumber = 0;
                            break;
                        case "BoardIdProvided":
                            if (int.TryParse(sValue, out nValue) == true)
                                ret.IsBoardIdProvided = (nValue == 1) ? true : false;
                            else
                                ret.IsBoardIdProvided = false;
                            break;
                        case "BoardId":
                            ret.BoardId = sValue;
                            break;
                        case "ProcessProgramName":
                            ret.ProcessProgramName = sValue;
                            break;
                        case "NumFids":
                            if (int.TryParse(sValue, out nValue) == true)
                                ret.NumFids = nValue;
                            else
                                ret.NumFids = 0;
                            break;
                        case "FidPosX":
                            ret.FidPosXList = GetFidPosValue(sValue);
                            break;
                        case "FidPosY":
                            ret.FidPosYList = GetFidPosValue(sValue);
                            break;
                        case "BoardStatus":
                            if (int.TryParse(sValue, out nValue) == true)
                                ret.BoardStatus = (nValue == 0) ? 0 : 1;
                            else
                                ret.BoardStatus = 0;
                            break;
                        case "StrokeFrontToRear":
                            if (int.TryParse(sValue, out nValue) == true)
                                ret.IsStrokeFrontToRear = (nValue == 1) ? true : false;
                            else
                                ret.IsStrokeFrontToRear = false;
                            break;
                        case "ProcessedTimeStampLow":
                            if (long.TryParse(sValue, out lValue) == true)
                                ret.ProcessedTimeStampLow = lValue;
                            else
                                ret.ProcessedTimeStampLow = 0;
                            break;
                        case "ProcessedTimeStampHigh":
                            if (long.TryParse(sValue, out lValue) == true)
                                ret.ProcessedTimeStampHigh = lValue;
                            else
                                ret.ProcessedTimeStampHigh = 0;
                            break;
                        case "PackagedDataLength":
                            if (int.TryParse(sValue, out nValue) == true)
                                ret.PackagedDataLength = nValue;
                            else
                                ret.PackagedDataLength = 0;
                            break;
                        case "PackagedData":
                            ret.PackagedData = GetPackagedData(sValue);
                            break;
                        default:
                            break;
                    }
                }

                reader.Dispose();
                reader = null;

                //PrintTime = new DateTime(1970, 1, 1, 0, 0, 0).AddHours(8).AddSeconds(processedTimeStampLow);
            }
            catch (Exception exception)
            {
                Log4.PrinterLogger.ErrorFormat("MPM Printer Data Pasre Exception: {0}", filepath);
                Log4.PrinterLogger.ErrorFormat("Message: {0}", exception.Message);
                return null;
            }

            return ret;
        
        }

        private List<double> GetFidPosValue(string target)
        {
            char[] separator = { ',' };
            string[] words = target.Split(separator);
            double value = 0;
            List<double> valueList = new List<double>();
            foreach (string s in words)
            {
                if (double.TryParse(s, out value) == true)
                {
                    valueList.Add(value);
                }
            }
            return valueList;
        }
        private List<string> GetPackagedData(string target)
        {
            char[] separator = { ' ' };
            string[] words = target.Split(separator);
            List<string> valueList = new List<string>();
            foreach (string s in words)
            {
                valueList.Add(s);
            }
            return valueList;
        }
    }

    public class MPMRPCPara
    {
        //[RegDesc(@"說明請見此參數的GetSysReg()函式呼叫", ProgramVersion.OnTheFly | ProgramVersion.StopAndGo, RegValueEditingGui.Unknown)]
        public string SideComputerName = "LaneA";
        //[RegDesc(@"說明請見此參數的GetSysReg()函式呼叫", ProgramVersion.OnTheFly | ProgramVersion.StopAndGo, RegValueEditingGui.Unknown)]
        public string SideIP = "192.168.0.1";
        //[RegDesc(@"說明請見此參數的GetSysReg()函式呼叫", ProgramVersion.OnTheFly | ProgramVersion.StopAndGo, RegValueEditingGui.Unknown)]
        public string SidePort = "1551";
        //[RegDesc(@"是否啟用RPC DualLane模式 (0或1)", ProgramVersion.OnTheFly | ProgramVersion.StopAndGo, RegValueEditingGui.Unknown)]
        public string ClientDualLaneEanbled = "0";
        //[RegDesc(@"SPI的RPC通訊埠", ProgramVersion.OnTheFly | ProgramVersion.StopAndGo, RegValueEditingGui.Unknown)]
        public string RPCServerPort = "1550";
        //[RegDesc(@"7007主程式與RPC外掛軟體交換資料的目錄", ProgramVersion.OnTheFly | ProgramVersion.StopAndGo, RegValueEditingGui.Unknown)]
        public string WorkingPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\RPC\";//7007主程式與RPC外掛軟體交換資料的目錄
        //[RegDesc(@"是否顯示SPI RPC Console Window", ProgramVersion.OnTheFly | ProgramVersion.StopAndGo, RegValueEditingGui.Unknown)]
        public bool bShowRPCWindow = false;
    }
    public enum eSpiExecutionStatus { Idle = 1, Executing = 3, Tuning = 4, Error = 5 }
    public class MPMPrinter : PrinterBase, IDisposable
    {
        private string target;
        private SharedFolderWatcher WatchedFolder;
        //private int count = 0;
        private StretchResult _StretchResult;
        private RotationResult _RotationResult;
        private CenterOffsetResult _CenterOffsetResult;

        private MPMRPCPara _RPCPara = new MPMRPCPara();
        private bool _IsConnect = false;
        public MPMPrinter(eAssignedLane_Printer lane) : base(ePrinterVendor.MPM, lane)
        {
            string errStr="";
            string section = String.Format("MPM_{0}", lane.ToString());
            
            //ulong inspectedLow = (ulong)((DateTime.Now.AddHours(-8) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);//Test
            //取得RPC參數
            using (IniFile ini = new IniFile())
            {
                
                _RPCPara.SideIP = ini.Read(section, "SideIP");
                _RPCPara.SidePort = ini.Read(section, "SidePort");
                _RPCPara.SideComputerName = ini.Read(section, "SideComputerName");

                _RPCPara.RPCServerPort = ini.Read(section, "RPCServerPort");

                var path = ini.Read(section, "WorkingPath");
                if (!String.IsNullOrEmpty(path))
                {
                    Log4.PrinterLogger.InfoFormat("Override WorkingPath ={0}",path);
                    _RPCPara.WorkingPath = path;
                }
                if (lane == eAssignedLane_Printer.Lane2)
                    _RPCPara.ClientDualLaneEanbled = "1";
                else
                    _RPCPara.ClientDualLaneEanbled = "0";
                bool bShow;
                Boolean.TryParse(ini.Read(section, "bShowRPCWindow"), out bShow);
                _RPCPara.bShowRPCWindow = bShow;
            }

           
            _IsConnect = Connect(ref errStr);
            if (!_IsConnect)
                TRMessageBox.Show("請確認MPM RPC連線");
           
        }
        #region Abstract
        public override void Activate()
        {
            
        }

        public override void Calculate(InspectedPanel currentPanel, object file)
        {
            _CenterOffsetResult = null;
            _RotationResult = null;
            _StretchResult = null;

            var Boxes = GetCandidateBoxes(currentPanel, PrinterCommonSetting);

            //Calculate
            CenterOffsetCorrectionAlgorithm cocAlgo = new CenterOffsetCorrectionAlgorithm(PrinterCommonSetting.Clone());
            RotationCorrectionAlgorithm rcAlgo = new RotationCorrectionAlgorithm(PrinterCommonSetting.Clone());
            StretchAlgorithm sAlgo = new StretchAlgorithm();

            _CenterOffsetResult = (CenterOffsetResult)cocAlgo.Calculate(Boxes, currentPanel, null);
            _RotationResult = (RotationResult)rcAlgo.Calculate(Boxes, currentPanel, _CenterOffsetResult);
            _StretchResult = (StretchResult)sAlgo.Calculate(null, currentPanel, null);
            _WipeReason = JudgeWipeHelper.JudgeWipeByPriorityStrategy(currentPanel, Boxes);
        }

        public void Dispose()
        {
            
            KillRpcProcess();
        }

        public override object Match(InspectedPanel currentPanel)
        {
            return null;
        }

        public override bool Output(InspectedPanel currentPanel, object file)
        {

            //WriteFileToMergedFolder(原)
            MPM_PrinterData MPMData = file as MPM_PrinterData;
            string netDrive;
            //if (!PrinterSFSetting.IsOutEnable)
            //{
            //    Log4.PrinterLogger.ErrorFormat("Doesn't have a out SharedFolder");
            //    return false;
            //}
            //else
            netDrive = _RPCPara.WorkingPath;//放在同層
            string path = netDrive + String.Format("{0:yyyyMMddHHmmss}.xml", currentPanel.InspectStartTime);
            Log4.PrinterLogger.InfoFormat(" path={0}", path);
            FileClass.TextWriter writer = new FileClass.TextWriter(path);
            writer.StrList.Add(string.Format("XCorrection={0}", Math.Round((_CenterOffsetResult.Dx * 0.001), 6).ToString()));
            writer.StrList.Add(string.Format("YCorrection={0}", Math.Round((_CenterOffsetResult.Dy * 0.001), 6).ToString()));
            writer.StrList.Add(string.Format("ThetaCorrection={0}", Math.Round(_RotationResult.Theta, 6).ToString()));
            writer.StrList.Add(string.Format("XCenterofRotation={0}", Math.Round((_RotationResult.Center.X * 0.001), 6).ToString()));
            writer.StrList.Add(string.Format("YCenterofRotation={0}", Math.Round((_RotationResult.Center.Y * 0.001), 6).ToString()));
            writer.Run();
            writer.Dispose();
            writer = null;

            //(原)SendToPrinter
            string dstPath = _RPCPara.WorkingPath + GetLane() + "IIPrtBoardInspectedData.txt";
            MoveFile(path, dstPath, true);

            long boardNumber;
            //long.TryParse(MPMData.PanelSN, out boardNumber);
            boardNumber = ViewModelLocator.Atom.PrinterWindowVM.SN;

            bool boardIDProvided = MPMData.IsBarcodeMatched(MPMData,currentPanel.Panel.PanelBarcode);

            string barCode = (boardIDProvided == true) ? MPMData.BoardId : "NONE";
            string modelName = currentPanel.Panel.ModelName;
            long boardStatus = 1;

            ulong inspectedLow = (ulong)((currentPanel.InspectStartTime.AddHours(-8) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
            
            //string dataPath = "\"" + MPMData.FullFilePath + "\"";
            string dataPath = "\"" + path + "\"";

            double dx = Math.Round((_CenterOffsetResult.Dx * 0.001), 6);
            double dy = Math.Round((_CenterOffsetResult.Dy * 0.001), 6);
            double theta = Math.Round(_RotationResult.Theta, 6);
            double cx = Math.Round((_RotationResult.Center.X * 0.001), 6);
            double cy = Math.Round((_RotationResult.Center.Y * 0.001), 6);

            modelName = modelName.Replace(" ", "");
            string argument = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13}",
                "IIPrtBoardInspectedData", GetLane(), boardNumber, (boardIDProvided == true) ? "1" : "0", barCode, modelName, boardStatus, inspectedLow, dataPath,
                dx, dy, theta, cx, cy);

            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.WorkingDirectory = _RPCPara.WorkingPath;
            info.FileName = "PClientExe.exe";
            info.Arguments = argument;
            Log4.PrinterLogger.InfoFormat("info.WorkingDirectory={0}", info.WorkingDirectory);
            Log4.PrinterLogger.InfoFormat("info.FileName        ={0}", info.FileName);
            Log4.PrinterLogger.InfoFormat("info.Arguments       ={0}", info.Arguments);

            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(info);
            proc.WaitForExit();

            bool IsWipe = (_WipeReason != eWipeStencilReason.NoNeedToWipe);
            if (IsWipe == true)
            {
               SendWipeNow(inspectedLow);
            }




            return true;
        }

        public override void UpdateHistory()
        {
            DxHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DxSetting.XAxis.Minimun + DxHistory.Count, _CenterOffsetResult.Dx));
            DyHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.DySetting.XAxis.Minimun + DyHistory.Count, _CenterOffsetResult.Dy));
            ThetaHistory.Add(new KeyValuePair<double, double>(ViewModelLocator.Atom.ChartHostVM.ThetaSetting.XAxis.Minimun + ThetaHistory.Count, _RotationResult.Theta));

        }
        #endregion

        #region
        private System.Diagnostics.Process process = null;
        private void KillRpcProcess()
        {
            if (process != null)
            {
                try
                {
                    process.Kill();
                    Log4.PrinterLogger.InfoFormat("Process killed!");
                }
                catch (Exception e)
                {
                    Log4.PrinterLogger.ErrorFormat("Exception {0} .. ", e.Message);                   
                }
                process = null;
            }
        }
       
        
        private string GetLane()
        {
            string strLane = "A";
            switch (LaneID)
            {
                case eAssignedLane_Printer.Lane1: strLane = "A"; break;
                case eAssignedLane_Printer.Lane2: strLane = "B"; break;
                default: break;
            }
            return strLane;
        }

        //場景: spi向printer查詢狀態 (連線時用)
        //發送IIPrtStatusRequest.txt
        private bool InquirePrinterStatus()
        {
            Log4.PrinterLogger.Info("@InquirePrinterStatus()");
            //刪除先前狀態
            string inquiredName = _RPCPara.WorkingPath + GetLane() + "IIPrtStatusRequest.txt";
            Log4.PrinterLogger.InfoFormat("inquiredName={0}", inquiredName);
            if (System.IO.File.Exists(inquiredName) == true)
            {
                FileProcess.ClearFileReadOnly(inquiredName);
                Log4.PrinterLogger.InfoFormat("{0} Exists!", inquiredName);
                try
                {
                    System.IO.File.Delete(inquiredName);
                }
                catch(Exception e)
                {
                    Log4.PrinterLogger.InfoFormat("Exception:{0}", e.Message);
                }
               
            }

            //發出更新狀態要求
            Log4.PrinterLogger.Info("Request an update...");
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            
            info.WorkingDirectory = _RPCPara.WorkingPath;
            info.FileName = "PClientExe.exe";
            info.Arguments = string.Format("{0} {1}", "IIPrtStatusRequest", GetLane());
            Log4.PrinterLogger.InfoFormat("info.WorkingDirectory={0}", info.WorkingDirectory);
            Log4.PrinterLogger.InfoFormat("info.FileName        ={0}", info.FileName);
            Log4.PrinterLogger.InfoFormat("info.Arguments       ={0}", info.Arguments);
            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(info);
            proc.WaitForExit();
            info = null;


            //查詢狀態
            bool isInquired = false;
            int msTimer = 500;
            int msSleep = 5;
            while (msTimer > 0)
            {
                if (System.IO.File.Exists(inquiredName) == true)
                {
                    isInquired = true;
                    break;
                }
                msTimer -= msSleep;
                System.Threading.Thread.Sleep(msSleep);
            }
            Log4.PrinterLogger.InfoFormat("return={0}", isInquired.ToString());
            return isInquired;
        }

        //場景: printer向spi查詢狀態 (SPI狀態更新時呼叫)
        //發送IIInspStatusRequest.txt
        public void SendSpiStatusToPrinter(eSpiExecutionStatus status)
        {
            Log4.PrinterLogger.InfoFormat("@SendSpiStatusToPrinter({0})", status.ToString());
        
            string fileName = _RPCPara.WorkingPath + "IIInspStatusRequest.txt";
            Log4.PrinterLogger.InfoFormat("fileName = {0}", fileName);
            FileClass.TextWriter writer = new FileClass.TextWriter(fileName);
            writer.StrList.Add(string.Format("MachineStatus={0}", (int)status));
            writer.StrList.Add(string.Format("MachineId={0}", /*exportDataReg.StationID*/"SPI"));//暫時
            writer.Run();
            writer.Dispose();
            writer = null;
        }
        //場景: spi傳送清鋼板訊號給printer
        //透過PClientExe.exe發送 command
        public void SendWipeNow(ulong inspectedLow)
        {
            Log4.PrinterLogger.InfoFormat("@SendWipeNow(0)", inspectedLow.ToString());
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.WorkingDirectory = _RPCPara.WorkingPath;
            info.FileName = "PClientExe.exe";
            info.Arguments = string.Format("IIPrtWipeNow {0} {1} 0 1", GetLane(), inspectedLow);
            Log4.PrinterLogger.InfoFormat("info.WorkingDirectory={0}", info.WorkingDirectory);
            Log4.PrinterLogger.InfoFormat("info.FileName        ={0}", info.FileName);
            Log4.PrinterLogger.InfoFormat("info.Arguments       ={0}", info.Arguments);
            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(info);
            proc.WaitForExit();
            info = null;
        }
        bool IsComputerNameDuplicated(/*string PrinterIP*/)
        {
            string AsideIP = "";
            string BsideIP = "";
            using (IniFile ini = new IniFile())
            {
                AsideIP = ini.Read("MPM_Lane1", "SideIP");
                BsideIP = ini.Read("MPM_Lane1", "SideIP");
            }
            string thisComputerName = ConvertIPToComputerName(AsideIP);
            string anotherComputerName = ConvertIPToComputerName(BsideIP);
            if (thisComputerName.ToUpper().CompareTo(anotherComputerName.ToUpper()) == 0)
                return true;
            else
                return false;
        }
        private string ConvertIPToComputerName(string ipAdress)
        {
            string machineName = string.Empty;
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAdress);

                machineName = hostEntry.HostName;
            }
            catch (Exception ex)
            {
                // Machine not found...
                Console.WriteLine(ex.ToString());
            }
            return machineName;
        }
        
        private bool Connect(ref string errStr)
        {
            #region 說明
            /* *
			 * 1. 寫入註冊表SOFTWARE\\Wow6432Node\\tri\\Printer
			 * 2. 讀取註冊表SOFTWARE\\Wow6432Node\\tri\\Printer\\bShowRPCServer=1 : 顯示Console RPC viewer
			 * 3. 開啟路徑
			 *		執行檔所在路徑\\RPC\\			(檔案交換路徑)
			 *		執行檔所在路徑\\RPC\\server\\	(RPC通訊軟體開啟路徑)
			 * */
            #endregion

            bool IsConnected = false;

            Log4.PrinterLogger.InfoFormat("Ping to {0} .. ", _RPCPara.SideIP);
            if (NetTool.Ping(_RPCPara.SideIP) == false)
            {
                errStr = "@CANT_CONNECT_TO_PRINTER_IP".Translate() + ": " + _RPCPara.SideIP;
                Log4.PrinterLogger.ErrorFormat("Connect False: {0} .. ", errStr);
                return false;
            }


            if (IsComputerNameDuplicated())
            {
                errStr = "Computer Name Is Duplicated!";
                Log4.PrinterLogger.ErrorFormat("IsComputerNameDuplicated: {0} .. ", errStr);
                return false;
            }



            KillRpcProcess();
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(_RPCPara.WorkingPath + "server\\RPCServer.exe");
      
            string strCreateRPCFail = "@CANT_CREATE_SPI_RPC_SERVER_FROM".Translate() + ": " + _RPCPara.WorkingPath + "server\\RPCServer.exe";

            Log4.PrinterLogger.InfoFormat("Process StartInfo ({0})", _RPCPara.WorkingPath + "server\\RPCServer.exe");
            Log4.PrinterLogger.InfoFormat("_RPCPara.bShowRPCWindow = ({0})", _RPCPara.bShowRPCWindow);
            if (_RPCPara.bShowRPCWindow == true)
            {
                info.CreateNoWindow = true;
                info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            }
            else
            {
                info.CreateNoWindow = false;
                info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            }
            try
            {
                process = System.Diagnostics.Process.Start(info);
                errStr = (process == null) ? strCreateRPCFail : string.Empty;
            }
            catch (Exception e)
            {
                Log4.PrinterLogger.ErrorFormat("Start Process Exception = {0}", e.Message);
                errStr = strCreateRPCFail;
            }
            if (process == null || errStr != string.Empty)
                return false;

            IsConnected = InquirePrinterStatus();
            if (IsConnected == true)
                SendSpiStatusToPrinter(eSpiExecutionStatus.Idle);
            else
                errStr = "@CANT_RECEIVE_PRINTER_STATUS".Translate();

            return IsConnected;
        }

        //場景: 查看是否有檔案存在資料夾中
        private bool IsClearQueue()
        {
            //note: spi檢查printer是否有發出堆板清除訊號 (在接收檔案前查看, 若為true, 則清除先前未處理的printerData)
            string file = _RPCPara.WorkingPath + GetLane() + "IIInspClearQueue.txt";
            return System.IO.File.Exists(file);
        }
        //按鈕Trigger ? 
        public void ManualWipeNow()
        {
            if (_IsConnect == false)
                return;
            ulong inspectedLow = (ulong)((DateTime.Now.AddHours(-8) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
            SendWipeNow(inspectedLow);
        }


        private bool MoveFile(string srcPath, string dstPath, bool bCopy = false)
        {
            Log4.PrinterLogger.InfoFormat("@MoveFile({0},{1},{2})", srcPath, dstPath,bCopy.ToString());
            //ClearFileReadOnly(srcPath);
            //ClearFileReadOnly(dstPath);

            if (!System.IO.File.Exists(srcPath))
                System.IO.File.Create(srcPath).Close();
            if (System.IO.File.Exists(dstPath))
                System.IO.File.Delete(dstPath);
            if ((Directory.Exists(dstPath)) == false)
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dstPath));
            if (bCopy == true)
                System.IO.File.Copy(srcPath, dstPath);
            else
                System.IO.File.Move(srcPath, dstPath);
            return true;
        }
        #endregion
    }
}
