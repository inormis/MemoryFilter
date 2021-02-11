using System.Linq;

namespace MemoryFilter.Settings {

    public class ConfigurationDto {
        public ConfigurationDto() {
            HistoryTargetPaths = new string[0];
            HistorySourcePaths = new string[0];
            HistoryPathFormats = new string[0];
            HistoryFileNamePrevixes = new string[0];
        }

        public ConfigurationDto(Configuration configuration) {
            HistoryTargetPaths = configuration.HistoryTargetPaths.ToArray();
            HistorySourcePaths = configuration.HistorySourcePaths.ToArray();
            HistoryPathFormats = configuration.HistoryPathFormats.ToArray();
            HistoryFileNamePrevixes = configuration.HistoryPathFormats.ToArray();
        }

        public string[] HistoryTargetPaths { get; set; }

        public string[] HistorySourcePaths { get; set; }

        public string[] HistoryPathFormats { get; set; }

        public string[] HistoryFileNamePrevixes { get; set; }
    }

}