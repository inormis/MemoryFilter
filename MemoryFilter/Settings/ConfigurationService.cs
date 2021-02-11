using System;
using System.IO;
using MemoryFilter.Importer.Services;
using Newtonsoft.Json;

namespace MemoryFilter.Settings {

    public class ConfigurationService : IConfigurationService {
        private readonly string _filePath;
        private readonly IFileService _fileService;

        public ConfigurationService(IFileService fileService) {
            _fileService = fileService;
            _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MemoryFilter", "settings.json");

            Load();
            Configuration.Changed += OnConfigurationChanged;
        }

        public IConfiguration Configuration { get; private set; }

        public void Save() {
            var dto = Configuration.Export();
            _fileService.WriteText(JsonConvert.SerializeObject(dto, Formatting.Indented), _filePath);
        }

        private void OnConfigurationChanged() {
            Save();
        }

        private void Load() {
            if (!_fileService.FileExists(_filePath)) {
                Configuration = new Configuration(_fileService);
                return;
            }

            try {
                var dto = JsonConvert.DeserializeObject<ConfigurationDto>(_fileService.ReadAllText(_filePath));
                Configuration = new Configuration(dto, _fileService);
            }
            catch (Exception) {
                Configuration = new Configuration(_fileService);
            }
        }
    }

}