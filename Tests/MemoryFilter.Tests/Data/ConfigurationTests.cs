using System;
using System.Collections.Generic;
using FluentAssertions;
using MemoryFilter.Domain;
using MemoryFilter.Importer;
using MemoryFilter.Importer.Services;
using MemoryFilter.Settings;
using MemoryFilter.Tests.Extensions;
using NSubstitute;
using Xunit;

namespace MemoryFilter.Tests {

    public class ConfigurationTests {
        private IFileService _fileService;

        public ConfigurationTests() {
            _fileService = Substitute.For<IFileService>();
        }
        private string TestPath(string pathFormat, string fileFormat, int year, int month, int day, string fileName,
            string cameraModel) {
            var сonfiguration = new Configuration(_fileService) {
                TargetDirectory = @"C:\Doms\files", PathFormat = pathFormat, FileNamePrefix = fileFormat
            };
            var mediaFile = Substitute.For<IMediaFile>()
                .Set(x => x.CreationDate, new DateTime(year, month, day))
                .Set(x => x.CameraModel, cameraModel)
                .Set(x => x.FileName, fileName);
            var actualPath = сonfiguration.CreatePath(mediaFile, сonfiguration.TargetDirectory);
            return actualPath;
        }

        [Fact]
        public void ApplyChanges_PushesCurrentSettingsToHistory() {
            var configuration = new Configuration(new ConfigurationDto {
                HistoryPathFormats = new[] {"f1", "f2", "f3"},
                HistorySourcePaths = new[] {"s1", "s2", "s3"},
                HistoryTargetPaths = new[] {"t1", "t2", "t3"},
                HistoryFileNamePrevixes = new[] {"n1", "n2", "n3"}
            },_fileService) {PathFormat = "f0", SourceDirectory = "s2", TargetDirectory = "t4", FileNamePrefix = ""};

            configuration.Apply();

            configuration.HistoryPathFormats.Should().BeEquivalentTo("f0", "f1", "f2", "f3");
            configuration.HistorySourcePaths.Should().BeEquivalentTo("s2", "s1", "s3");
            configuration.HistoryTargetPaths.Should().BeEquivalentTo("t4", "t1", "t2", "t3");
            configuration.HistoryFileNamePrefixes.Should().BeEquivalentTo("n1", "n2", "n3");
        }

        [Fact]
        public void PathAndNameFormatsProvided_PathGenerated() {
            var actualPath = TestPath(@"cam\yy mm\dd", "yy mm dd - ", 2015, 02, 25, "img.jpg", "Cannon");
            var expectedPath = @"C:\Doms\files\Cannon\2015 02\25\2015 02 25 - img.jpg";
            actualPath.Should().Be(expectedPath);
        }

        [Fact]
        public void PathFormatChanged_RaisesSamplePathChanged() {
            var configuration = new Configuration(_fileService) {TargetDirectory = @"d:\targeet\photos"};
            var changedPropertyNames = new List<string>();
            configuration.PropertyChanged += (sender, args) => changedPropertyNames.Add(args.PropertyName);
            configuration.PathFormat = "yy\\mm";
            changedPropertyNames.Should().Contain(nameof(Configuration.SamplePath));
        }

        [Fact]
        public void PathFormatProvidedOnly_PathGenerated() {
            var actualPath = TestPath(@"yy\yy mm\yy mm dd", "", 2015, 11, 9, "img.jpg", "Cannon");
            var expectedPath = @"C:\Doms\files\2015\2015 11\2015 11 09\img.jpg";
            actualPath.Should().Be(expectedPath);
        }

        [Fact]
        public void TargetDirectoryChanged_RaiesSamplePathChanged() {
            var configuration = new Configuration(_fileService) {PathFormat = "yy\\mm"};
            string changedPropertyName = null;
            configuration.PropertyChanged += (sender, args) => changedPropertyName = args.PropertyName;
            configuration.TargetDirectory = @"d:\targeet\photos";
            changedPropertyName.Should().Be(nameof(Configuration.SamplePath));
        }

        [Fact]
        public void GetPathNodes_ReturnNodesForEveryDirectory() {
            var configuration = new Configuration(_fileService) {
                PathFormat = @"cam yy\yy\yy mm dd wow"
            };

            var file = Substitute.For<IMediaFile>()
                .Set(x => x.CreationDate, new DateTime(2013, 1, 31))
                .Set(x => x.CameraModel, "Cannon");

            var pathNodes = configuration.GetPathNodes(file);

            pathNodes.Should().HaveCount(3);
            
            pathNodes[0].Should().BeEquivalentTo(new PathNode("Cannon 2013"));
            pathNodes[1].Should().BeEquivalentTo(new PathNode("Cannon 2013", "2013"));
            pathNodes[2].Should().BeEquivalentTo(new PathNode("Cannon 2013", "2013", "2013 01 31 wow"));
        }
    }
}