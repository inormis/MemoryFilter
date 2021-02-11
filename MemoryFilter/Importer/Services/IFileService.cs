using MemoryFilter.Domain;

namespace MemoryFilter.Importer.Services {

    public interface IFileService {
        string ReadAllText(string filePath);
        bool FileExists(string filePath);
        void WriteText(string text, string filePath);
        void Move(IMediaFile mediaFile, string newPath);
        void EnsureDirectoryExists(string targetRootFolder);
        bool DirectoryExists(string targetDirectory);
        void ShowInExplorer(string filePath);
        void Open(string filePath);
        decimal GetSizeInMb(string file);
        void Move(string sourceFilePath, string targetPath);
        bool IsValidDirectory(string directory);
    }

}