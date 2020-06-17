using GalaSoft.MvvmLight;
using PrinterCenter.Localization;
using PrinterCenter.Log;
using System.Collections.ObjectModel;

namespace PrinterCenter.UI.OneLaneSelector
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class LaneSelectorHostVM : ViewModelBase
    {

        /// <summary>
        /// Initializes a new instance of the LaneSelectorHostVM class.
        /// </summary>
        public LaneSelectorHostVM()
        {
            _LaneContainer = new ObservableCollection<ucOneLaneSelector>();
        }


        
        private ObservableCollection<ucOneLaneSelector> _LaneContainer;
        public ObservableCollection<ucOneLaneSelector> LaneContainer
        {
            get { return _LaneContainer; }
            set { Set(() => LaneContainer, ref _LaneContainer, value); }
        }


        #region Add按鈕執行程序
        private GalaSoft.MvvmLight.Command.RelayCommand _AddLaneCommand;
        public GalaSoft.MvvmLight.Command.RelayCommand AddLaneCommand
        {
            get { return _AddLaneCommand ?? (_AddLaneCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteAddLane, () => CanExecuteAddLane)); }
            set { _AddLaneCommand = value; }
        }
        bool _canExecuteAddLane = true;
        public bool CanExecuteAddLane
        {
            get { return _canExecuteAddLane; }
            set { if (value != _canExecuteAddLane) { _canExecuteAddLane = value; AddLaneCommand.RaiseCanExecuteChanged(); } }
        }
        void ExecuteAddLane()
        {
            Log4.PrinterLogger.Info("[A]Press Add button.");

            if (LaneContainer.Count < 2)
            {
                AddOneLaneSetting();

                if (LaneContainer.Count == 1)
                    PrinterManager.getInstance().EnableLane1SettingUI(true);
                else 
                if (LaneContainer.Count == 2)
                {
                    PrinterManager.getInstance().EnableLane1SettingUI(true);
                    PrinterManager.getInstance().EnableLane2SettingUI(true);
                }

            }
            else
            {
                Log4.PrinterLogger.Info("[A]upport at most 2 lane.");
                TRMessageBox.Show("@SUPPORT_AT_MOST_2_LANE".Translate(), "@PRINTER_CENTER".Translate());
            }
        }
        #endregion

        #region Delete按鈕執行程序

        private GalaSoft.MvvmLight.Command.RelayCommand _DeleteLaneCommand;
        public GalaSoft.MvvmLight.Command.RelayCommand DeleteLaneCommand
        {
            get { return _DeleteLaneCommand ?? (_DeleteLaneCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteDeleteLane, () => CanExecuteDeleteLane)); }
            set { _DeleteLaneCommand = value; }
        }
        bool _canExecuteDeleteLane = true;
        public bool CanExecuteDeleteLane
        {
            get { return _canExecuteDeleteLane; }
            set { if (value != _canExecuteDeleteLane) { _canExecuteDeleteLane = value; DeleteLaneCommand.RaiseCanExecuteChanged(); } }
        }
        void ExecuteDeleteLane()
        {
            Log4.PrinterLogger.Info("[A]Press Delete button.");
           
            if (LaneContainer.Count > 0)
            {
                RemoveOneLaneSetting();
     
                if (LaneContainer.Count == 1)
                {
                    PrinterManager.getInstance().EnableLane2SettingUI(false);
                }
                else 
                if (LaneContainer.Count == 0)
                {
                    PrinterManager.getInstance().EnableLane1SettingUI(false);
                    PrinterManager.getInstance().EnableLane2SettingUI(false);
                }


            }
            else
            {
                Log4.PrinterLogger.Info("[A]There is no data.");
                TRMessageBox.Show("@THERE_IS_NO_DATA".Translate(), "@PRINTER_CENTER".Translate());
            }
        }
        #endregion



        public void AddOneLaneSetting()
        {
           
            LaneContainer.Add(new ucOneLaneSelector(LaneContainer.Count));
       
        }
        /// <summary>
        /// 用於Load檔
        /// </summary>
        /// <param name="savedfile">The savedfile.</param>
        public void AddOneLaneSetting(OneLaneSelectorVM savedfile)
        {
            LaneContainer.Add(new ucOneLaneSelector(LaneContainer.Count, savedfile));
            //UI顯示
            if (LaneContainer.Count == 1)
                PrinterManager.getInstance().EnableLane1SettingUI(true);
            else
                if (LaneContainer.Count == 2)
            {
                PrinterManager.getInstance().EnableLane1SettingUI(true);
                PrinterManager.getInstance().EnableLane2SettingUI(true);
            }
        }
        public void RemoveOneLaneSetting()
        {
            LaneContainer.RemoveAt(LaneContainer.Count - 1);
        }

    }
}