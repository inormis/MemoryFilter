using Autofac;
using MemoryFilter.Domain;
using MemoryFilter.Importer;
using MemoryFilter.Importer.Services;
using MemoryFilter.Importer.Services.Notifications;
using MemoryFilter.Settings;
using MemoryFilter.ViewModel;

namespace MemoryFilter {

    public class AutofacModule : Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterType<MainViewModel>().SingleInstance().AsSelf();
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().SingleInstance();
            builder.RegisterSingleInstance<MediaService>();
            builder.RegisterSingleInstance<FileService>();
            builder.RegisterSingleInstance<ExtensionsProvider>();
            builder.RegisterSingleInstance<MediaFileManager>();
            builder.RegisterSingleInstance<MediaCollection>();
            builder.RegisterType<FilterTask>().As<IFilterTask>().InstancePerDependency().ExternallyOwned();
            builder.RegisterType<ProgressNotifier>().InstancePerDependency().AsSelf();
            builder.RegisterType<ExifDataService>().As<IExifDataService>();
            builder.RegisterType<MediaFile>().As<IMediaFile>().ExternallyOwned().InstancePerDependency();
            builder.RegisterType<ProgressNotifier>().As<IProgressNotifier>().ExternallyOwned().InstancePerDependency();
            builder.Register(context => context.Resolve<IConfigurationService>().Configuration).As<IConfiguration>();
        }
    }

    public static class AutofacExtensions {
        public static ContainerBuilder
            RegisterSingleInstance<T>(this ContainerBuilder builder) {
            builder.RegisterType<T>().AsImplementedInterfaces().SingleInstance();

            return builder;
        }
    }

}