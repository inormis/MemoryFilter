using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MemoryFilter.Importer.Services.Notifications {

    public interface IProgressNotifier : INotifierInstance, INotification {
        bool IsCancelled { get; }

        decimal Progress { get; }

        string CurrentAction { get; }

        ICommand CancelCommand { get; }

        ObservableCollection<LogItem> Logs { get; }

        void Update(decimal progress, string currentAction = null);

        void Log(LogItem item);
    }
}