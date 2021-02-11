using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using MemoryFilter.Annotations;
using MemoryFilter.Data;
using MemoryFilter.Domain;
using MemoryFilter.Importer;
using MemoryFilter.Importer.Services;

namespace MemoryFilter.Settings {

    public class Configuration : INotifyPropertyChanged, IConfiguration {
        private readonly IFileService _fileService;
        private readonly DistinctMaxStack<string> _historyFileNamePrefixes;
        private readonly DistinctMaxStack<string> _historyPathFormats;
        private readonly DistinctMaxStack<string> _historySourcePaths;
        private readonly DistinctMaxStack<string> _historyTargetPaths;
        private OverrideStrategy _overrideStrategy;
        private string _pathFormat;
        private string _sourceDirectory;
        private string _targetDirectory;

        public Configuration(IFileService fileService) : this(new ConfigurationDto(), fileService) {
        }

        public Configuration(ConfigurationDto configurationDto, IFileService fileService) {
            _fileService = fileService;
            _historyTargetPaths = new DistinctMaxStack<string>(7, configurationDto.HistoryTargetPaths);
            _historySourcePaths = new DistinctMaxStack<string>(7, configurationDto.HistorySourcePaths);
            _historyPathFormats = new DistinctMaxStack<string>(7, configurationDto.HistoryPathFormats);
            _historyFileNamePrefixes = new DistinctMaxStack<string>(7, configurationDto.HistoryFileNamePrevixes);
            OverrideStrategy = configurationDto.OverrideStrategy;
            PathFormat = HistoryPathFormats.FirstOrDefault();
            FileNamePrefix = HistoryFileNamePrefixes.FirstOrDefault();
        }

        public event Action Changed;

        public PathNode[] GetPathNodes(IMediaFile mediaFile) {
            var split = PathFormat.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);

            var result = new List<PathNode>(split.Length);
            var paths = new List<string>();
            foreach (var s in split) {
                paths.Add(GetFormatedThePath(mediaFile, s));
                result.Add(new PathNode(paths.ToArray()));
            }

            return result.ToArray();
        }

        public string GetNewRelativePath(IMediaFile mediaFile) {
            return GetFormatedThePath(mediaFile, PathFormat);
        }

        public event Action PathFormatChanged;

        public IEnumerable<string> HistoryTargetPaths => _historyTargetPaths;

        public IEnumerable<string> HistorySourcePaths => _historySourcePaths;

        public IEnumerable<string> HistoryPathFormats => _historyPathFormats;

        public IEnumerable<string> HistoryFileNamePrefixes => _historyFileNamePrefixes;

        public string DateFolderFormat { get; set; } = "yyyy MM dd";

        public bool Images { get; set; } = true;

        public bool Videos { get; set; } = true;

        public string FileNamePrefix { get; set; }

        public string SourceDirectory {
            get => _sourceDirectory;
            set {
                if (string.Compare(value, SourceDirectory, StringComparison.InvariantCultureIgnoreCase) == 0) {
                    return;
                }

                _sourceDirectory = value;
                OnPropertyChanged();
            }
        }

        public string TargetDirectory {
            get => _targetDirectory;
            set {
                _targetDirectory = value;
                OnPropertyChanged(nameof(SamplePath));
            }
        }

        public string PathFormat {
            get => _pathFormat;
            set {
                if (string.Compare(value, PathFormat, StringComparison.InvariantCultureIgnoreCase) == 0) {
                    return;
                }

                _pathFormat = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SamplePath));
                PathFormatChanged?.Invoke();
            }
        }

        public ConfigurationDto Export() {
            return new ConfigurationDto {
                HistoryTargetPaths = HistoryTargetPaths.ToArray(),
                HistorySourcePaths = HistorySourcePaths.ToArray(),
                HistoryPathFormats = HistoryPathFormats.ToArray(),
                HistoryFileNamePrevixes = HistoryPathFormats.ToArray(),
                OverrideStrategy = OverrideStrategy
            };
        }

        public OverrideStrategy OverrideStrategy {
            get => _overrideStrategy;
            set {
                _overrideStrategy = value;
                OnPropertyChanged(nameof(OverrideStrategy));
            }
        }

        public string SamplePath => null;
        //CreatePath(new MediaFile(@"D:\img1344.jpg", true, DateTime.Now, 132323, MediaType.Image, "Canon"));


        public void Apply() {
            if (_fileService.IsValidDirectory(SourceDirectory)) {
                return;
            }

            _historyTargetPaths.Add(TargetDirectory);
            _historySourcePaths.Add(SourceDirectory);
            if (!string.IsNullOrWhiteSpace(PathFormat)) {
                _historyPathFormats.Add(PathFormat);
            }

            if (!string.IsNullOrWhiteSpace(FileNamePrefix)) {
                _historyFileNamePrefixes.Add(FileNamePrefix);
            }

            Changed?.Invoke();
        }

        public bool IsSupported(IMediaFile mediaFile) {
            switch (mediaFile.MediaType) {
                case MediaType.Image:
                    return Images;
                case MediaType.Video:
                    return Videos;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mediaFile), mediaFile, null);
            }
        }

        public string CreatePath(IMediaFile mediaFile, string targetDirectory) {
            var path = targetDirectory;
            var split = PathFormat.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in split) {
                var reformat = GetFormatedThePath(mediaFile, s);
                path = Path.Combine(path, reformat);
            }

            path = Path.Combine(path, mediaFile.FileName);
            return path;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string GetFormatedThePath(IMediaFile mediaFile, string pattern) {
            return pattern.Replace("yy", mediaFile.CreationDate.Year.ToString())
                .Replace("mm", mediaFile.CreationDate.Month.ToString("00"))
                .Replace("dd", mediaFile.CreationDate.Day.ToString("00"))
                .Replace("cam", mediaFile.CameraModel);
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            Apply();
        }
    }

}