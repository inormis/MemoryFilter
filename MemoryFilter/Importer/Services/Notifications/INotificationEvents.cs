using System;

namespace MemoryFilter.Importer.Services.Notifications {

    public interface INotificationEvents {
        string Name { get; }
        event Action Changed;

        void Cancel();
    }

}