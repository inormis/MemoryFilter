namespace MemoryFilter.Importer.Services {

    public interface IExtensionsProvider {
        bool IsKnownExtension(string extension);
        bool IsMediaFile(string file);
        MediaType GetMediaType(string file);
    }

}