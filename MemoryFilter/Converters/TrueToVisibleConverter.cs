using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MemoryFilter.Converters {

    public class TrueToVisibleConverter : IValueConverter {
        public static TrueToVisibleConverter Instance = new TrueToVisibleConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool && (bool) value) {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Visibility && (Visibility) value == Visibility.Visible) {
                return true;
            }

            return false;
        }
    }

}