using System;
using System.Collections.Generic;
using System.IO;

namespace MemoryFilter.Importer.Services {

    public class ExtensionsProvider : IExtensionsProvider {
        private static readonly Dictionary<string, MediaType> DefaultMediaTypesMap =
            new Dictionary<string, MediaType>(StringComparer.InvariantCultureIgnoreCase) {
                {".jpg", MediaType.Image},
                {".jpeg", MediaType.Image},
                {".bmp", MediaType.Image},
                {".png", MediaType.Image},
                {".psd", MediaType.Image},
                {".gif", MediaType.Image},
                {".3g2", MediaType.Video},
                {".3gp", MediaType.Video},
                {".asf", MediaType.Video},
                {".avi", MediaType.Video},
                {".flv", MediaType.Video},
                {".mov", MediaType.Video},
                {".mp4", MediaType.Video},
                {".mpg", MediaType.Video},
                {".swf", MediaType.Video},
                {".mts", MediaType.Video},
                {".wmv", MediaType.Video}
            };

        public bool IsKnownExtension(string extension) {
            if (string.IsNullOrWhiteSpace(extension)) {
                return false;
            }

            return DefaultMediaTypesMap.ContainsKey(extension);
        }

        public bool IsMediaFile(string file) {
            return DefaultMediaTypesMap.ContainsKey(Path.GetExtension(file));
        }

        public MediaType GetMediaType(string file) {
            MediaType type;
            if (DefaultMediaTypesMap.TryGetValue(Path.GetExtension(file), out type)) {
                return type;
            }

            throw new Exception("File " + file + " is not supported");
        }
    }

}