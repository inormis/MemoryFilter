using System;
using System.Collections.Generic;
using System.ComponentModel;
using MemoryFilter.Domain;
using MemoryFilter.Importer.Services.Notifications;

namespace MemoryFilter.Importer {

    public interface IMediaCollection : INotifyPropertyChanged, IDisposable {
        IMediaItem[] Items { get; }
        int TotalImages { get; }
        int TotalVideos { get; }
        IProgressNotifier ProgressNotifier { get; }
        string SourceDirectory { get; set; }
        event Action IsEnabledChanged;
        IEnumerable<IMediaFile> GetFiles();
        void Rebuild();
    }

}