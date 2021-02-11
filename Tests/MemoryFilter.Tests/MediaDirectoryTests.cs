using System;
using System.Linq;
using FluentAssertions;
using MemoryFilter.Domain;
using MemoryFilter.Importer;
using MemoryFilter.Importer.Services;
using MemoryFilter.Tests.Creators;
using MemoryFilter.Tests.Extensions;
using NSubstitute;
using Xunit;

namespace MemoryFilter.Tests {

    public class MediaDirectoryTests {

        private MediaDirectory CreateMediaDirectory() {
            return new MediaDirectory("", "", MediaFileCreators.CreateRandom(3));
        }

        private static MediaFile CreateMediaFile(string filePath, bool hasExifData, DateTime creationDate,
            decimal sizeInMb,
            MediaType mediaType, string cameraModel) {
            return new MediaFile(
                new MediaFileDto(filePath, hasExifData, creationDate, sizeInMb, mediaType, cameraModel),
                Substitute.For<IFileService>());
        }

        [Fact]
        public void GetFiles_ReturnMatchingPredicate() {
        }

        [Fact]
        public void IsEnabledByDefault() {
            var mediaDirectory = CreateMediaDirectory();
            mediaDirectory.IsEnabled.Should().BeTrue();
            mediaDirectory.Items.All(x => x.IsEnabled == true).Should().BeTrue();
        }

        [Fact]
        public void SetAllChildrenEnabledToFalse() {
            var mediaDirectory = CreateMediaDirectory();
            mediaDirectory.IsEnabled = false;

            mediaDirectory.Items.ShouldForAllBe(file => file.IsEnabled, false);
        }

        [Fact]
        public void SetEnabledToNullIfOneOfChildrenIsDisabled() {
            var mediaDirectory = CreateMediaDirectory();
            mediaDirectory.Items.Last().IsEnabled = false;

            mediaDirectory.IsEnabled.Should().BeNull();
        }

        [Fact]
        public void SetIsEnabledUpdatesChildren() {
            var mediaDirectory = new MediaDirectory("D1", "",
                new IMediaItem[] {
                    new MediaDirectory("D1_1", "",
                        new IMediaItem[] {
                            new MediaDirectory("D1_1_1", "",
                                new IMediaItem[] {
                                    CreateMediaFile(@"C:\d1\d1_2\d1_2_3_file1", true, DateTime.Today, 1,
                                        MediaType.Image,
                                        "Nikon"),
                                    CreateMediaFile(@"C:\d1\d1_2\d1_2_3_file2", true, DateTime.Today, 1,
                                        MediaType.Image,
                                        "Nikon")
                                }),
                            CreateMediaFile(@"C:\d1\d1_2_file1", true, DateTime.Today, 1, MediaType.Image, "Nikon"),
                            CreateMediaFile(@"C:\d1\d1_2_file2", true, DateTime.Today, 1, MediaType.Image, "Nikon")
                        }),
                    new MediaDirectory("D1_2", "",
                        new IMediaItem[] {
                            new MediaDirectory("D1_2_1", "",
                                new IMediaItem[] {
                                    CreateMediaFile(@"C:\d1\d1_2\d1_2_3_file1", true, DateTime.Today, 1,
                                        MediaType.Image,
                                        "Nikon"),
                                    CreateMediaFile(@"C:\d1\d1_2\d1_2_3_file2", true, DateTime.Today, 1,
                                        MediaType.Image,
                                        "Nikon")
                                }),
                            CreateMediaFile(@"C:\d1\d1_2_file1", true, DateTime.Today, 1, MediaType.Image, "Nikon"),
                            CreateMediaFile(@"C:\d1\d1_2_file2", true, DateTime.Today, 1, MediaType.Image, "Nikon")
                        })
                }) {IsEnabled = false};


            mediaDirectory.ChildFiles.All(x => x.IsEnabled == false).Should().BeTrue();
        }
    }

}