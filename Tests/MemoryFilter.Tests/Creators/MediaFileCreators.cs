using System;
using System.IO;
using System.Linq;
using MemoryFilter.Domain;
using MemoryFilter.Importer;
using MemoryFilter.Importer.Services;
using NSubstitute;

namespace MemoryFilter.Tests.Creators {

    public static class MediaFileCreators {
        public static MediaFile CreateRandom(DateTime? date = null, string path = null,
            MediaType mediaType = MediaType.Image) {
            return new MediaFile(new MediaFileDto(path ?? Path.GetTempFileName(), false, date.GetValueOrDefault(DateTime.Today),
                1, mediaType, "Nikon"), Substitute.For<IFileService>() ) {IsEnabled = true};
        }

        public static MediaFile[] CreateRandom(int count, DateTime? date = null) {
            return Enumerable.Range(0, count).Select(x => CreateRandom(date)).ToArray();
        }
    }

}