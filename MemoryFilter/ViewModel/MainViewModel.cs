using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MemoryFilter.Domain;
using MemoryFilter.Importer;
using MemoryFilter.Importer.Services;
using MemoryFilter.Importer.Services.Notifications;
using MemoryFilter.Settings;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace MemoryFilter.ViewModel {

    public class MainViewModel : ViewModelBase {
        private readonly IMediaService _mediaService;
        private readonly IMediaFileManager mediaFileManager;
        public IConfiguration Configuration { get; }
        private TimeSpan currentPosition;
        private Func<IProgressNotifier> _progresssNotifierFactory;
        private IExecutionResult executionResult;

        private IProgressNotifier _filterProgress = ProgressNotifier.CancelledInstance;

        private IMediaFile selectedFile;
        private TimeSpan totalTimeSpan;
        private IFileService _fileService;
        public MainViewModel(IConfiguration configuration,
            IMediaService mediaService,
            IMediaCollection collection,
            IFileService fileService,
            Func<IProgressNotifier> progresssNotifierFactory,
            IMediaFileManager mediaFileManager) {
            this.mediaFileManager = mediaFileManager;
            this._mediaService = mediaService;
            Collection = collection;
            Configuration = configuration;
            _fileService = fileService;
            _progresssNotifierFactory = progresssNotifierFactory;
            SelectAllCommand = new RelayCommand(ExecuteSelectAll);
            SelectNoneCommand = new RelayCommand(ExecuteSelectNone);
            ExecuteCommand = new RelayCommand(Execute, CanExecute);
            CancelCommand = new RelayCommand(ExecuteCancel, CanExecuteCancel);
            BrowseSourceCommand = new RelayCommand(ExecuteBrowseSourcePath);
            BrowseTargetCommand = new RelayCommand(ExecuteBrowseTargetPath);
            OpenDirectoryCommand = new RelayCommand<IMediaFile>(OnOpenFileDirectory);
        }

        private void OnOpenFileDirectory(IMediaFile mediaFile) {
            _fileService.ShowInExplorer(mediaFile.FilePath);

        }
        public RelayCommand CancelCommand { get; }

        public IExecutionResult ExecutionResult {
            get { return executionResult; }
            private set {
                executionResult = value;
                RaisePropertyChanged(nameof(ExecutionResult));
                CancelCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand SelectAllCommand { get; }

        public RelayCommand SelectNoneCommand { get; }

        private string _targetDirectory;
        public string TargetDirectory {
            get { return _targetDirectory; }
            set {
                if (_targetDirectory == value) {
                    return;
                }
                _targetDirectory = value;
                ExecuteCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(TargetDirectory));
            }
        }

        public RelayCommand ExecuteCommand { get; }

        public RelayCommand BrowseSourceCommand { get; }
        
        public RelayCommand BrowseTargetCommand { get; }
        
        public IMediaCollection Collection { get; }

        public object Current {
            get { return selectedFile; }
            set {
                var newValue = value as IMediaFile;

                if (newValue == selectedFile) {
                    return;
                }
                SelectedFile = newValue;
                RaisePropertyChanged();
            }
        }

        public IMediaFile SelectedFile {
            get { return selectedFile; }
            private set {
                selectedFile = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand PlayCommand { get; }
        public RelayCommand PauseCommand { get; }
        public RelayCommand StopCommand { get; }

        public TimeSpan TotalTimeSpan {
            get { return totalTimeSpan; }
            set {
                totalTimeSpan = value;
                RaisePropertyChanged(nameof(TotalTime));
            }
        }

        public TimeSpan CurrentPosition {
            get { return currentPosition; }
            set {
                currentPosition = value;
                RaisePropertyChanged(nameof(CurrentTime));
            }
        }

        public string CurrentTime => string.Format(CultureInfo.CurrentCulture, "{0:00}:{1:00}",
            CurrentPosition.Minutes,
            CurrentPosition.Seconds);

        public string TotalTime => string.Format(CultureInfo.CurrentCulture, "{0:00}:{1:00}",
            totalTimeSpan.Minutes,
            totalTimeSpan.Seconds);

        public ICommand OpenDirectoryCommand { get; }

        private bool CanExecuteCancel() {
            return ExecutionResult != null;
        }

        private async void ExecuteBrowseSourcePath() {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog()) {
                if (Directory.Exists(Configuration.SourceDirectory))
                    dialog.SelectedPath = Configuration.SourceDirectory;

                var result = dialog.ShowDialog();
                if (result == DialogResult.OK) {
                    Configuration.SourceDirectory = dialog.SelectedPath;
                }
            }
        }

        private void ExecuteBrowseTargetPath() {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog()) {
                if (Directory.Exists(TargetDirectory))
                    dialog.SelectedPath = TargetDirectory;

                var result = dialog.ShowDialog();
                if (result == DialogResult.OK) {
                    TargetDirectory = dialog.SelectedPath;
                }
            }
        }

        private void ExecuteCancel() {
            ExecutionResult?.Cancel();
        }

        private bool CanExecute() {
            return !string.IsNullOrWhiteSpace(TargetDirectory);
        }

        public IProgressNotifier FilterProgress {
            get => _filterProgress;
            set {
                _filterProgress = value;
                base.RaisePropertyChanged(nameof(FilterProgress));
            }
        }
        private async void Execute() {
            using (var filterProgress = _progresssNotifierFactory()) {
                FilterProgress = filterProgress;
                await mediaFileManager.Execute(filterProgress, TargetDirectory);
                Configuration.TargetDirectory = TargetDirectory;
                Configuration.Apply();
                Collection.Rebuild();
            }
        }

        private void ExecuteSelectNone() {
            SetEnabledStateForAllDirectories(false);
        }

        private void ExecuteSelectAll() {
            SetEnabledStateForAllDirectories(true);
        }

        private void SetEnabledStateForAllDirectories(bool enabled) {
            if (Collection != null) {
                foreach (var directory in Collection.Items) {
                    directory.IsEnabled = enabled;
                }
            }
        }
    }
}