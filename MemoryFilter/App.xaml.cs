using System;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using MemoryFilter.Services;
using MemoryFilter.Settings;
using MemoryFilter.ViewModel;
using NLog;
using MessageBox = System.Windows.Forms.MessageBox;

namespace MemoryFilter {

    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public App() {
            SetupExceptionHandling();
        }

        private IContainer container;

        private void SetupExceptionHandling() {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception) e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");

            TaskScheduler.UnobservedTaskException += (s, e) =>
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
        }

        private void LogUnhandledException(Exception exception, string source) {
            try {
                exception.CaptureException();
            }
            catch (Exception ex) {
                Logger.Error(ex);
            }
        }

        protected override void OnStartup(StartupEventArgs e) {
            // base.OnStartup(e);

            var builder = new ContainerBuilder();

            builder.RegisterModule<AutofacModule>();

            container = builder.Build();
            try {

                var mainWindow = new MainWindow {DataContext = container.Resolve<MainViewModel>()};

                mainWindow.ShowDialog();
            }
            catch (Exception exception) {
                exception.CaptureException();
                MessageBox.Show(@"Something unexpected happened, we have notified the author.");
                System.Windows.Forms.Application.Exit();
            }
        }

        protected override void OnExit(ExitEventArgs e) {
            container.Resolve<IConfigurationService>().Save();
            base.OnExit(e);
        }
    }

}