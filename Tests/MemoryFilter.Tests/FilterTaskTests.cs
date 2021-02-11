//using System;
//using System.Threading.Tasks;
//using MemoryFilter.Domain;
//using MemoryFilter.Importer;
//using MemoryFilter.Importer.Services;
//using MemoryFilter.Tests.Creators;
//using MemoryFilter.Tests.Extensions;
//using NSubstitute;
//using Xunit;
//
//namespace MemoryFilter.Tests {
//
//    public class FileManagerTests {
//        public FileManagerTests() {
//            mediaFileServiceMock = Substitute.For<IMediaFileService>();
//            mediaCollectionMock = Substitute.For<IMediaDirectoriesCollection>()
//                .SetDirectory(October20, file1Image, file2Video, file3Image, file4Video);
//        }
//
//        private readonly MediaFile file1Image = MediaFileCreators.CreateRandom(October20);
//        private readonly MediaFile file2Video = MediaFileCreators.CreateRandom(October20, mediaType: MediaType.Video);
//        private readonly MediaFile file3Image = MediaFileCreators.CreateRandom(October20);
//        private readonly MediaFile file4Video = MediaFileCreators.CreateRandom(October20, mediaType: MediaType.Video);
//
//        private readonly IMediaFileService mediaFileServiceMock;
//        private readonly string targetfolderPath = "D:\\TargetFolder";
//        private readonly IMediaDirectoriesCollection mediaCollectionMock;
//        private static readonly DateTime October20 = new DateTime(2010, 10, 20);
//
//        private IFilterTaskSettings ExecuteAndWait(bool images = true, bool videos = true) {
//            var filterTaskSettings = new FilterTaskSettings(targetfolderPath, "yyyy mm dd", images, videos);
//            var filterTask =
//                new FilterTask(mediaCollectionMock, mediaFileServiceMock, filterTaskSettings);
//            if (filterTask.IsCompleted) {
//                return filterTaskSettings;
//            }
//
//            var source = new TaskCompletionSource<bool>();
//            filterTask.ProgressChanged += () => {
//                if (filterTask.IsCompleted) {
//                    source.SetResult(true);
//                }
//            };
//
//            source.Task.Wait(2000);
//            return filterTaskSettings;
//        }
//
//        [Fact]
//        public void CopyAllFiles() {
//            var filterTaskSettings = ExecuteAndWait();
//
//            var newTargetDirectory = filterTaskSettings.CreatePathForDate(October20);
//            mediaFileServiceMock.Received(1).Move(file1Image, newTargetDirectory);
//            mediaFileServiceMock.Received(1).Move(file2Video, newTargetDirectory);
//            mediaFileServiceMock.Received(1).Move(file3Image, newTargetDirectory);
//            mediaFileServiceMock.Received(1).Move(file4Video, newTargetDirectory);
//        }
//
//        [Fact]
//        public void DoNotMoveFileIfDisabled() {
//            var filterTaskSettings = ExecuteAndWait();
//            file1Image.IsSelected = false;
//            file4Video.IsSelected = false;
//
//            var newTargetDirectory = filterTaskSettings.CreatePathForDate(October20);
//            mediaFileServiceMock.DidNotReceive().Move(file1Image, newTargetDirectory);
//            mediaFileServiceMock.DidNotReceive().Move(file4Video, newTargetDirectory);
//        }
//
//        [Fact]
//        public void IgnoreImages() {
//            ExecuteAndWait(false);
//
//            mediaFileServiceMock.DidNotReceive().Move(file1Image, Arg.Any<string>());
//            mediaFileServiceMock.Received(1).Move(file2Video, Arg.Any<string>());
//            mediaFileServiceMock.DidNotReceive().Move(file3Image, Arg.Any<string>());
//            mediaFileServiceMock.Received(1).Move(file4Video, Arg.Any<string>());
//        }
//
//        [Fact]
//        public void IgnoreVideo() {
//            ExecuteAndWait(videos: false);
//
//            mediaFileServiceMock.Received(1).Move(file1Image, Arg.Any<string>());
//            mediaFileServiceMock.DidNotReceive().Move(file2Video, Arg.Any<string>());
//            mediaFileServiceMock.Received(1).Move(file3Image, Arg.Any<string>());
//            mediaFileServiceMock.DidNotReceive().Move(file4Video, Arg.Any<string>());
//        }
//    }
//}

