using System.Collections.Generic;
using System.Linq;
using MemoryFilter.Importer;

namespace MemoryFilter.Domain {

    public class MediaDirectory : MediaFileBase {
        public MediaDirectory(string title, string relativePath, IMediaItem[] items) : base(title, items) {
            RelativePath = relativePath;
            foreach (var file in Items) {
                file.IsEnabledChanged += OnChildItemIsEnabledChanged;
            }

            ChildFiles = GetFiles(items).ToArray();
            SizeInMb = items.Sum(x => x.SizeInMb);

            ImagesCount = GetEnabledMediaFiles(MediaType.Image);
            VideosCount = GetEnabledMediaFiles(MediaType.Video);

            OnChildItemIsEnabledChanged();
        }

        public bool IsExpanded { get; set; }


        public string RelativePath { get; }

        public override string Description {
            get {
                var enabledMediaFiles = GetEnabledMediaFiles(MediaType.Image);
                return $@"Size: {GetEnabledSize()}/{Items.Sum(x => x.SizeInMb)}mb,
Images: {enabledMediaFiles}/{ImagesCount}
Videos: {GetEnabledMediaFiles(MediaType.Video)}/{VideosCount}";
            }
        }

        public IMediaFile[] ChildFiles { get; }

        public override decimal SizeInMb { get; }

        public override int ImagesCount { get; }

        public override int VideosCount { get; }

        private int GetEnabledMediaFiles(MediaType mediaType) {
            return ChildFiles.Count(x => x.IsEnabled == true && x.MediaType == mediaType);
        }

        private decimal GetEnabledSize() {
            return ChildFiles.Where(x => x.IsEnabled == true).Sum(x => x.SizeInMb);
        }

        private static IEnumerable<IMediaFile> GetFiles(IMediaItem[] items) {
            foreach (var mediaFile in items.OfType<IMediaFile>()) {
                yield return mediaFile;
            }

            foreach (var directory in items.OfType<MediaDirectory>()) {
                foreach (var file in directory.ChildFiles) {
                    yield return file;
                }
            }
        }

        public override void SetIsEnabledFromParent(bool value) {
            isEnabled = value;
            foreach (var item in Items) {
                item.SetIsEnabledFromParent(value);
            }

            RaisePropertyChanged(nameof(IsEnabled));
        }

        protected override void OnDispose() {
            base.OnDispose();
            foreach (var item in Items) {
                item.IsEnabledChanged -= OnChildItemIsEnabledChanged;
                item.Dispose();
            }
        }

        private string GetSuffix() {
            return string.IsNullOrWhiteSpace(Title) ? Items.Length.ToString() : Title;
        }

        private void OnChildItemIsEnabledChanged() {
            var enabledCount = Items.Count(x => x.IsEnabled == true);

            if (enabledCount == Items.Length) {
                isEnabled = true;
            }
            else if (enabledCount == 0) {
                isEnabled = false;
            }
            else {
                isEnabled = null;
            }

            RaiseIsEnabledChanged();
        }
    }

}