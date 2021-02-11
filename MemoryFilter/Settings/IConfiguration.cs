using System;
using System.Collections.Generic;
using MemoryFilter.Domain;

namespace MemoryFilter.Settings {

    public interface IConfiguration {
        IEnumerable<string> HistoryTargetPaths { get; }
        IEnumerable<string> HistorySourcePaths { get; }
        IEnumerable<string> HistoryPathFormats { get; }
        IEnumerable<string> HistoryFileNamePrefixes { get; }
        string DateFolderFormat { get; set; }
        bool Images { get; set; }
        bool Videos { get; set; }
        string FileNamePrefix { get; set; }
        string SourceDirectory { get; set; }
        string TargetDirectory { get; set; }
        string PathFormat { get; set; }
        string SamplePath { get; }
        OverrideStrategy OverrideStrategy { get; set; }
        ConfigurationDto Export();
        void Apply();
        bool IsSupported(IMediaFile mediaFile);
        string CreatePath(IMediaFile mediaFile, string targetDirectory);
        event Action Changed;

        PathNode[] GetPathNodes(IMediaFile mediaFile);
        string GetNewRelativePath(IMediaFile mediaFile);

        event Action PathFormatChanged;
    }

}