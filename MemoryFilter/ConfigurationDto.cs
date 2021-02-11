using MemoryFilter.Settings;

namespace MemoryFilter {

    public class ConfigurationDto {
        public ConfigurationDto() {
            HistoryTargetPaths = new string[0];
            HistorySourcePaths = new string[0];
            HistoryPathFormats = new[] {
                @"yy\yy mm dd",
                @"yy\yy mm\yy mm dd",
                @"yy mm dd"
            };
            HistoryFileNamePrevixes = new string[0];
        }

        public string[] HistoryTargetPaths { get; set; }

        public string[] HistorySourcePaths { get; set; }

        public string[] HistoryPathFormats { get; set; }

        public string[] HistoryFileNamePrevixes { get; set; }

        public OverrideStrategy OverrideStrategy { get; set; } = OverrideStrategy.Skip;
    }

}