using System;
using System.Threading.Tasks;
using MemoryFilter.Importer.Services;
using MemoryFilter.Importer.Services.Notifications;

namespace MemoryFilter.Importer {

    public class MediaFileManager : IMediaFileManager {
        private readonly Func<IFilterTask> _filterTaskFactory;

        public MediaFileManager(Func<IFilterTask> filterTaskFactory) {
            _filterTaskFactory = filterTaskFactory;
        }

        public Task Execute(IProgressNotifier notifier, string targetDirectory) {
            var executionResult = _filterTaskFactory();
            return executionResult.Execute(notifier, targetDirectory);
        }
    }

}