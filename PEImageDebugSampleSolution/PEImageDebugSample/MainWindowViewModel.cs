using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PEImageDebugSample {
    public class MainWindowViewModel : INotifyPropertyChanged, INotifyPropertyChanging, INotifyDataErrorInfo {
        public MainWindowViewModel() {
            _fileItems = new ObservableCollection<object>();
            BindingOperations.EnableCollectionSynchronization(this.FileItems, _lockObj);
            _fileInfoCommand = new FileInfoCommand(this);
            _showDirectoryPickerCommand = new ShowDirectoryPicker(this);
        }

        private readonly FileInfoCommand _fileInfoCommand;

        public FileInfoCommand FileInfoCommand {
            get { return _fileInfoCommand; }
        }

        private readonly ShowDirectoryPicker _showDirectoryPickerCommand;

        public ShowDirectoryPicker ShowDirectoryPickerCommand {
            get { return _showDirectoryPickerCommand; }
        }

        private readonly object _lockObj = new object();

        private readonly ObservableCollection<object> _fileItems;
        public ObservableCollection<object> FileItems {
            get {
                return _fileItems;
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName) {
            if (string.IsNullOrEmpty(propertyName)) {
                yield break;
            }
            string value;
            if (_errors.TryGetValue(propertyName, out value)) {
                yield return value;
            }
        }

        private readonly ConcurrentDictionary<string, string> _errors = new ConcurrentDictionary<string, string>();
        public bool HasErrors {
            get {
                return _errors.Count > 0;
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;
        private readonly ConcurrentDictionary<string, PropertyChangingEventArgs> _propertyChangingEventArgsDictionary = new ConcurrentDictionary<string, PropertyChangingEventArgs>();
        private void RaisePropertyChanging([CallerMemberName]string propertyName = "") {
            var args = _propertyChangingEventArgsDictionary.GetOrAdd(propertyName, x => new PropertyChangingEventArgs(x));
            OnPropertyChanging(args);
        }
        protected virtual void OnPropertyChanging(PropertyChangingEventArgs e) {
            var handler = PropertyChanging;
            if (handler == null) {
                return;
            }
            handler(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly ConcurrentDictionary<string, PropertyChangedEventArgs> _propertyChangedEventArgsDictionary = new ConcurrentDictionary<string, PropertyChangedEventArgs>();
        private void RaisePropertyChanged([CallerMemberName]string propertyName = "") {
            var args = _propertyChangedEventArgsDictionary.GetOrAdd(propertyName, x => new PropertyChangedEventArgs(x));
            OnPropertyChanged(args);
        }
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
            var handler = PropertyChanged;
            if (handler == null) {
                return;
            }
            handler(this, e);
        }

        private string _targetDirectory  = "";
        public string TargetDirectory {
            get { return _targetDirectory; }
            set {
                if (_targetDirectory == value) {
                    return;
                }
                RaisePropertyChanging();
                _targetDirectory = value;
                RaisePropertyChanged();
            }
        }
    }
}
