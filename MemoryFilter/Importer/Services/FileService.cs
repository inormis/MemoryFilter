using System;
using System.Diagnostics;
using System.IO;
using MemoryFilter.Domain;

namespace MemoryFilter.Importer.Services {

    public class FileService : IFileService {
        private const int BytesInMegabyte = 1024 * 1024;

        public bool IsValidDirectory(string directory) {
            return !string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory);
        }

        public string ReadAllText(string filePath) {
            return File.ReadAllText(filePath);
        }

        public bool FileExists(string filePath) {
            return File.Exists(filePath);
        }

        public void WriteText(string text, string filePath) {
            EnsureDirectoryExists(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, text);
        }

        public void Move(IMediaFile mediaFile, string newPath) {
            Move(mediaFile.FilePath, newPath);
        }

        public void EnsureDirectoryExists(string targetRootFolder) {
            if (!DirectoryExists(targetRootFolder)) {
                Directory.CreateDirectory(targetRootFolder);
            }
        }

        public bool DirectoryExists(string targetDirectory) {
            return Directory.Exists(targetDirectory);
        }

        public void ShowInExplorer(string filePath) {
            Process.Start("explorer.exe", $"/select, \"{filePath}\"");
        }

        public void Open(string filePath) {
            Process.Start(filePath);
        }

        public decimal GetSizeInMb(string file) {
            var sizeInBytes = new FileInfo(file).Length;
            return Math.Round((decimal) sizeInBytes / BytesInMegabyte, 2);
        }

        public void Move(string sourceFilePath, string targetPath) {
            if (!FileExists(targetPath)) {
                File.Move(sourceFilePath, targetPath);
                return;
            }

            if (new FileInfo(sourceFilePath).Length == new FileInfo(targetPath).Length) {
                File.Delete(sourceFilePath);
            }
        }

        private class ExifData {
            public ExifData(bool isExif, DateTime creationDateTime, string cameraModel) {
                IsExif = isExif;
                CreationDateTime = creationDateTime;
                CameraModel = cameraModel;
            }

            public DateTime CreationDateTime { get; }
            public string CameraModel { get; }
            public bool IsExif { get; }
        }
    }

}