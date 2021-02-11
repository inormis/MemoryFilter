using System;
using MemoryFilter.Domain;

namespace MemoryFilter {

    public interface IAsyncResult {
        bool IsCancelled { get; }
        void Cancel();
    }

    public interface IAsyncResult<out T> : IAsyncResult {
        T Value { get; }
    }

    public interface IExecutionResult : IAsyncResult {
        decimal TotalSize { get; }
        decimal CompletedSizeInMb { get; }
        decimal Progress { get; }
        string PercentageString { get; }
        void MarkAsCompleted(IMediaFile mediaFile);
        event Action ProgressChanged;
    }

    public interface IExecutionResult<out T> : IAsyncResult {
        T Value { get; }
    }

}