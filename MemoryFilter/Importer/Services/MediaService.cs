using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MemoryFilter.Domain;
using MemoryFilter.Importer.Services.Notifications;
using MemoryFilter.Settings;
using NLog;

namespace MemoryFilter.Importer.Services {

    public class MediaService : IMediaService {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IConfiguration _configuration;

        private readonly IExtensionsProvider _extensionsProvider;
        private readonly IFileService _fileService;
        private readonly Func<MediaFileDto, IMediaFile> _mediaFileFactory;
        private IExifDataService _exifDataService;

        public MediaService(IExtensionsProvider extensionsProvider,
            IExifDataService exifDataService,
            IFileService fileService,
            Func<MediaFileDto, IMediaFile> mediaFileFactory,
            IConfiguration configuration) {
            _exifDataService = exifDataService;
            _fileService = fileService;
            _configuration = configuration;
            _extensionsProvider = extensionsProvider;
            _mediaFileFactory = mediaFileFactory;
        }

        public IEnumerable<string> GetMediaFiles(string directoryPath, INotifierInstance instance) {
            if (instance.IsCancelled) {
                yield break;
            }

            foreach (var subDirectoryPath in GetDirectoriesSafe(directoryPath)) {
                foreach (var file in ApplyAllFiles(subDirectoryPath)) {
                    {
                        if (instance.IsCancelled) {
                            yield break;
                        }

                        yield return file;
                    }
                }
            }

            if (instance.IsCancelled) {
                yield break;
            }

            foreach (var file in GetFilesSafe(directoryPath)) {
                if (_extensionsProvider.IsMediaFile(file)) {
                    yield return file;
                }
            }
        }

        public IMediaFile CreateMediaFile(string filePath) {
            var exifData = _exifDataService.Read(filePath);
            var size = _fileService.GetSizeInMb(filePath);
            var mediaType = _extensionsProvider.GetMediaType(filePath);
            var dto = new MediaFileDto(filePath, exifData.IsExif, exifData.CreationDateTime, size, mediaType,
                exifData.CameraModel);

            return _mediaFileFactory(dto);
        }

        public IMediaItem[] BuildTree(IMediaFile[] mediaFiles) {
            var lookup = mediaFiles.ToLookup(x => _configuration.GetNewRelativePath(x));
            var allNodes = mediaFiles.SelectMany(x => _configuration.GetPathNodes(x)).Distinct().ToArray();

            foreach (var node in allNodes) {
                var children = allNodes.Where(x => node.IsParentOf(x));
                node.SetChildren(children);

                if (lookup.Contains(node.FullPath)) {
                    node.SetFiles(lookup[node.FullPath]);
                }
            }

            var result = allNodes.Where(x => x.IsRoot)
                .Select(x => x.GetMediaItem())
                .ToArray();

            return result;
        }

        private IEnumerable<string> ApplyAllFiles(string folder) {
            foreach (var subDir in GetDirectoriesSafe(folder)) {
                foreach (var file in ApplyAllFiles(subDir)) {
                    yield return file;
                }
            }

            foreach (var file in GetFilesSafe(folder)) {
                if (_extensionsProvider.IsMediaFile(file)) {
                    yield return file;
                }
            }
        }

        private static string[] GetDirectoriesSafe(string folder) {
            try {
                return Directory.GetDirectories(folder);
            }
            catch (Exception e) {
                Logger.Error(e);

                return new string[0];
            }
        }

        private static string[] GetFilesSafe(string folder) {
            try {
                return Directory.GetFiles(folder);
            }
            catch (Exception e) {
                Logger.Error(e);

                return new string[0];
            }
        }
    }

}