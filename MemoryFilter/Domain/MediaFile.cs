using System;
using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using MemoryFilter.Importer;
using MemoryFilter.Importer.Services;

namespace MemoryFilter.Domain {

    public class MediaFileDto {
        public MediaFileDto(string filePath, bool hasExifData, DateTime creationDate, decimal sizeInMb,
            MediaType mediaType, string cameraModel) {
            FilePath = filePath;
            HasExifData = hasExifData;
            CreationDate = creationDate;
            SizeInMb = sizeInMb;
            MediaType = mediaType;
            CameraModel = cameraModel;
        }

        public string FilePath { get; }
        public bool HasExifData { get; }
        public DateTime CreationDate { get; }
        public decimal SizeInMb { get; }
        public MediaType MediaType { get; }
        public string CameraModel { get; }
    }

    public class MediaFile : MediaFileBase, IMediaFile {
        private readonly IFileService _fileService;
        private bool _completed;

        public MediaFile(MediaFileDto dto, IFileService service) : base(Path.GetFileName(dto.FilePath),
            new IMediaItem[0]) {
            _fileService = service;
            CreationDate = dto.CreationDate.Date;
            FilePath = dto.FilePath;
            HasExifData = dto.HasExifData;
            SizeInMb = dto.SizeInMb;
            MediaType = dto.MediaType;
            FileName = Path.GetFileName(FilePath);
            CameraModel = string.IsNullOrWhiteSpace(dto.CameraModel) ? "UnknownCamera" : dto.CameraModel;
            ImagesCount = MediaType == MediaType.Image ? 1 : 0;
            VideosCount = MediaType == MediaType.Video ? 1 : 0;

            ShowInExplorerCommand = new RelayCommand(OnShow);
            OpenFileCommand = new RelayCommand(OpenFile);
        }


        public bool HasExifData { get; }

        public ICommand ShowInExplorerCommand { get; }

        public ICommand OpenFileCommand { get; }

        public override string Description =>
            $@"Name: {FileName}
Path: {FilePath}
Created: {CreationDate} {(HasExifData ? "(EXIF)" : "")}
Size: {SizeInMb}mb
Camera Model: {(string.IsNullOrWhiteSpace(CameraModel) ? "-" : CameraModel)}";

        public override int ImagesCount { get; }

        public override int VideosCount { get; }

        public override void SetIsEnabledFromParent(bool value) {
            isEnabled = value;
            RaisePropertyChanged(nameof(IsEnabled));
        }

        public string FileName { get; }

        public DateTime CreationDate { get; }

        public string FilePath { get; }

        public override decimal SizeInMb { get; }

        public MediaType MediaType { get; }


        public bool Completed {
            get => _completed;
            set {
                if (Completed == value) {
                    return;
                }


                _completed = value;
                RaisePropertyChanged(nameof(Completed));
            }
        }

        public string CameraModel { get; }

        private void OpenFile() {
            _fileService.Open(FilePath);
        }

        private void OnShow() {
            _fileService.ShowInExplorer(((IMediaFile) this).FilePath);
        }
    }

}