using System.Collections.Generic;
using MemoryFilter.Domain;
using MemoryFilter.Importer.Services.Notifications;

namespace MemoryFilter.Importer.Services {

    public interface IMediaService {
        IEnumerable<string> GetMediaFiles(string directoryPath, INotifierInstance instance = null);

        IMediaFile CreateMediaFile(string filePath);

        IMediaItem[] BuildTree(IMediaFile[] mediaFiles);
    }

}