using System;
using MemoryFilter.Domain;
using MemoryFilter.Importer;
using NSubstitute;

namespace MemoryFilter.Tests.Extensions {

    public static class MediaDirectoriesCollectionExtensions {
        public static IMediaCollection SetDirectory(this IMediaCollection mock,
            params MediaFile[] mediaFiles) {
            return SetDirectory(mock, new DateTime(2000, 1, 1), mediaFiles);
        }

        public static IMediaCollection SetDirectory(this IMediaCollection mock,
            DateTime dateTime, params IMediaItem[] mediaItems) {
            mock.Items.Returns(mediaItems);

            return mock;
        }
    }

}