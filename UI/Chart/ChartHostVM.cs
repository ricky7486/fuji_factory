using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PrinterCenter.UI.Chart
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class AxisIniSetting
    {
        public bool ShowGridLines;
        public double Maximun;
        public double Minimun;
        public string Title;
    }
    public class ChartIniSetting
    {
        public ChartIniSetting()
        {
            XAxis = new AxisIniSetting();
            YAxis = new AxisIniSetting();
        }
        public AxisIniSetting XAxis;
        public AxisIniSetting YAxis;
    }
     public class ChartHostVM : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the ChartHostVM class.
        /// </summary>
        public ChartHostVM()
        {
            _Dx = new ObservableCollection<KeyValuePair<double, double>>();
            _Dy = new ObservableCollection<KeyValuePair<double, double>>();
            _Theta = new ObservableCollection<KeyValuePair<double, double>>();

            DxSetting = new ChartIniSetting();
            DySetting = new ChartIniSetting();
            ThetaSetting = new ChartIniSetting();

        }
        private ObservableCollection<KeyValuePair<double, double>> _Dx;
        public ObservableCollection<KeyValuePair<double, double>> Dx
        {
            get
            {
                if (_Dx == null)
                    _Dx = new ObservableCollection<KeyValuePair<double, double>>();
                return _Dx;
            }
            set { Set(() => Dx, ref _Dx, value); }
        }

        private ObservableCollection<KeyValuePair<double, double>> _Dy;
        public ObservableCollection<KeyValuePair<double, double>> Dy
        {
            get
            {
                if (_Dy == null)
                    _Dy = new ObservableCollection<KeyValuePair<double, double>>();
                return _Dy;
            }
            set { Set(() => Dy, ref _Dy, value); }
        }

        private ObservableCollection<KeyValuePair<double, double>> _Theta;
        public ObservableCollection<KeyValuePair<double, double>> Theta
        {
            get
            {
                if (_Theta == null)
                    _Theta = new ObservableCollection<KeyValuePair<double, double>>();
                return _Theta;
            }
            set { Set(() => Theta, ref _Theta, value); }
        }



        public ChartIniSetting DxSetting;

        public ChartIniSetting DySetting;

        public ChartIniSetting ThetaSetting;


    }
    

}