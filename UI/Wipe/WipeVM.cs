using GalaSoft.MvvmLight;
using PrinterCenter.Localization;
using PrinterCenter.Printer.JudgeWipe;
using PrinterCenter.UI.CommonSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PrinterCenter.UI.Wipe
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class WipeVM : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the WipeVM class.

        /// </summary>
        /// 
        #region 資料
        private Range _AvgVolRange;
        public Range AvgVolRange
        {
            get { return _AvgVolRange; }
            set { Set(() => AvgVolRange, ref _AvgVolRange, value); }
        }
        private Range _PeakRange;
        public Range PeakRange
        {
            get { return _PeakRange; }
            set { Set(() => PeakRange, ref _PeakRange, value); }
        }


        private int _PadFailTimes;
        public int PadFailTimes
        {
            get { return _PadFailTimes; }
            set { Set(() => PadFailTimes, ref _PadFailTimes, value); }
        }
        private int _CmpFailTimes;
        public int CmpFailTimes
        {
            get { return _CmpFailTimes; }
            set { Set(() => CmpFailTimes, ref _CmpFailTimes, value); }
        }
        private ObservableCollection<string> _WipeCombination;
        public ObservableCollection<string> WipeCombination
        {
            get { return _WipeCombination; }
            set { Set(() => WipeCombination, ref _WipeCombination, value); }
        }

        private String _SelectedItem;
        public String SelectedItem
        {
            get { return _SelectedItem; }
            set { Set(() => SelectedItem, ref _SelectedItem, value); }
        }
        private int _SelectedIndex;
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set { Set(() => SelectedIndex, ref _SelectedIndex, value); }
        }

        #endregion

        #region command
        private GalaSoft.MvvmLight.Command.RelayCommand _W1Command;
        public GalaSoft.MvvmLight.Command.RelayCommand W1Command
        {
            get { return _W1Command ?? (_W1Command = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteW1, () => CanExecuteW1)); }
            set { _W1Command = value; }
        }
        bool _canExecuteW1 = true;
        public bool CanExecuteW1
        {
            get { return _canExecuteW1; }
            set { if (value != _canExecuteW1) { _canExecuteW1 = value; W1Command.RaiseCanExecuteChanged(); } }
        }
        void ExecuteW1()
        {
            WipeCombination.Add(String.Format("W1.{0} ({1}-{2})",
                "@AVERAGE_VOLUME_PERCENTAGE".Translate(),
                ViewModelLocator.Atom.WipeVM.AvgVolRange.LowerBound,
                ViewModelLocator.Atom.WipeVM.AvgVolRange.UpperBound
                ));
            object[] _params = { (object)ViewModelLocator.Atom.WipeVM.AvgVolRange.LowerBound
                , (object)ViewModelLocator.Atom.WipeVM.AvgVolRange.UpperBound };
            var alg = JudgeWipeAlgorithmFactory.CreateAlgorithm("W1", _params);
            PrinterManager.getInstance().JudgeWipeRoutines.Add(alg);
        }
        private GalaSoft.MvvmLight.Command.RelayCommand _W2Command;
        public GalaSoft.MvvmLight.Command.RelayCommand W2Command
        {
            get { return _W2Command ?? (_W2Command = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteW2, () => CanExecuteW2)); }
            set { _W2Command = value; }
        }
        bool _canExecuteW2 = true;
        public bool CanExecuteW2
        {
            get { return _canExecuteW2; }
            set { if (value != _canExecuteW2) { _canExecuteW2 = value; W2Command.RaiseCanExecuteChanged(); } }
        }
        void ExecuteW2()
        {
            WipeCombination.Add(String.Format("W2.{0} ({1}-{2})",
                "@PEAK_OF_SINGLE_PAD_VOLUME".Translate(),
                ViewModelLocator.Atom.WipeVM.PeakRange.LowerBound,
                ViewModelLocator.Atom.WipeVM.PeakRange.UpperBound
                ));
            object[] _params = { (object)ViewModelLocator.Atom.WipeVM.PeakRange.LowerBound
                , (object)ViewModelLocator.Atom.WipeVM.PeakRange.UpperBound };
            var alg = JudgeWipeAlgorithmFactory.CreateAlgorithm("W2", _params);
            PrinterManager.getInstance().JudgeWipeRoutines.Add(alg);
        }
        private GalaSoft.MvvmLight.Command.RelayCommand _W3Command;
        public GalaSoft.MvvmLight.Command.RelayCommand W3Command
        {
            get { return _W3Command ?? (_W3Command = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteW3, () => CanExecuteW3)); }
            set { _W3Command = value; }
        }
        bool _canExecuteW3 = true;
        public bool CanExecuteW3
        {
            get { return _canExecuteW3; }
            set { if (value != _canExecuteW3) { _canExecuteW3 = value; W3Command.RaiseCanExecuteChanged(); } }
        }
        void ExecuteW3()
        {
            WipeCombination.Add(String.Format("W3.{0}", "@BRIDGE_DETECT".Translate()));
           
            var alg = JudgeWipeAlgorithmFactory.CreateAlgorithm("W3",null);
            PrinterManager.getInstance().JudgeWipeRoutines.Add(alg);
        }

        private GalaSoft.MvvmLight.Command.RelayCommand _W4Command;
        public GalaSoft.MvvmLight.Command.RelayCommand W4Command
        {
            get { return _W4Command ?? (_W4Command = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteW4, () => CanExecuteW4)); }
            set { _W4Command = value; }
        }
        bool _canExecuteW4 = true;
        public bool CanExecuteW4
        {
            get { return _canExecuteW4; }
            set { if (value != _canExecuteW4) { _canExecuteW4 = value; W4Command.RaiseCanExecuteChanged(); } }
        }
        void ExecuteW4()
        {
            WipeCombination.Add(string.Format("W4.{0} ({1})", "@CONTINUE_FAIL_AT_SAME_PAD".Translate(), ViewModelLocator.Atom.WipeVM.PadFailTimes));
            object[] _params = { (object)ViewModelLocator.Atom.WipeVM.PadFailTimes };
            var alg = JudgeWipeAlgorithmFactory.CreateAlgorithm("W4", _params);
            PrinterManager.getInstance().JudgeWipeRoutines.Add(alg);
        }

        private GalaSoft.MvvmLight.Command.RelayCommand _W5Command;
        public GalaSoft.MvvmLight.Command.RelayCommand W5Command
        {
            get { return _W5Command ?? (_W5Command = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteW5, () => CanExecuteW5)); }
            set { _W5Command = value; }
        }
        bool _canExecuteW5 = true;
        public bool CanExecuteW5
        {
            get { return _canExecuteW5; }
            set { if (value != _canExecuteW5) { _canExecuteW5 = value; W5Command.RaiseCanExecuteChanged(); } }
        }
        void ExecuteW5()
        {
            WipeCombination.Add(string.Format("W5.{0} ({1})", "@CONTINUE_FAIL_AT_SAME_COMPONENT".Translate(), ViewModelLocator.Atom.WipeVM.CmpFailTimes));
            object[] _params = { (object)ViewModelLocator.Atom.WipeVM.CmpFailTimes };
            var alg = JudgeWipeAlgorithmFactory.CreateAlgorithm("W5", _params);
            PrinterManager.getInstance().JudgeWipeRoutines.Add(alg);
        }

        private GalaSoft.MvvmLight.Command.RelayCommand _DeleteListCommand;
        public GalaSoft.MvvmLight.Command.RelayCommand DeleteListCommand
        {
            get { return _DeleteListCommand ?? (_DeleteListCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ExecuteDeleteList, () => CanExecuteDeleteList)); }
            set { _DeleteListCommand = value; }
        }
        bool _canExecuteDeleteList = true;
        public bool CanExecuteDeleteList
        {
            get { return _canExecuteDeleteList; }
            set { if (value != _canExecuteDeleteList) { _canExecuteDeleteList = value; DeleteListCommand.RaiseCanExecuteChanged(); } }
        }
        void ExecuteDeleteList()
        {
            PrinterManager.getInstance().JudgeWipeRoutines.RemoveAt(SelectedIndex);
            _WipeCombination.Remove(SelectedItem);
           
        }


        #endregion

        public WipeVM()
        {
            _WipeCombination = new ObservableCollection<string>();
            _AvgVolRange = new Range(0,100);
            _PeakRange = new Range(0,100);
        }
    }
}