using System.Windows;
using System.Windows.Controls;

namespace MemoryFilter {

    public class ExtendedTreeView : TreeView {
        public static readonly DependencyProperty CurrentProperty = DependencyProperty.Register("Current",
            typeof(object), typeof(ExtendedTreeView), new UIPropertyMetadata(null));

        public ExtendedTreeView() {
            SelectedItemChanged += ___ICH;
        }

        public object Current {
            get => GetValue(CurrentProperty);
            set => SetValue(CurrentProperty, value);
        }

        private void ___ICH(object sender, RoutedPropertyChangedEventArgs<object> e) {
            if (SelectedItem != null) {
                SetValue(CurrentProperty, SelectedItem);
            }
        }
    }

}