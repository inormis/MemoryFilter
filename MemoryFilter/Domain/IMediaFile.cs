using System;
using MemoryFilter.Importer;

namespace MemoryFilter.Domain {

    public interface IMediaFile : IMediaItem {
        string FileName { get; }
        DateTime CreationDate { get; }
        string FilePath { get; }
        MediaType MediaType { get; }
        bool Completed { get; set; }
        string CameraModel { get; }
    }

}