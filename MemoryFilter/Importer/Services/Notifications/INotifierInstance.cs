using System;

namespace MemoryFilter.Importer.Services.Notifications {

    public interface INotifierInstance : IDisposable {
        bool IsCancelled { get; }
    }

}