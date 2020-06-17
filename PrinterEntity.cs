using PrinterCenter.Localization;
using PrinterCenter.Printer;
using PrinterCenter.Service;
using PrinterCenter.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PrinterCenter
{
    public class PrinterEntity
    {
        public eAssignedLane_Printer Lane { private set; get; }
        public PrinterEntity(eAssignedLane_Printer lane, ePrinterVendor vendor)
        {
            Lane = lane;
            PrinterVendor = vendor;

        }
        public PrinterBase Printer { private set; get; }
        public ePrinterVendor PrinterVendor
        {
            get
            {
                return Printer.Vendor;
            }
            set
            {
                Log.Log4.PrinterLogger.InfoFormat("[Security] CheckPrinterLicense...");
                if (CheckPrinterLicense(value) == false)
                {
                    Log.Log4.PrinterLogger.InfoFormat("[Security] ...Failed!");
                    var ss = string.Format("{0}\n ( {1} {2} )",
                         "@PLEASE_CHECK_YOUR_LICENCE".Translate(),
                         value,
                          "@PLACER".Translate());
                    TRMessageBox.Show(ss,
                        "@CLOSED_LOOP".Translate(),
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    value = ePrinterVendor.None;
                }
                Log.Log4.PrinterLogger.InfoFormat("[Security] ...Pass!");
                GeneratePrinterObj(value);
            }
        }

        private bool CheckPrinterLicense(ePrinterVendor vendor)
        {
            if (TRILicense.IsVaild<eCategory55B>(eCategory55B.AdminTool) == true ||
                TRILicense.IsVaild<eCategory55B>(eCategory55B.PrintCL) == true)
                return true;
            switch (vendor)
            {
                case ePrinterVendor.MPM: return TRILicense.IsVaild<eCategory55B>(eCategory55B.PrintCL_MPM);
                case ePrinterVendor.DEK: return TRILicense.IsVaild<eCategory55B>(eCategory55B.PrintCL_DEK);
                case ePrinterVendor.EKRA: return TRILicense.IsVaild<eCategory61B>(eCategory61B.PrintCL_EKRA);
                case ePrinterVendor.DESEN: return TRILicense.IsVaild<eCategory61B>(eCategory61B.PrintCL_DESEN);
                case ePrinterVendor.MINAMI: return TRILicense.IsVaild<eCategory62B>(eCategory62B.PrintCL_MINAMI);
                case ePrinterVendor.GKG: return TRILicense.IsVaild<eCategory62B>(eCategory62B.PrintCL_GKG);
                case ePrinterVendor.HTGD: return true;
                case ePrinterVendor.ESE: return TRILicense.IsVaild<eCategory62B>(eCategory62B.PrintCL_ESE);
                case ePrinterVendor.INOTIS: return true; //No license
                case ePrinterVendor.YAMAHA: return true; //No license
                case ePrinterVendor.HANWHA: return true; //No license
                case ePrinterVendor.FUJI: return true;//No license
                case ePrinterVendor.None: return true;
                default: return false;
            }
        }

        private void GeneratePrinterObj(ePrinterVendor value)
        {
            switch (value)
            {
                case ePrinterVendor.DEK: Printer = new DekPrinter(Lane); break;
                case ePrinterVendor.EKRA: Printer = new EkraPrinter(Lane); break;
                case ePrinterVendor.MPM: Printer = new MPMPrinter(Lane); break;
                case ePrinterVendor.DESEN: Printer = new DesenPrinter(Lane); break;
                case ePrinterVendor.MINAMI: Printer = new MinamiPrinter(Lane); break;
                case ePrinterVendor.GKG: Printer = new GKGPrinter(Lane); break;
                case ePrinterVendor.HTGD: Printer = new HtgdPrinter(Lane); break;
                case ePrinterVendor.ESE: Printer = new EsePrinter(Lane); break;

                case ePrinterVendor.HANWHA: Printer = new HanwhaPrinter(Lane); break;
                case ePrinterVendor.YAMAHA: Printer = new YamahaPrinter(Lane); break;
                //case ePrinterVendor.None: Printer = new NonePrinter(); break;
                case ePrinterVendor.INOTIS: Printer = new INOTISPrinter(Lane); break;
                case ePrinterVendor.FUJI: Printer = new FujiPrinter(Lane); break;
                default:
                    //Printer = new PrinterBase(value,Lane);
                    break;
                //default: break;
            }
        }
    }
}
