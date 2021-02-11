using System.Windows;
using System.Windows.Controls;
using MemoryFilter.Importer.Services.Notifications;

namespace MemoryFilter.Controls {

    public class ProgressNotifierControl : ContentControl{
        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
            "Progress", typeof(IProgressNotifier), typeof(ProgressNotifierControl), new PropertyMetadata(default(IProgressNotifier)));

        public IProgressNotifier Progress {
            get => (IProgressNotifier) GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }
    }

}