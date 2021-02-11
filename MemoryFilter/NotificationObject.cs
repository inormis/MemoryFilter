using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace MemoryFilter {

    public class NotificationObject : INotifyPropertyChanged, IDisposable {
        public void Dispose() {
            OnDispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            if (!Dispatcher.CurrentDispatcher.CheckAccess()) {
                Dispatcher.CurrentDispatcher.Invoke(() => RaisePropertyChanged(propertyName),
                    DispatcherPriority.Background);
                return;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaiseAllPropertiesChanged(string propertyname) {
            RaisePropertyChanged(string.Empty);
        }

        protected virtual void OnDispose() {
        }
    }

}