using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PrinterCenter.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name
    //       "IPrinterDuplexService" in both code and config file together.

    #region ServiiceContract

    [ServiceContract(
      CallbackContract = typeof(IPrinterDuplexServiceCallback)
      )]
    public interface IPrinterDuplexService
    {
        [OperationContract]
        bool CheckCurrentSharedFolder(eAssignedLane_Printer Lane);

        [OperationContract]
        ePrinterVendor GetPrinterCurrentVendor(eAssignedLane_Printer Lane);

        [OperationContract]
        string GetWriteCompImagePath(eAssignedLane_Printer Lane);

        [OperationContract]
        bool IsNeedWriteCompImage(eAssignedLane_Printer Lane);

        [OperationContract]
        bool MoveToNextSharedFolder(eAssignedLane_Printer Lane);

        [OperationContract]
        bool SendCurrentInspectedPanelData(eAssignedLane_Printer Lane, int size);

        [OperationContract(IsOneWay = true)]
        void StartProcess(eAssignedLane_Printer Lane,string filename);
    }

    public interface IPrinterDuplexServiceCallback
    {
        [OperationContract]
        void AbortFlow();

        [OperationContract]
        void AutoLoadXmlFile(bool bPsudo, string solutionPath, bool bJumpMessageBox, bool bRunAfterOpened);

        //軟體Esc
        [OperationContract]
        bool ESCFlow();//硬體Esc

        [OperationContract]
        string GetValue(string field);

        [OperationContract]
        void GoStage();

        [OperationContract]
        bool OutputDir();

        [OperationContract]
        void PLCStart();//前站有板訊號

        [OperationContract]
        void UpdateCAD();
    }

    #endregion ServiiceContract

    #region DataContract

    [DataContract(Name = "eAssignedLane_Printer")]
    public enum eAssignedLane_Printer
    {
        [EnumMember]
        None,

        [EnumMember]
        Lane1,

        [EnumMember]
        Lane2
    }

    [DataContract(Name = "ePrinterVendor")]
    public enum ePrinterVendor
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        DEK,

        [EnumMember]
        EKRA,

        [EnumMember]
        MPM,

        [EnumMember]
        DESEN,

        [EnumMember]
        MINAMI,

        [EnumMember]
        GKG,

        [EnumMember]
        HTGD,

        [EnumMember]
        ESE,

        [EnumMember]
        INOTIS,

        [EnumMember]
        HANWHA,

        [EnumMember]
        YAMAHA,

        [EnumMember]
        FUJI
    }

    #endregion DataContract
}