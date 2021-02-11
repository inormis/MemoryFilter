using System;
using System.Collections.Generic;
using System.IO;
using ExifLibrary;

namespace MemoryFilter.Importer.Services {

    public interface IExifDataService {
        ExifData Read(string filePath);
    }

    class ExifDataService : IExifDataService {
        private readonly Dictionary<string, ExifData> _cacheDates =
            new Dictionary<string, ExifData>(StringComparer.InvariantCultureIgnoreCase);

        public ExifData Read(string filePath) {
            if (_cacheDates.TryGetValue(filePath, out var result)) {
                return result;
            }

            var exifData = GetExifData(filePath);
            var resultDateTime = GetCreationDate(filePath, exifData);

            var cameraModel = exifData?.CameraModel ?? "No Model";
            result = new ExifData(resultDateTime, cameraModel, exifData != null);
            _cacheDates[filePath] = result;

            return result;
        }

        private DateTime GetCreationDate(string filePath, ExifData data) {
            var resultDateTime = data?.CreationDateTime ?? DateTime.MinValue;

            if (resultDateTime != DateTime.MinValue)
                return resultDateTime;

            if (DateParseFromFileNameService.TryGetFrom(filePath, out var fileNameDate))
                return fileNameDate;

            var creation = File.GetCreationTime(filePath);
            var modified = File.GetLastWriteTime(filePath);

            return creation < modified ? creation : modified;
        }

        public ExifData GetExifData(string filePath) {
            try {
                var file = ImageFile.FromFile(filePath);
                var dateTime = file.Properties.Get<ExifDateTime>(ExifTag.DateTime);
                var camera = file.Properties.Get<ExifAscii>(ExifTag.Model).Value;

                return new ExifData(dateTime.Value, camera, true);
            }
            catch (Exception e) {
                return null;
            }
        }
    }

    public class ExifData {
        public ExifData(DateTime creationDateTime, string cameraModel, bool isExif) {
            CreationDateTime = creationDateTime;
            CameraModel = cameraModel;
            IsExif = isExif;
        }

        public DateTime CreationDateTime { get; }

        public string CameraModel { get; }

        public bool IsExif { get; }
    }

}