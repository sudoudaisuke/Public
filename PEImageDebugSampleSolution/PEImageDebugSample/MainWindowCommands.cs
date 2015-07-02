using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PEImageDebugSample {
    public class FileInfoCommand : ICommand, INotifyPropertyChanged, INotifyPropertyChanging {

        private readonly MainWindowViewModel _viewModel;

        internal MainWindowViewModel ViewModel {
            get { return _viewModel; }
        }

        public FileInfoCommand(MainWindowViewModel vm) {
            _viewModel = vm;
            _isTaskIdle = true;
        }

        public bool CanExecute(object parameter) {
            return IsTaskIdle;
        }

        private bool _isTaskIdle;
        /// <summary>
        /// コマンドのタスクが実行中かどうかを取得します。
        /// </summary>
        public bool IsTaskIdle {
            get { return _isTaskIdle; }
            private set {
                if (_isTaskIdle == value) {
                    return;
                }
                RaisePropertyChanging();
                _isTaskIdle = value;
                RaisePropertyChanged();
                OnCanExecuteChanged(EventArgs.Empty);
            }
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged(EventArgs e) {
            var handler = CanExecuteChanged;
            if (handler == null) {
                return;
            }
            handler(this, e);
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

        public async void Execute(object parameter) {
            try {
                IsTaskIdle = false;
                await Exec(ViewModel.TargetDirectory);
            } finally {
                IsTaskIdle = true;
            }
        }

        private async Task<object> Exec(string path) {
            var tasks = new List<Task<object>>();
            var entries = Directory.EnumerateFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);
            var files = new List<string>();
            foreach (var entry in new EnumerableExtention<string>(entries, x => x.GetType() == typeof(UnauthorizedAccessException))) {
                var attribute = File.GetAttributes(entry);
                if ((attribute & FileAttributes.Directory) == FileAttributes.Directory) {
                    tasks.Add(Exec(entry));
                } else {
                    //TODO: file
                }
            }
            var a = await Task.WhenAll(tasks);
            return a;
        }
    }

    public class ShowDirectoryPicker : ICommand {

        private readonly MainWindowViewModel _viewModel;

        internal MainWindowViewModel ViewModel {
            get { return _viewModel; }
        }

        public ShowDirectoryPicker(MainWindowViewModel viewModel) {
            _viewModel = viewModel;
            //TODO: CanExecuteChanged 
        }

        public bool CanExecute(object parameter) {
            return ViewModel.FileInfoCommand.IsTaskIdle;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter) {
            using (var dialog = new CommonOpenFileDialog()) {
                dialog.IsFolderPicker = true;
                var cr = dialog.ShowDialog(System.Windows.Application.Current.MainWindow);
                if (cr != CommonFileDialogResult.Ok) {
                    return;
                }
                _viewModel.TargetDirectory = dialog.FileName;
            }
        }
    }
}
