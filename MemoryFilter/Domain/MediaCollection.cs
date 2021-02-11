using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemoryFilter.Importer;
using MemoryFilter.Importer.Services;
using MemoryFilter.Importer.Services.Notifications;
using MemoryFilter.Settings;
using NLog;

namespace MemoryFilter.Domain {

    public class MediaCollection : NotificationObject, IMediaCollection {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration _configuration;
        private readonly IFileService _fileService;
        private readonly IMediaService _mediaService;
        private readonly Func<IProgressNotifier> _progressNotifierFactory;

        private IMediaItem[] _items;

        private IMediaFile[] _mediaFiles = new IMediaFile[0];

        private IProgressNotifier _progressNotifier =
            Importer.Services.Notifications.ProgressNotifier.CancelledInstance;

        private string _sourceDirectory;
        private int _totalImages;
        private int _totalVideos;

        public MediaCollection(IConfiguration configuration,
            Func<IProgressNotifier> progressNotifierFactory,
            IFileService fileService,
            IMediaService mediaService) {
            _fileService = fileService;
            _configuration = configuration;
            _progressNotifierFactory = progressNotifierFactory;
            _mediaService = mediaService;

            configuration.PathFormatChanged += UpdateTree;
            Items = new IMediaItem[0];
        }

        public string SourceDirectory {
            get => _sourceDirectory;
            set {
                if (_sourceDirectory == value) {
                    return;
                }

                _sourceDirectory = value;
                Rebuild();
                RaisePropertyChanged();
            }
        }

        public IProgressNotifier ProgressNotifier {
            get => _progressNotifier;
            private set {
                if (Equals(value, _progressNotifier)) {
                    return;
                }

                _progressNotifier.Dispose();
                _progressNotifier = value;
                RaisePropertyChanged();
            }
        }

        public IMediaItem[] Items {
            get => _items;
            private set {
                if (_items == value) {
                    return;
                }

                DisposeOldItems();
                _items = value;
                RaisePropertyChanged(nameof(Items));
                foreach (var item in _items) {
                    item.IsEnabledChanged += OnItemIsEnabledChanged;
                }

                TotalImages = _mediaFiles.Count(x => x.MediaType == MediaType.Image);
                TotalVideos = _mediaFiles.Count(x => x.MediaType == MediaType.Video);
                RaisePropertyChanged(nameof(Items));
            }
        }

        public int TotalImages {
            get => _totalImages;
            private set {
                _totalImages = value;
                RaisePropertyChanged(nameof(TotalImages));
            }
        }

        public int TotalVideos {
            get => _totalVideos;
            private set {
                _totalVideos = value;
                RaisePropertyChanged(nameof(TotalVideos));
            }
        }

        public event Action IsEnabledChanged;

        public IEnumerable<IMediaFile> GetFiles() {
            return _mediaFiles;
        }

        public void Rebuild() {
            Rebuild(SourceDirectory);
        }

        private void Rebuild(string sourceDirectory) {
            if (_fileService.IsValidDirectory(sourceDirectory)) {
                Task.Factory.StartNew(() => OnRebuild(sourceDirectory));
            }
        }

        private void OnRebuild(string sourceDirectory) {
            try {
                ProgressNotifier = _progressNotifierFactory();
                if (ExecuteRebuild(sourceDirectory, out var result)) {
                    _mediaFiles = result;
                    UpdateTree();
                    _configuration.SourceDirectory = sourceDirectory;
                }
                else {
                    ResetSourceDirectoryFromConfiguration();
                }
            }
            catch (Exception e) {
                _sourceDirectory = _configuration.SourceDirectory;
                RaisePropertyChanged(nameof(SourceDirectory));
                Logger.Error(e);
            }
            finally {
                ProgressNotifier.Dispose();
            }
        }

        private void ResetSourceDirectoryFromConfiguration() {
            _sourceDirectory = _configuration.SourceDirectory;
            RaisePropertyChanged(nameof(SourceDirectory));
        }

        private bool ExecuteRebuild(string sourceDirectory, out IMediaFile[] result) {
            result = new IMediaFile[0];
            ProgressNotifier.Update(0.0001m, "Searching media files");

            var filePaths = _mediaService.GetMediaFiles(sourceDirectory, ProgressNotifier).ToArray();

            if (ProgressNotifier.IsCancelled) {
                return false;
            }

            result = new IMediaFile[filePaths.Length];
            for (var index = 0; index < filePaths.Length; index++) {
                ProgressNotifier.Update((decimal) index / filePaths.Length, "Reading metadata");
                var mediaFile = _mediaService.CreateMediaFile(filePaths[index]);

                result[index] = mediaFile;
                if (ProgressNotifier.IsCancelled) {
                    return false;
                }
            }

            if (ProgressNotifier.IsCancelled) {
                return false;
            }

            return true;
        }

        private void OnItemIsEnabledChanged() {
            IsEnabledChanged?.Invoke();
        }

        public void UpdateTree() {
            var expandedPaths = GetDirectoriesRecursive(Items?.OfType<MediaDirectory>()).Where(x => x.IsExpanded)
                .Select(x => x.RelativePath).ToArray();
            var newItems = _mediaService.BuildTree(_mediaFiles);
            foreach (var item in GetDirectoriesRecursive(newItems)) {
                if (expandedPaths.Contains(item.RelativePath)) {
                    item.IsExpanded = true;
                }
            }

            Items = newItems;
        }

        private IEnumerable<MediaDirectory> GetDirectoriesRecursive(IEnumerable<IMediaItem> mediaDirectories) {
            if (mediaDirectories == null) {
                yield break;
            }

            foreach (var item in mediaDirectories.OfType<MediaDirectory>()) {
                yield return item;

                foreach (var itemChild in GetDirectoriesRecursive(item.Items)) {
                    yield return itemChild;
                }
            }
        }

        protected override void OnDispose() {
            base.OnDispose();
            DisposeOldItems();
            _configuration.PathFormatChanged -= UpdateTree;
        }

        private void DisposeOldItems() {
            if (Items == null) {
                return;
            }

            foreach (var item in Items) {
                item.IsEnabledChanged -= OnItemIsEnabledChanged;
                item.Dispose();
            }
        }
    }

}