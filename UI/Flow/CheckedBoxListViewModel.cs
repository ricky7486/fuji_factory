using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
/// <summary>
/// http://www.thejoyofcode.com/ViewModels_and_CheckListBoxes.aspx
/// </summary>
namespace PrinterCenter.UI.Flow
{

    public class CheckedBoxListViewModel : ViewModelBase
    {
        public CheckedBoxListViewModel()
        {
        }

        private CheckableObservableCollection<string> _items;

        public CheckableObservableCollection<string> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler pceh = PropertyChanged;
            if (pceh != null)
            {
                pceh(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
    }

    public class CheckWrapper<T> : INotifyPropertyChanged
    {
        private readonly CheckableObservableCollection<T> _parent;

        public CheckWrapper(CheckableObservableCollection<T> parent)
        {
            _parent = parent;
        }

        private T _value;

        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                CheckChanged();
                OnPropertyChanged("IsChecked");
            }
        }

        private void CheckChanged()
        {
            _parent.Refresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler pceh = PropertyChanged;
            if (pceh != null)
            {
                pceh(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


    public class CheckableObservableCollection<T> : ObservableCollection<CheckWrapper<T>>
    {
        private ListCollectionView _selected;

        public CheckableObservableCollection()
        {
            _selected = new ListCollectionView(this);
            _selected.Filter = delegate (object checkObject) {
                return ((CheckWrapper<T>)checkObject).IsChecked;
            };
        }

        public void Add(T item)
        {
            this.Add(new CheckWrapper<T>(this) { Value = item });
        }

        public ICollectionView CheckedItems
        {
            get { return _selected; }
        }

        internal void Refresh()
        {
            _selected.Refresh();
        }
    }
}