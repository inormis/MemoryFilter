using System;

namespace MemoryFilter.Domain {

    public abstract class MediaFileBase : NotificationObject, IMediaItem {
        protected bool? isEnabled = true;

        protected MediaFileBase(string title, IMediaItem[] items) {
            Items = items;
            Title = title;
        }

        public IMediaItem[] Items { get; }

        public abstract string Description { get; }

        public string Title { get; }

        public abstract int VideosCount { get; }

        public abstract void SetIsEnabledFromParent(bool value);

        public abstract decimal SizeInMb { get; }

        public abstract int ImagesCount { get; }

        public event Action IsEnabledChanged;

        public bool? IsEnabled {
            get => isEnabled;
            set {
                if (IsEnabled == value) {
                    return;
                }

                if (value == null) {
                    isEnabled = !isEnabled;
                }
                else {
                    isEnabled = value;
                }

                if (IsEnabled.HasValue) {
                    SetIsEnabledFromParent(IsEnabled.Value);
                }

                RaiseIsEnabledChanged();
            }
        }

        protected void RaiseIsEnabledChanged() {
            RaisePropertyChanged(nameof(IsEnabled));
            IsEnabledChanged?.Invoke();
        }
    }

}