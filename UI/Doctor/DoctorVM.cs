using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PrinterCenter.UI.Doctor
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class DoctorVM : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the DoctorVM class.
        /// </summary>
        public DoctorVM()
        {
            _Analysis = new ObservableCollection<KeyValuePair<string, double>>();
            _Analysis.Add(new KeyValuePair<string, double>("塞孔", 35));
            _Analysis.Add(new KeyValuePair<string, double>("刮刀壓力太大", 25));
            _Analysis.Add(new KeyValuePair<string, double>("刮刀壓力太小", 15));
            _Analysis.Add(new KeyValuePair<string, double>("刮刀速度太大", 25));

        }

        private ObservableCollection<KeyValuePair<string, double>> _Analysis;
        public ObservableCollection<KeyValuePair<string, double>> Analysis
        {
            get
            {
                if (_Analysis == null)
                    _Analysis = new ObservableCollection<KeyValuePair<string, double>>();
                return _Analysis;
            }
            set { Set(() => Analysis, ref _Analysis, value); }
        }
    }
}