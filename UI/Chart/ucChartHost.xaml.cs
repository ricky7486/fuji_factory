using PrinterCenter.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrinterCenter.UI.Chart
{
    /// <summary>
    /// Interaction logic for ucChartHost.xaml
    /// </summary>
    /// 

    public partial class ucChartHost : UserControl
    {
        private IniFile _iniFile = new IniFile();
        public ucChartHost()
        {
            InitializeComponent();

            if(_iniFile.IsSectionExist("Chart_Dx_X_Axis"))
                SetChartDx_X_AxisFromIni();
            if (_iniFile.IsSectionExist("Chart_Dx_Y_Axis"))
                SetChartDx_Y_AxisFromIni();

            if (_iniFile.IsSectionExist("Chart_Dy_X_Axis"))
                SetChartDy_X_AxisFromIni();
            if (_iniFile.IsSectionExist("Chart_Dy_Y_Axis"))
                SetChartDy_Y_AxisFromIni();

            if (_iniFile.IsSectionExist("Chart_Theta_X_Axis"))
                SetChartTheta_X_AxisFromIni();
            if (_iniFile.IsSectionExist("Chart_Theta_Y_Axis"))
                SetChartTheta_Y_AxisFromIni();


            string[] items = new string[]
            {
                "@LANE1".Translate(),
                "@LANE2".Translate()//,
                //"@LANE1".Translate()+" + "+"@LANE2".Translate()
            };
            cbDisplaySelector.ItemsSource = items;
            cbDisplaySelector.SelectedIndex = 0;
        
        }
        private void SetChartDx_X_AxisFromIni()
        {
            var setting = ViewModelLocator.Atom.ChartHostVM.DxSetting.XAxis;

            var Dx_XAxis = ((chartDx.Series.First() as LineSeries).IndependentAxis as LinearAxis);
            Dx_XAxis.Orientation = AxisOrientation.X;
            string title = _iniFile.Read("Chart_Dx_X_Axis", "Title");
            Dx_XAxis.Title = title;
            setting.Title = title;
            bool bShowGridLines = false;
            Boolean.TryParse(_iniFile.Read("Chart_Dx_X_Axis", "ShowGridLines"), out bShowGridLines);
            Dx_XAxis.ShowGridLines = bShowGridLines;
            setting.ShowGridLines = bShowGridLines;
            double dXMin = 0.0;
            double dXMax = 0.0;
            Double.TryParse(_iniFile.Read("Chart_Dx_X_Axis", "Minimum"), out dXMin);
            Double.TryParse(_iniFile.Read("Chart_Dx_X_Axis", "Maximun"), out dXMax);
            Dx_XAxis.Minimum = dXMin;
            Dx_XAxis.Maximum = dXMax;
            setting.Minimun = dXMin;
            setting.Maximun = dXMax;
        }
        private void SetChartDx_Y_AxisFromIni()
        {
            var setting = ViewModelLocator.Atom.ChartHostVM.DxSetting.YAxis;

            var Dx_YAxis = ((chartDx.Series.First() as LineSeries).DependentRangeAxis as LinearAxis);
            Dx_YAxis.Orientation = AxisOrientation.Y;
            string title =_iniFile.Read("Chart_Dx_Y_Axis", "Title");
            Dx_YAxis.Title = title;
            setting.Title = title;
            bool bShowGridLines = false;
            Boolean.TryParse(_iniFile.Read("Chart_Dx_Y_Axis", "ShowGridLines"), out bShowGridLines);
            Dx_YAxis.ShowGridLines = bShowGridLines;
            setting.ShowGridLines = bShowGridLines;
            double dXMin = 0.0;
            double dXMax = 0.0;
            Double.TryParse(_iniFile.Read("Chart_Dx_Y_Axis", "Minimum"), out dXMin);
            Double.TryParse(_iniFile.Read("Chart_Dx_Y_Axis", "Maximun"), out dXMax);
            Dx_YAxis.Minimum = dXMin;
            Dx_YAxis.Maximum = dXMax;
            setting.Minimun = dXMin;
            setting.Maximun = dXMax;
        }



        private void SetChartDy_X_AxisFromIni()
        {
            var setting = ViewModelLocator.Atom.ChartHostVM.DySetting.XAxis;

            var Dy_XAxis = ((chartDy.Series.First() as LineSeries).IndependentAxis as LinearAxis);
            Dy_XAxis.Orientation = AxisOrientation.X;
            string title = _iniFile.Read("Chart_Dy_X_Axis", "Title");
            Dy_XAxis.Title = title;
            setting.Title = title;
            bool bShowGridLines = false;
            Boolean.TryParse(_iniFile.Read("Chart_Dy_X_Axis", "ShowGridLines"), out bShowGridLines);
            Dy_XAxis.ShowGridLines = bShowGridLines;
            setting.ShowGridLines = bShowGridLines;
            double dXMin = 0.0;
            double dXMax = 0.0;
            Double.TryParse(_iniFile.Read("Chart_Dy_X_Axis", "Minimum"), out dXMin);
            Double.TryParse(_iniFile.Read("Chart_Dy_X_Axis", "Maximun"), out dXMax);
            Dy_XAxis.Minimum = dXMin;
            Dy_XAxis.Maximum = dXMax;
            setting.Minimun = dXMin;
            setting.Maximun = dXMax;
        }
        private void SetChartDy_Y_AxisFromIni()
        {
            var setting = ViewModelLocator.Atom.ChartHostVM.DySetting.YAxis;

            var Dy_YAxis = ((chartDy.Series.First() as LineSeries).DependentRangeAxis as LinearAxis);
            Dy_YAxis.Orientation = AxisOrientation.Y;
            string title = _iniFile.Read("Chart_Dy_Y_Axis", "Title");
            Dy_YAxis.Title = title;
            setting.Title = title;
            bool bShowGridLines = false;
            Boolean.TryParse(_iniFile.Read("Chart_Dy_Y_Axis", "ShowGridLines"), out bShowGridLines);
            Dy_YAxis.ShowGridLines = bShowGridLines;
            setting.ShowGridLines = bShowGridLines;
            double dXMin = 0.0;
            double dXMax = 0.0;
            Double.TryParse(_iniFile.Read("Chart_Dy_Y_Axis", "Minimum"), out dXMin);
            Double.TryParse(_iniFile.Read("Chart_Dy_Y_Axis", "Maximun"), out dXMax);
            Dy_YAxis.Minimum = dXMin;
            Dy_YAxis.Maximum = dXMax;
            setting.Minimun = dXMin;
            setting.Maximun = dXMax;
        }


        private void SetChartTheta_X_AxisFromIni()
        {
            var setting = ViewModelLocator.Atom.ChartHostVM.ThetaSetting.XAxis;

            var Theta_XAxis = ((chartTheta.Series.First() as LineSeries).IndependentAxis as LinearAxis);
            Theta_XAxis.Orientation = AxisOrientation.X;
            string title = _iniFile.Read("Chart_Theta_X_Axis", "Title");
            Theta_XAxis.Title = title;
            setting.Title = title;
            bool bShowGridLines = false;
            Boolean.TryParse(_iniFile.Read("Chart_Theta_X_Axis", "ShowGridLines"), out bShowGridLines);
            Theta_XAxis.ShowGridLines = bShowGridLines;
            setting.ShowGridLines = bShowGridLines;
            double dXMin = 0.0;
            double dXMax = 0.0;
            Double.TryParse(_iniFile.Read("Chart_Theta_X_Axis", "Minimum"), out dXMin);
            Double.TryParse(_iniFile.Read("Chart_Theta_X_Axis", "Maximun"), out dXMax);
            Theta_XAxis.Minimum = dXMin;
            Theta_XAxis.Maximum = dXMax;
            setting.Minimun = dXMin;
            setting.Maximun = dXMax;
        }
        private void SetChartTheta_Y_AxisFromIni()
        {
            var setting = ViewModelLocator.Atom.ChartHostVM.ThetaSetting.YAxis;

            var Theta_YAxis = ((chartTheta.Series.First() as LineSeries).DependentRangeAxis as LinearAxis);
            Theta_YAxis.Orientation = AxisOrientation.Y;
            string title =_iniFile.Read("Chart_Theta_Y_Axis", "Title");
            Theta_YAxis.Title = title;
            setting.Title = title;
            bool bShowGridLines = false;
            Boolean.TryParse(_iniFile.Read("Chart_Theta_Y_Axis", "ShowGridLines"), out bShowGridLines);
            Theta_YAxis.ShowGridLines = bShowGridLines;
            setting.ShowGridLines = bShowGridLines;
            double dXMin = 0.0;
            double dXMax = 0.0;
            Double.TryParse(_iniFile.Read("Chart_Theta_Y_Axis", "Minimum"), out dXMin);
            Double.TryParse(_iniFile.Read("Chart_Theta_Y_Axis", "Maximun"), out dXMax);
            Theta_YAxis.Minimum = dXMin;
            Theta_YAxis.Maximum = dXMax;
            setting.Minimun = dXMin;
            setting.Maximun = dXMax;
        }

        private void cbDisplaySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(cbDisplaySelector.SelectedIndex )//Lane1
            {
                case 0:
                    if(PrinterManager.getInstance().RemotePrinter[0].Printer!= null)
                    {
                        ViewModelLocator.Atom.ChartHostVM.Dx = PrinterManager.getInstance().RemotePrinter[0].Printer.DxHistory;
                        ViewModelLocator.Atom.ChartHostVM.Dy = PrinterManager.getInstance().RemotePrinter[0].Printer.DyHistory;
                        ViewModelLocator.Atom.ChartHostVM.Theta = PrinterManager.getInstance().RemotePrinter[0].Printer.ThetaHistory;
                    }
                    else
                    {
                        ViewModelLocator.Atom.ChartHostVM.Dx = null;
                        ViewModelLocator.Atom.ChartHostVM.Dy = null;
                        ViewModelLocator.Atom.ChartHostVM.Theta = null;
                    }
                    break;
                case 1:
                    if(PrinterManager.getInstance().RemotePrinter[1].Printer!=null)
                    {
                        ViewModelLocator.Atom.ChartHostVM.Dx = PrinterManager.getInstance().RemotePrinter[1].Printer.DxHistory;
                        ViewModelLocator.Atom.ChartHostVM.Dy = PrinterManager.getInstance().RemotePrinter[1].Printer.DyHistory;
                        ViewModelLocator.Atom.ChartHostVM.Theta = PrinterManager.getInstance().RemotePrinter[1].Printer.ThetaHistory;
                    }
                    else
                    {
                        ViewModelLocator.Atom.ChartHostVM.Dx = null;
                        ViewModelLocator.Atom.ChartHostVM.Dy = null;
                        ViewModelLocator.Atom.ChartHostVM.Theta = null;
                    }
                    break;
                case 2:
                    break;
            }
        }

        public void RefreshSelectionChanged()
        {
            cbDisplaySelector_SelectionChanged(this,null);
        }
    }
}
