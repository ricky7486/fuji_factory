
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace PrinterCenter.Service
{
    public class PrinterServiceHost
    {
        ServiceHost _serviceHost;
        public PrinterDuplexService PrinterDuplexServiceInstance;
        private PrinterServiceHost()
        {
            _serviceHost = new ServiceHost(typeof(PrinterDuplexService));

        }

        void _serviceHost_Faulted(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OpenHost(Action updateUI)
        {
            if (_serviceHost.State == CommunicationState.Closed)
                _serviceHost = new ServiceHost(typeof(PrinterDuplexService));
            _serviceHost.Open();

            if (_serviceHost.State == CommunicationState.Opened)
                if (updateUI != null)
                    updateUI();
        }

        public string HostState()
        {
            return _serviceHost.State.ToString();
        }

        public static PrinterServiceHost Instance()
        {
            return Singleton<PrinterServiceHost>.Instance;
        }
        public void CloseHost(Action updateUI)
        {
            _serviceHost.Close();
            if (_serviceHost.State == CommunicationState.Closed)
                if (updateUI != null)
                    updateUI();
        }



    }
}
