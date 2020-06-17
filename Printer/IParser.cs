using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrinterCenter.Printer
{
    public interface IParser
    {

        /// <summary>
        /// Parses the specified filepath.
        /// </summary>
        /// <param name="filepath">要分析的路徑</param>
        /// <returns>回傳分析完的客製化的物件</returns>
        object Parse(string filepath);
        /// <summary>
        /// Determines whether /[is barcode matched] [the specified fileobj].
        /// </summary>
        /// <param name="fileobj">剖析過後的客製化檔案物件</param>
        /// <param name="barcode">受測依據Barcode</param>
        /// <returns>
        ///   <c>true</c> if [is barcode matched] [the specified fileobj]; otherwise, <c>false</c>.
        /// </returns>
        bool IsBarcodeMatched(object fileobj, string barcode);
    }
}
