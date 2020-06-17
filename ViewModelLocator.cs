/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:PrinterCenter"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using PrinterCenter.UI.Chart;
using PrinterCenter.UI.Custom;
using PrinterCenter.UI.Doctor;
using PrinterCenter.UI.Flow;
using PrinterCenter.UI.FujiEasyLink;
using PrinterCenter.UI.OneLaneSelector;
using PrinterCenter.UI.Wipe;

namespace PrinterCenter
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public static ViewModelLocator Atom; // created in static constructor
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            Atom = new ViewModelLocator();
        }
        public ViewModelLocator()
        {

            SimpleIoc.Default.Register<PrinterWindowVM>();

            SimpleIoc.Default.Register<ChartHostVM>();
            SimpleIoc.Default.Register<FlowHostVM>();
            SimpleIoc.Default.Register<WipeVM>();
            SimpleIoc.Default.Register<DoctorVM>();

            //=========================
            SimpleIoc.Default.Register<LaneSelectorHostVM>();
            SimpleIoc.Default.Register<CustomVM>();
            SimpleIoc.Default.Register<FujiEasyLinkVM>();
        }
        public PrinterWindowVM PrinterWindowVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PrinterWindowVM>();
            }
        }
        public ChartHostVM ChartHostVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ChartHostVM>();
            }
        }
        public FlowHostVM FlowHostVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<FlowHostVM>();
            }
        }
        public WipeVM WipeVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WipeVM>();
            }
        }
        public DoctorVM DoctorVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DoctorVM>();
            }
        }


        //=================================
        public LaneSelectorHostVM LaneSelectorHostVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LaneSelectorHostVM>();
            }
        }

        public CustomVM CustomVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CustomVM>();
            }
        }
        public FujiEasyLinkVM FujiEasyLinkVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<FujiEasyLinkVM>();
            }
        }
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }

}