using System.Threading.Tasks;
using MemoryFilter.Importer.Services.Notifications;

namespace MemoryFilter.Importer {

    public interface IMediaFileManager {
        Task Execute(IProgressNotifier notifier, string targetDirectory);
    }

}