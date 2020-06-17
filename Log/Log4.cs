using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrinterCenter.Log
{
    public static class Log4
    {
        public static readonly ILog PrinterLogger = log4net.LogManager.GetLogger("PrinterLogger");
    }
}
