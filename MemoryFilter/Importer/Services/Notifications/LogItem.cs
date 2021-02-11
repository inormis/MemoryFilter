namespace MemoryFilter.Importer.Services.Notifications {

    public class LogItem {
        public LogItem(string message, string filePath, LogLevel logLevel) {
            Message = message;
            FilePath = filePath;
            LogLevel = logLevel;
        }

        public string Message { get; }

        public string FilePath { get; }

        public LogLevel LogLevel { get; }
    }

}