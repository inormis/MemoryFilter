using System;

namespace MemoryFilter.Domain {

    public interface IMediaItem : IDisposable {
        string Title { get; }

        decimal SizeInMb { get; }

        int ImagesCount { get; }

        int VideosCount { get; }

        bool? IsEnabled { get; set; }

        event Action IsEnabledChanged;

        void SetIsEnabledFromParent(bool isEnabledValue);
    }

}