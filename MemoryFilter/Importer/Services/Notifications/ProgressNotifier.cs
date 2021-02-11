using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;

namespace MemoryFilter.Importer.Services.Notifications {

    public class ProgressNotifier : NotificationObject, IProgressNotifier {
        public static readonly ProgressNotifier CancelledInstance;
        private readonly RelayCommand _cancelCommand;
        private string _currentAction = "";
        private bool _isCancelled;
        private decimal _progress;

        static ProgressNotifier() {
            CancelledInstance = new ProgressNotifier();
            CancelledInstance.Dispose();
        }

        public ProgressNotifier() {
            _cancelCommand = new RelayCommand(OnCancel, CanCancel);
            CancelCommand = _cancelCommand;
        }

        public ObservableCollection<LogItem> Logs { get; } = new ObservableCollection<LogItem>();

        public ICommand CancelCommand { get; }

        public decimal Progress {
            get => _progress;
            private set {
                if (Math.Abs(Progress - value) < 0.1m) {
                    _progress = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string CurrentAction {
            get => _currentAction;
            private set {
                if (_currentAction == value) return;

                _currentAction = value;
                RaisePropertyChanged();
            }
        }

        public bool IsCancelled {
            get => _isCancelled;
            private set {
                _isCancelled = value;
                RaisePropertyChanged();
            }
        }

        public void Update(decimal progress, string currentAction = null) {
            Progress = progress;
            CurrentAction = currentAction;
        }

        public void Log(LogItem item) {
            Logs.Add(item);
        }

        private bool CanCancel() {
            return IsCancelled == false;
        }

        private void OnCancel() {
            IsCancelled = true;
        }

        protected override void OnDispose() {
            if (IsCancelled) return;

            IsCancelled = true;
            _cancelCommand.RaiseCanExecuteChanged();
            Update(0, "Cancelled");
        }
    }

}