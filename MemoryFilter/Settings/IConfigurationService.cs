namespace MemoryFilter.Settings {

    public interface IConfigurationService {
        IConfiguration Configuration { get; }
        void Save();
    }

}