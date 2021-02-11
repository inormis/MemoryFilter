using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MemoryFilter.Domain;
using MemoryFilter.Importer.Services.Notifications;
using MemoryFilter.Services;
using MemoryFilter.Settings;

namespace MemoryFilter.Importer.Services {

    public class ExecutionResultBase : NotificationObject {
        public bool IsCancelled { get; private set; }

        public void Cancel() {
            IsCancelled = true;
        }
    }

    public interface IFilterTask {
        Task Execute(IProgressNotifier progressNotifier, string targetDirectory);
    }

    public class FilterTask : ExecutionResultBase, IExecutionResult, IFilterTask {
        private readonly IConfiguration _configuration;
        private readonly IFileService _fileService;
        private readonly IMediaCollection _mediaCollection;
        private decimal _completedSizeInMb;
        private bool _isCompleted;
        private decimal _progress;

        public FilterTask(IMediaCollection mediaCollection, IFileService fileService,
            IConfiguration configuration) {
            _fileService = fileService;
            _configuration = configuration;
            _mediaCollection = mediaCollection;
            _mediaCollection.IsEnabledChanged += OnIsEnabledChanged;
            TotalSize = mediaCollection.GetFiles()
                .Where(x => x.IsEnabled == true && configuration.IsSupported(x))
                .Sum(x => x.SizeInMb);
        }

        public bool IsCompleted {
            get => IsCancelled || _isCompleted;
            private set {
                _isCompleted = value;
                _progress = 0;
                ProgressChanged?.Invoke();
            }
        }

        public decimal TotalSize { get; }

        public decimal CompletedSizeInMb {
            get => _completedSizeInMb;
            private set {
                _completedSizeInMb = value;
                RaisePropertyChanged(nameof(CompletedSizeInMb));
            }
        }

        public decimal Progress {
            get => _progress;
            private set {
                _progress = value;
                RaisePropertyChanged(nameof(Progress));
            }
        }

        public void MarkAsCompleted(IMediaFile mediaFile) {
            CompletedSizeInMb += mediaFile.SizeInMb;
            Progress = Math.Round(CompletedSizeInMb / TotalSize * 100, 2);
            mediaFile.Completed = true;

            ProgressChanged?.Invoke();
        }

        public string PercentageString => (CompletedSizeInMb * 100 / TotalSize).ToString("N2");

        public event Action ProgressChanged;

        public Task Execute(IProgressNotifier progressNotifier, string targetDirectory) {
            return Task.Run(() => OnExecute(progressNotifier, targetDirectory));
        }

        private void OnExecute(IProgressNotifier progressNotifier, string targetDirectory) {
            try {
                _fileService.EnsureDirectoryExists(targetDirectory);
                foreach (var mediaFile in _mediaCollection.GetFiles()) {
                    if (progressNotifier.IsCancelled) {
                        return;
                    }

                    if (!_fileService.FileExists(mediaFile.FilePath)) {
                        progressNotifier.Log(new LogItem($"{mediaFile.FilePath} is missing in source path", mediaFile.FilePath, LogLevel.Warning));
                        continue;
                    }

                    var targetFilePath = _configuration.CreatePath(mediaFile, targetDirectory);
                    _fileService.EnsureDirectoryExists(Path.GetDirectoryName(targetFilePath));

                    if (_configuration.IsSupported(mediaFile)) {
                        _fileService.Move(mediaFile, targetFilePath);
                        MarkAsCompleted(mediaFile);
                    }
                }
            }
            catch (Exception e) {
                e.CaptureException();
            }   
        }

        private void OnIsEnabledChanged() {
        }

        protected override void OnDispose() {
            _mediaCollection.IsEnabledChanged -= OnIsEnabledChanged;
        }
    }

    public class ExecutionResultWithValue<T> : ExecutionResultBase, IAsyncResult<T> {
        public T Value { get; private set; }

        public void SetValue(T value) {
            Value = value;
        }
    }


    public interface IFilter {
        bool CanProcess(IMediaFile mediaFile);
    }

}