using System;
using System.Linq;
using System.Threading;
using FluentAssertions;
using MemoryFilter.Domain;
using MemoryFilter.Importer.Services;
using MemoryFilter.Importer.Services.Notifications;
using MemoryFilter.Settings;
using MemoryFilter.Tests.Extensions;
using NSubstitute;
using Xunit;

namespace MemoryFilter.Tests.Domain {

    public class MediaCollectionTests {
        public MediaCollectionTests() {
            _mediaService = Substitute.For<IMediaService>();
            _fileService = Substitute.For<IFileService>();
            _progressNotifier = Substitute.For<IProgressNotifier>();
            _configuration = new Configuration(new ConfigurationDto {
                HistoryPathFormats = new []{@"yy\yy mm dd"}
            },_fileService);
            _mediaCollection = new MediaCollection(_configuration, () => _progressNotifier,
                _fileService, _mediaService);

            _mediaService.GetMediaFiles(Arg.Any<string>(), Arg.Any<INotifierInstance>())
                .Returns(new[] {
                    Path1,
                    Path2,
                    Path3,
                    Path4
                });

            _mediaService.CreateMediaFile(Path1).Returns(MediaFile1);
            _mediaService.CreateMediaFile(Path2).Returns(MediaFile2);
            _mediaService.CreateMediaFile(Path3).Returns(MediaFile3);
            _mediaService.CreateMediaFile(Path4).Returns(MediaFile4);
        }

        private const string Path1 = @"C:\camera\img1";
        private const string Path2 = @"C:\camera\img2";
        private const string Path3 = @"C:\camera\img3";
        private const string Path4 = @"C:\camera\img4";

        private readonly IMediaService _mediaService;
        private readonly IProgressNotifier _progressNotifier;
        private readonly IConfiguration _configuration;
        private readonly MediaCollection _mediaCollection;

        private static readonly string NewSourceDirectory =
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        private static readonly IMediaFile MediaFile1 = Substitute.For<IMediaFile>().Set(file => file.CreationDate, new DateTime(2018, 08, 21));
        private static readonly IMediaFile MediaFile2 = Substitute.For<IMediaFile>().Set(file => file.CreationDate, new DateTime(2018, 08, 22));
        private static readonly IMediaFile MediaFile3 = Substitute.For<IMediaFile>().Set(file => file.CreationDate, new DateTime(2018, 08, 22));
        private static readonly IMediaFile MediaFile4 = Substitute.For<IMediaFile>().Set(file => file.CreationDate, new DateTime(2018, 08, 22));
        private readonly IFileService _fileService;

        [Fact]
        public void NoActionsInvolved_ItemsAreEmpty() {
            _mediaCollection.Items.Should().BeEmpty();
        }

        [Fact]
        public void ProgressNotifierIsCancelledByDefault() {
            _mediaCollection.ProgressNotifier.Should().NotBeNull();
            _mediaCollection.ProgressNotifier.IsCancelled.Should().BeTrue();
        }

        [Fact]
        public void SetInvalidSourceDirectory_DoesNotScanForMediaFiles() {
            _fileService.IsValidDirectory(NewSourceDirectory).Returns(false);
            _mediaCollection.SourceDirectory = NewSourceDirectory;
            _mediaService.DidNotReceive().GetMediaFiles(Arg.Any<string>(), Arg.Any<INotifierInstance>());
        }

        [Fact]
        public void SetSourceDirectory_DoNotUpdateConfiguration() {
            _mediaCollection.SourceDirectory = NewSourceDirectory;
            _configuration.SourceDirectory.Should().BeEmpty();
        }

        [Fact]
        public void SetValidSourceDirectory_ScansForMediaFiles() {
            _fileService.IsValidDirectory(NewSourceDirectory).Returns(true);
            var expectedItems = new[] {Substitute.For<IMediaItem>(), Substitute.For<IMediaItem>()};
            _mediaService.BuildTree(Arg.Any<IMediaFile[]>())
                .Returns(expectedItems);
            
            
            _mediaCollection.SourceDirectory = NewSourceDirectory;
Thread.Sleep(1000);

            _mediaCollection.Items.Should().HaveCount(1);
            var directory2018 = _mediaCollection.Items.Single();
            directory2018.Should().BeOfType<MediaDirectory>();

            directory2018.Title.Should().Be("2018");

            _mediaCollection.GetFiles().Should().HaveCount(4);
            _mediaCollection.GetFiles().Should().BeEquivalentTo(MediaFile1, MediaFile2, MediaFile3, MediaFile4);
            _mediaCollection.Items.Should().BeSameAs(expectedItems);
        }

        [Fact]
        public void SetValidSourcePath_BuildTree() {
            _fileService.IsValidDirectory(NewSourceDirectory).Returns(true);
            _mediaCollection.SourceDirectory = NewSourceDirectory;
        }

        [Fact]
        public void SourceDirectoryIsEmptyByDefault() {
            _mediaCollection.SourceDirectory.Should().BeNull();
        }
    }

}